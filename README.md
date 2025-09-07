Async Message Orchestrator 🚀

This repository demonstrates an event-driven architecture using Azure Functions, Azure Service Bus, Azure Redis Cache, SignalR, and Angular.
The project showcases how to send, process, and notify messages asynchronously between a frontend Angular app and backend services in the cloud.

📌 Tech Stack
Frontend

Angular (Reactive Forms, HttpClient)

Provides UI for submitting messages.

Communicates with Azure Functions via HTTP.

Displays real-time task completion status using SignalR.

Backend

Azure Functions (.NET)

SendMessage Function → Receives requests from Angular and pushes a small reference to Azure Service Bus Queue; large payloads are stored in Azure Redis.

ProcessQueue Function → Triggered by Service Bus. Retrieves the full message from Redis, processes it, and notifies downstream consumers via SignalR.

Azure Service Bus (Queue)

Provides decoupling between producers (Angular app → SendMessage) and consumers (ProcessQueue).

Ensures reliable delivery with retries and dead-lettering.

Azure Cache for Redis

Stores large messages instead of pushing them directly into Service Bus (to keep Service Bus lightweight).

Messages are referenced by an ID sent through Service Bus.

Azure SignalR Service

Sends real-time notifications to the Angular app when a task completes.

📂 Architecture Overview
flowchart TD

A[Angular App] -->|HTTP POST| B[Azure Function: SendMessage]
B -->|Message ID| C[Azure Service Bus Queue]
B -->|Full Payload| D[Azure Redis Cache]

C --> E[Azure Function: ProcessQueue]
E -->|Retrieve Payload| D
E -->|Notify Completion| F[Azure Function / API with SignalR]
F -->|Real-time Updates| A

⚡ Patterns & Concepts Covered
1. Event-Driven Architecture

Angular initiates events.

Backend reacts asynchronously through Service Bus triggers.

2. Queue-Based Load Leveling

Service Bus Queue buffers messages, ensuring smooth load handling.

Consumers process messages at their own pace.

3. Cache-Aside Pattern

Redis stores heavy payloads.

Service Bus only carries references (lightweight IDs).

4. Pub/Sub via SignalR

Backend notifies Angular in real-time when tasks are completed.

Decouples frontend from backend processing.

5. CORS & Config Management

Config values (Service Bus, Redis, SignalR keys) are injected via local.settings.json and Azure App Configuration.

Angular handles cross-origin requests securely.

⚙️ Local Setup
1. Clone Repo
git clone https://github.com/<your-repo>.git
cd async-message-orchestrator

2. Frontend (Angular)
cd frontend
npm install
ng serve


App runs at: http://localhost:4200

3. Backend (Azure Functions)
cd backend
func start

4. Configuration (local.settings.json)
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ServiceBusConnection": "<your-azure-service-bus-connection-string>",
    "QueueName": "task-queue",
    "RedisConnection": "<your-azure-redis-connection-string>",
    "AzureSignalRConnectionString": "<your-signalr-connection-string>"
  }
}

✅ How It Works

User submits a message in Angular.

Angular calls SendMessage Function.

Function stores the large message in Redis and enqueues the message ID in Service Bus Queue.

ProcessQueue Function is triggered, retrieves the full message from Redis.

After processing, it notifies Angular via SignalR.

Angular UI updates instantly with task completion.

🚀 Future Enhancements

Migrate from Queue → Topic (when scaling out to multiple subscribers).

Add retry policies & dead-letter monitoring.

Extend SignalR notifications for richer events (progress updates, not just completion).

Secure communication with Azure AD Authentication.

🏆 Summary

This repo is a blueprint for building scalable async systems using:

Angular → frontend with Reactive Forms + SignalR

Azure Functions → serverless backend

Service Bus Queue → reliable message delivery

Redis → optimized message handling

SignalR → real-time updates