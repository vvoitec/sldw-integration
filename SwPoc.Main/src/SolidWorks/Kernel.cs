using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;

namespace SwPoc.Main.SolidWorks
{
    public class Kernel
    {
        private static bool isVisible = true;
        private SldWorks _swApp;
        private ModelDoc2 _swDoc;
        private int _fileerror;
        private int _filewarning;
        EquationMgr swEquationMgr = default(EquationMgr);
        
        public Kernel()
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (!isWindows)
            {
                throw new ApplicationException("Not on Windows!");
            }
            var progId = "SldWorks.Application";

            var progType = System.Type.GetTypeFromProgID(progId);

            _swApp = System.Activator.CreateInstance(progType) as SldWorks;
            _swApp.Visible = isVisible;
        }
        
        public SldWorks Get()
        {
            return _swApp;
        }

        public Kernel Load(string path)
        {
            _swDoc = _swApp.OpenDoc6(path, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref _fileerror, ref _filewarning);
            
            return this;
        }

        public ModelDoc2 GetDoc()
        {
            return _swDoc;
        }

        public Kernel SetDimensions()
        {
            swEquationMgr = _swDoc.GetEquationMgr();

            var designTable = (DesignTable) _swDoc.GetDesignTable();
            designTable.EditTable2(true);
            string[] cells = {"5", "5", "30", "150"};
            designTable.Warn = true;
            var err = designTable.AddRow(cells);
            Console.WriteLine(err);
            err = designTable.UpdateTable((int)swDesignTableUpdateOptions_e.swUpdateDesignTableAll, true);
            Console.WriteLine(err);

            var swConfig = (Configuration)_swDoc.GetConfigurationByName("4");
            // swConfig.Select2(true, null);
            
            return this;
        }
    }
}