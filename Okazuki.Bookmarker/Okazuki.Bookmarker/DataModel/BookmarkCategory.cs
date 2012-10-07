using Okazuki.Bookmarker.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okazuki.Bookmarker.DataModel
{
    public class BookmarkCategory : BindableBase
    {
        private Guid id = Guid.NewGuid();

        public Guid Id
        {
            get { return this.id; }
            set { this.SetProperty(ref this.id, value); }
        }


        private string title;

        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        private ObservableCollection<Bookmark> bookmarks = new ObservableCollection<Bookmark>();

        public IEnumerable<Bookmark> Bookmarks
        {
            get { return this.bookmarks; }
            set 
            {
                this.SetProperty(ref this.bookmarks, new ObservableCollection<Bookmark>(value));
                this.CreateEmptyBookmarkIfNeed();
            }
        }

        public void AddBookmark(Bookmark bookmark)
        {
            this.bookmarks.Add(bookmark);
            this.RemoveEmptyBookmarkIfNeed();
        }

        public void RemoveBookmark(Bookmark bookmark)
        {
            this.bookmarks.Remove(bookmark);
            this.CreateEmptyBookmarkIfNeed();
        }

        public BookmarkCategory()
        {
            this.CreateEmptyBookmarkIfNeed();
        }

        private void CreateEmptyBookmarkIfNeed()
        {
            if (this.bookmarks.Any())
            {
                return;
            }

            this.bookmarks.Add(Bookmark.CreateEmpty());
        }

        private void RemoveEmptyBookmarkIfNeed()
        {
            if (this.bookmarks.Any(b => b.Id != Guid.Empty) && this.bookmarks.First().Id == Guid.Empty)
            {
                this.bookmarks.RemoveAt(0);
            }
        }
    }
}
