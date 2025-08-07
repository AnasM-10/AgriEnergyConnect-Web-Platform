
AgriEnergyConnect Platform
Overview
AgriEnergyConnect is a platform designed to connect farmers and agricultural employees, streamlining communication, management, and collaboration within the agricultural sector. Farmers can manage their products, and employees can assist with various tasks and access relevant information.

This README provides a guide on how to understand and use the AgriEnergyConnect platform.

Key Features
User Roles: The platform supports two primary roles:

Farmer: Can manage their product listings (add, view, edit, delete).

Employee: Can view farmer information, manage farmer accounts, and potentially filter and view product information across different farmers.

Farmer Dashboard: A central hub for farmers to access their product management tools.

Product Management: Farmers can add new products with details like name, category, production date, and description. They can also view, edit, and delete their existing product listings.

Farmer and Employee Management (Admin/Employee Functionality): Employees (with appropriate permissions) can add and view farmer accounts.

Role-Based Access: Different sections and functionalities of the platform are accessible based on the user's role, ensuring data privacy and appropriate access levels.

User Authentication: Secure login and registration system for both farmers and employees.

Technologies Used
This project was built using a Model-View-Controller (MVC) architectural pattern and the following technologies:

Backend:

C#

ASP.NET MVC

Entity Framework Core (for database interaction)

Frontend:

HTML

CSS

JavaScript

Database:

A local database created and managed within Visual Studio, likely using SQL Server Express.

Getting Started
To get a copy of this project up and running on your local machine, follow these simple steps.

Prerequisites:

You will need the following software installed on your computer:

Visual Studio (Community edition is free)

The .NET SDK (the specific version you used)

Installation:

Clone the repository to your local machine using Git:

Bash

git clone https://github.com/AnasM-10/AgriEnergyConnect-Web-Platform.git
Open the project in Visual Studio by double-clicking the solution file (.sln) in the main project folder.

Visual Studio will automatically install any necessary dependencies.

Once the project has loaded, press the F5 key or click the "Run" button in Visual Studio to start the application.

This will launch the application on your local web server, and you can access it through your browser.

How to Use the AgriEnergyConnect Platform
1. Registration
Access the Registration Page: Navigate to the "Register" link on the platform's homepage.

Fill in the Registration Form:

Provide your email address and a secure password.

Select your role: "Farmer" or "Employee".

If registering as a Farmer: You will be prompted to fill in additional information such as your First Name, Last Name, Contact Number, and Address. These details help in associating your products with your account.

If registering as an Employee: You may be asked for your First Name, Last Name, and Contact Number (these might be optional depending on the platform's configuration).

Submit the Form: Click the "Register" button to create your account.

Email Confirmation: You might receive an email to confirm your registration. Follow the instructions in the email to activate your account.

2. Login
Access the Login Page: Navigate to the "Login" link on the platform's homepage.

Enter Your Credentials: Provide the email address and password you used during registration.

Remember Me (Optional): Check the "Remember me?" box if you want the platform to remember your login on the current browser.

Log In: Click the "Log in" button to access your dashboard.

3. Farmer Functionality
Once logged in as a Farmer, you will be directed to your Farmer Dashboard. From here, you can:

View My Products: Click on the "View My Products" link in the navigation or on the dashboard. This will display a list of all the products you have added.

Edit Products: On the "View My Products" page, you will find an "Edit" option for each product. Click it to modify the product details (Name, Category, Production Date, Description). Remember to adhere to the validation rules (e.g., required fields, maximum lengths).

Delete Products: Similarly, you will find a "Delete" option to remove a product listing. You will likely be asked to confirm this action.

Add Product: Click on the "Add Product" link in the navigation or on the dashboard.

Fill in the form with the product's Name (required), Category, Production Date, and Description.

Click the "Add Product" button to save the new listing.

4. Employee Functionality
Once logged in as an Employee, you will be directed to your Employee Dashboard. From here, you may have access to the following (depending on your permissions):

View Farmers: A list of all registered farmers on the platform.

Add Farmer: An option to add new farmer accounts (this might be restricted to administrator roles).

Filter Products: Tools to filter and search products based on various criteria (e.g., category, farmer).

View Filtered Products: Display the results of your product filters.

Other Administrative Tasks: Depending on your role and permissions, you might have access to additional management features.

5. Navigation
The platform typically uses a navigation menu (usually at the top) to access different sections. Look for links such as:

Home: The main landing page.

Dashboard: Your personalized control panel.

Add Product (Farmer): To create new product listings.

View Products (Farmer): To see and manage your existing products.

View Farmers (Employee): To see the list of farmers.

Add Farmer (Employee/Admin): To register new farmers.

Filter Products (Employee): To search and filter product data.

Logout: To securely sign out of your account.

6. Common Elements
Forms: When adding or editing information, ensure you fill in all required fields (usually marked with an asterisk *) and adhere to any specified formats or length restrictions.

Buttons: Use buttons like "Save," "Add," "Edit," "Delete," and "Submit" to perform actions.

Messages: Look for success or error messages displayed after performing an action (e.g., "Product updated successfully!", "Please correct the errors below.").

Troubleshooting
Login Issues: Double-check your email and password. If you've forgotten your password, use the "Forgot your password?" link on the login page.

Registration Issues: Ensure all required fields are filled and that your password meets the complexity requirements (if any). If you don't receive a confirmation email, check your spam folder.

Data Not Saving: If you encounter issues saving changes, ensure you have filled in all required fields correctly and that your input does not exceed any length limitations. Refer to any error messages displayed on the page.

Access Denied: If you try to access a section that you don't have permission for based on your role, you might see an "Access Denied" message. Contact the platform administrator if you believe this is an error.

Support
If you encounter any issues or have questions about using the AgriEnergyConnect platform, please contact the support team at [insert support email address or contact information here].

Thank you for using AgriEnergyConnect!
