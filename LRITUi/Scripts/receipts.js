$(document).ready(function() {

    
    var mygrid = jQuery("#s3list").jqGrid({
        url: '/Receipts/GridData',
        datatype: "json",
        width: 600,
        height: 600,
        colNames: ['Message Id', 'Reference Id', 'Code', 'Destination', 'Originator', 'Message', 'TimeStamp'],
        colModel: [
	                { name: 'MessageId', index: 'MessageId', width: 50 },
	                { name: 'ReferenceId', index: 'ReferenceId', width: 50 },
	                { name: 'ReceiptCode', index: 'ReceiptCode', width: 10 },
	                { name: 'Destination', index: 'Destination', width: 40 },
	                { name: 'Originator', index: 'Originator', width: 40 },
	                { name: 'Message', index: 'Message', width: 80 },
	                { name: 'TimeStamp', index: 'TimeStamp', width: 40, searchoptions: { dataInit: function(el) { $(el).datepicker({ dateFormat: 'yy-mm-dd' }); } } }
                ],
        rowNum: 30,
        mtype: "GET",
        rowList: [10, 20, 30],
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
        caption: "Toolbar Search Example"
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
