// テキストボックスでのEnterキーを無効にする（改行は有効）
function cancelEnter() {
    if (event.keyCode == 13) {
        if (window.event.srcElement.type != 'submit' && window.event.srcElement.type != 'textarea') {
            return false;
        }
    }
}

// ダイアログ表示
function myPageLoaded(sender, args) {
    var actionName = $('#hid_ActionName').val();
    var title = $('#hid_title').val();

    if (actionName == 'Select' || actionName == 'New') {
        $('#editDialog').attr('title', title).dialog({
            bgiframe: true,
            width: 900,
            height: 700,
            modal: true,
            buttons: {
                '保存': function () {
                    document.getElementById("btn_save").click();
                },
                'キャンセル': function () {
                    $(this).dialog('close');
                }
            },
            close: function () {
                $('#hid_ActionName').val('');
                $('#hid_title').val('');
                $(this).dialog('destroy');
                window.event.returnValue = false;
            }
        });
    } else if (actionName == 'Show') {
        $('#editDialog').attr('title', title).dialog({
            bgiframe: true,
            width: 900,
            height: 700,
            modal: true,
            buttons: {
                '閉じる': function () {
                    $(this).dialog('close');
                }
            },
            close: function () {
                $('#hid_ActionName').val('');
                $('#hid_title').val('');
                $(this).dialog('destroy');
                window.event.returnValue = false;
            }
        });
    }
}

// btn_saveクリック時に、クライアント側での検証コントロールの結果で
// ダイアログを閉じるかどうか判断する
function on_btn_save_clientclick() {
    if (Page_ClientValidate()) {
        $('#editDialog').dialog('close');
    } else {
        // エラーメッセージが見えるようにスクロール位置を先頭にする
        $('#editDialog').scrollTop(0);
    }
}

// カレンダーを設定する
function setCalendar() {
    var op = {
        closeText: '閉じる',
        prevText: '&#x3c;前',
        nextText: '次&#x3e;',
        currentText: '今日',
        monthNames: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
        monthNamesShort: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
        dayNames: ['日曜日', '月曜日', '火曜日', '水曜日', '木曜日', '金曜日', '土曜日'],
        dayNamesShort: ['日', '月', '火', '水', '木', '金', '土'],
        dayNamesMin: ['日', '月', '火', '水', '木', '金', '土'],
        weekHeader: '週',
        dateFormat: 'yy/mm/dd',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: '年',
        showOn: 'both',
        buttonImageOnly: true,
        buttonImage: '../images/calendar.jpg'
    };

    $(".txt_Date").datepicker(op);
    $(".txt_Date").attr({
        size: "13",
        autocomplete: "off"
    });
}

// 利用日付の入力チェックを行う
function checkUseOfDate(sender, args) {
    var datestr = args.Value;
    var msg = new Array();
    if (!checkInputDate(datestr, msg)) {
        args.IsValid = false;
    }
    args.IsValid = true;
}

// 日付の入力チェックを行う
function checkInputDate(datestr, msg) {
    // 未入力はOKとする
    if (datestr == '') {
        return true;
    }
    // 正規表現による書式のチェックをする
    if (!datestr.match(/^\d{4}\/\d{2}\/\d{2}$/)) {
        msg[0] = "日付は『yyyy/MM/dd』形式で入力してください";
        return false;
    }

    // 日付の妥当性をチェックする
    var year = datestr.substr(0, 4);
    var month = datestr.substr(5, 2);
    var day = datestr.substr(8, 2);
    if (checkDate(year, month, day)) {
        // OK
        return true;
    } else {
        // NG
        msg[0] = "日付として正しくありません";
        return false;
    }
}

/**
* 日付の妥当性チェック
* year 年
* month 月
* day 日
*/
function checkDate(year, month, day) {
    var dt = new Date(year, month - 1, day);
    if (dt == null || dt.getFullYear() != year || dt.getMonth() + 1 != month || dt.getDate() != day) {
        return false;
    }
    return true;
}
