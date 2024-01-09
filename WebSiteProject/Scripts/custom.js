function CreatePost(action, object, token) {

    var form = $("#CreatePost");
    var myform = null;
    var hasoldform = false;
    if (form.length === 0) {
        myform = document.createElement("form");
        myform.id = 'CreatePost';
        myform.action = action;
        myform.method = "post";
    } else {
        myform = form[0];
        myform.action = action;
        hasoldform = true;
    }
    var tempopj = {};
    var input = null;

    if (token !== undefined && object["__requestverificationtoken"] === undefined) {
        input = document.createElement("input");
        input.id = "__requestverificationtoken";
        input.value = token;
        input.name = "__requestverificationtoken";
        input.type = "hidden";
        myform.appendChild(input);
    }
    for (var key in object) {
        if (form.length === 0) {
            input = document.createElement("input");
            input.id = key;
            input.value = object[key];
            input.name = key;
            input.type = "hidden";
            myform.appendChild(input);
        } else {
            var idoc = form.find("#" + key);
            if (idoc.length === 0) {
                if (object.hasOwnProperty(key)) {
                    input = document.createElement("input");
                    input.id = key;
                    input.value = object[key];
                    input.name = key;
                    input.type = "hidden";
                    myform.appendChild(input);
                }
            } else {
                $("#" + key).val(object[key]);
            }
        }
    }
    if (hasoldform === false) { document.body.appendChild(myform); }
    myform.submit();
}
function CheckInput() {
    var inputval = $("#form-body :input").filter(function () { return $(this).val() === "" && $(this).attr('name') !== undefined; });
    for (var idx = 0; idx < inputval.length; idx++) {
        $("#" + inputval[idx].name + "-error").show();
    }
    if (inputval.length > 0) { return false; } else {
        return true;
    }
}

function GetDatetimeFormat(d) {
    return ([d.getFullYear(),
    (d.getMonth() + 1 < 10) ? ("0" + (d.getMonth() + 1)) : (d.getMonth() + 1),
    (d.getDate() < 10) ? ("0" + d.getDate()) : d.getDate()].join('/') + ' ' +
        [(d.getHours() < 10) ? ("0" + d.getHours()) : d.getHours(),
        (d.getMinutes() < 10) ? ("0" + d.getMinutes()) : d.getMinutes(),
        (d.getSeconds() < 10) ? ("0" + d.getSeconds()) : d.getSeconds()].join(':'));
}
function RegisterClicklink(eventclick, eventitem, linkurl, obj, type) {
    $(eventclick).delegate((eventitem), "click", function () {
        if (obj == undefined) { obj = {}; }
        if (obj.idkey !== undefined) {
            obj[obj.idkey] = $(this).attr('value');
        } else { obj.id = $(this).attr('value'); }
        if (type == "UpdateItem") {
            var text = $(this).text();
            var dialog = bootbox.confirm({
                title: '修改' + obj.item,
                message: "<p class='btn_03' style='margin-left:5px'>" + (obj.itemname === undefined ? obj.item : obj.itemname) + "</p><input id='txt_name' class='form-control' type='text' style='width:400px;margin-left:5px;display:inline' value='" + text + "'/>" +
                    "<span class='required' id='nameerror' style='color:red;display:none;margin-left:5px'>必須輸入！</span>",
                buttons: {
                    cancel: {
                        label: '<i class="fa fa-times"></i>取消'
                    },
                    confirm: {
                        label: '<i class="fa fa-check"></i> 確認'
                    }
                }, callback: function (result) {
                    if (result) {
                        if ($("#txt_name").val() == "") {
                            $("#nameerror").show();
                            return false;
                        } else {
                            $("#nameerror").hide();
                        };
                        obj.name = $("#txt_name").val();
                        $.post(linkurl, obj, function (data) {
                            alert(data);
                            mytable.GetData(1);
                        }, "json");
                    }
                }
            });
        } else {
            CreatePost(linkurl, obj, obj.token);
        }
    });
}

function RegisterClickAll(eventclick, eventitem, clickinfo) { $(eventclick).click(function () { var thischk = this.checked; $(eventitem).prop('checked', thischk); var totalclick = $(".chksel:checked").length; $(clickinfo).text(totalclick); }); }
function RegisterOrder(eventid, targetid, posturl, postobj) {
    if (postobj === undefined) {
        postobj = {};
    }
    $(eventid).delegate(targetid, "change", function () {
        $(".sortedit").attr('disabled', 'disabled');
        $(".seqerror").text('').hide();
        var oldvalue = $(this).prev();
        var tvalue = $(this).val().match(/^[0-9]+$/g);
        if (tvalue === null) {
            var error = $(this).next(); if (error.length > 0) { error.text('必須為整數！'); error.show(); } $(".sortedit").removeAttr('disabled'); return false;
        } var idx = $(this).attr('idindex'); postobj.seq = $(this).val(); postobj.id = $(this).attr('idindex'); $.post(posturl, postobj, function (data) { $(".sortedit").removeAttr('disabled'); alert(data); mytable.GetData(1); }, "json");
    });
    $(eventid).delegate(".seqindex", "click", function () {
        postobj.seq = $(this).val();
        postobj.id = $(this).attr('idindex');
        $(this).attr('idindex'); $.post(posturl, postobj, function (data) { alert(data); mytable.GetData(1); }, "json");
    });
}
function RegisterDelete(eventid, targetitem, posturl, postobj) {
    $(eventid).click(function () {
        if (postobj === undefined) {
            postobj = {};
        }
        var check = $(targetitem);
        var str = [];
        var idlist = [];
        var keyidx = 2;
        if (postobj.keyindex != undefined) { keyidx = postobj.keyindex; }
        for (var idx = 0; idx < check.length; idx++) {
            var index = check[idx].getAttribute('index');
            str.push($("#tr_" + index).find('td').eq(keyidx).text());
            idlist.push(check[idx].getAttribute('value'));
        }
        if (idlist.length === 0) { return false; }
        var strname = str.join('，');
        postobj.idlist = idlist;
        postobj.delaccount = strname;
        bootbox.confirm({
            title: "確定刪除?",
            message: "是否確定刪除以下項目:" + strname + "?",
            buttons: { cancel: { label: '<i class="fa fa-times"></i>取消' }, confirm: { label: '<i class="fa fa-check"></i> 確認' } },
            callback: function (result) {
                if (result) { $.post(posturl, postobj, function (data) { alert(data); mytable.GetData(); }); }
            }
        });
    });
}
function CallAddDialog(posturl, postobj) {
    if (postobj === undefined) {
        postobj = {};
        postobj.id = -1;
    }
    var dialog = bootbox.confirm({
        title: (postobj.title === undefined ? '新增' + postobj.item : postobj.title),
        message: "<p class='btn_03' style='margin-left:5px'>" + (postobj.itemname === undefined ? postobj.item : postobj.itemname) + "</p><input id='txt_name' class='form-control' type='text' style='width:400px;margin-left:5px;display:inline'/>" +
            "<span class='required' id='nameerror' style='color:red;display:none;margin-left:5px'>必須輸入！</span>",
        buttons: {
            cancel: {
                label: '<i class="fa fa-times"></i>取消'
            },
            confirm: {
                label: '<i class="fa fa-check"></i> 確認'
            }
        }, callback: function (result) {
            if (result) {
                if ($("#txt_name").val() == "") {
                    $("#nameerror").show();
                    return false;
                } else {
                    $("#nameerror").hide();
                };
                postobj.id = -1;
                postobj.name = $("#txt_name").val();
                $.post(posturl, postobj, function (data) {
                    alert(data);
                    mytable.GetData(1);
                }, "json");
            }
        }
    });
}
function RegisterClick(eventid, targetid, posturl, postobj) {
    $(eventid).delegate((targetid), "click", function () {
        postobj.id = $(this).attr('value');
        postobj.status = this.checked;
        $(targetid).attr('disabled', 'disabled');
        $.post(posturl, postobj, function (data) {
            $(targetid).removeAttr('disabled');
            alert(data);
        });
    });
}
function RegisterSearchItem() {
    $(".search_icon").click(function () { var collapsed = $(this).find('i').hasClass('fa-search-minus'); $(".search_menu").slideToggle(); $('.search_icon').find('i').removeClass('fa-search'); $('.search_icon').find('i').addClass('fa-search-minus'); if (collapsed) $(this).find('i').toggleClass('fa-search-minus fa-2x fa-search fa-2x') });
    $("#btn_searchrefresh").click(function () { $("#searchForm :input").val(''); mytable.GetData(); });
}
function RegisterClickAndData(eventid, targetid, posturl, postobj, tableobj) {
    $(eventid).delegate((targetid), "click", function () {
        postobj.id = $(this).attr('value');
        $.post(posturl, postobj, function (data) {
            alert(data);
            tableobj.GetData(1);
        });
    });
}
