using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using RssStarterKit.ViewModels;

namespace RssStarterKit.Views
{
    public partial class RssItemView : PhoneApplicationPage
    {
        private MainViewModel _ViewModel;

        public RssItemView()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                _ViewModel = DataContext as MainViewModel;
            };

            FeedItemContentBrowser.Loaded += (sender, e) =>
            {
                var html = _ViewModel.BuildHtmlForSelectedItem();
                FeedItemContentBrowser.NavigateToString(html);
            };
        }

        private void VisitWebSiteButton_Click(object sender, EventArgs e)
        {
            var item = _ViewModel.SelectedItem;
            if (item == null)
                return;

            var task = new WebBrowserTask() { Uri = new Uri(item.Link, UriKind.Absolute) };
            task.Show();
        }
    }
}