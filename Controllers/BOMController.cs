using Admin.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class BOMController : Controller
    {
        // GET: BOM
        static int i = 2;
        public ActionResult BOM_Add_Data()
        {
            if (Table_Data_List.Count == 0)
            {
                List<BOMFields> itemQm = new List<BOMFields>();
                itemQm.Add(new BOMFields { SP_Description = "" });
                ViewBag.item = itemQm;
                return View();
            }
            else if (Table_Data_List.Count >= 3)
            {
                ViewBag.item = Table_Data_List;
                i++;
                return View();
            }
            else if (Table_Data_List.Count ==2)
            {
                ViewBag.item = Table_Data_List;
                return View();
            }
            else
            {
                ViewBag.item = Table_Data_List;
                return View();
            }
        }
        [HttpPost]
        public ActionResult Main(BOMFields new_data)
        {
            BOM_Insert dblogin = new BOM_Insert();
            string values = dblogin.P_Description(new_data.SP_Part_No);
            string Descp = dblogin.SP_Description(new_data.Part_No);
            if (values == null)
            {
                ViewBag.ErrorMessage = "Part_No is not Found";
                return View("BOM_Add_Data"); 
            }
            else
            {
                Record(new_data.Part_No, Descp, new_data.Quantity, values, new_data.SP_Part_No);
                BOM_Add_Data();
                return View("BOM_Add_Data");
            }
            
        }

        static List<BOMFields> Table_Data_List = new List<BOMFields>();
        public static List<BOMFields> Record(string tbl_part_no, string tbl_Descp, string tbl_Quan, string SP_Descp, string SP_Part_No)
        {
            Table_Data_List.Add(new BOMFields { Part_No1 = tbl_part_no, Description1 = tbl_Descp, Quantity1 = tbl_Quan, SP_Description = SP_Descp, SP_Part_No = SP_Part_No });
            return (Table_Data_List);
        }
        [HttpPost]
        public ActionResult Order(BOMFields name)
        {
            BOM_Insert BOM_SP = new BOM_Insert();
            BOM_SP.AddOrderDetails(Table_Data_List);
            ViewBag.BOM = "Submitted Successfully !!!!";
            Table_Data_List.Clear();
            BOM_Add_Data();
            return View("BOM_Add_Data");
        }

        public ActionResult Partno_to_Descp (BOMFields name)
        {
            BOM_Insert dblogin = new BOM_Insert();
            string Descp = dblogin.SP_Description(name.Part_to_Descp);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
    }
}