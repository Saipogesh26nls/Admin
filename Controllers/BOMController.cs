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
        [ValidateAntiForgeryToken]
        public ActionResult P_Description(BOMFields newuser)
        {
            BOM_Insert dblogin = new BOM_Insert();
            var values = dblogin.P_Description( newuser.SP_Part_No);
            
            if (values.Count == 0)
            {
                ViewBag.ErrorMessage = "Part-No Not Found !!!";
                return View("BOM_Add_Data");
            }
            else
            {
                ViewBag.ItemQ = values.Single();
                BOM_Add_Data();
                return View("BOM_Add_Data", ViewBag.ItemQ);
            }
        }
        public ActionResult Product_details(BOMFields new_data)
        {
            var Part_Nos = (new_data.Part_No, new_data.Description, new_data.Quantity);            
            ViewBag.ItemA= Part_Nos;
            return View("BOM_Add_Data", ViewBag.ItemA);

        }
    }
}