using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for FieldValidation
/// </summary>
public class FieldValidation
{
    private Dictionary<string, EditMetadata> validationMetadata;
    private Dictionary<string, List<Func<object, string>>> fieldMethods;

    public FieldValidation(Dictionary<string, EditMetadata> validationMetadata)
	{
        this.validationMetadata = validationMetadata;
        fieldMethods = new Dictionary<string, List<Func<object, string>>>();
	}

    public Dictionary<string, string> CheckValidation(NameValueCollection formData)
    {
        Dictionary<string, string> errors = new Dictionary<string, string>();
        foreach (string key in formData)
        {
            if (validationMetadata.ContainsKey(key))
                if (Regex.Match(formData[key], validationMetadata[key].GetValidationString()).Success)
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
                    errors.Add(key, validationMetadata[key].GetErrorString());
                }
        }
        return errors;
    }

    public void AddValidationMethod(string field, Func<object, string> function)
    {
        if(!fieldMethods.ContainsKey(field))
            fieldMethods.Add(field, new List<Func<object, string>>());

        fieldMethods[field].Add(function);
    }
}