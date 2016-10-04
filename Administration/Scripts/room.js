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

function pageLoaded(sender, args) {

    // スクロールバーを設定する
    //    setScrollBar();
    table_scroll_List();                // 一覧タブ
    table_scroll_Shisetsu();            // 会議室タブ

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

        // popoverを閉じる
        $('[data-toggle="popover"]').popover("hide");

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

                        //var tips = tbl.rows[row].cells[col].title;
                        //tips = tips.replace(/\r\n?/g, "\n");
                        //var tip = tips.split("\n");
                        //document.getElementById("txtYotoEdit").value = (tip[0] == undefined) ? "" : tip[0];

                        //var time = (tip[1] == undefined) ? "" : tip[1];
                        //document.getElementById("txtFrom").value = time.substr(1, 5);
                        //document.getElementById("txtTo").value = time.substr(9, 5);

                        var from = tbl.rows[row].cells[col].getAttribute("hide-start");
                        var to = tbl.rows[row].cells[col].getAttribute("hide-end");
                        $('#selRoomEdit').selectpicker('val', tbl.rows[row].cells[col].getAttribute("hide-roomId"));
                        $("#txtDate").datepicker("setDate", from.substr(0, 10));
                        $("#txtFrom").timepicker("setTime", from.substr(11, 5));
                        $("#txtTo").timepicker("setTime", to.substr(11, 5));
                        document.getElementById("txtYotoEdit").value = tbl.rows[row].cells[col].getAttribute("hide-comment");

                        $("#dialogEdit").modal();
                        return false;
                    }
                    break;
            }
        }
    });

    // テーブルのセルをクリックしたときの処理（会議室）
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

        // popoverを閉じる
        $('[data-toggle="popover"]').popover("hide");

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
                        document.getElementById("txtYotoEdit").value = (tip[0] == undefined) ? "" : tip[0];

                        var time = (tip[1] == undefined) ? "" : tip[1];
                        document.getElementById("txtFrom").value = time.substr(1, 5);
                        document.getElementById("txtTo").value = time.substr(9, 5);

                        $("#dialogEdit").modal();
                        return false;
                    }
                    break;
            }
        }
    });

    // 『Loading...』消去
    $.unblockUI();
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

// スクロールバーを設定する（会議室タブ）
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
        message: '<div><img src="/Images/ajax-loader.gif"></div>'
    });
    //スクロールバーの位置をクリアする
    if (tabNo == 1) {
        document.getElementById("hid_scrollLeft1").value = "";
    }
    else {
        document.getElementById("hid_scrollLeft2").value = "";
    }
}


// 会議室予約追加のためのボタンのイベント処理（一覧）
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

// 会議室予約追加のためのボタンのイベント処理（会議室）
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

// スクロールバーの位置を保存する（会議室）
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

// ボタンを初期表示にする（会議室）
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

// 指定されたセルの背景色を変更する（会議室）
function changeBackColor_Shisetsu(row, colStart, colEnd, color) {
    var tab = document.getElementById("tblBooking2");
    for (col = parseInt(colStart) ; col <= parseInt(colEnd) ; col++) {
        tab.rows[parseInt(row)].cells[col].style.backgroundColor = color;
    }
    document.getElementById("hid_clickRow2").value = row;
    document.getElementById("hid_clickColStart2").value = colStart;
    document.getElementById("hid_clickColEnd2").value = colEnd;
}

// 更新のときの入力エラーの処理（再表示）
function showEditDialogWithError(tf, mf, tt, mt, yoto, msg) {
    // 登録者名を表示しないのでコメントアウト
    //if (document.getElementById("hid_selected_tab").value == "1") {
    //    var row = document.getElementById("hid_clickRow1").value;
    //    var col = document.getElementById("hid_clickColStart1").value;
    //    var tbl = document.getElementById("tblBooking1");
    //} else {
    //    var row = document.getElementById("hid_clickRow2").value;
    //    var col = document.getElementById("hid_clickColStart2").value;
    //    var tbl = document.getElementById("tblBooking2");
    //}

    //var tips = tbl.rows[row].cells[col].title;
    //tips = tips.replace(/\r\n?/g, "\n");
    //var tip = tips.split("\n");
    //document.getElementById("name").value = (tip[0] == undefined) ? "" : tip[0];

    document.getElementById("txtFrom").value = tf + ":" + mf;
    document.getElementById("txtTo").value = tt + ":" + mt;
    document.getElementById("txtYotoEdit").value = yoto;
    $("#txtFrom").addClass("ui-state-error");
    updateTips(msg);

    $("#dialogEdit").modal();
}

// 編集ダイアログ用
function updateTips(t) {
    var tips = $(".validateTips");
    tips.text(t);
    // 消す時の動きが滑らかでないので、ハイライトは付けない　2016.10.03
    //tips
    //        .text(t)
    //        .addClass("ui-state-highlight");
    //setTimeout(function () {      
    //    tips.removeClass("ui-state-highlight", 1500);
    //}, 500);
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

 //使用用途入力ダイアログ
$(function () {
    var yoto = $("#txtYoto"),
        allFields = $([]).add(yoto);

    // ダイアログを閉じる時の処理
    $("#dialogYoto").on("hide.bs.modal", function (e) {
        allFields.removeClass("ui-state-error");
    });

    // ダイアログ内のキャンセルボタンをクリックされたとき
    $("#btnCancelYoto").click(function () {
        // 使用用途入力ダイアログを閉じる
        $("#dialogYoto").modal("hide");
        return false;
    });

    // ダイアログ内の設定ボタンをクリックされたとき
    $("#btnSaveYoto").click(function () {
        var bValid = true;
        allFields.removeClass("ui-state-error");

        bValid = bValid && checkLength("#txtYoto", "使用用途", 0, 50);
        if (bValid) {
            document.getElementById("hid_yoto1").value = $("#txtYoto").val();
            document.getElementById("hid_yoto2").value = $("#txtYoto").val();
            $("#dialogYoto").modal("hide");
             // サーバに処理を渡す
            if (document.getElementById("hid_add_tab").value == "1") {
                onclick_add_List(2);
            }
            else {
                onclick_add_Shisetsu(2);
            }
        }
    });

    $("#btnEnd1").click(function () {
        // 使用用途入力ダイアログを表示する
        document.getElementById("hid_add_tab").value = "1";
        $("#dialogYoto").modal();
        return false;
    });

    $("#btnEnd2").click(function () {
        // 使用用途入力ダイアログを表示する
        document.getElementById("hid_add_tab").value = "2";
        $("#dialogYoto").modal();
        return false;
    });

});

// 編集ダイアログ
$(function () {
    var yoto = $("#txtYotoEdit"),
        from = $("#txtFrom"),
        to = $("#txtTo"),
        allFields = $([]).add(yoto).add(from).add(to);

    // ダイアログを閉じる時の処理
    $("#dialogEdit").on("hide.bs.modal", function (e) {
        updateTips("");
        //allFields.val("").removeClass("ui-state-error");      どちらでもOK  2016.10.03
        allFields.removeClass("ui-state-error");
    });

    // ダイアログ内の削除ボタンをクリックされたとき
    $("#btnDeleteEdit").click(function () {
        var ret = confirm("会議室予約を削除してよろしいですか？");
        if (ret == true) {
            $("#dialogEdit").modal("hide");
            // サーバに処理を渡す
            if (document.getElementById("hid_selected_tab").value == "1") {
                onclick_add_List(4);
            }
            else {
                onclick_add_Shisetsu(4);
            }
        }
    });

    // ダイアログ内のキャンセルボタンをクリックされたとき
    $("#btnCancelEdit").click(function () {
        document.getElementById("hid_clickRow1").value = "";
        document.getElementById("hid_clickColStart1").value = "";
        document.getElementById("hid_clickColEnd1").value = "";
        document.getElementById("hid_clickRow2").value = "";
        document.getElementById("hid_clickColStart2").value = "";
        document.getElementById("hid_clickColEnd2").value = "";
        // 編集ダイアログを閉じる
        $("#dialogEdit").modal("hide");
        return false;
    });
    
    // ダイアログ内の更新ボタンをクリックされたとき
    $("#btnSaveEdit").click(function () {
        var bValid = true;
        allFields.removeClass("ui-state-error");

        var from = ("0" + document.getElementById("txtFrom").value.trim()).substr(-5, 5);
        var to = ("0" + document.getElementById("txtTo").value.trim()).substr(-5, 5);
        document.getElementById("txtFrom").value = from
        document.getElementById("txtTo").value = to
        // 開始時刻と終了時刻のチェックは外しておく 2016.10.03
        //bValid = bValid && checkStartTime();
        //bValid = bValid && checkEndTime();
        bValid = bValid && checkInputData();
        bValid = bValid && checkLength("#txtYotoEdit", "使用用途", 0, 50);
        if (bValid) {
            // サーバに処理を渡す
            if (document.getElementById("hid_selected_tab").value == "1") {
                document.getElementById("hid_time_from1").value = from.substr(0, 2);
                document.getElementById("hid_minutes_from1").value = from.substr(3, 2);
                document.getElementById("hid_time_to1").value = to.substr(0, 2);
                document.getElementById("hid_minutes_to1").value = to.substr(3, 2);
                document.getElementById("hid_yoto1").value = $("#txtYotoEdit").val();
                $("#dialogEdit").modal("hide");
                onclick_add_List(5);
            }
            else {
                document.getElementById("hid_time_from2").value = from.substr(0, 2);
                document.getElementById("hid_minutes_from2").value = from.substr(3, 2);
                document.getElementById("hid_time_to2").value = to.substr(0, 2);
                document.getElementById("hid_minutes_to2").value = to.substr(3, 2);
                document.getElementById("hid_yoto2").value = $("#txtYotoEdit").val();
                $("#dialogEdit").modal("hide");
                onclick_add_Shisetsu(5);
            }
        }
    });

    function checkInputData() {
        var ret = true;
        var from = document.getElementById("txtFrom").value;
        var to = document.getElementById("txtTo").value;

        // 日付として正しいかチェックする
        var target = document.getElementById("txtDate").value.trim();
        document.getElementById("txtDate").value = target;
        if (!ckDate(target)) {
            $("#txtDate").addClass("ui-state-error");
            updateTips("予約日付を正しく入力してください");
            ret = false;
        }

        // 時刻として正しいかチェックする
        if (!ckTime(from)) {
            $("#txtFrom").addClass("ui-state-error");
            updateTips("予約開始時刻を正しく入力してください");
            ret = false;
        }
        if (!ckTime(to)) {
            $("#txtTo").addClass("ui-state-error");
            updateTips("予約終了時刻を正しく入力してください");
            ret = false;
        }
        if (!ret) {
            return ret;
        }
        // 時刻の大小関係をチェックする
        if (from >= to) {
            $("#txtTo").addClass("ui-state-error");
            updateTips("予約時間を正しく入力してください");
            ret = false;
        }
        return ret;
    }

    function checkStartTime() {
        var from = document.getElementById("txtFrom").value;
        if (from < '08:00') {
            $("#txtFrom").addClass("ui-state-error");
            updateTips("開始時刻は 08:00 から設定してください");
            return false;
        } else {
            return true;
        }
    }

    function checkEndTime() {
        var to = document.getElementById("txtTo").value;
        if (to > '21:30') {
            $("#txtTo").addClass("ui-state-error");
            updateTips("終了時刻は 21:30 までで設定してください");
            return false;
        } else {
            return true;
        }
    }

});

// マニュアルモード
// マニュアルモード表示のチェックボックスが変更されたとき
function onclick_chkManualMode() {
    if (document.getElementById("chkManualMode").checked) {
        document.getElementById("reserve_bg2").style.display = "block";
    } else {
        document.getElementById("reserve_bg2").style.display = "none";
    }
}
// マニュアルモードの設定完了ボタンをクリックされたときの処理
function btnComplete_OnClick() {
    var target = $("#txtBookTimeTo"),
        yoto = $("#txtUse"),
        from = $("#txtBookTimeFrom"),
        to = $("#txtTo"),
        allFields = $([]).add(target).add(from).add(to).add(yoto);

    var bValid = true;
    updateTips2("");
    allFields.removeClass("ui-state-error");
    bValid = bValid && checkInputDataManual();
    return bValid;
}
// 入力チェック
function checkInputDataManual() {
    var ret1 = true;
    var ret2 = true;

    // 日付として正しいかチェックする
    var target = document.getElementById("txtBookDate").value.trim();
    document.getElementById("txtBookDate").value = target;
    if (!ckDate(target)) {
        $("#txtBookDate").addClass("ui-state-error");
        updateTips2("予約日付を正しく入力してください");
        ret1 = false;
    }

    // 時刻として正しいかチェックする
    var from = ("0" + document.getElementById("txtBookTimeFrom").value.trim()).substr(-5, 5);
    var to = ("0" + document.getElementById("txtBookTimeTo").value.trim()).substr(-5, 5);
    document.getElementById("txtBookTimeFrom").value = from
    document.getElementById("txtBookTimeTo").value = to

    if (!ckTime(from)) {
        $("#txtBookTimeFrom").addClass("ui-state-error");
        updateTips2("予約開始時刻を正しく入力してください");
        ret2 = false;
    }
    if (!ckTime(to)) {
        $("#txtBookTimeTo").addClass("ui-state-error");
        updateTips2("予約終了時刻を正しく入力してください");
        ret2 = false;
    }
    if (!ret2) {
        return ret2;
    }
    // 時刻の大小関係をチェックする
    if (from >= to) {
        $("#txtBookTimeTo").addClass("ui-state-error");
        updateTips2("予約時間を正しく入力してください");
        ret2 = false;
    }
    return (ret1 && ret2);
}
// エラーメッセージを表示する。
function updateTips2(t) {
    var tips = $(".validateTips2");
    if (t != "") {
        var s = tips.text();
        t = s + "<br />" + t;
    }
    tips.text(t);
    // 消す時の動きが滑らかでないので、ハイライトは付けない　2016.10.03
    //tips
    //        .text(t)
    //        .addClass("ui-state-highlight");
    //setTimeout(function () {      
    //    tips.removeClass("ui-state-highlight", 1500);
    //}, 500);
}

// 入力された値が日付でYYYY/MM/DD形式になっているか調べる
function ckDate(datestr) {
    // 正規表現による書式チェック
    if (!datestr.match(/^\d{4}\/\d{2}\/\d{2}$/)) {
        return false;
    }
    var vYear = datestr.substr(0, 4) - 0;
    var vMonth = datestr.substr(5, 2) - 1; // Javascriptは、0-11で表現
    var vDay = datestr.substr(8, 2) - 0;
    // 月,日の妥当性チェック
    if (vMonth >= 0 && vMonth <= 11 && vDay >= 1 && vDay <= 31) {
        var vDt = new Date(vYear, vMonth, vDay);
        if (isNaN(vDt)) {
            return false;
        } else if (vDt.getFullYear() == vYear && vDt.getMonth() == vMonth && vDt.getDate() == vDay) {
            return true;
        } else {
            return false;
        }
    } else {
        return false;
    }
}

// 入力された値が時間でHH:MM形式になっているか調べる
function ckTime(str) {
    // 正規表現による書式チェック
    if (!str.match(/^\d{2}\:\d{2}$/)) {
        return false;
    }
    var vHour = str.substr(0, 2) - 0;
    var vMinutes = str.substr(3, 2) - 0;
    if (vHour >= 0 && vHour <= 24 && vMinutes >= 0 && vMinutes <= 59) {
        return true;
    } else {
        return false;
    }
}

