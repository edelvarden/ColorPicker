using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ColorPicker.Helpers
{
    [Export(typeof(AppUpdateManager))]
    public class AppUpdateManager
    {

    }
}
