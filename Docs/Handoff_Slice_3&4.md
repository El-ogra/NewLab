# 🎯 Handoff Plan — Combined Vertical Slice: Function 3 + Function 4
## NewLab Laboratory Management System

> **Document type**: Handoff / Implementation-ready plan (analysis + planning only — no code changes yet).
> **Combined slice**: Function 3 (Test Results List — قائمة نتائج التحاليل) followed by Function 4 (Test Result Entry — إدخال نتائج التحاليل).
> **Companion plan**: يُقرأ إلى جانب `Docs/Handoff_Slice_7_2.md` و `Docs/Handoff_Slice_8&2_2.md` (نفس الأسلوب، نفس البنية، نفس مستوى التفصيل).

---

## 0) Meta & Scope

### 0.1) Repository Anchor
- **Repository**: `https://github.com/El-ogra/NewLab.git`
- **Branch**: `main`
- **Commit hash**: `d2e09f41c6e0effb36c3e88afbf2effa11b2d024`
- **Commit message**: "بعد تنفيذ الوظيفة الثامنة والوظيفة الثانية وإزالة ملف التخطيط الخاص بهما"
- **Verification**: ✅ تم الاستنساخ والـcheckout ومطابقة الـhash والرسالة بالحرف الواحد.

### 0.2) Reference Documents (Verified Read)
| # | المصدر | الحالة |
|---|---|---|
| 1 | `Docs/analysis_and_plan_v3.md` — أقسام Baseline + Decisions Table + Function 3 (سطور 467–546) + Function 4 (سطور 548–645) | مقروء بالكامل |
| 2 | `Docs/history.md` — Phase 5 (F1) + Phase 6 (F7) + Phase 7 (F8) + Phase 8 (F2) | مقروء بالكامل |
| 3 | Actual codebase عند الـcommit — كل ملف تحت `Models/`, `Data/`, `Services/`, `ViewModels/`, `Views/`, `App.xaml.cs`, `NewLab.csproj`, `Migrations/` | مفحوص مباشرةً وتم التحقق من مطابقته لـhistory.md |

### 0.3) Applicable Owner Decisions (from Decisions Compliance Table)
| Decision # | Applies to F3/F4? | Concrete impact in this slice |
|---|---|---|
| **6** | F3 | `SearchByAttendanceNumberAsync(int)` = رقم تسلسلي يومي بسيط، بدون ترميز مركّب. |
| **7** | F3 | `ShowAuditCommand` + `ShowFinancialTrackingCommand` مقيَّدان بـ `IsAdmin`; `CanExecute` يقرأ `ICurrentUserService.IsAdmin`. زرا "ب" و "ت" يظهران معطَّلين لغير Admin. |
| **8** | F4 | `IAutoCalculationService` يتضمن `CalculateINR(pt, control, isi)` و `CalculatePTTRatio(ptt, control)`؛ كيان `CalculationConstants` جديد لتخزين ISI + Control Times + Hgb% multipliers. زر "Constants" يفتح تحرير الثوابت. |
| **9** | F4 | زر "تاريخ مرضي" الأساسي فقط — بدون نافذة "تاريخ مخصص" ولا أمر مستقل. |
| **16** | F4 (عبر F8 الموجود) | `ITestResultEntryService.EvaluateResultAsync` يفوّض إلى `INormalRangeService.GetMatchingRangeAsync` (قاعدة أضيق مدى يفوز — مطبَّقة فعلياً في `NormalRangeService.cs`). |
| **17** | Cross-cutting | `Patient.Gender` = Male/Female — يستخدَم في F3 لعرض بيانات المريض وفي F4 لاختيار NormalRange المطابق. |

### 0.4) history.md Conventions Adopted in this Handoff
- ترقيم الأجزاء يطابق حرفياً analysis_and_plan_v3.md (Part 3.1–3.10 ثم Part 4.1–4.11).
- كل جزء له: **الملفات**، **الطبقة (MVVM Layer)**، **التبعيات**، **الناتج المتوقع**، **Build Gate**.
- كل Retro-Integration Section موثَّق كجدول (Change / File / What Changed) — نفس النمط في history.md Phases 6/7/8.
- Clarification Points تُختم بـ (CP-F3-x) أو (CP-F4-x).
- الملفات الجديدة تُذكر بالمسار النسبي من جذر المشروع فقط، بدون أي "src/".

---

## 1) MVVM Layer Map (Combined)

### 1.1) Function 3 — Layer Distribution
| الطبقة | العناصر التي ستُنشأ/تُعدَّل |
|---|---|
| **Model / Domain** | `Models/Domain/PatientTest.cs` (جديد) — Part 3.1؛ `Models/Domain/AuditLog.cs` (جديد) — Part 3.2. `TestStatus.cs` **موجود بالفعل**، لا يُعاد إنشاؤه (Clarification Point CP-F3-1). |
| **Data / Migrations** | تحديث `Data/NewLabDbContext.cs` بإضافة `DbSet<PatientTest>` + `DbSet<AuditLog>` + Fluent API؛ Migration `AddPatientTestsAndAuditLogs` — Part 3.3. |
| **Service / Contracts** | `Services/Interfaces/ITestResultsListService.cs` — Part 3.4. |
| **Service / Implementation** | `Services/Implementations/TestResultsListService.cs` — Part 3.5. |
| **Converters** | `Converters/TestStatusToIconConverter.cs` — Part 3.6. |
| **ViewModel** | `ViewModels/Pages/TestResultsListViewModel.cs` + record `PatientListItem` — Part 3.7. |
| **View** | `Views/Pages/TestResultsListView.xaml` + code-behind — Part 3.8. |
| **DI** | تسجيل الخدمة الجديدة + الـVM في `App.xaml.cs` — Part 3.5 (Impl) + توسيع `App.xaml` بـ `DataTemplate` — Part 3.8. |
| **Navigation / Toolbar** | تعديل `ViewModels/Pages/MainDashboardViewModel.cs` (`TargetViewType = typeof(TestResultsListView)`) + `Views/Windows/MainWindow.xaml` (`KeyBinding F4`) — Part 3.9. |
| **Retro-Integration → F1 view** | حذف الـplaceholder `PrintReceiptCommand` (رسالة "ستُفعَّل هذه الوظيفة في Function 3") من `PatientEntryViewModel.cs` أو استبداله بفتح `TestResultsListView` **إن قرر المالك ذلك** — انظر §2 Retro-Integration. الافتراضي: لا يُلمس زر "إيصال"، بل يُنشأ ربط جديد في الـToolbar فقط. |

### 1.2) Function 4 — Layer Distribution
| الطبقة | العناصر التي ستُنشأ/تُعدَّل |
|---|---|
| **Model / Domain** | `Models/Domain/TestResult.cs` — Part 4.1؛ `Models/Domain/SavedComment.cs` — Part 4.1؛ `Models/Domain/CalculationConstants.cs` — Part 4.5. `LabTestElement.cs` **موجود بالفعل** (بُني في F7) — يُستهلك هنا فقط (CP-F4-1). |
| **Data / Migrations** | إضافة `DbSet<TestResult>` + `DbSet<SavedComment>` + `DbSet<CalculationConstant>` + Fluent API في `Data/NewLabDbContext.cs`؛ Migration `AddTestResultsAndConstants` — Part 4.3. |
| **Service / Contracts** | `Services/Interfaces/ITestResultEntryService.cs` — Part 4.4؛ `Services/Interfaces/IAutoCalculationService.cs` — Part 4.5؛ `Services/Interfaces/IReportPdfGenerator.cs` — Part 4.7. |
| **Service / Implementation** | `Services/Implementations/TestResultEntryService.cs` (يستدعي `INormalRangeService.GetMatchingRangeAsync` + `EvaluateValueAsync`) — Part 4.6؛ `Services/Implementations/AutoCalculationService.cs` — Part 4.6؛ `Services/Implementations/ReportPdfGenerator.cs` — Part 4.7. |
| **Validation** | `Models/Validation/TestResultValidator.cs` — Part 4.6. |
| **ViewModel** | `ViewModels/Pages/TestResultEntryViewModel.cs` + record `TestResultRow` — Part 4.8؛ `ViewModels/Pages/CalculationConstantsViewModel.cs` (نافذة الثوابت) — Part 4.5. |
| **View** | `Views/Windows/TestResultEntryView.xaml` (Window/Modal) — Part 4.9؛ `Views/Windows/CalculationConstantsView.xaml` (Window مصغَّرة) — Part 4.5. |
| **DI** | تسجيل الخدمات الأربعة الجديدة + VMs في `App.xaml.cs` — Part 4.6/4.5. |
| **Retro-Integration → F3** | `TestResultsListViewModel.OpenTestEntryCommand` يستخدم `Func<TestResultEntryViewModel>` (نفس نمط `Func<NormalRangeViewModel>` في `LabTestManagementViewModel`) لفتح `TestResultEntryView` كـDialog — Part 4.10. |

---

## 2) Retro-Integration Section — الاكتشافات الفعلية والتفعيل الدقيق

فحص STEP 3/4 اكتشف **placeholders فعلية** بانتظار F3 وF4 داخل الكود. أدناه توثيق دقيق لكل واحدة، بنفس مستوى تفصيل قسم "Retro-Integration with Function 1" في history.md Phase 8.

### 2.1) Retro-Integration in F3 مع F1 (زر "إيصال" — placeholder فعلي)
**الموقع الدقيق**:
- **الملف**: `ViewModels/Pages/PatientEntryViewModel.cs`
- **الدالة**: `PrintReceipt()` — تقريباً السطر 195 (`[RelayCommand]` قبلها بسطر).
- **السطر الفعلي**: `_dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 3");`
- **الاستدعاء من الـXAML**: `Views/Pages/PatientEntryView.xaml` — سطران:
  - سطر 12: `<KeyBinding Key="F12" Command="{Binding PrintReceiptCommand}" />`
  - سطر 246: `<Button Content="إيصال" Command="{Binding PrintReceiptCommand}" ... />`

**قرار التفعيل — Clarification Point CP-F3-2**:
الرسالة في الـplaceholder تقول "ستُفعَّل هذه الوظيفة في Function 3"، لكن الوصف الوظيفي لـ Function 3 في analysis_and_plan_v3.md **لا يتضمن** "طباعة إيصال المريض". الإيصال منطقياً وثيق الصلة بـ Function 3 من زاوية أن F3 هي نقطة الوصول لتحاليل مريض معين. لذلك يُعامَل هذا الاندماج كنقطة توضيح للمالك، مع خيار افتراضي آمن:

- **الخيار الافتراضي (لا يغيّر السلوك في هذه الشريحة)**: يبقى `PrintReceiptCommand` كما هو (الرسالة الحالية)؛ يُوثَّق كـ Clarification Point CP-F3-2 لمعالجته مستقلاً.
- **البديل (يتطلب إقرار مالك)**: استبدال الرسالة بفتح Dialog طباعة إيصال يستخدم `IBarcodePrintService` (موجود) و`ILabTestService` (موجود) لتوليد PDF ملخّص المدفوعات. **هذا خيار مؤجَّل بانتظار تأكيد**.

**الآلية الدقيقة إن اعتُمِد التفعيل** (لن يُطبَّق افتراضياً):
- تُحقن `Func<TestResultsListViewModel>` factory في constructor `PatientEntryViewModel` (نفس نمط `Func<BarcodeViewModel>` الحالي).
- ضمن **Part 3.7 (TestResultsListViewModel)** يُضاف Public method مثل `ShowReceiptForPatient(int patientId)` أو أن يُنشأ Overload على نافذة الإيصال.
- **حالياً لا نضع تكاملاً عكسياً لـ "إيصال" في هذه الشريحة إلا بعد تأكيد المالك** — يُترك كـ Clarification Point CP-F3-2.

### 2.2) Retro-Integration in F3 مع F1 (زر "مرضى اليوم" — placeholder فعلي — تكامل عكسي حقيقي مؤكَّد)
**الموقع الدقيق**:
- **الملف**: `ViewModels/Pages/PatientEntryViewModel.cs`
- **الدالة**: `TodayPatients()` — تقريباً السطر 278 (`[RelayCommand]` قبلها بسطر).
- **السطر الفعلي**: `_dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 4");`
- **الاستدعاء من الـXAML**: `Views/Pages/PatientEntryView.xaml` سطر 258: `<Button Content="مرضى اليوم" Command="{Binding TodayPatientsCommand}" ... />`

**تحليل الغرض**: رسالة الـplaceholder تشير إلى Function 4، لكن قراءة أعمق:
- Function 4 = **إدخال نتائج التحاليل** (نافذة إدخال قيم لعينة تحليل واحد).
- زر "مرضى اليوم" في PatientEntryView وظيفياً = فتح **قائمة مرضى اليوم** = Function 3 (`TestResultsListView`) — القائمة الرئيسية.
- **تعارض حقيقي بين الرسالة (F4) والوصف الوظيفي (F3)**: يُوثَّق كـ Clarification Point CP-F3-3.

**قرار التفعيل — Clarification Point CP-F3-3 (اقتراح افتراضي، قابل للتأكيد)**:
اعتبار زر "مرضى اليوم" مدخلاً لـ Function 3 (TestResultsListView)، وتعديل الرسالة بحيث تفتح النافذة الفعلية. هذا التكامل العكسي مؤكَّد ويُنفَّذ ضمن الشريحة الحالية بغض النظر عن Clarification.

**الآلية الدقيقة داخل شريحة F3 + F4**:
- ضمن **Part 3.9** (ربط الوظيفة بالـToolbar وF4 عالمياً):
  - يُحقن `INavigationService _navigationService` (موجود بالفعل في constructor `PatientEntryViewModel`).
  - يُستبدل جسم الدالة `TodayPatients()` من:
    ```
    _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 4");
    ```
    إلى:
    ```
    _navigationService.NavigateTo<TestResultsListViewModel>();
    // + إبلاغ MainDashboardViewModel لتحديث CurrentFunctionView
    ```
  - **بديل أنظف** (يوصى به): بدلاً من التنقل من ViewModel، تُطلَق event/message إلى `MainDashboardViewModel` عبر آلية موجودة (استخدام `INavigationService` مباشرةً لأن `MainDashboardViewModel.OpenFunction` يعتمد على `_navigationService.CurrentViewModel`) — سيُوضَّح تفصيلاً في Part 3.9.
- التعديل يقع في **Part 3.9** حصراً، لا يُقسَّم على أجزاء أخرى.

**ضمان MVVM**: لا يُضاف DbContext ولا EF إلى `PatientEntryViewModel`؛ فقط constructor DI جديدة إن لزم (لكن `INavigationService` **موجود بالفعل** لأن F1 حُقنها).

### 2.3) Retro-Integration in F4 مع F3 (زر "Enter" / النقر المزدوج — placeholder سيُصنَع في F3 نفسها)
**الموقع الدقيق (سيُصنع في Part 3.7 ثم يُفعَّل في Part 4.10)**:
- **الملف**: `ViewModels/Pages/TestResultsListViewModel.cs` (جديد — Part 3.7).
- **الدالة**: `OpenTestEntryCommand`.
- **الحالة أثناء Part 3.7**: تُنشأ كأمر معطَّل (`CanExecute = false` أو `_dialogService.ShowMessage(...)`) لأن `TestResultEntryViewModel` ليس موجوداً بعد.

**الآلية الدقيقة لتفعيله في Part 4.10**:
- يُضاف إلى constructor `TestResultsListViewModel`: `Func<TestResultEntryViewModel> _entryVmFactory` (نفس نمط `Func<NormalRangeViewModel>` في `LabTestManagementViewModel` الحالي).
- الأمر يصبح:
  ```
  var vm = _entryVmFactory();
  await vm.LoadForPatientTestAsync(SelectedTest.Id);
  var window = new Views.Windows.TestResultEntryView
  {
      DataContext = vm,
      Owner = System.Windows.Application.Current.MainWindow
  };
  window.ShowDialog();
  ```
- بعد إغلاق النافذة تُستدعى `await RefreshCurrentPatientTestsAsync();` لتحديث الحالات على الشاشة.
- DI: `services.AddTransient<TestResultEntryViewModel>()` مُضاف في Part 4.6.

### 2.4) Retro-Integration المُستبعَدة صراحةً
- **F2 (Barcode)**: لا يوجد placeholder بانتظار F3/F4 في `BarcodeViewModel` أو `BarcodeView.xaml.cs`. لا تكامل عكسي مطلوب هنا.
- **F7 (LabTestManagement)**: `OpenNormalRangeAsync` مفعَّل مسبقاً لـ F8. لا يوجد أمر جديد في `LabTestManagementViewModel` يشير إلى F3/F4.
- **F8 (NormalRange)**: مُستهلَك في F4 كخدمة (`INormalRangeService.GetMatchingRangeAsync`)، لكن لا يوجد placeholder في `NormalRangeViewModel` بانتظار F3/F4.
- **MainDashboardViewModel**: زر "إدخال نتائج التحاليل" ضمن فئة "المرضى" `TargetViewType = null` حالياً — هذا **ليس** تكاملاً عكسياً بل ربط أوّلي يتم في Part 3.9.

---

## 3) Future-Impact Awareness — الأثر التصميمي المُراعى لـF5 وF6

**قاعدة صارمة**: لا يُصمَّم أي تنفيذ داخلي لـF5/F6 هنا. الأدناه هو فقط **الأثر التصميمي التمهيدي** الذي نضعه في الحسبان لتفادي إعادة الهيكلة لاحقاً.

### 3.1) نحو F5 (تسليم نتائج المرضى)
- `PatientTest.Status` (Part 3.1) يشمل قيمة `Delivered` من enum `TestStatus` الموجود. F5 ستحتاج تحويل الحالة إلى `Delivered` — لذا:
  - `ITestResultsListService.PatientListItem` سيتضمن حقلاً `bool AllTestsPrinted` (قابل للاستخدام لاحقاً في زر "تسليم").
  - `TestResultsListViewModel` سيتضمن `OpenDeliveryCommand` (F6 = زر تسليم) كأمر placeholder — نفس النمط الذي طبِّق سابقاً في F1 عندما وُضِعَت أوامر PrintReceipt/TodayPatients كـplaceholders.
- **لا يُنفَّذ التسليم هنا** — فقط اسم الأمر و placeholder message ("ستُفعَّل هذه الوظيفة في Function 5").
- **زر التراجع "مستلمة" (قرار 11)**: F3 لن تضيف زر تراجع؛ يُترك حصراً لـF5 حيث يتطلب `IsAdmin`.

### 3.2) نحو F6 (البحث عن مريض)
- `TestResultsListViewModel.OpenSearchCommand` (F3) placeholder برسالة "ستُفعَّل في Function 6"، بنفس نمط PrintReceipt.
- مربع البحث السريع في نافذة F3 (SearchByCode / SearchByAttendanceNumber) موجود ضمن F3 نفسها — **ليس** بحث F6 الذي هو نافذة منفصلة كاملة الميزات.
- `IPatientService.GetByLabIdAsync(labId)` — **موجود بالفعل** ويكفي F3 (يُستدعى داخل `SearchByCodeAsync`). لن يُوسَّع في هذه الشريحة.

### 3.3) نحو F5 من زاوية F4
- `PatientTest.IsPrinted` (Part 3.1) يُحدَّث في `TestResultEntryService.PrintReportAsync` (Part 4.4/4.7). حين يصير كل تحاليل المريض `IsPrinted = true`، F5 لاحقاً تعرف أن المريض قابل للتسليم — بدون أي تغيير على F4.
- تقرير PDF (Part 4.7): يخزَّن مساره في متغير مؤقت في الـVM؛ لا حاجة لجدول أرشيف الآن — F5 لا تعتمد على أرشيف.

### 3.4) Extension Points المتعمَّدة
| نقطة | الجزء | الغرض المستقبلي |
|---|---|---|
| `PatientListItem` record public | Part 3.4 | F5 تعيد استعماله في `DeliveryViewModel`. |
| `TestResultsListViewModel.OpenDeliveryCommand` (placeholder) | Part 3.7 | يُستبدل جسمه في F5 (نفس نمط تفعيل TodayPatients في هذه الشريحة). |
| `TestResultsListViewModel.OpenSearchCommand` (placeholder) | Part 3.7 | يُستبدل جسمه في F6. |
| `ITestResultEntryService.MarkReviewedAsync(patientTestId)` | Part 4.4 | Reused by F5 لعرض "مراجَعة" بجوار كل تحليل قبل التسليم. |

---

## 4) Function 3 — Full Vertical-Slice Plan (10 Parts)

### Part 3.1 — Enum PatientTestStatus + كيان PatientTest
- **الطبقة**: Model / Domain.
- **الملفات**:
  - `Models/Domain/PatientTest.cs` (جديد) — الحقول:
    - `int Id`
    - `int PatientVisitId` (FK → `PatientVisit`)
    - `int LabTestId` (FK → `LabTest`)
    - `TestStatus Status` (default = `TestStatus.New`)
    - `bool IsReviewed`
    - `bool IsPrinted`
    - `bool IsDelivered`
    - `int? EnteredByUserId`
    - `DateTime? EnteredAt`
    - `int? ReviewedByUserId`
    - `int? PrintedByUserId`
    - `int? DeliveredByUserId`
    - `string? Notes` (MaxLength 500)
    - `decimal Price` (Column decimal(18,2))
    - Navigation: `PatientVisit Visit`, `LabTest LabTest`, `User? EnteredByUser`, `User? ReviewedByUser`, `User? PrintedByUser`, `User? DeliveredByUser`.
- **Enum**: `Enums/TestStatus.cs` **موجود بالفعل** (`New, Entered, Reviewed, Printed, Delivered, AccountIssue, Completed`) — يُستخدم كما هو. Clarification Point CP-F3-1: لا يُعاد إنشاؤه.
- **التبعيات**: `PatientVisit` موجود (F1)، `LabTest` موجود (F7)، `User` موجود (baseline).
- **الناتج المتوقع**: كيان جديد جاهز للـmigration.
- **Build Gate**: `dotnet build` — 0 errors / 0 warnings.

### Part 3.2 — كيان AuditLog للتتبع
- **الطبقة**: Model / Domain.
- **الملفات**:
  - `Models/Domain/AuditLog.cs` (جديد) — الحقول:
    - `int Id`
    - `string EntityName` (MaxLength 100) — مثل "Patient", "PatientTest", "TestResult".
    - `int EntityId`
    - `string Action` (MaxLength 50) — مثل "Create", "Update", "Delete", "Enter", "Review", "Print", "Deliver".
    - `int UserId` (FK → User)
    - `DateTime Timestamp`
    - `string? Details` (MaxLength 2000)
    - Navigation: `User User`.
- **التبعيات**: `User` موجود.
- **الناتج المتوقع**: كيان تدقيق قابل للاستدعاء من كل الخدمات (F3, F4, F5 لاحقاً).
- **Build Gate**: `dotnet build` — 0/0.

### Part 3.3 — Migration
- **الطبقة**: Data / Migrations.
- **الملفات**:
  - `Data/NewLabDbContext.cs` (تعديل):
    - إضافة `public DbSet<PatientTest> PatientTests { get; set; }`
    - إضافة `public DbSet<AuditLog> AuditLogs { get; set; }`
    - Fluent API في `OnModelCreating`:
      - `PatientTest.HasOne(pt => pt.Visit).WithMany().HasForeignKey(pt => pt.PatientVisitId).OnDelete(Cascade)`.
      - `PatientTest.HasOne(pt => pt.LabTest).WithMany().HasForeignKey(pt => pt.LabTestId).OnDelete(Restrict)`.
      - `PatientTest.HasIndex(pt => new { pt.PatientVisitId, pt.LabTestId }).IsUnique()`.
      - `PatientTest.Property(pt => pt.Price).HasColumnType("decimal(18,2)")`.
      - `AuditLog.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(Restrict)`.
      - `AuditLog.HasIndex(a => new { a.EntityName, a.EntityId })`.
      - `AuditLog.HasIndex(a => a.Timestamp)`.
  - `Migrations/<timestamp>_AddPatientTestsAndAuditLogs.cs` (تُولَّد عبر `dotnet ef migrations add AddPatientTestsAndAuditLogs -c NewLabDbContext`).
  - `Migrations/<timestamp>_AddPatientTestsAndAuditLogs.Designer.cs` + `NewLabDbContextModelSnapshot.cs` (تُحدَّث تلقائياً).
- **التبعيات**: Parts 3.1 + 3.2.
- **الناتج المتوقع**: جدولان جديدان (`PatientTests`, `AuditLogs`) + Update-Database ينجح.
- **Build Gate**: `dotnet build` + `dotnet ef database update` كلاهما ينجح — 0/0 warnings.

### Part 3.4 — Service: ITestResultsListService (Interface)
- **الطبقة**: Service / Contracts.
- **الملفات**:
  - `Services/Interfaces/ITestResultsListService.cs` (جديد).
- **Records مصاحبة داخل نفس الملف** (نفس نمط `PatientTestRow` في `IPatientService.cs`):
  - `public sealed record PatientListItem(int PatientId, int VisitId, string FullName, TestStatus AggregateStatus, int VisitCount, bool IsImportant, int AttendanceNumber);`
- **الأساليب**:
  - `Task<List<PatientListItem>> GetTodayPatientsAsync();`
  - `Task<List<PatientListItem>> GetPatientsByFilterAsync(string filterMode, DateTime? forDate);` — filterMode ∈ { "Unwritten", "Unreviewed", "Unprinted", "Important", "Individual", "LabToLab", "Referral", "All" }.
  - `Task<List<PatientTest>> GetPatientTestsAsync(int patientId, DateTime? forDate);`
  - `Task<PatientListItem?> SearchByCodeAsync(string code);` — يبحث في `Patient.LabId` / `Patient.FileCode` / `Patient.VisitCode` (كلها موجودة على Patient).
  - `Task<PatientListItem?> SearchByAttendanceNumberAsync(int number, DateTime? forDate);` — **قرار 6**: يقرأ `PatientVisit.DailySequenceNumber == number` لليوم المطلوب.
  - `Task ToggleReviewedAsync(int patientTestId);`
  - `Task ToggleEnteredAsync(int patientTestId);`
  - `Task TogglePrintedAsync(int patientTestId);`
  - `Task UpdatePatientNoteAsync(int patientId, string note);`
  - `Task<List<AuditLog>> GetAuditForPatientAsync(int patientId);`
  - `Task<List<AuditLog>> GetAuditForTestAsync(int patientTestId);`
- **التبعيات**: Part 3.1, 3.2, `Patient`, `PatientVisit`.
- **الناتج المتوقع**: عقد الخدمة الكامل.
- **Build Gate**: 0/0.

### Part 3.5 — Implementation + DI Registration
- **الطبقة**: Service / Implementation + DI.
- **الملفات**:
  - `Services/Implementations/TestResultsListService.cs` (جديد):
    - Constructor: `NewLabDbContext _context, ICurrentUserService _currentUserService` (النمط نفسه في `PatientService`, `LabTestService`, `NormalRangeService` — لا يحقن Service أخرى؛ راجع "Exceptions & Technical Decisions" في Phase 8 من history.md).
    - `GetTodayPatientsAsync`: يستعلم `_context.PatientVisits.Where(v => v.VisitDate.Date == DateTime.Today)` مع Include(Patient) وتجميع حالة `PatientTest` لكل مريض (AggregateStatus = أدنى حالة بين كل تحاليله).
    - `SearchByAttendanceNumberAsync(int number, DateTime? forDate)`: **قرار 6** — `var d = forDate ?? DateTime.Today; return await _context.PatientVisits.Include(v => v.Patient).FirstOrDefaultAsync(v => v.VisitDate.Date == d.Date && v.DailySequenceNumber == number);`.
    - `SearchByCodeAsync(string code)`: يبحث تسلسلياً في `Patient.LabId`, `Patient.FileCode`, `Patient.VisitCode`.
    - `ToggleReviewedAsync`/`ToggleEnteredAsync`/`TogglePrintedAsync`: تحدّث `PatientTest.IsReviewed/IsPrinted/Status` + تسجّل `AuditLog` بـ `UserId = _currentUserService.CurrentUser?.Id ?? 0`. (لا حذف — لا فحص Admin هنا.)
    - `GetAuditForPatientAsync/GetAuditForTestAsync`: **يفترض** أن `TestResultsListViewModel` استدعى Admin-gate قبل استدعاء الخدمة (قرار 7)؛ الخدمة نفسها لا تفحص Admin لأن العرض قد يُستخدم من سياقات أخرى مستقبلاً.
  - `App.xaml.cs` (تعديل): إضافة `services.AddScoped<ITestResultsListService, TestResultsListService>();` ضمن كتلة "Register our new Services (Scoped)".
- **التبعيات**: Part 3.4.
- **الناتج المتوقع**: خدمة قابلة للحقن.
- **Build Gate**: 0/0.

### Part 3.6 — StatusIconConverter
- **الطبقة**: Converters (طبقة عرض).
- **الملفات**:
  - `Converters/TestStatusToIconConverter.cs` (جديد) — يحوِّل `TestStatus` إلى `PackIconKind` (MaterialDesignThemes.Wpf):
    - `New` → `Circle` باللون الأحمر.
    - `Entered` → `FileDocument` (ورقة).
    - `Reviewed` → `ArrowLeftRight` (أسهم).
    - `Printed` → `Printer` (طابعة).
    - `Delivered` → `Cart` (عربة).
    - `AccountIssue` → `CurrencyUsd` (دولار).
    - `Completed` → `Star` (نجمة).
  - يُسجَّل في `App.xaml`:
    ```
    <converters:TestStatusToIconConverter x:Key="TestStatusToIconConverter"/>
    ```
- **التبعيات**: enum `TestStatus` (موجود)، `MaterialDesignThemes` (5.1.0 موجودة).
- **الناتج المتوقع**: تحويل جاهز لـXAML.
- **Build Gate**: 0/0.

### Part 3.7 — TestResultsListViewModel (مع صلاحيات Admin — قرار 7)
- **الطبقة**: ViewModel.
- **الملفات**:
  - `ViewModels/Pages/TestResultsListViewModel.cs` (جديد) — `partial class : ObservableObject`.
- **Constructor Injection** (باحترام صارم لعدم وضع DbContext في VM):
  ```
  ITestResultsListService _testResultsListService,
  IPatientService _patientService,
  ILabTestService _labTestService,
  IDialogService _dialogService,
  INavigationService _navigationService,
  ICurrentUserService _currentUserService,
  Func<TestResultEntryViewModel> _entryVmFactory  // ← تُملأ في Part 4.10 (Retro-Integration)
  ```
  ملاحظة: في Part 3.7 وحده، لن يكون `TestResultEntryViewModel` موجوداً بعد. الحل النمطي المستخدم في الخطط السابقة:
  - **الخيار المعتمد**: يُنشأ Part 3.7 **بدون** `Func<TestResultEntryViewModel>`، ويُضاف constructor parameter في Part 4.10 كتعديل عكسي محدود (تماماً كما فعلت Phase 8 مع `Func<NormalRangeViewModel>` في `LabTestManagementViewModel`). `OpenTestEntryCommand` في Part 3.7 يعرض placeholder `_dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Part 4.10");`.
- **الخصائص الأساسية** (`[ObservableProperty]`):
  - `ObservableCollection<PatientListItem> Patients { get; }` = new().
  - `PatientListItem? selectedPatient`
  - `ObservableCollection<PatientTest> PatientTests { get; }` = new().
  - `PatientTest? selectedTest`
  - `string searchCode = string.Empty;`
  - `int? searchAttendanceNumber;`
  - `string filterMode = "All";`
  - `DateTime selectedDate = DateTime.Today;`
- **صلاحيات (قرار 7)**:
  - `public bool IsAdmin => _currentUserService.IsAdmin;`
- **الأوامر** (`[RelayCommand]`):
  - `RefreshCommand` (`RefreshAsync`) — يعيد تحميل `Patients` و `PatientTests`.
  - `OpenPatientDataCommand` (F2) — placeholder برسالة (F1 مسؤولة).
  - `OpenSearchCommand` (F3 shortcut) — **placeholder** برسالة "ستُفعَّل في Function 6".
  - `OpenTestEntryCommand` — **placeholder في Part 3.7**، يُفعَّل في Part 4.10.
  - `OpenDeliveryCommand` (F6) — **placeholder** برسالة "ستُفعَّل في Function 5".
  - `PrintAggregateReportCommand`, `PrintWorksheetCommand`, `PrintEnvelopeCommand`, `PrintHistoryCommand`, `PrintBlankReportCommand` — كل أمر: null-check → استدعاء `IReportPdfGenerator` (يُنشأ في Part 4.7 — قبل ذلك: placeholder).
  - `ToggleReviewedCommand` (F8) — `await _testResultsListService.ToggleReviewedAsync(SelectedTest.Id); await RefreshAsync();`.
  - `ToggleEnteredCommand` (F9) — نفس النمط.
  - `TogglePrintedCommand` (F12) — نفس النمط.
  - `UpdateNoteCommand` — `await _testResultsListService.UpdatePatientNoteAsync(SelectedPatient.PatientId, NoteText);`.
  - **`ShowAuditCommand`** — `[RelayCommand(CanExecute = nameof(CanShowAudit))]`؛ `private bool CanShowAudit() => IsAdmin;` — **قرار 7**.
  - **`ShowFinancialTrackingCommand`** — `[RelayCommand(CanExecute = nameof(CanShowFinancial))]`؛ `private bool CanShowFinancial() => IsAdmin;` — **قرار 7**.
  - `SearchCommand` — يفحص أولاً `SearchAttendanceNumber` (int) ثم `SearchCode` (string) → يستدعي الأسلوب المناسب في الخدمة → يعيّن `SelectedPatient`.
  - `PreviousDayCommand` / `NextDayCommand` — يعدّل `SelectedDate` ± 1 يوم ثم `RefreshAsync`.
- **`partial void OnSelectedPatientChanged`**: يستدعي `LoadPatientTestsAsync()` غير المتزامنة.
- **`partial void OnSelectedDateChanged`**: يستدعي `RefreshAsync()`.
- **`partial void OnFilterModeChanged`**: يستدعي `RefreshAsync()`.
- **التبعيات**: Parts 3.4, 3.5, 3.6.
- **الناتج المتوقع**: VM كامل بدون DbContext؛ زرا Audit/Financial معطَّلان لغير Admin تلقائياً.
- **Build Gate**: 0/0.

### Part 3.8 — TestResultsListView (UserControl)
- **الطبقة**: View.
- **الملفات**:
  - `Views/Pages/TestResultsListView.xaml` (جديد) — `UserControl` (بنفس بنية `PatientEntryView` و`LabTestManagementView`).
  - `Views/Pages/TestResultsListView.xaml.cs` (جديد) — code-behind أدنى (InitializeComponent + populating any ComboBox for FilterMode via array — Technical Note 4 من history.md).
- **التخطيط**:
  - `FlowDirection="RightToLeft"`.
  - `UserControl.InputBindings`:
    - `<KeyBinding Key="F5" Command="{Binding RefreshCommand}" />`
    - `<KeyBinding Key="F8" Command="{Binding ToggleReviewedCommand}" />`
    - `<KeyBinding Key="F9" Command="{Binding ToggleEnteredCommand}" />`
    - `<KeyBinding Key="F12" Command="{Binding TogglePrintedCommand}" />`
    - `<KeyBinding Key="Enter" Command="{Binding OpenTestEntryCommand}" />`
    - `<KeyBinding Key="Escape" Command="{Binding CloseFunctionCommand ...}"` — سيُوجَّه إلى `MainDashboardViewModel.CloseFunctionCommand` كما فعلت الشرائح السابقة.
  - `Grid` بثلاثة أعمدة:
    - **العمود 1 (يمين RTL)**: قائمة مرضى اليوم + مربع البحث (Code + Attendance) + التاريخ + أزرار Previous/NextDay + Filter ComboBox.
      - كل `ListBoxItem` يعرض: `PackIcon` يقرأ `AggregateStatus` عبر `TestStatusToIconConverter` + الاسم (باللون الأحمر إن `IsImportant = true`) + `Badge` بعدد الزيارات إذا > 0.
    - **العمود 2 (وسط)**: بيانات المريض المحدَّد (FullName, LabId, VisitCode, FileCode, Referral) + قائمة `PatientTests` مع `PackIcon` عن كل تحليل يقرأ `Status`.
    - **العمود 3 (يسار)**: أزرار F8/F9/F12 + زر "ملاحظات" (يفتح Popup لتعديل النص) + مجموعة أزرار "تقرير مجمع/ورقة عمل/ظرف/تاريخ مرضي/تقرير فارغ/العودة".
    - **زرّا "ب" و "ت"**: كل منهما `Button` عادي ملزَّم بأمر Admin-only:
      ```
      <Button Content="ب - سجل التدقيق"
              Command="{Binding ShowAuditCommand}"
              IsEnabled="{Binding IsAdmin}" />
      <Button Content="ت - تتبع مالي"
              Command="{Binding ShowFinancialTrackingCommand}"
              IsEnabled="{Binding IsAdmin}" />
      ```
      `IsEnabled` يقرأ `IsAdmin` مباشرةً + `CanExecute` من الـcommand يعطي طبقة ثانية من الحماية.
    - **زر "تاريخ مخصص"**: **غير موجود نهائياً في XAML** — قرار 9.
- **App.xaml — DataTemplate**:
  ```
  <DataTemplate DataType="{x:Type vmpages:TestResultsListViewModel}">
      <views:TestResultsListView />
  </DataTemplate>
  ```
- **التبعيات**: Part 3.7 (DataContext type).
- **الناتج المتوقع**: نافذة كاملة قابلة للعرض داخل `CurrentFunctionView`.
- **Build Gate**: 0/0 بعد `dotnet clean && dotnet build` (لتفادي مشكلة MC3074 المذكورة في history.md Technical Note 3).

### Part 3.9 — ربط الوظيفة بالـToolbar + F4 عالمياً + Retro-Integration مع F1
- **الطبقة**: Navigation / DI + Retro to F1.
- **الملفات**:
  - `ViewModels/Pages/MainDashboardViewModel.cs` (تعديل):
    - الحالة الحالية (سطر ~106): `new FunctionDefinition { Name = "إدخال نتائج التحاليل", IconName = "Flask" },` — بلا `TargetViewType`.
    - **التغيير**: `new FunctionDefinition { Name = "إدخال نتائج التحاليل", IconName = "Flask", TargetViewType = typeof(TestResultsListView) },`.
    - في method `OpenFunction`: إضافة branch:
      ```
      else if (function.TargetViewType == typeof(TestResultsListView))
      {
          _navigationService.NavigateTo<TestResultsListViewModel>();
          CurrentFunctionView = _navigationService.CurrentViewModel;
      }
      ```
  - `Views/Windows/MainWindow.xaml` (تعديل): إضافة `KeyBinding` إلى `Window.InputBindings`:
    ```
    <KeyBinding Key="F4" Command="{Binding OpenTestResultsListCommand}" />
    ```
  - إضافة `[RelayCommand] private void OpenTestResultsList()` في `MainDashboardViewModel` — يستدعي نفس منطق `OpenFunction` مع `typeof(TestResultsListView)`.
  - **Retro-Integration مع F1** (§2.2 أعلاه): تعديل `ViewModels/Pages/PatientEntryViewModel.cs`:
    - سطر `TodayPatients()` (~278) يصبح:
      ```
      _navigationService.NavigateTo<TestResultsListViewModel>();
      // MainDashboardViewModel سيرصد التغيير عبر آليته الحالية،
      // أو (أفضل) يُطلق أمر MainDashboardViewModel.OpenTestResultsListCommand عبر مرجع مشترك.
      ```
    - **البديل الأنظف الموصى به**: `PatientEntryViewModel.TodayPatients()` يستدعي `_navigationService.NavigateTo<TestResultsListViewModel>()`، ثم يعتمد على `MainDashboardViewModel` لتحديث `CurrentFunctionView` عبر subscribe لـevent — لكن `NavigationService` الحالية لا تُطلق event. الحل الأبسط الذي يحترم البنية الحالية:
      - تُضاف `Action<Type>? RequestOpenFunctionView` كخاصية على `PatientEntryViewModel`، ويتم ربطها من `MainDashboardViewModel` عند فتح الشاشة. عند الضغط على "مرضى اليوم"، يُستدعى `RequestOpenFunctionView?.Invoke(typeof(TestResultsListView))`. **هذا القرار يُوثَّق كـ Clarification Point CP-F3-3 مع الاقتراح التوضيحي**.
- **التبعيات**: Parts 3.7, 3.8.
- **الناتج المتوقع**:
  - النقر على "إدخال نتائج التحاليل" في Dashboard → يفتح `TestResultsListView`.
  - F4 من أي مكان في MainWindow → نفس الفتح.
  - زر "مرضى اليوم" في PatientEntryView → يفتح `TestResultsListView`.
- **Build Gate**: 0/0 (Clean + Rebuild).

### Part 3.10 — Build Verification (End-to-End)
- **الخطوات**:
  1. `dotnet clean && dotnet build` → 0/0.
  2. `dotnet ef database update -c NewLabDbContext` → migration `AddPatientTestsAndAuditLogs` تُطبَّق.
  3. تشغيل التطبيق → تسجيل دخول بحساب Admin.
  4. Toolbar → "المرضى" → "إدخال نتائج التحاليل" → تفتح `TestResultsListView`.
  5. اضغط F4 من أي مكان → نفس النافذة.
  6. تحديد مريض (يجب توفير بيانات اختبار من F1 + إضافة زيارة + تحاليل — يدوياً في DB أو عبر seed جديدة إن رأى المالك).
  7. F8 يبدّل حالة "مراجعة"، F9 يبدّل "مُدخَل"، F12 يبدّل "مُطبَع".
  8. البحث برقم الحضور 1 → ينتقل للمريض ذي `DailySequenceNumber = 1`.
  9. زر "ب" (سجل التدقيق) نشط لـAdmin، معطَّل لغير Admin (يُختبَر بإنشاء مستخدم Technician).
  10. زر "مرضى اليوم" من PatientEntryView → يفتح نفس النافذة.
- **الناتج المتوقع**: كل الخطوات ناجحة، 0 crashes.

---

## 5) Function 4 — Full Vertical-Slice Plan (11 Parts)

### Part 4.1 — كيان TestResult + SavedComment
- **الطبقة**: Model / Domain.
- **الملفات**:
  - `Models/Domain/TestResult.cs` (جديد) — الحقول:
    - `int Id`
    - `int PatientTestId` (FK → `PatientTest` من Part 3.1)
    - `int LabTestElementId` (FK → `LabTestElement` — موجود من F7)
    - `string? Value` (MaxLength 100)
    - `string? Unit` (MaxLength 50)
    - `bool IsAbnormal`
    - `bool IsCritical`
    - `string? FlagText` (MaxLength 100)
    - `string? Comment` (MaxLength 500)
    - `DateTime EnteredAt`
    - `DateTime? ReviewedAt`
    - Navigation: `PatientTest PatientTest`, `LabTestElement Element`.
  - `Models/Domain/SavedComment.cs` (جديد) — الحقول:
    - `int Id`
    - `int LabTestId` (FK → `LabTest`)
    - `string CommentText` (MaxLength 1000, Required)
    - `string Type` (MaxLength 20) — قيم: "Low", "High", "Critical", "General".
    - Navigation: `LabTest LabTest`.
- **التبعيات**: Part 3.1, F7 (`LabTest`, `LabTestElement`).
- **الناتج المتوقع**: كيانان جديدان.
- **Build Gate**: 0/0.

### Part 4.2 — كيان LabTestElement (استهلاك فقط)
- **الطبقة**: Model / Domain (استخدام).
- **الملفات**: **لا إنشاء** — `Models/Domain/LabTestElement.cs` **موجود بالفعل** (بُني في F7؛ raحظ file listing في history.md Phase 6).
- **Clarification Point CP-F4-1**: خطة v3 تقول "يُبنى في Function 7 لكن يُستخدم هنا" — مطابق للواقع. Part 4.2 لا ينشئ ملفاً؛ فقط يوثّق الاستهلاك.
- **الاستهلاك المطلوب في F4**:
  - `TestResultEntryService.GetPatientTestWithProfileAsync(patientTestId)` سيستدعي `_context.LabTests.Include(lt => lt.Elements).First(...)` لجلب عناصر البروفيل.
  - `TestResultEntryViewModel.ResultRows` سيتضمن صفاً لكل `LabTestElement`.
- **الناتج المتوقع**: توثيق فقط.
- **Build Gate**: N/A (لا كود جديد).

### Part 4.3 — Migration
- **الطبقة**: Data / Migrations.
- **الملفات**:
  - `Data/NewLabDbContext.cs` (تعديل):
    - إضافة `DbSet<TestResult> TestResults`, `DbSet<SavedComment> SavedComments`, `DbSet<CalculationConstant> CalculationConstants` (الأخير يُعرَّف كيانه في Part 4.5).
    - Fluent API:
      - `TestResult.HasOne(tr => tr.PatientTest).WithMany().HasForeignKey(tr => tr.PatientTestId).OnDelete(Cascade)`.
      - `TestResult.HasOne(tr => tr.Element).WithMany().HasForeignKey(tr => tr.LabTestElementId).OnDelete(Restrict)`.
      - `TestResult.HasIndex(tr => new { tr.PatientTestId, tr.LabTestElementId }).IsUnique()`.
      - `SavedComment.HasOne(sc => sc.LabTest).WithMany().HasForeignKey(sc => sc.LabTestId).OnDelete(Cascade)`.
      - `SavedComment.HasIndex(sc => new { sc.LabTestId, sc.Type })`.
      - `CalculationConstant.HasIndex(cc => new { cc.TestType, cc.ConstantName }).IsUnique()`.
      - `CalculationConstant.Property(cc => cc.ConstantValue).HasColumnType("decimal(18,6)")`.
    - Seed defaults لـ `CalculationConstants` (Hgb multipliers + إعدادات فارغة لـPT/PTT):
      ```
      new CalculationConstant { Id = 1, TestType = "Hgb", ConstantName = "AgeUnder1", ConstantValue = 8.25m, UpdatedAt = new DateTime(2026,1,1) },
      new CalculationConstant { Id = 2, TestType = "Hgb", ConstantName = "Age1To12",  ConstantValue = 7.50m, UpdatedAt = new DateTime(2026,1,1) },
      new CalculationConstant { Id = 3, TestType = "Hgb", ConstantName = "MaleOver12",   ConstantValue = 6.25m, UpdatedAt = new DateTime(2026,1,1) },
      new CalculationConstant { Id = 4, TestType = "Hgb", ConstantName = "FemaleOver12", ConstantValue = 6.75m, UpdatedAt = new DateTime(2026,1,1) },
      new CalculationConstant { Id = 5, TestType = "CBC", ConstantName = "HctMultiplier", ConstantValue = 3.3m, UpdatedAt = new DateTime(2026,1,1) },
      new CalculationConstant { Id = 6, TestType = "PT",  ConstantName = "ISI", ConstantValue = 1.0m, UpdatedAt = new DateTime(2026,1,1) },
      new CalculationConstant { Id = 7, TestType = "PT",  ConstantName = "ControlTime", ConstantValue = 12.0m, UpdatedAt = new DateTime(2026,1,1) },
      new CalculationConstant { Id = 8, TestType = "PTT", ConstantName = "ControlTime", ConstantValue = 30.0m, UpdatedAt = new DateTime(2026,1,1) },
      ```
  - `Migrations/<timestamp>_AddTestResultsAndConstants.cs`.
- **التبعيات**: Parts 4.1, 4.5.
- **الناتج المتوقع**: ثلاثة جداول جديدة + بيانات ابتدائية للثوابت.
- **Build Gate**: 0/0 + `dotnet ef database update` ينجح.

### Part 4.4 — Service: ITestResultEntryService
- **الطبقة**: Service / Contracts.
- **الملفات**:
  - `Services/Interfaces/ITestResultEntryService.cs` (جديد).
- **الأساليب**:
  - `Task<(PatientTest patientTest, LabTest labTest, List<LabTestElement> elements, List<TestResult> existingResults)> GetPatientTestWithProfileAsync(int patientTestId);`
  - `Task SaveResultsAsync(int patientTestId, IEnumerable<TestResult> results, string? comment);`
  - `Task<NormalRangeEvaluation> EvaluateResultAsync(decimal value, int labTestId, Patient patient);` — يفوّض إلى `INormalRangeService.GetMatchingRangeAsync(labTestId, patient)` ثم `EvaluateValueAsync(range, value)` (قرار 16 مطبَّق مسبقاً في `NormalRangeService.cs` السطور 63–79).
  - `Task<List<SavedComment>> GetSavedCommentsAsync(int labTestId);`
  - `Task MarkReviewedAsync(int patientTestId);`
  - `Task<byte[]> PreviewReportAsync(int patientTestId);`
  - `Task<byte[]> PrintReportAsync(int patientTestId);`
- **التبعيات**: `INormalRangeService` (موجود), `IReportPdfGenerator` (سيُنشأ في Part 4.7).
- **الناتج المتوقع**: عقد الخدمة.
- **Build Gate**: 0/0.

### Part 4.5 — Service: IAutoCalculationService (قرار 8)
- **الطبقة**: Service / Contracts + Implementation + Model + ViewModel + View.
- **الملفات**:
  - `Models/Domain/CalculationConstants.cs` (جديد):
    - `int Id`
    - `string TestType` (MaxLength 20) — "Hgb", "CBC", "PT", "PTT".
    - `string ConstantName` (MaxLength 50) — مثل "ISI", "ControlTime", "MaleOver12", "HctMultiplier".
    - `decimal ConstantValue` (decimal(18,6))
    - `DateTime UpdatedAt`
  - `Services/Interfaces/IAutoCalculationService.cs` (جديد):
    - `Task<decimal> CalculateHctAsync(decimal hgb);` → `hgb * HctMultiplier` (يقرأ الثابت من DB).
    - `Task<decimal> CalculateHgbPercentAsync(decimal hgb, int ageYears, Gender gender);` → يطبّق الفروع (4 حالات).
    - `Task<decimal> CalculateINRAsync(decimal ptPatient, decimal? overrideControlTime = null, decimal? overrideIsi = null);` → `Math.Pow((double)(ptPatient / control), (double)isi)` — قرار 8.
    - `Task<decimal> CalculatePTTRatioAsync(decimal pttPatient, decimal? overrideControlTime = null);` → `pttPatient / control` — قرار 8.
    - `Task<List<CalculationConstant>> GetConstantsAsync(string? testType = null);`
    - `Task UpdateConstantAsync(int id, decimal value);`
  - `Services/Implementations/AutoCalculationService.cs`:
    - Constructor: `NewLabDbContext _context` فقط (نفس النمط الأحادي).
    - يقرأ الثابت من `_context.CalculationConstants` بحسب `TestType + ConstantName`.
  - `ViewModels/Pages/CalculationConstantsViewModel.cs` (جديد):
    - `ObservableCollection<CalculationConstant> Constants { get; }`
    - `EditConstantCommand`, `SaveCommand`, `CloseCommand`.
    - Constructor: `IAutoCalculationService`, `IDialogService`, `ICurrentUserService`.
  - `Views/Windows/CalculationConstantsView.xaml` + `.xaml.cs` — Window صغير (400×500) يعرض DataGrid للثوابت مع تحرير.
- **التبعيات**: Part 4.3 (Migration يحتوي الكيان + Seed).
- **الناتج المتوقع**: خدمة الحساب الآلي + شاشة إدارة الثوابت.
- **Build Gate**: 0/0.

### Part 4.6 — Implementation + DI + Validation
- **الطبقة**: Service / Implementation + Validation + DI.
- **الملفات**:
  - `Services/Implementations/TestResultEntryService.cs` (جديد):
    - Constructor: `NewLabDbContext _context, INormalRangeService _normalRangeService, IReportPdfGenerator _reportGenerator, ICurrentUserService _currentUserService`.
    - `EvaluateResultAsync`: يستدعي `_normalRangeService.GetMatchingRangeAsync` + `EvaluateValueAsync` — لا يعيد اختراع منطق قرار 16.
    - `SaveResultsAsync`: يبدأ Transaction → يمسح `TestResult` القديم للـ`PatientTestId` → يضيف الجديد → يحدّث `PatientTest.Status = Entered` + `EnteredByUserId = _currentUserService.CurrentUser?.Id` + `EnteredAt = DateTime.Now` → يسجّل `AuditLog { Action = "Enter" }` → Commit.
    - `MarkReviewedAsync`: يعدّل `IsReviewed = true` + `ReviewedByUserId` + يسجّل audit "Review".
    - `PrintReportAsync`: يستدعي `_reportGenerator.GenerateAsync(patientTestId)` → يعدّل `IsPrinted = true` + `PrintedByUserId` + audit "Print" → يعيد `byte[]`.
  - `Models/Validation/TestResultValidator.cs` (جديد, FluentValidation):
    - `RuleFor(r => r.PatientTestId).GreaterThan(0);`
    - `RuleFor(r => r.LabTestElementId).GreaterThan(0);`
    - `RuleFor(r => r.Value).MaximumLength(100);`
    - `RuleFor(r => r.Comment).MaximumLength(500);`
  - `App.xaml.cs` (تعديل):
    ```
    services.AddScoped<ITestResultEntryService, TestResultEntryService>();
    services.AddScoped<IAutoCalculationService, AutoCalculationService>();
    services.AddScoped<IReportPdfGenerator, ReportPdfGenerator>();     // Part 4.7
    services.AddScoped<IValidator<TestResult>, TestResultValidator>();
    services.AddTransient<TestResultEntryViewModel>();                 // Part 4.8
    services.AddTransient<CalculationConstantsViewModel>();            // Part 4.5
    ```
  - Also App.xaml — DataTemplate for `TestResultEntryViewModel` **not needed** (يُفتح كـWindow modal، مثل NormalRangeView/BarcodeView) — Clarification Point CP-F4-2.
- **التبعيات**: Parts 4.1, 4.3, 4.4, 4.5.
- **الناتج المتوقع**: خدمات مسجَّلة.
- **Build Gate**: 0/0.

### Part 4.7 — QuestPDF ReportGenerator
- **الطبقة**: Service / Implementation.
- **الملفات**:
  - `Services/Interfaces/IReportPdfGenerator.cs` (جديد): `Task<byte[]> GenerateAsync(int patientTestId);`
  - `Services/Implementations/ReportPdfGenerator.cs` (جديد):
    - Constructor: `NewLabDbContext _context, INormalRangeService _normalRangeService, IAutoCalculationService _autoCalcService`.
    - أول سطر: `QuestPDF.Settings.License = LicenseType.Community;` (CP-F2-3 المطبَّق في `BarcodePrintService`).
    - يبني `Document.Create(container => container.Page(page => { ... }))` بـFlowDirection RTL:
      - Header: اسم المعمل + شعار.
      - بيانات المريض.
      - جدول: عناصر البروفيل + Value + Unit + NormalRange + Flag (تلوين cells الحرجة بالأحمر).
      - **قسم مخصص لـ INR و PTT Ratio** عند توفرها (قرار 8) — يعرض:
        ```
        INR = (PT_patient / ControlTime)^ISI = <computed>
        PTT Ratio = PTT_patient / ControlTime = <computed>
        ```
      - Comment section.
      - Footer: اسم المُدخِل + المُراجِع + التاريخ.
- **التبعيات**: `QuestPDF` (موجود 2026.7.1)، Parts 4.4, 4.5.
- **الناتج المتوقع**: خدمة PDF قابلة للحقن.
- **Build Gate**: 0/0.

### Part 4.8 — TestResultEntryViewModel (بدون تاريخ مخصص — قرار 9)
- **الطبقة**: ViewModel.
- **الملفات**:
  - `ViewModels/Pages/TestResultEntryViewModel.cs` (جديد).
- **Records مصاحبة**:
  - `public sealed record TestResultRow(int LabTestElementId, string Name) : ObservableObject` — مع `[ObservableProperty] string? value; string? unit; string? computedFlag; string? cellColor;`. **بديل بسيط**: class عادية `TestResultRow : ObservableObject`.
- **Constructor Injection**:
  ```
  ITestResultEntryService _entryService,
  IAutoCalculationService _autoCalcService,
  IReferralService _referralService,   // اختياري — لعرض بيانات المريض
  IDialogService _dialogService,
  IValidator<TestResult> _validator,
  ICurrentUserService _currentUserService,
  Func<CalculationConstantsViewModel> _constantsVmFactory   // لفتح شاشة الثوابت
  ```
- **الخصائص**:
  - `Patient? patient;` `LabTest? parentTest;`
  - `ObservableCollection<TestResultRow> ResultRows { get; }` = new();
  - `string? comment;` `bool isReviewed;`
  - `string historyButtonLabel = "تاريخ مرضي";` — يتحدّث حسب عدد النتائج السابقة.
  - `ObservableCollection<CalculationConstant> Constants { get; }` = new();  — عرض فقط، التحرير في شاشة منفصلة.
  - `ObservableCollection<SavedComment> SavedComments { get; }` = new();
- **`LoadForPatientTestAsync(int patientTestId)`** — public method يُستدعى من Retro-Integration:
  1. `var data = await _entryService.GetPatientTestWithProfileAsync(patientTestId);`
  2. تُعبَّأ `ResultRows` من `data.elements` وتُقتَرن بالقيم من `data.existingResults`.
  3. تحدَّث `historyButtonLabel` حسب عدد النتائج السابقة للمريض.
- **الأوامر**:
  - `SaveCommand` (F9) → بناء `List<TestResult>` من `ResultRows` → `_entryService.SaveResultsAsync(...)` → Dialog نجاح.
  - `PrintCommand` (F12) → `PrintReportAsync` → حفظ byte[] كـPDF مؤقت.
  - `PreviewCommand` (F11) → `PreviewReportAsync`.
  - `ToggleReviewedCommand` (F8) → `MarkReviewedAsync`.
  - `OpenHistoryCommand` — **الزر الأساسي فقط (قرار 9)** — يعرض قائمة نتائج المريض السابقة داخل Popup بسيط، **لا نافذة منفصلة "تاريخ مخصص"**.
  - `BackCommand` (Esc) — يُغلق النافذة عبر code-behind.
  - `MainMenuCommand` — يعيد إلى Dashboard.
  - `PickSavedCommentCommand(SavedComment)` — يُلحق تعليقاً محفوظاً إلى `Comment`.
  - `PickCommentFromNormalRangeCommand` — يقرأ `range.LowComment/HighComment/CriticalComment` من `INormalRangeService.GetMatchingRangeAsync` ويضيفه.
  - `UndoLastCommentCommand` — يعيد آخر إضافة (stack بسيط في memory).
  - **`EditConstantsCommand`** — **قرار 8**:
    ```
    var vm = _constantsVmFactory();
    var window = new Views.Windows.CalculationConstantsView { DataContext = vm, Owner = ... };
    window.ShowDialog();
    ```
- **تلوين النتائج**: عند تغيير `TestResultRow.Value`:
  - `partial void OnValueChanged(string? value)` → يستدعي `_entryService.EvaluateResultAsync(parsedValue, parentTest.Id, patient)` → يعيّن `cellColor` (Normal = Transparent، Abnormal = Yellow/Orange، Critical = Red).
- **حسابات تلقائية** (Hgb → Hct مثلاً): إن كان `parentTest.TestName == "CBC"` أو `parentTest` يحتوي element باسم "Hgb"، يُطلَق حساب `Hct` و `Hgb%` تلقائياً عبر `IAutoCalculationService`.
- **قرار 9 مؤكَّد**: لا يوجد `OpenCustomHistoryCommand`، لا زر "تاريخ مخصص" في الأمر أو الـXAML.
- **التبعيات**: Parts 4.4, 4.5, 4.6.
- **الناتج المتوقع**: VM كامل بدون DbContext.
- **Build Gate**: 0/0.

### Part 4.9 — TestResultEntryView (Window)
- **الطبقة**: View.
- **الملفات**:
  - `Views/Windows/TestResultEntryView.xaml` (جديد) — `Window` (نفس نمط `NormalRangeView`, `BarcodeView`).
    - `WindowStartupLocation="CenterOwner"`, `Height="750"`, `Width="1100"`, `FlowDirection="RightToLeft"`.
    - `Window.InputBindings`:
      - `F8` → `ToggleReviewedCommand`
      - `F9` → `SaveCommand`
      - `F11` → `PreviewCommand`
      - `F12` → `PrintCommand`
      - `Escape` → `BackCommand`
    - Layout: `Grid` 3 rows.
      - Row 0 (Auto): بيانات المريض (Patient.FullName, LabId, Referral).
      - Row 1 (*): `DataGrid ItemsSource="{Binding ResultRows}"`:
        - Column: Name (readonly).
        - Column: Value (editable TextBox).
        - Column: Unit (readonly).
        - Column: Flag (readonly) مع خلفية `cellColor`.
      - Row 2 (Auto):
        - `TextBox` للتعليق.
        - `Button` برتقالي "تعليقات محفوظة" يفتح Popup مع `ItemsSource="{Binding SavedComments}"`.
        - `Button` "تعليق من المعدل الطبيعي".
        - `Button` "تراجع تعليق".
        - أزرار: حفظ (F9) / طباعة (F12) / معاينة (F11) / **تاريخ مرضي** (Content = `{Binding HistoryButtonLabel}`) / رجوع (Esc) / القائمة الرئيسية.
    - **زر جانبي "Constants"** (قرار 8) — على يسار الشاشة (RTL):
      ```
      <Button Content="Constants"
              Command="{Binding EditConstantsCommand}"
              Margin="8" Padding="16,8" />
      ```
  - `Views/Windows/TestResultEntryView.xaml.cs` — code-behind أدنى: `InitializeComponent()` + `BackToTests_Click` يستدعي `Close()` (نفس نمط `NormalRangeView.xaml.cs`).
- **التبعيات**: Part 4.8.
- **الناتج المتوقع**: نافذة قابلة لـ`ShowDialog`.
- **Build Gate**: 0/0 (Clean + Rebuild لتجنّب MC3074).

### Part 4.10 — Retro-Integration مع Function 3
- **الطبقة**: ViewModel + DI.
- **الملفات المُعدَّلة**:
  - `ViewModels/Pages/TestResultsListViewModel.cs` (تعديل):
    - إضافة `Func<TestResultEntryViewModel> _entryVmFactory` إلى constructor (last parameter — نفس نمط history.md Phase 7 عند إضافة `Func<NormalRangeViewModel>` إلى `LabTestManagementViewModel`).
    - `OpenTestEntryCommand` يصبح:
      ```
      [RelayCommand]
      private async Task OpenTestEntryAsync()
      {
          if (SelectedTest == null)
          {
              _dialogService.ShowMessage("خطأ", "اختر تحليلاً أولاً");
              return;
          }
          var vm = _entryVmFactory();
          await vm.LoadForPatientTestAsync(SelectedTest.Id);
          var window = new Views.Windows.TestResultEntryView
          {
              DataContext = vm,
              Owner = System.Windows.Application.Current.MainWindow
          };
          window.ShowDialog();
          await RefreshCurrentPatientTestsAsync();  // تحديث الرموز على القائمة
      }
      ```
  - **مواقع التغيير** (نفس بنية Retro-Integration في history.md Phase 8):
    | Change | File | What Changed |
    |---|---|---|
    | Constructor added `Func<TestResultEntryViewModel>` | `ViewModels/Pages/TestResultsListViewModel.cs` | Factory pattern — نفس نمط `Func<NormalRangeViewModel>` في `LabTestManagementViewModel` |
    | `OpenTestEntryCommand` body | نفس الملف | Placeholder message removed → factory + LoadForPatientTestAsync + ShowDialog |
    | DI registration | `App.xaml.cs` | (تُضاف في Part 4.6): `services.AddTransient<TestResultEntryViewModel>()` |
    | InputBinding | `Views/Pages/TestResultsListView.xaml` | UNCHANGED — `OpenTestEntryCommand` مربوط بـEnter منذ Part 3.8 |
- **التبعيات**: Parts 3.7 + 4.8 + 4.9.
- **الناتج المتوقع**: النقر المزدوج / Enter على تحليل في F3 → يفتح `TestResultEntryView` من F4.
- **Build Gate**: 0/0.

### Part 4.11 — Build Verification (F4 End-to-End)
- **الخطوات**:
  1. `dotnet clean && dotnet build` → 0/0.
  2. `dotnet ef database update` — migration `AddTestResultsAndConstants` تُطبَّق (تشمل seed للثوابت).
  3. تشغيل → F4 → اختيار مريض → Enter على تحليل → تفتح `TestResultEntryView`.
  4. إدخال قيمة Hgb (مثلاً 12.5) → **يُحسَب Hct = 12.5 × 3.3 = 41.25 تلقائياً** في صف آخر.
  5. اختيار تحليل PT → إدخال قيمة (مثلاً 15.0) → **يُحسَب INR = (15/12)^1.0 = 1.25 تلقائياً** (باستخدام الثوابت الافتراضية).
  6. القيم غير الطبيعية تُلوَّن (Yellow/Orange/Red).
  7. F9 (حفظ) → Dialog نجاح → إغلاق → العودة إلى F3 مع تحديث حالة التحليل إلى Entered.
  8. F11 (معاينة) → PDF يفتح.
  9. F12 (طباعة) → PDF مطبوع + `IsPrinted = true` في DB.
  10. زر "Constants" → يفتح `CalculationConstantsView` → تعديل ISI → حفظ → إعادة تشغيل حساب INR بالقيمة الجديدة.
  11. Esc → يعود بدون حفظ.

---

## 6) Cross-Slice MVVM Compliance Checklist

| البند | F3 (Parts 3.1–3.10) | F4 (Parts 4.1–4.11) |
|---|---|---|
| لا `DbContext` داخل أي ViewModel | ✅ TestResultsListViewModel: تعتمد فقط على `ITestResultsListService` + `IPatientService` + `ILabTestService` + `IDialogService` + `INavigationService` + `ICurrentUserService`. | ✅ TestResultEntryViewModel: تعتمد فقط على `ITestResultEntryService` + `IAutoCalculationService` + `IValidator<TestResult>` + `IDialogService` + `ICurrentUserService` + `Func<CalculationConstantsViewModel>`. |
| Constructor Injection حصراً | ✅ | ✅ |
| فتح النوافذ عبر `Func<T>` factory | N/A — TestResultsListView تُعرض عبر ContentControl + DataTemplate (UserControl). | ✅ TestResultEntryView تُفتح عبر `Func<TestResultEntryViewModel>` من TestResultsListViewModel (Part 4.10). CalculationConstantsView عبر `Func<CalculationConstantsViewModel>` من TestResultEntryViewModel (Part 4.5/4.8). |
| `ICurrentUserService` للـAdmin gates | ✅ Part 3.7: `ShowAuditCommand.CanExecute = IsAdmin` + `ShowFinancialTrackingCommand.CanExecute = IsAdmin` (قرار 7). | ✅ لا Admin gates خاصة بـF4 (قرار 8/9/16 لا تفرضها)، لكن `_currentUserService` مستخدَم لتسجيل `EnteredByUserId`/`ReviewedByUserId`/`PrintedByUserId`. |
| FluentValidation | ✅ لا Validator جديد لـF3 (لا كيان form-driven مباشر). | ✅ `TestResultValidator` (Part 4.6). |
| Fire-and-forget من constructor لتحميل بيانات (CP-2 pattern) | ✅ `_ = LoadTodayPatientsAsync();` في constructor `TestResultsListViewModel`. | ✅ لا يُستخدم في `TestResultEntryViewModel` لأن التحميل يتم عبر `LoadForPatientTestAsync` من الـfactory بعد `Show`. |
| `[ObservableProperty]` + `[RelayCommand]` من CommunityToolkit.Mvvm | ✅ | ✅ |
| Enum ComboBox عبر code-behind (Technical Note 4) | ✅ `FilterMode` ComboBox في `TestResultsListView.xaml.cs`. | ✅ لا ComboBox enum-based في F4. |
| `dotnet clean && dotnet build` بعد كل Part | ✅ Build Gate على كل جزء. | ✅ نفس الشيء. |

---

## 7) Clarification Points

| CP | التعارض / السؤال المفتوح | الاقتراح | يتطلب تأكيد؟ |
|---|---|---|---|
| **CP-F3-1** | خطة v3 تقول في Part 3.1 "إضافة enum PatientTestStatus"، لكن الواقع يكشف أن `Enums/TestStatus.cs` موجود بالفعل (بُني في F1، يشمل كل القيم السبع). | لا يُعاد إنشاء الـenum؛ يُستهلَك كما هو. Part 3.1 يقتصر على `PatientTest.cs`. | ❌ (اتخذنا القرار الآمن) |
| **CP-F3-2** | placeholder `PrintReceipt()` في `PatientEntryViewModel.cs` (سطر ~195) يقول "ستُفعَّل في Function 3"، لكن Function 3 وظيفياً لا تشمل "طباعة إيصال" في المرجع. | يُترَك كما هو في هذه الشريحة (خيار افتراضي آمن)؛ يُوثَّق لمعالجة مستقلة لاحقاً. | ✅ (يفضَّل تأكيد من المالك) |
| **CP-F3-3** | placeholder `TodayPatients()` في `PatientEntryViewModel.cs` (سطر ~278) يقول "ستُفعَّل في Function 4"، لكن الزر وظيفياً هو مدخل لـ Function 3 (قائمة مرضى اليوم). | يُفعَّل كمدخل لـ F3 (`_navigationService.NavigateTo<TestResultsListViewModel>()`) — يُنفَّذ ضمن Part 3.9. الرسالة الأصلية في الكود مضللة. | ⚠️ (تنبيه للمالك: الرسالة كانت خاطئة) |
| **CP-F4-1** | خطة v3 تقول في Part 4.2 "كيان LabTestElement — يُبنى في Function 7 لكن يُستخدم هنا"، وهو موجود فعلاً في `Models/Domain/LabTestElement.cs` بعد F7. | Part 4.2 لا ينشئ أي ملف؛ يوثّق فقط الاستهلاك. | ❌ |
| **CP-F4-2** | `TestResultEntryView` يُفتح كـWindow modal (`ShowDialog`) أم كـUserControl داخل ContentControl؟ | خطة v3 تقول "Window أو Modal Overlay". اخترنا **Window modal** لأنه يفتح فوق `TestResultsListView` (نفس نمط `NormalRangeView` و `BarcodeView`) — بذلك لا حاجة لـDataTemplate في `App.xaml`. | ✅ (اختيار متعمَّد؛ يُذكر للتأكيد) |
| **CP-F4-3** | خطة v3 لم تحدد سلوك حفظ نتائج التحاليل لمريض ليس له `PatientVisit` بعد. | نفترض أن `PatientTest` لا يُنشأ إلا بعد إنشاء `PatientVisit` (منطقياً كل تحليل ينتمي إلى زيارة). في Part 3.5 (`GetTodayPatientsAsync`) نُقصر النتائج على من عنده Visit اليوم. | ⚠️ (يُوصى بتأكيد؛ قد يحتاج seed data لتجربة F3/F4). |
| **CP-F4-4** | ثوابت PT/PTT/Hgb — هل تُعتبر إعدادات عامة (واحدة لكامل المعمل) أم قابلة للتخصيص لكل تحليل؟ | افترضنا "إعدادات عامة" (جدول واحد `CalculationConstants` بدون FK لـ`LabTest`) — يطابق النظام المرجعي (زر Constants واحد). | ⚠️ (يفضَّل تأكيد) |
| **CP-F4-5** | خطة v3 تذكر "SavedComment" في Part 4.1 لكن الوصف الوظيفي (المنطقة 4) يذكر "قائمة تعليقات محفوظة" و "التعليق من المعدل الطبيعي" و "زر تراجع". لا تعليمات صريحة لإنشاء SavedComment من داخل نافذة F4. | افترضنا أن `SavedComment` تُدار من F7 (LabTestManagement) لاحقاً؛ حالياً `SavedComment` كيان + جدول فارغ + قراءة فقط. إنشاؤها يُترَك خارج نطاق الشريحة. | ⚠️ (يُوصى تأكيد؛ قد نحتاج شاشة CRUD مصغَّرة لاحقاً) |
| **CP-F4-6** | خطة v3 لا تذكر Admin-gate على أي أمر من أوامر F4 (اختلاف مقصود عن F3 الذي فيه قرار 7). | لا يُضاف Admin-gate على `SaveCommand`/`PrintCommand` في F4. Constants editing متاح للكل حسب الافتراض. | ⚠️ (يُوصى تأكيد؛ ربما يريد المالك تقييد تعديل الثوابت بـAdmin) |

---

## 8) Change Manifest

### 8.1) ملفات تُنشأ (Total: 22 ملفاً)

**Function 3 (10 files)**:
```
Models/Domain/PatientTest.cs                              [Part 3.1]
Models/Domain/AuditLog.cs                                 [Part 3.2]
Migrations/<timestamp>_AddPatientTestsAndAuditLogs.cs     [Part 3.3]
Migrations/<timestamp>_AddPatientTestsAndAuditLogs.Designer.cs   [Part 3.3]
Services/Interfaces/ITestResultsListService.cs            [Part 3.4]
Services/Implementations/TestResultsListService.cs        [Part 3.5]
Converters/TestStatusToIconConverter.cs                   [Part 3.6]
ViewModels/Pages/TestResultsListViewModel.cs              [Part 3.7]
Views/Pages/TestResultsListView.xaml                      [Part 3.8]
Views/Pages/TestResultsListView.xaml.cs                   [Part 3.8]
```

**Function 4 (12 files)**:
```
Models/Domain/TestResult.cs                               [Part 4.1]
Models/Domain/SavedComment.cs                             [Part 4.1]
Models/Domain/CalculationConstants.cs                     [Part 4.5]
Migrations/<timestamp>_AddTestResultsAndConstants.cs      [Part 4.3]
Migrations/<timestamp>_AddTestResultsAndConstants.Designer.cs [Part 4.3]
Services/Interfaces/ITestResultEntryService.cs            [Part 4.4]
Services/Interfaces/IAutoCalculationService.cs            [Part 4.5]
Services/Interfaces/IReportPdfGenerator.cs                [Part 4.7]
Services/Implementations/TestResultEntryService.cs        [Part 4.6]
Services/Implementations/AutoCalculationService.cs        [Part 4.5]
Services/Implementations/ReportPdfGenerator.cs            [Part 4.7]
Models/Validation/TestResultValidator.cs                  [Part 4.6]
ViewModels/Pages/TestResultEntryViewModel.cs              [Part 4.8]
ViewModels/Pages/CalculationConstantsViewModel.cs         [Part 4.5]
Views/Windows/TestResultEntryView.xaml                    [Part 4.9]
Views/Windows/TestResultEntryView.xaml.cs                 [Part 4.9]
Views/Windows/CalculationConstantsView.xaml               [Part 4.5]
Views/Windows/CalculationConstantsView.xaml.cs            [Part 4.5]
```

### 8.2) ملفات تُعدَّل (Total: 5 ملفات)

```
Data/NewLabDbContext.cs
   ├─ +DbSet<PatientTest> PatientTests                             [Part 3.3]
   ├─ +DbSet<AuditLog> AuditLogs                                   [Part 3.3]
   ├─ +DbSet<TestResult> TestResults                               [Part 4.3]
   ├─ +DbSet<SavedComment> SavedComments                           [Part 4.3]
   ├─ +DbSet<CalculationConstant> CalculationConstants             [Part 4.3]
   ├─ +Fluent API (indexes, FKs, decimal precision)                [Parts 3.3 + 4.3]
   └─ +Seed للثوابت (Hgb multipliers + PT/PTT defaults)             [Part 4.3]

App.xaml.cs
   ├─ +Scoped ITestResultsListService                              [Part 3.5]
   ├─ +Scoped ITestResultEntryService                              [Part 4.6]
   ├─ +Scoped IAutoCalculationService                              [Part 4.6]
   ├─ +Scoped IReportPdfGenerator                                  [Part 4.6]
   ├─ +Scoped IValidator<TestResult>                               [Part 4.6]
   ├─ +Transient TestResultsListViewModel                          [Part 3.7]
   ├─ +Transient TestResultEntryViewModel                          [Part 4.6]
   └─ +Transient CalculationConstantsViewModel                     [Part 4.5]

App.xaml
   ├─ +<DataTemplate DataType="TestResultsListViewModel">          [Part 3.8]
   └─ +<converters:TestStatusToIconConverter x:Key="...">          [Part 3.6]

ViewModels/Pages/MainDashboardViewModel.cs
   ├─ Update FunctionDefinition "إدخال نتائج التحاليل" — TargetViewType = typeof(TestResultsListView)  [Part 3.9]
   ├─ +Branch in OpenFunction() لـTestResultsListView             [Part 3.9]
   └─ +[RelayCommand] OpenTestResultsList()                        [Part 3.9]

Views/Windows/MainWindow.xaml
   └─ +<KeyBinding Key="F4" Command="{Binding OpenTestResultsListCommand}" />  [Part 3.9]

ViewModels/Pages/PatientEntryViewModel.cs
   └─ Rewrite body of TodayPatients() — placeholder message → open TestResultsListView (§2.2 / CP-F3-3)  [Part 3.9]
```

### 8.3) ملفات غير متأثرة (تُوثَّق للتأكيد)

```
Models/Domain/*.cs                → لا تُعدَّل (Patient, PatientVisit, LabTest, LabTestElement, NormalRange,
                                     TestGroup, Referral, ReferralPrice, SpecimenType, BarcodeSettings,
                                     BarcodeLabel, PatientCode, User, Role, UserRole, ToolbarItem,
                                     FunctionDefinition, Enums/*)
Services/Interfaces/*.cs (except new files)  → لا تُعدَّل (IPatientService, ILabTestService,
                                                 INormalRangeService, IBarcodeService, IBarcodePrintService,
                                                 IReferralService, IAuthService, IApplicationStartupService,
                                                 IDialogService, INavigationService, ICurrentUserService)
Services/Implementations/*.cs (except new files) → لا تُعدَّل
ViewModels/Pages/LabTestManagementViewModel.cs   → لا تُعدَّل (F7 stable)
ViewModels/Pages/NormalRangeViewModel.cs         → لا تُعدَّل (F8 stable)
ViewModels/Pages/BarcodeViewModel.cs             → لا تُعدَّل (F2 stable)
Views/Pages/LabTestManagementView.xaml           → لا تُعدَّل
Views/Windows/NormalRangeView.xaml               → لا تُعدَّل
Views/Windows/BarcodeView.xaml                   → لا تُعدَّل
Views/Windows/LoginView.xaml                     → لا تُعدَّل
Views/Windows/SetupView.xaml                     → لا تُعدَّل
Views/Controls/LatinSymbolsPad.xaml              → لا تُعدَّل
Views/Controls/DashboardContentControl.xaml      → لا تُعدَّل
Views/Controls/FunctionPlaceholderControl.xaml   → لا تُعدَّل
Helpers/BarcodeImageGenerator.cs                 → لا تُعدَّل
Migrations/*.cs (المكتملة)                       → لا تُعدَّل
NewLab.csproj                                    → لا تُعدَّل (لا NuGet جديدة — QuestPDF متوفرة)
appsettings.json                                 → لا تُعدَّل
```

**ملاحظة على `PatientEntryViewModel.PrintReceipt()`**: يبقى كما هو (CP-F3-2 يطلب التأكيد). لا يُعدَّل في هذه الشريحة.

---

## 9) Dependency Graph

### 9.1) Intra-Slice Dependencies (Function 3)
```
Part 3.1 (PatientTest)  ─┐
Part 3.2 (AuditLog)      ├─→ Part 3.3 (Migration) ─→ Part 3.4 (ITestResultsListService) ─→ Part 3.5 (Impl + DI)
                                                                                              │
                                                                                              ▼
                                                            Part 3.6 (StatusIconConverter) → Part 3.7 (VM) ─→ Part 3.8 (View)
                                                                                                                    │
                                                                                                                    ▼
                                                                                                            Part 3.9 (Toolbar + F4 + Retro F1)
                                                                                                                    │
                                                                                                                    ▼
                                                                                                            Part 3.10 (Build Verification)
```

### 9.2) Intra-Slice Dependencies (Function 4)
```
Part 4.1 (TestResult + SavedComment)  ─┐
Part 4.2 (LabTestElement — existing)   ├─→ Part 4.3 (Migration) ─→ Part 4.4 (ITestResultEntryService)
Part 4.5 (AutoCalc + Constants)        ─┘                                        │
                                                                                 ▼
                            Part 4.7 (ReportPdfGenerator) ────────────────→ Part 4.6 (Impl + DI + Validator)
                                                                                 │
                                                                                 ▼
                                                                            Part 4.8 (VM) ─→ Part 4.9 (View)
                                                                                                    │
                                                                                                    ▼
                                                                                            Part 4.10 (Retro F3)
                                                                                                    │
                                                                                                    ▼
                                                                                            Part 4.11 (Build Verification)
```

### 9.3) Cross-Slice Dependencies (F1/F7/F8 → F3 → F4)
```
F1 (existing)  ──→  Patient, PatientVisit, IPatientService  ────┐
F7 (existing)  ──→  LabTest, LabTestElement, TestGroup,         │
                    ILabTestService                              ├─→ Part 3.4/3.5 (F3 Service)
F8 (existing)  ──→  NormalRange, INormalRangeService            │
                    (GetMatchingRangeAsync, EvaluateValueAsync)  │
                                                                 │
                                                                 ▼
                                                            Part 3.7 (F3 VM) ─┐
                                                                              │
                                                                              ▼
                                                            Part 3.10 (F3 ready)
                                                                              │
                                                                              ▼
F3 (new) ──→ PatientTest, ITestResultsListService  ─────────→ Part 4.4/4.6 (F4 Service uses PatientTest)
                                                                              │
                                                                              ▼
                                                            Part 4.8/4.9 (F4 VM + View)
                                                                              │
                                                                              ▼
                                                            Part 4.10 (Retro F3 ↔ F4)
                                                                              │
                                                                              ▼
                                                            Part 4.11 (F4 ready)
```

### 9.4) External (Placeholder) Dependencies Forward to F5/F6
```
Part 3.7 (VM) ── placeholder OpenDeliveryCommand ─────→ F5 (future)
Part 3.7 (VM) ── placeholder OpenSearchCommand ───────→ F6 (future)
Part 3.1 (PatientTest.Status = Delivered) ────────────→ F5 (uses field)
Part 4.4 (MarkReviewedAsync) + Part 3.5 (TogglePrinted) → F5 (reads state before allowing delivery)
```

---

## 10) Sign-off Criteria

الشريحة تُعتبر مكتملة عند تحقيق كل ما يلي بلا استثناء:

1. **Build**: `dotnet clean && dotnet build` من جذر المشروع → 0 errors / 0 warnings.
2. **Database**:
   - `__EFMigrationsHistory` يحتوي على **صفَّين جديدين** بعد الشريحة (`AddPatientTestsAndAuditLogs`, `AddTestResultsAndConstants`) — بمجموع 7 migrations.
   - الجداول التالية موجودة: `PatientTests`, `AuditLogs`, `TestResults`, `SavedComments`, `CalculationConstants`.
   - `CalculationConstants` يحتوي 8 صفوف seed (Hgb×4, CBC×1, PT×2, PTT×1).
3. **Functional (F3)**:
   - Toolbar → "المرضى" → "إدخال نتائج التحاليل" يفتح `TestResultsListView`.
   - F4 من أي مكان يفتح نفس النافذة.
   - رموز الحالة تظهر بجوار كل مريض (7 رموز مطابقة للتصنيف).
   - البحث بـ`SearchByAttendanceNumber(int)` يعمل كتسلسل يومي بسيط (قرار 6).
   - زرا "ب" و "ت" نشطان لـAdmin ومعطَّلان لـTechnician (قرار 7 مُثبَت باختبار عملي بحسابين).
   - F8/F9/F12 تبدّل الرموز على الشاشة.
   - زر "مرضى اليوم" في `PatientEntryView` يفتح `TestResultsListView` (Retro-Integration §2.2 مؤكَّد).
4. **Functional (F4)**:
   - Enter / نقر مزدوج على تحليل في F3 يفتح `TestResultEntryView`.
   - إدخال Hgb يحسب Hct تلقائياً (Hct = Hgb × 3.3).
   - إدخال Hgb يحسب Hgb% تلقائياً حسب العمر/الجنس (قرار 8).
   - إدخال PT يحسب INR تلقائياً باستخدام ISI + ControlTime من DB (قرار 8).
   - إدخال PTT يحسب PTT Ratio تلقائياً (قرار 8).
   - القيم غير الطبيعية تُلوَّن (Yellow/Orange/Red) عبر `INormalRangeService.EvaluateValueAsync` (قرار 16 عبر F8 القائم).
   - F9 يحفظ + يعيّن `PatientTest.Status = Entered` + يسجّل AuditLog.
   - F11 يفتح معاينة PDF.
   - F12 يطبع PDF + يعيّن `IsPrinted = true` + يسجّل AuditLog.
   - F8 يعيّن `IsReviewed = true` + AuditLog.
   - زر "Constants" يفتح `CalculationConstantsView` — تعديل ISI ثم إعادة الحساب يعطي نتيجة مختلفة.
   - زر "تاريخ مرضي مخصص" **غير موجود في XAML** (قرار 9 مؤكَّد).
5. **MVVM Compliance**:
   - grep تأكيدي: `grep -rn "NewLabDbContext" ViewModels/` → **0 matches**.
   - grep تأكيدي: `grep -rn "new .*View(" ViewModels/` → matches فقط في `PrintBarcodeAsync`, `OpenNormalRangeAsync`, `OpenTestEntryAsync`, `EditConstantsCommand` (كلها ضمن Func<T> factory pattern).
   - كل constructor يتلقى تبعياته عبر DI.
6. **Retro-Integration**:
   - `TodayPatients()` في `PatientEntryViewModel` **لم يعد** يعرض رسالة placeholder — بل يفتح `TestResultsListView`.
   - `OpenTestEntryCommand` في `TestResultsListViewModel` **لم يعد** يعرض placeholder — بل يفتح `TestResultEntryView`.
7. **Documentation**:
   - `Docs/history.md` يُحدَّث بـPhases 9 (F3) + 10 (F4) — تُنفَّذ بعد اكتمال الكود، لا هنا.
8. **Clarification Points**:
   - CP-F3-2 (إيصال في PatientEntryVM) — موقف موثَّق دون تعديل الكود.
   - CP-F3-3 (زر "مرضى اليوم") — منفَّذ.
   - CP-F4-3, CP-F4-4, CP-F4-5, CP-F4-6 — كلها موثَّقة كأسئلة صريحة للمالك.
9. **Data safety**: لا حذف بيانات إنتاجية موجودة (Migrations = Additive فقط).
10. **Backward Compatibility**: كل الوظائف السابقة (F1, F7, F8, F2) تعمل كما كانت — يُختبَر بيدوياً بفتح `PatientEntryView`, `LabTestManagementView`, `BarcodeView`, `NormalRangeView` وحفظ سجل جديد في كل منها.

---

**End of Handoff_Slice_3&4.md**

*هذه الوثيقة تخطيطية بالكامل — لا يوجد كود مكتوب بعد. تنفيذ الأجزاء يبدأ فقط بعد مراجعة الـClarification Points وإقرار المالك.*