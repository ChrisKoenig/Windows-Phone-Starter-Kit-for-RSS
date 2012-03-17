using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace RssStarterKit.Models
{
    public class RssItem : ObservableObject
    {
        // Fields...
        private string _Guid;
        private string _Link;
        private string _Description;
        private DateTime? _PublishDate;
        private string _Title;

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

        public DateTime? PublishDate
        {
            get { return _PublishDate; }
            set
            {
                if (_PublishDate == value)
                    return;
                _PublishDate = value;
                RaisePropertyChanged(() => PublishDate);
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

        public string Guid
        {
            get { return _Guid; }
            set
            {
                if (_Guid == value)
                    return;
                _Guid = value;
                RaisePropertyChanged(() => Guid);
            }
        }
    }
}