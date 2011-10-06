<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Facturar : Seleccione el país
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <h2>Seleccione el país al que desea facturar</h2>
    <% using(Html.BeginForm("New", "Invoice", FormMethod.Get)) { %>
    <% var comboc = new SelectList((IEnumerable)ViewData["lrit_countries"], "id", "value"); %>
    <p>
        <%=Html.DropDownList("contract", comboc, new { @class = "input-small" })%>
        <%=Html.Hidden("emireci","0")%>
    </p>
    <p>
        <input type="submit" class="button" value="Facturar" />
    </p>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
</asp:Content>
