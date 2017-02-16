using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotRunner
{
    public class Point
    {
        private double x = 0;

        public double X { get { return x; } }

        private double y = 0;

        public double Y { get { return y; } }

        private double z = 0;

        public double Z { get { return z; } }

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
    }

    public class Vector
    {
        private double x = 0;

        public double X { get { return x; } }

        private double y = 0;

        public double Y { get { return y; } }

        private double z = 0;

        public double Z { get { return z; } }

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
        
        public Vector Dir { get { return new Vector(end.X-start.X, end.Y - start.Y, end.Z - start.Z)} }       

        public Line(double[] value)
        {
            //Color, Type, Line Font, Line Width, Layer ID, Layer Override, [Start], [End] 
            color = value[0];
            type = value[1];
            font = value[2];
            width = value[3];
            layer = value[4];
            layerOverride = value[5];
            start = new Point(new double[] { value[6], value[7], value[8] });
            end = new Point(new double[] { value[9], value[10], value[11] });
        }
    }
}
