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
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult ProductEntry()
        {
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            SqlDataAdapter _da = new SqlDataAdapter("Select * From Product_Master where P_Level>0", _con);
            DataTable _dt = new DataTable();
            _da.Fill(_dt);
            ViewBag.ProductList = ToSelectList(_dt,"P_Name","P_Name");
            SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Manufacturer_Details", _con);
            DataTable _dt1 = new DataTable();
            _da1.Fill(_dt1);
            ViewBag.MfdList = ToSelectList(_dt1, "M_Id", "M_Name");
            SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Manufacturer_Details", _con);
            DataTable _dt2 = new DataTable();
            _da2.Fill(_dt2);
            ViewBag.RegList = ToSelectList(_dt2, "M_Region", "M_Region");
            return View();
        }

        [NonAction]
        public SelectList ToSelectList(DataTable table, string valueField, string textField)
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

        [HttpPost]
        public ActionResult ProductEntry(ProductModel newuser)
        {
            ProductInsert dblogin = new ProductInsert();
            int userid;

            userid = dblogin.AddData(newuser.P_Name, newuser.P_Disp_Name, newuser.P_Manufacturer, newuser.P_Region, newuser.P_Part_No, newuser.P_Description, newuser.P_Cost, newuser.P_MRP, newuser.P_SP);
            Session["P_Id"] = userid;
            newuser.Reg_Success = "Registered Successfully !!!!";
            ProductEntry();
            return View("ProductEntry", newuser);
        }
    }
}