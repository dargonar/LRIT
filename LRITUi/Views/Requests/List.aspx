<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>


<asp:Content ID="Content4" ContentPlaceHolderID="HeadInfo" runat="server">
    <script language="javascript" type="text/javascript">
        var inout = <%= ViewData["msgInOut"] %>;
        
        var referenceId = <%= ViewData["referenceId"] == null ? "null" : "'" + ViewData["referenceId"] + "'"%>;
        
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
        
        formatter: function accessfmt(cellvalue, options, rowObject) {
            switch (cellvalue) {
                case 0: return "Restart-Reset";
                case 1: return "Coastal";
                case 2: return "Flag";
                case 3: return "Port Distance Trigger";
                case 4: return "Not defined";
                case 5: return "Port Time Trigger";
                case 6: return "Sar";
            }

            return "Desconocido";
        }

        formatter: function requestfmt(cellvalue, options, rowObject) {
            switch (cellvalue) {
                case 0: return "Restart-Reset";
                case 1: return "One time poll";
                case 2: return "15 minute periodic";
                case 3: return "30 minute periodic";
                case 4: return "1 hour periodic";
                case 5: return "3 hour periodic";
                case 6: return "6 hour periodic";
                case 7: return "Archived Info Request";
                case 8: return "Stop-Dont Start";
                case 9: return "Most recent position";
                case 10: return "12 hour periodic";
                case 11: return "24 hour periodic";
            }

            return "Desconocido";
        }

        formatter: function refidlink(cellvalue, options, rowObject) {
            if (inout == 0)
                inoutinv = 'Sent';
            else inoutinv = 'Received';
            return "<a href='" + virtualdir + "/Reports/" + inoutinv + "/" + cellvalue + "' title='Ver Reports referenciados'>" + cellvalue + "</a>";
        }



        $(document).ready(function() {


            var mygrid = jQuery("#s3list").jqGrid({
                url: virtualdir + '/Requests/GridData/' + inout,
                datatype: "json",
                width: 1020,
                height: 548,
                colNames: ['TimeStamp', 'Nro OMI', 'Requerido por', 'Provisto por', 'Acceso', 'Requerimiento', 'Tiempo de Inicio', 'Tiempo de fin', 'Message Id'],
                colModel: [
	                { name: 'TimeStamp', index: 'TimeStamp', width: 50,
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
	                { name: 'IMONum', index: 'IMONum', width: 50 },
	                { name: 'DataUserRequestor', index: 'DataUserRequestor', width: 40, formatter: lritidnamepairs, stype: 'select', editoptions: { value: selectvalues} },
	                { name: 'DataUserProvider', index: 'DataUserProvider', width: 40, formatter: lritidnamepairs, stype: 'select', editoptions: { value: selectvalues} },
	                { name: 'AccessType', index: 'AccessType', width: 40, formatter: accessfmt },
	                { name: 'RequestType', index: 'RequestType', width: 40, formatter: requestfmt <% if( ViewData["is_sar"] == "false" ) { %>, search:false <%}%> },
	                { name: 'StartTime', index: 'StartTime', width: 50, search: false },
	                { name: 'StopTime', index: 'StopTime', width: 50, search: false },
	                { name: 'MessageId', index: 'MessageId', width: 80, formatter: refidlink }
                  ],
                rowNum: 25,
                mtype: "POST",
                rowList: [25, 50, 100],
                pager: jQuery('#s3pager'),
                sortname: 'TimeStamp',
                gridview: true,
                multiboxonly: true,
                jsonReader: {
                    root: "Rows",
                    page: "Page",
                    total: "Total",
                    records: "Records",
                    repeatitems: false,
                    userdata: "UserData",
                    id: "Id"
                },
                viewrecords: true,
                rownumbers: true,
                sortorder: "desc",
                caption: "",
                search: referenceId != null ? true : false,
                postData: referenceId != null ? { MessageId: referenceId} : {}
            })
            .navGrid('#s3pager', { edit: false, add: false, del: false, search: false, refresh: false })
            .navButtonAdd("#s3pager", { caption: "Toggle", title: "Toggle Search Toolbar", buttonicon: 'ui-icon-pin-s',
                onClickButton: function() {
                    mygrid[0].toggleToolbar()
                }
            })
            .navButtonAdd("#s3pager", { caption: "Clear", title: "Clear Search", buttonicon: 'ui-icon-refresh',
                onClickButton: function() {
                    mygrid[0].clearToolbar()
                }
            });
            mygrid.filterToolbar();

            $("select[id^='gs_']").css('padding', '0px')
               .css('background-color', '#ffffff');

        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
        <h2>Listado de Requerimientos <%= (int)ViewData["msgInOut"]!=0 ? "Enviados" : "Recibidos" %></h2>
        <p>
            <div>
                <table id="s3list" class="scroll" cellpadding="0" cellspacing="0"></table> 
                <div id="s3pager" class="scroll" style="text-align:center;"></div> 
                <div id="filter" style="margin-left:30%;display:none">Search Invoices</div>
            </div>
        </p>   
    </div>
</asp:Content>


