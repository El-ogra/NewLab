using System;

namespace NewLab.Models.Domain
{
    public class FunctionDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public Type? TargetViewType { get; set; }
    }
}