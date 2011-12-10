using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using RssStarterKit.Localization;
using RssStarterKit.ViewModels;

namespace RssStarterKit.Views
{
    public partial class RssItemView : PhoneApplicationPage
    {
        public RssItemView()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.ItemView_AppBar_Button_Share;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.ItemView_AppBar_Button_Open;
            };

            FeedItemContentBrowser.Loaded += (sender, e) =>
            {
                var model = DataContext as MainViewModel;
                var html = model.BuildHtmlForSelectedItem();
                FeedItemContentBrowser.NavigateToString(html);
            };
        }

        private void VisitWebSiteButton_Click(object sender, EventArgs e)
        {
            var model = DataContext as MainViewModel;
            var item = model.SelectedItem;
            if (item == null)
                return;

            var task = new WebBrowserTask()
            {
                Uri = new Uri(item.Link, UriKind.Absolute)
            };
            task.Show();
        }

        private void ShareButton_Click(object sender, EventArgs e)
        {
            var model = DataContext as MainViewModel;
            model.ShareCurrentFeedItem();
        }
    }
}