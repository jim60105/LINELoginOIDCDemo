# LINE Login OpenID Connect Demo Project

這是 LINE 的 OAuth 2.0 OpenID Connect 串接練習專案。

- C# ASP<area>.NET Core 6 MVC 專案
- 使用 `Microsoft.AspNetCore.Authentication.OpenIdConnect` 套件
- Dockerize，包括開發環境和部署環境
- **OIDC實做的重點在 [Program.cs](/Program.cs) !**

## Try it out!

- 在 [LINE Developer Console](https://developers.line.biz/console) 註冊新的 Channel，取得「Channel ID」、「Channel secret」
- 在 「Callback URL」填入 `https://localhost:9001/signin-oidc`
- Git clone

  ```bash
  git clone https://github.com/jim60105/LINELoginOIDCDemo.git
  ```

- Visual Studio 啟動 `LINELoginOIDCDemo.sln`
- 使用套件 `dotnet dev-certs` 產生開發用自簽憑證，匯出，並信任。\
  密碼自己取，在下一步驟用到。參閱[官方文件](https://docs.microsoft.com/zh-tw/dotnet/core/additional-tools/self-signed-certificates-guide#with-dotnet-dev-certs)

  ```bash
  dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\LINELoginOIDCDemo.pfx -p YOURPASSWORD
  dotnet dev-certs https --trust
  ```

- Add User Secrets
  - 「方案總管」→專案上右鍵→「管理使用者密碼」\
    ![2022-06-29 17 22 50](https://user-images.githubusercontent.com/16995691/176410718-740f5f3f-4af2-455a-a8ad-954dfcb18c7e.png)
  - 填入設定\
    ![2022-06-29 17 23 32](https://user-images.githubusercontent.com/16995691/176410721-cfe4a9cb-5c56-4a1e-b9f6-aef6ce0c8825.png)

    ```json
    {
        "OpenIDConnect:ClientSecret": <Channel secret>,
        "OpenIDConnect:ClientId": <Channel ID>,
        "OpenIDConnect:Authority": "https://access.line.me",
        "Kestrel:Certificates:Development:Password": <上一步驟的密碼>
    }
    ```

- 把啟始專案改為「docker-compose」\
  ![2022-06-29 18 07 46](https://user-images.githubusercontent.com/16995691/176411228-b15ab530-dac2-41f8-be2c-4b75d2e95769.png)
- 啟動但不偵錯 (Ctrl+F5)
- 訪問 `https://localhost:9001/` ，拿回來的登入資訊將顯示在 `https://localhost:9001/Home/Privacy`

## 部署指引

- 修改 `docker-compose.yml` ，在 environment 填入 ClientId、ClientSecret
- `docker-compose up -d` 後會在 **9000** port 聽 **http** request
- 請放在 ReverseProxy 後面，接 https 轉後送 http
- 在  [LINE Developer Console](https://developers.line.biz/console) -「Callback URL」填入 `https://你的對外網域/signin-oidc` ，例如我的是 `https://oidcdemo.maki0419.com/signin-oidc`
- 訪問 `https://你的對外網域/`
