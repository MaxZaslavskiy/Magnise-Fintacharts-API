# Fintacharts API Service

This is a REST API service built with .NET 9 that provides real-time and historical price information for market assets. It integrates with the Fintacharts platform using REST for asset discovery and WebSockets for live market data streaming.

## Technologies Used
* Framework: .NET 9 (ASP.NET Core Web API)
* Database: PostgreSQL
* ORM: Entity Framework Core
* Real-time Data: WebSockets
* Containerization: Docker & Docker Compose

## Key Features
* Background Synchronization: Fetches and stores available market instruments from the Fintacharts API.
* Live WebSocket Streaming: Subscribes to real-time `l1-update` messages to continuously update asset prices in the background.
* Resilience: Implements auto-reconnect logic for WebSocket connection drops and database connection retries on startup.
* Dockerized: One-command setup for the entire environment with automatic EF Core migrations.

## How to Run

### Prerequisites
* Docker Desktop installed and running.

### Running via Docker Compose
1. Open a terminal in the root directory of the project (where the `docker-compose.yml` file is located).
2. Run the following command to build and start the application:

```bash
docker-compose up --build -d
```

3. The application will start and automatically apply database migrations. The API will be available at `http://localhost:8080`.
4. (Optional) To view the background service logs and real-time price updates: 

```bash
docker logs fintacharts_api
```

## API Endpoints

### 1. Synchronize Assets
Populates the database with supported market assets. Run this endpoint first.
* Method: POST
* URL: `http://localhost:8080/api/assets/sync`
* Response: 200 OK

### 2. Get Supported Assets
Retrieves the list of all supported assets currently stored in the database.
* Method: GET
* URL: `http://localhost:8080/api/assets`
* Response: JSON array of assets.

### 3. Get Real-Time Prices
Retrieves the latest price and exact UTC time of the last update for specific assets.
* Method: GET
* URL: `http://localhost:8080/api/assets/prices?symbols=EURUSD&symbols=AUDCAD`
* Response Example:

```json
[
    {
        "symbol": "AUDCAD",
        "price": 0.95404,
        "lastUpdate": "2026-03-26T19:03:04.800256Z"
    },
    {
        "symbol": "EURUSD",
        "price": 1.15313,
        "lastUpdate": "2026-03-26T19:03:12.908703Z"
    }
]
```