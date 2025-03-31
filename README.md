# KhuBot

KhuBot is an educational AI chatbot designed to assist students at Kharazmi University with their "Logic Fundamentals and Set Theory" course. The bot provides personalized assistance to students, allowing them to ask questions and receive guidance within the scope of the course material.

## 🌟 Features

- **User Authentication**: Secure login and registration system for students
- **Personalized Responses**: The bot addresses users by their first name and provides tailored assistance
- **Token Management**: Users have a token limit to manage usage
- **Rate Limiting**: Prevents abuse by limiting request frequency
- **Chat History**: Maintains conversation history for context-aware responses

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core Web API with Clean Architecture
  - Domain Layer: Core entities and business logic
  - Application Layer: Service interfaces and implementation
  - Infrastructure Layer: Database access and external services
  - API Layer: Controllers and middleware
- **AI Integration**: Uses DeepSeek API for generating responses
- **Database**: SQL Server for data persistence
- **Authentication**: JWT-based authentication
- **Logging**: Serilog with Seq integration

## 🏗️ Architecture

The project follows Clean Architecture principles with a clear separation of concerns:

- **KhuBot.Domain**: Contains entities, DTOs, and domain exceptions
- **KhuBot.Application**: Contains business logic and service interfaces
- **KhuBot.Infrastructure**: Contains data access and external service implementations
- **KhuBot.Api**: Contains controllers, middleware, and API configuration
- **KhuBot.UnitTest**: Contains unit tests for the application

## 🚀 Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server
- DeepSeek API key

### Configuration

1. Clone the repository
2. Create a `.env` file in the KhuBot.Api directory with the following variables:
   ```
   DATABASE_CONNECTION=your_connection_string
   DEEPSEEK_API_KEY=your_deepseek_api_key
   JWT_KEY=your_jwt_secret_key
   SEQ_SERVER=your_seq_server_url
   SEQ_API_KEY=your_seq_api_key
   ```

### Running the Application

1. Navigate to the KhuBot.Api directory
2. Run `dotnet restore` to restore dependencies
3. Run `dotnet build` to build the project
4. Run `dotnet run` to start the API

## 📝 API Endpoints

- **Authentication**
  - POST `/api/Auth/Login`: Authenticate a user and get a JWT token

- **Chat**
  - POST `/api/Chat/SendMessage`: Send a message to the bot
  - GET `/api/Chat/GetChatList`: Get the user's chat history

## 👨‍💻 Development

The project uses Entity Framework Core for database access. Run the following command to apply migrations:

```
dotnet ef database update
```

## 📄 License

This project is open source and available for educational purposes.

## 👤 Author

- Hesam Javadi - Student at Kharazmi University

---

*This project is designed to help students learn Logic Fundamentals and Set Theory at Kharazmi University.*