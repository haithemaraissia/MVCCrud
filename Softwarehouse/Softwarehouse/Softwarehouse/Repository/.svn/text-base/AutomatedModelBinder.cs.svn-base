using System.Reflection;
using System.Web.Mvc;
using System;

namespace Softwarehouse.MvcCrud.Repository
{
    public abstract class AutomatedModelBinder<TModel> : IModelBinder where TModel : new()
    {
        protected string primaryKey = "id";
        protected TModel model;
        protected ControllerContext controllerContext;
        #region IModelBinder Members
        public object BindModel(ControllerContext the_controllerContext, ModelBindingContext bindingContext)
        {
            model = new TModel();
            controllerContext = the_controllerContext;

            if (!string.IsNullOrEmpty(controllerContext.HttpContext.Request[primaryKey]))
            {
                PropertyInfo p = typeof(TModel).GetProperty(primaryKey);
                p.SetValue(model, int.Parse(controllerContext.HttpContext.Request[primaryKey]), null);
            }

            ExtraBindings();

            return model;
        }

        public abstract void ExtraBindings();

        public void parseFromForm(string propertyName, string errorMessage)
        {
            parseFromForm(propertyName, propertyName, errorMessage, true);
        }

        public void parseFromForm(string propertyName, string errorMessage, bool canBeEmpty)
        {
            parseFromForm(propertyName, propertyName, errorMessage, canBeEmpty);
        }

        public void parseFromForm(string propertyName, string formName, string errorMessage, bool canBeEmpty)
        {
            try
            {
                PropertyInfo p = typeof(TModel).GetProperty(propertyName);
                Type type = p.PropertyType;
                if(type == typeof(string))
                {
                    if (!canBeEmpty && string.IsNullOrEmpty(controllerContext.HttpContext.Request[formName]))
                    {
                        throw new Exception();
                    }

                    p.SetValue(model, controllerContext.HttpContext.Request[formName], null);
                }
                else
                {
                    //Convert from nullable to normal eg int? -> int
                    if (Nullable.GetUnderlyingType(type) != null)
                    {
                        if (String.IsNullOrEmpty(controllerContext.HttpContext.Request[formName]) && canBeEmpty)
                        {
                            p.SetValue(model, null, null);
                            return;
                        }

                        type = Nullable.GetUnderlyingType(type);
                    }

                    //Get the parse method
                    MethodInfo parse = type.GetMethod("Parse", new [] { typeof(string) });
                    
                    p.SetValue(model,  parse.Invoke(null, new object[] { controllerContext.HttpContext.Request[formName] }), null);
                    
                }
            }
            catch (Exception)
            {
                controllerContext.Controller.ViewData.ModelState.AddModelError(formName, errorMessage);
                controllerContext.Controller.ViewData.ModelState.SetModelValue(formName, new ValueProviderResult(controllerContext.HttpContext.Request[formName], controllerContext.HttpContext.Request[formName], null));
            }
        }

        #endregion
    }
}
