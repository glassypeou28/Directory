using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using Project.Models;

namespace Project.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string Username, string Password, string RememberMe, string ReturnUrl)
        {
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("UserType");

            string username = Username.Trim();
            string password = Password.Trim();
            bool rememberMe = RememberMe == null ? false : true;
            string returnUrl = ReturnUrl;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Username and password are required!";

                return RedirectToAction("Index");
            }
            else
            {
                EmployeeDetail employeeDetail = AttemptLogin(username, password);

                if (employeeDetail.PersonID > 0) //successful login
                {
                    HttpContext.Session.SetInt32("UserID", employeeDetail.PersonID);
                    HttpContext.Session.SetString("UserType", employeeDetail.PersonType);
                    TempData["Error"] = null;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Invalid username and/or password!";

                    return RedirectToAction("Index");
                }
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Remove("UserID");

            return RedirectToAction("Index");
        }

        private EmployeeDetail AttemptLogin(string loginName, string password)
        {
            EmployeeDetail employeeDetail = new EmployeeDetail();
            try
            {
                SQLiteConnection SQLiteConn = new SQLiteConnection();
                SQLiteConn.ConnectionString = @"Data Source=Directory.db; Integrated Security=true";
                SQLiteConn.Open();

                SQLiteCommand SQLitecmd = new SQLiteCommand();
                SQLiteDataReader SQLiteReader;

                SQLitecmd.Connection = SQLiteConn;
                SQLitecmd.CommandText = "SELECT ";
                SQLitecmd.CommandText += " P.PersonID ";
                SQLitecmd.CommandText += " , (SELECT PersonTypeName FROM PersonType WHERE PersonTypeID = P.PersonTypeID) AS PersonType ";
                SQLitecmd.CommandText += " FROM Person P WHERE P.LoginName = '" + loginName + "' AND P.Password = '" + password + "'";
                SQLiteReader = SQLitecmd.ExecuteReader();

                while (SQLiteReader.Read())
                {
                    employeeDetail.PersonID = SQLiteReader["PersonID"] == null ? 0 : Convert.ToInt32(SQLiteReader["PersonID"]);
                    employeeDetail.PersonType = SQLiteReader["PersonType"] == null ? "Operator" : SQLiteReader["PersonType"].ToString();
                }

                SQLiteReader.Close();
                SQLiteConn.Close();
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
            }

            return employeeDetail;
        }
    }
}