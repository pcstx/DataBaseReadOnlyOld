 var tab = null;
var accordion = null; 
var menu; var actionNode;
var dbUrl = location.protocol + "//" + location.host + '/Home/DataBaseJson';
var tableUrl = location.protocol + "//" + location.host + '/Home/TablesJson/';
var rowUrl = location.protocol + "//" + location.host + '/Home/RowsJson/';
var gridUrl = location.protocol + "//" + location.host + '/Home/RowsGrid/';
var searchUrl=location.protocol + "//" + location.host + '/Home/Select/';

var viewsUrl = location.protocol + "//" + location.host + '/Sider/ViewsJson/';
var procedureUrl = location.protocol + "//" + location.host + '/Sider/ProcedureJson/';
var treeViewObj, treeObj, treeProcedureObj;

$(function () {
    treeObj = $("ul[attr-name='tree1']"); //表
    treeViewObj = $("ul[data-name='treeView']"); //视图
    treeProcedureObj = $("ul[data-name='treeProcedure']"); //存储过程

    //布局
    $("#layout1").ligerLayout({ leftWidth: 260, height: '100%', heightDiff: -34, space: 4, onHeightChanged: f_heightChanged });

    var height = $(".l-layout-center").height(); 
    //Tab
    $("#framecenter").ligerTab({ height: height }); 
    //面板
    $("#accordion1").ligerAccordion({ height: height - 24, speed: null });

    $(".l-link").hover(function () {
        $(this).addClass("l-link-over");
    }, function () {
        $(this).removeClass("l-link-over");
    });

    $(".dbname").bind("change", function () { 
        var index = $(this).parent().parent().attr("data-index");
        var databasename = $(this).children("option:selected").val();
        var tree = $(treeObj[index]);
        var connectionName = tree.attr("data-connName");

        tree.ligerTree({
            checkbox: false,
            slide: true,
            nodeWidth: 250,
            btnClickToToggleOnly: false,
            // treeLine: false,
            idFieldName: 'id',
            textFieldName: 'name',
            url: tableUrl + "?dbName=" + databasename + "&connectionStringName=" + connectionName,
            isLeaf: function (data) {
                if (!data) return false;
                return data.type == "table";
            },
            delay: function (e) {
                var data = e.data;
                if (data.type == "database") {
                    return { url: tableUrl + "?dbName=" + data.name + "&connectionStringName=" + e.data.connName }
                } else if (data.type == "table") {
                    return { url: rowUrl + data.databaseName + '/' + data.name + "?connectionStringName=" + e.data.connName }
                }
                return true;
            },
            onSelect: SelectNode,
            onContextmenu: function (node, e) {
                if (node.data && node.data.type == "database") return;
                actionNode = node;
                menu.show({ top: e.pageY, left: e.pageX });
                return false;
            }
        });
    })

    $("#toptoolbar").ligerToolBar({
        items: [
                    {
                        text: '增加', click: function (item) {
                            alert(item.text);
                        }, icon: 'add'
                    },
                    { line: true },
                    { text: '删除', click: itemclick }
        ]
    });

    changeHeight();
    $(window).resize(changeHeight);

   
    /*
    (function buildTree() {
        for (var i = 0; i < treeObj.length; i++) {
            var tree = $(treeObj[i]);
            var attrData = tree.attr("attr-data");
            tree.ligerTree({
                checkbox: false,
                slide: true,
                nodeWidth: 250,
                btnClickToToggleOnly: false,
                // treeLine: false,
                idFieldName: 'id',
                textFieldName: 'name',
                url: dbUrl + "?connectionStringName=" + attrData,
                isLeaf: function (data) {
                    if (!data) return false;
                    return data.type == "table";
                },
                delay: function (e) {
                    var data = e.data;
                    if (data.type == "database") {
                        return { url: tableUrl + "?dbName=" + data.name + "&connectionStringName=" + e.data.connName }
                    } else if (data.type == "table") {
                        return { url: rowUrl + data.databaseName + '/' + data.name + "?connectionStringName=" + e.data.connName }
                    }
                    return true;
                },
                onSelect: SelectNode,
                onContextmenu: function (node, e) {
                    if (node.data && node.data.type == "database") return ;
                    actionNode = node;
                    menu.show({ top: e.pageY, left: e.pageX });
                    return false;
                }
            });
            
        }
    })();
     */

    function changeHeight() {
        var leftsideHeight = $(".l-scroll").height();

        var siderTopHeight = $(".siderTop").height();
        var siderViewHeight = $(".siderView").height();
         
        $(".treeView").height(leftsideHeight - siderTopHeight - siderViewHeight-30);
    }

    function Search() {
        var tabid = $(actionNode.target).attr("tabid_search");
        if (!tabid) {
            tabid = new Date().getTime();
            $(actionNode.target).attr("tabid_search", tabid)
        }
        f_addTab(tabid, actionNode.data.name + "_查询", searchUrl + "?dbName=" + actionNode.data.databaseName + "&tableName=" + actionNode.data.name + "&connectionStringName=" + actionNode.data.connName);
    }

    function Design() {
        if (actionNode.data && actionNode.data.type == "database") return;
        var tabid = $(actionNode.target).attr("tabid");
        if (!tabid) {
            tabid = new Date().getTime();
            $(actionNode.target).attr("tabid", tabid)
        }
        f_addTab(tabid, actionNode.data.name, gridUrl + actionNode.data.databaseName + "/" + actionNode.data.name + "?connectionStringName=" + actionNode.data.connName);
    }

    menu = $.ligerMenu({
        top: 100, left: 100, width: 120, items:
                    [
                    { text: '设计', click: Design },
                    { text: '查看', click: Search }
                    ]
    });
     
    tab = $("#framecenter").ligerGetTabManager();
    accordion = $("#accordion1").ligerGetAccordionManager();
    
    $("#pageloading").hide(); 
});

function SelectNode(node) {
    if (node.data && node.data.type == "database") return;
    var tabid = $(node.target).attr("tabid");
    if (!tabid) {
        tabid = new Date().getTime();
        $(node.target).attr("tabid", tabid)
    }
    f_addTab(tabid, node.data.name, gridUrl + node.data.databaseName + "/" + node.data.name + "?connectionStringName=" + node.data.connName);
}

function f_heightChanged(options) {
    if (tab)
        tab.addHeight(options.diff);
    if (accordion && options.middleHeight - 24 > 0)
        accordion.setHeight(options.middleHeight - 24);
}
function f_addTab(tabid, text, url) {
    tab.addTabItem({ tabid: tabid, text: text, url: url });
}
 
function itemclick(item) {
    alert(item.text);
}

function f_open() {
    $.ligerDialog.open({
        url: '../../welcome.htm', height: 300, width: null, buttons: [
            { text: '确定', onclick: function (item, dialog) { alert(item.text); } },
            { text: '取消', onclick: function (item, dialog) { dialog.close(); } }
        ], isResize: true
    });
}

function buildTree(index,databaseName) {
    var tree = $(treeViewObj[index]);
    var connName = tree.attr("data-connName"); 
    tree.ligerTree({
        checkbox: false,
        slide: true,
        nodeWidth: 250,
        btnClickToToggleOnly: false,
        // treeLine: false,
        idFieldName: 'Id',
        textFieldName: 'Name',
        url: viewsUrl + "?dbName=" + databaseName + "&connectionStringName=" + connName,
        isLeaf: function (data) {
            if (!data) return false;
            return data.type == "table";
        },
        delay: function (e) {
            var data = e.data;
            if (data.type == "database") {
                return { url: tableUrl + "?dbName=" + data.name + "&connectionStringName=" + e.data.connName }
            } else if (data.type == "table") {
                return { url: rowUrl + data.databaseName + '/' + data.name + "?connectionStringName=" + e.data.connName }
            }
            return true;
        },
        onSelect: SelectNode,
        onContextmenu: function (node, e) {
            if (node.data && node.data.type == "database") return;
            actionNode = node;
            menu.show({ top: e.pageY, left: e.pageX });
            return false;
        }
    });
}

function buildTableTree(index, databasename) {
    var tree = $(treeObj[index]);
    var connectionName = tree.attr("data-connName");

    tree.ligerTree({
        checkbox: false,
        slide: true,
        nodeWidth: 250,
        btnClickToToggleOnly: false,
        // treeLine: false,
        idFieldName: 'id',
        textFieldName: 'name',
        url: tableUrl + "?dbName=" + databasename + "&connectionStringName=" + connectionName,
        isLeaf: function (data) {
            if (!data) return false;
            return data.type == "table";
        },
        delay: function (e) {
            var data = e.data;
            if (data.type == "database") {
                return { url: tableUrl + "?dbName=" + data.name + "&connectionStringName=" + e.data.connName }
            } else if (data.type == "table") {
                return { url: rowUrl + data.databaseName + '/' + data.name + "?connectionStringName=" + e.data.connName }
            }
            return true;
        },
        onSelect: SelectNode,
        onContextmenu: function (node, e) {
            if (node.data && node.data.type == "database") return;
            actionNode = node;
            menu.show({ top: e.pageY, left: e.pageX });
            return false;
        }
    });
}

function buildProcedureTree(index, databasename) {
    var tree = $(treeProcedureObj[index]);
    var connectionName = tree.attr("data-connName");

    tree.ligerTree({
        checkbox: false,
        slide: true,
        nodeWidth: 250,
        btnClickToToggleOnly: false,
        // treeLine: false,
        idFieldName: 'Id',
        textFieldName: 'Name',
        url: procedureUrl + "?dbName=" + databasename + "&connectionStringName=" + connectionName,
        isLeaf: function (data) {
            if (!data) return false;
            return data.type == "table";
        },
        delay: function (e) {
            var data = e.data;
            if (data.type == "database") {
                return { url: tableUrl + "?dbName=" + data.name + "&connectionStringName=" + e.data.connName }
            } else if (data.type == "table") {
                return { url: rowUrl + data.databaseName + '/' + data.name + "?connectionStringName=" + e.data.connName }
            }
            return true;
        },
        onSelect: SelectNode,
        onContextmenu: function (node, e) {
            if (node.data && node.data.type == "database") return;
            actionNode = node;
            menu.show({ top: e.pageY, left: e.pageX });
            return false;
        }
    });
}