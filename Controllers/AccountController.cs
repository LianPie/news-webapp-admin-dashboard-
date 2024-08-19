using newsWebapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace newsWebapp.Controllers
{
    public class AccountController : Controller
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

        // GET: users
        public ActionResult Index()
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }

            string userrole = ViewBag.userrole;
            if (userrole.Contains("addNews"))
            {
                @ViewBag.role = "jornalist";
            }
            if (userrole.Contains("newsManagement") || userrole.Contains("tag&catManagment"))
            {
                @ViewBag.role = "admin";
            }
            else
            {
                @ViewBag.role = "user";
            }

            if (Session["User"] != null)
            {
                if (Session["Userid"] == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                user user = db.users.Find(Session["Userid"]);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
                return RedirectToAction("Login");
        }
        
        // GET: users

        public ActionResult UserMnagment()
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            string userrole = Convert.ToString(Session["Userrole"]);
            if (userrole.Contains("userManagment"))
            {
                return View(db.users.ToList());
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }

        // GET: users/Edit/5
        public ActionResult EditAccount(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            user user = db.users.Find(Session["Userid"]);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
           

        // POST: users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount([Bind(Include = "ID,usename,password,displayname,role")] user user)
        {
            if (ModelState.IsValid)
            {
                var u = db.users.Find(user.ID);

                if (user.password != null)
                {
                    // Hash the password
                    using (var sha256 = SHA256.Create())
                    {
                        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.password));
                        user.password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                        u.password = user.password;
                    }
                }

                u.displayname = user.displayname;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }


        // GET: users/logout
        public ActionResult Logout()
        {
            //session check, if exists -> destroy session
            if (Session["User"] != null)
            {
                Session.Abandon();
            }
            return RedirectToAction("Index");
        }

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }


        // POST: users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "usename,password")] user user)
        {
            if (ModelState.IsValid)
            {
                //check if user exists
                var existingUser = db.users.FirstOrDefault(u => u.usename == user.usename);
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.password));
                    user.password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
                if (existingUser != null && existingUser.password == user.password)
                {
                    //session start
                    Session["User"] = existingUser.usename;
                    Session["Userid"] = existingUser.ID;
                    Session["Userrole"] = existingUser.role;

                    return RedirectToAction("Index","home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            return View(user);
        }


        public ActionResult Edituser(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            string userrole = Convert.ToString(Session["Userrole"]);
            if (userrole.Contains("userManagment"))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                user user = db.users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

        }

        // POST: tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edituser([Bind(Include = "ID,usename,password,displayname,role")] user user, List<string> roles)
        {
            if (ModelState.IsValid)
            {
                user.role = "";
                foreach (string r in roles)
                {
                    if (r != roles[0])
                    {
                        user.role += ",";
                        user.role += r;
                    }
                    else
                        user.role += r;
                }
                var currentuser = db.users.Find(user.ID);
                currentuser.displayname = user.displayname;
                currentuser.role = user.role;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        // GET: users/Delete/5
        public ActionResult DeleteAccount(int? id)
        {
            if (checkLogin(Convert.ToInt32(Session["Userid"])) != 1)
            {
                return RedirectToAction("Login", "Account");
            }
                if (Session["Userid"] == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                user user = db.users.Find(Session["Userid"]);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
        }

        // POST: users/Delete/5
        [HttpPost, ActionName("DeleteAccount")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        } // POST: users/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteuserConfirmed(int id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
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