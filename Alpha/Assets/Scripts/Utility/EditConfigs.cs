using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainView;

namespace Utility
{
    public class EditConfigs
    {
        public SurfaceType? SurfacePaintMode { get; set; }

        public int BrushSize { get; set; }

        public EditConfigs()
        {
            SurfacePaintMode = null;
            BrushSize = 1;
        }
    }
}
