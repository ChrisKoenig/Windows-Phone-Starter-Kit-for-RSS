using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace RssStarterKit.Helpers
{
    public class IsoHelper
    {
        internal static void SaveIsoString(string filename, string data)
        {
            using (var file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists(filename))
                    file.DeleteFile(filename);
                using (var stream = file.CreateFile(filename))
                using (var writer = new StreamWriter(stream))
                    writer.Write(data);
            }
        }

        internal static string LoadIsoString(string filename)
        {
            using (var file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists(filename))
                {
                    using (var stream = file.OpenFile(filename, FileMode.Open))
                    using (var reader = new StreamReader(stream))
                        return reader.ReadToEnd();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}