<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Cambio de Contraseña
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
        <h2>Cambio de Contraseña</h2>
        <% using (Html.BeginForm()) { %>
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
                <label for="newPassword">Nueva Contraseña</label>
                <%= Html.Password("newPassword", "", new { @class = "input-medium" })%>
            </p>
            <p>
                <label for="confirmPassword">Repetir Nueva Contraseña</label>
                <%= Html.Password("confirmPassword", "", new { @class = "input-medium" })%>
            </p>
            <p>
                <input type="submit" value="Cambiar" class="button"/>
            </p>
        <% } %>
    </div>
</asp:Content>
