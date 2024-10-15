# ClockApi

ClockApi is a secure and efficient API for managing clock-related data. This project uses ASP\.NET Core and Entity Framework Core for the backend, and it includes JWT-based authentication and authorization.

## Features

- **JWT Authentication**: Secure your API endpoints with JSON Web Tokens.
- **Environment Variables**: Store sensitive information in a `.env` file.
- **Swagger Integration**: Easily test and explore your API with Swagger UI.
- **CORS Configuration**: Allow cross-origin requests from specified origins.
- **Database Integration**: Use SQLite for data storage with Entity Framework Core.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQLite](https://www.sqlite.org/download.html)

### Installation

1. **Clone the repository**:
    ```sh
    git clone https://github.com/yourusername/ClockApi.git
    cd ClockApi
    ```

2. **Install dependencies**:
    ```sh
    dotnet restore
    ```

3. **Create a `.env` file**:
    ```sh
    touch clockapi.env
    ```

4. **Add the following environment variables to `clockapi.env`**:
    ```dotenv
    JwtKey=your_jwt_key
    JwtIssuer=your_jwt_issuer
    JwtAudience=your_jwt_audience
    CertPassword=your_cert_password
    ```

### Running the Application

1. **Apply database migrations**:
    ```sh
    dotnet ef database update
    ```

2. **Run the application**:
    ```sh
    dotnet run
    ```

3. **Access Swagger UI**:
    Open your browser and navigate to `http://localhost:5000/swagger` to explore the API endpoints.

### Running on IIS

To run the ClockApi on IIS with development and production URLs, configure the `launchSettings.json` file and ensure that IIS is set up correctly.

1. **Development**: Use the `IIS Express` or `ClockApi` profile. This will use the URLs `https://localhost:5001` and `http://localhost:5000`.

2. **Production**: Use the `Production` profile. This will use the URLs `https://your-production-url.com` and `http://your-production-url.com`.

Ensure that your IIS is configured to handle the application correctly:
- Install the ASP\.NET Core Module.
- Create a new site in IIS and point it to the folder where your application is published.
- Set the application pool to use `No Managed Code`.

## Project Structure

- `Controllers/`: Contains the API controllers.
- `Contexts/`: Contains the database context classes.
- `Models/`: Contains the data models.
- `Interfaces/`: Contains the interface definitions.
- `wwwroot/`: Contains static files.

## Security

- **Environment Variables**: Sensitive information such as JWT keys and database connection strings are stored in the `.env` file.
- **JWT Authentication**: Secure your API endpoints by requiring a valid JWT token.
- **HTTPS**: Ensure secure communication by using HTTPS.

## Contributing

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes.
4. Commit your changes (`git commit -m 'Add some feature'`).
5. Push to the branch (`git push origin feature-branch`).
6. Open a pull request.
