using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace K_mean
{
    public class data
    {
        public Point point;
        public int set=0;
        
        // constructor
        public data(Point _point)
        {
            point = _point;
        }
        // constructor
        public data(Point _point,int _set)
        {
            point = _point;
            set = _set;
        }
    }
}
