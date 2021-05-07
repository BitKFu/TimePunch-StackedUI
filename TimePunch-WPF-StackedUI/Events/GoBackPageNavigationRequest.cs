﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TimePunch.StackedUI.Events
{
    public class GoBackPageNavigationRequest
    {
        /// <summary>
        /// Creates a new instance of the GoBackPageNavigationRequest class
        /// </summary>
        /// <param name="toPage"></param>
        public GoBackPageNavigationRequest(Page? toPage = null)
        {
            ToPage = toPage;
        }

        public Page? ToPage { get; }
    }
}
