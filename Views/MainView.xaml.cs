using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Shell;
using RssStarterKit.Localization;
using RssStarterKit.Messages;
using RssStarterKit.ViewModels;
using Coding4Fun.Toolkit.Controls;

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
                ResetAdControl();
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
            ResetAdControl();
        }

        private void ResetAdControl()
        {
            try
            {
                // show ad control?
                var viewModel = this.DataContext as MainViewModel;
                var adUnitId = viewModel.Settings.AdInfo.AdUnitId;
                var applicationId = viewModel.Settings.AdInfo.ApplicationId;
                if (adUnitId != null && applicationId != null && adUnitId.Length > 0 && applicationId.Length > 0)
                {
                    AdControl ad = new AdControl()
                    {
                        ApplicationId = applicationId,
                        AdUnitId = adUnitId,
                        Height = 80,
                        Width = 480,
                        IsEnabled = true,
                        IsAutoRefreshEnabled = true,
                        IsAutoCollapseEnabled = true,
                    };
                    LayoutRoot.Children.Add(ad);
                    Grid.SetRow(ad, 2);
                }
            }
            catch
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    // Something is wrong with the ad control
                    // common causes of this problem include:
                    //      - did you forget to upgrade the version number on your updated deployment?
                    //      - did you set an invalid ApplicationId and/or AdUnitId in your settings.xml file?
                    System.Diagnostics.Debugger.Break();
                }
            }
        }
    }
}