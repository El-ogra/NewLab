# 🎯 وثيقة تحليل النظام المرجعي وخطة الشرائح الرأسية — NewLab (الإصدار v3 — مُصحَّح)

> **الحالة**: تحليل وتخطيط فقط — لا يوجد أي كود مكتوب.
> **Commit مرجعي**: `786729799cdc3f8d204bd98c05168c88e0d66add`
> **رسالة Commit**: "بعد إضافه ملفات النظام المرجعي وإزاله ملف خطة العمل"
> **تم التحقق**: ✅ (تم الاستنساخ والـ checkout والتأكد من مطابقة الـ codebase)
> **النطاق**: الوظائف الثمانية المذكورة في الخطوة 5 فقط — لا شيء آخر.
> **هذا الإصدار**: يطبّق 17 قراراً إلزامياً من مالك المشروع فعلياً (وليس في الملاحظات فقط)، ويعيد ترتيب الوظائف بترتيب التنفيذ التقني، ويتتبّع آثار كل قرار في كامل الوثيقة.
> **المصادر المرجعية**: (1) `Docs/Reference system/output.pdf` داخل المستودع، (2) `Docs/Reference system/MinerU_markdown_...md`، (3) `Real_Lab_System_Reference.pdf` — الملف الأصلي المرفق مباشرة (Master Source الأعلى موثوقية عند التعارض).

---

## 📊 ملخص الحالة الراهنة للمشروع NewLab (Baseline لكل الوظائف)

قبل الدخول في الوظائف الثمانية، هذه هي الطبقات الموجودة فعلاً في الـ codebase عند الـ commit المرجعي (تم التحقق منها بالاستنساخ المباشر وفحص الملفات)، وهي **مشتركة** لكل الوظائف الثمانية:

| الطبقة | ما هو موجود فعلاً |
|---|---|
| **Domain Models** | `User` (Id, Username, PasswordHash, FullName, Email, PhoneNumber, IsActive, CreatedAt, LastLoginAt)، `Role` (Id, Name, Description — Seed: Admin/Technician/Receptionist)، `UserRole` (composite PK UserId+RoleId)، `ToolbarItem` (IconKind, Label, Category, Functions)، `FunctionDefinition` (Name, IconName, TargetViewType). لا يوجد أي كيان طبي. |
| **DbContext** | `NewLabDbContext` يحتوي فقط على `DbSet<User>`, `DbSet<Role>`, `DbSet<UserRole>` + Fluent API (UserRole composite key, Username unique index, Role seed). لا يوجد أي `DbSet` طبي. |
| **Migrations** | Migration وحيدة `20260721171559_InitialCreate` أنشأت جداول المصادقة فقط. |
| **Services** | `IApplicationStartupService`، `IAuthService`، `INavigationService` (NavigateTo<T>, GoBack, CurrentViewModel)، `IDialogService`. لا توجد خدمات طبية. |
| **ViewModels** | `ViewModelBase`, `SetupViewModel`, `LoginViewModel`, `MainDashboardViewModel` (يحتوي 11 فئة Toolbar؛ فقط فئة "المرضى" لها 4 `FunctionDefinition` بدون `TargetViewType`). |
| **Views (Windows)** | `SetupView`, `LoginView`, `MainWindow`. |
| **Views (Controls)** | `DashboardContentControl`, `FunctionPlaceholderControl` (Placeholder فقط). |
| **Converters** | `InverseBoolToVisibilityConverter`, `IconNameToKindConverter`. |
| **Navigation** | Stack-based، DI-driven، يعمل عبر `NavigateTo<TViewModel>()` لكن `CurrentContent` يعيد `this` (MainDashboardViewModel) — لم يُربط بـ ContentControl لعرض View فعلي بعد. |
| **DI Container** | `App.xaml.cs` يسجّل DbContext (Scoped)، AuthService (Scoped)، NavigationService (Singleton)، DialogService (Singleton)، و ViewModels (Transient). |
| **Toolbar الحالي** | 11 فئة. فقط فئة "المرضى" تحتوي على 4 FunctionDefinition: "إدخال نتائج التحاليل"، "إضافة وتعديل بيانات المرضى"، "بحث عن مريض"، "تسليم نتائج المرضى" — كلها بـ `TargetViewType = null` وتفتح `FunctionPlaceholderControl`. فئة "بيانات النظام" بلا Functions. |
| **الحزم المتاحة** | `MaterialDesignThemes` 5.1.0, `CommunityToolkit.Mvvm` 8.3.2, `EF Core 8` (SqlServer 8.0.8), `FluentValidation` 11.10.0, `Mapster` 7.4.0, `QuestPDF` 2026.7.1, `BCrypt.Net-Next` 4.2.0, `Serilog`. **يُضاف**: `ZXing.Net` (لباركود — قرار 3). |
| **المجلدات المخططة لكنها غير موجودة** | `Assets/`, `Helpers/`, `Resources/Styles/`, `Models/DTOs/` مذكورة في `history.md` لكنها غير موجودة فعلياً. |

**الاستنتاج العام**: البنية التحتية جاهزة (MVVM + DI + EF Core + MaterialDesign + Toolbar Shell). كل الوظائف الثمانية تتطلب إنشاء طبقات كاملة جديدة وربطها بالـ Toolbar ومفاتيح الاختصار. يجب إنشاء آلية عرض فعالة للـ Views داخل `DashboardContentControl` (عبر `DataTemplate` أو `ViewLocator`).

---

## 🧩 كيانات النطاق الأساسية المشتركة المطلوب إنشاؤها

### كيانات تُبنى في Function 1 (الخطوة 1 من 8):
- `Models/Domain/Patient.cs` — يتضمن أعمدة Boolean منفصلة للحقول الطبية الإضافية (قرار 1)
- `Models/Domain/Referral.cs` (جهة الإحالة)
- `Models/Domain/SpecimenType.cs` (نوع العينة/الحاوية)
- `Models/Domain/PatientVisit.cs` (زيارة/كود يوم)
- `Models/Domain/PatientTest.cs` (تحليل مطلوب لمريض معين)
- `Models/Domain/TestResult.cs` (نتيجة تحليل)
- `Models/Domain/PaymentTransaction.cs` (حركة مالية)
- `Models/Domain/AuditLog.cs` (سجل تدقيق)
- Enums: `Gender` (**Male/Female فقط** — قرار 17)، `BillingSystem` (Individual / LabToLab / Free)، `TestStatus` (New / Entered / Reviewed / Printed / Delivered / AccountIssue / Completed)، `AgeUnit` (Day / Month / Year)، `CodeType` (Case / File / Lab)

### كيانات تُبنى في Function 7 (الخطوة 2 من 8):
- `Models/Domain/TestGroup.cs` (المجموعة/البروفيل) — **بدون حقل Branch** (قرار 5)
- `Models/Domain/LabTest.cs` (تعريف التحليل) — **بدون حقل Branch** (قرار 5)
- `Models/Domain/LabTestElement.cs` (عناصر البروفيل)
- `Models/Domain/ReferralPrice.cs` (**جديد — قرار 15**: LabTestId + ReferralId + Price)
- `Models/Domain/SavedComment.cs` (تعليقات محفوظة)

### كيانات تُبنى في Function 8 (الخطوة 3 من 8):
- `Models/Domain/NormalRange.cs` (المعدل الطبيعي) — **Gender = Male/Female فقط، بدون Both** (قرار 17)

### كيانات تُبنى في Function 2 (الخطوة 4 من 8):
- `Models/Domain/BarcodeSettings.cs` — **بدون BranchNumber** (قرار 5)، يتضمن `LabelWidth` (افتراضي 38) و`LabelHeight` (افتراضي 25) بالملليمتر (قرار 4)

---

## ✅ جدول تطبيق القرارات — Decisions Compliance Table

| رقم القرار | القسم/الـPart المتأثر | ماذا تغيّر فعلياً (قبل → بعد) |
|---|---|---|
| **1** | F1 — Part 1.3، Open Questions | **قبل**: حقول طبية إضافية "كقائمة عامة دون تحديد شكل تخزينها — يحتاج تأكيد". **بعد**: أعمدة `Boolean` منفصلة على كيان `Patient` (`IsFasting`, `FastingHours`, `IsOnAnticoagulant`, `HasLiverTreatment`, `HasAntiviralTreatment`, `HasAntibiotic`, `IsPregnant`, `IsSmoker`) — Open Question مُغلَق ومحذوف. |
| **2** | F1 — Part 1.11 (DeleteCommand)، F6 — Part 6.3 (DeleteCommand) | **قبل**: "صلاحية حذف مرضى — هل نضيف Permission-based Authorization؟" **بعد**: فحص دور `Admin` فقط قبل تنفيذ الحذف؛ `DeleteCommand.CanExecute` يعيد `false` لغير Admin. Open Question مُغلَق في F1 و F6. |
| **3** | F2 — Part 2.7، Baseline (الحزم) | **قبل**: "اختيار مكتبة (ZXing.Net أو BarcodeStandard) — يحتاج قرار المالك". **بعد**: `ZXing.Net` حصراً — يُضاف كـ NuGet Package. Open Question مُغلَق. |
| **4** | F2 — Part 2.1 (BarcodeSettings) | **قبل**: "حجم شريحة الملصقات — يحتاج إعداد قابل للتخصيص أو قيمة افتراضية 38×25 مم". **بعد**: `LabelWidth = 38` و`LabelHeight = 25` (بالملليمتر) كقيم افتراضية قابلة للتعديل في `BarcodeSettings`. Open Question مُغلَق. |
| **5** | F2 — Part 2.1, 2.4, 2.7؛ F7 — Part 7.1, Reference Behavior | **قبل**: `BarcodeSettings` يحتوي `BranchNumber`؛ `LabTest` يحتوي `Branch`؛ `TestGroup` يحتوي `Branch`؛ صيغة الكود 13 خانة تتضمن موضع رقم فرع قابل للتخصيص؛ Open Question "مصدر رقم الفرع". **بعد**: حذف `BranchNumber` من `BarcodeSettings` نهائياً؛ حذف `Branch` من `LabTest` و `TestGroup`؛ حذف "Branch (فرع المعمل المسؤول)" من قائمة الـ 18 حقلاً في Reference Behavior؛ في صيغة الكود، الموضع المخصص للفرع يُثبَّت برمجياً كالثابت `"1"`؛ لا شاشة اختيار فرع، لا إعداد قابل للتخصيص. Open Question محذوف. |
| **6** | F3 — Reference Behavior Summary (المنطقة 8) | **قبل**: "رقم الحضور بصيغة (5) 12 — المرجع لا يشرح البنية الدلالية — يحتاج تأكيد". **بعد**: تفسير كامل باللغة العادية مع مثال عملي (انظر النص المعدَّل في F3). تبين أن "(5) 12" كان خطأ استخراج OCR لـ "(5 أو 21)"، وأن "رقم الحضور" هو التسلسل اليومي لترتيب دخول المريض. Open Question مُغلَق. |
| **7** | F3 — Part 3.7 (ShowAuditCommand)، Reference Behavior (المنطقة 6) | **قبل**: "صلاحيات ب و ت — يشير لنظام Permissions لم يُنشأ بعد". **بعد**: زرا "ب" (سجل التدقيق) و "ت" (تتبع مالي) يتطلبان دور `Admin` فقط — `CanExecute` يتحقق من الدور. Open Question مُغلَق. |
| **8** | F4 — Part 4.5 (AutoCalculationService)، Reference Behavior (ملاحظات) | **قبل**: "معادلات PT, PTT ISI — التفاصيل مقطوعة في الاستخراج — يحتاج مراجعة الـ PDF". **بعد**: استخراج كامل من `Real_Lab_System_Reference.pdf` (الصفحة 62): PT يتطلب إدخال ISI + Control Time + Concentration table كثوابت قبل أول استخدام؛ PTT يتطلب Control Time؛ المعادلة الضمنية INR = (PT_مريض / PT_ضابط)^ISI؛ نسبة PTT = PTT_مريض / PTT_ضابط. تمت إضافتها لـ `IAutoCalculationService` كإعدادات قابلة للإدخال. Open Question مُغلَق. |
| **9** | F4 — Reference Behavior (المنطقة 5)، Part 4.8 | **قبل**: "آلية عرض تاريخ مرضي مخصص — قد يحتاج نافذة منفصلة". **بعد**: زر "تاريخ مرضي مخصص" خارج النطاق رسمياً؛ يُنفَّذ فقط زر "تاريخ مرضي" الأساسي (Patient history — يفتح نافذة تاريخ المريض العامة بدون نافذة منفصلة مخصصة). Open Question مُغلَق. |
| **10** | F5 — Reference Behavior (سيناريو Barcode)، Part 5.8 | **قبل**: "بروتوكول قارئ الباركود — هل يفرض تسلسلاً أم يقبل أي كود؟". **بعد**: تصميم مرن — يقبل أي كود ممسوح ويكتشف نوعه تلقائياً (كود إيصال/ملف/معمل) من بنيته/بادئته، دون فرض تسلسل ثنائي صارم. Open Question مُغلَق. |
| **11** | F5 — Part 5.6 (UnmarkDeliveredCommand) | **قبل**: "زر مستلمة — هل يتطلب صلاحية إدارية؟ المرجع لا يحدد". **بعد**: زر "مستلمة" (تراجع عن تسليم بالخطأ) يتطلب دور `Admin` — `CanExecute` يتحقق. Open Question مُغلَق. |
| **12** | F6 — Part 6.1 (SearchCriteria)، Part 6.2، Part 6.4 | **قبل**: "قاعدة النسخ الاحتياطي — هل ننفذها الآن أم نتركها Stub؟". **بعد**: خيار البحث في قاعدة النسخ الاحتياطي = Stub ظاهر ومعطَّل (`IsEnabled = false`) في الواجهة، بلا آلية Backup فعلية في هذه المرحلة. Open Question مُغلَق. |
| **13** | F6 — Part 6.3 (DeleteCommand) | **قبل**: "صلاحية حذف المريض — نفس السؤال في F1". **بعد**: فحص دور `Admin` فقط — نفس منطق القرار 2. Open Question مُغلَق. |
| **14** | F7 — Part 7.7 (LabTestManagementView) | **قبل**: "لوحة الحروف اللاتينية — يحتاج قائمة محددة". **بعد**: مجموعة صغيرة شائعة طبياً: α β γ μ ± ≤ ≥ ° — تُعرض كأزرار قابلة للنقر، مصممة للتوسعة لاحقاً. Open Question مُغلَق. |
| **15** | F7 — Part 7.1 (كيان جديد ReferralPrice)، F1 — Part 1.6 (CalculateTotalAsync) | **قبل**: "تعدد الأسعار حسب الجهة — هل عبر جدول ReferralPrices منفصل؟". **بعد**: جدول منفصل `ReferralPrices` (LabTestId + ReferralId + Price) يُضاف في F7؛ منطق `CalculateTotalAsync` في F1 يُحدَّث للبحث عن سعر خاص بالجهة قبل الرجوع لـ PatientPrice/LabToLabPrice. Open Question مُغلَق. |
| **16** | F8 — Part 8.3 (GetMatchingRangeAsync) | **قبل**: "أولوية المطابقة عند تداخل المعدلات — أضيق مدى يفوز — يحتاج قرار المالك". **بعد**: قاعدة "أضيق مدى يفوز" (Most Specific Match) مُطبَّقة في `GetMatchingRangeAsync`: عند تعدد المعدلات المطابقة، يُختار الأضيق (الأصغر فارقاً بين AgeFrom و AgeTo). Open Question مُغلَق. |
| **17** | F8 — Part 8.1 (NormalRange.cs)، Reference Behavior | **قبل**: "حقل Sex = Both — هل نضيف قيمة Both أم نلتزم بالمرجع؟". **بعد**: لا تُضاف قيمة `Both` — `Gender` حصراً Male/Female؛ يتطلب إدخال 6 معدلات للتحاليل غير المعتمدة على الجنس (رجال/نساء × ثلاث فئات عمرية) كما يفعل النظام المرجعي. Open Question مُغلَق. |

---

## 🔑 ملخص قرارات مالك المشروع (17 قراراً) — الحالة بعد التطبيق

جميع القرارات الـ17 **مُطبَّقة فعلياً** في أقسام الوثيقة أدناه — لا يتبقّى أي Open Question مرتبط بها. كل تغيير موثَّق في جدول الالتزام أعلاه ومُضمَّن في القسم المعني. لم يتبقَّ سوى Open Questions قليلة جداً ناتجة عن غموض فعلي في المصادر، مذكورة في مكانها.
---

# 🔹 Function 1 (Execution Order: Step 1 of 8) — إضافة بيانات مريض جديد

## 1) Current State vs Target

**الموجود حالياً**:
- زر "إضافة وتعديل بيانات المرضى" في `MainDashboardViewModel` (فئة Patients) لكن بدون `TargetViewType` → يفتح `FunctionPlaceholderControl` فارغ.
- لا يوجد `Patient` model، ولا `DbSet<Patient>`، ولا `PatientService`، ولا `PatientEntryViewModel`، ولا `PatientEntryView`.

**المطلوب**:
- نافذة كاملة "بيانات المرضى" تحاكي النظام المرجعي: إدخال اسم/سن/جنس/لقب، اختيار نظام الحساب (Individual/LabToLab/Free)، جهة الإحالة (Autocomplete)، خانات معلومات إضافية (صيام/حمل/أدوية/تدخين) **كأعمدة Boolean منفصلة على الكيان (قرار 1)**، ملاحظات، اختيار التحاليل المطلوبة من قائمة (روتينية/كل/مجموعات) مع بحث بالاسم والرقم، عرض قائمة التحاليل المختارة مع إمكانية الحذف بالنقر المزدوج، حساب الإجمالي مع خصم ومدفوع وباقٍ **بمراعاة أسعار الجهات من جدول ReferralPrices (قرار 15)**، أزرار (اضافة/تعديل/حذف — **حذف Admin فقط، قرار 2** /الغاء/خالص/موافق/إيصال/باركود/لاب أي دي)، اختصار F2.

## 2) Reference Behavior Summary

مصدر المعلومات: `Docs/Reference system/MinerU_markdown_...md` — قسم "كيف يتم اضافة بيانات مريض جديد للنظام؟" (سطور 152–304)؛ تم التحقق ضد `Real_Lab_System_Reference.pdf` (الصفحات 7–14).

- **الوصول**: من النافذة الرئيسية → زر "المرضى" في الـ Toolbar العلوي → زر "اضافة وتعديل بيانات المرضى"، أو اختصار **F2** من أي نافذة رئيسية.
- **حقول الإدخال** (16 نقطة موصوفة):
  1. أزرار الأوامر (اضافة / تعديل / حذف / إلغاء) — **الحذف يتطلب دور Admin فقط (قرار 2)**.
  2. اسم المريض (نصائح إملائية: إلغاء الهمزة، استبدال التاء المربوطة بهاء، استخدام الياء لا الألف المقصورة). السن يُدخل قبل اللقب حتى يعرض النظام المعدل الطبيعي المناسب.
  3. الجنس (**Male/Female فقط — قرار 17**) واللقب المرتبط تلقائياً بالجنس.
  4. السن — إذا أصغر من شهر: يوم (1-29)، إذا أصغر من سنة: شهر (1-11)، بالسنوات مع كسر (2.5 = سنتين ونصف).
  5. **نظام الحساب**: Individual (سعر المعمل حتى مع جهة إحالة)، Lab to Lab (سعر الجهة إن وجدت قائمة لها وإلا سعر معمل لمعمل)، Free (مجاناً / إجمالي = صفر). **عند BillingSystem = LabToLab يُبحث عن سعر خاص بالجهة في جدول `ReferralPrices` أولاً، فإن لم يُوجد يُستخدم `LabToLabPrice` الافتراضي (قرار 15)**.
  6. اختيار "مريض مهم" لسهولة الوصول لنتائجه.
  7. جهة الإحالة (Autocomplete / حفظ تلقائي للجهات الجديدة / تطبيق خصم الجهة تلقائياً / الجهة الافتراضية = المعمل نفسه إذا لم تُدخل).
  8. حقل نصي "لاب تو لاب" لعدم إظهار اسم الجهة على التقرير.
  9. معلومات إضافية طبية: ساعات صيام، أدوية سيولة، علاج كبد/فيروسات/مضاد حيوي، حمل، تدخين — **كل واحدة كعمود Boolean منفصل على كيان Patient (قرار 1)**.
  10. اختيار نوع العينة إن سُحبت خارج المعمل (لتعليم في التقرير).
  11. ملاحظة عن المريض/العينات المتأخرة (تعليق بين الاستقبال والمعمل).
  12. قائمة منسدلة للتحاليل: (روتينية / كل التحاليل / مجموعات / مجموعات مخصصة) + مربع بحث بالاسم أو رقم التحليل.
  13. إضافة تحليل بالنقر المزدوج أو زر "اضافة التحليل" أو F1 لعرض بيانات التحليل. الحساب يُحدَّث تلقائياً حسب نظام الحساب **ومرجعية `ReferralPrices` (قرار 15)**.
  14. قائمة التحاليل المختارة — حذف واحد بالنقر المزدوج، حذف الكل بزر "حذف الكل".
  15. حساب المريض: خصم برقم أو نسبة، مبلغ مدفوع، زر "خالص" ثم "موافق"، Enter مرتين لتأكيد الدفع.
  16. زر "لاب أي دي" لتسجيل كود المعمل الدائم للمريض (بطاقة باركود) — عند الزيارة القادمة يُقرأ الباركود من خانة "لاب أي دي" ليستدعي بياناته.
- **زر مرضى اليوم**: يفتح قائمة منسدلة بمرضى اليوم لتعديل بياناتهم أو تحصيل مبالغ.
- **إيصال تلقائي**: يمكن ضبط الإيصال ليُطبع تلقائياً بعد حفظ المريض من نافذة "إعدادات إيصال".
- **اختصارات النافذة**: F1 (بيانات تحليل)، F2 (مريض جديد)، F3 (بحث)، F4 (نتائج)، F5 (Refresh)، F6 (تسليم)، F7 (عينات خارج)، F8 (تعديل)، F9 (حفظ)، F10 (حذف — **Admin فقط، قرار 2**)، F11 (باركود)، F12 (إيصال)، Enter (تنقل + إضافة تحليل)، Up/Down/Ctrl، Esc (إلغاء/رجوع).

## 3) Full Vertical-Slice Implementation Plan — أجزاء متتابعة

> كل جزء ينتهي بحالة Build خضراء قبل الانتقال للتالي.

### Part 1.1 — Enums الأساسية
- **الملفات**: `Models/Domain/Enums/Gender.cs` (**Male, Female فقط — قرار 17**)، `BillingSystem.cs`، `AgeUnit.cs`، `TestStatus.cs`، `CodeType.cs`.
- **التبعيات**: لا شيء.
- **الناتج المتوقع**: قيم Enum قابلة للاستخدام لاحقاً.

### Part 1.2 — كيانات مرجعية أساسية (Referral + SpecimenType)
- **الملفات**: `Models/Domain/Referral.cs` (Id, Name, DiscountPercent, IsDefaultLab, CreatedAt)، `Models/Domain/SpecimenType.cs` (Id, Name, ArabicName).
- **التبعيات**: Part 1.1.
- **الناتج**: نموذجان بسيطان بدون علاقات بعد.

### Part 1.3 — كيان Patient (مع الحقول الطبية Boolean — قرار 1)
- **الملفات**: `Models/Domain/Patient.cs` — الخصائص: `Id, FullName, Title, Gender (Male/Female فقط), AgeValue, AgeUnit, BillingSystem, IsImportant, ReferralId, ReferralHiddenOnReport, PhoneNumber, NationalId, LabId, FileCode, VisitCode, Notes, ExternalSpecimenTypeId, DiscountValue, DiscountIsPercent, PaidAmount, TotalAmount, CreatedByUserId, CreatedAt, UpdatedAt`.
- **الحقول الطبية الإضافية (قرار 1 — أعمدة Boolean منفصلة)**: `IsFasting` (bool), `FastingHours` (int?), `IsOnAnticoagulant` (bool), `HasLiverTreatment` (bool), `HasAntiviralTreatment` (bool), `HasAntibiotic` (bool), `IsPregnant` (bool), `IsSmoker` (bool).
- **لا يوجد**: حقل نصي مدمج، ولا جدول منفصل للحالات الطبية — كل حالة كعمود Boolean مستقل على كيان `Patient` مباشرة.
- **التبعيات**: Part 1.1, 1.2.
- **الناتج**: كيان المريض جاهز.

### Part 1.4 — كيان PatientVisit (كود يوم/زيارة)
- **الملفات**: `Models/Domain/PatientVisit.cs` (Id, PatientId, VisitDate, DailySequenceNumber, FullVisitCode).
- **التبعيات**: Part 1.3.
- **ملاحظة**: كود المريض 13 خانة يُبنى ديناميكياً — تفاصيله في Function 2. **رقم الفرع مُثبَّت برمجياً كـ "1" (قرار 5)**.

### Part 1.5 — تحديث DbContext + Migration
- **الملفات المعدَّلة**: `Data/NewLabDbContext.cs` — إضافة `DbSet<Patient>`, `DbSet<Referral>`, `DbSet<SpecimenType>`, `DbSet<PatientVisit>` + Fluent API للعلاقات + Seed لجهة "المعمل" الافتراضية.
- **الملفات الجديدة**: Migration `AddPatientsAndReferrals`.
- **التبعيات**: Parts 1.1–1.4.
- **الناتج**: قاعدة البيانات تحتوي على الجداول الجديدة.

### Part 1.6 — Service Interface: IPatientService (مع دعم ReferralPrices — قرار 15)
- **الملفات**: `Services/Interfaces/IPatientService.cs` بأساليب: `AddAsync`, `UpdateAsync`, `DeleteAsync` (**يُتحقق من دور Admin داخل التنفيذ — قرار 2**)، `GetByIdAsync`, `GetTodayPatientsAsync`, `GetByLabIdAsync`، `CalculateTotalAsync(patientTests, billingSystem, referral, discount)`.
- **منطق CalculateTotalAsync (قرار 15)**:
  - `BillingSystem.Individual` → سعر `PatientPrice` من `LabTest` (حتى مع وجود جهة إحالة).
  - `BillingSystem.LabToLab` → البحث في `ReferralPrices` بـ (LabTestId + ReferralId)؛ إن وُجد يُستخدم سعر الجهة؛ إن لم يُوجد يُستخدم `LabToLabPrice` الافتراضي.
  - `BillingSystem.Free` → الإجمالي = صفر.
  - بعد تحديد السعر الأساسي يُطبَّق خصم الجهة (DiscountPercent) ثم خصم المريض (DiscountValue/DiscountIsPercent).
- **التبعيات**: Part 1.5 + `ReferralPrices` (سيُنشأ في Function 7 — لكن الـ Interface يُعرَّف هنا).

### Part 1.7 — Service Interface: IReferralService
- **الملفات**: `Services/Interfaces/IReferralService.cs`: `SearchByNameAsync(prefix)`, `GetOrCreateAsync(name)`, `GetDefaultLabAsync`.
- **التبعيات**: Part 1.5.

### Part 1.8 — Implementations
- **الملفات**: `Services/Implementations/PatientService.cs`, `ReferralService.cs`.
- **PatientService.DeleteAsync**: يتحقق من دور المستخدم الحالي — إن لم يكن `Admin` يرمي `UnauthorizedAccessException` (قرار 2).
- **التبعيات**: 1.6, 1.7.
- **الناتج**: منطق CRUD + Autocomplete + إنشاء جهة تلقائي + حساب الإجمالي.

### Part 1.9 — تسجيل الخدمات في DI
- **الملفات المعدَّلة**: `App.xaml.cs` — إضافة `services.AddScoped<IPatientService, PatientService>()` و `IReferralService`.

### Part 1.10 — FluentValidation Validator
- **الملفات**: `Models/Validation/PatientValidator.cs` (Rules: FullName مطلوب، AgeValue ≥ 0، Gender مطلوب (**Male/Female فقط**)، تحقق نطاق السن حسب الوحدة 1-29 يوم / 1-11 شهر / 0-120 سنة).
- **التبعيات**: Part 1.3.

### Part 1.11 — PatientEntryViewModel
- **الملفات**: `ViewModels/Pages/PatientEntryViewModel.cs`.
- **الخصائص**: كل حقول Patient + خصائص Boolean للحقول الطبية (`IsFasting`, `IsOnAnticoagulant`, `HasLiverTreatment`, `HasAntiviralTreatment`, `HasAntibiotic`, `IsPregnant`, `IsSmoker`, `FastingHours`) + `IsEditMode`, `IsAddMode`, `ObservableCollection<PatientTestRow> SelectedTests`, `ObservableCollection<LabTest> AvailableTests`, `SearchText`, `TestListFilter`, `TotalAmount`, `DiscountValue`, `PaidAmount`, `Remaining`, `SelectedReferralName`, `ReferralSuggestions`, `IsAdmin` (لتفعيل/تعطيل زر الحذف — قرار 2).
- **الأوامر**: `AddPatientCommand`, `EditCommand`, `DeleteCommand` (**CanExecute يتحقق من `IsAdmin` — قرار 2**)، `CancelCommand`, `SaveCommand`, `PrintReceiptCommand`, `PrintBarcodeCommand` (يستدعي Function 2)، `AddSelectedTestCommand`, `RemoveTestCommand`, `RemoveAllTestsCommand`, `MarkAsPaidCommand`, `TodayPatientsCommand`, `LabIdCommand`.
- **الحقن**: `IPatientService`, `IReferralService`, `ILabTestService` (سيُنشأ في Function 7)، `IDialogService`, `INavigationService`, `IValidator<Patient>`، `IAuthService` (للحصول على دور المستخدم الحالي).

### Part 1.12 — PatientEntryView (UserControl)
- **الملفات**: `Views/Pages/PatientEntryView.xaml` + `.xaml.cs`.
- **التخطيط**: `Grid` بثلاث أعمدة (يمين: بيانات المريض بما فيها **أزرار Boolean للحقول الطبية (قرار 1)**، وسط: قائمة التحاليل + بحث، يسار: التحاليل المختارة + الحسابات). أزرار الأوامر أعلى/أسفل. FlowDirection=RightToLeft. أزرار Material Design. InputBindings لمفاتيح F1..F12/Enter/Esc.
- **زر الحذف**: مرتبط بـ `DeleteCommand` الذي يعتمد على `IsAdmin` — يظهر معطَّلاً لغير Admin (قرار 2).
- **التبعيات**: Part 1.11.

### Part 1.13 — ربط الوظيفة بالـ Toolbar
- **الملفات المعدَّلة**: `MainDashboardViewModel.cs` — للـ FunctionDefinition الخاصة بـ "إضافة وتعديل بيانات المرضى" ضع `TargetViewType = typeof(PatientEntryView)`.
- **الملفات المعدَّلة**: `MainWindow.xaml` أو `FunctionPlaceholderControl` — استبدال المحتوى الوهمي بـ `ContentControl` يعرض `View` عبر `DataTemplate` أو `ViewLocator`.
- **إن لزم**: إنشاء `ViewLocator` بسيط أو تسجيل `DataTemplate` لكل `ViewModel` في `App.xaml`.

### Part 1.14 — تسجيل الاختصار F2 عالمياً
- **الملفات المعدَّلة**: `MainWindow.xaml` — إضافة `Window.InputBindings` بمفتاح F2.

### Part 1.15 — Build Verification
- تشغيل `dotnet build` — 0 errors / 0 warnings.
- اختبار يدوي: Login → Toolbar → "المرضى" → "إضافة وتعديل بيانات المرضى" → إضافة مريض تجريبي مع تفعيل بعض الحقول Boolean (صيام/تدخين) → حفظه → التحقق من ظهوره في DB مع قيم Boolean الصحيحة.
- اختبار Admin: Login كـ Admin → زر الحذف مُفعَّل. Login كـ Technician → زر الحذف معطَّل.

## 4) Open Questions
- لا توجد أسئلة مفتوحة في هذه الوظيفة بعد تطبيق القرارات 1 و 2 و 15 و 17.

---

# 🔹 Function 7 (Execution Order: Step 2 of 8) — إضافة/تعديل بيانات تحليل

## 1) Current State vs Target

**الموجود حالياً**: فئة "بيانات النظام" موجودة في الـ Toolbar بلا `Functions`. لا `LabTest` model، لا `LabTestService`، لا نافذة إدارة تحاليل.

**المطلوب**: نافذة "بيانات التحاليل" مع قائمة كل التحاليل + بحث (بالكود/اسم مجموعة/اسم تحليل) + نموذج بيانات كامل للتحليل المحدد (Test/Report/Bill/History/Arabic/Group/Log group/Collection/Test time/Arrange number/Patient price/LabToLab price/Routine/See report/Print with other/Add with group/Main test) **بدون حقل Branch (قرار 5)** + اختيار SpecimenType + إعداد "يُرسل للخارج" + الأسئلة التي تظهر للمريض + **لوحة حروف لاتينية بمجموعة صغيرة قابلة للتوسعة (قرار 14)** + **جدول أسعار خاص بالجهات ReferralPrices (قرار 15)** + أزرار (إضافة/تعديل/حذف/انتقال للمعدل الطبيعي).

## 2) Reference Behavior Summary

مصدر المعلومات: `Docs/Reference system/MinerU_markdown_...md` — قسم "كيف يتم اضافة او تعديل بيانات تحليل؟" (سطور 682–781)؛ تم التحقيد ضد `Real_Lab_System_Reference.pdf` (الصفحات 33–38).

- الوصول: Toolbar "بيانات النظام" → "بيانات التحاليل".
- **8 مناطق**:
  1. قائمة كل التحاليل المسجلة أو نتيجة البحث.
  2. بحث بكود التحليل / اسم المجموعة / اسم التحليل.
  3. **بيانات التحليل** (**17 حقلاً — تم حذف "Branch" نهائياً بقرار 5**):
     - `Test name` (اسم البرنامج الداخلي)
     - `Report name` (اسم في التقرير — جزءان: كبير وصغير، مثال SGPT/ALT)
     - `Bill name` (اسم في الوصل — جزءان مع دمج تلقائي إن تشابه الجزء الأول)
     - `History name` (اسم مختصر في تاريخ المريض)
     - `Arabic name`
     - `Group name` (اسم المجموعة)
     - ~~`Branch` (فرع المعمل المسؤول)~~ — **محذوف بقرار 5**
     - `Log group` (مجموعة العمل — لطباعة ورقة عمل حسب الجهاز)
     - `Collection` (شروط سحب العينة)
     - `Test time` (بالأيام، صفر = نفس اليوم)
     - `Arrange number` (ترتيب في القوائم)
     - `Patient price` (سعر للمريض المستقل)
     - `Lab to Lab price` (سعر افتراضي للمعامل)
     - `Routine test` (يظهر في التحاليل الروتينية)
     - `See report` (متعدد النتائج مثل البول/الصورة الدم — لا يعرض النتيجة عند بيانات المريض)
     - `Print with other` (تحاليل كبيرة لا تُطبع مع تقرير مجمع)
     - `Add with group` (يُضاف تلقائياً عند اختيار المجموعة، لكنه غير مفعّل مبدئياً)
     - `Main test` (تحليل رئيسي له عناصر — العناصر تلغى هذه العلامة)
  4. تحديد `SpecimenType` (نوع الحاوية) لطباعة ملصق باركود خاص. مثال Creatinine Clearance = مصل + بول 24 ساعة (ملصقان).
  5. علامة "يُرسل للخارج" + اسم الجهة المرسل إليها + تكلفة — عند إدخال مريض له هذا التحليل يظهر في نافذة العينات الخارجية.
  6. أسئلة/تنبيهات تظهر للمريض عند اختيار هذا التحليل (مثال: صيام 12 ساعة للدهون).
  7. **لوحة إدخال حروف لاتينية — مجموعة صغيرة شائعة طبياً قابلة للتوسعة (قرار 14)**: α β γ μ ± ≤ ≥ ° — تُعرض كأزرار قابلة للنقر تُدرج الحرف في حقل النص المُركَّز عليه. مصممة لإضافة رموز أخرى لاحقاً دون إعادة بناء.
  8. أزرار: **اضافة تحليل / تعديل / حذف / الانتقال لنافذة المعدل الطبيعي** (يذهب لـ Function 8).
- **لتعديل عناصر تحليل رئيسي** (مثل صورة الدم): إدخال أول حرفين في خانة البحث الثالثة → عرض كل العناصر → اختيار العنصر.
- **أسعار خاصة بالجهات (قرار 15 — جديد)**: إضافة قسم/منطقة لربط التحليل بأسعار مخصصة لجهات إحالة معينة. كل سجل = (LabTestId + ReferralId + Price). يُستخدم في `CalculateTotalAsync` في Function 1 عند `BillingSystem.LabToLab`.
- لا اختصارات لوحة مفاتيح مذكورة.

## 3) Full Vertical-Slice Implementation Plan

### Part 7.1 — كيانات المرجعية (بدون Branch — قرار 5؛ مع ReferralPrice — قرار 15)
- **الملفات**: `Models/Domain/TestGroup.cs` (Id, Name, ~~Branch~~, LogGroup) — **حقل Branch محذوف (قرار 5)**؛ `Models/Domain/LabTest.cs` (Id, Code, TestName, ReportNameLarge, ReportNameSmall, BillNameLarge, BillNameSmall, HistoryName, ArabicName, TestGroupId, ~~Branch~~, LogGroup, Collection, TestTimeDays, ArrangeNumber, PatientPrice, LabToLabPrice, IsRoutine, IsSeeReport, IsPrintWithOther, IsAddWithGroup, IsMainTest, ParentLabTestId, DefaultSpecimenTypeId, IsSentExternal, ExternalReferralId, ExternalCost, PromptQuestion, IsActive) — **حقل Branch محذوف (قرار 5)**.
- **ملف جديد (قرار 15)**: `Models/Domain/ReferralPrice.cs` (Id, LabTestId, ReferralId, Price) — PK مركب اختياري على (LabTestId + ReferralId) لمنع التكرار، أو PK مستقل مع Index فريد.
- **ملف**: `Models/Domain/LabTestElement.cs` (Id, ParentLabTestId, Name, ArrangeNumber, IsMainTest).

### Part 7.2 — Migration
- إضافة `TestGroups`, `LabTests` (self-reference for parent/children)، `LabTestElements`، **`ReferralPrices` (قرار 15)** + FK إلى `SpecimenType` و `Referral`.
- Seed: مجموعة تحاليل افتراضية (Chemistry, Hematology, Urine) + بضعة تحاليل تجريبية.

### Part 7.3 — Service: ILabTestService (مع دعم ReferralPrices — قرار 15)
- **الأساليب**: `GetAllAsync`, `SearchAsync(code, groupName, testName)`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `GetElementsAsync(parentId)`, `GetRoutineTestsAsync`, `GetByGroupAsync`.
- **أساليب أسعار الجهات (قرار 15)**: `GetReferralPricesAsync(labTestId)` → قائمة بأسعار الجهات المرتبطة بالتحليل، `SetReferralPriceAsync(labTestId, referralId, price)`, `DeleteReferralPriceAsync(labTestId, referralId)`.

### Part 7.4 — FluentValidation: LabTestValidator
- Rules: Code فريد، TestName مطلوب، Prices ≥ 0، TestTimeDays ≥ 0.

### Part 7.5 — Implementation + DI
- `LabTestService.cs` + التسجيل في `App.xaml.cs`.

### Part 7.6 — LabTestManagementViewModel
- **الخصائص**: `ObservableCollection<LabTest> Tests`, `SelectedTest`, `SearchByCode`, `SearchByGroupName`, `SearchByTestName`, `AvailableSpecimenTypes`, `AvailableReferrals`, `AvailableGroups`, `IsEditMode`، **`ObservableCollection<ReferralPrice> ReferralPrices` (قرار 15)**، `SelectedReferralPrice`, `NewReferralPrice_ReferralId`, `NewReferralPrice_Price`.
- **الأوامر**: `SearchCommand`, `AddCommand`, `EditCommand`, `DeleteCommand`, `SaveCommand`, `CancelCommand`, `OpenNormalRangeCommand` (يفتح Function 8)، `BackCommand`، **`AddReferralPriceCommand`, `DeleteReferralPriceCommand` (قرار 15)**.

### Part 7.7 — LabTestManagementView (مع لوحة الحروف اللاتينية — قرار 14)
- **الملفات**: `Views/Pages/LabTestManagementView.xaml`.
- **التخطيط**: يمين قائمة التحاليل + بحث بـ 3 خانات، وسط نموذج البيانات (**17 حقلاً بدون Branch — قرار 5**)، أسفل SpecimenType selector + إعدادات خارجي + الأسئلة، **قسم أسعار الجهات (ReferralPrices — قرار 15)**: DataGrid صغير يعرض الجهة + السعر مع زر إضافة/حذف.
- **لوحة الحروف اللاتينية (قرار 14)**: ItemsControl صغير أسفل حقول النصوص يحتوي على أزرار: α | β | γ | μ | ± | ≤ | ≥ | ° — النقر على زر يُدرج الحرف في حقل النص المُركَّز عليه. مصمم كـ `UserControl` مستقل قابل للتوسعة بإضافة أزرار جديدة لاحقاً.

### Part 7.8 — تحديث الـ Toolbar
- تعديل `MainDashboardViewModel` — إضافة `FunctionDefinition` لـ "بيانات التحاليل" داخل فئة "بيانات النظام" مع `TargetViewType = typeof(LabTestManagementView)`.

### Part 7.9 — Build Verification
- فتح النافذة → إضافة تحليل جديد → البحث يجده → تعديله → إضافة سعر خاص بجهة (قرار 15) → التحقق في DB → حذف التحليل.

## 4) Open Questions
- لا توجد أسئلة مفتوحة بعد تطبيق القرارات 5 و 14 و 15.

---

# 🔹 Function 8 (Execution Order: Step 3 of 8) — إضافة/تعديل المعدلات الطبيعية لتحليل معين

## 1) Current State vs Target

**الموجود حالياً**: لا شيء — لا `NormalRange` model، لا نافذة، لا خدمة.

**المطلوب**: نافذة تُفتح من داخل Function 7 (زر "المعدل الطبيعي") — قائمة المعدلات المسجلة للتحليل المختار، نموذج تفاصيل معدل (16 حقلاً)، أزرار (اضافة مدى / تعديل / حفظ / تراجع / حذف / قائمة التحاليل)، لوحة رموز/أسس، دعم إدخال معدلات ست مرات للتحاليل التي لا تتغير معدلاتها بالجنس أو العمر — **Gender = Male/Female فقط بدون Both (قرار 17)**، **قاعدة "أضيق مدى يفوز" عند تداخل المعدلات (قرار 16)**.

## 2) Reference Behavior Summary

مصدر المعلومات: `Docs/Reference system/MinerU_markdown_...md` — قسم "كيف يتم اضافة او تعديل المعدلات الطبيعية لتحليل معين؟" (سطور 784–871)؛ تم التحقيد ضد `Real_Lab_System_Reference.pdf` (الصفحات 39–44).

- الوصول: من Function 7 → اختيار تحليل → زر "المعدل الطبيعي" (نقطة 4).
- **الاستخدام**: النظام يعرض تلقائياً المعدل المتوافق مع نوع/مرحلة عمر المريض. للتحاليل بلا فرق (مثل السكر) نُدخل **6 معدلات** (رجال 0-120 سنة، نساء 0-120 سنة، رجال 1-29 يوم، نساء 1-29 يوم، رجال 1-11 شهر، نساء 1-11 شهر) — **الالتزام الحرفي: Gender حصراً Male/Female، لا قيمة Both (قرار 17)**. عند اختلاف المعدل بالعمر نُدخل مثلاً (رجال 0-12 سنة + رجال 12-120 سنة + مثلها للنساء).
- **3 مناطق**:
  1. قائمة المعدلات المسجلة لهذا التحليل. لإضافة معدل: زر "اضافة مدى". للتعديل: اختيار المدى ثم "تعديل".
  2. أزرار: اضافة مدى / تعديل / حفظ / تراجع / حذف / قائمة التحاليل.
  3. **بيانات المعدل** (16 حقلاً):
     - `Test name` (يفضل عدم تغييره عن الاسم الرئيسي)
     - `Test unit` (وحدة تظهر بجوار النتيجة)
     - `Sex` (**Male/Female فقط — قرار 17، لا Both**)
     - `Age From : To`
     - `Age unit` (Day/Month/Year)
     - `Normal range` (النص المطبوع)
     - `Low limit` + `High limit` (القيم لاعتبار النتيجة غير طبيعية)
     - `Low flag` + `High flag` (نص يظهر عند التمييز)
     - `Low comment` + `High comment` + `Critical comment`
     - `Critical range` (النص المطبوع في حالة تفعيل طباعة المعدلات الحرجة)
     - `Critical Low limit` + `Critical High limit`
     - `Critical flag`
  4. لوحة إدخال أسس ورموز (يمكن نسخ ولصق) — **نفس مجموعة Function 7 القابلة للتوسعة (قرار 14)**.
- لا اختصارات لوحة مفاتيح مذكورة.

## 3) Full Vertical-Slice Implementation Plan

### Part 8.1 — كيان NormalRange (Gender = Male/Female فقط — قرار 17)
- **الملفات**: `Models/Domain/NormalRange.cs` (Id, LabTestId, TestName, TestUnit, Gender (**Male/Female فقط — لا Both، قرار 17**), AgeFrom, AgeTo, AgeUnit, NormalRangeText, LowLimit, HighLimit, LowFlag, HighFlag, LowComment, HighComment, CriticalComment, CriticalRangeText, CriticalLowLimit, CriticalHighLimit, CriticalFlag).
- **ملاحظة**: لا يوجد قيمة `Both` في `Gender` enum — يتطلب إدخال 6 معدلات للتحاليل غير المعتمدة على الجنس كما يفعل النظام المرجعي تماماً.

### Part 8.2 — Migration
- إضافة `NormalRanges` + FK إلى `LabTests` + Index على (LabTestId, Gender, AgeFrom, AgeTo, AgeUnit).

### Part 8.3 — Service: INormalRangeService (مع قاعدة أضيق مدى يفوز — قرار 16)
- **الأساليب**: `GetForTestAsync(labTestId)`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `GetMatchingRangeAsync(labTestId, patient)`, `EvaluateValueAsync(range, value)`.
- **`GetMatchingRangeAsync` — منطق المطابقة (قرار 16 — قاعدة "أضيق مدى يفوز")**:
  1. تصفية المعدلات حسب `LabTestId` المطابق.
  2. تصفية حسب `Gender` = جنس المريض (**Male أو Female فقط**).
  3. تصفية حسب نطاق العمر: تحويل عمر المريض إلى نفس `AgeUnit` لكل مدى ومقارنته بـ AgeFrom–AgeTo.
  4. **إن تعددت المعدلات المطابقة بعد الخطوات أعلاه، يُختار الأضيق** (الأصغر فارقاً بين AgeFrom و AgeTo). مثال: مريض عمره 10 سنوات، لدينا مدى (رجال 0-120) ومدى (رجال 0-12) → يُختار مدى (0-12) لأنه أضيق.
  5. إن لم يُوجد مطابقة، يعيد `null` — يُ treated كنتيجة بدون معدل مرجعي.
- **`EvaluateValueAsync`** → يعيد (Normal / Abnormal Low / Abnormal High / Critical Low / Critical High) مع Flag.

### Part 8.4 — FluentValidation: NormalRangeValidator
- Rules: LowLimit ≤ HighLimit، AgeFrom ≤ AgeTo، CriticalLow ≤ LowLimit، CriticalHigh ≥ HighLimit، **Gender ∈ {Male, Female} فقط (قرار 17)**.

### Part 8.5 — Implementation + DI
- `NormalRangeService.cs` + التسجيل.

### Part 8.6 — NormalRangeViewModel
- **الخصائص**: `LabTest ParentTest`, `ObservableCollection<NormalRange> Ranges`, `SelectedRange`, `IsAddMode`, `IsEditMode` + كل حقول Form. **`Gender` dropdown يحتوي Male/Female فقط (قرار 17)**.
- **الأوامر**: `AddRangeCommand`, `EditCommand`, `SaveCommand`, `CancelCommand`, `DeleteCommand`, `BackToTestsCommand`.

### Part 8.7 — NormalRangeView (Window أو Modal)
- **الملفات**: `Views/Windows/NormalRangeView.xaml`.
- **التخطيط**: أعلى اسم التحليل، يمين قائمة المعدلات، وسط نموذج البيانات (16 حقلاً مقسّم إلى قسمين: Normal / Critical)، **حقل Sex = ComboBox بـ Male/Female فقط (قرار 17)**، أسفل أزرار الأوامر + **لوحة الرموز (α β γ μ ± ≤ ≥ ° — قرار 14، نفس UserControl القابل للتوسعة)**.

### Part 8.8 — دمج مع Function 7
- في `LabTestManagementViewModel.OpenNormalRangeCommand` → يفتح `NormalRangeView` كـ Dialog ويمرّر `LabTestId`.

### Part 8.9 — دمج مع Function 4 (إدخال النتائج)
- عند إدخال قيمة نتيجة، `TestResultEntryService` يستدعي `INormalRangeService.GetMatchingRangeAsync` (**بتطبيق قاعدة أضيق مدى يفوز — قرار 16**) ثم `EvaluateValueAsync` للتلوين والتمييز.

### Part 8.10 — Build Verification
- فتح Function 7 → اختيار تحليل → "المعدل الطبيعي" → إضافة 6 معدلات (رجال/نساء × ثلاث فئات عمرية — **بدون Both، قرار 17**) → حفظ → العودة لـ Function 7 → التحقق في DB.
- اختبار قاعدة الأولوية (قرار 16): إضافة مدى (رجال 0-120) ومدى (رجال 0-12) → إدخال مريض عمره 10 سنوات → التحقق من اختيار المدى الأضيق (0-12).

## 4) Open Questions
- لا توجد أسئلة مفتوحة بعد تطبيق القرارات 14 و 16 و 17.

---

# 🔹 Function 2 (Execution Order: Step 4 of 8) — طباعة باركود للمريض

## 1) Current State vs Target

**الموجود حالياً**: لا شيء — لا يوجد أي منطق باركود، ولا نافذة، ولا خدمة، ولا مرجع في `MainDashboardViewModel`. الحزمة `QuestPDF` مثبّتة لكن غير مستخدمة.

**المطلوب**: نافذة "طباعة الباركود" تُفتح من داخل نافذة "بيانات المرضى" (زر باركود / F11) وتعرض ملصقات الأنابيب مصنّفة حسب نوع العينة، مع أزرار طباعة كل ملصق منفرد، طباعة كود الملف، طباعة كود المعمل، ومنزلق ضبط الإحداثيات (X/Y) للطباعة على شرائح الباركود. **مكتبة ZXing.Net حصراً (قرار 3)**. **حجم ملصق افتراضي 38×25 مم قابل للتخصيص (قرار 4)**. **لا رقم فرع — الموضع مُثبَّت برمجياً بـ "1" (قرار 5)**.

## 2) Reference Behavior Summary

مصدر المعلومات: `Docs/Reference system/MinerU_markdown_...md` — قسم "كيف يتم طباعة الاركود للمريض؟" (سطور 307–354)؛ تم التحقيد ضد `Real_Lab_System_Reference.pdf` (الصفحات 15–19).

- الوصول: زر "الباركود" من نافذة "بيانات المرضى" — يعمل حتى لمريض قديم معروض في النافذة.
- 3 أنواع أكواد لكل مريض:
  1. **كود حالة/اليوم** — تسلسلي لترتيب الدخول، يبدأ برقم 1 من اليسار.
  2. **كود الملف** — يبدأ برقم 3 من اليسار، يُطبع عند تسليم النتائج بالباركود ويُلصق على الملف مع اسم/نوع/سن/تاريخ.
  3. **كود المعمل** — دائم للمريض، يبدأ برقم 5 من اليسار، لسهولة الوصول لبياناته.
- **صيغة الكود** 13 خانة: `1-4-100623-1-006-8` = (لا يُنظر إليه) - (نوع الكود) - (تاريخ اليوم من اليمين YYMMDD) - (**رقم الفرع — مُثبَّت برمجياً بـ "1" (قرار 5)**) - (تسلسل يوم بـ 3 خانات) - (رقم يوم الأسبوع 1-7).
  - **بقرار 5**: الموضع الرابع (رقم الفرع) يُكتب دائماً كالثابت `"1"` في منطق توليد الكود داخل `BarcodeService` — لا إعداد قابل للتخصيص، لا شاشة اختيار فرع.
- **بنود النافذة**:
  1. زر طباعة كود الملف (يُطبع مع الكل عند اختيار "طباعة كود الملف مع الكل").
  2. زر طباعة كود المعمل (نشط فقط إن كان للمريض LabId).
  3. ملصقات الأنابيب/الحاويات — تختلف من مريض لآخر حسب التحاليل، يمكن طباعة كل ملصق منفرد.
  4. باركود إضافي (مثل حالات التبرع بالدم) — كتابة اسم التحليل ثم Enter ثم وصف العينة أسفله.
  5. منزلقان أفقي/رأسي لضبط مكان الطباعة على الشريحة، ثم زر "حفظ".
  6. سحب وإفلات لحذف تحليل من ملصق، أو نقل تحليل من ملصق إلى آخر.
- الاختصار F11 من نافذة المرضى.

## 3) Full Vertical-Slice Implementation Plan

### Part 2.1 — كيان BarcodeSettings + BarcodeLabel (بدون BranchNumber — قرار 5؛ مع أبعاد الملصق — قرار 4)
- **الملفات**: `Models/Domain/BarcodeSettings.cs` — الخصائص: `Id, OffsetX, OffsetY, PrintFileCodeWithAll`، **`LabelWidth` (int, بالملليمتر، افتراضي = 38 — قرار 4)**، **`LabelHeight` (int, بالملليمتر، افتراضي = 25 — قرار 4)**.
- **محذوف (قرار 5)**: ~~`BranchNumber`~~ — لا يوجد حقل BranchNumber في الكيان نهائياً.
- **`Models/Domain/BarcodeLabel.cs`** (نموذج غير مخزَّن: SpecimenType, PatientId, PatientName, Tests[], Code, Type).

### Part 2.2 — تحديث Patient + إضافة PatientCode
- **الملفات المعدَّلة**: `Models/Domain/Patient.cs` — تأكيد وجود `LabId`, `FileCode`, `VisitCode`.
- **الملفات الجديدة**: (اختياري) `Models/Domain/PatientCode.cs` (Id, PatientId, CodeType[Case/File/Lab], CodeValue, IssuedAt).

### Part 2.3 — Migration
- إضافة `BarcodeSettings` table + جدول `PatientCodes` (إن اعتمدنا Part 2.2). **لا عمود BranchNumber (قرار 5)**.

### Part 2.4 — Service: IBarcodeService (بدون BranchNumber — قرار 5)
- **الملفات**: `Services/Interfaces/IBarcodeService.cs`: `GenerateCaseCode(patient, visit)`, `GenerateFileCode(patient)`, `GenerateLabCode(patient)`, `GetOrCreateLabIdAsync(patient)`, `GetLabelsForPatientAsync(patientId)`, `GetSettingsAsync`, `SaveSettingsAsync`.
- **منطق توليد الكود (قرار 5)**: في بناء كود 13 خانة، موضع رقم الفرع يُكتب كالثابت البرمجي `const string BranchConstant = "1"` — لا يُقرأ من أي إعداد أو قاعدة بيانات. مثال: `1-4-100623-1-006-8` (الـ "1" الرابع ثابت).
- **منطق تجميع التحاليل**: حسب `SpecimenType`، بناء كود 13 خانة.

### Part 2.5 — Implementation
- **الملفات**: `Services/Implementations/BarcodeService.cs` — يعتمد على `NewLabDbContext` و `IPatientService`.

### Part 2.6 — تسجيل DI
- **الملفات المعدَّلة**: `App.xaml.cs` — إضافة `AddScoped<IBarcodeService, BarcodeService>()`.

### Part 2.7 — مولّد صور الباركود (ZXing.Net — قرار 3)
- **الملفات**: `Helpers/BarcodeImageGenerator.cs` باستخدام **`ZXing.Net` حصراً (قرار 3)** — يُضاف كـ NuGet Package `ZXing.Net`.
- **الناتج**: `BitmapSource GenerateCode128(string data)` — يولّد صورة باركود Code-128 من النص.

### Part 2.8 — PDF Layout عبر QuestPDF (مع أبعاد الملصق — قرار 4)
- **الملفات**: `Services/Implementations/BarcodePrintService.cs` — يبني PDF **بحجم شريحة ملصقات من `BarcodeSettings.LabelWidth × LabelHeight` (افتراضي 38×25 مم — قرار 4)**، يطبق OffsetX/Y.

### Part 2.9 — BarcodeViewModel
- **الخصائص**: `Patient`, `ObservableCollection<BarcodeLabel> Labels`, `OffsetX`, `OffsetY`, `PrintFileCodeWithAll`، **`LabelWidth`, `LabelHeight` (قرار 4)**، `ExtraBarcodeName`, `ExtraBarcodeDescription`.

### Part 2.10 — BarcodeView (Window)
- **الملفات**: `Views/Windows/BarcodeView.xaml` + `.xaml.cs`.
- **التخطيط**: قسم علوي أزرار الطباعة، وسط ItemsControl للملصقات (Drag & Drop)، أسفل منزلقان + زر حفظ + خانتان للباركود الإضافي.

### Part 2.11 — تسجيل الـ VM في DI
- **الملفات المعدَّلة**: `App.xaml.cs`.

### Part 2.12 — استدعاء النافذة من PatientEntryView
- **الملفات المعدَّلة**: `PatientEntryViewModel.PrintBarcodeCommand` تفتح `BarcodeView` كـ Modal.
- **InputBinding**: `F11` في `PatientEntryView`.

### Part 2.13 — Build Verification
- بناء + اختبار: فتح نافذة مريض → F11 → نافذة الباركود → التحقق من توليد كود بدون BranchNumber (الفرع = "1" ثابت) → التحقق من أبعاد الملصق 38×25 مم → طباعة تجريبية (PDF Preview).

## 4) Open Questions
- لا توجد أسئلة مفتوحة بعد تطبيق القرارات 3 و 4 و 5.

---

# 🔹 Function 3 (Execution Order: Step 5 of 8) — الوصول لنوافذ إدخال نتائج التحاليل

## 1) Current State vs Target

**الموجود حالياً**: زر "إدخال نتائج التحاليل" موجود ضمن فئة "المرضى" لكنه فارغ (بدون `TargetViewType`). لا اختصار F4. لا `TestResultsListView` ولا `TestResultsListViewModel`.

**المطلوب**: نافذة قائمة نتائج تحاليل مرضى اليوم، مع رموز حالة (7 حالات)، اختيار مريض لعرض تحاليله، أزرار طباعة، فلاتر، **بحث بكود/لاب أي دي/رقم الحضور (مع شرح واضح للصيغة — قرار 6)**، اختصار F4. **زرّا "ب" و"ت" يتطلبان دور Admin فقط (قرار 7)**.

## 2) Reference Behavior Summary

مصدر المعلومات: `Docs/Reference system/MinerU_markdown_...md` — قسم "كيف يتم الوصول لنوافذ ادخال نتائج التحاليل؟" (سطور 358–449)؛ تم التحقيد ضد `Real_Lab_System_Reference.pdf` (الصفحات 21–25).

- الوصول: Toolbar "المرضى" → "إدخال نتائج التحاليل"، أو F4 من أي نافذة رئيسية.
- **7 مناطق** في النافذة:
  1. قائمة مرضى اليوم فقط + رمز حالة بجوار كل اسم (7 حالات: دائرة حمراء = جديد غير مسجّل؛ ورقة = كُتبت النتائج؛ أسهم = تمت مراجعة/تراجع؛ طابعة = طُبعت؛ عربة = تم تسليم؛ دولار = باقي حساب؛ نجمة = مكتمل).
  2. بيانات المريض المحدد + خانة صغيرة خضراء بعدد الزيارات (لعملاء LabId) + اسم أحمر للمهمّين.
  3. قائمة تحاليل المريض + علامة أمام كل تحليل (كُتب/راجَع/طُبع/سُلِّم). نقر مزدوج يفتح نافذة إدخال النتائج (Function 4).
  4. أزرار F8/F9/F12 لتغيير الحالات يدوياً، وزر "ملاحظات".
  5. أزرار: (بيانات المرضى/تقرير مجمع/ورقة عمل/ظرف/تاريخ مرضي/**تاريخ مخصص — خارج النطاق (قرار 9)**/تقرير فارغ/العودة).
  6. **زر "بـ" (سجل التدقيق) — Admin فقط (قرار 7)**: لعرض من سجّل بيانات المريض ومن عدّلها + من أدخل النتائج ومن راجعها ومن طبعها ومن سلّمها بالتاريخ والوقت.
  7. فلاتر: غير المكتوبة/غير المراجعة/غير المطبوعة/المهمين/المستقلين/معامل/جهات + انتقال ليوم آخر.
  8. **مربع بحث بكود المريض / كود المعمل / كود الملف / رقم حضور اليوم — توضيح الصيغة (قرار 6)**:

**تفسير صيغة "رقم الحضور" (قرار 6 — مُستخرج من `Real_Lab_System_Reference.pdf` الصفحة 24)**:

في النظام المرجعي، "رقم الحضور" هو **التسلسل اليومي لترتيب دخول المريض للمعمل في ذلك اليوم**. يبدأ من 1 ويزداد بمقدار 1 لكل مريض جديد يُسجّل في نفس اليوم.

عند البحث في نافذة نتائج التحاليل، يمكن إدخال رقم الحضور مباشرة للوصول السريع لمريض معين ضمن مرضى اليوم. مثال عملي:
- المريض الأول اليوم → رقم حضوره = 1
- المريض الثاني → رقم حضوره = 2
- المريض العاشر → رقم حضوره = 10

**ما حدث في الوثيقة الأصلية v1**: الصيغة "(5) 12" التي وردت كانت **خطأ استخراج OCR**. النص الأصلي في `Real_Lab_System_Reference.pdf` (الصفحة 24، النص المعكوس RTL) يقرأ: *"ورقم الحضور فى هذا اليوم مثلا (5 أو 21) هكذا"* — أي أن النظام يعطي مثالين: رقم حضور 5 أو رقم حضور 21، والأقواس حول الأرقام للتوضيح فقط. أداة استخراج MinerU قرأت "أو" كأقواس مزدوجة وقلبت "21" إلى "12".

**كيف يُدخله المستخدم فعلياً**: يكتب المستخدم الرقم فقط (مثلاً 5 أو 21) في مربع البحث، فينتقل النظام مباشرة للمريض رقم 5 (أو 21) في قائمة مرضى اليوم. لا يوجد أي ترميز مركّب — إنها مجرد تسلسل رقمي يومي بسيط.

- **الاختصارات**: F2 (بيانات مريض)، F3 (بحث)، F4 (هنا)، F5 (Refresh)، F6 (تسليم)، F7 (عينات خارج)، F8 (مراجعة)، F9 (منتهية)، F12 (طبعت)، Enter (فتح إدخال النتيجة)، Up/Down/Ctrl، Esc.

## 3) Full Vertical-Slice Implementation Plan

### Part 3.1 — Enum PatientTestStatus + PatientTest كيان
- **الملفات**: `Enums/TestStatus.cs` (New, Entered, Reviewed, Printed, Delivered, AccountIssue, Completed).
- `Models/Domain/PatientTest.cs` (Id, PatientVisitId, LabTestId, Status, IsReviewed, IsPrinted, IsDelivered, EnteredByUserId, EnteredAt, ReviewedByUserId, PrintedByUserId, DeliveredByUserId, Notes, Price).

### Part 3.2 — كيان AuditLog للتتبع
- **الملفات**: `Models/Domain/AuditLog.cs` (Id, EntityName, EntityId, Action, UserId, Timestamp, Details).

### Part 3.3 — Migration
- إضافة `PatientTests` + `AuditLogs`.

### Part 3.4 — Service: ITestResultsListService
- **الأساليب**: `GetTodayPatientsAsync`, `GetPatientsByFilterAsync(filter)`, `GetPatientTestsAsync(patientId)`, `SearchByCodeAsync(code)`, `SearchByAttendanceNumberAsync(number)` (**رقم تسلسلي يومي بسيط — قرار 6**)، `ToggleReviewedAsync`, `ToggleEnteredAsync`, `TogglePrintedAsync`, `UpdatePatientNoteAsync`, `GetAuditForPatientAsync`, `GetAuditForTestAsync`.

### Part 3.5 — Implementation + DI Registration
- `Services/Implementations/TestResultsListService.cs` + إضافة في `App.xaml.cs`.

### Part 3.6 — StatusIconConverter
- **الملفات**: `Converters/TestStatusToIconConverter.cs`.

### Part 3.7 — TestResultsListViewModel (مع صلاحيات Admin لـ "ب" و"ت" — قرار 7)
- **الخصائص**: `ObservableCollection<PatientListItem> Patients`, `SelectedPatient`, `ObservableCollection<PatientTest> PatientTests`, `SelectedTest`, `SearchCode`, `FilterMode`, `SelectedDate`، **`IsAdmin` (لتفعيل/تعطيل زرّي "ب" و"ت" — قرار 7)**.
- **الأوامر**: `RefreshCommand`, `OpenPatientDataCommand` (F2)، `OpenSearchCommand` (F3)، `OpenTestEntryCommand` (Enter → Function 4)، `OpenDeliveryCommand` (F6)، `PrintAggregateReportCommand`, `PrintWorksheetCommand`, `PrintEnvelopeCommand`, `PrintHistoryCommand`, `PrintBlankReportCommand`, `ToggleReviewedCommand` (F8)، `ToggleEnteredCommand` (F9)، `TogglePrintedCommand` (F12)، `UpdateNoteCommand`، **`ShowAuditCommand` (CanExecute = IsAdmin — قرار 7)**، **`ShowFinancialTrackingCommand` (CanExecute = IsAdmin — قرار 7)**، `SearchCommand`, `PreviousDayCommand`, `NextDayCommand`.

### Part 3.8 — TestResultsListView
- **الملفات**: `Views/Pages/TestResultsListView.xaml`.
- **التخطيط**: Grid 3 أعمدة. FlowDirection=RightToLeft. InputBindings لكل الاختصارات.
- **زرّا "ب" و"ت"**: مرتبطان بأوامر CanExecute = IsAdmin — يظهران معطَّلين لغير Admin (قرار 7).
- **زر "تاريخ مخصص"**: **غير موجود نهائياً في الواجهة (قرار 9 — خارج النطاق)**.

### Part 3.9 — ربط الوظيفة بالـ Toolbar واختصار F4 عالمي
- تعديل `MainDashboardViewModel` — `TargetViewType = typeof(TestResultsListView)`.
- تعديل `MainWindow.xaml` — إضافة `KeyBinding` لـ F4.

### Part 3.10 — Build Verification
- بناء + اختبار: فتح النافذة → عرض مرضى اليوم → تحديد مريض → عرض تحاليله → F8/F9/F12 يبدّل الرموز → البحث برقم الحضور (مثلاً 5) ينتقل للمريض رقم 5 → زر "ب" يعمل مع Admin فقط.

## 4) Open Questions
- لا توجد أسئلة مفتوحة بعد تطبيق القرارات 6 و 7 و 9.

---

# 🔹 Function 4 (Execution Order: Step 6 of 8) — إدخال نتائج التحاليل

## 1) Current State vs Target

**الموجود حالياً**: لا شيء — لا `TestResult` model، ولا نافذة إدخال، ولا خدمة، ولا حساب تلقائي.

**المطلوب**: نافذة تفتح بالنقر المزدوج على تحليل من قائمة تحاليل المريض في Function 3، تعرض بيانات المريض، عناصر البروفيل مع تفعيل ما اختير، حقول إدخال قيم النتائج مع تلوين تلقائي، حقل تعليقات، أزرار (حفظ/طباعة/معاينة/**تاريخ مرضي أساسي فقط — قرار 9**/رجوع/القائمة الرئيسية)، اختتصارات F8/F9/F11/F12/Enter/Up/Down/Esc. **معادلات PT/PTT ISI مُستخرجة كاملة من المصدر الأصلي (قرار 8)**.

## 2) Reference Behavior Summary

مصدر المعلومات: `Docs/Reference system/MinerU_markdown_...md` — قسم "كيف يتم ادخال نتائج التحاليل" (سطور 453–511)؛ تم التحقيد ضد `Real_Lab_System_Reference.pdf` (الصفحات 26–28).

- الوصول: من نافذة نتائج التحاليل (Function 3) بالنقر المزدوج على تحليل. إذا كان بروفيل → تظهر كل عناصر البروفيل مع تفعيل التي اختيرت.
- **5 مناطق**:
  1. بيانات المريض.
  2. عناصر البروفيل — التحاليل المختارة مفعّلة، النظام يميّز النتائج غير الطبيعية والحرجة تلقائياً بتلوين وتسمية (عالي/منخفض/حرجة).
  3. علامات صح على المطبوع، ظل رمادي/ملوّن للمراجع.
  4. حقل التعليق: إدخال يدوي أو زر برتقالي يعرض قائمة تعليقات محفوظة، أو زر يجلب التعليق من المعدل الطبيعي، زر تراجع لآخر تعليق.
  5. أزرار: حفظ / طباعة / معاينة طباعة / **تاريخ مرضي (Patient history — الزر الأساسي فقط، اسمه يتغير حسب عدد نتائج المريض المخزنة — قرار 9: لا نافذة منفصلة لـ "تاريخ مخصص")** / رجوع (بدون حفظ) / القائمة الرئيسية.
- **الاختصارات**: F8 (اعتبار مراجعة)، F9 (حفظ)، F11 (معاينة طباعة)، F12 (طباعة التقرير)، Enter (خانة تالية / حفظ في آخر خانة + عودة)، Up/Down، Esc (رجوع بدون حفظ).
- **حسابات تلقائية (مستخرجة من `Real_Lab_System_Reference.pdf` الصفحة 62 — قسم "ملاحظات")**:

  **CBC (Complete Blood Count):**
  - `Hct (Hematocrit) = Hgb × 3.3` — في حالة عدم إدخال نتيجة الهيماتوكريت تُحسب من الهيموجلوبين.
  - `Hemoglobin % (Hgb%) = Hgb × 8.25` — (Male or Female, age less than one year)
  - `Hemoglobin % (Hgb%) = Hgb × 7.50` — (Male or Female, age from 1–12 years)
  - `Hemoglobin % (Hgb%) = Hgb × 6.25` — (Male, age greater than 12 years)
  - `Hemoglobin % (Hgb%) = Hgb × 6.75` — (Female, age greater than 12 years)
  - تعديل هذه الثوابت أثناء إدخال النتائج عبر زر "Constants" في جانب النافذة (كما في النظام المرجعي).

  **PT (Prothrombin Time) — (قرار 8 — مستخرج كامل من `Real_Lab_System_Reference.pdf` صفحة 62):**
  - يتطلب إدخال ثلاثة ثوابت قبل أول استخدام كإعدادات نظام:
    1. **ISI** (International Sensitivity Index) — معامل حساسية الـ reagent المستخدم (قيمة عشرية، عادةً بين 0.8 و 2.0).
    2. **Control Time** (PT الطبيعي/الضابط) — زمن البرومبوبلين للعينة الضابطة بالثواني.
    3. **Concentration table** — جدول تركيزات (إن لزم).
  - **المعادلة الضمنية**: `INR = (PT_المريض ÷ PT_الضابط)^ISI`
    - حيث `PT_المريض` = زمن البرومبوبلين للمريض بالثواني.
    - `PT_الضابط` = Control Time المُدخل.
    - `ISI` = المعامل المُدخل.
  - يتم تخزين ISI و Control Time كقيم قابلة للتعديل عبر زر "Constants" (نفس آلية Hgb% الثوابت).

  **PTT (Partial Thromboplastin Time) — (قرار 8):**
  - يتطلب إدخال ثابت واحد قبل أول استخدام:
    1. **Control Time** (PTT الطبيعي/الضابط) — زمن الـ PTT للعينة الضابطة بالثواني.
  - **المعادلة الضمنية**: `PTT Ratio = PTT_المريض ÷ PTT_الضابط`
    - حيث `PTT_المريض` = زمن الـ PTT للمريض بالثواني.
    - `PTT_الضابط` = Control Time المُدخل.
  - يتم تخزين Control Time كقيمة قابلة للتعديل عبر زر "Constants".

  **ملاحظة مهمة (قرار 8)**: النظام المرجعي لا يُعطي معادلات حسابية صريحة بصيغة "X = Y × Z" لـ PT و PTT كما يفعل مع Hct و Hgb%، بل يذكر **الثوابت المطلوبة** (ISI, Control Time) التي يجب إدخالها قبل أول استخدام. المعادلات الضمنية (INR و PTT Ratio) مُستنتجة من الممارسة الطبية المعيارية وتُستخدم لتوليد القيمة المحسوبة تلقائياً عند إدخال زمن المريض. النظام يخزّن هذه الثوابت ويستخدمها لحساب INR/Ratio وعرضها في التقرير.

## 3) Full Vertical-Slice Implementation Plan

### Part 4.1 — كيان TestResult + SavedComment
- **الملفات**: `Models/Domain/TestResult.cs` (Id, PatientTestId, LabTestElementId, Value, Unit, IsAbnormal, IsCritical, FlagText, Comment, EnteredAt, ReviewedAt).
- `Models/Domain/SavedComment.cs` (Id, LabTestId, CommentText, Type[Low/High/Critical/General]).

### Part 4.2 — كيان LabTestElement (لعناصر البروفيل)
- **الملفات**: `Models/Domain/LabTestElement.cs` (Id, ParentLabTestId, Name, ArrangeNumber, IsMainTest) — يُبنى في Function 7 لكن يُستخدم هنا.

### Part 4.3 — Migration
- إضافة `TestResults`, `SavedComments` + FK relationships.

### Part 4.4 — Service: ITestResultEntryService
- **الأساليب**: `GetPatientTestWithProfileAsync(patientTestId)`, `SaveResultsAsync(patientTestId, results, comments)`, `EvaluateResultAsync(value, normalRange)` → يعيد `IsAbnormal/IsCritical/Flag` (**باستخدام قاعدة أضيق مدى يفوز من Function 8 — قرار 16**)، `GetSavedCommentsAsync(labTestId)`, `MarkReviewedAsync`, `PreviewReportAsync`, `PrintReportAsync`.

### Part 4.5 — Service: IAutoCalculationService (معادلات PT/PTT ISI كاملة — قرار 8)
- **الأساليب**:
  - `CalculateHct(hgb)` → `hgb * 3.3`
  - `CalculateHgbPercent(hgb, ageYears, gender)` بالمعاملات 8.25 / 7.50 / 6.25 / 6.75 حسب العمر/الجنس.
  - **`CalculateINR(ptPatient, controlTime, isi)` → `Math.Pow(ptPatient / controlTime, isi)` (قرار 8)**.
  - **`CalculatePTTRatio(pttPatient, controlTime)` → `pttPatient / controlTime` (قرار 8)**.
- **كيان ثوابت**: `Models/Domain/CalculationConstants.cs` (Id, TestType [Hgb/CBC/PT/PTT], ConstantName, ConstantValue, UpdatedAt) — لتخزين ISI, PT Control Time, PTT Control Time, Hgb% multipliers. تُحرَّر عبر زر "Constants".
- **المصدر**: `Real_Lab_System_Reference.pdf` صفحة 62 + الممارسة الطبية المعيارية لمعادلات INR و PTT Ratio.

### Part 4.6 — Implementation + DI
- `TestResultEntryService.cs`, `AutoCalculationService.cs` + تسجيل.

### Part 4.7 — QuestPDF ReportGenerator
- **الملفات**: `Services/Implementations/ReportPdfGenerator.cs` — يبني PDF التقرير مع رأس/ذيل/جدول نتائج/تعليقات/معدلات طبيعية. **يشمل عرض INR و PTT Ratio عند توفرها (قرار 8)**.

### Part 4.8 — TestResultEntryViewModel (بدون "تاريخ مخصص" — قرار 9)
- **الخصائص**: `Patient`, `LabTest ParentTest`, `ObservableCollection<TestResultRow> ResultRows`, `Comment`, `IsReviewed`, `HistoryButtonLabel` (يتغير حسب العدد)، **`ObservableCollection<CalculationConstant> Constants` (قرار 8)**.
- **الأوامر**: `SaveCommand` (F9)، `PrintCommand` (F12)، `PreviewCommand` (F11)، `ToggleReviewedCommand` (F8)، `OpenHistoryCommand` (**الزر الأساسي فقط — يفتح نافذة تاريخ المريض العامة بدون نافذة مخصصة منفصلة، قرار 9**)، `BackCommand` (Esc)، `MainMenuCommand`، `PickSavedCommentCommand`، `PickCommentFromNormalRangeCommand`، `UndoLastCommentCommand`، **`EditConstantsCommand` (يفتح نافذحة تعديل ثوابت ISI/Control Time — قرار 8)**.

### Part 4.9 — TestResultEntryView (Window أو Modal Overlay)
- **الملفات**: `Views/Windows/TestResultEntryView.xaml`.
- **التخطيط**: أعلى بيانات المريض، وسط جدول عناصر البروفيل، تلوين خلفية للـ Abnormal/Critical، أسفل التعليق + أزرار الأوامر + **زر "Constants" في الجانب (قرار 8)**.

### Part 4.10 — دمج مع Function 3
- تعديل `TestResultsListViewModel.OpenTestEntryCommand` → يفتح `TestResultEntryView` كـ Dialog.

### Part 4.11 — Build Verification
- فتح تحليل → إدخال قيمة Hgb → التحقق من حساب Hct تلقائياً → إدخال قيمة PT → التحقق من حساب INR باستخدام ISI و Control Time (قرار 8) → التلوين يظهر → حفظ → العودة لقائمة النتائج.

## 4) Open Questions
- لا توجد أسئلة مفتوحة بعد تطبيق القرارات 8 و 9 و 16.

---

# 🔹 Function 5 (Execution Order: Step 7 of 8) — تسليم نتائج التحاليل للمريض

## 1) Current State vs Target

**الموجود حالياً**: زر "تسليم نتائج المرضى" في الـ Dashboard لكنه فارغ. لا `DeliveryService`، لا نافذة تسليم، لا اختصار F6.

**المطلوب**: نافذة تعرض مرضى اليوم غير المستلمين، رموز حالة، بيانات المريض، تحاليله + المبلغ، ملخّص، أزرار (تسليم يدوي/تسديد حساب/تصفية/العودة)، فلاتر، **بحث بكود مع كشف تلقائي للنوع (قرار 10)**، اختصار F6. **زر "مستلمة" يتطلب Admin (قرار 11)**.

## 2) Reference Behavior Summary

مصدر المعلومات: `Docs/Reference system/MinerU_markdown_...md` — قسم "كيف يتم تسليم نتائج التحاليل للمريض؟" (سطور 515–602)؛ تم التحقيد ضد `Real_Lab_System_Reference.pdf` (الصفحات 29–32).

- الوصول: Toolbar "المرضى" → "تسليم نتائج المرضى"، أو F6.
- **سيناريو Barcode (قرار 10 — تصميم مرن)**: يقبل النظام أي كود ممسوح ويكتشف نوعه تلقائياً من بنيته/بادئته (كود إيصال/حالة يبدأ بـ 1، كود ملف يبدأ بـ 3، كود معمل يبدأ بـ 5) — **دون فرض تسلسل ثنائي صارم**. عند مسح الكود، يبحث النظام عن المريض المرتبط به ويعرضه. يمكن بعدها تأكيد التسليم مباشرة.
- **بدون Barcode**: اختيار المريض من القائمة.
- **7 مناطق**:
  1. قائمة مرضى اليوم غير المستلمين + رموز الحالة السبعة.
  2. بيانات المريض المحدد (اسم أحمر للمهم).
  3. قائمة تحاليل المريض + علامات + مبلغ لكل تحليل.
  4. ملخّص: عدد غير المنتهي/غير المطبوع + باقي حساب — **زر "مستلمة" لإلغاء تسليم بالخطأ — Admin فقط (قرار 11)**.
  5. أزرار: تسليم يدوي / تسديد حساب / تصفية / العودة.
  6. فلاتر: غير المسلمة / معمل لمعمل / مستقل / مهم / فترة زمنية + خيار "الكل".
  7. مربع بحث بكود المريض/المعمل/الملف.
- الاختصارات نفس Function 3 (F2/F3/F4/F5/F6/F7/F8/F9/F12/Enter/Up/Down/Ctrl/Esc).

## 3) Full Vertical-Slice Implementation Plan

### Part 5.1 — كيانات مالية
- **الملفات**: `Models/Domain/PaymentTransaction.cs` (Id, PatientId, Amount, Type[Payment/Refund/Delivery], UserId, Timestamp, Note).

### Part 5.2 — تحديث Patient
- التأكد من وجود `DeliveredAt`, `TotalAmount`, `PaidAmount`, `Discount`.

### Part 5.3 — Migration
- إضافة `PaymentTransactions`.

### Part 5.4 — Service: IDeliveryService (مع كشف تلقائي للكود — قرار 10)
- **الأساليب**: `GetUndeliveredTodayAsync`, `GetPatientDeliveryStateAsync(patientId)`, `DeliverAllResultsAsync(patientId, userId)`, `UnmarkDeliveredAsync(patientId, userId)` (**يُتحقق من Admin داخل التنفيذ — قرار 11**)، `SettleAccountAsync(patientId, amount, userId)`, `SearchByCodeAsync(code)`, `FilterAsync(filter)`.
- **`SearchByCodeAsync` (قرار 10)**: يقبل أي نص كود ممسوح ويكتشف نوعه تلقائياً:
  - إن بدأ بـ "1" → كود حالة/يوم → بحث في `PatientVisit.FullVisitCode`.
  - إن بدأ بـ "3" → كود ملف → بحث في `Patient.FileCode`.
  - إن بدأ بـ "5" → كود معمل → بحث في `Patient.LabId`.
  - لا يُفرض تسلسل ثنائي — مجرد مسح الكود يكفي للوصول للمريض.

### Part 5.5 — Implementation + DI
- `DeliveryService.cs` + التسجيل. **`UnmarkDeliveredAsync` يتحقق من دور Admin — يرمي `UnauthorizedAccessException` لغير Admin (قرار 11)**.

### Part 5.6 — DeliveryViewModel (مع IsAdmin لزر "مستلمة" — قرار 11)
- **الخصائص**: `Patients`, `SelectedPatient`, `PatientTests`, `UndeliveredCount`, `UnprintedCount`, `RemainingBalance`, `FilterMode`, `SelectedDateRange`, `SearchCode`، **`IsAdmin` (قرار 11)**.
- **الأوامر**: `DeliverManuallyCommand`, `SettleAccountCommand`, **`UnmarkDeliveredCommand` (CanExecute = IsAdmin — قرار 11)**، `SearchCommand`, `ScanBarcodeCommand` (**كشف تلقائي للنوع — قرار 10**)، `BackToMainCommand` + أوامر التنقل.

### Part 5.7 — DeliveryView
- **الملفات**: `Views/Pages/DeliveryView.xaml`.
- **التخطيط**: مشابه لـ Function 3 مع عمود المبلغ + منطقة ملخّص مالي + أزرار التسليم/التسديد. **زر "مستلمة" معطَّل لغير Admin (قرار 11)**.

### Part 5.8 — دعم Barcode Scanner (كشف تلقائي — قرار 10)
- **الملفات**: `Helpers/BarcodeScannerListener.cs` — يستمع لأحداث الكيبورد السريعة لقراءة الكود من قارئ الباركود.
- **التكامل**: `DeliveryView` تحوي `TextBox` مخفياً يستقبل قراءة الباركود ويستدعي `ScanBarcodeCommand` — **الـ Service يكتشف نوع الكود تلقائياً (قرار 10)**.

### Part 5.9 — ربط الـ Toolbar + F6 عالمياً
- تعديل `MainDashboardViewModel` + إضافة `KeyBinding` F6 في `MainWindow.xaml`.

### Part 5.10 — Build Verification
- فتح النافذة → عرض غير المستلمين → مسح كود (يبدأ بـ 1/3/5) → كشف تلقائي للمريض (قرار 10) → التسليم يعمل → تسجيل `PaymentTransactions` في DB → محاولة "مستلمة" كـ Technician = معطّل، كـ Admin = يعمل (قرار 11).

## 4) Open Questions
- لا توجد أسئلة مفتوحة بعد تطبيق القرارات 10 و 11.

---

# 🔹 Function 6 (Execution Order: Step 8 of 8) — البحث عن مريض

## 1) Current State vs Target

**الموجود حالياً**: زر "بحث عن مريض" في الـ Dashboard فارغ. لا `SearchService` ولا `SearchView` ولا اختصار F3.

**المطلوب**: نافذة بحث بفلاتر، بحث بجزء من الاسم، بحث بهاتف/رقم قومي/كود، عرض حتى 100 نتيجة، عرض تحاليل المريض، ملخّص، أزرار (نتائج تحاليل/بيانات مريض/حذف — **Admin فقط، قرار 13**/نتائج مجموعة)، **خيار البحث في قاعدة النسخ الاحتياطي = Stub معطَّل (قرار 12)**، اختصار F3.

## 2) Reference Behavior Summary

مصدر المعلومات: `Docs/Reference system/MinerU_markdown_...md` — قسم "كيف يتم عملية البحث عن مريض" (سطور 607–677)؛ تم التحقيد ضد `Real_Lab_System_Reference.pdf` (الصفحات 33–36).

- الوصول: Toolbar "المرضى" → "بحث عن مريض"، أو F3.
- **4 مناطق**:
  1. **فلاتر بحث**: فترة زمنية، مرحلة عمرية، جنس (**Male/Female فقط**)، جهة إحالة. البحث بجزء من الاسم (يعرض أول 100 مريض)، الأول-حرفين + أي مكان في الاسم مثال: (اح) + (خال) → أحمد خالد.
  2. **حقول بحث**: هاتف/جوال/رقم قومي/كود حالة/كود معمل/كود ملف — كل واحد في مكان مخصص.
  3. نتيجة البحث (100 حد أقصى).
  4. قائمة تحاليل المريض المحدد.
  5. ملخّص: عدد التحاليل غير المنتهية/غير المطبوعة/غير المستلمة + المرضى ذوي الحسابات المفتوحة. أزرار: نتائج تحاليل / بيانات مريض / **حذف — Admin فقط (قرار 13)** / نتائج مجموعة.
- **خيار قاعدة النسخ الاحتياطي (قرار 12)**: قائمة مصفّرة لاختيار البحث في القاعدة الأساسية أو الاحتياطية — **Stub ظاهر ومعطَّل (`IsEnabled = false`) في هذه المرحلة، بلا آلية Backup فعلية**.
- الاختصارات: F2/F4/F5 (Refresh)/F6/F7/F10 (حذف — **Admin فقط، قرار 13**)/Esc.

## 3) Full Vertical-Slice Implementation Plan

### Part 6.1 — Service: IPatientSearchService (مع Backup = Stub — قرار 12)
- **الأساليب**: `SearchAsync(criteria)` حيث `SearchCriteria` تحتوي على: `NamePrefix, NameContains, PhoneNumber, NationalId, VisitCode, LabCode, FileCode, DateFrom, DateTo, AgeFrom, AgeTo, AgeUnit, Gender` (**Male/Female فقط**)، `ReferralId, Source[Main|Backup]`. حد النتائج 100.
- **`Source = Backup` (قرار 12)**: يرمي `NotImplementedException` أو يعيد نتائج فارغة مع رسالة "هذه الميزة غير مفعّلة في هذه المرحلة" — Stub فقط.
- **أسلوب**: `GetPatientTestsSummaryAsync(patientId)`.
- **أسلوب**: `GetPatientsGroupResultsAsync(patientIds, labTestId)`.

### Part 6.2 — Implementation + DI
- `PatientSearchService.cs` + التسجيل. **قاعدة البيانات الاحتياطية غير مُنفَّذة — Source = Backup = Stub معطَّل (قرار 12)**.

### Part 6.3 — SearchViewModel (مع IsAdmin للحذف — قرار 13؛ Backup Stub — قرار 12)
- **الخصائص**: كل حقول `SearchCriteria` + `ObservableCollection<PatientSearchRow> Results` + `SelectedResult` + `SelectedResultTests` + `Summary`، **`IsAdmin` (قرار 13)**، **`IsBackupSearchEnabled = false` (قرار 12 — Stub معطَّل)**.
- **الأوامر**: `SearchCommand`, `RefreshCommand` (F5)، `OpenPatientDataCommand` (F2)، `OpenResultsCommand` (F4)، **`DeleteCommand` (CanExecute = IsAdmin — قرار 13)**، `PrintGroupResultsCommand`، **`SwitchToBackupCommand` (معطَّل — قرار 12)**، `BackCommand` (Esc).

### Part 6.4 — SearchView
- **الملفات**: `Views/Pages/SearchView.xaml`.
- **التخطيط**: يمين فلاتر البحث + حقول البحث بالكود، وسط `DataGrid` نتائج البحث، أسفل قائمة تحاليل المريض + ملخّص + أزرار.
- **زر الحذف**: معطَّل لغير Admin (قرار 13).
- **قائمة مصدر البحث (Main/Backup)**: **عنصر Backup معطَّل بصرياً (`IsEnabled = false`) مع tooltip "غير مفعّل حالياً" — قرار 12**.

### Part 6.5 — ربط الـ Toolbar + F3 عالمي
- تعديل `MainDashboardViewModel` + `KeyBinding` F3 في `MainWindow.xaml`.

### Part 6.6 — Build Verification
- فتح النافذة → بحث بجزء من الاسم → عرض النتائج → تحديد مريض → عرض تحاليله → فتح Function 3 أو 1 → زر الحذف يعمل مع Admin فقط (قرار 13) → قائمة Backup معطَّلة (قرار 12).

## 4) Open Questions
- لا توجد أسئلة مفتوحة بعد تطبيق القرارات 12 و 13 و 17.

---

# 📌 الملاحظات الختامية

- **ترتيب التنفيذ التقني (مُطبَّق فعلياً في هذه الوثيقة)**: F1 → F7 → F8 → F2 → F3 → F4 → F5 → F6. المريض (F1) يحتاج بيانات تحاليل (F7) ومعدلات طبيعية (F8) لتكون ذات معنى قابلة للاختبار قبل بناء وظائف سير العمل اللاحقة.
- **Assets/Helpers/Resources**: مجلدات مذكورة في `history.md` لكنها غير موجودة فعلياً في الـ commit. سيلزم إنشاؤها عند الحاجة.
- **DataTemplates للتنقل**: النموذج الحالي `NavigationService` يعيد `object?` — لعرضه في `ContentControl` يلزم تسجيل `DataTemplate` لكل زوج (ViewModel ↔ View) في `App.xaml` أو استخدام `ViewLocator`.
- **F1..F12 عالمياً**: يجب مركزة الاختصارات في `MainWindow.xaml` عبر `Window.InputBindings`.
- **QuestPDF ترخيص Community**: يجب استدعاء `QuestPDF.Settings.License = LicenseType.Community;` مرة واحدة عند بدء التطبيق.
- **ZXing.Net (قرار 3)**: يُضاف كـ NuGet Package `ZXing.Net` — مكتبة توليد الباركود الوحيدة المعتمدة في هذه المرحلة.
- **دور Admin (قرارات 2, 7, 11, 13)**: يتم الحصول على دور المستخدم الحالي عبر `IAuthService` وتمريره كـ `IsAdmin` (bool) لكل ViewModel تحتاج فحص الصلاحية. زر الحذف/التدقيق/التراجع عن التسليم يُربط بـ `CanExecute` الذي يعتمد على `IsAdmin`. لا يُبنى نظام Permissions دقيق في هذه المرحلة.
- **جدول ReferralPrices (قرار 15)**: يُبنى في Function 7 ويُستخدم في Function 1 — الترتيب التنفيذي يضمن توفره قبل استخدامه في حساب الإجمالي.
- **حقول Boolean الطبية (قرار 1)**: 8 أعمدة Boolean منفصلة على `Patient` مباشرة — لا جدول منفصل ولا نص حر.
- **لا رقم فرع (قرار 5)**: تم حذف `BranchNumber` و `Branch` من كل الكيانات والإعدادات والإشارات المرجعية؛ موضع الفرع في صيغة الكود = ثابت برمجي "1".

---

**نهاية الوثيقة — الإصدار v3 مُصحَّح.**
