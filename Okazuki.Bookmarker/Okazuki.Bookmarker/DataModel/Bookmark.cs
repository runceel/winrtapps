using Okazuki.Bookmarker.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okazuki.Bookmarker.DataModel
{
    public class Bookmark : BindableBase
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

        private Uri uri;

        public Uri Uri
        {
            get { return this.uri; }
            set { this.SetProperty(ref this.uri, value); }
        }

        public static Bookmark CreateEmpty()
        {
            return new Bookmark
            {
                Id = Guid.Empty,
                Title = "ブックマークを追加してください",
                Uri = new Uri("about:blank")
            };
        }
    }
}
