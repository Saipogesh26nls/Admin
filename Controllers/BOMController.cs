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
            List<BOMFields> objOrder = new List<BOMFields> { new BOMFields { Part_No1 = "0", Description1 = "0", Quantity1 = "0" } };
            ViewBag.itemQm = 0;
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult P_Description(BOMFields newuser)
        {
            BOM_Insert dblogin = new BOM_Insert();
            var values = dblogin.P_Description(newuser.SP_Part_No);

            if (values.Count == 0)
            {
                ViewBag.ErrorMessage = "Part-No Not Found !!!";
                return View("BOM_Add_Data");
            }
            else
            {
                ViewBag.ItemQ = values.Single();
                return View("BOM_Add_Data", ViewBag.ItemQ);
            }
        }
        [HttpPost]
        public ActionResult Product_details(BOMFields new_data)
        {
            BOM_Insert dblogin = new BOM_Insert();
            var Part_Nos = dblogin.Table(new_data.Part_No, new_data.Description, new_data.Quantity);
            ViewBag.ItemA = Part_Nos.Single();
            return View("BOM_Add_Data", ViewBag.ItemA);
        }
    }
    /*static class ListTest
    {
        static List<string> _list;

        static ListTest()
        {
            _list = new List<string>();
        }
        public static void Record (string tbl_data)
        {
            _list.Add(tbl_data);
        }
    }
    class Program
    {
        [HttpPost]
        public static void Main(BOMFields new_data)
        {
            ListTest.Record(new_data.Part_No);
            ListTest.Record(new_data.Description);
            ListTest.Record(new_data.Quantity);
        }
    }*/
}