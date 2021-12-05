using System;
using System.Diagnostics;
using SolidWorks.Interop.cosworks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SwPoc.Main.Cli.Interfaces;
using SwPoc.Main.SolidWorks;

namespace SwPoc.Main.Cli.Commands
{
    public class StudyCommand : ICommand
    {
        public string GetName()
        {
            return "study";
        }

        public bool Handle()
        { 
            swApp = (new Kernel()).Get();

            dynamic COSMOSWORKS = default(dynamic);
            dynamic COSMOSObject = default(dynamic);
            // {67E9B217-A19F-4563-B8C1-BFE5E433F1BD}
            // Determine host SOLIDWORKS major version
            int swVersion = Convert.ToInt32(swApp.RevisionNumber().Substring(0, 2));

            // Calculate the version-specific ProgID of the Simulation add-in that is compatible with this version of SOLIDWORKS
            int cwVersion = swVersion - 15;
            String cwProgID = String.Format("SldWorks.Simulation.{0}", cwVersion);
            Debug.Print(cwProgID);
            
            string cwpath = swApp.GetExecutablePath() + @"\Simulation\cosworks.dll";

            swApp.LoadAddIn(cwpath);
 
            // Get the SOLIDWORKS Simulation object 
            COSMOSObject = swApp.GetAddInObject("{67E9B217-A19F-4563-B8C1-BFE5E433F1BD}");
            if (COSMOSObject == null) ErrorMsg(swApp, "COSMOSObject object not found", true);
            COSMOSWORKS = COSMOSObject.CosmosWorks;
            if (COSMOSWORKS == null) ErrorMsg(swApp, "COSMOSWORKS object not found", true);
    
            // Open the active document and use the COSMOSWORKS API
            int errors = 0;
            int warnings = 0;
            int errCode = 0;
            swApp.OpenDoc6("D:\\dev\\SwPoc\\assets\\assembly_schema.sldasm", (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
            var ActDoc = (CWModelDoc)COSMOSWORKS.ActiveDoc;
            if (ActDoc == null) ErrorMsg(swApp, "No active document", true);
 
            // Create new nonlinear study 
            var StudyMngr = (CWStudyManager)ActDoc.StudyManager;
            if (StudyMngr == null) ErrorMsg(swApp, "CWStudyManager object not created", true);
            var Study = (CWStudy)StudyMngr.CreateNewStudy("Static", (int)swsAnalysisStudyType_e.swsAnalysisStudyTypeStatic, (int)swsMeshType_e.swsMeshTypeSolid, out errCode);
            if (Study == null) ErrorMsg(swApp, "Study not created", true);
            
            // Get number of solid components 
            var SolidMgr = (CWSolidManager)Study.SolidManager;
            if (SolidMgr == null) ErrorMsg(swApp, "CWSolidManager object not created", true);
            var CompCount = SolidMgr.ComponentCount;
            int errorCode = 0;
            var SolidComponent = SolidMgr.GetComponentAt(0, out errorCode);
            if (SolidComponent == null) ErrorMsg(swApp, "CWSolidComponent object not created", true);
 
            // Get name of solid component 
            var SName = SolidComponent.ComponentName;
 
            // Apply user-defined material to the first component in the tree
            var SolidBody = (CWSolidBody)SolidComponent.GetSolidBodyAt(0, out errCode);
            
            if (errCode != 0) ErrorMsg(swApp, "No solid body", true);
            if (SolidBody == null) ErrorMsg(swApp, "CWSolidBody object not created", true);
            var CWMat = (CWMaterial)SolidBody.GetDefaultMaterial();
            CWMat.MaterialUnits = 0;
            if (CWMat == null) ErrorMsg(swApp, "No default material object", true);
            CWMat.MaterialName = "Alloy Steel";
            CWMat.SetPropertyByName("EX", 210000000000.0, 0);
            CWMat.SetPropertyByName("NUXY", 0.28, 0);
            CWMat.SetPropertyByName("GXY", 79000000000.0, 0);
            CWMat.SetPropertyByName("DENS", 7700, 0);
            CWMat.SetPropertyByName("SIGXT", 723825600, 0);
            CWMat.SetPropertyByName("SIGYLD", 620422000, 0);
            CWMat.SetPropertyByName("ALPX", 1.3E-05, 0);
            CWMat.SetPropertyByName("KX", 50, 0);
            CWMat.SetPropertyByName("C", 460, 0);
            errCode = SolidBody.SetSolidBodyMaterial(CWMat);
            if (errCode != 0) ErrorMsg(swApp, "Failed to apply material", true);
            
            // Meshing
            
            var CwMesh = (CWMesh)Study.Mesh;
            if (CwMesh == null)
                ErrorMsg(swApp, "No mesh");

            // Set type of mesh to curvature-based
            CwMesh.MesherType = (int)swsMesherType_e.swsMesherTypeAlternate;

            // Set mesh parameters
            CwMesh.GrowthRatio = 1.6;
            CwMesh.MinElementsInCircle = 8;
            double maxElementSize = 0.0;
            double minElementSize = 0.0;
            CwMesh.GetDefaultMaxAndMinElementSize((int)swsLinearUnit_e.swsLinearUnitMillimeters, out maxElementSize, out minElementSize);

            // Create mesh
            errCode = Study.CreateMesh((int)swsLinearUnit_e.swsLinearUnitMillimeters, 20, 1);
            Debug.Print("Error code: " + errCode);
            //0 indicates successful creation of the mesh
            if (errCode != 0)
                ErrorMsg(swApp, "Mesh failed; check error code");

            return true;
        }
        
        public void ErrorMsg(SldWorks SwApp, string Message, bool EndTest = false)
        {
            Console.WriteLine(Message);
            Console.WriteLine("'*** WARNING - General");
            Console.WriteLine("'*** " + Message);
            Console.WriteLine("");
            if (EndTest)
            {
            }
        }

        public SldWorks swApp;
    }
}