using newsWebapp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace newsWebapp.Controllers
{
    [RoutePrefix("Home")]   
    public class HomeController : Controller
    {

        newswebappEntities db = new newswebappEntities();
        public int checkLogin(int ? sessionid)
        {
            if (sessionid != null && sessionid != 0)
            {
                ViewBag.user = db.users.Find(sessionid).displayname;
                ViewBag.userrole = db.users.Find(sessionid).role;
                return 1;
            }
            else
            {
                return 0;
            }
        }


        [Route("~/")]
        [Route("~/Index")]
        public ActionResult Index()
        { 
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }          
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