using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Okazuki.WhatDay.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Okazuki.WhatDay.Test
{
    [TestClass]
    public class WhatTodayClientTest
    {
        private WhatTodayClient target;

        [TestInitialize]
        public void Initialize()
        {
            this.target = new WhatTodayClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.target = null;
        }

        [TestMethod]
        public void TestCreateUri()
        {
            var date = DateTimeOffset.ParseExact("20100112", "yyyyMMdd", null);
            var result = WhatTodayClient.CreateApiUri(date);
            Assert.AreEqual(
                "http://www.mizunotomoaki.com/wikipedia_daytopic/api.cgi/0112",
                result.AbsoluteUri);
        }

        [TestMethod]
        public async Task TestGetAnniversary()
        {
            var result = await this.target.GetAnniversary(
                DateTimeOffset.ParseExact("20100101", "yyyyMMdd", null));
            Assert.IsNotNull(result);

            var 正月 = result.Single(a => a.Title == "正月");
            Assert.AreEqual(
                "正月",
                正月.Title);
            Assert.AreEqual(string.Empty, 正月.Description);

            var 元日 = result.Single(a => a.Title == "元日");
            Assert.AreEqual(
                "元日",
                元日.Title);
            Assert.AreEqual(
                "1年の始めの日。日本では1948年7月施行の祝日法により国民の祝日となっている。",
                元日.Description);
        }

        [TestMethod]
        public void WikipediaUriTest()
        {
            Uri result = WhatTodayClient.GetWikipediaUri(
                DateTimeOffset.ParseExact("20100101", "yyyyMMdd", null));
            Assert.AreEqual(
                "http://ja.wikipedia.org/wiki/1%E6%9C%881%E6%97%A5",
                result.AbsoluteUri);
        }
    }
}
