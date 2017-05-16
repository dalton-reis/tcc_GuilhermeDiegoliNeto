using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainView;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Utility.TerrainData;

namespace EditConfigsScreen
{
    class UIControl : MonoBehaviour
    {
        public static UIControl Instance { get; private set; }
        public static EditConfigs EditConfigs { get; set; }

        public Dropdown dropdownSurface = null;
        public Slider sliderBrushSize = null;

        // Use this for initialization
        void Start()
        {
            Instance = this;

            SetConfigs(EditConfigs);
        }

        public void SetConfigs(EditConfigs configs)
        {
            sliderBrushSize.value = (float)configs.BrushSize;

            if (configs.SurfacePaintMode == null)
            {
                dropdownSurface.value = 0;
            }
            else
            {
                dropdownSurface.value = (int)(configs.SurfacePaintMode + 1);
            }
        }

        public EditConfigs GetConfigs()
        {
            EditConfigs configs = new EditConfigs();
            configs.BrushSize = (int)sliderBrushSize.value;

            if (dropdownSurface.value > 0)
            {
                configs.SurfacePaintMode = (SurfaceType)(dropdownSurface.value - 1);
            }

            return configs;
        }
    }
}
