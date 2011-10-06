$(document).ready(function() {


    var mygrid = jQuery("#s3list").jqGrid({
        url: virtualdir + '/Requests/SurpicGridData/' + inout,
        datatype: "json",
        width: 1020,
        height: 548,
        colNames: [ 'TimeStamp', 'Requerido Por', 'Item', 'Tipo Area', 'ID Mensaje', 'Nro de posiciones'],
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
	                { name: 'DataUserRequestor', index: 'DataUserRequestor', width: 50, formatter: lritidnamepairs, stype: 'select', editoptions: { value: selectvalues}},
	                { name: 'Item', index: 'Item', width: 80 , search: false},
	                { name: 'ItemElementName', index: 'ItemElementName', width: 40, search: false },
	                { name: 'MessageId', index: 'MessageId', width: 40 },
	                { name: 'NumberOfPositions', index: 'NumberOfPositions', width: 10 , search: false},
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
