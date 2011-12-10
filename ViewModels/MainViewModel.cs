using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using RssStarterKit.Configuration;
using RssStarterKit.Localization;
using RssStarterKit.Models;
using RssStarterKit.Services;

namespace RssStarterKit.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private RssItem _SelectedItem;
        private const string ISO_STORE_FILE = "settings.json";
        private bool _IsBusy = false;
        private RssFeed _SelectedFeed;
        private string _Title;
        private ObservableCollection<RssFeed> _Feeds;
        Settings settings;

        public MainViewModel()
        {
        }

        #region Properties

        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                if (_IsBusy == value)
                    return;
                _IsBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value)
                    return;
                _Title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public RssFeed SelectedFeed
        {
            get { return _SelectedFeed; }
            set
            {
                if (_SelectedFeed == value)
                    return;
                _SelectedFeed = value;
                LoadSelectedFeed();
                RaisePropertyChanged(() => SelectedFeed);
            }
        }

        public RssItem SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        public ObservableCollection<RssFeed> Feeds
        {
            get { return _Feeds; }
            set
            {
                if (_Feeds == value)
                    return;
                _Feeds = value;
                RaisePropertyChanged(() => Feeds);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// load the settings and all the feed data from isolated storage (json format)
        /// </summary>
        public void LoadState()
        {
            // don't reload the state if we already have state loaded
            if (settings != null)
                return;

            IsBusy = true;
            string data = IsoHelper.LoadIsoString(ISO_STORE_FILE);
            if (data == null)
            {
                var uri = new Uri("Settings.json", UriKind.Relative);
                var sri = Application.GetResourceStream(uri);
                using (var reader = new StreamReader(sri.Stream))
                    data = reader.ReadToEnd();
            }
            settings = JsonConvert.DeserializeObject<Settings>(data);
            Feeds = new ObservableCollection<RssFeed>(settings.RssFeeds);
            Title = settings.Title;
            IsBusy = false;
        }

        /// <summary>
        /// save the settings and all the feed data to isolated storage (json format)
        /// </summary>
        public void SaveState()
        {
            System.Threading.ThreadPool.QueueUserWorkItem((cb) =>
            {
                var json = JsonConvert.SerializeObject(settings);
                IsoHelper.SaveIsoString(ISO_STORE_FILE, json);
            });
        }

        /// <summary>
        /// assemble an html string for the web browser to display the contents of the selected feed item
        /// </summary>
        /// <returns></returns>
        public string BuildHtmlForSelectedItem()
        {
            string html;

            // retrieve the HTML from the included file
            using (var stream = Application.GetResourceStream(new Uri("Resources/preview.html", UriKind.Relative)).Stream)
            using (var reader = new StreamReader(stream))
                html = reader.ReadToEnd();

            // replace bits of the HTML via tokens with data from our settings and selected feed item
            var content = SelectedItem.Description;
            if (content.Trim().Length == 0)
                content = AppResources.ItemView_FeedItemHasNoContent;
            html = html.Replace("{{body.foreground}}", settings.Theme.BodyForeground);
            html = html.Replace("{{body.background}}", settings.Theme.BodyBackground);
            html = html.Replace("{{head.title}}", SelectedItem.Title);
            html = html.Replace("{{body.content}}", content);

            return html;
        }

        /// <summary>
        /// refresh the selected feed from the internet
        /// </summary>
        private void LoadSelectedFeed()
        {
            if (SelectedFeed.RefreshTimeStamp.HasValue &&
                SelectedFeed.RefreshTimeStamp.Value.AddMinutes(settings.RefreshIntervalInMinutes) < DateTime.Now)
            {
                // cached feed is OK to show
            }
            else
            {
                // refresh feed from database
                RefreshSelectedFeed();
            }
        }

        private static string NS_ATOM = "http://www.w3.org/2005/Atom";

        public void RefreshSelectedFeed()
        {
            // lock the UI
            IsBusy = true;

            // retrieve the feed and it's items from the internet
            var request = HttpWebRequest.CreateHttp(SelectedFeed.RssUrl) as HttpWebRequest;
            request.BeginGetResponse((token) =>
            {
                SyndicationFeed feed;

                // process the response
                using (var response = request.EndGetResponse(token) as HttpWebResponse)
                using (var stream = response.GetResponseStream())
                using (var reader = XmlReader.Create(stream))
                {
                    feed = SyndicationFeed.Load(reader);
                    var items = feed.Items.Select(item => new RssItem()
                    {
                        Title = item.Title.Text,
                        Link = item.Links.FirstOrDefault().Uri.AbsoluteUri,
                        Description = item.Summary.Text,
                        PublishDate = item.PublishDate.LocalDateTime,
                    });

                    // use the dispatcher thread to update properties of the SelectedFeed since it's bound to the UI
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        // these are simple mappings from the feed to the view object
                        SelectedFeed.SubTitle = feed.Title.Text;
                        SelectedFeed.Link = feed.Links.FirstOrDefault().Uri.AbsoluteUri;
                        SelectedFeed.Description = feed.Description.Text;
                        SelectedFeed.Items = new ObservableCollection<RssItem>(items);
                        SelectedFeed.LastBuildDate = feed.LastUpdatedTime.LocalDateTime;
                        SelectedFeed.RefreshTimeStamp = DateTime.Now;

                        // choose the best image for the ImageUrl
                        var icon = feed.ElementExtensions.ReadElementExtensions<string>("icon", NS_ATOM).FirstOrDefault();
                        if (icon != null && IsValidFileExtension(icon))
                        {
                            SelectedFeed.ImageUri = new Uri(icon, UriKind.Absolute);
                        }
                        else
                        {
                            if (feed.ImageUrl != null && IsValidFileExtension(feed.ImageUrl.AbsoluteUri))
                            {
                                SelectedFeed.ImageUri = feed.ImageUrl;
                            }
                            else
                            {
                                SelectedFeed.ImageUri = new Uri("/Images/RssFeed.png", UriKind.Relative);
                            }
                        }

                        // unlock the UI
                        IsBusy = false;
                    });
                    // cache back to IsolatedStorage
                    SaveState();
                }
            }, null);
        }

        /// <summary>
        /// only jpg and png images are supposed in windows phone
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsValidFileExtension(string path)
        {
            return path.EndsWith("png") || path.EndsWith("jpg");
        }

        /// <summary>
        /// share the current feed via the windows phone social media task
        /// </summary>
        public void ShareCurrentFeedItem()
        {
            const string format = "{0} (via {1})";
            var task = new ShareLinkTask()
            {
                LinkUri = new Uri(SelectedItem.Link),
                Title = SelectedItem.Title,
                Message = String.Format(format, SelectedItem.Title, settings.Title)
            };

            task.Show();
        }

        #endregion Methods
    }
}