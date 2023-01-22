using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Project.Models;
using Project.ViewModels.Maintenance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Project.Controllers
{
    public class MaintenanceController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new MaintenanceViewModel();
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
                SQLitecmd.CommandText += " FROM Person P";
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
                    employeeDetail.LoginName = SQLiteReader["LoginName"].ToString();
                    employeeDetail.Password = SQLiteReader["Password"].ToString();

                    viewModel.EmployeeList.DS1.Add(employeeDetail);
                }

                SQLiteReader.Close();
                SQLitecmd.CommandText = "SELECT * FROM DivisionDepartment;";
                SQLiteReader = SQLitecmd.ExecuteReader();

                while (SQLiteReader.Read())
                {
                    var divDept = new DivisionDepartment();

                    divDept.DivisionDepartmentID = Convert.ToInt32(SQLiteReader["DivisionDepartmentID"]);
                    divDept.Name = SQLiteReader["Name"].ToString();

                    viewModel.DivDeptList.Add(divDept);
                }

                SQLiteReader.Close();
                SQLitecmd.CommandText = "SELECT * FROM PersonType;";
                SQLiteReader = SQLitecmd.ExecuteReader();

                while (SQLiteReader.Read())
                {
                    var personType = new PersonType();

                    personType.PersonTypeID = Convert.ToInt32(SQLiteReader["PersonTypeID"]);
                    personType.Name = SQLiteReader["PersonTypeName"].ToString();

                    viewModel.PersonTypeList.Add(personType);
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

        public ActionResult EditAdd(int PersonID, string SaveType, string FirstName, string LastName, string DivisionDepartment, string EmailAddress
            , string Phone1, string Phone2, string Ext, string NotesComments, string PersonType, string LoginName, string Password)
        {
            try
            {
                SQLiteConnection SQLiteConn = new SQLiteConnection();
                SQLiteConn.ConnectionString = @"Data Source=Directory.db; Integrated Security=true";
                SQLiteConn.Open();

                SQLiteCommand SQLitecmd = new SQLiteCommand();
                SQLiteDataReader SQLiteReader;
                SQLitecmd.Connection = SQLiteConn;

                int PersonTypeID = 0;
                SQLitecmd.CommandText = "SELECT * FROM PersonType WHERE PersonTypeName = '" + PersonType + "'";
                SQLiteReader = SQLitecmd.ExecuteReader();
                while (SQLiteReader.Read())
                    PersonTypeID = Convert.ToInt32(SQLiteReader["PersonTypeID"]);

                int DivisionDepartmentID = 0;
                SQLiteReader.Close();
                SQLitecmd.CommandText = "SELECT * FROM DivisionDepartment WHERE Name = '" + DivisionDepartment + "'";
                SQLiteReader = SQLitecmd.ExecuteReader();
                while (SQLiteReader.Read())
                    DivisionDepartmentID = Convert.ToInt32(SQLiteReader["DivisionDepartmentID"]);

                SQLiteReader.Close();
                if (SaveType == "Save")
                {
                    SQLitecmd.CommandText = "UPDATE Person ";
                    SQLitecmd.CommandText += " SET FirstName = '" + FirstName + "' ";
                    SQLitecmd.CommandText += ", LastName = '" + LastName + "' ";
                    SQLitecmd.CommandText += ", DivisionDepartmentID = " + DivisionDepartmentID;
                    SQLitecmd.CommandText += ", EmailAddress = '" + EmailAddress + "' ";
                    SQLitecmd.CommandText += ", Phone1 = '" + Phone1 + "' ";
                    SQLitecmd.CommandText += ", Phone2 = '" + Phone2 + "' ";
                    SQLitecmd.CommandText += ", Ext = '" + Ext + "' ";
                    SQLitecmd.CommandText += ", NotesComments = '" + NotesComments + "' ";
                    SQLitecmd.CommandText += ", PersonTypeID = " + PersonTypeID;
                    SQLitecmd.CommandText += ", LoginName = '" + LoginName + "' ";
                    SQLitecmd.CommandText += ", Password = '" + Password + "' ";
                    SQLitecmd.CommandText += " WHERE PersonID = " + PersonID;
                }
                else
                {
                    SQLitecmd.CommandText = "INSERT INTO Person (FirstName, LastName, DivisionDepartmentID, EmailAddress, Phone1, Phone2, Ext, NotesComments, PersonTypeID, LoginName, Password)";
                    SQLitecmd.CommandText += " VALUES ('" + FirstName + "', '" + LastName + "', '" + DivisionDepartmentID + "', '" + EmailAddress + "', " + Phone1 + ", " + Phone2 + ", '" + Ext + "', '" + NotesComments + "', " + PersonTypeID + ", '" + LoginName + "', '" + Password + "')";
                }

                SQLiteReader = SQLitecmd.ExecuteReader();

                SQLiteReader.Close();
                SQLiteConn.Close();
            }
            catch(Exception e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction("Index", "Maintenance");
        }

        public ActionResult Delete(int PersonID)
        {
            try
            {
                SQLiteConnection SQLiteConn = new SQLiteConnection();
                SQLiteConn.ConnectionString = @"Data Source=Directory.db; Integrated Security=true";
                SQLiteConn.Open();

                SQLiteCommand SQLitecmd = new SQLiteCommand();
                SQLiteDataReader SQLiteReader;

                SQLitecmd.Connection = SQLiteConn;
                SQLitecmd.CommandText = "DELETE FROM Person WHERE PersonID = " + PersonID;
                SQLiteReader = SQLitecmd.ExecuteReader();

                SQLiteReader.Close();
                SQLiteConn.Close();
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction("Index", "Maintenance");
        }
    }
}