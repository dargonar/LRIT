<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="JQGridHelper" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Billing - Consumo
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
<script type="text/javascript">
$(document).ready(function() {
    
    var mygrid = jQuery("#s3list").jqGrid({
        url: virtualdir + '/Contract/ListJSON',
        datatype: "json",
        autowidth: true,
        height: 548,
        colNames: ['id', 'LRITId', 'Pais', 'Fecha Desde (acc)', 'Saldo Acreedor', 'Fecha Desde (deu).', 'Saldo Deudor', 'Periodo (fact)'],
        colModel: [
	                { name: 'id', index: 'id', width: 50, hidden: true },
	                { name: 'lritid', index: 'lritid', width: 50 },
	                { name: 'name', index: 'name', width: 50 },
	                { name: 'lastInvoice', index: 'lastInvoice', width: 50,
	                    searchoptions: {
	                        dataInit: function(el) {
	                            $(el).daterangepicker({
	                                onClose: function() {
	                                    mygrid[0].triggerToolbar();
	                                }
	                            });
	                        }
	                    }
	                },
	                { name: 'credit_balance', index: 'credit_balance', width: 50 },
	                { name: 'lastInvoiceRecv', index: 'lastInvoiceRecv', width: 50,
	                    searchoptions: {
	                        dataInit: function(el) {
	                            $(el).daterangepicker({
	                                onClose: function() {
	                                    mygrid[0].triggerToolbar();
	                                }
	                            });
	                        }
	                    }
	                },
	                { name: 'debit_balance', index: 'debit_balance', width: 50 },
	                { name: 'period', index: 'period', width: 50 }
	                
                ],
        mtype: "GET",
        rowList: [25, 50, 100],
        pager: jQuery('#s3pager'),
        gridview: true,
        multiboxonly: true,
        viewrecords: true,
        rownumbers: true,
        caption: "",
        <%=Html.RestoreJQState("contract")%>
    })
    .navGrid('#s3pager', { edit: false, add: false, del: false, search: false, refresh: false })
    .navButtonAdd("#s3pager", { caption: "Recargar", title: "Recargar", buttonicon: 'ui-icon-refresh',
        onClickButton: function() {
            mygrid[0].clearToolbar()
        }
    })
   .navButtonAdd('#s3pager', { caption: "Nuevo", title: "Nuevo Contrato", buttonicon: "ui-icon-plusthick",
       onClickButton: function() {
           window.location = virtualdir + '/Contract/New';
       }
   })
    .navButtonAdd('#s3pager', { caption: "Editar", title: "Editar Contrato", buttonicon: "ui-icon-pencil",
        onClickButton: function() {

            var gsr = mygrid.getGridParam('selrow');
            if (!gsr) {
                alert('Seleccione un contrato de la lista');
                return;
            }

            var id = mygrid.getRowData(gsr)['id'];
            window.location = virtualdir + '/Contract/Edit?id=' + id;
        }
    })
    .navButtonAdd('#s3pager', { caption: "Borrar", title: "Borrar Contrato", buttonicon: "ui-icon-minusthick",
        onClickButton: function() {

            var gsr = mygrid.getGridParam('selrow');
            if (!gsr) {
                alert('Seleccione un contrato de la lista');
                return;
            }

            if (!confirm('Esta seguro que desea eliminar este contrato?'))
                return;

            var id = mygrid.getRowData(gsr)['id'];
            window.location = virtualdir + '/Contract/Remove?id=' + id;
        }
    })
    .navButtonAdd('#s3pager', { caption: "Facturar", title: "Facturar", buttonicon: "ui-icon-calculator",
        onClickButton: function() {

            var gsr = mygrid.getGridParam('selrow');
            if (!gsr) {
                alert('Seleccione un contrato de la lista');
                return;
            }

            var id = mygrid.getRowData(gsr)['id'];
            window.location = virtualdir + '/Invoice/New?emireci=0&contract=' + id;
        }
    });

    mygrid.filterToolbar();

    $("select[id^='gs_']")
        .css('padding', '0px')
        .css('background-color', '#ffffff');
        
    <%=Html.RestoreJQStateScript("contract")%>
});
</script>    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
        <h2>Listado de Contratos</h2>
            <% if(ViewData.Keys.Contains("flash") ) { %>
            <br />
            <div class="message <%=ViewData["flash_type"]%>">
                <p><strong><%=ViewData["flash"]%></strong></p>
            </div>
            <% } %>

        
            <div>
                <table id="s3list" class="scroll" cellpadding="0" cellspacing="0"></table> 
                <div id="s3pager" class="scroll" style="text-align:center;"></div> 
                <div id="filter" style="margin-left:30%;display:none">Search Invoices</div>
            </div>
        
    </div>    
</asp:Content>
