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

namespace RssStarterKit.Views
{
    public partial class RssItemView : PhoneApplicationPage
    {
        public RssItemView()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                //._ViewModel = this.DataContext as MainViewModel;
            };

            FeedItemContentBrowser.Loaded += (sender, e) =>
            {
                //var html = _ViewModel.BuildHtmlForSelectedItem();
                //FeedItemContentBrowser.NavigateToString(html);
            };
        }

        private void VisitWebSiteButton_Click(object sender, EventArgs e)
        {
            //var item = _ViewModel.SelectedItem;
            //if (item == null)
            //    return;
            //var url = item.Link;

            //var task = new WebBrowserTask();
            //task.Uri = new Uri(url, UriKind.Absolute);
            //task.Show();
        }
    }
}