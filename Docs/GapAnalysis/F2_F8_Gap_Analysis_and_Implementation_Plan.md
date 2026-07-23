# 📋 تقرير تحليل الفجوات وخطة التنفيذ — الوظيفة 2 والوظيفة 8

**المشروع**: `NewLab` (https://github.com/El-ogra/NewLab.git)
**الفرع**: `main`
**Commit Hash**: `56968b429f7f49485b15e065bc7d23516ac626c8`
**رسالة الـ Commit**: "بعد تنفيذ الوظائف الخامسة والسادسة وإزالة ملف التخطيط الخاص بهما"
**تاريخ الـ Commit**: 2026-07-22
**النظام المرجعي**: Real Lab System v1.0 (الإصدار 1.13)
**الصفحات المرجعية**: 18–20 (الوظيفة 2)، 41–44 (الوظيفة 8)
**تاريخ التقرير**: 2026-07-23

---

## 🔎 التحقق من صحة المستودع (Rigorous Verification)

| بند | القيمة المطلوبة | القيمة الفعلية بعد الاستنساخ | الحالة |
|---|---|---|---|
| Repository URL | `https://github.com/El-ogra/NewLab.git` | تم الاستنساخ بنجاح | ✅ |
| Branch | `main` | `main` | ✅ |
| Commit Hash | `56968b429f7f49485b15e065bc7d23516ac626c8` | `56968b429f7f49485b15e065bc7d23516ac626c8` | ✅ مطابق تماماً |
| Commit Message | "بعد تنفيذ الوظائف الخامسة والسادسة وإزالة ملف التخطيط الخاص بهما" | مطابق حرفياً | ✅ |
| Commit Date | — | `2026-07-22 20:55:40 +0300` | ✅ |

لا تعارض. الانتقال إلى التحليل.

---

## 📌 القرارات الملزمة ذات الصلة (من الـ 17 قراراً)

| القرار | النص الملزم |
|---|---|
| **قرار 3** | مكتبة `ZXing.Net` حصراً لتوليد الباركود — لا `BarcodeStandard` ولا غيرها. |
| **قرار 4** | الحجم الافتراضي للملصق `LabelWidth = 38mm × LabelHeight = 25mm`، قابل للتخصيص عبر UI. |
| **قرار 5** | لا يوجد حقل `BranchNumber` — الموضع الرابع في صيغة الكود الـ 13 خانة يُثبَّت برمجياً كالثابت `"1"`. |
| **قرار 14** | لوحة الرموز الغربية `LatinSymbolsPad` UserControl مُعاد استخدامه في نافذة المعدلات الطبيعية. |
| **قرار 16** | قاعدة "أضيق مدى يفوز" (Most Specific Match) في `GetMatchingRangeAsync`. |
| **قرار 17** | `Gender = Male/Female` حصراً — لا قيمة `Both`؛ تُدخَل 6 معدلات للتحاليل غير المعتمدة على الجنس. |

---

# 🟦 الجزء الأول: مقارنة الوظيفة الثانية — طباعة الباركود للمريض

## 📊 جدول المقارنة التفصيلي

| # | المكون (حقل/زر/خاصية) | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|---|---|---|---|
| 1 | فتح النافذة من زر "الباركود" داخل نافذة "بيانات المرضى" | زر "الباركود" داخل نافذة بيانات المرضى | زر "باركود" داخل `PatientEntryView.xaml` (سطر 249) مربوط بـ `PrintBarcodeCommand` → `PatientEntryViewModel.PrintBarcodeAsync` (سطر 201) يفتح `BarcodeView` كـ Modal | ✅ مطابق |
| 2 | العمل لمريض قديم (سابق) أيضاً | يعمل الزر سواء كان المريض حديثاً أم قديماً | `PrintBarcodeAsync` يفحص `_editingPatientId != null` (مريض قديم) وينشئ Patient من الفورم إن كان جديداً — كلا الحالتين مدعومتان | ✅ مطابق |
| 3 | الاختصار العالمي `F11` من نافذة بيانات المرضى | مذكور في اختصارات الوظيفة 1 | `PatientEntryView.xaml` سطر 11: `<KeyBinding Key="F11" Command="{Binding PrintBarcodeCommand}" />` | ✅ مطابق |
| 4 | 3 أنواع أكواد (Case=1، File=3، Lab=5) | 3 أنواع، كل واحد يبدأ برقم مميز من اليسار | `Models/Domain/Enums/CodeType.cs`: `Case, File, Lab` (3 قيم). `BarcodeService.BuildCode13` سطر 139–146: `Case→1, File→3, Lab→5` | ✅ مطابق |
| 5 | صيغة الكود 13 خانة (1-4-100623-1-006-8) | 13 خانة: نوع + نوع مكرر + YYMMDD + فرع + تسلسل 3 خانات + يوم الأسبوع | `BarcodeService.BuildCode13`: `$"1{typeDigit}{datePart}{BranchConstant}{seqPart}{dayOfWeek}"` = 1+1+6+1+3+1 = 13 خانة. **ملاحظة**: البادئة الثابتة "1" هنا مختلفة عن "لا يُنظر إليه" في المرجع، لكن العدد صحيح | ⚠️ جزئي (الطول 13 صحيح، لكن دلالة الخانة الأولى غير مطابقة تماماً — الخانة الأولى في المرجع ديناميكية "لا يُنظر إليها"، وفي الكود ثابتة "1") |
| 6 | تثبيت رقم الفرع برمجياً كـ "1" (قرار 5) | الموضع الرابع = ثابت "1" | `BarcodeService.cs` سطر 15: `private const string BranchConstant = "1";` — يُستخدم في `BuildCode13` مباشرة، لا يُقرأ من DB | ✅ مطابق (قرار 5 مُطبَّق حرفياً) |
| 7 | ملصقات الأنابيب مصنّفة حسب نوع العينة/المادة المسحوبة | كل ملصق يمثل نوع عينة (Serum Fasting, Urine, ...) وتحته التحاليل المطلوبة | `BarcodeService.GetLabelsForPatientAsync` (سطر 76) **لا يجمّع** حسب `SpecimenType`؛ يُنشئ **ملصقاً واحداً فقط** باسم ثابت "Default" لكل مريض. الحقل `SpecimenTypeId` موجود في `BarcodeLabel` لكن غير مستخدم | ❌ غير مطابق (نقص جوهري) |
| 8 | إمكانية طباعة كل ملصق على حدة | زر "طباعة" على كل ملصق مستقل | `BarcodeView.xaml` سطر 90–95: زر `PrintLabelCommand` لكل ملصق داخل `ItemsControl.ItemTemplate` | ✅ مطابق |
| 9 | زر "طباعة كود الملف" (Print File Barcode) | زر مستقل يطبع كود الملف فقط | `BarcodeView.xaml` سطر 40–43: `Button Content="طباعة كود الملف" Command="{Binding PrintFileCodeCommand}"` — VM سطر 52–70 | ✅ مطابق |
| 10 | زر "طباعة كود المعمل" (Print Lab Id Barcode) — مفعّل فقط عند وجود LabId | زر مستقل نشط عند وجود كود معمل | `BarcodeView.xaml` سطر 44–47 + `BarcodeViewModel.CanPrintLabCode` سطر 93: `Patient?.LabId != null` | ✅ مطابق |
| 11 | Checkbox "طباعة كود الملف مع الكل" (زر برتقالي "طباعة كلا الأكواد") | خيار موجود؛ عند تفعيله يظهر الزر البرتقالي | `BarcodeView.xaml` سطر 52–55: `<CheckBox Content="طباعة كود الملف مع الكل" IsChecked="{Binding PrintFileCodeWithAll}" />` + منطق `PrintAllAsync` سطر 109–120 يُدرج كود الملف عند التفعيل. **الفارق**: لا يوجد زر برتقالي منفصل — الوظيفة مدمجة في زر "طباعة الكل" | ⚠️ جزئي (الخيار موجود وظيفياً، لكن التمييز البصري "الزر البرتقالي" غير محقق) |
| 12 | باركود إضافي (Other Barcode) لحالات خاصة كالتبرع بالدم | خانتان (اسم + وصف) + زر إضافة/طباعة | `BarcodeView.xaml` سطر 148–163: `ExtraBarcodeName` + `ExtraBarcodeDescription` + `AddExtraBarcodeCommand`. الـ VM سطر 141–167 | ✅ مطابق |
| 13 | آلية Enter/زر الأسهم للانتقال بين خانتي الباركود الإضافي | Enter بعد الاسم ثم زر السهم لكتابة الوصف | لا يوجد `KeyBinding` على Enter بين خانتي `ExtraBarcodeName` و `ExtraBarcodeDescription`، ولا زر أسهم — يوجد فقط زر "إضافة" مباشر | ⚠️ جزئي (الوظيفة تعمل عبر Tab/Click وليس Enter/Arrow حسب المرجع) |
| 14 | منزلق أفقي (Left/X) لضبط مكان الطباعة | Slider أفقي | `BarcodeView.xaml` سطر 114–117: `<Slider Value="{Binding OffsetX}" Minimum="-50" Maximum="50" />` | ✅ مطابق |
| 15 | منزلق رأسي (Top/Y) لضبط مكان الطباعة | Slider رأسي | `BarcodeView.xaml` سطر 120–123: `<Slider Value="{Binding OffsetY}" Minimum="-50" Maximum="50" />` | ✅ مطابق |
| 16 | زر "حفظ" لتثبيت إعدادات الإزاحة | زر Save لحفظ Offset | `BarcodeView.xaml` سطر 140–143: `Button Content="حفظ الإحداثيات" Command="{Binding SaveSettingsCommand}"` — يحفظ في `BarcodeSettings` عبر `BarcodeService.SaveSettingsAsync` | ✅ مطابق |
| 17 | حجم الملصق افتراضياً 38×25 مم قابل للتخصيص (قرار 4) | حجم قابل للضبط | `BarcodeSettings.cs`: `LabelWidth = 38, LabelHeight = 25` + `BarcodeViewModel`: `labelWidth = 38, labelHeight = 25`. `BarcodeView.xaml` سطر 126–137: TextBox لكل بُعد. `BarcodePrintService.cs` سطر 25: `page.Size(settings.LabelWidth, settings.LabelHeight, Unit.Millimetre)` | ✅ مطابق (قرار 4 مُطبَّق) |
| 18 | مكتبة ZXing.Net حصراً لتوليد الباركود (قرار 3) | ZXing.Net | `Helpers/BarcodeImageGenerator.cs` سطر 3–5: `using ZXing; using ZXing.Common; using ZXing.Windows.Compatibility;` + `BarcodeWriter<System.Drawing.Bitmap>` بصيغة `Format = BarcodeFormat.CODE_128`. لا مكتبة أخرى في `csproj` | ✅ مطابق (قرار 3 مُطبَّق) |
| 19 | سلة محذوفات (Drag & Drop) لحذف تحليل من ملصق | منطقة "سلة" لسحب العناصر إليها للحذف | `BarcodeView.xaml.cs` يحوي `Label_MouseLeftButtonDown`, `Label_DragOver`, `Label_Drop`. الـ VM يحوي `RemoveLabelCommand` (سطر 169–174). **لكن**: لا توجد منطقة UI مخصصة تحمل شكل "سلة محذوفات" في `BarcodeView.xaml` — الـ Drop handler الحالي **يعيد ترتيب الملصقات فقط** (`Labels.Move(draggedIndex, targetIndex)` في سطر 57 من `BarcodeView.xaml.cs`) ولا يحذف | ❌ غير مطابق (الوظيفة الحذفية عبر سلة السحب غير محققة في UI) |
| 20 | نقل تحليل من ملصق لآخر بالسحب والإفلات | Drag&Drop يسمح بنقل تحليل واحد بين ملصقين | يوجد Drag&Drop لكنه يعمل على مستوى **الملصق ككل** (يُعيد ترتيب `Labels`) لا على مستوى **التحاليل الفردية داخل الملصق**. `BarcodeLabel.Tests` هو `List<string>` غير قابل للسحب فردياً | ❌ غير مطابق |
| 21 | زر إضافي منفصل "طباعة الكل" (الأكواد+الملصقات) | نعم — عند اختيار "كود الملف مع الكل" | `BarcodeView.xaml` سطر 48–51: `Button Content="طباعة الكل" Command="{Binding PrintAllCommand}"` — VM سطر 103–123 يطبع `Labels` + كود الملف إن كان الـ Checkbox مفعّلاً | ✅ مطابق |
| 22 | كيان `PatientCode` لتخزين الأكواد الثلاثة (CP-F2-2) | ضمني — الأكواد تُطبع من بيانات المريض | `Models/Domain/PatientCode.cs`: `Id, PatientId, CodeType, CodeValue, IssuedAt`. `BarcodeService.GetOrCreateLabIdAsync` يُنشئ سجل `PatientCode` تلقائياً | ✅ إضافة موجبة (تخزين رسمي للأكواد) |
| 23 | الطباعة عبر PDF (QuestPDF Community) | طباعة مباشرة على الطابعة (في المرجع) | `BarcodePrintService.GeneratePdf` يُنتج PDF (بدلاً من طباعة مباشرة). يُحفظ الملف على Desktop باسم `barcode_{timestamp}.pdf`. القرار CP-F2-3 يبرر ذلك (`QuestPDF Community License`) | ⚠️ جزئي (الناتج النهائي PDF لا طباعة مباشرة — قرار مقصود، لكن يختلف عن المرجع) |
| 24 | Escape يغلق النافذة (سلوك مألوف) | غير موثق في المرجع | `BarcodeView.xaml` سطر 12: `<KeyBinding Key="Escape" Command="{Binding PrintAllCommand}" />` — **خطأ منطقي**: Escape مربوط بـ `PrintAllCommand` بدلاً من إغلاق النافذة | ⚠️ خطأ برمجي (Escape يطبع الكل بدل الإغلاق) |

## 📉 قائمة النواقص للوظيفة 2 (موجود في المرجع، غير موجود في نظامي)

1. **[حرج] تصنيف الملصقات حسب نوع العينة**: `GetLabelsForPatientAsync` يُرجع ملصقاً واحداً باسم "Default" فقط. يجب أن يجمّع التحاليل حسب `LabTest.DefaultSpecimenTypeId` وينتج ملصقاً لكل نوع عينة (Serum Fasting, Urine, Stool, EDTA, ...).
2. **[حرج] سلة محذوفات مرئية لحذف تحليل من ملصق**: `BarcodeView.xaml` لا يحوي أي `Border` يمثل سلة محذوفات؛ الـ `Drop` الحالي يُعيد ترتيب فقط ولا يحذف.
3. **[حرج] Drag & Drop على مستوى التحليل الفردي (لا الملصق ككل)**: يجب دعم سحب اسم تحليل من `Tests` list في ملصق ما وإفلاته على ملصق آخر أو على السلة.
4. **الزر البرتقالي المتمايز بصرياً "طباعة كلا الأكواد"**: المرجع يوصفه كزر منفصل (لون برتقالي) يظهر عند تفعيل Checkbox — في نظامي الوظيفة مدمجة في زر واحد.
5. **آلية Enter/زر الأسهم في خانتي الباركود الإضافي**: المرجع ينص "اضغط انتر أو زر الأسهم" — في نظامي زر "إضافة" فقط.
6. **KeyBinding `Escape` يجب أن يُغلق النافذة**، لا أن ينفّذ `PrintAllCommand` (خطأ برمجي واضح).

## 🔀 قائمة الاختلافات للوظيفة 2 (موجود لكن يعمل بشكل مختلف)

1. **بنية صيغة الكود الأولى**: المرجع يقول "الرقم الأول = لا يُنظر إليه" (ديناميكي)، بينما نظامي يجعله ثابتاً "1" دائماً. النتيجة النهائية للتمييز صحيحة (الرقم الثاني هو المميّز)، لكن التمثيل مختلف.
2. **مصير الطباعة**: PDF إلى Desktop بدلاً من طباعة مباشرة (قرار CP-F2-3 مقصود).
3. **Drag & Drop على الملصقات**: في نظامي يُعيد ترتيب الملصقات (`Labels.Move`)، بينما في المرجع الغرض هو حذف/نقل تحليل داخل ملصق.
4. **الملصق الوحيد "Default"**: يُنشأ ملصق واحد للمريض من `PatientVisits` (الزيارة الأخيرة)، بينما المرجع يعرض ملصقاً لكل نوع عينة مع اسم التحاليل تحته.

## ➕ قائمة الإضافات في نظامي (غير موجودة في المرجع)

1. **كيان `PatientCode`** كسجل مستقل يخزن الأكواد الثلاثة تاريخياً (`CodeType`, `CodeValue`, `IssuedAt`) — قرار CP-F2-2.
2. **حقلا `LabelWidth` و `LabelHeight` كـ TextBox في UI** — قرار 4 يوسّع سلوك المرجع (كان لديه Offset فقط).
3. **زر منفصل "طباعة الكل"** (`PrintAllAsync`) لطباعة كل الملصقات دفعة واحدة.
4. **إخراج PDF بواسطة QuestPDF** بدلاً من الطابعة (قرار مقصود CP-F2-3).
5. **`BarcodeSettings` جدول DB** لتخزين الإعدادات (Offset + PrintFileCodeWithAll + LabelWidth/Height) بشكل دائم.

---

# 🟪 الجزء الثاني: مقارنة الوظيفة الثامنة — المعدلات الطبيعية

## 📊 جدول المقارنة التفصيلي

| # | المكون (حقل/زر/خاصية) | الحالة في المرجع | الحالة في نظامي الحالي | الحالة |
|---|---|---|---|---|
| 1 | فتح النافذة من نافذة "بيانات التحاليل" (F7) عبر زر "المعدل الطبيعي" | زر داخل نافذة بيانات التحاليل | `LabTestManagementView.xaml` سطر 272: `Button Content="المعدل الطبيعي" Command="{Binding OpenNormalRangeCommand}"` → `LabTestManagementViewModel.OpenNormalRangeAsync` سطر 213 يفتح `NormalRangeView` كـ Dialog | ✅ مطابق |
| 2 | قائمة المعدلات تعرض الأعمدة: T.ID, Test Name, Sex, From, To, Age unit, Normal Range, Low limit, High limit, Test unit, P & S | 11 عموداً في DataGrid | `NormalRangeView.xaml` سطر 52–73: `ListBox` بـ `ItemTemplate` يعرض فقط `Gender + AgeFrom-AgeTo + AgeUnit + NormalRangeText` (سطرين). **لا يوجد DataGrid مع 11 عموداً منفصلة** | ❌ غير مطابق (نقص جوهري في تفصيل الأعمدة) |
| 3 | زر "إضافة مدى" | نعم | `NormalRangeView.xaml` سطر 218 (يمين) + سطر 255 (أسفل): `Button Content="اضافة مدى" Command="{Binding AddRangeCommand}"` | ✅ مطابق (مكرر مرتين للسهولة) |
| 4 | زر "تعديل" | نعم | `NormalRangeView.xaml` سطر 222 + 259: `EditCommand` — الـ VM سطر 84–90 | ✅ مطابق |
| 5 | زر "حفظ" | نعم | `NormalRangeView.xaml` سطر 226 + 263: `SaveCommand` — الـ VM سطر 92–128 مع FluentValidation | ✅ مطابق |
| 6 | زر "تراجع" | نعم | `NormalRangeView.xaml` سطر 230 + 267: `CancelCommand` — الـ VM سطر 130–134 | ✅ مطابق |
| 7 | زر "حذف" | نعم | `NormalRangeView.xaml` سطر 234 + 271: `DeleteCommand` (لون أحمر). الـ VM سطر 136–159 مع تحقق `IsAdmin` (قرار 2 مضاف) | ✅ مطابق (+ حماية Admin) |
| 8 | زر "قائمة التحاليل" | يرجع لنافذة بيانات التحاليل | `NormalRangeView.xaml` سطر 239 + 276: `BackToTests_Click` → `Window.Close()` (كود-بيهايند سطر 16–19) | ✅ مطابق |
| 9 | حقل `Test name` | نعم | `NormalRangeView.xaml` سطر 121–125: `TextBox` مربوط بـ `FormTestName` | ✅ مطابق |
| 10 | حقل `Test unit` | نعم | `NormalRangeView.xaml` سطر 127–130: `FormTestUnit` | ✅ مطابق |
| 11 | حقل `Sex` = Male/Female فقط بدون Both (قرار 17) | Male/Female فقط | `NormalRangeView.xaml.cs` سطر 12: `GenderCombo.ItemsSource = new[] { Gender.Male, Gender.Female };`. `Gender.cs` يحتوي `Male, Female` فقط. الـ VM سطر 49: `AvailableGenders => new[] { Gender.Male, Gender.Female };` | ✅ مطابق (قرار 17 مُطبَّق حرفياً) |
| 12 | حقلا `Age From : To` | نعم | `NormalRangeView.xaml` سطر 111–118: `FormAgeFrom` + `FormAgeTo` | ✅ مطابق |
| 13 | حقل `Age unit` (Day/Month/Year) | نعم | `NormalRangeView.xaml` سطر 97–102 + `NormalRangeView.xaml.cs` سطر 13: `AgeUnitCombo.ItemsSource = System.Enum.GetValues<AgeUnit>();` | ✅ مطابق |
| 14 | حقل `Normal range` (النص المطبوع) | نعم | `NormalRangeView.xaml` سطر 132–135: `FormNormalRangeText` | ✅ مطابق |
| 15 | حقلا `Low limit` + `High limit` | نعم | `NormalRangeView.xaml` سطر 143–150: `FormLowLimit` + `FormHighLimit`. النوع `decimal(18,4)` في الـ Entity | ✅ مطابق |
| 16 | حقلا `Low flag` + `High flag` | نعم | `NormalRangeView.xaml` سطر 159–166: `FormLowFlag` + `FormHighFlag` | ✅ مطابق |
| 17 | حقلا `Low comment` + `High comment` | نعم | `NormalRangeView.xaml` سطر 169–177: `FormLowComment` + `FormHighComment` | ✅ مطابق |
| 18 | حقل `Critical comment` | نعم | `NormalRangeView.xaml` سطر 208–211: `FormCriticalComment` | ✅ مطابق |
| 19 | حقل `Critical range` (النص المطبوع) | نعم | `NormalRangeView.xaml` سطر 182–185: `FormCriticalRangeText` | ✅ مطابق |
| 20 | حقلا `Critical Low limit` + `Critical High limit` | نعم | `NormalRangeView.xaml` سطر 193–200: `FormCriticalLowLimit` + `FormCriticalHighLimit` (nullable decimal) | ✅ مطابق |
| 21 | حقل `Critical flag` | نعم | `NormalRangeView.xaml` سطر 203–206: `FormCriticalFlag` | ✅ مطابق |
| 22 | **مجموع الحقول = 16** حقلاً (مطابق للمرجع) | 16 حقلاً | Test name + Test unit + Sex + Age From + Age To + Age unit + Normal range + Low limit + High limit + Low flag + High flag + Low comment + High comment + Critical range + Critical Low limit + Critical High limit + Critical flag + Critical comment = **18 حقلاً في `NormalRange.cs`**. المرجع يعتبر Age From/To كحقل واحد "Age From : To" و Low+High limit مزدوجاً — لذا العدد صحيح تفصيلياً | ✅ مطابق |
| 23 | قاعدة "أضيق مدى يفوز" (Most Specific Match) — قرار 16 | مُلزَم | `NormalRangeService.GetMatchingRangeAsync` سطر 62–85: بعد فلترة Gender + AgeRange يستدعي `matched.OrderBy(nr => nr.AgeTo - nr.AgeFrom).First()` | ✅ مطابق (قرار 16 مُطبَّق حرفياً) |
| 24 | إمكانية إدخال 6 معدلات (رجال/نساء × 3 فئات عمرية) للتحاليل غير المعتمدة على الجنس | ممكن (إدخال يدوي 6 مرات) | لا يمنع النظام إدخال 6 معدلات؛ الـ Validator لا يمنع تكرار Gender بنطاقات مختلفة. **لكن لا يوجد Wizard أو تسهيل خاص لهذا السيناريو** | ⚠️ جزئي (ممكن يدوياً، لا اختصار) |
| 25 | لوحة الرموز والأسس قابلة للنقر لإدراجها في الحقول (قرار 14) | مجموعة رموز/أسس قابلة للنسخ واللصق | `Views/Controls/LatinSymbolsPad.xaml` + `.xaml.cs` سطر 34–41: عند الضغط `TargetTextBox.SelectedText = symbol`. مربوطة بـ `NormalRangeTextBox` عبر `TargetTextBox="{Binding ElementName=NormalRangeTextBox}"` (سطر 248) | ⚠️ جزئي (اللوحة مربوطة بـ `NormalRangeTextBox` فقط — لا تعمل مع باقي حقول الفورم كـ `FormCriticalRangeText`, `FormLowComment`, ...) |
| 26 | الرموز في اللوحة | الأسس (¹²³...) والرموز الرياضية | `LatinSymbolsPad.xaml.cs` سطر 15: القائمة الافتراضية = `{ "α", "β", "γ", "μ", "±", "≤", "≥", "°" }`. **لا توجد الأسس ¹²³ إلخ** المذكورة في المرجع | ⚠️ جزئي (7 رموز يونانية/رياضية بدل الأسس) |
| 27 | البحث بأول حرفين في خانة البحث الثالثة لتعديل عناصر تحليل رئيسي (مثل CBC/الفيدال) | من نافذة بيانات التحاليل، خانة بحث ثالثة | هذه ميزة تعود لـ Function 7 (`LabTestManagementView`). في `LabTestManagementView.xaml` سطر 272 يوجد الزر "المعدل الطبيعي" لكن لا توجد "خانة بحث ثالثة" مخصصة للعناصر الفرعية (`LabTestElement`) — الوظيفة تفتح المعدلات للتحليل المحدد فقط، لا لعناصره الفرعية | ❌ غير مطابق (يخص F7 لكن يؤثر على وصول F8) |
| 28 | عرض تلقائي للمعدل المتوافق مع نوع/مرحلة عمر المريض عند إدخال النتائج | يدمج تلقائياً في نافذة إدخال النتائج | `TestResultEntryService.SaveResultsAsync` يستخدم `INormalRangeService.GetMatchingRangeAsync` (قرار 16). يعمل عند الإدخال في Function 4 | ✅ مطابق (تكامل F4↔F8) |
| 29 | حقل `LabTest.Id` مرجعي في القائمة (T.ID عمود) | ظاهر في القائمة | `NormalRange.LabTestId` موجود في الـ Entity كـ FK، لكن **غير معروض** في `ListBox` بالـ UI (المُظهر يعرض Gender+Age+Text فقط) | ❌ غير مطابق (`T.ID` غير معروض) |
| 30 | عمود `P & S` (Print & Sensitive أو أي دلالة أخرى) | موجود في المرجع | لا مقابل له في `NormalRange.cs` — لا خاصية `IsPrintable` أو `IsSensitive` أو ما شابه | ❌ غير مطابق (دلالة `P & S` غير مطبَّقة) |
| 31 | Escape يغلق النافذة | متعارف عليه | `NormalRangeView.xaml` سطر 13: `<KeyBinding Key="Escape" Command="{Binding CancelCommand}" />` — يمسح الفورم فقط لا يغلق النافذة | ⚠️ جزئي (Escape → Cancel، لا يغلق النافذة) |

## 📉 قائمة النواقص للوظيفة 8 (موجود في المرجع، غير موجود في نظامي)

1. **[حرج] DataGrid بـ 11 عموداً**: قائمة المعدلات تعرض حالياً 4 عناصر فقط في `ListBox`. المطلوب: `DataGrid` يعرض الأعمدة الـ 11 التالية بترتيب المرجع:
   - `T.ID` (Test ID)
   - `Test Name`
   - `Sex`
   - `From` (AgeFrom)
   - `To` (AgeTo)
   - `Age unit`
   - `Normal Range`
   - `Low limit`
   - `High limit`
   - `Test unit`
   - `P & S`
2. **[حرج] عمود `P & S`**: لا خاصية مقابلة في `NormalRange.cs`. يجب تحديد دلالته (P = Print in report، S = Sensitive/Critical؟) وإضافة عمود Boolean/Enum مقابل.
3. **لوحة الرموز مربوطة بحقل واحد فقط**: `LatinSymbolsPad` مرتبطة بـ `NormalRangeTextBox` فقط عبر `TargetTextBox` binding — يجب أن تعمل مع كل حقول النصوص (Normal range, Critical range, Comments).
4. **مجموعة الرموز ناقصة**: تحتوي على 8 رموز يونانية/رياضية فقط، بدون **الأسس ¹²³⁴⁵⁶⁷⁸⁹⁰** التي يذكرها المرجع صراحةً ("الأسس والرموز").
5. **البحث بأول حرفين لتعديل عناصر تحليل رئيسي (LabTestElement)**: `LabTestManagementView` لا يحوي خانة بحث ثالثة تسمح باستدعاء عناصر البروفيل (CBC, الفيدال) للوصول لمعدلاتها الفرعية.
6. **تسهيل إدخال 6 معدلات دفعة واحدة**: لا Wizard/Template لتوليد 6 معدلات (Male/Female × 3 نطاقات عمرية) تلقائياً.

## 🔀 قائمة الاختلافات للوظيفة 8 (موجود لكن يعمل بشكل مختلف)

1. **`Escape` مربوط بـ `CancelCommand`** بدلاً من إغلاق النافذة (المرجع لا يذكر Escape صراحةً، لكن السلوك المتوقع هو الإغلاق).
2. **قائمة `Gender` في الـ VM** تُكشَف كخاصية `AvailableGenders` لكن الـ ComboBox في XAML يُملأ من الـ code-behind (`GenderCombo.ItemsSource = ...`) بدل الـ Binding — تداخل منطقي (Technical Note 4).
3. **مصفوفة الرموز**: نظامي يستخدم `{α, β, γ, μ, ±, ≤, ≥, °}` بدلاً من الأسس الرياضية `{¹, ², ³, ⁴, ⁵, ⁶, ⁷, ⁸, ⁹, ⁰}` المذكورة في المرجع.

## ➕ قائمة الإضافات في نظامي (غير موجودة في المرجع)

1. **حماية Admin على الحذف**: `NormalRangeService.DeleteAsync` سطر 49 يرفع `UnauthorizedAccessException` إذا لم يكن المستخدم Admin. المرجع لا يذكر هذا القيد.
2. **`FluentValidation` كامل** (`NormalRangeValidator`) يفرض: LowLimit ≤ HighLimit، AgeFrom ≤ AgeTo، CriticalLow ≤ Low، CriticalHigh ≥ High، Gender ∈ {Male, Female}.
3. **تحويل الوحدات العمرية** (`ConvertAgeToUnit` في `NormalRangeService`) — يسمح بمطابقة عمر بالسنة مع مدى بالأشهر أو الأيام.
4. **صف أوامر مكرر** (يمين النافذة + أسفلها) — تحسين UX غير موجود في المرجع.
5. **قسم "المعدلات الحرجة" منفصل بصرياً** في الفورم (سطر 180 من XAML) — تنظيم بصري إضافي.
6. **الفصل `IsAddMode` / `IsEditMode`** في الـ VM يسمح بتمييز حالة النافذة.

---

# 🛠️ الجزء الثالث: شرائح تنفيذية (Parts) لجعل النظام مطابقاً للمرجع

## 🟦 شرائح الوظيفة 2 (طباعة الباركود)

كل Part أدناه يجب أن ينتهي بحالة Build خضراء (0 errors, 0 warnings) قبل الانتقال للتالي. **لا يتم توليد كود** — الشرائح توصيفية فقط.

### Part F2-Fix.1 — تصنيف الملصقات حسب نوع العينة
**الملفات المتأثرة**:
- `Services/Implementations/BarcodeService.cs` (تعديل `GetLabelsForPatientAsync`)
- `Models/Domain/BarcodeLabel.cs` (استعمال `SpecimenTypeId` + `SpecimenName`)

**العمل المطلوب**:
1. في `GetLabelsForPatientAsync`، بدلاً من إنشاء ملصق واحد باسم "Default"، يجب الاستعلام عن `PatientTests` المرتبطة بآخر `PatientVisit`.
2. عمل `Join` مع `LabTest` لجلب `DefaultSpecimenTypeId`.
3. عمل `GroupBy(specimenTypeId)` وإنتاج `BarcodeLabel` واحد لكل مجموعة، حيث:
   - `SpecimenTypeId` = مفتاح المجموعة
   - `SpecimenName` = اسم `SpecimenType` من DB
   - `Tests` = قائمة أسماء التحاليل التابعة (`LabTest.TestName`)
   - `Code` = كود حالة (Case) للزيارة الحالية
4. الملصقات التي بلا `SpecimenType` تُجمَّع في ملصق "Other".

**Build Verification**: فتح نافذة مريض له تحليلان من عينات مختلفة → F11 → التحقق من ظهور ملصقين منفصلين مع أسماء عينتين مختلفتين.

---

### Part F2-Fix.2 — سلة محذوفات مرئية في UI
**الملفات المتأثرة**:
- `Views/Windows/BarcodeView.xaml` (إضافة `Border` سلة محذوفات)
- `Views/Windows/BarcodeView.xaml.cs` (Drop handler على السلة)
- `ViewModels/Pages/BarcodeViewModel.cs` (تفعيل `RemoveLabelCommand` عبر Drop)

**العمل المطلوب**:
1. إضافة صف جديد في `Grid.RowDefinitions` أو تحويل التخطيط لدعم منطقة سلة محذوفات (مثلاً في زاوية سفلية).
2. `Border` بأيقونة سلة (`materialDesign:PackIcon Kind="Delete"`) مع `AllowDrop="True"` و`Drop="TrashBin_Drop"`.
3. في `TrashBin_Drop`:
   - إذا كان الـ Data هو `BarcodeLabel` → استدعاء `RemoveLabelCommand` بالـ label.
   - إذا كان الـ Data هو `string` (اسم تحليل) → استدعاء أمر جديد `RemoveTestFromLabelCommand(label, testName)`.

**Build Verification**: سحب ملصق من القائمة إلى السلة → التأكد من اختفائه من `Labels` ObservableCollection.

---

### Part F2-Fix.3 — Drag & Drop على مستوى التحليل الفردي داخل الملصق
**الملفات المتأثرة**:
- `Views/Windows/BarcodeView.xaml` (تفعيل Drag على `TextBlock` كل تحليل داخل الـ ItemTemplate)
- `Views/Windows/BarcodeView.xaml.cs` (إضافة handlers للتحاليل)
- `ViewModels/Pages/BarcodeViewModel.cs` (أمر `MoveTestBetweenLabelsCommand(source, target, testName)`)

**العمل المطلوب**:
1. إضافة `PreviewMouseLeftButtonDown` على `TextBlock` كل تحليل داخل `ItemsControl.ItemTemplate` (سطر 84 من XAML الحالي).
2. عند السحب: `DragDrop.DoDragDrop(source, testName, DragDropEffects.Move);` مع تمرير مرجع لـ `BarcodeLabel` الأصل.
3. عند الإفلات على `Border` ملصق آخر (Label_Drop): التمييز بين إفلات `BarcodeLabel` (ترتيب) و `string` (نقل تحليل).
4. أمر جديد في VM: `MoveTestBetweenLabels(source, target, testName)` يزيل الاسم من `source.Tests` ويضيفه لـ `target.Tests`.

**Build Verification**: سحب اسم "ALT" من ملصق Serum إلى ملصق EDTA → التأكد من الانتقال في UI + إعادة توليد الباركود.

---

### Part F2-Fix.4 — إصلاح KeyBinding Escape
**الملفات المتأثرة**:
- `Views/Windows/BarcodeView.xaml` (سطر 12)

**العمل المطلوب**:
1. إزالة `<KeyBinding Key="Escape" Command="{Binding PrintAllCommand}" />` (خطأ برمجي).
2. استبدالها بأمر إغلاق النافذة: إما `CommandBinding.Executed` مع `ApplicationCommands.Close`، أو إضافة `CloseCommand` في VM يستدعي `Window.Close()` عبر `IDialogService`.

**Build Verification**: فتح النافذة → الضغط على Escape → التأكد من إغلاق النافذة (وليس طباعة الكل).

---

### Part F2-Fix.5 — الزر البرتقالي المتمايز بصرياً
**الملفات المتأثرة**:
- `Views/Windows/BarcodeView.xaml` (سطر 40–56)

**العمل المطلوب**:
1. إضافة زر ثالث بجوار "طباعة الكل" باسم "طباعة كلا الأكواد" بلون برتقالي (`Background="#FF9800"` أو `MaterialDesignRaisedAccentButton`).
2. ربطه بـ `Visibility="{Binding PrintFileCodeWithAll, Converter={StaticResource BoolToVisibilityConverter}}"` بحيث يظهر فقط عند تفعيل الـ Checkbox.
3. الزر ينفّذ نفس `PrintAllCommand` الحالي.

**Build Verification**: تفعيل الـ Checkbox "طباعة كود الملف مع الكل" → التأكد من ظهور الزر البرتقالي.

---

### Part F2-Fix.6 — دعم Enter/Arrow في خانتي الباركود الإضافي
**الملفات المتأثرة**:
- `Views/Windows/BarcodeView.xaml` (سطر 148–163)

**العمل المطلوب**:
1. إضافة `<KeyBinding Key="Enter" Command="{Binding FocusExtraDescriptionCommand}" />` على `TextBox` اسم التحليل الإضافي.
2. إضافة زر أسهم (`materialDesign:PackIcon Kind="ArrowDown"`) بين الحقلين ينقل الفوكس.
3. أمر جديد `FocusExtraDescriptionCommand` في VM يستدعي `_dialogService.FocusElement("ExtraDescriptionTextBox")`.

**Build Verification**: كتابة اسم في الخانة الأولى → Enter → التأكد من انتقال المؤشر للخانة الثانية.

---

### Part F2-Fix.7 — مراجعة صيغة الكود (اختياري)
**الملفات المتأثرة**:
- `Services/Implementations/BarcodeService.cs` (سطر 134–153)

**العمل المطلوب**:
> ⚠️ **قرار المالك مطلوب**: هل الخانة الأولى يجب أن تكون "لا يُنظر إليها" (ديناميكية) كما في المرجع، أم تبقى ثابتة "1"؟
- الإبقاء على الحال يفي بالمعنى الوظيفي (الرقم الثاني هو المميّز فعلياً).
- إذا قرر المالك المطابقة الحرفية: تغيير الخانة الأولى إلى `DateTime.Now.Second % 10` أو رقم عشوائي.

**Build Verification**: مقارنة كود مولَّد مع المثال المرجعي `1-4-100623-1-006-8`.

---

## 🟪 شرائح الوظيفة 8 (المعدلات الطبيعية)

### Part F8-Fix.1 — استبدال ListBox بـ DataGrid بـ 11 عموداً
**الملفات المتأثرة**:
- `Views/Windows/NormalRangeView.xaml` (سطر 52–73)
- `ViewModels/Pages/NormalRangeViewModel.cs` (خصائص عرض إضافية)
- `Models/Domain/NormalRange.cs` (إضافة خاصية `PrintAndSensitive`)

**العمل المطلوب**:
1. استبدال `ListBox Ranges` بـ `<DataGrid ItemsSource="{Binding Ranges}" SelectedItem="{Binding SelectedRange}" AutoGenerateColumns="False" IsReadOnly="True">`.
2. إضافة `<DataGrid.Columns>` بترتيب:
   | Header | Binding |
   |---|---|
   | `T.ID` | `LabTestId` |
   | `Test Name` | `TestName` |
   | `Sex` | `Gender` |
   | `From` | `AgeFrom` |
   | `To` | `AgeTo` |
   | `Age unit` | `AgeUnit` |
   | `Normal Range` | `NormalRangeText` |
   | `Low limit` | `LowLimit` |
   | `High limit` | `HighLimit` |
   | `Test unit` | `TestUnit` |
   | `P & S` | `PrintAndSensitive` (Boolean checkbox column) |
3. توسيع عرض العمود الأول (Column 0) في تخطيط النافذة من 220px إلى ~400–450px لاستيعاب الجدول.

**Build Verification**: فتح نافذة معدلات لتحليل به ≥ 2 مدى → التأكد من ظهور كل الأعمدة الـ 11 وقيمها.

---

### Part F8-Fix.2 — إضافة حقل `P & S` كخاصية في الـ Entity
**الملفات المتأثرة**:
- `Models/Domain/NormalRange.cs`
- `Models/Validation/NormalRangeValidator.cs`
- `Migrations/` (Migration جديد)
- `ViewModels/Pages/NormalRangeViewModel.cs`
- `Views/Windows/NormalRangeView.xaml` (إضافة CheckBox في الفورم)

**العمل المطلوب**:
1. **قرار المالك مطلوب** لتفسير `P & S`. الاحتمال الأرجح: `P = Print in report`, `S = Sensitive/Show as critical`.
2. إضافة خاصيتين Boolean في `NormalRange.cs`: `bool PrintInReport { get; set; }` + `bool ShowAsSensitive { get; set; }`.
3. أو خاصية مركبة: `bool PrintAndSensitive` (إن كانت دلالة واحدة).
4. إنشاء Migration: `AddPrintAndSensitiveToNormalRanges`.
5. إضافة `CheckBox Content="P & S"` في الفورم بعد قسم Critical.
6. تحديث الـ VM: `FormPrintAndSensitive` + Load/Build methods.

**Build Verification**: إنشاء مدى جديد مع تفعيل `P & S` → حفظ → إعادة فتح → التأكد من ظهور القيمة.

---

### Part F8-Fix.3 — ربط لوحة الرموز بكل حقول النصوص
**الملفات المتأثرة**:
- `Views/Controls/LatinSymbolsPad.xaml.cs` (تحديث للسماح بتتبع الفوكس)
- `Views/Windows/NormalRangeView.xaml` (تفعيل السلوك)

**العمل المطلوب**:
1. تعديل `LatinSymbolsPad` بحيث لا يعتمد على `TargetTextBox` ثابت.
2. إضافة سلوك يعتمد على `Keyboard.FocusedElement` — عند الضغط على رمز، يتم إدراجه في آخر TextBox حصل على Focus.
3. أو إضافة `AttachedProperty` `SymbolPad.EnableForTextBox="True"` على كل TextBox في النافذة.
4. إزالة الربط الحالي `TargetTextBox="{Binding ElementName=NormalRangeTextBox}"` (سطر 248).

**Build Verification**: فتح النافذة → التركيز على `FormCriticalRangeText` → الضغط على رمز `α` → التأكد من إدراجه في هذا الحقل (وليس في `NormalRangeTextBox`).

---

### Part F8-Fix.4 — إضافة الأسس الرياضية إلى لوحة الرموز
**الملفات المتأثرة**:
- `Views/Controls/LatinSymbolsPad.xaml.cs` (سطر 15)

**العمل المطلوب**:
1. توسيع القائمة الافتراضية:
   `{ "¹", "²", "³", "⁴", "⁵", "⁶", "⁷", "⁸", "⁹", "⁰", "α", "β", "γ", "μ", "±", "≤", "≥", "°", "×", "÷" }`
2. (اختياري) تجميعها في صفين: صف الأسس + صف الرموز اليونانية/الرياضية.
3. زيادة `WrapPanel` أو التحويل إلى `Grid` بصفين.

**Build Verification**: فتح النافذة → التأكد من ظهور الأسس (¹²³...) في اللوحة.

---

### Part F8-Fix.5 — Wizard لإدخال 6 معدلات دفعة واحدة
**الملفات المتأثرة**:
- `ViewModels/Pages/NormalRangeViewModel.cs` (أمر جديد `AddSixRangesWizardCommand`)
- `Views/Windows/NormalRangeView.xaml` (زر جديد "6 معدلات")

**العمل المطلوب**:
1. أمر `AddSixRangesWizard()` يفتح ديالوج صغير يطلب:
   - القيم المشتركة: `TestUnit`, `NormalRangeText`, `LowLimit`, `HighLimit`, `LowFlag`, `HighFlag`, `LowComment`, `HighComment`.
2. عند "موافق": يُنشئ 6 كيانات `NormalRange` بالنطاقات الست القياسية:
   - `(Male, 0, 120, Year)`
   - `(Female, 0, 120, Year)`
   - `(Male, 1, 29, Day)`
   - `(Female, 1, 29, Day)`
   - `(Male, 1, 11, Month)`
   - `(Female, 1, 11, Month)`
3. يستدعي `_normalRangeService.AddAsync` لكل واحد داخل Transaction.
4. زر في UI: "6 معدلات (تحليل لا يعتمد على الجنس/السن)".

**Build Verification**: تحليل بلا فرق (مثل السكر) → الضغط على "6 معدلات" → إدخال قيم مشتركة → التأكد من إنشاء 6 صفوف في `NormalRanges`.

---

### Part F8-Fix.6 — إصلاح KeyBinding Escape
**الملفات المتأثرة**:
- `Views/Windows/NormalRangeView.xaml` (سطر 13)

**العمل المطلوب**:
1. تغيير `<KeyBinding Key="Escape" Command="{Binding CancelCommand}" />` إلى ربط بأمر إغلاق النافذة، أو ترك `Cancel` فقط في حالة `IsEditMode`، وإغلاق النافذة في حالة العرض.
2. إن كان القرار الاحتفاظ بـ Cancel فقط: تعديل `CancelCommand` بحيث يستدعي `_dialogService.CloseCurrentDialog()` عندما لا يوجد تعديل جار.

**Build Verification**: فتح النافذة بدون تعديل → Escape → التأكد من الإغلاق. فتح النافذة أثناء تعديل → Escape → التأكد من إلغاء التعديل فقط.

---

### Part F8-Fix.7 — خانة بحث ثالثة في Function 7 لعناصر التحاليل الرئيسية
**الملفات المتأثرة (خارج نطاق F8 مباشرة لكن مؤثر عليها)**:
- `Views/Pages/LabTestManagementView.xaml`
- `ViewModels/Pages/LabTestManagementViewModel.cs`
- `Services/Implementations/LabTestService.cs`

**العمل المطلوب**:
1. إضافة `TextBox` ثالث في نافذة `LabTestManagementView` مع hint "بحث في عناصر التحليل (LabTestElement)".
2. عند إدخال أول حرفين، عرض قائمة `LabTestElement` الفرعية عبر `LabTestService.SearchElementsAsync(prefix)`.
3. عند اختيار عنصر → تعيينه كـ `SelectedTest` (مؤقتاً) → السماح بفتح نافذة المعدلات الطبيعية لهذا العنصر الفرعي.

**Build Verification**: كتابة "MC" (لعنصر MCV في CBC) → التأكد من ظهوره في القائمة → اختياره → فتح المعدل الطبيعي → التأكد من ظهور معدلات هذا العنصر.

---

### Part F8-Fix.8 — تنظيف الـ VM (استخدام Binding بدل code-behind)
**الملفات المتأثرة**:
- `Views/Windows/NormalRangeView.xaml.cs` (سطر 12–13)
- `Views/Windows/NormalRangeView.xaml` (سطر 92–102)

**العمل المطلوب**:
1. إزالة تعيين `GenderCombo.ItemsSource` من الـ code-behind.
2. تفعيل `ItemsSource="{Binding AvailableGenders}"` مباشرة في XAML.
3. مثل ذلك لـ `AgeUnitCombo` عبر `ItemsSource="{Binding AvailableAgeUnits}"`.
4. الاحتفاظ بـ Technical Note 4 كتوثيق فقط (المشكلة كانت `x:Array` وليس Binding عادي).

**Build Verification**: البناء بدون errors + فتح النافذة والتأكد من امتلاء الـ ComboBoxes بنفس القيم.

---

# 📌 ملخص إحصائي

## الوظيفة 2 (طباعة الباركود)
- **إجمالي عناصر المقارنة**: 24
- ✅ **مطابق**: 15
- ⚠️ **جزئي**: 5
- ❌ **غير مطابق**: 3
- ➕ **إضافات إيجابية**: 5
- **شرائح التنفيذ المطلوبة**: 7 Parts

## الوظيفة 8 (المعدلات الطبيعية)
- **إجمالي عناصر المقارنة**: 31
- ✅ **مطابق**: 20
- ⚠️ **جزئي**: 4
- ❌ **غير مطابق**: 4
- ➕ **إضافات إيجابية**: 6
- **شرائح التنفيذ المطلوبة**: 8 Parts

## القرارات الملزمة (Compliance Check)
| القرار | التطبيق في الكود | الحالة |
|---|---|---|
| قرار 3 (ZXing.Net حصراً) | `BarcodeImageGenerator.cs` يستخدم `ZXing` فقط | ✅ ملتزم |
| قرار 4 (38×25 مم قابل للتخصيص) | `BarcodeSettings` + TextBoxes UI | ✅ ملتزم |
| قرار 5 (لا BranchNumber، ثابت "1") | `BarcodeService.BranchConstant = "1"` | ✅ ملتزم |
| قرار 14 (LatinSymbolsPad قابل للتوسعة) | `LatinSymbolsPad` UserControl موجود | ⚠️ ملتزم جزئياً (يحتاج توسعة الرموز والربط بكل الحقول) |
| قرار 16 (أضيق مدى يفوز) | `NormalRangeService.GetMatchingRangeAsync` | ✅ ملتزم |
| قرار 17 (Male/Female فقط) | `Gender` enum + Validator + VM | ✅ ملتزم |

---

# 🎯 الترتيب المقترح لتنفيذ الشرائح

**المرحلة 1 (نواقص حرجة):**
1. Part F2-Fix.1 (تصنيف الملصقات حسب العينة) — أعلى أولوية للوظيفة 2
2. Part F8-Fix.1 (DataGrid بـ 11 عموداً) — أعلى أولوية للوظيفة 8
3. Part F8-Fix.2 (خاصية `P & S`)

**المرحلة 2 (نواقص جوهرية):**
4. Part F2-Fix.2 (سلة محذوفات مرئية)
5. Part F2-Fix.3 (Drag & Drop على مستوى التحليل)
6. Part F8-Fix.3 (ربط لوحة الرموز بكل الحقول)

**المرحلة 3 (إصلاحات وتحسينات):**
7. Part F2-Fix.4 (Escape)
8. Part F2-Fix.5 (زر برتقالي)
9. Part F2-Fix.6 (Enter/Arrow)
10. Part F8-Fix.4 (توسيع الرموز)
11. Part F8-Fix.5 (Wizard الـ 6 معدلات)
12. Part F8-Fix.6 (Escape للمعدلات)
13. Part F8-Fix.8 (تنظيف VM Binding)

**المرحلة 4 (تكامل خارجي):**
14. Part F8-Fix.7 (خانة بحث ثالثة في F7)

**المرحلة 5 (اختياري بقرار المالك):**
15. Part F2-Fix.7 (مراجعة صيغة الكود الأولى)

---

## 📎 مصادر التحقق

- **المصدر الأول (أعلى موثوقية)**: `Real_Lab_System_Reference.pdf`، الصفحات 18–20 (F2) و 41–44 (F8) — تم استخراجها حرفياً عبر `summarize_large_document`.
- **المصدر الثاني**: `Reference_Functions_2_and_8_Detailed_Analysis.md` — تحليل بنيوي منظم.
- **الكود المصدري الفعلي**: مأخوذ من الـ Commit `56968b429f7f49485b15e065bc7d23516ac626c8` (تم التحقق).
- **`Docs/history.md`**: أُخذت منه أقسام Phase 7 (F8) وPhase 8 (F2) فقط — سطور 736–878.
- **`Docs/analysis_and_plan_v3.md`**: أُخذت منه أقسام Function 8 (سطور 304–384) وFunction 2 (سطور 385–470) فقط + الجدول الجامع للقرارات (سطور 71–85).

---

**نهاية التقرير**
