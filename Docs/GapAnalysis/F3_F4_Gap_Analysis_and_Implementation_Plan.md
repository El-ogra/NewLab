# 📊 F3 & F4 — Gap Analysis and Implementation Plan
# تحليل الفجوات وخطة التنفيذ للوظيفة الثالثة والوظيفة الرابعة

---

## 📌 معلومات المرجع

| البند | القيمة |
|------|--------|
| **Repository** | `https://github.com/El-ogra/NewLab.git` |
| **Branch** | `main` |
| **Commit Hash** | `56968b429f7f49485b15e065bc7d23516ac626c8` |
| **Commit Message** | "بعد تنفيذ الوظائف الخامسة والسادسة وإزالة ملف التخطيط الخاص بهما" |
| **المصادر المرجعية** | `Real_Lab_System_Reference.pdf` (الصفحات 21–28) + `Reference_Functions_3_and_4_Detailed_Analysis.md` |
| **قرارات المالك الملزمة** | القرارات الـ 17 في `Docs/analysis_and_plan_v3.md` |
| **تاريخ التحليل** | 2026-07-23 |

> ⚠️ ملاحظة حاسمة: تم اعتماد الأصل الحصري لهذا التحليل على شجرة الملفات الخاصة بالـ Commit `56968b4` أعلاه — لم يُنظر إلى أي تعديل لاحق.

---

## 🔑 مطبَق من القرارات الـ17 على F3/F4

| القرار | التطبيق المتوقع في F3/F4 |
|--------|--------------------------|
| **قرار 6** | "رقم الحضور" = `PatientVisit.DailySequenceNumber` — تسلسل يومي بسيط. لا ترميز مركّب. |
| **قرار 7** | زرَّا "ب" (Audit) و"ت" (Financial Tracking) — Admin فقط. |
| **قرار 8** | Hct=Hgb×3.3، Hgb% حسب العمر/الجنس (8.25/7.50/6.25/6.75)، PT/INR بمعامل ISI و Control Time، PTT Ratio مع Control Time — الثوابت تُحرَّر عبر زر "Constants". |
| **قرار 9** | زر "تاريخ مخصص" **خارج النطاق** — يبقى فقط زر "تاريخ مرضي" الأساسي. |
| **قرار 16** | تلوين النتائج يستخدم "أضيق مدى يفوز" في `INormalRangeService.GetMatchingRangeAsync`. |
| **قرار 17** | Gender = Male/Female فقط — بلا Both. |

---

# 🔹 الجزء الأول: مقارنة الوظيفة الثالثة — الوصول لنوافذ إدخال نتائج التحاليل

**النطاق المرجعي**: `Real_Lab_System_Reference.pdf` الصفحات 21–25، و`Reference_Functions_3_and_4_Detailed_Analysis.md`.
**النطاق التنفيذي**: `Views/Pages/TestResultsListView.xaml` (+ `.xaml.cs`)، `ViewModels/Pages/TestResultsListViewModel.cs`، `Services/Interfaces/ITestResultsListService.cs`، `Services/Implementations/TestResultsListService.cs`، `Converters/TestStatusToIconConverter.cs`، `Models/Domain/Enums/TestStatus.cs`، `Models/Domain/PatientTest.cs`، `Models/Domain/AuditLog.cs`، `Data/NewLabDbContext.cs`، `Views/Windows/MainWindow.xaml`، `ViewModels/Pages/MainDashboardViewModel.cs`، `App.xaml.cs`.

## 1) جدول المقارنة التفصيلي — F3

| # | المكون (حقل/زر/خاصية/اختصار/سلوك) | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|-------------------------------------|------------------|------------------------|--------|
| 1 | **الوصول من Toolbar → "المرضى" → "إدخال نتائج التحاليل"** | مسار قياسي من الشريط العلوي. | موجود: `MainDashboardViewModel` — فئة "المرضى" بها `FunctionDefinition` باسم "إدخال نتائج التحاليل" مع `TargetViewType = typeof(TestResultsListView)`. | ✅ مطابق |
| 2 | **اختصار F4 عالمي من أي نافذة رئيسية** | فتح نافذة F3 من أي نافذة. | `MainWindow.xaml` يحتوي `KeyBinding Key="F4" Command="OpenTestResultsListCommand"` — لكن F4 مربوط فقط بـ `MainWindow`، لا يوجد ربط عالمي داخل الشاشات الفرعية. | ⚠️ جزئي |
| 3 | **قائمة مرضى اليوم فقط (لا كل المرضى)** | تعرض المرضى المسجلين في يوم العمل الحالي فقط. | `TestResultsListService.GetPatientsByFilterAsync` يستخدم `v.VisitDate.Date == d.Date` — افتراضياً `DateTime.Today`. | ✅ مطابق |
| 4 | **رمز الحالة 1: 🔴 دائرة حمراء (جديد غير مسجل — TestStatus.New)** | مطلوب. | `TestStatusToIconConverter`: `New → "Circle"`، لون Red `RGB(244,67,54)`. | ✅ مطابق |
| 5 | **رمز الحالة 2: 📝 ورقة (Entered — كُتبت النتائج)** | مطلوب. | `Entered → "FileDocument"`. | ✅ مطابق |
| 6 | **رمز الحالة 3: ↕️ أسهم (Reviewed — تمت مراجعة/تراجع)** | مطلوب. | `Reviewed → "ArrowLeftRight"`. | ✅ مطابق |
| 7 | **رمز الحالة 4: 🖨️ طابعة (Printed)** | مطلوب. | `Printed → "Printer"`. | ✅ مطابق |
| 8 | **رمز الحالة 5: 🛒 عربة (Delivered — تم التسليم)** | مطلوب. | `Delivered → "Cart"`. | ✅ مطابق |
| 9 | **رمز الحالة 6: 💲 دولار (AccountIssue — باقي حساب)** | مطلوب. | `AccountIssue → "CurrencyUsd"`. | ✅ مطابق |
| 10 | **رمز الحالة 7: 🏅 نجمة (Completed — مكتمل)** | مطلوب. | `Completed → "Star"`. | ✅ مطابق |
| 11 | **حالة مُجمَّعة للمريض (Aggregate Status) لعرضها بجوار اسمه في قائمة اليوم** | تعرض حالة "الأدنى" (الأسوأ) للتنبيه. | `TestResultsListService.GetPatientsByFilterAsync`: `tests.Any(t => t.Status == New) ? New : tests.Min(t => t.Status)`. | ✅ مطابق |
| 12 | **بيانات المريض المحدد** (الاسم، الكود، الجنس، السن) | تظهر في المنطقة 2. | `TestResultsListView.xaml` يعرض `FullName`, `AttendanceNumber`, `VisitCount` فقط — لا يعرض الجنس ولا السن ولا الكود/LabId. | ⚠️ جزئي |
| 13 | **خانة صغيرة خضراء بعدد الزيارات (لعملاء LabId)** | لون أخضر مميّز. | `PatientListItem` يحتوي `VisitCount`، لكن نُصَ في XAML يظهر كنص عادي أمام "عدد التحاليل:" — **بدون خلفية خضراء وبدون شرط LabId** (هو في الحقيقة عدد التحاليل لا عدد الزيارات). | ⚠️ جزئي |
| 14 | **تمييز المريض المهم بالاسم الأحمر (`Patient.IsImportant = true`)** | خانة الاسم حمراء. | ملف XAML يستخدم `IsImportant` عن طريق `InverseBoolToVisibilityConverter` مرتبطاً بـ `FontWeight` — **لا يوجد تمييز باللون الأحمر لخانة الاسم**. | ❌ غير مطابق |
| 15 | **نسخ الكود / LabId بالنقرة الواحدة على الخانة** | Click-to-copy. | لا يوجد أي `MouseLeftButtonDown`/`Clipboard.SetText` مرتبط بخانات الكود/LabId (الحقول أصلاً غير معروضة). | ❌ غير مطابق |
| 16 | **قائمة تحاليل المريض** (`PatientTest` per Visit) | المنطقة 3. | `TestResultsListViewModel.LoadPatientTestsAsync` + `PatientTests` ObservableCollection معروضة في ListBox العمود الثاني. | ✅ مطابق |
| 17 | **علامات الحالة أمام كل تحليل (كُتب/راجَع/طُبع/سُلِّم)** | 4 علامات مرئية. | يوجد `PackIcon` واحد فقط مرتبط بـ `Status` عبر `TestStatusToIconConverter` — يُعرض رمز واحد يمثل الحالة الحالية. **لا توجد 4 علامات مستقلة/checkbox-style** لكل من: كُتب/راجَع/طُبع/سُلِّم أمام كل تحليل. | ⚠️ جزئي |
| 18 | **النقر المزدوج على تحليل يفتح نافذة إدخال النتائج (F4)** | Double-click behavior. | يوجد `KeyBinding Key="Enter" Command="OpenTestEntryCommand"` — الفتح مربوط بـ Enter فقط، **لا يوجد `MouseDoubleClick`/`InputBindings` على `ListBoxItem`** لفتح النافذة. | ❌ غير مطابق |
| 19 | **Enter يفتح نافذة إدخال النتائج للتحليل المحدد** | مطلوب. | مطبق: `KeyBinding Key="Enter" Command="{Binding OpenTestEntryCommand}"` + `OpenTestEntryAsync` يفتح `TestResultEntryView` بـ `ShowDialog`. | ✅ مطابق |
| 20 | **زر "تمت" (F9) للتحاليل الخارجية — يعتبر النظام أن التحليل كُتب وطُبع ورُاجع (Toggle)** | Toggle مركّب. | `ToggleEnteredAsync` في `TestResultsListService`: لا يقوم إلا بتغيير `Status` من `New → Entered`، **بدون تعيين IsReviewed/IsPrinted معاً**، وبدون إمكانية العكس (Toggle حقيقي). | ⚠️ جزئي |
| 21 | **F8 — Toggle مراجعة/إلغاء مراجعة** | Toggle عكسي. | `ToggleReviewedAsync`: يعكس `IsReviewed` (`pt.IsReviewed = !pt.IsReviewed`) ثم يعيّن `Status = Reviewed` عند الإدخال — لكن عند التراجع لا يُعاد `Status` للحالة السابقة. | ⚠️ جزئي |
| 22 | **F12 — Toggle طُبعت/لم تُطبع** | Toggle عكسي. | `TogglePrintedAsync`: نفس المشكلة — يعكس `IsPrinted` لكن لا يُعيد Status عند التراجع. | ⚠️ جزئي |
| 23 | **زر "ملاحظات" — إضافة/تعديل ملاحظة للمريض** | مطلوب. | في `TestResultsListView.xaml` يوجد قسم "ملاحظات" مع TextBox + زر "حفظ الملاحظات" مرتبط بـ `UpdateNoteCommand` → `UpdatePatientNoteAsync(patient.Notes)`. | ✅ مطابق |
| 24 | **زر "بيانات المرضى" للانتقال إلى F1** | زر انتقال. | `OpenPatientDataCommand` موجود في ViewModel + يُنفّذ `_navigationService.NavigateTo<PatientEntryViewModel>()` — لكن **لا يوجد زر مرئي له في `TestResultsListView.xaml`** (فقط الأمر معرَّف). | ⚠️ جزئي |
| 25 | **زر "تقرير مجمع"** | طباعة تقرير مجمع. | زر موجود في XAML مرتبط بـ `PrintAggregateReportCommand` — لكن التنفيذ Stub يعرض رسالة "ستُفعَّل هذه الوظيفة في Function 4". | ⚠️ جزئي |
| 26 | **زر "ورقة عمل / أمر شغل"** | مطلوب. | زر موجود، `PrintWorksheetCommand` = Stub. | ⚠️ جزئي |
| 27 | **زر "ظرف"** | طباعة ظرف. | زر موجود، `PrintEnvelopeCommand` = Stub. | ⚠️ جزئي |
| 28 | **زر "تاريخ مرضي"** | زر أساسي. | زر موجود، `PrintHistoryCommand` = Stub. | ⚠️ جزئي |
| 29 | **زر "تاريخ مخصص"** | **خارج النطاق — قرار 9** | غير موجود في الواجهة. | ✅ مطابق (قرار مالك) |
| 30 | **زر "تقرير فارغ"** | مطلوب. | زر موجود، `PrintBlankReportCommand` = Stub. | ⚠️ جزئي |
| 31 | **زر "العودة" إلى القائمة الرئيسية** | مطلوب. | في `MainWindow.xaml` زر "العودة للرئيسية" (`CloseFunctionCommand`) يُعالج الأمر — لا Esc key binding داخل `TestResultsListView`. | ⚠️ جزئي |
| 32 | **زر "ب" — سجل التدقيق (Audit Log) — Admin فقط (قرار 7)** | صلاحية Admin. | زر "ب - سجل التدقيق" موجود مع `IsEnabled="{Binding IsAdmin}"` + `ShowAuditCommand(CanExecute = IsAdmin)`. **لكن التنفيذ Stub**: "ستُفعَّل هذه الوظيفة في Phase 10". | ⚠️ جزئي |
| 33 | **زر "ت" — تتبع مالي — Admin فقط (قرار 7)** | صلاحية Admin. | زر "ت - تتبع مالي" موجود مع نفس البنية — التنفيذ Stub. | ⚠️ جزئي |
| 34 | **فلتر: غير المكتوبة** | Filter. | `FilterMode="Unwritten"` مطبَّق: `tests.Any(t => t.Status == TestStatus.New)`. | ✅ مطابق |
| 35 | **فلتر: غير المراجعة** | Filter. | `Unreviewed`: `tests.Any(t => !t.IsReviewed)`. | ✅ مطابق |
| 36 | **فلتر: غير المطبوعة** | Filter. | `Unprinted`: `tests.Any(t => !t.IsPrinted)`. | ✅ مطابق |
| 37 | **فلتر: المهمين (`IsImportant`)** | Filter. | `Important`: `visit.Patient.IsImportant`. | ✅ مطابق |
| 38 | **فلتر: المستقلين (Individual)** | فلتر BillingSystem. | في الكود: `case "Individual"` يُضيف كل المرضى دون فلترة على `BillingSystem` — **التعليق نفسه يذكر "BillingSystem filter - simplified for now"**. | ❌ غير مطابق |
| 39 | **فلتر: معامل (LabToLab)** | Filter. | نفس المشكلة — case فارغ، لا يفلتر فعلياً. | ❌ غير مطابق |
| 40 | **فلتر: جهات (Referral)** | Filter. | نفس المشكلة — case فارغ، لا يفلتر فعلياً. | ❌ غير مطابق |
| 41 | **الانتقال ليوم آخر (بأسهم/تاريخ محدد)** | Date navigation. | `PreviousDayCommand` + `NextDayCommand` + عرض `SelectedDate` — يعمل ويعيد التحميل عبر `OnSelectedDateChanged`. | ✅ مطابق |
| 42 | **قيمة `FilterMode` في ComboBox** | نصوص عربية للفلاتر. | ComboBoxItems تعرض النصوص العربية ("الكل", "غير مكتوب", "غير مراجع", ...) لكنها **لا تُطابق قيم الـ switch ("All", "Unwritten", ...)** — الـ Binding سيرسل النص العربي كـ Content فيفشل مطابقة الـ switch case. | ❌ غير مطابق |
| 43 | **بحث بكود المريض (VisitCode)** | جزء من مربع البحث. | `SearchByCodeAsync(code)` يبحث في `p.LabId == code \|\| p.FileCode == code \|\| p.VisitCode == code` — يدعم الأنواع الثلاثة. | ✅ مطابق |
| 44 | **بحث بكود المعمل (LabId)** | جزء من مربع البحث. | مغطى في نفس الاستعلام. | ✅ مطابق |
| 45 | **بحث بكود الملف (FileCode)** | جزء من مربع البحث. | مغطى في نفس الاستعلام. | ✅ مطابق |
| 46 | **بحث برقم الحضور (التسلسل اليومي — قرار 6)** | تسلسل يومي بسيط 1..N. | `SearchByAttendanceNumberAsync(number, forDate)` = `v.DailySequenceNumber == number && v.VisitDate.Date == d.Date`. | ✅ مطابق |
| 47 | **بحث بالاسم داخل مرضى اليوم** | مطلوب في المنطقة 8. | **غير موجود** — لا يوجد حقل بحث بالاسم ولا استعلام مخصص للاسم. | ❌ غير مطابق |
| 48 | **مسار الوصول العام: F2 (بيانات مرضى) — يعمل من داخل F3** | Global shortcut. | `MainWindow.xaml` فقط يحتوي `KeyBinding Key="F2"` — لا يوجد `KeyBinding F2` داخل `TestResultsListView`. | ⚠️ جزئي |
| 49 | **F3 (بحث)** | مطلوب في F3. | `MainWindow` يحتوي `KeyBinding F3 → OpenSearchCommand` — لا يوجد داخل `TestResultsListView`. | ⚠️ جزئي |
| 50 | **F4 (تحديث/تركيز على النافذة نفسها)** | مطلوب. | `MainWindow` يحتوي `KeyBinding F4` — يعيد فتح النافذة. لا يوجد داخل الـ UserControl. | ⚠️ جزئي |
| 51 | **F5 (Refresh)** | مطلوب. | `TestResultsListView` يحتوي `KeyBinding Key="F5" Command="RefreshCommand"` مرتبط بـ `RefreshAsync`. | ✅ مطابق |
| 52 | **F6 (تسليم — Delivery)** | مطلوب. | `MainWindow` يحتوي `KeyBinding F6 → OpenDeliveryCommand` — لا يوجد داخل الـ UserControl. | ⚠️ جزئي |
| 53 | **F7 (عينات مرسلة للخارج)** | مطلوب في المرجع. | **لا يوجد أي KeyBinding F7** لا في `MainWindow` ولا في `TestResultsListView`. | ❌ غير مطابق |
| 54 | **F8 — ToggleReviewed** | مطلوب. | موجود: `KeyBinding Key="F8" Command="ToggleReviewedCommand"`. | ✅ مطابق |
| 55 | **F9 — ToggleEntered/تمت** | مطلوب. | موجود: `KeyBinding Key="F9" Command="ToggleEnteredCommand"`. | ✅ مطابق |
| 56 | **F12 — TogglePrinted** | مطلوب. | موجود: `KeyBinding Key="F12" Command="TogglePrintedCommand"`. | ✅ مطابق |
| 57 | **Enter — عرض تحاليل المريض المحدد / فتح نافذة الإدخال للتحليل** | مطلوب. | `KeyBinding Key="Enter" Command="OpenTestEntryCommand"` — يفتح فقط نافذة الإدخال، **لا يفصل بين "عرض التحاليل" و"فتح نافذة"** (السلوك مبسّط). | ⚠️ جزئي |
| 58 | **Up / Down — التنقل بين المرضى والتحاليل** | تنقل داخلي. | ListBox يحتوي على تنقل افتراضي بمفاتيح الأسهم — يعمل ضمنياً. | ✅ مطابق |
| 59 | **Ctrl — التنقل بين مربع البحث/قائمة المرضى/قائمة التحاليل** | تنقل بين المناطق. | **لا يوجد أي معالجة لـ Ctrl** لنقل التركيز بين المناطق الثلاث. | ❌ غير مطابق |
| 60 | **Esc — العودة للقائمة الرئيسية** | مطلوب. | **لا يوجد `KeyBinding Escape`** في `TestResultsListView.xaml`. | ❌ غير مطابق |
| 61 | **Audit Logging عند F8/F9/F12** | `AuditLog` entity. | `TestResultsListService.LogAudit(...)` يُستدعى داخل `ToggleReviewedAsync/ToggleEnteredAsync/TogglePrintedAsync` ويكتب لـ `AuditLogs`. | ✅ مطابق |
| 62 | **زر "بحث" منفصل + مربع "بحث بالكود" + مربع "رقم الحضور"** | جزء من التخطيط. | موجود: TextBox `SearchCode` + TextBox `SearchAttendanceNumber` + `SearchCommand`. | ✅ مطابق |

---

## 2) قوائم النواقص / الاختلافات / الإضافات — F3

### 🔴 النواقص (Missing — في المرجع، غير موجود في نظامي)

1. **N-3-01** — عرض بيانات المريض الكاملة في المنطقة 2: **الجنس، السن، الكود، LabId، FileCode، VisitCode** غير معروضة في XAML.
2. **N-3-02** — **الخانة الخضراء لعدد الزيارات الفعلية** (مأخوذة من عدد `PatientVisit` للمريض ذي LabId المسجّل) — الحالي يعرض `VisitCount` = عدد التحاليل، وبدون خلفية خضراء.
3. **N-3-03** — **تمييز اسم المريض المهم باللون الأحمر** — Binding موجود على `IsImportant` لكن مرتبط بـ `FontWeight` عبر Converter خاطئ للنوع، بدلاً من `Foreground=Red`.
4. **N-3-04** — **نسخ الكود / LabId بالنقرة الواحدة** (Click-to-Copy) — غير موجود.
5. **N-3-05** — **علامات الحالة الأربع المستقلة أمام كل تحليل** (كُتب/راجَع/طُبع/سُلِّم) — الحالي يعرض أيقونة واحدة تمثل `Status` المُجمَّع.
6. **N-3-06** — **النقر المزدوج (Double-Click) على تحليل لفتح نافذة الإدخال** — الحالي مربوط بـ Enter فقط.
7. **N-3-07** — **زر "بيانات المرضى" لم يُعرَض في الواجهة** رغم وجود الأمر `OpenPatientDataCommand`.
8. **N-3-08** — **البحث بالاسم داخل قائمة مرضى اليوم** — غير موجود.
9. **N-3-09** — **KeyBindings المحلية داخل `TestResultsListView`** لـ: F2, F3, F4, F6, F7, Esc — كلها غير موجودة (F2/F3/F4/F6 موجودة عالمياً فقط في `MainWindow`).
10. **N-3-10** — **Ctrl** لتبديل التركيز بين البحث والقائمتين — غير موجود.
11. **N-3-11** — **`Esc`** للعودة إلى القائمة الرئيسية — غير مربوط.
12. **N-3-12** — **البحث بالاسم لـ "مرضى اليوم فقط"** — غير موجود.
13. **N-3-13** — **زر "F7" — عينات مرسلة للخارج** — غير موجود على مستوى النظام كله (لا KeyBinding ولا زر).
14. **N-3-14** — **تنفيذ فعلي (Real Implementation)** لأزرار: تقرير مجمع، ورقة عمل، ظرف، تاريخ مرضي، تقرير فارغ — كلها Stub تعرض رسالة "ستُفعَّل".
15. **N-3-15** — **تنفيذ فعلي** لزري "ب" (سجل التدقيق) و"ت" (تتبع مالي) — Stub رغم وجود صلاحية Admin.
16. **N-3-16** — **تنفيذ فعلي لفلاتر Individual/LabToLab/Referral** — الـ cases فارغة لا تفلتر شيئاً (تعليق داخل الكود يعترف بذلك: `simplified for now`).

### 🟡 الاختلافات (Different — موجود لكن يعمل بشكل مختلف)

1. **D-3-01** — **الحالة المُجمَّعة** تُعرض كأيقونة واحدة بجوار المريض (باستخدام `TestStatus.Min`)، بينما المرجع يذكر أن الأيقونة بجوار كل مريض قد تشير لأدنى حالة، لكن السلوك المتوقع أشمل: **الأيقونة أمام كل تحليل بشكل مستقل**.
2. **D-3-02** — **`ToggleReviewedAsync`** لا يعود بحالة `TestStatus` إلى `Entered` عند التراجع (يبقى `Reviewed`).
3. **D-3-03** — **`ToggleEnteredAsync`** ليس Toggle حقيقي — يذهب فقط من `New → Entered` ولا يعود، وليس Toggle كما وصف المرجع لسلوك "تمت" (F9).
4. **D-3-04** — **`TogglePrintedAsync`** لا يعود بحالة `TestStatus` عند التراجع.
5. **D-3-05** — **`FilterMode` Binding**: القيم في ComboBox عربية ("الكل", "غير مكتوب"، …) لكن الـ switch يقارنها بقيم إنجليزية ("All", "Unwritten", …) — **الفلتر لا يعمل فعلياً إلا في حالة "الكل" التي تُطابق دون قصد**.
6. **D-3-06** — **`FilterMode` نوع البيانات** = `string` يُعبَّأ بـ `ComboBoxItem` كامل بدلاً من `SelectedValuePath="Content"` — يجعل الـ Binding يستقبل كائن `ComboBoxItem` كنص.
7. **D-3-07** — **`VisitCount`** في `PatientListItem` = عدد التحاليل (`tests.Count`) وليس عدد الزيارات (`PatientVisits.Count for patient with LabId`).

### 🟢 الإضافات (Additions — موجود في نظامي غير مذكور في المرجع)

1. **A-3-01** — **زر "تسليم" و"بحث (F6)"** في عمود الأوامر — إضافات ناتجة عن دمج F3 مع F5/F6 (Retro-integration من Phase 11/12، ذُكرت في `history.md`).
2. **A-3-02** — عرض **`AttendanceNumber` بجوار الاسم بصيغة `[N]`** في قائمة اليوم — إضافة UX عملية غير مذكورة صراحة في المرجع.
3. **A-3-03** — استخدام **`materialDesign:PackIcon`** كطبقة عرض للأيقونات (تفصيل تنفيذي مقبول).
4. **A-3-04** — **`AuditLog` entity + `LogAudit()`** يُكتب على كل F8/F9/F12 (تجهيز مسبق لتنفيذ Admin buttons لاحقاً).

---

# 🔹 الجزء الثاني: مقارنة الوظيفة الرابعة — إدخال نتائج التحاليل

**النطاق المرجعي**: `Real_Lab_System_Reference.pdf` الصفحات 26–28 + `Reference_Functions_3_and_4_Detailed_Analysis.md`.
**النطاق التنفيذي**: `Views/Windows/TestResultEntryView.xaml`، `ViewModels/Pages/TestResultEntryViewModel.cs`، `Services/Interfaces/ITestResultEntryService.cs`، `Services/Implementations/TestResultEntryService.cs`، `Services/Implementations/AutoCalculationService.cs`، `Services/Implementations/NormalRangeService.cs`، `Models/Domain/TestResult.cs`، `Models/Domain/SavedComment.cs`، `Models/Domain/CalculationConstant.cs`، `Views/Windows/CalculationConstantsView.xaml`، `ViewModels/Pages/CalculationConstantsViewModel.cs`.

## 1) جدول المقارنة التفصيلي — F4

| # | المكون (حقل/زر/خاصية/اختصار/سلوك) | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|-------------------------------------|------------------|------------------------|--------|
| 1 | **فتح النافذة بالنقر المزدوج على تحليل من قائمة F3** | Double-click. | `TestResultsListViewModel.OpenTestEntryAsync` يفتح `TestResultEntryView` كـ Dialog — **لكن مُشغَّل بـ Enter فقط، لا Double-Click**. | ❌ غير مطابق (الآلية موجودة، لكن الإطلاق يستخدم Enter لا Double-Click) |
| 2 | **بيانات المريض في أعلى النافذة** (الاسم، الكود، الجنس، السن، تاريخ الزيارة) | مطلوب. | XAML يعرض 3 خانات فقط: `PatientName`, `PatientLabId`, `TestTitle` — **لا الجنس ولا السن ولا تاريخ الزيارة**. | ⚠️ جزئي |
| 3 | **عناصر البروفيل — تظهر كل عناصر البروفيل مع تفعيل ما اختير فقط** | `LabTestElement` — العناصر المختارة Enabled، الباقي Disabled. | `TestResultEntryService.GetPatientTestWithProfileAsync` يجلب **كل عناصر `LabTest.Elements`** ثم يبني `TestResultRow` لكل عنصر بلا تمييز بين "مختار" و"غير مختار" — **جميع العناصر Enabled دون تمييز**. | ❌ غير مطابق |
| 4 | **حقل إدخال قيمة لكل عنصر (`TestResult.Value`)** | مطلوب. | `DataGridTemplateColumn "Value"` مع `TextBox Text="{Binding Value}"`. | ✅ مطابق |
| 5 | **التلوين التلقائي عند إدخال النتيجة (عادي/غير طبيعي/حرج) — قاعدة أضيق مدى يفوز (قرار 16)** | `INormalRangeService.EvaluateValueAsync` + `GetMatchingRangeAsync` (narrowest wins). | `EvaluateResultAsync` في `TestResultEntryService` يستدعي `GetMatchingRangeAsync + EvaluateValueAsync` بشكل صحيح. **لكن** لا يوجد أي استدعاء لهذه الآلية أثناء الكتابة في `TextBox` — التلوين يعتمد على `existing?.IsCritical`/`IsAbnormal` المحفوظة مسبقاً فقط، لا يُحدَّث تلقائياً عند الكتابة. | ⚠️ جزئي |
| 6 | **قاعدة "أضيق مدى يفوز"** | مطلوبة. | مطبقة في `NormalRangeService.GetMatchingRangeAsync`: `matched.OrderBy(nr => nr.AgeTo - nr.AgeFrom).First()`. | ✅ مطابق |
| 7 | **التمييز النصي (High/Low/Critical) بجانب القيمة** | مطلوب. | يوجد عمود "Flag" يعرض `FlagText` + `CellColor` — لكن لا يُحدَّث تلقائياً أثناء الإدخال. | ⚠️ جزئي |
| 8 | **علامات مراجعة/طباعة (Checkboxes) لكل عنصر** | مطلوبة أمام كل تحليل. | **غير موجودة** — لا Checkbox column في DataGrid لعناصر البروفيل. | ❌ غير مطابق |
| 9 | **ظل رمادي/ملوّن للتحاليل المطبوعة** | UX cue. | غير موجود. | ❌ غير مطابق |
| 10 | **حقل التعليق يدوي** | مطلوب. | `TextBox Text="{Binding Comment}"` يعمل. | ✅ مطابق |
| 11 | **زر "Saved Comments" (برتقالي) — قائمة `SavedComment`** | مطلوب. | زر `PickSavedCommentCommand` موجود — **لكن التنفيذ لا يعرض قائمة اختيار** بل يستقبل `SavedComment?` كـ parameter (لا يوجد Popup/ListView يعرض القائمة). | ⚠️ جزئي |
| 12 | **زر "Comment from Normal Range" — جلب التعليق حسب النتيجة من `NormalRange.LowComment/HighComment/CriticalComment`** | مطلوب. | زر موجود في XAML **لكن غير مربوط بأي Command** (بدون `Command="..."`) — **لا يعمل**. | ❌ غير مطابق |
| 13 | **زر "Undo Comment" — التراجع عن آخر تعليق** | مطلوب. | زر موجود في XAML **بدون Command** — **لا يعمل**. | ❌ غير مطابق |
| 14 | **زر "حفظ" (Save)** | مطلوب. | `SaveCommand` → `SaveAsync` يُنفّذ التحقق ثم يستدعي `SaveResultsAsync`. | ✅ مطابق |
| 15 | **زر "طباعة" (Print)** | مطلوب. | `PrintCommand` → `PrintReportAsync` → QuestPDF `ReportPdfGenerator`. | ✅ مطابق |
| 16 | **زر "معاينة الطباعة" (Preview)** | مطلوب. | `PreviewCommand` → `PreviewReportAsync`. | ✅ مطابق |
| 17 | **زر "تاريخ مرضي" (Patient history) — الاسم يتغير حسب عدد نتائج المريض** | مطلوب. | `HistoryButtonLabel` observable + `OpenHistoryCommand` — لكن التنفيذ Stub ("تاريخ مرضي مخصص غير متاح حالياً") ولا آلية تغيير للاسم بناءً على العدد. | ⚠️ جزئي |
| 18 | **زر "رجوع" (Back — بدون حفظ)** | مطلوب. | `BackCommand` → يغلق النافذة (لا حفظ). | ✅ مطابق |
| 19 | **زر "القائمة الرئيسية" (Main Menu)** | مطلوب. | `MainMenuCommand` → يغلق النافذة فقط (لا يعود للقائمة الرئيسية فعلياً). | ⚠️ جزئي |
| 20 | **زر "تاريخ مخصص"** | **خارج النطاق — قرار 9** | غير موجود. | ✅ مطابق (قرار مالك) |
| 21 | **الحسابات التلقائية — Hct = Hgb × 3.3** | مطلوب. | `AutoCalculationService.CalculateHctAsync(hgb)` = `hgb × HctMultiplier` (Seed = 3.3). **لكن** لا يوجد استدعاء تلقائي من `TestResultEntryViewModel` عند إدخال Hgb — الخدمة معرَّفة ولا تُستخدَم في مسار الإدخال. | ⚠️ جزئي |
| 22 | **Hgb% حسب العمر/الجنس (8.25/7.50/6.25/6.75)** | مطلوب. | `CalculateHgbPercentAsync(hgb, ageYears, gender)` مطبق بالمعاملات الأربعة. **لا يوجد استدعاء تلقائي من الـ ViewModel**. | ⚠️ جزئي |
| 23 | **INR = (PT_مريض ÷ PT_ضابط) ^ ISI (قرار 8)** | مطلوب. | `CalculateINRAsync` مطبق مع قراءة ISI + ControlTime من `CalculationConstants`. **لا يوجد استدعاء تلقائي من الـ ViewModel**. | ⚠️ جزئي |
| 24 | **PTT Ratio = PTT_مريض ÷ PTT_ضابط (قرار 8)** | مطلوب. | `CalculatePTTRatioAsync` مطبق. **لا يوجد استدعاء تلقائي من الـ ViewModel**. | ⚠️ جزئي |
| 25 | **زر "Constants" — تعديل ثوابت PT/PTT/Hgb%** | مطلوب. | `EditConstantsCommand` يفتح `CalculationConstantsView` كـ Dialog + `CalculationConstantsViewModel` مع `LoadConstantsAsync`/`SaveAsync`. | ✅ مطابق |
| 26 | **F8 — اعتبار مراجعة (ToggleReviewed)** | مطلوب. | `KeyBinding F8 → ToggleReviewedCommand`. **لكن `ToggleReviewedAsync` في الـ ViewModel هو one-way (يعيّن `IsReviewed=true` فقط بدون Toggle حقيقي)**. | ⚠️ جزئي |
| 27 | **F9 — حفظ (Save)** | مطلوب. | `KeyBinding F9 → SaveCommand`. | ✅ مطابق |
| 28 | **F11 — معاينة طباعة** | مطلوب. | `KeyBinding F11 → PreviewCommand`. | ✅ مطابق |
| 29 | **F12 — طباعة التقرير** | مطلوب. | `KeyBinding F12 → PrintCommand`. | ✅ مطابق |
| 30 | **Enter — الانتقال للخانة التالية / حفظ في آخر خانة** | ملاحية Focus. | **غير مُنفَّذ** — لا `KeyBinding Enter` ولا معالجة Focus navigation داخل DataGrid. | ❌ غير مطابق |
| 31 | **Up / Down — التنقل بين خانات النتائج** | ملاحية. | Default DataGrid navigation يعمل ضمنياً. | ✅ مطابق |
| 32 | **Esc — العودة بدون حفظ** | مطلوب. | `KeyBinding Key="Escape" Command="BackCommand"`. | ✅ مطابق |
| 33 | **إدارة `PatientTest.Status` تلقائياً عند حفظ النتائج** | Save → Status = Entered. | `SaveResultsAsync` يعيّن `Status = TestStatus.Entered` + `EnteredByUserId` + `EnteredAt` + `AuditLog`. | ✅ مطابق |
| 34 | **Audit Logging عند Save/Review/Print** | مطلوب. | `TestResultEntryService` يكتب لـ `AuditLogs` عند كل من: `SaveResultsAsync` (Enter)، `MarkReviewedAsync` (Review)، `PrintReportAsync` (Print). | ✅ مطابق |
| 35 | **Transaction-based Save** | لضمان الاتساق. | `SaveResultsAsync` يفتح `BeginTransactionAsync`، يحذف القديم، يضيف الجديد، Commit — مع Rollback عند الفشل. | ✅ مطابق |
| 36 | **Seeded Constants الافتراضية** | القيم من صفحة 62. | `NewLabDbContext` يحقن 8 ثوابت (Hgb×4, CBC×1, PT×2, PTT×1) بالقيم الصحيحة (8.25/7.50/6.25/6.75/3.3/1.0/12.0/30.0). | ✅ مطابق |
| 37 | **FluentValidation لـ `TestResult`** | جودة إدخال. | `TestResultValidator` موجود + مسجَّل في `App.xaml.cs`. | ✅ مطابق (إضافة) |
| 38 | **PDF Report عبر QuestPDF (RTL) — يشمل جدول نتائج + بيانات مريض + INR/PTT Ratio** | مطلوب حسب قرار 8. | `ReportPdfGenerator` موجود ومسجَّل — لكن لم يتم التحقق من احتواء الـ PDF على INR/PTT Ratio بشكل صريح (قسم PDF خارج نطاق ملفات F4 المفحوصة). | ⚠️ جزئي (يحتاج تحقق) |

---

## 2) قوائم النواقص / الاختلافات / الإضافات — F4

### 🔴 النواقص (Missing — في المرجع، غير موجود في نظامي)

1. **N-4-01** — **فتح النافذة بالنقر المزدوج على تحليل** من قائمة F3 — الحالي يستخدم Enter فقط.
2. **N-4-02** — **عرض بيانات المريض الكاملة** في المنطقة 1: الجنس، السن، تاريخ الزيارة — الحالي يعرض الاسم + LabId + عنوان التحليل فقط.
3. **N-4-03** — **تمييز عناصر البروفيل المختارة (Enabled) من غير المختارة (Disabled)** — الحالي يعرض جميع العناصر مفعّلة بلا تمييز.
4. **N-4-04** — **التلوين والتمييز التلقائي أثناء الكتابة (Real-time evaluation)** — يجب استدعاء `EvaluateResultAsync` عند تغيّر `Value` وتحديث `CellColor` + `FlagText` لحظياً.
5. **N-4-05** — **Checkboxes للطباعة/المراجعة لكل عنصر** — غير موجودة.
6. **N-4-06** — **ظل رمادي/ملوّن للعناصر المراجعة/المطبوعة** — غير موجود.
7. **N-4-07** — **Popup/ListView لاختيار Saved Comment** — الأمر موجود لكن UX الاختيار غير مبني.
8. **N-4-08** — **Command لزر "Comment from Normal Range"** — الزر بلا Command (لا يعمل).
9. **N-4-09** — **Command لزر "Undo Comment"** + Stack للتعليقات — غير موجود.
10. **N-4-10** — **آلية تغيير اسم زر "تاريخ مرضي"** حسب عدد نتائج المريض المخزّنة — غير مطبقة.
11. **N-4-11** — **استدعاء الحسابات التلقائية (Hct/Hgb%/INR/PTT Ratio)** من الـ ViewModel عند تغيّر قيم Hgb/PT/PTT — الخدمة `IAutoCalculationService` جاهزة لكن **لم تُستدعَ من الـ ViewModel**.
12. **N-4-12** — **Enter navigation** بين خانات القيم داخل DataGrid — غير مطبق (لا KeyBinding ولا Focus logic).
13. **N-4-13** — **`Toggle` حقيقي لـ F8 Reviewed** — الحالي one-way.
14. **N-4-14** — **`MainMenuCommand`** يجب أن يعود إلى `MainWindow` مباشرة (يغلق أي نوافذ فرعية)، وليس فقط يغلق نافذة F4.
15. **N-4-15** — **تمرير معدلات NormalRange للعرض** أمام كل عنصر (نص Reference Range من `NormalRange.NormalRangeText`) — غير معروض في DataGrid.

### 🟡 الاختلافات (Different — موجود لكن يعمل بشكل مختلف)

1. **D-4-01** — **`TestResultRow.CellColor`** يُحسب مرة واحدة عند التحميل من `existing?.IsCritical/IsAbnormal` — لا يُحدَّث عند الكتابة.
2. **D-4-02** — **`BuildResults()`** يعيّن `IsAbnormal = (CellColor != "Transparent")` بدل الاستدعاء الفعلي لـ `EvaluateResultAsync` أثناء البناء — يجعل التقييم يعتمد على لون خلية قديم.
3. **D-4-03** — **`SavedComments`** يتم تحميلها لكل `LabTest.Id` (`_parentTest.Id`) — لا يتم تصنيفها حسب `Type (Low/High/Critical/General)` عند اختيار الجلب من `NormalRange` (لا يوجد Filter).
4. **D-4-04** — **`ToggleReviewedAsync` في الـ ViewModel** يعيّن `IsReviewed = true` فقط ولا يعكس الحالة.
5. **D-4-05** — **`OpenHistoryCommand`** يعرض رسالة "تاريخ مرضي مخصص غير متاح" — الصياغة تخلط بين "تاريخ مرضي" (متطلَب) و"تاريخ مخصص" (خارج النطاق قرار 9).
6. **D-4-06** — **Layout عام**: XAML يستخدم 3 صفوف بدل الـ 5 مناطق المذكورة في المرجع (بيانات مريض / عناصر / مراجعة+طباعة / تعليقات / أوامر) — بعض المناطق مندمجة في صف الأوامر.

### 🟢 الإضافات (Additions — موجود في نظامي غير مذكور صراحة في المرجع)

1. **A-4-01** — **`FluentValidation` لـ `TestResult`** قبل الحفظ — تحسين جودة.
2. **A-4-02** — **`Transaction-based Save`** يحذف كل النتائج القديمة ويضيف الجديدة ذرياً — تصميم آمن.
3. **A-4-03** — **`CalculationConstantsView` منفصلة** مع DataGrid لتحرير كل الثوابت في مكان واحد (بدل تحرير مضمَّن في نافذة F4) — تصميم مقبول.
4. **A-4-04** — **`AuditLog` entries** لكل من: إدخال، مراجعة، طباعة — تنفيذ يتجاوز الحد الأدنى للمرجع.
5. **A-4-05** — **`SavedComment` + `CalculationConstant` كـ DbSet مستقلين** + Seed 8 constants — بنية بيانات أنيقة.
6. **A-4-06** — **PDF Report عبر QuestPDF** (RTL) — تنفيذ فوق المتطلب البدائي.

---

# 🛠️ الجزء الثالث: خطة تنفيذ Parts لسد الفجوات — F3

> كل Part ينتهي بحالة Build خضراء (0 errors, 0 warnings) قبل الانتقال للتالي.
> **قاعدة النطاق**: لا يُلمس أي ملف خارج قائمة "الملفات المتأثرة" في Part إلا لضرورة سلامة البناء.

## Part F3-P1 — إثراء `PatientListItem` ببيانات المريض التفصيلية
**الملفات المتأثرة**:
- `Services/Interfaces/ITestResultsListService.cs` (توسعة `PatientListItem` record)
- `Services/Implementations/TestResultsListService.cs` (تعبئة الحقول الجديدة)

**العمل**:
1. توسيع `PatientListItem` ليشمل: `Gender`, `AgeValue`, `AgeUnit`, `LabId`, `FileCode`, `VisitCode`, `ActualVisitCount` (عدد `PatientVisit` للمريض بشرط `LabId != null`)، `Notes`.
2. في `GetPatientsByFilterAsync` / `SearchByCodeAsync` / `SearchByAttendanceNumberAsync`: تعبئة كل الحقول من `visit.Patient`.
3. حساب `ActualVisitCount` عبر `_context.PatientVisits.CountAsync(v => v.PatientId == patientId && v.Patient.LabId != null)`.

**التحقق**: Build يمر — Debug يظهر الحقول الجديدة في `SelectedPatient`.

## Part F3-P2 — عرض بيانات المريض في المنطقة 2 + التمييز الأحمر + الخانة الخضراء
**الملفات المتأثرة**:
- `Views/Pages/TestResultsListView.xaml` (تعديل StackPanel "Patient info")

**العمل**:
1. إضافة `TextBlock` لكل من: Gender، AgeValue+AgeUnit، LabId، FileCode، VisitCode.
2. تعديل `TextBlock` الاسم: إضافة `Foreground` مرتبط بـ `Converter` جديد `BoolToRedBrushConverter` بحيث `IsImportant=true → Red`.
3. إحاطة عدد الزيارات بـ `Border Background="Green" Padding="4"` + عرض `ActualVisitCount` (بدل `VisitCount`).
4. إضافة `MouseLeftButtonDown` handlers على خانات `LabId` و`VisitCode` تنسخ للحافظة (Click-to-Copy).

**التحقق**: تحديد مريض مهم → الاسم أحمر؛ خانة زيارات خضراء بعدد صحيح؛ النقر على LabId ينسخه.

## Part F3-P3 — إصلاح Binding الفلاتر (Comboمشكلة القيم العربية → English switch)
**الملفات المتأثرة**:
- `Views/Pages/TestResultsListView.xaml` (ComboBox الفلاتر)
- `ViewModels/Pages/TestResultsListViewModel.cs` (لا تغيير — أو تحويل Enum-based)

**العمل**:
1. تغيير كل `ComboBoxItem Content="الكل"` → `ComboBoxItem Content="الكل" Tag="All"`.
2. تعديل ComboBox: `SelectedValuePath="Tag"` و `SelectedValue="{Binding FilterMode}"` (بدل `SelectedItem`).
3. أو (أفضل): تحويل `FilterMode` إلى Enum `PatientListFilter { All, Unwritten, Unreviewed, Unprinted, Important, Individual, LabToLab, Referral }` مع Binding مباشر.

**التحقق**: اختيار "غير مكتوب" يستدعي `case "Unwritten"` فعلياً.

## Part F3-P4 — تنفيذ فعلي لفلاتر Individual/LabToLab/Referral
**الملفات المتأثرة**:
- `Services/Implementations/TestResultsListService.cs` (منطق الفلاتر البيلينج)

**العمل**:
1. جلب `Patient.BillingSystem` مع كل `Visit` (Include).
2. الاستبدال:
   - `case "Individual"` → `where visit.Patient.BillingSystem == BillingSystem.Individual`.
   - `case "LabToLab"` → `BillingSystem.LabToLab`.
   - `case "Referral"` → `where visit.Patient.ReferralId != null && visit.Patient.Referral?.IsDefaultLab == false`.
3. حذف التعليق `"BillingSystem filter - simplified for now"`.

**التحقق**: اختيار "معمل مع معمل" يعرض فقط مرضى `BillingSystem.LabToLab`.

## Part F3-P5 — Double-Click على تحليل لفتح F4
**الملفات المتأثرة**:
- `Views/Pages/TestResultsListView.xaml` (`ListBox` الخاص بـ `PatientTests`)

**العمل**:
1. إضافة `Style` على `ListBoxItem` مع `EventSetter Event="MouseDoubleClick"` → معالج code-behind أو `InteractionTrigger` من `Microsoft.Xaml.Behaviors`.
2. المعالج يستدعي `(DataContext as TestResultsListViewModel).OpenTestEntryCommand.Execute(null)`.

**التحقق**: النقر المزدوج على تحليل يفتح نافذة F4.

## Part F3-P6 — علامات الحالة الأربع المستقلة أمام كل تحليل
**الملفات المتأثرة**:
- `Views/Pages/TestResultsListView.xaml` (`DataTemplate` لعناصر `PatientTests`)
- `Converters/TestStatusToIconConverter.cs` (إن لزم تحويلات إضافية — أو استخدام `BooleanToIconConverter` جديد)

**العمل**:
1. تعديل `DataTemplate` ليعرض 4 `PackIcon`s متتالية:
   - `Entered` = FileDocument (منظّم من `IsWritten` أو `Status >= Entered`)
   - `Reviewed` = CheckDouble (من `IsReviewed`)
   - `Printed` = Printer (من `IsPrinted`)
   - `Delivered` = Cart (من `IsDelivered`)
2. لون الأيقونة: أخضر عند التفعيل، رمادي عند الإيقاف.

**التحقق**: تحليل مُدخَل ومُراجَع فقط يظهر بـ 2 أيقونتين ملوّنتين + 2 رماديتين.

## Part F3-P7 — إصلاح Toggle الحقيقي لـ F8/F9/F12
**الملفات المتأثرة**:
- `Services/Implementations/TestResultsListService.cs` (`ToggleReviewedAsync`, `ToggleEnteredAsync`, `TogglePrintedAsync`)

**العمل**:
1. `ToggleReviewedAsync`: عند `IsReviewed = true → false`، أعِد `Status` إلى `Entered` (أو `New` إن لم يُكتب).
2. `ToggleEnteredAsync`: جعله Toggle حقيقي — إن كان `Status == Entered` بلا نتائج فعلية، أعده لـ `New`. أضِف تعيين `IsReviewed=true`, `IsPrinted=true` عند F9 (السيناريو الخارجي للأجهزة).
3. `TogglePrintedAsync`: عند `IsPrinted = true → false`، أعِد `Status` إلى `Reviewed` (أو الحالة السابقة الصحيحة).

**التحقق**: F8 مرتين على نفس التحليل → تعود الحالة كما كانت.

## Part F3-P8 — إضافة KeyBindings المحلية داخل `TestResultsListView`
**الملفات المتأثرة**:
- `Views/Pages/TestResultsListView.xaml` (`UserControl.InputBindings`)
- `ViewModels/Pages/TestResultsListViewModel.cs` (إضافة `OpenPatientDataCommand` binding + Escape logic)

**العمل**:
1. إضافة داخل `UserControl.InputBindings`:
   - `F2 → OpenPatientDataCommand`
   - `F3 → OpenSearchCommand`
   - `F4 → RefreshCommand`
   - `F6 → OpenDeliveryCommand`
   - `Escape → CloseCommand` (يستدعي `_navigationService.Back()` أو يرفع Event للـ MainWindow).
2. إضافة `KeyBinding F7` مربوطة بـ Stub `OpenExternalSpecimensCommand` (يعرض رسالة "قيد التنفيذ") — للحفاظ على اكتمال الاختصارات المرجعية.

**التحقق**: كل مفتاح يستدعي أمرَه من داخل النافذة نفسها.

## Part F3-P9 — البحث بالاسم داخل مرضى اليوم
**الملفات المتأثرة**:
- `Services/Interfaces/ITestResultsListService.cs` (إضافة `SearchByNameAsync`)
- `Services/Implementations/TestResultsListService.cs`
- `ViewModels/Pages/TestResultsListViewModel.cs` (خاصية `SearchName` + منطق داخل `SearchAsync`)
- `Views/Pages/TestResultsListView.xaml` (TextBox جديد)

**العمل**:
1. `SearchByNameAsync(string partialName, DateTime forDate)` — `Contains` على `Patient.FullName` مع `VisitDate == forDate`.
2. `SearchAsync` في الـ ViewModel: أضِف فرعاً ثالثاً `else if (!string.IsNullOrWhiteSpace(SearchName)) …`.
3. TextBox جديد بـ Hint "بحث بالاسم" مربوط بـ `SearchName`.

**التحقق**: كتابة جزء من الاسم + بحث → تظهر النتائج المطابقة.

## Part F3-P10 — إضافة زر "بيانات المرضى" مرئي + تفعيل Stubs الطباعة الأساسية
**الملفات المتأثرة**:
- `Views/Pages/TestResultsListView.xaml` (زر جديد أعلى قسم التقارير)
- `ViewModels/Pages/TestResultsListViewModel.cs` (فعليّة `PrintAggregateReport`/`PrintWorksheet`/`PrintEnvelope`/`PrintHistory`/`PrintBlankReport`)
- `Services/Interfaces/IReportPdfGenerator.cs` + `Services/Implementations/ReportPdfGenerator.cs` (توسيع لدعم 5 قوالب)

**العمل**:
1. زر "بيانات المرضى (F2)" مرئي في العمود الأيسر مربوط بـ `OpenPatientDataCommand`.
2. توسيع `IReportPdfGenerator` بمعاملات: `GenerateAggregateAsync(patientId, date)`, `GenerateWorksheetAsync(patientId)`, `GenerateEnvelopeAsync(patientId)`, `GenerateHistoryAsync(patientId)`, `GenerateBlankAsync(patientId)`.
3. استبدال Stubs في الـ ViewModel بـ استدعاءات فعلية لـ `IReportPdfGenerator`.

**التحقق**: كل زر ينتج ملف PDF قابل للفتح.

## Part F3-P11 — تنفيذ سجل التدقيق (ب) وتتبع مالي (ت) — Admin
**الملفات المتأثرة**:
- `ViewModels/Pages/TestResultsListViewModel.cs` (`ShowAudit`, `ShowFinancialTracking`)
- ملف جديد: `Views/Windows/AuditLogView.xaml` + ViewModel `AuditLogViewModel.cs`
- `Services/Implementations/TestResultsListService.cs` (استخدام `GetAuditForPatientAsync`/`GetAuditForTestAsync` الموجودة)

**العمل**:
1. `AuditLogView` DataGrid يعرض: Timestamp، UserName، Action، Details.
2. `AuditLogViewModel` يحمّل قائمتين: `PatientAudits` + `TestAudits`.
3. استبدال Stubs في `ShowAudit`/`ShowFinancialTracking` بفتح `AuditLogView` كـ Dialog.
4. الحرص على `CanExecute = IsAdmin` (موجود مسبقاً).

**التحقق**: تسجيل دخول بمستخدم Admin → الزران يفتحان النافذة؛ مستخدم عادي → الزران Disabled.

## Part F3-P12 — إصلاح `VisitCount` = عدد الزيارات الفعلي (لا عدد التحاليل)
**الملفات المتأثرة**:
- `Services/Implementations/TestResultsListService.cs`

**العمل**:
- استبدال `tests.Count` في بناء `PatientListItem` بـ `ActualVisitCount` المحسوبة في Part F3-P1.

**التحقق**: خانة الزيارات الخضراء تعرض عدد الزيارات لا عدد التحاليل.

---

# 🛠️ الجزء الرابع: خطة تنفيذ Parts لسد الفجوات — F4

> كل Part ينتهي بحالة Build خضراء (0 errors, 0 warnings) قبل الانتقال للتالي.

## Part F4-P1 — إثراء بيانات المريض في `TestResultEntryViewModel`
**الملفات المتأثرة**:
- `ViewModels/Pages/TestResultEntryViewModel.cs` (خصائص جديدة)
- `Views/Windows/TestResultEntryView.xaml` (توسيع Row 0 لعرض 5 خانات)

**العمل**:
1. إضافة `[ObservableProperty]`: `patientGender`, `patientAge` (formatted `AgeValue + AgeUnit`)، `visitDate`.
2. تعبئتها في `LoadForPatientTestAsync` من `_patient` + `patientTest.Visit`.
3. تعديل Grid في XAML من 3 أعمدة إلى 5 أعمدة (Name / LabId / Gender / Age / VisitDate) + Title للتحليل في صف علوي.

**التحقق**: النافذة تعرض المعلومات الخمس أعلاها.

## Part F4-P2 — تمييز عناصر البروفيل المختارة (Enabled) عن غير المختارة (Disabled)
**الملفات المتأثرة**:
- `Services/Interfaces/ITestResultEntryService.cs` (إرجاع قائمة `SelectedElementIds` أو `IsSelectedForPatient` flag)
- `Services/Implementations/TestResultEntryService.cs` (`GetPatientTestWithProfileAsync`)
- `ViewModels/Pages/TestResultEntryViewModel.cs` (`TestResultRow.IsEnabled`)
- `Views/Windows/TestResultEntryView.xaml` (DataGrid columns `IsReadOnly` مرتبط بـ `IsEnabled`)

**العمل**:
1. تحديد مصدر "العناصر المختارة": يمكن الاعتماد على وجود `TestResult` سابق للعنصر أو حقل جديد `PatientTestElement (PatientTestId, LabTestElementId, IsSelected)` — الحل الأبسط: قائمة `SelectedElementIds` تُخزَّن في `PatientTest.SelectedElementIdsJson`.
2. `TestResultRow` يضاف له `IsEnabled` (default true للعناصر المختارة).
3. DataGrid: `IsReadOnly="{Binding !IsEnabled}"` مع تلوين رمادي للعناصر Disabled.

**التحقق**: بروفيل CBC مع اختيار Hgb + Hct فقط → العناصر الأخرى تظهر رمادية غير قابلة للتحرير.

## Part F4-P3 — التلوين والتمييز التلقائي أثناء الكتابة
**الملفات المتأثرة**:
- `ViewModels/Pages/TestResultEntryViewModel.cs` (`TestResultRow.OnValueChanged` partial method)
- `Services/Interfaces/ITestResultEntryService.cs` (Wrap إضافي لـ `EvaluateResultAsync` مع `labTestElementId`)

**العمل**:
1. جعل `TestResultRow` يعرف `_evaluator` (delegate) يمرَّر من الـ ViewModel.
2. عند تغيّر `Value` (partial method `OnValueChanged`): جرِّب Parse لـ decimal → إن نجح استدعِ `_entryService.EvaluateResultAsync(value, labTestId, patient)` → عيّن `CellColor` و `FlagText` من نتيجة `NormalRangeEvaluation`.
3. الألوان: Normal=Transparent، AbnormalLow/High=`#FFF9800`، CriticalLow/High=`#FFF44336` + نص FlagText.

**التحقق**: إدخال قيمة Hgb=20 لذكر بالغ → خلفية حمراء + FlagText="Critical High" فوراً.

## Part F4-P4 — Checkboxes للطباعة/المراجعة لكل عنصر
**الملفات المتأثرة**:
- `Views/Windows/TestResultEntryView.xaml` (2 أعمدة CheckBox جديدة)
- `ViewModels/Pages/TestResultEntryViewModel.cs` (`TestResultRow.IsReviewed`, `IsPrinted`)

**العمل**:
1. عمودان جديدان: "Reviewed" (`CheckBox Bound to IsReviewed`) و "Printed" (`CheckBox Bound to IsPrinted`) في DataGrid.
2. `SaveResultsAsync` يحمل حالة الـ Checkboxes إلى `TestResult` (Boolean fields — يحتاج توسيع الكيان + Migration).

**التحقق**: تعليم Checkbox يُحفظ ويظهر عند إعادة فتح النافذة.

## Part F4-P5 — Popup لاختيار Saved Comment
**الملفات المتأثرة**:
- `Views/Windows/TestResultEntryView.xaml` (Popup أو Menu مرفق بالزر)
- `ViewModels/Pages/TestResultEntryViewModel.cs` (`SavedComments` معروضة في Popup)

**العمل**:
1. تغليف زر "Saved Comments" بـ `materialDesign:PopupBox` أو `ContextMenu` يعرض `ItemsControl` من `SavedComments`.
2. النقر على عنصر ينفّذ `PickSavedCommentCommand` مع الـ `SavedComment` المختار (Command موجود).

**التحقق**: النقر يفتح قائمة، اختيار تعليق يضيفه لحقل `Comment`.

## Part F4-P6 — تفعيل زر "Comment from Normal Range"
**الملفات المتأثرة**:
- `Views/Windows/TestResultEntryView.xaml` (ربط Command)
- `ViewModels/Pages/TestResultEntryViewModel.cs` (`PickCommentFromNormalRangeCommand`)
- `Services/Interfaces/INormalRangeService.cs` (استخدام `EvaluateValueAsync` — موجود)

**العمل**:
1. أمر جديد `PickCommentFromNormalRangeAsync`:
   - يحدّد الصف المفعّل حالياً (SelectedRow) → Parse قيمته.
   - `range = GetMatchingRangeAsync(labTestId, patient)`.
   - `eval = EvaluateValueAsync(range, value)`.
   - حسب `eval.Category`: التقط `range.LowComment`/`HighComment`/`CriticalComment` وأضِفه لـ `Comment`.
2. ربط الزر بـ Command.

**التحقق**: مع نتيجة Low → يظهر تعليق `LowComment` تلقائياً.

## Part F4-P7 — تفعيل زر "Undo Comment" + Stack
**الملفات المتأثرة**:
- `ViewModels/Pages/TestResultEntryViewModel.cs` (`UndoLastCommentCommand` + `Stack<string> _commentHistory`)
- `Views/Windows/TestResultEntryView.xaml` (ربط Command)

**العمل**:
1. عند كل `Pick*Command`: `_commentHistory.Push(Comment)` قبل التعديل.
2. `UndoLastCommentAsync`: `Comment = _commentHistory.Pop()`.
3. `CanExecute = _commentHistory.Any()`.

**التحقق**: إضافة تعليق ثم Undo → يعود Comment للحالة السابقة.

## Part F4-P8 — استدعاء الحسابات التلقائية من الـ ViewModel
**الملفات المتأثرة**:
- `ViewModels/Pages/TestResultEntryViewModel.cs`

**العمل**:
1. بعد Save أو عند تغيّر قيمة `Hgb`/`PT`/`PTT`:
   - إن كان name = "Hgb" → احسب `Hct = CalculateHctAsync(hgb)` واملأ الصف Hct إن كان فارغاً.
   - إن كان name = "Hgb" مع `_patient.AgeValue+Gender` → احسب `Hgb%` واملأ الصف Hgb%.
   - إن كان name = "PT" → احسب `INR` واملأ الصف INR.
   - إن كان name = "PTT" → احسب `PTT Ratio` واملأ الصف Ratio.
2. حماية: تخطى الحساب إذا لم يوجد صف مطابق بالاسم في `ResultRows`.

**التحقق**: إدخال Hgb=15 لذكر عمره 30 → الصف Hct = 49.5، صف Hgb% = 240.

## Part F4-P9 — Enter navigation بين الخانات + Save في الأخيرة
**الملفات المتأثرة**:
- `Views/Windows/TestResultEntryView.xaml` (KeyBinding على DataGrid + Behavior)
- `Views/Windows/TestResultEntryView.xaml.cs` (معالج `PreviewKeyDown` على TextBoxes)

**العمل**:
1. Attach `KeyDown` handler على TextBox داخل `DataGridTemplateColumn.CellTemplate`:
   - على `Enter`: إن لم يكن آخر صف → Focus للصف التالي (`DataGrid.MoveFocus`) وإلا `SaveCommand.Execute()`.
2. اختبار متأنٍ للـ Up/Down (DataGrid يوفرها تلقائياً).

**التحقق**: Enter داخل خانة قيمة ينقل التركيز للأسفل؛ في آخر خانة يحفظ ويغلق.

## Part F4-P10 — إصلاح Toggle حقيقي لـ F8 Reviewed + تغيير اسم زر Patient history
**الملفات المتأثرة**:
- `ViewModels/Pages/TestResultEntryViewModel.cs` (`ToggleReviewedAsync`, `LoadForPatientTestAsync`)
- `Services/Implementations/TestResultEntryService.cs` (تعديل `MarkReviewedAsync` لدعم Toggle)

**العمل**:
1. `MarkReviewedAsync(int patientTestId, bool state)` — يقبل الحالة الجديدة بدل التعيين الأعمى.
2. الـ ViewModel: `ToggleReviewedAsync` = `await MarkReviewedAsync(_patientTestId, !IsReviewed); IsReviewed = !IsReviewed;`.
3. `HistoryButtonLabel`: احسب في `LoadForPatientTestAsync` عدد النتائج المخزنة لهذا المريض من هذا البروفيل: `count = TestResults.Count(r => r.PatientTest.LabTestId == _parentTest.Id && r.PatientTest.Visit.PatientId == _patient.Id)`، ثم صيغة: `HistoryButtonLabel = count > 0 ? $"تاريخ مرضي ({count})" : "تاريخ مرضي"`.

**التحقق**: F8 مرتين → `IsReviewed` يعود False. زر التاريخ يعرض العدد.

## Part F4-P11 — عرض `NormalRangeText` أمام كل عنصر
**الملفات المتأثرة**:
- `ViewModels/Pages/TestResultEntryViewModel.cs` (`TestResultRow.NormalRangeText`)
- `Services/Implementations/TestResultEntryService.cs` (`GetPatientTestWithProfileAsync` يجلب النطاقات الخاصة بالمريض)
- `Views/Windows/TestResultEntryView.xaml` (عمود جديد "Normal Range")

**العمل**:
1. أثناء بناء `TestResultRow`: استدعِ `_normalRangeService.GetMatchingRangeAsync(labTestId, _patient)` واملأ `NormalRangeText = range?.NormalRangeText`.
2. DataGrid: عمود جديد `Binding="{Binding NormalRangeText}" IsReadOnly=True Width=200`.

**التحقق**: كل صف يعرض معدله الطبيعي المطابق لجنس/سن المريض.

## Part F4-P12 — إصلاح `MainMenuCommand` ليعود فعلياً إلى Dashboard
**الملفات المتأثرة**:
- `ViewModels/Pages/TestResultEntryViewModel.cs`
- `ViewModels/Pages/MainDashboardViewModel.cs` (طريقة عامة `ReturnToDashboard()` — أو استخدام `INavigationService` موجود)

**العمل**:
1. `MainMenu()` في الـ ViewModel: يغلق نافذة `TestResultEntryView` + يغلق `TestResultsListView` (يستدعي `_navigationService.ReturnToDashboard()` أو رفع Event).
2. تنفيذ `ReturnToDashboard()` في `MainDashboardViewModel.CloseFunction()` نفسه (موجود مسبقاً).

**التحقق**: الضغط "Main Menu" داخل F4 → إغلاق كل النوافذ الفرعية والعودة إلى Toolbar الرئيسي.

---

# 📌 الملخص التنفيذي

## F3 — Test Results List
- **بنود مطابقة (✅)**: 31 من 62
- **بنود جزئية (⚠️)**: 20 من 62
- **بنود غير مطابقة (❌)**: 11 من 62
- **قرارات مالك محترمة (خارج النطاق)**: 1 (قرار 9 — تاريخ مخصص محذوف صراحة)

**أهم الفجوات الحرجة**:
1. الفلاتر غير المكتوبة/غير المراجعة/… لا تعمل فعلياً بسبب Comboمشكلة (D-3-05, D-3-06).
2. لا يوجد Double-Click على تحليل (N-3-06).
3. أزرار الطباعة الخمسة كلها Stubs (N-3-14).
4. عرض بيانات المريض في المنطقة 2 ناقص جداً (N-3-01, N-3-02, N-3-03).

## F4 — Test Result Entry
- **بنود مطابقة (✅)**: 17 من 38
- **بنود جزئية (⚠️)**: 13 من 38
- **بنود غير مطابقة (❌)**: 7 من 38
- **قرارات مالك محترمة (خارج النطاق)**: 1 (قرار 9)

**أهم الفجوات الحرجة**:
1. الحسابات التلقائية معرَّفة في الخدمة لكن لا تُستدعى من الـ ViewModel (N-4-11).
2. التلوين التلقائي أثناء الكتابة غير مطبق (N-4-04).
3. زرا "Comment from Normal Range" و"Undo Comment" بلا Command (N-4-08, N-4-09).
4. تمييز عناصر البروفيل المختارة عن غير المختارة غير مطبق (N-4-03).
5. Enter navigation بين الخانات غير مطبق (N-4-12).

## القرارات الـ17 — حالة الالتزام في F3/F4

| القرار | الحالة | التعليق |
|--------|--------|---------|
| **6** | ✅ محترم | `SearchByAttendanceNumberAsync` = تسلسل يومي بسيط. |
| **7** | ✅ محترم في Binding | `IsAdmin` gates موجودة، لكن التنفيذ الفعلي للأزرار Stub. |
| **8** | ⚠️ محترم في الخدمة، غير مستدعى من ViewModel | الخدمة `IAutoCalculationService` كاملة والثوابت في DB. |
| **9** | ✅ محترم | زر "تاريخ مخصص" غير موجود صراحة. |
| **16** | ✅ محترم | `GetMatchingRangeAsync` يطبق narrowest wins. |
| **17** | ✅ محترم | Enum `Gender` = Male/Female فقط (يُتحقق في F8 لا F4، لكن F4 يعتمد عليه). |

---

# 🔚 الخاتمة

- **حجم العمل المتبقي لـ F3**: 12 Parts (P1..P12).
- **حجم العمل المتبقي لـ F4**: 12 Parts (P1..P12).
- **إجمالي Parts**: 24 Parts سيؤدي تنفيذها بالتسلسل + Build خضراء بعد كل واحد إلى تطابق كامل بين نافذتَي F3/F4 في نظام NewLab وبين المرجع، ضمن حدود القرارات الـ17.

**نصيحة تنفيذ**: البدء بـ Parts التي تعالج الاختلافات السلوكية الحرجة (D-3-05 → F3-P3، N-4-11 → F4-P8، N-4-04 → F4-P3، N-3-06 → F3-P5) لأنها تُظهر تحسّناً ملموساً للمستخدم فور تنفيذها، مع تأجيل تنفيذ سجلات التدقيق التفصيلية (F3-P11) لدورة تنفيذ لاحقة.

---
**نهاية التقرير — F3_F4_Gap_Analysis_and_Implementation_Plan.md**
