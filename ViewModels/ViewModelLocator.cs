using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

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
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
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