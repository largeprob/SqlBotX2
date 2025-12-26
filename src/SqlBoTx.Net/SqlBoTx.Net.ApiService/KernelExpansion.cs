using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OpenAI;
using SqlBoTx.Net.ApiService.SkKernel;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Security.Cryptography;

namespace SqlBoTx.Net.ApiService
{
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //if (request.Content != null)
            //{
            //    // 打印发送的 JSON 内容
            //    var json = await request.Content.ReadAsStringAsync(cancellationToken);
            //    Console.WriteLine("=== REQUEST JSON ===");
            //    Console.WriteLine(json);
            //    Console.WriteLine("====================");
            //}
            return await base.SendAsync(request, cancellationToken);
        }
    }

    public static class KernelExpansion
    {
        extension(WebApplicationBuilder builder)
        {
            public WebApplicationBuilder AddKernelCompletion()
            {
                //注册模型客户端
                var enableKey = builder.Configuration.GetSection("Models").Get<List<ModleInfo>>();
                foreach (var model in enableKey)
                {

                    var openAIClientCredential = new ApiKeyCredential(model.Key);
                    var openAIClientOption = new OpenAIClientOptions
                    {
                        Endpoint = new Uri(model.Endpoint),
                      
                    };
                    //builder.Services.AddOpenAIChatClient(model.ModelId, new OpenAIClient(openAIClientCredential, openAIClientOption), model.ModelId);


                    if (model.ModelType == ModelType.Chat)
                    {
                        builder.Services.AddOpenAIChatClient(
                             modelId: model.ModelId,
                             apiKey: model.Key,
                             endpoint: new Uri(model.Endpoint),
                             serviceId: model.ModelId,
                             httpClient: new HttpClient(new LoggingHandler(new HttpClientHandler()))
                        );
                    }


                    if (model.ModelType == ModelType.Embedding)
                    {
                        builder.Services.AddEmbeddingGenerator(_ => new OpenAIClient(new ApiKeyCredential(model.Key), new OpenAIClientOptions
                        {
                            Endpoint = new Uri(model.Endpoint)
                        })
                        .GetEmbeddingClient(model.ModelId)
                        .AsIEmbeddingGenerator()
                        );

                        //                        var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()))
                        //                        {
                        //                            BaseAddress = new Uri(model.Endpoint)
                        //                        };
                        //#pragma warning disable SKEXP0010 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                        //                        builder.Services.AddOpenAIEmbeddingGenerator(
                        //                             modelId: model.ModelId,
                        //                             apiKey: model.Key,
                        //                             serviceId: model.ModelId,
                        //                             httpClient: new HttpClient(new LoggingHandler(new HttpClientHandler()))
                        //                        );


                    }

                }

                //注册瞬时Kernel
                builder.Services.AddTransient((serviceProvider) =>
                {
                    return new Kernel(serviceProvider);
                });

                return builder;
            }
        }
    }
}
