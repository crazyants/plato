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

        public int Order => 1000;

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {

            // This code is called only once per tenant during the construction of routes
            foreach (var controller in context.Result.Controllers)
            {
                var controllerType = controller.ControllerType.AsType();
                var module = _typedModuleProvider.GetModuleForDependency(controllerType)
                    .GetAwaiter()
                    .GetResult();
                if (module != null)
                {
                    controller.RouteValues.Add("area", module.Descriptor.Id);
                }
            }
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
        }

    }

}
