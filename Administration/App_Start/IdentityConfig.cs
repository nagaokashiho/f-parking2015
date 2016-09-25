﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Administration.Models;
using System.Linq;

namespace Administration
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 電子メールを送信するには、電子メール サービスをここにプラグインします。
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // テキスト メッセージを送信するには、SMS サービスをここにプラグインします。
            return Task.FromResult(0);
        }
    }

    // このアプリケーションで使用されるアプリケーション ユーザー マネージャーを設定します。UserManager は ASP.NET Identity の中で定義されており、このアプリケーションで使用されます。
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // ユーザー名の検証ロジックを設定します
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,         // アルファベットと数字だけがユーザー名に使えるかどうかを指定する
                RequireUniqueEmail = true                       // ユニークなメールアドレスである必要があるかどうかを指定する
            };

            // パスワードの検証ロジックを設定します
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,                             // パスワードに必要な桁数を指定する
                RequireNonLetterOrDigit = false,                // アルファベットでも数字でもない値が必要かどうかを指定する
                RequireDigit = false,                           // 数字が必要かどうかを指定する
                RequireLowercase = false,                       // 小文字英字が必要かどうかを指定する
                RequireUppercase = false,                       // 大文字英字が必要かどうかを指定する
            };

            // 2 要素認証プロバイダーを登録します。このアプリケーションでは、電話とメールをユーザー確認用コード受け取りのステップとして使用します
            // 独自のプロバイダーを作成して、ここにプラグインすることができます。
            manager.RegisterTwoFactorProvider("コードを電話で伝える", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "セキュリティ コードは {0} です"
            });
            manager.RegisterTwoFactorProvider("コードをメールで送信する", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "セキュリティ コード",
                BodyFormat = "セキュリティ コードは {0} です"
            });

            // ユーザー ロックアウトの既定値を設定
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole, string>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store)
            : base(store)
        {
        }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));

            if (!manager.Roles.Any())
            {
                // 初回に管理者ロール（今回は固定）を作成する
                manager.Create(new ApplicationRole
                {
                    Name = "Admin"
                });
            }

            return manager;
        }
    }

}
