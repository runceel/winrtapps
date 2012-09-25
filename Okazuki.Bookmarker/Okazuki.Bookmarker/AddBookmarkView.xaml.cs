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
    public sealed partial class AddBookmarkView : UserControl
    {
        public AddBookmarkView()
        {
            this.InitializeComponent();
        }

        public string Title
        {
            get { return this.textBoxTitle.Text; }
            set { this.textBoxTitle.Text = value; }
        }

        public string Uri
        {
            get { return this.textBoxUrl.Text; }
            set { this.textBoxUrl.Text = value; }
        }

        private async void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            var model = BookmarkerModel.GetDefault();
            await model.LoadAsync();
            this.comboBoxCategories.ItemsSource = model.Categories;
            this.comboBoxCategories.SelectedItem = model.Categories.FirstOrDefault();

        }
    }
}
