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
        /// <summary>
        /// 从数组中获得Line
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
        /// 得到草图线段中的直线
        /// </summary>
        /// <param name="swApp"></param>
        /// <param name="sketch">草图</param>
        /// <returns></returns>
        public static List<SketchSegment> GetSegmentLine(SldWorks swApp,Sketch sketch)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            SelectionMgr swSelMgr = (SelectionMgr)swDoc.SelectionManager;

            List<SketchSegment> segments = new List<SketchSegment>();
            //SelectData data = swSelMgr.CreateSelectData();            
            EnumSketchSegments enumSegments = sketch.IEnumSketchSegments();
            if (enumSegments != null)
            {
                SketchSegment segment;
                int next = 1;
                enumSegments.Next(1, out segment, ref next);

                while (segment != null)
                {
                    if (segment.GetType() == (int)swSketchSegments_e.swSketchLINE)
                        segments.Add(segment);
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
                true,
                (int)swStartConditions_e.swStartSketchPlane,
                0.0,
                false
                );

        }
    }
}
