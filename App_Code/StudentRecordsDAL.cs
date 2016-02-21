using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for StudentRecordsDAL
/// </summary>
public class StudentRecordsDAL
{
    private static OleDbConnection databaseCon;

    public StudentRecordsDAL()
    {
        
    }

    static StudentRecordsDAL()
    {
        //AppDomain.CurrentDomain.SetData("DataDirectory", HttpContext.Current.Server.MapPath("~/App_Data/"));
        databaseCon = new OleDbConnection(ConfigurationManager.ConnectionStrings["StudentRecordConnectionString"].ConnectionString);
    }

    public static ArrayList Query(String query)
    {
        if(databaseCon.State != ConnectionState.Open)
            databaseCon.Open();
        OleDbDataReader reader = new OleDbCommand(query, databaseCon).ExecuteReader();
        ArrayList data = new ArrayList();
        while (reader.Read())
        {
            Dictionary<string, object> arr = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
                arr[reader.GetName(i)] = reader[i];
            data.Add(arr);
        }
        databaseCon.Close();
        return data;
    }

    public static void Command(String command)
    {
        if (databaseCon.State != ConnectionState.Open)
            databaseCon.Open();
        new OleDbCommand(command, databaseCon).ExecuteNonQuery();
        databaseCon.Close();
    }

}