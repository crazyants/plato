using System.Threading.Tasks;

namespace Plato.Internal.Tasks.Abstractions
{

    public interface IBackgroundTaskProvider
    {

        int IntervalInSeconds { get; }

        Task ExecuteAsync();
        
    }

}
