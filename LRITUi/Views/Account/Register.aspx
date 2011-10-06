<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="LRITUi.Controllers" %>
<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Crear Nuevo Usuario
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
    <h2>Crear Nuevo Usuario</h2>
    <% using (Html.BeginForm("Register","Account")) { %>
        <% foreach (var state in ViewData.ModelState)
           {
               foreach (var error in ViewData.ModelState[state.Key].Errors)
               {%>
                 <div class="message error">
                    <p><strong><%=error.ErrorMessage%></strong></p>
                 </div>
            <% }
           } 
        %>

                <p>
                    <label for="username">Nombre del usuario:</label>
                    <%= Html.TextBox("username", "", new { @class = "input-medium" } )%>
                </p>
                <p>
                    <label for="email">Email:</label>
                    <%= Html.TextBox("email", "", new { @class = "input-medium" })%>
                </p>
                <p>
                    <label for="password">Password:</label>
                    <%= Html.Password("password", "", new { @class = "input-medium" })%>
                </p>
                <p>
                    <label for="confirmPassword">Confirmar password:</label>
                    <%= Html.Password("confirmPassword", "", new { @class = "input-medium" })%>
                </p>
                <p>
                    <label for="roleName">Elija el Rol:</label>
                    <%= Html.DropDownList("roleName")%>
                </p>
                <p>
                    <input type="submit" value="Agregar" class="button"/>
                </p>
    </div>
    <% } %>
</asp:Content>
