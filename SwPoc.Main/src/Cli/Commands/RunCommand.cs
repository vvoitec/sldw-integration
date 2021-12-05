using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SwPoc.Main.Cli.Interfaces;

namespace SwPoc.Main.Cli.Commands
{
    public class RunCommand : ICommand
    {
        private PartDoc _partDoc;
        ModelDoc2 _swModel;
        ModelDocExtension swExt;
        SelectionMgr swSelMgr;
        bool boolstatus;
        string y;
        double r1;
        double r2;
        bool rad;
        double ang;
        double xOff;
        double yOff;
        bool LockStart;
        bool LockEnd;
        int lErrors;
        int lWarnings;
        
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

            var swApp = System.Activator.CreateInstance(progType) as ISldWorks;
            swApp.Visible = true;
            
            _swModel = (ModelDoc2) swApp.NewPart();;
            swExt = _swModel.Extension;
            swSelMgr = (SelectionMgr)_swModel.SelectionManager;

            boolstatus = swExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            _swModel.SketchManager.InsertSketch(true);
            _swModel.ClearSelection2(true);

            SketchSegment skSegment = default(SketchSegment);
            skSegment = (SketchSegment)_swModel.SketchManager.CreateEquationSpline2("", "sin(x)", "", "0", "6.28", false, 0, 0, 0, true,
                true);
            _swModel.ViewZoomtofit2();

            System.Diagnostics.Debugger.Break();
            //Examine the graphics area, then press F5

            SketchSpline skSpline = default(SketchSpline);
            skSpline = (SketchSpline)skSegment;
            skSpline.GetEquationParameters(out y, out r1, out r2, out rad, out ang, out xOff, out yOff, out LockStart, out LockEnd);

            Debug.Print("y: " + y);
            Debug.Print("range start: " + r1);
            Debug.Print("range end: " + r2);
            Debug.Print("radian?: " + rad);
            Debug.Print("ang offset: " + ang);
            Debug.Print("x offset: " + xOff);
            Debug.Print("y offset: " + yOff);
            Debug.Print("lock start: " + LockStart);
            Debug.Print("lock end: " + LockEnd);

            // Change spline to a cosine curve
            y = "cos(x)";
            skSpline.SetEquationParameters(y, r1, r2, rad, ang, xOff, yOff, LockStart, LockEnd);

            // Reduce the number of points in the spline
            skSpline.Simplify(0.0);

            _swModel.SketchManager.InsertSketch(true);
            
            var swEquationMgr = _swModel.GetEquationMgr();
            swEquationMgr.Add3(0, "\"b\" = 2", true, (int)swInConfigurationOpts_e.swAllConfiguration, null);
            
            boolstatus = _swModel.Save3((int)swSaveAsOptions_e.swSaveAsOptions_Copy, ref lErrors, ref lWarnings);
            Console.WriteLine("Errors as defined in swFileSaveError_e: " + lErrors);
            Console.WriteLine("Warnings as defined in swFileSaveWarning_e: " + lWarnings);

            
            return true;
        }
    }
}