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

    }
}