# Hello, I'm Hennadzi Aksenchuk. This is my test task

Task Description:
You need to create an ASP.NET Core Web API that provides CRUD (Create, Read, Update, Delete) operations for managing a list of users. Each user should have the following attributes: Id, Name, Age, Email, and a related entity "Role" with values (User, Admin, Support, SuperAdmin), where one user can have multiple roles.

Requirements:

:white_check_mark: Create a new ASP.NET Core Web API project using C# and .NET Core.

:white_check_mark: Create a data model for the user (User) with attributes Id, Name, Age, Email, and a related entity "Role."

:white_check_mark: Create a controller (UserController) with methods to perform the following operations:

:white_check_mark: Get a list of all users (MUST implement pagination, sorting, and filtering for each attribute in the User and Role models).

:white_check_mark: Get a user by Id and all of their roles.

:white_check_mark: Add a new role to a user.

:white_check_mark: Create a new user.

:white_check_mark: Update user information by Id.

:white_check_mark: Delete a user by Id.

:white_check_mark: Add business logic to the controller to validate data:

:white_check_mark: Check for the presence of required fields (Name, Age, Email).

:white_check_mark: Check the uniqueness of Email.

:white_check_mark: Check that Age is a positive number.

:white_check_mark: Use Entity Framework Core (or any other ORM of your choice) to access and save user and role data in the database.

:white_check_mark: Create a migration using ORM to create the necessary table in the database.

:white_check_mark: Handle errors and return appropriate HTTP status codes (e.g., 404 for a missing user).

:white_check_mark: Document your API using Swagger or similar tools.

Optional Tasks (If Desired):

:x: Add authentication and authorization to your API using JWT tokens.

:white_check_mark: Configure action logging in the API (e.g., using Serilog).
