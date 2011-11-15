using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using RssStarterKit.Models;

namespace RssStarterKit.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IRssService service;

        public MainViewModel()
        {
            // retrieve the IRssService
            service = ServiceLocator.Current.GetInstance<IRssService>();

            // if runtime, load the real data
            if (!IsInDesignMode)
            {
                LoadData();
            }
        }

        #region Properties

        // Fields...
        private ObservableCollection<RssItem> _Items;

        public ObservableCollection<RssItem> Items
        {
            get { return _Items; }
            set
            {
                if (_Items == value)
                    return;
                _Items = value;
                RaisePropertyChanged(() => this.Items);
            }
        }

        public ObservableCollection<RssItem> NewItems
        {
            get
            {
                if (Items == null)
                    return null;
                var query = Items.Where(item => item.IsRead == false);
                return new ObservableCollection<RssItem>(query);
            }
        }

        public ObservableCollection<RssItem> FavoriteItems
        {
            get
            {
                if (Items == null)
                    return null;
                var query = Items.Where(item => item.IsFavorite == true);
                return new ObservableCollection<RssItem>(query);
            }
        }

        #endregion Properties

        #region Methods

        private void LoadData()
        {
            // load the Favorite feeds
            // load the latest RSS feeds
            // combine together
            // merge with the stats on those already read
        }

        private void AddFavorite(RssItem item)
        {
        }

        private void RemoveFavorite(RssItem item)
        {
        }

        private List<RssItem> GetFavorites()
        {
            return null;
        }

        private List<RssItem> GetFromFeed()
        {
            return null;
        }

        #endregion Methods

        #region Tombstoning support

        internal void LoadState()
        {
            // save to isolated storage
        }

        internal void SaveState()
        {
            // load from isolated storage
        }

        #endregion Tombstoning support
    }
}