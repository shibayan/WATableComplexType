<%@ Page Title="ホーム ページ" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebRole1._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        ASP.NET へようこそ!
    </h2>
    <p>
        ASP.NET の詳細については、<a href="http://www.asp.net" title="ASP.NET Web サイト">www.asp.net</a> を参照してください。
    </p>
    <p>
        <a href="http://go.microsoft.com/fwlink/?LinkID=152368"
            title="MSDN ASP.NET ドキュメント">MSDN で ASP.NET に関するドキュメント</a>を参照することもできます。
    </p>
    <asp:ListView runat="server" ID="ListView1">
        <LayoutTemplate>
            <table runat="server" id="table1" width="640px" border="1">
                <tr runat="server">
                    <th runat="server">
                        Comment.Text
                    </th>
                    <th runat="server">
                        Comment.User.Name
                    </th>
                    <th runat="server">
                        Comment.Tags
                    </th>
                </tr>
                <tr runat="server" id="itemPlaceholder">
                </tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr runat="server">
                <td>
                    <%# Eval("Text") %>
                </td>
                <td>
                    <%# Eval("User.Name") %>
                </td>
                <td>
                    <%# string.Join(",", (IEnumerable<string>)Eval("Tags")) %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
    <asp:Button runat="server" Text="追加" />
</asp:Content>
