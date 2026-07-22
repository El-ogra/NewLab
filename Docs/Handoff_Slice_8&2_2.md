# 🎯 Handoff Plan — Vertical Slices Function 8 + Function 2 (Combined)

## NewLab Laboratory Management System

---

## 📌 Meta & Scope

| البند | القيمة |
|---|---|
| **الوثيقة المصدر — التخطيط** | `Docs/analysis_and_plan_v3.md` (نطاق: قسم Function 8 حرفياً — 10 أجزاء / قسم Function 2 حرفياً — 13 جزءاً / Baseline / صفوف جدول القرارات 3-4-5-16-17) |
| **الوثيقة المصدر — التاريخ** | `Docs/history.md` (v1.4 — تحديث ما بعد F7) |
| **Commit hash المرجعي** | `40e1b0ff889d896dee710d04b7771785b1081a2c` (branch `main`) |
| **رسالة الـcommit المُتحقَّق منها** | "بعد تنفيذ الوظيفة السابعة وإزالة ملف التخطيط الخاص بها" ✅ (استُنسخ وتم checkout والتحقق مباشرة) |
| **الوظيفتان في نطاق الشريحة** | F8 — إضافة/تعديل المعدلات الطبيعية (Execution Order: Step 3 of 8) → **يُنفَّذ أولاً**؛ ثم F2 — طباعة باركود للمريض (Execution Order: Step 4 of 8) |
| **الوظائف المُكتمِلة قبل هذه الشريحة** | F1 (15/15 parts — Phase 5) + F7 (11/11 parts — Phase 6). البنية التحتية جاهزة: `ICurrentUserService` (Singleton) + `IDialogService` + `INavigationService` (Singleton) + `NewLabDbContext` مع 11 جدولاً + Migrations متسلسلة (`20260721171559_InitialCreate` → `20260722032039_AddPatientsAndReferrals` → `20260722063244_AddLabTestsAndReferralPrices`). |
| **القرارات المؤثرة على هذه الشريحة** | **F8**: قرار 14 (لوحة الرموز) — قرار 16 (أضيق مدى يفوز) — قرار 17 (Gender = Male/Female فقط). **F2**: قرار 3 (ZXing.Net حصراً) — قرار 4 (LabelWidth 38 / LabelHeight 25 مم قابل للتخصيص) — قرار 5 (لا BranchNumber؛ الفرع ثابت برمجياً "1"). |
| **بنية التنفيذ** | F8 كامل أولاً (Parts 8.1 → 8.10) ثم F2 كامل (Parts 2.1 → 2.13) — بنفس عدد الأجزاء وترتيبها وحدودها كما وردت حرفياً في `analysis_and_plan_v3.md`. |
| **بوابة البناء بعد كل جزء** | `dotnet clean && dotnet build` → **0 errors / 0 warnings** إلزامياً. البناء التدرّجي (Incremental) وحده لا يكفي — القسم 5 من Technical Notes في `history.md` يوثِّق أن الـIncremental Build أخفى تحذير MC3074 حتى تم Clean+Rebuild. |
| **قاعدة عدم الاجتهاد** | كل اسم حقل/كيان/خاصية يتبع نص التخطيط حرفياً؛ أي تعارض تقني حقيقي يُرحَّل إلى Clarification Points بدل تعديله. |

**اصطلاحات history.md المُعتَمدة في هذه الشريحة**:

- **CP (Clarification Point)** — سؤال تصميمي مُغلَق بقرار موثَّق (مثال: CP-1/CP-2/CP-3 في Phase 6).
- **Retro-Integration** — قسم يوثِّق التغييرات الرجعية على وظائف مكتملة سابقاً (نمط Phase 6 على F1).
- **Files Created / Files Modified / Files Deleted** — تقسيم Manifest الاعتيادي لكل Phase.
- **Build Status: 0 errors, 0 warnings (verified after each part)** — الصيغة الحرفية لبوابة البناء.

---

## 🧭 MVVM Layer Map — Cross-Slice

الشريحة كاملة تلتزم بنفس فصل الطبقات المُعتمَد منذ Phase 4:

```
┌────────────────────────────────────────────────────────────────────────────┐
│  Views (WPF)                                                                │
│  ─────────────────────────────                                              │
│  • NormalRangeView (Window)                          ← F8, Part 8.7         │
│  • BarcodeView (Window)                              ← F2, Part 2.10        │
│  • LatinSymbolsPad (UserControl)   [reused من F7 — قرار 14]                 │
│  • LabTestManagementView (UserControl)  [مُعدَّل رجعياً — F8]                │
│  • PatientEntryView (UserControl)       [مُعدَّل رجعياً — F2]                │
└────────────────────────────────────────────────────────────────────────────┘
                              ▲   binding only (لا code-behind logic)
┌────────────────────────────────────────────────────────────────────────────┐
│  ViewModels (CommunityToolkit.Mvvm)                                         │
│  ─────────────────────────────                                              │
│  • NormalRangeViewModel        ← F8, Part 8.6                               │
│  • BarcodeViewModel            ← F2, Part 2.9                               │
│  • LabTestManagementViewModel  [مُعدَّل رجعياً — F8 Part 8.8]                │
│  • PatientEntryViewModel       [مُعدَّل رجعياً — F2 Part 2.12]               │
└────────────────────────────────────────────────────────────────────────────┘
                              ▲   Constructor Injection فقط
┌────────────────────────────────────────────────────────────────────────────┐
│  Services + Validators                                                      │
│  ─────────────────────────────                                              │
│  • INormalRangeService / NormalRangeService     ← F8 Parts 8.3, 8.5         │
│  • NormalRangeValidator (FluentValidation)      ← F8 Part 8.4               │
│  • IBarcodeService / BarcodeService             ← F2 Parts 2.4, 2.5         │
│  • BarcodeImageGenerator (Helpers)              ← F2 Part 2.7               │
│  • BarcodePrintService                          ← F2 Part 2.8 (QuestPDF)    │
│  • ICurrentUserService [موجود]  — يُقرأ للـAdmin checks (قرار 2 امتداداً)     │
│  • IDialogService [موجود]       — لكل رسائل التأكيد/الأخطاء                  │
│  • INavigationService [موجود]   — لفتح Modal Windows                        │
│  • IPatientService [موجود]      — يستهلكه BarcodeService                    │
│  • ILabTestService [موجود]      — يستهلكه NormalRangeViewModel              │
└────────────────────────────────────────────────────────────────────────────┘
                              ▲   Scoped / Singleton كما يُملي عمر DbContext
┌────────────────────────────────────────────────────────────────────────────┐
│  Data (EF Core 8.0.8)                                                       │
│  ─────────────────────────────                                              │
│  • NewLabDbContext  [مُعدَّل: +2 DbSets في F8، +2 DbSets في F2]              │
│    – DbSet<NormalRange>                                                     │
│    – DbSet<BarcodeSettings>                                                 │
│    – DbSet<PatientCode>            [اختياري في Part 2.2]                    │
│  • Migrations:                                                              │
│    – 20260722XXXXXX_AddNormalRanges                    (F8, Part 8.2)       │
│    – 20260722YYYYYY_AddBarcodeSettingsAndPatientCodes  (F2, Part 2.3)       │
└────────────────────────────────────────────────────────────────────────────┘
                              ▲   Domain Models
┌────────────────────────────────────────────────────────────────────────────┐
│  Models/Domain                                                              │
│  ─────────────────────────────                                              │
│  • NormalRange.cs        ← F8, Part 8.1                                     │
│  • BarcodeSettings.cs    ← F2, Part 2.1                                     │
│  • BarcodeLabel.cs       ← F2, Part 2.1 (Transient — غير مخزَّن في DB)       │
│  • PatientCode.cs        ← F2, Part 2.2 (اختياري — يعتمد على قرار CP-F2-2)  │
└────────────────────────────────────────────────────────────────────────────┘
```

**قواعد MVVM المُلزَمة لكل جزء**:

1. **لا `DbContext` في أي ViewModel** — يذهب فقط عبر Services.
2. **Constructor Injection حصراً** — لا `ServiceProvider.GetRequiredService<>()` داخل ViewModel أو View.
3. **ICurrentUserService.IsAdmin** — المصدر الوحيد لقرار الصلاحية (`CanExecute`) — نفس نمط `LabTestService.DeleteAsync` و`PatientService.DeleteAsync`.
4. **Code-behind في Views محصور** بـ `InitializeComponent()` وربط `ItemsSource` للـ ComboBoxes التي فيها enum values (لأن `x:Array` مع enum يفشل التصنيف — Technical Note 4 في `history.md`).
5. **Async Void ممنوع** — استخدام `[RelayCommand] private async Task XxxAsync()` أو نمط `_ = LoadAsync()` fire-and-forget كما في CP-2 (Phase 6).

---

## 🔁 Retro-Integration Section

### Retro-Integration with Function 7 (مُلزَم لـ F8)

**التحقق المباشر من الكود (commit `40e1b0f`)**:

- الملف: `ViewModels/Pages/LabTestManagementViewModel.cs`, أسطر 207-211:
  ```csharp
  [RelayCommand]
  private void OpenNormalRange()
  {
      _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 8");
  }
  ```
- الملف: `Views/Pages/LabTestManagementView.xaml`, سطر 271-274:
  ```xml
  <Button Content="المعدل الطبيعي"
          Command="{Binding OpenNormalRangeCommand}"
          Style="{StaticResource MaterialDesignOutlinedButton}"
          Margin="4" Padding="16,8"/>
  ```

**استنتاج مُثبَت**: يوجد **placeholder فعلي واحد** بانتظار F8 داخل `LabTestManagementView` (زر "المعدل الطبيعي" مرتبط بالأمر `OpenNormalRangeCommand`). الآلية الحالية: `IDialogService.ShowMessage` تعرض رسالة "ستُفعَّل هذه الوظيفة في Function 8". هذا يتطابق حرفياً مع ما نصت عليه `analysis_and_plan_v3.md` (Function 8 → Part 8.8 → دمج مع Function 7).

**الآلية الدقيقة لتفعيل الـplaceholder — تُنفَّذ حصراً في Part 8.8**:

| الخطوة | المكان في الكود | التغيير المطلوب |
|---|---|---|
| 1 | `ViewModels/Pages/LabTestManagementViewModel.cs` — تعديل الحقول الخاصة | إضافة `private readonly Func<NormalRangeViewModel> _normalRangeVmFactory;` (يُحقَن عبر DI باستخدام `Func<T>` من `Microsoft.Extensions.DependencyInjection`، يُسجَّل تلقائياً حيث `NormalRangeViewModel` مسجَّل كـ Transient). |
| 2 | `ViewModels/Pages/LabTestManagementViewModel.cs` — constructor | إضافة معامل `Func<NormalRangeViewModel> normalRangeVmFactory` في نهاية القائمة (لا تُكسَر توقيعات F1). |
| 3 | `ViewModels/Pages/LabTestManagementViewModel.cs` — استبدال جسم `OpenNormalRange` | حذف السطر `_dialogService.ShowMessage("Info", ...)` واستبداله بـ: (أ) فحص `if (SelectedTest == null) { _dialogService.ShowMessage("خطأ", "اختر تحليلاً أولاً"); return; }` (ب) إنشاء VM: `var vm = _normalRangeVmFactory(); vm.LoadForTest(SelectedTest);` (ج) فتح `NormalRangeView` كـ Dialog: `var window = new NormalRangeView { DataContext = vm, Owner = Application.Current.MainWindow }; window.ShowDialog();`. |
| 4 | إضافة `[NotifyCanExecuteChangedFor(nameof(OpenNormalRangeCommand))]` على `SelectedTest` (اختياري — لتعطيل الزر عند عدم اختيار تحليل) وربط `CanExecute` بشرط `SelectedTest != null`. |
| 5 | `Views/Pages/LabTestManagementView.xaml` — الزر يبقى كما هو (لا تعديل XAML مطلوب — الأمر `OpenNormalRangeCommand` يبقى نفس الاسم). |
| 6 | `App.xaml.cs` — تسجيل DI | لا تعديل مطلوب لتوقيع `LabTestManagementViewModel` (المضاف `Func<NormalRangeViewModel>` يُحلّه الـcontainer تلقائياً طالما `NormalRangeViewModel` مسجَّل). |

**الجزء المسؤول عن هذا التكامل العكسي**: **Part 8.8** حصراً — لا تكامل عكسي مُفتَّت بين أجزاء F8.

---

### Retro-Integration with Function 1 (مُلزَم لـ F2)

**التحقق المباشر من الكود (commit `40e1b0f`)**:

- الملف: `ViewModels/Pages/PatientEntryViewModel.cs`, أسطر 187-191:
  ```csharp
  [RelayCommand]
  private void PrintBarcode()
  {
      _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 2");
  }
  ```
- نفس الملف، أسطر 226-230:
  ```csharp
  [RelayCommand]
  private void LookupLabId()
  {
      _dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 2");
  }
  ```
- الملف: `Views/Pages/PatientEntryView.xaml`, سطر 8-14 (`Window.InputBindings`) — يحتوي فعلياً:
  ```xml
  <KeyBinding Key="F11" Command="{Binding PrintBarcodeCommand}" />
  ```
- نفس الملف، سطر 227-230 و231-234 — زرَّا "باركود" و"لاب اي دي" مرتبطان بالأمرين أعلاه.

**استنتاج مُثبَت**: يوجد **placeholder فعلي مزدوج** بانتظار F2 داخل `PatientEntryView`:
1. زر "باركود" + KeyBinding `F11` → `PrintBarcodeCommand`.
2. زر "لاب اي دي" → `LookupLabIdCommand`.

كلاهما يعرض حالياً رسالة "ستُفعَّل هذه الوظيفة في Function 2" بلا سلوك حقيقي. هذا يتطابق حرفياً مع ما نصت عليه `analysis_and_plan_v3.md` (Function 2 → Part 2.12 → استدعاء النافذة من `PatientEntryView`).

**الآلية الدقيقة لتفعيل الـplaceholder — تُنفَّذ حصراً في Part 2.12**:

| الخطوة | المكان في الكود | التغيير المطلوب |
|---|---|---|
| 1 | `ViewModels/Pages/PatientEntryViewModel.cs` — الحقول الخاصة | إضافة `private readonly Func<BarcodeViewModel> _barcodeVmFactory;` + `private readonly IBarcodeService _barcodeService;`. |
| 2 | `ViewModels/Pages/PatientEntryViewModel.cs` — constructor | إضافة معاملَي `IBarcodeService barcodeService, Func<BarcodeViewModel> barcodeVmFactory` في نهاية القائمة. |
| 3 | `ViewModels/Pages/PatientEntryViewModel.cs` — استبدال جسم `PrintBarcode` | حذف `_dialogService.ShowMessage(...)` واستبداله بـ: (أ) `if (_editingPatientId == null) { _dialogService.ShowMessage("تنبيه", "احفظ بيانات المريض أولاً"); return; }` (ب) `var patient = await _patientService.GetByIdAsync(_editingPatientId.Value); if (patient == null) return;` (ج) `var vm = _barcodeVmFactory(); await vm.LoadForPatientAsync(patient);` (د) `var window = new BarcodeView { DataContext = vm, Owner = Application.Current.MainWindow }; window.ShowDialog();`. **ملاحظة**: تحويل الميثود لـ `async Task PrintBarcodeAsync` (نمط `[RelayCommand] private async Task ...`) — تناسق مع باقي الميثودات async في نفس الـVM. |
| 4 | `ViewModels/Pages/PatientEntryViewModel.cs` — استبدال جسم `LookupLabId` | حذف `_dialogService.ShowMessage(...)` واستبداله بـ: `if (_editingPatientId == null) { _dialogService.ShowMessage("تنبيه", "احفظ بيانات المريض أولاً"); return; }` ثم `var patient = await _patientService.GetByIdAsync(_editingPatientId.Value); if (patient == null) return; if (string.IsNullOrEmpty(patient.LabId)) { patient = await _barcodeService.GetOrCreateLabIdAsync(patient); LabId = patient.LabId; }`. **الميثود يتحول لـ** `async Task LookupLabIdAsync` — تناسق مع نمط CommunityToolkit.Mvvm. **ملاحظة تسمية حرِجة**: القسم 2 من Technical Notes في `history.md` يوثِّق تعارض `[ObservableProperty] labId` مع اسم الميثود `LabId`؛ لذلك تم تسميته `LookupLabId` مسبقاً — ومعالجته كـ `LookupLabIdAsync` تبقى ضمن نفس البادئة ولا تعيد فتح التعارض. |
| 5 | `Views/Pages/PatientEntryView.xaml` — الأزرار وInputBindings تبقى كما هي (`PrintBarcodeCommand`, `LookupLabIdCommand` — نفس الأسماء التي يُولِّدها CommunityToolkit من `PrintBarcodeAsync`/`LookupLabIdAsync`). |
| 6 | `App.xaml.cs` — لا تعديل مطلوب لـ`PatientEntryViewModel` بعد إضافة تسجيلات F2 (`IBarcodeService` Scoped + `BarcodeViewModel` Transient) — الـcontainer يحلّ Constructor Injection تلقائياً. |
| 7 | InputBinding `F11` في `PatientEntryView.xaml` **يبقى كما هو** — يستدعي `PrintBarcodeCommand` الفعلي الآن. |

**الجزء المسؤول عن هذا التكامل العكسي**: **Part 2.12** حصراً — لا تكامل عكسي مُفتَّت بين أجزاء F2.

---

## 🔮 Future-Impact Awareness (نحو F3–F6)

الشريحة الحالية تُقرَّر تصميمياً بشكل يُراعي متطلبات F3–F6 دون بناء أي تفاصيل داخلية لها. الأثر التصميمي المُراعى الآن:

| الوظيفة اللاحقة | الأثر التصميمي المُراعى الآن | الجزء المُنَظِّم |
|---|---|---|
| **F3 (نتائج التحاليل)** | `INormalRangeService.EvaluateValueAsync(range, value)` يعيد enum/DTO يوصِّف الحالة (Normal / Abnormal Low / Abnormal High / Critical Low / Critical High) — واجهة مستقرة تُستهلَك لاحقاً في `TestResultsListViewModel` لعرض رمز الحالة (7 رموز حسب Baseline). لا نُنشئ enum `EvaluationOutcome` نهائياً إلا داخل نطاق F8 — F3/F4 يُوسِّعانه إن لزم دون تعديل توقيع الميثود. | Part 8.3 |
| **F3 (بحث بالكود)** | `IBarcodeService` يُنشئ الأكواد الثلاثة (Case/File/Lab) بصيغة 13 خانة موحَّدة — يُتوقع أن `TestResultsListService.SearchByCodeAsync(code)` في F3 (Part 3.4) سيحلِّل بادئة الكود (Position 2 = CodeType) دون الحاجة لتعديل `IBarcodeService`. مُطابق لقرار 10 (تصميم مرن يقبل أي كود ممسوح). | Part 2.4 |
| **F4 (إدخال النتائج)** | `NormalRangeService.GetMatchingRangeAsync(labTestId, patient)` يُعرَّف بـsignature كامل (labTestId + Patient) — F4 (Part 4.5 / AutoCalculationService) سيستدعيه مباشرة لتلوين النتائج. **قرار 16 مُطبَّق داخل الخدمة نفسها** — F4 لا يُعيد تنفيذ قاعدة الأولوية. | Part 8.3, Part 8.9 |
| **F4 (نمط MVVM للنتائج)** | استخدام `Func<NormalRangeViewModel>` factory pattern عبر DI في Part 8.8 يُثبِت النمط الذي ستستخدمه F4 لفتح نافذة إدخال النتائج (Modal) من قائمة التحاليل — تجنُّب تسجيل `ViewLocator` عام قبل حاجة فعلية إليه. | Part 8.8 |
| **F5 (تسليم النتائج)** | `IBarcodeService.GenerateFileCode(patient)` — F5 سيولِّد نفس الصيغة على ملصق التسليم. `BarcodeImageGenerator.GenerateCode128(string data)` مصمَّم كـHelper مستقل عن السياق (لا يحتوي منطق مريض)، ما يسمح لـF5 بإعادة استخدامه كما هي. | Parts 2.4, 2.7 |
| **F5 (Barcode Scan)** | صيغة 13 خانة بموقع ثابت لـCodeType (خانة 2) وتاريخ اليوم (خانات 3-8) تجعل تحليل الكود الممسوح في F5 ممكناً بدون قاعدة بيانات — مطابقة لقرار 10 (تصميم مرن). | Part 2.4 |
| **F6 (البحث عن مريض)** | `PatientCode` كيان اختياري (Part 2.2) — إن اعتُمِد، F6 (Part 6.1) يمكنه البحث المباشر عبر `PatientCodes` بدلاً من مسح `Patient.LabId`/`FileCode`/`VisitCode` منفصلة. القرار النهائي حول اعتماد `PatientCode` مُرَحَّل كـ CP-F2-2 (انظر Clarification Points). | Part 2.2 |
| **F3–F6 عموماً (Admin gates)** | نمط `ICurrentUserService.IsAdmin` المُتَّبع في `NormalRangeService.DeleteAsync` (Part 8.5) يُثبِّت النمط الذي ستستخدمه F3 (زرَّا "ب" و"ت" — قرار 7)، F5 (زر "مستلمة" — قرار 11)، F6 (حذف مريض — قرار 13). لا تكرار للمنطق. | Parts 8.5 |
| **F3–F6 عموماً (لوحة الرموز)** | إعادة استخدام `LatinSymbolsPad` UserControl في Part 8.7 (F8) دون تعديل — يُثبِت أنه صالح كأداة مشتركة لأي حقل نصي في F3/F4 (تعليقات، إدخال قيم). | Part 8.7 |

**قاعدة صارمة**: أي حقل أو خدمة أو Enum يُقال "لاحقاً في F3" لا يُنشأ ولا يُطوَّر داخل هذه الشريحة — حتى لو بدا التصميم "ناقصاً". النقصان مقصود.

---

# ═══════════════════════════════════════════════════════════════

# 🔹 Function 8 — إضافة/تعديل المعدلات الطبيعية لتحليل معين

# ═══════════════════════════════════════════════════════════════

**Execution Order**: Step 3 of 8 (يُنفَّذ أولاً في هذه الشريحة قبل F2).
**عدد الأجزاء**: 10 (Parts 8.1 → 8.10) — مطابق حرفياً لعدد الأجزاء في `analysis_and_plan_v3.md`.

---

## Part 8.1 — كيان NormalRange (Gender = Male/Female فقط — قرار 17)

**الطبقة**: Model (Domain).

**الملفات المُنشَأة**:
- `Models/Domain/NormalRange.cs`

**التفاصيل**:
- الحقول (بالترتيب الحرفي من `analysis_and_plan_v3.md` Part 8.1):
  ```
  Id (int), LabTestId (int, FK), TestName (string 200),
  TestUnit (string 50), Gender (Gender enum — Male/Female حصراً — قرار 17),
  AgeFrom (int), AgeTo (int), AgeUnit (AgeUnit enum — Day/Month/Year — موجود من F1),
  NormalRangeText (string 200), LowLimit (decimal), HighLimit (decimal),
  LowFlag (string 50), HighFlag (string 50),
  LowComment (string 500), HighComment (string 500), CriticalComment (string 500),
  CriticalRangeText (string 200), CriticalLowLimit (decimal?), CriticalHighLimit (decimal?),
  CriticalFlag (string 50)
  ```
- **Navigation property**: `public LabTest LabTest { get; set; } = null!;` — يشير للكيان `LabTest` الموجود من F7.
- **الحقول decimal** تُحدَّد بـ `[Column(TypeName = "decimal(18,4)")]` (دقة 4 لأنها قد تحتوي قيم مثل 0.0125) — نمط مماثل للـPrices في F7 ولكن بدقة أعلى للقيم الطبية.
- **لا حقل `Both`** في `Gender` — التحقق يتم في Validator + مستوى قاعدة البيانات (`IsInEnum`).
- **الـnamespace**: `NewLab.Models.Domain`.

**الاعتماد**: يعتمد على `LabTest` (F7 مكتمل) وعلى enum `Gender` (F1 — موجود) وعلى enum `AgeUnit` (F1 — موجود).

**بوابة البناء**: `dotnet clean && dotnet build` → 0 errors / 0 warnings.

---

## Part 8.2 — Migration

**الطبقة**: Data (EF Core).

**الملفات المعدَّلة**:
- `Data/NewLabDbContext.cs` → إضافة `public DbSet<NormalRange> NormalRanges { get; set; }` + بلوك Fluent API.

**الملفات المُنشَأة**:
- `Migrations/<timestamp>_AddNormalRanges.cs` (بواسطة `dotnet ef migrations add AddNormalRanges -c NewLabDbContext`).
- `Migrations/<timestamp>_AddNormalRanges.Designer.cs`.
- تحديث `Migrations/NewLabDbContextModelSnapshot.cs` (تلقائي).

**تفاصيل Fluent API في `OnModelCreating`**:
```csharp
modelBuilder.Entity<NormalRange>()
    .HasOne(nr => nr.LabTest)
    .WithMany()                       // لا نضيف Collection في LabTest لتجنب تضخيم كيان F7
    .HasForeignKey(nr => nr.LabTestId)
    .OnDelete(DeleteBehavior.Cascade); // حذف تحليل يحذف معدلاته

modelBuilder.Entity<NormalRange>()
    .HasIndex(nr => new { nr.LabTestId, nr.Gender, nr.AgeFrom, nr.AgeTo, nr.AgeUnit });

modelBuilder.Entity<NormalRange>().Property(nr => nr.LowLimit).HasColumnType("decimal(18,4)");
modelBuilder.Entity<NormalRange>().Property(nr => nr.HighLimit).HasColumnType("decimal(18,4)");
modelBuilder.Entity<NormalRange>().Property(nr => nr.CriticalLowLimit).HasColumnType("decimal(18,4)");
modelBuilder.Entity<NormalRange>().Property(nr => nr.CriticalHighLimit).HasColumnType("decimal(18,4)");
```

**التحقق**: بعد `dotnet ef database update -c NewLabDbContext` → جدول `NormalRanges` موجود، `ProductVersion = 8.0.8`، `__EFMigrationsHistory` يحتوي 4 صفوف.

**بوابة البناء**: 0 errors / 0 warnings.

**ملاحظة**: `dotnet-ef` الأداة العامة قد تكون 10.0.6 — لا مشكلة (Technical Note 6 في history.md).

---

## Part 8.3 — Service: INormalRangeService (مع قاعدة أضيق مدى يفوز — قرار 16)

**الطبقة**: Service Interface.

**الملفات المُنشَأة**:
- `Services/Interfaces/INormalRangeService.cs`

**التوقيع الحرفي (كما ورد في `analysis_and_plan_v3.md` Part 8.3)**:
```csharp
namespace NewLab.Services.Interfaces
{
    public sealed record NormalRangeEvaluation(
        string Category, // "Normal" | "AbnormalLow" | "AbnormalHigh" | "CriticalLow" | "CriticalHigh"
        string? Flag);

    public interface INormalRangeService
    {
        Task<List<NormalRange>> GetForTestAsync(int labTestId);
        Task<NormalRange> AddAsync(NormalRange range);
        Task<NormalRange> UpdateAsync(NormalRange range);
        Task DeleteAsync(int rangeId);
        Task<NormalRange?> GetMatchingRangeAsync(int labTestId, Patient patient);
        Task<NormalRangeEvaluation> EvaluateValueAsync(NormalRange range, decimal value);
    }
}
```

**منطق GetMatchingRangeAsync (قرار 16 — قاعدة "أضيق مدى يفوز")**:
1. `Where(nr => nr.LabTestId == labTestId)`.
2. `Where(nr => nr.Gender == patient.Gender)` — لا حاجة لـ`Both` بحكم قرار 17.
3. تحويل عمر المريض إلى نفس `AgeUnit` لكل مدى (Day/Month/Year) ثم فلترة الفترة `AgeFrom ≤ patientAge ≤ AgeTo`.
4. **قاعدة الأضيق**: `OrderBy(nr => nr.AgeTo - nr.AgeFrom).FirstOrDefault()` — الفارق الأصغر يفوز. مثال محفوظ في التعليق داخل الميثود: `(0-12)` بفارق 12 يفوز على `(0-120)` بفارق 120 لمريض عمره 10 سنوات.
5. إن لم يُوجد → `return null;` (يُفسَّر لاحقاً في F4 كنتيجة بدون معدل مرجعي).

**منطق EvaluateValueAsync**:
- `value < CriticalLowLimit` → `("CriticalLow", CriticalFlag)`.
- `value > CriticalHighLimit` → `("CriticalHigh", CriticalFlag)`.
- `value < LowLimit` → `("AbnormalLow", LowFlag)`.
- `value > HighLimit` → `("AbnormalHigh", HighFlag)`.
- خلاف ذلك → `("Normal", null)`.

**الاعتماد**: كيان `NormalRange` (Part 8.1) + كيان `Patient` (F1 — موجود) + enum `Gender`/`AgeUnit` (F1 — موجود).

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 8.4 — FluentValidation: NormalRangeValidator

**الطبقة**: Validation.

**الملفات المُنشَأة**:
- `Models/Validation/NormalRangeValidator.cs`

**Rules الحرفية (من `analysis_and_plan_v3.md` Part 8.4)**:
```csharp
public class NormalRangeValidator : AbstractValidator<NormalRange>
{
    public NormalRangeValidator()
    {
        RuleFor(r => r.LabTestId).GreaterThan(0);
        RuleFor(r => r.TestName).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Gender)
            .IsInEnum()
            .Must(g => g == Gender.Male || g == Gender.Female)   // قرار 17
            .WithMessage("الجنس يجب أن يكون Male أو Female فقط");
        RuleFor(r => r.LowLimit).LessThanOrEqualTo(r => r.HighLimit)
            .WithMessage("Low limit يجب أن يكون أقل من أو يساوي High limit");
        RuleFor(r => r.AgeFrom).LessThanOrEqualTo(r => r.AgeTo)
            .WithMessage("Age From يجب أن يكون أقل من أو يساوي Age To");
        RuleFor(r => r.CriticalLowLimit)
            .LessThanOrEqualTo(r => r.LowLimit)
            .When(r => r.CriticalLowLimit.HasValue)
            .WithMessage("Critical Low يجب أن يكون أقل من أو يساوي Low limit");
        RuleFor(r => r.CriticalHighLimit)
            .GreaterThanOrEqualTo(r => r.HighLimit)
            .When(r => r.CriticalHighLimit.HasValue)
            .WithMessage("Critical High يجب أن يكون أكبر من أو يساوي High limit");
    }
}
```

**الاعتماد**: `NormalRange` (Part 8.1) + FluentValidation (موجود من F1/F7).

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 8.5 — Implementation + DI

**الطبقة**: Service Implementation + Composition Root.

**الملفات المُنشَأة**:
- `Services/Implementations/NormalRangeService.cs` — يعتمد على `NewLabDbContext` و`ICurrentUserService` (constructor injection).
- **DeleteAsync** يفحص `_currentUserService.IsAdmin` ويرمي `UnauthorizedAccessException("عملية الحذف تتطلب صلاحية Admin")` — تناسق مع نمط `LabTestService.DeleteAsync` و`PatientService.DeleteAsync` (نمط CP-3 من history.md Phase 6).

**الملفات المعدَّلة**:
- `App.xaml.cs` — في بلوك `ConfigureServices`، بعد `services.AddScoped<ILabTestService, LabTestService>();`:
  ```csharp
  services.AddScoped<INormalRangeService, NormalRangeService>();
  services.AddScoped<IValidator<NormalRange>, NormalRangeValidator>();
  ```

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 8.6 — NormalRangeViewModel

**الطبقة**: ViewModel.

**الملفات المُنشَأة**:
- `ViewModels/Pages/NormalRangeViewModel.cs`

**الخصائص الحرفية (من `analysis_and_plan_v3.md` Part 8.6)**:
- `public LabTest? ParentTest { get; private set; }` — يُعيَّن عبر `LoadForTest(LabTest test)`.
- `public ObservableCollection<NormalRange> Ranges { get; } = new();`
- `[ObservableProperty] private NormalRange? selectedRange;`
- حقول Form (16 حقلاً مطابقة لـPart 8.1) — كلها `[ObservableProperty]`.
- `[ObservableProperty] private bool isAddMode = true;`
- `[ObservableProperty] private bool isEditMode;`
- `public IEnumerable<Gender> AvailableGenders => new[] { Gender.Male, Gender.Female };` — **لا `Both`** (قرار 17).
- `public IEnumerable<AgeUnit> AvailableAgeUnits => Enum.GetValues<AgeUnit>();`
- `public bool IsAdmin => _currentUserService.IsAdmin;`

**الأوامر (Commands) الحرفية**:
- `AddRangeCommand` — يُهيِّئ الوضع للإضافة (ClearForm + IsAddMode=true).
- `EditCommand` — يحمِّل `SelectedRange` في Form.
- `SaveCommand` — Validate + Add/Update عبر `INormalRangeService` + `LoadRangesAsync()`.
- `CancelCommand` — ClearForm.
- `DeleteCommand` — `CanExecute = IsAdmin` (نمط CP-3) → يستدعي `INormalRangeService.DeleteAsync`.
- `BackToTestsCommand` — يغلق النافذة (يُنَفَّذ في code-behind عبر Action من الVM أو `Window.Close()` handler).

**Constructor**:
```csharp
public NormalRangeViewModel(
    INormalRangeService normalRangeService,
    IDialogService dialogService,
    IValidator<NormalRange> validator,
    ICurrentUserService currentUserService)
```

**Method عام**:
- `public async Task LoadForTest(LabTest test)` — يعيِّن `ParentTest = test;` ثم `await LoadRangesAsync();` ثم يعيِّن `FormTestName = test.TestName; FormTestUnit = ""; ...`.

**الاعتماد**: `INormalRangeService` (Part 8.5) + `IValidator<NormalRange>` (Part 8.4) + `ICurrentUserService` (موجود منذ Phase 5) + `IDialogService` (موجود).

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 8.7 — NormalRangeView (Window)

**الطبقة**: View.

**الملفات المُنشَأة**:
- `Views/Windows/NormalRangeView.xaml`
- `Views/Windows/NormalRangeView.xaml.cs`

**التخطيط الحرفي (من `analysis_and_plan_v3.md` Part 8.7)**:
- `<Window ... FlowDirection="RightToLeft" Title="المعدل الطبيعي" WindowStartupLocation="CenterOwner" Height="700" Width="1000">`
- Grid 3 صفوف: (Top) اسم التحليل من `{Binding ParentTest.TestName}` + رقم `{Binding ParentTest.Code}`. (Middle) Grid 3 أعمدة:
  - **يمين (Column 0)**: قائمة `Ranges` (`ListBox ItemsSource={Binding Ranges} SelectedItem={Binding SelectedRange}`) مع `ItemTemplate` يعرض `Gender + " " + AgeFrom + "-" + AgeTo + " " + AgeUnit`.
  - **وسط (Column 1)**: `ScrollViewer` يحتوي **قسمين**: قسم "Normal" (Gender ComboBox من `AvailableGenders` — Male/Female فقط قرار 17؛ AgeFrom/AgeTo/AgeUnit؛ NormalRangeText؛ LowLimit/HighLimit؛ LowFlag/HighFlag؛ LowComment/HighComment/CriticalComment) + قسم "Critical" (CriticalRangeText؛ CriticalLowLimit/CriticalHighLimit؛ CriticalFlag).
  - **يسار (Column 2)**: أزرار الأوامر عمودياً + `<controls:LatinSymbolsPad TargetTextBox="{Binding ElementName=NormalRangeTextBox}" />` (قرار 14 — إعادة استخدام UserControl الموجود من F7 دون تعديل).
- (Bottom) شريط أزرار: اضافة مدى / تعديل / حفظ / تراجع / حذف / قائمة التحاليل.
- `Window.InputBindings`: `<KeyBinding Key="Escape" Command="{Binding CancelCommand}" />`.

**Code-behind الحرفي**:
```csharp
public NormalRangeView()
{
    InitializeComponent();
    // Populate ComboBoxes بحكم Technical Note 4 (enum x:Array يفشل)
    GenderCombo.ItemsSource = new[] { Gender.Male, Gender.Female };
    AgeUnitCombo.ItemsSource = System.Enum.GetValues<AgeUnit>();
}
```

**قيود XAML/MaterialDesign المُلزَمة (من Technical Notes 3/4/5 في history.md)**:
- استخدام `<TextBox Style="{StaticResource MaterialDesignOutlinedTextBox}" materialDesign:HintAssist.Hint="..."/>` وليس `<materialDesign:MaterialDesignOutlinedTextBox>` (تجنب MC3074).
- لا `x:Array` مع enum values داخل XAML — الملء من code-behind.
- التحقق النهائي عبر `dotnet clean && dotnet build` لكشف تحذيرات XAML.

**الاعتماد**: `NormalRangeViewModel` (Part 8.6) + `LatinSymbolsPad` UserControl (F7 موجود).

**بوابة البناء**: 0 errors / 0 warnings **بعد Clean+Rebuild** (وليس Incremental).

---

## Part 8.8 — دمج مع Function 7

**الطبقة**: Retro-Integration (ViewModel + DI).

**الملفات المعدَّلة**:
- `ViewModels/Pages/LabTestManagementViewModel.cs` — تفعيل placeholder `OpenNormalRange` (انظر جدول Retro-Integration with F7 أعلاه للخطوات الست).
- `App.xaml.cs` — إضافة تسجيل `NormalRangeViewModel`:
  ```csharp
  services.AddTransient<NormalRangeViewModel>();
  ```
  (الـcontainer يحلّ `Func<NormalRangeViewModel>` تلقائياً من هذا التسجيل — لا حاجة لـفَبريك يدوي).

**نقاط التحقق**:
- الزر "المعدل الطبيعي" في `LabTestManagementView.xaml` (سطر 271-274) **لا يتغيَّر** — الاسم `OpenNormalRangeCommand` يبقى.
- استدعاء `window.ShowDialog()` وليس `.Show()` — النافذة modal لأنها تعتمد على تحليل مُختار.
- `SelectedTest == null` → عرض رسالة "اختر تحليلاً أولاً" وعدم فتح النافذة (نمط دفاعي).
- **لا حذف** لسطر أو خاصية من `LabTestManagementViewModel` — فقط استبدال داخل جسم الميثود.

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 8.9 — دمج مع Function 4 (إدخال النتائج)

**الطبقة**: Future-Impact only — **لا تنفيذ داخلي لـF4 هنا**.

**التوثيق**: هذا الجزء لا يُنشئ ملفات ولا يعدِّل ملفات. مهمته الوحيدة **توثيق التوقيع المستقر لـ`INormalRangeService`** بحيث F4 (Part 4.5 لاحقاً) يستهلكه كما هو:

```csharp
// في F4 لاحقاً (توضيحي فقط — لا تنفيذ الآن):
var range = await _normalRangeService.GetMatchingRangeAsync(labTestId, patient);
if (range != null)
{
    var evaluation = await _normalRangeService.EvaluateValueAsync(range, value);
    // evaluation.Category → تلوين النتيجة
    // evaluation.Flag     → عرض العلامة
}
```

**قيود مُلزَمة على هذه الشريحة**: لا يُضاف أي enum أو DTO مخصص لـF4 داخل F8 خارج ما نُصَّ عليه في Part 8.3 (`NormalRangeEvaluation`).

**بوابة البناء**: 0 errors / 0 warnings (بحكم عدم تغيير أي ملف).

---

## Part 8.10 — Build Verification

**الطبقة**: End-to-end verification.

**سيناريو الاختبار (من `analysis_and_plan_v3.md` Part 8.10)**:
1. `dotnet clean && dotnet build` → 0 errors / 0 warnings.
2. `dotnet ef database update -c NewLabDbContext` → migration `AddNormalRanges` تُطبَّق.
3. تشغيل التطبيق → تسجيل دخول Admin → التنقل إلى "بيانات النظام" → "بيانات التحاليل" → اختيار تحليل "Glucose" → الضغط على زر "المعدل الطبيعي" → **يجب أن تفتح نافذة `NormalRangeView`** (بدلاً من رسالة "ستُفعَّل في Function 8").
4. إضافة 6 معدلات (Male/Female × 3 فئات عمرية: 0-120 سنة، 1-29 يوم، 1-11 شهر) — **بدون Both** — قرار 17.
5. حفظ → التحقق في DB: `SELECT COUNT(*) FROM NormalRanges WHERE LabTestId=1` = 6.
6. اختبار قاعدة الأولوية (قرار 16): إضافة مدى (Male, 0-120 Year) + مدى (Male, 0-12 Year) → في الكود عبر Debugger أو Unit-Level: `GetMatchingRangeAsync(1, new Patient { Gender=Male, AgeValue=10, AgeUnit=Year })` → **يجب أن يعيد مدى (0-12) وليس (0-120)**.
7. اختبار الحذف بمستخدم غير Admin → `UnauthorizedAccessException` — رسالة "عملية الحذف تتطلب صلاحية Admin" تظهر عبر DialogService.
8. اختبار `EvaluateValueAsync` لثلاث قيم: (أ) داخل النطاق → "Normal". (ب) دون LowLimit وأعلى من CriticalLow → "AbnormalLow". (ج) دون CriticalLow → "CriticalLow".

**بوابة البناء النهائية للوظيفة**: 0 errors / 0 warnings **مع Clean+Rebuild الكامل** + قاعدة بيانات مُطبَّقة عليها الـmigration.

---

# ═══════════════════════════════════════════════════════════════

# 🔹 Function 2 — طباعة باركود للمريض

# ═══════════════════════════════════════════════════════════════

**Execution Order**: Step 4 of 8 (يُنفَّذ ثانياً في هذه الشريحة، بعد اكتمال F8).
**عدد الأجزاء**: 13 (Parts 2.1 → 2.13) — مطابق حرفياً لعدد الأجزاء في `analysis_and_plan_v3.md`.

---

## Part 2.1 — كيان BarcodeSettings + BarcodeLabel (بدون BranchNumber — قرار 5؛ مع أبعاد الملصق — قرار 4)

**الطبقة**: Models (Domain).

**الملفات المُنشَأة**:

**`Models/Domain/BarcodeSettings.cs`** — الخصائص الحرفية:
```csharp
public class BarcodeSettings
{
    public int Id { get; set; }
    public int OffsetX { get; set; }           // بالنقاط/millimeters — يُضبَط عبر Slider
    public int OffsetY { get; set; }
    public bool PrintFileCodeWithAll { get; set; }
    public int LabelWidth { get; set; } = 38;  // مم — قرار 4 (افتراضي 38)
    public int LabelHeight { get; set; } = 25; // مم — قرار 4 (افتراضي 25)
    // ⛔ محذوف نهائياً (قرار 5): BranchNumber — لا يوجد
}
```

**`Models/Domain/BarcodeLabel.cs`** — كيان Transient (غير مخزَّن في DB):
```csharp
public class BarcodeLabel
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int? SpecimenTypeId { get; set; }
    public string SpecimenName { get; set; } = string.Empty;
    public List<string> Tests { get; set; } = new();
    public string Code { get; set; } = string.Empty;       // كود 13 خانة
    public CodeType Type { get; set; }                     // enum من F1
}
```
**ملاحظة**: `BarcodeLabel` **لا يُضَاف كـ DbSet** — كيان View/Print فقط.

**الاعتماد**: enum `CodeType` (F1 — موجود).

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.2 — تحديث Patient + إضافة PatientCode

**الطبقة**: Model (Domain).

**تأكيد وجود حقول Patient الحالية** (من فحص `Models/Domain/Patient.cs`):
- ✅ `LabId (string?, MaxLength 50)` موجود.
- ✅ `FileCode (string?, MaxLength 50)` موجود.
- ✅ `VisitCode (string?, MaxLength 50)` موجود.
→ **لا تعديل مطلوب على `Patient.cs`**.

**الملفات المُنشَأة (اختياري — يعتمد على CP-F2-2)**:
- `Models/Domain/PatientCode.cs`:
  ```csharp
  public class PatientCode
  {
      public int Id { get; set; }
      public int PatientId { get; set; }
      public CodeType CodeType { get; set; }         // Case / File / Lab — enum من F1
      public string CodeValue { get; set; } = string.Empty;
      public DateTime IssuedAt { get; set; }
      public Patient Patient { get; set; } = null!;
  }
  ```

**اعتماد كيان `PatientCode` مُرَحَّل لـClarification Point (CP-F2-2)** — انظر Clarification Points في نهاية الوثيقة. القرار الافتراضي داخل هذه الخطة: **يُنشأ** لأن `analysis_and_plan_v3.md` يُدرجه صراحة (Part 2.2)، ويُتَوَقَّع أن يستفيد منه F6 (البحث عن مريض).

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.3 — Migration

**الطبقة**: Data (EF Core).

**الملفات المعدَّلة**:
- `Data/NewLabDbContext.cs` → إضافة:
  ```csharp
  public DbSet<BarcodeSettings> BarcodeSettings { get; set; }
  public DbSet<PatientCode> PatientCodes { get; set; }
  ```
  + Fluent API في `OnModelCreating`:
  ```csharp
  // BarcodeSettings — single row seeded على القيم الافتراضية
  modelBuilder.Entity<BarcodeSettings>().HasData(
      new BarcodeSettings { Id = 1, OffsetX = 0, OffsetY = 0,
                             PrintFileCodeWithAll = false,
                             LabelWidth = 38, LabelHeight = 25 } // قرار 4
  );

  // PatientCode
  modelBuilder.Entity<PatientCode>()
      .HasOne(pc => pc.Patient)
      .WithMany()
      .HasForeignKey(pc => pc.PatientId)
      .OnDelete(DeleteBehavior.Cascade);

  modelBuilder.Entity<PatientCode>()
      .HasIndex(pc => new { pc.PatientId, pc.CodeType }).IsUnique();

  modelBuilder.Entity<PatientCode>()
      .HasIndex(pc => pc.CodeValue);           // للبحث في F3/F5/F6
  ```

**الملفات المُنشَأة**:
- `Migrations/<timestamp>_AddBarcodeSettingsAndPatientCodes.cs` + `.Designer.cs`.
- تحديث `Migrations/NewLabDbContextModelSnapshot.cs` (تلقائي).

**التحقق**: `dotnet ef database update -c NewLabDbContext` → جدولا `BarcodeSettings` (مع سجل واحد افتراضي) و`PatientCodes` (فارغ). لا عمود `BranchNumber` في أي جدول (قرار 5).

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.4 — Service: IBarcodeService (بدون BranchNumber — قرار 5)

**الطبقة**: Service Interface.

**الملفات المُنشَأة**:
- `Services/Interfaces/IBarcodeService.cs`:
  ```csharp
  namespace NewLab.Services.Interfaces
  {
      public interface IBarcodeService
      {
          string GenerateCaseCode(Patient patient, PatientVisit visit);
          string GenerateFileCode(Patient patient);
          string GenerateLabCode(Patient patient);
          Task<Patient> GetOrCreateLabIdAsync(Patient patient);
          Task<List<BarcodeLabel>> GetLabelsForPatientAsync(int patientId);
          Task<BarcodeSettings> GetSettingsAsync();
          Task SaveSettingsAsync(BarcodeSettings settings);
      }
  }
  ```

**منطق توليد الكود (قرار 5 — الفرع مُثبَّت برمجياً)**:
- ثابت داخل `BarcodeService`: `private const string BranchConstant = "1";` — **لا يُقرأ من إعداد ولا من DB**.
- صيغة الكود 13 خانة (حسب `analysis_and_plan_v3.md`): `1-4-100623-1-006-8` = `(padding "1") - (CodeType numeric 1/3/5) - (YYMMDD معكوس اليوم) - (BranchConstant "1") - (تسلسل 3 خانات) - (رقم يوم الأسبوع 1-7)`.
- بادئات CodeType الثلاثة:
  - `CodeType.Case` → بادئة موقع 2 = "1" (يبدأ برقم 1 من اليسار).
  - `CodeType.File` → بادئة موقع 2 = "3" (يبدأ برقم 3 من اليسار).
  - `CodeType.Lab` → بادئة موقع 2 = "5" (يبدأ برقم 5 من اليسار).

**منطق تجميع التحاليل في ملصقات (`GetLabelsForPatientAsync`)**:
- يجلب تحاليل المريض (سيُستكمَل مع F3 لاحقاً — الآن يعتمد على `Patient.LabId` وتحاليله المسجَّلة عبر F1، أو stub قابل للتوسعة).
- يجمِّع التحاليل حسب `LabTest.DefaultSpecimenTypeId` (موجود على `LabTest` من F7).
- كل مجموعة → `BarcodeLabel` واحد مع كود Case فريد.

**الاعتماد**: `Patient`, `PatientVisit`, `LabTest` (كلها موجودة من F1/F7) + enum `CodeType` (F1) + `NewLabDbContext`.

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.5 — Implementation

**الطبقة**: Service Implementation.

**الملفات المُنشَأة**:
- `Services/Implementations/BarcodeService.cs` — Constructor Injection:
  ```csharp
  public BarcodeService(NewLabDbContext context, IPatientService patientService)
  ```
- **DailySequenceNumber**: يُقرأ من `PatientVisit.DailySequenceNumber` (موجود من F1).
- **رقم يوم الأسبوع**: `((int)DateTime.Today.DayOfWeek) + 1` (1-7 بدلاً من 0-6).
- **`GetOrCreateLabIdAsync`**: إذا `patient.LabId == null` → توليد كود Lab جديد + حفظ `patient.LabId = code` + `SaveChangesAsync` + إن اعتُمِد `PatientCode` (Part 2.2) → إضافة سجل `PatientCode { CodeType = Lab, CodeValue = code, IssuedAt = Now }`.
- **`GetSettingsAsync`**: `return await _context.BarcodeSettings.FirstOrDefaultAsync() ?? new BarcodeSettings { LabelWidth = 38, LabelHeight = 25 };` (قرار 4 fallback).
- **لا `ICurrentUserService`** هنا — لا عمليات إدارية Admin-only في F2 (لا تتطلب فحص حذف).

**الاعتماد**: `IBarcodeService` (Part 2.4) + `NewLabDbContext` + `IPatientService` (موجود من F1).

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.6 — تسجيل DI

**الطبقة**: Composition Root.

**الملفات المعدَّلة**:
- `App.xaml.cs` — في بلوك `ConfigureServices` بعد `services.AddScoped<INormalRangeService, ...>();` (المُضاف في F8 Part 8.5):
  ```csharp
  services.AddScoped<IBarcodeService, BarcodeService>();
  ```

**ملاحظة**: **لا يُضاف** validator لـBarcodeSettings — الحقول رقمية بسيطة، والتحقق يتم في الـVM (`LabelWidth > 0 && LabelHeight > 0`) دون FluentValidation.

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.7 — مولّد صور الباركود (ZXing.Net — قرار 3)

**الطبقة**: Helper (Static/Utility).

**الملفات المعدَّلة**:
- `NewLab.csproj` — إضافة `PackageReference`:
  ```xml
  <PackageReference Include="ZXing.Net" Version="0.16.9" />
  <PackageReference Include="ZXing.Net.Bindings.Windows.Compatibility" Version="0.16.14" />
  ```
  (نسخة `ZXing.Net.Bindings.Windows.Compatibility` مطلوبة لأن `ZXing.Net` الأساسي لا يحتوي `System.Drawing.Common` bindings في .NET 8).

**الملفات المُنشَأة**:
- `Helpers/BarcodeImageGenerator.cs`:
  ```csharp
  namespace NewLab.Helpers
  {
      public static class BarcodeImageGenerator
      {
          public static BitmapSource GenerateCode128(string data, int widthPx = 300, int heightPx = 100)
          {
              var writer = new ZXing.BarcodeWriter<System.Drawing.Bitmap>
              {
                  Format = ZXing.BarcodeFormat.CODE_128,
                  Options = new ZXing.Common.EncodingOptions
                  {
                      Width = widthPx, Height = heightPx, Margin = 5
                  },
                  Renderer = new ZXing.Windows.Compatibility.BitmapRenderer()
              };
              using var bitmap = writer.Write(data);
              return ConvertToBitmapSource(bitmap);
          }
          private static BitmapSource ConvertToBitmapSource(System.Drawing.Bitmap bmp) { /* … */ }
      }
  }
  ```
  - **لا تبعية على DI** — static class مُتَعمَّد لأنه Utility بحت.
  - **قرار 3**: `ZXing.Net` حصراً — **لا** `BarcodeStandard` ولا أي مكتبة أخرى.

**مجلد `Helpers/` جديد** (بحكم فحص Baseline، ذُكِر في history.md كمجلد مخطط لكنه غير موجود فعلياً — يُنشأ الآن).

**بوابة البناء**: 0 errors / 0 warnings **بعد `dotnet restore`**.

---

## Part 2.8 — PDF Layout عبر QuestPDF (مع أبعاد الملصق — قرار 4)

**الطبقة**: Service Implementation.

**الملفات المُنشَأة**:
- `Services/Implementations/BarcodePrintService.cs`:
  ```csharp
  namespace NewLab.Services.Implementations
  {
      public class BarcodePrintService : IBarcodePrintService
      {
          public byte[] GeneratePdf(IEnumerable<BarcodeLabel> labels, BarcodeSettings settings)
          {
              QuestPDF.Settings.License = LicenseType.Community;
              return Document.Create(container =>
              {
                  foreach (var label in labels)
                  {
                      container.Page(page =>
                      {
                          page.Size(settings.LabelWidth, settings.LabelHeight,
                                    QuestPDF.Infrastructure.Unit.Millimetre);   // قرار 4
                          page.Margin(2, QuestPDF.Infrastructure.Unit.Millimetre);
                          page.Content().Element(c => ComposeLabel(c, label, settings));
                      });
                  }
              }).GeneratePdf();
          }
          // ComposeLabel: يطبق OffsetX/OffsetY، يرسم صورة الباركود من BarcodeImageGenerator، يضيف الاسم/النوع/الكود
      }
  }
  ```
- `Services/Interfaces/IBarcodePrintService.cs`:
  ```csharp
  public interface IBarcodePrintService
  {
      byte[] GeneratePdf(IEnumerable<BarcodeLabel> labels, BarcodeSettings settings);
  }
  ```

**الملفات المعدَّلة**:
- `App.xaml.cs` — إضافة `services.AddScoped<IBarcodePrintService, BarcodePrintService>();`.

**اعتماد**: `QuestPDF 2026.7.1` (موجود من Baseline) + `BarcodeImageGenerator` (Part 2.7) + `BarcodeSettings` (Part 2.1).

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.9 — BarcodeViewModel

**الطبقة**: ViewModel.

**الملفات المُنشَأة**:
- `ViewModels/Pages/BarcodeViewModel.cs`

**الخصائص الحرفية (من `analysis_and_plan_v3.md` Part 2.9)**:
- `[ObservableProperty] private Patient? patient;`
- `public ObservableCollection<BarcodeLabel> Labels { get; } = new();`
- `[ObservableProperty] private int offsetX;`
- `[ObservableProperty] private int offsetY;`
- `[ObservableProperty] private bool printFileCodeWithAll;`
- `[ObservableProperty] private int labelWidth = 38;`  // قرار 4
- `[ObservableProperty] private int labelHeight = 25;` // قرار 4
- `[ObservableProperty] private string? extraBarcodeName;`
- `[ObservableProperty] private string? extraBarcodeDescription;`
- `[ObservableProperty] private BarcodeLabel? selectedLabel;`

**Constructor**:
```csharp
public BarcodeViewModel(
    IBarcodeService barcodeService,
    IBarcodePrintService printService,
    IPatientService patientService,
    IDialogService dialogService)
```

**Method عام**:
- `public async Task LoadForPatientAsync(Patient p)` — يعيِّن `Patient = p;` + `await LoadLabelsAsync();` + `await LoadSettingsAsync();`.

**الأوامر**:
- `PrintFileCodeCommand` — يولِّد ملصق كود الملف عبر `_barcodeService.GenerateFileCode(Patient)` ثم `IBarcodePrintService.GeneratePdf(...)`.
- `PrintLabCodeCommand` — `CanExecute = Patient?.LabId != null` (نشط فقط عند وجود LabId — نص التخطيط).
- `PrintLabelCommand(BarcodeLabel? label)` — يطبع ملصقاً واحداً.
- `PrintAllCommand` — يطبع كل الملصقات (+ ملصق كود الملف إن `PrintFileCodeWithAll == true`).
- `SaveSettingsCommand` — يحفظ `OffsetX/OffsetY/LabelWidth/LabelHeight/PrintFileCodeWithAll` عبر `IBarcodeService.SaveSettingsAsync`.
- `AddExtraBarcodeCommand` — يضيف ملصقاً يدوياً من `ExtraBarcodeName + ExtraBarcodeDescription`.

**لا `[RelayCommand] private void Print()` بلا لاحقة Async** — كل الأوامر التي تتصل بـService تكون `async Task ...Async()` (نمط CP-2).

**الاعتماد**: `IBarcodeService` (Part 2.4/2.5) + `IBarcodePrintService` (Part 2.8) + `IPatientService` (موجود) + `IDialogService`.

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.10 — BarcodeView (Window)

**الطبقة**: View.

**الملفات المُنشَأة**:
- `Views/Windows/BarcodeView.xaml`
- `Views/Windows/BarcodeView.xaml.cs`

**التخطيط الحرفي (من `analysis_and_plan_v3.md` Part 2.10)**:
- `<Window ... FlowDirection="RightToLeft" Title="طباعة الباركود" WindowStartupLocation="CenterOwner" Height="700" Width="900">`
- Grid 4 صفوف:
  - **Row 0 (Top)** — WrapPanel: أزرار "طباعة كود الملف" / "طباعة كود المعمل" (`CanExecute = Patient?.LabId != null`) / CheckBox "طباعة كود الملف مع الكل" (`IsChecked={Binding PrintFileCodeWithAll}`).
  - **Row 1 (Middle)** — `ItemsControl ItemsSource="{Binding Labels}"` مع `ItemTemplate` يعرض كل ملصق (SpecimenName + Tests كنص + صورة الباركود). زر "طباعة" لكل ملصق. **Drag & Drop** بين الملصقات — عبر `PreviewMouseLeftButtonDown` و`Drop` handlers في code-behind (نمط WPF قياسي، لا logic في VM).
  - **Row 2 (Bottom-Middle)** — Slider أفقي `Value={Binding OffsetX}` (Min=-50, Max=50) + Slider عمودي `Value={Binding OffsetY}` + زر "حفظ الإحداثيات" (`Command={Binding SaveSettingsCommand}`).
  - **Row 3 (Bottom)** — TextBox `Text={Binding ExtraBarcodeName}` (`materialDesign:HintAssist.Hint="اسم التحليل الإضافي"`) + TextBox `Text={Binding ExtraBarcodeDescription}` (`Hint="وصف العينة"`) + زر "إضافة" (`Command={Binding AddExtraBarcodeCommand}`).
- **إعدادات أبعاد الملصق** (قرار 4): NumericUpDown/TextBox `Text={Binding LabelWidth}` و`{Binding LabelHeight}` (مم) — قابلة للتعديل مع القيم الافتراضية 38×25.

**قيود XAML/MaterialDesign المُلزَمة** (نفس Part 8.7):
- `<TextBox Style="{StaticResource MaterialDesignOutlinedTextBox}" materialDesign:HintAssist.Hint="..."/>` — تجنب MC3074.
- التحقق بـ `dotnet clean && dotnet build`.

**Code-behind**:
```csharp
public BarcodeView()
{
    InitializeComponent();
}
// + معالجات Drag & Drop (بلا صلة بالـVM logic)
```

**الاعتماد**: `BarcodeViewModel` (Part 2.9).

**بوابة البناء**: 0 errors / 0 warnings **بعد Clean+Rebuild**.

---

## Part 2.11 — تسجيل الـ VM في DI

**الطبقة**: Composition Root.

**الملفات المعدَّلة**:
- `App.xaml.cs` — إضافة:
  ```csharp
  services.AddTransient<BarcodeViewModel>();
  ```
- **لا `DataTemplate`** يُضَاف في `App.xaml` لأن `BarcodeView` **Window** وليس UserControl مُضمَّناً في ContentControl — يُنشَأ يدوياً في Part 2.12 بـ `new BarcodeView { DataContext = vm }`.

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.12 — استدعاء النافذة من PatientEntryView

**الطبقة**: Retro-Integration (ViewModel + View).

**الملفات المعدَّلة**:
- `ViewModels/Pages/PatientEntryViewModel.cs` — تفعيل placeholderي `PrintBarcode` و`LookupLabId` (انظر جدول Retro-Integration with F1 أعلاه للخطوات السبع).
- `Views/Pages/PatientEntryView.xaml` — **`InputBinding` F11 يبقى كما هو**:
  ```xml
  <KeyBinding Key="F11" Command="{Binding PrintBarcodeCommand}" />
  ```
  **ملاحظة تسمية حرِجة**: بعد تحويل `PrintBarcode()` إلى `PrintBarcodeAsync()`, يولِّد CommunityToolkit.Mvvm الأمر باسم `PrintBarcodeCommand` (يحذف اللاحقة `Async` من اسم الأمر) — نفس الاسم القديم يبقى، ولا يُكسر أي binding.

**نقاط التحقق**:
- زر "باركود" و`F11` KeyBinding **يبقيان بنفس Binding** — `{Binding PrintBarcodeCommand}`.
- زر "لاب اي دي" **يبقى بنفس Binding** — `{Binding LookupLabIdCommand}` (LookupLabId بعد Async → `LookupLabIdCommand`).
- Modal Window يُفتح بـ `.ShowDialog()` وليس `.Show()`.
- **لا تعديل** على `Views/Windows/MainWindow.xaml` — الاختصار F11 محلي على `PatientEntryView`، لا يُنقل لـMainWindow (تجنب تعارض مع اختصارات لاحقة لـF3/F4).

**بوابة البناء**: 0 errors / 0 warnings.

---

## Part 2.13 — Build Verification

**الطبقة**: End-to-end verification.

**سيناريو الاختبار (من `analysis_and_plan_v3.md` Part 2.13)**:
1. `dotnet clean && dotnet build` → 0 errors / 0 warnings.
2. `dotnet ef database update -c NewLabDbContext` → migration `AddBarcodeSettingsAndPatientCodes` تُطبَّق. `SELECT * FROM BarcodeSettings` → صف واحد بـ`LabelWidth=38, LabelHeight=25, BranchNumber column NOT EXISTS` (قرار 5).
3. تشغيل التطبيق → تسجيل دخول → "المرضى" → "إضافة وتعديل بيانات المرضى" → إضافة مريض جديد → حفظ.
4. الضغط على زر "باركود" (أو F11) → **يجب أن تفتح نافذة `BarcodeView`** (بدلاً من رسالة "ستُفعَّل في Function 2").
5. التحقق من صيغة الكود المولَّد: `1-4-YYMMDD-1-XXX-D` حيث `1` الرابع (الفرع) **دائماً "1"** — قرار 5.
6. الضغط على "لاب اي دي" → إن لم يكن للمريض LabId، يُولَّد كود Lab + يُحفَظ.
7. تجريب Slider OffsetX/OffsetY → حفظ → إعادة فتح النافذة → القيم محفوظة.
8. تجريب طباعة ملصق → PDF preview بأبعاد 38×25 مم (قرار 4).
9. تعديل `LabelWidth=50, LabelHeight=30` → حفظ → إعادة الطباعة → PDF بالأبعاد الجديدة.
10. اختبار Drag & Drop لنقل تحليل بين ملصقات — سلوك بصري صحيح.

**بوابة البناء النهائية للوظيفة**: 0 errors / 0 warnings **مع Clean+Rebuild** + قاعدة بيانات مُطبَّقة عليها الـmigration + PDF preview يعمل.

---

# ═══════════════════════════════════════════════════════════════

# ✅ Cross-Slice MVVM Compliance Checklist

# ═══════════════════════════════════════════════════════════════

| # | القاعدة | تحقُّق F8 | تحقُّق F2 |
|---|---|---|---|
| 1 | لا `DbContext` داخل أي ViewModel | ✅ `NormalRangeViewModel` يستخدم `INormalRangeService` فقط | ✅ `BarcodeViewModel` يستخدم `IBarcodeService`/`IBarcodePrintService` فقط |
| 2 | Constructor Injection حصراً (لا Service Locator) | ✅ 4 مُعطيات في constructor `NormalRangeViewModel` | ✅ 4 مُعطيات في constructor `BarcodeViewModel` |
| 3 | `ICurrentUserService.IsAdmin` كمصدر وحيد لصلاحية Admin | ✅ `DeleteAsync` في `NormalRangeService` + `CanDelete => IsAdmin` في VM | N/A — لا عمليات Admin-only في F2 |
| 4 | Async Void ممنوع | ✅ كل الأوامر `[RelayCommand] private async Task XxxAsync()` | ✅ نفس النمط |
| 5 | Code-behind في Views محصور بـ InitializeComponent + ComboBox enum population (Technical Note 4) | ✅ `NormalRangeView.xaml.cs` — Gender/AgeUnit combos | ✅ `BarcodeView.xaml.cs` — Drag & Drop handlers (سلوك UI بحت) |
| 6 | XAML يستخدم `<TextBox Style="MaterialDesignOutlinedTextBox">` لا `<materialDesign:MaterialDesignOutlinedTextBox>` (Technical Note 3) | ✅ | ✅ |
| 7 | `dotnet clean && dotnet build` (وليس Incremental فقط) قبل إعلان 0 warnings (Technical Note 5) | ✅ Parts 8.7, 8.10 | ✅ Parts 2.10, 2.13 |
| 8 | لا تسمية Command تتعارض مع `[ObservableProperty]` (Technical Note 2) | ✅ `OpenNormalRange`, `AddRange`, `Edit`, `Save`, `Cancel`, `Delete`, `BackToTests` — لا تعارض | ✅ `PrintBarcode` (لا خاصية `Barcode`)، `LookupLabId` (بدلاً من `LabId`)، `AddExtraBarcode`, `SaveSettings` |
| 9 | `Func<T>` factory pattern لفتح Modal بدلاً من `IServiceProvider` | ✅ `Func<NormalRangeViewModel>` في LabTestManagementViewModel | ✅ `Func<BarcodeViewModel>` في PatientEntryViewModel |
| 10 | لا `ServiceProvider.GetRequiredService<>()` داخل ViewModel أو View | ✅ | ✅ |
| 11 | ObservableCollections مقروءة (`{ get; } = new();`) لا `set` | ✅ `Ranges` | ✅ `Labels` |
| 12 | تسجيلات DI مُنَظَّمة (Scoped for DbContext-dependent, Singleton for cross-scope) | ✅ Service Scoped + VM Transient | ✅ Service Scoped + VM Transient |

---

# ═══════════════════════════════════════════════════════════════

# ❓ Clarification Points

# ═══════════════════════════════════════════════════════════════

هذه نقاط اكتُشِفت أثناء الفحص الفعلي ولم يحسمها نص `analysis_and_plan_v3.md` بشكل قاطع. **لا تُغيَّر من تلقاء أحد** — تُطرح للمالك.

### CP-F8-1 — Navigation Property على LabTest

**السؤال**: هل نضيف `public ICollection<NormalRange> NormalRanges { get; set; }` على `Models/Domain/LabTest.cs`، أم نكتفي بـ FK-only دون Collection؟

**القرار الافتراضي المُتَّبَع في هذه الخطة**: **لا نضيف** — نكتفي بـ FK. السبب: تجنب تضخيم كيان `LabTest` (25 خاصية حالياً) وتجنب Include ثقيل غير مطلوب في F7. `INormalRangeService.GetForTestAsync(labTestId)` يُوفِّر الاستعلام المطلوب.

**البديل إن اعتُمِد**: تعديل `LabTest.cs` (Model من F7) لإضافة Collection — يتطلب إعادة فحص EF Core relationships.

---

### CP-F8-2 — قيمة `TestUnit` الافتراضية

**السؤال**: `NormalRange.TestUnit` — هل تُقرأ من `LabTest` (لا حقل TestUnit عليه حالياً!)، أم يُدخِلها المستخدم في كل معدل؟

**التحقق من الكود**: `Models/Domain/LabTest.cs` **لا يحتوي حقل TestUnit** — 25 خاصية موثَّقة كلها بدون Unit.

**القرار الافتراضي المُتَّبَع**: `TestUnit` حقل مُدخَل يدوياً في `NormalRange` (كما نص Part 8.1). يُوَرَّث من أول معدل مُدخَل للتحليل كقيمة افتراضية عبر VM.

**البديل**: إضافة `TestUnit` على `LabTest.cs` — يُرَحَّل لـF4 إن قرَّرها المالك، ليس F8.

---

### CP-F8-3 — عرض تعبيرات نصية للـLimits

**السؤال**: `NormalRangeText` نص مطبوع (مثل "70-110 mg/dL")، بينما `LowLimit`/`HighLimit` قيم عددية. هل نطلب من المستخدم إدخال الاثنين، أم نولِّد النص من الأرقام تلقائياً؟

**القرار الافتراضي**: إدخال يدوي منفصل — يسمح بتنسيقات مثل "<10" أو "≤ 5.0" التي لا يمكن استخراجها من قيمتين عدديتين. مطابق لـPart 8.1.

---

### CP-F2-1 — مصدر `DailySequenceNumber` عند إنشاء الكود قبل حفظ الزيارة

**السؤال**: `BarcodeService.GenerateCaseCode` يتطلب `PatientVisit.DailySequenceNumber` — لكن قد يُنشَأ الكود قبل حفظ الزيارة في DB. من يحسِّب التسلسل؟

**القرار الافتراضي**: `BarcodeService` يستعلم `_context.PatientVisits.Where(v => v.VisitDate == today).CountAsync() + 1` عند توليد الكود — يعمل حتى قبل حفظ الزيارة الحالية.

**تحذير**: قد يُنتِج تسلسلات مكرَّرة في حالة concurrency عالية — مقبول للنسخة الأولى، يُحل لاحقاً بـTransaction أو Sequence على مستوى DB.

---

### CP-F2-2 — هل نُنشئ كيان `PatientCode` أم لا؟

**السؤال**: `analysis_and_plan_v3.md` Part 2.2 يقول "(اختياري)" حرفياً — هل نُنشئه الآن؟

**القرار الافتراضي المُتَّبَع في هذه الخطة**: **نُنشئه** — لأن:
1. F6 (البحث عن مريض) سيستفيد من فهرس `PatientCode.CodeValue` للبحث السريع بأي من الأكواد الثلاثة.
2. تتبُّع تاريخ إصدار الأكواد (`IssuedAt`) قيمة تدقيقية.
3. Migration واحدة أرخص من migration ثانية لاحقة.

**البديل**: تأجيله لـF6 — يوفِّر migration الآن لكنه يخلق retro-integration لاحقاً.

---

### CP-F2-3 — نموذج ترخيص QuestPDF

**السؤال**: `BarcodePrintService` يعيِّن `QuestPDF.Settings.License = LicenseType.Community;` — هل النظام تجاري (يتطلب Professional)؟

**التحقق**: history.md يذكر "System designed to be deployed to multiple laboratories" — يوحي بترخيص تجاري.

**القرار الافتراضي**: `LicenseType.Community` (مجاني لـ<$1M annual revenue). المالك يُحدِّث اللاحقة إن وصل الإيراد للحد.

---

### CP-F2-4 — Drag & Drop في `BarcodeView` — منطق VM أم code-behind فقط؟

**السؤال**: هل نُنشئ Attached Behaviors للـDrag & Drop لتنقيته من code-behind، أم نتركه كـevent handlers مباشرة؟

**القرار الافتراضي**: event handlers مباشرة — بحكم أن العملية بصرية بحتة (نقل ملصق) ولا تُغيِّر حالة قاعدة بيانات. نمط WPF المقبول MVVM-friendly.

---

# ═══════════════════════════════════════════════════════════════

# 📝 Change Manifest

# ═══════════════════════════════════════════════════════════════

## Files Created — Function 8 (10 ملفات)

```
Models/Domain/NormalRange.cs                        # Part 8.1
Models/Validation/NormalRangeValidator.cs           # Part 8.4
Services/Interfaces/INormalRangeService.cs          # Part 8.3
Services/Implementations/NormalRangeService.cs      # Part 8.5
ViewModels/Pages/NormalRangeViewModel.cs            # Part 8.6
Views/Windows/NormalRangeView.xaml                  # Part 8.7
Views/Windows/NormalRangeView.xaml.cs               # Part 8.7
Migrations/<timestamp>_AddNormalRanges.cs           # Part 8.2
Migrations/<timestamp>_AddNormalRanges.Designer.cs  # Part 8.2
Migrations/NewLabDbContextModelSnapshot.cs          # (تحديث — Part 8.2)
```

## Files Modified — Function 8 (3 ملفات)

```
Data/NewLabDbContext.cs                             # +DbSet<NormalRange> + Fluent API (Part 8.2)
App.xaml.cs                                         # +Scoped INormalRangeService, +Scoped IValidator<NormalRange>, +Transient NormalRangeViewModel (Parts 8.5, 8.8)
ViewModels/Pages/LabTestManagementViewModel.cs      # retro: تفعيل OpenNormalRange (Part 8.8)
```

**Files Untouched by F8** (تم فحصها ولم تتأثر):
- `Models/Domain/LabTest.cs`, `Models/Domain/Patient.cs`, `Models/Domain/PatientVisit.cs`, `Models/Domain/Referral.cs`, `Models/Domain/SpecimenType.cs`
- `Services/Implementations/PatientService.cs`, `Services/Implementations/LabTestService.cs`, `Services/Implementations/ReferralService.cs`
- `Views/Pages/LabTestManagementView.xaml` (الزر موجود مسبقاً)
- `Views/Pages/PatientEntryView.xaml`, `Views/Windows/MainWindow.xaml`

## Files Deleted — Function 8

```
(لا يوجد)
```

---

## Files Created — Function 2 (13 ملف)

```
Models/Domain/BarcodeSettings.cs                    # Part 2.1
Models/Domain/BarcodeLabel.cs                       # Part 2.1  (Transient)
Models/Domain/PatientCode.cs                        # Part 2.2  (يعتمد على CP-F2-2)
Services/Interfaces/IBarcodeService.cs              # Part 2.4
Services/Implementations/BarcodeService.cs          # Part 2.5
Services/Interfaces/IBarcodePrintService.cs         # Part 2.8
Services/Implementations/BarcodePrintService.cs     # Part 2.8
Helpers/BarcodeImageGenerator.cs                    # Part 2.7  (مجلد Helpers/ جديد)
ViewModels/Pages/BarcodeViewModel.cs                # Part 2.9
Views/Windows/BarcodeView.xaml                      # Part 2.10
Views/Windows/BarcodeView.xaml.cs                   # Part 2.10
Migrations/<timestamp>_AddBarcodeSettingsAndPatientCodes.cs           # Part 2.3
Migrations/<timestamp>_AddBarcodeSettingsAndPatientCodes.Designer.cs  # Part 2.3
```

## Files Modified — Function 2 (3 ملفات)

```
Data/NewLabDbContext.cs                             # +DbSet<BarcodeSettings>, +DbSet<PatientCode> + Fluent API + Seed (Part 2.3)
App.xaml.cs                                         # +Scoped IBarcodeService, +Scoped IBarcodePrintService, +Transient BarcodeViewModel (Parts 2.6, 2.8, 2.11)
NewLab.csproj                                       # +ZXing.Net + ZXing.Net.Bindings.Windows.Compatibility (Part 2.7)
ViewModels/Pages/PatientEntryViewModel.cs           # retro: تفعيل PrintBarcode + LookupLabId (Part 2.12)
Migrations/NewLabDbContextModelSnapshot.cs          # (تحديث — Part 2.3)
```

## Files Deleted — Function 2

```
(لا يوجد)
```

**Files Untouched by F2** (تم فحصها ولم تتأثر):
- `Models/Domain/Patient.cs` — الحقول LabId/FileCode/VisitCode موجودة مسبقاً من F1 (Part 2.2 يؤكد فقط).
- `Views/Pages/PatientEntryView.xaml` — أزرار "باركود"/"لاب اي دي" وKeyBinding F11 موجودة مسبقاً من F1؛ Bindings الأسماء تبقى نفسها.
- `Views/Windows/MainWindow.xaml` — لا اختصار F11 عام مطلوب (اختصار محلي في PatientEntryView كافٍ).
- كل ملفات F7 (`LabTest.cs`, `TestGroup.cs`, `LabTestManagementViewModel.cs`, ...).
- كل ملفات F8 المُنشَأة في القسم الأول من هذه الشريحة (النفَاذ يبقى منفصلاً).

---

# ═══════════════════════════════════════════════════════════════

# 🕸 Dependency Graph

# ═══════════════════════════════════════════════════════════════

```
                    ┌────────────────────────────────────────────────┐
                    │              F1 + F7 (Completed)               │
                    │  Patient, LabTest, TestGroup, ReferralPrice,   │
                    │  ICurrentUserService, IPatientService,         │
                    │  ILabTestService, LatinSymbolsPad              │
                    └────────────────────────────────────────────────┘
                                     │
                    ┌────────────────┴────────────────┐
                    ▼                                 ▼
        ═══════════════════════          ═══════════════════════
              FUNCTION 8                       FUNCTION 2
        ═══════════════════════          ═══════════════════════

           8.1 (NormalRange)                2.1 (BarcodeSettings +
                │                            │    BarcodeLabel)
                ▼                            ▼
           8.2 (Migration)                  2.2 (PatientCode)
                │                            │
                ▼                            ▼
           8.3 (INormalRangeService)        2.3 (Migration)
                │                            │
                ▼                            ▼
           8.4 (Validator)                  2.4 (IBarcodeService)
                │                            │
                ▼                            ▼
           8.5 (Implementation + DI)        2.5 (Implementation)
                │                            │
                ▼                            ▼
           8.6 (NormalRangeViewModel)       2.6 (DI Registration)
                │                            │
                ▼                            ▼
           8.7 (NormalRangeView)            2.7 (BarcodeImageGenerator
                │                            │    + ZXing.Net)
                ▼                            ▼
           8.8 (Retro F7 —                  2.8 (BarcodePrintService +
                فتح النافذة من                    QuestPDF)
                LabTestManagementVM)         │
                │                            ▼
                ▼                           2.9 (BarcodeViewModel)
           8.9 (F4 Awareness — لا تنفيذ)     │
                │                            ▼
                ▼                           2.10 (BarcodeView)
           8.10 (Build Verification)         │
                                             ▼
                                            2.11 (VM Registration in DI)
                                             │
                                             ▼
                                            2.12 (Retro F1 —
                                                  تفعيل PrintBarcode +
                                                  LookupLabId في
                                                  PatientEntryVM)
                                             │
                                             ▼
                                            2.13 (Build Verification)

الأسهم بين F8 وF2 مقصودة على عرض إجمالي:
      • F8 لا يعتمد على F2.
      • F2 لا يعتمد على F8.
      • كلاهما يعتمد على F1 + F7 المُكتملَين.
      • يُنَفَّذ F8 أولاً بحكم Execution Order فقط، ليس بحكم Dependency.
```

**Critical path**:
- **F8**: 8.1 → 8.2 → 8.3 → 8.4 → 8.5 → 8.6 → 8.7 → 8.8 → 8.10  (Part 8.9 موازٍ توثيقي)
- **F2**: 2.1 → 2.2 → 2.3 → 2.4 → 2.5 → 2.6 → 2.7 → 2.8 → 2.9 → 2.10 → 2.11 → 2.12 → 2.13

**Cross-slice dependencies داخل هذه الوثيقة**: **لا يوجد** — الوظيفتان مستقلتان تماماً في الكود؛ الترتيب "F8 أولاً ثم F2" هو نظام تنفيذ خارجي فقط، وليس اعتماداً تقنياً.

---

# ═══════════════════════════════════════════════════════════════

# ✔️ Sign-off Criteria

# ═══════════════════════════════════════════════════════════════

**الشريحة تُعتبر مكتملة (Ready for Merge) عند تحقق كل بند من البنود التالية**:

## للفحص التقني (Automated)

- [ ] `dotnet clean && dotnet build` من الجذر → **0 errors / 0 warnings** (وليس فقط Incremental — Technical Note 5).
- [ ] `dotnet ef migrations list -c NewLabDbContext` يعرض 5 migrations بالترتيب:
  1. `20260721171559_InitialCreate`
  2. `20260722032039_AddPatientsAndReferrals`
  3. `20260722063244_AddLabTestsAndReferralPrices`
  4. `<new>_AddNormalRanges` (F8)
  5. `<new>_AddBarcodeSettingsAndPatientCodes` (F2)
- [ ] `dotnet ef database update -c NewLabDbContext` ينجح بلا استثناءات.
- [ ] الاستعلامات:
  - `SELECT COUNT(*) FROM sys.tables WHERE name IN ('NormalRanges','BarcodeSettings','PatientCodes')` = **3**.
  - `SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('BarcodeSettings') AND name = 'BranchNumber'` = **0** (قرار 5).
  - `SELECT LabelWidth, LabelHeight FROM BarcodeSettings WHERE Id = 1` = **(38, 25)** (قرار 4).

## للفحص السلوكي (Manual)

### F8:
- [ ] الضغط على زر "المعدل الطبيعي" في `LabTestManagementView` **لم يعد** يعرض رسالة "ستُفعَّل في Function 8" — بل يفتح `NormalRangeView`.
- [ ] `AvailableGenders` ComboBox يحتوي حصراً **Male** و**Female** — لا `Both` (قرار 17).
- [ ] إدخال 6 معدلات (Male/Female × 3 age brackets) لتحليل واحد ينجح.
- [ ] `GetMatchingRangeAsync(labTestId=1, patient={Male, 10 Year})` عند وجود مدَيَين (0-120) و(0-12) → يُعيد **مدى (0-12)** (قرار 16).
- [ ] الحذف بمستخدم غير Admin → `UnauthorizedAccessException` معروض عبر DialogService.

### F2:
- [ ] الضغط على زر "باركود" (أو F11) في `PatientEntryView` **لم يعد** يعرض رسالة "ستُفعَّل في Function 2" — بل يفتح `BarcodeView`.
- [ ] الضغط على زر "لاب اي دي" **لم يعد** يعرض الرسالة — يُولِّد LabId جديد إن لم يكن موجوداً.
- [ ] كود Case المولَّد من صيغة 13 خانة يحتوي **"1"** في موقع رقم الفرع دائماً (قرار 5) — مثال: `1-4-260722-1-001-4`.
- [ ] `LabelWidth=38, LabelHeight=25` مم افتراضياً (قرار 4)، وقابلة للتعديل والحفظ.
- [ ] PDF Preview يُولِّد ملصقات بأبعاد صحيحة (يمكن اختباره بأداة QuestPDF Companion أو حفظ الـPDF).
- [ ] `NewLab.csproj` يحتوي `<PackageReference Include="ZXing.Net" ... />` (قرار 3).

## للمراجعة (Review Gate)

- [ ] كل الملفات الجديدة والمعدَّلة تستخدم الـnamespace القياسية `NewLab.*` المطابقة لبنية المجلد.
- [ ] لا `DbContext` داخل أي ViewModel (Cross-Slice Checklist #1).
- [ ] كل Constructor Injection حصراً — لا `IServiceProvider.GetRequiredService<>()` (Cross-Slice Checklist #2, #10).
- [ ] Clarification Points الست (CP-F8-1..3 + CP-F2-1..4) عُرضِت على المالك وتم توثيق ردوده — أو اعتُمِدت القرارات الافتراضية صراحة.
- [ ] `Docs/history.md` مُحدَّث بـ Phase 7: Function 8 + Phase 8: Function 2 بنفس مستوى تفصيل Phase 5 و Phase 6.

**عند تحقق كل ما سبق** → الشريحة جاهزة للـcommit بمسمى مماثل لـ "بعد تنفيذ الوظيفتين الثامنة والثانية".

---

**نهاية الوثيقة — Handoff_Slice_8&2_2.md**