using System;
using System.Collections.Generic;
using System.Text;

namespace JeuDePoints.Domain.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = "";

        public static ValidationResult Ok() => new() { IsValid = true };
        public static ValidationResult Fail(string message) => new() { IsValid = false, Message = message };
    }
}
