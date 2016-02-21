using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Internal_Genders : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypes;
    private Dictionary<string, EditMetadata> validationTypes;

    protected Internal_Genders()
    {
        displayTypes = new Dictionary<string, DisplayMetadata>();
        displayTypes.Add("Gender_ID", new DisplayMetadata(DisplayMetadata.FieldTypes.ID, "Gender ID"));
        displayTypes.Add("Gender_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Gender Name"));

        validationTypes = new Dictionary<string, EditMetadata>();
        validationTypes.Add("Gender_ID", new EditMetadata(EditMetadata.FieldTypes.ID, "", "Gender ID"));
        validationTypes.Add("Gender_Name", new EditMetadata("^[a-z A-Z]+$", "You must enter a name with only letters and spaces.", "Gender"));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("Int")).Attributes.Add("class", "active");
        ((HtmlControl)Master.FindControl("IntG")).Attributes.Add("class", "active");
        ((Literal)Master.FindControl("pagetitle")).Text = " - Genders";

        RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Gender"));
        RecordsDataControl.BindEditData(StudentRecordsDAL.Query("SELECT * FROM Gender"));
        RecordsDataControl.SetTableName("Gender");
        RecordsDataControl.SetDisplayMetadata(displayTypes);
        RecordsDataControl.SetEditMetadata(validationTypes);
        RecordsDataControl.SetFunction(updateDB);
        RecordsDataControl.BuildControl();
    }

    private void updateDB(Dictionary<string, string> data, bool update)
    {
        if (update)
        {
            StudentRecordsDAL.Command("UPDATE Gender SET Gender_Name='" + data["Gender_Name"] + "' WHERE Gender_ID=" + data["Gender_ID"]);
        }
        else
            StudentRecordsDAL.Command("INSERT INTO Gender (Gender_Name) VALUES ('" + data["Gender_Name"] + "')");
    }
}