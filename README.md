# Project Charter

# NexFlow.Knowledge

## Visión del proyecto

NexFlow.Knowledge es un proyecto de aprendizaje orientado al desarrollo de soluciones de Inteligencia Artificial utilizando RAG (Retrieval-Augmented Generation) sobre .NET.

El propósito no es construir un producto comercial completo, sino desarrollar una aplicación con calidad profesional que permita comprender el funcionamiento de un sistema RAG desde cero y demostrar experiencia práctica durante entrevistas técnicas.

El proyecto debe mantener un equilibrio entre buenas prácticas de ingeniería y simplicidad, evitando la sobreingeniería sin sacrificar una arquitectura limpia y mantenible.

---

# Objetivos

## Objetivo principal

Desarrollar una API REST en .NET 10 capaz de procesar documentos relacionados con las normas de tránsito, almacenarlos en una base vectorial y responder preguntas utilizando un LLM mediante la técnica Retrieval-Augmented Generation.

---

## Objetivos de aprendizaje

Al finalizar el proyecto debo comprender completamente:

* Qué es un sistema RAG.
* Cómo funciona un LLM.
* Qué son los embeddings.
* Cómo se generan.
* Cómo funciona una base de datos vectorial.
* Qué es la búsqueda semántica.
* Cómo construir prompts utilizando contexto.
* Cómo integrar IA dentro de aplicaciones .NET.
* Cómo diseñar una solución desacoplada para diferentes proveedores de IA.

Además, el proyecto debe reforzar los conocimientos adquiridos durante el desarrollo de NexFlow respecto a:

* Clean Architecture.
* Dependency Injection.
* SOLID.
* Separación de responsabilidades.
* Diseño desacoplado.
* Organización de soluciones empresariales.

---

# Caso de uso

El sistema permitirá cargar documentos oficiales relacionados con las normas de tránsito colombianas.

Posteriormente el usuario podrá realizar preguntas utilizando lenguaje natural.

Ejemplos:

* ¿Qué documentos debo portar para conducir una motocicleta?
* ¿Cuál es la multa por conducir sin SOAT?
* ¿Cada cuánto debe realizarse la revisión técnico-mecánica?
* ¿Qué significa una línea amarilla continua?
* ¿Cuáles son las obligaciones de un conductor?

Las respuestas deberán construirse únicamente utilizando la información encontrada dentro de los documentos cargados.

Siempre que sea posible se indicará la fuente utilizada.

---

# Alcance del MVP

El proyecto incluirá únicamente:

* Carga de documentos PDF.
* Extracción de texto.
* Chunking.
* Generación de embeddings.
* Almacenamiento en una base vectorial.
* Búsqueda semántica.
* Construcción del contexto.
* Generación de respuestas mediante un LLM.
* Referencias del documento utilizado.

---

# Fuera del alcance

No se implementará:

* Frontend.
* Login.
* Usuarios.
* Roles.
* Permisos.
* Microservicios.
* Kubernetes.
* Eventos de dominio.
* CQRS completo.
* Arquitectura distribuida.
* Funcionalidades que no aporten directamente al aprendizaje de RAG.

---

# Arquitectura

Se utilizará la misma filosofía arquitectónica implementada en NexFlow.

El objetivo es reforzar el aprendizaje utilizando un dominio completamente diferente.

Sin embargo, se evitará agregar capas o patrones que no aporten valor al proyecto.

La solución estará organizada en:

* NexFlow.Knowledge.Api
* NexFlow.Knowledge.Application
* NexFlow.Knowledge.Domain
* NexFlow.Knowledge.Infrastructure

Cada proyecto deberá mantener una única responsabilidad.

---

# Principios del proyecto

Durante todo el desarrollo seguiremos las siguientes reglas:

* El aprendizaje es más importante que terminar rápido.
* Cada decisión técnica debe tener una justificación.
* No agregar complejidad innecesaria.
* Mantener el código limpio.
* Aplicar SOLID cuando aporte valor.
* Mantener una separación clara de responsabilidades.
* Diseñar pensando en la extensibilidad.
* Favorecer la composición sobre el acoplamiento.
* Evitar dependencias innecesarias entre capas.

Antes de agregar una nueva abstracción siempre responderemos:

"¿Realmente aporta valor al proyecto?"

Si la respuesta es no, no se implementará.

---

# Tecnologías

## Backend

* .NET 10
* ASP.NET Core Web API
* C#

## Inteligencia Artificial

* Semantic Kernel
* Ollama
* Modelo LLM local (Llama o Qwen)
* nomic-embed-text para embeddings

## Persistencia

* PostgreSQL
* pgvector

## Infraestructura

* Docker
* Docker Compose

Todo el proyecto deberá funcionar utilizando únicamente herramientas gratuitas.

---

# Preparado para el futuro

Aunque utilizaremos herramientas gratuitas, el diseño deberá permitir incorporar posteriormente:

* OpenAI
* Azure OpenAI
* Azure AI Search
* Pinecone
* Qdrant
* Weaviate

La lógica de negocio nunca deberá depender de un proveedor específico.

---

# Componentes principales

El proyecto girará alrededor de cuatro responsabilidades principales.

## Procesamiento de documentos

Responsable de:

* Leer PDFs.
* Extraer texto.
* Dividir contenido en chunks.

---

## Embeddings

Responsable de:

* Generar vectores.
* Abstraer el proveedor utilizado.

---

## Base vectorial

Responsable de:

* Guardar embeddings.
* Realizar búsquedas semánticas.

---

## Chat

Responsable de:

* Construir el contexto.
* Crear el prompt.
* Consultar el LLM.
* Devolver la respuesta.

---

# Flujo del sistema

1. El usuario carga un PDF.
2. El sistema extrae el texto.
3. El texto se divide en fragmentos.
4. Se generan embeddings.
5. Los embeddings se almacenan.
6. El usuario realiza una pregunta.
7. Se genera el embedding de la pregunta.
8. Se buscan los fragmentos más similares.
9. Se construye el contexto.
10. El LLM genera la respuesta.
11. Se devuelve la respuesta junto con sus referencias.

---

# Interfaces esperadas

La solución deberá mantenerse desacoplada mediante interfaces.

Ejemplos:

* IChatService
* IEmbeddingService
* IVectorStore
* IDocumentParser

Las implementaciones iniciales utilizarán Ollama y PostgreSQL.

En el futuro podrán reemplazarse sin modificar la lógica de negocio.

---

# Forma de trabajo

Cada nueva funcionalidad deberá desarrollarse siguiendo este orden:

1. Comprender el concepto.
2. Entender el problema que resuelve.
3. Diseñar la solución.
4. Implementar.
5. Explicar el código.
6. Validar el funcionamiento.
7. Identificar posibles mejoras.

No avanzaremos al siguiente paso hasta comprender completamente el actual.

---

# Plan del proyecto

## Sprint 1

Infraestructura

* Crear solución.
* Crear proyectos.
* Configurar Docker.
* Configurar PostgreSQL.
* Configurar pgvector.
* Configurar Ollama.
* Configurar Semantic Kernel.
* Configurar Swagger.
* Implementar carga de documentos.
* Extraer texto desde PDFs.

Objetivo:

Tener documentos cargados y procesados.

---

## Sprint 2

Motor RAG

* Implementar chunking.
* Generar embeddings.
* Guardar embeddings.
* Implementar búsqueda vectorial.
* Recuperar contexto relevante.

Objetivo:

Obtener correctamente los fragmentos relacionados con una pregunta.

---

## Sprint 3

Generación de respuestas

* Construir prompts.
* Integrar el LLM.
* Implementar el endpoint de preguntas.
* Agregar referencias.
* Documentar el proyecto.
* Preparar README.
* Publicar en GitHub.

Objetivo:

Responder preguntas utilizando únicamente la información de los documentos.

---

# Criterios de éxito

El proyecto estará terminado cuando sea posible:

* Levantar toda la solución utilizando Docker.
* Cargar el Código Nacional de Tránsito.
* Procesar automáticamente los documentos.
* Consultar información mediante lenguaje natural.
* Obtener respuestas fundamentadas en los documentos.
* Visualizar las referencias utilizadas.
* Comprender completamente cada componente implementado.

---

# Filosofía

No queremos construir el proyecto más grande.

Queremos construir el proyecto que más nos enseñe.

Cada línea de código deberá tener un propósito.

Cada componente deberá resolver un problema específico.

Cada decisión deberá permitirnos convertirnos en mejores desarrolladores Backend especializados en .NET e Inteligencia Artificial.

La prioridad no es terminar el proyecto.

La prioridad es comprender profundamente cómo funciona un sistema RAG profesional y salir del proyecto con conocimientos que puedan aplicarse posteriormente en soluciones empresariales reales.
