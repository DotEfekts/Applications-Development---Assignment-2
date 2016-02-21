using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Students_UnitAttempts : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypes;
    private Dictionary<string, EditMetadata> validationTypes;
    private string uattemptid = null;
    private string attemptid = null;
    private string unitcode = null;

    protected Students_UnitAttempts()
    {
        displayTypes = new Dictionary<string, DisplayMetadata>();
        displayTypes.Add("Unit_Att_ID", new DisplayMetadata(DisplayMetadata.FieldTypes.ID, "Unit Attempt ID"));
        displayTypes.Add("Student_Number", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Student Number"));
        displayTypes.Add("Student_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Student Name"));
        displayTypes.Add("Course_Code", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Code"));
        displayTypes.Add("Unit_Code", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Unit Code"));
        displayTypes.Add("Semester_Attempted", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Semester Attempted"));
        displayTypes.Add("Attempt_Mark", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Attempt Mark"));

        validationTypes = new Dictionary<string, EditMetadata>();
        validationTypes.Add("Unit_Att_ID", new EditMetadata(EditMetadata.FieldTypes.ID, "", "Unit Attempt ID"));

        Dictionary<string, string> courseAttempts = new Dictionary<string, string>();
        foreach (Dictionary<string, object> attemptData in StudentRecordsDAL.Query("SELECT * FROM Attempt_Lookup"))
            courseAttempts.Add(attemptData["Attempt_ID"].ToString(), attemptData["Attempt"].ToString());
        EditMetadata attempt = new EditMetadata(EditMetadata.FieldTypes.Dropdown, "You must add an entry to the student results table.", "Course Attempt");
        attempt.SetDropdownValues(courseAttempts);
        validationTypes.Add("Attempt_ID", attempt);

        Dictionary<string, string> units = new Dictionary<string, string>();
        foreach (Dictionary<string, object> unitData in StudentRecordsDAL.Query("SELECT * FROM Unit"))
            units.Add(unitData["Unit_ID"].ToString(), unitData["Unit_Code"].ToString());
        EditMetadata unit = new EditMetadata(EditMetadata.FieldTypes.Dropdown, "You must add an entry to the unit table.", "Unit");
        unit.SetDropdownValues(units);
        validationTypes.Add("Unit_ID", unit);

        validationTypes.Add("Semester_Attempted", new EditMetadata("^[0-9]{2}(1|2)$", "You must enter 2 numbers followed by a 1 or a 2.", "Semester Attempted"));
        validationTypes.Add("Attempt_Mark", new EditMetadata("^([0-9]{1,2}|100)$", "You must enter a mark between 0 and 100.", "Attempt Mark"));
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("Stu")).Attributes.Add("class", "active");
        ((HtmlControl)Master.FindControl("StuU")).Attributes.Add("class", "active");
        ((Literal)Master.FindControl("pagetitle")).Text = " - Unit Attempts";

        Dictionary<string, string> students = new Dictionary<string, string>();
        students.Add("0", "All Students");
        foreach (Dictionary<string, object> studentsData in StudentRecordsDAL.Query("SELECT * FROM Student"))
            students.Add(studentsData["Student_ID"].ToString(), studentsData["Student_Number"].ToString() + " - " + studentsData["Student_Name"]);
        filterBox.ID = "filterdrop_";
        filterBox.DataSource = students;
        filterBox.DataValueField = "Key";
        filterBox.DataTextField = "Value";
        filterBox.DataBind();

        if (IsPostBack)
        {
            foreach (string key in Request.Form)
            {
                if (key.EndsWith("addButton") || Regex.Match(key, "edit([0-9]+)$").Success || key.EndsWith("editRow"))
                {
                    filterBox.Visible = false;
                }
                else
                {
                    if (key.EndsWith("filterdrop_"))
                        filterBox.SelectedValue = Request.Form[key];
                }
            }

            if(filterBox.SelectedValue != "0")
                RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Unit_Attempt_Details WHERE Student_ID=" + filterBox.SelectedValue));
            else
                RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Unit_Attempt_Details"));
        } else
        {
            filterBox.SelectedValue = "0";
            RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Unit_Attempt_Details"));
        }

        RecordsDataControl.BindEditData(StudentRecordsDAL.Query("SELECT * FROM Unit_Attempt"));
        RecordsDataControl.SetTableName("Unit_Attempt");
        RecordsDataControl.SetDisplayMetadata(displayTypes);
        RecordsDataControl.SetEditMetadata(validationTypes);
        RecordsDataControl.AddValidationMethod("Unit_Att_ID", setUnitAttemptID);
        RecordsDataControl.AddValidationMethod("Attempt_ID", setAttemptID);
        RecordsDataControl.AddValidationMethod("Unit_ID", unitAttemptChecks);
        RecordsDataControl.AddValidationMethod("Semester_Attempted", multipleSemesterAttempts);
        RecordsDataControl.SetFunction(updateDB);
        RecordsDataControl.BuildControl();
    }

    private string setUnitAttemptID(object uattemptid)
    {
        this.uattemptid = uattemptid.ToString();
        return null;
    }

    private string setAttemptID(object attemptid)
    {
        this.attemptid = attemptid.ToString();
        return null;
    }

    private string unitAttemptChecks(object unitcode)
    {
        this.unitcode = unitcode.ToString();
        if (uattemptid == "")
            uattemptid = "0";

        int count = StudentRecordsDAL.Query("SELECT * FROM Unit_Attempt WHERE Attempt_ID=" + attemptid + " AND Unit_ID=" + unitcode.ToString() + " AND Unit_Att_ID<>" + uattemptid).Count;
        int passed = StudentRecordsDAL.Query("SELECT * FROM Unit_Attempt WHERE Attempt_ID=" + attemptid + " AND Unit_ID=" + unitcode.ToString() + " AND Unit_Att_ID<>" + uattemptid + " AND Attempt_Mark>49").Count;
        if (passed > 0)
            return "Unit cannot be passed twice.";
        else if (count > 2)
            return "Unit cannot be enrolled in a 4th time.";
        else
            return null;
    }

    private string multipleSemesterAttempts(object semester)
    {
        return StudentRecordsDAL.Query("SELECT * FROM Unit_Attempt WHERE Attempt_ID=" + attemptid + " AND Unit_ID=" + unitcode + " AND Semester_Attempted=" + semester.ToString() + " AND Unit_Att_ID<>" + uattemptid).Count == 0 ? null : "Unit already enrolled in semester.";
    }

    private void updateDB(Dictionary<string, string> data, bool update)
    {
        if (update)
        {
            StudentRecordsDAL.Command("UPDATE Unit_Attempt SET Attempt_ID=" + data["Attempt_ID"] + ", Unit_ID=" + data["Unit_ID"] +
                ", Semester_Attempted=" + data["Semester_Attempted"] + ", Attempt_Mark=" + data["Attempt_Mark"] + " WHERE Unit_Att_ID=" + data["Unit_Att_ID"]);
        }
        else
            StudentRecordsDAL.Command("INSERT INTO Unit_Attempt (Attempt_ID, Unit_ID, Semester_Attempted, Attempt_Mark) VALUES (" + 
                data["Attempt_ID"] + ", " + data["Unit_ID"] + ", " + data["Semester_Attempted"] + ", " + data["Attempt_Mark"] + ")");
    }
}