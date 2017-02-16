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
            swDoc.ClearSelection2(true);
            boolstatus = swDoc.Extension.SelectByID2(fet.Name, "SKETCH", 0, 0, 0, false, 0, null, 0);
            swDoc.ActivateSelectedFeature();

            //获得所有线段
            double[] lines = sketch.GetLines2((short)swCrossHatchFilter_e.swCrossHatchExclude);
            int count = sketch.GetLineCount2((short)swCrossHatchFilter_e.swCrossHatchExclude);
            
            
            //boolstatus = swDoc.Extension.SelectByID2("Line1", "SKETCHSEGMENT", -0.023950509454303562, 0.00944726436442634, 0.013460865810254692, true, 0, null, 0);
            //boolstatus = swDoc.Extension.SelectByID2("Line6", "SKETCHSEGMENT", 0.0056611571475044253, 0.0069563930633148238, -0.00047254587326235262, true, 0, null, 0);
            //swDoc.ClearSelection2(true);
            //boolstatus = swDoc.Extension.SelectByID2("Line1", "SKETCHSEGMENT", -0.023950509454303562, 0.00944726436442634, 0.013460865810254692, false, 1, null, 0);
            //boolstatus = swDoc.Extension.SelectByID2("Line6", "SKETCHSEGMENT", 0.0056611571475044253, 0.0069563930633148238, -0.00047254587326235262, true, 1, null, 0);
            //boolstatus = swDoc.SketchManager.SketchOffset(0.022, true, true, false, false, true);
            //swDoc.ClearSelection2(true);
            //swDoc.SketchManager.InsertSketch(true);
        }
    }
}
