using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Features
{
    public interface IFeatureSettings : ISerializable
    {
        string Title { get; set; }

        string Description { get; set; }

        string IconCss { get; set; }

    }

}
