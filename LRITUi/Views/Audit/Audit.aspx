<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@Register TagPrefix="ofc" Namespace="OpenFlashChart" Assembly="OpenFlashChart" %>  

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/js/swfobject.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/js/ofc.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/js/json2.js") %>"></script>

    <script type="text/javascript">
        $(function() {
            var period = '<%= ViewData["period"] %>';
            swfobject.embedSWF(virtualdir + "/open-flash-chart.swf", "fragment-1", "500", "350", "9.0.0", "expressInstall.swf", { "data-file": virtualdir + "/Audit/" + period + "/ShipAsp" });
            swfobject.embedSWF(virtualdir + "/open-flash-chart.swf", "fragment-2", "500", "350", "9.0.0", "expressInstall.swf", { "data-file": virtualdir + "/Audit/" + period + "/Asp" });
            swfobject.embedSWF(virtualdir + "/open-flash-chart.swf", "fragment-3", "500", "350", "9.0.0", "expressInstall.swf", { "data-file": virtualdir + "/Audit/" + period + "/AspDc" });
            swfobject.embedSWF(virtualdir + "/open-flash-chart.swf", "fragment-4", "500", "350", "9.0.0", "expressInstall.swf", { "data-file": virtualdir + "/Audit/" + period + "/Dc" });
        });
    </script>
    
    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Auditoria - Periodo <%= ViewData["periodo"] %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
    <h2>Demoras en recorrido de mensaje (<%= ViewData["periodo"] %>)</h2>
    <caption><font color="red">Maximo</font> - <font color="green">Minimo</font> - <font color="blue">Promedio</font></caption>
    <br />
    <div id="container-1">
        <div id="fragment-1">
        </div>

        <div id="fragment-2">
        </div>

        <div id="fragment-3">
        </div>
        <div id="fragment-4">
        </div>
    </div>
    </div>
</asp:Content>
