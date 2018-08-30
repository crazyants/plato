using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewAdaptors;

namespace Plato.Discuss.Channels.ViewAdaptors
{
    public class DiscussViewAdaptorProvider : BaseAdaptorProvider
    {

        public override Task<IViewAdaptorResult> ConfigureAsync()
        {

            return Task.FromResult((IViewAdaptorResult) default(IViewAdaptorResult));

            //return Adapt("Home.Index.Header", v =>
            //{
            //    v.AdaptView("Discuss.Index.Header");
            //    //v.AdaptModel<MarkdownViewModel>(model =>
            //    //{
            //    //    var markDownViewModel = new MarkdownViewModel();
            //    //    markDownViewModel.Value = model.Value;
            //    //    return markDownViewModel;
            //    //});
            //});


        }

    }
}
