using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ShellThing
{
    public class Persistence
    {
        Assembly stagerAssembly;

        public Persistence()
        {
            stagerAssembly = Assembly.GetEntryAssembly();
        }
    }
}
