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
        List<SketchSegment> segments = new List<SketchSegment>();//流道中心线，在Commit中获取

        #region 分流板参数

        double insert = 0.055;
        double width = 0.044;

        #endregion

        public Manifold(SldWorks swApp)
        {
            this.swApp = swApp;
        }

        public void Commit(Sketch sketch)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            swDoc.ClearSelection2(true);

            segments = NXFunction.GetSegmentLine(swApp, sketch);

            CreateRunnerCube(segments[1]);
            for (int i = 0; i < segments.Count; i++)
            {
                CreateRunnerCube(segments[i]);
            }
        }

        private void CreateRunnerCube(SketchSegment segment)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            swDoc.ClearSelection2(true);
            bool boolstatus = false;
            
            //TODO 遍历基准面或者创建基准面
            boolstatus = swDoc.Extension.SelectByID2("上视基准面", "PLANE", 0, 0, 0, false, 0, null, 0);
            swDoc.SketchManager.InsertSketch(true);
            
            Line line = segment.toLine();
            Point centrePoint = new Point((line.Start.X + line.End.X) / 2, (line.Start.Y + line.End.Y) / 2, (line.Start.Z + line.End.Z) / 2);
            Vector dir2 = new Vector(line.dir.Y, -line.dir.X, 0);
            Point point2 = line.End;
            point2.X += insert * line.dir.unit.X + width / 2 * dir2.unit.X;
            point2.Y += insert * line.dir.unit.Y + width / 2 * dir2.unit.Y;
            point2.Z += insert * line.dir.unit.Z + width / 2 * dir2.unit.Z;

            swDoc.SketchManager.CreateCenterRectangle(
                centrePoint.X, centrePoint.Y, centrePoint.Z,
                point2.X, point2.Y, point2.Z);
            
            boolstatus = swDoc.EditRebuild3();//退出草图并重建图形
        }

        public void Commit2(Sketch sketch)
        {
            Feature fet = (Feature)sketch;
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            SelectionMgr swSelMgr = (SelectionMgr)swDoc.SelectionManager;

            bool boolstatus = swDoc.Extension.SelectByID2(fet.Name, "SKETCH", 0, 0, 0, false, 0, null, 0);

            //swDoc.EditSketch();

            List<SketchSegment> segments = NXFunction.GetSegmentLine(swApp, sketch);
            //for (int i = 0; i < segments.Count; i++)
            //{
            //    segments[i].Select(true);                
            //    segments[i].CreateCube(swApp);
            //    //swDoc.ClearSelection2(true);
            //}
            //segments.SelectAll();            
            
            //boolstatus = swDoc.SketchManager.SketchOffset(0.022, true, true, false, false, true);
            
            boolstatus = swDoc.EditRebuild3();//退出草图并重建图形
        }        
    }
}

