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
            segments = NXFunction.GetSegmentLine(swApp, sketch);

            if (segments.Count == 0) return;
            
            //创建临时Cube
            for (int i = 0; i < segments.Count; i++)
            {
                CreateRunnerCube(segments[i],i);
            }

            //得到边的几何数据
            List<Line> lines = GetManifoldEdge();

            NewSketch(lines);

            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            for (int i = segments.Count - 1; i > -1; i--)
            {
                bool boolstatus = swDoc.Extension.SelectByID2("runnerCube" + i.ToString(), 
                    "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
                swDoc.Extension.DeleteSelection2((int)swDeleteSelectionOptions_e.swDelete_Absorbed);
            }
        }

        private void CreateRunnerCube(SketchSegment segment,int index)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            swDoc.ClearSelection2(true);

            bool boolstatus = swDoc.Extension.SelectByID2("前视基准面", "PLANE", 0, 0, 0, false, 0, null, 0);

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

            Sketch thisSketch = swDoc.SketchManager.ActiveSketch;
            Feature thisFet = (Feature)thisSketch;
            thisFet.Name = "runnerSketch"+index.ToString();

            swDoc.ClearSelection2(true);
            boolstatus = swDoc.Extension.SelectByID2(thisFet.Name, "SKETCH", 0, 0, 0, false, 0, null, 0);

            //合并，反向
            Feature myFeature = swDoc.SingleEndExtrusion(0.01,true,true);
            myFeature.Name = "runnerCube" + index.ToString();
            swDoc.ISelectionManager.EnableContourSelection = false;

            boolstatus = swDoc.EditRebuild3();//退出草图并重建图形
        }

        private List<Line> GetManifoldEdge()
        {
            List<Line> lines = new List<Line>();

            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            Feature feature = swDoc.GetFeatureInPrt("runnerCube0");
            object[] faces = feature.GetFaces();
            Vector target = new Vector(0, 0, 1);

            for (int i = 0; i < faces.Count(); i++)
            {
                Face2 face = (Face2)faces[i];
                Vector normal = new Vector(face.Normal);
                if (normal.isSameDir(target, 0.000005)!=1)
                    continue;
                object[] edges = face.GetEdges();
                double area = face.GetArea();
                int count = face.GetEdgeCount();
                for (int j = 0; j < count; j++)
                {
                    Edge edge = (Edge)edges[j];
                    Point start = new Point(edge.GetCurveParams3().StartPoint);
                    Point end = new Point(edge.GetCurveParams3().EndPoint);
                    Line line = new Line(start, end);
                    lines.Add(line);
                }
            }

            return lines;
        }

        private void NewSketch(List<Line> lines)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            ISketchManager ikm = swDoc.SketchManager;
            swDoc.ClearSelection2(true);

            bool boolstatus = swDoc.Extension.SelectByID2("前视基准面", "PLANE", 0, 0, 0, false, 0, null, 0);
            ikm.InsertSketch(true);

            for (int i = 0; i < lines.Count; i++)
            {
                ikm.CreateLine(lines[i].Start.X, lines[i].Start.Y, lines[i].Start.Z,
                        lines[i].End.X, lines[i].End.Y, lines[i].End.Z);
            }            

            Sketch thisSketch = ikm.ActiveSketch;
            Feature thisFet = (Feature)thisSketch;
            thisFet.Name = "ManifoldSketch";

            boolstatus = swDoc.Extension.SelectByID2(thisFet.Name, "SKETCH", 0, 0, 0, false, 0, null, 0);

            //合并，反向
            Feature myFeature = swDoc.SingleEndExtrusion(0.046, false, true);//BODYFEATURE
            myFeature.Name = "Manifold";//DeleteSelection2
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

