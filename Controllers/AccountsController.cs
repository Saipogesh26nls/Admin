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
    public class AccountsController : Controller
    {
        // GET: Accounts
        public ActionResult Accounts_Detail() // Accounts Detail View
        {
            if (Session["userID"] != null)
            {
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                SqlDataAdapter _da = new SqlDataAdapter("Select * From Account_Master where A_Level>0", _con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.ProductList = ToSelectList(_dt, "A_Name", "A_Name");
                return View();
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
        [NonAction]
        public SelectList ToSelectList(DataTable table, string textField, string valueField) //  For making Dropdown list
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
        public ActionResult Upload_Data(AccountsField name) // For Adding data to DB
        {
            AccountInsert dblogin = new AccountInsert();
            int userid = dblogin.Add_Data(name.A_Account_Name, name.A_Group, name.A_Door_No, name.A_Street, name.A_Area, name.A_City, name.A_State, name.A_Country, name.A_Pincode, name.A_Contact_No, name.A_Mobile_No, name.A_Email_Id, name.A_Closing_Bal, name.A_Open_Bal);
            return Json(name);
        }
    }
}