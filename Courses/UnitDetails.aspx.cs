using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class UnitDetails : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypes;
    private Dictionary<string, EditMetadata> validationTypes;
    private string id = null;

    protected UnitDetails()
    {
        displayTypes = new Dictionary<string, DisplayMetadata>();
        displayTypes.Add("Unit_ID", new DisplayMetadata(DisplayMetadata.FieldTypes.ID, "Unit ID"));
        displayTypes.Add("Unit_Code", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Unit Code"));
        displayTypes.Add("Unit_Title", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Unit Title"));
        displayTypes.Add("Staff_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Unit Coordinator"));
        displayTypes.Add("Unit_Credit_Points", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Unit Credit Points"));
        displayTypes.Add("Postgrad_Unit", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Postgrad Unit"));

        validationTypes = new Dictionary<string, EditMetadata>();
        validationTypes.Add("Unit_ID", new EditMetadata(EditMetadata.FieldTypes.ID, "", "Unit ID"));
        validationTypes.Add("Unit_Code", new EditMetadata("^[A-Z]{3}[0-9]{4}$", "A course code must be 3 capital letter followed by 4 numbers.", "Unit Code"));
        validationTypes.Add("Unit_Title", new EditMetadata("^[a-zA-Z\\s0-9]{5}([a-zA-Z\\s0-9]*)?$", "You must enter a unit title at least 5 characters with only letters, numbers, and spaces.", "Unit Title"));

        Dictionary<string, string> staff = new Dictionary<string, string>();
        foreach (Dictionary<string, object> staffData in StudentRecordsDAL.Query("SELECT * FROM Staff"))
            staff.Add(staffData["Staff_ID"].ToString(), staffData["Staff_Name"].ToString());
        EditMetadata staf = new EditMetadata(EditMetadata.FieldTypes.Dropdown, "You must add an entry to the staff table.", "Unit Coordinator");
        staf.SetDropdownValues(staff);
        validationTypes.Add("Unit_Coordinator", staf);

        validationTypes.Add("Unit_Credit_Points", new EditMetadata("^(15|20|60)$", "Credit points can be either 15, 20, or 60.", "Credit Points"));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("Cor")).Attributes.Add("class", "active");
        ((HtmlControl)Master.FindControl("CorU")).Attributes.Add("class", "active");
        ((Literal)Master.FindControl("pagetitle")).Text = " - Unit Details";

        RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Unit_Coord"));
        RecordsDataControl.BindEditData(StudentRecordsDAL.Query("SELECT * FROM Unit"));
        RecordsDataControl.SetTableName("Unit");
        RecordsDataControl.SetDisplayMetadata(displayTypes);
        RecordsDataControl.SetEditMetadata(validationTypes);
        RecordsDataControl.SetFunction(updateDB);
        RecordsDataControl.AddValidationMethod("Unit_ID", setID);
        RecordsDataControl.AddValidationMethod("Unit_Code", unitCodeUnique);
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

    private string unitCodeUnique(object code)
    {
        return StudentRecordsDAL.Query("SELECT * FROM Unit WHERE Unit_Code='" + code.ToString() + "' AND Unit_ID<>" + id).Count == 0 ? null : "Unit code not unique.";
    }

    private void updateDB(Dictionary<string, string> data, bool update)
    {
        if (update)
        {
            StudentRecordsDAL.Command("UPDATE Unit SET Unit_Code='" + data["Unit_Code"] + "', Unit_Title='" + data["Unit_Title"] + "', Unit_Coordinator=" + data["Unit_Coordinator"] +
                ", Unit_Credit_Points=" + data["Unit_Credit_Points"] + " WHERE Unit_ID=" + data["Unit_ID"]);
        }
        else
            StudentRecordsDAL.Command("INSERT INTO Unit (Unit_Code, Unit_Title, Unit_Coordinator, Unit_Credit_Points) VALUES ('" + data["Unit_Code"] + "', '" + data["Unit_Title"] + "', " + data["Unit_Coordinator"] +
                ", " + data["Unit_Credit_Points"] + ")");
    }
}