# NexFlow.Knowledge

## Visión del proyecto

**NexFlow.Knowledge** es un proyecto de aprendizaje y experimentación orientado al desarrollo de soluciones de **Inteligencia Artificial generativa utilizando RAG (Retrieval-Augmented Generation) sobre .NET**.

El proyecto busca comprender y construir, desde cero, los principales componentes que intervienen en un sistema RAG:

- Procesamiento de documentos.
- Extracción y preparación de texto.
- Chunking.
- Embeddings.
- Bases de datos vectoriales.
- Búsqueda semántica.
- Búsqueda textual.
- Búsqueda híbrida.
- Ranking y scoring de resultados.
- Construcción de contexto.
- Generación de respuestas mediante un LLM.
- Evaluación de la calidad de recuperación.

El objetivo no es construir un producto comercial completo, sino desarrollar una solución con una estructura cercana a un proyecto profesional que permita **comprender profundamente cómo funciona un sistema RAG y cómo integrarlo dentro de una aplicación Backend en .NET**.

---

# Objetivo principal

Construir una API REST en **.NET 10** capaz de procesar documentos, almacenarlos junto con sus representaciones vectoriales y responder preguntas en lenguaje natural utilizando información recuperada desde dichos documentos.

El sistema debe evitar que el modelo genere respuestas basadas únicamente en conocimiento propio cuando la información necesaria no se encuentre en los documentos proporcionados.

---

# Objetivos de aprendizaje

El proyecto busca comprender de forma práctica:

## RAG

- Qué es Retrieval-Augmented Generation.
- Cuál es el flujo completo de un sistema RAG.
- Diferencia entre recuperación y generación.
- Importancia de la calidad de recuperación para la respuesta final.

## Embeddings

- Qué es un embedding.
- Cómo se genera.
- Cómo representar texto mediante vectores.
- Cómo comparar similitud entre vectores.

## Recuperación

- Búsqueda semántica.
- Búsqueda textual.
- Búsqueda híbrida.
- Extracción de términos relevantes.
- Scoring.
- Ranking.
- Thresholds.
- Selección de contexto.

## LLM

- Integración con modelos locales.
- Construcción de prompts.
- Uso de contexto recuperado.
- Restricción de respuestas al contexto disponible.
- Diferencia entre recuperación de información y generación de lenguaje.

## Ingeniería de software

El proyecto también busca reforzar:

- Clean Architecture.
- SOLID.
- Dependency Injection.
- Separación de responsabilidades.
- Diseño desacoplado.
- Abstracciones.
- Testing.
- Manejo de errores.
- Persistencia con Entity Framework Core.
- Diseño de APIs REST.

---

# Caso de uso

El sistema permite cargar documentos y posteriormente realizar preguntas utilizando lenguaje natural.

Los documentos pueden pertenecer a diferentes dominios.

Por ejemplo:

- Normativa.
- Manuales técnicos.
- Documentación empresarial.
- Políticas internas.
- Procedimientos.
- Documentación de software.
- Documentos académicos.

El sistema no debe depender de un dominio específico.

Por esta razón, **los documentos de tránsito utilizados durante las primeras pruebas son solamente un caso de prueba y no una restricción del proyecto**.

---

# Principio fundamental

El sistema debe seguir el siguiente principio:

> **La respuesta debe estar fundamentada en la información recuperada desde los documentos.**

Cuando el contexto recuperado no contenga información suficiente, el sistema deberá indicarlo en lugar de inventar una respuesta.

---

# Alcance

El proyecto contempla:

## Procesamiento de documentos

- Carga de documentos.
- Almacenamiento.
- Extracción de texto.
- Normalización.
- Chunking.
- Persistencia de fragmentos.

## Recuperación

- Generación de embeddings.
- Búsqueda vectorial.
- Búsqueda textual.
- Búsqueda híbrida.
- Scoring de candidatos.
- Ranking de resultados.
- Selección de fragmentos relevantes.

## Generación

- Construcción de contexto.
- Construcción de prompts.
- Integración con LLM.
- Generación de respuestas.
- Retorno de referencias utilizadas.

## API

Endpoints principales:

```text
POST /api/knowledge/search
POST /api/knowledge/ask