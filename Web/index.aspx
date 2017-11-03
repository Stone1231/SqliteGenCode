<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        請上傳sqlite檔：<asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="取得objC程式" />
        <br />
        <font color="red">注意!暫時把 BOOL 存取使用sqlite3_column_int,sqlite3_bind_int  <br /> REAL, NUMERIC 存取使用sqlite3_column_double,sqlite3_bind_double</font>
        <br />Info.h<br />
        <asp:TextBox ID="TextBox1" runat="server" Columns="100" Rows="32" TextMode="MultiLine"></asp:TextBox>

        <br />Info.m<br />
        <asp:TextBox ID="TextBox2" runat="server" Columns="100" Rows="32" TextMode="MultiLine"></asp:TextBox>

        <br />DB.h<br />
        <asp:TextBox ID="TextBox3" runat="server" Columns="100" Rows="32" TextMode="MultiLine"></asp:TextBox>

        <br />DB.m<br />
        <asp:TextBox ID="TextBox4" runat="server" Columns="100" Rows="32" TextMode="MultiLine"></asp:TextBox>
    </div>
    </form>
</body>
</html>
