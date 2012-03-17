using System;
using System.Collections.Generic;
using System.Linq;
using Coding4Fun.Phone.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RssStarterKit.Localization;
using RssStarterKit.Messages;
using RssStarterKit.ViewModels;

namespace RssStarterKit.Views
{
    public partial class MainView : PhoneApplicationPage
    {
        private readonly App app = (App)App.Current;

        public MainView()
        {
            InitializeComponent();
            Messenger.Default.Register<NetworkUnavailableMessage>(this, (message) => ShowNetworkUnavailable());
            Loaded += (s, e) =>
            {
                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = AppResources.MainView_AppBar_Menu_About;
                if (!app.IsNetworkAvailable && !app.NetworkMessageShown)
                {
                    app.NetworkMessageShown = true;
                    ShowNetworkUnavailable();
                }
            };
        }

        private void ShowNetworkUnavailable()
        {
            var prompt = new MessagePrompt()
            {
                Message = AppResources.NetworkNotAvailableMessage,
            };
            prompt.Show();
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            var about = new AboutPrompt()
            {
                Title = Localization.AppResources.MainView_About_Title,
                Body = Localization.AppResources.MainView_About_Body,
            };
            about.Show();
        }

        private void ResetMenuItem_Click(object sender, EventArgs e)
        {
            var model = DataContext as MainViewModel;
            model.ResetFeeds();
        }
    }
}