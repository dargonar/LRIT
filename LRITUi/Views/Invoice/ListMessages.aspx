<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
<%=ViewData["title"]%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
    <script type="text/javascript">

        $(document).ready(function() {
            var mygrid = jQuery("#s3list").jqGrid({
                url: virtualdir + '/Invoice/ListMessagesJSON?invoice=<%=ViewData["invoice"]%>',
                datatype: "json",
                autowidth: true,
                height: 548,
                colNames: ["MsgType", "MsgId", "RefId", "TimeStamp", "Price"],
                colModel: [
	                { name: 'MsgType', index: 'MsgType', width: 50 },
	                { name: 'MsgId', index: 'MsgId', width: 50 },
	                { name: 'RefId', index: 'RefId', width: 50 },
	                { name: 'TimeStamp', index: 'TimeStamp', width: 50 },
	                { name: 'Price', index: 'Price', width: 50 },
                ],
                mtype: "GET",
                rowList: [25, 50, 100],
                pager: jQuery('#s3pager'),
                page: 1,
                rowNum: 25,
                sortname: 'TimeStamp',
                sortorder: 'desc',
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
    });

    mygrid.filterToolbar();

    $("select[id^='gs_']")
    .css('padding', '0px')
    .css('background-color', '#ffffff');

    });
    
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
        <h2><%=ViewData["title"]%></h2>
            <div>
                <table id="s3list" class="scroll" cellpadding="0" cellspacing="0"></table> 
                <div id="s3pager" class="scroll" style="text-align:center;"></div> 
                <div id="filter" style="margin-left:30%;display:none"></div>
            </div>
    </div>    
</asp:Content>

