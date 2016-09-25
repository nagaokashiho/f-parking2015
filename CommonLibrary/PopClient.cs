using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace CommonLibrary
{
    /// <summary>
    /// POP よりメールを受信するクラスです。
    /// </summary>
    public class PopClient : IDisposable
    {
        /// <summary>TCP 接続</summary>
        private TcpClient tcp = null;

        /// <summary>TCP 接続からのリーダー</summary>
        private StreamReader reader = null;

        /// <summary>
        /// コンストラクタです。POPサーバと接続します。
        /// </summary>
        /// <param name="hostname">POPサーバのホスト名。</param>
        /// <param name="port">POPサーバのポート番号（通常は110）。</param>
        /// <remarks></remarks>
        public PopClient(string hostname, int port)
        {
            // サーバと接続
            this.tcp = new TcpClient(hostname, port);
            this.reader = new StreamReader(this.tcp.GetStream(), Encoding.ASCII);

            // オープニング受信
            string s = ReadLine();
            if (!(s.ToUpper().StartsWith("+OK")))
            {
                throw new PopClientException("接続時に POP サーバが \"" + s + "\" を返しました。");
            }
        }

        /// <summary>
        /// 解放処理を行います。
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            if (this.reader != null)
            {
                ((IDisposable)this.reader).Dispose();
                this.reader = null;
            }
            if (this.tcp != null)
            {
                ((IDisposable)this.tcp).Dispose();
                this.tcp = null;
            }
        }

        /// <summary>
        /// POP サーバにログインします。
        /// </summary>
        /// <param name="username">ユーザ名。</param>
        /// <param name="password">パスワード。</param>
        /// <remarks></remarks>
        public void Login(string username, string password)
        {
            // ユーザ名送信
            SendLine("USER " + username);
            string s = ReadLine();
            if (!(s.ToUpper().StartsWith("+OK")))
            {
                throw new PopClientException("USER 送信時に POP サーバが \"" + s + "\" を返しました。");
            }

            // パスワード送信
            SendLine("PASS " + password);
            s = ReadLine();
            if (!(s.ToUpper().StartsWith("+OK")))
            {
                throw new PopClientException("PASS 送信時に POP サーバが \"" + s + "\" を返しました。");
            }
        }

        /// <summary>
        /// POP サーバに溜まっているメールのリストを取得します。
        /// </summary>
        /// <returns>System.String を格納した ArrayList。</returns>
        /// <remarks></remarks>
        public ArrayList GetList()
        {
            // LIST 送信
            SendLine("LIST");
            string s = ReadLine();
            if (!(s.ToUpper().StartsWith("+OK")))
            {
                throw new PopClientException("LIST 送信時に POP サーバが \"" + s + "\" を返しました。");
            }

            // サーバにたまっているメールの数を取得
            ArrayList list = new ArrayList();
            while (true)
            {
                s = ReadLine();
                if (s == ".")
                {
                    // 終端に到達
                    break;
                }
                // メール番号部分のみを取り出し格納
                int p = s.IndexOf(' ');
                if (p > 0)
                {
                    s = s.Substring(0, p);
                }
                list.Add(s);
            }
            return list;
        }

        /// <summary>
        /// POP サーバからメールを 1つ取得します。
        /// </summary>
        /// <param name="num">GetList() メソッドで取得したメールの番号。</param>
        /// <returns>メールの本体。</returns>
        /// <remarks></remarks>
        public string GetMail(string num)
        {
            // RETR 送信
            SendLine("RETR " + num);
            string s = ReadLine();
            if (!(s.ToUpper().StartsWith("+OK")))
            {
                throw new PopClientException("RETR 送信時に POP サーバが \"" + s + "\" を返しました。");
            }

            // メール取得
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                s = ReadLine();
                if (s == ".")
                {
                    // "." のみの場合はメールの終端を表す
                    break;
                }
                sb.Append(s);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// POP サーバのメールを 1つ削除します。
        /// </summary>
        /// <param name="num">GetList() メソッドで取得したメールの番号。</param>
        /// <remarks></remarks>
        public void DeleteMail(string num)
        {
            // DELE 送信
            SendLine("DELE " + num);
            string s = ReadLine();
            if (!(s.ToUpper().StartsWith("+OK")))
            {
                throw new PopClientException("DELE 送信時に POP サーバが \"" + s + "\" を返しました。");
            }
        }

        /// <summary>
        /// POP サーバと切断します。
        /// </summary>
        /// <remarks></remarks>
        public void Close()
        {
            // QUIT 送信
            SendLine("QUIT");
            string s = ReadLine();
            if (!(s.ToUpper().StartsWith("+OK")))
            {
                throw new PopClientException("QUIT 送信時に POP サーバが \"" + s + "\" を返しました。");
            }

            ((IDisposable)this.reader).Dispose();
            this.reader = null;
            ((IDisposable)this.tcp).Dispose();
            this.tcp = null;
        }

        /// <summary>
        /// POP サーバにコマンドを送信します。
        /// </summary>
        /// <param name="s">送信する文字列。</param>
        /// <remarks></remarks>
        private void Send(string s)
        {
            Print("送信: " + s);
            byte[] b = Encoding.ASCII.GetBytes(s);
            this.tcp.GetStream().Write(b, 0, b.Length);
        }

        /// <summary>
        /// POP サーバにコマンドを送信します。末尾に改行を付加します。
        /// </summary>
        /// <param name="s">送信する文字列。</param>
        /// <remarks></remarks>
        private void SendLine(string s)
        {
            Print("送信: " + s + "\\r\\n");
            byte[] b = Encoding.ASCII.GetBytes(s + Environment.NewLine);
            this.tcp.GetStream().Write(b, 0, b.Length);
        }

        /// <summary>
        /// POP サーバから 1行読み込みます。
        /// </summary>
        /// <returns>読み込んだ文字列。</returns>
        /// <remarks></remarks>
        private string ReadLine()
        {
            string s = this.reader.ReadLine();
            Print("受信: " + s + "\\r\\n");
            return s;
        }

        /// <summary>
        /// チェック用にコンソールに出力します。
        /// </summary>
        /// <param name="msg">出力する文字列。</param>
        /// <remarks></remarks>
        private void Print(string msg)
        {
            //Console.WriteLine(msg)
        }

    }
}
