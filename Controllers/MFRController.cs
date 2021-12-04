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
    public class MFRController : Controller
    {
        public ActionResult MFREntry()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MFREntry(MFRFields newuser)
        {
            MFR_Details_Upload dblogin = new MFR_Details_Upload();
            int userid;

            userid = dblogin.MFRUpload(newuser.M_Name, newuser.M_Country, newuser.M_Region, newuser.M_Address, newuser.M_Support_No, newuser.M_Contact_No, newuser.M_Sales_No, newuser.M_Website, newuser.M_Support_Email, newuser.M_Contact_Email, newuser.M_Sales_Email, newuser.M_Payment);
            Session["M_Name"] = userid;
            newuser.Regd_Success = "Registered Successfully !!!!";

            return View("MFREntry", newuser);
        }
    }
}