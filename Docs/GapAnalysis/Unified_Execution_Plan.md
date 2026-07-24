# 📋 Unified Execution Plan — NewLab
## خارطة الطريق التنفيذية الموحّدة لسدّ فجوات الوظائف الثمانية

> **محتوى هذا الملف هو النسخة النهائية من `Docs/GapAnalysis/Unified_Execution_Plan.md`**
>
> **الحالة**: تخطيط فقط — لا يُنفَّذ أي كود من محتوى هذه الوثيقة الآن؛ التنفيذ يتطلب تفويضاً منفصلاً.
>
> **Commit المرجعي المعتمد للفحص**: `45f04aca4c048dd69e8404db0521e11263102699`
> **رسالة الـ Commit**: "بعد إضافة تقرير الوكيل السحابي"
>
> **ملاحظة مركزية**: هذا الكوميت لا يُضيف أي تعديل على كود المصدر بعد الكوميت السابق `56968b4` الذي بُنيت عليه الخطط الأربع الأصلية؛ الفرق الوحيد بينهما هو إضافة ملف `Cloud_Consolidation_Analysis.md` نفسه إلى `Docs/GapAnalysis/`. بالتالي جميع الفجوات المُذكورة في الخطط الأربع (F1_F7, F2_F8, F3_F4, F5_F6) لا تزال قائمة كما وُصفت، وقد تم التحقق من ذلك بفحص مباشر للكود عند هذا الكوميت (انظر `Clarification Points` — `CL-00`).

---

## 🧭 مقدمة — نطاق الوثيقة والمصادر المعتمدة

**النطاق**: خطة تنفيذية موحّدة لسدّ الفجوات في الوظائف الثمانية (F1..F8) في نظام `NewLab`، مبنية على دمج مباشر بتمريرة واحدة للخطط الأربع الأصلية + تقرير التوحيد السحابي، مع الالتزام الكامل بالقرارات الـ17 الملزمة في `Docs/analysis_and_plan_v3.md`.

**المصادر المعتمدة** (مقروءة بالكامل ومُطبَّقة أثناء الدمج):
1. `Docs/GapAnalysis/Cloud_Consolidation_Analysis.md` — التقرير المرجعي الأعلى موثوقية:
   - Drift Report: 8 فجوات جديدة (N-01..N-08) + 4 تصحيحات وصفية (C-01..C-04) + 5 بنود مؤكَّدة (D-01..D-05).
   - تقرير التداخلات: 11 مكوّناً مشتركاً (T-01..T-11) + 6 تعارضات (X-01..X-06).
   - ترتيب تنفيذي جاهز عبر 10 مستويات (0-9).
2. `Docs/GapAnalysis/F1_F7_Gap_Analysis_and_Implementation_Plan.md` — 15 Part لـ F1 + 9 Parts لـ F7.
3. `Docs/GapAnalysis/F2_F8_Gap_Analysis_and_Implementation_Plan.md` — 7 Parts لـ F2 + 8 Parts لـ F8.
4. `Docs/GapAnalysis/F3_F4_Gap_Analysis_and_Implementation_Plan.md` — 12 Parts لـ F3 + 12 Parts لـ F4.
5. `Docs/GapAnalysis/F5_F6_Gap_Analysis_and_Implementation_Plan.md` — 11 Parts لـ F5 + 10 Parts لـ F6.
6. `Docs/history.md` — الحالة الفعلية الحالية للكود (Baseline).
7. `Docs/analysis_and_plan_v3.md` — القرارات الـ17 الملزمة (غير قابلة للتعديل).

**قواعد المعمارية الحاكمة لكل Part** (بلا استثناء):
- فصل صارم بين `Model` / `Service` / `ViewModel` / `View` / `DI`.
- الحقن عبر `Constructor` حصراً — لا `DbContext` مباشر في `ViewModel`.
- كل Part ينتهي ببناء نظيف: **0 errors / 0 warnings** قبل الانتقال للتالي.
- التزام حرفي بأسماء الحقول/الكيانات/الخدمات كما وردت في المصادر.
- أي تعارض تقني حقيقي غير محسوم يُسجَّل في `Clarification Points` — لا يُحسم اجتهاداً.

**بنية الوثيقة**: 10 أقسام مطابقة للمستويات (0-9) في التقرير المرجعي. كل فجوة جديدة (N-*) وتصحيح (C-*) مُدمَج مباشرة داخل الـ Part المعنية به (وليس كملاحق). المكوّنات المشتركة (T-01..T-11) تُبنى **مرة واحدة فقط** في المستوى 0، وكل Part لاحق يُشير إلى "استخدام" لا "إنشاء". التعارضات (X-01..X-06) طُبِّقت قراراتها كما وردت في التقرير المرجعي، وما لم يُحسم بوضوح نُقل إلى `Clarification Points`.

---

## 🗂️ Level 0 — البنى المشتركة التحضيرية (Foundation Layer)

> **قاعدة صارمة**: كل Part تحت هذا المستوى يُبنى مرة واحدة فقط، وتَستَهلكه كل Parts المستويات (1-9) لاحقاً بدلاً من إعادة إنشائه.

### Part L0-P1 — `EnumToArabicConverter` مشترك (يحلّ T-03)
**المسار**: `Converters/EnumToArabicConverter.cs` (جديد).

**المكوّن المشترك**: مصفوفة قيم عربية موحّدة لكل من:
- `Gender` → "ذكر" / "أنثى" (قرار 17: بلا `Both`/`Unknown`).
- `AgeUnit` → "يوم" / "شهر" / "سنة".
- `BillingSystem` → "فردي" / "معمل لمعمل" / "مجاناً".
- `TestStatus` → "جديد" / "مُدخل" / "مُراجَع" / "مطبوع" / "مُسلَّم" / "باقي حساب" / "مكتمل".
- `DeliveryFilterMode` / `PatientListFilter` (سيُنشآن في Parts لاحقة — يتم إدراج حالاتهما هنا لاحقاً بعد إنشاء الـ enums).

**التبعيات**: لا شيء.

**الناتج المتوقع**: `IValueConverter` واحد يخدم كل الـ ComboBoxes في F3 / F5 / F6 / F8 (يستهلكه T-03 المتكرر أربع مرات).

**اعتبارات الدمج المباشر**:
- يُعالج **D-3-05 / D-3-06** (`FilterMode` عربي vs switch إنجليزي في F3).
- يُعالج جزءاً من **N-08** (الحاجة لتغيير `FilterMode` من `string` إلى `enum`).
- يستهلكه Parts: L3-P1، L3-P2، L3-P3.

**بوابة البناء**: `dotnet build` → 0 errors / 0 warnings.

---

### Part L0-P2 — `BoolToRedBrushConverter` مشترك (يحلّ T-02)
**المسار**: `Converters/BoolToRedBrushConverter.cs` (جديد).

**المكوّن المشترك**: `IValueConverter` يُرجِع `Brushes.Red` عندما `IsImportant = true`، وإلا `DynamicResource MaterialDesignBody` (أو `Brushes.Black` كـ fallback).

**التبعيات**: لا شيء.

**الناتج المتوقع**: Converter وحيد يخدم:
- تلوين اسم المريض المهم في `TestResultsListView` (يستهلكه Part L5-P2).
- تلوين اسم المريض المهم في `DeliveryView` قائمة + منطقة تفاصيل (يستهلكه Part L5-P3).
- تلوين صف المريض المهم في `SearchView` عبر `DataTrigger` (يستهلكه Part L5-P4).

**اعتبارات الدمج المباشر**:
- يُغلق T-02 (التكرار الثلاثي بين F3-P2 + F5-C + F6-E).
- **C-01 من Drift Report**: يُبقي `LatinSymbolsPad` منفصلاً — لا علاقة له بهذا Part.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L0-P3 — تعميم `LatinSymbolsPad` عبر `AttachedProperty` + توسيع الرموز (يحلّ T-01)
**المسار**:
- `Views/Controls/LatinSymbolsPad.xaml.cs` (تعديل — يستبدل `TargetTextBox` الثابت).
- `Views/Controls/LatinSymbolsPad.xaml` (تعديل قائمة الرموز الافتراضية).
- `Helpers/LatinSymbolsPadAttach.cs` (جديد — AttachedProperty).

**المكوّن المشترك**: `LatinSymbolsPad` واحد يستمع لحدث `PreviewGotKeyboardFocus` على النافذة الأم لتتبع آخر `TextBox` مُركَّز عليه، ثم يُدرج الرمز في `Keyboard.FocusedElement` عند النقر.

**دمج فجوات مباشرة داخل هذا الـ Part**:
- **F7-A.5 (خطة F1_F7)**: تعميم اللوحة على كل الحقول عبر AttachedProperty.
- **F8-Fix.3 (خطة F2_F8)**: نفس المطلوب — ربطها بكل حقول النصوص في `NormalRangeView`.
- **F8-Fix.4 (خطة F2_F8)**: توسيع القائمة الافتراضية إلى: `¹ ² ³ ⁴ ⁵ ⁶ ⁷ ⁸ ⁹ ⁰ α β γ μ ± ≤ ≥ ° × ÷` (10 أسس + 8 رموز يونانية/رياضية + × ÷). قرار 14 يسمح بالتوسعة.

**التبعيات**: لا شيء.

**الناتج المتوقع**: أي `TextBox` في `NormalRangeView` أو `LabTestManagementView` يستقبل الرموز عند النقر عليها، دون الحاجة لربط `TargetTextBox` يدوياً.

**بوابة البناء**: 0 errors / 0 warnings + اختبار يدوي: التركيز على حقلين مختلفين → إدراج رمز في كل منهما.

---

### Part L0-P4 — `PatientDisplayInfo` record مشترك (يحلّ T-11 جزئياً)
**المسار**: `Models/DTOs/PatientDisplayInfo.cs` (جديد).

**المكوّن المشترك**: `record PatientDisplayInfo(int PatientId, string FullName, bool IsImportant, string? LabId, string? FileCode, string? VisitCode, Gender Gender, decimal AgeValue, AgeUnit AgeUnit)` — DTO موحّد للحقول التي تظهر في:
- `PatientListItem` (F3).
- `DeliveryPatientRow` (F5).
- `PatientSearchRow` (F6).

**التبعيات**: `Models/Domain/Enums/Gender.cs`, `AgeUnit.cs` (موجودة).

**الناتج المتوقع**: كل من `PatientListItem` / `DeliveryPatientRow` / `PatientSearchRow` يحتوي `PatientDisplayInfo Display` بدلاً من تكرار الحقول التسعة. **ملاحظة**: `AgeValue` من نوع `decimal` هنا — يفترض تنفيذ Part L1-P1 قبله (تحويل `Patient.AgeValue` إلى decimal). عدم تنفيذ L1-P1 أولاً سيبقي `PatientDisplayInfo.AgeValue` كـ `decimal` لكن مصدر البيانات لا يزال `int`؛ يُطلب تنفيذ L1-P1 قبله ضمن نفس الجلسة.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L0-P5 — `AttachedBehaviors` لـ `DoubleClickCommand` و `GlobalShortcuts` (يحلّ T-09, T-10)
**المسار**:
- `Helpers/Interactions/DoubleClickBehavior.cs` (جديد).
- `Helpers/Interactions/GlobalShortcuts.cs` (جديد).

**المكوّن المشترك**:
1. **`DoubleClickBehavior`** — `AttachedProperty` `Interactions.DoubleClickCommand="{Binding SomeCommand}"` يُرفَق على `DataGrid`/`ListBox`/`ListBoxItem` ويُنفِّذ الأمر عند MouseDoubleClick.
2. **`GlobalShortcuts`** — `AttachedProperty` `GlobalShortcuts.RegisterOn="True"` على `UserControl` يُضيف `KeyBindings` لكل من F2/F3/F4/F5/F6/F7/F8/F9/F12/Esc عبر البحث عن أوامر VM قياسية الأسماء (`OpenPatientDataCommand`, `OpenSearchCommand`, `OpenTestResultsListCommand`, `RefreshCommand`, `OpenDeliveryCommand`, `OpenExternalSpecimensCommand`, `ToggleReviewedCommand`, `ToggleEnteredCommand`/`ToggleFinishedCommand`, `TogglePrintedCommand`, `CloseCommand`).

**التبعيات**: لا شيء.

**الناتج المتوقع**:
- **T-10** (التكرار الثلاثي لـ MouseDoubleClick في F1 / F3 / F6) يُحلّ عبر `DoubleClickBehavior`.
- **T-09** (التكرار الرباعي لـ KeyBindings في F1 / F3 / F5 / F6) يُحلّ عبر `GlobalShortcuts`.
- Parts المستوى 8 تُختصر إلى Part واحد يفعِّل `GlobalShortcuts.RegisterOn` على كل UserControl معنيّ.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L0-P6 — تعريف `enums` جديدة مركزياً (يخدم T-03 وX-03)
**المسار**:
- `Models/Domain/Enums/PatientListFilter.cs` (جديد): `{ All, Unwritten, Unreviewed, Unprinted, Important, Individual, LabToLab, Referral }`.
- `Models/Domain/Enums/DeliveryFilterMode.cs` (جديد): `{ Undelivered, All, LabToLab, Individual, Important, CurrentUser }`.
- `Models/Domain/Enums/TestListMode.cs` (جديد): `{ Routine, All, Groups, CustomGroups }`.

**دمج X-03 مباشرة**: enum لكل من F3 وF5 يبقى منفصلاً لأن مجموعتَي الفلاتر مختلفتان (F3 يشمل Unwritten/Unreviewed/Unprinted، F5 يشمل Undelivered/CurrentUser). تم اعتماد هذا القرار كما ورد في التقرير المرجعي — لا يُقترح توحيدهما.

**التبعيات**: L0-P1 (لضمان أن الـ Converter يعرف الحالات الجديدة قبل استهلاكها).

**الناتج المتوقع**: enums جاهزة للاستخدام في L3-P1 (F3-P3)، L3-P2 (F5-A)، L3-P3 (F6-A)، L5-P8 (F1-A.5).

**بوابة البناء**: 0 errors / 0 warnings.

---

## 🗂️ Level 1 — كيانات جديدة + Migrations (Data Layer)

> يجب أن تسبق أي Service يستخدم الحقل الجديد.

### Part L1-P1 — Migration `AlterPatientAgeToDecimal` (يحلّ D-02 و F1-A.1)
**المسار**:
- `Models/Domain/Patient.cs` — تغيير `int AgeValue` → `decimal AgeValue`.
- `ViewModels/Pages/PatientEntryViewModel.cs` — نفس التغيير في الخاصية.
- `Models/Validation/PatientValidator.cs` — تحديث القواعد لتقبل عشري مع حدود النطاق (1-29 يوم / 1-11 شهر / 0-120 سنة).
- `Migrations/AlterPatientAgeToDecimal.cs` (جديد).

**فحص فعلي**: تم التأكد عند الكوميت `45f04ac` من أن `Patient.AgeValue` لا يزال `int` (السطر 21) — الفجوة قائمة.

**التبعيات**: L0-P4 (يعتمد `PatientDisplayInfo.AgeValue` على النوع `decimal`).

**الناتج المتوقع**: قبول قيمة "2.5 سنة" لطفل، مع Migration نظيفة.

**بوابة البناء**: 0 errors / 0 warnings + `dotnet ef database update` ينجح.

---

### Part L1-P2 — Migration `AddLabTestSpecimensAndLabelName` (F7-A.2 + F7-A.3)
**المسار**:
- `Models/Domain/LabTestSpecimen.cs` (جديد): `(Id, LabTestId, SpecimenTypeId, TubeOrder [1|2|3], LabelName)`.
- `Models/Domain/LabTest.cs` — إضافة `LabelName` (nullable string) + `ICollection<LabTestSpecimen> Specimens`.
- `Data/NewLabDbContext.cs` — إضافة `DbSet<LabTestSpecimen>` + Fluent API.
- `Migrations/AddLabTestSpecimensAndLabelName.cs` (جديد).

**التبعيات**: لا شيء.

**الناتج المتوقع**: دعم تحاليل بأنبوبين أو ثلاثة (مثال Creatinine Clearance = مصل + بول 24س).

**بوابة البناء**: 0 errors / 0 warnings + Migration نظيفة.

---

### Part L1-P3 — Migration `AddPrintAndSensitiveToNormalRanges` (F8-Fix.2)
**المسار**:
- `Models/Domain/NormalRange.cs` — إضافة `bool PrintInReport`, `bool ShowAsSensitive` (أو خاصية مركبة).
- `Models/Validation/NormalRangeValidator.cs` — قواعد لا تتغير جوهرياً.
- `Migrations/AddPrintAndSensitiveToNormalRanges.cs` (جديد).

**دمج C-04 مباشرة**: التقرير المرجعي أشار إلى أن تفسير `P & S` يحتاج قرار مالك جديد (ليس ضمن الـ17). **نقطة توضيح**: يُنشئ الحقل بالتفسير الاحتمالي الأرجح (`P = Print in report`, `S = Sensitive/Show as critical`)، لكن يُطلب تأكيد من المالك قبل تنفيذ أي منطق تجاري يعتمد على الحقلين — انظر `CL-01`.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L1-P4 — Migration `AddReceiptSettings` (F1-A.9 — جزء بيانات)
**المسار**:
- `Models/Domain/ReceiptSettings.cs` (جديد): `(Id, AutoPrintAfterSave, ShowTestsDetails)`.
- `Data/NewLabDbContext.cs` — إضافة `DbSet<ReceiptSettings>` + Seed لصف افتراضي.
- `Migrations/AddReceiptSettings.cs` (جديد).

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L1-P5 — Migration `AddPatientTestReviewedPrinted` (F4-P4 — جزء بيانات)
**المسار**:
- `Models/Domain/TestResult.cs` — إضافة `bool IsReviewed`, `bool IsPrinted` على مستوى العنصر (بجانب `PatientTest.IsReviewed`/`IsPrinted` الموجودَين على مستوى التحليل الكامل).
- `Migrations/AddTestResultReviewedPrinted.cs` (جديد).

**نقطة توضيح**: التقرير المرجعي لم يحسم صراحة ما إذا كانت هذه Checkboxes تُخزَّن على مستوى العنصر (`TestResult`) أم مستوى التحليل (`PatientTest`). الخطة الأصلية F3_F4 تشير إلى Checkbox "لكل عنصر"، ما يفرض التخزين على `TestResult` — انظر `CL-02`.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

## 🗂️ Level 2 — خدمات جديدة/موسَّعة (Service Layer)

### Part L2-P1 — `IReceiptPdfService` جديد (F1-A.9)
**المسار**:
- `Services/Interfaces/IReceiptPdfService.cs` (جديد).
- `Services/Implementations/ReceiptPdfService.cs` (جديد — QuestPDF: header + قائمة تحاليل + Total/Discount/Paid/Remaining + خيار `ShowTestsDetails`).
- `App.xaml.cs` — تسجيل `AddScoped<IReceiptPdfService, ReceiptPdfService>()`.

**دمج X-01 مباشرة**: التقرير المرجعي أوصى بإبقاء الخدمتين منفصلتين (`IReceiptPdfService` أمامي بسيط، `IReportPdfGenerator` طبي تفصيلي) لأن الإيصال أبسط بكثير من تقرير طبي — لا تُوحَّد.

**التبعيات**: L1-P4.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L2-P2 — توسيع `IReportPdfGenerator` بـ 5 قوالب (F3-P10)
**المسار**:
- `Services/Interfaces/IReportPdfGenerator.cs` — إضافة: `GenerateAggregateAsync(patientId, date)`, `GenerateWorksheetAsync(patientId)`, `GenerateEnvelopeAsync(patientId)`, `GenerateHistoryAsync(patientId)`, `GenerateBlankAsync(patientId)`.
- `Services/Implementations/ReportPdfGenerator.cs` — تنفيذ القوالب الخمسة.

**دمج X-04 مباشرة**: يعتمد هذا Part منطقياً على Part L2-P1 (لضمان استقرار قرار الفصل بين خدمتَي PDF قبل توسيع الطبية). ترتيب التنفيذ في هذا المستوى: L2-P1 ثم L2-P2.

**التبعيات**: L2-P1 (قرار تصميم).

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L2-P3 — توسيع `ITestResultsListService` بـ `SearchByNameAsync` (F3-P9)
**المسار**:
- `Services/Interfaces/ITestResultsListService.cs` — إضافة `SearchByNameAsync(string partialName, DateTime forDate)`.
- `Services/Implementations/TestResultsListService.cs` — تنفيذ: `Contains` على `Patient.FullName` مع `VisitDate == forDate`.

**التبعيات**: لا شيء (يستخدم البنية الحالية).

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L2-P4 — توسيع `IDeliveryService` بـ `ClearAccountAsync` + فلتر `OnlyCurrentUser` (F5-B + F5-F)
**المسار**:
- `Services/Interfaces/IDeliveryService.cs` — إضافة `ClearAccountAsync(patientId, userId)` + إضافة حقل `OnlyCurrentUser` أو `int? UserId` إلى `DeliveryFilter`.
- `Services/Implementations/DeliveryService.cs` — تنفيذ:
  - `ClearAccountAsync`: `patient.PaidAmount = patient.TotalAmount` + `PaymentTransaction{Type=Payment, Amount=remaining}` + `AuditLog{Action="ClearAccount"}` داخل Transaction.
  - إضافة `q.Where(v => v.Patient.CreatedByUserId == currentUserId)` عند تفعيل `OnlyCurrentUser`.

**التبعيات**: L0-P6 (enum `DeliveryFilterMode` يحتوي على `CurrentUser`).

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L2-P5 — توسيع `IPatientSearchService` بـ `GetOpenAccountsCountAsync` (F6-H)
**المسار**:
- `Services/Interfaces/IPatientSearchService.cs` — إضافة `Task<int> GetOpenAccountsCountAsync(SearchCriteria criteria)`.
- `Services/Implementations/PatientSearchService.cs` — تنفيذ: `_context.Patients.CountAsync(p => p.PaidAmount < p.TotalAmount /* + criteria */)`.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L2-P6 — استخدام `AutoCalculationService` من `TestResultEntryViewModel` (F4-P8)
**المسار**:
- `ViewModels/Pages/TestResultEntryViewModel.cs` — إضافة استدعاءات لدوال الخدمة (`CalculateHctAsync`, `CalculateHgbPercentAsync`, `CalculateINRAsync`, `CalculatePTTRatioAsync`) عند تغيّر قيم `Hgb`/`PT`/`PTT`.

**دمج N-02 مباشرة داخل هذا الـ Part**: كشف Drift Report أن `IAutoCalculationService` **يُحقن ويُستدعى فقط لجلب الثوابت** (`GetConstantsAsync` — سطر 89 من `TestResultEntryViewModel.cs`) دون أي استدعاء لدوال الحساب. تم التأكيد بفحص مباشر على الكوميت `45f04ac`. البنية التحتية (Migration + DI + Seed) موجودة بالكامل، فحساسية الاختراق قصيرة جداً: يكفي إضافة استدعاءات في `OnValueChanged` للصفوف المُسمّاة `Hgb` / `PT` / `PTT`، مع حماية `SkipIfRowMissing`.

**التبعيات**: L0-P6 (لا اعتماد مباشر — لكن يفضل بعد استقرار enums).

**بوابة البناء**: 0 errors / 0 warnings + اختبار: إدخال Hgb=15 لذكر بالغ → صف Hct = 49.5.

---

## 🗂️ Level 3 — إصلاح Bindings و Bugs المُغلَقة على الـ Enums

> يعتمد كامل هذا المستوى على `EnumToArabicConverter` في L0-P1 و enums الجديدة في L0-P6.

### Part L3-P1 — إصلاح Binding فلاتر F3 (F3-P3 + جزء N-08)
**المسار**:
- `Views/Pages/TestResultsListView.xaml` — تغيير ComboBox الفلاتر إلى `ItemsSource="{Binding EnumValues[PatientListFilter]}"` + `SelectedValue="{Binding FilterMode}"` + `EnumToArabicConverter` (من L0-P1).
- `ViewModels/Pages/TestResultsListViewModel.cs` — تغيير `FilterMode` من `string` إلى `PatientListFilter enum` (من L0-P6).
- `Services/Implementations/TestResultsListService.cs` — تغيير `switch (filterMode)` من `case "Unwritten"` إلى `case PatientListFilter.Unwritten`.

**دمج N-08 مباشرة**: أشار Drift Report إلى أن تغيير `FilterMode` من `string` إلى `enum` له تأثير مباشر على `TestResultsListViewModel` واستدعاءات `LoadPatientsAsync` وتبعات على `BuildFilter` — كل هذه التغييرات مُدمَجة في هذا Part كوحدة واحدة (لا تُقسَّم).

**فحص فعلي**: تم التأكيد أن `switch (filterMode)` في `TestResultsListService.cs:61-72` لا يزال يستخدم قيماً نصية إنجليزية بينما ComboBox عربي — الفجوة قائمة كما وُصفت.

**التبعيات**: L0-P1، L0-P6.

**بوابة البناء**: 0 errors / 0 warnings + اختبار: اختيار "غير مكتوب" يستدعي `PatientListFilter.Unwritten` فعلياً.

---

### Part L3-P2 — إصلاح Binding فلاتر F5 (F5-A)
**المسار**:
- `Views/Pages/DeliveryView.xaml` — نفس النمط: `ItemsSource="{Binding EnumValues[DeliveryFilterMode]}"` + `SelectedValue="{Binding FilterMode}"` + `EnumToArabicConverter`.
- `ViewModels/Pages/DeliveryViewModel.cs` — تحويل `FilterMode` إلى `DeliveryFilterMode enum`.
- `Services/Implementations/DeliveryService.cs` — تحديث `BuildFilter()` لاستهلاك الـ enum.

**التبعيات**: L0-P1، L0-P6، L2-P4 (اضافة `CurrentUser` تعتمد على الحقل الجديد في `DeliveryFilter`).

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L3-P3 — إصلاح Bindings الـ Enums في F6 (F6-A)
**المسار**:
- `Views/Pages/SearchView.xaml` — إصلاح ComboBoxes الثلاثة (`SelectedGender`, `SelectedAgeUnit`, `SelectedSource`) لاستخدام `SelectedValue` + `SelectedValuePath="Tag"` أو `ItemsSource` من enum + Converter.
- `ViewModels/Pages/SearchViewModel.cs` — إضافة قوائم `AvailableGenders`, `AvailableAgeUnits` كخصائص (بدلاً من الاعتماد على code-behind).

**دمج ضمني لتنظيف مشابه في F8-Fix.8**: `NormalRangeView.xaml.cs` يُعبِّئ `GenderCombo.ItemsSource` و`AgeUnitCombo.ItemsSource` من code-behind — يُستبدل بـ Binding من الـ VM بنفس الأسلوب. يُنفَّذ هذا التنظيف في Part منفصل عند F8 (L4-P3) لتجنب توسيع نطاق L3-P3.

**التبعيات**: L0-P1، L0-P6.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L3-P4 — إصلاح كشف بادئة الكود `trimmed[0]` (F5-G + T-05)
**المسار**:
- `Services/Implementations/DeliveryService.cs` — تعديل `SearchByCodeAsync`: `trimmed[1]` → `trimmed[0]`.
- `Helpers/BarcodeCodeTypeDetector.cs` (جديد — يستخلص منطق الكشف كدالة مشتركة يستهلكها F5 وF6).

**دمج T-05 مباشرة**: بدلاً من تكرار الكشف في `DeliveryService` و`PatientSearchService`، تُستخرَج الدالة `DetectCodeType(string code) → CodeType` مرة واحدة هنا كمكوّن مشترك.

**فحص فعلي**: تم التأكيد أن `DeliveryService.cs:249` يقرأ `trimmed[1]` — الفجوة قائمة كما وُصفت في D-05.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings + اختبار كود بادئته "1..." → `VisitCode`.

---

### Part L3-P5 — إصلاح KeyBinding `Escape` في `BarcodeView` (F2-Fix.4)
**المسار**:
- `Views/Windows/BarcodeView.xaml` — إزالة `<KeyBinding Key="Escape" Command="{Binding PrintAllCommand}" />` واستبدالها بأمر إغلاق النافذة (إما `ApplicationCommands.Close` أو `CloseCommand` جديد في VM يستدعي `IDialogService.CloseCurrentDialog()`).

**فحص فعلي**: تم التأكيد أن السطر 12 من `BarcodeView.xaml` لا يزال يحتوي `<KeyBinding Key="Escape" Command="{Binding PrintAllCommand}" />` — الفجوة قائمة (D-03 مؤكَّد).

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings + اختبار: Escape → إغلاق (لا طباعة).

---

## 🗂️ Level 4 — Toggles حقيقية + Admin gates تفعيلية

### Part L4-P1 — Toggle حقيقي لـ F8/F9/F12 في `TestResultsListService` (F3-P7)
**المسار**:
- `Services/Implementations/TestResultsListService.cs`:
  - `ToggleReviewedAsync`: عند `IsReviewed = true → false`، إعادة `Status` إلى `Entered` (أو `New` إن لم يُكتب فعلياً).
  - `ToggleEnteredAsync`: جعله Toggle حقيقي — إن كان `Status == Entered` بلا نتائج فعلية، أعده لـ `New`. وعند F9 لتحاليل خارجية، يعيّن معاً `IsReviewed=true, IsPrinted=true` كسيناريو "تمت" المرجعي.
  - `TogglePrintedAsync`: عند `IsPrinted = true → false`، إعادة `Status` إلى `Reviewed` (أو الحالة السابقة الصحيحة).

**دمج D-3-02 / D-3-03 / D-3-04 مباشرة**: كل السلوكيات one-way الحالية تُعاد صياغتها كـ Toggle حقيقي في هذا Part الوحيد.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings + اختبار: F8 مرتين → الحالة تعود كما كانت.

---

### Part L4-P2 — Toggle F8 حقيقي + `HistoryButtonLabel` في F4 (F4-P10)
**المسار**:
- `Services/Implementations/TestResultEntryService.cs` — تعديل `MarkReviewedAsync(int patientTestId, bool state)` ليقبل الحالة الجديدة.
- `ViewModels/Pages/TestResultEntryViewModel.cs`:
  - `ToggleReviewedAsync` = `await MarkReviewedAsync(_patientTestId, !IsReviewed); IsReviewed = !IsReviewed;`.
  - `HistoryButtonLabel`: حساب `count = TestResults.Count(...)` في `LoadForPatientTestAsync` → `HistoryButtonLabel = count > 0 ? $"تاريخ مرضي ({count})" : "تاريخ مرضي"`.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L4-P3 — تنفيذ فعلي لـ `AuditLog` + Financial Tracking (F3-P11) + تنظيف VM Binding (F8-Fix.8)
**المسار**:
- `Views/Windows/AuditLogView.xaml` (+ `.xaml.cs`) — DataGrid: Timestamp / UserName / Action / Details.
- `ViewModels/Windows/AuditLogViewModel.cs` (جديد) — يحمّل `PatientAudits` + `TestAudits` عبر `GetAuditForPatientAsync`/`GetAuditForTestAsync` الموجودتين.
- `ViewModels/Pages/TestResultsListViewModel.cs` — استبدال Stubs في `ShowAudit`/`ShowFinancialTracking` بفتح `AuditLogView` كـ Dialog.
- `Views/Windows/NormalRangeView.xaml.cs` — إزالة تعيين `GenderCombo.ItemsSource` و`AgeUnitCombo.ItemsSource` من code-behind.
- `Views/Windows/NormalRangeView.xaml` — تفعيل `ItemsSource="{Binding AvailableGenders}"` + `ItemsSource="{Binding AvailableAgeUnits}"` مباشرة في XAML.

**دمج مباشر لعدة تنظيفات**: كلا التغييرين (تنفيذ AuditLog + تنظيف Binding في NormalRange) يعتمدان فقط على قرار Admin gates + XAML/Code-behind cleanup ولا تعارض بينهما، فيُجمعان هنا لضمان أن نوافذ الـ Dialog كلها متسقة في نمط MVVM.

**التبعيات**: لا شيء (`CanExecute = IsAdmin` موجود مسبقاً).

**بوابة البناء**: 0 errors / 0 warnings.

---

## 🗂️ Level 5 — UI Enrichment (يعتمد على T-02 + T-11)

### Part L5-P1 — إثراء `PatientListItem` ببيانات المريض التفصيلية (F3-P1 + F3-P12)
**المسار**:
- `Services/Interfaces/ITestResultsListService.cs` — توسيع `PatientListItem` باستهلاك `PatientDisplayInfo` (من L0-P4) + `ActualVisitCount` + `Notes`.
- `Services/Implementations/TestResultsListService.cs` — تعبئة `ActualVisitCount = _context.PatientVisits.CountAsync(v => v.PatientId == patientId && v.Patient.LabId != null)` واستبدال `VisitCount = tests.Count` (وهو خطأ D-3-07) بـ `ActualVisitCount`.

**دمج D-3-07 مباشرة**: التصحيح جزء لا يتجزأ من إثراء `PatientListItem` — يُنفَّذان معاً.

**التبعيات**: L0-P4.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L5-P2 — عرض بيانات F3 + تلوين أحمر + خانة خضراء + Click-to-Copy (F3-P2)
**المسار**:
- `Views/Pages/TestResultsListView.xaml`:
  - إضافة `TextBlock` لكل من: Gender، AgeValue+AgeUnit، LabId، FileCode، VisitCode في StackPanel "Patient info".
  - `Foreground="{Binding IsImportant, Converter={StaticResource BoolToRedBrushConverter}}"` على اسم المريض (يستهلك L0-P2).
  - `Border Background="Green" Padding="4"` حول عدد الزيارات + `ActualVisitCount`.
  - `MouseLeftButtonDown` handlers على خانات `LabId` و`VisitCode` تنسخ للحافظة عبر `Clipboard.SetText`.
- **يُستهلَك في هذه العملية** Behavior من L0-P5 عند الحاجة لـ AttachedProperty بدل code-behind.

**التبعيات**: L0-P2، L0-P5، L5-P1.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L5-P3 — تلوين F5 + عرض VisitCode/LabId + Click-to-Copy (F5-C + F5-D)
**المسار**:
- `Services/Interfaces/IDeliveryService.cs` — تعديل `DeliveryPatientRow` لاستهلاك `PatientDisplayInfo` (من L0-P4).
- `Services/Implementations/DeliveryService.cs` — تعبئة الحقول في `FilterAsync` و`BuildDeliveryRowAsync`.
- `Views/Pages/DeliveryView.xaml`:
  - إزالة `FontWeight="{Binding IsImportant, Converter={x:Static converters:InverseBoolToVisibilityConverter.Instance}}"` (الخطأ المنطقي المُوثَّق).
  - `Foreground="{Binding IsImportant, Converter={StaticResource BoolToRedBrushConverter}}"` على اسم المريض في القالب + في القسم 2.
  - عرض `VisitCode`، `LabId`، `Gender`، `AgeValue+AgeUnit` في صف القائمة وفي بيانات المريض المحدد.
  - `MouseLeftButtonDown` على خانات الكود ينسخ عبر `Clipboard.SetText`.
- `ViewModels/Pages/DeliveryViewModel.cs` — `RelayCommand CopyCode(string code)`.

**التبعيات**: L0-P2، L0-P4، L0-P5.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L5-P4 — ComboBox جهة الإحالة + تلوين صف F6 + أعمدة ID/Unit (F6-B + F6-D + F6-E)
**المسار**:
- `Views/Pages/SearchView.xaml`:
  - إضافة `<ComboBox ItemsSource="{Binding AvailableReferrals}" SelectedItem="{Binding SelectedReferral}" DisplayMemberPath="Name" materialDesign:HintAssist.Hint="جهة الإحالة"/>`.
  - إضافة أعمدة `<DataGridTextColumn Header="ID" Binding="{Binding PatientId}"/>` و`<DataGridTextColumn Header="الوحدة" Binding="{Binding AgeUnit, Converter={StaticResource EnumToArabicConverter}}"/>`.
  - `DataGrid.RowStyle` مع `DataTrigger Binding="{Binding IsImportant}" Value="True" → Setter Foreground=Red` (يستهلك L0-P2 عبر Converter مباشر أو trigger).

**دمج N-04 مباشرة داخل هذا Part**: أشار Drift Report إلى أن `AvailableReferrals` مُحمَّلة فعلياً في `SearchViewModel.cs:44, 178-181` **بلا استخدام في XAML**، وأن `LoadReferralsAsync` تعمل على كل فتح للنافذة بلا هدف (تكلفة I/O غير مبررة). تم التأكيد بفحص مباشر عند الكوميت `45f04ac`. عند إضافة ComboBox في هذا Part، `LoadReferralsAsync` تصبح مُبرَّرة تلقائياً — لا داعي لتأجيلها أو حذفها.

**التبعيات**: L0-P1، L0-P2، L0-P4، L0-P6.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L5-P5 — إضافة Notes + SpecimenType ComboBox + Title auto (F1-A.2 + F1-A.3 + F1-A.4)
**المسار**:
- `Views/Pages/PatientEntryView.xaml`:
  - TextBox متعدد الأسطر مربوط بـ `Notes` أسفل قسم "المعلومات الطبية".
  - استبدال TextBox رقمي لـ `ExternalSpecimenTypeId` بـ `<ComboBox ItemsSource="{Binding AvailableSpecimenTypes}" DisplayMemberPath="Name" SelectedValuePath="Id" SelectedValue="{Binding ExternalSpecimenTypeId}"/>`.
- `ViewModels/Pages/PatientEntryViewModel.cs`:
  - `ObservableCollection<SpecimenType> AvailableSpecimenTypes` + `LoadSpecimenTypesAsync`.
  - `partial void OnGenderChanged(Gender value)` يعيّن `Title = "السيد"` أو `"السيدة"` (مع السماح بالتعديل اليدوي).
- `Data/NewLabDbContext.cs` — Seed لـ `SpecimenTypes` (Blood/Urine/Stool/Semen/CSF/Other) إن لم يكن موجوداً.

**دمج X-06 مباشرة**: قرار 17 (Male/Female فقط) يمنع خيار "Unknown"، فمنطق `OnGenderChanged` مبسَّط لعنصرين لا أكثر — لا Open Question هنا.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L5-P6 — قائمة التحاليل الديناميكية + بحث فلترة (F1-A.5)
**المسار**:
- `ViewModels/Pages/PatientEntryViewModel.cs`:
  - `SelectedTestListMode : TestListMode` (من L0-P6).
  - `LoadAvailableTestsAsync` يُعاد كتابته حسب المود (Routine / All / Groups / CustomGroups).
  - `AvailableTests` يُصفَّى عبر `ICollectionView` أو `partial void OnTestListFilterChanged(string value)` يستخدم `Contains` على `TestName` و`TestCode`.
- `Views/Pages/PatientEntryView.xaml` — ComboBox أعلى قائمة التحاليل يربط `SelectedTestListMode`.

**التبعيات**: L0-P6.

**بوابة البناء**: 0 errors / 0 warnings + اختبار: تغيير المود يعيد تحميل القائمة؛ الكتابة في مربع البحث تفلترها فعلياً.

---

### Part L5-P7 — Double-Click للإضافة/الحذف + Double-Click في F3 (F1-A.6 + F3-P5 + F6-F)
**المسار**:
- `Views/Pages/PatientEntryView.xaml` — تفعيل `Interactions.DoubleClickCommand="{Binding AddSelectedTestCommand}"` على ListBox `AvailableTests` و`Interactions.DoubleClickCommand="{Binding RemoveTestCommand}"` على DataGrid المختارة.
- `Views/Pages/TestResultsListView.xaml` — تفعيل `Interactions.DoubleClickCommand="{Binding OpenTestEntryCommand}"` على ListBox `PatientTests`.
- `Views/Pages/SearchView.xaml` — تفعيل `Interactions.DoubleClickCommand="{Binding OpenResultsCommand}"` على DataGrid النتائج.

**دمج T-10 مباشرة**: التكرار الثلاثي في F1 / F3 / F6 يُحلّ بـ AttachedBehavior من L0-P5 — لا `MouseDoubleClick` handlers مستقلة في كل ملف.

**دمج N-4-01 (خطة F3_F4 — بند 1)**: نتيجة تفعيل Double-Click في F3 لفتح F4، تصبح النافذة تُفتح بـ Double-Click وليس فقط بـ Enter كما كانت.

**التبعيات**: L0-P5.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L5-P8 — علامات الحالة الأربع المستقلة أمام كل تحليل (F3-P6)
**المسار**:
- `Views/Pages/TestResultsListView.xaml` — تعديل `DataTemplate` لعناصر `PatientTests` ليعرض 4 `PackIcon`s: Entered / Reviewed / Printed / Delivered — كل واحدة ملوّنة أخضر عند التفعيل، رمادي عند الإيقاف.
- `Converters/BooleanToActiveColorConverter.cs` (جديد) — يُرجِع Green عند true، Gray عند false.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L5-P9 — تصنيف الملصقات حسب نوع العينة + سلة محذوفات + Drag على مستوى التحليل (F2-Fix.1 + F2-Fix.2 + F2-Fix.3)
**المسار**:
- `Services/Implementations/BarcodeService.cs` — إعادة كتابة `GetLabelsForPatientAsync` لتجميع `PatientTests` حسب `LabTest.DefaultSpecimenTypeId` وإنتاج `BarcodeLabel` واحد لكل نوع عينة (Serum / Urine / EDTA / ...) + ملصق "Other" للتحاليل بلا SpecimenType.
- `Views/Windows/BarcodeView.xaml`:
  - إضافة `Border` سلة محذوفات (`PackIcon Kind="Delete"` + `AllowDrop="True"`).
  - `PreviewMouseLeftButtonDown` على TextBlock كل تحليل داخل `ItemTemplate` لتفعيل Drag.
- `Views/Windows/BarcodeView.xaml.cs`:
  - `TrashBin_Drop`: يميّز بين إفلات `BarcodeLabel` (حذف ملصق) و`string` (حذف تحليل).
  - `Label_Drop`: يميّز بين ترتيب (`BarcodeLabel`) ونقل تحليل (`string`).
- `ViewModels/Pages/BarcodeViewModel.cs` — `MoveTestBetweenLabelsCommand(source, target, testName)` + توسيع `RemoveLabelCommand` + `RemoveTestFromLabelCommand`.

**دمج مباشر**: الفجوات الثلاث (تصنيف، سلة، drag) مرتبطة بنفس Layout XAML ونفس الحقل `BarcodeLabel.SpecimenTypeId` — تُنفَّذ معاً لتجنب Layout thrashing.

**فحص فعلي**: تم التأكيد أن `BarcodeService.GetLabelsForPatientAsync` لا يزال يُرجِع ملصقاً واحداً باسم "Default".

**التبعيات**: L1-P2 (Tubes 1/2/3 يوفر مصدر البيانات الأدق للتصنيف، لكن ليس ضرورياً — يمكن الاستغناء عنه في نسخة أولى تعتمد على `DefaultSpecimenTypeId` فقط).

**بوابة البناء**: 0 errors / 0 warnings + اختبار: مريض بتحليلين من عينات مختلفة → ملصقان منفصلان.

---

### Part L5-P10 — DataGrid بـ 11 عموداً + `P & S` في `NormalRangeView` (F8-Fix.1 + F8-Fix.2 UI-side)
**المسار**:
- `Views/Windows/NormalRangeView.xaml`:
  - استبدال `ListBox Ranges` بـ `DataGrid` ذي 11 عموداً بالترتيب: T.ID / Test Name / Sex / From / To / Age unit / Normal Range / Low limit / High limit / Test unit / P & S.
  - توسيع عرض العمود الأول من 220px إلى ~450px.
  - إضافة `CheckBox` في الفورم بعد قسم Critical للحقل `PrintAndSensitive`.
- `ViewModels/Pages/NormalRangeViewModel.cs` — `FormPrintAndSensitive` + Load/Build methods.

**التبعيات**: L1-P3 (Migration + Entity property).

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L5-P11 — DataGrid متعدد الأعمدة + Tubes + LabelName + PromptQuestion + Back button في F7 (F7-A.1 + F7-A.2 + F7-A.3 + F7-A.6 + F7-A.7)
**المسار**:
- `Views/Pages/LabTestManagementView.xaml`:
  - استبدال `<ListBox>` بـ `<DataGrid AutoGenerateColumns="False">` مع 10 أعمدة: Id / ArrangeNumber / TestGroup.Name / TestName / PatientPrice / LabToLabPrice / ExternalReferral.Name / ExternalCost / DefaultSpecimenType.Name / IsRoutine (CheckBox column).
  - قسم "الحاويات (Tubes)" — DataGrid صغير يعرض `Specimens` مع 3 صفوف Add/Remove.
  - حقل TextBox جديد لـ `LabelName`.
  - زر "قائمة التحاليل" في شريط الأوامر السفلي يُنفِّذ `ClearForm() + SearchAsync`.
- `Services/Implementations/LabTestService.cs` — CRUD لـ `LabTestSpecimen`.
- `ViewModels/Pages/LabTestManagementViewModel.cs`:
  - `ObservableCollection<LabTestSpecimen> TestTubes`.
  - أوامر `AddTubeCommand`, `RemoveTubeCommand`.
  - `FormLabelName`.
- `ViewModels/Pages/PatientEntryViewModel.cs` — في `AddSelectedTest`، إذا `SelectedAvailableTest.PromptQuestion` غير فارغ، اعرضه عبر `IDialogService.ShowMessage("تنبيه", question)` (F7-A.7).

**دمج X-05 مباشرة**: التقرير المرجعي أشار إلى أن `F8-Fix.7` (خانة بحث ثالثة في F7 لعناصر التحليل الرئيسي) تُنفَّذ داخل نطاق F7. تُدرَج تحت Part `L5-P11` كإضافة صغيرة: خانة TextBox ثالثة تُفعِّل `LabTestService.SearchElementsAsync(prefix)` عند إدخال أول حرفين — يظهر عناصر `LabTestElement` الفرعية.

**التبعيات**: L1-P2.

**بوابة البناء**: 0 errors / 0 warnings.

---

## 🗂️ Level 6 — سلوكيات جديدة (Real-time evaluation + Auto-calc + Comments + Enter navigation)

### Part L6-P1 — تلوين real-time أثناء الكتابة (F4-P3)
**المسار**:
- `ViewModels/Pages/TestResultEntryViewModel.cs`:
  - `TestResultRow` يعرف delegate `_evaluator` يمرَّر من الـ ViewModel.
  - `partial void OnValueChanged(string value)` يجرِّب Parse لـ decimal → يستدعي `_entryService.EvaluateResultAsync(value, labTestId, patient)` → يعيّن `CellColor` (Transparent / #FFF9800 / #FFF44336) و`FlagText` من `NormalRangeEvaluation`.
- `Services/Interfaces/ITestResultEntryService.cs` — Wrap إضافي لـ `EvaluateResultAsync` بمعاملات `(value, labTestElementId, patient)`.

**دمج D-4-01 / D-4-02 مباشرة**: التقييم الحالي يعتمد على `CellColor` قديم مخزَّن، يُستبدل بتقييم لحظي حي.

**التبعيات**: لا شيء (يستخدم `EvaluateValueAsync` الموجود).

**بوابة البناء**: 0 errors / 0 warnings + اختبار: Hgb=20 لذكر بالغ → خلفية حمراء + FlagText="Critical High" فوراً.

---

### Part L6-P2 — تفعيل زر "Comment from Normal Range" + "Undo Comment" (F4-P6 + F4-P7)
**المسار**:
- `ViewModels/Pages/TestResultEntryViewModel.cs`:
  - `PickCommentFromNormalRangeCommand` (جديد): يحدّد الصف المفعّل → Parse قيمته → `GetMatchingRangeAsync` + `EvaluateValueAsync` → التقط `LowComment` / `HighComment` / `CriticalComment` وأضِفه لـ `Comment`.
  - `UndoLastCommentCommand` (جديد) + `Stack<string> _commentHistory`: كل `Pick*Command` يعمل `Push(Comment)` قبل التعديل؛ `Undo` يعمل `Pop`.
- `Views/Windows/TestResultEntryView.xaml`:
  - ربط الزر "Comment from Normal Range" (سطر 96) بـ `PickCommentFromNormalRangeCommand`.
  - ربط الزر "Undo Comment" (سطر 98) بـ `UndoLastCommentCommand` مع `CanExecute = _commentHistory.Any()`.

**دمج N-03 مباشرة داخل هذا Part**: كشف Drift Report أن الزرَّين `<Button Content="Comment from Normal Range" .../>` و`<Button Content="Undo Comment" .../>` (سطور 96-98 من `TestResultEntryView.xaml`) **موجودان بلا سمة `Command=""` نهائياً**، وأن الأمرَين المقابلَين `PickCommentFromNormalRangeCommand` و`UndoLastCommentCommand` **غير معرَّفَين أصلاً في الـ ViewModel**. بالتالي العمل مطلوب في **طبقتين لا طبقة واحدة**: (1) إنشاء الـ Commands في VM، (2) ربطهما بـ XAML — كلاهما مُدمَج داخل هذا Part الوحيد.

**فحص فعلي**: تم التأكيد على الكوميت `45f04ac` أن `Views/Windows/TestResultEntryView.xaml` سطر 96/98 يحتوي الزرَّين بلا Command، وأن الـ ViewModel لا يعرِّف الأمرَين.

**التبعيات**: لا شيء (يستخدم `EvaluateValueAsync` الموجود في `INormalRangeService`).

**بوابة البناء**: 0 errors / 0 warnings + اختبار: نتيجة Low → التقاط `LowComment` تلقائياً → Undo يعود.

---

### Part L6-P3 — تمييز عناصر البروفيل المختارة + Checkboxes للطباعة/المراجعة + NormalRangeText عمود (F4-P2 + F4-P4 + F4-P11)
**المسار**:
- `Services/Interfaces/ITestResultEntryService.cs` — إرجاع `SelectedElementIds` أو `IsSelectedForPatient` flag من `GetPatientTestWithProfileAsync`.
- `Services/Implementations/TestResultEntryService.cs`:
  - تحديد مصدر "العناصر المختارة" عبر وجود `TestResult` سابق أو `PatientTest.SelectedElementIdsJson`.
  - في `GetPatientTestWithProfileAsync` يستدعي `_normalRangeService.GetMatchingRangeAsync(labTestElementId, _patient)` ويملأ `NormalRangeText`.
- `ViewModels/Pages/TestResultEntryViewModel.cs` — `TestResultRow.IsEnabled` + `TestResultRow.IsReviewed` + `TestResultRow.IsPrinted` + `TestResultRow.NormalRangeText`.
- `Views/Windows/TestResultEntryView.xaml`:
  - عمود جديد "Normal Range" (`IsReadOnly=True Width=200`).
  - عمودان جديدان CheckBox: "Reviewed" و"Printed".
  - `IsReadOnly="{Binding !IsEnabled}"` مع تلوين رمادي للعناصر Disabled.

**نقطة توضيح**: تخزين الـ Checkboxes على مستوى `TestResult` مؤجَّل حتى تنفيذ L1-P5 (Migration) — يُلاحظ الترتيب. انظر `CL-02`.

**التبعيات**: L1-P5.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L6-P4 — Popup لاختيار Saved Comment (F4-P5)
**المسار**:
- `Views/Windows/TestResultEntryView.xaml` — تغليف زر "Saved Comments" بـ `materialDesign:PopupBox` يعرض `ItemsControl` من `SavedComments`.
- `ViewModels/Pages/TestResultEntryViewModel.cs` — النقر على عنصر ينفّذ `PickSavedCommentCommand` مع `SavedComment` المختار.

**التبعيات**: L6-P2 (تسجيل التاريخ لـ Undo).

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L6-P5 — Enter navigation + Save في الخانة الأخيرة (F4-P9)
**المسار**:
- `Views/Windows/TestResultEntryView.xaml.cs` — `PreviewKeyDown` handler على TextBox داخل `DataGridTemplateColumn.CellTemplate`:
  - على `Enter`: إن لم يكن آخر صف → `DataGrid.MoveFocus(FocusNavigationDirection.Down)`؛ وإلا `SaveCommand.Execute()`.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings + اختبار: Enter داخل خانة قيمة ينقل التركيز للأسفل؛ في آخر خانة يحفظ ويغلق.

---

### Part L6-P6 — إثراء بيانات المريض في `TestResultEntryView` + إصلاح `MainMenuCommand` (F4-P1 + F4-P12)
**المسار**:
- `ViewModels/Pages/TestResultEntryViewModel.cs`:
  - `[ObservableProperty]`: `patientGender`, `patientAge` (formatted `AgeValue + AgeUnit`), `visitDate`.
  - `MainMenu()` يستدعي `_navigationService.ReturnToDashboard()` بدلاً من إغلاق النافذة فقط.
- `Views/Windows/TestResultEntryView.xaml` — توسيع Row 0 من 3 أعمدة إلى 5 (Name / LabId / Gender / Age / VisitDate) + Title للتحليل في صف علوي.
- `ViewModels/Pages/MainDashboardViewModel.cs` — التحقق من وجود `ReturnToDashboard()` (أو استخدام `INavigationService` الموجود).

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

## 🗂️ Level 7 — Drag & Drop المتقدم + Advanced UI

### Part L7-P1 — تنفيذ زر "موافق" + Enter مرتين + طباعة الإيصال (F1-A.8 + F1-A.9 — الجزء VM/UI)
**المسار**:
- `ViewModels/Pages/PatientEntryViewModel.cs`:
  - `ConfirmCommand` (حفظ + إعادة تعيين النموذج).
  - `_lastEnterAt` DateTime لتتبع Enter المزدوج + `KeyboardEnterCommand`.
  - `PrintReceiptCommand` يستبدل placeholder بـ `await _receiptService.GeneratePdfAsync(patient, tests, settings)` وفتح PDF.
- `Views/Pages/PatientEntryView.xaml` — زر "موافق" جديد + InputBinding لـ Enter.
- **يُستهلَك** `IReceiptPdfService` من L2-P1.

**التبعيات**: L2-P1، L1-P4.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L7-P2 — إصلاح Fallback في `CalculateTotalAsync` + جهة افتراضية + إنشاء جهة تلقائي (F1-A.10 + F1-A.11 + F1-A.12)
**المسار**:
- `Services/Implementations/PatientService.cs` — في `CalculateTotalAsync` عند `BillingSystem.LabToLab` وعدم وجود `ReferralPrice`، جلب `LabTest.LabToLabPrice` من DB (`Include(l => l.LabTest)` أو `FindAsync`).
- `ViewModels/Pages/PatientEntryViewModel.cs`:
  - `SaveAsync` — قبل الحفظ، إذا `ReferralId == null && string.IsNullOrEmpty(SelectedReferralName)` استدع `_referralService.GetDefaultLabAsync()` وعيّن `ReferralId`.
  - إذا كتب المستخدم اسم جهة غير موجود، استدع `_referralService.GetOrCreateAsync(SelectedReferralName)` وعيّن `ReferralId`.

**دمج T-06 مباشرة**: يستهلك `IReferralService` الموجود — لا تعارض، فقط إعادة استخدام آمن.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L7-P3 — قائمة "مرضى اليوم" منسدلة + زر "إلغاء" ديناميكي (F1-A.13 + F1-A.14)
**المسار**:
- `Views/Pages/PatientEntryView.xaml`:
  - استبدال زر "مرضى اليوم" بـ `Menu` أو `Popup` يعرض DataTemplate لكل مريض اليوم مع أزرار "تعديل" و"تحصيل" لكل صف.
  - `DataTrigger` على `IsAddMode` / `IsEditMode` لإخفاء أزرار الإضافة/التعديل وإظهار "إلغاء" فقط بعد بدء العملية.
- `ViewModels/Pages/PatientEntryViewModel.cs` — `LoadTodayPatientsPopupAsync` يملأ `ObservableCollection<TodayPatientRow>` + أوامر `SelectTodayPatientCommand`, `CollectPaymentCommand`.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L7-P4 — أيقونات الحالة داخل جدول تحاليل F5 + علامات إضافية (F5-I + F5-K)
**المسار**:
- `Views/Pages/DeliveryView.xaml`:
  - عمود `Status` من `DataGridTextColumn` إلى `DataGridTemplateColumn` مع `<materialDesign:PackIcon Kind="{Binding Status, Converter={StaticResource TestStatusToIconConverter}}" Foreground="{Binding Status, Converter={StaticResource TestStatusToColorConverter}}"/>`.
  - أعمدة CheckMark/Circle لكل من `IsReviewed`، `IsEntered`، `IsPrinted`، `IsDelivered`.
- `Services/Interfaces/IDeliveryService.cs` — توسيع `DeliveryPatientTestRow` بـ `IsReviewed`, `IsEntered`.
- `Services/Implementations/DeliveryService.cs` — تعبئة الحقلين.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L7-P5 — عدد "غير المنتهية" + زر "تصفية حساب" + عدادات F6 + إحصائية الحسابات المفتوحة (F5-E + F5-F + F6-G + F6-H)
**المسار**:
- `Services/Interfaces/IDeliveryService.cs` — تعديل `GetPatientDeliveryStateAsync` لإرجاع `(int Unentered, int Undelivered, int Unprinted, decimal Remaining)`.
- `Services/Implementations/DeliveryService.cs` — حساب `unentered = tests.Count(t => t.Status == TestStatus.New)`.
- `ViewModels/Pages/DeliveryViewModel.cs`:
  - `[ObservableProperty] int unenteredCount`.
  - `ClearAccountCommand` (يستهلك L2-P4).
- `Views/Pages/DeliveryView.xaml` — عرض `UnenteredCount` + زر "تصفية حساب" مع Confirmation.
- `ViewModels/Pages/SearchViewModel.cs` — `[ObservableProperty] int openAccountsCount` + عرض `Summary.UnenteredCount/UnprintedCount/UndeliveredCount` (يستهلك L2-P5).
- `Views/Pages/SearchView.xaml` — 4 `StackPanel` جديدة (Unentered/Unprinted/Undelivered/OpenAccounts).

**التبعيات**: L2-P4، L2-P5.

**بوابة البناء**: 0 errors / 0 warnings.

---

## 🗂️ Level 8 — KeyBindings محلية + Escape سياقية (Consolidated via T-09)

### Part L8-P1 — تطبيق `GlobalShortcuts.RegisterOn="True"` على النوافذ الأربع (F1-A.7 + F3-P8 + F5-J + F6-I) + إصلاح F1 + إصلاح ملصق F9 + دعم Enter/Arrow في F2 + دعم Enter/Barcode في F6
**المسار**:
- `Views/Pages/PatientEntryView.xaml`:
  - إضافة `Helpers:GlobalShortcuts.RegisterOn="True"` على `<UserControl>`.
  - **إعادة تعيين `Key="F1"` من `AddSelectedTestCommand` إلى `ShowTestInfoCommand`** (بند 36 من خطة F1_F7 + N-01).
  - **دمج N-01 مباشرة**: تغيير نص الزر "إضافة التحليل (F1)" في السطر 153 إلى "إضافة التحليل" (بلا رمز F1) لتجنب التباس بصري مع دلالة F1 الجديدة "عرض بيانات التحليل". هذا يُعالج الأثر الجانبي الذي أغفلته الخطة الأصلية.
  - إضافة `KeyBinding F5` لـ `RefreshCommand`, `F7` لـ `OpenExternalSamplesCommand` (stub)، `F8` لـ `EditCommand`.
- `ViewModels/Pages/PatientEntryViewModel.cs` — `ShowTestInfoCommand`، `RefreshCommand` (جديدان).
- `Views/Pages/TestResultsListView.xaml` — `GlobalShortcuts.RegisterOn="True"` (يوفر F2/F3/F4/F6/F7/Esc تلقائياً، حسب تصميم Behavior في L0-P5) + أوامر VM مطلوبة (`OpenPatientDataCommand`, `OpenExternalSpecimensCommand` stub).
- `Views/Pages/DeliveryView.xaml`:
  - `GlobalShortcuts.RegisterOn="True"`.
  - **نقل F9 من `DeliverManuallyCommand` إلى `ToggleFinishedCommand`** (F5-J).
  - **دمج N-05 / X-02 مباشرة**: تغيير نص الزر (سطر 150 حالياً "F9 - تسليم يدوي") إلى "تسليم يدوي" (بدون رقم مفتاح) + إضافة زر منفصل صغير للتسليم اليدوي عبر أمر بدون اختصار F9 (أو `Ctrl+D` كبديل)، لضمان أن نقل F9 لا يترك تعارضاً بصرياً.
- `Views/Pages/SearchView.xaml`:
  - `GlobalShortcuts.RegisterOn="True"` (تُضاف F6 من داخل النافذة).
  - إضافة `<KeyBinding Key="Enter"/>` على TextBoxes الكود (VisitCode / LabCode / FileCode / Phone / NationalId).
  - ربط `BarcodeScannerListener` من `Helpers/` (T-04) لكشف بادئة الكود عبر `IBarcodeCodeTypeDetector` من L3-P4.
- `Views/Windows/BarcodeView.xaml`:
  - إضافة `<KeyBinding Key="Enter" Command="{Binding FocusExtraDescriptionCommand}"/>` على TextBox `ExtraBarcodeName`.
  - زر `PackIcon Kind="ArrowDown"` بين الحقلين لنقل الفوكس (F2-Fix.6).

**دمج T-04 مباشرة**: `BarcodeScannerListener` الموجود في `Helpers/` يُستهلَك في `SearchView.xaml.cs` (F6-C) بدون إعادة إنشاء — يُشار له كـ "استخدام" لا "إنشاء" (يعالج F1-A.15 أيضاً في `PatientEntryView.xaml.cs`).

**دمج T-09 مباشرة**: كل KeyBindings المحلية الأربعة تُحلّ بـ AttachedBehavior من L0-P5 في Part واحد — يوفر تكرار 4× قد كان في الخطط الأصلية.

**دمج N-05 مباشرة**: انظر أعلاه (نص زر F9 في `DeliveryView`).
**دمج N-01 مباشرة**: انظر أعلاه (نص زر F1 في `PatientEntryView`).

**فحص فعلي**:
- تم التأكيد أن `PatientEntryView.xaml:8` لا يزال يحوي `<KeyBinding Key="F1" Command="{Binding AddSelectedTestCommand}" />`، والزر المرئي في السطر 153 لا يزال بنص "إضافة التحليل (F1)".
- تم التأكيد أن `DeliveryView.xaml:9` لا يزال يحوي `<KeyBinding Key="F9" Command="{Binding DeliverManuallyCommand}" />` والزر في السطر 150 بنص "F9 - تسليم يدوي".

**التبعيات**: L0-P5، L3-P4 (`IBarcodeCodeTypeDetector`).

**بوابة البناء**: 0 errors / 0 warnings + اختبار لكل مفتاح: يستدعي أمرَه من داخل النافذة نفسها.

---

## 🗂️ Level 9 — Wizards + Reports + Print Stubs + سيناريو الباركود المزدوج

### Part L9-P1 — Wizard لإدخال 6 معدلات دفعة واحدة (F8-Fix.5)
**المسار**:
- `ViewModels/Pages/NormalRangeViewModel.cs` — `AddSixRangesWizardCommand` يفتح ديالوج صغير يطلب القيم المشتركة (`TestUnit`, `NormalRangeText`, `LowLimit`, `HighLimit`, `LowFlag`, `HighFlag`, `LowComment`, `HighComment`) + عند "موافق" يُنشئ 6 كيانات `NormalRange` بالنطاقات: `(Male|Female, 0-120, Year)`، `(Male|Female, 1-29, Day)`، `(Male|Female, 1-11, Month)` داخل Transaction.
- `Views/Windows/NormalRangeView.xaml` — زر جديد "6 معدلات (تحليل لا يعتمد على الجنس/السن)".
- `Views/Windows/NormalRangeSixWizardDialog.xaml` (+ `.xaml.cs`) — نافذة ديالوج جديدة.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L9-P2 — تفعيل Stubs الطباعة الخمسة + زر "بيانات المرضى" مرئي (F3-P10 — الجزء UI)
**المسار**:
- `Views/Pages/TestResultsListView.xaml` — زر "بيانات المرضى (F2)" مرئي في العمود الأيسر مربوط بـ `OpenPatientDataCommand` (يعالج N-3-07).
- `ViewModels/Pages/TestResultsListViewModel.cs` — استبدال Stubs بـ استدعاءات فعلية لـ `IReportPdfGenerator` (يستهلك L2-P2).

**التبعيات**: L2-P2.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L9-P3 — سيناريو الباركود المزدوج + تنبيه باقي الحساب (F5-H)
**المسار**:
- `ViewModels/Pages/DeliveryViewModel.cs`:
  - حالة `PendingReceipt` — إذا وصل كود بادئته 1، احفظ المريض الحالي وانتظر مسحة ثانية.
  - عند وصول كود بادئته 3، قارن `Patient.FileCode` مع بيانات `PendingReceipt`.
  - عند التطابق: `_dialogService.ShowConfirmation("هل تريد تسليم النتائج؟")`.
  - `DeliverManuallyAsync` قبل التأكيد: إذا `SelectedPatient.RemainingBalance > 0`، أظهر تنبيه إضافي "يوجد باقي حساب X — هل تريد الاستمرار؟".

**دمج N-07 مباشرة**: كشف Drift Report أن `BarcodeService.GenerateCaseCode` (سطر 24) يقرأ `visit.DailySequenceNumber` **مباشرة من كيان `PatientVisit` الممرَّر** بينما CP-F2-1 يقول "يُحسب من count of today's actual visits + 1". **نقطة توضيح**: إن لم يُدفع `PatientVisit.DailySequenceNumber` بقيمة صحيحة قبل استدعاء `GenerateCaseCode`، سيولّد كوداً برقم تسلسل صفر أو خاطئ — يؤثر مباشرة على سيناريو الباركود المزدوج (كود الإيصال). يُقترح إما إعادة كتابة `GenerateCaseCode` لتحسب التسلسل من DB بنفسها، أو ضمان أن `PatientVisitService.CreateVisitAsync` تدفع التسلسل قبل تمرير الزيارة — انظر `CL-03`.

**التبعيات**: L0-P5 (Barcode listener attached).

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L9-P4 — الزر البرتقالي "طباعة كلا الأكواد" (F2-Fix.5)
**المسار**:
- `Views/Windows/BarcodeView.xaml` — زر ثالث بجوار "طباعة الكل" بلون برتقالي (`Background="#FF9800"` أو `MaterialDesignRaisedAccentButton`) + `Visibility="{Binding PrintFileCodeWithAll, Converter={StaticResource BoolToVisibilityConverter}}"` — يُنفِّذ نفس `PrintAllCommand`.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings.

---

### Part L9-P5 — مراجعة صيغة الكود الأولى (F2-Fix.7 — اختياري بقرار مالك)
**المسار**:
- `Services/Implementations/BarcodeService.cs` (سطر 134-153).

**نقطة توضيح**: هذا Part اختياري. المرجع يقول "الخانة الأولى لا يُنظر إليها" (ديناميكية)، بينما التنفيذ الحالي ثابت "1". الوظيفة صحيحة عملياً (الخانة الثانية هي المميّز). **يحتاج قرار مالك جديد** لتحديد ما إذا كانت المطابقة الحرفية مطلوبة — انظر `CL-04`.

**التبعيات**: لا شيء.

**بوابة البناء**: 0 errors / 0 warnings (فقط عند إقرار المالك بالتنفيذ).

---

### Part L9-P6 — قراءة Lab ID عبر Barcode Scanner في PatientEntryView (F1-A.15)
**المسار**:
- `Views/Pages/PatientEntryView.xaml.cs` — دمج `BarcodeScannerListener` (T-04) لالتقاط الباركود وتعيين `LabId` واستدعاء `PatientService.GetByLabIdAsync` تلقائياً.

**دمج T-04 مرة أخرى**: نفس المكوّن المشترك المستخدَم في F5/F6 يُستهلَك هنا — لا إعادة إنشاء.

**التبعيات**: L0-P5.

**بوابة البناء**: 0 errors / 0 warnings.

---

## ✅ نظرة عامة على الاستهلاك (Reuse Matrix)

| المكوّن المشترك | يُبنى في | يُستهلَك في |
|---|---|---|
| `EnumToArabicConverter` (T-03) | L0-P1 | L3-P1, L3-P2, L3-P3, L5-P4, L5-P8 |
| `BoolToRedBrushConverter` (T-02) | L0-P2 | L5-P2, L5-P3, L5-P4 |
| `LatinSymbolsPad` (T-01) | L0-P3 | L5-P10, L5-P11 |
| `PatientDisplayInfo` (T-11) | L0-P4 | L5-P1, L5-P3, L5-P4 |
| `DoubleClickBehavior` + `GlobalShortcuts` (T-09/T-10) | L0-P5 | L5-P7, L8-P1, L9-P3, L9-P6 |
| Enums الجديدة (X-03) | L0-P6 | L3-P1, L3-P2, L3-P3, L5-P4, L5-P6 |
| `IBarcodeCodeTypeDetector` (T-05) | L3-P4 | L8-P1 (F5+F6), L9-P3 |
| `BarcodeScannerListener` (T-04) | موجود مسبقاً في F5 | L8-P1 (F6), L9-P6 (F1) |
| `IReferralService` (T-06) | موجود مسبقاً | L5-P4 (F6), L7-P2 (F1) |
| `IReceiptPdfService` / `IReportPdfGenerator` (T-07, X-01) | L2-P1 / L2-P2 | L7-P1, L9-P2 |
| `IsAdmin` gates (T-08) | موجود عبر `ICurrentUserService` | L4-P3, وكل زر Delete موجود |

---

## ❓ Clarification Points — نقاط غير محسومة تحتاج قرار مالك أو تحقق إضافي

### CL-00 — قاعدة الكوميت المرجعي
جميع Drift Reports في Cloud_Consolidation_Analysis.md بُنيت على مقارنة الكوميت `daae46a` بالخطط المُنتَجة على الكوميت السابق `56968b4`. الكوميت المعتمد للتحقق في هذه الوثيقة (`45f04ac`) يحتوي `daae46a` + إضافة ملف `Cloud_Consolidation_Analysis.md` نفسه فقط — لا فرق كود بينهما. تم التحقق يدوياً من ~10 نقاط حرجة (Patient.AgeValue، BarcodeService.BranchConstant، DeliveryService.trimmed[1]، BarcodeView Escape، TestResultsListService switch، TestResultEntryViewModel/AutoCalc، PatientEntryView F1، DeliveryView F9، SearchViewModel.AvailableReferrals، TestResultEntryView Comment buttons) — كلها لا تزال قائمة كما وُصفت. لا Drift ضمني.

### CL-01 — تفسير حقل `P & S` في `NormalRange`
تقرير C-04 من Drift Report يذكر أن تفسير `P & S` يحتاج قرار مالك جديد (ليس ضمن الـ17 قراراً). الخطة الأصلية F2_F8 اقترحت التفسير الاحتمالي (`P = Print in report`, `S = Sensitive/Show as critical`). **قبل تنفيذ Part L1-P3 كاملاً وأي منطق تجاري يعتمد عليه (تصفية العرض في التقرير، تنبيه للنتائج الحساسة)، يُطلب تأكيد المالك على التفسير الفعلي.**

### CL-02 — مستوى تخزين Checkboxes الطباعة/المراجعة في F4
الخطة الأصلية F3_F4 (Part F4-P4) تشير إلى Checkboxes "لكل عنصر" مما يفرض التخزين على `TestResult`. لكن التصميم الحالي يخزّن هذه العلامات على مستوى `PatientTest` (التحليل الكامل، لا العنصر). **يحتاج قرار مالك**: هل يبقى التخزين على `PatientTest` (وتُلغى Checkboxes لكل عنصر)، أم يُضاف على `TestResult` (ويُنفَّذ L1-P5 كما هو مخطط)؟

### CL-03 — مصدر `PatientVisit.DailySequenceNumber` في `BarcodeService.GenerateCaseCode`
Drift Report (N-07) كشف أن `BarcodeService.GenerateCaseCode` يقرأ التسلسل مباشرة من الكيان الممرَّر بدلاً من احتسابه من DB (`count of today's actual visits + 1` كما ورد في CP-F2-1). **يحتاج تحقق تصميمي**: هل الاعتماد على `PatientVisitService` لدفع التسلسل صحيح ومضمون في كل مسارات الاستدعاء؟ إن لم يكن، يجب إعادة كتابة `GenerateCaseCode` لتحسب التسلسل بنفسها. القرار يؤثر مباشرة على Part L9-P3.

### CL-04 — بنية الخانة الأولى في صيغة الكود 13 خانة
المرجع يقول "الخانة الأولى = لا يُنظر إليها" (ديناميكية)، بينما التنفيذ الحالي ثابت "1". لا يُخلّ ذلك بالوظيفة (المميّز هو الخانة الثانية `typeDigit`)، لكن **إن كانت المطابقة الحرفية مطلوبة تجارياً**، يُنفَّذ Part L9-P5. **قرار مالك مطلوب**.

### CL-05 — توحيد `IReceiptPdfService` مع `IReportPdfGenerator`
التقرير المرجعي أوصى بإبقاء الخدمتين منفصلتين (X-01)، لكن هذا قرار تصميم لا قرار مالك. **تنبيه**: إن ظهر لاحقاً تداخل كبير في القوالب (مثلاً بيانات مريض + قائمة تحاليل + إجماليات مكرَّرة)، يمكن إعادة النظر — لكن ليست فجوة الآن.

### CL-06 — تعارض `enums` بين F3 وF5
تم إبقاء `PatientListFilter` و`DeliveryFilterMode` منفصلَين (X-03) لأن مجموعتَي الفلاتر مختلفتان. **لا يحتاج قرار جديد** — سُجِّل للتوثيق فقط.

### CL-07 — الأزرار الأربعة "الحالة أمام كل تحليل" (F3-P6)
الخطة الأصلية تفترض توفر أعلام Boolean مستقلة `IsEntered` / `IsReviewed` / `IsPrinted` / `IsDelivered` على `PatientTest`. **تحقق تصميمي مطلوب**: تأكّد أن كل الأعلام الأربعة موجودة كأعمدة Boolean فعلية في الكيان (وليست مشتقّة من `Status` enum). لو أي منها مشتق، يجب إضافة عمود Boolean حقيقي + Migration قبل تنفيذ L5-P8.

### CL-08 — سلوك `SwitchToBackupCommand` الفارغ في `SearchViewModel`
قرار 12 يفرض أن الميزة Stub معطَّل. الأمر موجود لكن body فارغ. **قرار مالك مطلوب** (بسيط): هل يُحذف تماماً أم يُترك مع رسالة `"قريباً"` للتوثيق البصري للنية؟

---

## 🏁 Sign-off Criteria — معايير القبول النهائية (شاملة للوظائف الثمانية)

كل الوظائف الثمانية تُعتبر مكتملة بعد استيفاء **جميع** المعايير التالية:

### 1. البناء والاختبار الأساسي
- [ ] `dotnet build` يُنتج **0 errors / 0 warnings** بعد كل Part، بلا استثناء.
- [ ] `dotnet ef database update` يعمل على قاعدة نظيفة بلا أخطاء.
- [ ] كل Migration (L1-P1..L1-P5) تُنفَّذ Up ثم Down بدون فقد بيانات.

### 2. الالتزام بالقرارات الـ17
- [ ] لا يوجد كود يعتمد على قيم Gender خارج `Male/Female` (قرار 17).
- [ ] كل الحقول الطبية على `Patient` أعمدة Boolean مستقلة (قرار 1).
- [ ] `BranchConstant = "1"` مثبَّت برمجياً، بلا حقل Branch في `LabTest`/`TestGroup` (قرار 5).
- [ ] `ZXing.Net` هي المكتبة الوحيدة للباركود (قرار 3).
- [ ] `LabelWidth = 38, LabelHeight = 25` قابلة للتخصيص عبر UI (قرار 4).
- [ ] `GetMatchingRangeAsync` يطبّق "أضيق مدى يفوز" (قرار 16).
- [ ] كل عمليات الحذف تفحص `IsAdmin` وترمي `UnauthorizedAccessException` لغير Admin (قرارات 2, 11, 13).
- [ ] `IsBackupSearchEnabled = false` + Service يرمي `NotImplementedException` (قرار 12).
- [ ] Constants ISI / Control Time / Hgb% seeded بالقيم الصحيحة (قرار 8).
- [ ] لا زر "تاريخ مخصص" (قرار 9).

### 3. الوظيفة 1 — إضافة/تعديل بيانات المريض
- [ ] قبول عمر عشري (2.5 سنة) مع validation صحيح.
- [ ] حقل Notes ظاهر ومحفوظ.
- [ ] `ExternalSpecimenTypeId` ComboBox مُعبَّأ.
- [ ] `Title` يتحدّث تلقائياً حسب `Gender`.
- [ ] `F1` يعرض بيانات التحليل (لا يضيف تحليلاً)، والزر المرئي "إضافة التحليل" بلا رمز F1.
- [ ] Double-Click على تحليل يضيفه؛ Double-Click على المختار يحذفه.
- [ ] زر "موافق" + Enter مرتين يؤكّد الدفع.
- [ ] `PrintReceiptCommand` يُنتج PDF فعلياً.
- [ ] `LabToLab` fallback يستخدم `LabTest.LabToLabPrice` عند غياب `ReferralPrice`.
- [ ] `GetOrCreateAsync` تُستدعى تلقائياً في `SaveAsync` عند اسم جهة جديد.

### 4. الوظيفة 2 — طباعة الباركود
- [ ] `Escape` يغلق النافذة (لا يستدعي `PrintAllCommand`).
- [ ] الملصقات مُصنَّفة حسب `SpecimenType` (لا ملصق "Default" واحد).
- [ ] سلة محذوفات مرئية تحذف الملصق أو التحليل عبر Drop.
- [ ] Drag & Drop للتحاليل بين ملصقات يعمل.
- [ ] الزر البرتقالي "طباعة كلا الأكواد" ظاهر عند Checkbox المفعّل.
- [ ] Enter/زر الأسهم بين خانتَي الباركود الإضافي يعمل.

### 5. الوظيفة 3 — قائمة إدخال النتائج
- [ ] ComboBox الفلاتر يعمل فعلياً (كل `PatientListFilter.*` case).
- [ ] فلاتر Individual/LabToLab/Referral تفلتر فعلياً (لا `simplified for now`).
- [ ] Double-Click على تحليل يفتح F4.
- [ ] Toggle حقيقي لـ F8/F9/F12 (مرتين → عودة).
- [ ] بيانات المريض الكاملة معروضة (Gender/Age/LabId/FileCode/VisitCode).
- [ ] اسم المريض المهم أحمر؛ خانة الزيارات خضراء بعدد صحيح.
- [ ] Click-to-Copy على LabId / VisitCode يعمل.
- [ ] البحث بالاسم داخل مرضى اليوم يعمل.
- [ ] كل أزرار الطباعة الخمسة تُنتج PDF فعلياً.
- [ ] Admin يرى `AuditLogView` + Financial Tracking؛ غير Admin يراهما disabled.
- [ ] KeyBindings المحلية F2/F3/F4/F6/F7/Esc تعمل من داخل النافذة.

### 6. الوظيفة 4 — إدخال النتائج
- [ ] `IAutoCalculationService` تُستدعى فعلياً عند تغيّر Hgb/PT/PTT.
- [ ] تلوين real-time أثناء الكتابة يعمل (Critical High Hgb=20).
- [ ] زرا "Comment from Normal Range" و"Undo Comment" يعملان (Command + VM defined).
- [ ] Enter navigation ينقل التركيز؛ في الخانة الأخيرة يحفظ.
- [ ] تمييز عناصر البروفيل المختارة عن غير المختارة (Disabled).
- [ ] `NormalRangeText` معروض في عمود مستقل.
- [ ] `HistoryButtonLabel` يتغيّر حسب عدد النتائج.
- [ ] `MainMenuCommand` يعود إلى Dashboard فعلياً.

### 7. الوظيفة 5 — تسليم النتائج
- [ ] ComboBox `FilterMode` يعمل عبر `DeliveryFilterMode enum`.
- [ ] فلتر "حسب المستخدم" (`CurrentUser`) موجود ويعمل.
- [ ] `trimmed[0]` (لا `trimmed[1]`) يكشف بادئة الكود.
- [ ] F9 مربوط بـ `ToggleFinishedCommand`، والزر المرئي بلا رمز "F9".
- [ ] سيناريو الباركود المزدوج + تنبيه باقي الحساب يعمل.
- [ ] زر "تصفية حساب" موجود ويعمل.
- [ ] تلوين المريض المهم بالأحمر يعمل.
- [ ] Click-to-Copy على VisitCode/LabId يعمل.
- [ ] `UnenteredCount` معروض بجانب `UnprintedCount` + `UndeliveredCount`.

### 8. الوظيفة 6 — البحث
- [ ] ComboBox جهة الإحالة موجود وفعّال.
- [ ] Bindings الـ Enums (Gender/AgeUnit/Source) تعمل عبر `SelectedValue`.
- [ ] `BarcodeScannerListener` مربوط + `KeyBinding Enter` على حقول الكود.
- [ ] أعمدة ID و الوحدة معروضة في DataGrid النتائج.
- [ ] Double-Click على مريض يفتح `TestResultsListView`.
- [ ] عدادات `Unentered/Unprinted/Undelivered/OpenAccounts` معروضة.
- [ ] KeyBinding F6 داخل النافذة يفتح `DeliveryView`.
- [ ] تلوين صف المريض المهم بالأحمر يعمل عبر `DataTrigger`.

### 9. الوظيفة 7 — بيانات التحاليل
- [ ] `DataGrid` بالأعمدة العشرة يحلّ محلّ `ListBox` الحالي.
- [ ] دعم Tubes 1/2/3 يعمل (Creatinine Clearance = مصل + بول 24س).
- [ ] `LabelName` مخصص للملصق موجود ومحفوظ.
- [ ] زر "قائمة التحاليل" (Back) موجود.
- [ ] `PromptQuestion` يظهر في F1 عند اختيار تحليل يحمله.
- [ ] `LatinSymbolsPad` يعمل مع كل الحقول (لا `CodeTextBox` فقط).
- [ ] البحث الخاص بعناصر التحليل الرئيسي عبر أول حرفين يعمل.
- [ ] Bill name merge يعمل في `IReceiptPdfService`.

### 10. الوظيفة 8 — المعدلات الطبيعية
- [ ] `DataGrid` بـ 11 عموداً يعرض كل الأعمدة المطلوبة.
- [ ] حقل `PrintAndSensitive` (P & S) موجود مع Migration وCheckBox في الفورم.
- [ ] Wizard الـ 6 معدلات يعمل داخل Transaction.
- [ ] `LatinSymbolsPad` (T-01) يعمل مع كل حقول النصوص، بأسس وأرقام موسّعة.
- [ ] `Escape` سلوكه صحيح (Cancel عند تعديل، Close عند العرض).
- [ ] `NormalRangeView.xaml.cs` نظيف — ItemsSource عبر Binding من VM.

### 11. المعمارية والالتزام العام
- [ ] لا يوجد `DbContext` مباشر في أي `ViewModel`.
- [ ] كل الخدمات محقونة عبر Constructor.
- [ ] كل الـ Converters وBehaviors في L0 مبنية مرة واحدة ومستهلَكة من عدة أماكن (لا نسخ مكررة).
- [ ] `AuditLog` مكتوب لكل من Save/Review/Print/Deliver/UnmarkDelivered/SettleAccount/ClearAccount/Delete.
- [ ] كل Migration مُوثَّقة في `history.md` بعد تنفيذها.

---

**نهاية Unified_Execution_Plan.md — بانتظار تفويض المالك بالتنفيذ الفعلي.**
