using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FieldMetadata
/// </summary>
public class DisplayMetadata
{
    public enum FieldTypes { ID, Text, Date, Hidden, Checkbox };

    private FieldTypes fieldType;
    private bool header;
    private string displayName;

    public DisplayMetadata(FieldTypes fieldType, bool header, string displayName)
    {
        this.fieldType = fieldType;
        this.header = header;
        this.displayName = displayName;
    }

    public DisplayMetadata(FieldTypes fieldType, string displayName)
        : this(fieldType, false, displayName)
    {

    }

    public FieldTypes GetFieldType() { return fieldType; }
    public string GetDisplayName() { return displayName; }
    public bool IsHeader() { return header; }
}