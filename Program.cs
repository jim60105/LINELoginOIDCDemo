using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 如何將一個 ASP.NET Core 網站快速加入 LINE Login 功能 (OpenID Connect)
// https://blog.miniasp.com/post/2022/04/08/LINE-Login-with-OpenID-Connect-in-ASPNET-Core
builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = builder.Configuration["OpenIDConnect:Authority"];
                    options.ClientId = builder.Configuration["OpenIDConnect:ClientId"];
                    options.ClientSecret = builder.Configuration["OpenIDConnect:ClientSecret"];
                    options.ResponseType = "code";

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");

                    // LINE Login 不加入以下這一段設定，將無法走完 OpenID Connect 整個流程，最後會驗不過 JWT Token！ 🔥
                    options.Events = new OpenIdConnectEvents()
                    {
                        OnAuthorizationCodeReceived = context =>
                        {
                            context.TokenEndpointRequest?.SetParameter("id_token_key_type", "JWK");
                            return Task.CompletedTask;
                        }
                    };

                    options.SaveTokens = true;
                });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // <-- 加入這行
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .RequireAuthorization(); // <-- 加入這行，這會讓整個網站都需要登入才能用！

app.Run();
