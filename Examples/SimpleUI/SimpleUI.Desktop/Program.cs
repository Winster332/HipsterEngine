using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipsterEngine.Core.Configurations;
using HipsterEngine.Core.Desktop;
using SimpleUI.Core.Screens;

namespace SimpleUI.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            Builder.Create(new HipsterStartup(), 780, 720)
                .SetTargetFPS(60, 60)
                .Run(new StartupScreen());
        }
    }
}
