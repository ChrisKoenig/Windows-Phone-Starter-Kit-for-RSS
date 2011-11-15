using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RssStarterKit.Services;

namespace RssStarterKit.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                SimpleIoc.Default.Register<IRssDataService, MockRssDataService>();
            }
            else
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<IRssDataService, RssDataService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        internal void LoadState()
        {
            ServiceLocator.Current.GetInstance<MainViewModel>().LoadState();
        }

        internal void SaveState()
        {
            ServiceLocator.Current.GetInstance<MainViewModel>().SaveState();
        }
    }
}