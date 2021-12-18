using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;
using System.Xml.Linq;

namespace Admin.Controllers
{
    public class BOMController : Controller
    {
        // GET: BOM
        public ActionResult BOM_Add_Data()
        {
            if(_list.Count == 0)
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
        public ActionResult Main(BOMFields new_data)
        {
            BOM_Insert dblogin = new BOM_Insert();
            string values = dblogin.P_Description(new_data.SP_Part_No);
            Record(new_data.Part_No, new_data.Description, new_data.Quantity, values );
            BOM_Add_Data();
            return View("BOM_Add_Data");
        }

        static List<BOMFields> _list = new List<BOMFields>();
        public static List<BOMFields> Record(string tbl_part_no, string tbl_Descp, string tbl_Quan, string SP_Descp)
        {
            _list.Add(new BOMFields { Part_No1 = tbl_part_no, Description1 = tbl_Descp, Quantity1 = tbl_Quan, SP_Description = SP_Descp });
            return (_list);
        }

    }
}