using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexFlow.Knowledge.Application.Abstractions.AI;
using Pgvector;

namespace NexFlow.Knowledge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmbeddingTestController : ControllerBase
    {
        private readonly IEmbeddingGenerator _embeddingGenerator;

        public EmbeddingTestController(IEmbeddingGenerator embeddingGenerator)
        {
            _embeddingGenerator = embeddingGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> Test(
        CancellationToken cancellationToken)
        {
            const string question =
                "¿Cuál es la sanción por conducir un vehículo a velocidad superior a la máxima permitida?";

            const string exactC29 =
                "C.29. Conducir un vehículo a velocidad superior a la máxima permitida.";

            const string chunk250 =
                "C.26. Transitar en vehículos de 3.5 o más toneladas por el carril izquierdo de la vía cuando hubiere más de un carril. " +
                "C.27. Conducir un vehículo cuya carga o pasajeros obstruyan la visibilidad del conductor hacia el frente, atrás o costados. " +
                "C.28. Hacer uso de dispositivos propios de vehículos de emergencia. " +
                "C.29. Conducir un vehículo a velocidad superior a la máxima permitida. " +
                "C.30. No atender una señal de ceda el paso. " +
                "C.31. No acatar las señales o requerimientos impartidos por los agentes de tránsito.";

            const string chunk8 =
                "máximas por carril. Bahía de estacionamiento: Parte complementaria de la estructura de la vía utilizada como zona de transición entre la calzada y el andén, destinada al estacionamiento de vehículos. " +
                "Barrera para control vehicular: Dispositivo dotado de punzones pinchallantas para uso en retenes y puesto de control de las fuerzas militares, la Policía Nacional, las autoridades de tránsito y transporte. " +
                "Berma: Parte de la estructura de la vía, destinada al soporte lateral de la calzada para el tránsito de peatones, semovientes y ocasionalmente al estacionamiento de vehículos de emergencia. " +
                "Bicicleta: Vehículo no motorizado de dos o más ruedas en línea.";

            var questionEmbedding = await _embeddingGenerator.GenerateAsync(
                question,
                cancellationToken);

            var c29Embedding = await _embeddingGenerator.GenerateAsync(
                exactC29,
                cancellationToken);

            var chunk250Embedding = await _embeddingGenerator.GenerateAsync(
                chunk250,
                cancellationToken);

            var chunk8Embedding = await _embeddingGenerator.GenerateAsync(
                chunk8,
                cancellationToken);

            var results = new[]
            {
            new
            {
                Name = "C29 exacto",
                Similarity = CosineSimilarity(
                    questionEmbedding,
                    c29Embedding)
            },
            new
            {
                Name = "Chunk 250",
                Similarity = CosineSimilarity(
                    questionEmbedding,
                    chunk250Embedding)
            },
            new
            {
                Name = "Chunk 8",
                Similarity = CosineSimilarity(
                    questionEmbedding,
                    chunk8Embedding)
            }
        };

            return Ok(new
            {
                Question = question,
                Results = results
                    .OrderByDescending(x => x.Similarity)
            });
        }

        private static double CosineSimilarity(
            Vector a,
            Vector b)
        {
            var valuesA = a.ToArray();
            var valuesB = b.ToArray();

            if (valuesA.Length != valuesB.Length)
            {
                throw new InvalidOperationException(
                    $"Los vectores tienen dimensiones diferentes: " +
                    $"{valuesA.Length} y {valuesB.Length}.");
            }

            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (var i = 0; i < valuesA.Length; i++)
            {
                dotProduct += valuesA[i] * valuesB[i];
                magnitudeA += valuesA[i] * valuesA[i];
                magnitudeB += valuesB[i] * valuesB[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
                throw new InvalidOperationException(
                    "No se puede calcular la similitud con un vector de magnitud cero.");

            return dotProduct /
                   (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }
    }
}
