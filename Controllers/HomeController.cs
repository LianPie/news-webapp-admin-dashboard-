using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace newsWebapp.Controllers
{
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {
        [Route("~/")]
        [Route("~/Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("~/Contact")]
        public ActionResult Contact()
        {
            return View();
        }
    }
}