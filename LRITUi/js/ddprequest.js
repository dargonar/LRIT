$(document).ready(function() {


    var mygrid = jQuery("#s3list").jqGrid({
    url: virtualdir + '/DDPRequest/GridData/',
        datatype: "json",
        width: 1020,
        height: 548,
        //ArchivedDDPTimeStamp, ArchivedDDPTimeStampSpecified, ArchivedDDPVersionNum, DDPVersionNum, 
        //MessageId, MessageType, Originator,  test, TimeStamp,UpdateType, MsgInOutId

        colNames: ['ArchivedDDPTimeStamp', 'ArchivedDDPTimeStampSpecified', 'ArchivedDDPVersionNum', 
        'DDPVersionNum', 'MessageId', 'MessageType', 'Originator', 'test', 'UpdateType','MsgInOut','TimeStamp'],
        colModel: [
	                { name: 'ArchivedDDPTimeStamp', index: 'ArchivedDDPTimeStamp', width: 40 },
	                { name: 'ArchivedDDPTimeStampSpecified', index: 'ArchivedDDPTimeStampSpecified', width: 40 },
	                { name: 'ArchivedDDPVersionNum', index: 'ArchivedDDPVersionNum', width: 40 },
	                { name: 'DDPVersionNum', index: 'DDPVersionNum', width: 40 },
                    { name: 'MessageId', index: 'MessageId', width: 40 },
                    { name: 'MessageType', index: 'MessageType', width: 40 },
                    { name: 'Originator', index: 'Originator', width: 40 },
                    { name: 'test', index: 'test', width: 40 },
	                { name: 'UpdateType', index: 'UpdateType', width: 40 },
	                { name: 'MsgInOut', index: 'MsgInOut', width: 40 },                    
	                { name: 'TimeStamp', index: 'TimeStamp', width: 40 ,
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
});
