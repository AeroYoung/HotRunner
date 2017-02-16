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
    public static class SwFunction
    {
        public static List<Line> GetLines(double[] value, int count)
        {
            List<Line> result = new List<Line>();

            for (int i = 0; i < count; i++)
            {
                double[] a = new double[12];
                for (int j = 0; j < 12; j++)
                {
                    a[j] = value[count * 12 + j];
                }
                result.Add(new Line(a));
            }

            return result;
        }
    }
}
