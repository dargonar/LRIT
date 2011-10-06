<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DataCenterDataAccess.Invoice>" %>
<%@ Import Namespace="LRITUi"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ViewData["title"]%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            $("#dateFrom, #dateTo, #isueDate").datepicker({ dateFormat: 'yy-mm-dd' });
                
            <% if (ViewData["show_calc"] != null) { %>
            <% var ocont = (DataCenterDataAccess.Contract )ViewData["ocont"]; %>
            $("#calculate").click(function() {
                //Hide link show loader
                $(this).hide();
                $("#loading").show();

                //Calculate based on contract and dates
                $.ajax({
                    url: virtualdir + '/Invoice/Calculate?contract=<%=ocont.id%>&from=' + $("#dateFrom").val() + '&to=' + $("#dateTo").val(),
                    success: function(data) {
                        $("#amount").val(data);
                        $("#calculate").show();
                        $("#loading").hide();
                    },
                    error: function(data) {
                        alert('Ocurrió un error realizando el calculo');
                        $("#calculate").show();
                        $("#loading").hide();
                    }
                });
            });
            <% } %>

        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <% int emireci = ViewData["emireci"] != null ? (int)ViewData["emireci"] : Model.emitidarecibida; %>
    <% ViewData["invoice_states"] = emireci == 0 ? ViewData["invoice_states_emi"] : ViewData["invoice_states_reci"]; %>
    
    <h2><%=ViewData["title"]%></h2>
    <% if (ViewData["show_calc"] != null) { %>
    <% var ocont = (DataCenterDataAccess.Contract )ViewData["ocont"]; %>
    <div class="content-box box2" style="float:right">
        <h4>Detalles del contrato</h4><br />
        <p>* Monto minimo de facturación <strong>USD <%=ocont.minimun%></strong></p>
        <p>* Frecuencia de facturación <strong><%=ocont.period%></strong></p>
    </div>
    <br />
    <% }%>
                                    
    <% using (Html.BeginForm("Create", "Invoice", FormMethod.Post, new { @enctype = "multipart/form-data" }))
       {%>

            <% if (ViewData.Keys.Contains("flash"))
               { %>
            <div class="message <%=ViewData["flash_type"]%>">
                <p><strong><%=ViewData["flash"]%></strong></p>
            </div>
            <% } %>

            <!--ID-->
            <input type="hidden" name="id" value="<%=Model.id %>" />
            <input type="hidden" name="emitidarecibida" value="<%=emireci%>" />
            <% if (Model.id == 0)
               {%> 
            <input type="hidden" name="state" value="0" />
            <% } %>
            
            <% if (emireci == 1)
               {%> 
            <p>
                <label for="contract_id">Pais</label>
                <% var comboc = new SelectList((IEnumerable)ViewData["lrit_countries"], "id", "value", Model.contract_id); %>
                <%=Html.DropDownList("contract_id", comboc, new { @class = "input-small" })%>
            </p>
            <% }
               else
               { %>
                <%=Html.Hidden("contract_id", Model.contract_id)%>
            <% } %>
            
            <%if (Model.id != 0)
              {%>
            <p>
                <label for="state">Estado</label>
                <% var combos = new SelectList((IEnumerable)ViewData["invoice_states"], "id", "value", Model.state); %>
                <%=Html.DropDownList("state", combos, new { @class = "input-small" })%>
            </p>
            <%}%>
            <p>
                <label for="invoiceNumber">Numero de factura</label>
                <%= Html.TextBox("invoiceNumber", Model.invoiceNumber, new { @class = "input-small" })%>
                <%= Html.ErrorBR("invoiceNumber", "invalido", false)%>
            </p>
            <p>
                <label for="invoiceNumber">Fecha de emisión</label>
                <%= Html.TextBox("isueDate", String.Format("{0:yyyy-MM-dd}", Model.isueDate), new { @class = "input-small" })%>
                <%= Html.ErrorBR("isueDate", "invalido", false)%>
            </p>
            <p>
                <label for="dateFrom">Periodo facturado (desde-hasta)</label>
                <%= Html.TextBox("dateFrom", String.Format("{0:yyyy-MM-dd}", Model.dateFrom), new { @class = "input-small" })%>
                <%= Html.TextBox("dateTo", String.Format("{0:yyyy-MM-dd}", Model.dateTo), new { @class = "input-small" })%>
                <%= Html.ErrorBR("dateFrom", "Desde: invalido", false)%>
                <%= Html.ErrorBR("dateTo", "Hasta: invalido", false)%>
            </p>
            <p>
                <label for="amount">Monto/Moneda</label>
                <%= Html.TextBox("amount", String.Format("{0:F}", Model.amount), new { @class = "input-small" })%>
                <%= Html.TextBox("currency", Model.currency, new { @class = "input-tiny" })%>
                <%= Html.ErrorBR("amount", "Monto: invalido", false)%>
                <%= Html.ErrorBR("currency", "Moneda: invalido", false)%>
                <% if (ViewData["show_calc"] != null)
                   { %>
                <a id="calculate" href="#">Calcular</a>
                <img style="display:none" id="loading" alt="Espere por favor..." src="<%= Url.Content("~/img/ajax-loader.gif") %>" />
                <%}%>
            </p>
            <p>
                <label for="transfercost">Costo de transferencia</label>
                <%= Html.TextBox("transfercost", String.Format("{0:F}", Model.transfercost), new { @class = "input-tiny" })%>
                <%= Html.ErrorBR("transfercost", "Costo: invalido", false)%>
            </p>
            <p>
                <label for="interests">Intereses</label>
                <%= Html.TextBox("interests", String.Format("{0:F}", Model.interests), new { @class = "input-tiny" })%>
                <%= Html.ErrorBR("interests", "Intereses: invalido", false)%>
            </p>
            <p>
                <label for="bankreference">Referencia bancaria</label>
                <%= Html.TextBox("bankreference", Model.bankreference, new { @class = "input-medium" })%>
                <%= Html.ErrorBR("bankreference", "Referencia: invalida", false)%>
            </p>
            <p>
                <label for="notes">Notas</label>
                <%= Html.TextArea("notes", Model.notes)%>
                <%= Html.ErrorBR("notes", "Notas: invalida", false)%>
            </p>
            <% if (emireci == 1) {%> 
            <p>
                <label for="notes">Adjuntar documento</label>
                <input type="file" name="invoicefile" value="Buscar"/>
                <% if (Model.invoiceFile_id != null) { %>
                <%  var file = Model.InvoiceFile; %>
                <strong><a href="<%=Url.Action("DownloadInvoiceFile", "Invoice")%>?fileid=<%=file.id%>"><%=file.fileName%></a></strong>
                <% } %>
            </p>
            <% } %>
            <p>
                <input type="submit" class="button" value="Salvar" />
            </p>
    <% } %>

    <div>
        <a href="<%= Url.Action("List", "Invoice" )%>?emireci=<%=emireci%>">Volver al listado</a>
    </div>

</asp:Content>
