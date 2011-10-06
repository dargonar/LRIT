<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<DataCenterDataAccess.ActiveShipPositionRequest>>" %>
<%@ Import Namespace="LRITUi"%>


<asp:Content ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
   
    <h2>Estado actual de Barcos</h2>
	<table cellpadding="0" cellspacing="0" id="table_liquid">
	    <tr>
			<th style="border-left: 1px solid #959595;" >Nombre</th>
			<th>OMI ID</th>
			<th>Ultima Posicion</th>
			<th>Fecha</th>
		</tr>
		<% 
		for (int i = 0; i < (int)ViewData["total_barcos"]; i++)
        {
            var css_class = "alt";
            if (i % 2 != 0) css_class = "spec";

            if ((string)ViewData["State" + i] != "normal") css_class = (string)ViewData["State" + i];
        
         %>  
            <tr >
		    <td style="border-left: 1px solid #959595;" class="<%= css_class %>" ><%=(string)ViewData["Nombre"+i] %> </td>
			<td class="<%= css_class %>" ><%=(string)ViewData["OMIId"+i] %> </td>
			<td class="<%= css_class %>" ><%=(string)ViewData["UltimaPosicion"+i] %> </td>
			<td class="<%= css_class %>" ><%=(string)ViewData["Fecha"+i] %> </td>
		</tr>
        <%}%>
	<caption><font color="red">No hay comunicaci&oacute;n</font>/<font color="green">Requerimiento activo</font>/<font color="blue">Dentro de Standing Order</font></caption>
	</table>
	<br /><br />


    <h2>Estado de los segmentos de comunicaci&oacute;n</h2>
	<table class="status" cellpadding="0" cellspacing="0">
    	<tr>
			<th style="border-left: 1px solid #959595;" >Segmento</th>
			<th>Estado</th>
			<th>Ultima actualizacion</th>
		</tr>
		<tr>
		    <td style="border-left: 1px solid #959595;" >Request Activos</td>
		</tr>

	</table>
	<br /><br />

	<h2>Tráfico</h2>
	<table cellpadding="0" cellspacing="0" class="status">
    <tr>
		<th class="nobg" >&nbsp;</th>
    	<th>Dia</th>
		<th>Semana</th>
		<th>Mes</th>
	</tr>
	<tr>
		<td style="border-left: 1px solid #959595;" >IN</td>
	</tr>
	<tr>
		<td style="border-left: 1px solid #959595;" >OUT</td>
	</tr>
	</table>

    </div>
   
</asp:Content>



