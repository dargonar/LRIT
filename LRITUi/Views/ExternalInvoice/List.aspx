<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="JQGridHelper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ViewData["title"]%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
    <script type="text/javascript">

        <%=Html.ComboFilterEN("invoice_states_emien")%>

        $(document).ready(function() {
            var mygrid = jQuery("#s3list").jqGrid({
                url: virtualdir + '/ExternalInvoice/ListJSON',
                datatype: "json",
                autowidth: true,
                height: 548,
                colNames: ["id", "invoiceNumber", "isueDate", "currency", "amount", "state"],
                colModel: [
	                { name: 'id', index: 'id', width: 50 },
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
	                { name: 'state', index: 'state', width: 50, formatter: invoice_states_emien, stype: 'select', editoptions: { value: invoice_states_emien_edit } },
                ],
                mtype: "GET",
                rowList: [25, 50, 100],
                pager: jQuery('#s3pager'),
                page: 1 ,
                rowNum: 25 ,
                sortname: 'isueDate' ,
                sortorder: 'desc' ,
                gridview: true,
                multiboxonly: true,
                viewrecords: true,
                rownumbers: true,
                
                caption: "",
            })
    .navGrid('#s3pager', { edit: false, add: false, del: false, search: false, refresh: false })
    .navButtonAdd("#s3pager", { caption: "Recargar", title: "Recargar", buttonicon: 'ui-icon-refresh',
        onClickButton: function() {
            mygrid[0].clearToolbar()
        }
    })
    .navButtonAdd('#s3pager', { caption: "Messages", title: "View Invoiced Messages", buttonicon: "ui-icon-search",
        onClickButton: function() {

            var gsr = mygrid.getGridParam('selrow');
            if (!gsr) {
                alert('You must select an invoice from the list');
                return;
            }

            var id = mygrid.getRowData(gsr)['id'];
            window.location = virtualdir + '/Invoice/ListMessages?invoice=' + id;
        }
    });    

        mygrid.filterToolbar();

        $("select[id^='gs_']")
        .css('padding', '0px')
        .css('background-color', '#ffffff');
    });
    
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <h2><%=ViewData["title"]%></h2>
    <div>
        <table id="s3list" class="scroll" cellpadding="0" cellspacing="0"></table> 
        <div id="s3pager" class="scroll" style="text-align:center;"></div> 
        <div id="filter" style="margin-left:30%;display:none"></div>
    </div>
    
</asp:Content>

