using System.Net;
using System.Net.Mail;

namespace CommonLibrary { 
    /// <summary>
    /// SMTP でメールを送信するクラスです。
    /// </summary>
    public class SendMailClient
    {

        /// <summary>SMTP 接続</summary>
        private SmtpClient smtp = null;

        /// <summary>
        /// コンストラクタです。SMTPサーバと接続します。
        /// </summary>
        /// <param name="hostname">SMTPサーバのホスト名。</param>
        /// <param name="port">ポート。</param>
        /// <remarks></remarks>
        public SendMailClient(string hostname, int port)
        {
            // サーバと接続
            this.smtp = new SmtpClient(hostname);
            this.smtp.Port = port;

        }

        /// <summary>
        /// SMTP サーバにログインします。
        /// </summary>
        /// <param name="username">ユーザ名。</param>
        /// <param name="password">パスワード。</param>
        /// <remarks></remarks>
        public void Login(string username, string password)
        {

            this.smtp.Credentials = new NetworkCredential(username, password);

        }

        /// <summary>
        /// メールを送信します。
        /// </summary>
        /// <remarks></remarks>
        public void SendMail(bool ssl, MailMessage message)
        {

            this.smtp.EnableSsl = ssl; //SSL認証のときは、true
            this.smtp.Timeout = 100000; //←これが無いとダメ
            this.smtp.Send(message);

        }

        /// <summary>
        /// SMTP サーバから切断します。
        /// </summary>
        /// <remarks></remarks>
        public void Close()
        {

            // 処理はない。

        }
    }
}
