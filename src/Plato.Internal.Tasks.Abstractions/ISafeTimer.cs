namespace Plato.Internal.Tasks.Abstractions
{


    public delegate void TimerEventHandler(object sender, SafeTimerEventArgs e);

    public interface ISafeTimer
    {

        int IntervalInSeconds { get; set; }

        event TimerEventHandler Elapsed;
        
        void Start();

        void Stop();

        void WaitToStop();

    }

}
