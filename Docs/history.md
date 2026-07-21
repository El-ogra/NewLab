# NewLab - Project History & Documentation

## 📋 Project Overview
**Project Name**: NewLab  
**Description**: Medical Laboratory Management System  
**Type**: Desktop Application  
**Target Users**: Commercial sale to multiple medical laboratories  

---

## 🛠️ Technologies Used

### Core Technologies
- **Framework**: .NET 8.0 (LTS - Long Term Support)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Database**: SQL Server
- **Language**: C# 12

### Architectural Pattern
- **Pattern**: MVVM (Model-View-ViewModel)
- **Navigation**: ViewModel-first with ContentControl
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

---

## 📦 NuGet Packages Installed

| Package | Version | Purpose |
|---------|---------|---------|
| CommunityToolkit.Mvvm | 8.3.2 | MVVM helpers (ObservableObject, RelayCommand, source generators) |
| BCrypt.Net-Next | 4.2.0 | Secure password hashing (BCrypt algorithm with automatic salting) |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.8 | EF Core SQL Server provider |
| Microsoft.EntityFrameworkCore.Tools | 8.0.8 | EF Core migrations and scaffolding tools |
| Microsoft.Extensions.DependencyInjection | 8.0.0 | Dependency Injection container |
| Microsoft.Extensions.Hosting | 8.0.0 | Application lifecycle management |
| Microsoft.Extensions.Configuration.Json | 8.0.0 | JSON configuration support |
| Serilog | 3.1.1 | Structured logging |
| Serilog.Extensions.Hosting | 8.0.0 | Serilog integration with hosting |
| Serilog.Sinks.Console | 5.0.1 | Console logging output |
| Serilog.Sinks.File | 5.0.0 | File logging output |
| MaterialDesignThemes | 5.1.0 | Modern Material Design UI theme |
| MaterialDesignColors | 3.1.0 | Material Design color palette |
| FluentValidation | 11.10.0 | Model validation |
| FluentValidation.DependencyInjectionExtensions | 11.10.0 | FluentValidation DI integration |
| Mapster | 7.4.0 | Object-to-object mapping (faster alternative to AutoMapper) |
| Mapster.DependencyInjection | 1.0.1 | Mapster DI integration |
| QuestPDF | 2026.7.1 | PDF report generation |

---

## 🏗️ Project Structure

```
NewLab/
├── Assets/                     # Images, icons, logos, fonts
├── Converters/                 # IValueConverter implementations
├── Data/                       # EF Core DbContext and design-time factories
│   ├── NewLabDbContext.cs      # DbContext with DbSets for User, Role, UserRole
│   └── NewLabDbContextFactory.cs # Design-time factory for EF Core tooling
├── Helpers/                    # Extension methods, utility classes
├── Models/                     # Domain entities and DTOs
│   ├── Domain/                 # Rich domain models
│   │   ├── User.cs             # User entity (Id, Username, PasswordHash, FullName, etc.)
│   │   ├── Role.cs             # Role entity (Id, Name, Description)
│   │   └── UserRole.cs         # Many-to-many join entity (UserId, RoleId)
│   └── DTOs/                   # Data transfer objects
├── Resources/                  # Shared XAML resources
│   ├── Styles/                 # Control styles, themes
│   ├── Templates/              # DataTemplates, ControlTemplates
│   └── ResourceDictionaries/   # Merged dictionaries
├── Services/                   # Business logic & infrastructure
│   ├── Interfaces/             # Service contracts
│   │   ├── INavigationService.cs
│   │   ├── IDialogService.cs
│   │   ├── IApplicationStartupService.cs  # First-run detection, role seeding
│   │   └── IAuthService.cs                # Login, password hashing, account creation
│   ├── Implementations/        # Concrete implementations
│   │   ├── NavigationService.cs
│   │   ├── DialogService.cs
│   │   ├── ApplicationStartupService.cs   # Checks if Users table is empty
│   │   └── AuthService.cs                 # BCrypt hashing, credential validation
│   └── Factories/              # ViewModel factories
├── ViewModels/                 # MVVM ViewModels
│   ├── Base/                   # ViewModelBase, shared base classes
│   │   └── ViewModelBase.cs
│   ├── Pages/                  # Page-level ViewModels
│   │   ├── SetupViewModel.cs   # First-time admin account creation
│   │   └── LoginViewModel.cs   # Login form logic
│   ├── Dialogs/                # Dialog ViewModels
│   └── Components/             # Reusable component ViewModels
├── Views/                      # WPF Views
│   ├── Pages/                  # Main pages
│   ├── Dialogs/                # Modal dialogs
│   ├── Controls/               # Reusable UserControls
│   └── Windows/                # Shell windows
│       ├── SetupView.xaml/.cs  # First-time setup wizard
│       ├── LoginView.xaml/.cs  # Login screen (green/yellow theme)
│       └── MainWindow.xaml/.cs # Main application shell
├── Docs/                       # Project documentation
│   └── history.md              # This file
├── Migrations/                 # EF Core migrations
│   └── <timestamp>_InitialCreate.cs  # Initial database schema
├── App.xaml                    # Application resources, MaterialDesign theme, ShutdownMode
├── App.xaml.cs                 # Application entry point, DI setup, startup routing
└── appsettings.json            # Application configuration
```

---

## 📅 Development Phases

### ✅ Phase 1: Project Setup & Structure
**Status**: Completed  
**Date**: 2026-07-21

**Tasks Completed**:
1. ✅ Created WPF .NET 8 project
2. ✅ Created 24 folders for MVVM structure
3. ✅ Installed 17 NuGet packages
4. ✅ Aligned all Microsoft.Extensions.* packages to version 8.0.0 (LTS)
5. ✅ Fixed version conflicts (NU1603 warnings)
6. ✅ Build status: 0 errors, 0 warnings

---

### ✅ Phase 2.1: Base Infrastructure Files
**Status**: Completed  
**Date**: 2026-07-21

**Files Created**:
1. ✅ `ViewModels/Base/ViewModelBase.cs`
   - Base class inheriting from ObservableObject
   - Properties: Title, IsBusy
   - Uses CommunityToolkit.Mvvm source generators

2. ✅ `Services/Interfaces/INavigationService.cs`
   - Interface for navigation service
   - Methods: NavigateTo<T>(), GoBack(), CanGoBack

3. ✅ `Services/Interfaces/IDialogService.cs`
   - Interface for dialog service
   - Methods: ShowMessage(), ShowConfirmation()

**Build Status**: 0 errors, 0 warnings

---

### ✅ Phase 2.2.1: App.xaml Configuration
**Status**: Completed  
**Date**: 2026-07-21

**Tasks Completed**:
1. ✅ Modified `App.xaml` to include MaterialDesign theme
2. ✅ Added primary and secondary color definitions
3. ✅ Added color brushes (PrimaryBrush, SecondaryBrush, etc.)

**Build Status**: 0 errors, 0 warnings

---

### ✅ Phase 2.2.2: Service Implementations & DI Setup
**Status**: Completed  
**Date**: 2026-07-21

**Files Created**:
1. ✅ `Services/Implementations/NavigationService.cs`
   - Stack-based navigation implementation
   - Methods: NavigateTo<T>(), GoBack(), CanGoBack
   - Maintains navigation history

2. ✅ `Services/Implementations/DialogService.cs`
   - WPF MessageBox wrapper
   - Methods: ShowMessage(), ShowConfirmation()

**Files Modified**:
1. ✅ `App.xaml.cs`
   - Added IHost for application lifecycle
   - Configured Dependency Injection container
   - Registered INavigationService and IDialogService

**Build Status**: 0 errors, 0 warnings

---

### ✅ Phase 2.2.3: Application Configuration
**Status**: Completed  
**Date**: 2026-07-21

**Files Created**:
1. ✅ `appsettings.json`
   - ConnectionStrings section (DefaultConnection for SQL Server)
   - AppSettings section (empty values for commercial deployment)
   - Configurable: LabName, LabAddress, LabPhone, LabEmail, DefaultLanguage, Currency

**Files Modified**:
1. ✅ `NewLab.csproj`
   - Added ItemGroup to copy appsettings.json to output directory

**Build Status**: 0 errors, 0 warnings

---

### ✅ Phase 3.1: Authentication & First-Run Setup
**Status**: Completed  
**Date**: 2026-07-21

**Goal**: Implement a complete authentication system with a first-time setup wizard that creates the initial Administrator account when the database is empty, and a standard login screen for subsequent runs.

#### Step 1: NuGet Package Installation
- ✅ Installed `BCrypt.Net-Next` (v4.2.0) for secure password hashing
- ✅ Verified 0 version conflict warnings after installation

#### Step 2: Domain Models (EF Core Code-First)
- ✅ Created `Models/Domain/User.cs` — User entity with Id, Username, PasswordHash, FullName, Email, PhoneNumber, IsActive, CreatedAt, LastLoginAt
- ✅ Created `Models/Domain/Role.cs` — Role entity with Id, Name, Description, and UserRoles navigation
- ✅ Created `Models/Domain/UserRole.cs` — Many-to-many join entity with composite key (UserId, RoleId)

#### Step 3: DbContext & Database
- ✅ Created `Data/NewLabDbContext.cs` with DbSets for User, Role, UserRole
- ✅ Configured composite primary key on UserRole (UserId + RoleId)
- ✅ Configured unique index on User.Username
- ✅ Seeded 3 default roles: Admin (Id=1), Technician (Id=2), Receptionist (Id=3)
- ✅ Created `Data/NewLabDbContextFactory.cs` — Design-time factory for EF Core tooling (required because the app uses IHost-based DI, not a standard Program.cs)
- ✅ Created initial EF Core migration (`InitialCreate`) and applied to database
- ✅ Verified database `NewLabDb` created on `.\SQLEXPRESS` with all tables and seed data

#### Step 4: Service Interfaces
- ✅ Created `Services/Interfaces/IApplicationStartupService.cs`
  - `IsFirstRunAsync()` — Checks if Users table is empty
  - `SeedDefaultRolesAsync()` — Ensures default roles exist
- ✅ Created `Services/Interfaces/IAuthService.cs`
  - `ValidateCredentialsAsync()` — Validates username/password against database
  - `HashPassword()` / `VerifyPassword()` — BCrypt wrapper methods
  - `CreateAdminAccountAsync()` — Creates initial admin with Admin role

#### Step 5: Service Implementations
- ✅ Created `Services/Implementations/ApplicationStartupService.cs`
  - Uses EF Core `AnyAsync()` to check for existing users
  - Seeds roles if they don't exist (idempotent)
- ✅ Created `Services/Implementations/AuthService.cs`
  - BCrypt.Net v4.2.0 uses fully qualified `BCrypt.Net.BCrypt.HashPassword()` / `BCrypt.Net.BCrypt.Verify()` to resolve namespace/class name conflict
  - `CreateAdminAccountAsync()` queries for `Role.Name == "Admin"` and assigns via UserRole join table

#### Step 6: App.xaml.cs Startup Logic
- ✅ Registered `NewLabDbContext` with connection string from `appsettings.json`
- ✅ Registered `IApplicationStartupService` as **Scoped** (not Singleton — see Critical Fixes below)
- ✅ Registered `IAuthService` as **Scoped**
- ✅ Registered `SetupViewModel` and `LoginViewModel` as **Transient**
- ✅ Removed `StartupUri="MainWindow.xaml"` from `App.xaml` — window creation is now fully programmatic
- ✅ `OnStartup` runs EF Core migration, then checks `IsFirstRunAsync()` to route to SetupView or LoginView

#### Step 7: SetupView (First-Time Wizard)
- ✅ Created `ViewModels/Pages/SetupViewModel.cs` with properties: FullName, Username, Password, ConfirmPassword, Email, PhoneNumber
- ✅ Created `Views/Windows/SetupView.xaml` — Arabic UI with MaterialDesign outlined text boxes
- ✅ Created `Views/Windows/SetupView.xaml.cs` with PasswordChanged event handlers for secure PasswordBox binding
- ✅ Added `OnSuccess` Action callback for window switching (close SetupView → open MainWindow)
- ✅ Added `MainWindow.Closed` event handler to return to LoginView after first-time setup

#### Step 8: LoginView (Standard Login)
- ✅ Created `ViewModels/Pages/LoginViewModel.cs` with properties: Username, Password, ShowPassword, RememberLogin
- ✅ Created `Views/Windows/LoginView.xaml` — Green/yellow two-tone theme, "New Lab System login" title, Enter key binding
- ✅ Created `Views/Windows/LoginView.xaml.cs` with PasswordChanged event handler and OnSuccess callback
- ✅ Added `MainWindow.Closed` event handler to return to LoginView after logout

#### Step 9: MainWindow
- ✅ Created `Views/Windows/MainWindow.xaml` — MaterialDesign styled shell with Arabic welcome text
- ✅ Removed old root-level `MainWindow.xaml` (superseded by Views/Windows/ version)

**Build Status**: 0 errors, 0 warnings

---

#### 🔧 Critical Fixes Applied During This Phase

**Fix 1: Disposed DbContext Error**
- **Problem**: `IApplicationStartupService` was registered as Singleton but held a reference to a Scoped `NewLabDbContext`. When the startup scope was disposed, subsequent service calls threw "Cannot access a disposed context instance."
- **Solution**: Changed `IApplicationStartupService` from Singleton to Scoped. Used a separate DI scope in `OnStartup` for initial database checks, then passed `_host.Services` (not the disposed scope) to window constructors.

**Fix 2: Application Closing After Admin Creation**
- **Problem**: Default `ShutdownMode="OnLastWindowClose"` caused the app to shut down when SetupView closed, before MainWindow could appear.
- **Solution**: Added `ShutdownMode="OnExplicitShutdown"` to `App.xaml`. Application only shuts down when `Application.Current.Shutdown()` is explicitly called.

**Fix 3: LoginView Text Not Visible**
- **Problem**: MaterialDesign `MaterialDesignOutlinedTextBox` style has its own template colors. Setting explicit `Background="#1565C0"` and `Foreground="White"` on the TextBox conflicted with the style, making typed text invisible.
- **Solution**: Removed explicit Background/Foreground attributes. Changed label text from White to dark green (`#1B5E20`) for contrast on yellow background.

**Fix 4: Fields Too Small / Text Cut Off**
- **Problem**: TextBox and PasswordBox heights of 35-45px were too small for MaterialDesign outlined controls, which need room for the floating label.
- **Solution**: Increased field heights to 55px. Increased window dimensions (LoginView: 500x550, SetupView: 700x550).

**Fix 5: No Window After Closing MainWindow**
- **Problem**: With `ShutdownMode="OnExplicitShutdown"`, closing MainWindow left the app running with no visible windows.
- **Solution**: Added `MainWindow.Closed` event handlers in both SetupView.xaml.cs and LoginView.xaml.cs. When MainWindow closes, a new LoginView is created and shown, allowing the user to log in again or exit.

**Fix 6: MaterialDesign Theme URI Error**
- **Problem**: `App.xaml` referenced `MaterialDesignTheme.Defaults.xaml` which does not exist in MaterialDesignThemes v5.x (it was renamed in v4.x+). Runtime crash: "Cannot locate resource 'themes/materialdesigntheme.defaults.xaml'."
- **Solution**: Replaced with `<materialDesign:BundledTheme BaseTheme="Light" PrimaryColor="Blue" SecondaryColor="Amber" />` and `MaterialDesign2.Defaults.xaml` as documented in the official MaterialDesignInXAML v5.x README.

**Fix 7: PasswordBox Not Binding to ViewModel**
- **Problem**: WPF `PasswordBox` does not support direct data binding for security reasons. The `Password` and `ConfirmPassword` ViewModel properties remained empty.
- **Solution**: Added `PasswordChanged` event handlers in both SetupView.xaml.cs and LoginView.xaml.cs that manually sync the PasswordBox value to the ViewModel property.

---

## 🎯 Next Steps (Phase 3)
**Status**: Planned  
**Goal**: Build core UI screens for business operations

**Planned Screens**:
1. Patient Management (Add, Edit, Delete, Search)
2. Lab Tests Management (Define tests, prices, normal ranges)
3. Test Orders (Create orders for patients)
4. Results Entry (Enter test results)
5. Reports (Print test results, invoices)

---

## 📝 Notes & Decisions

### Architecture Decisions
- **MVVM Pattern**: Chosen for clean separation of concerns and testability
- **MaterialDesign**: Selected for modern, professional UI suitable for medical applications
- **Mapster over AutoMapper**: Faster and more modern object mapping library
- **QuestPDF**: Modern PDF generation library (code-based, flexible)
- **Code-First Approach**: Database will be created from code models (better control)

### Commercial Considerations
- All configuration values in appsettings.json are empty (ready for customer configuration)
- System designed to be deployed to multiple laboratories
- Each lab will configure: name, address, contact info, currency, language

### Environment Setup
- Verified local SQL Server environment on 2026-07-22. Updated appsettings.json to point to the correct SQLEXPRESS instance instead of the default instance.

### Authentication & Security Decisions
- **BCrypt.Net-Next over PBKDF2**: BCrypt is industry-standard, automatically salts passwords, and provides stronger security guarantees for a commercial product sold to multiple laboratories
- **Scoped Service Lifetimes**: Services that use DbContext must be Scoped (not Singleton) to prevent disposed context errors in DI
- **OnExplicitShutdown**: Required for multi-window applications where windows are created/destroyed programmatically
- **PasswordBox Security**: WPF PasswordBox intentionally blocks data binding to prevent password exposure in memory; PasswordChanged event handlers are the recommended workaround
- **Design-Time DbContext Factory**: Required because the app uses IHost-based DI (no standard Program.cs), and EF Core tools need to instantiate the DbContext outside of DI for migrations
- **First User is Always Admin**: `AuthService.CreateAdminAccountAsync()` hardcodes `Role.Name == "Admin"` — there is no role selection in the setup wizard. This is intentional for initial system configuration.

### Application Flow (Post-Implementation)
```
App Start → Run EF Core Migration → Check IsFirstRunAsync()
  ├── DB Empty → SetupView → Create Admin → Close SetupView → MainWindow
  │                                                        └── Close MainWindow → LoginView
  └── DB Has Users → LoginView → Validate Credentials → Close LoginView → MainWindow
                                                           └── Close MainWindow → LoginView
```

### Database State (Post-Implementation)
- **Users Table**: Contains initial admin user (e.g., `ahmed` / Ahmed Magdy)
- **Roles Table**: Seeded with Admin (Id=1), Technician (Id=2), Receptionist (Id=3)
- **UserRoles Table**: Admin user linked to Admin role via composite key

---

## 🔧 Technical Details

### Build Configuration
- **Target Framework**: net8.0-windows
- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled

### Database Connection
- **Server**: SQLEXPRESS (MSSQL16.SQLEXPRESS)
- **Database**: NewLabDb
- **Authentication**: Windows Authentication (Trusted_Connection=True)
- **Trust Server Certificate**: True
- **Service Status**: Running
- **Connection String**: `Server=.\SQLEXPRESS;Database=NewLabDb;Trusted_Connection=True;TrustServerCertificate=True;`

---

**Last Updated**: 2026-07-21  
**Document Version**: 1.1

---

## ✅ Verification Report
**Verified Date**: 2026-07-21  
**Verified By**: AI Agent (Independent Verification)

### Verification Results:
- ✅ NuGet package versions verified against `dotnet list package`
- ✅ Project structure verified against actual folder structure
- ✅ File contents verified against actual implementations
- ✅ Build configuration verified against NewLab.csproj

### Corrections Made (if any):
None - all content was accurate

---
