<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ViewData["title"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <h2><%=ViewData["title"] %></h2>
    
        <% using (Html.BeginForm("Change", "ExternalInvoice"))
           {%>

            <% if (ViewData.Keys.Contains("flash"))
               { %>
            <div class="message <%=ViewData["flash_type"]%>">
                <p><strong><%=ViewData["flash"]%></strong></p>
            </div>
            <% } %>
            
            <p>
                <label for="password">Password</label>
                <%=Html.Password("pass1", "", new { @class = "input-small" })%>
            </p>
            
            <p>
                <label for="password">Retype-Password</label>
                <%=Html.Password("pass2", "", new { @class = "input-small" })%>
            </p>
            <p>
                <input type="submit" class="button" value="Change" />
            </p>

            <%} %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
</asp:Content>
