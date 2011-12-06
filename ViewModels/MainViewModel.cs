using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
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
            // try to load from isolated storage. if there is nothing in Isolated Storage, then get from scratch
            if (IsInDesignMode)
            {
                LoadDesignTimeSettings();
            }
            else
            {
                if (!LoadState())
                    LoadSettingsFile();
            }
            Feeds = new ObservableCollection<RssFeed>(settings.RssFeeds);
            Title = settings.Title;
        }

        private void LoadDesignTimeSettings()
        {
            settings = new Settings()
            {
                Title = "DesignTime Data",
                RefreshIntervalInMinutes = 5,
                Theme = new ThemeInfo()
                {
                    BodyBackground = "#000000",
                    BodyForeground = "#ffffff"
                },
                RssFeeds = new List<RssFeed>() {
                    new RssFeed() {
                        Title = "Chris Koenig",
                        RssUrl = "http://feeds.feedburner.com/chriskoenig",
                        Items = new List<RssItem>()
                        {
                            new RssItem()
                            {
                                Title = "First Post",
                                Description = "This is the description of the first post. Text only.",
                                Link = "http://chriskoenig.net?p=100",
                            },
                            new RssItem()
                            {
                                Title = "Second Post",
                                Description = "This is the description of the second post. Text only.",
                                Link = "http://chriskoenig.net?p=101",
                            },
                            new RssItem()
                            {
                                Title = "Third Post",
                                Description = "This is the description of the first post. We have <b>html</b> as well, but only <i>simple</i> html.",
                                Link = "http://chriskoenig.net?p=102",
                            },
                        },
                    },
                    new RssFeed()
                    {
                        Title = "GiveCamp",
                        RssUrl = "http://feeds.feedburner.com/givecamp"
                    }
                }
            };
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
                RaisePropertyChanged(() => this.Title);
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
                RaisePropertyChanged(() => this.SelectedFeed);
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
                RaisePropertyChanged(() => this.SelectedItem);
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
                RaisePropertyChanged(() => this.Feeds);
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
            html = html.Replace("body.foreground", settings.Theme.BodyForeground);
            html = html.Replace("body.background", settings.Theme.BodyBackground);
            html = html.Replace("head.title", SelectedItem.Title);
            html = html.Replace("body.content", SelectedItem.Description);
            reader.Dispose();
            si.Stream.Dispose();
            return html;
        }

        private void LoadSelectedFeed()
        {
            if (SelectedFeed.RefreshTimeStamp.HasValue &&
                SelectedFeed.RefreshTimeStamp.Value.AddDays(settings.RefreshIntervalInMinutes) > DateTime.Today)
            {
                // cached feed is OK to show
            }
            else
            {
                // refresh feed from database
                RefreshFeed(SelectedFeed);
            }
        }

        private void RefreshFeed(RssFeed SelectedFeed)
        {
            //
        }

        #endregion Methods
    }
}