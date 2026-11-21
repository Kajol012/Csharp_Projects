# Campus Event Management System — Refined Windows Forms Edition

This is a C# Windows Forms desktop application using SQL Server and ADO.NET.

## Included roles and pages

- **Admin:** User Management, Category Management, Venue Management, Notice Management.
- **Organizer:** Event Management, Schedule Management, Speaker Management, Sponsor Management.
- **Student:** Event Registration, Feedback Management, Favourite Events, Participation Management.
- **Faculty:** Event Approval, Department Event Management, Resource Management, Event Evaluation.
- **Common pages:** Sign In, Student/Organizer Sign Up, My Profile, role-based Dashboard.

Each role has four management modules. Each module provides Create, View, Update and Delete operations.

## Requirements

- Windows 10/11
- Visual Studio 2022 with **.NET desktop development**
- .NET Framework 4.8 Developer Pack
- SQL Server Express or SQL Server LocalDB

## OOP concepts used

- **Inheritance:** `DashboardForm`, `LoginForm`, `RegisterForm`, `CrudForm` and `ProfileForm` inherit from the Windows Forms `Form` class.
- **Encapsulation:** form state and controls are stored in private fields, for example `CrudForm`'s `selectedId`, `grid` and `inputs`.
- **Abstraction:** `Db.Query`, `Db.Execute` and `Db.P` hide repetitive ADO.NET database-command details.
- **Polymorphism:** `ModuleAccessPolicy` defines abstract methods. `AdminModuleAccessPolicy` and `OwnerModuleAccessPolicy` override them. `CrudForm` stores either derived object in a `ModuleAccessPolicy` reference and calls the same methods; the correct override runs at runtime.

## Layout update\n\nThe dashboard uses a responsive 2-by-2 card layout so all four role modules are visible. The CRUD page uses resizable sections for the records grid, input fields and action buttons. Maximize the window for the best layout.\n\n## Run

1. Extract the ZIP to a normal folder.
2. Open `CampusEventManagement.csproj` in Visual Studio.
3. Open `Db.cs` and set `Server` to your SQL Server instance:
   - SQL Express: `.\SQLEXPRESS`
   - LocalDB: `(localdb)\MSSQLLocalDB`
   - Default instance: `.`
4. Confirm `Database.sql` is included in the project and copied to the output directory.
5. Build > Rebuild Solution.
6. Press F5. On first launch, the application creates `CampusEventDB` and seeds the initial accounts.
7. Sign in:
   - Admin: `admin` / `ChangeMe123!`
   - Faculty: `faculty` / `ChangeMe123!`
   - Student and Organizer: create an account using the Sign Up link.

## Important database note

The application uses the schema in `Database.sql`. If you already have an older `CampusEventDB` with a different `Users` table structure, back up any data first. For a clean classroom setup, close the application and remove the old `CampusEventDB` using SQL Server Management Studio, then run the app again.

## Notes

- Organizer, Student and Faculty records are scoped to the signed-in user's ID in the CRUD screens.
- Admin-created user accounts receive the initial password `ChangeMe123!`; users should change it before real use.
- This is an academic starter project. Review role permissions, data validation and password storage before production deployment.
