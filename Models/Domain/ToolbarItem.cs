using System.Collections.Generic;
using MaterialDesignThemes.Wpf;

namespace NewLab.Models.Domain
{
    public class ToolbarItem
    {
        public PackIconKind IconKind { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<FunctionDefinition> Functions { get; set; } = new();
    }
}