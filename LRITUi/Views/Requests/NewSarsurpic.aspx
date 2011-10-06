<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadInfo" runat="server">
<script language="javascript" type="text/javascript">
    $(document).ready(function() {
        $("#AreaIndex").change(function() { ReloadPage(); });
    
        $("#latitud").mask("99.99.S");
        $("#longitud").mask("999.99.W");
        <% if ((string)(ViewData["area"]) == "0") { %>
        $("#radio").mask("999");
        <% } %>

        <% if ((string)(ViewData["area"]) == "1") { %>
        $("#offnorte").mask("99.99.N");
        $("#offeste").mask("999.99.E");
        <% }  %>
    });

    function ReloadPage() {
        if ($("#AreaIndex").val() == 0)
            location.href = '<%= Url.Action("NewCircularSarsurpic","Requests") %>'
        else
            location.href = '<%= Url.Action("NewRectangularSarsurpic","Requests") %>'
    }
</script>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
    <h2>Nueva Solicitud SARSURPic</h2>

    <% using (Html.BeginForm("CreateAndSendSarsurpic", "Requests")) {%>

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
                <label for="DataUserRequestor">SAR Service</label>
                <% var comboc = new SelectList((IEnumerable)ViewData["sar_services"], "id", "value"); %>
                <%=Html.DropDownList("DataUserRequestor", comboc, new { @class = "input-big" })%>
            </p>

            <p>
                <label for="AreaIndex">Tipo de area</label>
                <%= Html.DropDownList("AreaIndex") %>
            </p>
            
            <p>
                <label for="Lat">Latitud (00.00.N/S)</label>
                <%= Html.TextBox("Lat", "", new { @id = "latitud", @class = "time input-small" })%>
            </p>
            <p>
                <label for="Long">Longitud (000.00.E/W)</label>
                <%= Html.TextBox("Long", "", new { @id = "longitud", @class = "time input-small" })%>
            </p>          
            
            <% if ((string)(ViewData["area"]) == "0")
               { %>
            <p>
                <label for="var1">Radio en MN (000)</label>
                <%= Html.TextBox("var1", "", new { @id = "radio", @class = "time input-small" })%>
            </p>
            
            <% }  %>

            <% if ((string)(ViewData["area"]) == "1")
               { %>
            <p>
                <label for="var1">Offset Norte (00.00.N)</label>
                <%= Html.TextBox("var1", "", new { @id="offnorte", @class = "time input-small" })%>
            </p>
            <p>
                <label for="var2">Offset Este (000.00.E)</label>
                <%= Html.TextBox("var2", "", new { @id="offeste", @class = "time input-small" })%>
            </p>
          
          
            <% }  %>
            <p>
                <label for="NumberOfPositions">Cantidad de Posiciones</label>
                <%= Html.DropDownList("NumberOfPositions")%>
            </p>
            
            <p>
                <input class="button" type="submit" value="Enviar" />
            </p>
    <% } %>
   
   </div>
</asp:Content>



