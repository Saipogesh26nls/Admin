using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace Admin.Models
{
    public class LoginField
    {
        public LoginModel RtnLogins(string cUser, string cPassword)
        {
            LoginModel LogDetail = new LoginModel();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd2 = "select user_id, Display_name, username, password, Roll, permission_detail FROM Login_Fields where username='" + cUser + "' and password='" + cPassword + "'";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                LogDetail.UserId = dr["user_id"].ToString();
                LogDetail.UserName = dr["username"].ToString();
                LogDetail.Roll = dr["Roll"].ToString();
                LogDetail.Display_name = dr["Display_name"].ToString();
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
                cmd2 = "INSERT INTO Log_User_Schedule(User_ID,Date_Of_Log,Login_Time) VALUES ('" + cUser + "','" + DateTime.Today.Date.ToString("MM/dd/yyyy HH:mm:ss") + "','" + DateTime.Now.ToString() + "')";
            }
            else
            {
                cmd2 = "INSERT INTO Log_User_Schedule(User_ID,Date_Of_Log,Logout_Time) VALUES ('" + cUser + "','" + DateTime.Today.Date.ToString("MM/dd/yyyy HH:mm:ss") + "','" + DateTime.Now.ToString() + "')";
            }

            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            int var;
            var = SqlCmd2.ExecuteNonQuery();
            return var;
        }
    }
}