# Library-Management-System-BOOKNEST
Final code in this directory

## Welcome to BookNest
Welcome to BookNest, a comprehensive and efficient library management system. This application allows users to manage books, issue books, and track issued books effortlessly.

## Table of Contents
1. [Introduction](#introduction)
2. [Project Setup](#project-setup)
3. [Admin Credentials](#admin-credentials)
4. [Features](#features)
    - [Authentication and Authorization](#authentication-and-authorization)
    - [Admin Dashboard](#admin-dashboard)
    - [Book Management](#book-management)
    - [Book Issue and Return](#book-issue-and-return)
    - [Reporting](#reporting)
    - [Search Functionality](#search-functionality)
5. [Usage](#usage)
6. [Acknowledgments](#acknowledgments)

## Introduction
BookNest is a powerful web-based library management system designed to simplify library management for both administrators and users. With a user-friendly interface and robust features, BookNest makes it easy to manage books, issue and return books, and generate reports.

## Project Setup

To set up the project locally, follow these steps:

1. **Clone the Repository:**

    ```sh
    git clone https://github.com/abu-nasim-lomani/Library-Management-System-BOOKNEST-.git
    cd Library-Management-System-BOOKNEST-
    ```

2. **Configure Database Connection:**

    Open the `appsettings.json` file and update the `ConnectionStrings` section to match your SQL Server configuration:

    ```json
    "ConnectionStrings": {
        "BookNestDB": "Server=DESKTOP-PM9D3IT;Database=BookNestD02;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=30;"
    }
    ```

3. **Update Database:**

    Open a terminal in the project directory and run the following commands to update the database:

    ```sh
    dotnet ef database update
    ```

4. **Build and Run the Solution:**

    - Clean the Solution:
        ```sh
        dotnet clean
        ```

    - Build the Solution:
        ```sh
        dotnet build
        ```

    - Run the Application:
        ```sh
        dotnet run
        ```

## Admin Credentials
Default Admin Credentials:

- **Email:** Admin@BookNest.com
- **Password:** @Shumonbd1@

## Features

### Authentication and Authorization
- **User Registration and Login:** Users can register and log in.
- **Role-based Access Control:** Different roles for administrators and regular users with specific permissions.

### Admin Dashboard
- **Comprehensive Dashboard:** Centralized dashboard for administrators to manage all activities.
- **Book Management:** Add, update, and delete books.

### Book Management
- **Add Book:** Add new books to the library.
- **Update Book:** Update existing book details.
- **Delete Book:** Delete books from the library.
- **View Book List:** View and search through the list of all books.

### Book Issue and Return
- **Request Book Issue:** Users can request to issue books.
- **Admin Issue Book:** Administrators can issue books to users.
- **Book Return:** Users can return issued books and administrators can manage returns.

### Reporting
- **Last 24 Hours Issues:** View a list of books issued in the last 24 hours.
- **Custom Reports:** Generate reports for books issued within a specific time frame.

### Search Functionality
- **Search Books:** Users can easily search for books.

## Usage

### View Last 24 Hours Issues:
- Log in as an admin.
- Navigate to the admin dashboard.
- Click the "View Last 24 Hours Issues" button.

### Book Management:
- Log in as an admin.
- Navigate to the book list.
- Use the "Update" or "Delete" buttons to manage books.

## Acknowledgments
This project was developed to make library management simple and efficient. Special thanks to all contributors who helped build and improve this project.
