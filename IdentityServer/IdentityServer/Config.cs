using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

public static class Config
{
    // کاربران تستی
    public static List<TestUser> GetTestUsers()
    {
        return new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim("name", "Alice Smith"),
                    new Claim("given_name", "Alice"),
                    new Claim("family_name", "Smith"),
                    new Claim("email", "alice@example.com"),
                    new Claim("email_verified", "true"),
                    new Claim("website", "http://alice.com"),
                    new Claim("role", "admin")
                }
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "bob",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim("name", "Bob Johnson"),
                    new Claim("given_name", "Bob"),
                    new Claim("family_name", "Johnson"),
                    new Claim("email", "bob@example.com"),
                    new Claim("email_verified", "true"),
                    new Claim("website", "http://bob.com"),
                    new Claim("role", "user")
                }
            }
        };
    }

    // منابع هویت
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("roles", "User roles", new List<string> { "role" })
        };
    }

    // منابع API
    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
        {
            new ApiResource("weather_api", "Weather API")
            {
                Scopes = { "weather_api.read", "weather_api.write" },
                UserClaims = { "role", "email" }
            },
            new ApiResource("user_api", "User Management API")
            {
                Scopes = { "user_api.read", "user_api.manage" },
                UserClaims = { "role", "email", "name" }
            }
        };
    }

    // scopeهای API
    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("weather_api.read", "Read weather data"),
            new ApiScope("weather_api.write", "Write weather data"),
            new ApiScope("user_api.read", "Read user data"),
            new ApiScope("user_api.manage", "Manage users")
        };
    }

    // کلاینت‌ها
    public static IEnumerable<Client> GetClients()
    {
        return new List<Client>
    {
        // کلاینت MVC - اصلاح شده
        new Client
        {
            ClientId = "mvc_client",
            ClientName = "MVC Client Application",
            ClientSecrets = { new Secret("mvc_secret".Sha256()) },
            
            // اضافه کردن ResourceOwnerPassword به AllowedGrantTypes
            AllowedGrantTypes = GrantTypes.CodeAndClientCredentials, // یا از لیست زیر استفاده کنید
            
            RequirePkce = true,
            RequireClientSecret = true,

            RedirectUris = {
                "https://localhost:5002/signin-oidc"
            },
            PostLogoutRedirectUris = {
                "https://localhost:5002/signout-callback-oidc"
            },

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                "roles",
                "weather_api.read",
                "user_api.read"
            },

            AllowOfflineAccess = true,
            AccessTokenLifetime = 3600,
            IdentityTokenLifetime = 3600
        },

        // کلاینت Web API
        new Client
        {
            ClientId = "api_client",
            ClientName = "API Client",
            ClientSecrets = { new Secret("api_secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.ClientCredentials,

            AllowedScopes =
            {
                "weather_api.read",
                "weather_api.write"
            },

            AccessTokenLifetime = 3600
        }
        // اضافه کردن این کلاینت جدید به متد GetClients
,new Client
{
    ClientId = "postman_test",
    ClientName = "Postman Test Client",
    ClientSecrets = { new Secret("postman_secret".Sha256()) },

    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,

    AllowedScopes =
    {
        IdentityServerConstants.StandardScopes.OpenId,
        IdentityServerConstants.StandardScopes.Profile,
        "weather_api.read",
        "weather_api.write",
        "user_api.read",
        "roles"
    },

    AccessTokenLifetime = 3600,
    AllowOfflineAccess = true
}
    };
    }
}