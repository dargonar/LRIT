<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DataCenterDataAccess.Ship>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	New
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
        <h2><%=ViewData["shipid"] != null ? "Edicion de" : "Nuevo" %> barco</h2>

    <% using (Html.BeginForm("Create", "Ship")) {%>
            
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
            <label for="ShipName">Nombre</label>
            <%= Html.TextBox("Name", Model.Name, new { @class = "input-small" } )%>
        </p>
        <p>
            <label for="IMOnum">Numero OMI:</label>
            <%= Html.TextBox("IMOnum", Model.IMONum, new { @class = "input-small" })%>
        </p>
        <p>
            <label for="EquipID">ID del equipo</label> 
            <%= Html.TextBox("EquipID", Model.EquipID, new { @class = "input-small" })%>
        </p>
        <p>
            <label for="MMSINum">Numero MMSI</label>
            <%= Html.TextBox("MMSINum", Model.MMSINum, new { @class = "input-small" })%>
        </p>
        <p>
            <label for="Eslora">Eslora</label>
            <%= Html.TextBox("Eslora", Model.Eslora, new { @class = "input-small" })%>
        </p>
        <p>
            <label for="Manga">Manga</label>
            <%= Html.TextBox("Manga", Model.Manga, new { @class = "input-small" })%>
        </p>
        <p>
            <label for="Manga">Calado</label>
            <%= Html.TextBox("Calado", Model.Calado, new { @class = "input-small" })%>
        </p>
        
        <p>
            <label for="Tipo_Navegacion">Tipo de Navegacion</label>
            <%= Html.TextBox("Tipo_Navegacion", Model.Tipo_Navegacion, new { @class = "input-small" })%>
        </p>    
        <p>
            <label for="Tipo_Navegacion_Ingles">Tipo Navegacion en Ingles</label>
            <%= Html.TextBox("Tipo_Navegacion_Ingles", Model.Tipo_Navegacion_Ingles, new { @class = "input-small" })%>
        </p>    
        <p>
            <label for="Tipo_Buque">Tipo de Buque</label>
            <%= Html.TextBox("Tipo_Buque", Model.Tipo_Buque, new { @class = "input-small" })%>
        </p>    
        <p>
            <label for="Tipo_Buque_Ingles">Tipo de Buque en Ingles</label>
            <%= Html.TextBox("Tipo_Buque_Ingles", Model.Tipo_Buque_Ingles, new { @class = "input-small" })%>
        </p>

        <p>
            <label for="DNID">Tipo de Equipo</label>
            <% if( ViewData["shipid"] == null ) { %>
            <%= Html.DropDownList("issata", new SelectList(new [] { new {@id=0, @value="Stratos"}, new {@id=1, @value="Satamatics"} }, "id", "value", 0))%>
            <% } else { %>
            <%= Html.TextBox("issata", Model.issata != 0 ? "Satamatics" : "Stratos", new { @class = "input-small", @readonly = "readonly" })%>
            <%} %>
        </p>    

        <% if ( ViewData["shipid"] == null || Model.issata == 0)
           { %>
        <p class="satero">
            <label for="DNID">DNID (Stratos)</label>
            <%=  ViewData["shipid"] != null ? Html.TextBox("DNID", Model.DNID, new { @class = "input-small", @readonly = "readonly" }) : Html.TextBox("DNID", Model.DNID, new { @class = "input-small" })%>
        </p>    

        <p class="satero">
            <label for="Member">Miembro (Stratos)</label>
            <%= ViewData["shipid"] != null ? Html.TextBox("Member", Model.Member, new { @class = "input-small", @readonly = "readonly" }) : Html.TextBox("Member", Model.Member, new { @class = "input-small" })%>
        </p>
        <% } %>

        <p>
            <label class="lblsata" for="Mobile">Mobile (<%=ViewData["shipid"] != null && Model.issata != 0 ? "Satamatics" : "Stratos"%>)</label>
            <%= ViewData["shipid"] != null ? Html.TextBox("Mobile", Model.Mobile, new { @class = "input-small", @readonly = "readonly" }) : Html.TextBox("Mobile", Model.Member, new { @class = "input-small" })%>
        </p>    

        <p>
            <%= Html.Hidden("shipid", ViewData["shipid"]) %>
        </p>

        <input class="button" type="submit" value="<%=ViewData["shipid"] != null ? "Guardar Cambios" : "Guardar" %>" />

    <% } %>
   </div>
   <script type="text/javascript">
     $(document).ready(function() {
       $("#issata").change(function() {
         if ($("#issata").val() != 0) {
           $(".satero").css('display', 'none');
           $(".lblsata").text('Mobile (Satamatics)');
           $("#DNID").val('777');
           $("#Member").val('777');
         }
         else {
           $(".satero").css('display', 'block');
           $(".lblsata").text('Mobile (Stratos)');
         }
       });
     });
   </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
</asp:Content>
