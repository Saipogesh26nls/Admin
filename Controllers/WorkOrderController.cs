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
    public class WorkOrderController : Controller
    {
        // GET: WorkOrder
        public ActionResult Work_Order()
        {
            if (Session["userID"] != null)
            {
                Workorder work_Order = new Workorder();
                work_Order.WO_Date = DateTime.Today;
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Process_Tag", _con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.Process = ToSelectList(_dt1, "Process_Id", "Process_Name");
                List<SelectListItem> _list = new List<SelectListItem>();
                _list.Add(new SelectListItem { Text = "Inhouse", Value = "1" });
                _list.Add(new SelectListItem { Text = "Vendor", Value = "2" });
                ViewBag.MfrOpt = new SelectList(_list, "Value", "Text");
                SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", _con);
                DataTable _dt5 = new DataTable();
                _da5.Fill(_dt5);
                ViewBag.MfdList = ToSelectList(_dt5, "A_code", "A_Name");
                SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
                DataTable _dt4 = new DataTable();
                _da4.Fill(_dt4);
                ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
                SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Product_Master where P_Level < 0 and (P_Parent Like '0002400646%' or P_Parent Like '0002400654%')", _con);
                DataTable _dt2 = new DataTable();
                _da2.Fill(_dt2);
                ViewBag.Product = ToSelectList(_dt2, "P_Part_No", "P_Name");
                _con.Close();
                return View(work_Order);
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
            WO_Insert dblogin = new WO_Insert();
            var Descp = dblogin.Descp_Qty(name.Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add_WO_to_DB(List<Workorder> name)
        {
            WO_Insert wO_Insert = new WO_Insert();
            var id = Convert.ToInt32(Session["userID"]);
            var wono = wO_Insert.Add_WO(name, id);
            return Json(wono);
        }
        public ActionResult WorkOrder_list()
        {
            if (Session["userID"] != null)
            {
                WO_Insert wO_Insert=new WO_Insert();
                var list = wO_Insert.WO_List();
                return View(list);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
        public ActionResult WO_to_BOM(int wono, DateTime wodate, string bomno)
        {
            if (Session["userID"] != null)
            {
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Process_Tag", _con);
            DataTable _dt1 = new DataTable();
            _da1.Fill(_dt1);
            ViewBag.Process = ToSelectList(_dt1, "Process_Id", "Process_Name");
            List<SelectListItem> _list = new List<SelectListItem>();
            _list.Add(new SelectListItem { Text = "Inhouse", Value = "1" });
            _list.Add(new SelectListItem { Text = "Vendor", Value = "2" });
            ViewBag.MfrOpt = new SelectList(_list, "Value", "Text");
            SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", _con);
            DataTable _dt5 = new DataTable();
            _da5.Fill(_dt5);
            ViewBag.MfdList = ToSelectList(_dt5, "A_code", "A_Name");
            SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
            DataTable _dt4 = new DataTable();
            _da4.Fill(_dt4);
            ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
            SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Product_Master where P_Level < 0 and (P_Parent Like '0002400646%' or P_Parent Like '0002400654%')", _con);
            DataTable _dt2 = new DataTable();
            _da2.Fill(_dt2);
            ViewBag.Product = ToSelectList(_dt2, "P_Part_No", "P_Name");
            Workorder workorder = new Workorder();
            DataSet ds = workorder.WO_BOM(Convert.ToInt32(bomno));
            ds.Tables[0].Columns.Add("P_Part_No");
            ds.Tables[0].Columns.Add("P_Description");
            ds.Tables[0].Columns.Add("P_Closing_Bal");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string Text = ds.Tables[0].Rows[i]["SP_code"].ToString();
                NewPurchase_Insert dblogin = new NewPurchase_Insert();
                var Descp = dblogin.Pcode_to_PartNo(Text);
                ds.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                ds.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
                ds.Tables[0].Rows[i]["P_Closing_Bal"] = Descp[0].P_Close_Bal;
            }
            ViewBag.BOM = ds.Tables[0];
            workorder.WO_No = wono;
            workorder.WO_Date = wodate;
            workorder.BOM_No = bomno;
            string cmd1 = "select * from Work_Order where WO_No = " + wono + "";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, _con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ViewBag._Process = dr["Process"].ToString();
                workorder.Mfr = dr["Mfr"].ToString();
                workorder.Note = dr["Note"].ToString();
                workorder.Quantity = (int)dr["Quantity"];
                if (dr["Mfr_Option"].ToString() == "Inhouse")
                {
                    ViewBag._Mfr_Option = "1";
                }
                else
                {
                    ViewBag._Mfr_Option = "2";
                }
                workorder.Product = dr["P_code"].ToString();
            }
            dr.Close();
            string cmd2 = "select P_Part_No from Product_Master where P_code = '" + workorder.Product + "'";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, _con);
            SqlDataReader dr2 = SqlCmd2.ExecuteReader();
            while (dr2.Read())
            {
                workorder.Product = dr2["P_Part_No"].ToString();
            }
            dr2.Close();
            string cmd3 = "select A_Name from Account_Master where A_code = '" + workorder.Mfr + "'";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, _con);
            SqlDataReader dr3 = SqlCmd3.ExecuteReader();
            while (dr3.Read())
            {
                workorder.Mfr = dr3["A_Name"].ToString();
            }
            dr3.Close();
            _con.Close();
            return View(workorder);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
    }
}