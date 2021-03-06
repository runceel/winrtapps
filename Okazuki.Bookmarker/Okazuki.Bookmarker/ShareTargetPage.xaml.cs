﻿using Okazuki.Bookmarker.DataModel;
using Okazuki.Bookmarker.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 共有ターゲット コントラクトのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234241 を参照してください

namespace Okazuki.Bookmarker
{
    /// <summary>
    /// このページを使用すると、他のアプリケーションがこのアプリケーションを介してコンテンツを共有できます。
    /// </summary>
    public sealed partial class ShareTargetPage : Okazuki.Bookmarker.Common.LayoutAwarePage
    {
        /// <summary>
        /// 共有操作について、Windows と通信するためのチャネルを提供します。
        /// </summary>
        private ShareOperation _shareOperation;

        private BookmarkerModel currentModel;

        public ShareTargetPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 他のアプリケーションがこのアプリケーションを介してコンテンツの共有を求めた場合に呼び出されます。
        /// </summary>
        /// <param name="args">Windows と連携して処理するために使用されるアクティベーション データ。</param>
        public async void Activate(ShareTargetActivatedEventArgs args)
        {
            this.currentModel = BookmarkerModel.GetDefault();
            await this.currentModel.LoadAsync();

            this._shareOperation = args.ShareOperation;

            // ビュー モデルを使用して、共有されるコンテンツのメタデータを通信します
            var shareProperties = this._shareOperation.Data.Properties;
            var thumbnailImage = new BitmapImage();
            this.DefaultViewModel["Title"] = shareProperties.Title;
            this.DefaultViewModel["Description"] = shareProperties.Description;
            this.DefaultViewModel["Image"] = thumbnailImage;
            this.DefaultViewModel["Sharing"] = false;
            this.DefaultViewModel["ShowImage"] = false;
            this.DefaultViewModel["Comment"] = String.Empty;
            this.DefaultViewModel["SupportsComment"] = true;

            this.addBookmarkView.Title = shareProperties.Title;
            this.addBookmarkView.Uri = (await this._shareOperation.Data.GetUriAsync()).ToString();

            Window.Current.Content = this;
            Window.Current.Activate();

            // 共有されるコンテンツの縮小版イメージをバックグラウンドで更新します
            if (shareProperties.Thumbnail != null)
            {
                var stream = await shareProperties.Thumbnail.OpenReadAsync();
                thumbnailImage.SetSource(stream);
                this.DefaultViewModel["ShowImage"] = true;
            }
        }

        /// <summary>
        /// ユーザーが [共有] をクリックしたときに呼び出されます。
        /// </summary>
        /// <param name="sender">共有を開始するときに使用される Button インスタンス。</param>
        /// <param name="e">ボタンがどのようにクリックされたかを説明するイベント データ。</param>
        private async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.addBookmarkView.SelectedCategory == null)
            {
                return;
            }

            if (!this.addBookmarkView.Validate())
            {
                return;
            }

            this.DefaultViewModel["Sharing"] = true;
            this._shareOperation.ReportStarted();

            var bookmark = this.addBookmarkView.CreateBookmark();
            var categoryId = this.addBookmarkView.SelectedCategory.Id;
            await DispatcherHolder.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                this.currentModel.AddBookmark(categoryId, bookmark);
                await this.currentModel.SaveAsync();
            });
            this.DefaultViewModel["Comment"] = string.Format("{0}を登録しました", bookmark.Title);

            this._shareOperation.ReportCompleted();
        }

    }
}
