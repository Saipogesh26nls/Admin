using Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
            if (Session["roll"] != null)
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
        public ActionResult Add_StockIssue(List<Issue> data) // Add Issue Stocks to DB
        {
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            string cmd1 = "Update Number_master Set Indent_No = Indent_No + 1 ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, _con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "Select Indent_No from Number_master ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, _con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            dr.Read();
            data[0].IndentNo = dr["Indent_No"].ToString();
            dr.Close();
            _con.Close();
            Stock_Issue_Insert dblogin = new Stock_Issue_Insert();
            dblogin.StockIssue_add(data);
            return Json(data);        
        }
        [HttpGet]
        public ActionResult IndentList() // To show full Indent list view
        {
            if (Session["userID"] != null)
            {
                Stock_Issue_Insert newPurchase_Insert = new Stock_Issue_Insert();
                var PM_Data = newPurchase_Insert.Indent_List();
                ViewBag.PL = PM_Data;
                return View(PM_Data);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
        [HttpGet]
        public ActionResult Indent_to_GoodsIssue(string Indent)
        {
            if (Session["userID"] != null)
            {
                GoodsRI Model = new GoodsRI();
                Model.Voucher_Date = DateTime.Today;
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
                DataSet set = Model.SelectIndent(Indent);
                set.Tables[0].Columns.Add("P_code");
                for (int i = 0; i < set.Tables[0].Rows.Count; i++)
                {
                    string Text = set.Tables[0].Rows[i]["PartNo"].ToString();
                    Stock_Issue_Insert dblogin = new Stock_Issue_Insert();
                    var Descp = dblogin.partno_to_pcode(Text);
                    set.Tables[0].Rows[i]["P_code"] = Descp;
                }
                ViewBag.Goods = set.Tables[0];
                Model.Ref_No = set.Tables[0].Rows[0]["IndentNo"].ToString();
                Model.Ref_Date = DateTime.Parse(set.Tables[0].Rows[0]["IndentDate"].ToString());
                ViewBag.Reason_int = set.Tables[0].Rows[0]["GI_value"].ToString();
                ViewBag.Process_int = set.Tables[0].Rows[0]["Process_value"].ToString();
                ViewBag.Project_int = set.Tables[0].Rows[0]["Project_value"].ToString();
                ViewBag.Request_int = set.Tables[0].Rows[0]["Request_value"].ToString();
                Model.GI_Tag = set.Tables[0].Rows[0]["GI_value"].ToString();
                Model.Process_Tag = set.Tables[0].Rows[0]["Process_value"].ToString();
                Model.Project = set.Tables[0].Rows[0]["Project_value"].ToString();
                Model.Employee = set.Tables[0].Rows[0]["Request_value"].ToString();
                Model.Note = set.Tables[0].Rows[0]["Note"].ToString();
                Model.Index_Type = 2;
                string cmd3 = "select GI_VoucherNo from Stock_Indent where IndentNo = '" + Indent + "'";
                SqlCommand SqlCmd3 = new SqlCommand(cmd3, _con);
                SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                int vno = 0;
                while (dr2.Read())
                {
                    vno = (int)dr2["GI_VoucherNo"];
                }
                dr2.Close();
                if(vno > 0)
                {
                    Model.Voucher_No = vno;
                }
                else
                {
                    Model.Voucher_No = (int)set.Tables[0].Rows[0]["GI_VoucherNo"];
                }
                _con.Close();
                return View(Model);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        } // Transfer data from StockIssue to GoodsIssue
        [HttpPost]
        public JsonResult Add_Goods(List<GoodsRI> data) // For Adding Goods to DB - Json
        {
            DateTime v = DateTime.Parse(data[0].V_Date);
            string v_date = v.ToString("yyyy-MM-dd");
            DateTime r = DateTime.Parse(data[0].R_Date);
            string r_date = r.ToString("yyyy-MM-dd");
            Stock_Issue_Insert dblogin = new Stock_Issue_Insert();
            int Vno = dblogin.Goods_add(data, v_date, r_date);
            return Json(Vno);
        }
    }
}