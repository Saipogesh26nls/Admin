using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Admin.Models
{
    public class LoginField
    {
        public LoginModel RtnLogins(string cUser, string cPassword)
        {
            LoginModel LogDetail = new LoginModel();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd2 = "select * FROM Users where username='" + cUser + "' and password='" + cPassword + "'";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                LogDetail.UserId = dr["user_id"].ToString();
                LogDetail.UserName = dr["username"].ToString();
                LogDetail.Roll = dr["Roll"].ToString();
                LogDetail.Display_name = dr["Display_name"].ToString();
                LogDetail.View = dr["View_Permission"].ToString();
                LogDetail.Add = dr["Add_Permission"].ToString();
                LogDetail.Edit = dr["Edit_Permission"].ToString();
                LogDetail.Delete = dr["Delete_Permission"].ToString();
                LogDetail.Menu = dr["Menu_Id"].ToString();
                LogDetail.Disable = dr["Disable"].ToString();
                return LogDetail;
            }
            else
            {
                return null;
            }
        }
        public int AddLogUser(string cUser, int nLoginout)
        {
            string cmd2 = "";
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            if (nLoginout == 1)
            {
                cmd2 = "INSERT INTO Log_User(User_ID,Date_Of_Log,Login_Time) VALUES ('" + cUser + "','" + DateTime.Today.Date.ToString("MM/dd/yyyy HH:mm:ss") + "','" + DateTime.Now.ToString() + "')";
            }
            else
            {
                cmd2 = "INSERT INTO Log_User(User_ID,Date_Of_Log,Logout_Time) VALUES ('" + cUser + "','" + DateTime.Today.Date.ToString("MM/dd/yyyy HH:mm:ss") + "','" + DateTime.Now.ToString() + "')";
            }

            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            int var;
            var = SqlCmd2.ExecuteNonQuery();
            return var;
        }
        public DataSet EditPurchase()
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select * from Menu ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SqlCmd1);
            da.Fill(ds);
            return ds;
        }
        public void Create_user(List<Createuser> data)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd = "update Number_Master set User_No = User_No + 1";
            SqlCommand SqlCmd = new SqlCommand(cmd, Con);
            SqlCmd.ExecuteNonQuery();
            string cmd2 = "Select User_No from Number_Master";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            int userid = 0;
            while (dr.Read())
            {
                userid = (int)dr["User_No"];
            }
            Con.Close();
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int i = 0;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[CreateUser]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@userid", SqlDbType.Int).Value = userid;
                sql_cmnd.Parameters.AddWithValue("@disp_name", SqlDbType.NVarChar).Value = data[0].displayname;
                sql_cmnd.Parameters.AddWithValue("@username", SqlDbType.NVarChar).Value = data[0].username;
                sql_cmnd.Parameters.AddWithValue("@password", SqlDbType.NVarChar).Value = data[0].password;
                sql_cmnd.Parameters.AddWithValue("@roll", SqlDbType.Int).Value = data[0].roll;
                sql_cmnd.Parameters.AddWithValue("@menu", SqlDbType.NVarChar).Value = data[i].Menu;
                sql_cmnd.Parameters.AddWithValue("@view", SqlDbType.Int).Value = data[i].view_val;
                sql_cmnd.Parameters.AddWithValue("@add", SqlDbType.Int).Value = data[i].add_val;
                sql_cmnd.Parameters.AddWithValue("@edit", SqlDbType.Int).Value = data[i].edit_val;
                sql_cmnd.Parameters.AddWithValue("@delete", SqlDbType.Int).Value = data[i].delete_val;
                sql_cmnd.Parameters.AddWithValue("@disable", SqlDbType.Int).Value = data[i].disable_val;
                sql_cmnd.ExecuteNonQuery();
                if (i == data.Count() - 1)
                {
                    break;
                }
                i++;
            }
            Con1.Close();
        }
        public List<SignupModel> List()
        {
            List<SignupModel> loginModels = new List<SignupModel>();
            List<int> userid = new List<int>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd2 = "SELECT user_id FROM Users GROUP BY user_id HAVING COUNT(*)>0";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr1 = SqlCmd2.ExecuteReader();
            while (dr1.Read())
            {
                userid.Add(Convert.ToInt32(dr1["user_id"]));
            }
            dr1.Close();
            if (userid.Count == 0)
            {
                Con.Close();
                return loginModels;
            }
            else
            {
                for (int i = 0; i < userid.Count; i++)
                {
                    string cmd1 = "select Top 1 user_id, Display_Name, username, password, Roll from Users where user_id = '" + userid[i] + "' ORDER BY user_id Asc;";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        loginModels.Add(new SignupModel
                        {
                            Id = (int)dr["user_id"],
                            DisplayName = dr["Display_name"].ToString(),
                            UserName = dr["username"].ToString(),
                            Password = dr["password"].ToString(),
                            Roll = (int)dr["Roll"],
                        }
                        );
                    }
                    dr.Close();
                }
                for (int i = 0; i < loginModels.Count; i++)
                {
                    string cmd3 = "select Roll from Roll where Id = '" + loginModels[i].Roll + "'";
                    SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
                    SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                    while (dr2.Read())
                    {
                        loginModels[i].Roll_Name = dr2["Roll"].ToString();
                    }
                    dr2.Close();
                }
                Con.Close();
                return loginModels;
            }
        }
        public DataSet EditUser(int id)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select * from Users where user_id = '" + id + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SqlCmd1);
            da.Fill(ds);
            return ds;
        }
    }
}