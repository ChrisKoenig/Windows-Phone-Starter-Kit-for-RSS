using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using RssStarterKit.Configuration;
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

        public void SaveState()
        {
            System.Threading.ThreadPool.QueueUserWorkItem((cb) =>
            {
                var json = JsonConvert.SerializeObject(settings);
                IsoHelper.SaveIsoString(ISO_STORE_FILE, json);
            });
        }

        internal string BuildHtmlForSelectedItem()
        {
            var si = Application.GetResourceStream(new Uri("Resources/preview.html", UriKind.Relative));
            var reader = new StreamReader(si.Stream);
            var html = reader.ReadToEnd();
            html = html.Replace("{{body.foreground}}", settings.Theme.BodyForeground);
            html = html.Replace("{{body.background}}", settings.Theme.BodyBackground);
            html = html.Replace("{{head.title}}", SelectedItem.Title);
            html = html.Replace("{{body.content}}", SelectedItem.Description);
            reader.Dispose();
            si.Stream.Dispose();
            return html;
        }

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

        public void RefreshSelectedFeed()
        {
            IsBusy = true;
            var request = HttpWebRequest.CreateHttp(SelectedFeed.RssUrl) as HttpWebRequest;
            request.BeginGetResponse((token) =>
            {
                var response = request.EndGetResponse(token) as HttpWebResponse;
                var stream = response.GetResponseStream();
                var doc = XDocument.Load(stream);
                var channel = doc.Root.Element("channel");
                var items = from item in doc.Descendants("item")
                            select new RssItem()
                            {
                                Title = item.Element("title").Value,
                                Link = item.Element("link").Value,
                                Description = item.Element("description").Value,
                                PublishDate = ParseRssDateTime(item.Element("pubDate").Value),
                            };
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    SelectedFeed.Title = channel.Element("title").Value;
                    SelectedFeed.Link = channel.Element("link").Value;
                    SelectedFeed.Description = channel.Element("description").Value;
                    if (channel.Element("image") != null && channel.Element("image").Element("url") != null)
                        SelectedFeed.ImageUrl = channel.Element("image").Element("url").Value;
                    SelectedFeed.Items = new ObservableCollection<RssItem>(items);
                    SelectedFeed.LastBuildDate = ParseRssDateTime(channel.Element("lastBuildDate").Value);
                    SelectedFeed.RefreshTimeStamp = DateTime.Now;
                    IsBusy = false;
                });

                // cache back to IsolatedStorage
                SaveState();
            }, null);
        }

        private DateTime ParseRssDateTime(string s)
        {
            // date comes in like "Tue, 06 Dec 2011 20:01:47 GMT" or like "Mon, 05 Dec 2011 13:52:05 +0000"
            DateTime date;
            if (DateTime.TryParse(s, out date))
                return date;
            else
                return DateTime.Now;
        }

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