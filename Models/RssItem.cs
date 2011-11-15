using System;
using System.Collections.Generic;
using System.Linq;

namespace RssStarterKit.Models
{
    public class RssItem
    {
        public string Title { get; set; }

        public DateTime PublishDate { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsRead { get; set; }
    }
}