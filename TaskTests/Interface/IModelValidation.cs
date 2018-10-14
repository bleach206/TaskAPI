using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskTests.Interface
{
    public interface IModelValidation
    {
        IEnumerable<ValidationResult> ValidateModels<T>(T model);
    }
}
