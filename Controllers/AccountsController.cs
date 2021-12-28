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
        public ActionResult Accounts_Detail()
        {
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            SqlDataAdapter _da = new SqlDataAdapter("Select * From Account_Master where A_Level>0", _con);
            DataTable _dt = new DataTable();
            _da.Fill(_dt);
            ViewBag.ProductList = ToSelectList(_dt, "A_Name", "A_Name");
            return View();
        }
        [NonAction]
        public SelectList ToSelectList(DataTable table, string textField, string valueField)
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
        public ActionResult Upload_Data(AccountsField newuser)
        {
            AccountInsert dblogin = new AccountInsert();
            int userid;

            userid = dblogin.Add_Data(newuser.A_Account_Name, newuser.A_Group, newuser.A_Door_No, newuser.A_Street, newuser.A_Area, newuser.A_City, newuser.A_State, newuser.A_Country, newuser.A_Pincode, newuser.A_Contact_No, newuser.A_Mobile_No, newuser.A_Email_Id, newuser.A_Closing_Bal, newuser.A_Open_Bal);
            Session["P_Id"] = userid;
            newuser.Reg_Success = "Registered Successfully !!!!";
            Accounts_Detail();
            return View("Accounts_Detail", newuser);
        }
    }
}