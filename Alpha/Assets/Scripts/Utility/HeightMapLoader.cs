using System;
using UnityEngine;
using UnityEngine.UI;
using TerrainView;

namespace Utility
{
    class HeightMapLoader : MonoBehaviour
    {
        public Text inputText = null;

        public void LoadHeightMap()
        {
            TerrainControl.Instance.LoadHeightmap(inputText.text);
            inputText.text = "";
        }
    }
}
