# meta-exchange
meta-exchange

## Running the Project with Docker

To run this project using Docker, follow the steps below:

### Prerequisites

- Ensure Docker and Docker Compose are installed on your system.
- The project requires the following .NET versions:
  - `CryptoExecutionService`: .NET 9.0
  - `meta-exchange.git`: .NET 8.0

### Build and Run Instructions

1. Clone the repository and navigate to the project root directory.
2. Use the provided `docker-compose.yml` file to build and run the services:

   ```bash
   docker-compose up --build
   ```

3. The services will be available at the following ports:
   - `cryptoexecutionservice`: [http://localhost:8080](http://localhost:8080)
   - `metaexchange`: [http://localhost:8081](http://localhost:8081)

### Configuration

- The `cryptoexecutionservice` service depends on the `metaexchange` service.
- Both services are connected via the `app_network` Docker network.

For further details, refer to the Dockerfiles and the `docker-compose.yml` file included in the project.