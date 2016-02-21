using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RecordsDataControl : System.Web.UI.UserControl
{
    private string editid = "";
    private Action<Dictionary<string, string>, bool> function;
    private Dictionary<string, EditMetadata> fieldTypes;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void BindViewData(ArrayList data)
    {
        RecordsDataView.BindViewData(data);
    }

    public void BindEditData(ArrayList data)
    {
        RecordsDataEdit.BindEditData(data);
    }

    public void SetTableName(string table)
    {
        RecordsDataView.SetTableName(table);
        RecordsDataEdit.SetTableName(table);
    }

    public void SetFunction(Action<Dictionary<string, string>, bool> function)
    {
        this.function = function;
    }

    public void SetDisplayMetadata(Dictionary<string, DisplayMetadata> fieldTypes)
    {
        RecordsDataView.SetFieldTypes(fieldTypes);
    }

    public void AddValidationMethod(string field, Func<object, string> function)
    {
        RecordsDataEdit.AddValidationMethod(field, function);
    }

    public void SetEditMetadata(Dictionary<string, EditMetadata> fieldTypes)
    {
        this.fieldTypes = fieldTypes;
        RecordsDataEdit.SetFieldTypes(fieldTypes);
    }

    public void BuildControl()
    {
        if (!IsPostBack)
        {
            RecordsDataEdit.Visible = false;
            RecordsDataView.BuildTable();
        }
        else
        {
            bool viewTable = true;
            bool tryEntry = false;
            foreach (string key in Request.Form)
            {
                if (key.EndsWith("addButton"))
                {
                    viewTable = false;
                    break;
                }
                else if (Regex.Match(key, "edit([0-9]+)$").Success)
                {
                    viewTable = false;
                    this.editid = Regex.Match(key, "edit([0-9]+)$").Groups[1].ToString();
                    break;
                }
                else if (key.EndsWith("editRow"))
                {
                    viewTable = false;
                    tryEntry = true;
                    break;
                }
            }

            if(viewTable)
            {
                RecordsDataEdit.Visible = false;
                RecordsDataView.Visible = true;
                RecordsDataView.BuildTable();
            }
            else
            {
                if (editid != "")
                    RecordsDataEdit.SetID(editid);
                RecordsDataView.Visible = false;
                RecordsDataEdit.Visible = true;
                RecordsDataEdit.PassData(Request.Form);
                if (tryEntry)
                {
                    if (RecordsDataEdit.IsValid())
                    {
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        bool update = false;
                        foreach (string key in Request.Form)
                        { 
                            foreach(KeyValuePair<string, EditMetadata> pair in fieldTypes)
                            {
                                if(key.EndsWith(pair.Key))
                                {
                                    if (fieldTypes[pair.Key].GetValidationType() != EditMetadata.FieldTypes.ReadOnly)
                                        param.Add(pair.Key, Request.Form[key]);
                                    if (fieldTypes[pair.Key].GetValidationType() == EditMetadata.FieldTypes.ID)
                                        if (Request.Form[key] != "")
                                            update = true;
                                }
                            }
                        }
                        function.Invoke(param, update);
                        Response.Redirect(Request.RawUrl);
                    }
                }
                RecordsDataEdit.BuildForm();
            }
        }
    }
}