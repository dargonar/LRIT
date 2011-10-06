formatter: function changeBool(cellvalue, options, rowObject) 
{
    if (cellvalue == '0')
        return "No";
    else
        return "Si";
}

$(document).ready(function() {

    var mygrid = jQuery("#s3list").jqGrid({
        url: virtualdir + '/DDP/GridData/' + ddpId,
        datatype: "json",
        width: 1020,
        height: 548,
        colNames: ['Gobierno', 'Data Center ID', 'ID LRIT', 'ID de Area', 'Nombre de Area', 'Tipo de Area', 'Standing Order'],
        colModel: [
	                { name: 'Name', index: 'Name', width: 50 },
	                { name: 'DataCenterId', index: 'DataCenterId', width: 40 },
	                { name: 'LRITId', index: 'LRITId', width: 40 },
	                { name: 'PlaceStringId', index: 'PlaceStringId', width: 40 },
	                { name: 'PlaceName', index: 'PlaceName', width: 50 },
	                { name: 'AreaType', index: 'AreaType', width: 50 },
	                { name: 'PlaceId', index: 'PlaceId', width: 50, formatter: changeBool, stype: 'select', editoptions: { value: ":Todo;1:Si;0:No"} },
                  ],
        rowNum: 25,
        mtype: "POST",
        rowList: [25, 50, 100],
        pager: jQuery('#s3pager'),
        sortname: 'Name',
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




