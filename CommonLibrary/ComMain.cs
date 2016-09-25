namespace CommonLibrary
{
    public class ComMain
    {
        // お問い合わせページ
        public struct Contact
        {
            public int contactKindNo { get; set; }
            public string contactKind { get; set; }
            public string name { get; set; }
            public string pref { get; set; }
            public string address { get; set; }
            public string tel { get; set; }
            public string mail { get; set; }
            public string message { get; set; }
        }

        // メール情報
        public struct MailInfo
        {
            public string popHostName { get; set; }
            public string popPort { get; set; }
            public string smtpHostName { get; set; }
            public string smtpPort { get; set; }
            public string mailAccount { get; set; }
            public string mailPassword { get; set; }
            public string mailAddress { get; set; }
            public string mailUserName { get; set; }
            public bool mailSsl { get; set; }
            public bool popBeforeSmtp { get; set; }
            public string mailSubject { get; set; }
            public string mailBody { get; set; }
        }

    }
}
