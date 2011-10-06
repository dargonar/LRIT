<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeadInfo" runat="server">

<script language="javascript" type="text/javascript">
  $(document).ready(function() {
    $(".time").datetimepicker({ dateFormat: 'yy-mm-dd' });

    $("#RequestTypeIndex").change(function() {
      ReloadPage();
    });

    $("#AccessTypeIndex").change(function() {
      ReloadPage();
    });
  });

    function ReloadPage() {
      location.href = virtualdir + '/NewRequest/' + $("#AccessTypeIndex").val() + '/' + $("#RequestTypeIndex").val();
    }
</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
    <h2>Nueva Solicitud</h2>
   
    <% using (Html.BeginForm("CreateAndSend", "Requests")) {%>

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
                <label for="AccessTypeIndex">Tipo de Acceso:</label>
                <%= Html.DropDownList("AccessTypeIndex") %>
            </p>
            <p>
                <label for="RequestTypeIndex">Tipo de Requerimiento:</label>
                <%= Html.DropDownList("RequestTypeIndex")%>
           </p>
            <p>
                <label for="IMOnum">Numero OMI:</label> 
                <%= Html.TextBox("IMOnum", "", new { @class = "input-small" } )%>
            </p>
            <p>
                <label for="DataUserProvider">LRIT Id del DataCenter:</label> 
                <%= Html.TextBox("DataUserProvider", ViewData["DataUserProvider"], new { @class = "input-small" })%>
            </p>

            <% if ( (string)(ViewData["RequestType"]) != "1")
                   if ((string)(ViewData["RequestType"]) != "9")
               { %>
            <p>
                <label for="StartTime">Fecha Inicio:</label>
                <%= Html.TextBox("strStartTime", string.Format("{0:yyyy-MM-dd HH:mm}",DateTime.UtcNow), new { @class = "time input-small" })%>
            </p>
            <p>
                <label for="StopTime">Fecha Fin:</label>
                <%= Html.TextBox("strStopTime", string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.UtcNow.AddHours(5)), new { @class = "time input-small" })%>
            </p>
            
            <% } %>
            
            <% if (((string)ViewData["AccessType"]) == "3" || ((string)ViewData["AccessType"]) == "5")
               { %>
            
            <p>
                <label for="ItemField">Puerto o PortFacility</label>
                <%= Html.DropDownList("ItemField")%>
            </p>
            <p>
                <label for="ItemElement">Tipo</label>
                <%= Html.DropDownList("ItemElement", (IEnumerable<SelectListItem>)ViewData["ItemElement"] ,  new { @class="dist" } ) %>
            </p>
            
            <% if (((string)ViewData["AccessType"]) == "3" || ((string)ViewData["AccessType"]) == "5")
               { %>
                <p>
                    <label for="Distance">Distancia:</label>
                    <%= Html.TextBox("Distance", "", new { @class = "dist input-small" })%>
                </p>
            
            <% } %>
            
            <% } %>
            <p>
                <input class="button" type="submit" value="Enviar" />
            </p>


    <% } %>
   
   </div>
</asp:Content>



