// Controllers/HomeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]  // نیاز به احراز هویت
        public IActionResult Secure()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return View(claims);
        }

        [Authorize(Policy = "AdminOnly")]  // نیاز به نقش admin
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CallApi()
        {
            try
            {
                // دریافت access token
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                if (string.IsNullOrEmpty(accessToken))
                {
                    ViewData["ApiData"] = "Access token not found. Please login again.";
                    return View();
                }

                var client = _httpClientFactory.CreateClient();

                // ایجاد درخواست HTTP و تنظیم دستی header
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5004/weather");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // ارسال درخواست
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ViewData["ApiData"] = content;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewData["ApiData"] = $"Error: {response.StatusCode} - {response.ReasonPhrase}\nDetails: {errorContent}";
                }
            }
            catch (HttpRequestException httpEx)
            {
                ViewData["ApiData"] = $"HTTP Request Error: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                ViewData["ApiData"] = $"Exception: {ex.Message}";
            }

            return View();
        }

        [Authorize]
        public IActionResult UserInfo()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Tokens()
        {
            try
            {
                // دریافت توکن‌ها از context
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var idToken = await HttpContext.GetTokenAsync("id_token");
                var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

                var tokens = new
                {
                    // Preview (first 100 characters)
                    AccessToken = accessToken?.Length > 100 ? accessToken.Substring(0, 100) + "..." : accessToken,
                    IdToken = idToken?.Length > 100 ? idToken.Substring(0, 100) + "..." : idToken,
                    RefreshToken = refreshToken,

                    // Full tokens
                    AccessTokenFull = accessToken,
                    IdTokenFull = idToken,

                    // Lengths
                    AccessTokenLength = accessToken?.Length ?? 0,
                    IdTokenLength = idToken?.Length ?? 0
                };

                return View(tokens);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CallUserApi()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                if (string.IsNullOrEmpty(accessToken))
                {
                    ViewData["UserApiData"] = "Access token not found";
                    return View("CallApi");
                }

                var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5004/weather/admin");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ViewData["UserApiData"] = content;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewData["UserApiData"] = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                ViewData["UserApiData"] = $"Exception: {ex.Message}";
            }

            return View("CallApi");
        }

        [Authorize]
        public async Task<IActionResult> RefreshTokens()
        {
            try
            {
                var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

                if (string.IsNullOrEmpty(refreshToken))
                {
                    TempData["RefreshResult"] = "No refresh token available";
                    return RedirectToAction("Tokens");
                }

                var client = _httpClientFactory.CreateClient();

                // Manual refresh token request without IdentityModel
                var requestData = new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "refresh_token"),
                    new("refresh_token", refreshToken),
                    new("client_id", "mvc_client"),
                    new("client_secret", "mvc_secret")
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/connect/token")
                {
                    Content = new FormUrlEncodedContent(requestData)
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON response manually
                    using var jsonDoc = System.Text.Json.JsonDocument.Parse(responseContent);
                    var root = jsonDoc.RootElement;

                    var newAccessToken = root.GetProperty("access_token").GetString();
                    var newRefreshToken = root.GetProperty("refresh_token").GetString();
                    var expiresIn = root.GetProperty("expires_in").GetInt32();

                    // Update the tokens in the current context
                    var authInfo = await HttpContext.AuthenticateAsync();
                    var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(expiresIn);

                    var tokens = new List<AuthenticationToken>
                    {
                        new AuthenticationToken { Name = "access_token", Value = newAccessToken },
                        new AuthenticationToken { Name = "refresh_token", Value = newRefreshToken },
                        new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o") }
                    };

                    var properties = authInfo.Properties;
                    if (properties != null)
                    {
                        properties.StoreTokens(tokens);
                        await HttpContext.SignInAsync(authInfo.Principal, properties);
                    }

                    TempData["RefreshResult"] = "Tokens refreshed successfully!";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["RefreshResult"] = $"Failed to refresh tokens: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["RefreshResult"] = $"Exception during refresh: {ex.Message}";
            }

            return RedirectToAction("Tokens");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    var client = _httpClientFactory.CreateClient();

                    // Manual UserInfo request without SetBearerToken
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/connect/userinfo");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        ViewData["UserInfo"] = content;
                    }
                    else
                    {
                        ViewData["UserInfoError"] = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
                else
                {
                    ViewData["UserInfoError"] = "No access token available";
                }
            }
            catch (Exception ex)
            {
                ViewData["UserInfoError"] = ex.Message;
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CallPublicApi()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://localhost:5004/weather/public");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ViewData["PublicApiData"] = content;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewData["PublicApiData"] = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                ViewData["PublicApiData"] = $"Exception: {ex.Message}";
            }

            return View("CallApi");
        }

        public async Task<IActionResult> Logout()
        {
            // Sign out from the application
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Also sign out from Identity Server (if you want single sign-out)
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}