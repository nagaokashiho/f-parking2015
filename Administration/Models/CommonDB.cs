using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Administration.Models
{
    class CommonDB
    {
        // DBコンテキスト
        private f_parkingContext db = new f_parkingContext();

         /// <summary>
        /// 会議室マスタをidの昇順に取得する
        /// </summary>
        /// <returns></returns>
        public IQueryable<Room> SelectRooms()
        {
            return db.Rooms.OrderBy(room => room.Id);
        }

         /// <summary>
        /// 【予約テーブル】から「現在日時」のデータを取得する。
        /// </summary>
        /// <param name="reservations"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="sisetsuId"></param>
        /// <param name="userID"></param>
        /// <param name="message"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool fncGetData_Reservations_Date(ref Reservation[] reservations, string startDateTime, string endDateTime, string sisetsuId, string userID, ref string message, ref ErrorInfo errorInfo)
        {
            int iCnt = 0;
            int intRecCounts = 0;
            DataRow drDtRow = null;

            //SQL設定
            System.Text.StringBuilder strSQL = new System.Text.StringBuilder();

            strSQL.AppendLine("SELECT Reservations.* ");
            strSQL.AppendLine("FROM ");
            strSQL.AppendLine("Reservations ");
            strSQL.AppendLine("WHERE ");
            strSQL.AppendLine("Reservations.DeleteFlag = 0 ");

            //「期間開始日時」が指定されている場合
            if (!string.IsNullOrEmpty(startDateTime))
            {
                strSQL.AppendLine("AND ");
                strSQL.AppendLine("Reservations.StartDateTime >= CAST('" + startDateTime + "' AS DATETIME) ");
            }

            //「期間終了日時」が指定されている場合
            if (!string.IsNullOrEmpty(startDateTime))
            {
                strSQL.AppendLine("AND ");
                strSQL.AppendLine("Reservations.EndDateTime < CAST('" + endDateTime + "' AS DATETIME) ");
            }

            //「会議室Id」が指定されている場合
            if (!string.IsNullOrEmpty(sisetsuId))
            {
                strSQL.AppendLine("AND ");
                strSQL.AppendLine("Reservations.RoomId = '" + sisetsuId + "' ");
            }

            //「予約者Id」が指定されている場合
            if (!string.IsNullOrEmpty(userID))
            {
                strSQL.AppendLine("AND ");
                strSQL.AppendLine("Reservations.UserId = '" + userID + "' ");
            }

            strSQL.AppendLine("ORDER BY Reservations.RoomId ");


            //========================== SQL実行 ===============================
            DataSet dsDtSet = GetDataSet(strSQL.ToString(), ref errorInfo);
            //==================================================================

            try
            {
                //「データ件数」を設定
                intRecCounts = dsDtSet.Tables[0].Rows.Count;

                //データが存在したか？
                if (intRecCounts > 0)
                {
                    //データ件数分
                    for (iCnt = 0; iCnt < intRecCounts; iCnt++)
                    {
                        //結果を参照
                        drDtRow = dsDtSet.Tables[0].Rows[iCnt];
                        //構造体に画面情報を格納
                        Array.Resize(ref reservations, iCnt + 1);

                        reservations[iCnt].Id = (int)fncChangeNull_To_Numeric(drDtRow["Id"]);
                        reservations[iCnt].RoomId = fncChangeNull_To_String(drDtRow["RoomId"]);
                        reservations[iCnt].StartDateTime = (DateTime)fncChangeNull_To_Date(drDtRow["StartDateTime"]);
                        reservations[iCnt].EndDateTime = (DateTime)fncChangeNull_To_Date(drDtRow["EndDateTime"]);
                        reservations[iCnt].Comment = fncChangeNull_To_String(drDtRow["Comment"]);
                        reservations[iCnt].UserId = fncChangeNull_To_String(drDtRow["UserId"]);
                        reservations[iCnt].DeleteFlag = (bool)fncChangeNull_To_Bit(drDtRow["DeleteFlag"]);
                        reservations[iCnt].AddUserId = fncChangeNull_To_String(drDtRow["AddUserId"]);
                        reservations[iCnt].EditUserId = fncChangeNull_To_String(drDtRow["EditUserId"]);
                        reservations[iCnt].AddDateTime = (DateTime)fncChangeNull_To_Date(drDtRow["AddDateTime"]);
                        reservations[iCnt].LastEditDateTime = (DateTime)fncChangeNull_To_Date(drDtRow["LastEditDateTime"]);
                    }
                    //戻り値設定
                    return true;
                }
                else
                {
                    //戻り値設定
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorInfo.Sql = strSQL.ToString();              //エラー発生SQL内容
                errorInfo.RetMessage = ex.Message;              //エラーメッセージ
                errorInfo.RetStackTrace = ex.StackTrace;        //メソッドが呼び出された順番
                errorInfo.RetString = ex.ToString();            //例外の内容
                return false;
            }
        }

        /// <summary>
        /// DataSetを取得する。
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sqlstr, ref ErrorInfo errorInfo)
        {
            DataSet tempGetDataSet = null;
            tempGetDataSet = null;

            //Dim connString As System.Configuration.ConnectionStringSettings = WebConfigurationManager.ConnectionStrings("conStringAdoSql3")

            //[web.config]から、Sql情報を取得する。
            string AdoDBString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            SqlConnection Conn = null;
            SqlDataAdapter DA = null;
            DataSet DSet = null;

            //-----------------【ADO.NET設定】---------------------
            Conn = new SqlConnection(AdoDBString);
            DA = new SqlDataAdapter();

            try
            {
                SqlCommand Cmd_Select = null;
                Cmd_Select = new SqlCommand(sqlstr, Conn);
                DA.SelectCommand = Cmd_Select;

                //タイムアウトの設定
                Cmd_Select.CommandTimeout = 20;

                //データ ソースからデータを取得
                DSet = new DataSet();
                DA.Fill(DSet);

                return DSet;
                //-----------------------------------------------------
            }
            catch (Exception ex)
            {

                //◆◆　エラー情報を格納　◆◆
                errorInfo.Sql = sqlstr;                     //エラー発生SQL内容
                errorInfo.RetMessage = ex.Message;          //エラーメッセージ
                errorInfo.RetStackTrace = ex.StackTrace;    //メソッドが呼び出された順番
                errorInfo.RetString = ex.ToString();        //例外の内容
            }
            finally
            {
                if (!((Conn == null)))
                {
                    Conn.Close();
                }
            }

            return tempGetDataSet;
        }
        public DataSet GetDataSet(string sqlstr)
        {
            ErrorInfo tempVar = null;
            return GetDataSet(sqlstr, ref tempVar);
        }

        /// <summary>
        /// 引数のobjValueがNullの場合、0を返す
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public object fncChangeNull_To_Numeric(object objValue)
        {
            return ((objValue == DBNull.Value) ? 0 : objValue);
        }

        /// <summary>
        /// 引数のobjValueがNullの場合、""を返す
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public string fncChangeNull_To_String(object objValue)
        {
            return ((objValue == DBNull.Value) ? "" : objValue.ToString().Trim());
        }

        /// <summary>
        /// 引数のobjValueがNullの場合、Falseを返す
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public object fncChangeNull_To_Bit(object objValue)
        {
            return ((objValue == DBNull.Value) ? false : objValue);
        }

        /// <summary>
        /// 引数のobjValueがNullの場合、Nothingを返す
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public object fncChangeNull_To_Date(object objValue)
        {
            return ((objValue == DBNull.Value) ? null : objValue);
        }



    }
}
