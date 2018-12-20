namespace Plato.Internal.Text.Diff.Model
{

    public class DiffBlock
    {

        public int DeleteStartA { get; }
        
        public int DeleteCountA { get; }

        public int InsertStartB { get; }

        public int InsertCountB { get; }
        
        public DiffBlock(int deleteStartA, int deleteCountA, int insertStartB, int insertCountB)
        {
            DeleteStartA = deleteStartA;
            DeleteCountA = deleteCountA;
            InsertStartB = insertStartB;
            InsertCountB = insertCountB;
        }

    }

}