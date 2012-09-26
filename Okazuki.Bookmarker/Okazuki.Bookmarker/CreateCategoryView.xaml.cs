using Okazuki.Bookmarker.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace Okazuki.Bookmarker
{
    public sealed partial class CreateCategoryView : UserControl
    {
        public CreateCategoryView()
        {
            this.InitializeComponent();
        }

        public event RoutedEventHandler CreateClicked;

        public string CategoryName
        {
            get { return this.textBoxCategoryName.Text; }
            set { this.textBoxCategoryName.Text = value; }
        }

        public bool Validate()
        {
            var validate = !string.IsNullOrWhiteSpace(this.CategoryName);
            this.textBlockCategoryErrorMessage.Text = validate ? string.Empty : "タイトルを入力してください";
            return validate;
        }

        public BookmarkCategory CreateBookmarkCategory()
        {
            if (!this.Validate())
            {
                throw new InvalidOperationException();
            }

            return new BookmarkCategory { Title = this.CategoryName };
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.CreateClicked != null)
            {
                this.CreateClicked(sender, e);
            }
        }
    }
}
