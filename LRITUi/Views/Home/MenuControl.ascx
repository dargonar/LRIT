<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="LRITUi"%>

			<h3>Menu</h3>
			<ul class="nav">
				<li><a href="#">Nueva Solicitud</a></li>
				<li class="last"><%= Html.RouteLink("Ver Solicitudes", "Reports", new { controller = "Reports", action = "List", inout = 0, page = 1 } )%></li>
			</ul>
