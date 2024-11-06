using TimePunch.MVVM.EventAggregation;
using TimePunch.StackedUI.Controller;

namespace TimePunch_WinUI_StackedUI_Demo.Core
{
    public class DemoKernel
    {
        private static DemoKernel? instance;

        private DemoKernel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            Controller = new DemoController(EventAggregator);
        }

        /// <summary>
        /// Gets or sets the Kernel Instance
        /// </summary>
        public static DemoKernel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DemoKernel(new EventAggregator());
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        /// <returns></returns>
        public IEventAggregator EventAggregator { get; }

        /// <summary>
        /// Gets the controller
        /// </summary>
        public IStackedController Controller { get; private set; }
    }
}
