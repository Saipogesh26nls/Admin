using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class IssueController : Controller
    {
        // GET: Issue
        public ActionResult StockIssue() // Stock Issue View
        {
            if (Session["userID"] != null)
            {
                GoodsRI Model = new GoodsRI();
                Model.Ref_Date = DateTime.Today;
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
                SqlDataAdapter _da2 = new SqlDataAdapter("Select * From GI_Tag", _con);
                DataTable _dt2 = new DataTable();
                _da2.Fill(_dt2);
                ViewBag.GI = ToSelectList(_dt2, "GI_Id", "TagName");
                SqlDataAdapter _da3 = new SqlDataAdapter("Select * From Employee_Master", _con);
                DataTable _dt3 = new DataTable();
                _da3.Fill(_dt3);
                ViewBag.Employee = ToSelectList(_dt3, "Id", "Employee_Name");
                List<SelectListItem> Index = new List<SelectListItem>();
                Index.Add(new SelectListItem { Text = "Goods-Receipt", Value = "1" });
                Index.Add(new SelectListItem { Text = "Goods-Issue", Value = "2" });
                ViewBag.Index = new SelectList(Index, "Value", "Text");
                SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
                DataTable _dt4 = new DataTable();
                _da4.Fill(_dt4);
                ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
                SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", _con);
                DataTable _dt5 = new DataTable();
                _da5.Fill(_dt5);
                ViewBag.MfdList = ToSelectList(_dt5, "A_code", "A_Name");
                string cmd1 = "Update Number_master Set Indent_No = Indent_No + 1 ";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, _con);
                SqlCmd1.ExecuteNonQuery();
                string cmd2 = "Select Indent_No from Number_master ";
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, _con);
                SqlDataReader dr = SqlCmd2.ExecuteReader();
                dr.Read();
                Model.Ref_No = dr["Indent_No"].ToString();
                dr.Close();
                _con.Close();
                return View(Model);
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
        public ActionResult P_to_DQ(GoodsRI name) // conversion of part_no to description, qty
        {
            Stock_Issue_Insert dblogin = new Stock_Issue_Insert();
            var Descp = dblogin.Issue_Descp(name.Part_No);
            name.Description = Descp.Description;
            Add_StockIssue(name);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Tab_P_to_DQ(GoodsRI name) // conversion of part_no to description, qty
        {
            Stock_Issue_Insert dblogin = new Stock_Issue_Insert();
            var Descp = dblogin.Issue_Descp_Qty(name.Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PM_List(GoodsRI name) // To show full Product Master stocks from DB
        {
            Goods_RI dblogin = new Goods_RI();
            var Descp = dblogin.PM_list(name.Package_letter, name.Value_letter, name.Partno_letter, name.Descp_letter);
            return Json(Descp);
        }
        public ActionResult Add_Product(GoodsRI name) // add new products to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Part_No from Product_Master where P_Part_No = '" + name.Add_PartNo + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string ItemQm = string.Empty;
            while (dr.Read())
            {
                ItemQm = dr["P_Part_No"].ToString();
            }
            if (ItemQm != name.Add_PartNo)
            {
                ProductInsert dblogin = new ProductInsert();
                int userid = dblogin.AddData(name.Add_Group, name.Add_Name, name.Add_Manufacturer, name.Add_Package, name.Add_Value, name.Add_PartNo, name.Add_Description, name.Add_Cost, name.Add_MRP, name.Add_SellPrice);
                Con.Close();
                return Json(ItemQm);
            }
            else
            {
                Con.Close();
                return Json(ItemQm);
            }

        }
        [HttpPost]
        public ActionResult Add_StockIssue(GoodsRI data) // Add Issue Stocks to DB
        {
            Stock_Issue_Insert dblogin = new Stock_Issue_Insert();
            dblogin.StockIssue_add(data);
            return Json(data);        
        } 
    }
}