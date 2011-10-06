$(document).ready(function() {

    var mygrid = jQuery("#s3list").jqGrid({
    url: virtualdir + '/Log/GridData/',
        datatype: "json",
        width: 1020,
        height: 548,
        colNames: ['Fecha', 'Subproceso', 'Nivel', 'Logger', 'Mensaje'],
        colModel: [
	                { name: 'Date', index: 'Date', width: 50,
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
	                { name: 'Thread', index: 'Thread', width: 10 },
	                { name: 'Level', index: 'Level', width: 10 },
	                { name: 'Logger', index: 'Logger', width: 80 },
	                { name: 'Message', index: 'Message', width: 80 },
                  ],
        rowNum: 25,
        mtype: "POST",
        rowList: [25, 50, 100],
        pager: jQuery('#s3pager'),
        sortname: 'Date',
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
});
