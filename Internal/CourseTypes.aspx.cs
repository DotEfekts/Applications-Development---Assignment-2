using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Internal_CourseTypes : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypes;
    private Dictionary<string, EditMetadata> validationTypes;

    protected Internal_CourseTypes()
    {
        displayTypes = new Dictionary<string, DisplayMetadata>();
        displayTypes.Add("Course_Type_ID", new DisplayMetadata(DisplayMetadata.FieldTypes.ID, "Course Type ID"));
        displayTypes.Add("Course_Type_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Course Type Name"));
        displayTypes.Add("Course_Type_Credit_Points", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Credit Points Required"));
        displayTypes.Add("Course_Type_Duration_Months", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Duration (Months)"));

        validationTypes = new Dictionary<string, EditMetadata>();
        validationTypes.Add("Course_Type_ID", new EditMetadata(EditMetadata.FieldTypes.ID, "", "Unit ID"));
        validationTypes.Add("Course_Type_Name", new EditMetadata(EditMetadata.FieldTypes.Text, "You must enter a name with only letters, numbers, and spaces.", "Course Type Name"));
        validationTypes.Add("Course_Type_Credit_Points", new EditMetadata(EditMetadata.FieldTypes.Number, "You must enter a number.", "Credit Points Required"));
        validationTypes.Add("Course_Type_Duration_Months", new EditMetadata(EditMetadata.FieldTypes.Number, "You must enter a number.", "Duration (Months)"));

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("Int")).Attributes.Add("class", "active");
        ((HtmlControl)Master.FindControl("IntT")).Attributes.Add("class", "active");
        ((Literal)Master.FindControl("pagetitle")).Text = " - Course Types";

        RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Course_Type"));
        RecordsDataControl.BindEditData(StudentRecordsDAL.Query("SELECT * FROM Course_Type"));
        RecordsDataControl.SetTableName("Course_Type");
        RecordsDataControl.SetDisplayMetadata(displayTypes);
        RecordsDataControl.SetEditMetadata(validationTypes);
        RecordsDataControl.SetFunction(updateDB);
        RecordsDataControl.BuildControl();
    }

    private void updateDB(Dictionary<string, string> data, bool update)
    {
        if (update)
        {
            StudentRecordsDAL.Command("UPDATE Course_Type SET Course_Type_Name='" + data["Course_Type_Name"] + "', Course_Type_Credit_Points=" + data["Course_Type_Credit_Points"] +
                ", Course_Type_Duration_Months=" + data["Course_Type_Duration_Months"] + " WHERE Course_Type_ID=" + data["Course_Type_ID"]);
        }
        else
            StudentRecordsDAL.Command("INSERT INTO Course_Type (Course_Type_Name, Course_Type_Credit_Points, Course_Type_Duration_Months) VALUES ('" + data["Course_Type_Name"] + "', " + data["Course_Type_Credit_Points"] +
                ", " + data["Course_Type_Duration_Months"] + ")");
    }
}