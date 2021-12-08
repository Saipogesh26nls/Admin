using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;

namespace Admin.Controllers
{
    public class BOMController : Controller
    {
        // GET: BOM
        public ActionResult BOM_Add_Data()
        {
            return View();
        }

        [HttpPost]
        public ActionResult P_Description(BOMFields newuser)
        {
            BOM_Insert dblogin = new BOM_Insert();
            var values = dblogin.P_Description( newuser.SP_Part_No);
            ViewBag.ItemQ = values;
            BOM_Add_Data();
            return View("BOM_Add_Data", ViewBag.ItemQ);
        }
    }
}