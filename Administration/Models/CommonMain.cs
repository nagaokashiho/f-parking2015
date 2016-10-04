using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Administration.Models
{
    class CommonMain
    {
        //-- エラー --
        public const string CNS_ERR_MSGERR = "エラー ： ";
        public const string CNS_ERR_MSG001 = "該当データがありません";
        public const string CNS_ERR_MSGM01 = "エラー ：【";
        public const string CNS_ERR_MSGM02 = "】が未選択です";
        public const string CNS_ERR_MSGM03 = "】が不正です";
        public const string CNS_ERR_MSGM04 = "】が日付として不正です";
        public const string CNS_ERR_MSGM05 = "】が未入力です";
        public const string CNS_ERR_MSGWOR = "ワーニング ： ";

        //-- ワーニング --
        public const string CNS_ALM_MSG001 = "";
        public const string CNS_ALM_MSGM01 = "ワーニング ：【";
        public const string CNS_ALM_MSGM02 = "】が重複しています";
        public const string CNS_ALM_MSGM03 = "ワーニング ： 他のデータで使用中のため削除できません";
        public const string CNS_ALM_MSGM04 = "ワーニング ： 印刷するデータが存在しません";
        public const string CNS_ALM_MSGM05 = "】を行ってください";
        public const string CNS_ALM_MSGM06 = "】を実施してください";
        public const string CNS_ALM_MSGM07 = "ワーニング ： 対象のデータが存在しません";
        public const string CNS_ALM_MSGM08 = "ワーニング ： 対象のデータは削除されています";
        public const string CNS_ALM_MSGM09 = "ワーニング ： 対象のデータに関して処理する権限がありません";

        //-- 確認 --
        public const string CNS_CNF_MSGM01 = "【";
        public const string CNS_CNF_MSGM02 = "】を削除してよろしいですか？";
        public const string CNS_CNF_MSGM03 = "全て削除してよろしいですか？";
        public const string CNS_CNF_MSGM04 = "】をデータベースから削除してよろしいですか？";

        //-- 情報 -- 
        public const string CNS_INF_MSG01 = "更新が完了しました";
        public const string CNS_INF_MSG02 = "削除が完了しました";
        

        /// <summary>
        /// メッセージボックスを表示する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
        public static void subMessageBox(System.Web.UI.Page target, string message)
        {
            string strMessageData = message.Replace(Environment.NewLine, "\\n");

            string script = "<script language='javascript'>" + "window.alert('" + strMessageData + "');" + "</script>";

            //target.Response.Write(script) <--- 背景が真っ白になってしまう！
            target.ClientScript.RegisterStartupScript(target.GetType(), "script_name", script);
        }

        /// <summary>
        /// メッセージボックスを表示する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
        public static void subMessageBox_upd(System.Web.UI.Page target, string message)
        {

            string strMessageData = message.Replace(Environment.NewLine, "\\n");

            string script = "<script language='javascript'>" + "alert('" + strMessageData + "');" + "</script>";

            ScriptManager.RegisterClientScriptBlock(target, target.GetType(), "script_name", script, false);

        }
    }
}
