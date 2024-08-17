using newsWebapp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace newsWebapp.Controllers
{
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {
        newswebappEntities db = new newswebappEntities();

        [Route("~/")]
        [Route("~/Index")]
        public ActionResult Index()
        {
            ViewBag.newsCount = db.news.Count();
            ViewBag.views = db.viewlogs.Where(v => DbFunctions.TruncateTime(v.viewdate) == DateTime.Today).Count();
            return View();
        }

        [Route("~/Contact")]
        public ActionResult Contact()
        {
            return View();
        }
    }
}