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
    public class Point
    {
        #region Property

        private double x = 0;

        public double X { get { return x; } set { x = value; } }

        private double y = 0;

        public double Y { get { return y; } set { y = value; } }

        private double z = 0;

        public double Z { get { return z; } set { z = value; } }

        #endregion

        public Point(double[] value)
        {
            x = value[0];
            y = value[1];
            z = value[2];
        }

        public Point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point(SketchPoint point)
        {
            this.x = point.X;
            this.y = point.Y;
            this.z = point.Z;
        }
    }

    public class Vector
    {
        #region property

        private double x = 0;

        public double X { get { return x; } set { x = value; } }

        private double y = 0;

        public double Y { get { return y; } set { y = value; } }

        private double z = 0;

        public double Z { get { return z; } set { z = value; } }

        public double Len { get { return Math.Sqrt(x * x + y * y + z * z); } }

        public Vector unit { get { return new Vector(x / Len, y / Len, z / Len); } }

        #endregion

        public Vector(double[] value)
        {
            x = value[0];
            y = value[1];
            z = value[2];
        }

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
                
    }

    public class Line
    {
        #region property

        private double color = 0;

        public double Color { get { return color; } }

        private double type = -1;

        public double Type { get { return type; } }

        private double font = 0;

        public double Font { get { return font; } }

        private double width = 0;

        public double Width { get { return width; } }

        private double layer = -1;

        public double Layer { get { return layer; } }

        private double layerOverride = 0;

        public double LayerOverride { get { return layerOverride; } }

        private Point start = new Point(0,0,0);
        
        public Point Start { get { return start; } }

        private Point end = new Point(0, 0, 0);

        public Point End { get { return end; } }

        private Vector dir = new Vector(0, 0, 0);

        /// <summary>
        /// 不是单位化的，相当于Vector
        /// </summary>
        public Vector Dir { get { return dir; } set { dir = value; } }

        private double len = 0;

        public double Len { get { return len; } }

        #endregion
        
        /// <summary>
        /// 从Sketch.GetLines2数组中获得数据
        /// </summary>
        /// <param name="value"></param>
        public Line(double[] value)
        {
            //Color, Type, Line Font, Line Width, Layer ID, Layer Override, [Start], [End] 
            color = value[0];
            type = value[1];
            font = value[2];
            width = value[3];
            layer = value[4];
            layerOverride = value[5];
            // In meters
            start = new Point(new double[] { value[6], value[7], value[8] });
            end = new Point(new double[] { value[9], value[10], value[11] });

            dir = new Vector(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
            len = Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y + dir.Z * dir.Z);
        }
        
        public Line(SketchSegment segment)
        {
            if (segment.GetType() != (int)swSketchSegments_e.swSketchLINE) return;

            ICurve curve = segment.GetCurve();
            //In meters
            double[] value = curve.LineParams;

            start = new Point(new double[] { value[0], value[1], value[2] });
            len = segment.GetLength();
            dir = new Vector(new double[] { value[3], value[4], value[5] });//可能是单位化的
            dir = dir.unit;
            dir.X *= len;
            dir.Y *= len;
            dir.Z *= len;
            end = new Point(start.X + dir.X, start.Y + dir.Y, start.Z + dir.Z);
            
        }

        public Line(SketchLine segment)
        {
            start = new Point(segment.GetStartPoint2());
            end = new Point(segment.GetEndPoint2());
            dir = new Vector(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
            len = Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y + dir.Z * dir.Z);
        }

        public Line(Point start, Point end)
        {
            this.start = start;
            this.end = end;

            dir = new Vector(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
            len = Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y + dir.Z * dir.Z);
        }
    }
}
