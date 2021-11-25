using System.Web.Mvc;

namespace protean.Infrastructure
{
    /// <summary>
    /// Trim all string inputs
    /// </summary>
    public class TrimModelBinder : IModelBinder
    {
        /// <summary>
        /// BindModel to trim all inputs
        /// </summary>
        /// <param name="controllerContext">ControllerContext</param>
        /// <param name="bindingContext">ModelBindingContext</param>
        /// <returns></returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult == null || valueResult.AttemptedValue == null)
            {
                return null;
            }
            else if (valueResult.AttemptedValue == string.Empty)
            {
                return string.Empty;
            }
            return valueResult.AttemptedValue.Trim();
        }
    }
}