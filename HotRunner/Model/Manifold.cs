using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HotRunner
{
    public class Manifold
    {
        SldWorks swApp = null;

        public Manifold(SldWorks swApp)
        {
            this.swApp = swApp;
        }

        public void Commit(Sketch sketch)
        {
            Feature fet = (Feature)sketch;
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            SelectionMgr swSelMgr = (SelectionMgr)swDoc.SelectionManager;

            bool boolstatus = swDoc.Extension.SelectByID2(fet.Name, "SKETCH", 0, 0, 0, false, 0, null, 0);

            swDoc.EditSketch();

            List<SketchSegment> segments = NXFunction.GetSegmentLine(swApp, sketch);
            segments.SelectAll();
            
            
            boolstatus = swDoc.SketchManager.SketchOffset(0.022, true, true, false, false, true);
            //swDoc.ClearSelection2(true);
            //boolstatus = swDoc.EditRebuild3();//退出草图并重建图形
        }
    }
}
