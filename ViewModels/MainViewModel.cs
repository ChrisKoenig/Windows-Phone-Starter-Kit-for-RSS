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
        private const string ISO_STORE_FILE = "settings.json";
        private bool _IsBusy;
        private RssFeed _SelectedFeed;
        private string _Title;
        private ObservableCollection<RssFeed> _Feeds;
        Settings settings;

        public MainViewModel()
        {
            // try to load from isolated storage. if there is nothing in Isolated Storage, then get from scratch
            if (!LoadState())
                LoadSettingsFile();
            _Feeds = new ObservableCollection<RssFeed>(settings.RssFeeds);
            _Title = settings.Title;
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

        #endregion Methods
    }
}