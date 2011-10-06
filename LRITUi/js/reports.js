formatter: function responsefmt(cellvalue, options, rowObject) {
    switch (cellvalue) {
        case 1: return "Coastal";
        case 2: return "Flag";
        case 3: return "Port";
        case 4: return "SAR";
    }

    return "Desconocido";
}

formatter: function refidlink(cellvalue, options, rowObject) {
if (cellvalue == 0)
    return "Por Standing Order";

if (inout == 0)
    inoutinv = 1;
else inoutinv = 0;
return "<a href='" + virtualdir + "/Requests/List/" + inoutinv + "/" + cellvalue + "' title='Ver Request de referencia'>" + cellvalue + "</a>";
}


$(document).ready(function() {

    var mygrid = jQuery("#s3list").jqGrid({
        url: virtualdir + '/Reports/GridData/' + inout,
        datatype: "json",
        width: 1020,
        height: 548,
        colNames: ['TimeStamp', 'Barco', 'Requerido por', 'Provisto por', 'Lat', 'Long', 'Respuesta Tipo', 'Id de Referencia', 'MessageId', 'Nro OMI'],
        colModel: [
	                { name: 'TimeStamp1', index: 'TimeStamp1', width: 50,
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
	                { name: 'ShipName', index: 'ShipName', width: 50 },
	                { name: 'DataUserRequestor', index: 'DataUserRequestor', width: 50, formatter: lritidnamepairs, stype: 'select', editoptions: { value: selectvalues}},
	                { name: 'DataUserProvider', index: 'DataUserProvider', width: 50, formatter: lritidnamepairs, stype: 'select', editoptions: { value: selectvalues} },
	                { name: 'Latitude', index: 'Latitude', width: 50, search: false },
	                { name: 'Longitude', index: 'Longitude', width: 50, search: false },
	                { name: 'ResponseType', index: 'ResponseType', width: 30, formatter: responsefmt, stype: 'select', editoptions: { value: ":TODOS;1:Coastal;2:Flag;3:Port;4:SAR;"} },
	                { name: 'ReferenceId', index: 'ReferenceId', width: 80, formatter: refidlink},
	                { name: 'MessageId', index: 'MessageId', width: 80 },
	                { name: 'IMONum', index: 'IMONum', width: 50 }
                  ],
        rowNum: 25,
        mtype: "POST",
        rowList: [25, 50, 100],
        pager: jQuery('#s3pager'),
        sortname: 'TimeStamp1',
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
        postData: referenceId != null ? { ReferenceId: referenceId} : {}
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
