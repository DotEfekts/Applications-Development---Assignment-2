using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EditMetadata
{
    public enum FieldTypes { Text, Number, Date, Bool, Dropdown, ReadOnly, ID, Custom };

    private FieldTypes validationType;
    private string customString;
    private string errorString;
    private string displayString;
    private Dictionary<string, string> dropdownValues;

    public EditMetadata(FieldTypes validationType, string errorString, string displayString)
    {
        this.validationType = validationType;
        this.customString = ".*";
        this.errorString = errorString;
        this.displayString = displayString;
    }

    public EditMetadata(string customString, string errorString, string displayString)
        : this(FieldTypes.Custom, errorString, displayString)
    {
        this.customString = customString;
    }

    public void SetDropdownValues(Dictionary<string, string> dropdownValues) { this.dropdownValues = dropdownValues; }

    public Dictionary<string, string> GetDropdownValues() 
    {
        if (dropdownValues.Count == 0)
            dropdownValues.Add("", "");
        return dropdownValues; 
    }

    public FieldTypes GetValidationType() { return validationType; }
    public string GetErrorString() { return errorString; }
    public string GetDisplayName() { return displayString; }

    public string GetValidationString()
    {
        switch(validationType) 
        {
            case FieldTypes.Text:
                return "^[a-z\\sA-Z0-9]+$";
            case FieldTypes.Number:
                return "^[0-9]+$";
            case FieldTypes.Date:
                return "^ (?: (?: 31(\\/| -|\\.)(?:0?[13578] | 1[02]))\\1 | (?: (?: 29 | 30)(\\/| -|\\.)(?:0?[1, 3 - 9] | 1[0 - 2])\\2))(?: (?: 1[6 - 9] |[2 - 9]\\d)?\\d{ 2})$|^ (?: 29(\\/| -|\\.)0 ? 2\\3(?:(?: (?: 1[6 - 9] |[2 - 9]\\d) ? (?: 0[48] |[2468][048] |[13579][26]) | (?: (?: 16 |[2468][048] |[3579][26])00))))$|^ (?: 0?[1 - 9] | 1\\d | 2[0 - 8])(\\/| -|\\.)(?: (?: 0?[1 - 9]) | (?: 1[0 - 2]))\\4(?:(?: 1[6 - 9] |[2 - 9]\\d) ?\\d{ 2})$";
            case FieldTypes.Custom:
            default:
                return customString;
        }
    }
}
