using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Features;
using Plato.Internal.Features.Abstractions;

namespace Plato.Articles.Drafts.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
               

        public FeatureEventHandler()
        {
     
        }
        
        public override Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task UninstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }
        
    }

}
