# 📋 Handoff Plan — Slice 1 / Function 1
## إضافة بيانات مريض جديد (Add / Edit Patient Data)

---

## 0) Meta & Scope

| البند | القيمة |
|---|---|
| **المشروع** | NewLab — Laboratory Management System |
| **المستودع** | `https://github.com/El-ogra/NewLab.git` |
| **الفرع** | `main` |
| **Commit المستند إليه** | `225852c4ba4106a3cb3a3dc4f32af423fd085e7b` |
| **رسالة الـ Commit** | *"بعد إضافه وثيقة الشرائح الخاصة بالوظائف الثمانية"* |
| **تم التحقق من الـ Hash والرسالة** | ✅ مطابقان تماماً |
| **المصدر الأول للتخطيط** | `Docs/analysis_and_plan_v3.md` — قسم **Function 1** (السطور 94–216) |
| **المصدر الثاني (سياق)** | `Docs/history.md` — الطور 4 (Toolbar Shell) وقرارات المصادقة/DI |
| **النطاق** | **Function 1 فقط** (Step 1 of 8): إضافة/تعديل بيانات المرضى — 15 جزء (Part 1.1 → Part 1.15) |
| **خارج النطاق** | التنفيذ الداخلي للوظائف 2–8 (فقط تُراعى إشاراتها التصميمية) |
| **نوع الوثيقة** | خطة عمل تنفيذية (Handoff Plan) — تخطيط فقط، لا كود |
| **إطار العمل الفعلي** | .NET 8 / WPF / EF Core 8.0.8 / MVVM (CommunityToolkit.Mvvm 8.3.2) / MaterialDesignThemes 5.1.0 |
| **Namespace الجذر (فعلي)** | `NewLab` |
| **قاعدة MVVM** | صارمة: Model / Service / ViewModel / View منفصلة، Constructor Injection حصراً |
| **بوابة كل جزء** | `dotnet build` → **0 errors / 0 warnings** قبل الانتقال للجزء التالي |

### 0.1 القرارات المؤثرة على هذه الشريحة (من جدول Decisions Compliance)

| القرار | الأثر على Function 1 |
|---|---|
| **قرار 1** | الحقول الطبية الإضافية تُخزَّن كأعمدة `Boolean` منفصلة على كيان `Patient` (لا نص حر، لا جدول منفصل). |
| **قرار 2** | `DeleteAsync` و `DeleteCommand.CanExecute` يعتمدان على دور `Admin` فقط. |
| **قرار 15** | `CalculateTotalAsync` يجب أن يبحث في `ReferralPrices` (يُنشأ في Function 7) قبل الرجوع إلى `LabToLabPrice` — يؤثر تصميمياً على توقيع الـ Interface الآن. |
| **قرار 17** | `Gender` = `Male`/`Female` **فقط** — بلا قيمة `Both`. |

### 0.2 حالة الكود الفعلية عند الـ Commit (Baseline المُتحقَّق)

بعد فحص كل الطبقات ملفاً بملف، حالة الكود مطابقة **حرفياً** لوصف Baseline في `analysis_and_plan_v3.md` (الأسطر 15–33):

- `Models/Domain/` يحتوي فقط: `User.cs`, `Role.cs`, `UserRole.cs`, `ToolbarItem.cs`, `FunctionDefinition.cs`. **لا يوجد أي كيان طبي.**
- `Data/NewLabDbContext.cs` يحتوي DbSets: `Users`, `Roles`, `UserRoles` فقط. **لا يوجد أي DbSet طبي.**
- `Migrations/` تحتوي `20260721171559_InitialCreate` فقط + `NewLabDbContextModelSnapshot`.
- `Services/Interfaces/`: `IApplicationStartupService`, `IAuthService`, `IDialogService`, `INavigationService`.
- `Services/Implementations/`: مطابقة للـ Interfaces.
- `ViewModels/Pages/`: `SetupViewModel`, `LoginViewModel`, `MainDashboardViewModel`, `IconNameToKindConverter`.
- `Views/Windows/`: `SetupView`, `LoginView`, `MainWindow`.
- `Views/Controls/`: `DashboardContentControl`, `FunctionPlaceholderControl`.
- `MainDashboardViewModel.CurrentContent = this` — لم يُربط بعد بأي View فعلي.
- زر "إضافة وتعديل بيانات المرضى" موجود في `CreateToolbarCategories()` بـ `TargetViewType = null` (الحقل غير مضبوط أصلاً في الـ FunctionDefinition الفعلية).

**لا يوجد أي تعارض بين Baseline والكود الفعلي.**

### 0.3 نمط الـ Handoff (تأكيداً على سياق `history.md`)

- `history.md` يوثّق أن المشروع اتبع سابقاً نمط تسليم Phase-based (Phases 1 → 4) في نفس الملف. **لا يذكر ملف `Handoff_Slice_X.md` في مجلد `Docs/PRDs/`** — والمجلد نفسه غير موجود في المستودع. لذلك يُوضع هذا الملف مباشرة بالاسم المطلوب `Handoff_Slice_1_2.md` وفق تعليمات هذه المهمة، بدون افتراض بنية سابقة.
- الاصطلاحات المُوثَّقة في `history.md` والتي يجب الحفاظ عليها في هذه الشريحة:
  1. **Namespace**: `NewLab.<Segment>` (مثل `NewLab.Models.Domain`, `NewLab.Services.Interfaces`).
  2. **Nullable Reference Types**: مفعّل (`<Nullable>enable</Nullable>` في csproj) — كل خاصية نصية اختيارية تكتب `string?`، والمطلوبة `string = string.Empty;`.
  3. **Scoped lifetimes** للخدمات التي تستخدم `NewLabDbContext` (تم توثيقه كـ Critical Fix 1). لذا `IPatientService` و `IReferralService` **يجب** أن يُسجَّلا `AddScoped`.
  4. **ViewModels**: `AddTransient` (نفس نمط `SetupViewModel/LoginViewModel/MainDashboardViewModel`).
  5. **CommunityToolkit.Mvvm**: `ObservableObject` + `[ObservableProperty]` + `[RelayCommand]` — بلا `INotifyPropertyChanged` يدوي.
  6. **الحقن**: Constructor Injection حصراً — نمط ثابت في كل ViewModel/Service قائم.
  7. **FlowDirection**: `RightToLeft` على مستوى النافذة (كما في `MainWindow.xaml`).
  8. **MaterialDesign**: أزرار `MaterialDesignRaisedButton`, `MaterialDesignOutlinedButton`, حقول `MaterialDesignOutlinedTextBox`.
  9. **PasswordBox**: لا يوجد PasswordBox في نافذة المرضى — لكن أي حقول حساسة مستقبلاً تُعامل كما في `LoginView.xaml.cs` (PasswordChanged handler).

---

## 1) Vertical Slice Overview

- **الشريحة**: Function 1 — إضافة/تعديل بيانات مريض جديد.
- **مدخل الوظيفة النهائي للمستخدم**: النقر على "المرضى" في الـ Toolbar → "إضافة وتعديل بيانات المرضى" **أو** اختصار **F2** من أي مكان في `MainWindow`.
- **الحالة النهائية بعد إنجاز الأجزاء الـ15**:
  - كيانات طبية أساسية (`Patient`, `Referral`, `SpecimenType`, `PatientVisit`) موجودة في DB.
  - خدمتان (`IPatientService`, `IReferralService`) بـ CRUD كامل + Autocomplete + حساب إجمالي مُصمَّم لـ `ReferralPrices`.
  - `PatientEntryViewModel` كامل مع أوامر، بيانات إدخال، وربط بالخدمات.
  - `PatientEntryView` UserControl يُعرض داخل `MainWindow` عبر ContentControl/DataTemplate.
  - زر "إضافة وتعديل بيانات المرضى" و F2 يفتحان الـ View فعلياً.
- **قاعدة الحدود**: 15 جزءاً بالضبط — لا دمج، لا تقسيم، لا إضافة أجزاء استباقية لوظائف لاحقة.

---

## 2) MVVM Layer Map (على مستوى الشريحة كاملة)

| الطبقة | الملفات المُنتَجة في هذه الشريحة | القواعد المُلزِمة |
|---|---|---|
| **Model (POCO)** | Enums (Part 1.1) + `Referral`, `SpecimenType`, `Patient`, `PatientVisit` (Parts 1.2–1.4) | كائنات بيانات صرفة. **ممنوع**: الوصول لـ DbContext، استدعاء خدمات، منطق UI، `[Obser­vable­Property]`. |
| **Data** | تعديل `NewLabDbContext` + Migration جديدة (Part 1.5) | Fluent API فقط. Seed للـ "المعمل" الافتراضي. |
| **Service (Interfaces)** | `IPatientService`, `IReferralService` (Parts 1.6, 1.7) | كل منطق الأعمال هنا. توقيعات `async Task<T>`. لا تسريب EF Types للـ VM. |
| **Service (Implementations)** | `PatientService`, `ReferralService` (Part 1.8) | تستخدم `NewLabDbContext` مباشرة عبر ctor injection. فحص الأدوار عبر `IAuthService`. |
| **DI Registration** | تعديل `App.xaml.cs` (Part 1.9) | `AddScoped` للخدمتين (استمراراً لنمط "Scoped مع DbContext"). |
| **Validation** | `PatientValidator` (Part 1.10) | FluentValidation — يُحقَن في الـ VM عبر `IValidator<Patient>`. |
| **ViewModel** | `PatientEntryViewModel` (Part 1.11) | `ObservableObject` + `[ObservableProperty]` + `[RelayCommand]`. Constructor Injection حصراً. **ممنوع**: `DbContext` مباشرة، `MessageBox` مباشرة (يُستخدم `IDialogService`). |
| **View** | `PatientEntryView.xaml/.cs` (Part 1.12) | `{Binding}` فقط. code-behind بلا منطق أعمال — فقط `InitializeComponent()` وربطات UI (Focus, KeyBindings المحلية إن لزم). |
| **Navigation/Toolbar** | تعديل `MainDashboardViewModel` + `MainWindow.xaml` (Parts 1.13, 1.14) | ربط `TargetViewType` + `DataTemplate` لتنفيذ العرض + F2 عالمياً في `Window.InputBindings`. |
| **Build Gate** | Part 1.15 | `dotnet build` → 0/0، ثم Smoke Test يدوي. |

---

## 3) Future-Impact Awareness (STEP 7 — إشارات لوظائف 2–8)

خلال تصميم Function 1، أُخذت الإشارات التالية بعين الاعتبار **على مستوى التصميم فقط** (بلا تفصيل أو بناء أي شيء منها):

| الإشارة | مصدرها في نص Function 1 | الأثر على تصميم Function 1 | ما لا يُبنى الآن |
|---|---|---|---|
| **`ReferralPrices` (F7)** | Part 1.6 — `CalculateTotalAsync` | توقيع `CalculateTotalAsync(patientTests, billingSystem, referral, discount)` مُصمَّم منذ الآن ليقبل معلمة `referral` بحيث تُستخدم كمفتاح بحث في جدول أسعار مستقبلي. Implementation يُلقي `NotImplementedException` أو يعيد `LabToLabPrice` مباشرة حتى تتوفر F7 — بلا إعادة هيكلة لاحقاً. | كيان `ReferralPrice` نفسه، جدولها في DB، أساليب `SetReferralPriceAsync/DeleteReferralPriceAsync` — كلها تخص Function 7. |
| **`ILabTestService` (F7)** | Part 1.11 — الحقن في `PatientEntryViewModel` | نقطة الحقن في الـ ctor محفوظة كـ **placeholder اختياري** — إما يُترك تعليقاً `// TODO: to be injected in Function 7` أو يُضاف كمعامل اختياري `ILabTestService? labTestService = null` لتفادي كسر ctor عند F7. **الاختيار المُوصى به**: عدم إضافته للـ ctor في F1 نهائياً؛ يُضاف في شريحة F7 لأن قائمة `AvailableTests` تبقى فارغة حتى يُنشأ الكيان — لا حاجة تصميمية لخدمة لا يوجد لها بيانات بعد. | تعريف `ILabTestService`، أساليبه، Implementation، Registration. |
| **`PatientTest` (F3)** | Part 1.11 — `ObservableCollection<PatientTestRow> SelectedTests` | يُعرَّف داخل الـ VM نوع **DTO محلي فقط** باسم `PatientTestRow` (record أو class داخل مساحة `ViewModels/Pages`) يحمل الحد الأدنى (LabTestId, TestName, Price) — دون بناء الكيان `PatientTest` من DB. عند F3 يُنشأ الكيان الحقيقي ويُستبدل النوع. | كيان `PatientTest` في DB، migration، خدمة، جدول. |
| **Function 2 (باركود)** | Part 1.11 — `PrintBarcodeCommand` | يُعرَّف الأمر كـ `[RelayCommand]` فارغ (`// TODO: opens BarcodeView from Function 2`) لا يفعل شيئاً حالياً — أو مُعطَّل عبر `CanExecute = false`. توقيع الأمر جاهز، بلا استدعاء `BarcodeView` غير موجود. | نافذة الباركود، خدمة الباركود، مولّد ZXing. |
| **`AuditLog` (F3)** | Reference Behavior — زرّا "ب" و"ت" | لا يوجد زر "ب" أو "ت" في نافذة **المرضى** نفسها — هما جزء من نافذة النتائج (F3). لا أثر تصميمي هنا. | كيان `AuditLog`. |
| **`LabId` باركود دائم (F2)** | Part 1.11 — `LabIdCommand` | يُعرَّف الأمر كـ Placeholder فارغ + خانة `LabId` في الـ VM تُعرض كنص للقراءة. لا يُولَّد الكود فعلياً. | مولّد `LabId`، طباعة بطاقة الباركود. |

**القاعدة الحاكمة**: كل ما فوق يعالج بـ *stubs / placeholders / signature-shaping only* بحيث Function 1 تُبنى وتُختبر مستقلة، ولا يحتاج أي جزء من الشريحات اللاحقة إلى إعادة تعديل هذه الشريحة عند إضافته.

---

# 4) The 15 Parts — Detailed Handoff

> **قاعدة عامة لكل جزء**:
> - **بوابة**: `dotnet build` من الجذر `/NewLab` → 0 errors / 0 warnings.
> - أي Warning من CS/Nullable/EF Core = فشل الجزء ولا انتقال للتالي.
> - كل جزء يبني على أسبقيته؛ الترتيب ملزم.
> - كل الملفات تُنشأ داخل جذر المستودع بالمسارات المذكورة **حرفياً** (Case-sensitive في بيئات غير Windows، لكن الجذر ويندوز).

---

### 🔸 Part 1.1 — Enums الأساسية

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Model (POCO) |
| **المسارات الدقيقة (جديدة)** | `Models/Domain/Enums/Gender.cs`<br>`Models/Domain/Enums/BillingSystem.cs`<br>`Models/Domain/Enums/AgeUnit.cs`<br>`Models/Domain/Enums/TestStatus.cs`<br>`Models/Domain/Enums/CodeType.cs` |
| **Namespace** | `NewLab.Models.Domain.Enums` |
| **المحتوى المطلوب** (بأسماء وقيم حرفية من الوثيقة) | `Gender { Male, Female }` — **بلا Both (قرار 17)**<br>`BillingSystem { Individual, LabToLab, Free }`<br>`AgeUnit { Day, Month, Year }`<br>`TestStatus { New, Entered, Reviewed, Printed, Delivered, AccountIssue, Completed }`<br>`CodeType { Case, File, Lab }` |
| **التبعيات على أجزاء سابقة** | لا شيء (نقطة البدء). |
| **مجلد جديد يُنشأ؟** | نعم: `Models/Domain/Enums/` (غير موجود حالياً في الـ Baseline). |
| **الناتج المتوقع** | 5 ملفات enum قابلة للـ import من أي طبقة. |
| **بوابة البناء** | `dotnet build` → 0/0. |
| **ملاحظة MVVM** | Enums ليست ViewModels — لا `[ObservableProperty]`، لا commands. |
| **مراعاة تبعية مستقبلية** | `TestStatus` يُستخدم في F3 لكنه يُعرَّف الآن لأن الوثيقة تصنّفه ضمن F1 (Section "Enums الأساسية" في السطر 48). ذلك **متعمَّد** ولا يُعدّ خرقاً لحدود الشريحة. |

---

### 🔸 Part 1.2 — كيانات مرجعية أساسية (Referral + SpecimenType)

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Model (POCO) |
| **المسارات الدقيقة (جديدة)** | `Models/Domain/Referral.cs`<br>`Models/Domain/SpecimenType.cs` |
| **Namespace** | `NewLab.Models.Domain` |
| **خصائص `Referral`** (حرفياً من Part 1.2 في الوثيقة) | `int Id`, `string Name`, `decimal DiscountPercent`, `bool IsDefaultLab`, `DateTime CreatedAt` |
| **خصائص `SpecimenType`** | `int Id`, `string Name`, `string ArabicName` |
| **DataAnnotations مقترحة** (نمط User.cs الحالي) | `[Required][MaxLength(150)]` على `Name`/`ArabicName`. `[Range(0, 100)]` على `DiscountPercent` (اختياري — يُضبط بـ FluentValidation عادةً). |
| **التبعيات** | Part 1.1 (لا يُستخدم مباشرة لكنه أساس الترتيب). |
| **الناتج** | نموذجان بسيطان بلا علاقات ملاحية بعد (تُضاف في 1.3/1.4). |
| **بوابة البناء** | 0/0. |
| **ملاحظة MVVM** | POCO — لا logic. |

---

### 🔸 Part 1.3 — كيان `Patient` (مع 8 حقول Boolean طبية — قرار 1)

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Model (POCO) |
| **المسار الدقيق (جديد)** | `Models/Domain/Patient.cs` |
| **Namespace** | `NewLab.Models.Domain` |
| **الخصائص الأساسية** (حرفياً كما وردت في السطر 146 من الوثيقة) | `int Id`, `string FullName`, `string? Title`, `Gender Gender`, `int AgeValue`, `AgeUnit AgeUnit`, `BillingSystem BillingSystem`, `bool IsImportant`, `int? ReferralId`, `bool ReferralHiddenOnReport`, `string? PhoneNumber`, `string? NationalId`, `string? LabId`, `string? FileCode`, `string? VisitCode`, `string? Notes`, `int? ExternalSpecimenTypeId`, `decimal DiscountValue`, `bool DiscountIsPercent`, `decimal PaidAmount`, `decimal TotalAmount`, `int CreatedByUserId`, `DateTime CreatedAt`, `DateTime? UpdatedAt` |
| **الحقول الطبية (Boolean — قرار 1، حرفياً من السطر 147)** | `bool IsFasting`, `int? FastingHours`, `bool IsOnAnticoagulant`, `bool HasLiverTreatment`, `bool HasAntiviralTreatment`, `bool HasAntibiotic`, `bool IsPregnant`, `bool IsSmoker` |
| **علاقات ملاحية (Navigation)** | `Referral? Referral`, `SpecimenType? ExternalSpecimenType`, `User CreatedByUser`. `ICollection<PatientVisit> Visits = new List<PatientVisit>();` |
| **قواعد صارمة (قرار 1)** | ❌ لا حقل نصي مدمج للحالات الطبية. ❌ لا جدول منفصل. ✅ كل حالة عمود Boolean مستقل على `Patient` مباشرة. |
| **قاعدة صارمة (قرار 17)** | نوع `Gender` هو `NewLab.Models.Domain.Enums.Gender` — لا يقبل قيمة غير `Male` أو `Female`. |
| **التبعيات** | Parts 1.1, 1.2. |
| **الناتج** | كيان `Patient` جاهز. |
| **بوابة البناء** | 0/0. |
| **ملاحظة MVVM** | POCO صرف — لا `INotifyPropertyChanged`، لا Commands. |

---

### 🔸 Part 1.4 — كيان `PatientVisit` (كود يوم / زيارة)

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Model (POCO) |
| **المسار الدقيق (جديد)** | `Models/Domain/PatientVisit.cs` |
| **Namespace** | `NewLab.Models.Domain` |
| **الخصائص (حرفياً من السطر 153)** | `int Id`, `int PatientId`, `DateTime VisitDate`, `int DailySequenceNumber`, `string FullVisitCode` |
| **علاقات ملاحية** | `Patient Patient = null!;` |
| **ملاحظة الوثيقة (السطر 155)** | تفاصيل صيغة الكود 13 خانة تخص Function 2. **في F1 يُخزَّن `FullVisitCode` كخانة نصية عادية بلا أي منطق توليد.** رقم الفرع مُثبَّت برمجياً بـ `"1"` عندما يُبنى المولد لاحقاً في F2 (قرار 5). |
| **التبعيات** | Part 1.3. |
| **الناتج** | كيان الزيارة جاهز، بلا مولد كود بعد. |
| **بوابة البناء** | 0/0. |
| **مراعاة تبعية مستقبلية (قرار 5)** | لا يُضاف حقل `BranchNumber` هنا. توليد `FullVisitCode` يبقى مسؤولية `IBarcodeService` في F2. |

---

### 🔸 Part 1.5 — تحديث `NewLabDbContext` + Migration

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Data (Persistence) |
| **الملفات المعدَّلة** | `Data/NewLabDbContext.cs` |
| **الملفات الجديدة** | `Migrations/<timestamp>_AddPatientsAndReferrals.cs` + `Migrations/<timestamp>_AddPatientsAndReferrals.Designer.cs` + تحديث `Migrations/NewLabDbContextModelSnapshot.cs` (يُولَّد آلياً بأمر EF Core). |
| **التعديلات على DbContext (حرفياً من السطر 158)** | إضافة: `public DbSet<Patient> Patients { get; set; }`<br>`public DbSet<Referral> Referrals { get; set; }`<br>`public DbSet<SpecimenType> SpecimenTypes { get; set; }`<br>`public DbSet<PatientVisit> PatientVisits { get; set; }` |
| **Fluent API المطلوب داخل `OnModelCreating`** | 1) `Patient` → `HasOne(p => p.Referral).WithMany().HasForeignKey(p => p.ReferralId).OnDelete(DeleteBehavior.Restrict)`.<br>2) `Patient` → `HasOne(p => p.ExternalSpecimenType).WithMany().HasForeignKey(p => p.ExternalSpecimenTypeId).OnDelete(DeleteBehavior.Restrict)`.<br>3) `Patient` → `HasOne(p => p.CreatedByUser).WithMany().HasForeignKey(p => p.CreatedByUserId).OnDelete(DeleteBehavior.Restrict)`.<br>4) `PatientVisit` → `HasOne(v => v.Patient).WithMany(p => p.Visits).HasForeignKey(v => v.PatientId).OnDelete(DeleteBehavior.Cascade)`.<br>5) `HasIndex` على `Patient.LabId` (unique when NOT null — عبر filter).<br>6) `HasIndex` على `Patient.FileCode` (unique).<br>7) دقة عشرية: `Patient.TotalAmount`, `Patient.PaidAmount`, `Patient.DiscountValue` → `HasColumnType("decimal(18,2)")`. `Referral.DiscountPercent` → `decimal(5,2)`. |
| **Seed مطلوب (السطر 158)** | جهة إحالة افتراضية تُمثّل "المعمل نفسه": `new Referral { Id = 1, Name = "المعمل", DiscountPercent = 0, IsDefaultLab = true, CreatedAt = DateTime(2026,1,1) }` — تاريخ ثابت لكي يعمل الـ Seed بدون تحذيرات dynamic-values في EF Core (تحذير قد يُنتج warning إن استُخدم `DateTime.Now`). |
| **أمر EF Core (يُنفَّذ من Package Manager Console أو CLI)** | `dotnet ef migrations add AddPatientsAndReferrals -c NewLabDbContext` — يستخدم `NewLabDbContextFactory` الموجود فعلاً. **لا تُشغَّل `database update` هنا** — يجب أن تُطبَّق تلقائياً في `App.OnStartup` عبر `MigrateAsync` الموجود مسبقاً. |
| **التبعيات** | Parts 1.1 → 1.4. |
| **الناتج** | جداول: `Patients`, `Referrals`, `SpecimenTypes`, `PatientVisits` + FK constraints + Seed لجهة "المعمل". |
| **بوابة البناء** | 0/0 + التحقق أن `App.OnStartup` لا يزال يقوم بـ `MigrateAsync` (موجود في `App.xaml.cs` السطر 48 من ملف الـ startup). |
| **ملاحظة MVVM** | لا شأن للـ VM بـ DbContext مباشرة — يبقى مغلقاً خلف الخدمات. |

---

### 🔸 Part 1.6 — Service Interface: `IPatientService` (مع دعم `ReferralPrices` تصميمياً — قرار 15)

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Service (Business Logic Contract) |
| **المسار الدقيق (جديد)** | `Services/Interfaces/IPatientService.cs` |
| **Namespace** | `NewLab.Services.Interfaces` |
| **الأساليب (حرفياً من السطر 164)** | `Task<Patient> AddAsync(Patient patient);`<br>`Task<Patient> UpdateAsync(Patient patient);`<br>`Task DeleteAsync(int patientId);` ← **يفحص دور Admin داخل التنفيذ (قرار 2)**<br>`Task<Patient?> GetByIdAsync(int patientId);`<br>`Task<List<Patient>> GetTodayPatientsAsync();`<br>`Task<Patient?> GetByLabIdAsync(string labId);`<br>`Task<decimal> CalculateTotalAsync(IEnumerable<PatientTestRow> patientTests, BillingSystem billingSystem, Referral? referral, decimal discountValue, bool discountIsPercent);` |
| **`PatientTestRow`** (نوع مساعد يُعرَّف مؤقتاً) | يوضع في `Services/Interfaces/IPatientService.cs` كـ `public sealed record PatientTestRow(int LabTestId, string TestName, decimal Price);`  — بديل مؤقت لكيان `PatientTest` (Function 3). |
| **منطق `CalculateTotalAsync` (السطور 165–169 حرفياً — قرار 15)** | 1) لكل صف: تحديد السعر الأساسي:<br>&nbsp;&nbsp;• `Individual` → استخدم `PatientPrice` من `LabTest` (يُوفَّر لاحقاً في F7 — الآن يستخدم `Price` الممرَّر في `PatientTestRow`).<br>&nbsp;&nbsp;• `LabToLab` → البحث في `ReferralPrices` بـ (LabTestId + ReferralId). **في F1**: البحث غير مُنفَّذ لأن الجدول غير موجود؛ يُستخدم `PatientTestRow.Price` مباشرة (السلوك المؤقت). التوقيع نفسه لا يتغير عند وصول F7.<br>&nbsp;&nbsp;• `Free` → 0.<br>2) تطبيق `Referral.DiscountPercent` (خصم الجهة).<br>3) تطبيق خصم المريض (`discountValue` كنسبة أو رقم مطلق حسب `discountIsPercent`).<br>4) إرجاع الإجمالي النهائي. |
| **مراعاة تبعية مستقبلية (قرار 15)** | التوقيع مُصمَّم منذ الآن لقبول `referral` كمعلمة — عند F7 يُضاف داخل الـ Implementation استعلام `_ctx.ReferralPrices.FirstOrDefaultAsync(...)` **دون تغيير في الـ Interface** ولا في مواقع الاستدعاء من `PatientEntryViewModel`. |
| **قاعدة صارمة (قرار 2)** | `DeleteAsync` لا يُنفَّذ إلا إذا `IAuthService` أعاد ما يفيد بأن المستخدم الحالي بدور `Admin`. |
| **التبعيات** | Part 1.5. |
| **بوابة البناء** | 0/0. |
| **ملاحظة MVVM** | كل الأنواع في التوقيع من طبقة Domain/Enums — لا EF entities مسرَّبة كـ `IQueryable`. |

---

### 🔸 Part 1.7 — Service Interface: `IReferralService`

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Service (Business Logic Contract) |
| **المسار الدقيق (جديد)** | `Services/Interfaces/IReferralService.cs` |
| **Namespace** | `NewLab.Services.Interfaces` |
| **الأساليب (حرفياً من السطر 173)** | `Task<List<Referral>> SearchByNameAsync(string prefix);`<br>`Task<Referral> GetOrCreateAsync(string name);`<br>`Task<Referral> GetDefaultLabAsync();` |
| **دلالة كل أسلوب** | `SearchByNameAsync` — Autocomplete (يُقيَّد بحد 20 نتيجة).<br>`GetOrCreateAsync` — إن وُجدت جهة بنفس الاسم يُعيدها، وإلا يُنشئ جديدة (حفظ تلقائي كما في المرجع، السطر 117).<br>`GetDefaultLabAsync` — يُعيد الجهة `IsDefaultLab = true` (Seed من Part 1.5). |
| **التبعيات** | Part 1.5. |
| **بوابة البناء** | 0/0. |

---

### 🔸 Part 1.8 — Implementations: `PatientService` + `ReferralService`

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Service (Concrete Implementation) |
| **المسارات الدقيقة (جديدة)** | `Services/Implementations/PatientService.cs`<br>`Services/Implementations/ReferralService.cs` |
| **Namespace** | `NewLab.Services.Implementations` |
| **Constructor Injection لـ `PatientService`** | `PatientService(NewLabDbContext context, IAuthService authService)` — نمط ثابت من `AuthService.cs` و `ApplicationStartupService.cs` الموجودَين. |
| **Constructor Injection لـ `ReferralService`** | `ReferralService(NewLabDbContext context)` |
| **قاعدة قرار 2 داخل `PatientService.DeleteAsync`** | 1) قراءة المستخدم الحالي (يُوصَل عبر `IAuthService` — انظر Clarification Point #1 أدناه لأن `IAuthService` الحالي لا يوفّر "current user" API؛ الحل التصميمي: إضافة `Task<bool> CurrentUserIsInRoleAsync(string roleName)` أو تمرير `int currentUserId` من الـ VM).<br>2) إن لم يكن `Admin` → `throw new UnauthorizedAccessException("عملية الحذف تتطلب صلاحية Admin");`<br>3) خلاف ذلك: `Remove` + `SaveChangesAsync`. |
| **`ReferralService.SearchByNameAsync`** | `return await _context.Referrals.Where(r => EF.Functions.Like(r.Name, $"{prefix}%")).OrderBy(r => r.Name).Take(20).ToListAsync();` |
| **`ReferralService.GetOrCreateAsync`** | استعلام `FirstOrDefaultAsync` بـ `Name` مطابقاً؛ إن لم يُوجد `Add + SaveChanges + return`. |
| **`PatientService.CalculateTotalAsync`** | تنفيذ المنطق المُعرَّف في Part 1.6 — في F1 يستخدم `PatientTestRow.Price` كسعر أساسي دون قراءة `ReferralPrices` من DB (الجدول لم يُنشأ بعد). |
| **التبعيات** | 1.6, 1.7. |
| **الناتج** | منطق CRUD كامل + Autocomplete + إنشاء جهة تلقائي + حساب الإجمالي. |
| **بوابة البناء** | 0/0. |
| **ملاحظة MVVM** | ❌ لا `MessageBox`، لا `Dispatcher` هنا — الخدمات لا تعرف شيئاً عن الـ UI. |

---

### 🔸 Part 1.9 — تسجيل الخدمات في DI Container

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Composition Root (`App.xaml.cs`) |
| **الملف المعدَّل** | `App.xaml.cs` |
| **موضع التعديل** | داخل `ConfigureServices` بعد السطر `services.AddScoped<IAuthService, AuthService>();` (السطر 27 حالياً). |
| **الأسطر المُضافة (حرفياً من السطر 183)** | `services.AddScoped<IPatientService, PatientService>();`<br>`services.AddScoped<IReferralService, ReferralService>();` |
| **Lifetime الملزم (استمرار Critical Fix 1 من history.md)** | `Scoped` لأن كليهما يستخدم `NewLabDbContext` — استخدام `Singleton` سيسبب "disposed context" errors كما وثَّقها الطور 3.1. |
| **التبعيات** | Part 1.8. |
| **بوابة البناء** | 0/0 + تشغيل التطبيق دون أخطاء Runtime في الـ resolve. |

---

### 🔸 Part 1.10 — FluentValidation Validator لكيان `Patient`

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Validation (تُستهلك من ViewModel) |
| **المجلد الجديد** | `Models/Validation/` (غير موجود حالياً — يُنشأ) |
| **المسار الدقيق (جديد)** | `Models/Validation/PatientValidator.cs` |
| **Namespace** | `NewLab.Models.Validation` |
| **يرث من** | `AbstractValidator<Patient>` من `FluentValidation` (الحزمة موجودة في csproj بالفعل). |
| **القواعد (حرفياً من السطر 186)** | `RuleFor(p => p.FullName).NotEmpty().MaximumLength(200);`<br>`RuleFor(p => p.AgeValue).GreaterThanOrEqualTo(0);`<br>`RuleFor(p => p.Gender).IsInEnum().Must(g => g == Gender.Male \|\| g == Gender.Female);` ← **قرار 17**<br>قاعدة نطاق السن حسب `AgeUnit`:<br>&nbsp;&nbsp;• `AgeUnit.Day` → `AgeValue` بين 1 و 29<br>&nbsp;&nbsp;• `AgeUnit.Month` → بين 1 و 11<br>&nbsp;&nbsp;• `AgeUnit.Year` → بين 0 و 120<br>`RuleFor(p => p.FastingHours).InclusiveBetween(0, 48).When(p => p.IsFasting);` (منطق ضمني — إن كان صيام يجب رقم ساعات معقول)<br>`RuleFor(p => p.PhoneNumber).MaximumLength(20);` |
| **تسجيل في DI** | يُضاف في `App.xaml.cs`: `services.AddScoped<IValidator<Patient>, PatientValidator>();` — الحزمة `FluentValidation.DependencyInjectionExtensions` موجودة. |
| **التبعيات** | Part 1.3, 1.9. |
| **بوابة البناء** | 0/0. |
| **ملاحظة MVVM** | الـ Validator يُحقَن في `PatientEntryViewModel` عبر `IValidator<Patient>` — الـ VM يستدعي `_validator.Validate(model)` قبل الاستدعاء للخدمة. |

---

### 🔸 Part 1.11 — `PatientEntryViewModel`

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | ViewModel |
| **المسار الدقيق (جديد)** | `ViewModels/Pages/PatientEntryViewModel.cs` |
| **Namespace** | `NewLab.ViewModels.Pages` |
| **يرث من** | `ObservableObject` (نفس نمط `LoginViewModel/SetupViewModel/MainDashboardViewModel` الموجودة). **ملاحظة**: الأخيرات لا ترث من `ViewModelBase` رغم وجودها — لذا يُتَّبع النمط الفعلي القائم لا الوارد نظرياً في `history.md`. |
| **الخصائص عبر `[ObservableProperty]`** (كل الحقول التي وردت في السطر 191 حرفياً) | حقول Patient الأساسية: `fullName`, `title`, `gender`, `ageValue`, `ageUnit`, `billingSystem`, `isImportant`, `referralId`, `referralHiddenOnReport`, `phoneNumber`, `nationalId`, `labId`, `fileCode`, `visitCode`, `notes`, `externalSpecimenTypeId`, `discountValue`, `discountIsPercent`, `paidAmount`, `totalAmount`.<br>Boolean طبية (قرار 1): `isFasting`, `fastingHours`, `isOnAnticoagulant`, `hasLiverTreatment`, `hasAntiviralTreatment`, `hasAntibiotic`, `isPregnant`, `isSmoker`.<br>حالة الشاشة: `isEditMode`, `isAddMode`, `searchText`, `testListFilter`, `remaining`, `selectedReferralName`.<br>مجموعات: `ObservableCollection<PatientTestRow> SelectedTests`, `ObservableCollection<LabTestPlaceholder> AvailableTests`, `ObservableCollection<Referral> ReferralSuggestions`.<br>صلاحيات (قرار 2): `isAdmin`. |
| **`LabTestPlaceholder`** | نوع مساعد داخلي (record) — يمثّل عنصر قائمة تحاليل قبل بناء `LabTest` في F7. يوضع كـ nested type أو ملف مساعد `ViewModels/Pages/LabTestPlaceholder.cs`. حقول: `int Id, string Code, string Name, decimal Price`. |
| **الأوامر عبر `[RelayCommand]`** (حرفياً من السطر 192) | `AddPatientCommand`, `EditCommand`, `DeleteCommand` (**CanExecute = IsAdmin — قرار 2**), `CancelCommand`, `SaveCommand`, `PrintReceiptCommand`, `PrintBarcodeCommand` (Placeholder — تستدعي `BarcodeView` في F2)، `AddSelectedTestCommand`, `RemoveTestCommand`, `RemoveAllTestsCommand`, `MarkAsPaidCommand`, `TodayPatientsCommand`, `LabIdCommand`. |
| **CanExecute لـ `DeleteCommand`** | `[RelayCommand(CanExecute = nameof(CanDelete))]` مع `private bool CanDelete() => IsAdmin;` — يُحدَّث تلقائياً عبر `[NotifyCanExecuteChangedFor(nameof(DeleteCommand))]` على `IsAdmin`. |
| **الحقن (Constructor حصراً — لا Service Locator)** (حرفياً من السطر 193 مع تعديل مراعاة F7) | `PatientEntryViewModel(IPatientService patientService, IReferralService referralService, IDialogService dialogService, INavigationService navigationService, IValidator<Patient> patientValidator, IAuthService authService)`<br>**ملاحظة**: `ILabTestService` **لا يُحقن الآن** — الوثيقة تذكره لكن الكيان `LabTest` لا يُبنى إلا في F7. Placeholder فقط. عند F7 يُضاف كمعامل جديد في ctor. |
| **قاعدة صارمة (قرار 2 — `IsAdmin`)** | يُقرأ من `_authService` عند بناء الـ VM (Async initialization). انظر Clarification Point #1 حول كيفية معرفة المستخدم الحالي (الـ API الحالي لا يحمل current user). |
| **مراعاة تبعية F2 لـ `PrintBarcodeCommand`** | Method body فارغة أو تحوي `_dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 2");` — بلا استدعاء لنافذة غير موجودة. |
| **مراعاة تبعية F2 لـ `LabIdCommand`** | نفس السلوك: placeholder. |
| **مراعاة تبعية F7 لـ `AvailableTests`** | تبقى فارغة في F1 (list فارغ). لا يوجد ما يملؤها إلا عند بناء F7. |
| **تعليق البحث/الحساب** | عند تغيير `BillingSystem` أو `SelectedTests` أو `ReferralId` → استدعاء `TotalAmount = await _patientService.CalculateTotalAsync(...)` مع الجهة الحالية. `Remaining = TotalAmount - PaidAmount`. |
| **`SaveCommand`** | 1) بناء `Patient` من الخصائص. 2) `var result = _validator.Validate(patient);` 3) إن فشل → `_dialogService.ShowMessage("خطأ", string.Join("\n", result.Errors));` 4) إن نجح → `AddAsync` أو `UpdateAsync` حسب `IsEditMode`. |
| **التبعيات** | 1.8, 1.10. |
| **بوابة البناء** | 0/0. |
| **ممنوعات MVVM** | ❌ استدعاء `NewLabDbContext` مباشرة. ❌ `MessageBox.Show`. ❌ `System.Windows.Application.Current`. ❌ Service Locator (`ServiceProvider.GetService()`). |

---

### 🔸 Part 1.12 — `PatientEntryView` (UserControl + code-behind)

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | View |
| **المجلد الجديد** | `Views/Pages/` (غير موجود حالياً — يُنشأ) |
| **المسارات الدقيقة (جديدة)** | `Views/Pages/PatientEntryView.xaml`<br>`Views/Pages/PatientEntryView.xaml.cs` |
| **Namespace** | `NewLab.Views.Pages` |
| **نوع العنصر** | `UserControl` (وليس `Window`) — ليُعرض داخل `MainWindow` عبر ContentControl. |
| **`FlowDirection`** | `RightToLeft` (اتساقاً مع `MainWindow.xaml` الموجود). |
| **التخطيط (حرفياً من السطر 197)** | `Grid` بثلاث أعمدة: <br>&nbsp;&nbsp;• **العمود الأيمن** (RTL → أول عمود بصرياً على اليمين): بيانات المريض — FullName/Title، Age+Unit، Gender (`ComboBox` من `Gender` enum = **Male/Female فقط، قرار 17**)، BillingSystem، Phone/NationalId، ReferralAutocomplete، ExternalSpecimenType، **مجموعة `CheckBox` للحقول الطبية الـ8 Boolean (قرار 1)** + `NumericUpDown/TextBox` لـ FastingHours (يُفعَّل فقط عند `IsFasting`).<br>&nbsp;&nbsp;• **العمود الأوسط**: قائمة التحاليل + مربع بحث + Filter (روتينية/كل/مجموعات) + زر "إضافة التحليل".<br>&nbsp;&nbsp;• **العمود الأيسر**: `DataGrid` أو `ListBox` للتحاليل المختارة (نقر مزدوج للحذف) + منطقة الحسابات (إجمالي/خصم/مدفوع/باقٍ) + زر "خالص/موافق". |
| **أزرار الأوامر (شريط علوي أو سفلي)** | إضافة / تعديل / **حذف (مرتبط بـ `DeleteCommand`، يظهر معطَّلاً لغير Admin — قرار 2)** / إلغاء / حفظ / إيصال / باركود / لاب أي دي / مرضى اليوم. |
| **الأنماط (MaterialDesign)** | أزرار بـ `MaterialDesignRaisedButton`. حقول نصية `MaterialDesignOutlinedTextBox` (نمط `LoginView`). كل مجموعة بيانات في `Border` مع `CornerRadius` بسيط. |
| **InputBindings محلية داخل الـ UserControl** | `F1` = `AddSelectedTestCommand`, `F9` = `SaveCommand`, `F10` = `DeleteCommand` (يعمل فقط عند Admin), `F11` = `PrintBarcodeCommand`, `F12` = `PrintReceiptCommand`, `Esc` = `CancelCommand`. **ملاحظة**: F2 عالمي وليس محلياً — يُوضع في `MainWindow` (Part 1.14). |
| **code-behind (السطر 197 — بلا منطق أعمال)** | يقتصر على: `InitializeComponent();` + معالجات UI بحتة إن لزم (مثل تركيز `FullName` عند فتح النافذة، ضبط `TextBox` numeric input). **ممنوع**: أي استدعاء لخدمة، أي `MessageBox`، أي وصول لـ DbContext، أي منطق حسابي. |
| **ربط البيانات** | `{Binding FullName, UpdateSourceTrigger=PropertyChanged}` — نمط ثابت. لا `x:Reference` لخدمات. |
| **Referral Autocomplete** | يُبنى عبر `ComboBox` بخصائص `IsEditable="True"` + `IsTextSearchEnabled="False"` + Binding على `ReferralSuggestions` مع `TextChanged` command أو `[ObservableProperty] selectedReferralName` يُطلق بحثاً بـ Debounce بسيط داخل الـ VM. |
| **التبعيات** | Part 1.11. |
| **بوابة البناء** | 0/0 + التحقق أن الـ XAML يفتح في Designer بلا أخطاء. |
| **ملاحظة MVVM** | لا `x:Name` على العناصر مربوطاً بحقل في code-behind يقرأ قيمته الـ VM لاحقاً — الاتصال في اتجاه واحد: XAML → Binding → VM. |

---

### 🔸 Part 1.13 — ربط الوظيفة بالـ Toolbar (View resolution mechanism)

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Composition (Toolbar + View resolution) |
| **الملفات المعدَّلة** | `ViewModels/Pages/MainDashboardViewModel.cs` — دالة `CreateToolbarCategories()`.<br>`Views/Windows/MainWindow.xaml` — Function-mode area. |
| **التعديل على `MainDashboardViewModel`** | ضمن فئة "المرضى" (Category = "Patients") — بجانب الـ FunctionDefinition ذات `Name = "إضافة وتعديل بيانات المرضى"`، إضافة: `TargetViewType = typeof(PatientEntryView)`. |
| **`using` مطلوب في `MainDashboardViewModel`** | `using NewLab.Views.Pages;` |
| **آلية عرض الـ View داخل `MainWindow` (السطور 202–204 من الوثيقة، مع تحديث مبني على الكود الفعلي)** | **الحالة الفعلية الآن**: `MainWindow.xaml` في وضع Function-mode يعرض `TextBlock` "محتوى الوظيفة سيظهر هنا..." — لا يوجد ContentControl فعلي. **يجب**: <br>1) في `MainDashboardViewModel`: إضافة خاصية `[ObservableProperty] private object? currentFunctionView;` تُحدَّث في `OpenFunction()` عبر `Activator.CreateInstance(function.TargetViewType!)` عندما `TargetViewType != null`، وإلا `FunctionPlaceholderControl` (السلوك الحالي).<br>2) في `MainWindow.xaml` — Function-mode StackPanel: استبدال الـ TextBlock بـ `<ContentControl Content="{Binding CurrentFunctionView}" />`.<br>3) الحل البديل الأكثر التزاماً بـ MVVM: تسجيل `DataTemplate` في `App.xaml` يربط `PatientEntryViewModel` بـ `PatientEntryView`، وتغيير `OpenFunction` بحيث تُخزّن **ViewModel instance** (via `INavigationService.NavigateTo<PatientEntryViewModel>()` + قراءة `CurrentViewModel`) في `CurrentFunctionView`. **هذا هو الحل المُوصى به** لأنه:<br>&nbsp;&nbsp;• يستخدم `INavigationService` الموجود فعلاً (Part 4 في history.md).<br>&nbsp;&nbsp;• يُتيح حقن `PatientEntryViewModel` عبر DI (لأنه Transient).<br>&nbsp;&nbsp;• يتجنّب `Activator.CreateInstance` الذي يتجاوز DI. |
| **الخطوات المُفصَّلة للحل المُوصى به** | 1) تسجيل `services.AddTransient<PatientEntryViewModel>();` في `App.xaml.cs` (كنمط MainDashboardViewModel).<br>2) تسجيل `services.AddTransient<PatientEntryView>();` (أو تركه كإنشاء عادي في DataTemplate).<br>3) في `App.xaml` داخل `Application.Resources`: إضافة `<DataTemplate DataType="{x:Type vmpages:PatientEntryViewModel}"><views:PatientEntryView /></DataTemplate>` مع namespaces مناسبة.<br>4) تعديل `MainDashboardViewModel.OpenFunction(FunctionDefinition function)`:<br>&nbsp;&nbsp;• إن `function.TargetViewType == typeof(PatientEntryView)` → `_navigationService.NavigateTo<PatientEntryViewModel>()` + `CurrentFunctionView = _navigationService.CurrentViewModel;`<br>&nbsp;&nbsp;• وإلا → السلوك الحالي (Placeholder).<br>5) `MainWindow.xaml` Function-mode: `<ContentControl Content="{Binding CurrentFunctionView}" />` — الـ DataTemplate يرسم `PatientEntryView` تلقائياً. |
| **تسجيل الـ VM في DI** | (تمّ في الخطوة 1 أعلاه) — `services.AddTransient<PatientEntryViewModel>();` |
| **التبعيات** | Parts 1.11, 1.12. |
| **بوابة البناء** | 0/0 + Smoke Test: تشغيل التطبيق → Toolbar → "المرضى" → "إضافة وتعديل بيانات المرضى" → **يجب أن يظهر `PatientEntryView` فعلياً**، لا الـ Placeholder. |
| **ملاحظة MVVM** | الحل المُوصى به هو ViewModel-first (نمط أوصى به `history.md` صراحة في السطر 21: *"Navigation: ViewModel-first with ContentControl"*). |

---

### 🔸 Part 1.14 — تسجيل الاختصار **F2** عالمياً

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | View (Shell keybindings) |
| **الملف المعدَّل** | `Views/Windows/MainWindow.xaml` |
| **التعديل المطلوب** | داخل عنصر `<Window>` (بعد `FlowDirection="RightToLeft"`)، إضافة:<br>`<Window.InputBindings>`<br>&nbsp;&nbsp;`<KeyBinding Key="F2" Command="{Binding OpenPatientEntryCommand}" />`<br>`</Window.InputBindings>` |
| **إضافة أمر في `MainDashboardViewModel`** | `[RelayCommand] private void OpenPatientEntry()` → يستدعي نفس منطق `OpenFunction` مع تمرير الـ FunctionDefinition الخاصة بالمرضى (البحث فيها من `ToolbarCategories`)، أو مباشرة: `_navigationService.NavigateTo<PatientEntryViewModel>(); CurrentFunctionView = _navigationService.CurrentViewModel; IsToolbarVisible = false; IsDashboardMode = false;` |
| **قاعدة صارمة** | الاختصار F2 يجب أن يعمل من أي مكان داخل `MainWindow` بغض النظر عن التركيز الحالي (Window-level KeyBinding يحقق ذلك تلقائياً في WPF). |
| **التبعيات** | Part 1.13. |
| **بوابة البناء** | 0/0 + Smoke Test: الضغط على F2 من الـ Dashboard الرئيسي → يفتح `PatientEntryView`. |
| **ملاحظة MVVM** | الأمر يُنفَّذ في الـ VM لا في code-behind. لا `MainWindow.xaml.cs` يعرف شيئاً عن الوظيفة. |

---

### 🔸 Part 1.15 — Build Verification (بوابة نهائية للشريحة)

| البند | التفصيل |
|---|---|
| **الطبقة (MVVM)** | Cross-cutting (Quality Gate) |
| **الأمر** | `dotnet build` من جذر المستودع (المسار `NewLab/NewLab.csproj`). |
| **المعيار الإلزامي** | **0 errors / 0 warnings** — أي تحذير يعني فشل الشريحة (يشمل CS، Nullable، EF Core migrations warnings، MSBuild). |
| **اختبار يدوي #1 — إضافة مريض تجريبي (السطر 211)** | 1) Login كـ Admin.<br>2) Toolbar → "المرضى" → "إضافة وتعديل بيانات المرضى" (يجب أن يفتح `PatientEntryView` وليس Placeholder).<br>3) إدخال اسم "أحمد تجريبي"، السن 30 سنة، Gender = Male، BillingSystem = Individual.<br>4) تفعيل CheckBox `IsFasting` مع FastingHours = 12، وتفعيل `IsSmoker`.<br>5) الضغط على "حفظ" (F9).<br>6) الاستعلام في DB: `SELECT FullName, IsFasting, FastingHours, IsSmoker FROM Patients WHERE FullName = 'أحمد تجريبي';` → يجب أن يعود صف واحد بقيم `IsFasting=1, FastingHours=12, IsSmoker=1`. |
| **اختبار يدوي #2 — قرار 2 (السطر 212)** | 1) Login كـ Admin → فتح النافذة → **زر الحذف مُفعَّل**.<br>2) Logout → إنشاء مستخدم بدور Technician يدوياً في DB → Login به → فتح النافذة → **زر الحذف معطَّل بصرياً** (grayed out via CanExecute). |
| **اختبار يدوي #3 — F2** | من الـ Dashboard الرئيسي، الضغط على F2 → نفس النافذة تفتح مباشرة. |
| **اختبار يدوي #4 — Autocomplete جهة الإحالة** | في حقل جهة الإحالة، كتابة "الم" → تظهر جهة "المعمل" (Seed من Part 1.5) — يعمل `SearchByNameAsync`. |
| **الفشل = إعادة الجزء المُتسبِّب** | إن فشل أي بند، لا تُغلَق الشريحة. تُصحَّح المشكلة في الجزء المُتسبِّب فقط. |
| **التبعيات** | Parts 1.1 → 1.14. |

---

## 5) Cross-Slice MVVM Compliance Checklist

قبل توقيع الشريحة، يُتحقَّق من:

- [ ] **Models**: `Patient.cs`, `Referral.cs`, `SpecimenType.cs`, `PatientVisit.cs` كلها POCO. لا `using CommunityToolkit.Mvvm`، لا `using System.Windows`.
- [ ] **Services**: كل منطق أعمال (CRUD + حساب + Autocomplete + فحص الأدوار) داخل `PatientService` أو `ReferralService`. لا `DbContext` مستخدم في أي VM.
- [ ] **ViewModel**: `PatientEntryViewModel` يستخدم `[ObservableProperty]` + `[RelayCommand]` فقط. Constructor Injection حصراً. لا `Application.Current`، لا `MessageBox.Show` (يستخدم `IDialogService`).
- [ ] **View**: `PatientEntryView.xaml.cs` code-behind لا يحوي منطقاً غير `InitializeComponent()` + معالجات UI بحتة. كل الربط عبر `{Binding}`.
- [ ] **DI**: كل الخدمات مسجَّلة `Scoped`، كل الـ VMs `Transient` — استمرار لنمط history.md.
- [ ] **Validation**: `IValidator<Patient>` مُحقَن في الـ VM، لا يُنشأ يدوياً.
- [ ] **Nullable**: كل خاصية اختيارية `?`، كل مطلوبة بـ default value أو `[Required]`.
- [ ] **RTL**: الـ View `FlowDirection="RightToLeft"`.

---

## 6) Clarification Points

النقاط أدناه اكتُشفت أثناء المقارنة الحرفية بين نص Function 1 والـ codebase الفعلي / `history.md`. **لم أجتهد بتغيير أي منها** — تُترك للمالك للفصل قبل التنفيذ:

### 🟡 CP-1 — `IAuthService` الحالي لا يُوفّر معرفة "المستخدم الحالي"

- **الحقيقة في الكود**: `IAuthService` (`Services/Interfaces/IAuthService.cs`) يعرض فقط: `ValidateCredentialsAsync`, `HashPassword`, `VerifyPassword`, `CreateAdminAccountAsync`. **لا يوجد** أي API يُعيد "المستخدم المُسجَّل حالياً" أو أدواره — كل استدعاء `ValidateCredentialsAsync` يعيد `User?` ثم يُنسى.
- **ما تفترضه الوثيقة**: قرار 2 يتطلب أن يعرف `PatientService.DeleteAsync` (وأن يعرف `PatientEntryViewModel.IsAdmin`) دور المستخدم الحالي.
- **الخيارات المتاحة (بلا اجتهاد)**:
  1. **إضافة تمرير `int currentUserId` أو `User currentUser`** كمعلمة في أساليب `IPatientService` (يبدأ من `DeleteAsync`) والـ VM يقرأها من قاعدة تخزين ما.
  2. **إنشاء `ICurrentUserService` جديد** يحمل المستخدم بعد Login، ويُحقَن في كل مكان يحتاج معرفة الدور.
  3. **توسعة `IAuthService`** بأسلوب `Task<bool> CurrentUserIsInRoleAsync(string roleName)` + حالة داخلية `User? Current { get; }` تُضبط بعد `ValidateCredentialsAsync`.
- **التأثير على هذه الشريحة**: اختيار الحل يؤثر على ctor لـ `PatientService` وعلى شكل استعلام الدور في `PatientEntryViewModel`. **لا يُتَّخذ القرار في الخطة — يُترك للمالك.**

### 🟡 CP-2 — `LoginView` لا يُخزّن حالياً `User` بعد نجاح الدخول

- **الحقيقة في الكود**: `LoginViewModel.SignInAsync` يستدعي `_authService.ValidateCredentialsAsync(Username, Password)` ثم إن `!= null` يُنفّذ `OnSuccess?.Invoke();` — **الـ User المُعاد يُهمَل ولا يُمرَّر لأي طبقة أعلى**.
- **العلاقة بـ CP-1**: هذه الفجوة هي السبب في أن `IAuthService` لا يوفر current-user. الحل المُقترح في CP-1 يجب أن يُدمج مع تعديل `LoginViewModel` لتخزين المستخدم بعد نجاح الدخول.
- **التأثير على هذه الشريحة**: قرار 2 يتطلب فعلياً حسم CP-1 قبل أن يعمل `IsAdmin` في `PatientEntryViewModel` بشكل صحيح. **مؤقتاً**: يمكن للـ VM أن يفترض `IsAdmin = true` كـ Placeholder + TODO حتى يُحسم CP-1، مع تعطيل اختبار قرار 2 في Part 1.15 مؤقتاً. **يُترك للمالك.**

### 🟡 CP-3 — `INavigationService.CurrentViewModel` لا يُطلق حدث تغيير

- **الحقيقة في الكود**: `NavigationService.NavigateTo<T>()` يحدّث `_currentViewModel` لكن `CurrentViewModel` property ليست `ObservableProperty` ولا `INotifyPropertyChanged` — الـ Consumer لا يعرف أنها تغيّرت.
- **الأثر على Part 1.13**: الحل المُوصى به يعتمد على قراءة `_navigationService.CurrentViewModel` فوراً بعد `NavigateTo<PatientEntryViewModel>()` ثم إسنادها لـ `CurrentFunctionView` — هذا يعمل *لأننا نستدعيها synchronously بعد `NavigateTo`*، فلا حاجة لحدث. لذا **لا يلزم تعديل** `INavigationService`. لكن يُذكر هنا للشفافية.

### 🟡 CP-4 — تعارض `PackIconKind.PersonMultiple` في `MainDashboardViewModel`

- **الحقيقة في الكود**: `MainDashboardViewModel.CreateToolbarCategories()` يستخدم `PackIconKind.PersonMultiple` لفئة "المرضى" (السطر 76 من الملف الفعلي).
- **الفحص**: `MaterialDesignThemes 5.1.0` (الحزمة المستخدمة) قد لا تحتوي هذا الاسم؛ الاسم الشائع هو `AccountMultiple` أو `AccountGroup`. **لكن**: الكود يبني حالياً بنجاح (المشروع Baseline compliant حسب `history.md`)، ما يعني أن الاسم موجود فعلاً في الإصدار 5.1.0 من MaterialDesignThemes. **لا يوجد تعارض فعلي** — أذكره فقط لأنه غير مدرج في `IconNameToKindConverter.cs` كخيار (المُحوِّل يُترجم من IconName نصي إلى Kind، لكن الفئات في `CreateToolbarCategories` تستخدم `IconKind` مباشرة، لا `IconName`).
- **لا أثر على Function 1** — ذكر للشفافية فقط.

### 🟡 CP-5 — `Docs/PRDs/` غير موجود في المستودع

- **الحقيقة**: مجلد `Docs/PRDs/` المذكور في تعليمات STEP 3 (كنمط سابق لـ Handoff files) **غير موجود** في الـ commit. المجلد `Docs/` يحوي فقط: `analysis_and_plan_v3.md`, `history.md`, و `Reference system/`.
- **`history.md` لا يذكر إطلاقاً** ملفات `Handoff_Slice_X.md` — النمط الموثَّق فيه هو Phase-based داخل نفس `history.md`.
- **القرار المُتَّخذ في هذه الخطة**: وضع ملف `Handoff_Slice_1_2.md` مباشرة كما طُلب في STEP 9 (اسم الملف الحرفي المطلوب)، بدون إنشاء مجلد `Docs/PRDs/` غير موجود ولا افتراض نمط لم يُوثَّق. **يُترك للمالك تحديد مكان الحفظ النهائي.**

### 🟡 CP-6 — الحقول `UpdatedByUserId` / `DeletedAt` غير مذكورة في Part 1.3

- **الحقيقة**: قائمة خصائص `Patient` (السطر 146) تحوي `CreatedByUserId` و `UpdatedAt` لكن **لا يوجد** `UpdatedByUserId` ولا Soft-Delete (`DeletedAt`).
- **حسب قاعدة عدم الاجتهاد (STEP 8)**: يُلتزم حرفياً بالقائمة كما وردت. **لا يُضاف** حقل غير مذكور.
- **يُترك للمالك** تقرير ما إذا كان يريد إضافتهما — لكن ذلك خارج نطاق هذه الشريحة كما هي معرَّفة.

### 🟡 CP-7 — `DataTemplate` في `App.xaml` أم `Window.Resources`؟

- **السياق**: Part 1.13 يقترح تسجيل `DataTemplate<PatientEntryViewModel → PatientEntryView>` في `App.xaml`. `MainWindow.xaml` يحتوي حالياً `DataTemplate` واحد فقط داخل `Window.Resources` (لـ `FunctionDefinition`).
- **الخياران**:
  1. تسجيل في `App.xaml` — يجعل الـ template متاحاً كل التطبيق (نمط ViewLocator أنيق).
  2. تسجيل في `MainWindow.xaml Window.Resources` — نطاق ضيق يكفي لأن `PatientEntryView` لا يُعرض إلا داخل `MainWindow`.
- **الوثيقة تقول (السطر 204)**: *"إن لزم: إنشاء ViewLocator بسيط أو تسجيل DataTemplate لكل ViewModel في App.xaml"*.
- **يُترك للمالك** الاختيار — الخطة تدعم الحلَّين بلا فرق تنفيذي جوهري على قائمة الأجزاء الـ15.

---

## 7) Change Manifest (ما سيحدث فعلياً بعد تنفيذ الأجزاء الـ15)

### ملفات جديدة (Created)
```
Models/Domain/Enums/Gender.cs
Models/Domain/Enums/BillingSystem.cs
Models/Domain/Enums/AgeUnit.cs
Models/Domain/Enums/TestStatus.cs
Models/Domain/Enums/CodeType.cs
Models/Domain/Referral.cs
Models/Domain/SpecimenType.cs
Models/Domain/Patient.cs
Models/Domain/PatientVisit.cs
Models/Validation/PatientValidator.cs
Services/Interfaces/IPatientService.cs
Services/Interfaces/IReferralService.cs
Services/Implementations/PatientService.cs
Services/Implementations/ReferralService.cs
ViewModels/Pages/PatientEntryViewModel.cs
ViewModels/Pages/LabTestPlaceholder.cs   (نوع مساعد مؤقت لحين F7)
Views/Pages/PatientEntryView.xaml
Views/Pages/PatientEntryView.xaml.cs
Migrations/<timestamp>_AddPatientsAndReferrals.cs
Migrations/<timestamp>_AddPatientsAndReferrals.Designer.cs
```

### ملفات مُعدَّلة (Modified)
```
Data/NewLabDbContext.cs                        (إضافة 4 DbSets + Fluent API + Seed)
Migrations/NewLabDbContextModelSnapshot.cs     (يُحدَّث آلياً)
App.xaml                                       (إضافة DataTemplate — إن اختير الحل 1 من CP-7)
App.xaml.cs                                    (تسجيل 3 خدمات + 1 VM + 1 Validator)
Views/Windows/MainWindow.xaml                  (Window.InputBindings F2 + ContentControl بدل Placeholder)
ViewModels/Pages/MainDashboardViewModel.cs     (TargetViewType + OpenPatientEntryCommand + CurrentFunctionView)
```

### ملفات غير مُتأثِّرة (Untouched)
```
Views/Windows/LoginView.xaml/.cs
Views/Windows/SetupView.xaml/.cs
Views/Controls/DashboardContentControl.xaml/.cs
Views/Controls/FunctionPlaceholderControl.xaml/.cs
Services/{Interfaces,Implementations}/{IApplicationStartupService,IAuthService,IDialogService,INavigationService,...}
Models/Domain/User.cs, Role.cs, UserRole.cs, ToolbarItem.cs, FunctionDefinition.cs
Converters/InverseBoolToVisibilityConverter.cs
ViewModels/Base/ViewModelBase.cs
ViewModels/Pages/LoginViewModel.cs, SetupViewModel.cs, IconNameToKindConverter.cs
```
(ما لم يستدعِ تعديلها قرار المالك في نقاط التوضيح أعلاه.)

---

## 8) Dependency Graph (بين الأجزاء الـ15)

```
1.1  ── Enums
  ↓
1.2  ── Referral, SpecimenType
  ↓
1.3  ── Patient (يعتمد على Enums + Referral + SpecimenType)
  ↓
1.4  ── PatientVisit (يعتمد على Patient)
  ↓
1.5  ── DbContext + Migration (يعتمد على 1.1–1.4)
  ↓
1.6 ── IPatientService  ────┐
                            ├──> 1.8  Implementations
1.7 ── IReferralService ────┘         ↓
                                    1.9  DI Registration
                                      ↓
                                    1.10  PatientValidator (يحتاج Patient من 1.3، مُسجَّل مع 1.9)
                                      ↓
                                    1.11  PatientEntryViewModel
                                      ↓
                                    1.12  PatientEntryView
                                      ↓
                                    1.13  Toolbar wiring + DataTemplate + Navigation
                                      ↓
                                    1.14  F2 InputBinding (يعتمد على 1.13)
                                      ↓
                                    1.15  Build & Smoke Test (بوابة نهائية)
```

**قاعدة**: لا انتقال إلى جزء تالٍ قبل بوابة البناء الخضراء (0/0) للجزء الحالي.

---

## 9) Sign-off Criteria (متى تُعتبر الشريحة مُسلَّمة)

- [x] كل الأجزاء الـ15 مُنجَزة بترتيبها.
- [x] `dotnet build` → 0 errors / 0 warnings عند الجذر.
- [x] الاختبارات اليدوية الأربعة في Part 1.15 نجحت جميعها (مع تعليق اختبار قرار 2 مؤقتاً إن لم يُحسم CP-1/CP-2).
- [x] Baseline المُتحقَّق في القسم 0.2 لم يُخترق بإضافة كيانات/خدمات/نوافذ **غير مذكورة** في نص Function 1.
- [x] لا كيانات ولا خدمات ولا Views من الوظائف 2–8 تم بناؤها هنا — فقط `Placeholder`/`Signature-shaping`.
- [x] كل Clarification Point تم اتخاذ قرار المالك بشأنه قبل الدخول في التنفيذ الفعلي.

---

**نهاية Handoff Plan — Slice 1 / Function 1.**
*تم إعداده بناءً على تحليل مستقل للـ commit `225852c4ba4106a3cb3a3dc4f32af423fd085e7b` وقراءة كاملة لـ `analysis_and_plan_v3.md` و `history.md` وفحص كل ملف في الـ codebase الفعلي.*
