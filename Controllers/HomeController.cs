using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new Employees();
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
                SQLitecmd.CommandText += " , P.FirstName ";
                SQLitecmd.CommandText += " , P.LastName ";
                SQLitecmd.CommandText += " , (SELECT Name FROM DivisionDepartment WHERE DivisionDepartmentID = P.DivisionDepartmentID) AS DivisionDepartment ";
                SQLitecmd.CommandText += " , P.EmailAddress ";
                SQLitecmd.CommandText += " , P.Phone1 ";
                SQLitecmd.CommandText += " , P.Phone2 ";
                SQLitecmd.CommandText += " , P.Ext ";
                SQLitecmd.CommandText += " , P.NotesComments ";
                SQLitecmd.CommandText += " , (SELECT PersonTypeName FROM PersonType WHERE PersonTypeID = P.PersonTypeID) AS PersonType ";
                SQLitecmd.CommandText += " , P.LoginName ";
                SQLitecmd.CommandText += " , P.Password ";
                SQLitecmd.CommandText += " FROM Person P ";
                SQLitecmd.CommandText += " ORDER BY ";
                SQLitecmd.CommandText += " DivisionDepartment ";
                SQLitecmd.CommandText += " , (CASE ";
                SQLitecmd.CommandText += "  WHEN PersonType = 'VP' THEN 0 ";
                SQLitecmd.CommandText += "  WHEN PersonType = 'Director' THEN 1 ";
                SQLitecmd.CommandText += "  WHEN PersonType = 'Assistant Director' THEN 2 ";
                SQLitecmd.CommandText += "  WHEN PersonType = 'Assistant Manager' THEN 3 ";
                SQLitecmd.CommandText += "  WHEN PersonType = 'Supervisor' THEN 4 ";
                SQLitecmd.CommandText += "  END) ASC ";
                SQLitecmd.CommandText += " , P.LastName ";
                SQLiteReader = SQLitecmd.ExecuteReader();

                while (SQLiteReader.Read())
                {
                    var employeeDetail = new EmployeeDetail();

                    employeeDetail.PersonID = Convert.ToInt32(SQLiteReader["PersonID"]);
                    employeeDetail.FirstName = SQLiteReader["FirstName"].ToString();
                    employeeDetail.LastName = SQLiteReader["LastName"].ToString();
                    employeeDetail.DivDept = SQLiteReader["DivisionDepartment"].ToString();
                    employeeDetail.Email = SQLiteReader["EmailAddress"].ToString();
                    employeeDetail.Phone1 = SQLiteReader["Phone1"].ToString();
                    employeeDetail.Phone2 = SQLiteReader["Phone2"].ToString();
                    employeeDetail.Ext = SQLiteReader["Ext"].ToString();
                    employeeDetail.Notes = SQLiteReader["NotesComments"].ToString();
                    employeeDetail.PersonType = SQLiteReader["PersonType"].ToString();

                    viewModel.DS1.Add(employeeDetail);
                }

                SQLiteReader.Close();
                SQLiteConn.Close();
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
            }

            return View(viewModel);
        }
    }
}