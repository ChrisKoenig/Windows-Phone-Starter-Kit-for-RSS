using System;

namespace RssStarterKit.Localization
{
    public class Localization
    {
        private readonly static AppResources localizedresources = new AppResources();

        public AppResources LocalizedResources
        {
            get
            {
                return localizedresources;
            }
        }
    }
}