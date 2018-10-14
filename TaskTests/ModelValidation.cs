using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TaskTests.Interface;

namespace TaskTests
{
    public class ModelValidation : IModelValidation
    {
        public IEnumerable<ValidationResult> ValidateModels<T>(T model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
    }
}
