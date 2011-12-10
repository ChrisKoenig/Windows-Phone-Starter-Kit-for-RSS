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
            AboutBoxDisappear.Completed += (x, y) =>
            {
                AboutBox.IsOpen = false;
            };
            Loaded += (s, e) =>
            {
                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = AppResources.MainView_AppBar_Menu_About;
            };
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox.IsOpen = true;
            AboutBoxAppear.Begin();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (AboutBox.IsOpen)
            {
                AboutBoxDisappear.Begin();
                e.Cancel = true;
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }

        private void AboutBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (AboutBox.IsOpen)
                AboutBoxDisappear.Begin();
        }

        private void ResetMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}