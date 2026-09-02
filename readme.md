# SimpleAuth

SimpleAuth is a full-stack authentication sample built with:

- .NET 10 ASP.NET Core API
- React frontend (Vite + React Router)
- PostgreSQL database
- Docker Compose for local development and orchestration

This project implements a basic authentication flow with user registration, login, JWT-based authorization, and an authenticated protected page.

## Tech Stack

- Backend: .NET 10, ASP.NET Core Minimal API
- Frontend: React, Vite, TypeScript
- Database: PostgreSQL 18
- Containerization: Docker Compose

## Project Structure

- `SimpleAuth/` - Docker Compose and environment configuration
- `SimpleAuth/SimpleAuthAPI/` - .NET 10 backend API
- `SimpleAuth/simple-auth-ui/` - React frontend application

## Features

- User registration
- User login
- JWT token generation and validation
- Protected route / authenticated view
- PostgreSQL persistence
- Docker-based local setup

## Required Environment Variables

The `.env` file is included in the repository inside the `SimpleAuth` for simplicity but you can change it following this structure.

Example:

```env
DB_DATABASE=simpleauth
DB_USER=postgres
DB_PASSWORD=your_secure_password
JWT_SECRET=change_this_to_a_long_random_secret
JWT_ISSUER=simpleauth-api
JWT_AUDIENCE=simpleauth-frontend
FRONTEND_SITE=http://localhost:3000
```

### Variable explanation

- `DB_DATABASE`: PostgreSQL database name
- `DB_USER`: PostgreSQL username
- `DB_PASSWORD`: PostgreSQL password
- `JWT_SECRET`: Secret key used to sign JWT tokens
- `JWT_ISSUER`: JWT issuer value expected by the API
- `JWT_AUDIENCE`: JWT audience value expected by the API
- `FRONTEND_SITE`: Allowed frontend origin for CORS in production mode

> The backend reads these values from the environment and injects them into the application configuration.

## Run with Docker Compose

From the project root folder:

```bash
cd SimpleAuth
docker compose --env-file .env up --build
```

This will start the following services:

- API: http://localhost:5000
- Frontend: http://localhost:3000
- PostgreSQL: localhost:5434

## Services Overview

### Backend API

The .NET 10 API is built from `SimpleAuthAPI/Dockerfile` and runs on port `5000` mapped to container port `8080`.

It:

- connects to PostgreSQL using `ConnectionStrings__Database`
- validates JWT tokens using `Jwt:Secret`, `Jwt:Issuer`, and `Jwt:Audience`
- applies database migrations automatically on startup

### Frontend

The React app is built from `simple-auth-ui/Dockerfile` and is exposed on port `3000`.

The frontend is configured to call the backend API through the `VITE_API_URL` build argument.

### PostgreSQL

A PostgreSQL 18 container is created with:

- database: `${DB_DATABASE}`
- user: `${DB_USER}`
- password: `${DB_PASSWORD}`

The database is mounted to a Docker volume named `pgdata` and is exposed on host port `5434`.

## Stop the Project

```bash
docker compose down
```

To remove the database volume as well:

```bash
docker compose down -v
```

## Local Development Notes

If you want to run the backend outside Docker, make sure your machine has the .NET 10 SDK installed and configure the same environment variables in your shell or user-secrets setup.
