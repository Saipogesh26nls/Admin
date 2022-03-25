using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Add_New_Employee()
        {
            return View();
        }
        public ActionResult Add_New_Project()
        {
            return View();
        }
    }
}