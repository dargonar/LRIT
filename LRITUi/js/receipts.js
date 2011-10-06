formatter: function errorCode(cellvalue, options, rowObject) 
{
    switch(cellvalue)
    {
        case 0 : return "No titulado para recibir información";
        case 1 : return "No hay barcos en en área SARSURPIC";
        case 2 : return "IDE no disponible";
        case 3 : return "DC no disponible";
        case 4 : return "PSC no disponible";
        case 5 : return "El barco no responde";
        case 6 : return "El barco no está disponible";
        case 7 : return "Falla en el sistema";
        case 8 : return "No se pudo cargar DDP";
        case 9 : return "Version de DDP incorrecta, mensaje descartado";
    }

    return "Desconocido";
}

$(document).ready(function() {
    
    var mygrid = jQuery("#s3list").jqGrid({
    url: virtualdir + '/Receipts/GridData/' + inout,
        datatype: "json",
        width: 1020,
        height: 548,
        colNames: ['TimeStamp', 'Reference Id', 'Code', 'Destination', 'Originator', 'Message', 'Message Id'],
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
	                
	                { name: 'ReferenceId', index: 'ReferenceId', width: 50 },
	                { name: 'ReceiptCode', index: 'ReceiptCode', width: 50, formatter: errorCode, stype: 'select', editoptions: { value: ":Todo;0:No titulado para recibir información;1:No hay barcos en en área SARSURPIC;2:IDE no disponible;3:DC no disponible;4:PSC no disponible;5:El barco no responde;6:El barco no está disponible;7:Falla en el sistema;8:No se pudo cargar DDP;9:Version de DDP incorrecta, mensaje descartado"} },
	                { name: 'Destination', index: 'Destination', width: 40, formatter: lritidnamepairs },
	                { name: 'Originator', index: 'Originator', width: 40, formatter: lritidnamepairs },
	                { name: 'Message', index: 'Message', width: 80 },
	                { name: 'MessageId', index: 'MessageId', width: 50 }
                ],
        rowNum: 25,
        mtype: "GET",
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
        caption: ""
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
