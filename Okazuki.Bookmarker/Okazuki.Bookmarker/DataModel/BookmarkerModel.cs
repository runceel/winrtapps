using Okazuki.Bookmarker.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Okazuki.Bookmarker.DataModel
{
    public class BookmarkerModel : BindableBase
    {
        private readonly object SyncRoot = new object();

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
            try
            {
                Monitor.Enter(this.SyncRoot);
                var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(
                    StoreFileName, CreationCollisionOption.ReplaceExisting);
                using (var s = await file.OpenStreamForWriteAsync())
                {
                    await SaveAsync(s);
                }
            }
            finally
            {
                Monitor.Exit(this.SyncRoot);
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

        public bool AddBookmark(Guid categoryId, Bookmark bookmark)
        {
            var category = this.Categories.SingleOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                return false;
            }

            category.AddBookmark(bookmark);
            return true;
        }

        public void ChangeCategory(BookmarkCategory newCategory, Bookmark bookmark)
        {
            var currentCategory = this.GetCategoryByBookmark(bookmark);
            if (currentCategory != newCategory)
            {
                currentCategory.RemoveBookmark(bookmark);
                newCategory.AddBookmark(bookmark);
            }
        }

        public int MoveNextCategory(BookmarkCategory targetCategory)
        {
            if (targetCategory == this.categories.Last())
            {
                return this.categories.Count - 1;
            }

            var index = this.categories.IndexOf(targetCategory);
            this.categories.RemoveAt(index);
            this.categories.Insert(++index, targetCategory);
            return index;
        }

        public int MovePrevCategory(BookmarkCategory targetCategory)
        {
            if (targetCategory == this.categories.First())
            {
                return 0;
            }

            var index = this.categories.IndexOf(targetCategory);
            this.categories.RemoveAt(index);
            this.categories.Insert(--index, targetCategory);
            return index;
        }

        public Bookmark GetBookmarkById(Guid id)
        {
            return this.Categories
                .SelectMany(c => c.Bookmarks)
                .SingleOrDefault(b => b.Id == id);
        }
    }
}
