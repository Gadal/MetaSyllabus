using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.Controls;
using System.Reflection;

namespace MetaSyllabus.Views
{
    public class PatchedZoomControl : ZoomControl
    {
        private MethodInfo updateViewportBase;

        new private void UpdateViewport()
        {
            if (updateViewportBase == null)
            {
                updateViewportBase = base.GetType().GetMethod("UpdateViewport", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            updateViewportBase.Invoke(this, null);
        }
    }
}
