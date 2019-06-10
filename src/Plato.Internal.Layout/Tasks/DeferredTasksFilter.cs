using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Layout.Tasks
{
    //public class DeferredTasksFilter : IActionFilter, IAsyncResultFilter
    //{

    //    private readonly IDeferredTaskStore _deferredTaskManager;
    //    private readonly IRunningShellTable _runningShellTable;
    //    private readonly IPlatoHost _platoHost;

    //    public DeferredTasksFilter(
    //        IDeferredTaskStore deferredTaskManager,
    //        IPlatoHost platoHost,
    //        IRunningShellTable runningShellTable)
    //    {
    //        _deferredTaskManager = deferredTaskManager;
    //        _platoHost = platoHost;
    //        _runningShellTable = runningShellTable;
    //    }

    //    public void OnActionExecuting(ActionExecutingContext context)
    //    {
    //        return;
    //    }

    //    public void OnActionExecuted(ActionExecutedContext context)
    //    {
    //        return;
    //    }

    //    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    //    {

    //        // Process everything else first
    //        await next();

    //        // The controller action didn't return a view result so no need to continue execution
    //        if (!(context.Result is ViewResult result))
    //        {
    //            return;
    //        }

    //        // Check early to ensure we are working with a LayoutViewModel
    //        if (!(result?.Model is LayoutViewModel model))
    //        {
    //            return;
    //        }

    //        //Get ShellSettings for current tennet
    //       var shellSettings = _runningShellTable.Match(context.HttpContext);

    //        // Create a new scope only if there are pending tasks
    //        if (_deferredTaskManager.Process(context.HttpContext)) 
    //        {
    //            var shellContext = _platoHost.GetOrCreateShellContext(shellSettings);
    //            using (var pendingScope = shellContext.CreateServiceScope())
    //            {
    //                if (pendingScope != null)
    //                {
    //                    var deferredTaskContext = new DeferredTaskContext(pendingScope.ServiceProvider);
    //                    await _deferredTaskManager.ExecuteTaskAsync(deferredTaskContext);
    //                }
    //            }
    //        }

    //    }
    //}
}
