using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;

namespace SqlBoTx.Net.ApiService
{
    public static class KernelExpansion
    {
        extension(WebApplicationBuilder builder)
        {
            public WebApplicationBuilder AddKernelCompletion()
            {
                var enableKey = builder.Configuration.GetSection("Agent")["Enable"];
                var chatAI = builder.Configuration.GetSection("Agent:" + enableKey);
                if (chatAI == null) throw new Exception("AI Model config is null");

                #region 注册IChatCompletionService
                string modelId = chatAI["ModelId"]!;
                string apiKey = chatAI["Key"]!;
                string endpoint = chatAI["Endpoint"]!;

                var openAIClientCredential = new ApiKeyCredential(apiKey);
                var openAIClientOption = new OpenAIClientOptions
                {
                    Endpoint = new Uri(endpoint),
                };

                builder.Services.AddOpenAIChatCompletion(modelId, new OpenAIClient(openAIClientCredential, openAIClientOption));
                #endregion


                //注册瞬时Kernel核心
                builder.Services.AddTransient((serviceProvider) =>
                {
                    return new Kernel(serviceProvider);
                });

                return builder;
            }
        }
    }
}
