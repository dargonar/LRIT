<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DataCenterDataAccess.Price>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ViewData["title"]%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
<script type="text/javascript">
    $(document).ready(function() {
        $("#effectiveDate").datepicker({ dateFormat: 'yy-mm-dd' });
    });
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <h2><%=ViewData["title"]%></h2>
<br />
<% if(ViewData.Keys.Contains("flash") ) { %>
<div class="message <%=ViewData["flash_type"]%>">
    <p><strong><%=ViewData["flash"]%></strong></p>
</div>
<% } %>
<div style="clear:both;"></div>
<% var price = (DataCenterDataAccess.Price)ViewData["price"]; %>
<div style="float:left" class="content-box box-grey">
<h4>Publicado <%=string.Format("{0:yyyy-MM-dd}",price.issueDate) %>, valido desde <strong><%=string.Format("{0:yyyy-MM-dd}",price.effectiveDate) %></strong></h4>
<hr />
<%Html.RenderPartial("PriceTable", price);%>
</div>
<br />
<div style="clear:both;"></div>
<br />
<% if(ViewData["last_update"] != null) { %>
<div style="float:left" class="content-box">
<h4>Ultimo envio de precios <%=string.Format("{0:yyyy-MM-dd}",ViewData["last_update"])%></h4>
</div>
<%}%>
<br />
<div style="clear:both;"></div>
<br />
<h2>Cambio de precio</h2>
    <% using (Html.BeginForm("Change", "Pricing"))
       {%>
            <p>
                <label for="effectiveDate">Fecha para hacer efectivos los precios</label>
                <%= Html.TextBox("effectiveDate", String.Format("{0:yyyy-MM-dd}", Model.effectiveDate), new { @class = "input-small" })%>
            </p>
            <p>
                <label for="currency">Moneda</label>
                <%= Html.TextBox("currency", "USD", new { @class = "input-small", @readonly="readonly" })%>
            </p>
            <p>
                <label for="PeriodicRateChange">PeriodicRateChange</label>
                <%= Html.TextBox("PeriodicRateChange", String.Format("{0:F}", Model.PeriodicRateChange), new { @class = "input-small" })%>
            </p>
            <p>
                <label for="Poll">Poll</label>
                <%= Html.TextBox("Poll", String.Format("{0:F}", Model.Poll), new { @class = "input-small" })%>
            </p>
            <p>
                <label for="PositionReport">PositionReport</label>
                <%= Html.TextBox("PositionReport", String.Format("{0:F}", Model.PositionReport), new { @class = "input-small" })%>
            </p>
            <p>
                <label for="ArchivePositionReport">ArchivePositionReport</label>
                <%= Html.TextBox("ArchivePositionReport", String.Format("{0:F}", Model.ArchivePositionReport), new { @class = "input-small" })%>
            </p>
            <p>
                <input type="submit" class="button" value="Enviar" />
            </p>

        <%}%>

</asp:Content>
