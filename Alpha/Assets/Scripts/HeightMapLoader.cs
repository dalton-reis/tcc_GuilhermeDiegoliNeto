using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class HeightMapLoader : MonoBehaviour
    {
        public TerrainControl terrainControl = null;
        public Text inputText = null;

        public void LoadHeightMap()
        {
            terrainControl.LoadHeightmap(inputText.text);
            inputText.text = "";
        }
    }
}
