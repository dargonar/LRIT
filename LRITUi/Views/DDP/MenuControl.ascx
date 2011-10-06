<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="LRITUi"%>

            <h3>Menu</h3>
			<ul class="nav">
                <% 
                    foreach (var DDPVersion in (List<DataCenterDataAccess.DDPVersion>)ViewData["DDPVersions"])
                   {
                       // Html.ActionLink(DDPVersion.DDPVersion1, "List", new { ddpid = DDPVersion.Id });
                       Response.Write(Helpers.ListElement(Url.Action("List", "DDP", new { ddpid = DDPVersion.Id }), DDPVersion.DDPVersion1));
                    }
                 %>

			</ul>

