using Okazuki.WhatDay.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// アイテム ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234233 を参照してください

namespace Okazuki.WhatDay
{
    /// <summary>
    /// アイテムのコレクションのプレビューを表示するページです。このページは、分割アプリケーションで使用できる
    /// グループを表示し、その 1 つを選択するために使用されます。
    /// </summary>
    public sealed partial class MainPage : Okazuki.WhatDay.Common.LayoutAwarePage
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
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            await Refresh();
        }

        private async Task Refresh()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.None)
            {
                var dlg = new MessageDialog("インターネットに接続した状態でアプリケーションを起動してください");
                await dlg.ShowAsync();
                return;
            }

            this.DefaultViewModel["Items"] = await new WhatTodayClient().GetAnniversary(DateTimeOffset.Now);
        }

        private async void wikipediaButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
                WhatTodayClient.GetWikipediaUri(DateTimeOffset.Now));
        }

        private async void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }
    }
}
