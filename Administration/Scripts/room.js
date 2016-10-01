// ****************************************************
// 貸し会議室管理用
// ****************************************************

// ****************************************************
// Site.Master の中の　ContentPlaceHolderのIDが
// htmlコントロールのIDにプレフィックスとし
// て付加される場合がある。本来は付加されないはずだが、
// runat="server"が付いていて、ClientIDMode="Static"が
// 付いているものが、プレフィックスが付くようだ。
// aspコントロールは、『ClientIDMode="Static"』を
// 設定しているため、そのままのID。
// ****************************************************
var idPrefix = "MainContent_";

$(function () {
    pageLoaded();
});

function pageLoad() {
    Sys.WebForms.PageRequestManager.getInstance().remove_pageLoaded(pageLoaded);
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(pageLoaded);
}

function onclick_chkManualMode() {
    if (document.getElementById("chkManualMode").checked) {
        document.getElementById("reserve_bg2").style.display = "block";
    } else {
        document.getElementById("reserve_bg2").style.display = "none";
    }
}

// bootstrap-select
$(window).on('load', function () {
    $('.selectpicker').selectpicker({
        'selectedText': 'cat'
    });
});

function pageLoaded(sender, args) {

    // スクロールバーを設定する
    //    setScrollBar();
    table_scroll_List();                // 一覧タブ
    table_scroll_Shisetsu();            // 施設タブ

    // 入力エラーでない場合のみ、初期化する
    if (document.getElementById("hid_addMode1").value == "") {
        // ボタンを初期表示にする
        initButton_List();
    } else {
        // 入力エラーの場合は、ダイアログを再表示する
        var ft = document.getElementById("hid_time_from1").value;
        var fm = document.getElementById("hid_minutes_from1").value;
        var tt = document.getElementById("hid_time_to1").value;
        var tm = document.getElementById("hid_minutes_to1").value;
        var yoto = document.getElementById("hid_yoto1").value;
        var msg = "設定した時間範囲内に既に予約が登録されています";
        showEditDialogWithError(ft, fm, tt, tm, yoto, msg);
    }

    if (document.getElementById("hid_addMode2").value == "") {
        // ボタンを初期表示にする
        initButton_Shisetsu();
    } else {
        // 入力エラーの場合は、ダイアログを再表示する
        var ft = document.getElementById("hid_time_from2").value;
        var fm = document.getElementById("hid_minutes_from2").value;
        var tt = document.getElementById("hid_time_to2").value;
        var tm = document.getElementById("hid_minutes_to2").value;
        var yoto = document.getElementById("hid_yoto2").value;
        var msg = "設定した時間範囲内に既に予約が登録されています";
        showEditDialogWithError(ft, fm, tt, tm, yoto, msg);
    }

    // テーブルのセルをクリックしたときの処理（一覧）
    $("#tblBooking1 td").click(function (event) {
        // currentTarget のindex()で列番号を取得
        var col = $(event.currentTarget).index();
        var row = $(event.currentTarget.parentNode).index();
        var mode = document.getElementById("hid_addMode1").value;
        var oldRow = document.getElementById("hid_clickRow1").value;
        var oldColStart = document.getElementById("hid_clickColStart1").value;
        var oldColEnd = document.getElementById("hid_clickColEnd1").value;
        var tbl = document.getElementById("tblBooking1");

        // 閲覧モードの場合は、処理を抜ける
        var editMode = document.getElementById("hid_editMode1").value;
        if (editMode.toLowerCase() == "false") return false;

        // ヘッダ行が2行あるので、3行目（row=2）からがデータ行となる
        if ((row > 1)) {
            switch (mode) {
                case "start":   // 設定開始
                    // 予約情報がないセルのみイベントを受け付ける
                    var c = new RGBColor(tbl.rows[row].cells[col].style.backgroundColor);
                    if (c.toHex().toUpperCase() == "#FFFFFF") {
                        if (oldRow == "") {
                            // 新規設定の場合は、予約開始時刻を設定する
                            changeBackColor_List(row, col, col, "#FF0000");      // red
                            // 設定完了ボタンとキャンセルボタンを使用可能にする
                            document.getElementById("btnEnd1").style.visibility = "visible";
                            document.getElementById("btnCancel1").style.visibility = "visible";
                        }
                        else {
                            // 設定中の場合
                            if (parseInt(oldRow) == parseInt(row)) {
                                // 設定中の行と、今回クリックした行が同一の場合
                                if (col > oldColStart) {
                                    // 前回クリックしたセルよりも右側をクリックした場合で、
                                    // 途中に予約済みのデータがないときのみ処理をする
                                    for (i = parseInt(oldColStart) + 1; i <= col; i++) {
                                        var c = new RGBColor(tbl.rows[row].cells[i].style.backgroundColor);
                                        if ((c.toHex().toUpperCase() != "#FFFFFF") && ((c.toHex().toUpperCase() != "#FF0000"))) {
                                            break;
                                        }
                                    }
                                    if (i > col) {
                                        changeBackColor_List(row, oldColStart, col, "#FF0000");      //red
                                    }
                                }
                            }
                            else {
                                // 設定中の行と、今回クリックした行が異なる場合は
                                // 今まで設定した分をクリアして、新たに設定を始める
                                var c = new RGBColor(tbl.rows[oldRow].cells[oldColStart].style.backgroundColor);
                                if (c.toHex().toUpperCase() == "#FF0000") {
                                    changeBackColor_List(oldRow, oldColStart, oldColEnd, "#FFFFFF"); // white
                                }
                                changeBackColor_List(row, col, col, "#FF0000");      //red
                            }
                        }
                    }
                    break;
                case "end":     // 設定終了
                    break;
                default:
                    // 自分が予約したデータのみ修正可能
                    var c = new RGBColor(tbl.rows[row].cells[col].style.backgroundColor);
                    if (c.toHex().toUpperCase() == '#0000FF') {
                        // 背景色を変更しないので、クリックしたセルの位置だけ保存する
                        document.getElementById("hid_clickRow1").value = row;
                        document.getElementById("hid_clickColStart1").value = col;
                        document.getElementById("hid_clickColEnd1").value = col;

                        var tips = tbl.rows[row].cells[col].title;
                        tips = tips.replace(/\r\n?/g, "\n");
                        var tip = tips.split("\n");
                        document.getElementById("name").value = (tip[0] == undefined) ? "" : tip[0];

                        var time = (tip[1] == undefined) ? "" : tip[1];
                        selected_option("time_from", time.substr(1, 2));
                        selected_option("minutes_from", time.substr(4, 2));
                        selected_option("time_to", time.substr(9, 2));
                        selected_option("minutes_to", time.substr(12, 2));

                        document.getElementById("edit_yoto").value = (tip[2] == undefined) ? "" : tip[2];
                        $("#dialog-edit").dialog("open");
                        return false;
                    }
                    break;
            }
        }
    });

    // テーブルのセルをクリックしたときの処理（施設）
    $("#tblBooking2 td").click(function (event) {
        // currentTarget のindex()で列番号を取得
        var col = $(event.currentTarget).index();
        var row = $(event.currentTarget.parentNode).index();
        var mode = document.getElementById("hid_addMode2").value;
        var oldRow = document.getElementById("hid_clickRow2").value;
        var oldColStart = document.getElementById("hid_clickColStart2").value;
        var oldColEnd = document.getElementById("hid_clickColEnd2").value;
        var tbl = document.getElementById("tblBooking2");

        // 閲覧モードの場合は、処理を抜ける
        var editMode = document.getElementById("hid_editMode2").value;
        if (editMode.toLowerCase() == "false") return false;

        // ヘッダ行が2行あるので、3行目（row=2）からがデータ行となる
        if ((row > 1)) {
            switch (mode) {
                case "start":   // 設定開始
                    // 予約情報がないセルのみイベントを受け付ける
                    var c = new RGBColor(tbl.rows[row].cells[col].style.backgroundColor);
                    if (c.toHex().toUpperCase() == "#FFFFFF") {
                        if (oldRow == "") {
                            // 新規設定の場合は、予約開始時刻を設定する
                            changeBackColor_Shisetsu(row, col, col, "#FF0000");      // red
                            // 設定完了ボタンとキャンセルボタンを使用可能にする
                            document.getElementById("btnEnd2").style.visibility = "visible";
                            document.getElementById("btnCancel2").style.visibility = "visible";
                        }
                        else {
                            // 設定中の場合
                            if (parseInt(oldRow) == parseInt(row)) {
                                // 設定中の行と、今回クリックした行が同一の場合
                                if (col > oldColStart) {
                                    // 前回クリックしたセルよりも右側をクリックした場合で、
                                    // 途中に予約済みのデータがないときのみ処理をする
                                    for (i = parseInt(oldColStart) + 1; i <= col; i++) {
                                        var c = new RGBColor(tbl.rows[row].cells[i].style.backgroundColor);
                                        if ((c.toHex().toUpperCase() != "#FFFFFF") && ((c.toHex().toUpperCase() != "#FF0000"))) {
                                            break;
                                        }
                                    }
                                    if (i > col) {
                                        changeBackColor_Shisetsu(row, oldColStart, col, "#FF0000");      //red
                                    }
                                }
                            }
                            else {
                                // 設定中の行と、今回クリックした行が異なる場合は
                                // 今まで設定した分をクリアして、新たに設定を始める
                                var c = new RGBColor(tbl.rows[oldRow].cells[oldColStart].style.backgroundColor);
                                if (c.toHex().toUpperCase() == "#FF0000") {
                                    changeBackColor_Shisetsu(oldRow, oldColStart, oldColEnd, "#FFFFFF"); // white
                                }
                                changeBackColor_Shisetsu(row, col, col, "#FF0000");      //red
                            }
                        }
                    }
                    break;
                case "end":     // 設定終了
                    break;
                default:
                    // 自分が予約したデータのみ修正可能
                    var c = new RGBColor(tbl.rows[row].cells[col].style.backgroundColor);
                    if (c.toHex().toUpperCase() == '#0000FF') {
                        // 背景色を変更しないので、クリックしたセルの位置だけ保存する
                        document.getElementById("hid_clickRow2").value = row;
                        document.getElementById("hid_clickColStart2").value = col;
                        document.getElementById("hid_clickColEnd2").value = col;

                        var tips = tbl.rows[row].cells[col].title;
                        tips = tips.replace(/\r\n?/g, "\n");
                        var tip = tips.split("\n");
                        document.getElementById("name").value = (tip[0] == undefined) ? "" : tip[0];

                        var time = (tip[1] == undefined) ? "" : tip[1];
                        selected_option("time_from", time.substr(1, 2));
                        selected_option("minutes_from", time.substr(4, 2));
                        selected_option("time_to", time.substr(9, 2));
                        selected_option("minutes_to", time.substr(12, 2));

                        document.getElementById("edit_yoto").value = (tip[2] == undefined) ? "" : tip[2];
                        $("#dialog-edit").dialog("open");
                        return false;
                    }
                    break;
            }
        }
    });

    // 『Loading...』消去
    //$.unblockUI();
}

// ドロップダウンを選択状態にする
function selected_option(control_id, control_value) {
    var pulldown_option = document.getElementById(control_id).getElementsByTagName('option');
    for (i = 0; i < pulldown_option.length; i++) {
        if (pulldown_option[i].value == control_value) {
            pulldown_option[i].selected = true;
            break;
        }
    }
}

// スクロールバーを設定する（一覧タブ）
// ただし、非表示のテーブルのスクロール位置は設定できない
function table_scroll_List() {
    var div = document.getElementById("divRight1");
    var tbl = document.getElementById("tblBooking1");
    var scroll = document.getElementById("hid_scrollLeft1").value;
    var startTime = 8;                      // 表示開始時刻(時)
    var endTime = 21;                       // 表示終了時刻(時)
    var CellDuration = 15;                  // 1列の時間(分)
    var cellWidth = 30;                     // 列幅(px)
    var today = new Date();
    var nowtime = today.getHours();
    var col, scrollLeft;
    if (scroll == "") {
        if (nowtime < startTime) {
            col = 0;
        } else if (nowtime >= endTime) {
            col = ((endTime - startTime) * (60 / CellDuration)) - 1;
        } else {
            col = (nowtime - startTime) * (60 / CellDuration);
        }
        if (tbl.rows.length > 0) {
            //            scrollLeft = tbl.offsetLeft + tbl.rows[1].cells[col].offsetLeft;
            scrollLeft = tbl.offsetLeft + (cellWidth * col);
        }
        else {
            scrollLeft = tbl.offsetLeft;
        }
    }
    else {
        scrollLeft = scroll;
    }
    div.scrollLeft = scrollLeft;
    document.getElementById("hid_scrollLeft1").value = scrollLeft;
}

// スクロールバーを設定する（施設タブ）
// ただし、非表示のテーブルのスクロール位置は設定できない
function table_scroll_Shisetsu() {
    var div = document.getElementById("divRight2");
    var tbl = document.getElementById("tblBooking2");
    var scroll = document.getElementById("hid_scrollLeft2").value;
    var startTime = 8;                      // 表示開始時刻(時)
    var endTime = 21;                       // 表示終了時刻(時)
    var CellDuration = 15;                  // 1列の時間(分)
    var cellWidth = 30;                     // 列幅(px)
    var today = new Date();
    var nowtime = today.getHours();
    var col, scrollLeft;
    if (scroll == "") {
        if (nowtime < startTime) {
            col = 0;
        } else if (nowtime >= endTime) {
            col = ((endTime - startTime) * (60 / CellDuration)) - 1;
        } else {
            col = (nowtime - startTime) * (60 / CellDuration);
        }
        if (tbl.rows.length > 0) {
            //            scrollLeft = tbl.offsetLeft + tbl.rows[1].cells[col].offsetLeft;
            scrollLeft = tbl.offsetLeft + (cellWidth * col);
        }
        else {
            scrollLeft = tbl.offsetLeft;
        }
    }
    else {
        scrollLeft = scroll;
    }
    div.scrollLeft = scrollLeft;
    document.getElementById("hid_scrollLeft2").value = scrollLeft;
}

function dispLoading(tabNo) {
    // 『Loading...』表示
    $.blockUI({
        message: '<div><img src="../img/ajax-loader.gif"></div>'
    });
    // スクロールバーの位置をクリアする
    if (tabNo == 1) {
        document.getElementById("hid_scrollLeft1").value = "";
    }
    else {
        document.getElementById("hid_scrollLeft2").value = "";
    }
}


// 施設予約追加のためのボタンのイベント処理（一覧）
function onclick_add_List(mode) {

    // スクロールバーの位置を保存する
    setScrollLeft_List();

    switch (mode) {
        case 1:     // 設定開始
            document.getElementById("btnStart1").style.visibility = "hidden";
            document.getElementById("btnEnd1").style.visibility = "hidden";
            document.getElementById("btnCancel1").style.visibility = "visible";
            document.getElementById("hid_addMode1").value = "start";
            break;
        case 2:     // 設定終了
            document.getElementById("hid_addMode1").value = "end";
            // サーバ側に処理を渡す
            document.getElementById("btnDummy1").click();
            break;
        case 3:     // キャンセル
            // 設定中の状態を元に戻す
            var oldRow = document.getElementById("hid_clickRow1").value;
            var oldColStart = document.getElementById("hid_clickColStart1").value;
            var oldColEnd = document.getElementById("hid_clickColEnd1").value;
            changeBackColor_List(oldRow, oldColStart, oldColEnd, "#FFFFFF"); // white
            // ボタンを初期表示にする
            initButton_List();
            break;
        case 4:     // 削除
            document.getElementById("hid_addMode1").value = "delete";
            // サーバ側に処理を渡す
            document.getElementById("btnDummy1").click();
            break;
        case 5:     // 更新
            document.getElementById("hid_addMode1").value = "update";
            // サーバ側に処理を渡す
            document.getElementById("btnDummy1").click();
            break;
        default:
    }
    window.event.returnValue = false;
}

// 施設予約追加のためのボタンのイベント処理（施設）
function onclick_add_Shisetsu(mode) {

    // スクロールバーの位置を保存する
    setScrollLeft_Shisetsu();

    switch (mode) {
        case 1:     // 設定開始
            document.getElementById("btnStart2").style.visibility = "hidden";
            document.getElementById("btnEnd2").style.visibility = "hidden";
            document.getElementById("btnCancel2").style.visibility = "visible";
            document.getElementById("hid_addMode2").value = "start";
            break;
        case 2:     // 設定終了
            document.getElementById("hid_addMode2").value = "end";
            // サーバ側に処理を渡す
            document.getElementById("btnDummy2").click();
            break;
        case 3:     // キャンセル
            // 設定中の状態を元に戻す
            var oldRow = document.getElementById("hid_clickRow2").value;
            var oldColStart = document.getElementById("hid_clickColStart2").value;
            var oldColEnd = document.getElementById("hid_clickColEnd2").value;
            changeBackColor_Shisetsu(oldRow, oldColStart, oldColEnd, '#FFFFFF'); // white
            // ボタンを初期表示にする
            initButton_Shisetsu();
            break;
        case 4:     // 削除
            document.getElementById("hid_addMode2").value = "delete";
            // サーバ側に処理を渡す
            document.getElementById("btnDummy2").click();
            break;
        case 5:     // 更新
            document.getElementById("hid_addMode2").value = "update";
            // サーバ側に処理を渡す
            document.getElementById("btnDummy2").click();
            break;
        default:
    }
    window.event.returnValue = false;
}

function setScrollLeft() {
    setScrollLeft_List();
    setScrollLeft_Shisetsu();
}

// スクロールバーの位置を保存する（一覧）
function setScrollLeft_List() {
    var div = document.getElementById("divRight1");
    document.getElementById("hid_scrollLeft1").value = div.scrollLeft;
}

// スクロールバーの位置を保存する（施設）
function setScrollLeft_Shisetsu() {
    var div = document.getElementById("divRight2");
    document.getElementById("hid_scrollLeft2").value = div.scrollLeft;
}

// ボタンを初期表示にする（一覧）
function initButton_List() {
    var editMode = document.getElementById("hid_editMode1").value;
    if (editMode.toLowerCase() == "false") {
        // 閲覧モードの場合
        document.getElementById("btnStart1").style.visibility = "hidden";
        document.getElementById("btnEnd1").style.visibility = "hidden";
        document.getElementById("btnCancel1").style.visibility = "hidden";
    }
    else {
        // 編集モードの場合
        document.getElementById("btnStart1").style.visibility = "visible";
        document.getElementById("btnEnd1").style.visibility = "hidden";
        document.getElementById("btnCancel1").style.visibility = "hidden";
    }
    document.getElementById("hid_addMode1").value = "";
    document.getElementById("hid_clickRow1").value = "";
    document.getElementById("hid_clickColStart1").value = "";
    document.getElementById("hid_clickColEnd1").value = "";
    document.getElementById("hid_yoto1").value = "";
}

// ボタンを初期表示にする（施設）
function initButton_Shisetsu() {
    var editMode = document.getElementById("hid_editMode2").value;
    if (editMode.toLowerCase() == "false") {
        // 閲覧モードの場合
        document.getElementById("btnStart2").style.visibility = "hidden";
        document.getElementById("btnEnd2").style.visibility = "hidden";
        document.getElementById("btnCancel2").style.visibility = "hidden";
    }
    else {
        // 編集モードの場合
        document.getElementById("btnStart2").style.visibility = "visible";
        document.getElementById("btnEnd2").style.visibility = "hidden";
        document.getElementById("btnCancel2").style.visibility = "hidden";
    }
    document.getElementById("hid_addMode2").value = "";
    document.getElementById("hid_clickRow2").value = "";
    document.getElementById("hid_clickColStart2").value = "";
    document.getElementById("hid_clickColEnd2").value = "";
    document.getElementById("hid_yoto2").value = "";
}

function onclick_Cell(resource, datetime) {
    alert('resource = ' + resource + '\ndatetime = ' + datetime);
}

// 指定されたセルの背景色を変更する（一覧）
function changeBackColor_List(row, colStart, colEnd, color) {
    var tab = document.getElementById("tblBooking1");
    for (col = parseInt(colStart) ; col <= parseInt(colEnd) ; col++) {
        tab.rows[parseInt(row)].cells[col].style.backgroundColor = color;
    }
    document.getElementById("hid_clickRow1").value = row;
    document.getElementById("hid_clickColStart1").value = colStart;
    document.getElementById("hid_clickColEnd1").value = colEnd;
}

// 指定されたセルの背景色を変更する（施設）
function changeBackColor_Shisetsu(row, colStart, colEnd, color) {
    var tab = document.getElementById("tblBooking2");
    for (col = parseInt(colStart) ; col <= parseInt(colEnd) ; col++) {
        tab.rows[parseInt(row)].cells[col].style.backgroundColor = color;
    }
    document.getElementById("hid_clickRow2").value = row;
    document.getElementById("hid_clickColStart2").value = colStart;
    document.getElementById("hid_clickColEnd2").value = colEnd;
}

// 更新のときの入力エラーの処理（再表示）（一覧）
function showEditDialogWithError(tf, mf, tt, mt, yoto, msg) {
    if (document.getElementById("hid_selected_tab").value == "1") {
        var row = document.getElementById("hid_clickRow1").value;
        var col = document.getElementById("hid_clickColStart1").value;
        var tbl = document.getElementById("tblBooking1");
    } else {
        var row = document.getElementById("hid_clickRow2").value;
        var col = document.getElementById("hid_clickColStart2").value;
        var tbl = document.getElementById("tblBooking2");
    }

    var tips = tbl.rows[row].cells[col].title;
    tips = tips.replace(/\r\n?/g, "\n");
    var tip = tips.split("\n");
    document.getElementById("name").value = (tip[0] == undefined) ? "" : tip[0];

    selected_option("time_from", tf);
    selected_option("minutes_from", mf);
    selected_option("time_to", tt);
    selected_option("minutes_to", mt);

    document.getElementById("edit_yoto").value = yoto;
    $("#time_from").addClass("ui-state-error");
    updateTips(msg);

    $("#dialog-edit").dialog("open");
}

function updateTips(t) {
    var tips = $(".validateTips");
    tips
            .text(t)
            .addClass("ui-state-highlight");
    setTimeout(function () {
        tips.removeClass("ui-state-highlight", 1500);
    }, 500);
}

function checkLength(o, n, min, max) {
    if ($(o).val().length > max || $(o).val().length < min) {
        $(o).addClass("ui-state-error");
        updateTips(n + " は " + min + "文字以上" + max + "文字以下で入力してください");
        return false;
    } else {
        return true;
    }
}

// 使用用途入力ダイアログ
//$(function () {
//    var yoto = $("#txtYoto"),
//        allFields = $([]).add(yoto);

//    /*
//        function updateTips(t) {
//            tips
//                .text(t)
//                .addClass("ui-state-highlight");
//            setTimeout(function () {
//                tips.removeClass("ui-state-highlight", 1500);
//            }, 500);
//        }
    
//        function checkLength(o, n, min, max) {
//            if ($(o).val().length > max || $(o).val().length < min) {
//                $(o).addClass("ui-state-error");
//                updateTips(n + " は " + min + "文字以上" + max + "文字以下で入力してください");
//                return false;
//            } else {
//                return true;
//            }
//        }
//    */

//    $("#dialog-yoto").dialog({
//        autoOpen: false,
//        modal: true,
//        resizable: false,
//        width: 380,     // 2015.01.16 追加
//        buttons: {
//            "設定": function () {
//                var bValid = true;
//                allFields.removeClass("ui-state-error");

//                bValid = bValid && checkLength(txtYoto, "使用用途", 0, 20);
//                if (bValid) {
//                    document.getElementById("hid_yoto1").value = $("#txtYoto").val();
//                    document.getElementById("hid_yoto2").value = $("#txtYoto").val();
//                    $(this).dialog("close");
//                    // サーバに処理を渡す
//                    if (document.getElementById("hid_add_tab").value == "1") {
//                        onclick_add_List(2);
//                    }
//                    else {
//                        onclick_add_Shisetsu(2);
//                    }
//                }
//            },
//            "キャンセル": function () {
//                $(this).dialog("close");
//            }
//        },
//        close: function () {
//            allFields.val("").removeClass("ui-state-error");
//        }
//    });

//    $("#btnEnd1").click(function () {
//        document.getElementById("hid_add_tab").value = "1";
//        $("#dialog-yoto").dialog("open");
//        return false;
//    });

//    $("#btnEnd2").click(function () {
//        document.getElementById("hid_add_tab").value = "2";
//        $("#dialog-yoto").dialog("open");
//        return false;
//    });

//});

// 編集ダイアログ
//$(function () {
//    var yoto = $("#edit_yoto"),
//        time = $("#time_from"),
//        minit = $("#minutes_to"),
//        allFields = $([]).add(yoto).add(time).add(minit);

//    /*
//        function updateTips(t) {
//            tips
//                .text(t)
//                .addClass("ui-state-highlight");
//            setTimeout(function () {
//                tips.removeClass("ui-state-highlight", 1500);
//            }, 500);
//        }
    
//        function checkLength(o, n, min, max) {
//            if ($(o).val().length > max || $(o).val().length < min) {
//                $(o).addClass("ui-state-error");
//                updateTips(n + " は " + min + "文字以上" + max + "文字以下で入力してください");
//                return false;
//            } else {
//                return true;
//            }
//        }
//    */

//    function checkInputData() {
//        var from = getSelected("time_from") + getSelected("minutes_from");
//        var to = getSelected("time_to") + getSelected("minutes_to");
//        if (from >= to) {
//            $("#time_from").addClass("ui-state-error");
//            updateTips("予約時間を正しく入力してください");
//            return false;
//        } else {
//            return true;
//        }
//    }

//    function checkEndTime() {
//        var t = getSelected("time_to");
//        var m = getSelected("minutes_to");
//        if (t == '21') {
//            if (m != '00') {
//                $("#minutes_to").addClass("ui-state-error");
//                updateTips("終了時刻は 21:00 までで設定してください");
//                return false;
//            } else {
//                return true;
//            }
//        } else {
//            return true;
//        }
//    }

//    function getSelected(control_id) {
//        var retValue = '';
//        var pulldown_option = document.getElementById(control_id).getElementsByTagName('option');
//        for (i = 0; i < pulldown_option.length; i++) {
//            if (pulldown_option[i].selected) {
//                retValue = pulldown_option[i].value;
//                break;
//            }
//        }
//        return retValue;
//    }

//    $("#dialog-edit").dialog({
//        autoOpen: false,
//        modal: true,
//        resizable: false,
//        width: 380,     // 2015.01.16 追加
//        buttons: {
//            "削除": function () {
//                var ret = confirm("施設予約を削除してよろしいですか？");
//                if (ret == true) {
//                    $(this).dialog("close");
//                    // サーバに処理を渡す
//                    if (document.getElementById("hid_selected_tab").value == "1") {
//                        onclick_add_List(4);
//                    }
//                    else {
//                        onclick_add_Shisetsu(4);
//                    }
//                }
//            },
//            "更新": function () {
//                var bValid = true;
//                allFields.removeClass("ui-state-error");
//                bValid = bValid && checkEndTime();
//                bValid = bValid && checkInputData();
//                bValid = bValid && checkLength(edit_yoto, "使用用途", 0, 20);
//                if (bValid) {
//                    // サーバに処理を渡す
//                    if (document.getElementById("hid_selected_tab").value == "1") {
//                        document.getElementById("hid_time_from1").value = getSelected("time_from");
//                        document.getElementById("hid_minutes_from1").value = getSelected("minutes_from");
//                        document.getElementById("hid_time_to1").value = getSelected("time_to");
//                        document.getElementById("hid_minutes_to1").value = getSelected("minutes_to");
//                        document.getElementById("hid_yoto1").value = $("#edit_yoto").val();
//                        $(this).dialog("close");
//                        onclick_add_List(5);
//                    }
//                    else {
//                        document.getElementById("hid_time_from2").value = getSelected("time_from");
//                        document.getElementById("hid_minutes_from2").value = getSelected("minutes_from");
//                        document.getElementById("hid_time_to2").value = getSelected("time_to");
//                        document.getElementById("hid_minutes_to2").value = getSelected("minutes_to");
//                        document.getElementById("hid_yoto2").value = $("#edit_yoto").val();
//                        $(this).dialog("close");
//                        onclick_add_Shisetsu(5);
//                    }
//                }
//            },
//            "キャンセル": function () {
//                document.getElementById("hid_clickRow1").value = "";
//                document.getElementById("hid_clickColStart1").value = "";
//                document.getElementById("hid_clickColEnd1").value = "";
//                document.getElementById("hid_clickRow2").value = "";
//                document.getElementById("hid_clickColStart2").value = "";
//                document.getElementById("hid_clickColEnd2").value = "";
//                $(this).dialog("close");
//            }
//        },
//        close: function () {
//            updateTips("");
//            allFields.val("").removeClass("ui-state-error");
//        }
//    });
//});
