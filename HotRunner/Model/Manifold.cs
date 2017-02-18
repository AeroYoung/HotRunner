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
        #region 分流板参数 单位m

        private double runnerDiameter = 0.009;//浇口直径

        public double RunnerDiameter { get { return runnerDiameter; } }

        private int series = 9;//系列

        public int Series { get { return series; } }
            
        private double manifoldInsert = 0.055;//浇口到分流板壁面

        private double manifoldW = 0.044;//分流板宽度

        private double manifoldH = 0.046;//分流板高度

        #endregion

        #region 公共变量

        SldWorks swApp = null;

        Sketch basicSketch = null;//流道和点位图

        private List<SketchLine> runnerSegments = new List<SketchLine>();//流道中心线，在Commit中获取

        private List<SketchArc> gateArcs = new List<SketchArc>();//出胶口

        private List<Point> gatePoints = new List<Point>();

        private List<Line> contourLine = new List<Line>();//计算得到的轮廓

        private List<SketchLine> contourSegmentLine = new List<SketchLine>();//轮廓的草图线段

        #endregion

        public Manifold(SldWorks swApp, Sketch basicSketch)
        {
            this.swApp = swApp;
            this.basicSketch = basicSketch;

            runnerSegments = basicSketch.GetSegmentLine(swApp);
            gateArcs = basicSketch.GetSegmentArc(swApp);

            #region 1.从Sketch中获得流道直径

            for (int i = 0; i < gateArcs.Count; i++)
            {
                runnerDiameter = gateArcs[i].GetRadius() * 2;
                series = (int)(runnerDiameter * 1000);
                gatePoints.Add(new Point(gateArcs[i].GetCenterPoint2()));
            }

            #endregion

            #region 2.设置或获取SW Global Variable。单位是mm，主意转换！

            swApp.SetGlobalVariable("RunnerDiameter", runnerDiameter * 1000);

            swApp.SetGlobalVariable("Series", series);

            manifoldInsert = swApp.GetGlobalVariable("ManifoldInsert", manifoldInsert * 1000) / 1000;

            manifoldW = swApp.GetGlobalVariable("ManifoldW", manifoldW * 1000) / 1000;

            swApp.SetGlobalVariable("ManifoldW2", manifoldW * 1000 / 2);//分流板宽度的一半，用于标注
             
            manifoldH = swApp.GetGlobalVariable("ManifoldH", manifoldH * 1000) / 1000;

            #endregion
        }

        public void Commit()
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            
            if (runnerSegments.Count == 0) return;

            ManifoldBody();

            
        }

        #region 1.分流板主体

        /// <summary>
        /// 创建分流板主体，并关联ManifoldH全局变量
        /// </summary>
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

            #endregion

            #region 3.标注尺寸

            Sketch thisSketch = ikm.ActiveSketch;
            Feature thisFet = (Feature)thisSketch;
            thisFet.Name = "ManifoldSketch";

            //1. 分流板宽度
            contourSegmentLine = thisSketch.GetSegmentLine(swApp);
            DimensionManifoldW();

            #endregion

            #region 3.生成特征

            boolstatus = swDoc.Extension.SelectByID2(thisFet.Name, "SKETCH", 0, 0, 0, false, 0, null, 0);
            
            Feature myFeature = swDoc.SingleEndExtrusion(manifoldH, false, true);
            myFeature.Name = "Manifold";

            swApp.SetGlobalVariable("D1@Manifold", "ManifoldH");

            #endregion

            #region 4.删除临时特征
            for (int i = runnerSegments.Count - 1; i > -1; i--)
            {
                boolstatus = swDoc.Extension.SelectByID2("runnerCube" + i.ToString(),
                    "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
                swDoc.Extension.DeleteSelection2((int)swDeleteSelectionOptions_e.swDelete_Absorbed);
            }
            #endregion
        }

        private void CreateRunnerCube(SketchLine segment,int index)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            swDoc.ClearSelection2(true);

            bool boolstatus = swDoc.Extension.SelectByID2("前视基准面", "PLANE", 0, 0, 0, false, 0, null, 0);

            swDoc.SketchManager.InsertSketch(true);

            #region  画矩形

            Line line = segment.toLine();
            Vector dir2 = new Vector(line.Dir.Y, -line.Dir.X, 0);
            Point point1 = new Point(0, 0, 0);
            Point point2 = new Point(0, 0, 0);

            if (IsCoincideWithGate(line.Start))
            {
                point1 = line.Start;
                point1.X += manifoldW / 2 * dir2.unit.X - manifoldInsert * line.Dir.unit.X;
                point1.Y += manifoldW / 2 * dir2.unit.Y - manifoldInsert * line.Dir.unit.Y;
                point1.Z += manifoldW / 2 * dir2.unit.Z - manifoldInsert * line.Dir.unit.Z;

                point2 = line.End;
                point2.X -= manifoldW / 2 * dir2.unit.X;
                point2.Y -= manifoldW / 2 * dir2.unit.Y;
                point2.Z -= manifoldW / 2 * dir2.unit.Z;
            }
            else if (IsCoincideWithGate(line.End))
            {
                point1 = line.End;
                point1.X += manifoldW / 2 * dir2.unit.X + manifoldInsert * line.Dir.unit.X;
                point1.Y += manifoldW / 2 * dir2.unit.Y + manifoldInsert * line.Dir.unit.Y;
                point1.Z += manifoldW / 2 * dir2.unit.Z + manifoldInsert * line.Dir.unit.Z;

                point2 = line.Start;
                point2.X -= manifoldW / 2 * dir2.unit.X;
                point2.Y -= manifoldW / 2 * dir2.unit.Y;
                point2.Z -= manifoldW / 2 * dir2.unit.Z;
            }
            else
            {
                point1 = line.End;
                point1.X += manifoldW / 2 * dir2.unit.X;
                point1.Y += manifoldW / 2 * dir2.unit.Y;
                point1.Z += manifoldW / 2 * dir2.unit.Z;

                point2 = line.Start;
                point2.X -= manifoldW / 2 * dir2.unit.X;
                point2.Y -= manifoldW / 2 * dir2.unit.Y;
                point2.Z -= manifoldW / 2 * dir2.unit.Z;
            }
            
            swDoc.SketchManager.CreateCornerRectangle(
                point1.X, point1.Y, point1.Z,
                point2.X, point2.Y, point2.Z);

            #endregion

            Sketch thisSketch = swDoc.SketchManager.ActiveSketch;
            Feature thisFet = (Feature)thisSketch;
            thisFet.Name = "runnerSketch"+index.ToString();

            swDoc.ClearSelection2(true);
            boolstatus = swDoc.Extension.SelectByID2(thisFet.Name, "SKETCH", 0, 0, 0, false, 0, null, 0);

            //临时特征
            Feature myFeature = swDoc.SingleEndExtrusion(0.01,true,true);
            myFeature.Name = "runnerCube" + index.ToString();
            swDoc.ISelectionManager.EnableContourSelection = false;

            boolstatus = swDoc.EditRebuild3();//退出草图并重建图形
        }

        private bool IsCoincideWithGate(Point point)
        {
            double tolerance = 0.000005;

            bool result = false;

            for (int i = 0; i < gatePoints.Count; i++)
            {
                if (point.isCoincode(gatePoints[i], tolerance))
                {
                    return true;
                }
            }

            return result;
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
        
        private void DimensionManifoldW()
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            ISketchManager ikm = swDoc.SketchManager;
            double tolerance = 0.05;

            
            List<SketchLine> contours = new List<SketchLine>(contourSegmentLine.ToArray());
            List<SketchLine> runner = new List<SketchLine>(runnerSegments.ToArray());
            
            for (int i = 0; i < contours.Count; i++)
            {
                for (int j = 0; j < runner.Count; j++)
                {
                    if (!runner[j].isParallerTo(contours[i], tolerance))
                        continue;
                    
                    double distance = contours[i].DistanceTo(runner[j]);
                    double d = Math.Abs(distance - manifoldW / 2);
                    if (d > 0.0001)
                        continue;
                    
                    contours[i].DimensionWith(runner[j], manifoldW / 2, "", swApp);
                    
                }
            }
                   
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