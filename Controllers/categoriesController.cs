using newsWebapp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace newsWebapp.Controllers
{
    public class categoriesController : Controller
    {
        newswebappEntities db = new newswebappEntities();

        public int checkLogin(int? sessionid)
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
        // GET: categories
        public ActionResult Index()
        {

            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }

            string userrole = Convert.ToString(Session["Userrole"]);

            if (userrole.Contains("tag&catManagment"))
                ViewBag.role = 1;
            else ViewBag.role = 0;
            return View(db.categories.ToList());
        }

        // GET: categories/Details/5
        public ActionResult Details(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            string userrole = Convert.ToString(Session["Userrole"]);

            if (userrole.Contains("tag&catManagment"))
                ViewBag.role = 1;
            else ViewBag.role = 0;

            ViewBag.uid = Convert.ToInt32(Session["Userid"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.categories.Find(id);
            ViewBag.title = category.title;
            if (category == null)
            {
                return HttpNotFound();
            }
            //search for record that contains "cat title" in cat field.
            return View(db.news.Where(n => n.cat.Contains(category.title)));
        }

        // GET: categories/Create
        public ActionResult Create()
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            string userrole = Convert.ToString(Session["Userrole"]);

            if (userrole.Contains("tag&catManagment"))
            {
                    return View();
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
        }

        // POST: categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,link,title")] category category)
        {
            if (ModelState.IsValid)
            {
                category.link = "categories/"+category.title;
                db.categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            string userrole = Convert.ToString(Session["Userrole"]);

                if (userrole.Contains("tag&catManagment"))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    category category = db.categories.Find(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    return View(category);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            
        }

        // POST: categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,link,title")] category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            string userrole = Convert.ToString(Session["Userrole"]);

                if (userrole.Contains("tag&catManagment"))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    category category = db.categories.Find(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    return View(category);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            
        }

        // POST: categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            category category = db.categories.Find(id);
            db.categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
