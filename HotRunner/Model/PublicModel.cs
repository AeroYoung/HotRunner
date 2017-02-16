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

        public Point(double value)
        { }
    }

    public class Line
    {
        //Color, Type, Line Font, Line Width, Layer ID, Layer Override, [Start], [End] 

        private double color;

        public double Color { get { return color; } }

        private double type;

        public double Type { get { return type; } }

        private double font;

        public double Font { get { return font; } }

        private double width;

        public double Width { get { return width; } }

        private double layer;

        public double Layer { get { return layer; } }

        private double layerOverride;

        public double LayerOverride { get { return layerOverride; } }
                

        public Line(double[] value)
        { }
    }
}
