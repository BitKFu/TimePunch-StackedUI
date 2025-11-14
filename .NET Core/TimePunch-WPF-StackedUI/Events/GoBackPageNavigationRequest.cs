using System.Windows.Controls;

namespace TimePunch.StackedUI.Events
{
    public class GoBackPageNavigationRequest
    {
        /// <summary>
        /// Creates a new instance of the GoBackPageNavigationRequest class
        /// </summary>
        public GoBackPageNavigationRequest(Page? toPage = null, bool takeLock = true)
        {
            ToPage = toPage;
            TakeLock = takeLock;    
        }

        public bool TakeLock { get; set; }

        public Page? ToPage { get; }
    }
}
