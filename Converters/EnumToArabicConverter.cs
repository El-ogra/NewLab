using System;
using System.Globalization;
using System.Windows.Data;
using NewLab.Models.Domain.Enums;

namespace NewLab.Converters
{
    public class EnumToArabicConverter : IValueConverter
    {
        public static readonly EnumToArabicConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Gender gender)
                return gender switch
                {
                    Gender.Male => "ذكر",
                    Gender.Female => "أنثى",
                    _ => value.ToString()!
                };

            if (value is AgeUnit ageUnit)
                return ageUnit switch
                {
                    AgeUnit.Day => "يوم",
                    AgeUnit.Month => "شهر",
                    AgeUnit.Year => "سنة",
                    _ => value.ToString()!
                };

            if (value is BillingSystem billing)
                return billing switch
                {
                    BillingSystem.Individual => "فردي",
                    BillingSystem.LabToLab => "معمل لمعمل",
                    BillingSystem.Free => "مجاناً",
                    _ => value.ToString()!
                };

            if (value is TestStatus status)
                return status switch
                {
                    TestStatus.New => "جديد",
                    TestStatus.Entered => "مُدخل",
                    TestStatus.Reviewed => "مُراجَع",
                    TestStatus.Printed => "مطبوع",
                    TestStatus.Delivered => "مُسلَّم",
                    TestStatus.AccountIssue => "باقي حساب",
                    TestStatus.Completed => "مكتمل",
                    _ => value.ToString()!
                };

            if (value is PaymentType paymentType)
                return paymentType switch
                {
                    PaymentType.Payment => "دفعة",
                    PaymentType.Refund => "استرداد",
                    PaymentType.Delivery => "تسليم",
                    _ => value.ToString()!
                };

            if (value is PatientListFilter patientFilter)
                return patientFilter switch
                {
                    PatientListFilter.All => "الكل",
                    PatientListFilter.Unwritten => "غير مكتوب",
                    PatientListFilter.Unreviewed => "غير مُراجَع",
                    PatientListFilter.Unprinted => "غير مطبوع",
                    PatientListFilter.Important => "مهم",
                    PatientListFilter.Individual => "فردي",
                    PatientListFilter.LabToLab => "معمل لمعمل",
                    PatientListFilter.Referral => "إحالة",
                    _ => value.ToString()!
                };

            if (value is DeliveryFilterMode deliveryFilter)
                return deliveryFilter switch
                {
                    DeliveryFilterMode.Undelivered => "غير مسلَّم",
                    DeliveryFilterMode.All => "الكل",
                    DeliveryFilterMode.LabToLab => "معمل لمعمل",
                    DeliveryFilterMode.Individual => "فردي",
                    DeliveryFilterMode.Important => "مهم",
                    DeliveryFilterMode.CurrentUser => "حسب المستخدم",
                    _ => value.ToString()!
                };

            if (value is TestListMode testListMode)
                return testListMode switch
                {
                    TestListMode.Routine => "روتيني",
                    TestListMode.All => "الكل",
                    TestListMode.Groups => "مجموعات",
                    TestListMode.CustomGroups => "مجموعات مخصصة",
                    _ => value.ToString()!
                };

            return value.ToString()!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("EnumToArabicConverter does not support ConvertBack.");
        }
    }
}
