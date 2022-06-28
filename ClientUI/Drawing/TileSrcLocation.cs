using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Drawing
{
    internal struct TileSrcLocation
    {
        internal Point position;
        internal Point widthHeight;

        internal TileSrcLocation(Point position, Point widthHeight)
        {
            this.position = position;
            this.widthHeight = widthHeight;
        }

        internal TileSrcLocation(TileSrcLocation copy)
        {
            position = new Point(copy.position.X, copy.position.Y);
            widthHeight = new Point(copy.widthHeight.X, copy.widthHeight.Y);
        }
    }
}
