using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Administration.Models;

namespace Administration.Models
{
    // User クラスに複数のプロパティを追加することによって、ユーザーの User データを追加できます。詳細については、http://go.microsoft.com/fwlink/?LinkID=317594 を参照してください。
    public class ApplicationUser : IdentityUser
    {
        public string DispUserName { get; set; }        // オリジナル

        public ClaimsIdentity GenerateUserIdentity(ApplicationUserManager manager)
        {
            // authenticationType は、CookieAuthenticationOptions.AuthenticationType に定義されている種類と一致する必要があります
            var userIdentity = manager.CreateIdentity(this, DefaultAuthenticationTypes.ApplicationCookie);
            // カスタム ユーザー要求をここに追加します
            return userIdentity;
        }

        public Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            return Task.FromResult(GenerateUserIdentity(manager));
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string pRoleName) : base(pRoleName)
        {
        }
    }

    /// <summary>
    /// What's New テーブル用クラス
    /// </summary>
    public class WhatsNew
    {
        public int id { get; set; }
        public string Division { get; set; }
        public DateTime PublicationDate { get; set; }
        public int No { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public bool ImportantFlag { get; set; }
        public bool OpenFlag { get; set; }
        public int NewDays { get; set; }
        public int OpenDays { get; set; }
    }

    /// <summary>
    /// 会議室マスタ用クラス
    /// </summary>
    public class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RyakuName { get; set; }
    }

    /// <summary>
    /// 予約テーブル用クラス
    /// </summary>
    public class Reservation
    {
        public int Id { get; set; }
        public string RoomId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Comment { get; set; }
        public string UserId { get; set; }
        public bool DeleteFlag { get; set; }
        public string AddUserId { get; set; }
        public string EditUserId { get; set; }
        public DateTime AddDateTime { get; set; }
        public DateTime LastEditDateTime { get; set; }
    }

    /// <summary>
    /// 予約表示用クラス（一覧）
    /// </summary>
    public class ReservationList
    {
        public string RoomId;
        public string RoomName;
        public string RoomRyakuName;
        public Reservation[] Reservations;
    }
    /// <summary>
    /// 予約表示用クラス（会議室）
    /// </summary>
    public class ReservationRoom
    {
        public DateTime ReservationDate;
        public Reservation[] Reservations;
    }
    
    /// <summary>
    /// エラー情報用クラス
    /// </summary>
    public class ErrorInfo
    {
        public string Sql;
        public string RetMessage;
        public string RetStackTrace;
        public string RetString;
    }
}

#region ヘルパー
namespace Administration
{
    public static class IdentityHelper
    {
        // 外部ログインをリンクするときに XSRF 用に使用します
        public const string XsrfKey = "XsrfId";

        public const string ProviderNameKey = "providerName";
        public static string GetProviderNameFromRequest(HttpRequest request)
        {
            return request.QueryString[ProviderNameKey];
        }

        public const string CodeKey = "code";
        public static string GetCodeFromRequest(HttpRequest request)
        {
            return request.QueryString[CodeKey];
        }

        public const string UserIdKey = "userId";
        public static string GetUserIdFromRequest(HttpRequest request)
        {
            return HttpUtility.UrlDecode(request.QueryString[UserIdKey]);
        }

        public static string GetResetPasswordRedirectUrl(string code, HttpRequest request)
        {
            var absoluteUri = "/Account/ResetPassword?" + CodeKey + "=" + HttpUtility.UrlEncode(code);
            return new Uri(request.Url, absoluteUri).AbsoluteUri.ToString();
        }

        public static string GetUserConfirmationRedirectUrl(string code, string userId, HttpRequest request)
        {
            var absoluteUri = "/Account/Confirm?" + CodeKey + "=" + HttpUtility.UrlEncode(code) + "&" + UserIdKey + "=" + HttpUtility.UrlEncode(userId);
            return new Uri(request.Url, absoluteUri).AbsoluteUri.ToString();
        }

        private static bool IsLocalUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }

        public static void RedirectToReturnUrl(string returnUrl, HttpResponse response)
        {
            if (!String.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            else
            {
                response.Redirect("~/");
            }
        }
    }
}
#endregion
