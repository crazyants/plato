
namespace Plato.Internal.Models
{
    public interface ILabelBase
    {

        int Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Alias { get; set; }

        string IconCss { get; set; }

        string ForeColor { get; set; }

        string BackColor { get; set; }
        
    }

}
