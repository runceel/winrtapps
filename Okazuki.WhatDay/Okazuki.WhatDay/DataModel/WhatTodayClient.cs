using Okazuki.WhatDay.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Okazuki.WhatDay.DataModel
{
    public class WhatTodayClient
    {
        private static readonly string ApiUri = "http://www.mizunotomoaki.com/wikipedia_daytopic/api.cgi/{0:00}{1:00}";

        private static readonly string WikipediaUri = "http://ja.wikipedia.org/wiki/{0}";

        public static Uri CreateApiUri(DateTimeOffset date)
        {
            return new Uri(
                string.Format(ApiUri, date.Month, date.Day), UriKind.Absolute);
        }

        public async Task<ObservableCollection<AnniversaryDetail>> GetAnniversary(DateTimeOffset date)
        {
            var client = new HttpClient();
            var result = await client.GetAsync(
                WhatTodayClient.CreateApiUri(date));
            using (var s = await result.Content.ReadAsStreamAsync())
            {
                return await Task.Run(() =>
                {
                    var doc = XDocument.Load(s);
                    var anniversaryDetails = doc.Root
                        .Descendants("kinenbi_detail")
                        .First()
                        .Descendants("item")
                        .Select(elm =>
                            new AnniversaryDetail
                            {
                                Title = elm.Element("title").Value,
                                Description = elm.Element("description").Value
                            });
                    return new ObservableCollection<AnniversaryDetail>(anniversaryDetails.Any() ? anniversaryDetails : Enumerable.Repeat(AnniversaryDetail.Empty, 1));
                });
            }
        }

        public static Uri GetWikipediaUri(DateTimeOffset date)
        {
            var dateUriText = Uri.EscapeUriString(string.Format("{0}月{1}日", date.Month, date.Day));
            return new Uri(
                string.Format(WhatTodayClient.WikipediaUri, dateUriText),
                UriKind.Absolute);
        }
    }

    public class AnniversaryDetail : BindableBase
    {
        private string title;

        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        private string description;

        public string Description
        {
            get { return this.description; }
            set { this.SetProperty(ref this.description, value); }
        }

        public static AnniversaryDetail Empty 
        {
            get
            {
                return new AnniversaryDetail
                {
                    Title = "何も無い記念日",
                    Description = "何も記念するべきことがない記念すべき日"
                };
            }
        }
    }
}
