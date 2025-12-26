using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.SemanticKernel;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    /// <summary>
    ///  Kernel 工厂
    /// </summary>
    public class KernelFactory
    {
        private IConfiguration _configuration;
        public KernelFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static Kernel CreateKernel(IServiceProvider serviceProvider)
        {
            var builder = Kernel.CreateBuilder();
            //builder.AddOpenAIChatClient(modelId, endpoint, apiKey);
            //builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
            //builder.Plugins.AddFromType<TimePlugin>();
            Kernel kernel = builder.Build();
            return kernel;
        }
    }
}
