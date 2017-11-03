<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>未命名頁面</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button2" runat="server" Text="瀏覽" OnClick="Button2_Click" />
        <br />
        AllTableNames<br />
        <asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        ObjectName
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                </Columns>
        </asp:GridView>
        <br />
        Columns<br />
        <asp:GridView ID="GridView2" runat="server" OnRowDataBound="GridView2_RowDataBound">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        DataType.Name
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        ObjectName
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br />
        PrimaryKey<br />
        <asp:GridView ID="GridView3" runat="server"></asp:GridView>
        <br />

        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="上傳" />
    </div>
    </form>
</body>
</html>
