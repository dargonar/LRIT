<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>



<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

            <div class="content_block">
            <h2>Listado de Barcos</h2>
            
			
				<table id="table_auto" cellspacing="0">
					<tr>
						<th style="border-left: 1px solid #959595;" >Nombre</th>
						<th>ID OMI</th>
						<th>ID del Equipo</th>
						<th>Numero MMSI</th>
						<th>Calado</th>
						<th>Eslora</th>
						<th>Manga</th>
						<th>Tipo de Buque</th>
						<th>Tipo de Navegacion</th>
						<th>DNID</th>
						<th>Miembro</th>
						<th>Mobile Number</th>
						<th>Tipo</th>
						<th colspan="2">Acciones</th>
					</tr>
				<% 
				int i = 0;
				foreach (var ship in (List<DataCenterDataAccess.Ship>)ViewData["barcos"])
                {
                    string caladotrim = string.Empty;
                    if (ship.Calado != null)
                    {
                      try
                      {
                        caladotrim = int.Parse(ship.Calado.Value.ToString()).ToString();
                      }
                      catch
                      {
                        caladotrim = "";
                      }
                    }
                    var css_class = "alt";
                    if( i++ % 2 != 0 ) css_class ="spec";
           		%>  <tr>
						<td style="border-left: 1px solid #959595;" class="<%= css_class %>"><%=ship.Name %> </td>
						<td class="<%= css_class %>"><%=ship.IMONum %> </td>
						<td class="<%= css_class %>"><%=ship.EquipID %> </td>
						<td class="<%= css_class %>"><%=ship.MMSINum %> </td>
						<td class="<%= css_class %>"><%=caladotrim %> </td>
						<td class="<%= css_class %>"><%=ship.Eslora %> </td>
						<td class="<%= css_class %>"><%=ship.Manga %> </td>
						<td class="<%= css_class %>"><%=ship.Tipo_Buque %> </td>
						<td class="<%= css_class %>"><%=ship.Tipo_Navegacion %> </td>
						<td class="<%= css_class %>"><%=ship.issata!=0?"--":ship.DNID.ToString() %> </td>
						<td class="<%= css_class %>"><%=ship.issata!=0?"--":ship.Member.ToString() %> </td>
						<td class="<%= css_class %>"><%=ship.Mobile %> </td>
						<td class="<%= css_class %>"><%=ship.issata!=0?"Satamatics":"Stratos" %> </td>
						<td class="<%= css_class %>"><a href="<%= Url.Action("Edit", "Ship", new { id = ship.Id})  %>">Editar</a> </td>
						<td class="<%= css_class %>"><a onclick="return confirm('Esta seguro que desea borrar el barco?');" href="<%= Url.Action("Delete", "Ship", new { id = ship.Id})  %>" onclick="return confirm('¿Esta seguro que desea eliminar la información del barco?')">Borrar</a> </td>
					</tr>
           
       <%}%>
				</table>
				<br />
				<input type="button" class="button" onclick="javascript: location.href='<%= Url.Action("New", "Ship")%>';" value="Nuevo" />
            </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
</asp:Content>
