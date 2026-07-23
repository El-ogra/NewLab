# 📊 تقرير تحليل الفجوات وخطة التنفيذ — الوظيفة 5 والوظيفة 6

**المشروع**: NewLab — نظام معمل تحاليل طبية (WPF + EF Core + Material Design)
**Commit**: `56968b429f7f49485b15e065bc7d23516ac626c8`
**رسالة الـ Commit**: *"بعد تنفيذ الوظائف الخامسة والسادسة وإزالة ملف التخطيط الخاص بهما"*
**Branch**: `main`
**تاريخ التحليل**: 2026-07-23
**المرجع**: `Real_Lab_System_Reference.pdf` (الصفحات 29–36) + `Reference_Functions_5_and_6_Detailed_Analysis.md`

---

## 📑 جدول المحتويات

- [الجزء الأول: الوظيفة الخامسة — تسليم نتائج التحاليل](#f5)
  - [1. جدول المقارنة التفصيلي — F5](#f5-table)
  - [2. النواقص في F5](#f5-missing)
  - [3. الاختلافات في F5](#f5-diffs)
  - [4. الإضافات في F5](#f5-extras)
- [الجزء الثاني: الوظيفة السادسة — البحث عن مريض](#f6)
  - [1. جدول المقارنة التفصيلي — F6](#f6-table)
  - [2. النواقص في F6](#f6-missing)
  - [3. الاختلافات في F6](#f6-diffs)
  - [4. الإضافات في F6](#f6-extras)
- [الجزء الثالث: خطة التنفيذ التفصيلية للوظيفة 5](#f5-plan)
- [الجزء الرابع: خطة التنفيذ التفصيلية للوظيفة 6](#f6-plan)
- [الجزء الخامس: ملخص القرارات الحاكمة](#decisions)

---

<a id="f5"></a>
# 🔹 الجزء الأول: الوظيفة الخامسة — تسليم نتائج التحاليل للمريض

**الصفحات المرجعية**: 29 – 32 (من أصل 63)
**الملفات المشمولة في هذا التحليل**:
- `Models/Domain/Enums/PaymentType.cs`
- `Models/Domain/Patient.cs`, `PatientTest.cs`, `PaymentTransaction.cs`, `AuditLog.cs`
- `Services/Interfaces/IDeliveryService.cs`
- `Services/Implementations/DeliveryService.cs`
- `ViewModels/Pages/DeliveryViewModel.cs`
- `Views/Pages/DeliveryView.xaml` / `DeliveryView.xaml.cs`
- `Converters/TestStatusToIconConverter.cs`
- `Helpers/BarcodeScannerListener.cs`
- `App.xaml.cs` (DI)
- `Views/Windows/MainWindow.xaml` (اختصار F6 العالمي)
- `ViewModels/Pages/MainDashboardViewModel.cs`

<a id="f5-table"></a>
## 1. جدول المقارنة التفصيلي — الوظيفة 5

### 1.1 الوصول للنافذة والاختصار العالمي

| # | المكون (حقل/زر/خاصية/اختصار/سلوك) | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|-------------------------------------|------------------|------------------------|--------|
| 1 | مسار الوصول (المرضى → تسليم نتائج المرضى) | موصوف في المرجع (Toolbar → المرضى → زر "تسليم نتائج المرضى") | موجود في `MainDashboardViewModel.CreateToolbarCategories()` تحت فئة "المرضى" كـ `FunctionDefinition` يستهدف `typeof(DeliveryView)` | ✅ مطابق |
| 2 | اختصار عالمي `F6` من أي نافذة | يفتح نافذة تسليم النتائج | `MainWindow.xaml` يحوي `<KeyBinding Key="F6" Command="{Binding OpenDeliveryCommand}" />` والـ VM يحتوي على `OpenDeliveryCommand` صحيح | ✅ مطابق |
| 3 | الحالة عند الفتح: عرض مرضى اليوم غير المستلمين تلقائياً | يعرض `TestStatus ≠ Delivered/Completed` لتاريخ اليوم فقط | `DeliveryViewModel` constructor يستدعي `_ = RefreshAsync()`؛ الفلتر الافتراضي `FilterMode = "Undelivered"`؛ `FilterDateFrom = FilterDateTo = DateTime.Today`؛ `GetUndeliveredTodayAsync` يبني `DeliveryFilter(OnlyUndelivered: true, DateFrom/DateTo = Today)` | ✅ مطابق |

### 1.2 القسم 1 — قائمة مرضى اليوم غير المستلمين

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 4 | قائمة مرضى (`ListBox`) | تعرض مرضى اليوم أو نتيجة البحث | `<ListBox ItemsSource="{Binding Patients}" SelectedItem="{Binding SelectedPatient}">` مع `DataTemplate` مخصص | ✅ مطابق |
| 5 | رمز حالة (Icon) بجوار كل مريض | 7 حالات مختلفة (7 أيقونات) | `TestStatusToIconConverter` يُرجِع 7 أيقونات: Circle / FileDocument / ArrowLeftRight / Printer / Cart / CurrencyUsd / Star، مربوطة بـ `<materialDesign:PackIcon Kind="{Binding AggregateStatus, Converter=...}" />` | ✅ مطابق |
| 6 | لون الرمز يعبر عن الحالة | يُفترض تلون الرمز | `TestStatusToColorConverter` موجود ويُطبَّق على الأيقونة (Foreground) | ✅ مطابق |
| 7 | تمييز المريض المهم (`IsImportant`) باللون الأحمر في خانة الاسم | يجب تمييز اسم المريض بالأحمر إن `IsImportant = true` | يوجد `IsImportant` في `DeliveryPatientRow`، لكن الـ `TextBlock` يستخدم فقط `FontWeight="{Binding IsImportant, Converter={x:Static converters:InverseBoolToVisibilityConverter.Instance}}"` — وهذا **خطأ منطقي**: يربط FontWeight بمحوّل Visibility، ولا يُلوّن الخط بالأحمر إطلاقاً | ❌ غير مطابق |
| 8 | نسخ الكود (`VisitCode` أو `LabId`) بضغطة واحدة بالفأرة | يجب أن يُنسخ عند الضغط مرة | لا يوجد أي منطق `MouseLeftButtonDown` أو Command مرتبط بنسخ الكود في `DeliveryView.xaml`؛ الحقول الظاهرة لا تحوي `VisitCode`/`LabId` أصلاً | ❌ غير مطابق |
| 9 | عرض `VisitCode` أو `LabId` في صف القائمة | مطلوب لعرضه ونسخه | القالب يعرض فقط `FullName` و `AttendanceNumber`؛ لا يعرض `VisitCode` أو `LabId` | ❌ غير مطابق |

#### جدول رموز الحالة السبعة — التطابق التفصيلي

| # | الحالة (المرجع) | أيقونة المرجع | التطبيق الحالي (Icon Kind) | الحالة |
|---|-----------------|--------------|----------------------------|--------|
| 10.1 | جديد غير مسجل | 🔴 دائرة حمراء | `Circle` بلون أحمر (244,67,54) | ✅ مطابق |
| 10.2 | كُتبت النتائج | 📝 ورقة | `FileDocument` بلون أزرق | ✅ مطابق |
| 10.3 | تمت مراجعة/تراجع | ↕️ أسهم | `ArrowLeftRight` بلون برتقالي | ✅ مطابق |
| 10.4 | لم تُطبع | 🖨️ طابعة | `Printer` بلون أخضر | ✅ مطابق |
| 10.5 | لم يُسلَّم | 🛒 عربة | `Cart` بلون بنفسجي | ✅ مطابق |
| 10.6 | باقي حساب | 💲 دولار/جنيه | `CurrencyUsd` بلون أصفر | ✅ مطابق |
| 10.7 | مكتمل | 🏅 نجمة | `Star` بلون فيروزي | ✅ مطابق |

### 1.3 القسم 2 — بيانات المريض المحدد

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 11 | عرض اسم المريض المحدد | مطلوب | `<TextBlock Text="{Binding SelectedPatient.FullName, FallbackValue='اختر مريضاً'}" FontSize="18" FontWeight="Bold" />` | ✅ مطابق |
| 12 | عرض الكود / الجنس / السن | مطلوب (تفاصيل المريض) | يظهر فقط `AttendanceNumber` و `TestCount`؛ لا يظهر الجنس، السن، أو الكود، أو الإحالة | ❌ غير مطابق |
| 13 | تلوين اسم المريض المهم بالأحمر | مطلوب | لا يوجد أي Trigger أو Setter يُلوّن `Foreground="Red"` عند `IsImportant = true` | ❌ غير مطابق |

### 1.4 القسم 3 — قائمة التحاليل + المبالغ

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 14 | جدول تحاليل المريض | `DataGrid` مع أعمدة | `<DataGrid ItemsSource="{Binding PatientTests}">` مع 5 أعمدة | ✅ مطابق |
| 15 | عمود اسم التحليل | مطلوب | `TestName` — Width="*" | ✅ مطابق |
| 16 | عمود الحالة | مطلوب (يعرض علامة الحالة الحالية) | `Status` كنص فقط (يعرض قيمة enum). لا يستخدم `TestStatusToIconConverter` داخل الجدول | ⚠️ جزئي |
| 17 | علامات "كُتبت / روُجعت / طُبعت / سُلِّمت" | كل علامة لها رمز | `IsPrinted` و `IsDelivered` كنص Boolean فقط (True/False)؛ لا توجد علامات لـ `IsReviewed` أو `IsEntered` كأعمدة | ⚠️ جزئي |
| 18 | المبلغ لكل تحليل (`Price`) | مطلوب | `Price` كعمود بصيغة `{0:F2}` | ✅ مطابق |
| 19 | مصدر السعر (BillingSystem + ReferralPrices) | مطلوب أن يعكس نظام الحساب | `DeliveryPatientTestRow.Price` يُملأ من `pt.Price` (السعر المسجل عند إضافة التحليل)؛ لا يُعاد الحساب من `ReferralPrices` هنا | ⚠️ جزئي (السعر المسجل صحيح، لكن لا إعادة حساب) |

### 1.5 القسم 4 — ملخص الحالة المالية والتسليم

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 20 | عدد التحاليل غير المنتهية | مطلوب | لا يوجد `UnenteredCount` في `DeliveryViewModel` — يوجد فقط `UndeliveredCount` و `UnprintedCount` | ❌ غير مطابق |
| 21 | عدد غير المطبوعة | مطلوب | `UnprintedCount` موجود ومربوط في XAML | ✅ مطابق |
| 22 | عدد غير المستلمة | مطلوب | `UndeliveredCount` موجود ومربوط في XAML | ✅ مطابق |
| 23 | باقي الحساب (Remaining) | `TotalAmount - PaidAmount` | `RemainingBalance` مربوط ويُحسب في `GetPatientDeliveryStateAsync` | ✅ مطابق |
| 24 | زر "مستلمة" لإلغاء تسليم بالخطأ | يتطلب Admin فقط (قرار 11) | زر `UnmarkDeliveredCommand` موجود مع `IsEnabled="{Binding IsAdmin}"` في XAML، و`CanUnmark()` = `IsAdmin && AggregateStatus >= Delivered`، والـ Service يتحقق ويرمي `UnauthorizedAccessException` | ✅ مطابق |

### 1.6 القسم 5 — أزرار الأوامر

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 25 | زر "تسليم يدوي" | مطلوب | `DeliverManuallyCommand` مربوط بزر "F9 - تسليم يدوي" — يستدعي `DeliverAllResultsAsync` + Confirmation | ✅ مطابق |
| 26 | زر "تسديد حساب" | تسجيل `PaymentTransaction` من نوع Payment | `SettleAccountCommand` يستدعي `SettleAccountAsync(patientId, amount, userId)` — ينشئ `PaymentTransaction{Type=Payment}` ويُحدّث `patient.PaidAmount` | ✅ مطابق |
| 27 | زر "تصفية حساب" (خالص) | يجعل `PaidAmount = TotalAmount` | **لا يوجد زر "تصفية حساب" منفصل** في `DeliveryView.xaml` أو أمر `ClearAccountCommand` في الـ VM؛ يمكن الوصول إلى نفس النتيجة يدوياً بإدخال المبلغ المتبقي في التسديد، لكن لا يوجد زر أوامر مباشر | ❌ غير مطابق |
| 28 | زر "العودة" | العودة للقائمة الرئيسية | `BackToMainCommand` → `_navigationService.GoBack()` مربوط بزر "العودة" | ✅ مطابق |

### 1.7 القسم 6 — الفلاتر والتصفية

| # | فلتر | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|------|------------------|------------------------|--------|
| 29 | غير المسلمة | `OnlyUndelivered = true` | `FilterMode == "Undelivered"` → `ComboBoxItem Content="غير مستلم"` | ✅ مطابق |
| 30 | معمل لمعمل (`LabToLab`) | مطلوب | `FilterMode == "LabToLab"` → `ComboBoxItem Content="معمل مع معمل"` — لكن **الـ ComboBox يربط `SelectedItem` بـ `FilterMode` مباشرة (string)**، بينما الـ ComboBoxItems لديها `Content` عربي وليس `Tag`؛ نتيجة: القيمة المرسلة للـ VM ستكون `ComboBoxItem` وليس السلسلة "LabToLab" — الفلتر لن يعمل | ⚠️ جزئي (منطق موجود لكن Binding معطوب) |
| 31 | مستقل (`Individual`) | مطلوب | نفس المشكلة (Binding معطوب) | ⚠️ جزئي |
| 32 | مهم (`IsImportant`) | مطلوب | `OnlyImportant` موجود في الفلتر، لكن نفس مشكلة Binding | ⚠️ جزئي |
| 33 | فترة زمنية (DateFrom/DateTo) | مطلوب | `<DatePicker SelectedDate="{Binding FilterDateFrom}" />` + `FilterDateTo` — يعمل | ✅ مطابق |
| 34 | الكل | عرض جميع النتائج (مسلمة + غير مسلمة) | `ComboBoxItem "الكل"` موجود، لكن لا يمرَّر `OnlyUndelivered = false` صراحة (فقط عبر `FilterMode != "Undelivered"`) — نظرياً يعمل، عملياً معطوب بسبب Binding | ⚠️ جزئي |
| 35 | حسب المستخدم (Current User) | عرض النتائج حسب المستخدم الحالي | **غير موجود** في `DeliveryFilter` ولا في الـ ComboBox | ❌ غير مطابق |

### 1.8 القسم 7 — مربع البحث بالكود والباركود

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 36 | مربع بحث بالكود (`VisitCode` أو `LabId` أو `FileCode`) | يبحث في الثلاثة | `SearchCode` + `SearchByCodeCommand` → `SearchByCodeAsync` | ✅ مطابق |
| 37 | كشف تلقائي للنوع من بادئة الكود (قرار 10) | 1→VisitCode، 3→FileCode، 5→LabId | `SearchByCodeAsync` يفحص `trimmed[1]` (الرقم الثاني) ويوجّه للـ Search المناسب؛ يوجد fallback يبحث في الثلاثة | ⚠️ جزئي — المرجع يقول "يبدأ بـ 1/3/5" (البادئة = الرقم الأول)، بينما التنفيذ يفحص `trimmed[1]` (الرقم الثاني) — انحراف طفيف عن التصميم المرن للقرار 10 |
| 38 | استخدام مع قارئ الباركود | مطلوب — يُحفَّز عند مسح الكود | `BarcodeScannerListener` مربوط في `DeliveryView.xaml.cs`؛ يستدعي `ScanBarcodeCommand` عند اكتشاف قراءة سريعة تنتهي بـ Enter | ✅ مطابق |
| 39 | سيناريو تطابق كود الإيصال (1) + كود الملف (3) + التأكيد | يقارن النظام بيانات المريض ثم يسأل "هل تريد تسليم النتائج؟" | لا يوجد سيناريو مقارنة مزدوجة (Receipt + File) في `DeliveryViewModel`؛ فقط بحث ببادئة واحدة | ❌ غير مطابق |
| 40 | تنبيه عند وجود باقي حساب | مطلوب | لا يوجد تنبيه صريح عند التسليم في حالة `RemainingBalance > 0`؛ `DeliverAllResultsAsync` تُنفَّذ مباشرة | ❌ غير مطابق |
| 41 | التسليم من الملف عند فقدان الوصل | مطلوب | ممكن عبر `FileCode` (بادئة 3)، لكن لا يوجد سيناريو منفصل | ⚠️ جزئي |

### 1.9 اختصارات لوحة المفاتيح داخل النافذة

| # | مفتاح | الوظيفة المرجعية | التطبيق الحالي | الحالة |
|---|-------|-------------------|-----------------|--------|
| 42 | `F2` — بيانات مريض | مطلوب | **غير موجود** في `<UserControl.InputBindings>` لـ `DeliveryView.xaml` | ❌ غير مطابق |
| 43 | `F3` — نافذة البحث | مطلوب | **غير موجود** كـ KeyBinding داخل النافذة | ❌ غير مطابق |
| 44 | `F4` — إدخال النتائج | مطلوب | **غير موجود** | ❌ غير مطابق |
| 45 | `F5` — Refresh | مطلوب | `<KeyBinding Key="F5" Command="{Binding RefreshCommand}" />` | ✅ مطابق |
| 46 | `F6` — إعادة تحميل النافذة | مطلوب | **غير موجود** داخل النافذة (F6 عالمي يفتح النافذة فقط) | ⚠️ جزئي |
| 47 | `F7` — نافذة العينات الخارج | مطلوب | **غير موجود** — الوظيفة نفسها غير مُنفَّذة | ❌ غير مطابق (مقصود على مستوى النظام) |
| 48 | `F8` — تبديل مراجَعة | مطلوب | **غير موجود** | ❌ غير مطابق |
| 49 | `F9` — تبديل منتهية | المرجع: تبديل حالة "منتهية" | التطبيق: `F9` مربوط بـ `DeliverManuallyCommand` (تسليم يدوي) — **معنى مختلف تماماً** | ❌ غير مطابق (استخدام مختلف) |
| 50 | `F12` — تبديل طُبعت | مطلوب | **غير موجود** | ❌ غير مطابق |
| 51 | `Enter` — عرض تحاليل / فتح إدخال | مطلوب | لا يوجد `KeyBinding Enter` صريح — الاختيار من `ListBox` يحمّل التحاليل تلقائياً عبر `OnSelectedPatientChanged` | ⚠️ جزئي |
| 52 | `Up/Down` — التنقل | مطلوب | يعمل ضمنياً عبر `ListBox` القياسي | ✅ مطابق |
| 53 | `Ctrl` — التنقل بين المناطق | مطلوب | لا يوجد Focus Manager أو `KeyBinding Ctrl+...` مخصص | ❌ غير مطابق |
| 54 | `Esc` — العودة للقائمة الرئيسية | مطلوب | `<KeyBinding Key="Escape" Command="{Binding BackToMainCommand}" />` | ✅ مطابق |

<a id="f5-missing"></a>
## 2. النواقص في الوظيفة 5 (في المرجع، غير موجود في نظامي)

1. **تلوين اسم المريض المهم بالأحمر** في كل من قائمة المرضى (القسم 1) وبيانات المريض المحدد (القسم 2). Binding الحالي يستخدم `InverseBoolToVisibilityConverter` بشكل خاطئ على `FontWeight`.
2. **نسخ الكود بضغطة واحدة** (`VisitCode` / `LabId`) من صف المريض في القائمة.
3. **عرض `VisitCode` / `LabId`** في صف قائمة المرضى.
4. **بيانات المريض المحدد** (الجنس، السن، الكود، الإحالة) في القسم 2.
5. **عدد التحاليل غير المنتهية** (`UnenteredCount`) في القسم 4.
6. **زر "تصفية حساب"** كأمر مستقل ينفّذ `PaidAmount = TotalAmount`.
7. **فلتر "حسب المستخدم"** في ComboBox الفلترة.
8. **سيناريو الباركود المزدوج**: مسح كود الإيصال ثم كود الملف مع مقارنة البيانات + سؤال "هل تريد تسليم النتائج؟".
9. **تنبيه باقي الحساب** قبل تنفيذ التسليم.
10. **اختصارات لوحة المفاتيح داخل النافذة**: F2, F3, F4, F8, F12, وإصلاح F9 (يجب أن يبدّل "منتهية" وليس تسليم يدوي).
11. **علامات الحالة السبع** داخل جدول تحاليل المريض (`DataGrid` يستخدم النص فقط، لا الأيقونات).

<a id="f5-diffs"></a>
## 3. الاختلافات في الوظيفة 5 (موجود لكن يعمل مختلفاً)

1. **Binding `FilterMode` في ComboBox معطوب**: `SelectedItem="{Binding FilterMode}"` مع `ComboBoxItem` (Content-only) — النتيجة أن `FilterMode` يستقبل كائن `ComboBoxItem` بدلاً من string، فيفشل مطابقة `"Undelivered"/"LabToLab"/"Individual"/"Important"`.
2. **كشف بادئة الكود**: التنفيذ يفحص `trimmed[1]` (الحرف الثاني) بدلاً من `trimmed[0]` (البادئة الأولى) كما يوصف في المرجع (قرار 10).
3. **F9 يعني "تسليم يدوي"** في التنفيذ، بينما المرجع يقول F9 = "تبديل حالة منتهية".
4. **جدول تحاليل المريض** يعرض `Status` و `IsPrinted` و `IsDelivered` كنصوص (True/False أو اسم enum) بدلاً من أيقونات ملوّنة كما في القائمة الرئيسية.
5. **السعر (`Price`)** يستخدم القيمة المسجّلة في `PatientTest.Price` مباشرة (وهذا صحيح للسجل التاريخي) ولا يُعاد حسابه من `BillingSystem` و `ReferralPrices` كما يوصف في المرجع نظرياً — لكن هذا هو السلوك المطلوب (السعر التاريخي عند التسجيل يبقى).

<a id="f5-extras"></a>
## 4. الإضافات في الوظيفة 5 (في نظامي، غير موصوفة في المرجع)

1. **`PaymentType.Delivery`**: تسجيل حدث التسليم كـ `PaymentTransaction { Amount = 0, Type = Delivery }` في `DeliverAllResultsAsync` (لتتبع الأحداث).
2. **`AuditLog { Action = "Deliver" }` و `Action = "UnmarkDelivered"` و `Action = "SettleAccount"`**: تسجيل كامل في سجل التدقيق لكل عملية.
3. **Transaction (`BeginTransactionAsync`)** حول عملية التسليم لضمان الذرّية.
4. **`DeliveredAt` و `DeliveredByUserId` على مستوى `Patient` بالإضافة إلى `PatientTest`**: تتبع مزدوج (مستوى الزيارة + مستوى المريض).
5. **زر "F5 - تحديث" في اللوحة الجانبية** بالإضافة إلى KeyBinding — تكرار مقصود للسهولة.

---

<a id="f6"></a>
# 🔹 الجزء الثاني: الوظيفة السادسة — البحث عن مريض

**الصفحات المرجعية**: 33 – 36 (من أصل 63)
**الملفات المشمولة**:
- `Services/Interfaces/IPatientSearchService.cs`
- `Services/Implementations/PatientSearchService.cs`
- `ViewModels/Pages/SearchViewModel.cs`
- `Views/Pages/SearchView.xaml` / `SearchView.xaml.cs`
- `App.xaml.cs`, `MainWindow.xaml`, `MainDashboardViewModel.cs`

<a id="f6-table"></a>
## 1. جدول المقارنة التفصيلي — الوظيفة 6

### 1.1 الوصول للنافذة والاختصار العالمي

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 1 | مسار الوصول (Toolbar → المرضى → بحث عن مريض) | موصوف | `MainDashboardViewModel` يحتوي على `FunctionDefinition "بحث عن مريض"` مع `TargetViewType = typeof(SearchView)` | ✅ مطابق |
| 2 | اختصار عالمي `F3` | يفتح نافذة البحث | `MainWindow.xaml`: `<KeyBinding Key="F3" Command="{Binding OpenSearchCommand}" />` — و `OpenSearchCommand` موجود | ✅ مطابق |

### 1.2 القسم 1 — فلاتر البحث

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 3 | فلتر فترة زمنية (`DateFrom` – `DateTo`) | مطلوب | `DateFrom`/`DateTo` كـ `DateTime?` في `SearchCriteria` و `SearchViewModel`؛ DatePickers مربوطة | ✅ مطابق |
| 4 | فلتر مرحلة عمرية (`AgeFrom` – `AgeTo` + `AgeUnit`) | مطلوب | `AgeFrom`, `AgeTo`, `SelectedAgeUnit` موجودة كخصائص + TextBoxes + ComboBox `AgeUnit` | ✅ مطابق |
| 5 | فلتر جنس (Male/Female فقط — قرار 17) | مطلوب | `SelectedGender` من نوع `Gender?` + ComboBox بـ "ذكر"/"أنثى" — لكن **الـ Binding معطوب** (نفس مشكلة F5): `SelectedItem` مع `ComboBoxItem Content` فقط دون `Tag`؛ النتيجة كائن ComboBoxItem وليس `Gender` enum | ⚠️ جزئي (Binding معطوب) |
| 6 | فلتر جهة إحالة (`Referral`) | مطلوب | `SelectedReferral` موجود في VM + `AvailableReferrals` مُحمّلة من `LoadReferralsAsync`، لكن **لا يوجد `ComboBox` أو `AutoComplete` مربوط بـ `SelectedReferral` في `SearchView.xaml`** — الخاصية موجودة لكن غير معروضة | ❌ غير مطابق |
| 7 | البحث ببداية الاسم (Prefix) | مطلوب | `NamePrefix` + `TextBox` مع `HintAssist.Hint="بداية الاسم"` — `PatientSearchService` يستخدم `StartsWith` | ✅ مطابق |
| 8 | البحث بجزء من الاسم (Contains) | مطلوب | `NameContains` + `TextBox` "يحتوي على" — `Contains` في الـ Service | ✅ مطابق |
| 9 | استخدام الطريقتين معاً (Prefix + Contains) | مطلوب — مثال: (اح) + (خال) → أحمد خالد | كلاهما مستقل في الـ Query (`AndAlso`): إذا أُدخل الاثنان يُطبَّق كلاهما بصيغة AND — يعمل نظرياً لكن الأمثلة في المرجع تُلمّح لأنه يعمل على أجزاء متعددة داخل نفس الاسم | ⚠️ جزئي |
| 10 | حد أقصى 100 مريض | مطلوب | `MaxResults = 100` افتراضي في `SearchCriteria`؛ `Take(criteria.MaxResults)` في الـ Service | ✅ مطابق |

### 1.3 القسم 2 — حقول البحث بالكود والهاتف

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 11 | الهاتف / الجوال | مطلوب | `PhoneNumber` + TextBox | ✅ مطابق |
| 12 | الرقم القومي | مطلوب | `NationalId` + TextBox | ✅ مطابق |
| 13 | كود الحالة (`VisitCode`) | مطلوب | `VisitCode` + TextBox | ✅ مطابق |
| 14 | كود المعمل (`LabId`) | مطلوب | `LabCode` (يُستخدم `LabId` في Patient) + TextBox | ✅ مطابق |
| 15 | كود الملف (`FileCode`) | مطلوب | `FileCode` + TextBox | ✅ مطابق |
| 16 | استخدام قارئ الباركود مباشرة في حقول الكود | مطلوب | لا يوجد `BarcodeScannerListener` في `SearchView.xaml.cs` — الحقول قياسية فقط | ❌ غير مطابق |
| 17 | إدخال الكود ثم `Enter` بدون قارئ | مطلوب | لا يوجد `KeyBinding Enter` مربوط بـ `SearchCommand` داخل TextBox؛ يجب الضغط على زر "F5 - بحث" | ⚠️ جزئي |

### 1.4 القسم 3 — جدول نتيجة البحث

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 18 | جدول النتائج | مطلوب | `<DataGrid ItemsSource="{Binding Results}" SelectedItem="{Binding SelectedResult}">` | ✅ مطابق |
| 19 | حد 100 نتيجة | مطلوب | مُطبَّق في الـ Service | ✅ مطابق |
| 20 | الفرز (Sortable) | مطلوب — "الفرز عن طريق الإحصائيات" | `CanUserSortColumns="True"` في DataGrid | ✅ مطابق |
| 21 | عمود ID | مطلوب | **غير معروض** في الأعمدة الظاهرة (`PatientId` موجود في `PatientSearchRow` لكن ليس كعمود) | ❌ غير مطابق |
| 22 | عمود Lab ID | مطلوب | `LabId` عمود ظاهر | ✅ مطابق |
| 23 | عمود Patient Name | مطلوب | `FullName` عمود ظاهر | ✅ مطابق |
| 24 | عمود Date | مطلوب | `CreatedAt` بصيغة `yyyy-MM-dd` | ✅ مطابق |
| 25 | عمود Gender | مطلوب | `Gender` عمود ظاهر | ✅ مطابق |
| 26 | عمود Age | مطلوب | `AgeValue` عمود ظاهر (رقم فقط) | ⚠️ جزئي (لا يظهر AgeUnit) |
| 27 | عمود Unit (وحدة العمر) | مطلوب | **غير معروض** كعمود مستقل (`AgeUnit` موجود في `PatientSearchRow` لكن ليس في XAML) | ❌ غير مطابق |
| 28 | عمود Referred By | مطلوب | `ReferralName` عمود ظاهر | ✅ مطابق |
| 29 | تلوين المريض المهم بالأحمر | مطلوب (متطابق مع F5) | `IsImportant` موجود في `PatientSearchRow` لكن لا Trigger لتلوين الصف بالأحمر في DataGrid | ❌ غير مطابق |

### 1.5 القسم 4 — قائمة تحاليل المريض المحدد

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 30 | عرض تحاليل المريض المحدد | مطلوب | `SelectedResultTests` مع `ListBox` وأيقونة `PackIcon` تعرض الحالة | ✅ مطابق |
| 31 | النقر المزدوج يذهب لنتائج مجمعة | مطلوب | لا يوجد `MouseDoubleClick` مربوط بـ Command على DataGrid | ❌ غير مطابق |

### 1.6 القسم 5 — ملخص وأزرار الأوامر

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 32 | عدد التحاليل غير المنتهية | مطلوب | `Summary.UnenteredCount` في `PatientTestsSummary` — لكن **غير معروض** في XAML؛ يظهر فقط `Total/Paid/Remaining` | ❌ غير مطابق |
| 33 | عدد التحاليل غير المطبوعة | مطلوب | `Summary.UnprintedCount` في السجل — **غير معروض** في XAML | ❌ غير مطابق |
| 34 | عدد التحاليل غير المستلمة | مطلوب | `Summary.UndeliveredCount` في السجل — **غير معروض** في XAML | ❌ غير مطابق |
| 35 | المرضى ذوي الحسابات المفتوحة | مطلوب (مؤشر إحصائي على مستوى النافذة) | **غير موجود** كإحصائية | ❌ غير مطابق |
| 36 | زر "نتائج تحاليل" | يذهب لـ Function 3/4 | `OpenResultsCommand` → `NavigateTo<TestResultsListViewModel>()` | ✅ مطابق |
| 37 | زر "بيانات مريض" | يذهب لـ Function 1 | `OpenPatientDataCommand` → `NavigateTo<PatientEntryViewModel>()` | ✅ مطابق |
| 38 | زر "حذف" | يتطلب Admin فقط (قرار 13) | `DeleteCommand` مع `CanExecute = IsAdmin && SelectedResult != null` + `IsEnabled="{Binding IsAdmin}"` + Service يتحقق ويرمي `UnauthorizedAccessException` | ✅ مطابق |
| 39 | زر "نتائج مجموعة" | طباعة نتائج مجموعة مشتركة | `PrintGroupResultsCommand` موجود لكن يعرض رسالة "ستتوفر هذه الميزة قريباً" فقط (Stub) | ⚠️ جزئي |

### 1.7 القسم 6 — خيار قاعدة النسخ الاحتياطي (قرار 12)

| # | المكون | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|--------|------------------|------------------------|--------|
| 40 | قائمة اختيار مصدر البحث (Main/Backup) | مطلوب — Stub معطَّل بصرياً في هذه المرحلة | `<ComboBox SelectedItem="{Binding SelectedSource}">` + `ComboBoxItem "النسخة الاحتياطية (غير مفعّل)" IsEnabled="False" ToolTip="غير مفعّل حالياً"` | ✅ مطابق (وفق قرار 12) |
| 41 | Service يرفض `SearchSource.Backup` | مطلوب | `PatientSearchService.SearchAsync` يرمي `NotImplementedException("هذه الميزة غير مفعّلة في هذه المرحلة")` | ✅ مطابق |
| 42 | `IsBackupSearchEnabled = false` كإشارة | مطلوب | موجود كخاصية في `SearchViewModel` (`public bool IsBackupSearchEnabled => false`) | ✅ مطابق |

### 1.8 اختصارات لوحة المفاتيح داخل النافذة

| # | مفتاح | الوظيفة المرجعية | التطبيق الحالي | الحالة |
|---|-------|-------------------|-----------------|--------|
| 43 | `F2` — بيانات مريض | مطلوب | `<KeyBinding Key="F2" Command="{Binding OpenPatientDataCommand}" />` | ✅ مطابق |
| 44 | `F4` — إدخال النتائج | مطلوب | `<KeyBinding Key="F4" Command="{Binding OpenResultsCommand}" />` | ✅ مطابق |
| 45 | `F5` — Refresh | مطلوب | `<KeyBinding Key="F5" Command="{Binding RefreshCommand}" />` | ✅ مطابق |
| 46 | `F6` — نافذة التسليم | مطلوب | **غير موجود** كـ KeyBinding داخل `SearchView.xaml` | ❌ غير مطابق |
| 47 | `F7` — عينات خارج | مطلوب | **غير موجود** (الوظيفة نفسها غير مُنفَّذة — انحراف مقصود على مستوى النظام) | ⚠️ جزئي |
| 48 | `F10` — حذف (Admin) | مطلوب | `<KeyBinding Key="F10" Command="{Binding DeleteCommand}" />` | ✅ مطابق |
| 49 | `Esc` — العودة | مطلوب | `<KeyBinding Key="Escape" Command="{Binding BackCommand}" />` | ✅ مطابق |

<a id="f6-missing"></a>
## 2. النواقص في الوظيفة 6 (في المرجع، غير موجود في نظامي)

1. **ComboBox لجهة الإحالة (`SelectedReferral`)**: الخاصية موجودة والبيانات مُحمَّلة، لكن لا يوجد عنصر UI مربوط بها في `SearchView.xaml`.
2. **دعم قارئ الباركود** في حقول الكود (VisitCode / LabId / FileCode) — لا يوجد `BarcodeScannerListener`.
3. **`KeyBinding Enter`** على TextBoxes الكود لتنفيذ `SearchCommand` مباشرة.
4. **عمود ID** في DataGrid النتائج.
5. **عمود Unit** (وحدة العمر) في DataGrid النتائج.
6. **تلوين صف المريض المهم بالأحمر** في DataGrid النتائج (Trigger على `IsImportant`).
7. **النقر المزدوج على المريض** في DataGrid → يفتح نتائج مجمعة (Function 3/4).
8. **إحصائيات النافذة في القسم 5**: عدد التحاليل غير المنتهية / غير المطبوعة / غير المستلمة (على مستوى النتائج كلها، ليس المريض المحدد فقط).
9. **إحصائية "المرضى ذوي الحسابات المفتوحة"**.
10. **عرض `Summary.UnenteredCount` و `Summary.UnprintedCount` و `Summary.UndeliveredCount`** في لوحة الملخص الجانبية (موجودة في الـ record لكن غير مربوطة في XAML).
11. **`KeyBinding F6`** داخل النافذة لفتح Function 5.
12. **زر "نتائج مجموعة"** كتنفيذ فعلي (حالياً Stub — يمكن تركه Stub وفق نطاق الوظائف الثمانية الأساسية).

<a id="f6-diffs"></a>
## 3. الاختلافات في الوظيفة 6 (موجود لكن يعمل مختلفاً)

1. **Binding `SelectedGender` معطوب**: `SelectedItem` مربوط بـ `Gender?` لكن `ComboBoxItem Content="ذكر"/"أنثى"` (بدون `Tag` أو `ItemsSource` من enum)؛ نتيجة: `SelectedGender` يستقبل `ComboBoxItem` وليس `Gender` enum.
2. **Binding `SelectedAgeUnit` معطوب**: نفس المشكلة — ComboBoxItems عربية بدون Tag.
3. **`SelectedSource` Binding معطوب**: `SelectedItem="{Binding SelectedSource}"` مع `ComboBoxItem Tag="Main"/"Backup"` — لكن `SelectedItem` يعطي `ComboBoxItem` كاملاً لا الـ Tag؛ يجب `SelectedValue` + `SelectedValuePath="Tag"`.
4. **الأمر `SwitchToBackupCommand`**: موجود في الـ VM لكنه فارغ (body فارغ) — لا يفعل شيئاً حتى لو كان مفعّلاً.
5. **زر "نتائج مجموعة"** يعرض رسالة "قريباً" (Stub) بدلاً من الطباعة الفعلية.

<a id="f6-extras"></a>
## 4. الإضافات في الوظيفة 6 (في نظامي، غير موصوفة في المرجع)

1. **`AuditLog { Action = "Delete" }`** عند حذف مريض — تتبع كامل.
2. **Composable IQueryable Search** (`CP-F6-2`): بناء الاستعلام بشكل ديناميكي مع `.Where()` المتعاقبة — تصميم أنظف من المرجع.
3. **`GetSummaryAsync`** كخدمة منفصلة تُرجع `PatientTestsSummary` بحقول أكثر تفصيلاً من المرجع (Total/Paid/Remaining بجانب العدادات).

---

<a id="f5-plan"></a>
# 🛠️ الجزء الثالث: خطة التنفيذ التفصيلية للوظيفة 5

الهدف: جعل نافذة تسليم النتائج مطابقة للمرجع 100%. مقسّمة إلى **Parts** متتالية؛ كل Part ينتهي بـ **Build = 0 errors, 0 warnings** قبل الانتقال للتالي.

---

### 🔹 Part 5.A — إصلاح Binding الفلترة (Enum بدل String)

**الهدف**: جعل `FilterMode` ComboBox يعمل بشكل صحيح.

**الملفات المُعدَّلة**:
- `ViewModels/Pages/DeliveryViewModel.cs`
- `Views/Pages/DeliveryView.xaml`

**التغييرات**:
1. تحويل `FilterMode` من `string` إلى `enum DeliveryFilterMode { Undelivered, All, LabToLab, Individual, Important, CurrentUser }`.
2. إضافة enum جديد في `Services/Interfaces/IDeliveryService.cs` أو مساحة مخصصة.
3. تحديث ComboBox في XAML لاستخدام `ObjectDataProvider` أو `ItemsSource` من enum values، مع Converter لعرض النص العربي.
4. تحديث `BuildFilter()` ليعمل مع القيم الجديدة.

**اختبار Build**: تشغيل `dotnet build` → التأكد من 0 errors + 0 warnings.

---

### 🔹 Part 5.B — إضافة فلتر "حسب المستخدم"

**الملفات المُعدَّلة**:
- `Services/Interfaces/IDeliveryService.cs`
- `Services/Implementations/DeliveryService.cs`
- `ViewModels/Pages/DeliveryViewModel.cs`
- `Views/Pages/DeliveryView.xaml`

**التغييرات**:
1. إضافة `int? UserId` أو `bool OnlyCurrentUser` إلى `DeliveryFilter`.
2. في `DeliveryService.FilterAsync`: إضافة `q.Where(v => v.Patient.CreatedByUserId == currentUserId)` عند التفعيل.
3. إضافة `DeliveryFilterMode.CurrentUser` إلى الـ enum.
4. إضافة `ComboBoxItem "حسب المستخدم"` في XAML.

---

### 🔹 Part 5.C — تلوين المريض المهم بالأحمر

**الملفات المُعدَّلة**:
- `Converters/BoolToRedForegroundConverter.cs` (جديد)
- `Views/Pages/DeliveryView.xaml`

**التغييرات**:
1. إنشاء `BoolToRedForegroundConverter` يُرجع `Brushes.Red` عندما `IsImportant = true`، وإلا `Brushes.Black` (أو `DynamicResource MaterialDesignBody`).
2. تسجيله في `UserControl.Resources`.
3. استخدامه في `<TextBlock Foreground="{Binding IsImportant, Converter={StaticResource BoolToRedForegroundConverter}}">` في كل من:
   - قالب صف قائمة المرضى (`ListBox.ItemTemplate`)
   - `SelectedPatient.FullName` في القسم 2
4. إزالة `FontWeight="{Binding IsImportant, Converter={x:Static converters:InverseBoolToVisibilityConverter.Instance}}"` الخاطئ.

---

### 🔹 Part 5.D — عرض VisitCode / LabId + نسخ بضغطة

**الملفات المُعدَّلة**:
- `Services/Interfaces/IDeliveryService.cs` (تعديل `DeliveryPatientRow`)
- `Services/Implementations/DeliveryService.cs`
- `Views/Pages/DeliveryView.xaml`
- `ViewModels/Pages/DeliveryViewModel.cs`

**التغييرات**:
1. إضافة `VisitCode`, `LabId`, `FileCode`, `Gender`, `AgeValue`, `AgeUnit` إلى `DeliveryPatientRow` record.
2. تعبئتها في `FilterAsync` و `BuildDeliveryRowAsync`.
3. عرضها في `ListBox.ItemTemplate` وفي القسم 2 (بيانات المريض).
4. إضافة `Command="{Binding DataContext.CopyCodeCommand, RelativeSource={RelativeSource AncestorType=ListBox}}" CommandParameter="{Binding VisitCode}"` على `TextBlock` عبر `Hyperlink` أو `Button` مصمم كنص.
5. إضافة `RelayCommand CopyCode(string code)` في الـ VM يستخدم `Clipboard.SetText(code)`.

---

### 🔹 Part 5.E — إضافة عدد "غير المنتهية" (UnenteredCount)

**الملفات المُعدَّلة**:
- `Services/Interfaces/IDeliveryService.cs` (تعديل tuple الإرجاع)
- `Services/Implementations/DeliveryService.cs`
- `ViewModels/Pages/DeliveryViewModel.cs`
- `Views/Pages/DeliveryView.xaml`

**التغييرات**:
1. تعديل `GetPatientDeliveryStateAsync` لإرجاع `(int Unentered, int Undelivered, int Unprinted, decimal Remaining)`.
2. حساب `unentered = tests.Count(t => t.Status == TestStatus.New)`.
3. إضافة `[ObservableProperty] private int unenteredCount;` في الـ VM.
4. عرضها في القسم 4 مع `UnprintedCount` و `UndeliveredCount`.

---

### 🔹 Part 5.F — إضافة زر "تصفية حساب"

**الملفات المُعدَّلة**:
- `Services/Interfaces/IDeliveryService.cs`
- `Services/Implementations/DeliveryService.cs`
- `ViewModels/Pages/DeliveryViewModel.cs`
- `Views/Pages/DeliveryView.xaml`

**التغييرات**:
1. إضافة `Task<PaymentTransaction> ClearAccountAsync(int patientId, int userId)` في `IDeliveryService`.
2. التنفيذ: `patient.PaidAmount = patient.TotalAmount` + `PaymentTransaction{Type=Payment, Amount=remaining}` + `AuditLog{Action="ClearAccount"}`.
3. `[RelayCommand] ClearAccountAsync` في الـ VM.
4. زر "تصفية حساب" في XAML مع Confirmation.

---

### 🔹 Part 5.G — إصلاح كشف بادئة الكود (قرار 10)

**الملفات المُعدَّلة**:
- `Services/Implementations/DeliveryService.cs`

**التغييرات**:
- تعديل `SearchByCodeAsync` لاستخدام `trimmed[0]` (البادئة الأولى) بدلاً من `trimmed[1]` (كما يوصف المرجع صراحة).
- التأكد من أن الكود يبدأ بـ 1/3/5 على الحرف الأول لا الثاني.

---

### 🔹 Part 5.H — سيناريو الباركود المزدوج + تنبيه باقي الحساب

**الملفات المُعدَّلة**:
- `ViewModels/Pages/DeliveryViewModel.cs`
- `Services/Implementations/DeliveryService.cs` (اختياري لدعم مقارنة)

**التغييرات**:
1. إضافة حالة `PendingReceipt` في الـ VM: إذا وصل كود بادئته 1، احفظ المريض الحالي وانتظر مسحة ثانية.
2. عند وصول كود بادئته 3، قارن `Patient.FileCode` مع بيانات `PendingReceipt`.
3. عند التطابق: `_dialogService.ShowConfirmation("هل تريد تسليم النتائج؟")`.
4. في `DeliverManuallyAsync` قبل التأكيد: إذا `SelectedPatient.RemainingBalance > 0`، أظهر تنبيه إضافي "يوجد باقي حساب X — هل تريد الاستمرار؟".

---

### 🔹 Part 5.I — أيقونات الحالة داخل جدول التحاليل

**الملفات المُعدَّلة**:
- `Views/Pages/DeliveryView.xaml`

**التغييرات**:
- تحويل عمود `Status` من `DataGridTextColumn` إلى `DataGridTemplateColumn` يعرض `<materialDesign:PackIcon Kind="{Binding Status, Converter={StaticResource TestStatusToIconConverter}}" Foreground="{Binding Status, Converter={StaticResource TestStatusToColorConverter}}"/>`.
- إضافة أعمدة مماثلة لـ `IsReviewed` و `IsPrinted` و `IsDelivered` بأيقونات checkmark/cross.

---

### 🔹 Part 5.J — اختصارات لوحة المفاتيح داخل النافذة

**الملفات المُعدَّلة**:
- `Views/Pages/DeliveryView.xaml`
- `ViewModels/Pages/DeliveryViewModel.cs`

**التغييرات**:
1. **F9**: نقل من `DeliverManuallyCommand` إلى `ToggleFinishedCommand` (تبديل "منتهية" على التحليل المحدد).
2. **F2**: `GoToPatientEntryCommand` → `NavigateTo<PatientEntryViewModel>()`.
3. **F3**: `GoToSearchCommand` → `NavigateTo<SearchViewModel>()`.
4. **F4**: `GoToTestResultsCommand` → `NavigateTo<TestResultsListViewModel>()`.
5. **F8**: `ToggleReviewedCommand`.
6. **F12**: `TogglePrintedCommand`.
7. **F6 داخلي**: `RefreshCommand` أو Reload كامل.
8. إضافة زر منفصل "تسليم يدوي" بدون اختصار F9 (أو استخدام Ctrl+D).

---

### 🔹 Part 5.K — تحديث جدول التحاليل ليعرض علامات أكثر

**الملفات المُعدَّلة**:
- `Services/Interfaces/IDeliveryService.cs` (تعديل `DeliveryPatientTestRow`)
- `Services/Implementations/DeliveryService.cs`
- `Views/Pages/DeliveryView.xaml`

**التغييرات**:
1. إضافة `IsReviewed` و `IsEntered` إلى `DeliveryPatientTestRow`.
2. أعمدة `DataGridTemplateColumn` لكل علامة (Reviewed / Entered / Printed / Delivered) بأيقونة CheckCircle/Circle.

---

<a id="f6-plan"></a>
# 🛠️ الجزء الرابع: خطة التنفيذ التفصيلية للوظيفة 6

---

### 🔸 Part 6.A — إصلاح Bindings الـ Enum في ComboBoxes

**الملفات المُعدَّلة**:
- `Views/Pages/SearchView.xaml`
- `ViewModels/Pages/SearchViewModel.cs` (اختياري: إضافة قوائم Enum values)

**التغييرات**:
1. **`SelectedGender`**: استخدام `ItemsSource="{Binding AvailableGenders}"` مع `ComboBox SelectedValuePath` أو `ObjectDataProvider` لـ `Gender` enum + `EnumToArabicConverter`. (Male → "ذكر"، Female → "أنثى" — قرار 17).
2. **`SelectedAgeUnit`**: نفس المنهج لـ `AgeUnit` enum (Day/Month/Year → يوم/شهر/سنة).
3. **`SelectedSource`**: تحويل ComboBox إلى `SelectedValue="{Binding SelectedSource}" SelectedValuePath="Tag"` + `ComboBoxItem Content="القاعدة الأساسية" Tag="{x:Static local:SearchSource.Main}"`.

---

### 🔸 Part 6.B — إضافة ComboBox جهة الإحالة

**الملفات المُعدَّلة**:
- `Views/Pages/SearchView.xaml`

**التغييرات**:
- إضافة `<ComboBox ItemsSource="{Binding AvailableReferrals}" SelectedItem="{Binding SelectedReferral}" DisplayMemberPath="Name" materialDesign:HintAssist.Hint="جهة الإحالة" Width="150" />` بجانب فلاتر الجنس/العمر.

---

### 🔸 Part 6.C — دعم Enter و Barcode في حقول الكود

**الملفات المُعدَّلة**:
- `Views/Pages/SearchView.xaml`
- `Views/Pages/SearchView.xaml.cs`
- `ViewModels/Pages/SearchViewModel.cs`

**التغييرات**:
1. إضافة `<TextBox.InputBindings><KeyBinding Key="Enter" Command="{Binding SearchCommand}"/></TextBox.InputBindings>` على كل TextBox من (VisitCode / LabCode / FileCode / PhoneNumber / NationalId).
2. إضافة `BarcodeScannerListener` في `SearchView.xaml.cs` مماثل لـ `DeliveryView.xaml.cs`.
3. إضافة `[RelayCommand] private async Task ScanBarcodeAsync(string raw)` في الـ VM: كشف تلقائي للنوع (1→VisitCode، 3→FileCode، 5→LabCode) وتعبئة الحقل المناسب ثم تنفيذ `SearchAsync`.

---

### 🔸 Part 6.D — إكمال أعمدة DataGrid (ID + Unit)

**الملفات المُعدَّلة**:
- `Views/Pages/SearchView.xaml`

**التغييرات**:
- إضافة `<DataGridTextColumn Header="ID" Binding="{Binding PatientId}" Width="50"/>`.
- إضافة `<DataGridTextColumn Header="الوحدة" Binding="{Binding AgeUnit}" Width="60"/>` بعد عمود السن.

---

### 🔸 Part 6.E — تلوين صف المريض المهم في DataGrid

**الملفات المُعدَّلة**:
- `Views/Pages/SearchView.xaml`

**التغييرات**:
- إضافة `<DataGrid.RowStyle><Style TargetType="DataGridRow"><Style.Triggers><DataTrigger Binding="{Binding IsImportant}" Value="True"><Setter Property="Foreground" Value="Red"/></DataTrigger></Style.Triggers></Style></DataGrid.RowStyle>`.

---

### 🔸 Part 6.F — النقر المزدوج → نتائج مجمعة

**الملفات المُعدَّلة**:
- `Views/Pages/SearchView.xaml`

**التغييرات**:
- إضافة `<DataGrid.InputBindings><MouseBinding MouseAction="LeftDoubleClick" Command="{Binding OpenResultsCommand}"/></DataGrid.InputBindings>` على DataGrid النتائج.

---

### 🔸 Part 6.G — عرض عدادات "غير منتهية / غير مطبوعة / غير مستلمة" في الملخص

**الملفات المُعدَّلة**:
- `Views/Pages/SearchView.xaml`

**التغييرات**:
- إضافة 3 `StackPanel` جديدة تعرض `Summary.UnenteredCount`, `Summary.UnprintedCount`, `Summary.UndeliveredCount` (الحقول موجودة أصلاً في `PatientTestsSummary`).

---

### 🔸 Part 6.H — إضافة إحصائية "المرضى ذوي الحسابات المفتوحة"

**الملفات المُعدَّلة**:
- `Services/Interfaces/IPatientSearchService.cs`
- `Services/Implementations/PatientSearchService.cs`
- `ViewModels/Pages/SearchViewModel.cs`
- `Views/Pages/SearchView.xaml`

**التغييرات**:
1. إضافة `Task<int> GetOpenAccountsCountAsync(SearchCriteria criteria)` في الـ Interface.
2. التنفيذ: `_context.Patients.CountAsync(p => p.PaidAmount < p.TotalAmount /* ضمن نفس criteria */)`.
3. `[ObservableProperty] private int openAccountsCount;` في الـ VM، يُحدَّث بعد كل `SearchAsync`.
4. عرضها في XAML بجانب عدادات النتائج.

---

### 🔸 Part 6.I — KeyBinding F6 داخل النافذة

**الملفات المُعدَّلة**:
- `Views/Pages/SearchView.xaml`
- `ViewModels/Pages/SearchViewModel.cs`

**التغييرات**:
1. إضافة `[RelayCommand] private void OpenDelivery() => _navigationService.NavigateTo<DeliveryViewModel>();`.
2. `<KeyBinding Key="F6" Command="{Binding OpenDeliveryCommand}" />`.

---

### 🔸 Part 6.J — تنظيف `SwitchToBackupCommand` الفارغ

**الملفات المُعدَّلة**:
- `ViewModels/Pages/SearchViewModel.cs`

**التغييرات**:
- إما إزالة الأمر تماماً (لأن الميزة Stub — قرار 12)، أو تركه مع رسالة `_dialogService.ShowMessage("قريباً", "قاعدة النسخ الاحتياطي غير مفعّلة حالياً")` لتوضيح النية.

---

<a id="decisions"></a>
# 📌 الجزء الخامس: ملخص القرارات الحاكمة (17 قرار)

القرارات ذات الصلة المباشرة بالوظيفتين 5 و 6:

| # | القرار | التطبيق في F5/F6 |
|---|--------|-------------------|
| **10** | كشف تلقائي لبادئة كود الباركود (1/3/5) — تصميم مرن | مُطبَّق في `SearchByCodeAsync` لكن يستخدم `trimmed[1]` بدل `trimmed[0]` (يحتاج إصلاح — Part 5.G) |
| **11** | زر "مستلمة" (إلغاء تسليم) — Admin فقط | مُطبَّق بشكل صحيح في `UnmarkDeliveredAsync` + `CanUnmark()` + `IsEnabled` |
| **12** | Backup Search — Stub معطَّل | مُطبَّق: `IsBackupSearchEnabled = false`، ComboBoxItem `IsEnabled="False"`، Service يرمي `NotImplementedException` |
| **13** | حذف مريض — Admin فقط | مُطبَّق: `DeletePatientAsync` يتحقق ويرمي `UnauthorizedAccessException`، `DeleteCommand.CanExecute = IsAdmin` |
| **17** | Gender = Male/Female فقط (لا Both) | مُطبَّق في `SearchCriteria.Gender` و `Patient.Gender`، لكن Binding ComboBox معطوب (يحتاج إصلاح — Part 6.A) |

### قرارات لا تُعتبر انحرافات (ملتزمة عمداً):

- **حذف Branch**: لا حقل Branch في `Patient` ولا فلتر Branch في F5/F6 — التزام كامل.
- **عدم وجود "تاريخ مخصص"** كفلتر إضافي — التزام.
- **Backup = Stub** — التزام (قرار 12).
- **PrintGroupResults = Stub** (`CP-F6-4`) — خارج نطاق الوظائف الثمانية الأساسية.
- **ZXing.Net حصراً** للباركود — لا يوجد استخدام لباركود بديل.

---

# 📊 ملخص إحصائي

## الوظيفة 5 — التسليم

- **إجمالي المكونات المفحوصة**: 54
- **✅ مطابق**: 24 (44%)
- **⚠️ جزئي**: 11 (20%)
- **❌ غير مطابق**: 19 (35%)
- **📌 نواقص فعلية للتنفيذ**: 11
- **🔧 اختلافات تحتاج إصلاح**: 5
- **⭐ إضافات مقبولة**: 5

## الوظيفة 6 — البحث

- **إجمالي المكونات المفحوصة**: 49
- **✅ مطابق**: 25 (51%)
- **⚠️ جزئي**: 5 (10%)
- **❌ غير مطابق**: 19 (39%)
- **📌 نواقص فعلية للتنفيذ**: 12
- **🔧 اختلافات تحتاج إصلاح**: 5
- **⭐ إضافات مقبولة**: 3

---

# ✅ الخلاصة والتوصية

الوظيفتان 5 و 6 مبنيتان على هيكل صحيح ومنطق سليم في طبقة الـ Service، مع التزام كامل بالقرارات الحاكمة (10, 11, 12, 13, 17). أهم الفجوات هي فجوات UI/UX:

1. **Bindings معطوبة في ComboBoxes** (FilterMode في F5 + Gender/AgeUnit/Source في F6) — إصلاح تقني بسيط بأولوية عالية.
2. **مكونات UI مفقودة** (ComboBox الإحالة، أعمدة ID/Unit، تلوين المهم بالأحمر، عدادات إضافية) — إضافات مباشرة.
3. **سلوكيات مفقودة** (نسخ بضغطة، النقر المزدوج، سيناريو الباركود المزدوج، تنبيه باقي الحساب) — إضافات منطق في الـ VM.
4. **اختصارات لوحة المفاتيح** غير كاملة داخل النافذتين — إضافات سطر واحد لكل مفتاح.

خطة التنفيذ مقسّمة إلى **11 Part للـ F5** و **10 Parts للـ F6**، كل Part صغير وقابل للتحقق منه ببناء نظيف (0 errors / 0 warnings).

**نهاية التقرير**
