using System.Collections;
using Utility.TerrainAlgorithm;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace TerrainView
{
    public class TerrainControl : MonoBehaviour
    {
        /*
         * Classe geral de controle e acesso ao terreno
         */

        // Singleton
        public static TerrainControl Instance { get; private set; }

        public Terrain soilLayer;
        public Terrain waterLayer;

        public float[,] heights;

        public TransformSet transformSet { get; private set; }

        // Parâmetros UI
        public Text textMass = null;

        void Start()
        {
            Instance = this;

            int x = soilLayer.terrainData.heightmapWidth;
            int z = soilLayer.terrainData.heightmapHeight;
            heights = soilLayer.terrainData.GetHeights(0, 0, x, z);

            transformSet = new TransformSet();
        }

        public void LoadHeightmap(string filename)
        {
            // http://damienclassen.blogspot.com.br/2014/02/loading-terrain-heightmap-data-via-c.html

            byte[] image = File.ReadAllBytes(filename);

            // Load heightmap.
            Texture2D heightmap = new Texture2D(2,2);
            heightmap.LoadImage(image);

            // Acquire an array of colour values.
            Color[] values = heightmap.GetPixels();
            heights = new float[heightmap.height, heightmap.width];

            // Run through array and read height values.
            int index = 0;
            for (int z = 0; z < heightmap.height; z++)
            {
                for (int x = 0; x < heightmap.width; x++)
                {
                    heights[z, x] = values[index].r;
                    index++;
                }
            }

            // Now set terrain heights.
            soilLayer.terrainData.heightmapResolution = heightmap.height;
            soilLayer.terrainData.SetHeights(0, 0, heights);

            waterLayer.terrainData.heightmapResolution = heightmap.height;

            ResetAllTransforms();
        }

        public void LoadHeights(float[,] newHeights)
        {
            heights = newHeights;
            soilLayer.terrainData.heightmapResolution = newHeights.GetLength(0);
            soilLayer.terrainData.SetHeights(0, 0, newHeights);

            waterLayer.terrainData.heightmapResolution = newHeights.GetLength(0);

            ResetAllTransforms();
        }

        void Update()
        {
            RunAllTransforms();
        }

        void FixedUpdate()
        {
            soilLayer.ApplyDelayedHeightmapModification();
        }

        private void RunAllTransforms()
        {
            if (GameControl.Instance.BackgroundMode)
                return;

            foreach (TerrainTransform transform in transformSet.transformSet)
            {
                if (transform.IsActive())
                {
                    transform.ApplyTransform(ref heights);
                }
            }

            soilLayer.terrainData.SetHeightsDelayLOD(0, 0, heights);

            UpdateWaterLayer();
            UpdateMass();
        }

        private void ResetAllTransforms()
        {
            foreach (TerrainTransform transform in transformSet.transformSet)
            {
                transform.Reset();
            }
        }

        private void UpdateMass()
        {
            if (textMass != null)
            {
                float sumHeights = 0.0f;
                foreach (float height in heights)
                {
                    sumHeights += height;
                }
                textMass.text = sumHeights.ToString();
            }
        }

        private void UpdateWaterLayer()
        {
            HydroErosionTransform hydro = transformSet[TransformIndex.HydroErosion] as HydroErosionTransform;
            waterLayer.gameObject.SetActive(hydro.Configs.Active);
            waterLayer.terrainData.SetHeightsDelayLOD(0, 0, hydro.GetWaterMatrix(heights));
        }

        private void MouseEditTerrain()
        {
            // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    RaiseTerrain(hit.point);
                }
            }
            if (Input.GetMouseButton(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    LowerTerrain(hit.point);
                }
            }
        }

        private void RaiseTerrain(Vector3 point)
        {
            // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

            int terX = (int)((point.x / soilLayer.terrainData.size.x) * heights.GetLength(0));
            int terZ = (int)((point.z / soilLayer.terrainData.size.z) * heights.GetLength(1));
            float y = heights[terX, terZ];
            y += 0.001f;
            float[,] height = new float[1, 1];
            height[0, 0] = y;
            heights[terX, terZ] = y;
            soilLayer.terrainData.SetHeights(terX, terZ, height);
        }

        private void LowerTerrain(Vector3 point)
        {
            // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

            int terX = (int)((point.x / soilLayer.terrainData.size.x) * heights.GetLength(0));
            int terZ = (int)((point.z / soilLayer.terrainData.size.z) * heights.GetLength(1));
            float y = heights[terX, terZ];
            y -= 0.001f;
            float[,] height = new float[1, 1];
            height[0, 0] = y;
            heights[terX, terZ] = y;
            soilLayer.terrainData.SetHeights(terX, terZ, height);
        }
    }
}