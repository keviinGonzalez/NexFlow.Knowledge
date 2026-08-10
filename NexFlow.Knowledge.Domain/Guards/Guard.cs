using NexFlow.Knowledge.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Domain.Guards
{
    public static class Guard
    {
        public static void ValidateRequired(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new DomainException($"{parameterName} es requerido.");
            }
        }

        public static void ValidateRequired(string? value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException($"{parameterName} es requerido.");
            }
        }

        public static void ValidateRequired<T>(T? value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new DomainException($"{parameterName} es requerido.");
            }
        }

        public static void ValidateMaxLength(string? value, int maxLength, string parameterName)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                throw new DomainException($"{parameterName} no puede exceder {maxLength} caracteres.");
            }
        }
    }
}
