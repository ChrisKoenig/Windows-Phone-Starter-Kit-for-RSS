using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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
                _ViewModel = this.DataContext as MainViewModel;
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