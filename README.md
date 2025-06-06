# BookApi

A RESTful Web API for managing a collection of books, built with ASP.NET Core 8.0 and Entity Framework Core.

## Overview

BookApi is a comprehensive backend service that provides CRUD operations for book resources. It includes features such as pagination, searching, and proper HTTP status code responses following RESTful principles.

## Features

- **Complete CRUD Operations**:
  - Create new books
  - Retrieve books (individual or paginated list)
  - Update existing books
  - Delete books
  
- **Search Functionality**:
  - Search books by title or author
  
- **Robust Error Handling**:
  - Appropriate HTTP status codes
  - Validation of input data
  
- **Pagination**:
  - Configurable page size
  - Page navigation

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | Get a paginated list of books |
| GET | `/api/books/{id}` | Get a specific book by ID |
| POST | `/api/books` | Create a new book |
| PUT | `/api/books/{id}` | Update an existing book |
| DELETE | `/api/books/{id}` | Delete a book |
| GET | `/api/books/search?query={query}` | Search for books by title or author |

## Technical Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: Entity Framework Core with SQL Server
- **Testing**: xUnit with in-memory database provider
- **Documentation**: Swagger/OpenAPI
- **Containerization**: Docker

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (or another database provider supported by EF Core)

### Installation

1. Clone the repository:
   ```
   git clone https://github.com/Ronohayon27/BookApi.git
   ```

2. Navigate to the project directory:
   ```
   cd BookApi
   ```

3. Build the project:
   ```
   dotnet build
   ```

4. Run the application:
   ```
   dotnet run
   ```

5. Access the API at `https://localhost:5001/api/books` or `http://localhost:5000/api/books`

### Running Tests

The project includes comprehensive unit tests for the API controllers. To run the tests:

```
cd BookApi.Tests
dotnet test
```

Or from the solution root:

```
dotnet test BookApi.Tests/BookApi.Tests.csproj
```

To run tests with code coverage:

```
dotnet test BookApi.Tests/BookApi.Tests.csproj --collect:"XPlat Code Coverage"
```

## Project Structure

- **Controllers/**: Contains API controllers
- **Models/**: Data models and DTOs
- **Data/**: Database context and repository implementations
- **BookApi.Tests/**: Unit tests for the API

## Book Model

The Book entity includes the following properties:

- `Id`: Unique identifier
- `Title`: Book title
- `Author`: Book author
- `PublicationDate`: Date of publication
- `Price`: Book price

## Testing

The project uses xUnit for testing with an in-memory database to ensure isolation. Tests cover all CRUD operations and edge cases such as:

- Successful operations with valid data
- Handling of invalid inputs
- Not found scenarios
- Pagination edge cases
- Search functionality

## Docker Support

The application can be containerized using Docker. A Dockerfile is included in the repository.

### Building the Docker Image

```
docker build -t bookapi .
```

### Running the Container

```
docker run -d -p 8080:80 --name bookapi-container bookapi
```

This will start the API on port 8080 of your host machine.

### Docker Compose (Optional)

If you have a `docker-compose.yml` file, you can use:

```
docker-compose up -d
```

## License

This project is open source and available under the [MIT License](LICENSE).
