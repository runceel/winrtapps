using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Okazuki.Bookmarker.Utils
{
    public static class DispatcherHolder
    {
        public static CoreDispatcher Dispatcher { get; private set; }

        public static void InitializeIfNeeded(DependencyObject obj)
        {
            if (Dispatcher != null)
            {
                return;
            }

            Dispatcher = obj.Dispatcher;
        }
    }
}
