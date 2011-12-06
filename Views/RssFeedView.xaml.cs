using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Controls;
using RssStarterKit.ViewModels;

namespace RssStarterKit.Views
{
    public partial class RssFeedView : PhoneApplicationPage
    {
        public RssFeedView()
        {
            InitializeComponent();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            var model = DataContext as MainViewModel;
            model.RefreshSelectedFeed();
        }
    }
}