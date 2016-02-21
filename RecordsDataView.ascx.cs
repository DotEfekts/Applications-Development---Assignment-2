using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class RecordsDataView : System.Web.UI.UserControl
{
    private ArrayList data;
    private bool editable = true;
    private string table;
    private string idCol;
    private Dictionary<string, DisplayMetadata> fieldTypes;

    protected RecordsDataView()
    {

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    protected void add_button(object sender, EventArgs e)
    {
        
    }

    protected void delete_button(object sender, EventArgs e)
    {
        ArrayList deleteQueue = new ArrayList();
        foreach (string key in Request.Form)
        {
            Match match = Regex.Match(key, "^check([0-9]+)$");
            if (match.Success)
                deleteQueue.Add(Request.Form["rowid" + match.Groups[1]]);
        }

        ArrayList deleteErr = new ArrayList();
        string errTable = "";
        foreach (string id in deleteQueue)
            try {
                StudentRecordsDAL.Command("DELETE FROM " + table + " WHERE " + idCol + " = " + id);
            } catch (OleDbException ex)
            {
                errTable = Regex.Match(ex.Message, "'([a-z_A-Z]+)' includes related records\\.$").Groups[1].ToString();
                deleteErr.Add(id);
            }
        errTable = errTable.Replace("_", " ");
        errTable += "s";

        if (errTable != "s")
            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeyClient", "alert('Unable to delete " + deleteErr.Count + " row(s) due to related entries in " + errTable + ".'); reloadAsGet();", true);
        else
            Response.Redirect(Request.RawUrl);
    }

    public void SetFieldTypes(Dictionary<string, DisplayMetadata> fieldTypes)
    {
        foreach (KeyValuePair<string, DisplayMetadata> entry in fieldTypes)
            if (entry.Value.GetFieldType() == DisplayMetadata.FieldTypes.ID)
                this.idCol = entry.Key;
        this.fieldTypes = fieldTypes;
    }

    public void BindViewData(ArrayList data)
    {
        this.data = data;
    }

    public void IsEditable(bool editable)
    {
        this.editable = editable;
    }

    public void SetTableName(string table)
    {
        this.table = table;
    }

    public void BuildTable()
    {
        dataview.Controls.Clear();

        if (fieldTypes == null)
            throw new Exception("fieldTypes not initialized");

        HtmlTableRow headers = new HtmlTableRow();
        headers.Attributes.Add("class", "headers");

        if (editable)
        {
            HtmlTableCell checkH = new HtmlTableCell("th");
            checkH.InnerText = "";
            checkH.Width = "30px";
            headers.Controls.Add(checkH);
        }

        foreach (KeyValuePair<string, DisplayMetadata> entry in fieldTypes)
        {
            if (entry.Value.IsHeader())
            {
                HtmlTableCell heading = new HtmlTableCell("th");
                heading.InnerText = entry.Value.GetDisplayName();
                heading.Attributes.Add("class", "bold");
                heading.Attributes.Add("col", entry.Key);
                headers.Controls.Add(heading);
            }
        }

        if (editable)
        {
            HtmlTableCell rowbuttonth = new HtmlTableCell("th");
            Button addButton = new Button();
            Button deleteButton = new Button();

            addButton.Click += new System.EventHandler(add_button);
            addButton.Text = "Add";
            addButton.ID = "addButton";
            deleteButton.Click += new System.EventHandler(delete_button);
            deleteButton.Text = "Delete";
            deleteButton.ID = "deleteButton";
            deleteButton.OnClientClick = "return confirmcount()";

            rowbuttonth.InnerText = "";
            rowbuttonth.Width = "100px";
            rowbuttonth.Attributes.Add("class", "rowbuttons");

            rowbuttonth.Controls.Add(addButton);
            rowbuttonth.Controls.Add(deleteButton);
            headers.Controls.Add(rowbuttonth);
        }

        dataview.Controls.Add(headers);

        if(data.Count == 0)
        {
            HtmlTableRow displaytr = new HtmlTableRow();
            HtmlTableCell displaytd = new HtmlTableCell();
            displaytr.Attributes.Add("class", "datarow");
            displaytd.InnerText = "No entries found in table.";
            displaytd.Attributes.Add("colspan", "99");
            displaytd.Attributes.Add("class", "emptytablecell");
            displaytr.Controls.Add(displaytd);
            dataview.Controls.Add(displaytr);
            return;
        }

        int counter = 0;
        foreach (Dictionary<string, object> row in data)
        {
            HtmlTableRow displaytr = new HtmlTableRow();
            HtmlTableRow datatr = new HtmlTableRow();
            HtmlTableCell datatd = new HtmlTableCell();
            int rowid = 0;

            displaytr.Attributes.Add("class", "datarow");
            datatr.Attributes.Add("class", "datasec");
            datatd.Attributes.Add("colspan", "99");

            if (editable)
            {
                HtmlTableCell checkC = new HtmlTableCell();
                HtmlGenericControl check = new HtmlGenericControl("input");

                checkC.Attributes.Add("class", "rowcheck");
                check.Attributes.Add("type", "checkbox");
                check.Attributes.Add("id", "check" + counter);
                check.Attributes.Add("name", "check" + counter);
                checkC.Controls.Add(check);
                displaytr.Controls.Add(checkC);
            }


            HtmlInputRadioButton slider = new HtmlInputRadioButton();
            HtmlGenericControl collapser = new HtmlGenericControl("div");
            HtmlGenericControl divsub = new HtmlGenericControl("div");

            slider.Attributes.Add("class", "collapserad");
            slider.Attributes.Add("id", table + "radio" + counter);
            collapser.Attributes.Add("class", "collapser");
            divsub.Attributes.Add("class", "subdata");

            collapser.Controls.Add(divsub);
            datatd.Controls.Add(slider);
            
            foreach (KeyValuePair<string, object> column in row)
            {
                HtmlGenericControl valuelabel = new HtmlGenericControl("label");
                HtmlGenericControl value = new HtmlGenericControl("label");
                DisplayMetadata.FieldTypes fieldType = DisplayMetadata.FieldTypes.Hidden;

                valuelabel.Attributes.Add("class", "bold");
                if (fieldTypes.ContainsKey(column.Key))
                {
                    valuelabel.InnerText = fieldTypes[column.Key].GetDisplayName() + ":";
                    fieldType = fieldTypes[column.Key].GetFieldType();
                }

                switch (fieldType)
                {
                    case DisplayMetadata.FieldTypes.Checkbox:
                        if((bool)column.Value)
                            value.InnerText = "Yes";
                        else
                            value.InnerText = "No";
                        break;
                    case DisplayMetadata.FieldTypes.Text:
                        value.InnerText = column.Value.ToString();
                        break;
                    case DisplayMetadata.FieldTypes.Date:
                        value.InnerText = ((DateTime)column.Value).ToLongDateString();
                        break;
                    case DisplayMetadata.FieldTypes.ID:
                    case DisplayMetadata.FieldTypes.Hidden:
                    default:
                        valuelabel.InnerText = "";
                        value = new HtmlGenericControl("input");
                        value.Attributes.Add("type", "hidden");
                        value.Attributes.Add("class", "hidden");
                        value.Attributes.Add("name", column.Key.ToString() + counter);
                        value.Attributes.Add("id", column.Key.ToString() + counter);
                        value.Attributes.Add("value", column.Value.ToString());
                        if (fieldType == DisplayMetadata.FieldTypes.ID)
                        {
                            rowid = (int)column.Value;
                            value.Attributes["name"] = "rowid" + counter;
                        }
                        break;
                }

                if (valuelabel.InnerText == "")
                    divsub.Controls.Add(value);
                else
                {
                    divsub.Controls.Add(valuelabel);
                    divsub.Controls.Add(value);
                    divsub.Controls.Add(new LiteralControl("<br />"));

                    if(fieldTypes[column.Key].IsHeader())
                    {
                        HtmlTableCell td = new HtmlTableCell();
                        HtmlGenericControl headerlabel = new HtmlGenericControl("label");

                        headerlabel.InnerText = value.InnerText;
                        headerlabel.Attributes.Add("for", table + "radio" + counter);
                        headerlabel.Attributes.Add("class", "collapselabel");
                        td.Controls.Add(headerlabel);
                        displaytr.Controls.Add(td);
                    }
                }
            }

            if (editable)
            {
                HtmlTableCell buttontd = new HtmlTableCell();
                Button editbutton = new Button();
                Button deletebutton = new Button();

                editbutton.ID = "edit" + rowid;
                editbutton.Text = "Edit";

                deletebutton.OnClientClick = "return confirm('Are you sure you want to delete this row?')";
                deletebutton.ID = "delete" + rowid;
                deletebutton.Text = "Delete";
                deletebutton.Click += new System.EventHandler(btnDeleteClick);

                buttontd.Attributes.Add("class", "rowbuttons");
                buttontd.Controls.Add(editbutton);
                buttontd.Controls.Add(deletebutton);
                displaytr.Controls.Add(buttontd);
            }

            datatd.Controls.Add(collapser);
            datatr.Controls.Add(datatd);
            dataview.Controls.Add(displaytr);
            dataview.Controls.Add(datatr);
            counter++;
        }
    }

    private void btnDeleteClick(object sender, EventArgs e)
    {
        string id;
        Match match = Regex.Match(((Button)sender).ID, "^delete([0-9]+)$");
            if (match.Success)
               id = match.Groups[1].ToString();
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "KeyClient", "alert('Unable to delete row due to unknown error')", true);
                return;
            }
        
        string errTable = "";
        try
        {
            StudentRecordsDAL.Command("DELETE FROM " + table + " WHERE " + idCol + " = " + id);
        }
        catch (OleDbException ex)
        {
            errTable = Regex.Match(ex.Message, "'([a-z_A-Z]+)' includes related records\\.$").Groups[1].ToString();
        }
        errTable = errTable.Replace("_", " ");
        errTable += "s";

        if (errTable != "s")
            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeyClient", "alert('Unable to delete row due to related entries in " + errTable + ".'); reloadAsGet();", true);
        else
            Response.Redirect(Request.RawUrl);
    }
}