using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Administration.Models;

namespace Administration.Account
{
    public partial class Manage : System.Web.UI.Page
    {
        private ApplicationUserManager manager;

        protected void Page_Init(object sender, EventArgs e)
        {
            // 色々なメソッドで使用するため、UserManagerオブジェクトをフィールドに保管しておく
            manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }
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
        }

        /// <summary>
        /// ユーザーをユーザー名順で取得します。
        /// </summary>
        public IEnumerable<ApplicationUser> SelectUser()
        {
            return manager.Users.OrderBy(user => user.UserName);
        }

        /// <summary>
        /// ユーザー情報を更新します。
        /// </summary>
        /// <param name="user">画面で入力したユーザー情報。</param>
        public void UpdateUser(ApplicationUser user)
        {
             // 更新対象ユーザー情報取得
            var target = manager.FindById(user.Id);

            // 入力値を反映する
            target.Email = user.Email;
            target.UserName = target.Email;
            target.DispUserName = user.DispUserName;

            // パスワードが入力されていたら、パスワードも設定する
            var Password = UserGridView.Rows[UserGridView.EditIndex].FindControl("Password") as TextBox;
            if (String.IsNullOrEmpty(Password.Text) == false)
            {
                var password = Password.Text;

                // 入力したパスワードを検証する
                var validateResult = manager.PasswordValidator.ValidateAsync(password)
                  .GetAwaiter()
                  .GetResult();
                if (validateResult.Succeeded == false)
                {
                    var error = validateResult.Errors.FirstOrDefault();
                    ErrorMessage.Text = error;
                    ModelState.AddModelError("error", error);
                    return;
                }

                // パスワードはハッシュ化して設定する
                var passwordHash = manager.PasswordHasher.HashPassword(password);
                target.PasswordHash = passwordHash;
            }

            // 更新する
            var result = manager.Update(target);
            if (result.Succeeded == false)
            {
                var error = result.Errors.FirstOrDefault();
                ErrorMessage.Text = error;
                ModelState.AddModelError("error", error);
                return;
            }

            // 管理者ロールを更新
            var IsAdmin = UserGridView.Rows[UserGridView.EditIndex].FindControl("IsAdmin") as CheckBox;
            if (IsAdmin.Checked)
            {
                //（1）管理者ロールに追加
                result = manager.AddToRole(user.Id, "Admin");
            }
            else
            {
                //（2）管理者ロールから削除
                result = manager.RemoveFromRole(user.Id, "Admin");
            }
            if (result.Succeeded == false)
            {
                var error = result.Errors.FirstOrDefault();
                ErrorMessage.Text = error;
                ModelState.AddModelError("error", error);
                return;
            }

        }

        /// <summary>
        /// ユーザー情報を削除します。
        /// </summary>
        /// <param name="user">画面で選択したユーザー情報。</param>
        public void DeleteUser(ApplicationUser user)
        {
            // 削除対象ユーザー取得
            var target = manager.FindById(user.Id);

            // 削除する
            var result = manager.Delete(target);
            if (result.Succeeded == false)
            {
                var error = result.Errors.FirstOrDefault();
                ErrorMessage.Text = error;
                ModelState.AddModelError("error", error);
                return;
            }
        }

        /// <summary>
        /// 新たなユーザーを作成します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CreateUser_Click(object sender, EventArgs e)
        {
            // 一覧の編集状態を解除
            UserGridView.SetEditRow(-1);

            if (!Page.IsValid)
            {
                return;
            }

            // 入力した情報でユーザー情報を作成
            var user = new ApplicationUser();
            user.UserName = Email.Text;
            user.Email = Email.Text;
            user.DispUserName = DispUserName.Text;

            // パスワードを指定し、新たなユーザーを作成
            var result = manager.Create(user, Password.Text);
            if (result.Succeeded == false)
            {
                var error = result.Errors.FirstOrDefault();
                ErrorMessage.Text = error;
                return;
            }

            // 新規ユーザー項目をクリア
            Email.Text = "";
            Password.Text = "";
            DispUserName.Text = "";

            // ユーザー一覧をリフレッシュ
            UserGridView.DataBind();
        }

        protected void UserGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 管理者ロールに属するユーザーなら管理者にチェックする
                var isAdmin = e.Row.FindControl("IsAdmin") as CheckBox;
                var user = e.Row.DataItem as ApplicationUser;
                isAdmin.Checked = manager.IsInRole(user.Id, "Admin");
            }
        }
    }
}
