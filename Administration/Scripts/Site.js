// ****************************************************
// 貸し会議室管理用
// ****************************************************

$(function () {
    // DatePicker
    $('.datepicker').datepicker({
        autoclose: 'true',
        format: 'yyyy/mm/dd',
        language: 'ja',
        todayHighlight: true,
        beforeShowDay: function(date) {
            var result;
            var dd = date.getFullYear() + "/" + (date.getMonth() + 1) + "/" + date.getDate();
            var hName = ktHolidayName(dd);
            // まず、祝日をチェック
            if(hName != "") {
                result = [true, "date-holiday", hName];
            } else {
                // 次に、曜日をチェック
                switch (date.getDay()) {
                    case 0: //日曜日
                        result = [true, "date-holiday"];
                        break;
                    case 6: //土曜日
                        result = [true, "date-saturday"];
                        break;
                    default:
                        result = [true];
                        break;
                }
            }
            return result;
        },    
    });

   // TimePicker
    $('.timepicker').timepicker({
        showInputs: false,
        disableFocus: true,
        minuteStep: 30
    });
});
