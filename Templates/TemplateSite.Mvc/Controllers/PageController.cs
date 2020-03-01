using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;

using Piranha.Mvc;
using System.Threading.Tasks;
using System.Configuration;
using TemplateSite.Mvc.Services;
using System.Text.RegularExpressions;
using Piranha;

namespace TemplateSite.Mvc.Controllers
{
	/// <summary>
	/// The page controller is the standard controller displaying a page
	/// generated from the manager interface.
	/// </summary>
    public class PageController : SinglePageController
    {
		/// <summary>
		/// Gets a standard page.
		/// </summary>
		/// <returns>The view result</returns>
        public ActionResult Index() {
			var model = GetModel() ;
            //new CrawlerServices().GetMatchs("");
            return View(model.GetView(), model) ;
        }

        public ActionResult M88Guide()
        {
            return View();
        }

        public ActionResult SopcastLink()
        {
            
            return View();
        }

        public async Task<ActionResult> LoadLiveLink(string live, string tab)
        {
            var sopServ = new CrawlerServices();
            bool isLiveStream = string.IsNullOrEmpty(live) == false;
            var links = await sopServ.GetLiveChampionAsync(tab, isLiveStream);

            return Json(links, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateLivescore(string matchId)
        {
            var sopServ = new CrawlerServices();
            var matches = await sopServ.GetSopcastMatchAsync(matchId);
            if (string.IsNullOrEmpty(matchId) == false)
            {
                matches = matches.Where(m => m.Id.Equals(matchId, StringComparison.OrdinalIgnoreCase))
                                 .ToList();
            }
            return Json(matches, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> FindSopcastLink(string link)
        {
            var sopServ = new CrawlerServices();
            var links = await sopServ.FindSopcastLinkAsync(link);

            return Json(links, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LiveStream()
        {
            return View();
        }

        [Route("tran-dau/{url}")]
        public async Task<ActionResult> LiveStreamDetail(string url)
        {
            if (string.IsNullOrEmpty(url)) return RedirectToAction("LiveStream");
            var host = ConfigurationManager.AppSettings["HostCrawlerUrl"];
            url = host.TrimEnd('/') + "/" + url.Replace("_", "/").TrimStart('/');

            var serv = new CrawlerServices();
            var match = await serv.GetLiveMatch(url);
            // match.Id = 
            var pattern = ".*-livetv(\\d*)";
            var grs = Regex.Match(url, pattern);
            if (grs != null && grs.Groups != null && grs.Groups.Count > 1)
            {
                match.Id = grs.Groups[1].Value;
            }

            return View(match);
        }        
                
        public ActionResult TrustedHost()
        {
            return View();
        }

        public ActionResult TrustedHostv2()
        {
            var model = GetModel();

            return View(model.GetView(), model);
        }
    }
}
