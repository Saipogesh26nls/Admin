using Admin.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

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
                itemQm.Add(new BOMFields { Part_No1 = "", Description1 = "", Quantity1 = "" });
                ViewBag.item = itemQm;
                return View();
            }
            else
            {
                ViewBag.item = _list;
                return View();
            }
        }
        public ActionResult BOM_Clear_View()
        {
            _list.Clear();
            BOM_Add_Data();
            return View("BOM_Add_Data");
        }
        [HttpPost]
        public ActionResult Main(BOMFields new_data)
        {
            BOM_Insert dblogin = new BOM_Insert();
            string values = dblogin.P_Description(new_data.SP_Part_No);
            if(values == null)
            {
                ViewBag.ErrorMessage = "Part_No is not Found";
                return View("BOM_Add_Data"); 
            }
            else
            {
                Record(new_data.Part_No, new_data.Description, new_data.Quantity, values);
                BOM_Add_Data();
                return View("BOM_Add_Data");
            }
            
        }

        static List<BOMFields> _list = new List<BOMFields>();
        public static List<BOMFields> Record(string tbl_part_no, string tbl_Descp, string tbl_Quan, string SP_Descp)
        {
            _list.Add(new BOMFields { Part_No1 = tbl_part_no, Description1 = tbl_Descp, Quantity1 = tbl_Quan, SP_Description = SP_Descp });
            return (_list);
        }
        [HttpPost]
        public ActionResult Order()
        {
            BOM_Insert BOM_SP = new BOM_Insert(); 
            BOM_SP.AddOrderDetails(_list);
            ViewBag.BOM = "Submitted Successfully !!!!";
            _list.Clear();
            BOM_Add_Data();
            return View("BOM_Add_Data");
        }
    }
}