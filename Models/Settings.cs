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
        public string AdUnitId { get; set; }

        public string ApplicationId { get; set; }
    }

    public class Settings
    {
        public AdvertisingInfo AdInfo { get; set; }

        public bool PreviewEnabled { get; set; }

        public int RefreshIntervalInMinutes { get; set; }

        public List<RssFeed> RssFeeds { get; set; }

        public RssFeed SelectedFeed { get; set; }

        public RssItem SelectedItem { get; set; }

        public ThemeInfo Theme { get; set; }

        public string Title { get; set; }

        public int Version { get; set; }
    }
}