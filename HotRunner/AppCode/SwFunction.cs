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
    public static class NXFunction
    {
        #region 2D Sketch

        /// <summary>
        /// 从Sketch.GetLines2数组中获得Line
        /// </summary>
        /// <param name="value">数组 Sketch.GetLines2</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public static List<Line> GetLines(double[] value, int count)
        {
            List<Line> result = new List<Line>();

            for (int i = 0; i < count; i++)
            {
                double[] a = new double[12];
                for (int j = 0; j < 12; j++)
                {
                    a[j] = value[(count - 1) * 12 + j];
                }
                result.Add(new Line(a));
            }

            return result;
        }

        /// <summary>
        /// 得到Sketch中的线段
        /// </summary>
        /// <param name="swApp"></param>
        /// <param name="sketch">草图</param>
        /// <returns></returns>
        public static List<SketchLine> GetSegmentLine(this Sketch sketch, SldWorks swApp)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            SelectionMgr swSelMgr = (SelectionMgr)swDoc.SelectionManager;

            List<SketchLine> segments = new List<SketchLine>();
            EnumSketchSegments enumSegments = sketch.IEnumSketchSegments();
            if (enumSegments != null)
            {
                SketchSegment segment;
                int next = 1;
                enumSegments.Next(1, out segment, ref next);

                while (segment != null)
                {
                    if (segment.GetType() == (int)swSketchSegments_e.swSketchLINE)
                        segments.Add((SketchLine)segment);
                    segment = null;
                    enumSegments.Next(1, out segment, ref next);
                }
            }

            return segments;
        }
        
        public static Line toLine(this SketchLine segment)
        {
            return new Line(segment);
        }

        /// <summary>
        /// 得到Sketch中的圆弧和圆
        /// </summary>
        /// <param name="swApp"></param>
        /// <param name="sketch">草图</param>
        /// <returns></returns>
        public static List<SketchArc> GetSegmentArc(this Sketch sketch, SldWorks swApp)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            SelectionMgr swSelMgr = (SelectionMgr)swDoc.SelectionManager;

            List<SketchArc> segments = new List<SketchArc>();
            EnumSketchSegments enumSegments = sketch.IEnumSketchSegments();
            if (enumSegments != null)
            {
                SketchSegment segment;
                int next = 1;
                enumSegments.Next(1, out segment, ref next);

                while (segment != null)
                {
                    if (segment.GetType() == (int)swSketchSegments_e.swSketchARC)
                        segments.Add((SketchArc)segment);
                    segment = null;
                    enumSegments.Next(1, out segment, ref next);
                }
            }

            return segments;
        }
        
        /// <summary>
        /// 线段全部加入选择集
        /// </summary>
        /// <param name="segments"></param>
        public static void SelectAll(this List<SketchSegment> segments)
        {
            bool boolstatus = false;
            for (int i = 0; i < segments.Count; i++)
            {
                boolstatus = segments[i].Select(true);
            }
        }

        #endregion

        #region 3D特征

        /// <summary>
        /// 薄壁拉伸
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="swApp"></param>
        public static void CreateCube(this SketchSegment segment, SldWorks swApp)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            //bool boolstatus = false;
            Feature myFeature = null;
            IFeatureManager featMan = swDoc.FeatureManager;
            myFeature = featMan.FeatureExtrusionThin2(
                true, 
                false, 
                true,
                (int)swEndConditions_e.swEndCondMidPlane,
                (int)swEndConditions_e.swEndCondMidPlane,
                0.01, 
                0.01,
                false,//拔模
                false, 
                false, 
                false,
                0, 
                0,
                false, 
                false, 
                false, 
                false, 
                false,//merge
                0.022, 
                0.022, 
                0,
                2, 
                0, 
                false, 
                0.005, 
                false,
                true
                ,
                (int)swStartConditions_e.swStartSketchPlane,
                0.0,
                false
                );

        }

        public static Feature SingleEndExtrusion(this ModelDoc2 swDoc,double h,bool merge,bool flip)
        {
            Feature feature = swDoc.FeatureManager.FeatureExtrusion2(
                true,//True for single ended, false for double ended
                false,//True to flip side to cut
                flip,//反向
                (int)swEndConditions_e.swEndCondBlind,//Termination type for first end 
                (int)swEndConditions_e.swEndCondBlind,
                h,//Depth of extrusion for first end in meters
                0,
                false,//True allows draft angle in first direction, false does not allow drafting
                false,
                false,
                false,
                0.017453292519943334,//draft angle 1
                0.017453292519943334,//draft angle 2
                false,//OffsetReverse1 
                      //If you chose to offset the first end condition from another face or plane,
                      //then True specifies offset in direction away from the sketch, 
                      //false specifies offset from the face or plane in direction toward the sketch
                false,//OffsetReverse2
                false,//TranslateSurface1 
                      //When you choose swEndcondOffsetFromSurface as the termination type 
                      //then True specifies that the end of the extrusion is a translation 
                      //of the reference surface, false specifies to use a true offset
                false,//TranslateSurface2
                merge,//True to merge the results in a multibody part, false to not
                true,
                true,
                (int)swStartConditions_e.swStartSketchPlane,//t0:Start condition 
                0,//If t0 set to swStartOffset, then specify offset value
                false
                );

            return feature;
        }

        #endregion

        #region 获得对象

        public static Feature GetFeatureInPrt(this ModelDoc2 swDoc,string name)
        {
            Feature feature = null;            
            string featureName = "";

            feature = swDoc.FirstFeature();
            
            while (feature != null)
            {
                featureName = feature.Name;
                if (featureName == name) return feature;
                else feature = feature.GetNextFeature();
            }

            return null;
        }

        #endregion

        #region 几何计算

        public static bool isCoincode(this Point source, Point target,double tolerance)
        {
            if (Math.Abs(source.X - target.X) < tolerance &&
               Math.Abs(source.Y - target.Y) < tolerance &&
               Math.Abs(source.Z - target.Z) < tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isParallerTo(this Vector source, Vector target, double tolerance)
        {
            if (Math.Abs(source.unit.X - target.unit.X) < tolerance &&
               Math.Abs(source.unit.Y - target.unit.Y) < tolerance &&
               Math.Abs(source.unit.Z - target.unit.Z) < tolerance)
            {
                return true;
            }
            else if (Math.Abs(source.unit.X + target.unit.X) < tolerance &&
               Math.Abs(source.unit.Y + target.unit.Y) < tolerance &&
               Math.Abs(source.unit.Z + target.unit.Z) < tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否同向
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns>1 同向;2 反向;-1 其他</returns>
        public static int isSameDir(this Vector source, Vector target, double tolerance)
        {
            if (Math.Abs(source.unit.X - target.unit.X) < tolerance &&
               Math.Abs(source.unit.Y - target.unit.Y) < tolerance &&
               Math.Abs(source.unit.Z - target.unit.Z) < tolerance)
            {
                return 1;
            }
            else if (Math.Abs(source.unit.X + target.unit.X) < tolerance &&
               Math.Abs(source.unit.Y + target.unit.Y) < tolerance &&
               Math.Abs(source.unit.Z + target.unit.Z) < tolerance)
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }

        #endregion

        #region 方程式
        

        #endregion
    }
}
