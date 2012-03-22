using System;
using System.Collections.Generic;
using System.Linq;
using RssStarterKit.Models;

namespace RssStarterKit.Configuration
{
    public class ThemeInfo
    {
        public string BodyForeground { get; set; }

        public string BodyBackground { get; set; }
    }

    public class AdvertisingInfo
    {
        public string ApplicationId { get; set; }

        public string AdUnitId { get; set; }
    }

    public class Settings
    {
        public string Title { get; set; }

        public int Version { get; set; }

        public int RefreshIntervalInMinutes { get; set; }

        public List<RssFeed> RssFeeds { get; set; }

        public ThemeInfo Theme { get; set; }

        public bool PreviewEnabled { get; set; }

        public RssFeed SelectedFeed { get; set; }

        public RssItem SelectedItem { get; set; }

        public AdvertisingInfo AdInfo { get; set; }
    }
}