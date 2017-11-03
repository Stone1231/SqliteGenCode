using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;  
public partial class basePage : System.Web.UI.Page 
{
    protected string DBFileName {
        get {
            if (this.ViewState["DBFileName"] != null){
                return ViewState["DBFileName"].ToString();
            }
            else {
                return "johnsonDB.sqlite";//"";
            }
        }
        set {
            ViewState["DBFileName"] = value;
        }
    
    }
    protected string Language 
    {
        get
        {
            if (this.ViewState["Language "] != null)
            {
                return ViewState["Language "].ToString();
            }
            else
            {
                return "objc";//"";
            }
        }
        set
        {
            ViewState["Language"] = value;
        }

    }

    protected string NewLine = "\r\n";

    protected void Page_Load(object sender, EventArgs e)
    {


    }

    protected DataTable getTableSchema(string tableName)
    {
        SQLiteConnection conn = new SQLiteConnection(string.Format("Data Source={0}\\DbFile\\{1}", Server.MapPath("."), DBFileName));
        conn.Open();
        SQLiteCommand cmd = conn.CreateCommand();

        cmd.CommandText = "SELECT * FROM " + tableName;

        SQLiteDataAdapter da = new SQLiteDataAdapter();
        da.SelectCommand = cmd;

        DataTable dt = new DataTable();
        da.FillSchema(dt, SchemaType.Source);

        conn.Close();

        return dt; 
    }

    protected IList<string> getAllTableName() {
        IList<string> tableNames = new List<string>();

        SQLiteConnection conn = new SQLiteConnection(string.Format("Data Source={0}\\DbFile\\{1}", Server.MapPath("."), DBFileName));
        conn.Open();
        SQLiteCommand cmd = conn.CreateCommand();

        cmd.CommandText = "select name from sqlite_master where type = 'table' and name <> 'sqlite_sequence'";
        
        SQLiteDataReader dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            tableNames.Add(dr["name"].ToString());
        }

        return tableNames;
    }

    protected string ConvertType(string strType)
    {
        string str = "";
        switch (Language)
        {
            case "objc":
                switch (strType)
                {
                    case "Int32":
                    case "Int64":
                    case "Integer":
                        str = "int";
                        break;
                    case "String":
                        str = "NSString";
                        break;
                    case "Boolean":
                        str = "BOOL";
                        break;
                    case "Decimal":
                        str = "double";//暫時
                        break;
                    case "Single":
                        str = "double";//暫時
                        break;
                    case "Double":
                        str = "double";
                        break;
                    case "Byte[]":
                        str = "NSData";
                        break;
                    case "DateTime":
                        str = "NSDate";
                        break;
                    default:
                        str = strType;
                        break;
                }
                break;
        }
        return str;
    }

    protected string getObjectName(string strTableClass)
    {
        string str = "";
        Array ary1 = strTableClass.Split('_');
        
        if (ary1.Length > 1)
        {
            int i = 0;
            int j = 0;
            for (i = 0; i <= ary1.Length - 1; i++)
            {
                string ary2 = ary1.GetValue(i).ToString();
                if (ary2.Length > 0)
                {
                    for (j = 0; j <= ary2.Length - 1; j++)
                    {
                        if (j == 0)
                        {
                            str += ary2[j].ToString().ToUpper();
                        }
                        else
                        {
                            str += ary2[j].ToString().ToLower();
                        }
                    }
                }
            }
        }
        else
        {
            str = strTableClass;
        }

        return str;
    }

    protected string getInfoClass(string tabName) {
        return string.Format("{0}Info", getObjectName(tabName));
    }

    protected string getDBClass(string tabName)
    {
        return string.Format("{0}DB", getObjectName(tabName));
    }

    protected string getDeclare(DataColumn dc){
        string typeName = ConvertType(dc.DataType.Name);

        if (typeName.StartsWith("NS"))
        {
            return string.Format("    {0} *{1};", typeName, getObjectName(dc.ColumnName));
        }
        else 
        {
            return string.Format("    {0} {1};", typeName, getObjectName(dc.ColumnName));
        }
    }

    protected string getPkParams(DataColumn[] dcs)
    {
        StringBuilder strs = new StringBuilder();
        for (int i = 0; i < dcs.Length; i++) {
            if (i > 0)
            {
                strs.AppendFormat(" {0}", getObjectName(dcs[i].ColumnName));
            }
            strs.Append(":");

            string typeName = ConvertType(dcs[i].DataType.Name);
            if (typeName.StartsWith("NS"))
            {
                strs.AppendFormat("({0} *){1}", typeName, getObjectName(dcs[i].ColumnName));
            }
            else
            {
                strs.AppendFormat("({0}){1}", typeName, getObjectName(dcs[i].ColumnName));
            }
        }

        return strs.ToString();
    }

    protected string getPkSqlWhere(DataColumn[] dcs)
    {
        StringBuilder strs = new StringBuilder();
        for (int i = 0; i < dcs.Length; i++)
        {
            if (i > 0)
            {
                strs.Append("and ");
            }
            strs.AppendFormat("{0} = ? ", dcs[i].ColumnName);
        }

        return strs.ToString();
    }

    //protected string getInsertColsString(DataTable dt) { 
    

    //}

    protected string getProperty(DataColumn dc){
        string typeName = ConvertType(dc.DataType.Name);
        string objectName = getObjectName(dc.ColumnName);
        if (typeName.StartsWith("NS"))
        {
            //return string.Format("@property (nonatomic, retain) {0} *{1};", typeName, objectName);
            return string.Format("@property (nonatomic, strong) {0} *{1};", typeName, objectName);
        }
        else
        {
            return string.Format("@property (nonatomic) {0} {1};", typeName, objectName);
        }
    }

    protected string getSynthesize(DataColumnCollection dcs) {
        StringBuilder strs = new StringBuilder();
        strs.Append("@synthesize "); 
        for (int i = 0; i < dcs.Count; i++) {
            if (i > 0) {
                strs.Append(", ");
            }
            strs.Append(getObjectName(dcs[i].ColumnName)); 
        }
            return strs.ToString();
    }

    protected string getFunKeyParams(DataColumn[] dcs) {
        StringBuilder strs = new StringBuilder();
        string typeName;
        string objectName;
        for (int i = 0; i < dcs.Length; i++) {
            typeName = ConvertType(dcs[i].DataType.Name);
            objectName = getObjectName(dcs[i].ColumnName);
            if (i > 0) {
                strs.AppendFormat(" {0}", objectName);
            }

            if (typeName.StartsWith("NS"))
            {
                strs.AppendFormat(":({0} *){1}", typeName, objectName);
            }
            else {
                strs.AppendFormat(":({0}){1}", typeName, objectName);
            }
        }
        strs.Append(";");
        return strs.ToString();
    }

    protected string getReadDbCol(DataColumn dc, int i)
    {
        StringBuilder strs = new StringBuilder();
        switch (Language)
        {
            case "objc":
                switch (dc.DataType.Name)
                {
                    case "Int32":
                    case "Int64":
                    case "Integer":
                        strs.AppendFormat("        info.{0} = sqlite3_column_int(stm,{1});", getObjectName(dc.ColumnName), i);strs.Append(NewLine);
                        break;
                    case "String":
                        strs.AppendFormat("        info.{0} = [NSString stringWithUTF8String:(char *)sqlite3_column_text(stm,{1})];", getObjectName(dc.ColumnName), i);strs.Append(NewLine);
                        break;
                    case "Boolean"://?暫時
                        strs.AppendFormat("        info.{0} = sqlite3_column_int(stm,{1});", getObjectName(dc.ColumnName), i);strs.Append(NewLine);
                        break;
                    case "Decimal"://?暫時
                        strs.AppendFormat("        info.{0} = sqlite3_column_double(stm,{1});", getObjectName(dc.ColumnName), i);strs.Append(NewLine);
                        break;
                    case "Single"://?暫時
                        strs.AppendFormat("        info.{0} = sqlite3_column_double(stm,{1});", getObjectName(dc.ColumnName), i); strs.Append(NewLine);
                        break;
                    case "Double":
                        strs.AppendFormat("        info.{0} = sqlite3_column_double(stm,{1});", getObjectName(dc.ColumnName), i);strs.Append(NewLine);
                        break;
                    case "Byte[]":
                        strs.AppendFormat("        info.{0} = [[NSData alloc] initWithBytes:sqlite3_column_blob(stm, {1}) length:sqlite3_column_bytes(stm, {1})];", getObjectName(dc.ColumnName), i);strs.Append(NewLine);
                        break;
                    case "DateTime":
                        strs.Append("        NSDateFormatter *dateFormat = [[NSDateFormatter alloc] init];" + NewLine);
                        strs.Append("        [dateFormat setDateFormat:@\"yyyy-MM-dd HH:mm:ss\"];" + NewLine);
                        strs.AppendFormat("        info.{0} = [dateFormat dateFromString:[NSString stringWithUTF8String:(char *)sqlite3_column_text(stm, {1})]];", getObjectName(dc.ColumnName), i);strs.Append(NewLine);
                        break;
                    default:
                        break;
                }
                break;
        }
        return strs.ToString();
    }

    protected string getBindDbCol(DataColumn dc, int i)
    {
        StringBuilder strs = new StringBuilder();

        switch (Language)
        {
            case "objc":
                switch (dc.DataType.Name)
                {
                    case "Int32":
                    case "Int64":
                    case "Integer":
                        strs.AppendFormat("            sqlite3_bind_int(stm, {0}, info.{1});", i + 1, getObjectName(dc.ColumnName));strs.Append(NewLine);
                        break;
                    case "String":
                        strs.AppendFormat("            sqlite3_bind_text(stm, {0}, [info.{1} UTF8String], -1, SQLITE_TRANSIENT);", i + 1, getObjectName(dc.ColumnName));strs.Append(NewLine);
                        break;
                    case "Boolean"://?暫時                    
                        strs.AppendFormat("            sqlite3_bind_int(stm, {0}, info.{1});", i + 1, getObjectName(dc.ColumnName));strs.Append(NewLine);
                        break;
                    case "Decimal"://?暫時
                        strs.AppendFormat("        sqlite3_bind_double(stm, {0}, info.{1});", i + 1, getObjectName(dc.ColumnName));strs.Append(NewLine);
                        break;
                    case "Single"://?暫時
                        strs.AppendFormat("        sqlite3_bind_double(stm, {0}, info.{1});", i + 1, getObjectName(dc.ColumnName)); strs.Append(NewLine);
                        break;
                    case "Double":
                        strs.AppendFormat("        sqlite3_bind_double(stm, {0}, info.{1});", i + 1, getObjectName(dc.ColumnName));strs.Append(NewLine);
                        break;
                    case "Byte[]":   
                        strs.AppendFormat("        sqlite3_bind_blob(stm, {0}, [info.{1} bytes], [info.{1} length], NULL);", i + 1, getObjectName(dc.ColumnName));strs.Append(NewLine);
                        break;
                    case "DateTime":
                        strs.Append("        NSDateFormatter *dateFormat = [[NSDateFormatter alloc] init];" + NewLine);
                        strs.Append("        [dateFormat setDateFormat:@\"yyyy-MM-dd HH:mm:ss\"];" + NewLine);
                        strs.AppendFormat("        NSString *dateString=[dateFormat stringFromDate:info.{0}];", getObjectName(dc.ColumnName));strs.Append(NewLine);
                        strs.AppendFormat("        sqlite3_bind_text(stm, {0}, [dateString UTF8String] , -1, SQLITE_TRANSIENT);", i + 1);strs.Append(NewLine);
                        break;
                    default:
                        break;
                }
                break;
        }
        return strs.ToString();
    }
}
