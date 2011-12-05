using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using RssStarterKit.Models;

namespace RssStarterKit.Services
{
    public class RssDataService
    {
        public void LoadRssFeed(string url, Action<RssFeed> func)
        {
            RssFeed feed;
            var request = HttpWebRequest.CreateHttp(url) as HttpWebRequest;
            request.BeginGetResponse((token) =>
            {
                var response = request.EndGetResponse(token) as HttpWebResponse;
                var stream = response.GetResponseStream();
                var doc = XDocument.Load(stream);
                var channel = doc.Root.Element("channel");
                feed = new RssFeed()
                {
                    Title = channel.Element("title").Value,
                    Link = channel.Element("link").Value,
                    Description = channel.Element("description").Value,
                    ImageUrl = channel.Element("image").Element("url").Value,
                    Items = (from item in doc.Descendants("item")
                             select new RssItem()
                             {
                                 Title = item.Element("title").Value,
                                 Link = item.Element("link").Value,
                                 Description = item.Element("title").Value,
                                 IsFavorite = false,
                                 PublishDate = ParseRssDateTime(item.Element("pubDate").Value),
                             }).ToList(),
                };
                func(feed);
            }, null);
        }

        private DateTime ParseRssDateTime(string p)
        {
            return DateTime.Now;
        }

        //public List<RssFeed> LoadStoredFromIsoStore()
        //{
        //    var items = GetIsoFeed("stored.dat");
        //    return items;
        //}

        //private List<RssFeed> GetIsoFeed(string filename)
        //{
        //    using (var file = IsolatedStorageFile.GetUserStoreForApplication())
        //    {
        //        if (file.FileExists(filename))
        //        {
        //            using (var stream = file.OpenFile(filename, System.IO.FileMode.Open))
        //            {
        //                using (var reader = new StreamReader(stream))
        //                {
        //                    var data = reader.ReadToEnd();
        //                    var items = JsonConvert.DeserializeObject<List<RssFeed>>(data);
        //                    return items;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return new List<RssFeed>();
        //        }
        //    }
        //}
    }
}