using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class StudentResults : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypes;
    private Dictionary<string, EditMetadata> validationTypes;
    private string attemptID = null;
    private string studentID = null;

    protected StudentResults()
    {
        displayTypes = new Dictionary<string, DisplayMetadata>();
        displayTypes.Add("Attempt_ID", new DisplayMetadata(DisplayMetadata.FieldTypes.ID, "Attempt ID"));
        displayTypes.Add("Student_Number", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Student Number"));
        displayTypes.Add("Student_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Student Name"));
        displayTypes.Add("Course_Code", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Code"));
        displayTypes.Add("Course_Average", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Average"));
        displayTypes.Add("Credit_Points", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Credit Points Earned"));
        displayTypes.Add("Credit_Points_Required", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Credit Points Required"));
        displayTypes.Add("Course_Status", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Status"));
        displayTypes.Add("Units_Attempted", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Units Attempted"));
        displayTypes.Add("Highest_Mark", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Highest Mark"));
        displayTypes.Add("Lowest_Mark", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Lowest Mark"));

        validationTypes = new Dictionary<string, EditMetadata>();
        validationTypes.Add("Attempt_ID", new EditMetadata(EditMetadata.FieldTypes.ID, "", "Attempt ID"));

        Dictionary<string, string> students = new Dictionary<string, string>();
        foreach (Dictionary<string, object> studentData in StudentRecordsDAL.Query("SELECT * FROM Student"))
            students.Add(studentData["Student_ID"].ToString(), studentData["Student_Name"].ToString());
        EditMetadata student = new EditMetadata(EditMetadata.FieldTypes.Dropdown, "You must add an entry to the student table.", "Student");
        student.SetDropdownValues(students);
        validationTypes.Add("Student_ID", student);

        Dictionary<string, string> courses = new Dictionary<string, string>();
        foreach (Dictionary<string, object> courseData in StudentRecordsDAL.Query("SELECT * FROM Course"))
            courses.Add(courseData["Course_ID"].ToString(), courseData["Course_Title"].ToString());
        EditMetadata course = new EditMetadata(EditMetadata.FieldTypes.Dropdown, "You must add an entry to the course table.", "Course");
        course.SetDropdownValues(courses);
        validationTypes.Add("Course_ID", course);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("Stu")).Attributes.Add("class", "active");
        ((HtmlControl)Master.FindControl("StuR")).Attributes.Add("class", "active");
        ((Literal)Master.FindControl("pagetitle")).Text = " - Student Results";

        RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Course_Attempt_Details"));
        RecordsDataControl.BindEditData(StudentRecordsDAL.Query("SELECT * FROM Course_Attempt"));
        RecordsDataControl.SetTableName("Course_Attempt");
        RecordsDataControl.SetDisplayMetadata(displayTypes);
        RecordsDataControl.SetEditMetadata(validationTypes);
        RecordsDataControl.AddValidationMethod("Attempt_ID", setAttemptID);
        RecordsDataControl.AddValidationMethod("Student_ID", setStudentID);
        RecordsDataControl.AddValidationMethod("Course_ID", checkEnrolment);
        RecordsDataControl.SetFunction(updateDB);
        RecordsDataControl.BuildControl();
    }

    private string setAttemptID(object attemptID)
    {
        if (attemptID.ToString() == "")
            this.attemptID = "0";
        else
            this.attemptID = attemptID.ToString();
        return null;
    }

    private string setStudentID(object studentID)
    {
        this.studentID = studentID.ToString();
        return null;
    }

    private string checkEnrolment(object courseid)
    {
        return StudentRecordsDAL.Query("SELECT * FROM Course_Attempt WHERE Attempt_ID<>" + attemptID + " AND Student_ID=" + studentID + " AND Course_ID=" + courseid.ToString()).Count == 0 ? null : "Course already enrolled in.";
    }

    private void updateDB(Dictionary<string, string> data, bool update)
    {
        if (update)
        {
            StudentRecordsDAL.Command("UPDATE Course_Attempt SET Student_ID=" + data["Student_ID"] + ", Course_ID=" + data["Course_ID"] + " WHERE Attempt_ID=" + data["Attempt_ID"]);
        }
        else
            StudentRecordsDAL.Command("INSERT INTO Course_Attempt (Student_ID, Course_ID) VALUES (" + data["Student_ID"] + ", " + data["Course_ID"] + ")");
    }
}