using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Owin.Security;
using System.Web;

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

            // 애플리케이션을 구성하는 방법에 대한 자세한 내용은 https://go.microsoft.com/fwlink/?LinkID=316888을 참조하세요.

            // 쿠키 인증 미들웨어 설정
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                LoginPath = new PathString("/Home/Login") // 로그인 페이지 경로
            });

            // OpenID Connect 미들웨어 설정
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["ClientId"],
                Authority = ConfigurationManager.AppSettings["Authority"],
                RedirectUri = ConfigurationManager.AppSettings["RedirectUri"],

                // ID Token 요청 (사용자 인증)
                ResponseType = "id_token code", // 'id_token'과 'code' 모두 요청

                // Access Token을 요청하려면 추가로 'scope'에 'openid profile email'을 설정
                Scope = "openid profile email api_access", // api_access는 보호된 API에 대한 접근을 의미

                Resource = ConfigurationManager.AppSettings["Resource"],

                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true
                },

                SignInAsAuthenticationType = "Cookies",  // SignInAsAuthenticationType 명시적 설정

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context =>
                    {
                        System.Diagnostics.Debug.WriteLine("Authentication failed: " + context.Exception.Message);
                        context.HandleResponse();
                        context.Response.Redirect("/Home/Error");
                        return System.Threading.Tasks.Task.FromResult(0);
                    },

                    // 인증 성공 후 Access Token을 요청하고 API에 대한 호출을 처리
                    AuthorizationCodeReceived = async context =>
                    {
                        var code = context.Code;

                        using (var client = new HttpClient())
                        {
                            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
                            {
                                { "client_id", ConfigurationManager.AppSettings["ClientId"] },
                                { "client_secret", ConfigurationManager.AppSettings["ClientSecret"] },
                                { "code", code },
                                { "redirect_uri", ConfigurationManager.AppSettings["RedirectUri"] },
                                { "grant_type", "authorization_code" }
                            });

                            try
                            {
                                var tokenResponse = await client.PostAsync(ConfigurationManager.AppSettings["TokenEndpoint"], tokenRequest);
                                tokenResponse.EnsureSuccessStatusCode();

                                var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();
                                var accessToken = ExtractAccessTokenFromResponse(tokenResponseString);
                                System.Diagnostics.Debug.WriteLine($"Access Token: {accessToken}");

                                // 세션에 액세스 토큰 저장
                                HttpContext.Current.Session["access_token"] = accessToken;

                                // AuthenticationResponseGrant 생성
                                var identity = context.AuthenticationTicket.Identity;
                                var properties = new AuthenticationProperties
                                {
                                    AllowRefresh = true,
                                    IssuedUtc = DateTime.UtcNow,
                                    ExpiresUtc = DateTime.UtcNow.AddHours(1) // 토큰 유효기간 설정
                                };

                                // AuthenticationResponseGrant 설정
                                context.OwinContext.Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(identity, properties);

                                // 토큰 저장 여부 확인
                                var accessTokenInSession = HttpContext.Current.Session["access_token"];
                                System.Diagnostics.Debug.WriteLine($"Stored Access Token in Session: {accessTokenInSession}");
                            }
                            catch (HttpRequestException ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Token request failed: {ex.Message}");
                            }
                        }
                    }

                }
            });
        }

        // 토큰 응답에서 Access Token을 추출하는 메서드
        private string ExtractAccessTokenFromResponse(string responseString)
        {
            // JSON 응답을 JObject로 파싱
            var jsonResponse = JObject.Parse(responseString);

            // Access Token을 추출하여 반환
            var accessToken = jsonResponse["access_token"]?.ToString();

            return accessToken;
        }
    }
}
