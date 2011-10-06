<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="JQGridHelper" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Billing - <%=ViewData["title"]%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
    <script type="text/javascript">

        <% int emireci = (int)ViewData["emireci"]; %>
        <%=Html.ComboFilter("lrit_countries")%>
        <% ViewData["invoice_states"] = emireci == 0 ? ViewData["invoice_states_emi"] : ViewData["invoice_states_reci"]; %>
        <%=Html.ComboFilter("invoice_states")%>

        function document_download(cellvalue, options, rowObject) {
            if( cellvalue != null )
               return '<a href="' + virtualdir + '/Invoice/DownloadInvoiceFile?fileid=' + cellvalue + '" title="Descargar documento">Descargar</a>';
            
            return 'N/A';
        }
        
        $(document).ready(function() {
            var mygrid = jQuery("#s3list").jqGrid({
                url: virtualdir + '/Invoice/ListJSON?emireci=<%=emireci%>',
                datatype: "json",
                autowidth: true,
                height: 548,
                colNames: ["id", "País", "Nro. Factura", "Fecha Emisión", "Moneda", "Monto", "Estado", "Documento"],
                colModel: [
	                { name: 'id', index: 'id', width: 50, hidden: true },
	                { name: 'contract_id', index: 'contract_id', width: 50, formatter: lrit_countries, stype: 'select', editoptions: { value: lrit_countries_edit} },
	                { name: 'invoiceNumber', index: 'invoiceNumber', width: 50 },
	                { name: 'isueDate', index: 'isueDate', width: 50,
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
	                { name: 'currency', index: 'currency', width: 50 },
	                { name: 'amount', index: 'amount', width: 50 },
	                { name: 'state', index: 'state', width: 50, formatter: invoice_states, stype: 'select', editoptions: { value: invoice_states_edit }},
	                { name: 'invoiceFile_id', index: 'invoiceFile_id', width: 50, formatter:document_download, hidden: <%=emireci == 1 ? "false" : "true" %> },
                ],
                mtype: "GET",
                rowList: [25, 50, 100],
                pager: jQuery('#s3pager'),
                <%=Html.RestoreJQState("invoice")%>
                gridview: true,
                multiboxonly: true,
                viewrecords: true,
                rownumbers: true,
                caption: "",
                ondblClickRow: function(rowid) {
                    var id = mygrid.getRowData(rowid)['id'];
                    window.location = virtualdir + '/Invoice/Edit?id=' + id;
                }
            })
    .navGrid('#s3pager', { edit: false, add: false, del: false, search: false, refresh: false })
    .navButtonAdd("#s3pager", { caption: "Recargar", title: "Recargar", buttonicon: 'ui-icon-refresh',
        onClickButton: function() {
            mygrid[0].clearToolbar()
        }
    });
    <% if( ((int)ViewData["emireci"]) == 1 ) { %>
   mygrid.navButtonAdd('#s3pager', { caption: "Nueva", title: "Nueva Factura", buttonicon: "ui-icon-plusthick",
       onClickButton: function() {
       window.location = virtualdir + '/Invoice/New?emireci=<%=ViewData["emireci"]%>';
       }
   })
   <%}%>
   
   mygrid.navButtonAdd('#s3pager', { caption: "Editar", title: "Editar Factura", buttonicon: "ui-icon-pencil",
        onClickButton: function() {

            var gsr = mygrid.getGridParam('selrow');
            if (!gsr) {
                alert('Debe seleccionar una factura de la lista');
                return;
            }

            var id = mygrid.getRowData(gsr)['id'];
            window.location = virtualdir + '/Invoice/Edit?id=' + id;
        }
    })
    .navButtonAdd('#s3pager', { caption: "Eliminar", title: "Eliminar", buttonicon: "ui-icon-minusthick",
        onClickButton: function() {

            var gsr = mygrid.getGridParam('selrow');
            if (!gsr) {
                alert('Debe seleccionar una factura de la lista');
                return;
            }

            if (!confirm('Esta seguro que desea eliminar esta factura?'))
                return;

            var id = mygrid.getRowData(gsr)['id'];
            window.location = virtualdir + '/Invoice/Remove?id=' + id;
        }
    })
    .navButtonAdd('#s3pager', { caption: "Mensajes", title: "Ver mensajes", buttonicon: "ui-icon-search",
        onClickButton: function() {

            var gsr = mygrid.getGridParam('selrow');
            if (!gsr) {
                alert('Debe seleccionar una factura de la lista');
                return;
            }

            var id = mygrid.getRowData(gsr)['id'];
            window.location = virtualdir + '/Invoice/ListMessages?invoice=' + id;
        }
    });
    <% if( ((int)ViewData["emireci"]) == 0 ) { %>
    mygrid.navButtonAdd('#s3pager', { caption: "Imprimir", title: "Imprimir factura", buttonicon: "ui-icon-print",
        onClickButton: function() {

            var gsr = mygrid.getGridParam('selrow');
            if (!gsr) {
                alert('Debe seleccionar una factura de la lista');
                return;
            }

            var id = mygrid.getRowData(gsr)['id'];
            window.open(virtualdir + '/Invoice/Print?id=' + id, "_blank");
        }
    });
    <% } %>    
    

        mygrid.filterToolbar();

        $("select[id^='gs_']")
        .css('padding', '0px')
        .css('background-color', '#ffffff');
        
        //Update toolbar search filter values
        <%=Html.RestoreJQStateScript("invoice")%>
    });
    
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
        <h2><%=ViewData["title"]%></h2>
            <% if(ViewData.Keys.Contains("flash") ) { %>
            <br />
            <div class="message <%=ViewData["flash_type"]%>">
                <p><strong><%=ViewData["flash"]%></strong></p>
            </div>
            <% } %>
        
            <div>
                <table id="s3list" class="scroll" cellpadding="0" cellspacing="0"></table> 
                <div id="s3pager" class="scroll" style="text-align:center;"></div> 
                <div id="filter" style="margin-left:30%;display:none"></div>
            </div>
        
    </div>    
</asp:Content>
