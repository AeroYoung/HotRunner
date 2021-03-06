﻿using SolidWorks.Interop.sldworks;
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

        #region 尺寸标注

        public static void DimensionWith(this SketchLine l1, SketchLine l2, double value,string linkName, SldWorks swApp)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            bool bl = false;
            swDoc.ClearSelection2(true);
            
            bl = ((SketchSegment)l1).Select(false);
            bl = ((SketchSegment)l2).Select(true);

            Point textPoint = new Point((l1.toLine().Start.X + l2.toLine().Start.X) / 2,
                (l1.toLine().Start.Y + l2.toLine().Start.Y) / 2,
                (l1.toLine().Start.Z + l2.toLine().Start.Z) / 2);

            swDoc.AddDimension2(textPoint.X, textPoint.Y, 0);
            
            swDoc.ClearSelection2(true);
        }

        public static void DimensionWith(this SketchLine l1, SketchLine l2,string linkName, SldWorks swApp)
        {
            
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;
            bool bl = false;
                swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate,
                false);
            swDoc.ClearSelection2(true);

            bl = ((SketchSegment)l1).Select(false);
            bl = ((SketchSegment)l2).Select(true);
            
            Point textPoint = new Point((l1.toLine().Start.X + l2.toLine().Start.X) / 2,
                (l1.toLine().Start.Y + l2.toLine().Start.Y) / 2,
                (l1.toLine().Start.Z + l2.toLine().Start.Z) / 2);

            DisplayDimension disDim = swDoc.AddDimension2(textPoint.X, textPoint.Y, 0);

            disDim.SetLinkedText("" + linkName + "");

            swDoc.ClearSelection2(true);
            swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate,
                true);
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

        public static bool isParallerTo(this SketchLine l1, SketchLine l2, double tolerance)
        {
            Vector source = l1.toLine().Dir;
            Vector target = l2.toLine().Dir;

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

        public static double DistanceTo(this SketchLine l1, SketchLine l2)
        {
            Point p1 = l1.toLine().Start;
            Point p2 = l1.toLine().End;
            Point p3 = l2.toLine().Start;

            Vector v1 = new Vector(p1, p2);
            Vector v2 = new Vector(p1, p3);

            Vector s = new Vector(
                v1.Y*v2.Z-v1.Z*v2.Y,
                v1.Z*v2.X-v1.X*v2.Z,
                v1.X*v2.Y-v1.Y*v2.X);
            double area = s.Len;

            double h = area / l1.toLine().Len;
            return h;
        }

        public static double DistanceTo(this Point p1, Point p2)
        {
            double x = Math.Pow(p1.X - p2.X, 2);
            double y = Math.Pow(p1.Y - p2.Y, 2);
            double z = Math.Pow(p1.Z - p2.Z, 2);

            return Math.Sqrt(x + y + z);
        }

        #endregion

        #region 方程式

        /// <summary>
        /// 获取全局变量的方程式内容，若无则创建
        /// </summary>
        /// <param name="swApp"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetFunction(this SldWorks swApp,string variableName,string defaultFunction)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;

            IEquationMgr equationMgr = swDoc.GetEquationMgr();
            int count = equationMgr.GetCount();

            for (int i = count - 1; i > -1; i--)
            {
                string equation = equationMgr.Equation[i];

                if (equation == null || !equation.Contains("\"") || !equation.Contains("="))
                    continue;

                int s = equation.IndexOf("\"") + 1;
                int e = equation.IndexOf("=");
                string name = equation.Substring(s, e - s).Trim();
                e = name.LastIndexOf("\"");
                name = name.Substring(0, e).Trim();

                if (name == variableName)
                {
                    string function = equation.Substring(equation.IndexOf("=") + 1);
                    
                    return function;
                }                
            }

            //若无，或者转换失败则添加全局变量
            string newEquation = "\"" + variableName + "\"" + " = " + defaultFunction;
            equationMgr.Add2(-1, newEquation, false);

            return defaultFunction;
        }

        /// <summary>
        /// 获取计算后的值，若无则创建
        /// </summary>
        /// <param name="swApp"></param>
        /// <param name="variableName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double GetGlobalVariableValue(this SldWorks swApp, string variableName, double defaultValue)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;

            IEquationMgr equationMgr = swDoc.GetEquationMgr();
            //equationMgr.EvaluateAll();
            int count = equationMgr.GetCount();

            for (int i = count - 1; i > -1; i--)
            {
                string equation = equationMgr.Equation[i];

                if (equation == null || !equation.Contains("\"") || !equation.Contains("="))
                    continue;

                int s = equation.IndexOf("\"") + 1;
                int e = equation.IndexOf("=");
                string name = equation.Substring(s, e - s).Trim();
                e = name.LastIndexOf("\"");
                name = name.Substring(0, e).Trim();

                if (name == variableName)
                    return equationMgr.Value[i];
                
            }

            //若无，或者转换失败则添加全局变量
            string newEquation = "\"" + variableName + "\"" + " = " + defaultValue.ToString();
            equationMgr.Add2(-1, newEquation, false);

            return defaultValue;
        }
        
        /// <summary>
        /// 更新或者创建全局变量值
        /// </summary>
        /// <param name="swApp"></param>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public static void SetGlobalVariable(this SldWorks swApp, string variableName, double value)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;

            IEquationMgr equationMgr = swDoc.GetEquationMgr();
            int count = equationMgr.GetCount();

            string equation = "";

            for (int i = count - 1; i > -1; i--)
            {
                equation = equationMgr.Equation[i];

                if (equation == null || !equation.Contains("\"") || !equation.Contains("="))
                    continue;

                int s = equation.IndexOf("\"") + 1;
                int e = equation.IndexOf("=");
                string name = equation.Substring(s, e - s).Trim();
                e = name.LastIndexOf("\"");
                name = name.Substring(0, e).Trim();

                if (name == variableName)
                    equationMgr.Delete(i);
            }


            equation = "\"" + variableName + "\"" + " = " + value.ToString();
            equationMgr.Add2(-1, equation, false);

            return;
        }

        /// <summary>
        /// 函数自动在=后面加上引号
        /// </summary>
        /// <param name="swApp"></param>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public static void SetGlobalVariable(this SldWorks swApp, string variableName, string value)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;

            IEquationMgr equationMgr = swDoc.GetEquationMgr();
            int count = equationMgr.GetCount();

            string equation = "";

            for (int i = count - 1; i > -1 ; i--)
            {
                equation = equationMgr.Equation[i];

                if (equation == null || !equation.Contains("\"") || !equation.Contains("="))
                    continue;

                int s = equation.IndexOf("\"") + 1;
                int e = equation.IndexOf("=");
                string name = equation.Substring(s, e - s).Trim();
                e = name.LastIndexOf("\"");
                name = name.Substring(0, e).Trim();

                if (name == variableName)
                {
                    equationMgr.Delete(i);
                }
            }

            equation = "\"" + variableName + "\"" + " = \"" + value.ToString() + "\"";
            equationMgr.Add2(-1, equation, false);
        }

        /// <summary>
        /// 函数不在=后面加上引号，用于方程式
        /// </summary>
        /// <param name="swApp"></param>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public static void SetFunction(this SldWorks swApp, string variableName, string value)
        {
            ModelDoc2 swDoc = (ModelDoc2)swApp.ActiveDoc;

            IEquationMgr equationMgr = swDoc.GetEquationMgr();
            int count = equationMgr.GetCount();

            string equation = "";

            for (int i = count - 1; i > -1; i--)
            {
                equation = equationMgr.Equation[i];

                if (equation == null || !equation.Contains("\"") || !equation.Contains("="))
                    continue;

                int s = equation.IndexOf("\"") + 1;
                int e = equation.IndexOf("=");
                string name = equation.Substring(s, e - s).Trim();
                e = name.LastIndexOf("\"");
                name = name.Substring(0, e).Trim();

                if (name == variableName)
                {
                    equationMgr.Delete(i);
                }
            }

            equation = "\"" + variableName + "\"" + " = " + value.ToString() + "";
            equationMgr.Add2(-1, equation, false);
        }
        
        #endregion
    }
}