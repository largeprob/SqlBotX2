using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OpenAI;
using Qdrant.Client;
using SqlBoTx.Net.ApiService.Dto;
using SqlBoTx.Net.ApiService.SqlPlugin;

namespace SqlBoTx.Net.ApiService.Controllers
{
    public class Hotel
    {
        [VectorStoreKey]
        public ulong HotelId { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public string HotelName { get; set; }

        [VectorStoreData(IsFullTextIndexed = true)]
        public string Description { get; set; }

        [VectorStoreVector(Dimensions: 64, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public string[] Tags { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class RAGDemoController : ControllerBase
    {
        private readonly Kernel _kernel;

        public RAGDemoController(Kernel kernel)
        {
            _kernel = kernel;
        }



        /// <summary>
        /// Index1
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task Index1()
        {
            var vectorStore = new QdrantVectorStore(new QdrantClient("110.41.187.122"), ownsClient: true);
            var collection = vectorStore.GetCollection<ulong, Hotel>("skhotels");

            await collection.EnsureCollectionExistsAsync();

            string descriptionText = "A place where everyone can be happy.";
            ulong hotelId = 1;

            await collection.UpsertAsync(new Hotel
            {
                HotelId = hotelId,
                HotelName = "Hotel Happy",
                Description = descriptionText,
                DescriptionEmbedding = await GenerateEmbeddingAsync(descriptionText),
                Tags = new[] { "luxury", "pool" }
            });

            Hotel? retrievedHotel = await collection.GetAsync(hotelId);
        }

        public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string textToVectorize)
        {
            var embeddingGenerator = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            var embedding = await embeddingGenerator.GenerateAsync(textToVectorize,new EmbeddingGenerationOptions
            {
                Dimensions = 64,
            });
            return embedding.Vector;
        }
    }
}
