using MassTransit;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings;
using SqlBoTx.Net.Application.Vectors;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.BusinessObjectives.Events;
using Wolverine.Attributes;

namespace SqlBoTx.Net.Application.BusinessObjectives.Events
{
    [Transactional]
    [WolverineHandler]
    public class UpsertConsumer
    {
        private readonly ILogger<UpsertConsumer> _logger;
        private readonly QdrantVectorService _qdrantVectorService;
        public UpsertConsumer(ILogger<UpsertConsumer> logger, QdrantVectorService qdrantVectorService)
        {
            _logger = logger;
            _qdrantVectorService = qdrantVectorService;
        }

        public async Task Handle(Upsert command)
        {
            _logger.LogInformation("新增事件 {Id} {BusinessName}", command.Id, command.BusinessName);
            var collection = await _qdrantVectorService.EnsureCollectionAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective");
            await collection.UpsertAsync(new BusinessObjectiveEmbeddingModel
            {
                Id = (ulong)command.Id,
                Embedding = await _qdrantVectorService.Embedding(command.BusinessName),
            });

            if (string.IsNullOrWhiteSpace(command.Synonyms))
            {
                var list = new List<BusinessObjectiveSynonymEmbeddingModel>();
                var collection2 = await _qdrantVectorService.EnsureCollectionAsync<ulong, BusinessObjectiveSynonymEmbeddingModel>("business_objective_synonyms");
                foreach (var item in command.Synonyms.Split(","))
                {
                    list.Add(new BusinessObjectiveSynonymEmbeddingModel
                    {
                        Id = Guid.CreateVersion7(),
                        MataData = item,
                        ObjectiveMetaDataId = command.Id,
                        ObjectiveMetaDataName = command.BusinessName,
                        ObjectiveMetaDataDescription = command.Synonyms,
                        Embedding = await _qdrantVectorService.Embedding(item),
                    });
                }
                await collection2.UpsertAsync(list);
            }
         
        }
    }
}
