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
            if(_list == null)
            {
                List<BOMFields> itemQm = new List<BOMFields>();
                itemQm.Add(new BOMFields { Part_No1 = "0", Description1 = "0", Quantity1 = "0" });
                ViewBag.item = itemQm;
                return View();
            }
            else
            {
                ViewBag.item = _list;
                return View();
            }
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
                return View("BOM_Add_Data");
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
        [HttpPost]
        public ActionResult Main(BOMFields new_data)
        {
            Record(new_data.Part_No, new_data.Description, new_data.Quantity);
            BOM_Add_Data();
            return View("BOM_Add_Data");
        }
        static List<BOMFields> _list = new List<BOMFields>();
        public static List<BOMFields> Record(string tbl_part_no, string tbl_Descp, string tbl_Quan)
        {
            _list.Add(new BOMFields { Part_No1 = tbl_part_no, Description1 = tbl_Descp, Quantity1 = tbl_Quan });
            return (_list);
        }
    }
}