using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.TopParticipants.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {             
        
        public override Task<IViewProviderResult> BuildIndexAsync(Topic entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Topic entity, IViewProviderContext context)
        {
            return Task.FromResult(Views(
                View<Topic>("Topic.Participants.Display.Sidebar", model => entity).Zone("sidebar").Order(5)
            ));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Topic entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
               
        public override Task<IViewProviderResult> BuildUpdateAsync(Topic entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }

}
