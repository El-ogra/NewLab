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
| ZXing.Net | 0.16.11 | Barcode image generation (Code128 format) — see Exceptions below |
| ZXing.Net.Bindings.Windows.Compatibility | 0.16.14 | ZXing Windows System.Drawing bindings for .NET 8 |

---

## 🏗️ Project Structure

```
NewLab/
├── Assets/                     # Images, icons, logos, fonts
├── Converters/                 # IValueConverter implementations
│   └── InverseBoolToVisibilityConverter.cs
├── Data/                       # EF Core DbContext and design-time factories
│   ├── NewLabDbContext.cs      # DbContext with DbSets for User, Role, UserRole, Patient, Referral, SpecimenType, PatientVisit, TestGroup, LabTest, LabTestElement, ReferralPrice, NormalRange, BarcodeSettings, PatientCode
│   └── NewLabDbContextFactory.cs # Design-time factory for EF Core tooling
├── Helpers/                    # Extension methods, utility classes
│   └── BarcodeImageGenerator.cs # ZXing.Net CODE_128 barcode generation helper (NEW Phase 8)
├── Models/                     # Domain entities and DTOs
│   ├── Domain/                 # Rich domain models
│   │   ├── Enums/              # NEW (Phase 5)
│   │   │   ├── Gender.cs
│   │   │   ├── BillingSystem.cs
│   │   │   ├── AgeUnit.cs
│   │   │   ├── TestStatus.cs
│   │   │   └── CodeType.cs
│   │   ├── User.cs             # User entity (Id, Username, PasswordHash, FullName, etc.)
│   │   ├── Role.cs             # Role entity (Id, Name, Description)
│   │   ├── UserRole.cs         # Many-to-many join entity (UserId, RoleId)
│   │   ├── Patient.cs          # NEW (Phase 5) — 8 Boolean medical fields
│   │   ├── PatientVisit.cs     # NEW (Phase 5)
│   │   ├── Referral.cs         # NEW (Phase 5)
│   │   ├── SpecimenType.cs     # NEW (Phase 5)
│   │   ├── TestGroup.cs        # NEW (Phase 6)
│   │   ├── LabTest.cs          # NEW (Phase 6) — 25 properties, no Branch (Decision 5)
│   │   ├── LabTestElement.cs   # NEW (Phase 6)
│   │   ├── ReferralPrice.cs    # NEW (Phase 6) — Decision 15
│   │   ├── NormalRange.cs      # NEW (Phase 7) — 20 fields, decimal(18,4) precision, Gender=Male/Female only
│   │   ├── BarcodeSettings.cs  # NEW (Phase 8) — Id, OffsetX, OffsetY, PrintFileCodeWithAll, LabelWidth, LabelHeight
│   │   ├── BarcodeLabel.cs     # NEW (Phase 8) — Transient (no DbSet), barcode label layout data
│   │   └── PatientCode.cs      # NEW (Phase 8) — Patient code records (LabCode, FileCode)
│   ├── Validation/             # NEW (Phase 5)
│   │   ├── PatientValidator.cs
│   │   ├── LabTestValidator.cs # NEW (Phase 6)
│   │   └── NormalRangeValidator.cs # NEW (Phase 7)
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
│   │   ├── IAuthService.cs                # Login, password hashing, account creation
│   │   ├── ICurrentUserService.cs         # NEW (Phase 5) — current user tracking
│   │   ├── IPatientService.cs             # NEW (Phase 5)
│   │   ├── IReferralService.cs            # NEW (Phase 5)
│   │   ├── ILabTestService.cs             # NEW (Phase 6)
│   │   ├── INormalRangeService.cs         # NEW (Phase 7) — CRUD + GetMatchingRangeAsync + EvaluateValueAsync
│   │   ├── IBarcodeService.cs             # NEW (Phase 8) — Code generation + PatientCode records
│   │   └── IBarcodePrintService.cs        # NEW (Phase 8) — PDF barcode generation
│   ├── Implementations/        # Concrete implementations
│   │   ├── NavigationService.cs
│   │   ├── DialogService.cs
│   │   ├── ApplicationStartupService.cs   # Checks if Users table is empty
│   │   ├── AuthService.cs                 # BCrypt hashing, credential validation
│   │   ├── CurrentUserService.cs          # NEW (Phase 5) — Singleton
│   │   ├── PatientService.cs              # NEW (Phase 5), MODIFIED (Phase 6) — ReferralPrices query
│   │   ├── ReferralService.cs             # NEW (Phase 5)
│   │   ├── LabTestService.cs              # NEW (Phase 6)
│   │   ├── NormalRangeService.cs          # NEW (Phase 7) — Decision 16 (narrowest range wins)
│   │   ├── BarcodeService.cs              # NEW (Phase 8) — BranchConstant="1", BuildCode13
│   │   └── BarcodePrintService.cs         # NEW (Phase 8) — QuestPDF, LabelWidth×LabelHeight mm
│   └── Factories/              # ViewModel factories
├── ViewModels/                 # MVVM ViewModels
│   ├── Base/                   # ViewModelBase, shared base classes
│   │   └── ViewModelBase.cs
│   ├── Pages/                  # Page-level ViewModels
│   │   ├── SetupViewModel.cs   # First-time admin account creation
│   │   ├── LoginViewModel.cs   # Login form logic
│   │   ├── MainDashboardViewModel.cs  # Toolbar navigation shell
│   │   ├── IconNameToKindConverter.cs # Icon name to PackIconKind converter
│   │   ├── PatientEntryViewModel.cs   # NEW (Phase 5), MODIFIED (Phase 6) — ILabTestService injection, MODIFIED (Phase 8) — IBarcodeService + Func<BarcodeViewModel>
│   │   ├── LabTestManagementViewModel.cs  # NEW (Phase 6), MODIFIED (Phase 7) — Func<NormalRangeViewModel>
│   │   ├── NormalRangeViewModel.cs    # NEW (Phase 7) — 16 form fields, 7 commands, Decision 16 + 17
│   │   └── BarcodeViewModel.cs        # NEW (Phase 8) — 10 properties, 7 commands, drag-and-drop support
│   ├── Dialogs/                # Dialog ViewModels
│   └── Components/             # Reusable component ViewModels
├── Views/                      # WPF Views
│   ├── Pages/                  # NEW (Phase 5)
│   │   ├── PatientEntryView.xaml      # NEW (Phase 5), MODIFIED (Phase 6) — ListBox bindings, MODIFIED (Phase 8) — F11 InputBinding
│   │   ├── PatientEntryView.xaml.cs
│   │   ├── LabTestManagementView.xaml # NEW (Phase 6)
│   │   └── LabTestManagementView.xaml.cs # NEW (Phase 6)
│   ├── Dialogs/                # Modal dialogs
│   ├── Controls/               # Reusable UserControls
│   │   ├── DashboardContentControl.xaml/.cs
│   │   ├── FunctionPlaceholderControl.xaml/.cs
│   │   ├── LatinSymbolsPad.xaml       # NEW (Phase 6) — Decision 14
│   │   └── LatinSymbolsPad.xaml.cs    # NEW (Phase 6)
│   └── Windows/                # Shell windows
│       ├── SetupView.xaml/.cs  # First-time setup wizard
│       ├── LoginView.xaml/.cs  # Login screen (green/yellow theme)
│       ├── MainWindow.xaml/.cs # Main application shell
│       ├── NormalRangeView.xaml/.cs    # NEW (Phase 7) — 3-column RTL, Ranges list + Form + Commands
│       └── BarcodeView.xaml/.cs        # NEW (Phase 8) — 4-row RTL, Labels + Drag&Drop + Offset + Extra
├── Docs/                       # Project documentation
│   └── history.md              # This file
├── Migrations/                 # EF Core migrations
│   ├── 20260721171559_InitialCreate.cs
│   ├── 20260722032039_AddPatientsAndReferrals.cs  # NEW (Phase 5)
│   ├── 20260722063244_AddLabTestsAndReferralPrices.cs  # NEW (Phase 6)
│   ├── 20260722104729_AddNormalRanges.cs           # NEW (Phase 7)
│   └── 20260722110834_AddBarcodeSettingsAndPatientCodes.cs  # NEW (Phase 8)
├── App.xaml                    # Application resources, MaterialDesign theme, DataTemplates
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

### ✅ Phase 4: Main Dashboard Window (Navigation Shell)
**Status**: Completed  
**Date**: 2026-07-22

**Goal**: Build the main dashboard navigation shell with a top toolbar (11 category icons, RTL), a content area that switches between dashboard mode (function buttons grid) and function mode (placeholder with back button), and integrate it into the existing Login/Setup → MainWindow flow.

#### Part 1: NavigationService DI Rewrite
- ✅ Modified `Services/Interfaces/INavigationService.cs`
  - Relaxed generic constraint from `where TViewModel : ViewModelBase` to `where TViewModel : class`
  - Added `object? CurrentViewModel` property
- ✅ Modified `Services/Implementations/NavigationService.cs`
  - Added `IServiceProvider` constructor dependency
  - Replaced `Activator.CreateInstance<TViewModel>()` with `_serviceProvider.GetRequiredService<TViewModel>()`
  - Changed history stack from `Stack<ViewModelBase>` to `Stack<object>`

#### Part 2: Toolbar Model
- ✅ Created `Models/Domain/ToolbarItem.cs`
  - Properties: `PackIconKind IconKind`, `string Label`, `string Category`, `List<FunctionDefinition> Functions`

#### Part 3: Function Definition Model
- ✅ Created `Models/Domain/FunctionDefinition.cs`
  - Properties: `string Name`, `string IconName`, `Type? TargetViewType`

#### Part 4: MainDashboardViewModel
- ✅ Created `ViewModels/Pages/MainDashboardViewModel.cs`
  - 11 toolbar categories with MaterialDesign icons and Arabic labels
  - 4 Patients function buttons
  - Commands: SelectCategoryCommand, OpenFunctionCommand, CloseFunctionCommand, ExitCommand
  - State: IsToolbarVisible, IsDashboardMode, SelectedCategory, CurrentFunction

#### Part 5: MainWindow.xaml Redesign
- ✅ Replaced stub `Views/Windows/MainWindow.xaml`
  - Top toolbar: Border + ItemsControl + WrapPanel, RTL
  - 11 PackIcon buttons
  - Content area: two StackPanels toggled by IsDashboardMode
  - Dashboard mode: UniformGrid Columns="2" for function buttons
  - Function mode: placeholder with Back button
- ✅ Created `ViewModels/Pages/IconNameToKindConverter.cs`
- ✅ Created `Converters/InverseBoolToVisibilityConverter.cs`
- ✅ Modified `App.xaml` — added converter resource

#### Part 6: MainWindow.xaml.cs DI Integration
- ✅ Modified `Views/Windows/MainWindow.xaml.cs` — accepts IServiceProvider
- ✅ Modified `Views/Windows/LoginView.xaml.cs` — passes serviceProvider
- ✅ Modified `Views/Windows/SetupView.xaml.cs` — passes serviceProvider

#### Part 7: DashboardContentControl
- ✅ Created `Views/Controls/DashboardContentControl.xaml` + `.cs`

#### Part 8: FunctionPlaceholderControl
- ✅ Created `Views/Controls/FunctionPlaceholderControl.xaml` + `.cs`

#### Part 9: DI Container Registration
- ✅ Modified `App.xaml.cs` — added MainDashboardViewModel Transient

#### Part 10: Window Flow Integration
- ✅ Verified: Login → MainWindow → Exit → Login

#### Updated Project Structure
```
NewLab/
├── Converters/
│   └── InverseBoolToVisibilityConverter.cs  # NEW
├── Models/Domain/
│   ├── ToolbarItem.cs          # NEW
│   ├── FunctionDefinition.cs   # NEW
│   ...
├── ViewModels/Pages/
│   ├── MainDashboardViewModel.cs  # NEW
│   ├── IconNameToKindConverter.cs  # NEW
│   ...
├── Views/Controls/
│   ├── DashboardContentControl.xaml/.cs      # NEW
│   └── FunctionPlaceholderControl.xaml/.cs   # NEW
├── Views/Windows/
│   ├── MainWindow.xaml/.cs     # MODIFIED
│   ├── LoginView.xaml/.cs      # MODIFIED
│   └── SetupView.xaml.cs       # MODIFIED
└── Services/
    ├── Interfaces/INavigationService.cs  # MODIFIED
    └── Implementations/NavigationService.cs  # MODIFIED
```

**Build Status**: 0 errors, 0 warnings

---

### ✅ Phase 5: Function 1 — Prerequisite Fix & Full Execution
**Status**: Completed  
**Date**: 2026-07-22

**Goal**: Resolve CP-1/CP-2 (current-user tracking gap), then execute all 15 parts of Function 1 (Add/Edit Patient Data) per `Docs/Handoff_Slice_1_2.md`.

---

#### 🔧 Critical Fix 8: ICurrentUserService — Current User Tracking (CP-1/CP-2)

**Problem**: `IAuthService` only exposed `ValidateCredentialsAsync`, `HashPassword`, `VerifyPassword`, and `CreateAdminAccountAsync`. No API existed to query "who is the currently logged-in user" or their role after a successful login. `LoginViewModel.SignInAsync` received the `User` object from `ValidateCredentialsAsync` but discarded it immediately — never storing it anywhere accessible to the rest of the system. This prevented Decision 2 (Admin-only delete) from working correctly in Function 1 and all future functions (F3, F5, F6).

**Solution**: Created `ICurrentUserService` (interface + Singleton implementation) as an additive service — `IAuthService` was not modified.

**Files Created**:
1. ✅ `Services/Interfaces/ICurrentUserService.cs`
   - `User? CurrentUser { get; }` — nullable before login
   - `bool IsAdmin { get; }` — checks `UserRoles.Any(ur => ur.Role?.Name == "Admin")`
   - `void SetCurrentUser(User user)` — called after successful login
   - `void Clear()` — for logout

2. ✅ `Services/Implementations/CurrentUserService.cs`
   - Registered as **Singleton** (not Scoped) — user identity must persist across Scoped service resolutions for the entire session lifetime

**Files Modified**:
1. ✅ `App.xaml.cs` — added `services.AddSingleton<ICurrentUserService, CurrentUserService>()`
2. ✅ `ViewModels/Pages/LoginViewModel.cs`
   - Added `ICurrentUserService` constructor dependency
   - In `SignInAsync`: `_currentUserService.SetCurrentUser(user)` called before `OnSuccess?.Invoke()` — user is no longer discarded

**Build Status**: 0 errors, 0 warnings

---

#### Function 1 Execution: 15/15 Parts Completed

All 15 parts from `Docs/Handoff_Slice_1_2.md` executed sequentially with build verification after each.

##### Files Created (20 files)

```
Models/Domain/Enums/Gender.cs              # Male, Female (no Both — Decision 17)
Models/Domain/Enums/BillingSystem.cs        # Individual, LabToLab, Free
Models/Domain/Enums/AgeUnit.cs              # Day, Month, Year
Models/Domain/Enums/TestStatus.cs           # New, Entered, Reviewed, Printed, Delivered, AccountIssue, Completed
Models/Domain/Enums/CodeType.cs             # Case, File, Lab
Models/Domain/Referral.cs                   # Id, Name, DiscountPercent, IsDefaultLab, CreatedAt
Models/Domain/SpecimenType.cs               # Id, Name, ArabicName
Models/Domain/Patient.cs                    # 8 Boolean medical fields (Decision 1), all properties per spec
Models/Domain/PatientVisit.cs               # Id, PatientId, VisitDate, DailySequenceNumber, FullVisitCode
Models/Validation/PatientValidator.cs       # FluentValidation: FullName, Age ranges by AgeUnit, Gender, FastingHours
Services/Interfaces/IPatientService.cs      # CRUD + CalculateTotalAsync + PatientTestRow record
Services/Interfaces/IReferralService.cs     # SearchByNameAsync, GetOrCreateAsync, GetDefaultLabAsync
Services/Implementations/PatientService.cs  # Full CRUD, Admin-check on DeleteAsync via ICurrentUserService
Services/Implementations/ReferralService.cs # Autocomplete, auto-create, default lab retrieval
ViewModels/Pages/PatientEntryViewModel.cs   # Full VM with all properties, commands, validation
ViewModels/Pages/LabTestPlaceholder.cs      # Temporary record until Function 7
Views/Pages/PatientEntryView.xaml           # 3-column layout: Patient data | Available tests | Selected tests + actions
Views/Pages/PatientEntryView.xaml.cs        # Minimal code-behind (InitializeComponent + ComboBox population)
Migrations/<timestamp>_AddPatientsAndReferrals.cs           # EF Core migration
Migrations/<timestamp>_AddPatientsAndReferrals.Designer.cs  # Migration snapshot
```

##### Files Modified (6 files)

```
Data/NewLabDbContext.cs                     # +4 DbSets + Fluent API (6 relationships, 2 indexes, 3 decimal precision) + Seed default Referral
App.xaml                                    # +DataTemplate mapping PatientEntryViewModel → PatientEntryView
App.xaml.cs                                 # +4 Scoped services (IPatientService, IReferralService, IValidator<Patient>) + 1 Transient VM
Views/Windows/MainWindow.xaml               # +Window.InputBindings (F2) + ContentControl replacing placeholder text
ViewModels/Pages/MainDashboardViewModel.cs  # +CurrentFunctionView property + OpenFunction navigation logic + OpenPatientEntryCommand + TargetViewType on Patient function
```

##### Decisions Applied

| Decision | Implementation |
|----------|---------------|
| **Decision 1** | 8 Boolean medical columns on Patient entity (IsFasting, IsOnAnticoagulant, HasLiverTreatment, HasAntiviralTreatment, HasAntibiotic, IsPregnant, IsSmoker, FastingHours) — no separate table, no text field |
| **Decision 2** | `PatientService.DeleteAsync` checks `_currentUserService.IsAdmin` before proceeding; throws `UnauthorizedAccessException` if not Admin; `DeleteCommand.CanExecute` bound to `CanDelete => IsAdmin` |
| **Decision 15** | `CalculateTotalAsync` signature accepts `Referral?` parameter for future ReferralPrices lookup (F7) — in F1 uses `PatientTestRow.Price` directly |
| **Decision 17** | `Gender` enum contains only `Male` and `Female` — no `Both` value |

**Build Status**: 0 errors, 0 warnings (verified after each part)

---

### ✅ Phase 6: Function 7 — Lab Test Definitions & Pricing
**Status**: Completed  
**Date**: 2026-07-22

**Goal**: Execute all 11 parts of Function 7 (Add/Edit Lab Test Data) per `Docs/Handoff_Slice_7_2.md`, including retro-integration with Function 1 (PatientService.CalculateTotalAsync ReferralPrices lookup, LabTestPlaceholder replacement).

---

#### Function 7 Execution: 11/11 Parts Completed

All 11 parts executed sequentially with build verification after each.

##### Files Created (14 files)

```
Models/Domain/TestGroup.cs               # Id, Name, LogGroup, ICollection<LabTest> Tests
Models/Domain/LabTest.cs                 # 25 properties (no Branch — Decision 5), navigation props
Models/Domain/LabTestElement.cs          # Id, ParentLabTestId, Name, ArrangeNumber, IsMainTest
Models/Domain/ReferralPrice.cs           # Id, LabTestId, ReferralId, Price (Decision 15)
Models/Validation/LabTestValidator.cs    # FluentValidation: Code, TestName, ReportNameLarge, prices ≥ 0
Services/Interfaces/ILabTestService.cs   # CRUD + SearchAsync + GetRoutineTestsAsync + ReferralPrices methods
Services/Implementations/LabTestService.cs # Full impl with ICurrentUserService.Admin check on DeleteAsync
ViewModels/Pages/LabTestManagementViewModel.cs # Full VM: 17 form fields, search, referral prices, commands
Views/Controls/LatinSymbolsPad.xaml      # Reusable UserControl (Decision 14): α β γ μ ± ≤ ≥ °
Views/Controls/LatinSymbolsPad.xaml.cs   # DependencyProperty TargetTextBox + Symbols
Views/Pages/LabTestManagementView.xaml   # 3-column RTL layout: test list | form | referral prices
Views/Pages/LabTestManagementView.xaml.cs # Minimal code-behind
Migrations/<timestamp>_AddLabTestsAndReferralPrices.cs          # EF Core migration
Migrations/<timestamp>_AddLabTestsAndReferralPrices.Designer.cs # Migration snapshot
```

##### Files Modified (5 files)

```
Data/NewLabDbContext.cs                  # +4 DbSets + Fluent API (10 configurations) + Seed (3 TestGroups, 3 LabTests)
App.xaml                                 # +DataTemplate LabTestManagementViewModel → LabTestManagementView
App.xaml.cs                              # +Scoped ILabTestService, +Scoped IValidator<LabTest>, +Transient LabTestManagementViewModel
ViewModels/Pages/MainDashboardViewModel.cs # +FunctionDefinition "بيانات التحاليل" in SystemData + OpenFunction branch
Services/Implementations/PatientService.cs # CalculateTotalAsync: async + _context.ReferralPrices lookup (retro F1)
```

##### Files Deleted (1 file)

```
ViewModels/Pages/LabTestPlaceholder.cs   # Replaced by real LabTest entity
```

##### Retro-Integration with Function 1

| Change | File | What Changed |
|--------|------|-------------|
| PatientService.CalculateTotalAsync | `Services/Implementations/PatientService.cs` | LabToLab branch now queries `_context.ReferralPrices` with fallback to `test.Price`; method converted to async |
| PatientEntryViewModel.AvailableTests | `ViewModels/Pages/PatientEntryViewModel.cs` | Type changed from `ObservableCollection<LabTestPlaceholder>` to `ObservableCollection<LabTest>` |
| PatientEntryViewModel constructor | `ViewModels/Pages/PatientEntryViewModel.cs` | Added `ILabTestService` injection + `_ = LoadAvailableTestsAsync()` (fire-and-forget per CP-2) |
| AddSelectedTestCommand | `ViewModels/Pages/PatientEntryViewModel.cs` | Implemented: reads `SelectedAvailableTest`, builds `PatientTestRow` by BillingSystem, adds to `SelectedTests` |
| RemoveTest signature | `ViewModels/Pages/PatientEntryViewModel.cs` | Changed from `RemoveTest(LabTestPlaceholder?)` to `RemoveTest(LabTest?)` |
| PatientEntryView ListBox | `Views/Pages/PatientEntryView.xaml` | Added `DisplayMemberPath="TestName"` and `SelectedItem="{Binding SelectedAvailableTest}"` |
| LabTestPlaceholder.cs | Deleted | No longer needed |

##### Decisions Applied

| Decision | Implementation |
|----------|---------------|
| **Decision 5** | No `Branch` field on `LabTest`, `TestGroup`, or anywhere in F7 — branch fixed to "1" in code |
| **Decision 14** | `LatinSymbolsPad` created as independent reusable `UserControl` with `DependencyProperty TargetTextBox` and `DependencyProperty Symbols` (extensible) |
| **Decision 15** | `ReferralPrice` entity created, `ReferralPrices` DbSet + table in DB, `ILabTestService` has `GetReferralPricesAsync`/`SetReferralPriceAsync`/`DeleteReferralPriceAsync`, `PatientService.CalculateTotalAsync` queries real `ReferralPrices` table |

##### Clarification Points Applied

| CP | Decision | Implementation |
|----|----------|---------------|
| **CP-1** (Back button) | No separate `BackCommand` in `LabTestManagementViewModel` | Relies on existing generic "العودة للرئيسية" button in MainWindow (bound to `CloseFunctionCommand`) |
| **CP-2** (Load AvailableTests) | Fire-and-forget from constructor | `_ = LoadAvailableTestsAsync();` in `PatientEntryViewModel` constructor — matches existing `_ = RecalculateTotalAsync();` pattern |
| **CP-3** (Admin-only delete) | Yes — Admin only | `LabTestService.DeleteAsync` checks `_currentUserService.IsAdmin`; `DeleteCommand.CanExecute` bound to `IsAdmin` in VM |

**Build Status**: 0 errors, 0 warnings (verified after each part)

---

#### Database Migration: AddLabTestsAndReferralPrices

##### Migration Creation
- ✅ EF Core migration created via `dotnet ef migrations add AddLabTestsAndReferralPrices -c NewLabDbContext`
- ✅ `ProductVersion` in migration and snapshot: `8.0.8` (project version)
- ✅ Tables created: TestGroups, LabTests, LabTestElements, ReferralPrices
- ✅ Unique indexes: `LabTests.Code`, `ReferralPrices(LabTestId, ReferralId)`
- ✅ Seed data: 3 TestGroups (Chemistry, Hematology, Urine), 3 LabTests (Glucose, Hemoglobin, Urine Analysis)

##### Migration Application
- ✅ Migration applied via `dotnet ef database update -c NewLabDbContext`

##### Verification
```
Migration list:
- 20260721171559_InitialCreate
- 20260722032039_AddPatientsAndReferrals
- 20260722063244_AddLabTestsAndReferralPrices

Tables confirmed (via generated SQL script):
- TestGroups ✅
- LabTests ✅
- LabTestElements ✅
- ReferralPrices ✅
```

#### Technical Notes & Issues Resolved During Execution

##### 1. Circular Dependency: Patient ↔ PatientVisit
- **Issue**: `Patient.Visits` navigation property requires `PatientVisit` type, but `PatientVisit.Patient` requires `Patient` type. Parts 1.3 and 1.4 cannot be built independently.
- **Resolution**: Created both entities in the same build step. Part 1.3 and 1.4 were verified together.

##### 2. Name Collision: `LabId` Property vs `LabId` Command
- **Issue**: `[ObservableProperty] private string? labId` generates a property named `LabId`. `[RelayCommand] private void LabId()` also targets a name that conflicts with the generated property in the CommunityToolkit.Mvvm source generator.
- **Resolution**: Renamed the command method to `LookupLabId()`, generating `LookupLabIdCommand` instead.

##### 3. MaterialDesign XAML Tag Not Found (MC3074)
- **Issue**: `<materialDesign:MaterialDesignOutlinedTextBox>` does not exist as a direct XAML element in MaterialDesignThemes 5.x. Initial XAML contained this invalid tag.
- **Resolution**: Replaced all instances with the correct pattern: `<TextBox Style="{StaticResource MaterialDesignOutlinedTextBox}" materialDesign:HintAssist.Hint="..." />` — consistent with the existing pattern in `LoginView.xaml`.

##### 4. Enum Values in XAML x:Array
- **Issue**: `x:Array` with enum types (e.g., `enums:Gender`, `enums:AgeUnit`) fails XAML compilation — the XAML parser cannot resolve enum values inline.
- **Resolution**: Removed enum arrays from XAML. ComboBox items populated in code-behind (`PatientEntryView.xaml.cs`) by setting `ItemsSource` to explicit enum value arrays.

##### 5. Incremental Build Hiding Warnings
- **Issue**: `dotnet build` (incremental) showed 0 warnings, but `Clean + Rebuild` revealed the MC3074 XAML warning. Incremental builds skip XAML recompilation when .baml cache is valid.
- **Resolution**: After fixing the XAML, verified with explicit `dotnet clean` then `dotnet build` — confirmed 0 errors, 0 warnings on full rebuild.

##### 6. dotnet-ef 10.0.6 vs EF Core 8.0.8 Compatibility
- **Issue**: Global tool `dotnet-ef` version 10.0.6 is newer than the project's EF Core 8.0.8.
- **Resolution**: Migration generated correctly — `ProductVersion` in both the migration Designer.cs and ModelSnapshot.cs reads `8.0.8` (the project version, not the tool version). No compatibility warnings produced.

---

#### Database Migration: AddPatientsAndReferrals

##### Migration Creation
- ✅ EF Core migration `AddPatientsAndReferrals` created via `dotnet ef migrations add AddPatientsAndReferrals -c NewLabDbContext`
- ✅ `ProductVersion` in migration and snapshot: `8.0.8` (project version)
- ✅ Tables created: Referrals, SpecimenTypes, Patients, PatientVisits
- ✅ Seed data: Default Referral ("المعمل", IsDefaultLab=true, DiscountPercent=0, CreatedAt=2026-01-01)
- ✅ Fluent API: 6 relationships (3 Restrict, 1 Cascade, 2 required), 2 indexes (LabId unique filtered, FileCode unique), 4 decimal precision configurations

##### Migration Application
- ✅ Migration applied via `Update-Database` in Package Manager Console
- ✅ `App.OnStartup` contains `await dbContext.Database.MigrateAsync()` — auto-apply on app start

##### Verification (SQL Direct Queries)
```
Query: SELECT name FROM sys.tables 
       WHERE name IN ('Patients','Referrals','SpecimenTypes','PatientVisits') 
       ORDER BY name;
Result: 4 rows — Patients, PatientVisits, Referrals, SpecimenTypes

Query: SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;
Result: 2 rows — 20260721171559_InitialCreate (8.0.8), 
                  20260722032039_AddPatientsAndReferrals (8.0.8)
```

---

### ✅ Phase 7: Function 8 — Normal Range Management
**Status**: Completed  
**Date**: 2026-07-22

**Goal**: Execute all 10 parts of Function 8 (Add/Edit Normal Ranges per test) per `Docs/Handoff_Slice_8&2_2.md`, including retro-integration with Function 7 (LabTestManagementViewModel → NormalRangeView).

---

#### Function 8 Execution: 10/10 Parts Completed

All 10 parts executed sequentially with build verification after each.

##### Files Created (9 files)

```
Models/Domain/NormalRange.cs              # 20 fields + LabTest nav, decimal(18,4) precision, Gender=Male/Female only (Decision 17)
Models/Validation/NormalRangeValidator.cs # FluentValidation: LabTestId, TestName, Gender (IsInEnum + Male/Female), Low≤High, AgeFrom≤AgeTo, CriticalLow≤Low, CriticalHigh≥High
Services/Interfaces/INormalRangeService.cs # CRUD + GetMatchingRangeAsync + EvaluateValueAsync + NormalRangeEvaluation record
Services/Implementations/NormalRangeService.cs # Full impl: Admin-check on DeleteAsync, Decision 16 (narrowest range wins), ConvertAgeToUnit
ViewModels/Pages/NormalRangeViewModel.cs  # Full VM: 16 form fields, AvailableGenders (Male/Female only), 7 commands, LoadForTest
Views/Windows/NormalRangeView.xaml        # 3-column RTL Window (700×1000): Ranges list | Form (Normal+Critical sections) | Commands + LatinSymbolsPad
Views/Windows/NormalRangeView.xaml.cs     # InitializeComponent + Gender/AgeUnit ComboBox population (Technical Note 4)
Migrations/20260722104729_AddNormalRanges.cs           # EF Core migration
Migrations/20260722104729_AddNormalRanges.Designer.cs  # Migration snapshot
```

##### Files Modified (3 files)

```
Data/NewLabDbContext.cs                  # +DbSet<NormalRange> + Fluent API (HasOne→WithMany, composite index, 4×decimal(18,4))
App.xaml.cs                              # +Scoped INormalRangeService, +Scoped IValidator<NormalRange>, +Transient NormalRangeViewModel (Parts 8.5, 8.8)
ViewModels/Pages/LabTestManagementViewModel.cs # RETRO: Func<NormalRangeViewModel> injected, OpenNormalRange→OpenNormalRangeAsync (Part 8.8)
```

##### Files Deleted (0 files)

No files were deleted in Function 8.

##### Retro-Integration with Function 7

| Change | File | What Changed |
|--------|------|-------------|
| `OpenNormalRange` → `OpenNormalRangeAsync` | `ViewModels/Pages/LabTestManagementViewModel.cs` | Placeholder `_dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 8")` replaced with: null-check on SelectedTest → factory-create NormalRangeVM → LoadForTest → ShowDialog |
| Constructor added `Func<NormalRangeViewModel>` | `ViewModels/Pages/LabTestManagementViewModel.cs` | Factory pattern — NormalRangeViewModel registered as Transient in DI, container resolves Func<T> automatically |
| DI registration | `App.xaml.cs` | `services.AddTransient<NormalRangeViewModel>()` added |

##### Decisions Applied

| Decision | Implementation |
|----------|---------------|
| **Decision 14** | `LatinSymbolsPad` UserControl reused in NormalRangeView (Column 2) — no modification needed |
| **Decision 16** | `GetMatchingRangeAsync`: filters by Gender + Age range, then `OrderBy(nr => nr.AgeTo - nr.AgeFrom).First()` — narrowest matching range wins |
| **Decision 17** | `AvailableGenders` = Male/Female only — no `Both`; enforced in entity, validator, and ViewModel |
| **CP-F8-1** | No navigation collection on `LabTest` — access only via `INormalRangeService.GetForTestAsync(labTestId)` |
| **CP-F8-2** | `TestUnit` is manual entry per range, inherited as default from first range for same test |
| **CP-F8-3** | `NormalRangeText` is fully manual, no auto-generation from LowLimit/HighLimit |

**Build Status**: 0 errors, 0 warnings (verified after each part)

---

### ✅ Phase 8: Function 2 — Barcode Printing & Patient Codes
**Status**: Completed  
**Date**: 2026-07-22

**Goal**: Execute all 13 parts of Function 2 (Barcode generation, printing, and settings) per `Docs/Handoff_Slice_8&2_2.md`, including retro-integration with Function 1 (PatientEntryViewModel → BarcodeView).

---

#### Function 2 Execution: 13/13 Parts Completed

All 13 parts executed sequentially with build verification after each.

##### Files Created (12 files)

```
Models/Domain/BarcodeSettings.cs          # Id, OffsetX, OffsetY, PrintFileCodeWithAll, LabelWidth=38, LabelHeight=25 (Decision 4), no BranchNumber (Decision 5)
Models/Domain/BarcodeLabel.cs             # Transient (no DbSet): PatientId, PatientName, SpecimenTypeId, SpecimenName, Tests, Code, Type
Models/Domain/PatientCode.cs              # Id, PatientId, CodeType, CodeValue, IssuedAt, Patient nav — per CP-F2-2
Services/Interfaces/IBarcodeService.cs    # 7 methods: GenerateCaseCode, GenerateFileCode, GenerateLabCode, GetOrCreateLabIdAsync, GetLabelsForPatientAsync, GetSettingsAsync, SaveSettingsAsync
Services/Implementations/BarcodeService.cs # BranchConstant="1", BuildCode13 (13-char format), PatientCode records in GetOrCreateLabIdAsync
Services/Interfaces/IBarcodePrintService.cs # GeneratePdf(labels, settings) → byte[]
Services/Implementations/BarcodePrintService.cs # QuestPDF Community license, LabelWidth×LabelHeight mm pages, BarcodeImageGenerator integration, BitmapSourceExtensions helper
Helpers/BarcodeImageGenerator.cs          # Static helper: ZXing.Net CODE_128, BarcodeWriter<Bitmap>, ConvertToBitmapSource
ViewModels/Pages/BarcodeViewModel.cs      # Full VM: 10 properties, 7 commands (PrintFileCode, PrintLabCode, PrintLabel, PrintAll, SaveSettings, AddExtraBarcode, RemoveLabel), CanPrintLabCode=LabId!=null
Views/Windows/BarcodeView.xaml            # 4-row RTL Window (700×900): Print buttons + CheckBox | Labels ItemsControl + Drag&Drop | Offset Sliders + LabelWidth/Height + Save | Extra barcode inputs
Views/Windows/BarcodeView.xaml.cs         # InitializeComponent + Drag&Drop handlers (MouseLeftButtonDown, DragOver, Drop) — pure UI logic in code-behind per CP-F2-4
Migrations/20260722110834_AddBarcodeSettingsAndPatientCodes.cs           # EF Core migration
Migrations/20260722110834_AddBarcodeSettingsAndPatientCodes.Designer.cs  # Migration snapshot
```

##### Files Modified (4 files)

```
Data/NewLabDbContext.cs                  # +DbSet<BarcodeSettings>, +DbSet<PatientCode> + Fluent API (seed, relationships, indexes) (Part 2.3)
App.xaml.cs                              # +Scoped IBarcodeService, +Scoped IBarcodePrintService, +Transient BarcodeViewModel (Parts 2.6, 2.8, 2.11)
NewLab.csproj                            # +ZXing.Net 0.16.11, +ZXing.Net.Bindings.Windows.Compatibility 0.16.14 (Part 2.7)
ViewModels/Pages/PatientEntryViewModel.cs # RETRO: IBarcodeService + Func<BarcodeViewModel> injected, PrintBarcode→PrintBarcodeAsync, LookupLabId→LookupLabIdAsync (Part 2.12)
```

##### Files Deleted (0 files)

No files were deleted in Function 2.

##### Retro-Integration with Function 1

| Change | File | What Changed |
|--------|------|-------------|
| `PrintBarcode` → `PrintBarcodeAsync` | `ViewModels/Pages/PatientEntryViewModel.cs` | Placeholder replaced with: null-check on patient → factory-create BarcodeVM → LoadForPatientAsync → ShowDialog |
| `LookupLabId` → `LookupLabIdAsync` | `ViewModels/Pages/PatientEntryViewModel.cs` | Placeholder replaced with: null-check → check existing LabId → `_barcodeService.GetOrCreateLabIdAsync(patient)` → set LabId property |
| Constructor added `IBarcodeService` + `Func<BarcodeViewModel>` | `ViewModels/Pages/PatientEntryViewModel.cs` | Factory pattern — same as Phase 7 NormalRangeViewModel pattern |
| InputBinding F11 | `Views/Pages/PatientEntryView.xaml` | UNCHANGED — `PrintBarcodeCommand` generated by CommunityToolkit from `PrintBarcodeAsync` (same name, no break) |

##### Exceptions & Technical Decisions

| Part | What Changed | Why | Impact |
|------|-------------|-----|--------|
| **Part 2.5** — BarcodeService Constructor | Constructor receives `NewLabDbContext context` only (no `IPatientService`) | All Services in this project use DbContext directly — no Service injects another Service. Adding `IPatientService` would create the only exception to this pattern. Patient operations (`_context.Patients.FirstOrDefaultAsync`, `_context.Patients.Update`) are simple and don't need the abstraction layer. | No functional impact — identical behavior. Architecturally consistent with PatientService, LabTestService, NormalRangeService, etc. |
| **Part 2.7** — ZXing.Net version | Installed `ZXing.Net` 0.16.11 instead of Handoff-specified 0.16.9 | `ZXing.Net.Bindings.Windows.Compatibility 0.16.14` requires `ZXing.Net >= 0.16.11` for net8.0 target (enforced by NuGet dependency resolution). The Handoff specification of 0.16.9 + Bindings 0.16.14 is internally contradictory for .NET 8. Version 0.16.11 is a maintenance release (bug fixes only, no API changes) published 2025-10-26. | No functional impact — API compatible. Downgrading to 0.16.9 would cause NuGet restore failure. |

##### Decisions Applied

| Decision | Implementation |
|----------|---------------|
| **Decision 3** | `ZXing.Net` exclusively — no BarcodeStandard or other libraries |
| **Decision 4** | `LabelWidth=38mm, LabelHeight=25mm` defaults in BarcodeSettings seed + BarcodeViewModel; customizable via TextBoxes in BarcodeView |
| **Decision 5** | `BranchConstant = "1"` — no BranchNumber column in DB (verified: 0 columns), branch position hardcoded in BuildCode13 |
| **CP-F2-1** | DailySequenceNumber = count of today's actual visits + 1 (computed in GenerateCaseCode from PatientVisit) |
| **CP-F2-2** | PatientCode entity created (not deferred to F6) — migration now, simpler than retro-integration later |
| **CP-F2-3** | `QuestPDF.Settings.License = LicenseType.Community` — suitable for <$1M revenue |
| **CP-F2-4** | Drag & Drop via code-behind event handlers in BarcodeView (not attached behaviors) — pure visual behavior, no DB state change |

**Build Status**: 0 errors, 0 warnings (verified after each part)

---

### ✅ Phase 9: Function 3 — Test Results List
**Status**: Completed  
**Date**: 2026-07-22

**Goal**: Execute all 10 parts of Function 3 (Test Results List — قائمة نتائج التحاليل) per `Docs/Handoff_Slice_3&4.md`, including retro-integration with Function 1 (PatientEntryViewModel.TodayPatients → TestResultsListView).

---

#### Function 3 Execution: 10/10 Parts Completed

All 10 parts executed sequentially with build verification after each.

##### Files Created (10 files)

```
Models/Domain/PatientTest.cs                      # Part 3.1 — FK PatientVisitId+LabTestId, Status, IsReviewed/IsPrinted/IsDelivered, Price decimal(18,2)
Models/Domain/AuditLog.cs                         # Part 3.2 — EntityName, EntityId, Action, UserId, Timestamp, Details
Services/Interfaces/ITestResultsListService.cs    # Part 3.4 — PatientListItem record + 12 methods
Services/Implementations/TestResultsListService.cs # Part 3.5 — Full implementation with audit logging
Converters/TestStatusToIconConverter.cs           # Part 3.6 — TestStatus→PackIconKind + color converter
ViewModels/Pages/TestResultsListViewModel.cs      # Part 3.7 — 10 properties, 15 commands, Admin gates on Audit/Financial
Views/Pages/TestResultsListView.xaml              # Part 3.8 — 3-column RTL layout
Views/Pages/TestResultsListView.xaml.cs           # Part 3.8 — Minimal code-behind
```

Plus 2 auto-generated migration files:
```
Migrations/<timestamp>_AddPatientTestsAndAuditLogs.cs           # Part 3.3
Migrations/<timestamp>_AddPatientTestsAndAuditLogs.Designer.cs  # Part 3.3
```

##### Files Modified (7 files)

```
Data/NewLabDbContext.cs                  # +DbSet<PatientTest>, +DbSet<AuditLog> + Fluent API (FK, indexes, decimal precision)
Models/Domain/PatientVisit.cs            # +ICollection<PatientTest> PatientTests navigation property
App.xaml.cs                              # +Scoped ITestResultsListService, +Transient TestResultsListViewModel
App.xaml                                 # +DataTemplate TestResultsListViewModel → TestResultsListView
ViewModels/Pages/MainDashboardViewModel.cs # +TargetViewType=typeof(TestResultsListView), +OpenTestResultsListCommand, +F4 branch in OpenFunction
Views/Windows/MainWindow.xaml            # +KeyBinding F4 → OpenTestResultsListCommand
ViewModels/Pages/PatientEntryViewModel.cs # TodayPatients(): placeholder → _navigationService.NavigateTo<TestResultsListViewModel>()
```

##### Retro-Integration with Function 1

| Change | File | What Changed |
|--------|------|-------------|
| `TodayPatients()` body | `ViewModels/Pages/PatientEntryViewModel.cs` | Placeholder message replaced with `_navigationService.NavigateTo<TestResultsListViewModel>()` |

##### Decisions Applied

| Decision | Implementation |
|----------|---------------|
| **Decision 6** | `SearchByAttendanceNumberAsync` = `PatientVisit.DailySequenceNumber == number` for the requested date — simple daily serial, no composite encoding |
| **Decision 7** | `ShowAuditCommand.CanExecute = IsAdmin`, `ShowFinancialTrackingCommand.CanExecute = IsAdmin` — Admin-only buttons in F3 |
| **CP-F3-1** | No new `PatientTestStatus` enum — reused existing `Enums/TestStatus.cs` (7 values) |
| **CP-F3-2** | `PrintReceipt()` placeholder left unchanged in PatientEntryViewModel |
| **CP-F3-3** | `TodayPatients()` in PatientEntryViewModel now opens TestResultsListView |

**Build Status**: 0 errors, 0 warnings (verified after each part)

---

### ✅ Phase 10: Function 4 — Test Result Entry
**Status**: Completed  
**Date**: 2026-07-22

**Goal**: Execute all 11 parts of Function 4 (Test Result Entry — إدخال نتائج التحاليل) per `Docs/Handoff_Slice_3&4.md`, including retro-integration with Function 3 (TestResultsListViewModel.OpenTestEntry → TestResultEntryView).

---

#### Function 4 Execution: 11/11 Parts Completed

All 11 parts executed sequentially with build verification after each.

##### Files Created (12 files)

```
Models/Domain/TestResult.cs                        # Part 4.1 — FK PatientTestId+LabTestElementId, Value, Unit, IsAbnormal/IsCritical, FlagText
Models/Domain/SavedComment.cs                      # Part 4.1 — FK LabTestId, CommentText, Type (Low/High/Critical/General)
Models/Domain/CalculationConstant.cs               # Part 4.5 — TestType, ConstantName, ConstantValue decimal(18,6)
Models/Validation/TestResultValidator.cs           # Part 4.6 — FluentValidation: PatientTestId, LabTestElementId, Value, Comment
Services/Interfaces/ITestResultEntryService.cs     # Part 4.4 — GetPatientTestWithProfile, SaveResults, EvaluateResult, MarkReviewed, Preview/PrintReport
Services/Interfaces/IAutoCalculationService.cs     # Part 4.5 — CalculateHct, CalculateHgbPercent, CalculateINR, CalculatePTTRatio, Get/UpdateConstants
Services/Interfaces/IReportPdfGenerator.cs         # Part 4.7 — GenerateAsync(int patientTestId) → byte[]
Services/Implementations/AutoCalculationService.cs # Part 4.5 — Reads CalculationConstants from DB, applies formulas
Services/Implementations/TestResultEntryService.cs # Part 4.6 — Transaction-based save, delegates to INormalRangeService for evaluation
Services/Implementations/ReportPdfGenerator.cs     # Part 4.7 — QuestPDF Community license, RTL report with patient data + results table
ViewModels/Pages/TestResultEntryViewModel.cs       # Part 4.8 — LoadForPatientTest, Save/Print/Preview/Reviewed commands, TestResultRow class
ViewModels/Pages/CalculationConstantsViewModel.cs  # Part 4.5 — LoadConstants, Save command
Views/Windows/TestResultEntryView.xaml             # Part 4.9 — Window modal (ShowDialog), 3-row: patient info + DataGrid + commands
Views/Windows/TestResultEntryView.xaml.cs          # Part 4.9 — Minimal code-behind
Views/Windows/CalculationConstantsView.xaml        # Part 4.5 — Window modal, DataGrid for editing constants
Views/Windows/CalculationConstantsView.xaml.cs     # Part 4.5 — Close_Click handler
```

Plus 2 auto-generated migration files:
```
Migrations/<timestamp>_AddTestResultsAndConstants.cs           # Part 4.3
Migrations/<timestamp>_AddTestResultsAndConstants.Designer.cs  # Part 4.3
```

##### Files Modified (3 files)

```
Data/NewLabDbContext.cs                  # +DbSet<TestResult>, +DbSet<SavedComment>, +DbSet<CalculationConstant> + Fluent API + Seed 8 CalculationConstants
App.xaml.cs                              # +Scoped ITestResultEntryService, IAutoCalculationService, IReportPdfGenerator, IValidator<TestResult> + Transient TestResultEntryViewModel, CalculationConstantsViewModel
ViewModels/Pages/TestResultsListViewModel.cs # +Func<TestResultEntryViewModel> factory, OpenTestEntry → ShowDialog pattern
```

##### Retro-Integration with Function 3

| Change | File | What Changed |
|--------|------|-------------|
| Constructor added `Func<TestResultEntryViewModel>` | `ViewModels/Pages/TestResultsListViewModel.cs` | Factory pattern — same as Phase 7 NormalRangeViewModel pattern |
| `OpenTestEntryCommand` body | `ViewModels/Pages/TestResultsListViewModel.cs` | Placeholder → factory + LoadForPatientTestAsync + ShowDialog + refresh |
| DI registration | `App.xaml.cs` | `services.AddTransient<TestResultEntryViewModel>()` |

##### Decisions Applied

| Decision | Implementation |
|----------|---------------|
| **Decision 8** | `IAutoCalculationService` — INR = (ptPatient/controlTime)^ISI, PTT Ratio = pttPatient/controlTime — constants from CalculationConstants table |
| **Decision 9** | No custom history window — only basic "تاريخ مرضي" button (no `OpenCustomHistoryCommand`) |
| **Decision 16** | `EvaluateResultAsync` delegates to `INormalRangeService.GetMatchingRangeAsync` + `EvaluateValueAsync` |
| **CP-F4-1** | `LabTestElement` entity reused from F7 — no new entity created |
| **CP-F4-2** | `TestResultEntryView` is a Window modal (ShowDialog) — no DataTemplate in App.xaml |
| **CP-F4-3** | PatientTest requires PatientVisit — no standalone test entry |
| **CP-F4-4** | CalculationConstants = global settings (single table, no FK to LabTest) |
| **CP-F4-5** | SavedComment: created as entity + table, read-only for now (no CRUD UI) |
| **CP-F4-6** | Constants editing available to all users (no Admin gate) |

##### Database Seed (CalculationConstants)

| Id | TestType | ConstantName | ConstantValue |
|----|----------|-------------|---------------|
| 1 | Hgb | AgeUnder1 | 8.25 |
| 2 | Hgb | Age1To12 | 7.50 |
| 3 | Hgb | MaleOver12 | 6.25 |
| 4 | Hgb | FemaleOver12 | 6.75 |
| 5 | CBC | HctMultiplier | 3.3 |
| 6 | PT | ISI | 1.0 |
| 7 | PT | ControlTime | 12.0 |
| 8 | PTT | ControlTime | 30.0 |

**Build Status**: 0 errors, 0 warnings (verified after each part)

---

## 🎯 Next Steps
**Status**: In Progress  

**Completed**:
- ✅ Phase 5: Function 1 — Patient Management (Add/Edit) — 15/15 parts
- ✅ Phase 6: Function 7 — Lab Test Definitions & Pricing — 11/11 parts
- ✅ Phase 7: Function 8 — Normal Range Management — 10/10 parts
- ✅ Phase 8: Function 2 — Barcode Printing & Patient Codes — 13/13 parts
- ✅ Phase 9: Function 3 — Test Results List — 10/10 parts
- ✅ Phase 10: Function 4 — Test Result Entry — 11/11 parts

**Remaining Functions**:
1. Function 5: Result delivery (F5)
2. Function 6: Search for patient (F6)

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

### Database State (Post-Phase 8)
- **Users Table**: Contains initial admin user (e.g., `ahmed` / Ahmed Magdy)
- **Roles Table**: Seeded with Admin (Id=1), Technician (Id=2), Receptionist (Id=3)
- **UserRoles Table**: Admin user linked to Admin role via composite key
- **Referrals Table**: Seeded with default lab ("المعمل", IsDefaultLab=true)
- **SpecimenTypes Table**: Empty (ready for data entry)
- **Patients Table**: Empty (ready for Function 1 data entry)
- **PatientVisits Table**: Empty (ready for Function 1 visits)
- **TestGroups Table**: Seeded with Chemistry, Hematology, Urine (Decision 5 — no Branch field)
- **LabTests Table**: Seeded with Glucose, Hemoglobin, Urine Analysis
- **LabTestElements Table**: Empty (ready for Function 4)
- **ReferralPrices Table**: Empty (ready for Function 7 data entry)
- **NormalRanges Table**: Empty (ready for Normal Range data entry via F8 UI) — NEW (Phase 7)
- **BarcodeSettings Table**: Seeded with 1 row (Id=1, OffsetX=0, OffsetY=0, PrintFileCodeWithAll=false, LabelWidth=38, LabelHeight=25) — NEW (Phase 8)
- **PatientCodes Table**: Empty (ready for Patient Code records via F2) — NEW (Phase 8)
- **PatientTests Table**: Empty (ready for F3 test orders) — NEW (Phase 9)
- **AuditLogs Table**: Empty (audit trail for all operations) — NEW (Phase 9)
- **TestResults Table**: Empty (ready for F4 result entry) — NEW (Phase 10)
- **SavedComments Table**: Empty (ready for saved comments) — NEW (Phase 10)
- **CalculationConstants Table**: Seeded with 8 rows (Hgb×4, CBC×1, PT×2, PTT×1) — NEW (Phase 10)
- **__EFMigrationsHistory**: 7 rows — InitialCreate + AddPatientsAndReferrals + AddLabTestsAndReferralPrices + AddNormalRanges + AddBarcodeSettingsAndPatientCodes + AddPatientTestsAndAuditLogs + AddTestResultsAndConstants

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

**Last Updated**: 2026-07-22  
**Document Version**: 1.6

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
