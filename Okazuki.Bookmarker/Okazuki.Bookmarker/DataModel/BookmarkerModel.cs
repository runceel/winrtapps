using Okazuki.Bookmarker.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Okazuki.Bookmarker.DataModel
{
    public class BookmarkerModel : BindableBase
    {
        private static readonly string StoreFileName = "_applicationData.xml";

        private static readonly BookmarkerModel defaultInstance = new BookmarkerModel();

        public static BookmarkerModel GetDefault()
        {
            return defaultInstance;
        }

        /// <summary>
        /// 初期化済みかどうかのフラグ
        /// </summary>
        private bool isLoaded = false;

        private ObservableCollection<BookmarkCategory> categories = new ObservableCollection<BookmarkCategory>();

        public ObservableCollection<BookmarkCategory> Categories
        {
            get { return this.categories; }
            set { this.SetProperty(ref this.categories, value); }
        }

        public async Task SaveAsync()
        {
            var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(
                StoreFileName, CreationCollisionOption.ReplaceExisting);
            using (var s = await file.OpenStreamForWriteAsync())
            {
                await SaveAsync(s);
            }
        }

        public async Task SaveAsync(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(ObservableCollection<BookmarkCategory>));
            var ms = new MemoryStream();
            serializer.WriteObject(ms, this.categories);
            ms.Seek(0, SeekOrigin.Begin);
            await ms.CopyToAsync(stream);
        }

        public async Task LoadAsync()
        {
            if (this.isLoaded)
            {
                return;
            }

            try
            {
                var file = await ApplicationData.Current.RoamingFolder.GetFileAsync(StoreFileName);
                using (var s = await file.OpenStreamForReadAsync())
                {
                    await LoadAsync(s);
                }
            }
            catch (FileNotFoundException)
            {
                this.InitializeCategoryData();
            }
        }

        private void InitializeCategoryData()
        {
            this.isLoaded = true;

            this.Categories.Clear();
            this.Categories.Add(new BookmarkCategory
            {
                Title = "SNS",
                Bookmarks = new ObservableCollection<Bookmark>
                {
                    new Bookmark { Title = "Twitter", Uri = new Uri("https://twitter.com/") },
                    new Bookmark { Title = "Facebook", Uri = new Uri("https://www.facebook.com/") },
                    new Bookmark { Title = "mixi", Uri = new Uri("http://mixi.jp/") },
                }
            });

            this.Categories.Add(new BookmarkCategory
            {
                Title = "ニュース",
                Bookmarks = new ObservableCollection<Bookmark>
                {
                    new Bookmark { Title = "Google News", Uri = new Uri("https://news.google.co.jp/") },
                    new Bookmark { Title = "MSN産経ニュース", Uri = new Uri("http://sankei.jp.msn.com/") },
                    new Bookmark { Title = "Yahoo!ニュース", Uri = new Uri("http://headlines.yahoo.co.jp/hl") },
                }
            });

            this.Categories.Add(new BookmarkCategory
            {
                Title = "便利",
                Bookmarks = new ObservableCollection<Bookmark>
                {
                    new Bookmark { Title = "Amazon", Uri = new Uri("http://www.amazon.co.jp/") },
                    new Bookmark { Title = "Yahoo!天気", Uri = new Uri("http://weather.yahoo.co.jp/weather/") },
                    new Bookmark { Title = "Google", Uri = new Uri("https://www.google.co.jp/") },
                }
            });
        }

        public async Task LoadAsync(Stream stream)
        {
            if (this.isLoaded)
            {
                return;
            }

            this.isLoaded = true;

            var serializer = new DataContractSerializer(typeof(ObservableCollection<BookmarkCategory>));
            var data = await Task.Run(() => 
                serializer.ReadObject(stream) as ObservableCollection<BookmarkCategory>);
            if (data == null)
            {
                return;
            }

            this.Categories.Clear();
            foreach (var category in data)
            {
                this.Categories.Add(category);
            }
        }

        public BookmarkCategory GetCategoryByBookmark(Bookmark bookmark)
        {
            return this.Categories.FirstOrDefault(c => c.Bookmarks.Any(b => b.Id == bookmark.Id));
        }
    }
}
