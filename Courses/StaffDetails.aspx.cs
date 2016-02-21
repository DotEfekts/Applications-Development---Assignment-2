using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Courses_Staff : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypes;
    private Dictionary<string, EditMetadata> validationTypes;

    protected Courses_Staff()
    {
        displayTypes = new Dictionary<string, DisplayMetadata>();
        displayTypes.Add("Staff_ID", new DisplayMetadata(DisplayMetadata.FieldTypes.ID, "Staff ID"));
        displayTypes.Add("Staff_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Staff Name"));

        validationTypes = new Dictionary<string, EditMetadata>();
        validationTypes.Add("Staff_ID", new EditMetadata(EditMetadata.FieldTypes.ID, "", "Staff ID"));
        validationTypes.Add("Staff_Name", new EditMetadata("^[a-zA-Z]+ [a-zA-Z]+([a-z\\sA-Z]+)?$", "You must enter a first name and a last name with only letters.", "Staff Name"));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("Cor")).Attributes.Add("class", "active");
        ((HtmlControl)Master.FindControl("CorS")).Attributes.Add("class", "active");
        ((Literal)Master.FindControl("pagetitle")).Text = " - Staff Details";

        RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Staff"));
        RecordsDataControl.BindEditData(StudentRecordsDAL.Query("SELECT * FROM Staff"));
        RecordsDataControl.SetTableName("Staff");
        RecordsDataControl.SetDisplayMetadata(displayTypes);
        RecordsDataControl.SetEditMetadata(validationTypes);
        RecordsDataControl.SetFunction(updateDB);
        RecordsDataControl.BuildControl();
    }

    private void updateDB(Dictionary<string, string> data, bool update)
    {
        if (update)
        {
            StudentRecordsDAL.Command("UPDATE Staff SET Staff_Name='" + data["Staff_Name"] + "' WHERE Staff_ID=" + data["Staff_ID"]);
        }
        else
            StudentRecordsDAL.Command("INSERT INTO Staff (Staff_Name) VALUES ('" + data["Staff_Name"] + "')");
    }
}