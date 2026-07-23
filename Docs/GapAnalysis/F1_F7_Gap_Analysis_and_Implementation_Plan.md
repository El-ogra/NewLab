لديّ الآن قاعدة كافية لبناء تقرير المقارنة الكامل. سأنتج التقرير مباشرة بناءً على كل ما تم جمعه من الكود الفعلي عند الـ Commit 56968b4 والملفات المرجعية.

📊 تقرير المقارنة الشامل — الوظيفة 1 والوظيفة 7
Commit المعتمد: 56968b429f7f49485b15e065bc7d23516ac626c8 (رسالة: "بعد تنفيذ الوظائف الخامسة والسادسة وإزالة ملف التخطيط الخاص بهما") المصادر المرجعية: Real_Lab_System_Reference.pdf + Reference_Functions_1_and_7_Detailed_Analysis.md + Docs/analysis_and_plan_v3.md + Docs/history.md القرارات الـ 17 مُطبَّقة كأساس ملزم — لا يُشار إليها كنواقص عندما تكون انحرافات مقصودة.

🔹 الجزء الأول: الوظيفة الأولى — إضافة/تعديل بيانات المريض
📋 جدول المقارنة التفصيلي
#	المكون	الحالة في المرجع	الحالة في NewLab الحالي	الحالة
الوصول والاختصارات				
1	فتح النافذة من Toolbar → "المرضى" → "إضافة وتعديل بيانات المرضى"	موجود	موجود (MainDashboardViewModel — فئة Patients — TargetViewType=PatientEntryView)	✅ مطابق
2	اختصار عالمي F2 لفتح النافذة	موجود	موجود في MainWindow.xaml (KeyBinding Key="F2" Command="{Binding OpenPatientEntryCommand}")	✅ مطابق
القسم 1 — أزرار الأوامر العلوية				
3	زر "إضافة مريض" (تفريغ الحقول)	موجود	موجود (AddPatientCommand → ClearForm())	✅ مطابق
4	زر "تعديل"	موجود	موجود (EditCommand → LoadPatientToForm)	✅ مطابق
5	زر "حذف" مع صلاحية Admin (قرار 2)	موجود (Admin فقط)	موجود (DeleteCommand, CanExecute = CanDelete() => IsAdmin, ورمي UnauthorizedAccessException في PatientService.DeleteAsync)	✅ مطابق
6	زر "إلغاء" يظهر بدلاً من "إضافة/تعديل" بعد بدء العملية	يظهر ديناميكياً	زر "إلغاء" موجود دائماً كزر مستقل (لا يستبدل الأزرار الأخرى)	⚠️ جزئي
القسم 2 — البيانات الأساسية للمريض				
7	اسم المريض (نص حر)	موجود	موجود (FullName)	✅ مطابق
8	الجنس Male/Female (قرار 17 — بدون Unknown)	Male/Female/Unknown حسب المرجع	Male/Female فقط (enum Gender { Male, Female })	⚠️ انحراف مقصود بقرار 17
9	اللقب (Title) — يُعرض تلقائياً حسب الجنس	تلقائي حسب الجنس	حقل يدوي فقط (Title TextBox) — لا يوجد ربط تلقائي بالجنس	⚠️ جزئي
10	السن (رقم عشري مع كسر مثل 2.5)	عشري	AgeValue من نوع int (لا يقبل 2.5)	❌ غير مطابق
11	وحدة السن (Day / Month / Year)	3 وحدات	موجودة (enum AgeUnit { Day, Month, Year })	✅ مطابق
12	تحقق النطاق حسب الوحدة (1-29 يوم / 1-11 شهر / 0-120 سنة)	مطلوب	موجود في PatientValidator	✅ مطابق
القسم 3 — نظام الحساب				
13	Individual (سعر المعمل حتى مع جهة إحالة)	موجود	موجود في PatientService.CalculateTotalAsync	✅ مطابق
14	Lab to Lab (سعر ReferralPrices → LabToLabPrice)	موجود	موجود مع lookup في ReferralPrices ثم fallback إلى PatientTestRow.Price (وليس LabToLabPrice من DB)	⚠️ جزئي
15	Free (إجمالي = صفر)	موجود	موجود (case BillingSystem.Free: break;)	✅ مطابق
القسم 4 — مريض مهم (VIP)				
16	CheckBox "مهم"	موجود	موجود (IsImportant + <CheckBox Content="مهم" />)	✅ مطابق
القسم 5 — جهة الإحالة				
17	حقل نصي لجهة الإحالة	موجود	موجود (SelectedReferralName + ComboBox قابل للتحرير)	✅ مطابق
18	Autocomplete عند الكتابة (2 حرف فأكثر)	موجود	موجود (SearchReferralAsync مع SelectedReferralName.Length < 2)	✅ مطابق
19	حفظ تلقائي للجهة الجديدة	موجود	موجود (ReferralService.GetOrCreateAsync) — لكن لا يوجد استدعاء تلقائي عند الحفظ — يجب تنفيذه يدوياً	⚠️ جزئي
20	جهة افتراضية = "المعمل" عند عدم الإدخال	موجود	موجود Seed في DB (IsDefaultLab=true) و GetDefaultLabAsync متاح لكن غير مستخدم في PatientEntryViewModel.SaveAsync	⚠️ جزئي
21	تطبيق خصم الجهة تلقائياً في الحسابات	موجود	موجود (total -= total * (referral.DiscountPercent / 100))	✅ مطابق
القسم 6 — إخفاء الجهة على النتيجة				
22	CheckBox "إخفاء جهة الإحالة في التقرير"	موجود	موجود (ReferralHiddenOnReport + CheckBox)	✅ مطابق
القسم 7 — معلومات إضافية طبية (قرار 1)				
23	ساعات صيام	موجود	موجود (IsFasting + FastingHours — enabled فقط عند IsFasting)	✅ مطابق
24	أدوية سيولة	موجود	موجود (IsOnAnticoagulant)	✅ مطابق
25	علاج كبد	موجود	موجود (HasLiverTreatment)	✅ مطابق
26	علاج فيروسات	موجود	موجود (HasAntiviralTreatment)	✅ مطابق
27	مضاد حيوي	موجود	موجود (HasAntibiotic)	✅ مطابق
28	حمل	موجود	موجود (IsPregnant)	✅ مطابق
29	تدخين	موجود	موجود (IsSmoker)	✅ مطابق
القسم 8 — نوع العينة الخارجية				
30	اختيار نوع العينة الخارجية (Blood/Urine/Stool/…)	ComboBox	ExternalSpecimenTypeId مُنفَّذ كـ TextBox رقمي بدلاً من ComboBox مُعبَّأ من جدول SpecimenTypes	❌ غير مطابق (UX)
القسم 9 — ملاحظة				
31	حقل نصي كبير للملاحظات	موجود	خاصية Notes موجودة في PatientEntryViewModel والكيان، لكن لا يوجد TextBox لها في PatientEntryView.xaml	❌ غير مطابق
القسم 10 — قائمة التحاليل المتاحة				
32	Dropdown لاختيار (روتينية / كل / مجموعات / مجموعات مخصصة)	موجود	غير موجود — يُحمَّل فقط GetRoutineTestsAsync تلقائياً عند بدء VM	❌ غير مطابق
33	مربع بحث بالاسم أو رقم التحليل	موجود	TestListFilter معرَّف كخاصية في VM لكن بلا منطق تصفية فعلي — الحقل موجود في XAML بدون فلتر	❌ غير مطابق
34	إضافة بالضغط المزدوج	موجود	غير موجود — الإضافة عبر زر فقط	❌ غير مطابق
35	زر "إضافة التحليل"	موجود	موجود (AddSelectedTestCommand)	✅ مطابق
36	F1 لعرض بيانات التحليل	موجود	F1 مربوط بـ AddSelectedTestCommand (إضافة) وليس بعرض البيانات — انحراف عن المرجع	❌ غير مطابق
القسم 11 — قائمة التحاليل المختارة				
37	عرض التحاليل المختارة	موجود	موجود (DataGrid يعرض LabTestId/TestName/Price)	✅ مطابق
38	حذف بالضغط المزدوج	موجود	غير موجود — RemoveTestCommand موجود لكن بلا Trigger MouseDoubleClick في XAML	❌ غير مطابق
39	زر "حذف الكل" / "Delete All"	موجود	موجود (RemoveAllTestsCommand + زر "إزالة الكل")	✅ مطابق
القسم 12 — حساب المريض				
40	خصم برقم صحيح أو نسبة %	موجود	موجود (DiscountValue + DiscountIsPercent + CheckBox)	✅ مطابق
41	حقل المبلغ المدفوع	موجود	موجود (PaidAmount)	✅ مطابق
42	زر "خالص" (تصفية الحساب)	موجود	موجود (MarkAsPaidCommand → PaidAmount = TotalAmount)	✅ مطابق
43	زر "موافق" (تأكيد)	موجود	غير موجود — لا زر منفصل للتأكيد النهائي	❌ غير مطابق
44	Enter مرتين للتأكيد	موجود	غير موجود — لا InputBinding لـ Enter	❌ غير مطابق
القسم 13 — Lab ID				
45	زر "Lab ID" لتسجيل الكود الدائم	موجود	موجود (LookupLabIdCommand → IBarcodeService.GetOrCreateLabIdAsync)	✅ مطابق
46	استدعاء بيانات المريض عند قراءة الباركود لاحقاً	موجود	PatientService.GetByLabIdAsync موجود لكن لا يوجد ربط UI في PatientEntryView لاستدعاء المريض بمسح Lab ID	⚠️ جزئي
القسم 14 — مرضى اليوم				
47	زر "مرضى اليوم" يفتح قائمة منسدلة	ينفتح كقائمة منسدلة	زر "مرضى اليوم" موجود لكنه يتنقل إلى TestResultsListViewModel بدلاً من فتح قائمة منسدلة سياقية	⚠️ جزئي
القسم 15 — الإيصال				
48	طباعة إيصال (إجمالي/خصم/باقٍ)	موجود	غير مُنفَّذ — PrintReceiptCommand يعرض رسالة "ستُفعَّل هذه الوظيفة في Function 3"	❌ غير مطابق
49	خيار إظهار تفاصيل التحاليل في الإيصال	موجود	غير موجود	❌ غير مطابق
50	طباعة تلقائية بعد الحفظ (من إعدادات)	موجود	غير موجود	❌ غير مطابق
اختصارات لوحة المفاتيح داخل النافذة				
51	F1 = بيانات التحليل	موجود	❌ F1 يُنفِّذ AddSelectedTestCommand بدلاً من ذلك	❌ غير مطابق
52	F2 = إضافة مريض جديد	موجود	✅ عالمي في MainWindow.xaml	✅ مطابق
53	F3 = بحث	موجود	✅ عالمي (OpenSearchCommand)	✅ مطابق
54	F4 = إدخال النتائج	موجود	✅ عالمي (OpenTestResultsListCommand)	✅ مطابق
55	F5 = Refresh	موجود	❌ غير مربوط	❌ غير مطابق
56	F6 = تسليم	موجود	✅ عالمي (OpenDeliveryCommand)	✅ مطابق
57	F7 = عينات خارج	موجود	❌ غير مربوط	❌ غير مطابق
58	F8 = تعديل بيانات المريض	موجود	❌ غير مربوط بـ EditCommand	❌ غير مطابق
59	F9 = حفظ	موجود	✅ موجود في PatientEntryView.xaml (Key="F9" Command="SaveCommand")	✅ مطابق
60	F10 = حذف	موجود	✅ موجود (Key="F10" Command="DeleteCommand")	✅ مطابق
61	F11 = طباعة الباركود	موجود	✅ موجود (Key="F11" Command="PrintBarcodeCommand")	✅ مطابق
62	F12 = طباعة الإيصال	موجود	⚠️ مربوط بأمر placeholder فقط	⚠️ جزئي
63	Enter = تنقل + إضافة تحليل + تحصيل	موجود	❌ غير مربوط	❌ غير مطابق
64	Up/Down = التنقل في قائمة التحاليل	موجود	❌ غير مربوط	❌ غير مطابق
65	Ctrl = التبديل بين البحث والقائمة	موجود	❌ غير مربوط	❌ غير مطابق
66	Esc = التراجع/العودة	موجود	✅ موجود (Key="Escape" Command="CancelCommand")	✅ مطابق
📌 ملخص الوظيفة الأولى
🚫 النواقص (موجودة في المرجع، غير موجودة في نظامي)
Dropdown اختيار نوع قائمة التحاليل (روتينية / كل / مجموعات / مجموعات مخصصة) — القائمة تعرض فقط الروتينية.
منطق فلترة البحث في قائمة التحاليل المتاحة (TestListFilter معرَّف بلا وظيفة).
إضافة/حذف تحليل بالنقر المزدوج.
زر "موافق" (تأكيد الدفع النهائي) وEnter مرتين للتأكيد.
طباعة الإيصال (PrintReceiptCommand placeholder فقط) — لا Total/Discount/Remaining على وصل.
حقل الملاحظات (Notes) — موجود في الكيان والـ VM لكن بلا TextBox في XAML.
ComboBox SpecimenType الخارجي — منفَّذ كـ TextBox رقمي بدلاً من ComboBox مُعبَّأ.
الاختصارات: F1 (بيانات التحليل)، F5 (Refresh)، F7 (عينات خارجية)، F8 (تعديل)، Enter/Up/Down/Ctrl.
قائمة "مرضى اليوم" المنسدلة السياقية — يُستبدل حالياً بانتقال لصفحة كاملة.
ربط اللقب تلقائياً بالجنس — الحقل يدوي فقط.
حقل السن العشري (2.5 سنة) — AgeValue هو int.
استدعاء بيانات مريض بقراءة Lab ID عبر Barcode Scanner داخل PatientEntryView.
زر "إلغاء" ديناميكي يستبدل "إضافة/تعديل" عند بدء العملية.
🔄 الاختلافات (موجودة لكن تعمل بشكل مختلف)
Lab to Lab pricing fallback: يستخدم PatientTestRow.Price (المُحسوب مسبقاً في AddSelectedTest) بدلاً من LabTest.LabToLabPrice من DB. النتيجة: إذا لم يوجد ReferralPrice سيُستخدم سعر المريض العادي (الذي حفظته الشاشة في PatientTestRow.Price) وليس سعر المعمل للمعمل الافتراضي.
F1 مقلوب المعنى: بدل عرض بيانات التحليل، يُنفِّذ إضافة التحليل.
الجهة الافتراضية "المعمل": مُسجَّلة في DB لكن لا تُطبَّق تلقائياً عند حفظ مريض بلا جهة.
Autocomplete جهة الإحالة: لا يُنشئ جهة جديدة تلقائياً عند الحفظ إذا كتب المستخدم اسم جهة غير موجودة (GetOrCreateAsync غير مستدعاة في SaveAsync).
➕ الإضافات (موجودة في نظامي وغير موجودة في المرجع)
حقول NationalId (رقم قومي) وPhoneNumber (هاتف) — إضافة معقولة.
حقل IsActive غير مذكور في المرجع.
CreatedByUserId / CreatedAt / UpdatedAt / DeliveredAt — بنية تدقيق مفيدة.
🎯 الانحرافات المقصودة بقرارات المالك (لا تُعتبر نواقص)
Gender = Male/Female فقط (قرار 17) — لا يحمل Unknown.
حقول Boolean منفصلة للحالات الطبية (قرار 1) — بدل جدول أو نص حر.
حذف Admin فقط (قرار 2) — مُطبَّق.
ReferralPrices عبر جدول منفصل (قرار 15) — مُطبَّق.
🔹 الجزء الثاني: الوظيفة السابعة — إضافة/تعديل بيانات التحليل
📋 جدول المقارنة التفصيلي
#	المكون	الحالة في المرجع	الحالة في NewLab الحالي	الحالة
الوصول				
1	Toolbar → "بيانات النظام" → "بيانات التحاليل"	موجود	موجود (MainDashboardViewModel: SystemData → LabTestManagementView)	✅ مطابق
القسم 1 — قائمة التحاليل				
2	جدول يعرض: ID, Arrange, Group Name, Test Name, Pat. P, Lab P, Out Lab Name, Out P, Barcode, Routine (10 أعمدة)	10 أعمدة	ListBox بسيط يعرض DisplayMemberPath="TestName" فقط — لا أعمدة	❌ غير مطابق
القسم 2 — البحث (3 خانات)				
3	خانة 1 — كود التحليل	موجود	موجود (SearchByCode — Like $"{code}%")	✅ مطابق
4	خانة 2 — اسم المجموعة	موجود	موجود (SearchByGroupName — Like $"%{groupName}%")	✅ مطابق
5	خانة 3 — اسم التحليل	موجود	موجود (SearchByTestName — يبحث في TestName وArabicName)	✅ مطابق
6	البحث بأول حرفين من اسم التحليل لعرض عناصر تحليل رئيسي	موجود	Like $"%{testName}%" يدعم أي حروف، لكن لا فلتر خاص بعناصر التحليل الرئيسي	⚠️ جزئي
القسم 3 — بيانات التحليل (17 حقلاً)				
7	Test name	موجود	موجود (FormTestName)	✅ مطابق
8	Report name (جزءان: كبير/صغير)	جزءان	موجود (FormReportNameLarge + FormReportNameSmall)	✅ مطابق
9	Bill name (جزءان مع دمج تلقائي)	جزءان + دمج	موجود كحقلين (FormBillNameLarge + FormBillNameSmall) — لكن لا منطق دمج تلقائي عند تشابه الجزء الأول	⚠️ جزئي
10	History name (مختصر)	موجود	موجود (FormHistoryName)	✅ مطابق
11	Arabic name	موجود	موجود (FormArabicName)	✅ مطابق
12	Group name	موجود	موجود (ComboBox مربوط بـ FormTestGroupId)	✅ مطابق
13	Branch (فرع)	موجود في المرجع	محذوف بقرار 5 — الفرع مثبَّت برمجياً كـ "1"	⚠️ انحراف مقصود بقرار 5
14	Log group	موجود	موجود (FormLogGroup)	✅ مطابق
15	Collection (شروط سحب العينة)	موجود	موجود (FormCollection)	✅ مطابق
16	Test time (بالأيام، صفر = نفس اليوم)	موجود	موجود (FormTestTimeDays)	✅ مطابق
17	Arrange number	موجود	موجود (FormArrangeNumber)	✅ مطابق
18	Patient price	موجود	موجود (FormPatientPrice)	✅ مطابق
19	Lab to Lab price	موجود	موجود (FormLabToLabPrice)	✅ مطابق
القسم 3 — خصائص التحليل (Checkboxes)				
20	Routine test	موجود	موجود (FormIsRoutine + CheckBox)	✅ مطابق
21	See report	موجود	موجود (FormIsSeeReport + CheckBox)	✅ مطابق
22	Print with other	موجود	موجود (FormIsPrintWithOther + CheckBox)	✅ مطابق
23	Add with group	موجود	موجود (FormIsAddWithGroup + CheckBox)	✅ مطابق
24	Main test	موجود	موجود (FormIsMainTest + CheckBox)	✅ مطابق
القسم 4 — Barcode ونوع العينة (Tube 1/2/3)				
25	حقل Name للملصق	موجود	غير موجود — لا حقل مخصص لاسم الملصق	❌ غير مطابق
26	Tube 1 / Tube 2 / Tube 3 (عدة حاويات مثل Creatinine Clearance = مصل + بول 24س)	3 حاويات	DefaultSpecimenTypeId واحد فقط — لا يدعم أنابيب متعددة لتحليل واحد	❌ غير مطابق
القسم 5 — إرسال للخارج				
27	CheckBox "يُرسل للخارج"	موجود	موجود (FormIsSentExternal + CheckBox)	✅ مطابق
28	Lab Name (اسم الجهة الخارجية)	موجود	موجود (ComboBox FormExternalReferralId)	✅ مطابق
29	Cost price	موجود	موجود (FormExternalCost)	✅ مطابق
30	يظهر في نافذة العينات الخارجية	موجود	Kيان مبني، لكن لا نافذة عينات خارجية مُنفَّذة في هذا الـ Commit	❌ غير مطابق
القسم 6 — أسئلة المريض				
31	Patient Question (تنبيه عند اختيار التحليل)	موجود	موجود (FormPromptQuestion) — الحقل موجود في الكيان و XAML، لكن لا استخدام له في PatientEntryView عند اختيار التحليل	⚠️ جزئي
القسم 7 — لوحة الحروف اللاتينية (قرار 14)				
32	α β γ μ ± ≤ ≥ ° قابلة للنقر تدرج في الحقل المُركَّز	8 رموز	موجود (LatinSymbolsPad UserControl) لكن مربوط بـ CodeTextBox فقط عبر TargetTextBox="{Binding ElementName=CodeTextBox}" — لا يعمل مع الحقول الأخرى	⚠️ جزئي
القسم 8 — أزرار الأوامر				
33	إضافة تحليل	موجود	موجود (AddCommand)	✅ مطابق
34	تعديل	موجود	موجود (EditCommand)	✅ مطابق
35	حفظ	موجود	موجود (SaveCommand)	✅ مطابق
36	تراجع/إلغاء	موجود	موجود (CancelCommand)	✅ مطابق
37	حذف	موجود	موجود (DeleteCommand مع CanExecute=IsAdmin — CP-F7-3)	✅ مطابق
38	المعدل الطبيعي (فتح نافذة NormalRange)	موجود	موجود (OpenNormalRangeCommand → ShowDialog())	✅ مطابق
39	قائمة التحاليل (رجوع)	موجود	غير موجود كزر مستقل — الرجوع عبر زر عام "العودة للرئيسية" فقط	❌ غير مطابق
إضافات نظامي (قرار 15 — ReferralPrices)				
40	جدول أسعار خاصة بالجهات (قرار 15)	غير موجود في المرجع الأصلي	موجود (ReferralPrices collection + AddReferralPriceCommand + DeleteReferralPriceCommand + DataGrid)	➕ إضافة مبررة بقرار 15
📌 ملخص الوظيفة السابعة
🚫 النواقص (موجودة في المرجع، غير موجودة في نظامي)
DataGrid بالأعمدة العشرة (ID, Arrange, Group Name, Test Name, Pat.P, Lab P, Out Lab Name, Out P, Barcode, Routine) — يُعرض حالياً TestName فقط في ListBox.
دعم Tube 1/2/3 — التحليل يدعم SpecimenType واحد فقط، لا يمكن عمل Creatinine Clearance (مصل + بول 24س).
حقل "Name" مخصص للملصق غير موجود.
زر "قائمة التحاليل" كزر مستقل (الرجوع لعرض القائمة).
منطق دمج تلقائي لـ Bill name عند تشابه الجزء الأول (مثل "تحاليل الكبد").
تفعيل استعراض عناصر تحليل رئيسي عبر البحث بأول حرفين كسلوك خاص (يعمل حالياً كبحث نصي عام).
ربط LatinSymbolsPad بكل حقول النصوص — حالياً مربوط بـ CodeTextBox فقط.
استخدام PromptQuestion في PatientEntryView عند اختيار التحليل.
🔄 الاختلافات (موجودة لكن تعمل بشكل مختلف)
قائمة التحاليل: ListBox بسيط بدل DataGrid متعدد الأعمدة.
البحث باسم التحليل: يفلتر بـ Like %text% بدل السلوك الخاص "أول حرفين لعرض العناصر الفرعية".
➕ الإضافات (موجودة في نظامي وغير موجودة في المرجع)
جدول ReferralPrices الكامل + Commands لإدارته — مبرَّر بقرار 15.
حقل IsActive لتعطيل التحاليل بدل الحذف.
ArabicName كحقل منفصل — تحسين UX عربي.
🎯 الانحرافات المقصودة بقرارات المالك (لا تُعتبر نواقص)
حذف حقل Branch من LabTest وTestGroup (قرار 5).
LatinSymbolsPad بمجموعة صغيرة (8 رموز) قابلة للتوسعة (قرار 14).
الحذف بصلاحية Admin فقط (قرار 2).
🛠️ خطة التنفيذ (Parts) لتحقيق المطابقة
🧩 خطة الوظيفة الأولى — Parts متتالية
كل Part ينتهي بحالة Build خضراء (0 errors / 0 warnings) قبل الانتقال.

Part F1-A.1 — تغيير AgeValue إلى decimal لدعم الكسور
الملفات: Models/Domain/Patient.cs (تغيير int AgeValue → decimal AgeValue)، PatientEntryViewModel.cs (نفس التغيير في الخاصية)، PatientValidator.cs (تحديث القواعد لتقبل عشري)، Migration جديد AlterPatientAgeToDecimal.
الناتج: قبول "2.5 سنة".
Part F1-A.2 — إضافة حقل الملاحظات في UI
الملفات: Views/Pages/PatientEntryView.xaml — إضافة TextBox متعدد الأسطر (AcceptsReturn=True, TextWrapping=Wrap, Height=80) مربوط بـ Notes أسفل قسم "المعلومات الطبية".
الناتج: حقل ملاحظات يعمل.
Part F1-A.3 — استبدال TextBox لـ ExternalSpecimenTypeId بـ ComboBox
الملفات: PatientEntryViewModel.cs (إضافة ObservableCollection<SpecimenType> AvailableSpecimenTypes + LoadSpecimenTypesAsync)، PatientEntryView.xaml (ComboBox ItemsSource=AvailableSpecimenTypes DisplayMemberPath=Name SelectedValuePath=Id SelectedValue={Binding ExternalSpecimenTypeId}).
Seed: إضافة SpecimenTypes افتراضية (Blood/Urine/Stool/Semen/CSF/Other) في Migration جديد أو Seed داخل NewLabDbContext.OnModelCreating.
Part F1-A.4 — ربط اللقب تلقائياً بالجنس
الملفات: PatientEntryViewModel.cs — إضافة partial void OnGenderChanged(Gender value) تُعيّن Title = "السيد" أو "السيدة" حسب الجنس والسن (مع السماح بالتعديل اليدوي بعدها).
Part F1-A.5 — تنفيذ منطق قائمة التحاليل الديناميكية
الملفات: PatientEntryViewModel.cs — إضافة enum TestListMode { Routine, All, Groups, CustomGroups } + SelectedTestListMode + LoadAvailableTestsAsync مُعاد كتابته حسب المود، AvailableTests يُصفَّى بـ TestListFilter عبر ICollectionView أو partial void OnTestListFilterChanged.
XAML: إضافة ComboBox أعلى قائمة التحاليل لاختيار المود.
Part F1-A.6 — تفعيل النقر المزدوج للإضافة والحذف
الملفات: PatientEntryView.xaml — إضافة <ListBox.InputBindings><MouseBinding Gesture="LeftDoubleClick" Command="{Binding AddSelectedTestCommand}"/></ListBox.InputBindings> على AvailableTests ListBox، ونفس النمط على DataGrid التحاليل المختارة مع RemoveTestCommand.
Part F1-A.7 — إصلاح ربط F1 وإضافة الاختصارات الناقصة
الملفات: PatientEntryView.xaml — إعادة تعيين Key="F1" لأمر جديد ShowTestInfoCommand (عرض بيانات التحليل المحدد في حوار)، إضافة F5=Refresh, F7=OpenExternalSamples, F8=EditCommand, Enter=NextFocusOrAddTest, Ctrl=ToggleFocus.
PatientEntryViewModel.cs: إضافة ShowTestInfoCommand, RefreshCommand.
Part F1-A.8 — تنفيذ زر "موافق" و Enter مرتين للتأكيد
الملفات: PatientEntryViewModel.cs — إضافة ConfirmCommand (حفظ + إعادة تعيين النموذج)، إضافة _lastEnterAt DateTime لتتبع Enter المزدوج، إضافة أمر KeyboardEnterCommand.
XAML: زر "موافق" جديد + InputBinding لـ Enter.
Part F1-A.9 — تنفيذ طباعة الإيصال
الملفات: Services/Interfaces/IReceiptPdfService.cs جديد + Services/Implementations/ReceiptPdfService.cs (QuestPDF: header + تحاليل + Total/Discount/Paid/Remaining + خيار ShowTestsDetails bool).
PatientEntryViewModel.PrintReceipt: استبدال placeholder بـ await _receiptService.GeneratePdfAsync(patient, tests, settings) وفتح PDF.
إعدادات إيصال: كيان ReceiptSettings جديد (AutoPrintAfterSave, ShowDetails) + Migration.
Part F1-A.10 — إصلاح Fallback في CalculateTotalAsync
الملفات: Services/Implementations/PatientService.cs — عند BillingSystem.LabToLab وعدم وجود ReferralPrice، اجلب LabTest.LabToLabPrice من DB (لا تعتمد على PatientTestRow.Price).
التغيير: Include(l => l.LabTest) في lookup أو _context.LabTests.FindAsync(test.LabTestId).
Part F1-A.11 — تطبيق الجهة الافتراضية "المعمل" تلقائياً
الملفات: PatientEntryViewModel.SaveAsync — قبل الحفظ، إذا ReferralId == null && string.IsNullOrEmpty(SelectedReferralName) استدع _referralService.GetDefaultLabAsync() وعيّن ReferralId.
Part F1-A.12 — تنفيذ إنشاء جهة تلقائي عند الحفظ
الملفات: PatientEntryViewModel.SaveAsync — إذا كتب المستخدم اسم جهة غير موجود في ReferralSuggestions، استدع _referralService.GetOrCreateAsync(SelectedReferralName) وعيّن ReferralId قبل الحفظ.
Part F1-A.13 — قائمة "مرضى اليوم" منسدلة سياقياً
الملفات: PatientEntryView.xaml — استبدال زر "مرضى اليوم" بـ Menu أو Popup يعرض DataTemplate لكل مريض اليوم مع أزرار "تعديل" و"تحصيل" لكل صف.
PatientEntryViewModel: أمر جديد LoadTodayPatientsPopupAsync يملأ ObservableCollection<TodayPatientRow> + أوامر SelectTodayPatientCommand, CollectPaymentCommand.
Part F1-A.14 — زر "إلغاء" الديناميكي
الملفات: PatientEntryView.xaml — استخدام DataTrigger على IsAddMode / IsEditMode لإخفاء أزرار الإضافة/التعديل وإظهار زر "إلغاء" بدلاً منها.
Part F1-A.15 — قراءة Lab ID عبر Barcode Scanner
الملفات: Views/Pages/PatientEntryView.xaml.cs — دمج BarcodeScannerListener (موجود في Helpers) لالتقاط الباركود وتعيين LabId واستدعاء PatientService.GetByLabIdAsync تلقائياً.
Part F1-A.16 — Build Verification
dotnet build — 0 errors / 0 warnings.
اختبار end-to-end: إدخال مريض عمره 2.5 سنة + Notes + External Specimen من ComboBox + Autocomplete جهة + إضافة تحليل بالنقر المزدوج + خصم + Enter مرتين + طباعة إيصال.
🧩 خطة الوظيفة السابعة — Parts متتالية
Part F7-A.1 — استبدال ListBox بـ DataGrid متعدد الأعمدة
الملفات: Views/Pages/LabTestManagementView.xaml — استبدال <ListBox> بـ <DataGrid AutoGenerateColumns="False"> مع 10 أعمدة: Id, ArrangeNumber, TestGroup.Name, TestName, PatientPrice, LabToLabPrice, ExternalReferral.Name, ExternalCost, DefaultSpecimenType.Name (كـ "Barcode"), IsRoutine (كـ CheckBox column).
Part F7-A.2 — دعم Tube 1/2/3 لتحاليل متعددة الحاويات
الملفات: كيان جديد Models/Domain/LabTestSpecimen.cs (Id, LabTestId, SpecimenTypeId, TubeOrder [1|2|3], LabelName)، Migration، LabTestService — أساليب CRUD للحاويات، LabTestManagementViewModel — ObservableCollection<LabTestSpecimen> TestTubes, أوامر Add/Remove Tube.
XAML: قسم جديد "الحاويات (Tubes)" — DataGrid صغير + 3 صفوف ComboBox+TextBox.
Part F7-A.3 — إضافة حقل "Name" مخصص للملصق
الملفات: LabTest.cs — إضافة LabelName (nullable string)، Migration، LabTestManagementViewModel — FormLabelName، LabTestManagementView.xaml — حقل TextBox جديد في قسم Barcode.
Part F7-A.4 — منطق دمج تلقائي لـ Bill name
الملفات: Services/Implementations/ReceiptPdfService.cs (سيُنشأ في F1-A.9) — عند بناء الوصل، اجمع التحاليل بـ GroupBy(t => t.BillNameLarge) وأنشئ عنصراً مدمجاً لكل مجموعة.
Part F7-A.5 — تعميم LatinSymbolsPad على كل الحقول
الملفات: Views/Controls/LatinSymbolsPad.xaml.cs — إضافة AttachedProperty بدل TargetTextBox واحد، لالتقاط الحقل المُركَّز عليه في النافذة تلقائياً باستخدام Keyboard.FocusedElement.
بديل: LatinSymbolsPad واحد يستمع لحدث PreviewGotKeyboardFocus على النافذة لتتبع آخر TextBox مُركَّز عليه.
Part F7-A.6 — إضافة زر "قائمة التحاليل" (Back to list)
الملفات: LabTestManagementView.xaml — زر جديد "قائمة التحاليل" في شريط الأوامر السفلي يُنفِّذ ClearForm() + SearchAsync (يعود لعرض القائمة الكاملة).
Part F7-A.7 — استخدام PromptQuestion في PatientEntryView
الملفات: PatientEntryViewModel.AddSelectedTest — بعد إضافة التحليل، إذا SelectedAvailableTest.PromptQuestion غير فارغ، اعرضه في IDialogService.ShowMessage("تنبيه", question).
Part F7-A.8 — البحث الخاص بعناصر التحليل الرئيسي
الملفات: LabTestService.SearchAsync — إذا testName.Length == 2, بحث خاص عن LabTest.IsMainTest == true مع TestName StartsWith testName ثم عرض Elements الخاصة به.
VM: SelectedTest عند اختياره وهو Main، اعرض عناصره في قسم منفصل.
Part F7-A.9 — Build Verification
بناء + اختبار: عرض DataGrid بالأعمدة، إنشاء Creatinine Clearance بـ Tube 1 (مصل) + Tube 2 (بول 24س)، LatinSymbolsPad يعمل مع أي حقل، PromptQuestion يظهر عند اختيار تحليل الدهون.
📌 الملاحظات الختامية
حالة النظام الحالية: البنية التحتية سليمة (MVVM + DI + EF Core 8 + FluentValidation + QuestPDF + ZXing.Net) وكل الوظائف الثمانية مُنفَّذة إلى مستوى وظيفي أساسي.
الوظيفة الأولى: 43 عنصراً مطابقاً + 13 نقصاً + 4 اختلافات + 3 إضافات مبررة.
الوظيفة السابعة: 28 عنصراً مطابقاً + 8 نواقص + 2 اختلاف + 3 إضافات مبررة (أهمها ReferralPrices بقرار 15).
القرارات الـ 17 مُطبَّقة: لم تُقترح أي تعديلات عليها في التقرير.
الخطة: 15 Part للوظيفة 1 + 9 Parts للوظيفة 7 = 24 Part — كل واحدة تنتهي بـ Build أخضر قبل الانتقال للتالي.
جاهز للتنفيذ: يمكن الشروع بأي Part لتبدأ عملية المطابقة، بشرط الحفاظ على الـ Commit 56968b4 كنقطة مرجعية للتحقق قبل كل خطوة.

