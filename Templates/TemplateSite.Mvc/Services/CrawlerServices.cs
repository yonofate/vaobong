using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemplateSite.Mvc.Models;
using ServiceStack.Text;
using System.IO;

namespace TemplateSite.Mvc.Services
{
    public class CrawlerServices
    {
        /// <summary>
        /// get sopcast link or livestream link
        /// </summary>
        /// <param name="url">an url that load data for sopcast link, values is: all | today | online.</param>
        /// <param name="isLiveStream">determine that load sopcast or livestream</param>
        /// <returns>a collection of champion</returns>
        public async Task<List<GameChampion>> GetLiveChampionAsync(string url, bool isLiveStream = false)
        {
            List<GameChampion> champions = new List<GameChampion>();
            string sourceUrl;

            if (isLiveStream)
            {
                sourceUrl = ConfigurationManager.AppSettings["LiveSourceUrl"];
            }
            else
            {
                sourceUrl = ConfigurationManager.AppSettings["SopcastSourceUrl"];
            }

            if (string.IsNullOrEmpty(sourceUrl)) return champions;

            // download html from url
            sourceUrl = sourceUrl.TrimEnd('/');
            switch (url)
            {
                case "online": sourceUrl += "/truc-tuyen";
                    break;
                case "today": sourceUrl += "/hom-nay";
                    break;
                default: // all
                    break;
            }
            Debug.WriteLine("url: " + sourceUrl);

            //using (var webClient = new WebClient())
            //{
            //    webClient.Encoding = Encoding.UTF8;
            //    webClient.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
            //    html = await webClient.DownloadStringTaskAsync(sourceUrl);
            //}
            //html = await DownloadHtmlAsync(sourceUrl);

            //if (string.IsNullOrEmpty(html) || string.IsNullOrWhiteSpace(html)) return champions;

            //// parse data

            //HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(html);

            //if (doc.DocumentNode == null ||
            //    doc.DocumentNode.Descendants().Any() == false)
            //    return champions; // no data found

            var doc = await LoadHtmlDocAsync(sourceUrl);
            if (doc == null) return champions;

            // get all div
            var rows = doc.SelectNodes("//div[@class='content']/div");
            if (rows == null || rows.Count == 0) return champions;

            string host = ConfigurationManager.AppSettings["HostCrawlerUrl"];
            if (string.IsNullOrEmpty(host) == false) host = host.TrimEnd('/');
            int rowCount = rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var rowClass = row.GetAttributeValue("class", string.Empty); // expect row-tall
                // if (string.IsNullOrEmpty(rowClass)) continue;

                if (rowClass.Contains("row-tall")) // champion row
                {
                    GameChampion champ = new GameChampion();
                    champ.Champion = row.SelectSingleNode(".//div[@class='left']/a").InnerText();
                    champ.FlagClass = row.SelectSingleNode(".//div[@class='left']/span").AttributeValue("class", string.Empty);
                    Debug.WriteLine("{0} : {1}", champ.Champion, champ.FlagClass, 1);

                    // get sibling row for matches

                    try
                    {
                        HtmlNode sibling = row.NextSibling;
                        while (sibling != null)
                        {
                            var sibClass = sibling.GetAttributeValue("class", string.Empty);
                            if (!string.IsNullOrEmpty(sibClass) && !sibClass.Contains("row-tall"))
                            {
                                // todo: check attr itemscope

                                // match row
                                GameMatch match = new GameMatch();
                                match.Date = sibling.SelectSingleNode("./div[contains(@class,'time-playing')]").InnerText();
                                match.Team1 = sibling.SelectSingleNode("./div[contains(@class,'ply') and contains(@class,'t-home')]").InnerText();
                                match.Team2 = sibling.SelectSingleNode("./div[contains(@class,'ply') and contains(@class,'t-away')]").InnerText();
                                match.Score = sibling.SelectSingleNode("./div[contains(@class,'sco')]").InnerText();
                                var matchUrl = sibling.SelectSingleNode("./div[contains(@class,'live-centre')]/a").AttributeValue("href", string.Empty);
                                match.Url = host + "/" + matchUrl.TrimStart('/');
                                // http://vi.live3s.com/link-sopcast/hang-2-duc/eintr-frankfurt-vs-nurnberg-link893026
                                var elapsed = sibling.SelectSingleNode("./div[@class='min']/span");
                                if (elapsed != null)
                                {
                                    match.ElapsedTime = elapsed.InnerText; // elapsedTime get by ajax
                                    var sid = elapsed.GetAttributeValue("id", string.Empty);
                                    string pattern = ".*-(\\d+)";
                                    if (string.IsNullOrEmpty(sid) == false && Regex.IsMatch(sid, pattern))
                                    {
                                        var grs = Regex.Match(sid, pattern);
                                        match.Id = grs.Groups[1].Value;
                                    }
                                }

                                Debug.WriteLine("> {0} - {1} - {2} - {3}", match.Date, match.Team1, match.Score, match.Team2);

                                // add match to champion
                                champ.Matches.Add(match);

                                // get next sibling
                                sibling = sibling.NextSibling;
                                i++; // skip match row
                            }
                            else
                            {
                                break; // next sibling is champion
                            }
                        }
                    }
                    catch (Exception ex) {
                        Debug.WriteLine("parse data error: " + ex.Message);
                    } // parse data error

                    champions.Add(champ);
                }
            }

            return champions;
        }

        /// <summary>
        /// update livescore
        /// </summary>
        /// <returns></returns>
        public async Task<List<GameMatch>> GetSopcastMatchAsync(string matchId)
        {
            List<GameMatch> matches = new List<GameMatch>();
            string url = ConfigurationManager.AppSettings["SopcastLiveUrl"];

            if (string.IsNullOrEmpty(url)) return matches;

            //string html = string.Empty;
            //using (var web = new WebClient())
            //{
            //    html = await web.DownloadStringTaskAsync(url);
            //}
            string html = await DownloadHtmlAsync(url);

            if (string.IsNullOrEmpty(html)) return matches;

            var arr = html.Split(';');
            if (arr.Length == 0) return matches;
            int len = arr.Length;
            for (int i = 0; i < len; i++)
            {
                var infos = arr[i].Split(','); // 894271,90+,1,3
                if (infos.Length != 4) continue;

                var match = new GameMatch();
                match.Id = infos[0];
                match.ElapsedTime = infos[1];
                match.Score = string.Format("{0} - {1}", infos[2], infos[3]);

                matches.Add(match);

                if (!string.IsNullOrEmpty(matchId) && matchId.Equals(match.Id, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            return matches;
        }

        /// <summary>
        /// get sopcast link for given match url
        /// </summary>
        /// <param name="link">an url of match to load sopcast link</param>
        /// <returns></returns>
        public async Task<List<SopcastLink>> FindSopcastLinkAsync(string link)
        {
            List<SopcastLink> links = new List<SopcastLink>();
            //string html = string.Empty;

            //using (var webClient = new WebClient())
            //{
            //    webClient.Encoding = Encoding.UTF8;
            //    webClient.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
            //    html = await webClient.DownloadStringTaskAsync(link);
            //}
            //string html = await DownloadHtmlAsync(link);
            //if (string.IsNullOrEmpty(html)) return links;

            //HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(html);

            //if (doc.DocumentNode == null ||
            //    doc.DocumentNode.Descendants().Any() == false)
            //    return links;

            var doc = await LoadHtmlDocAsync(link);
            if (doc == null) return links;

            var table = doc.SelectSingleNode("//div[@class='news-articles']/div[contains(@class,'link-other')]/table");
            if (table == null || table.ChildNodes == null || table.ChildNodes.Count == 0)
                return links;

            var rows = table.SelectNodes("//tr");
            int length = rows.Count;
            for (int i = 1; i < length; i++) // skip first row
            {
                var row = rows[i];
                var sopLink = new SopcastLink();
                sopLink.Url = row.ChildNodes[3].InnerText;
                sopLink.Bitrate = row.ChildNodes[2].InnerText;
                sopLink.Lang = row.ChildNodes[1].InnerText;

                links.Add(sopLink);
            }

            return links;
        }

        public async Task<LiveMatch> GetLiveMatch(string url)
        {   
            var doc = await LoadHtmlDocAsync(url);
            if (doc == null) return null;

            var match = new LiveMatch();
            var streamInfo = doc.SelectSingleNode("//div[@class='content']/div[@class='row-news']//div[@class='stream-info']");
            if (streamInfo == null) return null;
            // champion
            match.Champion = streamInfo.SelectSingleNode("./div[@class='shortInfo']").InnerText();
            match.DateTime = streamInfo.SelectSingleNode(".//span[@class='time-by-timezone']").InnerText();
            match.OriginServer = doc.SelectSingleNode("//div[@class='content']//div[@class='news-articles']/div[@id='zone-match-livetv-stream']/iframe").AttributeValue("src", string.Empty);

            // stream info            
            match.TeamA.TeamName = streamInfo.SelectSingleNode("./div[@class='competeTeams']/span[contains(@class,'teamA') and contains(@class, 'tright')]").InnerText();
            match.TeamB.TeamName = streamInfo.SelectSingleNode("./div[@class='competeTeams']/span[contains(@class,'teamA') and contains(@class, 'tleft')]").InnerText();
            match.TeamA.Score = streamInfo.SelectSingleNode(".//span[@class='score-teamA']").InnerText();
            match.TeamB.Score = streamInfo.SelectSingleNode(".//span[@class='score-teamB']").InnerText();
            match.TeamA.TeamFlag = streamInfo.SelectSingleNode("./span[@class='bageTeamA']/img").AttributeValue("src", string.Empty);
            match.TeamB.TeamFlag = streamInfo.SelectSingleNode("./span[@class='bageTeamB']/img").AttributeValue("src", string.Empty);

            // server
            var arrServers = doc.SelectNodes("//div[@class='content']//div[@id='zone-other-livetv']/a");
            if (arrServers != null && arrServers.Count > 0)
            {
                var count = arrServers.Count;
                for (int i = 0; i < count; i++)
                {
                    var a = arrServers[i];
                    var server = new LiveServer();
                    server.Id = a.GetAttributeValue("data-pid", string.Empty);
                    if (string.IsNullOrEmpty(server.Id))
                    {
                        // try get by title if data-pid cannot get value
                        server.Id = a.GetAttributeValue("title", string.Empty);
                    }
                    // a.SelectSingleNode("")

                    server.Text = a.InnerText;
                    if (string.IsNullOrEmpty(server.Text) == false) server.Text = server.Text.Replace("&nbsp;", string.Empty);
                    server.Flags = a.SelectSingleNode("./span").AttributeValue("class", string.Empty);

                    match.Servers.Add(server);
                }
            }

            return match;
        }

        #region helpers
        private async Task<string> DownloadHtmlAsync(string url)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url)) return string.Empty;

            string html = string.Empty;
            using (var webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
                html = await webClient.DownloadStringTaskAsync(url);
            }

            return html;
        }

        private async Task<HtmlNode> LoadHtmlDocAsync(string url)
        {
            var html = await DownloadHtmlAsync(url);
            if (string.IsNullOrEmpty(html)) return null;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            if (doc.DocumentNode == null || doc.DocumentNode.Descendants().Any() == false) return null;

            return doc.DocumentNode;
        }
        #endregion

        #region NewServices
        public LiveMatch GetMatchs(string url)
        {
            var CookieLogin = new CookieCollection();
            //CookieLogin.Add(new Cookie("WEBSESSID", "39b8f5ea01510c50eed45244212f73f5", "", "3.uv128.com"));
            //CookieLogin.Add(new Cookie("nlbi_1214910", "czlnMKIfYCmSWrAJFOvyQQAAAAAKusk/Y3NIsL5SdHyfvxpA", "", "3.uv128.com"));
            //CookieLogin.Add(new Cookie("cookie_user_onlinekey_ddz", "110f1c0b89685ffdcafc18eb92264a75", "", "3.uv128.com"));
            //CookieLogin.Add(new Cookie("cookie_user_onlineip_ddz", "116.100.62.176", "", "3.uv128.com"));
            //CookieLogin.Add(new Cookie("vietnamese_version", "0", "", "3.uv128.com"));
            //CookieLogin.Add(new Cookie("cookie_user_language", "simplified", "", "3.uv128.com"));
            //CookieLogin.Add(new Cookie("cookie_user_language_ddz", "simplified", "", "3.uv128.com"));
            //CookieLogin.Add(new Cookie("cookie_userdb_language", "simplified", "", "3.uv128.com"));
            //CookieLogin.Add(new Cookie("cookie_userdb_language", "simplified", "", "3.uv128.com"));
            //CookieLogin.Add(new Cookie("cookie_userdb_language", "simplified", "", "3.uv128.com"));
            var str = "http://3.uv128.com".GetStringFromUrl(requestFilter: getRequest =>
                {
                    getRequest.CookieContainer = new CookieContainer();
                    //getRequest.CookieContainer.Add(CookieLogin);
                    getRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    getRequest.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                    getRequest.Headers.Add("Accept-Language", "vi,en-US;q=0.8,en;q=0.6");
                    getRequest.KeepAlive = true;
                    getRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
                    //getRequest.Headers.Add("X-Forwarded-For", account.FakeIp);
                    //getRequest.Referer = string.Format("{0}/", account.HostUrl);
                    //getRequest.Host = account.HostUrl.Replace("https://", "").Replace("http://", "");
                    //getRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
                    //getRequest.Headers.Add("Vary", "Accept");
                },
                    responseFilter: response =>
                    {
                        CookieLogin.Add(response.Cookies);
                        var u = response.Headers["Location"];
                    });
            str = "http://3.uv128.com/A0099_0050/d4/".GetStringFromUrl(requestFilter: getRequest =>
            {
                getRequest.CookieContainer = new CookieContainer();
                //getRequest.CookieContainer.Add(CookieLogin);
                getRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                getRequest.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                getRequest.Headers.Add("Accept-Language", "vi,en-US;q=0.8,en;q=0.6");
                getRequest.KeepAlive = true;
                getRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
                //getRequest.Headers.Add("X-Forwarded-For", account.FakeIp);
                getRequest.Referer = "http://3.uv128.com/";
                //getRequest.Host = account.HostUrl.Replace("https://", "").Replace("http://", "");
                //getRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
                //getRequest.Headers.Add("Vary", "Accept");
            },
                    responseFilter: response =>
                    {
                        CookieLogin.Add(response.Cookies);
                        var u = response.Headers["Location"];
                    });

            string url_login = "";
            "http://3.uv128.com/A0099_0000/soccer_api/free_account.php?r=logindemo&language=english&fl=d4".GetStringFromUrl(requestFilter: getRequest =>
        {
            getRequest.CookieContainer = new CookieContainer();
            getRequest.CookieContainer.Add(CookieLogin);
            getRequest.Referer = "http://3.uv128.com/A0099_0050/d4/login_header.php?language=english";
            getRequest.AllowAutoRedirect = false;
        },
        responseFilter: response =>
        {
            CookieLogin.Add(response.Cookies);
            url_login = response.Headers["Location"];
        });


            url_login.GetStringFromUrl(requestFilter: getRequest =>
            {
                getRequest.CookieContainer = new CookieContainer();
                getRequest.CookieContainer.Add(CookieLogin);
                getRequest.Referer = "http://3.uv128.com/A0099_0050/d4/login_header.php?language=english";
                getRequest.AllowAutoRedirect = false;
            },
        responseFilter: response =>
        {
            CookieLogin.Add(response.Cookies);
            url_login = response.Headers["Location"];
        });
            var postData = "game=1&date=&action=gtSchedule";

            var byteArray = Encoding.ASCII.GetBytes(postData);
            "http://3.uv128.com/A0099_0050/d4/live-channel-action.php".PostBytesToUrl(byteArray,
                    requestFilter: getRequest =>
                    {
                        getRequest.CookieContainer = new CookieContainer();
                        getRequest.CookieContainer.Add(CookieLogin);
                        getRequest.AllowAutoRedirect = true;
                        getRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
                        getRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                        getRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                        getRequest.KeepAlive = true;
                        getRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        getRequest.Headers.Add("Origin", "http://3.uv128.com");
                        getRequest.Referer = "http://3.uv128.com/A0099_0050/d4/live_channel.php?t=1&game=1";
                        getRequest.Host = "3.uv128.com";
                        getRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                        //getRequest.Headers.Add("X-Forwarded-For", account.FakeIp);
                    },
                    responseFilter: response =>
                    {
                        CookieLogin.Add(response.Cookies);
                        var sr = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    });
            return null;
        }
        #endregion
    }
}