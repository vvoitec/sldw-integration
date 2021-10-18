using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SwPoc.Main.Cli.Interfaces;

namespace SwPoc.Main.Cli.Commands
{
    public class RunCommand : ICommand
    {
        public string GetName()
        {
            return "run";
        }

        public bool Handle()
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (!isWindows)
            {
                throw new ApplicationException("Not on Windows!");
            }
            var progId = "SldWorks.Application";

            var progType = System.Type.GetTypeFromProgID(progId);

            var app = System.Activator.CreateInstance(progType) as SolidWorks.Interop.sldworks.ISldWorks;
            app.Visible = true;

            return true;
        }
    }
}