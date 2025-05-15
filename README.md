# Agri-Energy Connect Platform

The Agri-Energy Connect platform is a web application designed to bridge the gap between the agricultural sector and green energy technology providers. This platform facilitates collaboration, sharing of resources, and innovation in sustainable agriculture and renewable energy.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Setup Instructions](#setup-instructions)
3. [User Roles and Accounts](#user-roles-and-accounts)
4. [Using the Application](#using-the-application)
5. [Design and Implementation](#design-and-implementation)
6. [Troubleshooting](#troubleshooting)
7. [Video Demonstration](#video-demonstration)

## Prerequisites

To run this application, you need:
- .NET 9.0 SDK or later
- SQL Server (LocalDB or higher)
- Visual Studio 2022 or later (recommended)
- Internet connection (for loading Bootstrap and Font Awesome resources)

## Setup Instructions

### Database Configuration
1. The application uses Entity Framework Core with SQL Server.
2. Open the `appsettings.json` file and verify the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AgriEnergyConnect;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```
3. Modify the connection string if you want to use a different SQL Server instance.

### Building and Running the Application
1. Clone or download the repository to your local machine.
2. Open the solution file `Prog7311_Part2.sln` in Visual Studio.
3. Build the solution (Ctrl+Shift+B).
4. Run the application (F5 or Ctrl+F5).
5. The application will automatically create the database and seed it with sample data on first run.

### Alternative: Using the Command Line
1. Navigate to the project directory.
2. Run `dotnet restore` to restore all packages.
3. Run `dotnet build` to build the application.
4. Run `dotnet run` to start the application.
5. Access the application at `https://localhost:5001` or `http://localhost:5000`.

## User Roles and Accounts

### Predefined User Accounts
The application comes with two predefined user accounts for testing:

1. **Employee Account**
   - Username: `admin`
   - Password: `admin123`
   - Role: Employee

2. **Farmer Account**
   - Username: `farmer1`
   - Password: `farmer123`
   - Role: Farmer

### Registration
- New farmers can register through the registration page.
- New employee accounts can only be created by system administrators (outside the scope of this prototype).

## Using the Application

### Login Process
1. Navigate to the home page.
2. Click on "Login" in the navigation bar.
3. Enter your username and password.
4. Click the "Login" button.

### For Farmers

#### Managing Your Profile
1. After logging in, click on your username or "Profile" in the navigation bar.
2. You can update your farm information including:
   - Farm name
   - Location
   - Description
   - Contact information
3. Click "Save Changes" to update your profile.

#### Adding Products
1. Navigate to "My Products" from the navigation menu.
2. Click the "Add New Product" button.
3. Fill in the product details:
   - Name
   - Category (Crop, Livestock, Dairy, Poultry, Fruit, Vegetable, GreenEnergy, or Other)
   - Description
   - Price
   - Production Date
   - Upload an image (optional)
4. Click "Save" to add the product to your profile.

#### Managing Products
1. On the "My Products" page, you can view all your listed products.
2. Use the "Edit" button to update product details.
3. Use the "Delete" button to remove products from your listing.

### For Employees

#### Adding New Farmers
1. After logging in as an employee, you can add a new farmer in two ways:
   - Click the "Add New Farmer" button on the employee dashboard page, or
   - Click on "Add Farmer" in the navigation menu
2. Fill in the farmer's details:
   - Username
   - Password
   - Email
   - Farm Name
   - Location
   - Description
   - Contact Information
3. Click "Create Account" to register the new farmer.

#### Viewing Farmer Products
1. From the employee dashboard, you will see a list of all registered farmers.
2. Click "View Products" next to a farmer's name to see their product listings.
3. Products will be displayed with their details and images.

#### Filtering Products
1. On the "Filter Products" page, you can search for products based on various criteria:
   - Select a specific farmer from the dropdown
   - Set a date range for production dates
   - Choose a product category
2. Click "Apply Filters" to see the filtered results.
3. Results will be paginated if there are many products.

#### Managing Farmers
1. On the "Farmers" page, you can manage farmer accounts.
2. Use the "Delete" option to remove a farmer account if necessary (this will also remove all their products).

## Design and Implementation

### Technologies Used
- **ASP.NET Core MVC**: Web application framework
- **Entity Framework Core**: ORM for database operations
- **SQL Server**: Database engine
- **Bootstrap 5**: Frontend framework for responsive design
- **Font Awesome**: Icon library
- **BCrypt.NET**: Password hashing

### Architecture and Design Patterns
The application implements:
- **MVC Pattern**: Separation of concerns with Models, Views, and Controllers
- **Repository Pattern**: For data access abstraction through AppDbContext
- **Factory Pattern**: For user creation with different roles and profiles
- **Observer Pattern**: For handling authentication events and notifications

### Security Features
- Password hashing using BCrypt
- Role-based authorization
- Form validation and anti-forgery tokens
- Input sanitization

## Troubleshooting

### Common Issues

#### Database Connection Errors
- Verify that SQL Server is running.
- Check that the connection string in `appsettings.json` is correct.
- Ensure you have permission to create databases if it's the first run.

#### Login Issues
- Make sure you're using the correct username and password.
- Clear browser cookies if experiencing persistent login problems.

#### Image Upload Issues
- Ensure the `/wwwroot/uploads/products` directory has write permissions.
- Check that your image is in an accepted format (jpg, jpeg, png, gif).
- Image size should be reasonable (under 5MB).

## Video Demonstration

A complete demonstration of the Agri-Energy Connect Platform is available on YouTube:

[![Agri-Energy Connect Platform Demo](https://img.youtube.com/vi/Rl48DMD8IoY/0.jpg)](https://www.youtube.com/watch?v=Rl48DMD8IoY)

The video demonstrates:
- System architecture and design patterns implementation
- Employee features (adding farmers, viewing and filtering products)
- Farmer features (adding and managing products)
- Authentication and security features
- Responsive design and user interf

## Known Issues / Note to Marker

**Database Seeding and Demo Data**

- The application includes substantial demonstration data in the seeding logic (admin user, farmer user, and products). This is shown in the accompanying demonstration video.
- However, in some cases, only the admin user is seeded when running the application on a freshly created database. This is due to the seeding logic checking for any existing user and skipping the rest of the demo data if one is found.
- As a result, the farmer and product demo data may not always appear automatically, even though the code for seeding them is present and works as shown in the video.
- All core functionality is present and works as intended, and the application allows for adding farmers and products through the UI.
- If you would like to see the full demo data, please refer to the video demonstration.

Thank you for your understanding.
---
