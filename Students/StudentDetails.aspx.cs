using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class StudentDetails : System.Web.UI.Page
{
    private Dictionary<string, DisplayMetadata> displayTypes;
    private Dictionary<string, EditMetadata> validationTypes;

    protected StudentDetails()
    {
        displayTypes = new Dictionary<string, DisplayMetadata>();
        displayTypes.Add("Student_ID", new DisplayMetadata(DisplayMetadata.FieldTypes.ID, "Student ID"));
        displayTypes.Add("Student_Number", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Student Number"));
        displayTypes.Add("Student_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Student Name"));
        displayTypes.Add("Student_Email", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Student Email"));
        displayTypes.Add("Student_Password", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Student Passsword"));
        displayTypes.Add("Gender_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Student Gender"));
        displayTypes.Add("Date_Of_Birth", new DisplayMetadata(DisplayMetadata.FieldTypes.Date, true, "Date of Birth"));
        displayTypes.Add("Phone_Number", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, true, "Phone Number"));
        displayTypes.Add("Address_Line_One", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Address Line 1"));
        displayTypes.Add("Address_Line_Two", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Address Line 2"));
        displayTypes.Add("City", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "City"));
        displayTypes.Add("State_Name", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "State"));
        displayTypes.Add("Postcode", new DisplayMetadata(DisplayMetadata.FieldTypes.Text, "Postcode"));

        validationTypes = new Dictionary<string, EditMetadata>();
        validationTypes.Add("Student_ID", new EditMetadata(EditMetadata.FieldTypes.ID, "", "Student ID"));
        validationTypes.Add("Student_Number", new EditMetadata(EditMetadata.FieldTypes.ReadOnly, "", "Student Number"));
        validationTypes.Add("Student_Name", new EditMetadata("^[a-zA-Z]+ [a-zA-Z]+([a-z\\sA-Z]+)?$", "You must enter a first name and a last name with only letters.", "Student Name"));
        validationTypes.Add("Student_Email", new EditMetadata(EditMetadata.FieldTypes.ReadOnly, "", "Student Email"));
        validationTypes.Add("Student_Password", new EditMetadata("^(?=(.*[A-Z]){2})(?=(.*\\d){2})[a-zA-Z\\d]{8,}$", "You must enter an alphanumeric password that is at least 8 characters, with at least 2 upper case letters and 2 numbers.", "Student Password"));

        Dictionary<string, string> genders = new Dictionary<string, string>();
        foreach (Dictionary<string, object> stateData in StudentRecordsDAL.Query("SELECT * FROM Gender"))
            genders.Add(stateData["Gender_ID"].ToString(), stateData["Gender_Name"].ToString());
        EditMetadata gender = new EditMetadata(EditMetadata.FieldTypes.Dropdown, "You must add an entry to the gender table.", "Student Gender");
        gender.SetDropdownValues(genders);
        validationTypes.Add("Gender", gender);

        validationTypes.Add("Date_Of_Birth", new EditMetadata(EditMetadata.FieldTypes.Date, "You must enter a date in the format DD/MM/YYYY.", "Date of Birth"));
        validationTypes.Add("Phone_Number", new EditMetadata("^[0-9]{10}$", "You must enter a 10 digit phone number (including area code).", "Phone Number"));
        validationTypes.Add("Address_Line_One", new EditMetadata(EditMetadata.FieldTypes.Text, "You must enter an address.", "Address Line 1"));
        validationTypes.Add("Address_Line_Two", new EditMetadata("^[a-zA-Z 0-9]*$", "You must enter only letters numbers and spaces.", "Address Line 2"));
        validationTypes.Add("City", new EditMetadata(EditMetadata.FieldTypes.Text, "You must enter a city.", "City"));

        Dictionary<string, string> states = new Dictionary<string, string>();
        foreach (Dictionary<string, object> stateData in StudentRecordsDAL.Query("SELECT * FROM State"))
            states.Add(stateData["State_ID"].ToString(), stateData["State_Name"].ToString());
        EditMetadata state = new EditMetadata(EditMetadata.FieldTypes.Dropdown, "You must add an entry to the state table.", "State");
        state.SetDropdownValues(states);
        validationTypes.Add("State", state);

        validationTypes.Add("Postcode", new EditMetadata("^[0-9]{4}$", "You must enter a valid postcode.", "Postcode"));

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((HtmlControl)Master.FindControl("Stu")).Attributes.Add("class", "active");
        ((HtmlControl)Master.FindControl("StuD")).Attributes.Add("class", "active");
        ((Literal)Master.FindControl("pagetitle")).Text = " - Student Details";

        RecordsDataControl.BindViewData(StudentRecordsDAL.Query("SELECT * FROM Student_State"));
        RecordsDataControl.BindEditData(StudentRecordsDAL.Query("SELECT * FROM Student"));
        RecordsDataControl.SetTableName("Student");
        RecordsDataControl.SetDisplayMetadata(displayTypes);
        RecordsDataControl.SetEditMetadata(validationTypes);
        RecordsDataControl.SetFunction(updateDB);
        RecordsDataControl.BuildControl();

    }

    public void updateDB(Dictionary<string, string> data, bool update) {
        string studentEmail = data["Student_Name"].Substring(0, 1) + data["Student_Name"].Split(' ')[1].Substring(0, Math.Min(4, data["Student_Name"].Split(' ')[1].Length));
        string counter = "";
        int counterint = 0;
        while(StudentRecordsDAL.Query("SELECT * FROM Student_State WHERE Student_Email='" + studentEmail + counter + "@our.ecu.edu.au'").Count != 0)
        {
            counterint++;
            counter = counterint.ToString();
        }

        if (update)
            StudentRecordsDAL.Command("UPDATE Student SET Student_Name='" + data["Student_Name"] + "', Student_Email='" + studentEmail + counter + "@our.ecu.edu.au', " +
                " Student_Password='" + data["Student_Password"] + "', Gender=" + data["Gender"] + ", Date_Of_Birth=#" + data["Date_Of_Birth"] + "#, Phone_Number='" +
                data["Phone_Number"] + "', Address_Line_One='" + data["Address_Line_One"] + "', Address_Line_Two='" +
                data["Address_Line_Two"] + "', City='" + data["City"] + "', State=" + data["State"] + ", Postcode='" + data["Postcode"] + "' WHERE Student_ID=" + data["Student_ID"]);
        else
        {
            Random rand = new Random();
            int studentNumber = rand.Next();
            string studentNumberStr = "10000000";
            studentNumber = rand.Next();
            studentNumberStr = studentNumberStr.Substring(0, 8 - Math.Min(8, studentNumber.ToString().Length));
            studentNumberStr = studentNumberStr + studentNumber.ToString().Substring(0, Math.Min(8, studentNumber.ToString().Length));

            while (StudentRecordsDAL.Query("SELECT * FROM Student_State WHERE Student_Number='" + studentNumberStr + "'").Count != 0)
            {
                studentNumber = rand.Next();
                studentNumberStr = studentNumberStr.Substring(0, Math.Min(8, studentNumber.ToString().Length));
                studentNumberStr = studentNumberStr + studentNumber.ToString().Substring(0, Math.Min(8, studentNumber.ToString().Length));
            }

            StudentRecordsDAL.Command("INSERT INTO Student (Student_Number, Student_Name, Student_Email, Student_Password, Gender, Date_Of_Birth, Phone_Number, Address_Line_One, " +
                "Address_Line_Two, City, State, Postcode) VALUES ('" + studentNumberStr + "', '" + data["Student_Name"] + "', '" + studentEmail + counter + "@our.ecu.edu.au', '" + data["Student_Password"] + "', " + 
                data["Gender"] + ", #" + data["Date_Of_Birth"] + "#, '" + data["Phone_Number"] + "', '" + data["Address_Line_One"] + "', '" +
                data["Address_Line_Two"] + "', '" + data["City"] + "', " + data["State"] + ", '" + data["Postcode"] + "')");
        }
    }

    
}