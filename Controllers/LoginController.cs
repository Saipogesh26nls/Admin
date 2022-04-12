using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authorise(LoginModel fldLogin)
        {

            LoginField dblogin = new LoginField();
            var LoginD = dblogin.RtnLogins(fldLogin.UserName, fldLogin.Password);


            if (LoginD != null)
            {
                Session["userID"] = LoginD.UserId;
                Session["userName"] = LoginD.UserName;
                Session["roll"] = LoginD.Roll;
                Session["displayname"] = LoginD.Display_name;
                dblogin.AddLogUser(LoginD.UserId, 1);
                return RedirectToAction("ProductEntry", "Product");

            }
            else
            {
                fldLogin.LoginErr = "Invalid username or password";
                return View("Login", fldLogin);
            }


        }

        public ActionResult Logout()
        {
            LoginField dbLogin = new LoginField();
            dbLogin.AddLogUser(Session["userid"].ToString(), 2);
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }
        public ActionResult Err()
        {
            return View();
        }
    }
}