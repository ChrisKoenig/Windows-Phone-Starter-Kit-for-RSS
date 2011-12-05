using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace RssStarterKit.Models
{
    public class RssFeed : ObservableObject
    {
        // Fields...
        private string _Title;
        private string _RssUrl;
        private DateTime? _RefreshTimeStamp;
        private string _Link;
        private string _Description;
        private string _ImageUrl;
        private List<RssItem> _Items;

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

        public string Link
        {
            get { return _Link; }
            set
            {
                if (_Link == value)
                    return;
                _Link = value;
                RaisePropertyChanged(() => this.Link);
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
                RaisePropertyChanged(() => this.Description);
            }
        }

        public string ImageUrl
        {
            get { return _ImageUrl; }
            set
            {
                if (_ImageUrl == value)
                    return;
                _ImageUrl = value;
                RaisePropertyChanged(() => this.ImageUrl);
            }
        }

        public List<RssItem> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                RaisePropertyChanged(() => this.ImageUrl);
            }
        }
    }
}