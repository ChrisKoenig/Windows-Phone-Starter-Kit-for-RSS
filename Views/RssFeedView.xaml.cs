using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RssStarterKit.Localization;
using RssStarterKit.ViewModels;

namespace RssStarterKit.Views
{
    public partial class RssFeedView : PhoneApplicationPage
    {
        public RssFeedView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.FeedView_AppBar_Button_Refresh;
            };
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            var model = DataContext as MainViewModel;
            model.RefreshSelectedFeed();
        }
    }
}