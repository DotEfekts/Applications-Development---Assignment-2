using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Internal_States : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypes;
    private Dictionary<string, EditMetadata> validationTypes;

    protected Internal_States()
    {
        displayTypes = new Dictionary<string, DisplayMetadata>();
        displayTypes.Add("State_ID", new DisplayMetadata(DisplayMetadata.FieldTypes.ID, "State ID"));
        displayTypes.Add("State_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "State Name"));

        validationTypes = new Dictionary<string, EditMetadata>();
        validationTypes.Add("State_ID", new EditMetadata(EditMetadata.FieldTypes.ID, "", "State ID"));
        validationTypes.Add("State_Name", new EditMetadata("^[a-z A-Z]+$", "You must enter a name with only letters and spaces.", "State"));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("Int")).Attributes.Add("class", "active");
        ((HtmlControl)Master.FindControl("IntS")).Attributes.Add("class", "active");
        ((Literal)Master.FindControl("pagetitle")).Text = " - States";

        RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM State"));
        RecordsDataControl.BindEditData(StudentRecordsDAL.Query("SELECT * FROM State"));
        RecordsDataControl.SetTableName("State");
        RecordsDataControl.SetDisplayMetadata(displayTypes);
        RecordsDataControl.SetEditMetadata(validationTypes);
        RecordsDataControl.SetFunction(updateDB);
        RecordsDataControl.BuildControl();
    }

    private void updateDB(Dictionary<string, string> data, bool update)
    {
        if (update)
        {
            StudentRecordsDAL.Command("UPDATE State SET State_Name='" + data["State_Name"] + "' WHERE State_ID=" + data["State_ID"]);
        }
        else
            StudentRecordsDAL.Command("INSERT INTO State (State_Name) VALUES ('" + data["State_Name"] + "')");
    }
}