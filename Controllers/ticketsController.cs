using newsWebapp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace newsWebapp.Controllers
{
    public class ticketsController : Controller
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
        // GET: tickets
        public ActionResult Index()
        {

            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            string userrole = Convert.ToString(Session["Userrole"]);
            if (userrole.Contains("userManagment"))
            {
                ViewBag.role = userrole.Contains("tag&catManagment") ? 1 : 0;
                var models = (from tickets in db.tickets 
                              join user in db.users on tickets.senderid equals user.ID into ticketsgroup 
                              from user in ticketsgroup.DefaultIfEmpty() // Left outer join
                              where tickets != null 
                              select new ticketlist
                              {
                                  username = user != null ? user.usename : null,
                                  ticketID = tickets.ID, 
                                  ticketTitle = tickets.title,
                                  priority = tickets.priority,
                                  status = tickets.status
                              }).OrderByDescending(p => p.priority).Where(s => s.status == 1 || s.status == 3).ToList();


                return View(models);
            }
            else
            {
                int uid = Convert.ToInt32(Session["Userid"]);
                var models = (from user in db.users // Access Users DbSet
                              join tickets in db.tickets on user.ID equals tickets.senderid into ticketsgroup // Join News DbSet
                              from tickets in ticketsgroup.DefaultIfEmpty() // Left outer join
                              where tickets != null && tickets.senderid == uid// Filter based on existence of news record
                              select new ticketlist
                              {
                                  username = user != null ? user.usename : null,
                                  ticketID = tickets.ID, // Use null-conditional operator for missing news
                                  ticketTitle = tickets.title,
                                  priority = tickets.priority,
                                  status = tickets.status
                              }).OrderByDescending(p => p.priority).ToList();


                return View(models);
            }

            
            
        }

        // GET: tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            string userrole = Convert.ToString(Session["Userrole"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = db.tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: tickets/Create
        public ActionResult Create()
        {

            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            string userrole = Convert.ToString(Session["Userrole"]);

                return View();
            

        }

        // POST: tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,title,content,priority,senderid,status,responsid")] ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.responsid = 0;
                ticket.senderid = Convert.ToInt32(Session["Userid"]);
                ticket.status = 1;

                db.tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticket);
        }

        // GET: tickets/Edit/5
        public ActionResult UserResponse(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = db.tickets.Find(id);
            var res = db.responses.Find(ticket.responsid);
            ViewBag.response = res;
            var admin = db.users.Find(res.adminid);
            var sender = db.users.Find(ticket.senderid);
            ViewBag.admin = admin != null ? admin.usename : "deleted user";
            ViewBag.sender = sender != null ? sender.usename : "deleted user";
            string userrole = Convert.ToString(Session["Userrole"]);
            ViewBag.role = userrole.Contains("userManagment") ? 1 : 0;

            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserResponse([Bind(Include = "ID,title,content,priority,senderid,status,responsid")] ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var currentTicket = db.tickets.Find(ticket.ID);
                currentTicket.content += "/" + ticket.content;
                currentTicket.status = 3;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        // GET: responses/Edit/5
        public ActionResult Respond(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ticket = db.tickets.Find(id);
            ViewBag.targetTicket = ticket;

            if (db.responses.Find(ticket.responsid) != null)
            {
                var res = db.responses.Find(ticket.responsid);
                ViewBag.response = res;
                var admin = db.users.Find(res.adminid);
                ViewBag.admin = admin != null ? admin.usename : "deleted user";
            }
            var sender = db.users.Find(ticket.senderid);
            ViewBag.sender = sender != null ? sender.usename : "deleted user";
            string userrole = Convert.ToString(Session["Userrole"]);
            ViewBag.role = userrole.Contains("userManagment") ? 1 : 0;

            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: responses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Respond([Bind(Include = "ID,title,content,adminid,ticketid")] response response)
        {
            if (ModelState.IsValid)
            {

                response.adminid = Convert.ToInt32(Session["Userid"]);
                var targetTicket = db.tickets.Find(response.ticketid);

                if (db.responses.Find(response.ID)== null)
                {
                    db.responses.Add(response);
                    targetTicket.status = 2;
                    targetTicket.responsid = response.ID;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }
                else
                {
                    var res = db.responses.Find(response.ID);
                    res.content += "\n" + response.content;
                    targetTicket.status = 2;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }



                return RedirectToAction("Index");
            }
            return View(response);
        }



        // GET: tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = db.tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ticket ticket = db.tickets.Find(id);
            db.tickets.Remove(ticket);
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