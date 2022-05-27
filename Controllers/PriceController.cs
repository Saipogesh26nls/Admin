using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using System.Net;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Admin.Controllers
{
    public class PriceController : Controller
    {
        public ActionResult Price_List() // PM List View
        {
            if (Session["userID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Err","Login");
            }
        }
        public ActionResult List_Update(Price_List name) // add DB dat to Price_List View
        {
            Goods_RI dblogin = new Goods_RI();
            var Descp = dblogin.PM_list(name.Package_letter, name.Value_letter, name.Partno_letter, name.Descp_letter);
            return Json(Descp);
        }
        public ActionResult Price_Insert(Price_List name) // Adding Data to DB
        {
            Price_Updation price_Updation = new Price_Updation();
            price_Updation.AddPrice(name.Part_No, name.P_Cost, name.P_Price_USD, name.P_MRP, name.P_SP, name.Stock);
            return Json(name);
        }
        public ActionResult Inventory_Flow() // Inventory Flow view
        {
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                SqlDataAdapter _da = new SqlDataAdapter("Select * From Project_Master", _con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.Project = ToSelectList(_dt, "Project_Id", "Project_Name");
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Process_Tag", _con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.Process = ToSelectList(_dt1, "Process_Id", "Process_Name");
                SqlDataAdapter _da3 = new SqlDataAdapter("Select * From Employee_Master", _con);
                DataTable _dt3 = new DataTable();
                _da3.Fill(_dt3);
                ViewBag.Employee = ToSelectList(_dt3, "Id", "Employee_Name");
                _con.Close();
                return View();
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
        public ActionResult Project_update_list(Inventory name) // add DB data to List View
        {
            Price_Updation dblogin = new Price_Updation();
            var list = dblogin.Inventory_List(name.Product, name.Project, name.Process, name.User);
            return Json(list);
        }
        [NonAction]
        public SelectList ToSelectList(DataTable table, string valueField, string textField) // For making Dropdown list
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(new SelectListItem()
                {
                    Text = row[textField].ToString(),
                    Value = row[valueField].ToString()
                });
            }
            return new SelectList(list, "Value", "Text");
        }
        public ActionResult Stock_Statement() // Stock Statement view
        {
            /*Price_Updation price_Updation = new Price_Updation();
            var data = price_Updation.Get_stocks();
            data.Tables[0].Columns.Add("Total");
            for (int i = 0; i < data.Tables[0].Rows.Count; i++)
            {
                var qty = data.Tables[0].Rows[i]["P_Closing_Balance"];
                var price = data.Tables[0].Rows[i]["P_Cost"];
                data.Tables[0].Rows[i]["Total"] = (int)qty * Convert.ToDouble(price);
            }
            ViewBag.Stocks = data.Tables[0];*/
            if (Session["userID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
        public ActionResult Data_Get(Value name) // add DB data to Price_List View
        {
            Goods_RI dblogin = new Goods_RI();
            var Descp = dblogin.PM_list(name.package, name.value, name.partno, name.description);
            return Json(Descp);
        }
        public ActionResult Purchase_Data(Inventory name) // get purchase data from DB
        {
            Price_Updation dblogin = new Price_Updation();
            var list = dblogin.Receipt_Data(name.Product);
            return Json(list);
        }  
        public ActionResult MaterialIndex() // Material Index view
        {
            if (Session["userID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
        public ActionResult Get_Partno_from_DB(Material_Index name) // Get PO and PV data from DB
        {
            Price_Updation price_Updation = new Price_Updation();
            var list = price_Updation.Material_data(name.Part_No);
            return Json(list);
        }
    }
}