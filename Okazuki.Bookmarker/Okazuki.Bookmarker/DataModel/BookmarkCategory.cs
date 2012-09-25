using Okazuki.Bookmarker.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<Bookmark> Bookmarks
        {
            get { return this.bookmarks; }
            set { this.SetProperty(ref this.bookmarks, value); }
        }

    }
}
