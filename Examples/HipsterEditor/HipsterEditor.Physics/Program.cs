using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipsterEditor.Physics.Screens;
using HipsterEngine.Core.Configurations;
using HipsterEngine.Core.Desktop;

namespace HipsterEditor.Physics
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var window = new MainWindow(1024, 720))
            {
                window.Run(60);
            }
        }
    }
}
