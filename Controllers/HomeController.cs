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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult ProductEntry()
        {
            DB_Con_Str OCon = new DB_Con_Str();
            string ConString = OCon.DB_Data();
            SqlConnection _con = new SqlConnection(ConString);
            _con.Open();
            SqlDataAdapter _da = new SqlDataAdapter("Select * From Product_Master where P_Level>0", ConString);
            DataTable _dt = new DataTable();
            _da.Fill(_dt);
            ViewBag.ProductList = ToSelectList(_dt,"P_Name","P_Name");

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

            userid = dblogin.AddData(newuser.P_Name, newuser.P_Disp_Name, newuser.P_Part_No, newuser.P_Description, newuser.P_Cost, newuser.P_MRP, newuser.P_SP);
            Session["P_Name"] = userid;
            newuser.Reg_Success = "Registered Successfully !!!!";

            return View("ProductEntry", newuser);
        }

        /*[HttpPost]
        public ActionResult AddData(ProductModel newuser)
        {
            ProductInsert dblogin = new ProductInsert();
            int userid;

            userid = dblogin.AddData(newuser .P_Name, newuser. P_Disp_Name, newuser .P_Part_No, newuser. P_Description, newuser .P_Cost, newuser. P_MRP, newuser. P_SP);
            Session["Pid"] = userid;
            newuser.Reg_Success = "Registered Successfully !!!!";

            return View("ProductEntry", newuser);
    }*/

    }
}