<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DataCenterDataAccess.Price>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ViewData["title"]%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <h2><%=ViewData["title"]%></h2>
    <br />
    <% if(ViewData.Keys.Contains("flash") ) { %>
    <div class="message <%=ViewData["flash_type"]%>">
        <p><strong><%=ViewData["flash"]%></strong></p>
    </div>
    <% } %>
    <div style="clear:both;"></div>
</asp:Content>
