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
        public string Order()
        {
            AddOrderDetails(_list);
            return ("Hello");
        }
        [HttpPost]
        public string AddOrderDetails(List<BOMFields> orderDetail)
        {
            //Converting List to XML using LINQ to XML    
            // the xml doc will get stored into OrderDetails object of XDocument    
            XDocument OrderDetails = new XDocument(new XDeclaration("1.0", "UTF - 8", "yes"),
            new XElement("OrderDetail",
            from OrderDet in orderDetail
            select new XElement("OrderDetails",
            new XElement("MP_Description", OrderDet.SP_Description),
            new XElement("Part_No", OrderDet.Part_No1),
            new XElement("Description", OrderDet.Description1),
            new XElement("Quantity", OrderDet.Quantity1))));

            DB_Con_Str OCon = new DB_Con_Str();
            string ConString = OCon.DB_Data();
            SqlConnection Con = new SqlConnection(ConString);
            Con.Open();

            SqlCommand sql_cmnd = new SqlCommand("[dbo].[BOM_Prod]", Con);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@xml", OrderDetails);
            sql_cmnd.ExecuteNonQuery();
            return ("Hi");
        }
    }
}