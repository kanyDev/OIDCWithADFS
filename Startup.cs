using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

[assembly: OwinStartup(typeof(OIDCWithADFS.Startup))]

namespace OIDCWithADFS
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // SSL 인증서 검증 무시 설정 (개발 환경에서만 사용)
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            // 쿠키 인증 미들웨어 설정
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                LoginPath = new PathString("/Default.aspx") // 로그인 페이지 경로
            });

            // OpenID Connect 미들웨어 설정
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["ClientSecret"],
                Authority = ConfigurationManager.AppSettings["Authority"],
                RedirectUri = ConfigurationManager.AppSettings["RedirectUri"],
                ResponseType = "id_token token", // Access Token도 요청
                Scope = "openid profile email",
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true // 발급자 검증 활성화
                },
                SignInAsAuthenticationType = "Cookies", // SignInAsAuthenticationType 명시적 설정
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context =>
                    {
                        System.Diagnostics.Debug.WriteLine("Authentication failed: " + context.Exception.Message);
                        context.HandleResponse(); // 응답 처리
                        context.Response.Redirect("/Error.aspx"); // 오류 페이지로 리디렉션
                        return Task.FromResult(0);
                    },
                    SecurityTokenValidated = context =>
                    {
                        // 인증이 성공적으로 완료된 후 클레임을 처리할 수 있습니다.
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}