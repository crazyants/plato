namespace Plato.Internal.Data.Schemas.Abstractions
{
    public class SchemaBuilderOptions
    {

        public string Version { get; set; } = "1.0.0";

        public string ModuleName { get; set; }

        public bool DropTablesBeforeCreate { get; set; }

        public bool DropProceduresBeforeCreate { get; set; } = true;
        
        public bool DropCatalogBeforeCreate { get; set; } 

        public bool DropIndexesBeforeCreate { get; set; } 
        
        public bool CheckColumnExistsBeforeCreate { get; set; } = true;

    }
}
