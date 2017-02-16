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
    }

    public class Line
    {
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

        private Point start;
        
        public Point Start { get { return start; } }

        private Point end;

        public Point End { get { return end; } }

        public Line(double[] value)
        {
            //Color, Type, Line Font, Line Width, Layer ID, Layer Override, [Start], [End] 
            color = value[0];
            type = value[1];
            font = value[2];
            width = value[3];
            layer = value[4];
            layerOverride = value[5];
        }
    }
}
