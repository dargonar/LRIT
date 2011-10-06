<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>





<asp:Content ID="Content4" ContentPlaceHolderID="HeadInfo" runat="server">
    <script language="javascript">
        var ddpId = <%= ViewData["ddpId"] %>;
    </script>
    <script src="<%= Url.Content("~/js/ddp.js") %>" type="text/javascript"></script>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="top-bar">
        <h2>Información del DDP (version <%= ViewData["version"] %>)</h2>
    </div>

    <br/>

    <div class="table listing">
        <table id="s3list" class="scroll" cellpadding="0" cellspacing="0"></table> 
        <div id="s3pager" class="scroll" style="text-align:center;"></div> 
        <div id="filter" style="margin-left:30%;display:none">Search Invoices</div>
    </div>
    
</asp:Content>


