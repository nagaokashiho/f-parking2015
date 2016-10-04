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
        public bool fncGetData_Reservations_Date(ref List<Reservation> reservations, string startDateTime, string endDateTime, string sisetsuId, string userID, ref string message, ref ErrorInfo errorInfo)
        {
            //SQL設定
            System.Text.StringBuilder strSQL = new System.Text.StringBuilder();

            strSQL.AppendLine("SELECT Reservations.* ");
            strSQL.AppendLine("FROM ");
            strSQL.AppendLine("Reservations ");
            strSQL.AppendLine("WHERE ");
            strSQL.AppendLine("Reservations.DeleteFlag = 'False' ");

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
                //データが存在したか？
                if (dsDtSet.Tables[0].Rows.Count > 0)
                {
                    foreach(DataRow drDtRow in dsDtSet.Tables[0].Rows)
                    {
                        reservations.Add(new Reservation()
                        {
                            Id = (int)fncChangeNull_To_Numeric(drDtRow["Id"]),
                            RoomId = fncChangeNull_To_String(drDtRow["RoomId"]),
                            StartDateTime = (DateTime)fncChangeNull_To_Date(drDtRow["StartDateTime"]),
                            EndDateTime = (DateTime)fncChangeNull_To_Date(drDtRow["EndDateTime"]),
                            Comment = fncChangeNull_To_String(drDtRow["Comment"]),
                            UserId = fncChangeNull_To_String(drDtRow["UserId"]),
                            DeleteFlag = (bool)fncChangeNull_To_Bit(drDtRow["DeleteFlag"]),
                            AddUserId = fncChangeNull_To_String(drDtRow["AddUserId"]),
                            EditUserId = fncChangeNull_To_String(drDtRow["EditUserId"]),
                            AddDateTime = (DateTime)fncChangeNull_To_Date(drDtRow["AddDateTime"]),
                            LastEditDateTime = (DateTime)fncChangeNull_To_Date(drDtRow["LastEditDateTime"])
                        });

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
        /// 【予約テーブル】『Reservations』に指定範囲内に既に予約データが登録されているかチェックする。
        /// </summary>
        /// <param name="startDate">検索開始日付</param>
        /// <param name="startTime">検索開始時間</param>
        /// <param name="endDate">検索終了日付</param>
        /// <param name="endTime">検索終了時間</param>
        /// <param name="sisetsuId">施設コード</param>
        /// <param name="intOutYoyakuID">検索から除外する「予約ID」</param>
        /// <param name="strMsg">エラーメッセージ</param>
        /// <param name="errorInfo">エラー情報用構造体</param>
        /// <returns>True = 該当データあり, False = 該当データなし</returns>
        public bool fncCheck_YoyakuData_TimeRange_UMU(string startDate, string startTime, string endDate, string endTime, string sisetsuId, int intOutYoyakuID, ref string strMsg, ref ErrorInfo errorInfo)
        {
            //SQL設定
            System.Text.StringBuilder strSQL = new System.Text.StringBuilder();

            strSQL.AppendLine("SELECT * ");
            strSQL.AppendLine("FROM ");
            strSQL.AppendLine("Reservations ");
            strSQL.AppendLine("WHERE ");
            strSQL.AppendLine("(");

            //今回指定の「開始時刻」とDB「予約開始日時」が同じデータが既に登録されている
            strSQL.AppendLine("(CONVERT(VARCHAR,StartDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,StartDateTime,108) = '" + startTime + "') ");
            strSQL.AppendLine("OR ");
            //今回指定の「終了時刻」とDB「予約終了日時」が同じデータが既に登録されている
            strSQL.AppendLine("(CONVERT(VARCHAR,EndDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,EndDateTime,108) = '" + endTime + "') ");
            strSQL.AppendLine("OR ");

            //DB「予約開始日時」に、今回指定の「開始時刻」よりが大きく、今回指定の「終了時刻」より小さいデータが既に登録されている
            //※DB「予約開始日時」をまたぐか？
            strSQL.AppendLine("(");
            strSQL.AppendLine("(CONVERT(VARCHAR,StartDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,StartDateTime,108) > '" + startTime + "') ");
            strSQL.AppendLine("AND ");
            strSQL.AppendLine("(CONVERT(VARCHAR,StartDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,StartDateTime,108) < '" + endTime + "') ");
            strSQL.AppendLine(") ");

            strSQL.AppendLine("OR ");

            //DB「予約終了日時」に、今回指定の「開始時刻」よりが大きく、今回指定の「終了時刻」より小さいデータが既に登録されている
            //※DB「予約終了日時」をまたぐか？
            strSQL.AppendLine("(");
            strSQL.AppendLine("(CONVERT(VARCHAR,EndDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,EndDateTime,108) > '" + startTime + "') ");
            strSQL.AppendLine("AND ");
            strSQL.AppendLine("(CONVERT(VARCHAR,EndDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,EndDateTime,108) < '" + endTime + "') ");
            strSQL.AppendLine(") ");

            strSQL.AppendLine("OR ");

            //DB「予約開始日時」が、今回指定の「開始時刻」より大きく、しかも、DB「予約終了日時」が、今回指定の「終了時刻」より小さいデータが既に登録されている
            //※DB「予約終了日時」を完全に含んでしまう？
            strSQL.AppendLine("(");
            strSQL.AppendLine("(CONVERT(VARCHAR,StartDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,StartDateTime,108) > '" + startTime + "') ");
            strSQL.AppendLine("AND ");
            strSQL.AppendLine("(CONVERT(VARCHAR,EndDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,EndDateTime,108) < '" + endTime + "') ");
            strSQL.AppendLine(") ");

            strSQL.AppendLine("OR ");

            //DB「予約開始日時」が、今回指定の「開始時刻」より小さく、しかも、DB「予約終了日時」が、今回指定の「終了時刻」より大きいデータが既に登録されている
            //※DB「予約終了日時」に完全に含まれてしまう？
            strSQL.AppendLine("(");
            strSQL.AppendLine("(CONVERT(VARCHAR,StartDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,StartDateTime,108) < '" + startTime + "') ");
            strSQL.AppendLine("AND ");
            strSQL.AppendLine("(CONVERT(VARCHAR,EndDateTime,111) = '" + startDate + "' AND CONVERT(VARCHAR,EndDateTime,108) > '" + endTime + "') ");
            strSQL.AppendLine(") ");

            strSQL.AppendLine(") ");

            //「会議室Id」が指定されているか？
            if (!string.IsNullOrEmpty(sisetsuId))
            {
                strSQL.AppendLine("AND ");
                strSQL.AppendLine("RoomId = '" + sisetsuId + "' ");
            }

            //検索から除外する「予約ID」が指定されているか？
            if (intOutYoyakuID > 0)
            {
                strSQL.AppendLine("AND ");
                strSQL.AppendLine("Id <> " + intOutYoyakuID.ToString() + " ");
            }

            strSQL.AppendLine("AND ");
            strSQL.AppendLine("DeleteFlag = 'False' ");

            //「施設コード」が指定されているか？
            if (!string.IsNullOrEmpty(sisetsuId))
            {
                strSQL.AppendLine("ORDER BY StartDateTime ");
            }
            else
            {
                strSQL.AppendLine("ORDER BY RoomId, StartDateTime ");
            }

            //========================== SQL実行 ===============================
            DataSet dsDtSet = GetDataSet(strSQL.ToString(), ref errorInfo);
            //==================================================================

            try
            {
                if (dsDtSet.Tables[0].Rows.Count > 0)
                {
                    //戻り値設定
                    return true;
                }
                else
                {
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
