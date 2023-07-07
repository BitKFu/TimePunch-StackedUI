using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimePunch.StackedUI.Controller
{
    public enum  StackedMode
    {
        /// <summary>
        /// In full width mode a frame always extend to the full width of the parent window
        /// </summary>
        FullWidth,

        /// <summary>
        /// In resizeable mode, the frame stays with its width until the user expands it
        /// </summary>
        Resizeable,

        /// <summary>
        /// In place stack mode is a stack that does not extend horizontal, but do a replace
        /// </summary>
        InPlace
    }
}
