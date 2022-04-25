using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

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
                return RedirectToAction("ProductEntry", "Product");
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
            dbLogin.AddLogUser(Session["userid"].ToString(), 2);
            Session.Abandon();
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
                List<SignupModel> loginModels = new List<SignupModel>();
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                Con.Open();
                string cmd2 = "select * from Users";
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                SqlDataReader dr = SqlCmd2.ExecuteReader();
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
                    loginModels.Add(new SignupModel
                    {
                        Id = (int)dr["user_id"],
                        DisplayName = dr["Display_name"].ToString(),
                        UserName = dr["username"].ToString(),
                        Password = dr["password"].ToString(),
                        Roll = (int)dr["Roll"],
                        View = view_val,
                        Add = add_val,
                        Edit = edit_val,
                        Delete = delete_val
                    }
                    );
                }
                dr.Close();
                for(int i = 0; i < loginModels.Count; i++)
                {
                    string cmd3 = "select Roll from Roll where Id = '"+loginModels[i].Roll+"'";
                    SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
                    SqlDataReader dr1 = SqlCmd3.ExecuteReader();
                    while (dr1.Read())
                    {
                        loginModels[i].Roll_Name = dr1["Roll"].ToString();
                    }
                    dr1.Close();
                }
                Con.Close();
                return View(loginModels);
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
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd = "Insert into Users(Display_name, username, password, Roll, View_Permission, Add_Permission, Edit_Permission, Delete_Permission) values ('" + name[0].displayname + "','" + name[0].username + "','" + name[0].password + "','" + name[0].roll + "','" +name[0].View+ "','"+name[0].Add+ "','"+name[0].Edit+ "','"+name[0].Delete+ "')";
            SqlCommand SqlCmd = new SqlCommand(cmd, Con);
            SqlCmd.ExecuteNonQuery();
            Con.Close();
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
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {
                SignupModel loginModels = new SignupModel();
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                Con.Open();
                string cmd = "select * from Users where user_id = " + id + "";
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
                SqlDataAdapter _da = new SqlDataAdapter("Select * From Roll", Con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.LROLL = ToSelectList(_dt, "Id", "Roll");
                Con.Close();
                return View(loginModels);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        } // Edit User View
        [HttpPost]
        public ActionResult UpdateUser(SignupModel name)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd = "Update Users set Display_name = '" + name.DisplayName + "', username = '" + name.UserName + "', password = '" + name.Password + "', Roll = '" + name.Roll + "', View_Permission = '" + name.View_val + "', Add_Permission = '" + name.Add_val + "', Edit_Permission = '" + name.Edit_val + "', Delete_Permission = '" + name.Delete_val + "' where user_id = '" + name.Id + "'";
            SqlCommand SqlCmd = new SqlCommand(cmd, Con);
            SqlCmd.ExecuteNonQuery();
            Con.Close();
            return Json(name);
        } // Add Updated user data to db
        public ActionResult DeleteUser(int id)
        {
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {
                SignupModel loginModels = new SignupModel();
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                Con.Open();
                string cmd = "select * from Users where user_id = " + id + "";
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