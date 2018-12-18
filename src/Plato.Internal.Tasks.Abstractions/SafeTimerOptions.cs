using System;

namespace Plato.Internal.Tasks.Abstractions
{
    public class SafeTimerOptions
    {

        public Type Owner { get; set; }

        /// <summary>
        /// Gets or sets a boolean to indicate if the timer callback should run immediately. 
        /// </summary>
        public bool RunOnStart { get; set; }

        /// <summary>
        /// Gets or sets a boolean to indicate if the timer shuld only run once and then terminate. 
        /// </summary>
        public bool RunOnce { get; set; }

        /// <summary>
        /// Gets or sets the interval in seconds to wait between timer callbacks.
        /// </summary>
        public int IntervalInSeconds { get; set; }

    }

}
