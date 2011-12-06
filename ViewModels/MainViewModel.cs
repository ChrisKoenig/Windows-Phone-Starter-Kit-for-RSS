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
        private bool _IsBusy;
        private RssFeed _SelectedFeed;
        private string _Title;
        private ObservableCollection<RssFeed> _Feeds;
        Settings settings;

        public MainViewModel()
        {
            if (!IsInDesignMode)
            {
                // try to load from isolated storage.
                // if there is nothing in Isolated Storage,
                // then get from the supplied file.
                if (!LoadState())
                    LoadSettingsFile();
                Feeds = new ObservableCollection<RssFeed>(settings.RssFeeds);
                Title = settings.Title;
            }
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
                RaisePropertyChanged(() => SelectedFeed);
                LoadSelectedFeed();
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

        private void LoadSettingsFile()
        {
            var uri = new Uri("Settings.json", UriKind.Relative);
            var sri = Application.GetResourceStream(uri);
            using (var reader = new StreamReader(sri.Stream))
            {
                var json = reader.ReadToEnd();
                settings = JsonConvert.DeserializeObject<Settings>(json);
            }
        }

        public bool LoadState()
        {
            IsBusy = true;
            var json = IsoHelper.LoadIsoString(ISO_STORE_FILE);
            if (json == null)
                return false;
            settings = JsonConvert.DeserializeObject<Settings>(json);
            IsBusy = false;
            return true;
        }

        public void SaveState()
        {
            IsBusy = true;
            var json = JsonConvert.SerializeObject(settings);
            IsoHelper.SaveIsoString(ISO_STORE_FILE, json);
            IsBusy = false;
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
                });

                // cache back to IsolatedStorage
                SaveState();
            }, null);
        }

        private DateTime ParseRssDateTime(string s)
        {
            // date comes in like this: Tue, 06 Dec 2011 20:01:47 GMT
            // date also comes in like this: Mon, 05 Dec 2011 13:52:05 +0000
            DateTime date;
            if (DateTime.TryParse(s, out date))
                return date;
            else
                return DateTime.Now;
        }

        #endregion Methods
    }
}