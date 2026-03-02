using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SqlBoTx.Net.ApiService.Dto;
using SqlBoTx.Net.ApiService.SqlPlugin;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Dapper.SqlMapper;
using static System.Net.Mime.MediaTypeNames;

namespace SqlBoTx.Net.ApiService
{
    /// <summary>
    /// minimal API 映射扩展
    /// </summary>
    public static class MapApiExpansion
    {
        extension(WebApplication app) {

            /// <summary>
            /// 系统接口
            /// </summary>
            /// <returns></returns>
            public WebApplication SystemApi()
            {
                // 登录接口，演示使用 Cookie 存储用户信息
                app.MapGet("/login", async (string account, string password, HttpContext context) =>
                {
                    if (account != "admin" || password != "123456")
                    {
                        return Results.BadRequest("用户名或密码错误");
                    }

                    // 构造用户信息
                    var user = new UserInfo
                    {
                        Id = 1,
                        Username = "测试账号",
                        Email = "admin@example.com",
                        FirstName = "largeprob",
                        LastName = "largeprob",
                        Age = 25,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    // 将用户信息序列化存入 Cookie（也可存 Token）
                    var json = JsonSerializer.Serialize(user);

                    context.Response.Cookies.Append("auth_user", json, new CookieOptions
                    {
                        HttpOnly = true,         
                        Secure = true,       
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                    return Results.Ok(user);

                }).WithName("login");

                // 检查用户登录状态接口
                app.MapGet("/checkUser", async (HttpContext context) =>
                {
                    if (context.Request.Cookies.TryGetValue("auth_user", out var json))
                    {
                        var user = JsonSerializer.Deserialize<UserInfo>(json);
                        return Results.Ok(new { data = user });
                    }
                    return Results.Unauthorized(); 
                }).WithName("checkUser");

                return app;
            }
        }
    }
}
