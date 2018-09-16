using System.Threading.Tasks;
using Plato.Internal.Layout.ViewAdaptors;

namespace Plato.Markdown.ViewAdaptors
{
    public class EditorViewAdaptorProvider : BaseAdaptorProvider
    {

        public override Task<IViewAdaptorResult> ConfigureAsync()
        {
            return Adapt("Editor", v =>
            {
                v.AdaptView("Markdown");
                //v.AdaptModel<MarkdownViewModel>(model =>
                //{
                //    var markDownViewModel = new MarkdownViewModel();
                //    markDownViewModel.Value = model.Value;
                //    return markDownViewModel;
                //});
            });


        }
    }
}
