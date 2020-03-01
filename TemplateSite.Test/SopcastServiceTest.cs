using System.Threading.Tasks;
using NUnit.Framework;
using TemplateSite.Mvc.Services;
using System.Text.RegularExpressions;

namespace TemplateSite.Test
{
    [TestFixture]
    public class SopcastServiceTest
    {
        [Test]
        public async Task GetChampionAsyncTest()
        {
            var serv = new CrawlerServices();

            var champs = await serv.GetLiveChampionAsync("today");

            Assert.That(champs.Count > 0);
        }

        [Test]
        [Repeat(5)]
        public async Task GetMatchAsyncTest()
        {
            var serv = new CrawlerServices();

            var matches = await serv.GetSopcastMatchAsync("");

            Assert.That(matches.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetSopcastLinkAsyncTest()
        {
            var sopServ = new CrawlerServices();
            var url = "http://live3s.com/sopcast/germany/germany-bundesliga-2/eintr-frankfurt-vs-nurnberg-link893026";
            url = "http://vi.live3s.com/link-sopcast/hang-2-duc/wurzburger-kickers-vs-msv-duisburg-link893072";
            var links = await sopServ.FindSopcastLinkAsync(url);

            Assert.That(links.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetLiveMatchAsyncTest()
        {
            var serv = new CrawlerServices();
            var url = "http://vi.live3s.com/truc-tiep-bong-da/hang-2-tay-ban-nha/osasuna-vs-numancia-livetv895362";
            var match = await serv.GetLiveMatch(url);

            Assert.IsNotNullOrEmpty(match.Champion);
        }

        [Test]
        public void TestGetIdCorrectly()
        {
            string sid = "match-id-884084";
            sid = "truc-tiep-bong-da_giao-huu_anh-vs-tho-nhi-ky-livetv893075";
            string id = "",
                pattern = ".*-livetv(\\d*)";
            
            if (Regex.IsMatch(sid, pattern))
            {
                var match = Regex.Match(sid, pattern);
                id = match.Groups[1].Value;
                //if (!string.IsNullOrEmpty(id))
                //    id = id.Replace("livetv", string.Empty);
            }

            Assert.That(id, Is.EqualTo("893075"));
        }
    }
}
