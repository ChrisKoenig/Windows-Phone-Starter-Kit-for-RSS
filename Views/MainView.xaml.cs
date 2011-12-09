using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RssStarterKit.Localization;

namespace RssStarterKit.Views
{
    public partial class MainView : PhoneApplicationPage
    {
        public MainView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = AppResources.MainView_AppBar_Menu_About;
            };
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox.IsOpen = true;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (AboutBox.IsOpen)
            {
                AboutBox.IsOpen = false;
                e.Cancel = true;
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }
    }
}