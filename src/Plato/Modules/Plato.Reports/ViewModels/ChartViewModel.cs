namespace Plato.Reports.ViewModels
{

    public class ChartViewModel<TModel> where TModel : class
    {

        public ChartOptions Options { get; set; }

        public TModel Data { get; set; }
        
    }

    public class ChartOptions
    {

        public string Title { get; set; }



    }
}
