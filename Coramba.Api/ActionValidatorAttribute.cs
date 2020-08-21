using System;
using System.Linq;

namespace Coramba.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ActionValidatorAttribute: Attribute
    {
        public Type[] ValidatorTypes { get; }

        public ActionValidatorAttribute(params Type[] validatorTypes)
        {
            if (validatorTypes == null) throw new ArgumentNullException(nameof(validatorTypes));
            if (validatorTypes.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(validatorTypes));

            if (validatorTypes.Any(t => !typeof(IActionValidator).IsAssignableFrom(t)))
                throw new Exception($"ValidatorType must implements {typeof(IActionValidator)}");

            ValidatorTypes = validatorTypes;
        }
    }
}
