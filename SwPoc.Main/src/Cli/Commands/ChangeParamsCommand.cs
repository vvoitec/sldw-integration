using System;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SwPoc.Main.Cli.Interfaces;
using SwPoc.Main.SolidWorks;

namespace SwPoc.Main.Cli.Commands
{
    public class ChangeParamsCommand: ICommand
    {
        private const string PartPath = "D:\\dev\\SwPoc\\assets\\part_schema_2.sldprt";
    
        public string GetName()
        {
            return "change-params";
        }

        public bool Handle()
        {
            var app = (new Kernel()).Load(PartPath).SetDimensions();
            return true;
        }
    }
}