🏗️ Main Dashboard Window — Architecture & Implementation Plan
1. Current State Analysis
What Exists (Reusable)
Component	Status	Reusable?
ViewModelBase	✅ Has Title, IsBusy	Yes — extend for dashboard
INavigationService	⚠️ Uses Activator.CreateInstance (not DI-aware)	Needs rewrite
IDialogService	✅ MessageBox wrapper	Yes
IAuthService	✅ Complete	Yes
MaterialDesign Theme	✅ Light/Blue/Amber configured	Yes
Color Resources	✅ PrimaryBrush, SecondaryBrush, etc.	Yes
DI Container	✅ IHost + services registered	Yes — add new registrations
Window Management	✅ OnExplicitShutdown + Closed event pattern	Yes
What Needs Building
Component	Status	Priority
MainDashboardViewModel	❌ Does not exist	Critical
MainWindow.xaml	⚠️ Blank stub (welcome text only)	Critical
MainWindow.xaml.cs	⚠️ No DI, no ViewModel	Critical
INavigationService rewrite	⚠️ Not DI-aware	High
Toolbar icon definitions	❌ No icons exist	Critical
Category function buttons	❌ None defined	Critical
Function placeholder views	❌ Views/Controls/ empty	Medium
App.xaml.cs registrations	⚠️ Missing dashboard VM	Critical
Key Architectural Decisions Needed
Decision 1: Navigation Pattern

Current NavigationService uses Activator.CreateInstance<T>() which requires parameterless constructors
Dashboard ViewModels need DI-injected services
Solution: Rewrite NavigationService to use IServiceProvider instead of Activator.CreateInstance
Decision 2: Two-Mode Architecture

Dashboard Mode: Toolbar visible, content shows function buttons
Function Mode: Toolbar hidden, content shows detailed function
Solution: Use a single MainDashboardViewModel with IsToolbarVisible and IsDashboardMode properties. Content switching via CurrentContent (object) bound to a ContentControl.
Decision 3: Icon Strategy

No icon assets in the project
MaterialDesignThemes 5.1.0 includes MaterialDesignIcons pack with 5,000+ icons
Solution: Use MaterialDesign PackIcon with Kind property for each toolbar item
Decision 4: RTL Layout

Arabic UI requires right-to-left flow
Solution: Use FlowDirection="RightToLeft" on the toolbar container
2. Proposed Architecture
MVVM Structure (Text Diagram)
┌─────────────────────────────────────────────────────────────┐
│                        MainWindow                            │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  TOP TOOLBAR (FlowDirection=RTL)                       │  │
│  │  [Exit] [About] [Tips] [Settings] [SysData] [Users]  │  │
│  │  [Stats] [Accounts] [Worksheet] [Tools] [Patients]    │  │
│  └───────────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  CONTENT AREA (ContentControl)                         │  │
│  │                                                        │  │
│  │  Dashboard Mode:                                       │  │
│  │    ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐  │  │
│  │    │ Button1 │ │ Button2 │ │ Button3 │ │ Button4 │  │  │
│  │    └─────────┘ └─────────┘ └─────────┘ └─────────┘  │  │
│  │                                                        │  │
│  │  Function Mode:                                        │  │
│  │    ┌─────────────────────────────────────────────┐    │  │
│  │    │  Function Content / User Control              │    │  │
│  │    │  [Back Button]                                │    │  │
│  │    └─────────────────────────────────────────────┘    │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
Data Flow
User clicks toolbar icon
    → MainDashboardViewModel.SelectCategoryCommand
    → Sets SelectedCategory, IsDashboardMode=true
    → ContentControl shows DashboardView with category buttons

User clicks function button
    → MainDashboardViewModel.OpenFunctionCommand(parameter)
    → Sets IsToolbarVisible=false, IsDashboardMode=false
    → ContentControl shows function's UserControl

User clicks Back / closes function
    → MainDashboardViewModel.CloseFunctionCommand
    → Sets IsToolbarVisible=true, IsDashboardMode=true
    → ContentControl shows DashboardView again

User clicks Exit
    → MainDashboardViewModel.ExitCommand
    → LoginView.Closed event handler triggers → new LoginView shown
Navigation Pattern
Trigger	Action	State Change
App starts → MainWindow opens	Default state	IsToolbarVisible=true, IsDashboardMode=true, SelectedCategory=Patients
Click toolbar icon	Select category	SelectedCategory updated, stays in Dashboard Mode
Click function button	Open function	IsToolbarVisible=false, IsDashboardMode=false, CurrentFunction set
Click Back button	Close function	IsToolbarVisible=true, IsDashboardMode=true, CurrentFunction=null
Click Exit	Exit app	MainWindow.Close() → LoginView reopens (existing pattern)
3. Implementation Plan (10 Parts)
Part 1: Rewrite NavigationService for DI-Awareness
Files: Services/Interfaces/INavigationService.cs, Services/Implementations/NavigationService.cs Description: Replace Activator.CreateInstance<T>() with IServiceProvider.GetService<T>() so ViewModels can have constructor-injected dependencies. Dependencies: None Expected Outcome: NavigationService can now resolve ViewModels with DI

Part 2: Create Toolbar Model
Files: Models/Domain/ToolbarItem.cs (new) Description: Define a simple model for toolbar items: IconKind (MaterialDesign icon), Label (Arabic text), Category (enum or string), Functions (list of function definitions). Dependencies: None Expected Outcome: Data model representing each toolbar button

Part 3: Create Function Definition Model
Files: Models/Domain/FunctionDefinition.cs (new) Description: Define a model for function buttons: Name (Arabic), Icon, TargetViewType (Type of UserControl to load). Dependencies: Part 2 Expected Outcome: Data model representing each function button in the dashboard

Part 4: Create Dashboard Category Data
Files: ViewModels/Pages/MainDashboardViewModel.cs (new) Description: Define all 11 toolbar categories with their icons, labels, and associated function buttons. Implement SelectCategoryCommand, OpenFunctionCommand, CloseFunctionCommand, ExitCommand. Manage IsToolbarVisible, IsDashboardMode, SelectedCategory, CurrentFunction state. Dependencies: Parts 1, 2, 3 Expected Outcome: Complete ViewModel managing dashboard state and navigation

Part 5: Design MainWindow.xaml (Toolbar + Content Area)
Files: Views/Windows/MainWindow.xaml (replace stub) Description: Complete redesign:

Top toolbar: Horizontal ItemsControl with FlowDirection="RightToLeft", 11 PackIcon buttons
Toolbar visibility bound to IsToolbarVisible
Content area: ContentControl bound to CurrentContent
Dashboard view: Grid of function buttons (4 buttons for Patients category)
Function view: Placeholder UserControl with "Back" button Dependencies: Part 4 Expected Outcome: Professional dashboard UI with toolbar and content switching
Part 6: Update MainWindow.xaml.cs for DI
Files: Views/Windows/MainWindow.xaml.cs (replace stub) Description: Accept IServiceProvider in constructor, resolve MainDashboardViewModel, set DataContext, wire up Closed event for LoginView return. Dependencies: Parts 4, 5 Expected Outcome: MainWindow properly integrated with DI container

Part 7: Create Dashboard UserControl
Files: Views/Controls/DashboardContentControl.xaml + .cs (new) Description: UserControl that displays the function buttons grid for the currently selected category. Receives the list of functions via dependency property or DataContext. Dependencies: Parts 3, 4 Expected Outcome: Reusable control showing function buttons for any category

Part 8: Create Function Placeholder UserControl
Files: Views/Controls/FunctionPlaceholderControl.xaml + .cs (new) Description: Generic placeholder control that shows the function name and a "Back" button. Will be replaced by actual function views in future phases. Dependencies: Part 4 Expected Outcome: Placeholder for function content (returns to dashboard on "Back" click)

Part 9: Update App.xaml.cs DI Registrations
Files: App.xaml.cs (modify) Description: Register MainDashboardViewModel as Transient. Ensure MainWindow constructor accepts IServiceProvider. Dependencies: Parts 4, 6 Expected Outcome: DI container aware of all new components

Part 10: Update Window Flow (LoginView → MainWindow)
Files: Views/Windows/LoginView.xaml.cs, Views/Windows/SetupView.xaml.cs (modify) Description: Update OnSuccess callbacks to pass IServiceProvider to MainWindow constructor (if changed to accept it). Dependencies: Parts 6, 9 Expected Outcome: Complete flow: Login → MainWindow (with dashboard) → Exit → Login

4. Technical Decisions
Why Single ViewModel for Dashboard?
Chosen: Single MainDashboardViewModel managing all state Alternative: Separate ViewModel per category (PatientsViewModel, ToolsViewModel, etc.) Trade-off: Single VM is simpler for the dashboard shell. Individual category ViewModels will be created later when actual functionality is implemented. The dashboard VM acts as a coordinator/router.

Why ContentControl + Visibility Toggle?
Chosen: ContentControl with IsToolbarVisible boolean toggle Alternative: Frame navigation or multiple ContentControl with Visibility bindings Trade-off: ContentControl is the most MVVM-friendly approach. Boolean toggle is simpler than managing visibility per-element. Frame navigation would require page registration overhead.

Why MaterialDesign PackIcon?
Chosen: Use materialDesign:PackIcon Kind="..." for toolbar icons Alternative: Custom icon images in Assets/ Trade-off: PackIcon is built-in, no asset management needed, consistent with MaterialDesign theme, scalable. Custom images would require asset creation and management.

Why Rewrite NavigationService?
Chosen: Modify to use IServiceProvider instead of Activator.CreateInstance Alternative: Keep current, create ViewModels manually in code-behind Trade-off: DI-aware navigation is more maintainable and testable. Manual creation couples ViewModels to specific Views.

RTL Layout Strategy
Chosen: FlowDirection="RightToLeft" on toolbar container Alternative: Manual reverse ordering of items Trade-off: FlowDirection handles text and layout automatically. Manual ordering is fragile and harder to maintain.

5. Risks & Mitigation
Risk	Impact	Mitigation
Toolbar icons may look inconsistent	Low	Use MaterialDesign PackIcon with consistent Size and Foreground
ContentControl may not update binding properly	Medium	Use ContentControl.ContentTemplate or DataTemplate for proper MVVM binding
Function loading may be slow	Low	Functions are UserControls (not windows), loaded in-memory
RTL layout may break button alignment	Medium	Test with FlowDirection="RightToLeft" on specific containers, not the whole window
MaterialDesign icon names may not match expectations	Low	Reference MaterialDesignIcons documentation for exact Kind values
MainWindow constructor change may break existing window flow	Medium	Update LoginView.xaml.cs and SetupView.xaml.cs OnSuccess callbacks simultaneously
6. Estimated Complexity
Part	Complexity	Time Estimate	Dependencies
Part 1: NavigationService rewrite	Simple	15 min	None
Part 2: ToolbarItem model	Simple	10 min	None
Part 3: FunctionDefinition model	Simple	10 min	Part 2
Part 4: MainDashboardViewModel	Complex	45 min	Parts 1, 2, 3
Part 5: MainWindow.xaml redesign	Complex	60 min	Part 4
Part 6: MainWindow.xaml.cs DI	Simple	10 min	Parts 4, 5
Part 7: DashboardContentControl	Medium	30 min	Parts 3, 4
Part 8: FunctionPlaceholderControl	Simple	15 min	Part 4
Part 9: App.xaml.cs registrations	Simple	5 min	Parts 4, 6
Part 10: Window flow update	Simple	10 min	Parts 6, 9
Total		~3 hours	
Recommended Implementation Order
Parts 1-3 (foundation models) — no UI impact, safe to build
Parts 4, 9 (ViewModel + DI) — core logic, testable
Parts 5-6 (MainWindow) — visible result
Parts 7-8 (controls) — supporting components
Part 10 (flow integration) — final wiring
7. Toolbar Icon Mapping (MaterialDesign Kind Values)
#	Arabic Label	MaterialDesign Icon Kind	Description
1	المرضى	PersonMultiple	Patient management
2	أدوات	Wrench	System tools
3	ورقة عمل	FileDocument	Worksheet
4	حسابات	CreditCard	Accounts
5	إحصائيات	ChartBar	Statistics
6	المستخدمين	AccountGroup	Users
7	بيانات النظام	Database	System data
8	إعدادات	Cog	Settings
9	هل تعلم	HelpCircle	Did you know
10	نبذة	Information	About
11	خروج	ExitToApp	Exit
8. Default Dashboard State (Patients Category)
When the app starts and MainWindow opens:

┌─────────────────────────────────────────────────────────────┐
│ TOOLBAR (RTL):                                              │
│ [خروج] [نبذة] [هل تعلم] [إعدادات] [بيانات] [المستخدمين] │
│ [إحصائيات] [حسابات] [ورقة عمل] [أدوات] [المرضى ◉]      │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  المرضى ( Patients )                                         │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐                         │
│  │  إدخال نتائج │  │  إضافة وتعديل│                         │
│  │  التحاليل     │  │  بيانات المرضى│                         │
│  └──────────────┘  └──────────────┘                         │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐                         │
│  │  بحث عن مريض │  │  تسليم نتائج │                         │
│  │               │  │  المرضى      │                         │
│  └──────────────┘  └──────────────┘                         │
│                                                              │
└─────────────────────────────────────────────────────────────┘
This plan provides a complete roadmap for implementing the Main Dashboard. Shall I proceed with implementation when you approve, or do you have questions about any part of the plan?

