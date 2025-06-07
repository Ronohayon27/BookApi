# BookApi

A RESTful Web API for managing a collection of books, built with ASP.NET Core 8.0 and Entity Framework Core.

## Overview

BookApi is a comprehensive backend service that provides CRUD operations for book resources. It includes features such as pagination, searching, and proper HTTP status code responses following RESTful principles.

This project was created as a home assignment to demonstrate RESTful API development skills using .NET Core.

## Features

- **Complete CRUD Operations**:
  - Create new books
  - Retrieve books (individual or paginated list)
  - Update existing books
  - Delete books
- **Search Functionality**:
  - Search books by title or author
- **Comprehensive Error Handling**:
  - Global exception handling middleware
  - Appropriate HTTP status codes
  - Detailed error responses
  - Validation of input data
  - Error logging
- **Pagination**:
  - Configurable page size
  - Page navigation

## API Documentation

### Endpoints

| Method | Endpoint                          | Description                         | Status Codes  |
| ------ | --------------------------------- | ----------------------------------- | ------------- |
| GET    | `/api/books`                      | Get a paginated list of books       | 200, 400, 404 |
| GET    | `/api/books/{id}`                 | Get a specific book by ID           | 200, 404      |
| POST   | `/api/books`                      | Create a new book                   | 201, 400      |
| PUT    | `/api/books/{id}`                 | Update an existing book             | 204, 400, 404 |
| DELETE | `/api/books/{id}`                 | Delete a book                       | 204, 404      |
| GET    | `/api/books/search?query={query}` | Search for books by title or author | 200, 400      |

### Request and Response Examples

#### Get Books (GET /api/books)

Query Parameters:

- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 10)

Response (200 OK):

```json
[
  {
    "id": 1,
    "title": "The Great Gatsby",
    "author": "F. Scott Fitzgerald",
    "publicationDate": "1925-04-10T00:00:00",
    "price": 12.99
  },
  {
    "id": 2,
    "title": "To Kill a Mockingbird",
    "author": "Harper Lee",
    "publicationDate": "1960-07-11T00:00:00",
    "price": 14.99
  }
]
```

#### Create Book (POST /api/books)

Request Body:

```json
{
  "title": "1984",
  "author": "George Orwell",
  "publicationDate": "1949-06-08T00:00:00",
  "price": 11.99
}
```

Response (201 Created):

```json
{
  "id": 3,
  "title": "1984",
  "author": "George Orwell",
  "publicationDate": "1949-06-08T00:00:00",
  "price": 11.99
}
```

## Technical Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: Entity Framework Core with SQLite

- **Testing**: xUnit with in-memory database provider
- **Documentation**: Swagger/OpenAPI
- **Containerization**: Docker

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server/SQLite (or another database provider supported by EF Core)

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

5. Access the API at the URL shown in the console output (default is `http://localhost:5220` or `https://localhost:7188`)

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

- `Id`: Unique identifier (auto generated)
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
docker run -d -p 8080:8080 --name bookapi-container bookapi
```

This will start the API on port 8080 of your host machine.

## Assumptions Made

1. **Database**: The application assumes the use of Entity Framework Core with an in-memory database for development and testing. In a production environment, a persistent database like SQL Server would be used.

2. **Authentication**: The current implementation does not include authentication or authorization.

3. **Validation**: Basic validation is implemented for the Book model, assuming that all books must have a title, author, and valid publication date.

4. **Pagination**: The API assumes that pagination is a requirement for listing books, with sensible defaults (page 1, 10 items per page).

5. **Search**: The search functionality assumes case-insensitive matching for both title and author fields.

6. **Date Format and Type**: The publicationDate property is implemented using the DateTime type, as required by the assignment. Even though only the date component is semantically relevant for books, I left it as it is.

7. **Duplicate Entries**: The API allows duplicate values for book properties such as Title, Author, PublicationDate, and Price. However, the Id field is enforced as a unique identifier and cannot be duplicated.

## Future Improvements

1. **Authentication and Authorization**: Implement JWT-based authentication and role-based authorization.

2. **Searching**: Add the ability to search with pagination, if the numebr is too large.

3. **Caching**: Implement response caching to improve performance for frequently accessed resources.

4. **Logging and Monitoring**: Enhance logging and add monitoring capabilities for better observability.

## Development Decisions

1. **Repository Pattern**: Not implemented in favor of direct DbContext usage for simplicity in this small assignment. For larger applications, a repository pattern would be beneficial.

2. **In-Memory Database for Testing**: Used to ensure tests are isolated and don't depend on external systems.

3. **RESTful Design**: Followed REST principles for resource naming, HTTP methods, and status codes to create a predictable and standard API.

4. **Global Error Handling**: Implemented a centralized error handling middleware that catches all unhandled exceptions, logs them, and returns appropriate HTTP status codes and standardized error responses. This approach ensures consistent error handling across the entire API without duplicating error handling code in each controller.

5. **Working With Git**: Since this project was developed individually, all work was done on a single main branch. In a team setting, I would have implemented a branching strategy (e.g., feature, develop, and release branches) to support collaborative development and code reviews.

## License

This project is open source and available under the [MIT License](LICENSE).
