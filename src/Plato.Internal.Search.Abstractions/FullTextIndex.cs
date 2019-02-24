namespace Plato.Internal.Search.Abstractions
{
    public class FullTextIndex
    {

        public string TableName { get; set; }

        public string[] ColumnNames { get; set; }

        public int LanguageCode { get; set; } = 1033;

        public short FillFactor { get; set; } = 30;

        protected FullTextIndex(string tableName)
        {
            TableName = tableName;
        }

        public FullTextIndex(string tableName, string[] columnNames) : this(tableName)
        {
            ColumnNames = columnNames;
        }

        public FullTextIndex(
            string tableName,
            string[] columnNames,
            int languageCode) : this(tableName, columnNames)
        {
            LanguageCode = languageCode;
        }
        public FullTextIndex(
            string tableName,
            string[] columnNames,
            int languageCode,
            short fillFactor) : this(tableName, columnNames, languageCode)
        {
            FillFactor = fillFactor;
        }
        
    }

}
