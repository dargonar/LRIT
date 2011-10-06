<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Lista de Usuarios
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
        <h2>Lista de Usuarios</h2>
        <table id="table_auto" cellspacing="0">
		    <tr>
		    <th style="border-left: 1px solid #959595;" >Nombre</th>
            <th>Rol</th>
            <th colspan="2">Acciones</th>
	        </tr>
		    <%
            
            int i = 0;  
	        foreach (MembershipUser user in (MembershipUserCollection)ViewData["users"])
            {
                var css_class = "alt";
                if (i++ % 2 != 0) css_class = "spec";
                var rol = Roles.GetRolesForUser(user.UserName)[0];
              
   		    %>  <tr>
				    <td style="border-left: 1px solid #959595;" class="<%= css_class %>" ><%=user.UserName %> </td>
				    <td class="<%= css_class %>" > <%=rol %></td>
				    <td class="<%= css_class %>" ><a href="<%= Url.Action("ChangePassword", "Account", new { id = user.ProviderUserKey }) %>">Cambiar Password</a></td>
				    <td class="<%= css_class %>" ><%if (user.UserName != "admin") { %><a onclick="return confirm('Esta seguro que desea borrar el usuario?');" href="<%= Url.Action("Delete","Account", new {id = user.ProviderUserKey} ) %>" >Borrar</a><% } %></td>
			    </tr>
          <%}%>
        </table>
        <br />
        <input type="submit" class="button" value="Nuevo" onclick="javascript: location.href='<%= Url.Action("Register", "Account") %>';"/>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
</asp:Content>
