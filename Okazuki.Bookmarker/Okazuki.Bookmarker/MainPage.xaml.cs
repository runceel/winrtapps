﻿using Okazuki.Bookmarker.DataModel;
using Okazuki.UI.Flyouts;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// グループ化されたアイテム ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234231 を参照してください

namespace Okazuki.Bookmarker
{
    /// <summary>
    /// グループ化されたアイテムのコレクションを表示するページです。
    /// </summary>
    public sealed partial class MainPage : Okazuki.Bookmarker.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        /// <param name="navigationParameter">このページが最初に要求されたときに
        /// <see cref="Frame.Navigate(Type, Object)"/> に渡されたパラメーター値。
        /// </param>
        /// <param name="pageState">前のセッションでこのページによって保存された状態の
        /// ディクショナリ。ページに初めてアクセスするとき、状態は null になります。</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var model = BookmarkerModel.GetDefault();
            await model.LoadAsync();
            this.DefaultViewModel["Groups"] = model.Categories;
            this.groupedItemsViewSource.View.MoveCurrentTo(null);
            this.AppBarButtonSetup();
        }

        private async void BookmarkItem_Click(object sender, ItemClickEventArgs e)
        {
            var bookmark = e.ClickedItem as Bookmark;
            if (bookmark.Id == Guid.Empty)
            {
                return;
            }

            if (ApplicationView.Value == ApplicationViewState.Snapped)
            {
                await Launcher.LaunchUriAsync(bookmark.Uri);
            }
            else
            {
                this.Frame.Navigate(typeof(BrowsePage), bookmark.Id.ToString());
            }
        }

        private void buttonAddCategory_Click(object sender, RoutedEventArgs e)
        {
            var model = BookmarkerModel.GetDefault();
            var view = new CreateCategoryView();
            var popup = FlyoutUtils.CreateFlyout(this.BottomAppBar, (Button)sender, view);
            view.CreateClicked += (_, __) =>
            {
                if (!view.Validate())
                {
                    return;
                }

                var category = view.CreateBookmarkCategory();
                model.Categories.Add(category);
                var nowait = model.SaveAsync();
                popup.IsOpen = false;
            };

            popup.IsOpen = true;
        }

        private void buttonAddBookmark_Click(object sender, RoutedEventArgs e)
        {
            var model = BookmarkerModel.GetDefault();
            var view = new AddBookmarkView();

            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.White),
                Width = view.MinWidth,
                Height = view.MinHeight
            };
            border.Child = view;
            var popup = FlyoutUtils.CreateFlyout(this.BottomAppBar, (Button)sender, border);

            view.AddClicked += (_, __) =>
            {
                if (!view.Validate())
                {
                    return;
                }

                model.AddBookmark(view.SelectedCategory.Id, view.CreateBookmark());
                var nowait = model.SaveAsync();
                popup.IsOpen = false;
            };

            popup.IsOpen = true;
        }

        private void itemGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.AppBarButtonSetup();
        }

        private void AppBarButtonSetup()
        {
            this.buttonEditBookmark.Visibility = Visibility.Collapsed;
            this.buttonDeleteBookmark.Visibility = Visibility.Collapsed;
            this.buttonPrevCategory.Visibility = Visibility.Collapsed;
            this.buttonNextCategory.Visibility = Visibility.Collapsed;
            this.buttonDeleteCategory.Visibility = Visibility.Collapsed;

            if (semanticZoom.IsZoomedInViewActive)
            {
                this.BottomAppBar.IsOpen = groupedItemsViewSource.View.CurrentItem != null;

                if (groupedItemsViewSource.View.CurrentItem == null)
                {
                    return;
                }

                this.buttonEditBookmark.Visibility = Visibility.Visible;
                this.buttonDeleteBookmark.Visibility = Visibility.Visible;

            }
            else
            {
                this.BottomAppBar.IsOpen = itemGridZoomedOutView.SelectedItem != null;

                if (itemGridZoomedOutView.SelectedItem == null)
                {
                    return;
                }

                this.buttonPrevCategory.Visibility = Visibility.Visible;
                this.buttonNextCategory.Visibility = Visibility.Visible;
                this.buttonDeleteCategory.Visibility = Visibility.Visible;
            }
        }

        private void buttonEditBookmark_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = groupedItemsViewSource.View.CurrentItem as Bookmark;
            if (bookmark == null || bookmark.Id == Guid.Empty)
            {
                return;
            }

            var model = BookmarkerModel.GetDefault();
            var currentCategory = model.GetCategoryByBookmark(bookmark);

            var view = new AddBookmarkView
            {
                Categories = model.Categories,
                SelectedCategory = currentCategory,
                Title = bookmark.Title,
                Uri = bookmark.Uri.ToString()
            };

            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.White),
                Width = view.MinWidth,
                Height = view.MinHeight
            };
            border.Child = view;
            var popup = FlyoutUtils.CreateFlyout(this.BottomAppBar, (Button)sender, border);

            view.AddClicked += (_, __) =>
            {
                if (!view.Validate())
                {
                    return;
                }

                bookmark.Title = view.Title;
                bookmark.Uri = new Uri(view.Uri, UriKind.Absolute);
                model.ChangeCategory(view.SelectedCategory, bookmark);
                var nowait = model.SaveAsync();
                popup.IsOpen = false;
            };
            popup.IsOpen = true;
        }

        private void buttonPrevCategory_Click(object sender, RoutedEventArgs e)
        {
            var model = BookmarkerModel.GetDefault();
            var category = model.Categories[itemGridZoomedOutView.SelectedIndex];
            if (category == null)
            {
                return;
            }

            this.itemGridZoomedOutView.SelectedIndex = model.MovePrevCategory(category);
            var nowait = model.SaveAsync();
        }

        private void buttonNextCategory_Click(object sender, RoutedEventArgs e)
        {
            var model = BookmarkerModel.GetDefault();
            var category = model.Categories[itemGridZoomedOutView.SelectedIndex];
            if (category == null)
            {
                return;
            }

            this.itemGridZoomedOutView.SelectedIndex = model.MoveNextCategory(category);
            var nowait = model.SaveAsync();
        }

        private void buttonDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var model = BookmarkerModel.GetDefault();
            var category = model.Categories[itemGridZoomedOutView.SelectedIndex];
            if (category == null)
            {
                return;
            }

            model.Categories.Remove(category);
            var nowait = model.SaveAsync();
        }

        private void semanticZoom_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            this.AppBarButtonSetup();
        }

        private void buttonDeleteBookmark_Click(object sender, RoutedEventArgs e)
        {
            var model = BookmarkerModel.GetDefault();
            var selectedBookmark = groupedItemsViewSource.View.CurrentItem as Bookmark;
            if (selectedBookmark == null)
            {
                return;
            }

            var currentCategory = model.GetCategoryByBookmark(selectedBookmark);
            currentCategory.RemoveBookmark(selectedBookmark);
            var nowait = model.SaveAsync();
        }

    }
}
