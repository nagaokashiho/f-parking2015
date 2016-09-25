using System;
using System.Collections;
using System.Net.Mail;

namespace CommonLibrary
{
    public class SendMailMain
    {

        private static PopClient pop; // popサーバ
        private static ArrayList list; // smtpサーバにたまっているメールのリスト
        private static SendMailClient smtp; // smtpサーバ

        /// <summary>
        /// メール送信処理
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="toAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool SendMail(ComMain.MailInfo mail, string toAddress, string subject, string body, ref string errorMsg)
        {

            //メール初期処理
            if (!(InitMail(mail, ref errorMsg)))
            {
                return false;
            }

            //メール送信処理
            if (!(MainMail(mail, toAddress, subject, body, ref errorMsg)))
            {
                return false;
            }

            //メール終了処理
            if (!(EndMail(ref errorMsg)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// メール初期処理
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool InitMail(ComMain.MailInfo mail, ref string errorMsg)
        {

            try
            {
                if (mail.popBeforeSmtp)
                {
                    // ***** １度メールを受信しないと、送信できないから *****
                    // POP サーバに接続します。
                    pop = new PopClient(mail.popHostName, Convert.ToInt32(mail.popPort));

                    // POP サーバにログインします。
                    pop.Login(mail.mailAccount, mail.mailPassword);

                    // POP サーバに溜まっているメールのリストを取得します。
                    list = pop.GetList();

                    // 送信専用アドレスで受信したメールは不要のはずなので削除する
                    for (int i = 0; i < list.Count; i++)
                    {
                        // ★注意★
                        // 削除したメールを元に戻すことはできません。
                        // 本当に削除していい場合は以下のコメントをはずしてください。
                        pop.DeleteMail(Convert.ToString(list[i]));
                    }
                    // ***** １度メールを受信しないと、送信できないから *****
                }

                // SMTP サーバに接続します。
                smtp = new SendMailClient(mail.smtpHostName, Convert.ToInt32(mail.smtpPort));

                // SMTP サーバにログインします。
                smtp.Login(mail.mailAccount, mail.mailPassword);

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// メール送信処理
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="toAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool MainMail(ComMain.MailInfo mail, string toAddress, string subject, string body, ref string errorMsg)
        {
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(mail.mailAddress, mail.mailUserName);
                message.To.Add(new MailAddress(toAddress));
                message.Subject = subject;
                message.Body = body;

                smtp.SendMail(mail.mailSsl, message);

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }

            return true;

        }

        /// <summary>
        /// メール終了処理
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool EndMail(ref string errorMsg)
        {

            try
            {
                // POPサーバから切断します。
                if (pop != null)
                {
                    pop.Close();
                }

                // SMTPサーバから切断します。
                smtp.Close();

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }

            return true;

        }

    }
}
