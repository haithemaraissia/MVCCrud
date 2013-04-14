<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tblProduct>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Product
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <%= ((ComponentGrid)ViewData["ComponentGrid"]).renderIncludes %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Edit Product</h2>
    <% Html.RenderPartial("CreateEdit"); %>
    <hr />
    <h2>Components</h2>
    <% using (Html.BeginForm("ComponentAdd", "Product"))
       { %>
        Add Component <%= Html.TextBox("name", "") %> <%= Html.Hidden("product_id", Model.id) %> <input type="submit" value="+" />
    <% } %>
    <%= ((ComponentGrid)ViewData["ComponentGrid"]).renderGrid %>
</asp:Content>
