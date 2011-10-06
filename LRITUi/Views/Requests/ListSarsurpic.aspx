<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>


<asp:Content ID="Content4" ContentPlaceHolderID="HeadInfo" runat="server">
    <script language="javascript" type="text/javascript">
        var inout = <%= ViewData["msgInOut"] %>;
        
        formatter: function lritidnamepairs(cellvalue, options, rowObject) 
        {
            switch(cellvalue)
                {
                
                <%  var dplist = ":Todos;";
                    var dic = new Dictionary<int, string>();
                    dic = (Dictionary<int, string>)ViewData["LritIDNamePairs"];
                    foreach (var cg in dic)
                    { 
                        Response.Write(string.Format("case \"{0}\" : return \"{1}\";\n", cg.Key, cg.Value));
                        dplist += cg.Key + ":" + cg.Value + ";";
                    }
                %>
                }
            return "Desconocido";
        }
        
        <% Response.Write("selectvalues = \"" + dplist + "\";"); %>

        
        
    </script>
    <script src="<%= Url.Content("~/js/sarsurpicrequests.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
        <h2>Listado de Requerimientos Sarsurpic <%= (int)ViewData["msgInOut"]!=0 ? "Enviados" : "Recibidos" %></h2>
        <p>
            <div>
                <table id="s3list" class="scroll" cellpadding="0" cellspacing="0"></table> 
                <div id="s3pager" class="scroll" style="text-align:center;"></div> 
                <div id="filter" style="margin-left:30%;display:none">Search Invoices</div>
            </div>
        </p>   
    </div>
</asp:Content>


