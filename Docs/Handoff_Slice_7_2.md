# 📋 Handoff Plan — Vertical Slice 7 (Function 7): إضافة/تعديل بيانات تحليل

> **الحالة**: تخطيط تنفيذي فقط — لا كود بعد.
> **النسخة**: 7.2 (Handoff مبني على تحليل مستقل للكود الفعلي بعد اكتمال F1).
> **تاريخ الإعداد**: 2026-07-22.

---

## 1) Meta & Scope

### Commit المرجعي (تم التحقق بالاستنساخ والـ checkout)
- **Repository**: `https://github.com/El-ogra/NewLab.git`
- **Branch**: `main`
- **Commit Hash**: `fe77e02cd211dae5f365a2645f54e94940beac12`
- **Commit Message**: "بعد إزاله الملفات المرجعية وملف خطة تنفيذ الوظيفة الأولى"
- **حالة التحقق**: ✅ Hash ورسالة الـ commit مُطابقان تماماً لما ورد في تعليمات الـ Handoff.

### النطاق
- **الوظيفة**: `Function 7 — إضافة/تعديل بيانات تحليل` (Execution Order: **Step 2 of 8**).
- **الوثيقة المرجعية**: `Docs/analysis_and_plan_v3.md` — القسم `# 🔹 Function 7` (سطور 219–300).
- **الوثيقة السياقية**: `Docs/history.md` — إصدار 1.3، Phase 5 (توثيق اكتمال F1 و Critical Fix 8 `ICurrentUserService`).

### القرارات المؤثرة على هذه الشريحة (من جدول القرارات في `analysis_and_plan_v3.md`)
| رقم | القرار | التطبيق في F7 |
|---|---|---|
| **5** | حذف `Branch` من `LabTest` و `TestGroup` و `BarcodeSettings` نهائياً؛ موضع الفرع في صيغة الكود يُثبَّت برمجياً بـ `"1"`. | Part 7.1: عدم إضافة حقل `Branch` في `LabTest` أو `TestGroup`. |
| **14** | لوحة الحروف اللاتينية = مجموعة صغيرة شائعة `α β γ μ ± ≤ ≥ °` قابلة للتوسعة (UserControl مستقل). | Part 7.7: يُنشأ `LatinSymbolsPad.xaml` كـ `UserControl` مستقل يُدرج الحرف في `TextBox` المُركَّز عليه. |
| **15** | كيان جديد `ReferralPrice` (`LabTestId + ReferralId + Price`) يُبنى في F7؛ منطق `CalculateTotalAsync` في F1 يُحدَّث للبحث في هذا الجدول قبل الرجوع لـ `LabToLabPrice`. | Part 7.1: كيان `ReferralPrice.cs`؛ Part 7.3: أساليب `GetReferralPricesAsync/SetReferralPriceAsync/DeleteReferralPriceAsync` في `ILabTestService`؛ **Part 7.10 (Retro): تعديل `PatientService.CalculateTotalAsync` في F1**. |

### اصطلاحات history.md المُعتمدة كإطار عمل
- **`ICurrentUserService` (Singleton — Critical Fix 8)**: أي جزء من F7 يحتاج فحص صلاحية Admin يستخدم `_currentUserService.IsAdmin` **بلا استثناء** — لا يُخترع مصدر بديل لهوية المستخدم. الأساليب المتاحة: `CurrentUser` (nullable)، `IsAdmin`، `SetCurrentUser(user)`، `Clear()`.
- **Scoped Services on DbContext**: أي خدمة تستهلك `NewLabDbContext` تُسجَّل `Scoped` (كما `PatientService`, `ReferralService`, `AuthService`). Singleton فقط للـ`CurrentUserService`, `NavigationService`, `DialogService`.
- **Constructor Injection حصراً**: بلا Service Locator داخل ViewModels؛ لا `DbContext` في ViewModel.
- **ViewModel-first Navigation**: `INavigationService.NavigateTo<TViewModel>()` + `DataTemplate` مطبَّق في `App.xaml` (كما ربط `PatientEntryViewModel → PatientEntryView`).
- **Enum items في XAML**: لا تُعرَّف عبر `x:Array` مباشرة — تُملأ في code-behind (تحذير مستند من Technical Notes رقم 4 في `history.md`).
- **MaterialDesign OutlinedTextBox**: `<TextBox Style="{StaticResource MaterialDesignOutlinedTextBox}" materialDesign:HintAssist.Hint="..." />` — لا يوجد Element بديل (تحذير مستند من Technical Note 3).
- **Naming Collision**: تجنُّب تعارض أسماء `[ObservableProperty]` مع `[RelayCommand]` (تحذير Technical Note 2 — سبق حلّه في F1 بإعادة تسمية إلى `LookupLabId`).
- **Migration ProductVersion**: `8.0.8` (نسخة المشروع، لا نسخة أداة `dotnet-ef`).
- **Auto-Migration on Startup**: `App.OnStartup` يستدعي `dbContext.Database.MigrateAsync()` تلقائياً — لا يحتاج تعديل من F7.

### قاعدة عدم الاجتهاد
جميع أسماء الحقول الـ 17 لكيان `LabTest` وأسماء الخصائص التسع لـ `TestGroup/LabTestElement/ReferralPrice` مأخوذة **حرفياً** من نص Function 7 في `analysis_and_plan_v3.md` (سطور 219–300). أي تعارض تقني حقيقي يُذكر في قسم **Clarification Points** أدناه، دون تغييره من تلقاء الخطة.

---

## 2) MVVM Layer Map

قبل الدخول في الأجزاء، هذا التوزيع الطبقي الكامل لكل الملفات المتوقَّع إنشاؤها/تعديلها في F7 — بلا لبس بين الطبقات:

| الطبقة | الملفات (المسارات المبنية على فحص الكود الفعلي) |
|---|---|
| **Model — Domain Entities** | `Models/Domain/TestGroup.cs` (جديد) — `Models/Domain/LabTest.cs` (جديد) — `Models/Domain/LabTestElement.cs` (جديد) — `Models/Domain/ReferralPrice.cs` (جديد، قرار 15) |
| **Model — Validation** | `Models/Validation/LabTestValidator.cs` (جديد) — يستخدم `FluentValidation` 11.10.0 المُثبَّتة |
| **Data Layer** | `Data/NewLabDbContext.cs` (**معدَّل**: +4 DbSets + Fluent API + Seed) — `Migrations/<timestamp>_AddLabTestsAndReferralPrices.cs` (جديد) + Designer |
| **Service — Interfaces** | `Services/Interfaces/ILabTestService.cs` (جديد) |
| **Service — Implementations** | `Services/Implementations/LabTestService.cs` (جديد) — يستهلك `NewLabDbContext` عبر Constructor Injection |
| **Service — Retro-Integration (Layer of F1)** | `Services/Implementations/PatientService.cs` (**معدَّل** — Part 7.10) — تفعيل استعلام `ReferralPrices` الحقيقي |
| **DI Container** | `App.xaml.cs` (**معدَّل**: +Scoped `ILabTestService`, +Scoped `IValidator<LabTest>`, +Transient `LabTestManagementViewModel`) |
| **DI Container — DataTemplate** | `App.xaml` (**معدَّل**: +`DataTemplate DataType={x:Type vmpages:LabTestManagementViewModel}`) |
| **ViewModel** | `ViewModels/Pages/LabTestManagementViewModel.cs` (جديد) — Constructor Injection لـ `ILabTestService`, `IReferralService`, `IDialogService`, `INavigationService`, `IValidator<LabTest>`, `ICurrentUserService` |
| **ViewModel — Retro-Integration (Layer of F1)** | `ViewModels/Pages/PatientEntryViewModel.cs` (**معدَّل** — Part 7.11) — استبدال `LabTestPlaceholder` بـ `LabTest`، تعبئة `AvailableTests` عبر `ILabTestService`، تفعيل `AddSelectedTestCommand` |
| **View — UserControl** | `Views/Pages/LabTestManagementView.xaml` + `.xaml.cs` (جديد) — نمط ثابت مع `PatientEntryView` (FlowDirection=RTL, ثلاثة أعمدة، MaterialDesign) |
| **View — Reusable UserControl** | `Views/Controls/LatinSymbolsPad.xaml` + `.xaml.cs` (جديد، قرار 14) — لوحة رموز قابلة للتوسعة |
| **View — Shell (F1's file)** | `Views/Pages/PatientEntryView.xaml.cs` (**معدَّل — Part 7.11**) — إزالة تعبئة اليدوية للاختبارات الوهمية إن وُجدت، الاعتماد على ViewModel |
| **Toolbar Wiring** | `ViewModels/Pages/MainDashboardViewModel.cs` (**معدَّل** — Part 7.9): إضافة `FunctionDefinition` "بيانات التحاليل" في فئة `SystemData` مع `TargetViewType = typeof(LabTestManagementView)`، وتوسيع `OpenFunction` liftup لدعم النوع الجديد كما وُسِّع لـ `PatientEntryView` سابقاً |
| **Placeholder Removal** | `ViewModels/Pages/LabTestPlaceholder.cs` (**محذوف** — Part 7.11) |

**تأكيد MVVM**:
- ❌ لا `DbContext` في ViewModel.
- ❌ لا `new Service()` داخل ViewModel — DI حصراً.
- ❌ لا Static Service Locator.
- ✅ code-behind للـ Views محصور بتهيئة عناصر لا يمكن ربطها في XAML (مثل ملء ComboBox enums — نفس نمط `PatientEntryView.xaml.cs`).
- ✅ فحص Admin للحذف يمر عبر `ICurrentUserService.IsAdmin` — لا يُخترع نمط جديد.

---

## 3) Retro-Integration with Function 1 (⚠️ نقطة حاسمة — نظير Future-Impact Awareness بالاتجاه المعاكس)

Function 7 هي الشريحة التي **تُغلق** ديوني F1 المؤقتة. يجب التعامل مع هذا القسم كجزء لا يتجزأ من المخرج التنفيذي — لا كملاحظة جانبية.

### 3.1 الحلول المؤقتة المتبقية من F1 (مؤكَّدة بالفحص الفعلي عند الـ commit)

| الأصل المؤقت في F1 | مكانه الفعلي (تم فحصه) | السبب المؤقت | الحل النهائي في F7 |
|---|---|---|---|
| `LabTestPlaceholder` (record) | `ViewModels/Pages/LabTestPlaceholder.cs` (سطر 3) — `public sealed record LabTestPlaceholder(int Id, string Code, string Name, decimal Price)` | كيان `LabTest` الحقيقي لم يكن مبنياً | استبدال كامل بـ `LabTest` (Part 7.1) واستهلاكه من `PatientEntryViewModel.AvailableTests` (Part 7.11) |
| `ObservableCollection<LabTestPlaceholder> AvailableTests` | `ViewModels/Pages/PatientEntryViewModel.cs` سطر 65 | لا خدمة تحاليل | تغيير نوعه إلى `ObservableCollection<LabTest>` وتعبئته من `ILabTestService.GetAllAsync()` أو `GetRoutineTestsAsync()` (Part 7.11) |
| `RemoveTest(LabTestPlaceholder? test)` | `ViewModels/Pages/PatientEntryViewModel.cs` سطر 200 | نفس السبب | تغيير التوقيع إلى `RemoveTest(LabTest? test)` مع نفس منطق البحث في `SelectedTests` عبر `LabTestId` (Part 7.11) |
| `AddSelectedTestCommand` (فارغ) | `ViewModels/Pages/PatientEntryViewModel.cs` سطر 193–197 (تعليق: `// TODO: to be implemented in Function 7 with ILabTestService`) | لا خدمة تحاليل | تعبئة فعلية: يقرأ عنصر مُحدَّد من `AvailableTests`، يبني `PatientTestRow` من (`LabTest.Id, LabTest.TestName, PatientPrice OR LabToLabPrice`)، ويُضيفه إلى `SelectedTests`، ثم يستدعي `RecalculateTotalAsync()` (Part 7.11) |
| منطق `LabToLab` في `CalculateTotalAsync` = يُستخدم `test.Price` مباشرة | `Services/Implementations/PatientService.cs` سطور 92–95 — تعليق موثَّق: `// In F1: use PatientTestRow.Price directly (ReferralPrices table not yet created in F7)` | جدول `ReferralPrices` لم يكن موجوداً | استبدال المنطق بالاستعلام الفعلي من `_context.ReferralPrices` بـ `(LabTestId, ReferralId)`؛ Fallback إلى `LabTest.LabToLabPrice` عند غياب سعر خاص بالجهة (Part 7.10) |

### 3.2 الآلية الدقيقة لاستبدال `LabTestPlaceholder` بـ `LabTest`

**ضمن Part 7.11 (LOCKED — لا تُنفَّذ خارجه)**:

1. حذف الملف `ViewModels/Pages/LabTestPlaceholder.cs` بالكامل.
2. في `ViewModels/Pages/PatientEntryViewModel.cs`:
   - إضافة `using NewLab.Models.Domain;` (لأن `LabTest` سيكون في `NewLab.Models.Domain`).
   - تعديل السطر 65: `public ObservableCollection<LabTest> AvailableTests { get; } = new();`
   - تعديل السطر 200: `private void RemoveTest(LabTest? test)` — يبقى المنطق: `var row = SelectedTests.FirstOrDefault(t => t.LabTestId == test.Id);` (يعمل كما هو لأن `LabTest.Id` موجود في الكيان الحقيقي بنفس الاسم).
   - إضافة حقن `ILabTestService` في constructor كـ dependency جديد (Part 7.11 تُعدِّل constructor).
   - إضافة أسلوب `LoadAvailableTestsAsync()` يُستدعى في `AddPatientCommand` (وحين فتح الشاشة أول مرة) لملء `AvailableTests` من `_labTestService.GetRoutineTestsAsync()` كافتراضي (يُطابق سلوك النظام المرجعي: القائمة تبدأ بـ "التحاليل الروتينية").
   - تعبئة أسلوب `AddSelectedTest` (السطر 194): يأخذ `SelectedAvailableTest` (خاصية جديدة `[ObservableProperty] private LabTest? selectedAvailableTest;`)، ويبني `PatientTestRow` وفق `BillingSystem`:
     - `Individual` → `test.PatientPrice`
     - `LabToLab` → `test.LabToLabPrice` (بدون استعلام `ReferralPrices` في هذه النقطة — الحساب النهائي يتم في `CalculateTotalAsync`).
     - `Free` → `0m`.

3. في `Views/Pages/PatientEntryView.xaml`:
   - تعديل `DataGrid` (السطر 176): استبدال `Binding LabTestId` (يبقى كما هو لأن `PatientTestRow.LabTestId` غير مُتَأثِّر) — لا تغيير مطلوب هنا فعلياً.
   - تعديل `ListBox Grid.Row="2"` (السطر 148): إضافة `DisplayMemberPath="TestName"` (بدلاً من عرض ToString الافتراضي لـ `LabTest`) و `SelectedItem="{Binding SelectedAvailableTest}"`.

**متى بالضبط**: الاستبدال يقع **حصراً داخل Part 7.11 (Retro-Integration ViewModel)** — لا يُبعثر عبر أجزاء أخرى. Part 7.1 يُنشئ `LabTest` كيان، Part 7.5 يوفّر `ILabTestService`، لكن نقطة **الاستبدال** بحد ذاتها = Part 7.11.

### 3.3 الآلية الدقيقة لتفعيل الاستعلام الحقيقي في `CalculateTotalAsync`

**ضمن Part 7.10 (LOCKED)**:

الكود الحالي في `Services/Implementations/PatientService.cs` (تم فحصه — سطور 82–123):

```
case BillingSystem.LabToLab:
    // In F1: use PatientTestRow.Price directly (ReferralPrices table not yet created in F7)
    total += test.Price;
    break;
```

**التعديل المطلوب في Part 7.10**:

1. حقن `NewLabDbContext` باقٍ كما هو في `PatientService` constructor (موجود من F1 — سطر 15). **لا حاجة لإضافة `ILabTestService`** لأنه يخلق dependency دائري محتمل؛ الاستعلام يذهب مباشرة عبر `_context.ReferralPrices`.
2. توسيع `PatientTestRow` **غير مطلوب** — التوقيع الحالي `(int LabTestId, string TestName, decimal Price)` كافٍ لأن `Price` سيبقى القيمة الافتراضية (المُسبَقة من `LabToLabPrice`) وسيُستبدل داخل الحلقة بسعر الجهة إذا وُجد.
3. الاستبدال:

```
case BillingSystem.LabToLab:
    decimal priceForThisTest = test.Price; // fallback = LabToLabPrice preloaded by VM
    if (referral != null)
    {
        var referralPrice = await _context.ReferralPrices
            .FirstOrDefaultAsync(rp => rp.LabTestId == test.LabTestId
                                    && rp.ReferralId == referral.Id);
        if (referralPrice != null)
        {
            priceForThisTest = referralPrice.Price;
        }
    }
    total += priceForThisTest;
    break;
```

4. **تحويل الأسلوب إلى `async` حقيقي**: التوقيع الحالي يعيد `Task.FromResult(...)` (سطر 122). بعد إضافة `await`، سيصبح الأسلوب `async Task<decimal>` طبيعياً — الحلقة `foreach` يجب أن تصبح `async` friendly. **التعديل على التوقيع في الواجهة `IPatientService`**: يبقى `Task<decimal>` كما هو (سطر 18 في `IPatientService.cs`) — لا تغيير في العقد الخارجي.

5. حذف التعليق التوثيقي القديم `// In F1: use PatientTestRow.Price directly ...`.

**متى بالضبط**: Part 7.10 يأتي **بعد** Part 7.2 (Migration للـ `ReferralPrices`) و **قبل** Part 7.11 (تحديث `PatientEntryViewModel`) — لضمان أن أي إعادة حساب في الشاشة تعتمد على السلوك النهائي.

### 3.4 التصنيف الرباعي (Retro-Integration Impact Audit)

| البند | التصنيف | التفاصيل |
|---|---|---|
| `ICurrentUserService` كمصدر Admin في `LabTestService.DeleteAsync` | **Matching** | يُستخدم بنفس النمط الفعلي في `PatientService.DeleteAsync` (تم فحصه — سطور 40–53). لا يُخترع مصدر جديد. |
| توقيع `IPatientService.CalculateTotalAsync` | **Matching** | يبقى كما هو من F1 (يقبل `Referral?`) — تم تصميمه في Part 1.6 مسبقاً لدعم قرار 15، والآن يُفعَّل داخلياً بلا تغيير عقد خارجي. |
| توقيع `IPatientService.CalculateTotalAsync` — `Referral?` argument | **Matching** | مطابق تماماً للـ interface الفعلي الحالي (سطر 18). |
| `PatientEntryViewModel.AvailableTests` نوع Collection | **Needs Work** | يحتاج تعديل من `ObservableCollection<LabTestPlaceholder>` إلى `ObservableCollection<LabTest>`. |
| `PatientEntryViewModel` — Constructor Injection لـ `ILabTestService` | **Missing** | غير موجود حالياً — يُضاف في Part 7.11. يستوجب أيضاً تحديث تسجيل DI في `App.xaml.cs` (Part 7.5 سيسجّل الـ Service). |
| `PatientEntryViewModel.AddSelectedTestCommand` | **Missing** | Command موجود شكلياً لكن جسمه فارغ (`// TODO`). يُملأ في Part 7.11. |
| `PatientService.CalculateTotalAsync` — `LabToLab` branch | **Needs Work** | يحتاج استبدال منطق `total += test.Price` بمنطق استعلام `ReferralPrices` مع fallback. |
| ملف `LabTestPlaceholder.cs` | **Needs Work** | يُحذف كلياً في Part 7.11. |
| Migration `AddPatientsAndReferrals` — هل يحتاج تعديل؟ | **Matching** | لا — الجداول القائمة (`Patients`, `Referrals`, `SpecimenTypes`, `PatientVisits`) لا تُلمس. Migration جديدة `AddLabTestsAndReferralPrices` تُضاف بجانبها. |
| Fluent API القائمة في `NewLabDbContext` | **Matching** | تبقى كما هي — F7 يُضيف قواعد جديدة داخل نفس `OnModelCreating` دون تعديل القواعد السابقة. |
| Seed data للـ `Referral` الافتراضي (المعمل) | **Matching** | يبقى — لا يُلمس. F7 يضيف Seed لـ `TestGroups` و عينات `LabTests` فقط. |

---

## 4) Future-Impact Awareness نحو F8 وبقية الوظائف

هذا القسم يُحدِّد الأثر التصميمي الملزم مراعاته الآن — دون بناء منطق F8/F2/F3/F4/F5/F6.

### 4.1 F8 (المعدلات الطبيعية — Step 3 of 8) — الأثر المباشر
- **زر "الانتقال لنافذة المعدل الطبيعي"** في `LabTestManagementView` (منطقة 8 من نص المرجع، Part 7.7):
  - **الأمر في ViewModel**: `OpenNormalRangeCommand` (تُصرَّح في Part 7.6).
  - **التنفيذ الآن (Signature-Shaping فقط)**: يستدعي `_dialogService.ShowMessage("Info", "ستُفعَّل هذه الوظيفة في Function 8")` — نفس النمط المستخدم في F1 لـ `PrintBarcode` (سطور 187–191 في `PatientEntryViewModel.cs`).
  - **الأثر التصميمي المُراعى**: الأمر يحتاج معرفة `SelectedTest.Id` وقت التنفيذ الفعلي في F8. هذا موجود أصلاً كـ `[ObservableProperty] private LabTest? selectedTest;`. لا يحتاج F7 أي إعداد إضافي في الـ ViewModel لدعم F8.
  - **CanExecute**: يعيد `SelectedTest != null` — يمنع فتح النافذة قبل اختيار تحليل. هذا سلوك دائم لا يحتاج تعديل عند بناء F8.
  - **⚠️ ما لا يُبنى الآن**: لا `INormalRangeService`، لا كيان `NormalRange`، لا `NormalRangeView` — كلها من نطاق F8.

### 4.2 F2 (الباركود — Step 4 of 8)
- كيان `LabTest` يحتوي `DefaultSpecimenTypeId` (FK اختياري إلى `SpecimenType`) — سيُستخدم في F2 لتجميع الملصقات حسب نوع العينة. **الأثر الآن**: التأكد من أن العلاقة معرَّفة صحيحاً في Fluent API لـ Part 7.2. لا كود F2 يُبنى.

### 4.3 F3 و F4 (نتائج التحاليل)
- `LabTest.IsMainTest` + `LabTestElement.ParentLabTestId` — بروفيلات متعددة العناصر. **الأثر الآن**: كيان `LabTestElement` يُبنى كاملاً في Part 7.1 حتى يستهلكه F4 لاحقاً بلا تعديل بنيوي.
- `LabTest.IsSeeReport`, `IsPrintWithOther`, `IsRoutine`, `IsAddWithGroup` — flags تتحكم في سلوك F3/F4. **الأثر الآن**: تُخزَّن كأعمدة `bool` في الجدول (Part 7.2) — لا منطق تشغيلي يُبنى.

### 4.4 F5 (تسليم النتائج)
- لا أثر مباشر من F7 — F5 تعمل على `Patient` و `PatientTest` (كيان مؤجَّل لـ F3).

### 4.5 F6 (البحث)
- `LabTest.Code` سيُستخدم كأحد حقول البحث في F6 (`GetPatientsGroupResultsAsync`). **الأثر الآن**: التأكد من `Unique Index` على `LabTest.Code` في Part 7.2.

### 4.6 قاعدة عامة (نظير Handoff_Slice_1_2.md)
**ممنوع** إنشاء أي stub منطقي (بلا Service بلا كيان) في F7 من أجل F8 أو غيرها. الحد الأقصى المسموح: **`_dialogService.ShowMessage("Info", ...)` لأي زر يستدعي وظيفة لاحقة**.

---

## 5) الأجزاء التفصيلية

> نفس عدد الأجزاء الوارد حرفياً في `analysis_and_plan_v3.md` (9 أجزاء من Part 7.1 إلى Part 7.9)، **بالإضافة إلى جزئين retro-integration مصنَّفَين بأرقام امتدادية Part 7.10 و Part 7.11** — لأنهما فعلياً **تعديلات على ملفات شريحة سابقة (F1)** لا إنشاء بنية جديدة في F7. لم يُدمج أي جزء أصلي، ولم يُقسَّم، ولم يُضَف جزء جديد يوسِّع نطاق F7 نفسه.

### Part 7.1 — كيانات المرجعية (بدون Branch — قرار 5؛ مع ReferralPrice — قرار 15)

- **الطبقة**: Model / Domain.
- **الملفات (جديدة)**:
  - `Models/Domain/TestGroup.cs`:
    - Properties: `int Id`, `string Name` (Required, MaxLength 150), `string? LogGroup` (MaxLength 100).
    - **لا حقل `Branch`** (قرار 5).
    - Navigation: `ICollection<LabTest> Tests` (init `new List<LabTest>()`).
  - `Models/Domain/LabTest.cs` (17 حقلاً — بدون Branch):
    - `int Id`
    - `string Code` (Required, MaxLength 50) — سيُصبح Unique في Part 7.2.
    - `string TestName` (Required, MaxLength 200) — Test name (اسم البرنامج الداخلي).
    - `string ReportNameLarge` (MaxLength 200)، `string? ReportNameSmall` (MaxLength 200) — Report name (جزءان).
    - `string BillNameLarge` (MaxLength 200)، `string? BillNameSmall` (MaxLength 200) — Bill name.
    - `string HistoryName` (MaxLength 100) — History name.
    - `string ArabicName` (MaxLength 200) — Arabic name.
    - `int? TestGroupId` — Group name → FK إلى `TestGroup`.
    - **لا حقل `Branch`** (قرار 5).
    - `string? LogGroup` (MaxLength 100) — Log group.
    - `string? Collection` (MaxLength 500) — Collection (شروط سحب العينة).
    - `int TestTimeDays` (default 0) — Test time.
    - `int ArrangeNumber` — Arrange number.
    - `decimal PatientPrice` (decimal(18,2)) — Patient price.
    - `decimal LabToLabPrice` (decimal(18,2)) — Lab to Lab price.
    - `bool IsRoutine` — Routine test.
    - `bool IsSeeReport` — See report.
    - `bool IsPrintWithOther` — Print with other.
    - `bool IsAddWithGroup` — Add with group.
    - `bool IsMainTest` — Main test.
    - `int? ParentLabTestId` — Self-reference (لعناصر تحليل رئيسي).
    - `int? DefaultSpecimenTypeId` — FK إلى `SpecimenType` (منطقة 4 من المرجع).
    - `bool IsSentExternal` — علامة "يُرسل للخارج" (منطقة 5).
    - `int? ExternalReferralId` — FK إلى `Referral` (اسم الجهة المرسل إليها).
    - `decimal? ExternalCost` — تكلفة الإرسال الخارجي.
    - `string? PromptQuestion` (MaxLength 500) — الأسئلة/التنبيهات (منطقة 6).
    - `bool IsActive` (default true).
    - Navigation properties:
      - `TestGroup? TestGroup`
      - `LabTest? ParentLabTest`
      - `SpecimenType? DefaultSpecimenType`
      - `Referral? ExternalReferral`
      - `ICollection<LabTestElement> Elements`
      - `ICollection<ReferralPrice> ReferralPrices`
  - `Models/Domain/LabTestElement.cs`:
    - `int Id`, `int ParentLabTestId`, `string Name` (Required, MaxLength 200), `int ArrangeNumber`, `bool IsMainTest`.
    - Navigation: `LabTest ParentLabTest = null!`.
  - `Models/Domain/ReferralPrice.cs` (قرار 15):
    - `int Id` (PK مستقل — سبب: تسهيل تحديث/حذف السجل بـ Id واحد؛ Unique Index على `(LabTestId, ReferralId)` يُضاف في Part 7.2 لمنع التكرار — كما أشار الوثيقة "PK مستقل مع Index فريد").
    - `int LabTestId`, `int ReferralId`, `decimal Price` (decimal(18,2)).
    - Navigation: `LabTest LabTest = null!`, `Referral Referral = null!`.
- **التبعيات**: كيان `Referral` و `SpecimenType` الموجودان أصلاً من F1 (تم فحصهما).
- **الناتج المتوقع**: 4 ملفات كيان جديدة تُترجم بلا أخطاء.
- **Build Gate**: `dotnet build` — 0 errors / 0 warnings (لا Migration بعد).

### Part 7.2 — Migration

- **الطبقة**: Data.
- **الملفات المعدَّلة**:
  - `Data/NewLabDbContext.cs`:
    - إضافة `DbSet<TestGroup> TestGroups`, `DbSet<LabTest> LabTests`, `DbSet<LabTestElement> LabTestElements`, `DbSet<ReferralPrice> ReferralPrices`.
    - Fluent API الإضافي داخل `OnModelCreating` (بعد الفقرات القائمة 1–9، بلا لمس الحالي):
      - `LabTest.Code` → Unique Index (استعداداً لـ F6 — Future Impact 4.5).
      - `LabTest.TestGroupId` → FK `Restrict`.
      - `LabTest.ParentLabTestId` → FK self-reference `Restrict`.
      - `LabTest.DefaultSpecimenTypeId` → FK `Restrict`.
      - `LabTest.ExternalReferralId` → FK `Restrict`.
      - `LabTest.PatientPrice`, `LabToLabPrice`, `ExternalCost` → `decimal(18,2)`.
      - `LabTestElement.ParentLabTestId` → FK `Cascade` (حذف تحليل رئيسي يحذف عناصره).
      - `ReferralPrice.LabTestId` → FK `Cascade` (حذف التحليل يحذف أسعاره).
      - `ReferralPrice.ReferralId` → FK `Restrict`.
      - `ReferralPrice` → Unique Index على `(LabTestId, ReferralId)`.
    - Seed:
      - 3 `TestGroup` افتراضية (Chemistry, Hematology, Urine) — بلا حقل Branch.
      - ~3 `LabTest` تجريبية بسيطة (Glucose, Hemoglobin, Urine Analysis) — للاختبار اليدوي في Part 7.9.
- **الملفات الجديدة**:
  - `Migrations/<timestamp>_AddLabTestsAndReferralPrices.cs` (+ Designer + تحديث `NewLabDbContextModelSnapshot.cs`).
- **التبعيات**: Part 7.1.
- **الناتج المتوقع**: قاعدة البيانات تحتوي 4 جداول جديدة بعد `dotnet ef database update`. Migration ProductVersion = `8.0.8` (تحذير مستند من Technical Note 6 في history.md).
- **Build Gate**: `dotnet build` — 0 errors / 0 warnings + `dotnet ef migrations add AddLabTestsAndReferralPrices -c NewLabDbContext` ينجح بلا تحذيرات.

### Part 7.3 — Service Interface: ILabTestService (مع دعم ReferralPrices — قرار 15)

- **الطبقة**: Service / Interfaces.
- **الملفات (جديدة)**: `Services/Interfaces/ILabTestService.cs`.
- **الأساليب**:
  - `Task<List<LabTest>> GetAllAsync()`
  - `Task<List<LabTest>> SearchAsync(string? code, string? groupName, string? testName)` — 3 خانات بحث (منطقة 2 من المرجع).
  - `Task<LabTest?> GetByIdAsync(int labTestId)`
  - `Task<LabTest> AddAsync(LabTest labTest)`
  - `Task<LabTest> UpdateAsync(LabTest labTest)`
  - `Task DeleteAsync(int labTestId)` — يفحص `ICurrentUserService.IsAdmin` داخلياً (مبدأ نفسه المطبَّق في `PatientService.DeleteAsync`؛ رغم أن نص المرجع لا يذكر صراحة أن حذف تحليل يحتاج Admin، فإن الاتساق مع F1 يقتضي ذلك — انظر **Clarification Point CP-3**).
  - `Task<List<LabTestElement>> GetElementsAsync(int parentLabTestId)`
  - `Task<List<LabTest>> GetRoutineTestsAsync()` — يستخدمها `PatientEntryViewModel` في Part 7.11.
  - `Task<List<LabTest>> GetByGroupAsync(int testGroupId)`
  - **قرار 15 — أساليب ReferralPrices**:
    - `Task<List<ReferralPrice>> GetReferralPricesAsync(int labTestId)`
    - `Task SetReferralPriceAsync(int labTestId, int referralId, decimal price)` — Upsert (Update if exists, else Add).
    - `Task DeleteReferralPriceAsync(int labTestId, int referralId)`.
- **التبعيات**: Part 7.1.
- **الناتج المتوقع**: عقد الخدمة جاهز للاستخدام في ViewModels.
- **Build Gate**: `dotnet build` — 0 errors / 0 warnings.

### Part 7.4 — FluentValidation: LabTestValidator

- **الطبقة**: Model / Validation.
- **الملفات (جديدة)**: `Models/Validation/LabTestValidator.cs`.
- **القواعد**:
  - `Code` — `NotEmpty`, `MaximumLength(50)`.
  - `TestName` — `NotEmpty`, `MaximumLength(200)`.
  - `ReportNameLarge` — `NotEmpty`, `MaximumLength(200)`.
  - `PatientPrice` — `GreaterThanOrEqualTo(0)`.
  - `LabToLabPrice` — `GreaterThanOrEqualTo(0)`.
  - `ExternalCost` — `GreaterThanOrEqualTo(0).When(l => l.IsSentExternal)`.
  - `TestTimeDays` — `GreaterThanOrEqualTo(0)`.
  - `ArrangeNumber` — `GreaterThanOrEqualTo(0)`.
  - **Uniqueness of `Code`**: لا تُفحَص في FluentValidator (يُترك للـ Unique Index في قاعدة البيانات) — نفس الأسلوب المُتَّبَع مع `LabId`/`FileCode` في F1 (فحص الكود الفعلي).
- **التبعيات**: Part 7.1.
- **Build Gate**: `dotnet build` — 0/0.

### Part 7.5 — Implementation + DI

- **الطبقة**: Service / Implementations + DI Container.
- **الملفات (جديدة)**: `Services/Implementations/LabTestService.cs`.
- **الحقن (Constructor)**:
  - `NewLabDbContext context`
  - `ICurrentUserService currentUserService` (لفحص Admin في `DeleteAsync` — راجع CP-3).
- **منطق DeleteAsync**: إن لم يكن `IsAdmin` → `UnauthorizedAccessException` — رسالة عربية موحَّدة مع نمط `PatientService` ("عملية الحذف تتطلب صلاحية Admin").
- **منطق SetReferralPriceAsync**:
  - Upsert: بحث بـ `(LabTestId, ReferralId)`؛ إن وُجد → تحديث `Price`؛ إن لم يُوجد → إضافة سجل جديد.
- **منطق SearchAsync**:
  - كل معيار اختياري (nullable/empty tolerant).
  - `code` → `Where(l => EF.Functions.Like(l.Code, $"{code}%"))`.
  - `groupName` → `Where(l => l.TestGroup != null && EF.Functions.Like(l.TestGroup.Name, $"%{groupName}%"))`.
  - `testName` → `Where(l => EF.Functions.Like(l.TestName, $"%{testName}%") || EF.Functions.Like(l.ArabicName, $"%{testName}%"))`.
  - النمط مُطابق لـ `ReferralService.SearchByNameAsync` الفعلي (تم فحصه).
- **الملفات المعدَّلة**:
  - `App.xaml.cs`:
    - `services.AddScoped<ILabTestService, LabTestService>();`
    - `services.AddScoped<IValidator<LabTest>, LabTestValidator>();`
- **التبعيات**: Parts 7.1, 7.2, 7.3, 7.4.
- **Build Gate**: `dotnet build` — 0/0.

### Part 7.6 — LabTestManagementViewModel

- **الطبقة**: ViewModel.
- **الملفات (جديدة)**: `ViewModels/Pages/LabTestManagementViewModel.cs`.
- **الحقن (Constructor Injection)**:
  - `ILabTestService labTestService`
  - `IReferralService referralService` (لملء قائمة الجهات في قسم ReferralPrices)
  - `IDialogService dialogService`
  - `INavigationService navigationService`
  - `IValidator<LabTest> labTestValidator`
  - `ICurrentUserService currentUserService`
- **الخصائص (`[ObservableProperty]`)**:
  - `ObservableCollection<LabTest> Tests { get; } = new();`
  - `LabTest? selectedTest;` — رأس النموذج.
  - `string? searchByCode;`, `string? searchByGroupName;`, `string? searchByTestName;`
  - `ObservableCollection<SpecimenType> AvailableSpecimenTypes { get; } = new();`
  - `ObservableCollection<Referral> AvailableReferrals { get; } = new();`
  - `ObservableCollection<TestGroup> AvailableGroups { get; } = new();`
  - `bool isEditMode;`, `bool isAddMode = true;`
  - **قرار 15**: `ObservableCollection<ReferralPrice> ReferralPrices { get; } = new();`
  - `ReferralPrice? selectedReferralPrice;`
  - `int? newReferralPrice_ReferralId;`
  - `decimal newReferralPrice_Price;`
  - `bool IsAdmin => _currentUserService.IsAdmin;` — حسب CP-3.
- **الأوامر (`[RelayCommand]`)**:
  - `SearchCommand` → يستدعي `_labTestService.SearchAsync(...)` ويحدِّث `Tests`.
  - `AddCommand` → مسح النموذج، `IsAddMode = true`, `IsEditMode = false`.
  - `EditCommand` → تعبئة النموذج من `SelectedTest`.
  - `DeleteCommand(CanExecute=IsAdmin)` → تأكيد + استدعاء `_labTestService.DeleteAsync`.
  - `SaveCommand` → validate → Add/Update.
  - `CancelCommand` → مسح النموذج.
  - `OpenNormalRangeCommand(CanExecute=SelectedTest!=null)` → placeholder إلى F8 (`_dialogService.ShowMessage`) — راجع القسم 4.1.
  - `BackCommand` → إغلاق الشاشة (يمكن استخدام `INavigationService.GoBack` إن كان history متوفراً — راجع CP-1).
  - **قرار 15**:
    - `AddReferralPriceCommand(CanExecute = SelectedTest != null && NewReferralPrice_ReferralId != null && NewReferralPrice_Price >= 0)` → `_labTestService.SetReferralPriceAsync`.
    - `DeleteReferralPriceCommand(CanExecute = SelectedReferralPrice != null)`.
- **التبعيات**: Parts 7.3, 7.5.
- **Build Gate**: `dotnet build` — 0/0.
- **ملاحظة تسمية (Technical Note 2 من history.md)**: لا يوجد تعارض `[ObservableProperty]` مع `[RelayCommand]` هنا لأن جميع الخصائص Prefixed بشكل مختلف.

### Part 7.7 — LabTestManagementView + LatinSymbolsPad (قرار 14)

- **الطبقة**: View.
- **الملفات (جديدة)**:
  - `Views/Controls/LatinSymbolsPad.xaml` + `.xaml.cs` — **UserControl مستقل قابل للتوسعة (قرار 14)**:
    - محتوى: `ItemsControl` يحتوي `WrapPanel` من `Button` لكل رمز في المجموعة الافتراضية `α β γ μ ± ≤ ≥ °`.
    - يعرِّض `DependencyProperty TargetTextBox` (نوع `TextBox`) — يربطه المستهلك بمرجع `TextBox` المُركَّز عليه.
    - في `Button.Click` code-behind: يُدرج النص عبر `TargetTextBox.SelectedText = symbol; TargetTextBox.Focus();`.
    - قابل للتوسعة: قائمة الرموز في `DependencyProperty` قائم بذاته من نوع `IEnumerable<string>` مع قيمة افتراضية للمجموعة الحالية — إضافة رموز مستقبلاً = تعديل القيمة الافتراضية فقط، بلا لمس المستهلكين.
    - **Future Impact نحو F8**: نفس UserControl سيُستخدم داخل `NormalRangeView` — التصميم قابل لإعادة الاستخدام بلا تعديل.
  - `Views/Pages/LabTestManagementView.xaml` + `.xaml.cs`:
    - `UserControl`, `FlowDirection="RightToLeft"`.
    - `InputBindings`: `Escape` → `CancelCommand`؛ لا اختصار F مخصَّص (نص المرجع لا يذكر اختصارات).
    - **تخطيط ثلاثة أعمدة** (يُطابق نمط `PatientEntryView` الفعلي):
      - **العمود 1 (يمين)**: قائمة كل التحاليل (`ListBox ItemsSource="{Binding Tests}" SelectedItem="{Binding SelectedTest}" DisplayMemberPath="TestName"`) + 3 حقول بحث (بحث بالكود / اسم المجموعة / اسم التحليل) + زر Search.
      - **العمود 2 (وسط)**: نموذج بيانات التحليل (17 حقلاً بدون Branch، MaterialDesign OutlinedTextBox + CheckBoxes + ComboBoxes) + SpecimenType selector + إعدادات "يُرسل للخارج" + حقل الأسئلة.
        - ComboBoxes (`TestGroup`, `DefaultSpecimenType`, `ExternalReferral`) — تُملأ عبر ItemsSource من `AvailableGroups/AvailableSpecimenTypes/AvailableReferrals` (لا `x:Array` — Technical Note 4 من history.md).
        - **لوحة الحروف اللاتينية**: عنصر `<controls:LatinSymbolsPad TargetTextBox="{Binding ElementName=FocusedTextBox}" />` يظهر أسفل الحقول النصية الرئيسية.
      - **العمود 3 (يسار)**: قسم أسعار الجهات (قرار 15):
        - `DataGrid ItemsSource="{Binding ReferralPrices}" SelectedItem="{Binding SelectedReferralPrice}"` مع أعمدة (اسم الجهة، السعر).
        - أسفله: ComboBox لاختيار جهة + TextBox للسعر + زرا `AddReferralPriceCommand` و `DeleteReferralPriceCommand`.
    - **شريط الأوامر الأسفل** (WrapPanel): إضافة / تعديل / حذف / حفظ / إلغاء / **الانتقال للمعدل الطبيعي** (Placeholder F8) / رجوع.
    - `code-behind` (`.xaml.cs`) محصور بتهيئة enum ComboBoxes (إن لزم) — نفس نمط `PatientEntryView.xaml.cs`.
- **التبعيات**: Part 7.6.
- **Build Gate**: `dotnet build` — 0/0 (تشغيل `dotnet clean` قبله للتأكد من عدم إخفاء تحذيرات XAML — Technical Note 5 في history.md).

### Part 7.8 — تحديث الـ Toolbar

- **الطبقة**: ViewModel (Shell) + DataTemplate.
- **الملفات المعدَّلة**:
  - `ViewModels/Pages/MainDashboardViewModel.cs`:
    - داخل `CreateToolbarCategories()` — فئة `SystemData` (سطور 148–152 حالياً): استبدال `Functions = new List<FunctionDefinition>()` (فارغة) بقائمة تحتوي:
      - `new FunctionDefinition { Name = "بيانات التحاليل", IconName = "TestTube", TargetViewType = typeof(LabTestManagementView) }`.
    - داخل `OpenFunction(FunctionDefinition? function)` (سطور 47–63): توسيع الشرط `if (function.TargetViewType == typeof(PatientEntryView))` ليدعم أيضاً `typeof(LabTestManagementView)` — بأسلوب `else if` مماثل يستدعي `_navigationService.NavigateTo<LabTestManagementViewModel>()`. **بديل أنظف** (يستحق مراجعة بعد Build Verification): تحويل الشرط إلى Dictionary-based mapping، لكن هذا **خارج نطاق F7** ويجب مناقشته كـ Clarification.
  - `App.xaml`:
    - إضافة `xmlns:vmpages="clr-namespace:NewLab.ViewModels.Pages"` — موجود أصلاً (سطر 7 حالياً — تم فحصه).
    - إضافة `xmlns:views="clr-namespace:NewLab.Views.Pages"` — موجود أصلاً (سطر 8).
    - إضافة `DataTemplate` جديد بعد `PatientEntryViewModel` (سطور 18–20):
      ```
      <DataTemplate DataType="{x:Type vmpages:LabTestManagementViewModel}">
          <views:LabTestManagementView />
      </DataTemplate>
      ```
  - `App.xaml.cs`:
    - `services.AddTransient<LabTestManagementViewModel>();` (بعد `PatientEntryViewModel` — سطر 48 حالياً).
- **التبعيات**: Parts 7.5, 7.6, 7.7.
- **Build Gate**: `dotnet build` — 0/0.

### Part 7.9 — Build Verification (نهاية بناء F7 الأصلي)

- **البناء**: `dotnet clean && dotnet build` — 0 errors / 0 warnings (Technical Note 5 يفرض `clean` كامل قبل التحقق النهائي).
- **Migration Apply**: `dotnet ef database update -c NewLabDbContext` — يجب أن يُضيف 4 جداول جديدة و 2 unique indexes، دون أخطاء.
- **اختبار يدوي**:
  1. Login كـ Admin → Toolbar → "بيانات النظام" → "بيانات التحاليل".
  2. تظهر النافذة مع Seed data (3 تحاليل تجريبية).
  3. اختيار Glucose → عرض بياناته في العمود الأوسط.
  4. تعديل `LabToLabPrice` → حفظ → التحقق في DB (`SELECT LabToLabPrice FROM LabTests WHERE Code='...'`).
  5. إضافة `ReferralPrice` جديد لـ Glucose × جهة "المعمل" (Seed من F1) بسعر مخصَّص → التحقق في DB (`SELECT * FROM ReferralPrices`).
  6. Login كـ Technician → زر الحذف معطَّل (CP-3).
- **⚠️ ملاحظة**: Part 7.9 لا يُغلق الشريحة — يجب تنفيذ Parts 7.10 و 7.11 قبل اعتبار F7 مكتملاً، وإلا يبقى `LabTestPlaceholder` مستخدماً في F1 والحساب الحقيقي لأسعار الجهات معطَّلاً.

### Part 7.10 — [RETRO] تفعيل استعلام ReferralPrices في PatientService.CalculateTotalAsync

- **الطبقة**: Service / Implementation (تعديل على ملف من F1).
- **الملفات المعدَّلة**:
  - `Services/Implementations/PatientService.cs` — الحلقة داخل `CalculateTotalAsync` (سطور 82–123):
    - استبدال branch `case BillingSystem.LabToLab` وفق الكود الوارد في القسم 3.3 أعلاه.
    - حذف التعليق التوثيقي `// In F1: use PatientTestRow.Price directly ...`.
    - تحويل الحلقة إلى `async` واستخدام `await` — إزالة `Task.FromResult` (سطر 122) والاعتماد على `async Task<decimal>`.
- **التبعيات**: Part 7.2 (وجود جدول `ReferralPrices` و DbSet).
- **بلا تغيير في العقد**: توقيع `IPatientService.CalculateTotalAsync` يبقى كما هو (سطر 18 في `IPatientService.cs`).
- **Build Gate**: `dotnet build` — 0/0.
- **اختبار يدوي**:
  1. إضافة ReferralPrice تجريبي: Glucose × جهة X بسعر 25.
  2. فتح `PatientEntryView`، اختيار BillingSystem=LabToLab، اختيار جهة X.
  3. إضافة تحليل Glucose (سعر Lab-to-Lab الافتراضي = 30).
  4. **النتيجة المتوقعة**: `TotalAmount = 25` (سعر الجهة الخاص) — لا 30.

### Part 7.11 — [RETRO] استبدال LabTestPlaceholder + تفعيل AddSelectedTestCommand

- **الطبقة**: ViewModel (تعديل على ملف من F1) + View (تعديل بسيط) + حذف ملف.
- **الملفات المحذوفة**: `ViewModels/Pages/LabTestPlaceholder.cs`.
- **الملفات المعدَّلة**:
  - `ViewModels/Pages/PatientEntryViewModel.cs`:
    - Constructor: إضافة `ILabTestService labTestService` وحفظه في `_labTestService`.
    - سطر 65: تغيير `ObservableCollection<LabTestPlaceholder>` → `ObservableCollection<LabTest>`.
    - إضافة `[ObservableProperty] private LabTest? selectedAvailableTest;`
    - سطر 194 (`AddSelectedTest`): تنفيذ فعلي (وفق التفصيل في القسم 3.2 نقطة 2 أعلاه).
    - سطر 200 (`RemoveTest`): تغيير التوقيع من `LabTestPlaceholder?` إلى `LabTest?` — يبقى المنطق: `SelectedTests.FirstOrDefault(t => t.LabTestId == test.Id)`.
    - إضافة `LoadAvailableTestsAsync` — يُستدعى في constructor أو بأول فتح للشاشة (يمكن إتباع نمط `OnActivated` لاحقاً، لكن الأبسط: استدعاء في constructor بعد الحقن — راجع CP-2).
    - إضافة `using NewLab.Models.Domain;`.
  - `App.xaml.cs`: **لا تغيير مطلوب** — تسجيل `ILabTestService` تم في Part 7.5.
  - `Views/Pages/PatientEntryView.xaml`:
    - `ListBox` سطر 148 — إضافة `DisplayMemberPath="TestName"` و `SelectedItem="{Binding SelectedAvailableTest}"`.
  - `Views/Pages/PatientEntryView.xaml.cs`: **لا تغيير مطلوب** — code-behind الحالي محصور بـ enums وليس بـ tests.
- **التبعيات**: Parts 7.5, 7.10.
- **Build Gate**: `dotnet build` — 0/0.
- **اختبار يدوي**:
  1. فتح `PatientEntryView` (F2).
  2. قائمة `AvailableTests` تعرض التحاليل الروتينية من DB (Seed data من Part 7.2).
  3. اختيار تحليل → زر "إضافة التحليل" → يظهر في `SelectedTests` بسعره الصحيح.
  4. `TotalAmount` يُحسب بمنطق Part 7.10 الحقيقي (يشمل استعلام ReferralPrices عند LabToLab).

---

## 6) Cross-Slice MVVM Compliance Checklist

| البند | الحالة |
|---|---|
| كل ViewModel يحقن اعتماداته عبر Constructor فقط | ✅ (Parts 7.6, 7.11) |
| لا `DbContext` داخل ViewModel | ✅ — ViewModels تستهلك `ILabTestService`, `IReferralService`, `ICurrentUserService` فقط |
| لا `new Service()` مباشرة داخل ViewModel | ✅ |
| فحص Admin يمر عبر `ICurrentUserService.IsAdmin` — لا مصدر بديل | ✅ (Parts 7.5 `LabTestService.DeleteAsync`, 7.6 `LabTestManagementViewModel.IsAdmin`) |
| `DeleteCommand.CanExecute = IsAdmin` (نمط F1) | ✅ (Part 7.6) — راجع CP-3 |
| Command placeholders لوظائف لاحقة تستخدم `IDialogService.ShowMessage` — لا تُبنى منطق مُسبَق | ✅ (Part 7.6 `OpenNormalRangeCommand`) |
| Enum ComboBoxes تُملأ في code-behind لا في XAML `x:Array` | ✅ (Part 7.7) — Technical Note 4 |
| MaterialDesign TextBox عبر `Style="{StaticResource MaterialDesignOutlinedTextBox}"` — لا Tag مخصَّص | ✅ (Part 7.7) — Technical Note 3 |
| تسجيل ViewModels كـ Transient في DI | ✅ (Part 7.8 `LabTestManagementViewModel`) |
| تسجيل Services كـ Scoped (بسبب DbContext) | ✅ (Part 7.5 `ILabTestService`) |
| تسجيل Validators كـ Scoped عبر `IValidator<T>` | ✅ (Part 7.5 `IValidator<LabTest>`) |
| DataTemplate في `App.xaml` لكل ViewModel↔View جديد | ✅ (Part 7.8) |
| تسجيل الاختصارات (F-keys) في `MainWindow.xaml` عالمياً / أو في UserControl خاصاً | N/A — F7 بلا اختصار F مخصَّص (نص المرجع لا يذكر واحداً) |
| Migration ProductVersion = `8.0.8` (نسخة المشروع) | ✅ (Part 7.2) — Technical Note 6 |
| `dotnet clean` قبل التحقق النهائي (كشف تحذيرات XAML) | ✅ (Part 7.9, 7.11) — Technical Note 5 |
| منع تعارض `[ObservableProperty]` × `[RelayCommand]` بالتسمية | ✅ — لا تعارض متوقَّع بأسماء Part 7.6 — Technical Note 2 |

---

## 7) Clarification Points

خلال الفحص الفعلي للكود ومقارنته مع نص Function 7 في `analysis_and_plan_v3.md`، برزت 3 نقاط تحتاج قراراً صريحاً من مالك المشروع قبل بدء التنفيذ. **لم تُبَتّ من تلقاء الخطة** — كل نقطة تحمل التوصية المرتبطة بها فقط للنظر.

### CP-1 — سلوك زر "رجوع" في `LabTestManagementView`

- **الوضع**: نص المرجع (منطقة 8) يذكر أزرار "اضافة تحليل / تعديل / حذف / الانتقال لنافذة المعدل الطبيعي" — بلا زر رجوع صريح. لكن `MainWindow.xaml` (سطور 122–130) يعرض `Button Content="العودة للرئيسية"` مرتبطاً بـ `CloseFunctionCommand` بشكل دائم لكل شاشة function.
- **السؤال**: هل يكفي زر "العودة للرئيسية" الشامل في `MainWindow`، أم نضيف زراً منفصلاً `BackCommand` داخل `LabTestManagementView` (كما فُعل جزئياً في نص F8)؟
- **التوصية**: الاكتفاء بزر MainWindow لتجنب ازدواجية — يظل `INavigationService.GoBack` متاحاً كـ hook للـ VM بلا زر بصري.

### CP-2 — توقيت تعبئة `AvailableTests` في `PatientEntryViewModel` (Part 7.11)

- **الوضع**: `PatientEntryViewModel` مسجَّل كـ Transient (سطر 48 في `App.xaml.cs` الفعلي). كل مرة يُفتح `PatientEntryView` يُنشأ instance جديد. Constructor sync ولا يقبل `await`.
- **السؤال**: هل نُطلق `LoadAvailableTestsAsync()` كـ `_ = LoadAvailableTestsAsync();` (fire-and-forget) من داخل constructor، أم عبر آلية `OnLoaded` من الـ View؟
- **التوصية**: fire-and-forget من constructor للحفاظ على النمط الحالي في `PatientEntryViewModel` (سطور 254, 258 تستخدم نفس النمط: `_ = RecalculateTotalAsync();`).

### CP-3 — صلاحية حذف تحليل (`LabTestService.DeleteAsync`) — Admin only؟

- **الوضع**: نص Function 7 في `analysis_and_plan_v3.md` يذكر زر "حذف" بدون قيد صلاحية صريح، وجدول القرارات لا يحتوي على قرار خاص بحذف التحاليل (القرارات 2, 7, 11, 13, 16 تتعلق بحذف مرضى / زر ب و ت / زر مستلمة / حذف مرضى في F6 / أضيق مدى). لكن روح القرارات هي: أي حذف بيانات = Admin only.
- **السؤال**: هل يُتَعامل حذف تحليل كـ Admin-only اتساقاً مع نمط F1/F6، أم يُترك متاحاً لكل الأدوار؟
- **التوصية**: **Admin-only** — اتساق مع F1 (`PatientService.DeleteAsync` سطور 40–46) واستخدام نفس رسالة الخطأ. هذه توصية تنفيذية مضمَّنة في Parts 7.5 و 7.6 و Cross-Slice Checklist، لكن تعتمد صلاحية النقل النهائية على قرار مالك المشروع. إن تقرر رفض هذه التوصية، تُحذف فقط: (أ) فحص Admin داخل `LabTestService.DeleteAsync`، (ب) `IsAdmin` من `LabTestManagementViewModel` و `CanExecute` لـ `DeleteCommand` — لا يؤثر على أي جزء آخر من الخطة.

---

## 8) Change Manifest

### 🆕 Created (New files — 12 files)

| # | المسار | الطبقة | الجزء |
|---|---|---|---|
| 1 | `Models/Domain/TestGroup.cs` | Model | 7.1 |
| 2 | `Models/Domain/LabTest.cs` | Model | 7.1 |
| 3 | `Models/Domain/LabTestElement.cs` | Model | 7.1 |
| 4 | `Models/Domain/ReferralPrice.cs` | Model | 7.1 |
| 5 | `Models/Validation/LabTestValidator.cs` | Validation | 7.4 |
| 6 | `Migrations/<timestamp>_AddLabTestsAndReferralPrices.cs` (+ Designer) | Data | 7.2 |
| 7 | `Services/Interfaces/ILabTestService.cs` | Service Interface | 7.3 |
| 8 | `Services/Implementations/LabTestService.cs` | Service Impl | 7.5 |
| 9 | `ViewModels/Pages/LabTestManagementViewModel.cs` | ViewModel | 7.6 |
| 10 | `Views/Pages/LabTestManagementView.xaml` + `.xaml.cs` | View | 7.7 |
| 11 | `Views/Controls/LatinSymbolsPad.xaml` + `.xaml.cs` | View (Reusable UserControl) | 7.7 |
| 12 | (تحديث `NewLabDbContextModelSnapshot.cs`) | Data | 7.2 |

### ✏️ Modified — Native F7 files (3 files)

| # | المسار | التعديل | الجزء |
|---|---|---|---|
| 1 | `Data/NewLabDbContext.cs` | +4 DbSets + Fluent API 6 قواعد + Seed 3 مجموعات + 3 تحاليل | 7.2 |
| 2 | `App.xaml.cs` | +Scoped `ILabTestService`, +Scoped `IValidator<LabTest>`, +Transient `LabTestManagementViewModel` | 7.5, 7.8 |
| 3 | `App.xaml` | +DataTemplate `LabTestManagementViewModel → LabTestManagementView` | 7.8 |
| 4 | `ViewModels/Pages/MainDashboardViewModel.cs` | +FunctionDefinition "بيانات التحاليل"، توسيع `OpenFunction` liftup | 7.8 |

### ⚠️ Modified — Retro-Integration files (من شريحة سابقة F1 — 3 ملفات)

| # | المسار | التعديل الرجعي | الجزء |
|---|---|---|---|
| 1 | `Services/Implementations/PatientService.cs` | استبدال منطق `LabToLab` في `CalculateTotalAsync` بالاستعلام الحقيقي من `_context.ReferralPrices` (قرار 15) — حذف تعليق `// In F1: ...` | 7.10 |
| 2 | `ViewModels/Pages/PatientEntryViewModel.cs` | تغيير نوع `AvailableTests` إلى `ObservableCollection<LabTest>` — حقن `ILabTestService` في constructor — تعبئة `AddSelectedTestCommand` — إضافة `SelectedAvailableTest` — إضافة `LoadAvailableTestsAsync` — تعديل توقيع `RemoveTest(LabTest?)` — إزالة import للـ Placeholder | 7.11 |
| 3 | `Views/Pages/PatientEntryView.xaml` | إضافة `DisplayMemberPath="TestName"` و `SelectedItem="{Binding SelectedAvailableTest}"` على `ListBox Grid.Row="2"` | 7.11 |

### 🗑️ Deleted (1 file)

| # | المسار | السبب | الجزء |
|---|---|---|---|
| 1 | `ViewModels/Pages/LabTestPlaceholder.cs` | استبدل بالكيان الحقيقي `LabTest` | 7.11 |

### ⚪ Untouched (verified files remaining stable)

- `Models/Domain/Patient.cs`, `PatientVisit.cs`, `Referral.cs`, `SpecimenType.cs` — كيانات F1 الطبية (تم فحصها).
- `Models/Domain/Enums/{Gender,BillingSystem,AgeUnit,TestStatus,CodeType}.cs` — Enums F1.
- `Models/Domain/{User,Role,UserRole,ToolbarItem,FunctionDefinition}.cs` — كيانات ما قبل F1.
- `Models/Validation/PatientValidator.cs` — لا تغيير.
- `Services/Interfaces/{IApplicationStartupService,IAuthService,ICurrentUserService,IDialogService,INavigationService,IPatientService,IReferralService}.cs`.
- `Services/Implementations/{ApplicationStartupService,AuthService,CurrentUserService,DialogService,NavigationService,ReferralService}.cs`.
- `ViewModels/Pages/{SetupViewModel,LoginViewModel,IconNameToKindConverter}.cs`.
- `ViewModels/Base/ViewModelBase.cs`.
- `Views/Windows/{SetupView,LoginView,MainWindow}.xaml/.xaml.cs` — بلا تغيير في هذه الشريحة (F3/F4 سيضيفان F-keys لاحقاً).
- `Views/Controls/{DashboardContentControl,FunctionPlaceholderControl}.xaml/.xaml.cs`.
- `Converters/InverseBoolToVisibilityConverter.cs`.
- `Migrations/20260721171559_InitialCreate.cs` و `20260722032039_AddPatientsAndReferrals.cs` — لا تُلمس؛ Migration جديدة تُضاف بجانبهما.
- `appsettings.json`, `NewLab.csproj`, `AssemblyInfo.cs`.

---

## 9) Dependency Graph بين الأجزاء

```
                                 ┌──────────────────────┐
                                 │  Part 7.1            │
                                 │  Domain Entities     │
                                 │  (TestGroup, LabTest,│
                                 │  LabTestElement,     │
                                 │  ReferralPrice)      │
                                 └──────────┬───────────┘
                                            │
                       ┌────────────────────┼────────────────────┐
                       │                    │                    │
                       ▼                    ▼                    ▼
              ┌───────────────┐    ┌────────────────┐   ┌──────────────┐
              │  Part 7.2     │    │  Part 7.3      │   │  Part 7.4    │
              │  DbContext +  │    │  ILabTestSvc   │   │  Validator   │
              │  Migration    │    │  Interface     │   │              │
              └──────┬────────┘    └────────┬───────┘   └──────┬───────┘
                     │                      │                  │
                     └───────────┬──────────┴──────────────────┘
                                 ▼
                        ┌──────────────────┐
                        │  Part 7.5        │
                        │  LabTestService  │
                        │  Impl + DI       │
                        └────────┬─────────┘
                                 │
                                 ▼
                        ┌──────────────────┐
                        │  Part 7.6        │
                        │  LabTestMgmtVM   │
                        └────────┬─────────┘
                                 │
                                 ▼
                        ┌──────────────────┐
                        │  Part 7.7        │
                        │  View +          │
                        │  LatinSymbolsPad │
                        └────────┬─────────┘
                                 │
                                 ▼
                        ┌──────────────────┐
                        │  Part 7.8        │
                        │  Toolbar +       │
                        │  DataTemplate DI │
                        └────────┬─────────┘
                                 │
                                 ▼
                        ┌──────────────────┐
                        │  Part 7.9        │
                        │  Build Verif     │
                        │  (native F7)     │
                        └────────┬─────────┘
                                 │
                    ┌────────────┴────────────┐
                    ▼                         ▼
       ┌────────────────────────┐   ┌────────────────────────┐
       │  Part 7.10 [RETRO]     │   │  Part 7.11 [RETRO]     │
       │  PatientService        │   │  PatientEntryVM +      │
       │  CalculateTotalAsync   │   │  View + delete         │
       │  (uses ReferralPrices) │   │  LabTestPlaceholder    │
       └────────────┬───────────┘   └────────────┬───────────┘
                    │                            │
                    └──────────────┬─────────────┘
                                   ▼
                          ┌──────────────────┐
                          │  Final Sign-off  │
                          │  Verification    │
                          └──────────────────┘
```

**قواعد التسلسل**:
- 7.10 يعتمد على 7.2 فقط (وجود DbSet لـ `ReferralPrices`)، لذلك يمكن نظرياً تشغيله بعد 7.2 مباشرة — لكن يُوصى بتنفيذه بعد 7.9 للحفاظ على تسلسل مستقر (بناء نظيف قبل ضرب الملفات المرتدة).
- 7.11 يعتمد على 7.5 (`ILabTestService` مسجَّل ومنفَّذ) و 7.10 (`CalculateTotalAsync` نهائي) — يجب أن يكون آخر جزء.
- ضمن 7.10 و 7.11 لا يوجد اعتماد متبادل — يمكن تنفيذهما بأي ترتيب داخلي، لكن التوصية: 7.10 قبل 7.11.

---

## 10) Sign-off Criteria

الشريحة تُعتبَر مكتملة **فقط** إذا استوفت جميع البنود التالية (بلا استثناء):

### 10.1 Build & Compilation
- [ ] `dotnet clean && dotnet build` — **0 errors, 0 warnings** بعد كل جزء من 7.1 إلى 7.11.
- [ ] `dotnet ef migrations add AddLabTestsAndReferralPrices -c NewLabDbContext` — ينجح بلا تحذيرات، ProductVersion=8.0.8.
- [ ] `dotnet ef database update -c NewLabDbContext` — يُنشئ 4 جداول جديدة + 2 unique indexes بلا خطأ.

### 10.2 Database State
- [ ] `SELECT name FROM sys.tables WHERE name IN ('TestGroups','LabTests','LabTestElements','ReferralPrices') ORDER BY name;` → 4 صفوف.
- [ ] `SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;` → 3 صفوف (InitialCreate, AddPatientsAndReferrals, AddLabTestsAndReferralPrices).
- [ ] Seed data موجود: 3 صفوف في `TestGroups` + ≥3 صفوف في `LabTests`.

### 10.3 UI Wiring
- [ ] Login → "بيانات النظام" في Toolbar → زر "بيانات التحاليل" يظهر ويفتح `LabTestManagementView`.
- [ ] النموذج يعرض 17 حقلاً بلا `Branch` — تأكيد بصري.
- [ ] `LatinSymbolsPad` تُدرج الحرف عند النقر في الحقل المُركَّز عليه.

### 10.4 Retro-Integration
- [ ] ملف `ViewModels/Pages/LabTestPlaceholder.cs` **غير موجود** (`git status` يؤكد الحذف).
- [ ] `grep -rn "LabTestPlaceholder" --include="*.cs" NewLab/` → لا مطابقات.
- [ ] فتح `PatientEntryView` (F2) → قائمة `AvailableTests` تعرض التحاليل الحقيقية من DB (لا list فارغة).
- [ ] اختيار BillingSystem=LabToLab + جهة لديها ReferralPrice خاص → `TotalAmount` يعكس السعر الخاص لا `LabToLabPrice` الافتراضي.
- [ ] اختيار BillingSystem=LabToLab + جهة بدون ReferralPrice → `TotalAmount` يعكس `LabToLabPrice` الافتراضي.
- [ ] `grep -n "In F1: use PatientTestRow.Price directly" NewLab/Services/Implementations/PatientService.cs` → لا مطابقة (تعليق F1 المؤقت حُذف).

### 10.5 Admin Authorization (CP-3 محسوم بالإيجاب)
- [ ] Login كـ Admin → زر "حذف" في `LabTestManagementView` مُفعَّل ويعمل.
- [ ] Login كـ Technician (اختبار بديل: تعديل يدوي لـ UserRoles في DB) → زر "حذف" معطَّل.

### 10.6 Placeholder toward F8
- [ ] زر "الانتقال لنافذة المعدل الطبيعي" يعرض `MessageBox` بنص "ستُفعَّل هذه الوظيفة في Function 8" — لا crash، لا محاولة فتح نافذة غير موجودة.

### 10.7 Documentation Update
- [ ] `Docs/history.md` يُحدَّث بإضافة "Phase 6: Function 7 — Execution" بعد اكتمال الشريحة (خارج نطاق هذه الخطة — يُنفَّذ من قبل المُنفِّذ بعد اكتمال Sign-off التقني).

### 10.8 Decisions Compliance
- [ ] **قرار 5**: Grep على مصدر الكود لكلمة `Branch` (خارج ملفات F1 السابقة أو Toolbar) → لا مطابقة داخل `LabTest.cs` أو `TestGroup.cs`.
- [ ] **قرار 14**: `LatinSymbolsPad.xaml` موجود ومنطقه قابل للتوسعة (`DependencyProperty` للرموز).
- [ ] **قرار 15**: `ReferralPrices` DbSet + جدول DB + استعلام فعلي في `CalculateTotalAsync` — كل الروابط الثلاثة موجودة.

---

**نهاية الوثيقة — Handoff_Slice_7_2.md**

*هذه خطة تحليل وتخطيط تنفيذي فقط. لم يُكتب أي كود، ولم يُنفَّذ أي جزء. التنفيذ يبدأ فقط بعد مراجعة مالك المشروع للنقاط الثلاث في قسم Clarification Points واتخاذ قرارات صريحة بشأنها.*
