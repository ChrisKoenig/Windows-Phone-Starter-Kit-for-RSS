using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
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

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var t = sender as TextBlock;
            var url = t.Text;
            var uri = new Uri(url, UriKind.Absolute);
            var task = new WebBrowserTask()
            {
                Uri = uri
            };
            task.Show();
        }
    }
}