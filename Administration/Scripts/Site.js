// ****************************************************
// サイト共通
// ****************************************************

$(function () {
    // Bootstrap DatePicker
    $('.datepicker').datepicker({
        autoclose: 'true',
        format: 'yyyy/mm/dd',
        language: 'ja',
        todayHighlight: true,
        beforeShowDay: function(date) {
            // まず、祝日をチェック
            var myDate = new Object();
            var dd = date.getFullYear() + "/" + (date.getMonth() + 1) + "/" + date.getDate();
            var hName = ktHolidayName(dd);
            // まず、祝日をチェック
            if(hName != "") {
                myDate.enabled = true;
                myDate.classes = "date-holiday";
                myDate.tooltip = hName;
            } else {
                // 次に、曜日をチェック
                switch (date.getDay()) {
                    case 0:     // 日曜日
                        myDate.enabled = true;
                        myDate.classes = "date-sunday";
                        myDate.tooltip = "";
                        break;
                    case 6:     // 土曜日
                        myDate.enabled = true;
                        myDate.classes = "date-saturday";
                        myDate.tooltip = "";
                        break;
                    default:
                        myDate.enabled = true;
                        myDate.classes = "";
                        myDate.tooltip = "";
                }
            }
            return myDate;
        },
    });

    // Bootstrap TimePicker
    $('.timepicker').timepicker({
        showInputs: false,      // ウィジェット内のテキスト編集不可
        minuteStep: 30,         // minute(分)フィールドの刻み間隔（分）
        showMeridian: false,    // 24時間表示
    });

    // Bootstrap SelectPickse
    $('.selectpicker').selectpicker({
        //width: 'fit',
    });

    // Bootstrap Popover
    $('[data-toggle="popover"]').popover({
        container: 'body',
        trigger: 'hover',
        html: true,
        placement: 'auto',
    });
});
