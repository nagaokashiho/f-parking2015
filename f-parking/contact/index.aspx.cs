using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary;

namespace f_parking.contact
{
    public partial class index : System.Web.UI.Page
    {
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

            // 初期表示を行う
            dispInit();

        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // 検証OKならば、確認ページを表示する
                ComMain.Contact cont = new ComMain.Contact();

                if (rdoParking.Checked) { cont.contactKindNo = 1; cont.contactKind = "駐車場に関するお問い合わせ"; }
                else if (rdoRoom.Checked) { cont.contactKindNo = 2; cont.contactKind = "貸し会議室に関するお問い合わせ"; }
                else if (rdoByke.Checked) { cont.contactKindNo = 3; cont.contactKind = "バイクガレージに関するお問い合わせ"; }
                else if (rdoTenant.Checked) { cont.contactKindNo = 4; cont.contactKind = "テナントに関するお問い合わせ"; }
                else if (rdoOther.Checked) { cont.contactKindNo = 5; cont.contactKind = "その他"; }

                cont.name = txtName.Text.Trim();
                cont.pref = selPref.SelectedValue;
                cont.address = txtAddress.Text.Trim();
                cont.tel = txtTel.Text.Trim();
                cont.mail = txtMail.Text.Trim();
                cont.message = txtMessage.Text.Trim();

                Session["Contact"] = cont;

                Response.Redirect("confirm.aspx");

            }
        }

        private void dispInit()
        {
            // セッションに値が格納されている場合は、
            if (Session["Contact"] != null)
            {
                ComMain.Contact cont = new ComMain.Contact();
                cont = (ComMain.Contact)Session["Contact"];
                switch (cont.contactKindNo)
                {
                    case 1:
                        rdoParking.Checked = true;
                        break;
                    case 2:
                        rdoRoom.Checked = true;
                        break;
                    case 3:
                        rdoByke.Checked = true;
                        break;
                    case 4:
                        rdoTenant.Checked = true;
                        break;
                    case 5:
                        rdoOther.Checked = true;
                        break;
                }
                txtName.Text = cont.name ;
                selPref.SelectedValue = cont.pref ;
                txtAddress.Text = cont.address;
                txtTel.Text = cont.tel;
                txtMail.Text= cont.mail;
                txtMessage.Text = cont.message ;
            }

        }
    }
}