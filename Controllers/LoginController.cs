using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;

namespace Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            if (Session["userID"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("ProductEntry", "Product");
            }
        } // Login View
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authorise(LoginModel fldLogin)
        {
            LoginField dblogin = new LoginField();
            var LoginD = dblogin.RtnLogins(fldLogin.UserName, fldLogin.Password);

            if (LoginD != null)
            {
                Session["userID"] = LoginD.UserId;
                Session["userName"] = LoginD.UserName;
                Session["roll"] = LoginD.Roll;
                Session["displayname"] = LoginD.Display_name;
                Session["View"] = LoginD.View;
                Session["Add"] = LoginD.Add;
                Session["Edit"] = LoginD.Edit;
                Session["Delete"] = LoginD.Delete;
                Session["Disable"] = LoginD.Disable;
                var numbers = LoginD.Menu.Split(',').Select(Int32.Parse).ToList();
                Session["Menu"] = numbers;
                dblogin.AddLogUser(LoginD.UserId, 1);
                var roll = Convert.ToInt32(Session["roll"]);
                var view = Convert.ToInt32(Session["View"]);
                var add = Convert.ToInt32(Session["Add"]);
                var edit = Convert.ToInt32(Session["Edit"]);
                var delete = Convert.ToInt32(Session["Delete"]);
                return RedirectToAction("Initial_Screen", "Product");
            }
            else
            {
                fldLogin.LoginErr = "Invalid username or password";
                return View("Login", fldLogin);
            }
        } // Authorise the fields with db
        public ActionResult Logout()
        {
            LoginField dbLogin = new LoginField();
            dbLogin.AddLogUser(Session["userID"].ToString(), 2);
            Session.Abandon();
            Session["roll"] = null;
            Session["userID"] = null;
            return RedirectToAction("Login", "Login");
        } // Logout and show Login View
        public ActionResult Err()
        {
            return View();
        } // Error View
        public ActionResult Signup()
        {
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {
                LoginField loginField = new LoginField();
                var List = loginField.List();
                return View(List);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        } // Users List View
        public ActionResult CreateUser()
        {
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                SqlDataAdapter _da = new SqlDataAdapter("Select * From Roll", _con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.LROLL = ToSelectList(_dt, "Id", "Roll");
                LoginField newPurchase_Insert = new LoginField();
                DataSet PM_Data = newPurchase_Insert.EditPurchase();
                ViewBag.PL = PM_Data.Tables[0];
                _con.Close();
                return View();
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        } // Create new user View
        [HttpPost]
        public ActionResult Add_UserData(List<Createuser> name)
        {
            LoginField newLoginField = new LoginField();
            newLoginField.Create_user(name);
            return Json(name);
        } // Add User data to db
        public ActionResult ChangePassword()
        {
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll > 1)
            {
                SignupModel loginModels = new SignupModel();
                var userid = Convert.ToInt32(Session["userID"]);
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                Con.Open();
                string cmd = "select * from Users where user_id = " + userid + "";
                SqlCommand SqlCmd = new SqlCommand(cmd, Con);
                SqlDataReader dr = SqlCmd.ExecuteReader();
                while (dr.Read())
                {
                    int view = (int)dr["View_Permission"];
                    int add = (int)dr["Add_Permission"];
                    int edit = (int)dr["Edit_Permission"];
                    int delete = (int)dr["Delete_Permission"];
                    bool view_val = false;
                    bool add_val = false;
                    bool edit_val = false;
                    bool delete_val = false;
                    if (view > 0)
                    {
                        view_val = true;
                    }
                    if (add > 0)
                    {
                        add_val = true;
                    }
                    if (edit > 0)
                    {
                        edit_val = true;
                    }
                    if (delete > 0)
                    {
                        delete_val = true;
                    }
                    loginModels.Id = (int)dr["user_id"];
                    loginModels.DisplayName = dr["Display_name"].ToString();
                    loginModels.UserName = dr["username"].ToString();
                    loginModels.Password = dr["password"].ToString();
                    loginModels.Roll = (int)dr["Roll"];
                    loginModels.View = view_val;
                    loginModels.Add = add_val;
                    loginModels.Edit = edit_val;
                    loginModels.Delete = delete_val;
                }
                dr.Close();
                Con.Close();
                SqlDataAdapter _da = new SqlDataAdapter("Select * From Roll", Con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.LROLL = ToSelectList(_dt, "Id", "Roll");
                return View(loginModels);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        } // ChangePassword View
        public ActionResult EditUser(int id)
        {
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            List<Createuser> createusers = new List<Createuser>();
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {
                LoginField loginField = new LoginField();
                DataSet Data = loginField.EditUser(id);
                Data.Tables[0].Columns.Add("View");
                Data.Tables[0].Columns.Add("Add");
                Data.Tables[0].Columns.Add("Edit");
                Data.Tables[0].Columns.Add("Delete");
                Data.Tables[0].Columns.Add("Disable_val");
                Data.Tables[0].Columns.Add("Menu");
                for (int i = 0; i < Data.Tables[0].Rows.Count; i++)
                {
                    bool view = false;
                    bool add = false;
                    bool edit = false;
                    bool delete = false;
                    bool disable = false;
                    if ((int)Data.Tables[0].Rows[i]["View_Permission"] == 1)
                    {
                        view = true;
                    }
                    if ((int)Data.Tables[0].Rows[i]["Add_Permission"] == 1)
                    {
                        add = true;
                    }
                    if ((int)Data.Tables[0].Rows[i]["Edit_Permission"] == 1)
                    {
                        edit = true;
                    }
                    if ((int)Data.Tables[0].Rows[i]["Delete_Permission"] == 1)
                    {
                        delete = true;
                    }
                    if ((int)Data.Tables[0].Rows[i]["Disable"] == 1)
                    {
                        disable = true;
                        view = false;
                        add = false;
                        edit = false;
                        delete = false;
                    }
                    Data.Tables[0].Rows[i]["View"] = view;
                    Data.Tables[0].Rows[i]["Add"] = add;
                    Data.Tables[0].Rows[i]["Edit"] = edit;
                    Data.Tables[0].Rows[i]["Delete"] = delete;
                    Data.Tables[0].Rows[i]["Disable_val"] = disable;
                    string cmd1 = "select Menu from Menu where Id = '" + (int)Data.Tables[0].Rows[i]["Menu_Id"] + "'";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, _con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        Data.Tables[0].Rows[i]["Menu"] = dr["Menu"].ToString();
                    }
                    dr.Close();
                    createusers.Add(new Createuser
                    {
                        View = view,
                        Add = add,
                        Edit = edit,
                        Delete = delete,
                        Disable_Enable = disable
                    }
                    );
                }
                ViewBag.PL = Data.Tables[0];
                SignupModel signupModel = new SignupModel();
                signupModel.DisplayName = Data.Tables[0].Rows[0]["Display_name"].ToString();
                signupModel.UserName = Data.Tables[0].Rows[0]["username"].ToString();
                signupModel.Password = Data.Tables[0].Rows[0]["password"].ToString();
                signupModel.Roll = (int)Data.Tables[0].Rows[0]["Roll"];
                signupModel.Id = id;
                SqlDataAdapter _da = new SqlDataAdapter("Select * From Roll", _con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.LROLL = ToSelectList(_dt, "Id", "Roll");
                ViewBag.Boolean = createusers;
                _con.Close();
                return View(signupModel);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        } // Edit User View
        [HttpPost]
        public ActionResult UpdateUser(List<Createuser> name)
        {
            LoginField newLoginField = new LoginField();
            newLoginField.Edit_user(name);
            return Json(name);
        } // Add Updated user data to db
        public ActionResult DeleteUser(int id)
        {
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            List<Createuser> createusers = new List<Createuser>();
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {
                LoginField loginField = new LoginField();
                DataSet Data = loginField.EditUser(id);
                Data.Tables[0].Columns.Add("View");
                Data.Tables[0].Columns.Add("Add");
                Data.Tables[0].Columns.Add("Edit");
                Data.Tables[0].Columns.Add("Delete");
                Data.Tables[0].Columns.Add("Disable_val");
                Data.Tables[0].Columns.Add("Menu");
                for (int i = 0; i < Data.Tables[0].Rows.Count; i++)
                {
                    bool view = false;
                    bool add = false;
                    bool edit = false;
                    bool delete = false;
                    bool disable = false;
                    if ((int)Data.Tables[0].Rows[i]["View_Permission"] == 1)
                    {
                        view = true;
                    }
                    if ((int)Data.Tables[0].Rows[i]["Add_Permission"] == 1)
                    {
                        add = true;
                    }
                    if ((int)Data.Tables[0].Rows[i]["Edit_Permission"] == 1)
                    {
                        edit = true;
                    }
                    if ((int)Data.Tables[0].Rows[i]["Delete_Permission"] == 1)
                    {
                        delete = true;
                    }
                    if ((int)Data.Tables[0].Rows[i]["Disable"] == 1)
                    {
                        disable = true;
                        view = false;
                        add = false;
                        edit = false;
                        delete = false;
                    }
                    Data.Tables[0].Rows[i]["View"] = view;
                    Data.Tables[0].Rows[i]["Add"] = add;
                    Data.Tables[0].Rows[i]["Edit"] = edit;
                    Data.Tables[0].Rows[i]["Delete"] = delete;
                    Data.Tables[0].Rows[i]["Disable_val"] = disable;
                    string cmd1 = "select Menu from Menu where Id = '" + (int)Data.Tables[0].Rows[i]["Menu_Id"] + "'";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, _con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        Data.Tables[0].Rows[i]["Menu"] = dr["Menu"].ToString();
                    }
                    dr.Close();
                    createusers.Add(new Createuser
                    {
                        View = view,
                        Add = add,
                        Edit = edit,
                        Delete = delete,
                        Disable_Enable = disable
                    }
                    );
                }
                ViewBag.PL = Data.Tables[0];
                SignupModel signupModel = new SignupModel();
                signupModel.DisplayName = Data.Tables[0].Rows[0]["Display_name"].ToString();
                signupModel.UserName = Data.Tables[0].Rows[0]["username"].ToString();
                signupModel.Password = Data.Tables[0].Rows[0]["password"].ToString();
                signupModel.Roll = (int)Data.Tables[0].Rows[0]["Roll"];
                signupModel.Id = id;
                SqlDataAdapter _da = new SqlDataAdapter("Select * From Roll", _con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.LROLL = ToSelectList(_dt, "Id", "Roll");
                ViewBag.Boolean = createusers;
                _con.Close();
                return View(signupModel);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        } // Delete user view
        [HttpPost]
        public ActionResult Deleteuserdata(SignupModel name)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd = "Delete from Users where user_id = "+name.Id+"";
            SqlCommand SqlCmd = new SqlCommand(cmd, Con);
            SqlCmd.ExecuteNonQuery();
            Con.Close();
            return Json(name);
        } // Delete user data from db
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