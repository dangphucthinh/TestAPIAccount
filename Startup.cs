using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(WebApplication2.Startup))]

namespace WebApplication2
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCors(CorsOptions.AllowAll);

            //OAuthAuthorizationServerOptions option
            //     = new OAuthAuthorizationServerOptions
            //     {
            //         TokenEndpointPath = new PathString("/token"),
            //         Provider = new ApplicationOAuthProvider(),
            //         AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
            //         AllowInsecureHttp = true,
            //         RefreshTokenProvider = 
            //     };
            //app.UseOAuthAuthorizationServer(option);
            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());


            //
            double tokenTimeout = double.Parse(ConfigurationManager.AppSettings["tokenTimeout"].ToString());
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new OAuthCustomeTokenProvider(), // We will create
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(tokenTimeout),
                AllowInsecureHttp = true,
                RefreshTokenProvider = new OAuthCustomRefreshTokenProvider() // We will create
            };
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}
