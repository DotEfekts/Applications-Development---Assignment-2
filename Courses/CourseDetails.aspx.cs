using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class CourseDetails : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypes;
    private Dictionary<string, EditMetadata> validationTypes;
    private string id = null;

    protected CourseDetails()
    {
        displayTypes = new Dictionary<string, DisplayMetadata>();
        displayTypes.Add("Course_ID", new DisplayMetadata(DisplayMetadata.FieldTypes.ID, "Course ID"));
        displayTypes.Add("Course_Code", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Code"));
        displayTypes.Add("Course_Title", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Title"));
        displayTypes.Add("Staff_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Coordinator"));
        displayTypes.Add("Course_Type_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Type Name"));
        displayTypes.Add("Course_Type_Credit_Points", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Course Type Credit Points"));
        displayTypes.Add("Course_Type_Duration_Months", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Course Type Duration Months"));

        validationTypes = new Dictionary<string, EditMetadata>();
        validationTypes.Add("Course_ID", new EditMetadata(EditMetadata.FieldTypes.ID, "", "Course ID"));
        validationTypes.Add("Course_Code", new EditMetadata("^[A-Z][0-9]{2}$", "A course code must be a capital letter followed by 2 numbers.", "Course Code"));
        validationTypes.Add("Course_Title", new EditMetadata("^[a-zA-Z\\s0-9]{5}([a-zA-Z\\s0-9]*)?$", "You must enter a course title at least 5 characters with only letters, numbers, and spaces.", "Course Title"));
        
        Dictionary<string, string> staff = new Dictionary<string, string>();
        foreach (Dictionary<string, object> staffData in StudentRecordsDAL.Query("SELECT * FROM Staff"))
            staff.Add(staffData["Staff_ID"].ToString(), staffData["Staff_Name"].ToString());
        EditMetadata staf = new EditMetadata(EditMetadata.FieldTypes.Dropdown, "You must add an entry to the staff table.", "Course Coordinator");
        staf.SetDropdownValues(staff);
        validationTypes.Add("Course_Coordinator", staf);


        Dictionary<string, string> courseTypes = new Dictionary<string, string>();
        foreach (Dictionary<string, object> courseTypeData in StudentRecordsDAL.Query("SELECT * FROM Course_Type"))
            courseTypes.Add(courseTypeData["Course_Type_ID"].ToString(), courseTypeData["Course_Type_Name"].ToString());
        EditMetadata course = new EditMetadata(EditMetadata.FieldTypes.Dropdown, "You must add an entry to the course type table.", "Course Type");
        course.SetDropdownValues(courseTypes);
        validationTypes.Add("Course_Type", course);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("Cor")).Attributes.Add("class", "active");
        ((HtmlControl)Master.FindControl("CorD")).Attributes.Add("class", "active");
        ((Literal)Master.FindControl("pagetitle")).Text = " - Course Details";

        RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Course_Details_Type"));
        RecordsDataControl.BindEditData(StudentRecordsDAL.Query("SELECT * FROM Course"));
        RecordsDataControl.SetTableName("Course");
        RecordsDataControl.SetDisplayMetadata(displayTypes);
        RecordsDataControl.SetEditMetadata(validationTypes);
        RecordsDataControl.AddValidationMethod("Course_ID", setID);
        RecordsDataControl.AddValidationMethod("Course_Code", courseCodeUnique);
        RecordsDataControl.SetFunction(updateDB);
        RecordsDataControl.BuildControl();
    }
    private string setID(object id)
    {
        if (id.ToString() == "")
            this.id = "0";
        else
            this.id = id.ToString();
        return null;
    }
    private string courseCodeUnique(object code)
    {
        return StudentRecordsDAL.Query("SELECT * FROM Course WHERE Course_Code='" + code.ToString() + "' AND Course_ID<>" + id).Count == 0 ? null : "Course code not unique.";
    }

    private void updateDB(Dictionary<string, string> data, bool update)
    {
        if (update)
        {
            StudentRecordsDAL.Command("UPDATE Course SET Course_Code='" + data["Course_Code"] + "', Course_Title='" + data["Course_Title"] + "', " +
                " Course_Coordinator=" + data["Course_Coordinator"] + ", Course_Type=" + data["Course_Type"] + " WHERE Course_ID=" + data["Course_ID"]);
        }
        else
            StudentRecordsDAL.Command("INSERT INTO Course (Course_Code, Course_Title, Course_Coordinator, Course_Type) VALUES ('" + data["Course_Code"] + "', '" + data["Course_Title"] + "', " + data["Course_Coordinator"] + ", " + data["Course_Type"] + ")");
    }
}