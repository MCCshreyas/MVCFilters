/// <summary>
/// Validates the ModelState of given action method. 
/// <remarks>
/// Only apply this attribute on HttpPost controller action method, otherwise it throws <see cref="NotSupportedException"/> exception.
/// </remarks>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ValidateModelState : ActionFilterAttribute
{
        public string IgnoreProperties { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod == "POST")
            {
                var currentModelState = filterContext.Controller.ViewData.ModelState;

                if(IgnoreProperties != null)
                {
                    var propertiesToIgnore = IgnoreProperties.Split(',');

                    foreach(var key in propertiesToIgnore)
                    {
                        if(currentModelState.ContainsKey(key))
                        {
                            currentModelState.Remove(key);
                        }
                    }
                }

                if (!currentModelState.IsValid)
                {

                    string errorMessages = string.Join(", ", currentModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, errorMessages);
                }
            }
            else
            {
                throw new NotSupportedException("Please use ValidateModelState Attribute only for POST Action to validate the posted data. You have applied this attribute on " + filterContext.ActionDescriptor.ActionName + " action.");
            }

            base.OnActionExecuting(filterContext);
        }
    }
