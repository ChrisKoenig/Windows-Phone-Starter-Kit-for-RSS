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

    public class Settings
    {
        public string Title { get; set; }

        public int RefreshIntervalInMinutes { get; set; }

        public List<RssFeed> RssFeeds { get; set; }

        public ThemeInfo Theme { get; set; }

        public Settings()
        {
        }
    }
}