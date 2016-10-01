using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Administration.Models;
using System.Data.Entity;
using System.Drawing;
using System.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Administration.Rooms
{
    public partial class index : System.Web.UI.Page
    {
        private CommonDB commonDB = new CommonDB();

        //タブ番号   
        protected enum enumTab : int
        {
            List = 1, //一覧
            Shisetsu = 2 //施設
        }

        protected const int CNS_SHISETSU_CELL_DURATION = 30;        //列の表示間隔（分）
        protected const int CNS_SHISETSU_CELL_WIDTH = 30;           //列の幅（px）
        protected const int CNS_SHISETSU_TITLE_CELL_WIDTH = 160;    //タイトル列の幅（px）
        protected const int CNS_SHISETSU_DISP_HOURS = 6;            //表示時間数
        protected const string CNS_SHISETSU_BOOK_NONE = "#FFFFFF";  //white
        protected const string CNS_SHISETSU_BOOK_ME = "#0000FF";    //blue
        protected const string CNS_SHISETSU_BOOK_OTHER = "#808080"; //gray
        protected const string CNS_SHISETSU_REQUEST = "#FF0000";    //red
        protected const int CNS_SHISETSU_DATE_CNT = 7;              //施設タブでの表示行数

        //施設予約管理時間(Start)
        protected const int CNS_Facility_TimeStart = 7;         //AM7:00（予約可能時刻は、8:00-）
                                                                //施設予約管理時間(End)
        protected const int CNS_Facility_TimeEnd = 22;          //PM10:00（予約可能時刻は平日-21:30、土日祝-17:30）
                                                                //施設予約管理単位
        protected const int CNS_Facility_DivisionTime = 30;     //30分単位

        protected void Page_Load(object sender, EventArgs e)
        {
            // ポストバック時はリターン（何も処理しない）
            if (IsPostBack == true)
            {
                return;
            }

            // キャッシュを無効化する
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            // 初期設定をする
            initDisp();

        }

        /// <summary>
        /// 初期設定をする
        /// </summary>
        protected void initDisp()
        {
            //マニュアル設定用「予約日付」に本日日付を設定する
            DateTime target = DateTime.Now;
            txtBookDateStart.Text = target.ToString("yyyy/MM/dd");

            //会議室タブ[DropDownList]【会議室】設定
            if (fncSet_DropDownList_Rooms(selSisetsu2))
            {
                selSisetsu2.SelectedIndex = 0; //初期値表示
            }

            //マニュアルモード[DropDownList]【会議室】設定
            if (fncSet_DropDownList_Rooms(selSisetsu))
            {
                selSisetsu.SelectedIndex = 0; //初期値表示
            }

            //予約データを表示する
            subDispBookData_List(target, true);
            subDispBookData_Room(target, true);

            //タブの初期表示を設定する
            hid_selected_tab.Value = "1";
            //subSetDispTab(enumTab.List);
        }

        /// <summary>
        /// 予約データ表示（一覧）
        /// </summary>
        /// <param name="target">対象日付</param>
        /// <param name="getMode">True = データ取得あり, False = データ取得なし（再表示）</param>
        private void subDispBookData_List(DateTime target, bool getMode)
        {
            //表示日付を設定する
            txtDispDate1.Text = target.ToString("yyyy/MM/dd");

            //テーブルを作成する
            subCreatetable(target, enumTab.List);

            //テーブルのデータをクリアする
            subClearTable(enumTab.List);

            if (getMode)
            {
                //予約データを取得する
                subGetBookingData_List(target);
            }

            //テーブルにデータを設定する
            this.subSetTable_List(target);

            //セッションにデータを格納する
            Session["targetDateList"] = target.Date;

        }

        /// <summary>
        /// 予約データ表示（施設）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="getMode"></param>
        private void subDispBookData_Room(DateTime target, bool getMode)
        {
 
            //表示日付を設定する
            txtDispDate2.Text = target.ToString("yyyy/MM/dd");

            //テーブルを作成する
            subCreatetable(target, enumTab.Shisetsu);

            //テーブルのデータをクリアする
            subClearTable(enumTab.Shisetsu);

            if (getMode)
            {
                //予約データを取得する
                subGetBookingData_Room(target);
            }

            //テーブルにデータを設定する
            subSetTable_Room(target);

            //セッションにデータを格納する
            Session["targetDateShisetsu"] = target.Date;
            Session["RoomId"] = selSisetsu2.SelectedValue;

        }

        /// <summary>
        /// テーブル作成処理
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tab"></param>
        protected void subCreatetable(DateTime target, enumTab tab)
        {
            DateTime stratDate = new DateTime(target.Year, target.Month, target.Day, CNS_Facility_TimeStart, 0, 0);
            DateTime endDate = new DateTime(target.Year, target.Month, target.Day, CNS_Facility_TimeEnd, 0, 0);

            TextBox txtDispDate = null;
            Table tblTitle = null;
            Table tblBooking = null;
            if (tab == enumTab.List)
            {
                //一覧タブ
                txtDispDate = txtDispDate1;
                tblTitle = tblTitle1;
                tblBooking = tblBooking1;
            }
            else
            {
                //施設タブ
                txtDispDate = txtDispDate2;
                tblTitle = tblTitle2;
                tblBooking = tblBooking2;
            }
            //表示日付を設定する
            txtDispDate.Text = target.ToString("yyyy/MM/dd");

            //カラのテーブルに行と列を追加する
            TableRow tr = null;
            TableCell tc = null;
            int cellCnt = (CNS_Facility_TimeEnd - CNS_Facility_TimeStart) * (60 / CNS_SHISETSU_CELL_DURATION);

            //タイトル列用のテーブルを作成する
            //テーブルの全行を削除する
            tblTitle.Rows.Clear();
            //テーブルの幅を指定する
            tblTitle.Width = CNS_SHISETSU_TITLE_CELL_WIDTH;
            // ヘッダ行（１行目［時］）を出力する
            // 左上（コーナー）
            tr = new TableRow();
            tc = new TableCell();
            tc.CssClass = "corner";
            tc.Text = "<div class=\"div_corner\" unselectable=\"on\"></div>";
            tr.Cells.Add(tc);
            tblTitle.Rows.Add(tr);

            if (tab == enumTab.List)
            {
                // 一覧タブ
                var rooms = commonDB.SelectRooms();
                foreach (var r in rooms)
                {
                    tr = new TableRow();
                    tc = new TableCell();
                    // タイトル列
                    tc = new TableCell();
                    tc.CssClass = "title_col";
                    tc.Text = "<div class=\"div_title_col\" unselectable=\"on\">" + r.RyakuName + "</div>";
                    tr.Cells.Add(tc);
                    tblTitle.Rows.Add(tr);
                }
            }
            else
            {
                // 施設タブ
                for (var i = 0; i < CNS_SHISETSU_DATE_CNT; i++)
                {
                    tr = new TableRow();
                    tc = new TableCell();
                    // タイトル列
                    tc = new TableCell();
                    tc.CssClass = "title_col";
                    tc.Text = "<div class=\"div_title_col\" unselectable=\"on\">" + target.AddDays(i).ToString("yyyy/MM/dd（ddd）") + "</div>";
                    tr.Cells.Add(tc);
                    tblTitle.Rows.Add(tr);
                }
            }

            //テーブルの全行を削除する
            tblBooking.Rows.Clear();
            //テーブルの幅を指定する
            tblBooking.Width = CNS_SHISETSU_CELL_WIDTH * cellCnt;
            // ［時］行
            tr = new TableRow();
            for (var hours = CNS_Facility_TimeStart; hours < CNS_Facility_TimeEnd; hours++)
            {
                tc = new TableCell();
                tc.ColumnSpan = (60 / CNS_SHISETSU_CELL_DURATION);
                tc.Text = "<div class=\"div_row1\" unselectable=\"on\">" + hours.ToString() + " 時" + "</div>";
                if (hours == CNS_Facility_TimeEnd - 1)
                {
                    tc.CssClass = "title_row1 col_end";
                }
                else
                {
                    tc.CssClass = "title_row1 col_other";
                }
                tr.Cells.Add(tc);
            }
            tblBooking.Rows.Add(tr);

            // ヘッダ行（２行目［分］）を出力する
            tr = new TableRow();
            // ［分］行
            for (var hours = 0; hours < cellCnt; hours++)
            {
                tc = new TableCell();
                tc.Text = "<div class=\"div_row2\" unselectable=\"on\">" + string.Format("{0:00}", (CNS_SHISETSU_CELL_DURATION * (hours % 4))) + "</div>";
                if (hours == cellCnt - 1)
                {
                    tc.CssClass = "title_row2 col_end";
                }
                else
                {
                    if ((hours % 4) == 3)
                    {
                        tc.CssClass = "title_row2 col_other";
                    }
                    else
                    {
                        tc.CssClass = "title_row2 col_other2";
                    }
                }
                tr.Cells.Add(tc);
            }
            tblBooking.Rows.Add(tr);

            // データ行を出力する
            int maxCnt = 0;
            if (tab == enumTab.List)
            {
                // 一覧タブ
                Room[] rooms = (Room[])Session["Rooms"];
                maxCnt = rooms.Count();
            }
            else
            {
                // 施設タブ
                maxCnt = CNS_SHISETSU_DATE_CNT;
            }
            // データ（時間）行
            for (var i = 0; i < maxCnt; i++)
            {
                tr = new TableRow();
                for (var hours = 0; hours < cellCnt; hours++)
                {
                    tc = new TableCell();
                    if (hours == cellCnt - 1)
                    {
                        tc.CssClass = "data_col_other col_end";
                    }
                    else
                    {
                        if ((hours % 4) == 3)
                        {
                            tc.CssClass = "data_col_other col_other";
                        }
                        else
                        {
                            tc.CssClass = "data_col_other col_other2";
                        }
                    }
                    tc.BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_NONE);
                    tc.Text = "<div unselectable=\"on\" class=\"div_data_col_other\" ><!-- --></div>";
                    tc.ToolTip = string.Empty;
                    tc.Style["cursor"] = "pointer";
                    tr.Cells.Add(tc);
                }
                tblBooking.Rows.Add(tr);
            }

        }

        /// <summary>
        /// テーブル作成処理
        /// </summary>
        /// <param name="tab"></param>
        protected void subClearTable(enumTab tab)
        {
            // データ行をクリアする
            Table tblBooking = null;
            if (tab == enumTab.List)
            {
                //一覧タブ
                tblBooking = tblBooking1;
            }
            else
            {
                //施設タブ
                tblBooking = tblBooking2;
            }

            // データ（時間）行
            for (var row = 2; row < tblBooking.Rows.Count; row++)
            {
                for (var col = 0; col < tblBooking.Rows[row].Cells.Count; col++)
                {
                    var tempVar = tblBooking.Rows[row].Cells[col];
                    if (col == tblBooking.Rows[row].Cells.Count - 1)
                    {
                        tempVar.CssClass = "data_col_other col_end";
                    }
                    else
                    {
                        if ((col % 4) == 3)
                        {
                            tempVar.CssClass = "data_col_other col_other";
                        }
                        else
                        {
                            tempVar.CssClass = "data_col_other col_other2";
                        }
                    }
                    tempVar.BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_NONE);
                    tempVar.Text = "<div unselectable=\"on\" class=\"div_data_col_other\" ><!-- --></div>";
                    tempVar.ToolTip = "";
                }
            }

        }

        /// <summary>
        /// 予約テーブルのデータを設定する
        /// </summary>
        /// <param name="target"></param>
        protected void subGetBookingData_List(DateTime target)
        {
            // 予約テーブルのデータをセッション変数用のワークに設定する
            DateTime startDate = new DateTime(target.Year, target.Month, target.Day, CNS_Facility_TimeStart, 0, 0);
            DateTime endDate = new DateTime(target.Year, target.Month, target.Day, CNS_Facility_TimeEnd, 0, 0);
            int cellCnt = (CNS_Facility_TimeEnd - CNS_Facility_TimeStart) * (60 / CNS_SHISETSU_CELL_DURATION);

            Room[] rooms = (Room[])Session["Rooms"];
            ReservationList[] reservationLists = new ReservationList[rooms.Count()];
            for (var i = 0; i < reservationLists.Count(); i++)
            {
                reservationLists[i].RoomId = rooms[i].Id;
                reservationLists[i].RoomName = rooms[i].Name;
                reservationLists[i].RoomRyakuName = rooms[i].RyakuName;
                Array.Resize(ref reservationLists[i].Reservations, cellCnt - 1);
            }

            //会議室マスタのデータのみだが、とりあえず保存する
            Session["ReservationLists"] = reservationLists;

            // 予約テーブルからデータを取得する（表示日付予約分）
            Reservation[] reservations = new Reservation[1];
            string message = "";
            ErrorInfo errorInfo = new ErrorInfo();
            if (commonDB.fncGetData_Reservations_Date(ref reservations, startDate.ToString(), endDate.ToString(), "", "", ref message, ref errorInfo))
            {
                Session["ReservationsForList"] = reservations;
            }
            else
            {
                Session["ReservationsForList"] = null;
            }
        }

        /// <summary>
        /// 予約テーブルのデータを設定する
        /// </summary>
        /// <param name="target"></param>
        protected void subGetBookingData_Room(DateTime target)
        {
            // 施設予約マスタのデータをセッション変数用のワークに設定する
            DateTime startDate = new DateTime(target.Year, target.Month, target.Day, CNS_Facility_TimeStart, 0, 0);
            DateTime endDate = new DateTime(target.AddDays(CNS_SHISETSU_DATE_CNT - 1).Year, target.AddDays(CNS_SHISETSU_DATE_CNT - 1).Month, target.AddDays(CNS_SHISETSU_DATE_CNT - 1).Day, CNS_Facility_TimeEnd, 0, 0);
            int cellCnt = (CNS_Facility_TimeEnd - CNS_Facility_TimeStart) * (60 / CNS_SHISETSU_CELL_DURATION);

            ReservationRoom[] reservationRooms = new ReservationRoom[CNS_SHISETSU_DATE_CNT];
            for (var i = 0; i < reservationRooms.Count(); i++)
            {
                reservationRooms[i].ReservationDate = target.AddDays(i).Date;
                Array.Resize(ref reservationRooms[i].Reservations, cellCnt - 1);
            }

            //日付のデータのみだが、とりあえず保存する
            Session["ReservationRooms"] = reservationRooms;

            // 予約テーブルからデータを取得する（表示日付予約分）
            Reservation[] reservations = new Reservation[1];
            string message = "";
            ErrorInfo errorInfo = new ErrorInfo();
            if (commonDB.fncGetData_Reservations_Date(ref reservations, startDate.ToString(), endDate.ToString(), selSisetsu2.SelectedValue, "", ref message, ref errorInfo))
            {
                Session["ReservationsForRoom"] = reservations;
            }
            else
            {
                Session["ReservationsForRoom"] = null;
            }
        }

        /// <summary>
        /// ドロップダウンリストにデータを設定する
        /// </summary>
        /// <param name="ddl"></param>
        protected bool fncSet_DropDownList_Rooms(DropDownList ddl)
        {
            // 会議室マスタを取得
            try
            {
                IQueryable<Room> rooms = commonDB.SelectRooms();
                Room[] rm = new Room[rooms.Count()];
                int i = 0;
                foreach (var r in rooms)
                {
                    // ドロップダウンリストにデータを設定
                    ddl.Items.Add(new ListItem(r.RyakuName, r.Id));
                    rm[i].Id = r.Id;
                    rm[i].Name = r.Name;
                    rm[i].RyakuName = r.RyakuName;
                    i++;
                }
                Session["Rooms"] = rm;
                return true;
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
            Session.Remove("Rooms");
            return false;
        }

        /// <summary>
        /// テーブルにデータを設定する
        /// </summary>
        /// <param name="target"></param>
        protected void subSetTable_List(DateTime target)
        {
            string LoginUserId = Context.User.Identity.GetUserId();
            Reservation[] reservations = (Reservation[])Session["ReservationsForList"];
            ReservationList[] reservationLists = (ReservationList[])Session["ReservationLists"];
            DateTime startDate = new DateTime(target.Year, target.Month, target.Day, CNS_Facility_TimeStart, 0, 0);
            DateTime endDate = new DateTime(target.Year, target.Month, target.Day, CNS_Facility_TimeEnd, 0, 0);
            int cellCnt = (CNS_Facility_TimeEnd - CNS_Facility_TimeStart) * (60 / CNS_SHISETSU_CELL_DURATION);

            if (reservations == null || reservationLists == null)
            {
                return;
            }

            for (var i = 0; i < reservations.Count(); i++)
            {
                for (var j = 0; j < reservationLists.Count(); j++)
                {
                    if (reservations[i].RoomId == reservationLists[j].RoomId)
                    {
                        for (var k = 0; k < cellCnt; k++)
                        {
                            DateTime startTime = startDate.AddMinutes(k * CNS_SHISETSU_CELL_DURATION);
                            DateTime endTime = startDate.AddMinutes((k + 1) * CNS_SHISETSU_CELL_DURATION);
                            if (reservations[i].StartDateTime <= startTime && reservations[i].EndDateTime >= endTime)
                            {
                                reservationLists[j].Reservations[k] = reservations[i];
                                // テーブルのセルの背景色を変更する
                                if (LoginUserId == reservations[i].UserId)
                                {
                                    tblBooking1.Rows[j + 2].Cells[k].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_ME);
                                }
                                else
                                {
                                    tblBooking1.Rows[j + 2].Cells[k].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_OTHER);
                                }
                                // ツールチップを設定する
                                string tips = string.Empty;
                                tips = reservations[i].Comment + Environment.NewLine + "(" + reservations[i].StartDateTime.ToString("HH:mm") + " - " + reservations[i].EndDateTime.ToString("HH:mm") + ")";
                                tblBooking1.Rows[j + 2].Cells[k].ToolTip = tips;
                            }
                        }
                    }
                }
            }
            //予約情報のデータが更新されたので、再度保存する
            Session["ReservationLists"] = reservationLists;
        }

        /// <summary>
        /// テーブルにデータを設定する
        /// </summary>
        /// <param name="target"></param>
        protected void subSetTable_Room(DateTime target)
        {
            string LoginUserId = Context.User.Identity.GetUserId();
            Reservation[] reservations = (Reservation[])Session["ReservationsForRoom"];
            ReservationRoom[] reservationRooms = (ReservationRoom[])Session["ReservationRooms"];
            int cellCnt = (CNS_Facility_TimeEnd - CNS_Facility_TimeStart) * (60 / CNS_SHISETSU_CELL_DURATION);

            if (reservations == null || reservationRooms == null)
            {
                return;
            }

            for (var i = 0; i < reservations.Count(); i++)
            {
                for (var j = 0; j < reservationRooms.Count(); j++)
                {
                    DateTime startDate = reservationRooms[j].ReservationDate.Date.AddHours(CNS_Facility_TimeStart);
                    DateTime endDate = reservationRooms[j].ReservationDate.Date.AddHours(CNS_Facility_TimeEnd);
                    if (reservations[i].StartDateTime < endDate && reservations[i].EndDateTime >= startDate)
                    {
                        for (var k = 0; k < cellCnt; k++)
                        {
                            DateTime startTime = startDate.AddMinutes(k * CNS_SHISETSU_CELL_DURATION);
                            DateTime endTime = startDate.AddMinutes((k + 1) * CNS_SHISETSU_CELL_DURATION);
                            if (reservations[i].StartDateTime <= startTime && reservations[i].EndDateTime >= endTime)
                            {
                                reservationRooms[j].Reservations[k] = reservations[i];
                                // テーブルのセルの背景色を変更する
                                if (LoginUserId == reservations[i].UserId)
                                {
                                    tblBooking2.Rows[j + 2].Cells[k].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_ME);
                                }
                                else
                                {
                                    tblBooking2.Rows[j + 2].Cells[k].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_OTHER);
                                }
                                // ツールチップを設定する
                                string tips = string.Empty;
                                tips = reservations[i].Comment + Environment.NewLine + "(" + reservations[i].StartDateTime.ToString("HH:mm") + " - " + reservations[i].EndDateTime.ToString("HH:mm") + ")";
                                tblBooking2.Rows[j + 2].Cells[k].ToolTip = tips;
                            }
                        }
                    }
                }
            }
            //予約情報のデータが更新されたので、再度保存する
            Session["ReservationRooms"] = reservationRooms;
        }

        /// <summary>
        /// ログイン中のユーザー名を取得する
        /// </summary>
        /// <returns></returns>
        protected string GetDispUserName(String userId)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = manager.FindById(userId);

            return user.DispUserName;
        }



    }
}