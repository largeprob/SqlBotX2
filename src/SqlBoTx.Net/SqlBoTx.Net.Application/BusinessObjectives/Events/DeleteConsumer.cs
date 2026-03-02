using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Qdrant.Client.Grpc;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings;
using SqlBoTx.Net.Application.Vectors;
using SqlBoTx.Net.Domain.BusinessObjectives.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Wolverine.Attributes;

namespace SqlBoTx.Net.Application.BusinessObjectives.Events
{
    [Transactional]
    [WolverineHandler]
    public class DeleteConsumer  
    {
        private readonly ILogger<DeleteConsumer> _logger;
        private readonly QdrantVectorService _qdrantVectorService;
        public DeleteConsumer(ILogger<DeleteConsumer> logger, QdrantVectorService qdrantVectorService)
        {
            _logger = logger;
            _qdrantVectorService = qdrantVectorService;
        }

        public async Task Handle(Delete command)
        {
            _logger.LogInformation("删除事件");

            //var collection = await _qdrantVectorService.EnsureCollectionAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective");
            //await collection.DeleteAsync((ulong)command.Id);
            await _qdrantVectorService.VectorClient!.DeleteAsync("business_objective", (ulong)command.Id);

            //var filter = new Filter();
            //filter.Must.Add(new Condition
            //{
            //    Field = new FieldCondition
            //    {
            //        Key = "BusinessObjectiveId",
            //        Match = new Match { Integer = command.Id }
            //    }
            //});
            //await _qdrantVectorService.vectorClient!.DeleteAsync("business_objective_synonyms", filter);


            var filter = new Filter();
            filter.Must.Add(new Condition
            {
                Field = new FieldCondition
                {
                    Key = "MataData.Id",
                    Match = new Match { Integer = command.Id }
                }
            });
            await _qdrantVectorService.VectorClient!.DeleteAsync("business_objective_synonyms", filter);
        }
    }
}
