﻿using Duende.IdentityServer.Models;

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
               new ApiScope("Users.WebApi.read"),
               new ApiScope("Users.WebApi.write"),

               new ApiScope("Chats.WebApi.read"),
               new ApiScope("Chats.WebApi.write"),

               new ApiScope("Subscriptions.WebApi.read"),
               new ApiScope("Subscriptions.WebApi.write"),

               new ApiScope("Notifications.WebApi.read"),
               new ApiScope("Notifications.WebApi.write"),

               new ApiScope("Reports.WebApi.read"),
               new ApiScope("Reports.WebApi.write"),

           };

        public static IEnumerable<ApiResource> ApiResources => new[] {
            new ApiResource("Users.WebApi")
             {
                 Scopes=new List<string>{ "Users.WebApi.read", "Users.WebApi.write" },
                 ApiSecrets=new List<Secret>{new Secret("Users.WebApi.Secret".Sha256())},
             },

            new ApiResource("Chats.WebApi")
             {
                 Scopes=new List<string>{ "Chats.WebApi.read", "Chats.WebApi.write" },
                 ApiSecrets=new List<Secret>{new Secret("Chats.WebApi.Secret".Sha256())},
             },

            new ApiResource("Notifications.WebApi")
             {
                 Scopes=new List<string>{ "Notifications.WebApi.read", "Notifications.WebApi.write" },
                 ApiSecrets=new List<Secret>{new Secret("Notifications.WebApi.Secret".Sha256())},
             },

            new ApiResource("Subscriptions.WebApi")
             {
                 Scopes=new List<string>{ "Subscriptions.WebApi.read", "Subscriptions.WebApi.write" },
                 ApiSecrets=new List<Secret>{new Secret("Subscriptions.WebApi.Secret".Sha256())},
             },

            new ApiResource("Reports.WebApi")
             {
                 Scopes=new List<string>{ "Reports.WebApi.read", "Reports.WebApi.write" },
                 ApiSecrets=new List<Secret>{new Secret("Reports.WebApi.Secret".Sha256())},
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
                    RedirectUris = { "" },
                    FrontChannelLogoutUri="",
                    PostLogoutRedirectUris={ "" },
                    AllowOfflineAccess = true,
                    AllowedScopes = {"openid", "profile"},
                    RequireConsent = true,
                    RequirePkce=true,
                    AllowPlainTextPkce=true
                },


                new Client
                {
                    ClientId = "moderator",
                    ClientSecrets = {new Secret("OnlyAdminKnowsThisSecret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "" },
                    FrontChannelLogoutUri="",
                    PostLogoutRedirectUris={ "" },
                    AllowOfflineAccess = true,
                    AllowedScopes = {"openid", "profile"},
                    RequireConsent = true,
                    RequirePkce=true,
                    AllowPlainTextPkce=true
                }
           };
    }
}