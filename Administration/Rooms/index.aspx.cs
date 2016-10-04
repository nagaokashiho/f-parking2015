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
        private f_parkingContext db = new f_parkingContext();

        // タブ番号   
        protected enum enumTab : int
        {
            List = 1,   // 一覧
            Room = 2    // 会議室
        }

        protected const int CNS_SHISETSU_CELL_DURATION = 30;        //列の表示間隔（分）
        protected const int CNS_SHISETSU_CELL_WIDTH = 30;           //列の幅（px）
        protected const int CNS_SHISETSU_TITLE_CELL_WIDTH = 160;    //タイトル列の幅（px）
        protected const int CNS_SHISETSU_DISP_HOURS = 6;            //表示時間数
        protected const string CNS_SHISETSU_BOOK_NONE = "#FFFFFF";  //white
        protected const string CNS_SHISETSU_BOOK_ME = "#0000FF";    //blue
        protected const string CNS_SHISETSU_BOOK_OTHER = "#808080"; //gray
        protected const string CNS_SHISETSU_REQUEST = "#FF0000";    //red
        protected const int CNS_SHISETSU_DATE_CNT = 7;              //会議室タブでの表示行数

        // 施設予約管理時間(Start)
        protected const int CNS_Facility_TimeStart = 7;             //AM7:00（予約可能時刻は、8:00-）
                                                                    //予約管理時間(End)
        protected const int CNS_Facility_TimeEnd = 22;              //PM10:00（予約可能時刻は平日-21:30、土日祝-17:30）
                                                                    //予約管理単位
        protected const int CNS_Facility_DivisionTime = 30;         //30分単位

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
            // マニュアル設定用「予約日付」に本日日付を設定する
            DateTime target = DateTime.Now;
            txtBookDate.Text = target.ToString("yyyy/MM/dd");

            // 会議室タブ[DropDownList]【会議室】設定
            if (fncSet_DropDownList_Rooms(selRoom2))
            {
                selRoom2.SelectedIndex = 0; //初期値表示
            }

            // マニュアルモード[DropDownList]【会議室】設定
            if (fncSet_DropDownList_Rooms(selRoom))
            {
                selRoom.SelectedIndex = 0; //初期値表示
            }

            // 編集ダイアログ[DropDownList]【会議室】設定
            if (fncSet_DropDownList_Rooms(selRoomEdit))
            {
                selRoom.SelectedIndex = 0; //初期値表示
            }

            // 予約データを表示する
            subDispBookData_List(target, true);
            subDispBookData_Room(target, true);

            // タブの初期表示を設定する
            hid_selected_tab.Value = "1";
            subSetDispTab(enumTab.List);
        }

        /// <summary>
        /// 予約データ表示（一覧）
        /// </summary>
        /// <param name="target">対象日付</param>
        /// <param name="getMode">True = データ取得あり, False = データ取得なし（再表示）</param>
        private void subDispBookData_List(DateTime target, bool getMode)
        {
            // 表示日付を設定する
            txtDispDate1.Text = target.ToString("yyyy/MM/dd");

            // テーブルを作成する
            subCreatetable(target, enumTab.List);

            // テーブルのデータをクリアする
            subClearTable(enumTab.List);

            if (getMode)
            {
                // 予約データを取得する
                subGetBookingData_List(target);
            }

            // テーブルにデータを設定する
            subSetTable_List(target);

            // セッションにデータを格納する
            Session["targetDateList"] = target.Date;

        }

        /// <summary>
        /// 予約データ表示（施設）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="getMode"></param>
        private void subDispBookData_Room(DateTime target, bool getMode)
        {

            // 表示日付を設定する
            txtDispDate2.Text = target.ToString("yyyy/MM/dd");

            // テーブルを作成する
            subCreatetable(target, enumTab.Room);

            // テーブルのデータをクリアする
            subClearTable(enumTab.Room);

            if (getMode)
            {
                // 予約データを取得する
                subGetBookingData_Room(target);
            }

            // テーブルにデータを設定する
            subSetTable_Room(target);

            // セッションにデータを格納する
            Session["targetDateRoom"] = target.Date;
            Session["RoomId"] = selRoom2.SelectedValue;

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
                // 一覧タブ
                txtDispDate = txtDispDate1;
                tblTitle = tblTitle1;
                tblBooking = tblBooking1;
            }
            else
            {
                // 会議室タブ
                txtDispDate = txtDispDate2;
                tblTitle = tblTitle2;
                tblBooking = tblBooking2;
            }
            // 表示日付を設定する
            txtDispDate.Text = target.ToString("yyyy/MM/dd");

            // カラのテーブルに行と列を追加する
            TableRow tr = null;
            TableCell tc = null;
            int cellCnt = (CNS_Facility_TimeEnd - CNS_Facility_TimeStart) * (60 / CNS_SHISETSU_CELL_DURATION);

            // タイトル列用のテーブルを作成する
            // テーブルの全行を削除する
            tblTitle.Rows.Clear();
            // テーブルの幅を指定する
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
                // 会議室タブ
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

            //　テーブルの全行を削除する
            tblBooking.Rows.Clear();
            //　テーブルの幅を指定する
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
                tc.Text = "<div class=\"div_row2\" unselectable=\"on\">" + string.Format("{0:00}", (CNS_SHISETSU_CELL_DURATION * (hours % (60 / CNS_SHISETSU_CELL_DURATION)))) + "</div>";
                if (hours == cellCnt - 1)
                {
                    tc.CssClass = "title_row2 col_end";
                }
                else
                {
                    if ((hours % (60 / CNS_SHISETSU_CELL_DURATION)) == (60 / CNS_SHISETSU_CELL_DURATION) - 1)
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
                List<Room> rooms = (List<Room>)Session["Rooms"];
                maxCnt = rooms.Count();
            }
            else
            {
                // 会議室タブ
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
                        if ((hours % (60 / CNS_SHISETSU_CELL_DURATION)) == (60 / CNS_SHISETSU_CELL_DURATION) - 1)
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
                // 一覧タブ
                tblBooking = tblBooking1;
            }
            else
            {
                // 施設タブ
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
                        if ((col % (60 / CNS_SHISETSU_CELL_DURATION)) == (60 / CNS_SHISETSU_CELL_DURATION) - 1)
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

            List<Room> rooms = (List<Room>)Session["Rooms"];
            List<ReservationList> reservationLists = new List<ReservationList>();
            foreach (Room r in rooms)
            {
                reservationLists.Add(new ReservationList()
                {
                    RoomId = r.Id,
                    RoomName = r.Name,
                    RoomRyakuName = r.RyakuName,
                    Reservations = new Reservation[cellCnt - 1]
                });
            }

            // 会議室マスタのデータのみだが、とりあえず保存する
            Session["ReservationLists"] = reservationLists;

            // 予約テーブルからデータを取得する（表示日付予約分）
            List<Reservation> reservations = new List<Reservation>();
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
            // 予約マスタのデータをセッション変数用のワークに設定する
            DateTime startDate = new DateTime(target.Year, target.Month, target.Day, CNS_Facility_TimeStart, 0, 0);
            DateTime endDate = new DateTime(target.AddDays(CNS_SHISETSU_DATE_CNT - 1).Year, target.AddDays(CNS_SHISETSU_DATE_CNT - 1).Month, target.AddDays(CNS_SHISETSU_DATE_CNT - 1).Day, CNS_Facility_TimeEnd, 0, 0);
            int cellCnt = (CNS_Facility_TimeEnd - CNS_Facility_TimeStart) * (60 / CNS_SHISETSU_CELL_DURATION);

            // 対象の会議室情報を取得する
            List<Room> rooms = (List<Room>)Session["Rooms"];
            Room rInfo = new Room();
            foreach (Room r in rooms)
            {
                if(selRoom2.SelectedValue == r.Id)
                {
                    rInfo = r;
                }
            }
            List<ReservationRoom> reservationRooms = new List<ReservationRoom>();
            for (var i = 0; i < CNS_SHISETSU_DATE_CNT; i++)
            {
                reservationRooms.Add(new ReservationRoom()
                {
                    ReservationDate = target.AddDays(i).Date,
                    RoomId = rInfo.Id,
                    RoomName = rInfo.Name,
                    RoomRyakuName = rInfo.RyakuName,
                    Reservations = new Reservation[cellCnt - 1]
                });
            }

            // 日付のデータのみだが、とりあえず保存する
            Session["ReservationRooms"] = reservationRooms;

            // 予約テーブルからデータを取得する（表示日付予約分）
            List<Reservation> reservations = new List<Reservation>();
            string message = "";
            ErrorInfo errorInfo = new ErrorInfo();
            if (commonDB.fncGetData_Reservations_Date(ref reservations, startDate.ToString(), endDate.ToString(), selRoom2.SelectedValue, "", ref message, ref errorInfo))
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
                foreach (var r in rooms)
                {
                    // ドロップダウンリストにデータを設定
                    ddl.Items.Add(new ListItem(r.RyakuName, r.Id));
                }
                Session["Rooms"] = (List<Room>)rooms.ToList();
                return true;
            }
            catch (Exception e)
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
            List<Reservation> reservations = (List<Reservation>)Session["ReservationsForList"];
            List<ReservationList> reservationLists = (List<ReservationList>)Session["ReservationLists"];
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
                                    tblBooking1.Rows[j + 2].Cells[k].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_ME);
                                    //tblBooking1.Rows[j + 2].Cells[k].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_OTHER);
                                }
                                // popoverを設定する
                                tblBooking1.Rows[j + 2].Cells[k].Attributes["data-toggle"] = "popover";
                                string dt = "【" + reservationLists[j].RoomRyakuName + "】"
                                            + reservations[i].StartDateTime.ToString("yyyy/MM/dd(ddd) HH:mm")
                                            + " - " + reservations[i].EndDateTime.ToString("HH:mm");
                                tblBooking1.Rows[j + 2].Cells[k].Attributes["title"] = dt;
                                //tblBooking1.Rows[j + 2].Cells[k].Attributes["data-content"] = reservations[i].Comment.Replace(Environment.NewLine, "<br />");
                                tblBooking1.Rows[j + 2].Cells[k].Attributes["data-content"] = reservations[i].Comment.Replace("\n", "<br />");

                                // オリジナルの隠しデータを保存する
                                tblBooking1.Rows[j + 2].Cells[k].Attributes["hide-roomId"] = reservationLists[j].RoomId;
                                tblBooking1.Rows[j + 2].Cells[k].Attributes["hide-room"] = reservationLists[j].RoomRyakuName;
                                tblBooking1.Rows[j + 2].Cells[k].Attributes["hide-start"] = reservations[i].StartDateTime.ToString();
                                tblBooking1.Rows[j + 2].Cells[k].Attributes["hide-end"] = reservations[i].EndDateTime.ToString();
                                tblBooking1.Rows[j + 2].Cells[k].Attributes["hide-comment"] = reservations[i].Comment;
                            }
                        }
                    }
                }
            }
            // 予約情報のデータが更新されたので、再度保存する
            Session["ReservationLists"] = reservationLists;
        }

        /// <summary>
        /// テーブルにデータを設定する
        /// </summary>
        /// <param name="target"></param>
        protected void subSetTable_Room(DateTime target)
        {
            string LoginUserId = Context.User.Identity.GetUserId();
            List<Reservation> reservations = (List<Reservation>)Session["ReservationsForRoom"];
            List<ReservationRoom> reservationRooms = (List<ReservationRoom>)Session["ReservationRooms"];
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
                                    tblBooking2.Rows[j + 2].Cells[k].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_ME);
                                    //tblBooking2.Rows[j + 2].Cells[k].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_BOOK_OTHER);
                                }
                                // popoverを設定する
                                tblBooking2.Rows[j + 2].Cells[k].Attributes["data-toggle"] = "popover";
                                string dt = "【" + reservationRooms[j].RoomRyakuName + "】"
                                            + reservations[i].StartDateTime.ToString("yyyy/MM/dd(ddd) HH:mm")
                                            + " - " + reservations[i].EndDateTime.ToString("HH:mm");
                                tblBooking2.Rows[j + 2].Cells[k].Attributes["title"] = dt;
                                tblBooking2.Rows[j + 2].Cells[k].Attributes["data-content"] = reservations[i].Comment;

                                // オリジナルの隠しデータを保存する
                                tblBooking2.Rows[j + 2].Cells[k].Attributes["hide-roomId"] = reservationRooms[j].RoomId;
                                tblBooking2.Rows[j + 2].Cells[k].Attributes["hide-room"] = reservationRooms[j].RoomRyakuName;
                                tblBooking2.Rows[j + 2].Cells[k].Attributes["hide-start"] = reservations[i].StartDateTime.ToString();
                                tblBooking2.Rows[j + 2].Cells[k].Attributes["hide-end"] = reservations[i].EndDateTime.ToString();
                                tblBooking2.Rows[j + 2].Cells[k].Attributes["hide-comment"] = reservations[i].Comment;
                            }
                        }
                    }
                }
            }
            // 予約情報のデータが更新されたので、再度保存する
            Session["ReservationRooms"] = reservationRooms;
        }

        /// <summary>
        /// タブの表示を設定する
        /// </summary>
        /// <param name="tab"></param>
        private void subSetDispTab(enumTab tab)
        {
            //if (tab == enumTab.List)
            //{
            //    li1.Attributes["class"] = "selected";
            //    tabList.Style["display"] = "block";
            //    li2.Attributes["class"] = "";
            //    tabRoom.Style["display"] = "none";
            //}
            //else
            //{
            //    li1.Attributes["class"] = "";
            //    tabList.Style["display"] = "none";
            //    li2.Attributes["class"] = "selected";
            //    tabRoom.Style["display"] = "block";
            //}
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

        /// <summary>
        /// ボタンの表示を初期化する
        /// </summary>
        /// <param name="tab"></param>
        private void subClearButton(enumTab tab)
        {
            Button btnStart = null;
            Button btnEnd = null;
            Button btnCancel = null;
            HiddenField hid_addMode = null;
            HiddenField hid_clickRow = null;
            HiddenField hid_clickColStart = null;
            HiddenField hid_clickColEnd = null;
            HiddenField hid_yoto = null;

            if (tab == enumTab.List)
            {
                btnStart = btnStart1;
                btnEnd = btnEnd1;
                btnCancel = btnCancel1;
                hid_addMode = hid_addMode1;
                hid_clickRow = hid_clickRow1;
                hid_clickColStart = hid_clickColStart1;
                hid_clickColEnd = hid_clickColEnd1;
                hid_yoto = hid_yoto1;
            }
            else
            {
                btnStart = btnStart2;
                btnEnd = btnEnd2;
                btnCancel = btnCancel2;
                hid_addMode = hid_addMode2;
                hid_clickRow = hid_clickRow2;
                hid_clickColStart = hid_clickColStart2;
                hid_clickColEnd = hid_clickColEnd2;
                hid_yoto = hid_yoto2;
            }

            btnStart.Style["visibility"] = "visible";
            btnEnd.Style["visibility"] = "hidden";
            btnCancel.Style["visibility"] = "hidden";
            hid_addMode.Value = "";
            hid_clickRow.Value = "";
            hid_clickColStart.Value = "";
            hid_clickColEnd.Value = "";
            hid_yoto.Value = "";
        }

        /// <summary>
        /// 再表示ボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReload_Click(object sender, System.EventArgs e)
        {
            DateTime target = DateTime.MinValue;
            TextBox txtDispDate = null;
            DateTime targetList = DateTime.MinValue;
            DateTime targetRoom = DateTime.MinValue;

            if (((Button)sender).ClientID == "btnReload1")
            {
                txtDispDate = txtDispDate1;
            }
            else
            {
                txtDispDate = txtDispDate2;

                // 会議室が未選択？
                if (selRoom2.SelectedIndex < 0)
                {
                    CommonMain.subMessageBox_upd(this, CommonMain.CNS_ERR_MSGERR + "「会議室」が選択されていません！");
                    // フォーカスを設定
                    SetFocus(selRoom2);
                    return;
                }
            }

            // 入力された日付をチェックする
            if (DateTime.TryParse(txtDispDate.Text, out target))
            {
                if (((Button)sender).ClientID == "btnReload1")
                {
                    targetList = target;
                    targetRoom = (DateTime)Session["targetDateRoom"];
                    selRoom2.SelectedValue = (string)Session["RoomId"];
                }
                else
                {
                    targetList = (DateTime)Session["targetDateList"];
                    targetRoom = target;
                }

                // 非表示タブを表示状態にしないとデータが設定できないので、全部表示状態にする
                // tabList.Style["display"] = "block";
                // tabRoom.Style["display"] = "block";

                // 施設予約データを表示する
                subDispBookData_List(targetList, true);
                subDispBookData_Room(targetRoom, true);

                // ボタンと隠しデータを初期化する
                subClearButton(enumTab.List);
                subClearButton(enumTab.Room);

                // 再表示対象でないほうのタブのスクロールバーに元の値を設定する
                if (((Button)sender).ID == "btn_reload1")
                {
                    tabList.Attributes["scrollLeft"] = hid_scrollLeft1.Value;
                    this.subSetDispTab(enumTab.List);
                }
                else
                {
                    tabRoom.Attributes["scrollLeft"] = hid_scrollLeft2.Value;
                    this.subSetDispTab(enumTab.Room);
                }
            }
            else
            {
                CommonMain.subMessageBox_upd(this, CommonMain.CNS_ERR_MSGERR + "【表示日付】が不正です");
                // 指定するコントロールにフォーカスをセットする
                SetFocus(txtDispDate);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnComplete_Click(object sender, System.EventArgs e)
        {
            //=========== 入力値合理性チェック ===========
            if (!(fncCheckInpData()))
            {
                DateTime targetList = (DateTime)Session["targetDateList"];
                DateTime targetShisetsu = (DateTime)Session["targetDateRoom"];

                // 施設選択を元に戻す
                selRoom2.SelectedValue = (string)Session["RoomId"];

                // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                subDispBookData_List(targetList, false);
                subDispBookData_Room(targetShisetsu, false);

                if (hid_selected_tab.Value == "1")
                {
                    // スクロール位置を設定する
                    tabList.Attributes["scrollLeft"] = hid_scrollLeft1.Value;
                    // タブの表示を設定する
                    this.subSetDispTab(enumTab.List);
                }
                else
                {
                    // スクロール位置を設定する
                    tabRoom.Attributes["scrollLeft"] = hid_scrollLeft2.Value;
                    // タブの表示を設定する
                    this.subSetDispTab(enumTab.Room);
                }
                return;
            }

            //=========== 既予約登録チェック ===========
            if (!(fncCheckTimeData()))
            {
                DateTime targetList = (DateTime)Session["targetDateList"];
                DateTime targetRoom = (DateTime)Session["targetDateRoom"];

                // 施設選択を元に戻す
                selRoom2.SelectedValue = (string)Session["RoomId"];

                // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                subDispBookData_List(targetList, false);
                subDispBookData_Room(targetRoom, false);

                if (hid_selected_tab.Value == "1")
                {
                    // スクロール位置を設定する
                    tabList.Attributes["scrollLeft"] = hid_scrollLeft1.Value;
                    // タブの表示を設定する
                    subSetDispTab(enumTab.List);
                }
                else
                {
                    // スクロール位置を設定する
                    tabRoom.Attributes["scrollLeft"] = hid_scrollLeft2.Value;
                    // タブの表示を設定する
                    subSetDispTab(enumTab.Room);
                }
                return;
            }

            //=========== データ登録 ===========
            //施設コード
            string roomId = selRoom.SelectedValue;

            //予約開始日時
            string from = "0" + txtBookTimeFrom.Text.Trim();
            from = from.Substring(from.Length - 5, 5) + ":00";
            //予約終了日時
            string to = "0" + txtBookTimeTo.Text.Trim();
            to = to.Substring(to.Length - 5, 5) + ":00";
            //指定日（年月日）
            string setDay = txtBookDate.Text;

            //----「予約開始日時」編集
            DateTime startDatetime = DateTime.Parse(setDay + " " + from);
            //----「予約終了日時」編集
            DateTime endDatetime = DateTime.Parse(setDay + " " + to);

            try {
                //【予約テーブル】『Reservations』に新しい予約情報を登録する。
                Reservation rsv = new Reservation();
                rsv.RoomId = roomId;
                rsv.StartDateTime = startDatetime;
                rsv.EndDateTime = endDatetime;
                rsv.Comment = txtUse.Text.Trim();
                AddReservations(rsv);

                CommonMain.subMessageBox(this, "登録が完了しました！");

                // フォーカスを設定
                SetFocus(txtBookTimeTo);

                DateTime targetList = DateTime.MinValue;
                DateTime targetRoom = DateTime.MinValue;
                if (hid_selected_tab.Value == enumTab.List.ToString())
                {
                    targetList = DateTime.Parse(setDay);
                    targetRoom = DateTime.Parse(txtDispDate2.Text);
                }
                else
                {
                    targetList = DateTime.Parse(txtDispDate1.Text);
                    DateTime bakDate = (DateTime)Session["targetDateRoom"];
                    if (bakDate.Date <= DateTime.Parse(setDay).Date && DateTime.Parse(setDay).Date < bakDate.AddDays(CNS_SHISETSU_DATE_CNT).Date)
                    {
                        targetRoom = bakDate;
                    }
                    else
                    {
                        targetRoom = DateTime.Parse(setDay);
                    }
                    selRoom2.SelectedValue = selRoom.SelectedValue;
                }

                // 予約データを表示する
                subDispBookData_List(targetList, true);
                subDispBookData_Room(targetRoom, true);

                // ボタンと隠しデータを初期化する
                subClearButton(enumTab.List);
                subClearButton(enumTab.Room);

                if (hid_selected_tab.Value == "1")
                {
                    //スクロール位置を設定する
                    hid_scrollLeft1.Value = fncGetoffsetLeft(startDatetime.Hour).ToString();
                    tabList.Attributes["scrollLeft"] = hid_scrollLeft1.Value;
                    //タブの表示を設定する
                    this.subSetDispTab(enumTab.List);
                }
                else
                {
                    //スクロール位置を設定する
                    hid_scrollLeft2.Value = fncGetoffsetLeft(startDatetime.Hour).ToString();
                    tabRoom.Attributes["scrollLeft"] = hid_scrollLeft2.Value;
                    //タブの表示を設定する
                    this.subSetDispTab(enumTab.Room);
                }
            }
            catch (Exception ex)
            {
                CommonMain.subMessageBox(this, ex.Message);
                //フォーカスを設定
                SetFocus(txtBookTimeTo);
            }
        }


        /// <summary>
        /// ダミーボタンクリック時の処理（予約テーブルの追加、更新、削除）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDummy_Click(object sender, System.EventArgs e)
        {
            HiddenField hid_addMode = null;
            DateTime target = DateTime.MinValue;
            bool blnRet = false;

            if (((Button)sender).ClientID == "btnDummy1")
            {
                hid_addMode = hid_addMode1;
            }
            else
            {
                hid_addMode = hid_addMode2;
            }

            switch (hid_addMode.Value)
            {
                case "":
                    break;
                case "end":
                    // 予約追加完了
                    if (((Button)sender).ClientID == "btnDummy1")
                    {
                        // 施設予約テーブルにデータを追加する
                        blnRet = fncAddBookingData_List();
                        // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                        target = (DateTime)Session["targetDateRoom"];
                        subDispBookData_Room(target, blnRet);
                    }
                    else
                    {
                        // 施設予約テーブルにデータを追加する
                        blnRet = fncAddBookingData_Room();
                        // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                        target = (DateTime)Session["targetDateList"];
                        subDispBookData_List(target, blnRet);
                    }

                    break;
                case "delete":
                    // 削除
                    if (((Button)sender).ID == "btnDummy1")
                    {
                        // 施設予約テーブルのデータを削除する
                        blnRet = fncDeleteBookingData_List();
                        // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                        target = (DateTime)Session["targetDateRoom"];
                        this.subDispBookData_Room(target, blnRet);
                    }
                    else
                    {
                        // 施設予約テーブルのデータを削除する
                        blnRet = fncDeleteBookingData_Room();
                        // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                        target = (DateTime)Session["targetDateList"];
                        subDispBookData_List(target, blnRet);
                    }

                    break;
                case "update":
                    if (((Button)sender).ID == "btnDummy1")
                    {
                        // 予約テーブルのデータを更新する
                        blnRet = fncUpdateBookingData_List();
                        // 予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                        target = (DateTime)Session["targetDateRoom"];
                        subDispBookData_Room(target, blnRet);
                    }
                    else
                    {
                        // 予約テーブルのデータを更新する
                        blnRet = fncUpdateBookingData_Room();
                        // 予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                        target = (DateTime)Session["targetDateList"];
                        this.subDispBookData_List(target, blnRet);
                    }
                    break;
            }

        }

        /// <summary>
        /// 予約テーブルにデータを追加する
        /// </summary>
        /// <returns></returns>
        protected bool fncAddBookingData_List()
        {
            string yoto = hid_yoto1.Value;
            DateTime target = (DateTime)Session["targetDateList"];
            List<Room> rooms = (List<Room>)Session["Rooms"];
            int row = int.Parse(hid_clickRow1.Value);
            int colStart = int.Parse(hid_clickColStart1.Value);
            int colEnd = int.Parse(hid_clickColEnd1.Value);
            DateTime startDatetime = subGetBookDatetime(target.Date, colStart, 0);
            DateTime endDatetime = subGetBookDatetime(target.Date, colEnd, 1);
            var roomId = rooms[row - 2].Id;     // ヘッダ行が２行あるから

            // 既予約登録チェック
            if (fncCheckTimeData(roomId, startDatetime, endDatetime))
            {
                // データ登録
                // 【予約テーブル】『Reservations』に新しい予約情報を登録する。
                Reservation rsv = new Reservation();
                rsv.RoomId = roomId;
                rsv.StartDateTime = startDatetime;
                rsv.EndDateTime = endDatetime;
                rsv.Comment = yoto.Trim();
                AddReservations(rsv);
                // 施設予約データを表示する
                subDispBookData_List(target, true);
                // javascript用の処理モードを初期化する
                hid_addMode1.Value = "";
                return true;
            }
            else
            {
                CommonMain.subMessageBox_upd(this, CommonMain.CNS_ERR_MSGERR + "設定した時間範囲内に既に予約が登録されています！");
                // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                subDispBookData_List(target, false);
                // 編集中のセルの背景色を設定する
                for (var i = colStart; i <= colEnd; i++)
                {
                    tblBooking1.Rows[row].Cells[i].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_REQUEST);
                }
                // フォーカスを設定
                SetFocus(btnEnd1);
                return false;
            }

        }

        /// <summary>
        /// 予約テーブルにデータを追加する
        /// </summary>
        /// <returns></returns>
        protected bool fncAddBookingData_Room()
        {
            string yoto = hid_yoto1.Value;
            DateTime target = (DateTime)Session["targetDateRoom"];
            int row = int.Parse(hid_clickRow2.Value);
            int colStart = int.Parse(hid_clickColStart2.Value);
            int colEnd = int.Parse(hid_clickColEnd2.Value);
            DateTime startDatetime = subGetBookDatetime(target.AddDays(row - 2).Date, colStart, 0);
            DateTime endDatetime = subGetBookDatetime(target.AddDays(row - 2).Date, colEnd, 1);
            var roomId = (String)Session["RoomId"];

            // 会議室選択を元に戻す
            selRoom2.SelectedValue = roomId;

            // 既予約登録チェック
            if (fncCheckTimeData(roomId, startDatetime, endDatetime))
            {
                // データ登録
                // 【予約テーブル】『Reservations』に新しい予約情報を登録する。
                Reservation rsv = new Reservation();
                rsv.RoomId = roomId;
                rsv.StartDateTime = startDatetime;
                rsv.EndDateTime = endDatetime;
                rsv.Comment = yoto.Trim();
                AddReservations(rsv);
                // 施設予約データを表示する
                subDispBookData_Room(target, true);
                // javascript用の処理モードを初期化する
                hid_addMode2.Value = "";
                return true;
            }
            else
            {
                CommonMain.subMessageBox_upd(this, CommonMain.CNS_ERR_MSGERR + "設定した時間範囲内に既に予約が登録されています！");
                // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                subDispBookData_Room(target, false);
                // 編集中のセルの背景色を設定する
                for (var i = colStart; i <= colEnd; i++)
                {
                    tblBooking2.Rows[row].Cells[i].BackColor = ColorTranslator.FromHtml(CNS_SHISETSU_REQUEST);
                }
                // フォーカスを設定
                SetFocus(btnEnd2);
                return false;
            }
        }

        /// <summary>
        /// 予約テーブルのデータを削除する
        /// </summary>
        /// <returns></returns>
        protected bool fncDeleteBookingData_List()
        {
            List<ReservationList> reservationLists = (List<ReservationList>)Session["ReservationLists"];
            DateTime target = (DateTime)Session["targetDateList"];
            int row = int.Parse(hid_clickRow1.Value);
            int colStart = int.Parse(hid_clickColStart1.Value);
            int colEnd = int.Parse(hid_clickColEnd1.Value);
            Reservation rsv = reservationLists[row - 2].Reservations[colStart];

            try
            {
                // 削除対象データ取得
                var targetRsv = db.Reservations.Find(rsv.Id);

                // 削除フラグを設定する
                targetRsv.DeleteFlag = true;
                targetRsv.UserId = Context.User.Identity.GetUserId();
                targetRsv.EditUserId = Context.User.Identity.GetUserId();
                targetRsv.LastEditDateTime = DateTime.Now;

                // 更新する
                db.Entry(targetRsv).State = EntityState.Modified;
                db.SaveChanges();

                // 予約データを表示する
                subDispBookData_List(target, true);
                // javascript用の処理モードを初期化する
                hid_addMode1.Value = "";
                return true;

            }
            catch(Exception ex)
            {
                CommonMain.subMessageBox_upd(this, CommonMain.CNS_ERR_MSGERR + ex.Message);
                // 予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                this.subDispBookData_List(target, false);
                // フォーカスを設定
                SetFocus(btnEnd1);
                return false;
            }
        }

        /// <summary>
        /// 予約テーブルのデータを削除する
        /// </summary>
        /// <returns></returns>
        protected bool fncDeleteBookingData_Room()
        {

            List<ReservationRoom> reservationRooms = (List<ReservationRoom>)Session["ReservationRooms"];
            DateTime target = (DateTime)Session["targetDateRoom"];
            int row = int.Parse(hid_clickRow2.Value);
            int colStart = int.Parse(hid_clickColStart2.Value);
            int colEnd = int.Parse(hid_clickColEnd2.Value);
            Reservation rsv = reservationRooms[row - 2].Reservations[colStart];

            // 会議室選択を元に戻す
            selRoom2.SelectedValue = (String)Session["RoomId"];

            try {
                // 削除対象データ取得
                var targetRsv = db.Reservations.Find(rsv.Id);

                // 削除フラグを設定する
                targetRsv.DeleteFlag = true;
                targetRsv.UserId = Context.User.Identity.GetUserId();
                targetRsv.EditUserId = Context.User.Identity.GetUserId();
                targetRsv.LastEditDateTime = DateTime.Now;

                // 更新する
                db.Entry(targetRsv).State = EntityState.Modified;
                db.SaveChanges();
                // 予約データを表示する
                subDispBookData_Room(target, true);
                // javascript用の処理モードを初期化する
                hid_addMode2.Value = "";
                return true;
            }
            catch(Exception ex)
            {
                CommonMain.subMessageBox_upd(this, CommonMain.CNS_ERR_MSGERR + ex.Message);
                // 予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                subDispBookData_Room(target, false);
                // フォーカスを設定
                SetFocus(btnEnd2);
                return false;
            }
        }

        /// <summary>
        /// 予約テーブルのデータを更新する
        /// </summary>
        /// <returns></returns>
        protected bool fncUpdateBookingData_List()
        {
            List<ReservationList> reservationLists = (List<ReservationList>)Session["ReservationLists"];
            DateTime target = (DateTime)Session["targetDateList"];
            int row = int.Parse(hid_clickRow1.Value);
            int colStart = int.Parse(hid_clickColStart1.Value);
            int colEnd = int.Parse(hid_clickColEnd1.Value);
            Reservation rsv = reservationLists[row - 2].Reservations[colStart];

            // 更新データを設定する
            DateTime dt = target;
            DateTime startDatetime = dt.AddHours(double.Parse(hid_time_from1.Value)).AddMinutes(double.Parse(hid_minutes_from1.Value));
            DateTime endDatetime = dt.AddHours(double.Parse(hid_time_to1.Value)).AddMinutes(double.Parse(hid_minutes_to1.Value));

            // 既予約登録チェック
            if (fncCheckTimeData(rsv.RoomId, startDatetime, endDatetime, rsv.Id))
            {
                try
                {
                    // 更新対象データ取得
                    var targetRsv = db.Reservations.Find(rsv.Id);
                    targetRsv.StartDateTime = startDatetime;
                    targetRsv.EndDateTime = endDatetime;
                    targetRsv.Comment = hid_yoto1.Value.Trim();
                    targetRsv.UserId = Context.User.Identity.GetUserId();
                    targetRsv.EditUserId = Context.User.Identity.GetUserId();
                    targetRsv.LastEditDateTime = DateTime.Now;

                    // 更新する
                    db.Entry(targetRsv).State = EntityState.Modified;
                    db.SaveChanges();

                    // 予約データを表示する
                    subDispBookData_List(target, true);
                    // javascript用の処理モードを初期化する
                    hid_addMode1.Value = "";
                    return true;
                }
                catch (Exception ex)
                {
                    CommonMain.subMessageBox_upd(this, CommonMain.CNS_ERR_MSGERR + ex.Message);
                    // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                    subDispBookData_List(target, false);
                    // フォーカスを設定
                    SetFocus(btnEnd1);
                    return false;
                }
            }
            else
            {
                // 予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                subDispBookData_List(target, false);
                return false;
            }
        }

        /// <summary>
        /// 予約テーブルのデータを更新する
        /// </summary>
        /// <returns></returns>
        protected bool fncUpdateBookingData_Room()
        {
            List<ReservationRoom> reservationRooms = (List<ReservationRoom>)Session["ReservationRooms"];
            DateTime target = (DateTime)Session["targetDateRoom"];
            int row = int.Parse(hid_clickRow2.Value);
            int colStart = int.Parse(hid_clickColStart2.Value);
            int colEnd = int.Parse(hid_clickColEnd2.Value);
            Reservation rsv = reservationRooms[row - 2].Reservations[colStart];

            // 更新データを設定する
            DateTime dt = rsv.StartDateTime.Date;
            DateTime startDatetime = dt.AddHours(double.Parse(hid_time_from2.Value)).AddMinutes(double.Parse(hid_minutes_from2.Value));
            DateTime endDatetime = dt.AddHours(double.Parse(hid_time_to2.Value)).AddMinutes(double.Parse(hid_minutes_to2.Value));

            // 既予約登録チェック
            if (fncCheckTimeData(rsv.RoomId, startDatetime, endDatetime, rsv.Id))
            {
                try{
                    // 更新対象データ取得
                    var targetRsv = db.Reservations.Find(rsv.Id);
                    targetRsv.StartDateTime = startDatetime;
                    targetRsv.EndDateTime = endDatetime;
                    targetRsv.Comment = hid_yoto2.Value.Trim();
                    targetRsv.UserId = Context.User.Identity.GetUserId();
                    targetRsv.EditUserId = Context.User.Identity.GetUserId();
                    targetRsv.LastEditDateTime = DateTime.Now;

                    // 更新する
                    db.Entry(targetRsv).State = EntityState.Modified;
                    db.SaveChanges();

                    // 予約データを表示する
                    subDispBookData_Room(target, true);
                    // javascript用の処理モードを初期化する
                    hid_addMode2.Value = "";
                    return true;
                }
                catch (Exception ex)
                {
                    CommonMain.subMessageBox_upd(this, CommonMain.CNS_ERR_MSGERR + ex.Message);
                    // 施設予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                    subDispBookData_Room(target, false);
                    // フォーカスを設定
                    SetFocus(btnEnd2);
                    return false;
                }
            }
            else
            {
                // 予約データを表示する（新たに読み直さず、以前表示していたデータを再表示する）
                subDispBookData_Room(target, false);
                return false;
            }

        }


        /// <summary>
        /// 日時を、DateTime型で取得する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="col"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        private DateTime subGetBookDatetime(DateTime target, int col, int kind)
        {
            DateTime start = target.Date.AddHours(CNS_Facility_TimeStart);
            return start.AddMinutes((col + kind) * CNS_SHISETSU_CELL_DURATION);
        }

        /// <summary>
        /// 既予約登録チェック
        /// </summary>
        /// <param name="strParaSisetsuCD"></param>
        /// <param name="startDatetime"></param>
        /// <param name="endDatetime"></param>
        /// <param name="intOutYoyakuID"></param>
        /// <returns></returns>
        private bool fncCheckTimeData(string strParaSisetsuCD, DateTime startDatetime, DateTime endDatetime, int intOutYoyakuID = 0)
        {
            //開始日
            string strStartDate = startDatetime.ToString("yyyy/MM/dd");
            //開始時間
            string strStartTime = startDatetime.ToString("hh:mm:ss");
            //終了日
            string strEndDate = endDatetime.ToString("yyyy/MM/dd");
            //終了時間
            string strEndTime = endDatetime.ToString("hh:mm:ss");

            //【予約テーブル】『Reservations』に指定範囲内に既に予約データが登録されているかチェックする。
            string message = "";
            ErrorInfo errorInfo = new ErrorInfo();
            if (commonDB.fncCheck_YoyakuData_TimeRange_UMU(strStartDate, strStartTime, strEndDate, strEndTime, strParaSisetsuCD, intOutYoyakuID, ref message, ref errorInfo))
            {
                //データが登録されている
                return false;
            }
            return true;
        }

        /// <summary>
        /// 予約テーブルにデータを追加する
        /// </summary>
        /// <param name="Reservations"></param>
        public void AddReservations(Reservation Reservations)
        {
            Reservation rsv = new Reservation();
            rsv.RoomId = Reservations.RoomId;
            rsv.StartDateTime = Reservations.StartDateTime;
            rsv.EndDateTime = Reservations.EndDateTime;
            rsv.Comment = Reservations.Comment;
            rsv.UserId = Context.User.Identity.GetUserId();
            rsv.DeleteFlag = false;
            rsv.AddUserId = Context.User.Identity.GetUserId();
            rsv.EditUserId = Context.User.Identity.GetUserId();
            rsv.AddDateTime = DateTime.Now;
            rsv.LastEditDateTime = DateTime.Now;

            // 追加する
            db.Reservations.Add(rsv);
            db.SaveChanges();
        }

        /// <summary>
        /// 入力値合理性チェック
        /// </summary>
        /// <returns></returns>
        private bool fncCheckInpData()
        {
            // 会議室が未選択？
            if (selRoom.SelectedIndex < 0)
            {
                CommonMain.subMessageBox(this, CommonMain.CNS_ERR_MSGERR + "「会議室」が選択されていません！");
                // フォーカスを設定
                SetFocus(selRoom);
                return false;
            }

            // 未入力項目があるか？
            //---- 予約日付
            if (txtBookDate.Text.Trim() == "")
            {
                CommonMain.subMessageBox(this, CommonMain.CNS_ERR_MSGERR + "予約日付が未設定です！");
                // フォーカスを設定
                SetFocus(txtBookDate);
                return false;
            }
            //---- 予約開始時刻
            if (txtBookTimeFrom.Text.Trim() == "")
            {
                CommonMain.subMessageBox(this, CommonMain.CNS_ERR_MSGERR + "予約開始時刻が未設定です！");
                // フォーカスを設定
                SetFocus(txtBookTimeFrom);
                return false;
            }
            //---- 予約終了時刻
            if (txtBookTimeTo.Text.Trim() == "")
            {
                CommonMain.subMessageBox(this, CommonMain.CNS_ERR_MSGERR + "予約終了時刻が未設定です！");
                // フォーカスを設定
                SetFocus(txtBookTimeTo);
                return false;
            }

            // 予約時刻の大小チェック
            string from = "0" + txtBookTimeFrom.Text.Trim();
            from = from.Substring(from.Length - 5, 5);
            string to = "0" + txtBookTimeTo.Text.Trim();
            to = to.Substring(to.Length - 5, 5);
            if (from.CompareTo(to) >= 0)
            {
                CommonMain.subMessageBox(this, CommonMain.CNS_ERR_MSGERR + "予約開始時刻 ＜ 予約終了時刻 で設定してください！");
                // フォーカスを設定
                SetFocus(txtBookTimeTo);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 既予約登録チェック
        /// </summary>
        /// <returns></returns>
        private bool fncCheckTimeData()
        {
            // 会議室コード
            string roomId = selRoom.SelectedValue;

            // 入力データ取得
            string from = "0" + txtBookTimeFrom.Text.Trim();
            string to = "0" + txtBookTimeTo.Text.Trim();
            string setDay = txtBookDate.Text;
            string startTime = from.Substring(from.Length - 5, 5) + ":00";
            string endTime = to.Substring(to.Length - 5, 5) + ":00";

            //【予約テーブル】『Reservations』に指定範囲内に既に予約データが登録されているかチェックする。
            string message = "";
            ErrorInfo errorInfo = new ErrorInfo();
            if (commonDB.fncCheck_YoyakuData_TimeRange_UMU(setDay, startTime, setDay, endTime, roomId, 0, ref message, ref errorInfo))
            {
                //データが登録されている
                CommonMain.subMessageBox(this, CommonMain.CNS_ERR_MSGERR + "設定した時間範囲内に既に予約が登録されています！");
                //フォーカスを設定
                SetFocus(txtBookTimeFrom);
                return false;
            }
            return true;
        }

        /// <summary>
        /// テーブルのオフセットを取得する
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        private int fncGetoffsetLeft(int hour)
        {
            int offsetLeft = 0;
            int tableWithd = (CNS_Facility_TimeEnd - CNS_Facility_TimeStart) * (60 / CNS_SHISETSU_CELL_DURATION) * CNS_SHISETSU_CELL_WIDTH;

            offsetLeft = (hour - CNS_Facility_TimeStart) * (60 / CNS_SHISETSU_CELL_DURATION) * CNS_SHISETSU_CELL_WIDTH;
            if (offsetLeft < 0)
            {
                offsetLeft = 0;
            }
            else if (offsetLeft > tableWithd - (CNS_SHISETSU_DISP_HOURS * (60 / CNS_SHISETSU_CELL_DURATION) * CNS_SHISETSU_CELL_WIDTH))
            {
                offsetLeft = tableWithd - (CNS_SHISETSU_DISP_HOURS * (60 / CNS_SHISETSU_CELL_DURATION) * CNS_SHISETSU_CELL_WIDTH);
            }
            return offsetLeft;
        }
    }
}