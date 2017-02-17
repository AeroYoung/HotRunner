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
        #region 分流板参数

        private double manifoldInsert = 0.055;//浇口到分流板壁面

        private double manifoldW = 0.044;//分流板宽度

        private double manifoldH = 0.046;//分流板高度

        #endregion

        #region 公共变量

        SldWorks swApp = null;

        private List<SketchSegment> runnerSegments = new List<SketchSegment>();//流道中心线，在Commit中获取

        private List<Line> contourLine = new List<Line>();//计算得到的轮廓
        
        #endregion

        public Manifold(SldWorks swApp)
        {
            this.swApp = swApp;
        }

        public void Commit(Sketch sketch)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            runnerSegments = NXFunction.GetSegmentLine(swApp, sketch);

            if (runnerSegments.Count == 0) return;

            ManifoldBody();
        }

        #region 1.分流板主体

        private void ManifoldBody()
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            ISketchManager ikm = swDoc.SketchManager;

            #region 1.创建临时Cube
            for (int i = 0; i < runnerSegments.Count; i++)
            {
                CreateRunnerCube(runnerSegments[i], i);
            }
            #endregion

            #region 2.得到边的几何数据，重新绘图

            contourLine = GetManifoldEdge();
           
            swDoc.ClearSelection2(true);

            bool boolstatus = swDoc.Extension.SelectByID2("前视基准面", "PLANE", 0, 0, 0, false, 0, null, 0);
            ikm.InsertSketch(true);

            for (int i = 0; i < contourLine.Count; i++)
            {
                ikm.CreateLine(contourLine[i].Start.X, contourLine[i].Start.Y, contourLine[i].Start.Z,
                        contourLine[i].End.X, contourLine[i].End.Y, contourLine[i].End.Z);
            }

            Sketch thisSketch = ikm.ActiveSketch;
            Feature thisFet = (Feature)thisSketch;
            thisFet.Name = "ManifoldSketch";

            boolstatus = swDoc.Extension.SelectByID2(thisFet.Name, "SKETCH", 0, 0, 0, false, 0, null, 0);
            
            Feature myFeature = swDoc.SingleEndExtrusion(manifoldH, false, true);
            myFeature.Name = "Manifold";

            #endregion

            #region 3.删除临时特征
            for (int i = runnerSegments.Count - 1; i > -1; i--)
            {
                boolstatus = swDoc.Extension.SelectByID2("runnerCube" + i.ToString(),
                    "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
                swDoc.Extension.DeleteSelection2((int)swDeleteSelectionOptions_e.swDelete_Absorbed);
            }
            #endregion
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
            point2.X += manifoldInsert * line.dir.unit.X + manifoldW / 2 * dir2.unit.X;
            point2.Y += manifoldInsert * line.dir.unit.Y + manifoldW / 2 * dir2.unit.Y;
            point2.Z += manifoldInsert * line.dir.unit.Z + manifoldW / 2 * dir2.unit.Z;

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

        #endregion

        #region 2.倒角

        private void ManifoldChamfer()
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            bool boolstatus = false;
            
            boolstatus = swDoc.Extension.SelectByID2("", "EDGE", 0.022298795930680626, 0.15715131653854542, -0.023204627553809587, true, 0, null, 0);
            boolstatus = swDoc.Extension.SelectByID2("", "EDGE", 0.022823469233060223, 0.021417719320993456, -0.027173159468418362, true, 0, null, 0);
            boolstatus = swDoc.Extension.SelectByID2("", "EDGE", 0.2480754615000933, 0.021566930129438333, -0.021339355300995067, true, 0, null, 0);
            Feature myFeature = null;
            myFeature = ((Feature)(swDoc.FeatureManager.InsertFeatureChamfer(4, 1, 0.01, 0.78539816339745, 0, 0, 0, 0)));
        }

        #endregion
    }
}