using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Hosting.Web
{
    public class ModuleApplicationModelProvider : IApplicationModelProvider
    {

        private readonly ITypedModuleProvider _typedModuleProvider;

        public ModuleApplicationModelProvider(
            ITypedModuleProvider typedModuleProvider)
        {
            _typedModuleProvider = typedModuleProvider;
        }

        // Ensure DefaultApplicationModelProvider executes first
        // https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/Mvc.Core/src/ApplicationModels/DefaultApplicationModelProvider.cs
        public int Order => 995;

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {

            foreach (var controllerType in context.ControllerTypes)
            {
                var test = "test";
            }

        }

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {

            // This code is called only once per tenant during the construction of routes
            foreach (var controller in context.Result.Controllers)
            {
                var controllerType = controller.ControllerType.AsType();

                // TODO: Can we remove GetAwaiter here?
                var module = _typedModuleProvider.GetModuleForDependency(controllerType)
                    .GetAwaiter()
                    .GetResult();
                if (module != null)
                {
                    if (!controller.RouteValues.ContainsKey("area"))
                    {
                        controller.RouteValues.Add("area", module.Descriptor.Id);
                    }                    
                }
            }
        }
        
    }

}
