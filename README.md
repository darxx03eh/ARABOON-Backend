# 🃜🃚🃖🃁🂭🂺 ARABOON ﹏𓊝﹏

[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/aspnet/core/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-68217A?style=for-the-badge&logo=efcore&logoColor=white)](https://learn.microsoft.com/en-us/ef/)
[![ARABOON](https://img.shields.io/badge/ARABOON-FF6600?style=for-the-badge&logoColor=white)](#)

### Table of Contents

- [Overview](#-overview)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Prerequisites](#️-prerequisites)
- [Running the App](#-running-the-app)
- [API Documentation](#-api-documentation)
- [Folder Structure](#-folder-structure)
- [Authentication & Authorization](#-authentication--authorization)
- [Features](#-features)
- [Database Schema](#-database-schema)
- [Author](#-author)

## 📋 Overview

Our platform is a premium online manga and manhwa reader, designed to deliver a seamless and engaging experience for fans worldwide. Users can browse, search, and read a curated collection of manga and manhwa with high-quality images and organized chapters. The platform offers a responsive interface compatible across devices, combining intuitive navigation with advanced filtering to help readers discover their next favorite series effortlessly.

---

## </> Tech Stack

| Layer           | Technology                                         |
| --------------- | -------------------------------------------------- |
| Backend         | ASP.NET Core 8 (Web API)                           |
| ORM             | Entity Framework Core 8                            |
| Database        | SQL Server                                         |
| Auth            | JWT Bearer Authentication                          |
| Validation      | FluentValidation                                   |
| Object Mapping  | AutoMapper                                         |
| Email Service   | MailKit                                            |
| Architecture    | Clean Architecture With CQRS and Mediator Patterns |
| Background jobs | Hangfire                                           |

---

## 🧭 Architecture

This project follows **Clean Architecture**, which provides a separation of concerns through layered structuring:

- `Araboon.API`: Presentation layer exposing RESTful APIs
- `Araboon.Core`: Middlewares, Behaviors, Exceptions and Features Organized by Mediator
- `Araboon.Data`: Entities, Response, Routing and DTOs
- `Araboon.Infrastructure`: Database, Repository and Database Configurations
- `Araboon.Service`: Business logic and external services

---

## 🛠️ Prerequisites

To run this project locally:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Visual Studio 2022+ or CLI

---

## 🚀 Running the App

1. **Clone the repository**:

   ```bash
   git clone https://github.com/darxx03eh/ARABOON-Backend.git
   cd ARABOON-Backend
   ```

2. **Configure the connection string** in `appsettings.json` under the `WebAPI` project:

   ```json
   "Logging": {
   "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
     }
   },
   "AllowedHosts": "*",
   "ConnectionStrings": {
     "AraboonConnection": "server=.;database=ARABOON;Trusted_Connection=True;TrustServerCertificate=True;"
   },
   "JwtSettings": {
     "Issuer": "YOUR-ISSUER",
     "Audience": "YOUR-AUDIENCE",
     "SecretKey": "SECRETKEY",
     "ValidateAudience": true,
     "ValidateIssuer": true,
     "ValidateLifetime": true,
     "ValidateIssuerSigningKey": true,
     "AccessTokenExpireDate": 1,
     "RefreshTokenExpireDate": 7
   },
   "EmailSettings": {
     "FromEmail": "YOUR-EMAIL",
     "Password": "PASSWORD",
     "SmtpServer": "smtp.gmail.com",
     "Port": 587,
     "UseSSL": false
   },
   "CloudinarySettings": {
     "CloudName": "CLOUD-NAME",
     "ApiKey": "APIKEY",
     "ApiSecret": "APISECRET"
   },
   "EncryptionSettings": {
     "Key": "YOUR-ENCRYPTION-KEY"
   },
   "HangfireSettings": {
	 "UserName": "HANGFIRE-USERNAME",
	 "Password": "HANGFIRE-PASSWORD"
   }
  ```

3. **Apply Migrations**:

   ```bash
   dotnet ef database update --project ARABOON.Infrastructure
   ```

4. **Run the application** (WebAPI only):

   ```bash
   dotnet run --project ARABOON.API
   ```

5. **Browse to Swagger UI**:
   ```
   https://localhost:<port>/swagger
   ```

---

## 📚 API Documentation

Swagger UI is enabled by default. After running the project, navigate to:

```
https://localhost:<port>/swagger
```

**Authentications**
HTTP Method | Endpoint | Description |
------------|----------------------------------------------------------------------|-------------------------------------------------|
POST | /Api/V1/Authentication/RegistrationUser | register a new account |
POST | /Api/V1/Authentication/SignIn | log in into account |
POST | /Api/V1/Authentication/SendConfirmationEmail | send email for confirm the account |
POST | /Api/V1/Authentication/GenerateRefreshToken | generate a new token after expire or refresh |
POST | /Api/V1/Authentication/LogOut | revoke refresh token to avoid refresh new token |
POST | /Api/V1/Authentication/SendForgetPasswordEmail | send a code to reset the password for account |
POST | /Api/V1/Authentication/ForgetPasswordConfirmation | confirm the code that s ent to reset password |
POST | /Api/V1/Authentication/ResetPassword | reset your account's password after confirm |
GET | /Api/V1/Authentication/EmailConfirmation?email={value}&token={value} | confirm your email by link |

**Users**

| HTTP Method | Endpoint                                                                      | Description                                            |
| ----------- | ----------------------------------------------------------------------------- | ------------------------------------------------------ |
| GET         | /Api/V1/users/profile/{username}                                              | create a new city                                      |
| GET         | /Api/V1/users/change-email/confirm?userId={value}&email={value}&token={value} | confirm your new email after change it                 |
| PUT         | /Api/V1/users/upload/profile-image                                            | change your profile image                              |
| PUT         | /Api/V1/users/upload/cover-image                                              | change your cover image                                |
| DELETE      | /Api/V1/users/profile-image                                                   | delete your profile image                              |
| DELETE      | /Api/V1/users/cover-image                                                     | delete your cover image                                |
| PATCH       | /Api/V1/Users/change-password                                                 | change the password for account                        |
| PATCH       | /Api/V1/users/change-username                                                 | change the password for account                        |
| PATCH       | /Api/V1/users/change-email                                                    | change the email for account                           |
| PATCH       | /Api/V1/users/change-bio                                                      | change the bio for account                             |
| PATCH       | /Api/V1/users/change-name                                                     | change the name for account                            |
| PATCH       | /Api/V1/users/crop-data                                                       | change the crop data for image to view it in front end |
| PATCH       | /Api/V1/users/cover-image/cropped-image                                       | change cropped image for cover image                   |

**Mangas**

| HTTP Method | Endpoint                                                                                        | Description                                                        |
| ----------- | ----------------------------------------------------------------------------------------------- | ------------------------------------------------------------------ |
| GET         | /Api/V1/Manga/GetCategoriesHomePageMangas                                                       | show the manga in home page category by categoy                    |
| GET         | /Api/V1/Manga/GetHottestMangas                                                                  | show the top ranking manga in home page                            |
| GET         | /Api/V1/Manga/GetMangaByID/{id}                                                                 | show the manga Information                                         |
| GET         | /Api/V1/Manga/GetMangaByCategoryName?category={value}&pageNumber={value}                        | show the manga by specific category                                |
| GET         | /Api/V1/Manga/GetPaginatedHottestManga?pageNumber={value}                                       | show manga list using pagination                                   |
| GET         | /Api/V1/Manga/GetMangaByStatus?Status={value}&PageNumber={value}&OrderBy={value}&Filter={value} | show manga list by status, filtering and ordering using pagination |
| GET         | /Api/V1/Manga?search={value}                                                                    | search for specific manga                                          |
| GET         | /Api/V1/Manga/{id}/comments?pageNumber={value}                                                  | view comments for specific manga                                   |
| GET         | /Api/V1/Manga/{id}/comments-count                                                               | view number of comments in specific manga                          |
| GET         | /Api/V1/Manga/dashboard?Search={value}&PageNumber={value}&PageSize={value}                      | view manga and search for it using pagination `admin only`         |
| POST        | /Api/V1/Manga                                                                                   | add new manga `admin only`                                         |
| DELETE      | /Api/V1/Manga/{id}                                                                              | delete existing manga `admin only`                                 |
| DELETE      | /Api/V1/Manga/{id}/image                                                                        | delete image for existing manga `admin only`                       |
| PATCH       | /Api/V1/Manga/upload-image                                                                      | upload a new image for existing manga `admon only`                 |
| PUT         | /Api/V1/Manga                                                                                   | update existing manga `admin only`                                 |
| PATCH       | /Api/V1/Manga/{id}/arabic-toggle                                                                | toggle activate or deactivate arabic for manga `admin only`        |
| PATCH       | /Api/V1/Manga/{id}/english-toggle                                                               | toggle activate or deactivate english for manga `admin only`       |
| PATCH       | /Api/V1/Manga/{id}/active-toggle                                                                | toggle activate or deactivate manga `admin only`                   |

**Favorites**

| HTTP Method | Endpoint                                                | Description                                          |
| ----------- | ------------------------------------------------------- | ---------------------------------------------------- |
| POST        | /Api/V1/Favorites/AddToFavorites/{id}                   | add manga to favorite `authorize only`               |
| DELETE      | /Api/V1/Favorites/RemoveFromFavorites/{id}              | remove from favorite `authorize only`                |
| GET         | /Api/V1/Favorites/ViewFavoritesManga?pageNumber={value} | show your favorite using pagination `authorize only` |

**CompletedReads**

| HTTP Method | Endpoint                                                          | Description                                                |
| ----------- | ----------------------------------------------------------------- | ---------------------------------------------------------- |
| POST        | /Api/V1/CompletedReads/AddToCompletedReads/{id}                   | add manga to complete reads `authorize only`               |
| DELETE      | /Api/V1/CompletedReads/RemoveFromCompletedReads/{id}              | remove from complete reads `authorize only`                |
| GET         | /Api/V1/CompletedReads/ViewCompletedReadsManga?pageNumber={value} | show your complete reads using pagination `authorize only` |

**CurrentlyReading**

| HTTP Method | Endpoint                                                              | Description                                                   |
| ----------- | --------------------------------------------------------------------- | ------------------------------------------------------------- |
| POST        | /Api/V1/CurrentlyReading/AddToCurrentlyReading/{id}                   | add manga to currently reading `authorize only`               |
| DELETE      | /Api/V1/CurrentlyReading/RemoveFromCurrentlyReading/{id}              | remove from currently reading `authorize only`                |
| GET         | /Api/V1/CurrentlyReading/ViewCurrentlyReadingManga?pageNumber={value} | show your currently reading using pagination `authorize only` |

**ReadingLaters**

| HTTP Method | Endpoint                                                      | Description                                               |
| ----------- | ------------------------------------------------------------- | --------------------------------------------------------- |
| POST        | /Api/V1/ReadingLater/AddToReadingLater/{id}                   | add manga to reading later `authorize only`               |
| DELETE      | /Api/V1/ReadingLater/RemoveFromReadingLater/{id}              | remove from reading later `authorize only`                |
| GET         | /Api/V1/ReadingLater/ViewReadingLaterManga?pageNumber={value} | show your reading later using pagination `authorize only` |

**Notifications**

| HTTP Method | Endpoint                                                        | Description                                               |
| ----------- | --------------------------------------------------------------- | --------------------------------------------------------- |
| POST        | /Api/V1/Notifications/AddToNotifications/{id}                   | add manga to notifications `authorize only`               |
| DELETE      | /Api/V1/Notifications/RemoveFromNotifications/{id}              | remove from notifications `authorize only`                |
| GET         | /Api/V1/Notifications/ViewNotificationsManga?pageNumber={value} | show your notifications using pagination `authorize only` |

**ChapterViews**

| HTTP Method | Endpoint                                                         | Description                             |
| ----------- | ---------------------------------------------------------------- | --------------------------------------- |
| POST        | /Api/V1/ChapterView/MarkAsRead?mangaId={value}&chapterId={value} | mark chapter as read `authorize only`   |
| DELETE      | /Api/V1/ChapterView/MarkAsRead?mangaId={value}&chapterId={value} | mark chapter as unread `authorize only` |

**Chapters**

| HTTP Method | Endpoint                                                                                 | Description                                                 									|
| ----------- | ---------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------|
| GET         | /Api/V1/Chapters/ViewChaptersForSpecificMangaByLanguage?MangaID={value}&Language={value} | view chapters as arabic or english                          									|
| GET         | /Api/V1/Chapters/images?MangaId={value}&ChapterNo={value}&Language={value}               | get images for specific chapter                             									|
| POST        | /Api/V1/Chapters/read                                                                    | increaming views by 1 when you finished reading the chapter								    |
| POST        | /Api/V1/Chapters                                                                         | add new chapter to specific manga and upload images and send notifications in the background |

**Categories**

| HTTP Method | Endpoint                          | Description                                                             |
| ----------- | --------------------------------- | ----------------------------------------------------------------------- |
| GET         | /Api/V1/Categories/GetCategories  | get categories for filtering in manga list                              |
| POST        | /Api/V1/Categories                | add new category `admin only`                                           |
| DELETE      | /Api/V1/Categories/{id}           | delete existing category `admin only`                                   |
| POST        | /Api/V1/Categories/{id}/active    | active existing category `admin only`                                   |
| DELETE      | /Api/V1/Categories/{id}/deactive  | deactive existing category `admin only`                                 |
| PUT         | /Api/V1/Categories                | update existing category Information `admin only`                       |
| GET         | /Api/V1/Categories?search={value} | get categories or some category by serach it for dashboard `admin only` |
| GET         | /Api/V1/Categories/{id}           | get category information `admin only`                                   |

**Comments**

| HTTP Method | Endpoint                                         | Description                                        |
| ----------- | ------------------------------------------------ | -------------------------------------------------- |
| POST        | /Api/V1/comments                                 | add comments for specific manga `authorize only`   |
| DELETE      | /Api/V1/comments/{id}                            | delete existing comment `authorize only`           |
| PUT         | /Api/V1/comments/{id}                            | update existing comment `authorize only`           |
| POST        | /Api/V1/comments/{id}/like                       | add like to existing comment `authorize only`      |
| DELETE      | /Api/V1/comments/{id}/like                       | remove like from existing comment `authorize only` |
| GET         | /Api/V1/comments/{id}/replies?pageNumber={value} | get replies for existing comment                   |

**Replies**

| Method | Endpoint                  | Description                                      |
| ------ | ------------------------- | ------------------------------------------------ |
| POST   | /Api/V1/replies           | add reply to a specific user `authorize only`    |
| DELETE | /Api/V1/replies/{id}      | delete existing reply `authorize only`           |
| PUT    | /Api/V1/replies/{id}      | update existing reply `authorize only`           |
| POST   | /Api/V1/replies/{id}/like | add like to existing reply `authorize only`      |
| DELETE | /Api/V1/replies/{id}/like | remove like from existing reply `authorize only` |

**Ratings**

| Method | Endpoint                   | Description                                  |
| ------ | -------------------------- | -------------------------------------------- |
| POST   | /Api/V1/ratings            | add rate for existing manga `authorize only` |
| DELETE | /Api/V1/ratings/{id}       | remove existing rate `authorize only`        |
| GET    | /Api/V1/ratings/manga/{id} | show ratings for existing manga              |

---

## 🗂 Folder Structure

```
ARABOON-Backend/
├───Araboon.API
│   ├───Bases
│   ├───Controllers
│   ├───EmailTemplates
├───Araboon.Core
│   ├───Bases
│   ├───Behaviors
│   ├───Exceptions
│   ├───Features
│   │   ├───Authentications
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   ├───Models
│   │   │   │   └───Validators
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───Categories
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   ├───Models
│   │   │   │   └───Validators
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───ChapterImages
│   │   │   ├───Commands
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───Chapters
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   └───Models
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───ChapterViews
│   │   │   └───Commands
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───Comments
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   └───Models
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───CompletedReads
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   └───Models
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───CurrentlyReadings
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   └───Models
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───Favorites
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   └───Models
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───Mangas
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   ├───Models
│   │   │   │   └───Validators
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───Notifications
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   └───Models
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───Ratings
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   ├───Models
│   │   │   │   └───Validators
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───ReadingLaters
│   │   │   ├───Commands
│   │   │   │   ├───Handlers
│   │   │   │   └───Models
│   │   │   └───Queries
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   ├───Replies
│   │   │   └───Commands
│   │   │       ├───Handlers
│   │   │       └───Models
│   │   └───Users
│   │       ├───Commands
│   │       │   ├───Handlers
│   │       │   ├───Models
│   │       │   └───Validators
│   │       └───Queries
│   │           ├───Handlers
│   │           └───Models
│   ├───Mapping
│   │   ├───Authentications
│   │   │   └───CommandMapping
│   │   ├───Categories
│   │   │   └───QueryMapping
│   │   ├───Chapters
│   │   │   └───QueryMapping
│   │   └───Mangas
│   │       └───QueryMapping
│   ├───Middlewares
│   └───Translations
├───Araboon.Data
│   ├───DTOs
│   │   └───Mangas
│   ├───Entities
│   │   └───Identity
│   ├───Enums
│   ├───Helpers
│   ├───Response
│   │   ├───Authentications
│   │   ├───Categories
│   │   │   └───Queries
│   │   ├───ChapterImages
│   │   ├───Chapters
│   │   │   └───Queries
│   │   ├───Comments
│   │   │   └───Queries
│   │   ├───CompletedReads
│   │   │   └───Queries
│   │   ├───CurrentlyReadings
│   │   │   └───Queries
│   │   ├───Favorites
│   │   │   └───Queries
│   │   ├───Mangas
│   │   │   └───Queries
│   │   ├───Notifications
│   │   │   └───Queries
│   │   ├───Ratings
│   │   ├───ReadingLaters
│   │   │   └───Queries
│   │   └───Users
│   │       └───Queries
│   ├───Routing
│   └───Wrappers
├───Araboon.Infrastructure
│   ├───Commons
│   ├───Configurations
│   ├───Data
│   ├───IRepositories
│   ├───Migrations
│   ├───Repositories
│   ├───Resolvers
│   │   ├───ChaptersResolver
│   │   └───MangasResolver
│   └───Seeder
└───Araboon.Service
    ├───Implementations
    ├───Interfaces
```

---

## 🔒 Authentication & Authorization

- Uses **JWT Tokens**
- Supports roles: `User`, `Admin`
- Role-based access via `[Authorize(Roles = "Admin")]`

---

## 📧 Features

- 🔐 Refresh token with secure HTTP-only storage
- 📢 Notifications via Hangfire background jobs
- 🧩 Mediator & CQRS architecture
- 🧠 Unit of Work for repositories
- 🌐 Arabic & English responses
- 📬 Email confirmations via MailKit

---

## 🖧 Database Schema

Below is the Entity-Relationship Diagram (ERD) illustrating the structure of the database, including tables, relationships, and keys used in the application.

![ERD Diagram](ERD/ARABOON.svg)

## 🪶 Author

Built by `Mahmoud Darawsheh - darxx03eh`  
email: darxx03eh@gmail.com  
[Mahmoud Darawsheh - linkedin](https://www.linkedin.com/in/mahmoud-darawsheh)  
[Mahmoud Darawsheh - github](https://github.com/darxx03eh)

---

<p align="center">© 2025 ARABOON. All rights reserved.</p>
<p align="center">Built with ☯︎ by Abdullah Noor & Mahmoud Darawsheh</p>
