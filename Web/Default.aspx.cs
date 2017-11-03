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
using System.IO;
using System.Text;
public partial class _Default : basePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {


    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        UpDBFile();
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        ViewInfo();
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e) {
        if (e.Row.RowType == DataControlRowType.DataRow){
            Literal Literal1 = e.Row.FindControl("Literal1") as Literal;
            string tname = e.Row.DataItem as string;
            Literal1.Text = this.getObjectName(tname);
        }
    }
    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) {
            Literal Literal1 = e.Row.FindControl("Literal1") as Literal;
            DataColumn dc = e.Row.DataItem as DataColumn;
            Literal1.Text = dc.DataType.Name;

            Literal Literal2 = e.Row.FindControl("Literal2") as Literal;
            Literal2.Text = this.getObjectName(dc.ColumnName);
        }
    }

    private void UpDBFile() { 
        if (File.Exists(string.Format("{0}\\DbFile\\{1}", Server.MapPath("."), FileUpload1.FileName)))
        {
            File.Delete(string.Format("{0}\\DbFile\\{1}", Server.MapPath("."), FileUpload1.FileName));
        }

        FileUpload1.SaveAs(string.Format("{0}\\DbFile\\{1}", Server.MapPath("."), FileUpload1.FileName));

        if (File.Exists(string.Format("{0}\\DbFile\\{1}", Server.MapPath("."), FileUpload1.FileName))) {
            this.DBFileName = FileUpload1.FileName;
            GetCode();
        }
    }
    private void GetCode() {
        IList<string> tabNames = this.getAllTableName();

        foreach (string tabName in tabNames) {
            SetTableCode(tabName);
        }

        DataTable dt = this.getTableSchema(tabNames[tabNames.Count - 1]);
        
        GridView2.DataSource = dt.Columns;
        GridView2.DataBind();

        GridView3.DataSource = dt.PrimaryKey;
        GridView3.DataBind();
    }
    private void ViewInfo() {
        IList<string> tabNames = this.getAllTableName();

        GridView1.DataSource = tabNames;
        GridView1.DataBind();

        //DataTable dt = this.getTableSchema("question_d");
        DataTable dt = this.getTableSchema("test_table");
        //DataTable dt = this.getTableSchema("aaa");
        //DataTable dt = this.getTableSchema(tabNames[tabNames.Count - 1]);

        GridView2.DataSource = dt.Columns;
        GridView2.DataBind();

        GridView3.DataSource = dt.PrimaryKey;
        GridView3.DataBind();
    }

    private StringBuilder hFileStr = new StringBuilder();
    private StringBuilder mFileStr = new StringBuilder();
    
    private void SetTableCode(string tabName) {
        DataTable dt = this.getTableSchema(tabName);


        //dt.Columns;

        //dt.PrimaryKey;
    }
}
