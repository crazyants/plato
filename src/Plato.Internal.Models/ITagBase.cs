namespace Plato.Internal.Models
{

    public interface ITagBase
    {
        int Id { get; set; }
        
        string Name { get; set; }
        
        string Description { get; set; }
        
        string Alias { get; set; }


    }
    
}
