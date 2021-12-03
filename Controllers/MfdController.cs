using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;

namespace Admin.Controllers
{
    public class MfdController : Controller
    {
        public ActionResult MfdEntry()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MfdEntry(MfdFields newuser)
        {
            Mfd_Details_Upload dblogin = new Mfd_Details_Upload();
            int userid;

            userid = dblogin.MfdUpload(newuser.M_Name, newuser.M_Country, newuser.M_Region, newuser.M_Address, newuser.M_Support_No, newuser.M_Contact_No, newuser.M_Sales_No, newuser.M_Website, newuser.M_Support_Email, newuser.M_Contact_Email, newuser.M_Sales_Email, newuser.M_Payment);
            Session["M_Name"] = userid;
            newuser.Regd_Success = "Registered Successfully !!!!";

            return View("MfdEntry", newuser);
        }
    }
}