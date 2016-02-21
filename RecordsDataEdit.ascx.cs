using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class RecordsDataEdit : System.Web.UI.UserControl
{
    private string table = "";
    private string editid = "";
    private ArrayList data;
    private Dictionary<string, string> dataValidation;
    private Dictionary<string, EditMetadata> fieldTypes;
    private Dictionary<string, List<Func<object, string>>> fieldMethods;

    protected RecordsDataEdit()
    {
        fieldMethods = new Dictionary<string, List<Func<object, string>>>();
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void SetID(string editid)
    {
        this.editid = editid;
    }

    public void SetTableName(string table)
    {
        this.table = table;
    }

    public void BindEditData(ArrayList data)
    {
        this.data = data;
    }

    public void SetFieldTypes(Dictionary<string, EditMetadata> fieldTypes)
    {
        this.fieldTypes = fieldTypes;
    }

    public void BuildForm()
    {
        editor.Controls.Clear();

        if (fieldTypes == null)
            throw new Exception("fieldTypes not initialized");

        KeyValuePair<string, EditMetadata> id = new KeyValuePair<string, EditMetadata>();
        foreach (KeyValuePair<string, EditMetadata> field in fieldTypes)
            if (field.Value.GetValidationType() == EditMetadata.FieldTypes.ID)
                id = field;

        Dictionary<string, object> datarow = null;
        if (editid != null && id.Key != null)
            foreach (Dictionary<string, object> row in data)
                if (row[id.Key].ToString() == editid)
                    datarow = row;

        Dictionary<string, DropDownList> dropdowns = new Dictionary<string, DropDownList>();
        foreach(KeyValuePair<string, EditMetadata> field in fieldTypes)
        {
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell labelCell = new HtmlTableCell();
            HtmlTableCell inputCell = new HtmlTableCell();
            HtmlGenericControl label = new HtmlGenericControl("label");
            HtmlGenericControl input = new HtmlGenericControl("input");
            HtmlGenericControl hidden = new HtmlGenericControl("input");
            hidden.Attributes.Add("type", "hidden");
            EditMetadata.FieldTypes fieldType = EditMetadata.FieldTypes.Custom;

            label.Attributes.Add("class", "bold");

            if (fieldTypes.ContainsKey(field.Key))
            {
                label.InnerText = fieldTypes[field.Key].GetDisplayName() + ":";
                fieldType = fieldTypes[field.Key].GetValidationType();
            }

            switch (fieldType)
            {
                case EditMetadata.FieldTypes.Bool:
                    input.Attributes.Add("type", "checkbox");
                    break;
                case EditMetadata.FieldTypes.ReadOnly:
                    input.Attributes.Add("type", "text");
                    input.Attributes.Add("disabled", "");
                    break;
                case EditMetadata.FieldTypes.Text:
                case EditMetadata.FieldTypes.Number:
                case EditMetadata.FieldTypes.Date:
                case EditMetadata.FieldTypes.Custom:
                    input.Attributes.Add("type", "text");
                    break;
                default:
                    input.Attributes.Add("type", "hidden");
                    break;
            }

            if (fieldType != EditMetadata.FieldTypes.ID)
                labelCell.Controls.Add(label);

            if (fieldType != EditMetadata.FieldTypes.Dropdown)
            {
                if (Request.Form[field.Key] != null)
                {
                    input.Attributes.Add("value", Request.Form[field.Key]);
                    if (fieldType == EditMetadata.FieldTypes.ReadOnly)
                        hidden.Attributes.Add("value", Request.Form[field.Key]);
                    if (fieldType == EditMetadata.FieldTypes.Bool)
                        if (Request.Form[field.Key] == "True")
                        {
                            if (fieldType == EditMetadata.FieldTypes.ReadOnly)
                                hidden.Attributes.Add("value", Request.Form[field.Key]);
                            input.Attributes.Add("checked", "");
                        }
                }
                else if (datarow != null)
                {
                    if (fieldType == EditMetadata.FieldTypes.ReadOnly)
                        hidden.Attributes.Add("value", datarow[field.Key].ToString());
                    input.Attributes.Add("value", datarow[field.Key].ToString());
                    if (fieldType == EditMetadata.FieldTypes.Bool)
                        if ((bool)datarow[field.Key] == true)
                        {
                            if (fieldType == EditMetadata.FieldTypes.ReadOnly)
                                hidden.Attributes.Add("value", datarow[field.Key].ToString());
                            input.Attributes.Add("checked", "");
                        }
                    if (fieldType == EditMetadata.FieldTypes.Date)
                    {
                        if (fieldType == EditMetadata.FieldTypes.ReadOnly)
                            input.Attributes.Add("value", ((DateTime)datarow[field.Key]).ToShortDateString());
                        input.Attributes.Add("value", ((DateTime)datarow[field.Key]).ToShortDateString());

                    }
                }

                if (fieldType == EditMetadata.FieldTypes.ReadOnly)
                {
                    hidden.Attributes.Add("id", field.Key);
                    hidden.Attributes.Add("name", field.Key);
                }
                else
                {
                    input.Attributes.Add("id", field.Key);
                    input.Attributes.Add("name", field.Key);
                }

                inputCell.Controls.Add(hidden);
                inputCell.Controls.Add(input);
            }
            else
            {
                DropDownList list = new DropDownList();
                list.ID = "rdedrop_" + field.Key;
                list.DataSource = field.Value.GetDropdownValues();
                list.DataValueField = "Key";
                list.DataTextField = "Value";
                list.DataBind();
                if (Request.Form[field.Key] != null)
                    list.SelectedValue = Request.Form[field.Key];
                else if (datarow != null)
                    list.SelectedValue = datarow[field.Key].ToString();
                inputCell.Controls.Add(list);
            }

            if (dataValidation != null)
                if (dataValidation.ContainsKey(field.Key))
                {
                    label.Attributes["class"] = "bold invalid";
                    HtmlGenericControl errorMessage = new HtmlGenericControl("span");
                    errorMessage.Attributes.Add("class", "invalid");
                    errorMessage.InnerText = dataValidation[field.Key];
                    inputCell.Controls.Add(errorMessage);
                }

            row.Controls.Add(labelCell);
            row.Controls.Add(inputCell);
            editor.Controls.Add(row);
        }

        HtmlTableRow buttonrow = new HtmlTableRow();
        HtmlTableCell buttoncell = new HtmlTableCell();
        Button editbutton = new Button();
        Button cancelbutton = new Button();

        buttoncell.ColSpan = 2;

        if(datarow != null || (Request.Form[id.Key] != "" && Request.Form[id.Key] != null))
            editbutton.Text = "Edit";
        else
            editbutton.Text = "Add";

        editbutton.ID = "editRow";
        editbutton.Click += new System.EventHandler(btnClickEdit);

        cancelbutton.ID = "cancel";
        cancelbutton.Text = "Cancel";
        cancelbutton.Click += new System.EventHandler(btnClickCancel);

        buttoncell.Attributes.Add("class", "rowbuttons");
        buttoncell.Controls.Add(editbutton);
        buttoncell.Controls.Add(cancelbutton);
        buttonrow.Controls.Add(buttoncell);
        editor.Controls.Add(buttonrow);

    }

    private void btnClickEdit(object sender, EventArgs e)
    {
    }

    private void btnClickCancel(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }

    public bool IsValid()
    {
        return dataValidation.Count == 0 ? true : false;
    }

    public void PassData(NameValueCollection formData)
    {
        this.dataValidation = CheckValidation(formData);
    }

    public Dictionary<string, string> CheckValidation(NameValueCollection formData)
    {
        Dictionary<string, string> errors = new Dictionary<string, string>();
        foreach (string key in formData)
        {
            if(key.Contains("rdedrop_"))
            {
                string keyNoDrop = key.Substring(key.IndexOf("rdedrop_") + 8);
                if (formData[key] == "")
                {
                    errors.Add(keyNoDrop, fieldTypes[keyNoDrop].GetErrorString());
                    continue;
                }

                if (fieldMethods.ContainsKey(keyNoDrop))
                {
                    foreach (Func<object, string> function in fieldMethods[keyNoDrop])
                    {
                        string functionReturn = function.Invoke(formData[key]);
                        if (functionReturn != null)
                        {
                            errors.Add(keyNoDrop, functionReturn);
                            break;
                        }
                    }
                }
            }
             
            if (fieldTypes.ContainsKey(key))
                if (Regex.Match(formData[key], fieldTypes[key].GetValidationString()).Success)
                {
                    if (fieldMethods.ContainsKey(key))
                    {
                        foreach (Func<object, string> function in fieldMethods[key])
                        {
                            string functionReturn = function.Invoke(formData[key]);
                            if (functionReturn != null)
                            {
                                errors.Add(key, functionReturn);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    errors.Add(key, fieldTypes[key].GetErrorString());
                }
        }
        return errors;
    }

    public void AddValidationMethod(string field, Func<object, string> function)
    {
        if (!fieldMethods.ContainsKey(field))
            fieldMethods.Add(field, new List<Func<object, string>>());

        fieldMethods[field].Add(function);
    }
}