using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class StudentLogin : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypesCourses;
    private Dictionary<string, DisplayMetadata> displayTypesUnits;
    protected StudentLogin()
    {
        displayTypesCourses = new Dictionary<string, DisplayMetadata>();
        displayTypesCourses.Add("Course_Code", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Code"));
        displayTypesCourses.Add("Course_Average", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Average"));
        displayTypesCourses.Add("Credit_Points", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Credit Points Earned"));
        displayTypesCourses.Add("Credit_Points_Required", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Credit Points Required"));
        displayTypesCourses.Add("Course_Status", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Status"));
        displayTypesCourses.Add("Units_Attempted", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Units Attempted"));
        displayTypesCourses.Add("Highest_Mark", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Highest Mark"));
        displayTypesCourses.Add("Lowest_Mark", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Lowest Mark"));

        displayTypesUnits = new Dictionary<string, DisplayMetadata>();
        displayTypesUnits.Add("Course_Code", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Code"));
        displayTypesUnits.Add("Unit_Code", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Unit Code"));
        displayTypesUnits.Add("Semester_Attempted", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Semester Attempted"));
        displayTypesUnits.Add("Attempt_Mark", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Attempt Mark"));
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("StuL")).Attributes.Add("class", "active");

        if (!IsPostBack)
        {
            data.Visible = false;
        }
        else
        {
            if (Regex.Match(Request.Form["usernameinput"], "^[a-zA-Z]+[0-9]*@our\\.ecu\\.edu\\.au").Success)
                if (Regex.Match(Request.Form["passwordinput"], "^(?=(.*[A-Z]){2})(?=(.*\\d){2})[a-zA-Z\\d]{8,}$").Success)
                    if (StudentRecordsDAL.Query("SELECT * FROM Student WHERE UCASE(Student_Email)='" + Request.Form["usernameinput"].ToUpper() +
                        "' AND Student_Password='" + Request.Form["passwordinput"] + "'").Count > 0)
                    {
                        login.Visible = false;
                        data.Visible = true;

                        stunam.InnerText = ((Dictionary<string, object>)StudentRecordsDAL.Query("SELECT * FROM Student WHERE UCASE(Student_Email)='" + Request.Form["usernameinput"].ToUpper() +
                        "' AND Student_Password='" + Request.Form["passwordinput"] + "'")[0])["Student_Name"].ToString();

                        stunum.InnerText = ((Dictionary<string, object>)StudentRecordsDAL.Query("SELECT * FROM Student WHERE UCASE(Student_Email)='" + Request.Form["usernameinput"].ToUpper() +
                        "' AND Student_Password='" + Request.Form["passwordinput"] + "'")[0])["Student_Number"].ToString();

                        RecordsDataViewCourses.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Course_Attempt_Details WHERE UCASE(Student_Email)='" + Request.Form["usernameinput"].ToUpper() + "'"));
                        RecordsDataViewCourses.SetFieldTypes(displayTypesCourses);
                        RecordsDataViewCourses.IsEditable(false);
                        RecordsDataViewCourses.SetTableName("Course");
                        RecordsDataViewCourses.BuildTable();

                        RecordsDataViewUnits.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Unit_Attempt_Details WHERE UCASE(Student_Email)='" + Request.Form["usernameinput"].ToUpper() + "'"));
                        RecordsDataViewUnits.SetFieldTypes(displayTypesUnits);
                        RecordsDataViewUnits.IsEditable(false);
                        RecordsDataViewUnits.SetTableName("Unit");
                        RecordsDataViewUnits.BuildTable();
                        return;
                    }
            data.Visible = false;
            login.Visible = true;
            invalidup.Text = "Invalid username or password.";
        }

    }
}