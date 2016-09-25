using System;
using System.Web;
using CommonLibrary;
using System.Configuration;
using System.IO;

namespace f_parking.contact
{

    public partial class confirm : System.Web.UI.Page
    {
        ComMain.Contact cont = new ComMain.Contact();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == true)
            {
                // ポストバックの際には、変数にセッションの値を再設定する。
                cont = (ComMain.Contact)Session["Contact"];
                return;
            }

            // キャッシュを無効化する
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            // 初期表示を行う
            dispInit();

        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("index.aspx");        // 前ページに戻る
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (sendMail())
            {
                // セッションをクリアする
                Session.Remove("Contact");
                // ページ遷移
                Response.Redirect("end.html");
            }

        }

        private void dispInit()
        {
            // セッションの値をページに表示する
            if (Session["Contact"] == null)
            {
                // トップページに戻る
                Response.Redirect("../index.aspx");
                return;
            }
            cont = (ComMain.Contact)Session["Contact"];
            lblContactKind.Text = cont.contactKind;
            lblName.Text = cont.name;
            lblPref.Text = cont.pref;
            lblAddress.Text = cont.address;
            lblTel.Text = cont.tel;
            lblMail.Text = cont.mail;
            lblMessage.Text = cont.message.Replace("\r\n", "<br />");
        }

        private bool sendMail()
        {
            string receiveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // メールサーバの情報を取得する
            ComMain.MailInfo mail = new ComMain.MailInfo();
            mail.popHostName = ConfigurationManager.AppSettings["POP HostName"];
            mail.popPort = ConfigurationManager.AppSettings["POP Port"];
            mail.smtpHostName = ConfigurationManager.AppSettings["SMTP HostName"];
            mail.smtpPort = ConfigurationManager.AppSettings["SMTP Port"];
            mail.mailAccount = ConfigurationManager.AppSettings["MAIL Account"];
            mail.mailPassword = ConfigurationManager.AppSettings["MAIL Password"];
            mail.mailAddress = ConfigurationManager.AppSettings["MAIL Address"];
            mail.mailUserName = ConfigurationManager.AppSettings["MAIL UserName"];
            mail.mailSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["MAIL SSL"]);
            mail.popBeforeSmtp = Convert.ToBoolean(ConfigurationManager.AppSettings["POP Before SMTP"]);
            
            //　管理者にメールを送信する
            //メール本文のテンプレートファイルを読み込んで、変数を置換する
             using (var sr = new StreamReader(Server.MapPath(ConfigurationManager.AppSettings["ToAdmin Body"])))
            {
                // パラメータ（%～%）の置換
                var body = sr.ReadToEnd()
                       .Replace("%DATE%", receiveTime)
                       .Replace("%CONTACT%", cont.contactKind)
                       .Replace("%NAME%", cont.name)
                       .Replace("%PREF%", cont.pref)
                       .Replace("%ADDRESS%", cont.address)
                       .Replace("%TEL%", cont.tel)
                       .Replace("%MAIL%", cont.mail)
                       .Replace("%MESSAGE%", cont.message);

                //メールを送信する
                string toAddress = ConfigurationManager.AppSettings["ToAdmin Address"];
                string subject = ConfigurationManager.AppSettings["ToAdmin Subject"];
                string errorMsg = string.Empty;
                if (!(SendMailMain.SendMail(mail, toAddress, subject, body, ref errorMsg)))
                {
                    //エラーの場合はメッセージを表示する
                    lblError.Text = errorMsg;
                    return false;
                }
            }

            //申込者にメールを送信する
            //メール本文のテンプレートファイルを読み込んで、変数を置換する
            using (var sr = new StreamReader(Server.MapPath(ConfigurationManager.AppSettings["ToCustomer Body"])))
            {
                // パラメータ（%～%）の置換
                var body = sr.ReadToEnd()
                       .Replace("%DATE%", receiveTime)
                       .Replace("%CONTACT%", cont.contactKind)
                       .Replace("%NAME%", cont.name)
                       .Replace("%PREF%", cont.pref)
                       .Replace("%ADDRESS%", cont.address)
                       .Replace("%TEL%", cont.tel)
                       .Replace("%MAIL%", cont.mail)
                       .Replace("%MESSAGE%", cont.message);

                //メールを送信する
                string toAddress = cont.mail;
                string subject = ConfigurationManager.AppSettings["ToCustomer Subject"];
                string errorMsg = string.Empty;
                if (!(SendMailMain.SendMail(mail, toAddress, subject, body, ref errorMsg)))
                {
                    //エラーの場合はメッセージを表示する
                    lblError.Text = errorMsg;
                    return false;
                }
            }

            return true;
        }
    }
}