namespace Plato.Internal.Data.Schemas.Abstractions
{
    public class SchemaBuilderOptions
    {

        public string Version { get; set; } = "1.0.0";

        public string ModuleName { get; set; }

        public bool DropTablesBeforeCreate { get; set; } = false;

        public bool DropProceduresBeforeCreate { get; set; } = false;
        
        public bool DropCatalogBeforeCreate { get; set; } = false;

        public bool DropIndexesBeforeCreate { get; set; } = false;

    }
}
