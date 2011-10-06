<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DataCenterDataAccess.Contract>" %>
<%@ Import Namespace="LRITUi"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ViewData["title"]%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            $("#lastInvoice, #lastInvoiceRecv").datepicker({ dateFormat: 'yy-mm-dd' });
        });
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <h2><%=ViewData["title"]%></h2>

    <% using (Html.BeginForm("Create","Contract")) {%>

            <% if(ViewData.Keys.Contains("flash") ) { %>
            <div class="message <%=ViewData["flash_type"]%>">
                <p><strong><%=ViewData["flash"]%></strong></p>
            </div>
            <% } %>

            <!--ID-->
            <input type="hidden" name="id" value="<%=Model.id %>" />
            
            <p>
                <label for="lritid">LRITID (Data User Requestor):</label>
                <%= Html.TextBox("lritid", Model.lritid, new { @class = "input-small" } ) %>
                <%= Html.ErrorBR("lritid", "invalido", false)%>
            </p>
            <p>
                <label for="name">Nombre (Pais)</label>
                <%= Html.TextBox("name", Model.name, new { @class = "input-medium" }) %>
                <%= Html.ErrorBR("name", "invalido", false)%>
            </p>
            <p>
                <label for="minimun">Minimo (de facturación en USD)</label>
                <%= Html.TextBox("minimun", Model.minimun, new { @class = "input-small" }) %>
                <%= Html.ErrorBR("minimun", "invalido", false)%>
            </p>
            <p>
                <label for="period">Periodo (de facturación)</label>
                <%= Html.TextBox("period", Model.period, new { @class = "input-small" }) %>
                <%= Html.ErrorBR("period", "invalido", false)%>
            </p>
            
            <p  <% if (Model.id == 0) { %> style="display:none" <% } %> >
                <label for="lastInvoice">Ultima factura (acc):</label>
                <%= Html.TextBox("lastInvoice", String.Format("{0:yyyy-MM-dd}", Model.lastInvoice), new { @class = "input-small" }) %>
                <%= Html.ErrorBR("lastInvoice", "invalido", false)%>
            </p>
            <p  <% if (Model.id == 0) { %> style="display:none" <% } %> >
                <label for="lastInvoice">Ultima factura (deu):</label>
                <%= Html.TextBox("lastInvoiceRecv", String.Format("{0:yyyy-MM-dd}", Model.lastInvoiceRecv), new { @class = "input-small" }) %>
                <%= Html.ErrorBR("lastInvoiceRecv", "invalido", false)%>
            </p>
            <br /><br />
            <p>
                <input type="submit" class="button" value="Save" />
            </p>

    <% } %>

    <div>
        <%=Html.ActionLink("Ir al listado", "List") %>
    </div>

</asp:Content>