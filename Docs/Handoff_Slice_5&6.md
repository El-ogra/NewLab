# 🎯 Handoff Plan — Slice 5 & 6 (Function 5 + Function 6)
## NewLab Laboratory Management System — نهاية المرحلة (Last Vertical Slice)

> **مرحلة تخطيط فقط — لا كود.** هذه الوثيقة تُقدَّم للمنفّذ لتنفيذ الشريحتين الأخيرتين
> (تسليم النتائج + البحث عن المريض) وفق نفس مستوى التفصيل والمنهجية المتّبعة في
> `Handoff_Slice_3&4.md` وسابقاتها.

---

## 1) Meta & Scope

| البند | القيمة |
|---|---|
| **Repository** | https://github.com/El-ogra/NewLab.git |
| **Branch** | `main` |
| **Base Commit** | `020b23f3c8642c4b1705c73b01936d5687f15122` |
| **Commit Message** | "بعد تنفيذ الوظيفة الثالثة والوظيفة الرابعة وإزاله ملف التخطيط الخاص بهما" |
| **Stack (unchanged)** | .NET 8, WPF, EF Core 8.0.8, SQL Server, MVVM (CommunityToolkit.Mvvm), MaterialDesignInXAML, FluentValidation, QuestPDF, ZXing.Net |
| **Slice Scope** | Function 5 (تسليم النتائج) + Function 6 (البحث عن مريض) |
| **Slice Order** | Step 7 من 8 (F5) + Step 8 من 8 (F6) — **آخر شريحتين في المشروع** |

### القرارات المؤثرة على هذه الشريحة (من `analysis_and_plan_v3.md`)

| رقم القرار | ملخصه | مكان تطبيقه في هذه الشريحة |
|---|---|---|
| **قرار 10** | قارئ الباركود يقبل أي كود ممسوح ويكتشف نوعه تلقائياً (يبدأ بـ 1=Case، بـ 3=File، بـ 5=Lab) — لا تسلسل ثنائي صارم. | F5 — Part 5.4 (`SearchByCodeAsync`) + Part 5.8 (`BarcodeScannerListener`). |
| **قرار 11** | زر "مستلمة" (تراجع عن التسليم بالخطأ) يتطلّب دور **Admin** — `CanExecute = IsAdmin` + فحص داخل الخدمة. | F5 — Part 5.5 (`UnmarkDeliveredAsync` يرمي `UnauthorizedAccessException`) + Part 5.6 (`UnmarkDeliveredCommand.CanExecute`). |
| **قرار 12** | قاعدة النسخ الاحتياطي = **Stub معطَّل** (`IsEnabled = false`) — لا آلية Backup فعلية في هذه المرحلة. | F6 — Part 6.1 (`Source = Backup` يرمي `NotImplementedException`) + Part 6.4 (`ComboBoxItem` معطَّل بصرياً). |
| **قرار 13** | زر حذف المريض في F6 = **Admin فقط** (نفس منطق قرار 2). | F6 — Part 6.3 (`DeleteCommand.CanExecute = IsAdmin`) + الخدمة تتحقق من `_currentUserService.IsAdmin`. |
| **قرار 17** | `Gender` حصراً `Male/Female` (لا قيمة `Both`). | F6 — Part 6.1 (`SearchCriteria.Gender` من نوع `Enums/Gender.cs` الموجود سلفاً). |

### مصادر الحقيقة المرجعية

- `Docs/analysis_and_plan_v3.md` — القسم F5 (سطور 648–716) و F6 (سطور 718–770).
- `Docs/history.md` — Phase 5 → Phase 10 (لتأكيد الحالة المكتملة لـ F1..F4).
- `Docs/Reference system/MinerU_markdown_...md` — قسم "كيف يتم تسليم نتائج التحاليل" (515–602) وقسم "كيف يتم عملية البحث عن مريض" (607–677).

### حالة الـ Baseline المؤكَّدة من الكود (Ground Truth)

| البند | الحالة الفعلية عند الـcommit |
|---|---|
| Migrations المطبَّقة | `InitialCreate`, `AddPatientsAndReferrals`, `AddLabTestsAndReferralPrices`, `AddNormalRanges`, `AddBarcodeSettingsAndPatientCodes`, `AddPatientTestsAndAuditLogs`, `AddTestResultsAndConstants` (7 migrations). |
| DbSets الموجودة | `Users, Roles, UserRoles, Patients, Referrals, SpecimenTypes, PatientVisits, TestGroups, LabTests, LabTestElements, ReferralPrices, NormalRanges, BarcodeSettings, PatientCodes, PatientTests, AuditLogs, TestResults, SavedComments, CalculationConstants` (19). |
| **الحقول المالية على `Patient`** | `TotalAmount`, `PaidAmount`, `DiscountValue`, `DiscountIsPercent` **موجودة** — يبقى `DeliveredAt` **غير موجود** ويجب إضافته (Part 5.2). |
| Placeholders تُنتظر التفعيل | `TestResultsListView.xaml` سطر 188 → زر "تسليم" مرتبط بـ `OpenDeliveryCommand` (VM: سطر 89 يعرض `_dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 5")`). سطر 190 → زر "بحث (F6)" مرتبط بـ `OpenSearchCommand` (VM: سطر 67 نفس النمط، رسالة "Function 6"). |
| Placeholder ثانوي في `MainDashboardViewModel` | `FunctionDefinition` باسم "بحث عن مريض" و"تسليم نتائج المرضى" داخل category "Patients" بدون `TargetViewType` (يفتح `FunctionPlaceholderControl` فارغ). |
| Barcode helper موجود | `Helpers/BarcodeImageGenerator.cs` فقط (توليد صور). **لا يوجد** `BarcodeScannerListener` لالتقاط قراءات الكيبورد السريعة — سيُنشأ في Part 5.8. |
| `IReportPdfGenerator` | موجود، ويستقبل `int patientTestId` — سيُستخدم داخل `DeliveryService` لطباعة تقارير التسليم (اختياري في هذه الشريحة، معلَّق في Open Questions). |

---

## 2) MVVM Layer Map

```
┌──────────────────────────────── Views (WPF/XAML) ────────────────────────────────┐
│  Views/Pages/DeliveryView.xaml                (F5 — UserControl, DataTemplate)   │
│  Views/Pages/SearchView.xaml                  (F6 — UserControl, DataTemplate)   │
└──────────────────────────────────────────────────────────────────────────────────┘
                                       ▲ DataContext
┌────────────────────────── ViewModels (CommunityToolkit.Mvvm) ────────────────────┐
│  ViewModels/Pages/DeliveryViewModel.cs        (F5)                               │
│  ViewModels/Pages/SearchViewModel.cs          (F6)                               │
│  ViewModels/Pages/TestResultsListViewModel.cs (retro: OpenDelivery/OpenSearch)   │
│  ViewModels/Pages/MainDashboardViewModel.cs   (retro: TargetViewType + commands) │
└──────────────────────────────────────────────────────────────────────────────────┘
                                       ▲ DI
┌───────────────────────────── Services (Interfaces) ──────────────────────────────┐
│  Services/Interfaces/IDeliveryService.cs        (F5)                             │
│  Services/Interfaces/IPatientSearchService.cs   (F6)                             │
│  Services/Interfaces/IBarcodeService.cs         (reused — code-type detection)   │
│  Services/Interfaces/IReportPdfGenerator.cs     (reused — delivery reports)      │
│  Services/Interfaces/ICurrentUserService.cs     (reused — IsAdmin gates)         │
│  Services/Interfaces/ITestResultsListService.cs (reused — SearchByCodeAsync)     │
└──────────────────────────────────────────────────────────────────────────────────┘
                                       ▲
┌───────────────────────────── Services (Implementations) ─────────────────────────┐
│  Services/Implementations/DeliveryService.cs         (F5)                        │
│  Services/Implementations/PatientSearchService.cs    (F6)                        │
└──────────────────────────────────────────────────────────────────────────────────┘
                                       ▲ EF Core
┌────────────────────────────────── Domain / EF ───────────────────────────────────┐
│  Models/Domain/PaymentTransaction.cs   (NEW — F5, Part 5.1)                      │
│  Models/Domain/Enums/PaymentType.cs    (NEW — F5, Part 5.1)                      │
│  Models/Domain/Patient.cs              (MODIFIED — +DeliveredAt, Part 5.2)       │
│  Data/NewLabDbContext.cs               (MODIFIED — +DbSet<PaymentTransaction>)   │
│  Migrations/<ts>_AddPaymentTransactionsAndDeliveredAt.cs (NEW)                   │
└──────────────────────────────────────────────────────────────────────────────────┘
```

### Cross-cutting

- **Validation**: `FluentValidation` — سيُضاف `PaymentTransactionValidator` (اختياري، خفيف).
- **Helpers**: `Helpers/BarcodeScannerListener.cs` (جديد — F5 Part 5.8).
- **Converters**: يمكن إعادة استخدام `TestStatusToIconConverter` و`InverseBoolToVisibilityConverter` الموجودَين.

---

## 3) Retro-Integration Section (تفعيل الـ Placeholders السابقة)

تحدَّد بدقّة نقطتا الـ Retro-Integration الوحيدتان في هذا الـcommit:

### R-1 — تفعيل `OpenDeliveryCommand` في `TestResultsListViewModel` (نتيجة F3)

- **الملف**: `ViewModels/Pages/TestResultsListViewModel.cs` (السطر 88–92 حالياً).
- **الحالة الحالية**:
  ```csharp
  [RelayCommand]
  private void OpenDelivery()
  {
      _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 5");
  }
  ```
- **المطلوب**: استبدال الجسم باستدعاء `INavigationService.NavigateTo<DeliveryViewModel>()` — على غرار ما فُعِل مع `TestResultsListViewModel` من داخل `PatientEntryViewModel.TodayPatients()` (السطر 278–280).
- **تسجيل الـ VM**: إضافة `services.AddTransient<DeliveryViewModel>()` في `App.xaml.cs`.
- **تسجيل الـ DataTemplate**: إضافة `<DataTemplate DataType="{x:Type vmpages:DeliveryViewModel}"><views:DeliveryView /></DataTemplate>` في `App.xaml`.

### R-2 — تفعيل `OpenSearchCommand` في `TestResultsListViewModel` (نتيجة F3)

- **الملف**: `ViewModels/Pages/TestResultsListViewModel.cs` (السطر 65–69 حالياً).
- **الحالة الحالية**:
  ```csharp
  [RelayCommand]
  private void OpenSearch()
  {
      _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 6");
  }
  ```
- **المطلوب**: استبدال الجسم بـ `_navigationService.NavigateTo<SearchViewModel>()`.
- **تسجيل الـ VM**: إضافة `services.AddTransient<SearchViewModel>()` في `App.xaml.cs`.
- **تسجيل الـ DataTemplate**: إضافة `<DataTemplate DataType="{x:Type vmpages:SearchViewModel}"><views:SearchView /></DataTemplate>` في `App.xaml`.

### R-3 — تفعيل `FunctionDefinition` للتسليم والبحث في `MainDashboardViewModel`

- **الملف**: `ViewModels/Pages/MainDashboardViewModel.cs` (السطور 124–125 داخل category "Patients").
- **الحالة الحالية**:
  ```csharp
  new FunctionDefinition { Name = "بحث عن مريض",       IconName = "Magnify"   },
  new FunctionDefinition { Name = "تسليم نتائج المرضى", IconName = "FileCheck" }
  ```
  (بدون `TargetViewType` → يفتح `FunctionPlaceholderControl` فارغ.)
- **المطلوب**:
  ```csharp
  new FunctionDefinition { Name = "بحث عن مريض",       IconName = "Magnify",   TargetViewType = typeof(SearchView)   },
  new FunctionDefinition { Name = "تسليم نتائج المرضى", IconName = "FileCheck", TargetViewType = typeof(DeliveryView) }
  ```
- إضافة فرعَين جديدَين داخل `OpenFunction(FunctionDefinition function)` — على غرار الفرع الحالي لـ `TestResultsListView` (السطور 63–66):
  ```csharp
  else if (function.TargetViewType == typeof(DeliveryView))
  {
      _navigationService.NavigateTo<DeliveryViewModel>();
      CurrentFunctionView = _navigationService.CurrentViewModel;
  }
  else if (function.TargetViewType == typeof(SearchView))
  {
      _navigationService.NavigateTo<SearchViewModel>();
      CurrentFunctionView = _navigationService.CurrentViewModel;
  }
  ```
- إضافة `OpenDeliveryCommand` و`OpenSearchCommand` بنفس نمط `OpenTestResultsList()` (السطور 96–102) — تُستدعى من الـ `KeyBinding` العالمي.

### R-4 — تفعيل الاختصارَين F6 (تسليم) و F3 (بحث) عالمياً

- **الملف**: `Views/Windows/MainWindow.xaml` (السطور 12–15 حالياً).
- **الحالة الحالية**:
  ```xml
  <Window.InputBindings>
      <KeyBinding Key="F2" Command="{Binding OpenPatientEntryCommand}" />
      <KeyBinding Key="F4" Command="{Binding OpenTestResultsListCommand}" />
  </Window.InputBindings>
  ```
- **المطلوب — إضافة سطرَين**:
  ```xml
  <KeyBinding Key="F6" Command="{Binding OpenDeliveryCommand}" />
  <KeyBinding Key="F3" Command="{Binding OpenSearchCommand}" />
  ```

---

## 4) Future-Impact Awareness — نهاية المرحلة

> **هاتان الوظيفتان (F5 + F6) هما الأخيرتان في خطة الوظائف الثمانية.**
> بعد إنجاز هذه الشريحة يصبح المشروع مكتملاً وظيفياً وفق الخطة الرأسية v3، ولن يتبقى أيّ
> `FunctionDefinition` في `MainDashboardViewModel` بدون `TargetViewType` ضمن مسار الوظائف
> الثمانية الأساسية.

### حالة الـ Placeholders بعد إنجاز هذه الشريحة (Expected Post-State)

| الـ Placeholder | الحالة قبل الشريحة | الحالة بعد الشريحة |
|---|---|---|
| `OpenDeliveryCommand` (TestResultsListVM) | Dialog "Function 5" | يفتح `DeliveryView` |
| `OpenSearchCommand` (TestResultsListVM) | Dialog "Function 6" | يفتح `SearchView` |
| `OpenPatientData` (TestResultsListVM سطر 60) | Dialog "Function 2" | **يبقى** — F2 (Barcode Printing) لا يفتح "بيانات مريض"؛ يجب أن يُوجَّه إلى `PatientEntryView` (نفس `_navigationService.NavigateTo<PatientEntryViewModel>()`). **يُصحَّح في هذه الشريحة كتنظيف نهائي** — انظر Clarification Point CP-F5-3. |
| `FunctionDefinition "بحث عن مريض"` | لا `TargetViewType` | يفتح `SearchView` |
| `FunctionDefinition "تسليم نتائج المرضى"` | لا `TargetViewType` | يفتح `DeliveryView` |

### الفئات الفارغة في الـ Toolbar (خارج نطاق هذه الشريحة)

- Categories: `Tools`, `Worksheet`, `Accounts`, `Statistics`, `Users`, `Settings`, `Tips`, `About`, `Exit` — كلها بـ `Functions = new List<FunctionDefinition>()` فارغة. هذه ليست جزءاً من الوظائف الثمانية، وعدم امتلائها **ليس عيباً** بل مطابق للخطة v3.
- بعد اكتمال F5+F6: يمكن إضافة سيناريو "مرحلة II" لملء هذه الفئات، لكنه خارج نطاق هذا الـHandoff.

### Non-Regression Checklist لنهاية المرحلة

1. F1 (PatientEntry) يفتح ويحفظ مريضاً + يستدعي Barcode View.
2. F7 (LabTestManagement) يعرض القائمة ويحرّرها.
3. F8 (NormalRange) يفتح كـ modal من F7.
4. F2 (Barcode) يطبع الملصقات من F1.
5. F3 (TestResultsList) يعرض مرضى اليوم — وأزرار "تسليم" و"بحث" **لم تعد تعرض Dialog Info** بل تنتقل فعلياً.
6. F4 (TestResultEntry) يفتح modal من F3.
7. **F5 (Delivery)** يعرض غير المستلمين لليوم ويقبل مسح باركود ويسلّم فعلياً.
8. **F6 (Search)** يبحث بجزء من الاسم/الهاتف/الكود ويعرض 100 نتيجة.
9. اختصارات F2/F3/F4/F6 تعمل من أي مكان في `MainWindow`.

---

## 5) Function 5 — Parts 5.1 → 5.10 (تسليم نتائج التحاليل)

### Part 5.1 — كيانات مالية جديدة

**الملفات الجديدة**:

- `Models/Domain/Enums/PaymentType.cs`:
  ```csharp
  namespace NewLab.Models.Domain.Enums
  {
      public enum PaymentType
      {
          Payment,   // تسديد حساب من المريض
          Refund,    // استرجاع مبلغ
          Delivery   // تسجيل عملية تسليم (بدون تحصيل)
      }
  }
  ```
- `Models/Domain/PaymentTransaction.cs`:
  ```csharp
  public class PaymentTransaction
  {
      public int Id { get; set; }
      public int PatientId { get; set; }
      [Column(TypeName = "decimal(18,2)")]
      public decimal Amount { get; set; }
      public PaymentType Type { get; set; }
      public int UserId { get; set; }
      public DateTime Timestamp { get; set; } = DateTime.Now;
      [MaxLength(500)] public string? Note { get; set; }

      // Navigation properties
      public Patient Patient { get; set; } = null!;
      public User User { get; set; } = null!;
  }
  ```

**التبعيات**: `Enums/PaymentType.cs` — مستقلة.

### Part 5.2 — تحديث `Patient` بحقل `DeliveredAt`

**الحالة الحالية**: `TotalAmount`, `PaidAmount`, `DiscountValue`, `DiscountIsPercent` **موجودة**. `DeliveredAt` **غير موجود**.

**المطلوب** (تعديل `Models/Domain/Patient.cs`):
```csharp
public DateTime? DeliveredAt { get; set; }
public int? DeliveredByUserId { get; set; }
public User? DeliveredByUser { get; set; }
```
لا يُلمس أي حقل مالي موجود — فقط الإضافة.

### Part 5.3 — Migration `AddPaymentTransactionsAndDeliveredAt`

**التعديلات في `Data/NewLabDbContext.cs`**:

- إضافة: `public DbSet<PaymentTransaction> PaymentTransactions { get; set; }`
- Fluent API جديدة (في قسم جديد رقم 22 بعد الـSeed الحالي للـ LabTests):
  ```csharp
  modelBuilder.Entity<PaymentTransaction>()
      .HasOne(pt => pt.Patient)
      .WithMany()
      .HasForeignKey(pt => pt.PatientId)
      .OnDelete(DeleteBehavior.Cascade);

  modelBuilder.Entity<PaymentTransaction>()
      .HasOne(pt => pt.User)
      .WithMany()
      .HasForeignKey(pt => pt.UserId)
      .OnDelete(DeleteBehavior.Restrict);

  modelBuilder.Entity<PaymentTransaction>()
      .HasIndex(pt => new { pt.PatientId, pt.Timestamp });

  modelBuilder.Entity<PaymentTransaction>()
      .Property(pt => pt.Amount)
      .HasColumnType("decimal(18,2)");
  ```
- FK للحقل الجديد `DeliveredByUserId` على `Patient`:
  ```csharp
  modelBuilder.Entity<Patient>()
      .HasOne(p => p.DeliveredByUser)
      .WithMany()
      .HasForeignKey(p => p.DeliveredByUserId)
      .OnDelete(DeleteBehavior.Restrict);
  ```

**Migration Command**: `dotnet ef migrations add AddPaymentTransactionsAndDeliveredAt`

### Part 5.4 — `IDeliveryService` (Interface)

**الملف**: `Services/Interfaces/IDeliveryService.cs`

```csharp
public sealed record DeliveryPatientRow(
    int PatientId,
    int VisitId,
    string FullName,
    TestStatus AggregateStatus,
    int TestCount,
    int UndeliveredCount,
    int UnprintedCount,
    decimal RemainingBalance,
    bool IsImportant,
    int AttendanceNumber);

public sealed record DeliveryPatientTestRow(
    int PatientTestId,
    int LabTestId,
    string TestName,
    TestStatus Status,
    bool IsPrinted,
    bool IsDelivered,
    decimal Price);

public sealed record DeliveryFilter(
    bool OnlyUndelivered = true,
    bool OnlyLabToLab = false,
    bool OnlyIndividual = false,
    bool OnlyImportant = false,
    DateTime? DateFrom = null,
    DateTime? DateTo = null);

public interface IDeliveryService
{
    Task<List<DeliveryPatientRow>> GetUndeliveredTodayAsync();
    Task<List<DeliveryPatientRow>> FilterAsync(DeliveryFilter filter);
    Task<List<DeliveryPatientTestRow>> GetPatientTestsAsync(int patientId);
    Task<(int Undelivered, int Unprinted, decimal Remaining)> GetPatientDeliveryStateAsync(int patientId);
    Task DeliverAllResultsAsync(int patientId, int userId);
    Task UnmarkDeliveredAsync(int patientId, int userId);   // Admin-only (Decision 11)
    Task<PaymentTransaction> SettleAccountAsync(int patientId, decimal amount, int userId, string? note = null);
    Task<DeliveryPatientRow?> SearchByCodeAsync(string code);   // Decision 10 — auto-detect
}
```

### Part 5.5 — `DeliveryService` (Implementation) + DI

**الملف**: `Services/Implementations/DeliveryService.cs`

**نقاط تنفيذية بارزة**:

1. **الحقن**: `NewLabDbContext`, `ICurrentUserService`, وإن رغبنا في تقارير تسليم فورية `IReportPdfGenerator` (اختياري).
2. **`GetUndeliveredTodayAsync`**:
   - `_context.PatientVisits.Include(v => v.Patient).Include(v => v.PatientTests).ThenInclude(pt => pt.LabTest)`
   - فلترة: `VisitDate.Date == DateTime.Today` و`PatientTests.Any(t => !t.IsDelivered)`.
   - تجميع الـ `RemainingBalance = Patient.TotalAmount - Patient.PaidAmount`.
3. **`SearchByCodeAsync` (قرار 10 — تصميم مرن)**:
   ```csharp
   if (string.IsNullOrWhiteSpace(code)) return null;
   var trimmed = code.Trim();
   // نمط الكود 13-خانة: 1{typeDigit}{yyMMdd}{branch=1}{seq3}{dow}
   // موضع typeDigit هو الخانة الثانية.
   if (trimmed.Length >= 2)
   {
       var typeDigit = trimmed[1];
       return typeDigit switch
       {
           '1' => await SearchByVisitCodeAsync(trimmed),   // Case
           '3' => await SearchByFileCodeAsync(trimmed),    // File
           '5' => await SearchByLabIdAsync(trimmed),       // Lab
           _   => await SearchByFallbackAsync(trimmed)     // Any of the three columns
       };
   }
   return await SearchByFallbackAsync(trimmed);
   ```
   > **ملاحظة تصميمية**: البادئة الحقيقية للكود 13-خانة هي الرقم "1" الثابت في المقدّمة (Padding — انظر `BarcodeService.BuildCode13`)، ورقم النوع في الخانة الثانية. إن ورد الكود بصيغة قديمة تبدأ مباشرة بـ 3 أو 5 (كود ملف/معمل خارجي غير موحّد)، يعتمد الخيار الأخير `SearchByFallbackAsync`.
4. **`DeliverAllResultsAsync(patientId, userId)`**:
   - Transaction (`_context.Database.BeginTransactionAsync`).
   - جلب كل `PatientTest` للمريض عبر Visits.
   - تحديث: `IsDelivered = true, DeliveredByUserId = userId, Status = TestStatus.Delivered` (إن كانت الحالة السابقة `>= Printed`).
   - إضافة `PaymentTransaction { Type = Delivery, Amount = 0, ... }` كسجل تدقيق.
   - إضافة `AuditLog { EntityName = "Patient", Action = "Deliver", ... }`.
   - تعيين `Patient.DeliveredAt = DateTime.Now, Patient.DeliveredByUserId = userId`.
5. **`UnmarkDeliveredAsync(patientId, userId)` — قرار 11**:
   ```csharp
   if (!_currentUserService.IsAdmin)
       throw new UnauthorizedAccessException("عملية إلغاء التسليم تتطلّب صلاحية Admin");
   ```
   يتبعها إعادة كل `PatientTest.IsDelivered = false` وتصفير `Patient.DeliveredAt`.
6. **`SettleAccountAsync(patientId, amount, userId, note)`**:
   - يزيد `Patient.PaidAmount += amount`.
   - يضيف `PaymentTransaction { Type = Payment, Amount = amount, UserId = userId, Note = note }`.

**تسجيل DI** في `App.xaml.cs`:
```csharp
services.AddScoped<IDeliveryService, DeliveryService>();
```

### Part 5.6 — `DeliveryViewModel`

**الملف**: `ViewModels/Pages/DeliveryViewModel.cs` (نمط CommunityToolkit.Mvvm — مطابق لـ `TestResultsListViewModel`).

**الخصائص**:
```csharp
[ObservableProperty] private ObservableCollection<DeliveryPatientRow> patients = new();
[ObservableProperty] private DeliveryPatientRow? selectedPatient;
[ObservableProperty] private ObservableCollection<DeliveryPatientTestRow> patientTests = new();
[ObservableProperty] private int undeliveredCount;
[ObservableProperty] private int unprintedCount;
[ObservableProperty] private decimal remainingBalance;
[ObservableProperty] private string filterMode = "Undelivered";
[ObservableProperty] private DateTime? filterDateFrom = DateTime.Today;
[ObservableProperty] private DateTime? filterDateTo   = DateTime.Today;
[ObservableProperty] private string searchCode = string.Empty;
[ObservableProperty] private decimal settlementAmount;

public bool IsAdmin => _currentUserService.IsAdmin;    // Decision 11
```

**الأوامر**:
```csharp
[RelayCommand] private Task RefreshAsync() { ... }
[RelayCommand(CanExecute = nameof(CanDeliver))]  private Task DeliverManuallyAsync() { ... }
[RelayCommand(CanExecute = nameof(CanSettle))]   private Task SettleAccountAsync() { ... }
[RelayCommand(CanExecute = nameof(CanUnmark))]   private Task UnmarkDeliveredAsync() { ... }   // Decision 11
[RelayCommand] private Task SearchByCodeAsync() { ... }        // Decision 10 — auto-detect
[RelayCommand] private Task ScanBarcodeAsync(string raw) { ... }  // called by BarcodeScannerListener
[RelayCommand] private void BackToMain() { _navigationService.GoBack(); }

private bool CanDeliver() => SelectedPatient != null;
private bool CanSettle()  => SelectedPatient != null && SettlementAmount > 0;
private bool CanUnmark()  => IsAdmin && SelectedPatient != null && SelectedPatient.AggregateStatus >= TestStatus.Delivered;   // Decision 11
```

**Constructor Injection**:
```csharp
public DeliveryViewModel(
    IDeliveryService deliveryService,
    IDialogService dialogService,
    INavigationService navigationService,
    ICurrentUserService currentUserService)
```

**Observation Hooks** (partial methods):
```csharp
partial void OnSelectedPatientChanged(DeliveryPatientRow? value)
{
    if (value != null) _ = LoadTestsAndSummaryAsync(value.PatientId);
    DeliverManuallyCommand.NotifyCanExecuteChanged();
    SettleAccountCommand.NotifyCanExecuteChanged();
    UnmarkDeliveredCommand.NotifyCanExecuteChanged();
}
partial void OnFilterModeChanged(string value) => _ = RefreshAsync();
```

### Part 5.7 — `DeliveryView`

**الملف**: `Views/Pages/DeliveryView.xaml` (UserControl، `FlowDirection="RightToLeft"`).

**الـLayout** — 7 مناطق عبر `Grid` (3 أعمدة × 3 صفوف) — على نمط `TestResultsListView.xaml`:

1. **العمود 1 (الأيمن) — قائمة مرضى اليوم غير المستلمين**:
   - `TextBox` بحث بالكود (يستدعي `SearchByCodeCommand`).
   - `TextBox` مخفي (`Visibility="Collapsed"` أو `Opacity=0`, `Focusable=True`) — يستقبل قراءة الباركود عبر `BarcodeScannerListener` ويطلق `ScanBarcodeCommand`.
   - `ComboBox` الفلاتر (غير المسلمة / معمل لمعمل / فردي / مهم / الكل).
   - `DatePicker × 2` لفترة زمنية.
   - `ListBox` المرضى مع `PackIcon` (نفس `TestStatusToIconConverter`).
2. **العمود 2 (الوسط) — بيانات المريض + تحاليله + المبالغ**:
   - أعلى: `TextBlock` اسم المريض (أحمر عريض إذا `IsImportant = true`).
   - أسفل: `DataGrid` أو `ListBox` لـ `PatientTests` — أعمدة: (الاسم / الحالة / مطبوع / مسلَّم / السعر).
3. **العمود 3 (الأيسر) — الملخص + الأزرار**:
   - `TextBlock`: `UndeliveredCount`, `UnprintedCount`, `RemainingBalance`.
   - `Button "تسليم يدوي" Command="{Binding DeliverManuallyCommand}"`.
   - `TextBox` مبلغ + `Button "تسديد حساب" Command="{Binding SettleAccountCommand}"`.
   - `Button "مستلمة (Admin)" Command="{Binding UnmarkDeliveredCommand}" IsEnabled="{Binding IsAdmin}"` — قرار 11.
   - `Button "تصفية"` (Refresh) و`Button "العودة"` (BackToMain).

**InputBindings** (نفس نمط `TestResultsListView.xaml`):
```xml
<UserControl.InputBindings>
    <KeyBinding Key="F5"  Command="{Binding RefreshCommand}" />
    <KeyBinding Key="F9"  Command="{Binding DeliverManuallyCommand}" />
    <KeyBinding Key="Escape" Command="{Binding BackToMainCommand}" />
</UserControl.InputBindings>
```

### Part 5.8 — `BarcodeScannerListener` (قرار 10)

**الملف**: `Helpers/BarcodeScannerListener.cs`

**الفكرة**: قراء الباركود التجاريون يُرسلون التسلسل بسرعة عالية جداً (>50 حرفاً/ثانية) وينهون الإدخال بـ `Enter`.

**التنفيذ المقترح**:
```csharp
public class BarcodeScannerListener
{
    private readonly StringBuilder _buffer = new();
    private DateTime _lastKeyTime = DateTime.MinValue;
    private const int MaxIntervalMs = 50;

    public event Action<string>? BarcodeScanned;

    public void OnPreviewKeyDown(KeyEventArgs e)
    {
        var now = DateTime.Now;
        if ((now - _lastKeyTime).TotalMilliseconds > MaxIntervalMs)
            _buffer.Clear();
        _lastKeyTime = now;

        if (e.Key == Key.Enter)
        {
            var raw = _buffer.ToString();
            _buffer.Clear();
            if (!string.IsNullOrEmpty(raw)) BarcodeScanned?.Invoke(raw);
            return;
        }

        var ch = KeyToChar(e.Key);
        if (ch != null) _buffer.Append(ch);
    }

    private static char? KeyToChar(Key k) => k switch
    {
        >= Key.D0 and <= Key.D9 => (char)('0' + (k - Key.D0)),
        >= Key.NumPad0 and <= Key.NumPad9 => (char)('0' + (k - Key.NumPad0)),
        _ => null
    };
}
```

**التكامل مع `DeliveryView`**:
- في `DeliveryView.xaml.cs`:
  ```csharp
  private readonly BarcodeScannerListener _listener = new();
  public DeliveryView()
  {
      InitializeComponent();
      _listener.BarcodeScanned += raw =>
      {
          if (DataContext is DeliveryViewModel vm) vm.ScanBarcodeCommand.Execute(raw);
      };
      PreviewKeyDown += (s, e) => _listener.OnPreviewKeyDown(e);
  }
  ```
- `DeliveryViewModel.ScanBarcodeAsync(raw)` يستدعي `_deliveryService.SearchByCodeAsync(raw)` (الذي يطبّق قرار 10).

### Part 5.9 — ربط الـ Toolbar + F6 عالمياً

نُفَّذ في قسم Retro-Integration (R-3 + R-4). لا عمل إضافي.

### Part 5.10 — Build Verification

**Manual Test Script**:
1. `dotnet build` → 0 errors / 0 warnings.
2. `dotnet ef database update` → Migration `AddPaymentTransactionsAndDeliveredAt` تُطبَّق.
3. Login بحساب Receptionist → `F6` → تفتح `DeliveryView`.
4. اختيار مريض → `PatientTests` تظهر → زر "تسليم يدوي" يعمل → تحقق من:
   - `PatientTest.IsDelivered = true` في DB.
   - `PatientTest.Status = Delivered`.
   - `Patient.DeliveredAt` مُعبَّأ.
   - سجل `PaymentTransactions` جديد بـ `Type = Delivery`.
   - سجل `AuditLogs` جديد بـ `EntityName = "Patient", Action = "Deliver"`.
5. تسديد حساب: `SettlementAmount = 50` → `PaymentTransactions.Type = Payment` + `Patient.PaidAmount += 50`.
6. زر "مستلمة" **معطَّل** كـ Technician → إعادة Login كـ Admin → الزر مُفعَّل → يعمل → إعادة `IsDelivered = false`.
7. مسح باركود صيغته `11260722100112` (Case) → المريض المطابق يُعرض تلقائياً.
8. مسح باركود يبدأ بـ `13...` (File) → نفس السلوك.
9. مسح باركود يبدأ بـ `15...` (Lab) → نفس السلوك.
10. من `TestResultsListView` (F4) → زر "تسليم" → ينتقل إلى `DeliveryView`.

---

## 6) Function 6 — Parts 6.1 → 6.6 (البحث عن مريض)

### Part 6.1 — `IPatientSearchService` (Interface)

**الملف**: `Services/Interfaces/IPatientSearchService.cs`

```csharp
public enum SearchSource
{
    Main,
    Backup    // Stub — Decision 12
}

public sealed record SearchCriteria(
    string? NamePrefix       = null,   // أول حرفين
    string? NameContains     = null,   // مقطع في أي مكان
    string? PhoneNumber      = null,
    string? NationalId       = null,
    string? VisitCode        = null,
    string? LabCode          = null,
    string? FileCode         = null,
    DateTime? DateFrom       = null,
    DateTime? DateTo         = null,
    int? AgeFrom             = null,
    int? AgeTo               = null,
    AgeUnit? AgeUnit         = null,
    Gender? Gender           = null,      // Male/Female فقط — Decision 17
    int? ReferralId          = null,
    SearchSource Source      = SearchSource.Main,
    int MaxResults           = 100);

public sealed record PatientSearchRow(
    int PatientId,
    string FullName,
    Gender Gender,
    int AgeValue,
    AgeUnit AgeUnit,
    string? PhoneNumber,
    string? NationalId,
    string? LabId,
    string? FileCode,
    string? VisitCode,
    string? ReferralName,
    DateTime CreatedAt,
    bool IsImportant);

public sealed record PatientTestsSummary(
    int TotalTests,
    int UnenteredCount,
    int UnprintedCount,
    int UndeliveredCount,
    decimal Total,
    decimal Paid,
    decimal Remaining);

public interface IPatientSearchService
{
    Task<List<PatientSearchRow>> SearchAsync(SearchCriteria criteria);
    Task<List<PatientTest>> GetPatientTestsSummaryAsync(int patientId);
    Task<PatientTestsSummary> GetSummaryAsync(int patientId);
    Task<List<PatientTest>> GetPatientsGroupResultsAsync(IEnumerable<int> patientIds, int labTestId);
    Task DeletePatientAsync(int patientId);   // Admin-only — Decision 13
}
```

**قرار 12 — Backup Stub**:
```csharp
// داخل SearchAsync:
if (criteria.Source == SearchSource.Backup)
    throw new NotImplementedException("هذه الميزة غير مفعّلة في هذه المرحلة");
```

### Part 6.2 — `PatientSearchService` (Implementation) + DI

**الملف**: `Services/Implementations/PatientSearchService.cs`

**الحقن**: `NewLabDbContext`, `ICurrentUserService`.

**منطق `SearchAsync`**:
```csharp
var q = _context.Patients.Include(p => p.Referral).AsQueryable();

if (!string.IsNullOrWhiteSpace(criteria.NamePrefix))
    q = q.Where(p => p.FullName.StartsWith(criteria.NamePrefix));

if (!string.IsNullOrWhiteSpace(criteria.NameContains))
    q = q.Where(p => p.FullName.Contains(criteria.NameContains));

if (!string.IsNullOrWhiteSpace(criteria.PhoneNumber))
    q = q.Where(p => p.PhoneNumber == criteria.PhoneNumber);

if (!string.IsNullOrWhiteSpace(criteria.NationalId))
    q = q.Where(p => p.NationalId == criteria.NationalId);

if (!string.IsNullOrWhiteSpace(criteria.VisitCode))
    q = q.Where(p => p.VisitCode == criteria.VisitCode);

if (!string.IsNullOrWhiteSpace(criteria.LabCode))
    q = q.Where(p => p.LabId == criteria.LabCode);

if (!string.IsNullOrWhiteSpace(criteria.FileCode))
    q = q.Where(p => p.FileCode == criteria.FileCode);

if (criteria.DateFrom.HasValue) q = q.Where(p => p.CreatedAt >= criteria.DateFrom.Value);
if (criteria.DateTo.HasValue)   q = q.Where(p => p.CreatedAt < criteria.DateTo.Value.AddDays(1));

if (criteria.Gender.HasValue)     q = q.Where(p => p.Gender == criteria.Gender.Value);
if (criteria.ReferralId.HasValue) q = q.Where(p => p.ReferralId == criteria.ReferralId.Value);

// Age filter — قسمة حسب AgeUnit (ملاحظة: مقارنة AgeValue + AgeUnit كثنائية)
if (criteria.AgeFrom.HasValue && criteria.AgeUnit.HasValue)
    q = q.Where(p => p.AgeUnit == criteria.AgeUnit.Value && p.AgeValue >= criteria.AgeFrom.Value);
if (criteria.AgeTo.HasValue && criteria.AgeUnit.HasValue)
    q = q.Where(p => p.AgeUnit == criteria.AgeUnit.Value && p.AgeValue <= criteria.AgeTo.Value);

return await q
    .OrderByDescending(p => p.CreatedAt)
    .Take(criteria.MaxResults)
    .Select(p => new PatientSearchRow(
        p.Id, p.FullName, p.Gender, p.AgeValue, p.AgeUnit,
        p.PhoneNumber, p.NationalId, p.LabId, p.FileCode, p.VisitCode,
        p.Referral != null ? p.Referral.Name : null,
        p.CreatedAt, p.IsImportant))
    .ToListAsync();
```

**`DeletePatientAsync` — قرار 13**:
```csharp
if (!_currentUserService.IsAdmin)
    throw new UnauthorizedAccessException("حذف المريض يتطلّب صلاحية Admin");

var patient = await _context.Patients.FindAsync(patientId);
if (patient == null) return;
_context.Patients.Remove(patient);
// AuditLog: EntityName="Patient", Action="Delete", UserId
_context.AuditLogs.Add(new AuditLog { EntityName = "Patient", EntityId = patientId, Action = "Delete", UserId = _currentUserService.CurrentUser!.Id });
await _context.SaveChangesAsync();
```

**تسجيل DI** في `App.xaml.cs`:
```csharp
services.AddScoped<IPatientSearchService, PatientSearchService>();
```

### Part 6.3 — `SearchViewModel`

**الملف**: `ViewModels/Pages/SearchViewModel.cs`

**الخصائص**:
```csharp
[ObservableProperty] private string? namePrefix;
[ObservableProperty] private string? nameContains;
[ObservableProperty] private string? phoneNumber;
[ObservableProperty] private string? nationalId;
[ObservableProperty] private string? visitCode;
[ObservableProperty] private string? labCode;
[ObservableProperty] private string? fileCode;
[ObservableProperty] private DateTime? dateFrom;
[ObservableProperty] private DateTime? dateTo;
[ObservableProperty] private int? ageFrom;
[ObservableProperty] private int? ageTo;
[ObservableProperty] private AgeUnit? selectedAgeUnit;
[ObservableProperty] private Gender? selectedGender;      // Male/Female — Decision 17
[ObservableProperty] private Referral? selectedReferral;
[ObservableProperty] private SearchSource selectedSource = SearchSource.Main;

[ObservableProperty] private ObservableCollection<PatientSearchRow> results = new();
[ObservableProperty] private PatientSearchRow? selectedResult;
[ObservableProperty] private ObservableCollection<PatientTest> selectedResultTests = new();
[ObservableProperty] private PatientTestsSummary? summary;

public bool IsAdmin              => _currentUserService.IsAdmin;                       // Decision 13
public bool IsBackupSearchEnabled => false;                                            // Decision 12 — Stub disabled
public List<Referral> AvailableReferrals { get; private set; } = new();
```

**الأوامر**:
```csharp
[RelayCommand] private Task SearchAsync() { ... }                              // يبني SearchCriteria من الخصائص ويستدعي الخدمة
[RelayCommand] private Task RefreshAsync() => SearchAsync();                   // F5
[RelayCommand] private void OpenPatientData() { ... }                          // F2 — NavigateTo<PatientEntryViewModel>
[RelayCommand] private void OpenResults()     { ... }                          // F4 — NavigateTo<TestResultsListViewModel>
[RelayCommand(CanExecute = nameof(CanDelete))] private Task DeleteAsync() { ... }   // F10 — Decision 13
[RelayCommand] private Task PrintGroupResultsAsync() { ... }                   // اختياري: يستدعي IReportPdfGenerator لكل مريض
[RelayCommand(CanExecute = nameof(CanBackup))]  private void SwitchToBackup() { ... }  // معطَّل — Decision 12
[RelayCommand] private void Back() => _navigationService.GoBack();             // Esc

private bool CanDelete() => IsAdmin && SelectedResult != null;                 // Decision 13
private bool CanBackup() => IsBackupSearchEnabled;                             // Decision 12
```

**الحقن**:
```csharp
public SearchViewModel(
    IPatientSearchService searchService,
    IReferralService referralService,
    IDialogService dialogService,
    INavigationService navigationService,
    ICurrentUserService currentUserService)
```

### Part 6.4 — `SearchView`

**الملف**: `Views/Pages/SearchView.xaml` (UserControl، `FlowDirection="RightToLeft"`).

**الـLayout** — 4 مناطق:

1. **يمين — فلاتر البحث**: `DatePicker × 2`, `TextBox` عمر من–إلى + `ComboBox` `AgeUnit`, `ComboBox` `Gender` (Male/Female فقط)، `ComboBox` `Referral`.
2. **حقول البحث بالكود**: صف من `TextBox`ات (هاتف / رقم قومي / كود حالة / كود معمل / كود ملف / اسم-بداية / اسم-يحتوي) — كل واحد في مكان مخصص.
3. **وسط — `DataGrid` Results (100 صف كحد أقصى)**:
   ```xml
   <DataGrid ItemsSource="{Binding Results}" SelectedItem="{Binding SelectedResult}"
             AutoGenerateColumns="False" IsReadOnly="True">
       <DataGrid.Columns>
           <DataGridTextColumn Header="الاسم"       Binding="{Binding FullName}" />
           <DataGridTextColumn Header="الجنس"       Binding="{Binding Gender}" />
           <DataGridTextColumn Header="السن"         Binding="{Binding AgeValue}" />
           <DataGridTextColumn Header="الهاتف"      Binding="{Binding PhoneNumber}" />
           <DataGridTextColumn Header="كود المعمل" Binding="{Binding LabId}" />
           <DataGridTextColumn Header="الإحالة"     Binding="{Binding ReferralName}" />
           <DataGridTextColumn Header="التاريخ"     Binding="{Binding CreatedAt, StringFormat='{}{0:yyyy-MM-dd}'}" />
       </DataGrid.Columns>
   </DataGrid>
   ```
4. **أسفل — تحاليل المريض المحدَّد + Summary + أزرار**:
   - `ListBox` `SelectedResultTests`.
   - `TextBlock`s للـ `Summary`.
   - أزرار: "نتائج تحاليل" (`OpenResultsCommand`) / "بيانات مريض" (`OpenPatientDataCommand`) / **"حذف — Admin"** (`DeleteCommand`, `IsEnabled="{Binding IsAdmin}"`) / "نتائج مجموعة" (`PrintGroupResultsCommand`).
   - **قائمة مصدر البحث (قرار 12)**:
     ```xml
     <ComboBox SelectedItem="{Binding SelectedSource}">
         <ComboBoxItem Content="القاعدة الأساسية" Tag="Main" />
         <ComboBoxItem Content="النسخة الاحتياطية (غير مفعّل)" Tag="Backup"
                       IsEnabled="False"
                       ToolTip="غير مفعّل حالياً" />
     </ComboBox>
     ```

**InputBindings**:
```xml
<UserControl.InputBindings>
    <KeyBinding Key="F2"  Command="{Binding OpenPatientDataCommand}" />
    <KeyBinding Key="F4"  Command="{Binding OpenResultsCommand}" />
    <KeyBinding Key="F5"  Command="{Binding RefreshCommand}" />
    <KeyBinding Key="F6"  Command="{Binding /* delivery */}" />    <!-- reserved -->
    <KeyBinding Key="F10" Command="{Binding DeleteCommand}" />    <!-- Admin-only -->
    <KeyBinding Key="Escape" Command="{Binding BackCommand}" />
</UserControl.InputBindings>
```

### Part 6.5 — ربط الـ Toolbar + F3 عالمي

نُفَّذ ضمن قسم Retro-Integration (R-3 + R-4). لا عمل إضافي.

### Part 6.6 — Build Verification

**Manual Test Script**:
1. `dotnet build` → 0 errors / 0 warnings.
2. Login عادي → `F3` → تفتح `SearchView`.
3. إدخال جزء من الاسم (مثال: "أح") في `NameContains` → زر "بحث" → قائمة تحتوي "أحمد ..." تظهر (حتى 100 صف).
4. إدخال رقم هاتف كامل → صف واحد.
5. إدخال Lab Code → صف واحد.
6. تحديد مريض → تحاليله تظهر أسفل + الـ Summary يُعبَّأ.
7. زر "بيانات مريض" → ينتقل إلى `PatientEntryView`.
8. زر "نتائج تحاليل" → ينتقل إلى `TestResultsListView`.
9. زر "حذف" **معطَّل** لـ Technician → إعادة Login كـ Admin → مُفعَّل → عند النقر: تأكيد → المريض يُحذف + `AuditLog` جديد.
10. عنصر "النسخة الاحتياطية" في القائمة **مُعطَّل** بصرياً (Decision 12).
11. اختصار `F3` من `MainWindow` → يفتح `SearchView`.

---

## 7) Clarification Points (نقاط توضيح مقترحة على المالك — بلا Open Questions مبدئية)

جميع القرارات 10–13، 17 مُطبَّقة. النقاط التالية سيؤخذ بها كخيارات افتراضية إن لم يُصدر المالك توجيهاً مغايراً:

| # | النقطة | القرار الافتراضي المقترح |
|---|---|---|
| **CP-F5-1** | هل يُطبَع تقرير تسليم PDF تلقائياً عند نقر "تسليم يدوي"؟ | **لا** — التقارير الفردية تُطبع من F4 بالفعل عبر `IReportPdfGenerator`. زر "تسليم" في F5 يسجّل الحدث فقط. |
| **CP-F5-2** | هل يُضاف `Enums/PaymentType.cs = Payment/Refund/Delivery` أم يكفي `Payment/Refund` مع تسجيل التسليم في `AuditLog` فقط؟ | **يُضاف `Delivery`** — لتوحيد سجل المدفوعات وأثر التسليم في مكان واحد. |
| **CP-F5-3** | `OpenPatientData()` في `TestResultsListViewModel` (سطر 60) لا يزال يعرض Dialog "Function 2" — هل يُصحَّح ضمن هذه الشريحة؟ | **نعم** — يُحوَّل إلى `_navigationService.NavigateTo<PatientEntryViewModel>()` كتنظيف نهائي (خارج قرار صريح، لكن يخدم "نهاية المرحلة"). |
| **CP-F5-4** | هل يُدعَم فلتر "فترة زمنية" في F5 يتجاوز اليوم الحالي؟ | **نعم** — عبر `DeliveryFilter.DateFrom/DateTo` — الافتراضي اليوم فقط. |
| **CP-F6-1** | هل يُتَخذ `Contains` كحساس لحالة أم لا؟ | **غير حسّاس** — SQL Server LIKE في الـcollation الافتراضي `SQL_Latin1_General_CP1_CI_AS` غير حسّاس أصلاً. |
| **CP-F6-2** | هل يُبنى `PatientSearchService.SearchAsync` كـ query واحدة أم عدّة queries متتابعة؟ | **query واحدة** — `IQueryable` composable كما في التنفيذ أعلاه. |
| **CP-F6-3** | تحاليل المريض في `SearchView` تُعرض من أيّ زيارة؟ | **جميع الزيارات** (`_context.PatientTests.Include(pt => pt.Visit).Where(pt => pt.Visit.PatientId == pid)`) — للحصول على الصورة الكاملة، مع فرز عكسي بالتاريخ. |
| **CP-F6-4** | زر "نتائج مجموعة" (`PrintGroupResultsCommand`) — هل ضمن نطاق هذه الشريحة؟ | **Stub بسيط** يُظهر رسالة "Feature Coming Soon" (خارج نطاق الوظائف الثمانية). |

---

## 8) Change Manifest — قائمة الملفات (مضافة/معدَّلة)

### 8.1 Function 5 — الملفات الجديدة (11)

```
Models/Domain/Enums/PaymentType.cs
Models/Domain/PaymentTransaction.cs
Services/Interfaces/IDeliveryService.cs
Services/Implementations/DeliveryService.cs
Helpers/BarcodeScannerListener.cs
ViewModels/Pages/DeliveryViewModel.cs
Views/Pages/DeliveryView.xaml
Views/Pages/DeliveryView.xaml.cs
Migrations/<ts>_AddPaymentTransactionsAndDeliveredAt.cs
Migrations/<ts>_AddPaymentTransactionsAndDeliveredAt.Designer.cs
Models/Validation/PaymentTransactionValidator.cs   (اختياري — خفيف)
```

### 8.2 Function 6 — الملفات الجديدة (5)

```
Services/Interfaces/IPatientSearchService.cs
Services/Implementations/PatientSearchService.cs
ViewModels/Pages/SearchViewModel.cs
Views/Pages/SearchView.xaml
Views/Pages/SearchView.xaml.cs
```

### 8.3 الملفات المعدَّلة (7)

| الملف | التغيير |
|---|---|
| `Models/Domain/Patient.cs` | +`DeliveredAt`, +`DeliveredByUserId`, +Nav `DeliveredByUser` (Part 5.2). |
| `Data/NewLabDbContext.cs` | +`DbSet<PaymentTransaction>` + Fluent API + FK لـ`DeliveredByUser` (Part 5.3). |
| `App.xaml.cs` | +`IDeliveryService`, +`IPatientSearchService` (Scoped) + `DeliveryViewModel`, `SearchViewModel` (Transient). |
| `App.xaml` | +`DataTemplate` لـ `DeliveryViewModel → DeliveryView` و`SearchViewModel → SearchView`. |
| `ViewModels/Pages/MainDashboardViewModel.cs` | +`TargetViewType = typeof(SearchView)` و`typeof(DeliveryView)` + فرعَين في `OpenFunction` + `OpenDeliveryCommand`, `OpenSearchCommand`. |
| `ViewModels/Pages/TestResultsListViewModel.cs` | تفعيل `OpenDeliveryCommand` و`OpenSearchCommand` + (اختياري CP-F5-3) `OpenPatientData()`. |
| `Views/Windows/MainWindow.xaml` | +`<KeyBinding Key="F3" ... />`, +`<KeyBinding Key="F6" ... />`. |

### 8.4 لا حذف

`Files Deleted: 0` — لا يُحذف أي ملف من الـcommit المرجعي.

---

## 9) Dependency Graph — ترتيب التنفيذ الحتمي

```
                        ┌─────────────────────────────┐
                        │  Part 5.1: PaymentType enum │
                        │           + PaymentTx       │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 5.2: Patient.Deliv..  │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 5.3: Migration        │
                        │ (AddPaymentTx+DeliveredAt)  │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 5.4: IDeliveryService │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 5.5: DeliveryService  │
                        │       + DI registration     │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 5.6: DeliveryViewModel│
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 5.7: DeliveryView     │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 5.8: BarcodeScanner   │
                        │  Listener + integration     │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 5.9: Retro (Toolbar,  │
                        │            F6, TRL VM)      │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 5.10: Build Verify F5 │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 6.1: ISearchService   │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 6.2: SearchService+DI │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 6.3: SearchViewModel  │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 6.4: SearchView.xaml  │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 6.5: Retro (F3+MD VM) │
                        └──────────────┬──────────────┘
                                       ▼
                        ┌─────────────────────────────┐
                        │  Part 6.6: Build Verify F6  │
                        └─────────────────────────────┘
```

**قاعدة Build**: بعد كل Part، `dotnet build` يجب أن يعطي **0 errors / 0 warnings**، وإلا يُوقف التنفيذ ويُصلَح قبل الانتقال إلى Part التالي (سياسة موحّدة مع Handoff السابق).

---

## 10) Sign-off Criteria — لكامل النظام (End-of-Project)

بما أن هذه هي **الشريحة الأخيرة** ضمن الوظائف الثمانية، معايير القبول هنا تشمل النظام كاملاً.

### 10.1 Build & Migration

- [ ] `dotnet restore && dotnet build -c Release` → 0 errors / 0 warnings.
- [ ] `dotnet ef database update` يطبّق Migration الجديدة دون أخطاء.
- [ ] عدد الـMigrations الكلي بعد هذه الشريحة = **8** (السبعة الموجودة + `AddPaymentTransactionsAndDeliveredAt`).
- [ ] Model Snapshot متزامن مع الـmodels.

### 10.2 Functional Coverage (8 من 8)

| # | Function | Route | حالة نهاية المرحلة |
|---|---|---|---|
| 1 | إضافة بيانات مريض | Toolbar → المرضى → "إضافة" أو F2 | ✅ |
| 2 | طباعة باركود | من داخل F1 (زر "باركود") | ✅ |
| 3 | قائمة نتائج التحاليل | Toolbar → المرضى → "إدخال نتائج التحاليل" أو F4 | ✅ |
| 4 | إدخال النتائج | Modal من F3 (Enter على تحليل) | ✅ |
| 5 | **تسليم النتائج** | Toolbar → المرضى → "تسليم" أو F6 | **✅ ضمن هذه الشريحة** |
| 6 | **البحث عن مريض** | Toolbar → المرضى → "بحث" أو F3 | **✅ ضمن هذه الشريحة** |
| 7 | إضافة/تعديل تحاليل | Toolbar → بيانات النظام → "بيانات التحاليل" | ✅ |
| 8 | المعدلات الطبيعية | Modal من F7 | ✅ |

### 10.3 Cross-cutting

- [ ] جميع القرارات الـ17 مُطبَّقة كما في `analysis_and_plan_v3.md` (قرارات 10/11/12/13/17 تحديداً في هذه الشريحة).
- [ ] لا يوجد `_dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function N")` في أي `ViewModel` بعد هذه الشريحة (تحقق بـ `grep -rn "ستُفعَّل هذه الوظيفة" ViewModels/`).
- [ ] لا يوجد `FunctionDefinition` داخل category "Patients" أو "SystemData" بدون `TargetViewType`.
- [ ] `MainWindow.xaml` يحوي `<KeyBinding>` لكل من F2, F3, F4, F6.
- [ ] كل عمليات الحذف الحساسة (Patient, LabTest, NormalRange) وعملية إلغاء التسليم تتحقق من `_currentUserService.IsAdmin` — التحقق مضاعف: `CanExecute` في الـVM + `throw UnauthorizedAccessException` في الـService.

### 10.4 Data Integrity

- [ ] جدول `PaymentTransactions` منشأ ويحوي سجلاً واحداً على الأقل بعد اختبار "تسديد حساب".
- [ ] `Patient.DeliveredAt` مُعبَّأ بعد "تسليم يدوي" ويصفَّر بعد "مستلمة".
- [ ] `AuditLogs` يحوي إدخالات `Action = "Deliver"` و`Action = "Delete"` (F6) لجميع العمليات الحساسة.
- [ ] فرادة `Patient.LabId` و`Patient.FileCode` محفوظة (كما تفرضها الـindexes الموجودة).

### 10.5 Regression (End-to-End Smoke)

1. أنشئ مريضاً جديداً (F1) بتحليلَين → تحقق من ظهوره في F3.
2. اطبع باركود من F1 (F2) → مسحه في F5 → المريض يظهر ذاتياً.
3. من F3 → اضغط Enter → F4 يفتح كـmodal → أدخل نتيجة → أغلق.
4. عد إلى F3 → زر "تسليم" → F5 تفتح → "تسليم يدوي" → المريض يختفي من "غير المستلمين".
5. من F5 (Admin) → "مستلمة" → المريض يعود.
6. F3 → البحث بجزء من الاسم → 1 صف.
7. F6 (search) → البحث بنفس الاسم → 1 صف → زر "بيانات مريض" → F1 يفتح لنفس المريض.

### 10.6 Documentation

- [ ] `Docs/history.md` يُحدَّث بقسم `Phase 11: Function 5` وقسم `Phase 12: Function 6` بنفس بنية الأقسام السابقة.
- [ ] `Docs/analysis_and_plan_v3.md` يُوسم بـ "COMPLETED" لأقسام F5 و F6.
- [ ] `Handoff_Slice_5&6.md` (هذا الملف) يُحفظ في `Docs/` ثم يُحذف بعد التنفيذ كما فُعِل مع `Handoff_Slice_3&4.md`.

---

## 11) Appendix — الحالة المؤكَّدة عند الـCommit المرجعي

**Baseline Verified** (بحث فعلي في الكود، لا افتراض):

- ✅ لا يوجد `PaymentTransaction.cs` — `grep -r "PaymentTransaction" --include="*.cs"` = 0 نتائج.
- ✅ لا يوجد `IDeliveryService.cs` أو `DeliveryService.cs`.
- ✅ لا يوجد `IPatientSearchService.cs` أو `PatientSearchService.cs`.
- ✅ لا يوجد `DeliveryViewModel.cs` أو `SearchViewModel.cs`.
- ✅ لا يوجد `DeliveryView.xaml` أو `SearchView.xaml`.
- ✅ `Patient.cs` لا يحوي `DeliveredAt`.
- ✅ `Helpers/` يحوي `BarcodeImageGenerator.cs` فقط (لا `BarcodeScannerListener.cs`).
- ✅ `TestResultsListView.xaml` سطر 188 و 190 يحويان زرَي "تسليم" و"بحث (F6)" ينتظران التفعيل.
- ✅ `TestResultsListViewModel.cs` سطر 65–69 (`OpenSearch`) وسطر 88–92 (`OpenDelivery`) placeholders يعرضان Dialog.
- ✅ `MainWindow.xaml` سطور 12–15 تحوي `KeyBinding` لـ F2 و F4 فقط (لا F3 ولا F6).
- ✅ `MainDashboardViewModel.cs` سطور 124–125 تحوي `FunctionDefinition`ين للتسليم والبحث بدون `TargetViewType`.

**End of Handoff — Slice 5 & 6.**
