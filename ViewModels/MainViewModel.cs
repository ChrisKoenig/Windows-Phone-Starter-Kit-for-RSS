using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Tasks;
using RssStarterKit.Configuration;
using RssStarterKit.Helpers;
using RssStarterKit.Localization;
using RssStarterKit.Messages;
using RssStarterKit.Models;

namespace RssStarterKit.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly static XNamespace NS_MEDIA = "http://search.yahoo.com/mrss/";
        private readonly static XNamespace NS_ITUNES = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        private readonly static string NS_ATOM = "http://www.w3.org/2005/Atom";
        private RssItem _SelectedItem;
        private const string ISO_STORE_FILE = "settings.xml";
        private bool _IsBusy;
        private bool _PreviewEnabled;
        private RssFeed _SelectedFeed;
        private string _Title;
        private ObservableCollection<RssFeed> _Feeds;
        Settings settings;

        #region Properties

        public Visibility NetworkErrorVisibility
        {
            get
            {
                var app = (App)Application.Current;
                return app.IsNetworkAvailable ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool PreviewEnabled
        {
            get
            {
                return _PreviewEnabled;
            }
            set
            {
                if (_PreviewEnabled == value)
                    return;
                _PreviewEnabled = value;
                RaisePropertyChanged(() => PreviewEnabled);
            }
        }

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

        public Settings Settings
        {
            get
            {
                return settings;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// load the settings and all the feed data from isolated storage
        /// </summary>
        public void LoadState(bool forceRefresh = false)
        {
            IsBusy = true;

            if (forceRefresh)
            {
                settings = GetSettingsFromConfigFile();
                InitializeProperties();
            }
            else
            {
                var isoData = IsoHelper.LoadIsoString(ISO_STORE_FILE);
                if (isoData == null || isoData.Length == 0)
                {
                    settings = GetSettingsFromConfigFile();
                    InitializeProperties();
                }
                else
                {
                    var configSettings = GetSettingsFromConfigFile();
                    var isoSettings = DeserializeSettings(isoData);
                    if (isoSettings.Version < configSettings.Version)
                    {
                        settings = configSettings;
                        InitializeProperties();
                    }
                    else
                    {
                        settings = isoSettings;
                        InitializeProperties();
                    }
                }
            }

            IsBusy = false;
        }

        private void InitializeProperties()
        {
            Feeds = new ObservableCollection<RssFeed>(settings.RssFeeds);
            PreviewEnabled = settings.PreviewEnabled;
            Title = settings.Title;
            if (settings.SelectedFeed != null)
                SelectedFeed = settings.SelectedFeed;
            if (settings.SelectedItem != null)
                SelectedItem = settings.SelectedItem;
            SaveState();
        }

        private Settings DeserializeSettings(string data)
        {
            Settings settings;
            try
            {
                var ser = new DataContractSerializer(typeof(Settings));
                using (var sr = new StringReader(data))
                using (var xr = XmlReader.Create(sr))
                    settings = (Settings)ser.ReadObject(xr);
            }
            catch
            {
                settings = new Settings() { Version = -1 };
            }
            return settings;
        }

        private string SerializeSettings(Settings settings)
        {
            string data;
            var ser = new DataContractSerializer(typeof(Settings));
            using (var stream = new MemoryStream())
            {
                ser.WriteObject(stream, settings);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    data = reader.ReadToEnd();
                }
            }
            return data;
        }

        private Settings GetSettingsFromConfigFile()
        {
            string data;
            var uri = new Uri("Settings/" + ISO_STORE_FILE, UriKind.Relative);
            var sri = Application.GetResourceStream(uri);
            using (var reader = new StreamReader(sri.Stream))
                data = reader.ReadToEnd();
            return DeserializeSettings(data);
        }

        /// <summary>
        /// save the settings and all the feed data to isolated storage
        /// </summary>
        public void SaveState()
        {
            settings.SelectedFeed = SelectedFeed;
            settings.SelectedItem = SelectedItem;
            string data = SerializeSettings(settings);
            IsoHelper.SaveIsoString(ISO_STORE_FILE, data);
        }

        /// <summary>
        /// assemble an html string for the web browser to display the contents of the selected feed item
        /// </summary>
        /// <returns></returns>
        public string BuildHtmlForSelectedItem()
        {
            string html;

            // is your preview window not showing up? Make sure the PreviewEnabled setting
            // is set to "true" in the Settings.xml configuration file.

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
            if (SelectedFeed == null)
                return;

            var compDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(settings.RefreshIntervalInMinutes));

            if (SelectedFeed.RefreshTimeStamp.HasValue &&
                DateTime.Compare(SelectedFeed.RefreshTimeStamp.Value, compDate) > 0)
            {
                // cached feed is OK to show
            }
            else
            {
                // refresh feed from network
                var app = (App)App.Current;
                if (!app.IsNetworkAvailable)
                {
                    MessengerInstance.Send<NetworkUnavailableMessage>(new NetworkUnavailableMessage());
                    return;
                }

                RefreshSelectedFeed();
            }
        }

        /// <summary>
        /// This is the workhorse of the MainViewModel, responsible for retrieving the data from the RSS
        /// feed and processing it for viewing
        /// </summary>
        public void RefreshSelectedFeed()
        {
            var app = (App)App.Current;
            if (!app.IsNetworkAvailable)
                return;

            // lock the UI
            IsBusy = true;

            // retrieve the feed and it's items from the internet
            var request = HttpWebRequest.CreateHttp(SelectedFeed.RssUrl) as HttpWebRequest;
            request.BeginGetResponse((token) =>
            {
                // process the response
                using (var response = request.EndGetResponse(token) as HttpWebResponse)
                using (var stream = response.GetResponseStream())
                using (var reader = XmlReader.Create(stream))
                {
                    var feed = GetFeedDataFromReader(reader);
                    feed.RssUrl = request.RequestUri.AbsoluteUri;

                    // use the dispatcher thread to update properties of the SelectedFeed since it's bound to the UI
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        // man, this is ugly
                        SelectedFeed.Description = feed.Description.OrNoneProvided();
                        SelectedFeed.FeedType = feed.FeedType;
                        SelectedFeed.ImageUri = feed.ImageUri;
                        SelectedFeed.Items = feed.Items;
                        SelectedFeed.LastBuildDate = feed.LastBuildDate;
                        SelectedFeed.Link = feed.Link;
                        SelectedFeed.RefreshTimeStamp = DateTime.Now;
                        SelectedFeed.RssUrl = feed.RssUrl;

                        // unlock the UI
                        IsBusy = false;
                    });

                    // cache back to IsolatedStorage
                    SaveState();
                }
            }, null);
        }

        private RssFeed GetFeedDataFromReader(XmlReader reader)
        {
            var doc = XDocument.Load(reader);

            var feed = new RssFeed();

            var defaultNamespace = doc.Root.GetDefaultNamespace();
            if (defaultNamespace.NamespaceName.EndsWith("Atom"))
                LoadAtomFeed(doc, feed);
            else
                LoadRssFeed(doc, feed);

            return feed;
        }

        private void LoadAtomFeed(XDocument doc, RssFeed feed)
        {
            SetMediaImage(feed, doc);
            feed.Description = doc.Root.GetSafeElementString("summary");
            feed.Items = new ObservableCollection<RssItem>();
            feed.LastBuildDate = doc.Root.GetSafeElementDate("updated");
            feed.Link = doc.Root.GetLink("self");

            foreach (var item in doc.Root.Elements(doc.Root.GetDefaultNamespace() + "entry"))
            {
                var newItem = new RssItem()
                {
                    Title = item.GetSafeElementString("title"),
                    Description = item.GetSafeElementString("description"),
                    PublishDate = item.GetSafeElementDate("published"),
                    Guid = item.GetSafeElementString("id"),
                    Link = item.GetLink("alternate"),
                };
                feed.Items.Add(newItem);
            }
        }

        private void LoadRssFeed(XDocument doc, RssFeed feed)
        {
            SetMediaImage(feed, doc);
            feed.Description = doc.Root.Element("channel").GetSafeElementString("description");
            feed.Items = new ObservableCollection<RssItem>();
            feed.Link = doc.Root.Element("channel").GetSafeElementString("link");

            foreach (var item in doc.Descendants("item"))
            {
                var newItem = new RssItem()
                {
                    Title = item.GetSafeElementString("title"),
                    Link = item.GetSafeElementString("link"),
                    Description = item.GetSafeElementString("description"),
                    PublishDate = item.GetSafeElementDate("pubDate"),
                    Guid = item.GetSafeElementString("guid"),
                };
                feed.Items.Add(newItem);
            }

            var lastItem = feed.Items.OrderByDescending(i => i.PublishDate).FirstOrDefault();
            if (lastItem != null)
            {
                feed.LastBuildDate = lastItem.PublishDate;
            }
        }

        private void SetMediaImage(RssFeed feed, XDocument doc)
        {
            var image = doc.Root.Descendants(NS_ITUNES + "image").FirstOrDefault();
            if (image != null)
            {
                feed.ImageUri = new Uri(image.Attribute("href").Value);
                return;
            }

            var thumbnail = doc.Element(NS_MEDIA + "thumbnail");
            if (thumbnail != null)
            {
                feed.ImageUri = new Uri(thumbnail.Value);
                return;
            }

            // if all else fails...
            feed.ImageUri = new Uri("/Images/FeedType/rss.jpg", UriKind.Relative);
        }

        /// <summary>
        /// only jpg and png images are supported in windows phone
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        //private bool IsValidFileExtension(string path)
        //{
        //    return path.EndsWith("png") || path.EndsWith("jpg");
        //}

        /// <summary>
        /// share the current feed via the windows phone social media task
        /// </summary>
        public void ShareCurrentFeedItem()
        {
            if (SelectedItem == null)
                return;
            const string format = "{0} (via {1})";
            var task = new ShareLinkTask()
            {
                LinkUri = new Uri(SelectedItem.Link),
                Title = SelectedItem.Title,
                Message = String.Format(format, SelectedItem.Title, settings.Title)
            };

            task.Show();
        }

        // do a "full reset" on all the retrieved feeds
        internal void ResetFeeds()
        {
            LoadState(forceRefresh: true);
        }

        #endregion Methods
    }
}