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
            ModelDoc2 swDoc = null;
            bool boolstatus = false;
            swDoc = ((ModelDoc2)(swApp.ActiveDoc));
            
            boolstatus = swDoc.Extension.SelectByID2(fet.Name, "SKETCH", 0, 0, 0, false, 0, null, 0);
            swDoc.EditSketch();
            swDoc.ClearSelection2(true);

            //获得所有线段
            double[] linesValue = sketch.GetLines2((short)swCrossHatchFilter_e.swCrossHatchExclude);
            int count = sketch.GetLineCount2((short)swCrossHatchFilter_e.swCrossHatchExclude);
            List<Line> lines = SwFunction.GetLines(linesValue, count);

            swDoc.ClearSelection2(true);
            for (int i = 0; i < count; i++)
            {
                boolstatus = swDoc.Extension.SelectByID2("Line1", "SKETCHSEGMENT", (lines[i].Start.X+lines[i].End.X)/2, (lines[i].Start.Y + lines[i].End.Y) / 2, 0, true, 1, null, 0);
            }
            boolstatus = swDoc.SketchManager.SketchOffset(0.022, true, true, false, false, true);
            //swDoc.ClearSelection2(true);
            //boolstatus = swDoc.EditRebuild3();//退出草图并重建图形
        }
    }
}
