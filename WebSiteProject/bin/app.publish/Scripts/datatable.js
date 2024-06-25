(function ($) {
    var TableCollection = function (elements) {
        this.TableList = [];
        for (var idx = 0; idx < elements.length; idx++) {
            var table = new TableObj($(elements[idx]));
            this.TableList.push(table);
        }
    };

    var TableObj = function (eventtable) {
        var myobj = this;
        this.tableId = eventtable[0].id;
        this.paginationId = 'pagination';
        this.iskeepSelect = false;
        this.tableinfoId = 'tableinfo';
        this.paginationEventClass = 'btn_page';
        this.keepcheckid = [];
        this.keepchecktag = 'chksel';
        this.keepcheckSelectAlltag = 'chk_all';
        this.eventtable = eventtable;
        this.pagesize = parseInt(eventtable.attr('data-page-size'));
        this.maxpagination = eventtable.attr('data-max-pagination');
        this.datafiledid = eventtable.attr('data-filed-id');
        this.pageidx = 1;
        this.firstpagestr = '第一頁';
        this.lastpagestr = '最末頁';
        this.frontpagestr = '上一頁';
        this.nextpagestr = '下一頁';
        this.paginationcallback;
        this.SearchModelBase = {};
        this.totalcnt = 0;
        var getdataFn = this.GetData;
        var tmepSearchModelBase = this.SearchModelBase;
        tmepSearchModelBase.Search = '';
        tmepSearchModelBase.Sort = eventtable.attr('data-sort-name');
        tmepSearchModelBase.Order = 'desc';
        tmepSearchModelBase.Limit = this.pagesize;
        tmepSearchModelBase.NowPage = this.pageidx;
        tmepSearchModelBase.FiledID = '';
        if (this.datafiledid !== undefined) {
            tmepSearchModelBase.FiledID = this.datafiledid;
        }
        $("#" + this.paginationId).delegate(("." + this.paginationEventClass), "click", function () {
            tmepSearchModelBase.NowPage = parseInt($(this).attr('pageindex'));
            if (myobj.iskeepSelect) {
                var sel = $(myobj.eventtable).find('.' + myobj.keepchecktag);
                for (var idx = 0; idx < sel.length; idx++) {
                    var _index = $(sel[idx]).val();
                    if (sel[idx].checked) {
                        if (jQuery.inArray(_index, myobj.keepcheckid) < 0) {
                            myobj.keepcheckid.push(_index);
                        }
                    } else {
                        myobj.keepcheckid = jQuery.grep(myobj.keepcheckid, function (value) {
                            return value != _index;
                        });
                    }
                }
            }
            myobj.GetData(tmepSearchModelBase.NowPage);
            if (myobj.paginationcallback !== undefined) {
                myobj.paginationcallback();
            }
        });
        //change
        $("#" + this.paginationId).delegate(".page_list", "change", function () {
            tmepSearchModelBase.NowPage = $(this).val();
            if (myobj.iskeepSelect) {
                var sel = $(myobj.eventtable).find('.' + myobj.keepchecktag);
                for (var idx = 0; idx < sel.length; idx++) {
                    var _index = $(sel[idx]).val();
                    if (sel[idx].checked) {
                        if (jQuery.inArray(_index, myobj.keepcheckid) < 0) {
                            myobj.keepcheckid.push(_index);
                        }
                    } else {
                        myobj.keepcheckid = jQuery.grep(myobj.keepcheckid, function (value) {
                            return value != _index;
                        });
                    }
                }
            }
            myobj.GetData(tmepSearchModelBase.NowPage);
            if (myobj.paginationcallback !== undefined) {
                myobj.paginationcallback();
            }
        });
    };

    TableObj.prototype.GetData = function (_p, columnfn, rowfn) {
        if (_p === undefined) { _p = 1; }
        var mytable = this.eventtable;
        var myobj = this;
        var searchbase = this.SearchModelBase;
        var ths = mytable.find('thead tr th');
        var tr = mytable.find('thead tr');
        var pageidx = _p;
        var pagesize = searchbase.Limit;
        var maxpagination = myobj.maxpagination;
        var paginationId = myobj.paginationId;
        var tableinfoId = myobj.tableinfoId;
        var iskeepSelect = myobj.iskeepSelect;
        var keepchecktag = myobj.keepchecktag;
        var keepcheckid = myobj.keepcheckid;
        var keepcheckSelectAlltag = this.keepcheckSelectAlltag;
        if (columnfn === undefined) { columnfn = this.settingTrData; }
        if (rowfn === undefined) { rowfn = this.settingTdData; }
        var tableinfoFn = this.settingTableInfo;
        var tablepaginationFn = this.settingPagination;
        if (this.maxpagination === undefined) { maxpagination = 10; }
        searchbase.Offset = ((pageidx - 1) * pagesize);
      
        $.post(mytable.attr('data-url'), searchbase, function (data) {
            var strlist = [];
            if (data != null && data.rows != null) {
                myobj.totalcnt = data.total;
                //myobj=null;
                mytable.find("tbody").empty();
                var ridx = searchbase.Offset;
                for (var i = 0; i < data.rows.length; i++) {
                    ridx += 1;
                    var filedclass = tr[0].getAttribute('filed-class');
                    var filedstyle = tr[0].getAttribute('filed-style');
                    strlist.push(columnfn(ridx, data.rows[i], searchbase.FiledID, filedclass, filedstyle));
                    for (var tidx = 0; tidx < ths.length; tidx++) {
                        var th = $(ths[tidx]);
                        var filedname = th.attr('filed-name');
                        var tableid = th.attr('filed-id');
                        var type = th.attr('filed-type');
                        var fileditemclass = th.attr('filed-item-class') === undefined ? "" : th.attr('filed-item-class');
                        strlist.push(rowfn(data.rows[i], (tidx + 1), ridx, filedname, tableid === undefined ? myobj.datafiledid : tableid, type, fileditemclass,myobj.totalcnt));
                    }
                    strlist.push('</tr>');
                }
                $(strlist.join('')).appendTo(mytable.find("tbody"));
                var pagecnt = Math.ceil(data.total / pagesize);
                var changepagination = Math.ceil(maxpagination / 2) - 1;
                var firstidx = pageidx - changepagination;
                if (pageidx + parseInt(changepagination) >= pagecnt) {
                    firstidx = pagecnt - maxpagination + 1;
                }
                if (firstidx <= 0) { firstidx = 1; }
                var endidx = firstidx + parseInt(maxpagination) - 1;
                if (endidx > pagecnt) { endidx = pagecnt; }
                var beforeidx = pageidx - 1;
                if (beforeidx <= 0) { beforeidx = 1; }
                var nextidx = parseInt(pageidx) + 1;
                if (nextidx > pagecnt) { nextidx = pagecnt; }
                $("#" + paginationId).empty();
                if (pagecnt > 0) {
                    var strpagination = tablepaginationFn(pageidx, firstidx, endidx, beforeidx, nextidx, pagecnt);
                    $(strpagination.join('')).appendTo($("#" + paginationId));
                }
                tableinfoFn(tableinfoId, data.total, (searchbase.Offset + 1), data.rows.length);
                if ($("#" + keepcheckSelectAlltag).length > 0) { $("#" + keepcheckSelectAlltag)[0].checked = false; }
                if (iskeepSelect && keepcheckid.length > 0) {
                    var _sel = mytable.find('.' + keepchecktag);
                    var checkcount = 0;
                    for (var sidx = 0; sidx < _sel.length; sidx++) {
                        var _index = $(_sel[sidx]).val();
                        if (_index !== undefined) {
                            _index = _index.replace("index", "");
                            if (jQuery.inArray(_index, keepcheckid) >= 0) {
                                checkcount += 1;
                                $(_sel[sidx]).attr('checked', 'checked');
                            }
                        }
                    }
                    if (checkcount === _sel.length) {
                        if ($("#" + keepcheckSelectAlltag).length > 0) {
                            $("#" + keepcheckSelectAlltag)[0].checked = true;
                        }
                    }
                }
                myobj = null;
            }
        });
    };

    TableObj.prototype.settingTrData = function (idx, row, dataid, classstr, style) {
        var tr = "<tr id='tr_" + idx + "'";
        if (classstr !== undefined && classstr != '' && classstr != null) {
            tr += " class='" + classstr + "'";
        }
        if (style !== undefined && style != '' && style != null) {
            tr += " style='" + classstr + "'";
        }

        if (dataid !== undefined && dataid != '' && dataid != null) {
            var value = row[dataid];
            if (value != undefined && value != '' && value != null) {
                tr += (" " + dataid + "='" + value + "'");
            }
        }
        tr += ">";
        return tr;
    };

    TableObj.prototype.settingTdData = function (row, columnidx, idx, filedname, tableid, type, fileditemclass) {
        var tr = "<td>";
        var itemclass = '';
        if (fileditemclass !== undefined && fileditemclass != '') {
            itemclass = " class='" + fileditemclass + "'";
        }
        if (tableid !== undefined && tableid != '') {
            var id = row[tableid];

            if (type == 'checkbox') {
                if (id != undefined) {
                    tr += "<input  type='checkbox'  id='" + id + "' index='" + idx + "'" + itemclass + ">";
                } else {
                    tr += "<input  type='checkbox'  index='" + idx + "'" + itemclass + ">";
                }
            } else {
                if (id != undefined) {
                    tr = "<td id='" + id + "'>";
                }
            }
        }
        if (filedname !== undefined && filedname != '') {
            var value = row[filedname];
            if (value !== undefined) {
                if (type === 'checkbox') {
                    if (value === true) {
                        tr += "<input  type='checkbox'  checked='checked'  index='" + idx + "'" + itemclass + ">";
                    } else {
                        tr += "<input  type='checkbox'  index='" + idx + "'" + itemclass + ">";
                    }
                } else {
                    tr += value;
                }
            }
        } else {
            if (type == 'checkbox') {
                tr += "<input  type='checkbox'  index='" + idx + "'" + itemclass + ">";
            }
        }
        tr += "</td>";
        return tr;
    };

    TableObj.prototype.settingTableInfo = function (tableinfoId, totalcnt, offsetcnt, nowpagecnt) {
        $("#" + tableinfoId).text('總共' + totalcnt + '筆資料 顯示' + offsetcnt + "~" + (offsetcnt + nowpagecnt - 1));
    };

    TableObj.prototype.settingPagination = function (pageidx, firstidx, endidx, beforeidx, nextidx, pagecnt) {
        var strpagination = [];
        strpagination.push("<button type='button' class='btn_page' title='" + this.mytable.firstpagestr+"' pageindex='1'><i class='fa fa-angle-double-left'></i></button> ");
        strpagination.push("<button type='button' class='btn_page' title='" + this.mytable.frontpagestr + "' pageindex='" + beforeidx + "'><i class='fa fa-angle-left'></i></button> <select class='form-control page_list'>");
        for (var pidx = 1; pidx <= pagecnt; pidx++) {
            if (pidx == pageidx) {
                strpagination.push(" <option  value='" + pidx + "' selected>" + pidx + "</option>");
            } else {
                strpagination.push(" <option value='" + pidx + "'>" + pidx + "</option>");
            }
        } 
        strpagination.push("</select> <button type='button' class='btn_page' title='" + this.mytable.nextpagestr+"' pageindex='" + nextidx + "'><i class='fa fa-angle-right'></i></button> ");
        strpagination.push("<button type='button' class='btn_page' title='" + this.mytable.lastpagestr+"' pageindex='" + pagecnt + "'><i class='fa fa-angle-double-right'></i></button>");
        return strpagination;
    };
    $.fn.myDataTable = function () {
        var tables = new TableCollection($(this));
        return tables;
    };
}(jQuery));

function TableCheckAllByClass(element, tagclass) {
    if (element.checked === true) {
        $("." + tagclass).attr('checked', 'checked');
    } else {
        $("." + tagclass).removeAttr('checked');
    }
}

function myDataTableTr(row, columnidx, idx, filedname, tableid, type, fileditemclass, totalcnt) {
    if (type === 'delcheckbox') {
        return "<td class='text-center'><label class='mt-checkbox mt-checkbox-single mt-checkbox-outline'>" +
            "<input type='checkbox' class='checkboxes chksel'  value='" + row[tableid] + "'  index='" + idx + "'/><span></span></label></td>";
    } else if (type === 'numbertextcheck') {
        return "<td class='text-center delete_th'><button type='button' class='btn btn-default btn-xs seqindex' value='0' idindex='" + row[tableid] + "'><i class='fa fa-angle-double-up'></i></button> " +
            "<button type='button' class='btn btn-default btn-xs seqindex' value='" + (row[filedname] - 1) + "' idindex='" + row[tableid] + "'><i class='fa fa-angle-up'></i></button> " +
            "<button type='button' class='btn btn-default btn-xs seqindex' value='" + (row[filedname] + 1) + "' idindex='" + row[tableid] + "'><i class='fa fa-angle-down'></i></button> " +
            "<button type='button' class='btn btn-default btn-xs seqindex' value='" + totalcnt + "' idindex='" + row[tableid] + "'><i class='fa fa-angle-double-down'></i></button> "+
            "<input type='hidden' value='" + row[filedname]+"'/><input  type='text'  value='" + row[filedname] +
            "'  class='editinput btn btn-default btn-xs sequence_list " + fileditemclass + "' idindex='" + row[tableid] + "'/>" +
            "<span class='required seqerror' style='color:red;display:none;margin-left:5px;font-size:12px'></span></td > ";
        //<td class="text-center delete_th">
        //    
        //    <button type="button" class="btn btn-default btn-xs"><i class="fa fa-angle-up"></i></button>
        //    <button type="button" class="btn btn-default btn-xs"><i class="fa fa-angle-down"></i></button>
        //    <button type="button" class="btn btn-default btn-xs"><i class="fa fa-angle-double-down"></i></button>
        //    <input type="text" class="btn btn-default btn-xs sequence_list" value="1" />
        //</td>
    } else if (type === "checkbox") {
        if (row[filedname]) {
            return "<td class='text-center'><label class='mt-checkbox mt-checkbox-single mt-checkbox-outline'>" +
                "<input type='checkbox' class='" + fileditemclass+"' checked='checked' value='" + row[tableid] + "' index='" + idx + "'/><span></span></label></td>";
        } else {
            return "<td class='text-center'><label class='mt-checkbox mt-checkbox-single mt-checkbox-outline'>" +
                "<input type='checkbox' class='" + fileditemclass+"' value='" + row[tableid] + "' index='" + idx + "'/><span></span></label></td>";
        }
    } else if (type === 'link') {
        return "<td><a href='#' class='edit' value='" + row[tableid] + "'>" + row[filedname] + "</a></td>";
    } else if (type === 'boolean') {
        return "<td class='text-center'>" + (row[filedname] ===true? "是" : "否") + "</td>";
    } else if (type === 'linkcenter') {
        return "<td class='text-center'><a href='#' class='edit' value='" + row[tableid] + "'>" + row[filedname] + "</a></td>";
    } else if (type === 'text') {
        return "<td class='text-center'><input  type='text'  value='" + row[filedname] + "'  class='editinput form-control input-xsmall " + fileditemclass + "'/>></td>";
    } else if (type === 'button') {
        return "<td class='text-center'><button class='" + fileditemclass + "' id='btn_" + row[tableid] + "' value='" + row[tableid] + "' >" + filedname+"</button></td>";
    } else if (type === 'function') {
        return eval(filedname)(row, columnidx, idx, filedname, tableid, type, fileditemclass);
    } else if (type === 'image') {
        return "<td class='text-center'><a href='" + row[filedname]+"' target='_blank'><img id='btn_" + row[tableid] + "'  class='" + fileditemclass + "' src='" + row[filedname]+"'  value='" + row[tableid] + "'/></a></td>";
    } else {
        return "<td class='" + fileditemclass+"'>" + row[filedname] + "</td>";
    }
}  