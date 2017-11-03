using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data;
public partial class index : basePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        UpDBFile();
    }
    private void UpDBFile()
    {
        if (File.Exists(string.Format("{0}\\DbFile\\{1}", Server.MapPath("."), FileUpload1.FileName)))
        {
            File.Delete(string.Format("{0}\\DbFile\\{1}", Server.MapPath("."), FileUpload1.FileName));
        }

        FileUpload1.SaveAs(string.Format("{0}\\DbFile\\{1}", Server.MapPath("."), FileUpload1.FileName));

        if (File.Exists(string.Format("{0}\\DbFile\\{1}", Server.MapPath("."), FileUpload1.FileName)))
        {
            this.DBFileName = FileUpload1.FileName;
            GetCode();
        }
    }

    IList<string> tabNames;
    IList<DataTable> dbTabs;
    private void GetCode()
    {
        tabNames = this.getAllTableName();
        dbTabs = new List<DataTable>();
        foreach (string tabName in tabNames) {
            DataTable dt = this.getTableSchema(tabName);
            dbTabs.Add(dt);
        }

        SetInfoH();
        SetInfoM();
        SetDbH();
        SetDbM();
    }

    private void SetInfoH() {
        StringBuilder infoH = new StringBuilder();
        infoH.Append("#import <Foundation/Foundation.h>"); infoH.Append(NewLine);

        for (int i = 0; i < tabNames.Count; i++) {
            infoH.AppendFormat("@interface {0} : NSObject{{", getInfoClass(tabNames[i])); infoH.Append(NewLine);

            StringBuilder infoH2 = new StringBuilder();

            for (int j = 0; j < dbTabs[i].Columns.Count; j++)
            {
                infoH.AppendFormat("    {0}", getDeclare(dbTabs[i].Columns[j])); infoH.Append(NewLine);
                infoH2.Append(getProperty(dbTabs[i].Columns[j])); infoH2.Append(NewLine);
            }
            infoH.Append("}"); infoH.Append(NewLine);
            infoH.Append(infoH2.ToString());
            infoH.Append("@end"); infoH.Append(NewLine); 
            infoH.Append(NewLine);
        }

        TextBox1.Text = infoH.ToString();
    }

    private void SetInfoM()
    {
        StringBuilder infoM = new StringBuilder();
        infoM.Append("#import \"Info.h\""); infoM.Append(NewLine);

        for (int i = 0; i < tabNames.Count; i++)
        {
            infoM.AppendFormat("@implementation {0}", getInfoClass(tabNames[i])); infoM.Append(NewLine);
            infoM.Append("@synthesize ");

            for (int j = 0; j < dbTabs[i].Columns.Count; j++)
            {
                if (j > 0)
                {
                    infoM.Append(",");
                }
                infoM.Append(getObjectName(dbTabs[i].Columns[j].ColumnName));
            }
            infoM.Append(";"); infoM.Append(NewLine);
            infoM.Append("@end"); infoM.Append(NewLine); 
            infoM.Append(NewLine);
        }
        TextBox2.Text = infoM.ToString();
    }

    private void SetDbH()
    {
        StringBuilder dbH = new StringBuilder();

        dbH.Append("#import <Foundation/Foundation.h>"); dbH.Append(NewLine);
        dbH.Append("#import <sqlite3.h>"); dbH.Append(NewLine);
        dbH.Append("#import \"Info.h\""); dbH.Append(NewLine);
        dbH.Append(NewLine);
        dbH.Append("@interface baseDB : NSObject {"); dbH.Append(NewLine);
        dbH.Append("NSString *sDatabaseName;"); dbH.Append(NewLine);
        dbH.Append("}"); dbH.Append(NewLine);
        dbH.Append("@end"); dbH.Append(NewLine);
        dbH.Append(NewLine);

        for (int i = 0; i < tabNames.Count; i++)
        {
            dbH.AppendFormat("@interface {0} : baseDB", getDBClass(tabNames[i])); dbH.Append(NewLine);
            dbH.Append("-(NSMutableArray *) getAll;"); dbH.Append(NewLine);
            dbH.AppendFormat("-({0} *) getInfo{1};", getInfoClass(tabNames[i]), getPkParams(dbTabs[i].PrimaryKey)); dbH.Append(NewLine);
            dbH.AppendFormat("-(void)addNew:({0} *)info;", getInfoClass(tabNames[i])); dbH.Append(NewLine);
            dbH.AppendFormat("-(void)update:({0} *)info;", getInfoClass(tabNames[i])); dbH.Append(NewLine);
            dbH.AppendFormat("-(void)delete{0};", getPkParams(dbTabs[i].PrimaryKey)); dbH.Append(NewLine);
            dbH.Append("@end");dbH.Append(NewLine); 
            dbH.Append(NewLine);
        }

        TextBox3.Text = dbH.ToString();
    }

    private void SetDbM()
    {
        StringBuilder dbM = new StringBuilder();

        dbM.Append("#import \"DB.h\""); dbM.Append(NewLine);
        dbM.Append(NewLine);
        dbM.Append("@implementation baseDB"); dbM.Append(NewLine);
        dbM.Append("-(id)init"); dbM.Append(NewLine);
        dbM.Append("{"); dbM.Append(NewLine);
        dbM.Append("    if (self == [super init])"); dbM.Append(NewLine);
        dbM.Append("   {       "); dbM.Append(NewLine);
        dbM.Append("        //sDatabaseName = 自行輸入資料庫連接路徑"); dbM.Append(NewLine);
        dbM.Append("    }"); dbM.Append(NewLine);
        dbM.Append("    return self;"); dbM.Append(NewLine);
        dbM.Append("}"); dbM.Append(NewLine);
        dbM.Append("@end"); dbM.Append(NewLine);
        dbM.Append(NewLine);


        for (int i = 0; i < tabNames.Count; i++)
        {
            dbM.AppendFormat("@implementation {0}", getDBClass(tabNames[i])); dbM.Append(NewLine);

            //getAll
            dbM.Append("-(NSMutableArray *) getAll{"); dbM.Append(NewLine);
            dbM.Append("sqlite3 *database;");
            dbM.Append("NSMutableArray *muArr =  [[NSMutableArray alloc] init];");
            dbM.Append("if(sqlite3_open([sDatabaseName UTF8String],&database)==SQLITE_OK){"); dbM.Append(NewLine);
            dbM.Append("    NSMutableString *sql=[[NSMutableString alloc] initWithString:@\"\"];"); dbM.Append(NewLine);
            dbM.AppendFormat("    [sql appendString:@\"select * from {0}\"];", tabNames[i]); dbM.Append(NewLine);
            dbM.Append("    sqlite3_stmt *stm;"); dbM.Append(NewLine);
            dbM.Append("    if(sqlite3_prepare_v2(database,[sql UTF8String],-1,&stm,NULL)==SQLITE_OK){"); dbM.Append(NewLine);
            dbM.Append("        muArr = [self getArr:stm];"); dbM.Append(NewLine);
            dbM.Append("    }"); dbM.Append(NewLine);
            dbM.Append("    sqlite3_finalize(stm);"); dbM.Append(NewLine);
            dbM.Append("}"); dbM.Append(NewLine);
            dbM.Append("sqlite3_close(database);"); dbM.Append(NewLine);
            dbM.Append("return muArr;"); dbM.Append(NewLine);
            dbM.Append("}"); dbM.Append(NewLine);
            dbM.Append(NewLine);

            //getInfo
            dbM.AppendFormat("-({0} *) getInfo{1}{{", getInfoClass(tabNames[i]), getPkParams(dbTabs[i].PrimaryKey)); dbM.Append(NewLine);
            dbM.Append("sqlite3 *database;"); dbM.Append(NewLine);
            dbM.AppendFormat("{0} *info=[[{0} alloc] init];",getInfoClass(tabNames[i])); dbM.Append(NewLine);
            dbM.Append("if(sqlite3_open([sDatabaseName UTF8String],&database)==SQLITE_OK){"); dbM.Append(NewLine);
            dbM.Append("    NSMutableString *sql=[[NSMutableString alloc] initWithString:@\"\"];"); dbM.Append(NewLine);
            dbM.AppendFormat("    [sql appendString:@\"select * from {0} where {1} \"];", tabNames[i], getPkSqlWhere(dbTabs[i].PrimaryKey)); dbM.Append(NewLine);       
            dbM.Append("    sqlite3_stmt *stm;"); dbM.Append(NewLine);
            dbM.Append("    if(sqlite3_prepare_v2(database,[sql UTF8String],-1,&stm,NULL)==SQLITE_OK){"); dbM.Append(NewLine);
            for (int j = 0; j < dbTabs[i].PrimaryKey.Length; j++) {
                dbM.Append(getBindDbCol(dbTabs[i].PrimaryKey[j], j + 1));
            }
            dbM.Append("        NSMutableArray *muArr = [self getArr:stm];"); dbM.Append(NewLine);
            dbM.Append("        if (muArr.count > 0) {"); dbM.Append(NewLine);
            dbM.AppendFormat("            info = ({0} *)[muArr objectAtIndex:0];", getInfoClass(tabNames[i])); dbM.Append(NewLine);
            dbM.Append("        }"); dbM.Append(NewLine);
            dbM.Append("    }"); dbM.Append(NewLine);
            dbM.Append("    sqlite3_finalize(stm);"); dbM.Append(NewLine);
            dbM.Append("}"); dbM.Append(NewLine);
            dbM.Append("sqlite3_close(database);"); dbM.Append(NewLine);
            dbM.Append("return info;"); dbM.Append(NewLine);
            dbM.Append("}"); dbM.Append(NewLine);
            dbM.Append(NewLine);

            //addNew
            dbM.AppendFormat("-(void)addNew:({0} *)info{{", getInfoClass(tabNames[i])); dbM.Append(NewLine);
            dbM.Append("    sqlite3 *database;"); dbM.Append(NewLine);
            dbM.Append("    if(sqlite3_open([sDatabaseName UTF8String],&database)==SQLITE_OK){"); dbM.Append(NewLine);
            dbM.Append("        NSMutableString *sql=[[NSMutableString alloc] initWithString:@\"\"];"); dbM.Append(NewLine);
            StringBuilder dbM2 = new StringBuilder();
            StringBuilder dbM3 = new StringBuilder();
            for (int j = 0; j < dbTabs[i].Columns.Count; j++)
            {
                if (!dbTabs[i].Columns[j].AutoIncrement)
                {
                    if (dbM2.ToString() != "")
                    {
                        dbM2.Append(",");
                    }
                    dbM2.Append(dbTabs[i].Columns[j].ColumnName);

                    if (dbM3.ToString() != "")
                    {
                        dbM3.Append(",");
                    }
                    dbM3.Append("?");
                }
            }
            dbM.AppendFormat("        [sql appendString:@\"insert into {0} ({1}) values ({2}) \"];", tabNames[i], dbM2.ToString(), dbM3.ToString()); dbM.Append(NewLine);        
            dbM.Append("        sqlite3_stmt *stm;"); dbM.Append(NewLine);
            dbM.Append("        if(sqlite3_prepare_v2(database,[sql UTF8String]  , -1,&stm,NULL)==SQLITE_OK){"); dbM.Append(NewLine);

            int k = 0;
            for (int j = 0; j < dbTabs[i].Columns.Count; j++)
            {
                if (!dbTabs[i].Columns[j].AutoIncrement)
                {
                    dbM.Append(getBindDbCol(dbTabs[i].Columns[j], j + 1 - k));
                }
                else {
                    k++;
                }
            }
            dbM.Append("        }"); dbM.Append(NewLine);
            dbM.Append("        if(SQLITE_DONE != sqlite3_step(stm))"); dbM.Append(NewLine);
            dbM.Append("        {"); dbM.Append(NewLine);
            dbM.AppendFormat("            NSLog(@\"error insert {0}\");", tabNames[i]); dbM.Append(NewLine);
            dbM.Append("        }"); dbM.Append(NewLine);
            dbM.Append("        sqlite3_reset(stm);"); dbM.Append(NewLine);
            dbM.Append("    }"); dbM.Append(NewLine);
            dbM.Append("    sqlite3_close(database);"); dbM.Append(NewLine);
            dbM.Append("}"); dbM.Append(NewLine);
            dbM.Append(NewLine);

            //update
            dbM.AppendFormat("-(void)update:({0} *)info{{", getInfoClass(tabNames[i])); dbM.Append(NewLine);
            dbM.Append("    sqlite3 *database;"); dbM.Append(NewLine);
            dbM.Append("    if(sqlite3_open([sDatabaseName UTF8String],&database)==SQLITE_OK){"); dbM.Append(NewLine);
            dbM.Append("        NSMutableString *sql=[[NSMutableString alloc] initWithString:@\"\"];"); dbM.Append(NewLine);

            StringBuilder dbM4 = new StringBuilder();
            for (int j = 0; j < dbTabs[i].Columns.Count; j++)
            {
                bool ispk= false;
                foreach(DataColumn dc in dbTabs[i].PrimaryKey){
                    if (dc.ColumnName == dbTabs[i].Columns[j].ColumnName){   
                        ispk = true;
                    }
                }
                if(!ispk){
                    if(dbM4.ToString()!=""){
                        dbM4.Append(","); 
                    }
                    dbM4.AppendFormat("{0} = ? ",dbTabs[i].Columns[j].ColumnName);
                }
            }
            dbM.AppendFormat("        [sql appendString:@\"update {0} set {1} where {2}\"];",tabNames[i],dbM4.ToString(),getPkSqlWhere(dbTabs[i].PrimaryKey)); dbM.Append(NewLine);  
            dbM.Append("        sqlite3_stmt *stm;"); dbM.Append(NewLine);
            dbM.Append("        if(sqlite3_prepare_v2(database,[sql UTF8String], -1, &stm, NULL)==SQLITE_OK){"); dbM.Append(NewLine);

            int k2 = 0;
            int k3 = 0;
           for (int j = 0; j < dbTabs[i].Columns.Count; j++)
            {
                bool ispk= false;
                foreach(DataColumn dc in dbTabs[i].PrimaryKey){
                    if (dc.ColumnName == dbTabs[i].Columns[j].ColumnName){   
                        ispk = true;
                        k2++;
                    }
                }
                if(!ispk){                                         
                    dbM.Append(getBindDbCol(dbTabs[i].Columns[j], j + 1 - k2));
                    k3 = j + 1 - k2;
                }
            }

            for(int j=0;j<dbTabs[i].PrimaryKey.Length;j++){ 
                dbM.Append(getBindDbCol(dbTabs[i].Columns[j], j + 1 + k3));
            }

            dbM.Append("        }"); dbM.Append(NewLine);
            dbM.Append("        if(SQLITE_DONE != sqlite3_step(stm))"); dbM.Append(NewLine);
            dbM.Append("        {"); dbM.Append(NewLine);
            dbM.AppendFormat("            NSLog(@\"error update {0}\");",tabNames[i]); dbM.Append(NewLine);
            dbM.Append("        }"); dbM.Append(NewLine);
            dbM.Append("        sqlite3_reset(stm);"); dbM.Append(NewLine);
            dbM.Append("    }"); dbM.Append(NewLine);
            dbM.Append("    sqlite3_close(database);"); dbM.Append(NewLine);
            dbM.Append("}"); dbM.Append(NewLine);
            dbM.Append(NewLine);

            //delete
            dbM.AppendFormat("-(void)delete{0}{{", getPkParams(dbTabs[i].PrimaryKey)); dbM.Append(NewLine);
            dbM.Append("    sqlite3 *database;"); dbM.Append(NewLine);
            dbM.Append("    if(sqlite3_open([sDatabaseName UTF8String],&database)==SQLITE_OK){"); dbM.Append(NewLine);
            dbM.Append("        NSMutableString *sql=[[NSMutableString alloc] initWithString:@\"\"];"); dbM.Append(NewLine);
            dbM.AppendFormat("        [sql appendString:@\"delete from {0} where {1}\"];", tabNames[i], getPkSqlWhere(dbTabs[i].PrimaryKey)); dbM.Append(NewLine);
            dbM.Append("        sqlite3_stmt *stm;"); dbM.Append(NewLine);
            dbM.Append("        if(sqlite3_prepare_v2(database,[sql UTF8String], -1, &stm, NULL)==SQLITE_OK){"); dbM.Append(NewLine);

            for (int j = 0; j < dbTabs[i].PrimaryKey.Length; j++)
            {
                dbM.Append(getBindDbCol(dbTabs[i].PrimaryKey[j], j + 1));
            }

            dbM.Append("        }"); dbM.Append(NewLine);
            dbM.Append("        if(SQLITE_DONE != sqlite3_step(stm))"); dbM.Append(NewLine);
            dbM.Append("        {"); dbM.Append(NewLine);
            dbM.AppendFormat("            NSLog(@\"error delete {0} \");", tabNames[i]); dbM.Append(NewLine);
            dbM.Append("        }"); dbM.Append(NewLine);
            dbM.Append("        sqlite3_reset(stm);"); dbM.Append(NewLine);
            dbM.Append("    }"); dbM.Append(NewLine);
            dbM.Append("    sqlite3_close(database);"); dbM.Append(NewLine);
            dbM.Append("}"); dbM.Append(NewLine);
            dbM.Append(NewLine);

            //getArr
            dbM.Append("-(NSMutableArray *) getArr:(sqlite3_stmt *)stm{"); dbM.Append(NewLine);
            dbM.Append("    NSMutableArray *muArr =  [[NSMutableArray alloc] init];"); dbM.Append(NewLine);
            dbM.Append("    while(sqlite3_step(stm)==SQLITE_ROW){"); dbM.Append(NewLine);
            dbM.AppendFormat("        {0} *info=[[{0} alloc] init];", getInfoClass(tabNames[i])); dbM.Append(NewLine);

            for (int j = 0; j < dbTabs[i].Columns.Count; j++)
            {
                dbM.Append(getReadDbCol(dbTabs[i].Columns[j], j));
            }

            dbM.Append("        [muArr addObject:info];"); dbM.Append(NewLine);
            dbM.Append("    }"); dbM.Append(NewLine);
            dbM.Append("    return muArr;"); dbM.Append(NewLine);
            dbM.Append("}"); dbM.Append(NewLine);

            dbM.Append(NewLine);

            dbM.Append("@end"); dbM.Append(NewLine);
            dbM.Append(NewLine);
        }
        TextBox4.Text = dbM.ToString();
    }
}