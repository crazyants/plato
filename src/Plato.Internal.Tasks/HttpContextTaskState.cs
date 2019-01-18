using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    public class HttpContextTaskState : IDeferredTaskState
    {
        private static readonly object Key = typeof(HttpContextTaskState);
        private readonly HttpContext _httpContext;

        public HttpContextTaskState(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public IList<Func<DeferredTaskContext, Task>> Tasks
        {
            get
            {
                if (!_httpContext.Items.TryGetValue(Key, out var tasks))
                {
                    tasks = new List<Func<DeferredTaskContext, Task>>();
                    _httpContext.Items[Key] = tasks;
                }

                return (IList<Func<DeferredTaskContext, Task>>)tasks;
            }
        }
    }

}
