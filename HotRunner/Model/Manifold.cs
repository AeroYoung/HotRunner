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
            swDoc.ClearSelection2(true);

            //获得所有线段
            double[] linesValue = sketch.GetLines2((short)swCrossHatchFilter_e.swCrossHatchExclude);
            int count = sketch.GetLineCount2((short)swCrossHatchFilter_e.swCrossHatchExclude);
            List<Line> lines = SwFunction.GetLines(linesValue, count);

            //swDoc.ClearSelection2(true);
            List<SketchSegment> segments = new List<SketchSegment>();
            SelectData data = swSelMgr.CreateSelectData();
            data.Mark = 1000;
            EnumSketchSegments enumSegments = sketch.IEnumSketchSegments();
            if (enumSegments != null)
            {
                SketchSegment segment;
                int next = 1;
                enumSegments.Next(1,out segment,ref next);
                
                while (segment != null)
                {
                    segments.Add(segment);
                    segment = null;
                    enumSegments.Next(1, out segment, ref next);
                }
            }
            for (int i = 0; i < segments.Count; i++)
            {
                boolstatus = segments[i].Select(true);
            }
            
            boolstatus = swDoc.SketchManager.SketchOffset(0.022, true, true, false, false, true);
            //swDoc.ClearSelection2(true);
            //boolstatus = swDoc.EditRebuild3();//退出草图并重建图形
        }
    }
}
