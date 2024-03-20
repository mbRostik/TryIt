using Duende.IdentityServer.Models;

namespace IdentityServer.WebApi
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
           new IdentityResource[]
           {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
           };

        public static IEnumerable<ApiScope> ApiScopes =>
           new ApiScope[]
           {
               new ApiScope("Users.WebApi.Scope"),
               

               new ApiScope("Chats.WebApi.Scope"),

               new ApiScope("Subscriptions.WebApi.Scope"),

               new ApiScope("Notifications.WebApi.Scope"),

               new ApiScope("Reports.WebApi.Scope"),

               new ApiScope("Posts.WebApi.Scope"),

               new ApiScope("Aggregator.WebApi.Scope"),

           };

        public static IEnumerable<ApiResource> ApiResources => new[] {
            new ApiResource("Users.WebApi")
             {
                 Scopes=new List<string>{ "Users.WebApi.Scope"},
                 ApiSecrets=new List<Secret>{new Secret("Users.WebApi.Secret".Sha256())},
             },
            new ApiResource("Posts.WebApi")
             {
                 Scopes=new List<string>{ "Posts.WebApi.Scope"},
                 ApiSecrets=new List<Secret>{new Secret("Posts.WebApi.Secret".Sha256())},
             },
            new ApiResource("Chats.WebApi")
             {
                 Scopes=new List<string>{ "Chats.WebApi.Scope" },
                 ApiSecrets=new List<Secret>{new Secret("Chats.WebApi.Secret".Sha256())},
             },

            new ApiResource("Notifications.WebApi")
             {
                 Scopes=new List<string>{ "Notifications.WebApi.Scope"},
                 ApiSecrets=new List<Secret>{new Secret("Notifications.WebApi.Secret".Sha256())},
             },

            new ApiResource("Subscriptions.WebApi")
             {
                 Scopes=new List<string>{ "Subscriptions.WebApi.Scope"},
                 ApiSecrets=new List<Secret>{new Secret("Subscriptions.WebApi.Secret".Sha256())},
             },

            new ApiResource("Reports.WebApi")
             {
                 Scopes=new List<string>{ "Reports.WebApi.Scope"},
                 ApiSecrets=new List<Secret>{new Secret("Reports.WebApi.Secret".Sha256())},
             },
            new ApiResource("Aggregator.WebApi")
             {
                Scopes=new List<string>{ "Aggregator.WebApi.Scope"},
                 ApiSecrets=new List<Secret>{new Secret("Aggregator.WebApi.Secret".Sha256())},
             }
        };

        public static IEnumerable<Client> Clients =>
           new Client[]
           {
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = {new Secret("OnlyUserKnowsThisSecret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:5173/signin-oidc" },
                    FrontChannelLogoutUri="https://localhost:5173/signout-oidc",
                    PostLogoutRedirectUris={ "https://localhost:5173/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = 
                    {"openid", "profile", "Users.WebApi.Scope",
                        "Chats.WebApi.Scope",
                        "Subscriptions.WebApi.Scope",
                        "Notifications.WebApi.Scope",
                        "Reports.WebApi.Scope", "Posts.WebApi.Scope",  "Aggregator.WebApi.Scope"},
                    RequireConsent = true,
                    RequirePkce=true,
                    AllowPlainTextPkce=true,
                    AllowedCorsOrigins = { "https://localhost:5173" }
                }
           };
    }
}