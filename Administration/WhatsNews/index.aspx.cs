using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Administration.Models;
using System.Data.Entity;

namespace Administration.WhatsNews
{
    public partial class index : System.Web.UI.Page
    {
        private f_parkingContext db = new f_parkingContext();

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

            // 新規項目をクリア
            initDisp();
        }

        // 掲載日付の降順で取得します。
        public IQueryable<WhatsNew> SelectNews()
        {
            return db.WhatsNews.OrderByDescending(news => news.PublicationDate);
        }

        // What's New 情報を更新します。
        public void UpdateNews(WhatsNew news)
        {
            // 更新対象データ取得
            var target = db.WhatsNews.Find(news.id);

            // 入力値を反映する
            target.Division = "0";
            target.PublicationDate = news.PublicationDate;
            target.No = news.No;
            target.Title = news.Title;
            target.Message = news.Message;
            target.Url = news.Url;
            target.ImportantFlag = false;
            target.OpenFlag = news.OpenFlag;
            target.NewDays = 0;
            target.OpenDays = news.OpenDays;

            // 更新する
            db.Entry(target).State = EntityState.Modified;
            db.SaveChanges();

        }

        // What's New 情報を削除します。
        public void DeleteNews(WhatsNew news)
        {
            // 削除対象データ取得
            var target = db.WhatsNews.Find(news.id);

            // 削除する
            db.Entry(target).State = EntityState.Deleted;
            db.SaveChanges();
        }

        protected void NewsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 公開フラグを設定する
                var isOpen = e.Row.FindControl("chkOpenFlag") as CheckBox;
                var news = e.Row.DataItem as WhatsNew;
                isOpen.Checked = news.OpenFlag;
            }
        }

        protected void CreateNews_Click(object sender, EventArgs e)
        {
            // 一覧の編集状態を解除
            NewsGridView.SetEditRow(-1);

            if (!Page.IsValid)
            {
                return;
            }

            // 入力した情報で情報を作成
            var news = new WhatsNew();
            news.Division = "0";
            news.PublicationDate = DateTime.Parse(txtPublicationDate.Text);
            news.No = int.Parse(txtNo.Text);
            news.Title = txtTitle.Text.Trim();
            news.Message = txtMessage.Text.Trim();
            news.Url = txtUrl.Text.Trim();
            news.ImportantFlag = false;
            news.OpenFlag = chkOpenFlag.Checked;
            news.NewDays = 0;
            news.OpenDays = int.Parse(txtOpenDays.Text);

            // 追加する
            db.WhatsNews.Add(news);
            db.SaveChanges();

            // 新規項目をクリア
            initDisp();

            // What's New 一覧をリフレッシュ
            NewsGridView.DataBind();

        }

        /// <summary>
        /// 新規項目をクリア
        /// </summary>
        protected void initDisp()
        {
            // 新規項目をクリア
            txtPublicationDate.Text = string.Format("{0:yyyy/MM/dd}", DateTime.Now);
            txtNo.Text = "1";
            txtTitle.Text = "";
            txtMessage.Text = "";
            txtUrl.Text = "";
            chkOpenFlag.Checked = true;
            txtOpenDays.Text = "0";

        }
    }
}