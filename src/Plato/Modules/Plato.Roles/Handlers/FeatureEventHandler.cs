//using System.Collections.Generic;
//using System.Data;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using Plato.Internal.Data.Schemas.Abstractions;
//using Plato.Internal.Features.Abstractions;
//using Plato.Roles.Services;

//namespace Plato.Roles.Handlers
//{
//    public class FeatureEventHandler : BaseFeatureEventHandler
//    {

//        public string Version { get; } = "1.0.0";
        
//        private readonly IDefaultRolesManager _defaultRolesManager;

//        public FeatureEventHandler(IDefaultRolesManager defaultRolesManager)
//        {
//            _defaultRolesManager = defaultRolesManager;
//        }

//        #region "Implementation"

//        public override Task InstallingAsync(IFeatureEventContext context)
//        {
//            // table are installed during set-up
//            return Task.CompletedTask;
//        }

//        public override async Task InstalledAsync(IFeatureEventContext context)
//        {

//            // Add default roles for features when they are added
//            await _defaultRolesManager.InstallDefaultRolesAsync();

//        }

//        public override Task UninstallingAsync(IFeatureEventContext context)
//        {
//            return Task.CompletedTask;
//        }

//        public override Task UninstalledAsync(IFeatureEventContext context)
//        {
//            return Task.CompletedTask;
//        }

//        #endregion
        
//    }
//}
