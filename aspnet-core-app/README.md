# ASP.NET Core Application

This project is an ASP.NET Core 9.0 web application that includes a simple structure for managing products and users. Below is an overview of the project's components:

## Project Structure

- **Controllers**
  - `HomeController.cs`: Manages requests for the Index, Users, and Products views. It exports methods such as `Index`, `Products`, and `Users` to return the corresponding views.

- **Models**
  - `Product.cs`: Defines the `Product` class, representing a product with properties like `ProductId`, `ProductName`, and `Price`.
  - `User.cs`: Defines the `User` class, representing a user with properties like `UserId`, `UserName`, and `Email`.

- **Views**
  - **Home**
    - `Index.cshtml`: The main view displayed when the user accesses the root of the application.
    - `Products.cshtml`: Displays the list of products.
    - `Users.cshtml`: Displays the list of users.
  - **Shared**
    - `_Layout.cshtml`: Defines the shared layout for all views, including sections for the main content, scripts, and styles.
  - `_ViewImports.cshtml`: Contains import directives for views, allowing the inclusion of namespaces and partial files.
  - `_ViewStart.cshtml`: Defines the default layout for all views.

- **wwwroot**
  - **css**
    - `site.css`: Contains the CSS styles for the application.
  - **js**
    - `site.js`: Contains the JavaScript code for the application.

- **Configuration Files**
  - `appsettings.json`: Contains application configuration settings, such as connection strings and application parameters.
  - `aspnet-core-app.csproj`: The project configuration file for the ASP.NET Core application, specifying dependencies and build settings.
  - `Program.cs`: The entry point of the application, configuring the web server and necessary services.

## Getting Started

To run the application, ensure you have the .NET SDK installed. You can build and run the application using the following commands:

```bash
dotnet build
dotnet run
```

Visit `http://localhost:5000` in your browser to access the application.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.