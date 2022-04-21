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
    public class GroupController : Controller
    {
        [HttpGet]
        public ActionResult GroupEntry() // Group Entry View
        {
            if (Session["userID"] != null)
            {
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                SqlDataAdapter _da = new SqlDataAdapter("Select * From Product_Master where P_Level>0", _con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.GroupList = ToSelectList(_dt, "P_Name", "P_Name");

                return View();
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
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

        [HttpPost]
        public ActionResult Add_GroupEntry(GroupFields name) // Adding Data to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Disp_Name from Product_Master where P_Disp_Name = '" + name.P_Disp_Name + "' and P_Level > 0";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string ItemQm = string.Empty;
            while (dr.Read())
            {
                ItemQm = dr["P_Disp_Name"].ToString();
            }
            if(ItemQm != name.P_Disp_Name)
            {
                GroupInsert dblogin = new GroupInsert();
                int userid = dblogin.AddData(name.P_Name, name.P_Disp_Name, name.P_Manufacturer, name.P_Region, name.P_Part_No, name.P_Description, name.P_Cost, name.P_MRP, name.P_SP);
                return Json(ItemQm);
            }
            else
            {
                Con.Close();
                return Json(ItemQm);
            }
            
        }
    }
}