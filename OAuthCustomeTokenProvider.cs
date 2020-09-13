using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.IO;
using WebApplication2.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace WebApplication2
{
    public class OAuthCustomeTokenProvider : OAuthAuthorizationServerProvider
    {
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //return Task.Factory.StartNew(() =>
            //{
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            //var userName = context.UserName;
            //var password = context.Password;
            //var userService = new UserService();
            //var user = userService.Validate(userName, password);
            var user = await manager.FindAsync(context.UserName, context.Password);
            if (user != null)
            {
                var claims = new List<Claim>()
            {
                new Claim("Username", user.UserName),
                new Claim("Email", user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("LoggedOn", DateTime.Now.ToString())
            };
                //        foreach (var role in user.Roles)
                //            claims.Add(new Claim(ClaimTypes.Role, role));

                var data = new Dictionary<string, string>
            {
                { "UserName", user.UserName },
                    {"Email", user.Email },
                    {"FirstName", user.FirstName },
                    {"LastName", user.LastName },
                    
                //{ "roles", string.Join(",", user.Roles)}
            };
                var properties = new AuthenticationProperties(data);

                ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims,
                    Startup.OAuthOptions.AuthenticationType);

                var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
            }
            else
            {
                context.SetError("invalid_grant", "Either email or password is incorrect");
            }

            //});
        }
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }


        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
    }
}