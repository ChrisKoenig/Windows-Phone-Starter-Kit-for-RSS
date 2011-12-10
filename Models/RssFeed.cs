using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;

namespace RssStarterKit.Models
{
    public class RssFeed : ObservableObject
    {
        // Fields...
        private string _FeedType;
        private string _SubTitle;
        private string _Title;
        private string _RssUrl;
        private DateTime? _RefreshTimeStamp;
        private DateTime? _LastBuildDate;
        private string _Link;
        private string _Description;
        private Uri _ImageUri;
        private ObservableCollection<RssItem> _Items;

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

        public string SubTitle
        {
            get { return _SubTitle; }
            set
            {
                if (_SubTitle == value)
                    return;
                _SubTitle = value;
                RaisePropertyChanged(() => SubTitle);
            }
        }

        public string FeedType
        {
            get { return _FeedType; }
            set
            {
                if (_FeedType == value)
                    return;
                _FeedType = value;
                RaisePropertyChanged(() => FeedType);
            }
        }

        public string RssUrl
        {
            get { return _RssUrl; }
            set
            {
                if (_RssUrl == value)
                    return;
                _RssUrl = value;
                RaisePropertyChanged(() => RssUrl);
            }
        }

        public DateTime? RefreshTimeStamp
        {
            get { return _RefreshTimeStamp; }
            set
            {
                if (_RefreshTimeStamp == value)
                    return;
                _RefreshTimeStamp = value;
                RaisePropertyChanged(() => RefreshTimeStamp);
            }
        }

        public DateTime? LastBuildDate
        {
            get { return _LastBuildDate; }
            set
            {
                if (_LastBuildDate == value)
                    return;
                _LastBuildDate = value;
                RaisePropertyChanged(() => LastBuildDate);
            }
        }

        public string Link
        {
            get { return _Link; }
            set
            {
                if (_Link == value)
                    return;
                _Link = value;
                RaisePropertyChanged(() => Link);
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value)
                    return;
                _Description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        public Uri ImageUri
        {
            get { return _ImageUri; }
            set
            {
                if (_ImageUri == value)
                    return;
                _ImageUri = value;
                RaisePropertyChanged(() => ImageUri);
            }
        }

        public ObservableCollection<RssItem> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                RaisePropertyChanged(() => Items);
            }
        }
    }
}