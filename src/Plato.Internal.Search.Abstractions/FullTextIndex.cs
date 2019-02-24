namespace Plato.Internal.Search.Abstractions
{
    public class FullTextIndex
    {

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public int LanguageCode { get; set; }

        public int FillFactor { get; set; }

        protected FullTextIndex(string tableName)
        {
            TableName = tableName;
        }

        public FullTextIndex(string tableName, string columnName) : this(tableName)
        {
            ColumnName = columnName;
        }

        public FullTextIndex(
            string tableName,
            string columnName,
            int languageCode) : this(tableName, columnName)
        {
            LanguageCode = languageCode;
        }
        public FullTextIndex(
            string tableName,
            string columnName,
            int languageCode,
            int fillFactor) : this(tableName, columnName, languageCode)
        {
            FillFactor = fillFactor;
        }


    }
}
