using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_project.Models;

namespace MVC_project.Controllers
{
    public class LoginController : Controller
    {
        private string regularUser = "user";
        CinemaEntities db = new CinemaEntities();
        // GET: Home
        [HttpGet]
        public ActionResult Login()
        {
            USER usr = new USER();
            bool flag = true;
            ViewBag.flag = flag;
            return View(usr);
        }
        [HttpPost]
        public ActionResult Login(USER user)
        {
           
           
            var myUser = db.USER
                       .FirstOrDefault(x => x.Username == user.Username
                       && x.Password == user.Password );
            if (myUser != null)
            {
                user.Permission =(from x in db.USER
                                   where x.Username == user.Username && x.Password == user.Password
                                   select x.Permission).Single();
                return RedirectToAction("MovieGallery", "Home",user);
            }
            else
            {
                bool flag = false;
                ViewBag.flag = flag;
                return View();
            }
        }
        public ActionResult Registration()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Registration(USER user)
        {

            var myUser = db.USER
                       .FirstOrDefault(x => x.Username == user.Username);
            if (myUser == null)
            {
                user.Permission = regularUser;
                db.USER.Add(user);
                db.SaveChanges();
            }
            else
            {
                bool flag = false;
                ViewBag.flag = flag;
                return View();
            }

            return RedirectToAction("CurrentMovieGallery", "Home");
        }
    }
}