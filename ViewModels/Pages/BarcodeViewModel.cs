using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;

namespace NewLab.ViewModels.Pages
{
    public partial class BarcodeViewModel : ObservableObject
    {
        private readonly IBarcodeService _barcodeService;
        private readonly IBarcodePrintService _printService;
        private readonly IPatientService _patientService;
        private readonly IDialogService _dialogService;

        [ObservableProperty] private Patient? patient;
        public ObservableCollection<BarcodeLabel> Labels { get; } = new();

        [ObservableProperty] private int offsetX;
        [ObservableProperty] private int offsetY;
        [ObservableProperty] private bool printFileCodeWithAll;
        [ObservableProperty] private int labelWidth = 38;
        [ObservableProperty] private int labelHeight = 25;
        [ObservableProperty] private string? extraBarcodeName;
        [ObservableProperty] private string? extraBarcodeDescription;
        [ObservableProperty] private BarcodeLabel? selectedLabel;

        public BarcodeViewModel(
            IBarcodeService barcodeService,
            IBarcodePrintService printService,
            IPatientService patientService,
            IDialogService dialogService)
        {
            _barcodeService = barcodeService;
            _printService = printService;
            _patientService = patientService;
            _dialogService = dialogService;
        }

        public async Task LoadForPatientAsync(Patient p)
        {
            Patient = p;
            await LoadLabelsAsync();
            await LoadSettingsAsync();
        }

        [RelayCommand]
        private async Task PrintFileCodeAsync()
        {
            if (Patient == null) return;

            var code = _barcodeService.GenerateFileCode(Patient);
            var labels = new List<BarcodeLabel>
            {
                new BarcodeLabel
                {
                    PatientId = Patient.Id,
                    PatientName = Patient.FullName,
                    SpecimenName = "كود الملف",
                    Code = code,
                    Type = Models.Domain.Enums.CodeType.File
                }
            };

            await PrintLabelsAsync(labels);
        }

        [RelayCommand(CanExecute = nameof(CanPrintLabCode))]
        private async Task PrintLabCodeAsync()
        {
            if (Patient == null) return;

            var code = _barcodeService.GenerateLabCode(Patient);
            var labels = new List<BarcodeLabel>
            {
                new BarcodeLabel
                {
                    PatientId = Patient.Id,
                    PatientName = Patient.FullName,
                    SpecimenName = "كود المعمل",
                    Code = code,
                    Type = Models.Domain.Enums.CodeType.Lab
                }
            };

            await PrintLabelsAsync(labels);
        }

        private bool CanPrintLabCode() => Patient?.LabId != null;

        [RelayCommand]
        private async Task PrintLabelAsync(BarcodeLabel? label)
        {
            if (label == null) return;
            await PrintLabelsAsync(new List<BarcodeLabel> { label });
        }

        [RelayCommand]
        private async Task PrintAllAsync()
        {
            if (Patient == null) return;

            var allLabels = Labels.ToList();

            if (PrintFileCodeWithAll)
            {
                var fileCode = _barcodeService.GenerateFileCode(Patient);
                allLabels.Insert(0, new BarcodeLabel
                {
                    PatientId = Patient.Id,
                    PatientName = Patient.FullName,
                    SpecimenName = "كود الملف",
                    Code = fileCode,
                    Type = Models.Domain.Enums.CodeType.File
                });
            }

            await PrintLabelsAsync(allLabels);
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            var settings = new BarcodeSettings
            {
                OffsetX = OffsetX,
                OffsetY = OffsetY,
                PrintFileCodeWithAll = PrintFileCodeWithAll,
                LabelWidth = LabelWidth,
                LabelHeight = LabelHeight
            };

            await _barcodeService.SaveSettingsAsync(settings);
            _dialogService.ShowMessage("نجاح", "تم حفظ الإعدادات بنجاح");
        }

        [RelayCommand]
        private void AddExtraBarcode()
        {
            if (string.IsNullOrWhiteSpace(ExtraBarcodeName))
            {
                _dialogService.ShowMessage("خطأ", "يرجى إدخال اسم التحليل");
                return;
            }

            var label = new BarcodeLabel
            {
                PatientId = Patient?.Id ?? 0,
                PatientName = Patient?.FullName ?? string.Empty,
                SpecimenName = ExtraBarcodeName,
                Code = "",
                Type = Models.Domain.Enums.CodeType.Case
            };

            if (!string.IsNullOrWhiteSpace(ExtraBarcodeDescription))
            {
                label.Tests.Add(ExtraBarcodeDescription);
            }

            Labels.Add(label);
            ExtraBarcodeName = null;
            ExtraBarcodeDescription = null;
        }

        [RelayCommand]
        private void RemoveLabel(BarcodeLabel? label)
        {
            if (label != null)
                Labels.Remove(label);
        }

        private async Task LoadLabelsAsync()
        {
            Labels.Clear();
            if (Patient == null) return;

            var labels = await _barcodeService.GetLabelsForPatientAsync(Patient.Id);
            foreach (var l in labels)
                Labels.Add(l);
        }

        private async Task LoadSettingsAsync()
        {
            var settings = await _barcodeService.GetSettingsAsync();
            OffsetX = settings.OffsetX;
            OffsetY = settings.OffsetY;
            PrintFileCodeWithAll = settings.PrintFileCodeWithAll;
            LabelWidth = settings.LabelWidth;
            LabelHeight = settings.LabelHeight;
        }

        private async Task PrintLabelsAsync(List<BarcodeLabel> labels)
        {
            if (labels == null || !labels.Any())
            {
                _dialogService.ShowMessage("خطأ", "لا توجد ملصقات للطباعة");
                return;
            }

            try
            {
                var settings = new BarcodeSettings
                {
                    LabelWidth = LabelWidth,
                    LabelHeight = LabelHeight
                };

                var pdfBytes = _printService.GeneratePdf(labels, settings);
                var filePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"barcode_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
                _dialogService.ShowMessage("نجاح", $"تم حفظ ملف PDF في:\n{filePath}");
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("خطأ", "حدث خطأ أثناء الطباعة: " + ex.Message);
            }
        }
    }
}
