using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ColorPicker.Helpers
{
    public static class Helper
    {
        public static string GetKeyName(uint virtualKeyCode)
        {
            return ((Key)KeyInterop.KeyFromVirtualKey((int)virtualKeyCode)).ToString();
        }
    }
}
