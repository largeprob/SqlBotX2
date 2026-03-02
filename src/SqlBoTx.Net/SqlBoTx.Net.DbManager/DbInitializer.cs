using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Application.Vectors;
using SqlBoTx.Net.DbManager.SeedDatas;
using SqlBoTx.Net.Domain.TableFields;
using SqlBoTx.Net.Domain.TableStructures;
using SqlBoTx.Net.EFCore;
using SqlBoTx.Net.Share.Helpers;
using System.Diagnostics;

namespace SqlBoTx.Net.DbManager
{
    public class DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SqlBotxDBContext>();
            await InitializeDatabaseAsync(dbContext, cancellationToken);
        }

        public async Task InitializeDatabaseAsync(SqlBotxDBContext dbContext, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            //执行数据库迁移
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);

            logger.LogInformation("数据库迁移完成，花费 {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);

            await SeedDataAsync(dbContext, cancellationToken);
        }



        /// <summary>
        /// 种子数据
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SeedDataAsync(SqlBotxDBContext dbContext, CancellationToken cancellationToken)
        {
            logger.LogInformation("开始初始化种子数据");

            await new LocalDb_CRM(dbContext, serviceProvider.GetRequiredService<QdrantVectorService>()).RunAsync();

            logger.LogInformation("种子数据完成");
        }


 
    }

}

