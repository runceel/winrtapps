﻿using Okazuki.Bookmarker.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 基本ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234237 を参照してください

namespace Okazuki.Bookmarker
{
    /// <summary>
    /// 多くのアプリケーションに共通の特性を指定する基本ページ。
    /// </summary>
    public sealed partial class BrowsePage : Okazuki.Bookmarker.Common.LayoutAwarePage
    {
        public BrowsePage()
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
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var model = BookmarkerModel.GetDefault();
            await model.LoadAsync();

            var id = navigationParameter as string;
            if (id == null)
            {
                this.GoHome(this, null);
            }

            var bookmark = model.GetBookmarkById(new Guid(id));
            this.DefaultViewModel["bookmark"] = bookmark;
        }

        /// <summary>
        /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
        /// このページに関連付けられた状態を保存します。値は、
        /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
        /// </summary>
        /// <param name="pageState">シリアル化可能な状態で作成される空のディクショナリ。</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private async void NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            var dlg = new MessageDialog("ページの読み込みに失敗しました");
            await dlg.ShowAsync();
        }

        private async void buttonOpenBrowser_Click(object sender, RoutedEventArgs e)
        {
            var bookmark = (Bookmark) this.DefaultViewModel["bookmark"];
            await Launcher.LaunchUriAsync(bookmark.Uri);
        }

    }
}
