

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorResult
    {
        string ViewName { get; set; }

        IViewAdaptorBuilder AdaptorBuilder { get; set; }

    }

    public class ViewAdaptorResult : IViewAdaptorResult
    {
        public string ViewName { get; set; }

        public IViewAdaptorBuilder AdaptorBuilder { get; set; }


    }
}
