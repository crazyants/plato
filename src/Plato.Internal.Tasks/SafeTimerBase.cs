using System.Threading;

namespace Plato.Internal.Tasks
{

    public abstract class SafeTimerBase 
    {
   
        private volatile bool _isRunning;
        
        public bool IsRunning => _isRunning;

        public bool PerformingTasks { get; set; }
        
        public virtual void Start()
        {
            _isRunning = true;
        }

        public virtual void Stop()
        {
            _isRunning = false;
        }

        public virtual void WaitToStop()
        {
            _isRunning = false;
        }

    }

}
