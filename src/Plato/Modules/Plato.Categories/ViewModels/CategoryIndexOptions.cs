using System.Runtime.Serialization;
using Plato.Internal.Data.Abstractions;

namespace Plato.Categories.ViewModels
{
    public class CategoryIndexOptions
    {

        [DataMember(Name = "featureId")]
        public int FeatureId { get; set; }

        [DataMember(Name = "categoryId")]
        public int CategoryId { get; set; }

        [DataMember(Name = "sort")]
        public SortBy Sort { get; set; } = SortBy.Auto;

        [DataMember(Name = "order")]
        public OrderBy Order { get; set; } = OrderBy.Desc;


    }
    public enum SortBy
    {
        Auto = 0,
        Name = 1,
        Description = 2,
        Entities = 3,
        Replies = 4,
        SortOrder = 5
    }

}
