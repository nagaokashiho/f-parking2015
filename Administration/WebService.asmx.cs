using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using Administration.Models;

namespace Administration
{
    /// <summary>
    /// WebService の概要の説明です
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // この Web サービスを、スクリプトから ASP.NET AJAX を使用して呼び出せるようにするには、次の行のコメントを解除します。
    [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        private CommonDB commonDB = new CommonDB();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// 指定した会議室の予約状況を取得する
        /// </summary>
        /// <param name="roomId">会議室ID</param>
        /// <param name="startDatetime">予約開始日時</param>
        /// <param name="endDatetime">予約終了日時</param>
        /// <param name="outYoyakuID">検索時に除外する予約ID</param>
        /// <returns>true:予約済　false:空き</returns>
        [WebMethod]
        public bool isReserved(string roomId, DateTime startDatetime, DateTime endDatetime, int outYoyakuId = 0)
        {
            // 【予約テーブル】『Reservations』に指定範囲内に既に予約データが登録されているかチェックする。
            string message = "";
            ErrorInfo errorInfo = new ErrorInfo();
            if (commonDB.fncCheck_YoyakuData_TimeRange_UMU(startDatetime, endDatetime, roomId, outYoyakuId, ref message, ref errorInfo))
            {
                // データが登録されている
                return true;
            }
            return false;
        }
    }
}
