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
            _con.Close();
            return View(work_Order);
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

    }
}