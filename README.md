# 🌸 Bloom & Co - Flower Shop E-Commerce

Bloom & Co is a premium flower shop e-commerce platform built with **ASP.NET Core MVC**. It offers a seamless shopping experience for floral arrangements, featuring a modern design, user-friendly interface, and a robust administration system.

## ✨ Features

- **Modern UI/UX**: A clean, responsive design optimized for both desktop and mobile.
- **Product Management**: Browse flowers by category, view detailed product information, and check availability.
- **Shopping Cart**: Real-time cart management with local storage synchronization.
- **User Dashboard**: personalized experience for customers to track orders, manage profiles, and view wishlists.
- **Admin Panel**: Comprehensive management suite for products, categories, orders, users, and discounts.
- **Marketing Tools**: Integrated campaign system and discount code functionality.
- **Security**: Secure authentication and authorization system.

## 🚀 Technologies Used

*   **Backend**: [.NET 8.0 / ASP.NET Core MVC](https://dotnet.microsoft.com/en-us/apps/aspnet)
*   **Database**: [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) with SQL Server
*   **Frontend**: HTML5, Vanilla CSS3, Javascript, Bootstrap 5
*   **Version Control**: Git & GitHub

## 🛠️ Installation & Setup

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/Peps2ne/flowershop-aspdotnetwebsite.git
    ```
2.  **Database Configuration**:
    Update the connection string in `appsettings.json` to point to your SQL Server instance:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BloomDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```
3.  **Apply Migrations**:
    Open the Package Manager Console and run:
    ```powershell
    Update-Database
    ```
4.  **Run the Application**:
    ```bash
    dotnet run
    ```

## 📸 Preview

*Stay tuned for screenshots!*

## 📄 License

This project is licensed under the MIT License.

---

Built with ❤️ by [Peps2ne](https://github.com/Peps2ne)
