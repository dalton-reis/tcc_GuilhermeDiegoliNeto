using System.Collections;
using Utility.TerrainAlgorithm;
using Utility;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

namespace TerrainView
{
    public enum SurfaceType
    {
        Soil = 0,
        Grass,
        Plants,
        Concrete,
        Shade,
    }

    public class TerrainControl : MonoBehaviour
    {
        /*
         * Classe geral de controle e acesso ao terreno
         */

        // Singleton
        public static TerrainControl Instance { get; private set; }

        // Dados do terreno
        public Terrain rockLayer;
        public Terrain soilLayer;
        public Terrain waterLayer;

        public float[,] RockHeights { get; set; }
        public float[,] SoilHeights { get; set; }
        public float[,] WaterHeights { get; set; }

        public int[,] SoilTypes { get; set; }

        // Dados da simulação
        public TransformSet transformSet { get; private set; }

        private int LastSimulationTime { get; set; }
        public int SimulationInterval { get; set; }

        public EditConfigs EditConfigs { get; set; }

        // Parâmetros UI
        public Text textMass = null;

        // Estado da simulação
        bool UpdateTextures { get; set; }
        bool UpdateShades { get; set; }
        bool UpdateMeshes { get; set; }

        void SetAllUpdates(bool update)
        {
            UpdateTextures = update;
            UpdateShades = update;
            UpdateMeshes = update;
        }

        void Start()
        {
            Instance = this;

            int x = soilLayer.terrainData.heightmapWidth;
            int z = soilLayer.terrainData.heightmapHeight;

            RockHeights = rockLayer.terrainData.GetHeights(0, 0, x, z);
            SoilHeights = soilLayer.terrainData.GetHeights(0, 0, x, z);
            WaterHeights = waterLayer.terrainData.GetHeights(0, 0, x, z);

            SoilTypes = new int[SoilHeights.GetLength(0), SoilHeights.GetLength(1)];

            SimulationInterval = 5000;
            EditConfigs = new EditConfigs();

            transformSet = new TransformSet();

            UpdateViews(true);
        }

        public void LoadSoilHeightmap(string filename, float minDepth)
        {
            byte[] image = File.ReadAllBytes(filename);

            // Load heightmap.
            Texture2D heightmap = new Texture2D(2, 2);
            heightmap.LoadImage(image);

            float[,] OldRockHeights = RockHeights;

            // Acquire an array of colour values.
            Color[] values = heightmap.GetPixels();
            SoilHeights = new float[heightmap.height, heightmap.width];
            RockHeights = new float[heightmap.height, heightmap.width];
            WaterHeights = new float[heightmap.height, heightmap.width];

            SoilTypes = new int[SoilHeights.GetLength(0), SoilHeights.GetLength(1)];

            // Run through array and read height values.
            int index = 0;
            for (int z = 0; z < heightmap.height; z++)
            {
                for (int x = 0; x < heightmap.width; x++)
                {
                    SoilHeights[z, x] = values[index].r;

                    if (z < OldRockHeights.GetLength(0) && x < OldRockHeights.GetLength(1))
                    {
                        RockHeights[z, x] = OldRockHeights[z, x];
                        if (RockHeights[z, x] > SoilHeights[z, x] - minDepth)
                            RockHeights[z, x] = SoilHeights[z, x] - minDepth;
                    }
                    else
                    {
                        RockHeights[z, x] = 0;
                    }

                    WaterHeights[z, x] = SoilHeights[z, x];
                    index++;
                }
            }

            rockLayer.terrainData.heightmapResolution = RockHeights.GetLength(0);
            rockLayer.terrainData.SetHeights(0, 0, RockHeights);

            soilLayer.terrainData.heightmapResolution = heightmap.height;
            soilLayer.terrainData.SetHeights(0, 0, SoilHeights);

            waterLayer.terrainData.heightmapResolution = heightmap.height;
            waterLayer.terrainData.SetHeights(0, 0, WaterHeights);

            ResetAllTransforms();
            UpdateViews(true);
        }

        public void LoadRockHeightmap(string filename, float minDepth)
        {
            byte[] image = File.ReadAllBytes(filename);

            // Load heightmap.
            Texture2D heightmap = new Texture2D(2, 2);
            heightmap.LoadImage(image);

            float[,] OldSoilHeights = SoilHeights;

            // Acquire an array of colour values.
            Color[] values = heightmap.GetPixels();
            SoilHeights = new float[heightmap.height, heightmap.width];
            RockHeights = new float[heightmap.height, heightmap.width];
            WaterHeights = new float[heightmap.height, heightmap.width];

            SoilTypes = new int[SoilHeights.GetLength(0), SoilHeights.GetLength(1)];

            // Run through array and read height values.
            int index = 0;
            for (int z = 0; z < heightmap.height; z++)
            {
                for (int x = 0; x < heightmap.width; x++)
                {
                    RockHeights[z, x] = values[index].r;

                    if (z < OldSoilHeights.GetLength(0) && x < OldSoilHeights.GetLength(1))
                    {
                        SoilHeights[z, x] = OldSoilHeights[z, x];
                        if (SoilHeights[z, x] < RockHeights[z, x] + minDepth)
                            SoilHeights[z, x] = RockHeights[z, x] + minDepth;
                    }
                    else
                    {
                        SoilHeights[z, x] = RockHeights[z, x] + minDepth;
                    }

                    WaterHeights[z, x] = SoilHeights[z, x];
                    index++;
                }
            }

            rockLayer.terrainData.heightmapResolution = RockHeights.GetLength(0);
            rockLayer.terrainData.SetHeights(0, 0, RockHeights);

            soilLayer.terrainData.heightmapResolution = heightmap.height;
            soilLayer.terrainData.SetHeights(0, 0, SoilHeights);

            waterLayer.terrainData.heightmapResolution = heightmap.height;
            waterLayer.terrainData.SetHeights(0, 0, WaterHeights);

            ResetAllTransforms();
            UpdateViews(true);
        }

        public void LoadHeightmap(string filename, float minDepth)
        {
            // http://damienclassen.blogspot.com.br/2014/02/loading-terrain-heightmap-data-via-c.html

            byte[] image = File.ReadAllBytes(filename);

            // Load heightmap.
            Texture2D heightmap = new Texture2D(2,2);
            heightmap.LoadImage(image);

            // Acquire an array of colour values.
            Color[] values = heightmap.GetPixels();
            SoilHeights = new float[heightmap.height, heightmap.width];
            RockHeights = new float[heightmap.height, heightmap.width];
            WaterHeights = new float[heightmap.height, heightmap.width];

            SoilTypes = new int[SoilHeights.GetLength(0), SoilHeights.GetLength(1)];

            // Run through array and read height values.
            int index = 0;
            for (int z = 0; z < heightmap.height; z++)
            {
                for (int x = 0; x < heightmap.width; x++)
                {
                    SoilHeights[z, x] = values[index].r;
                    RockHeights[z, x] = SoilHeights[z, x] - minDepth;
                    WaterHeights[z, x] = SoilHeights[z, x];
                    index++;
                }
            }

            rockLayer.terrainData.heightmapResolution = RockHeights.GetLength(0);
            rockLayer.terrainData.SetHeights(0, 0, RockHeights);

            soilLayer.terrainData.heightmapResolution = heightmap.height;
            soilLayer.terrainData.SetHeights(0, 0, SoilHeights);

            waterLayer.terrainData.heightmapResolution = heightmap.height;
            waterLayer.terrainData.SetHeights(0, 0, WaterHeights);

            ResetAllTransforms();
            UpdateViews(true);
        }

        public void LoadHeights(float[,] soil, float[,]rock)
        {
            SoilHeights = soil;
            RockHeights = rock;
            // TODO: Salvar e carregar camada de água

            SoilTypes = new int[SoilHeights.GetLength(0), SoilHeights.GetLength(1)];

            rockLayer.terrainData.heightmapResolution = RockHeights.GetLength(0);
            rockLayer.terrainData.SetHeights(0, 0, RockHeights);

            soilLayer.terrainData.heightmapResolution = SoilHeights.GetLength(0);
            soilLayer.terrainData.SetHeights(0, 0, SoilHeights);

            waterLayer.terrainData.heightmapResolution = SoilHeights.GetLength(0);

            ResetAllTransforms();
            UpdateViews(true);
        }

        // ----------------

        void Update()
        {
            int currentTime = Environment.TickCount;
            if (currentTime - LastSimulationTime >= SimulationInterval)
            {
                RunAllTransforms();
                LastSimulationTime = currentTime;
            }

            UpdateViews();
        }

        void FixedUpdate()
        {
            MouseSetSoilType();

            soilLayer.ApplyDelayedHeightmapModification();
            waterLayer.ApplyDelayedHeightmapModification();
        }

        private void RunAllTransforms()
        {
            if (GameControl.Instance.BackgroundMode)
                return;

            foreach (TerrainTransform transform in transformSet.transformSet)
            {
                if (transform.IsActive())
                {
                    transform.ApplyTransform(RockHeights, SoilHeights);
                    UpdateMeshes = transform.UpdateMeshes;
                    UpdateShades = transform.UpdateShades;
                    UpdateTextures = transform.UpdateTextures;
                    transform.ResetUpdateStates();
                }
            }

            if (UpdateMeshes)
            {
                soilLayer.terrainData.SetHeightsDelayLOD(0, 0, SoilHeights);
            }
        }

        private void ResetAllTransforms()
        {
            foreach (TerrainTransform transform in transformSet.transformSet)
            {
                transform.Reset();
            }
        }

        void UpdateViews(bool forceUpdate = false)
        {
            if (forceUpdate)
                SetAllUpdates(true);

            UpdateWaterLayer();
            UpdateMass();
            UpdateSoilTexture();
            UpdateWaterShade();

            SetAllUpdates(false);
        }

        private void UpdateMass()
        {
            if (textMass != null)
            {
                float sumHeights = 0.0f;
                for (int x = 0; x < SoilHeights.GetLength(0); x++)
                {
                    for (int y = 0; y < SoilHeights.GetLength(1); y++)
                    {
                        sumHeights += (SoilHeights[x,y] - RockHeights[x, y]);
                    }
                }

                textMass.text = sumHeights.ToString();
            }
        }

        private void UpdateWaterLayer()
        {
            if (UpdateMeshes)
            {
                HydroErosionTransform hydro = transformSet[TransformIndex.HydroErosion] as HydroErosionTransform;
                waterLayer.gameObject.SetActive(hydro.Configs.Active);

                WaterHeights = hydro.GetWaterMatrix(SoilHeights);
                waterLayer.terrainData.SetHeightsDelayLOD(0, 0, WaterHeights);
            }
        }

        private void UpdateSoilTexture()
        {
            if (UpdateTextures || UpdateShades)
            {
                float[, ,] alphaMap = soilLayer.terrainData.GetAlphamaps(0, 0, soilLayer.terrainData.alphamapWidth, soilLayer.terrainData.alphamapHeight);
                int shadeIndex = (int)SurfaceType.Shade;

                for (int x = 0; x < alphaMap.GetLength(0); x++)
                {
                    for (int y = 0; y < alphaMap.GetLength(1); y++)
                    {
                        if (UpdateTextures)
                        {
                            for (int z = 0; z < alphaMap.GetLength(2); z++)
                            {
                                float value = 0;

                                if (z == SoilTypes[x, y])
                                {
                                    value = 1.0f;
                                }
                                else if (z == shadeIndex)
                                {
                                    value = 1.0f - ((SoilHeights[x, y] - RockHeights[x, y]) * 10);
                                }

                                alphaMap[x, y, z] = value;
                            }
                        }
                        else
                        {
                            alphaMap[x, y, shadeIndex] = 1.0f - ((SoilHeights[x, y] - RockHeights[x, y]) * 10);
                        }
                    }
                }

                soilLayer.terrainData.SetAlphamaps(0, 0, alphaMap);
            }
        }

        private void UpdateWaterShade()
        {
            if (UpdateShades)
            {
                float[, ,] alphaMap = waterLayer.terrainData.GetAlphamaps(0, 0, waterLayer.terrainData.alphamapWidth, waterLayer.terrainData.alphamapHeight);

                for (int x = 0; x < alphaMap.GetLength(0); x++)
                {
                    for (int y = 0; y < alphaMap.GetLength(1); y++)
                    {
                        float waterMass = WaterHeights[x, y] - SoilHeights[x, y];
                        alphaMap[x, y, 1] = 1.0f - (waterMass * 10);
                    }
                }

                waterLayer.terrainData.SetAlphamaps(0, 0, alphaMap);
            }
        }

        // ----------------

        private void MouseSetSoilType()
        {
            if (EditConfigs.SurfacePaintMode != null && Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    SetSoilType(hit.point);
                    UpdateTextures = true;
                }
            }
        }

        private void SetSoilType(Vector3 point)
        {
            int maxX = SoilHeights.GetLength(0);
            int maxZ = SoilHeights.GetLength(1);

            int begX = (int)((point.x / soilLayer.terrainData.size.x) * maxX);
            int begZ = (int)((point.z / soilLayer.terrainData.size.z) * maxZ);

            begX -= EditConfigs.BrushSize;
            begZ -= EditConfigs.BrushSize;

            int end = EditConfigs.BrushSize * 2;

            int type = (int)EditConfigs.SurfacePaintMode;

            for (int x = 0; x < end; ++x)
            {
                int curX = begX + x;
                if (curX < 0 || curX >= maxX)
                    continue;

                for (int z = 0; z < end; ++z)
                {
                    int curZ = begZ + z;
                    if (curZ < 0 || curZ >= maxZ)
                        continue;

                    SoilTypes[curZ, curX] = type;
                }
            }
        }

        // ----------------

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

            int terX = (int)((point.x / soilLayer.terrainData.size.x) * SoilHeights.GetLength(0));
            int terZ = (int)((point.z / soilLayer.terrainData.size.z) * SoilHeights.GetLength(1));
            float y = SoilHeights[terX, terZ];
            y += 0.001f;
            float[,] height = new float[1, 1];
            height[0, 0] = y;
            SoilHeights[terX, terZ] = y;
            soilLayer.terrainData.SetHeights(terX, terZ, height);
        }

        private void LowerTerrain(Vector3 point)
        {
            // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

            int terX = (int)((point.x / soilLayer.terrainData.size.x) * SoilHeights.GetLength(0));
            int terZ = (int)((point.z / soilLayer.terrainData.size.z) * SoilHeights.GetLength(1));
            float y = SoilHeights[terX, terZ];
            y -= 0.001f;
            float[,] height = new float[1, 1];
            height[0, 0] = y;
            SoilHeights[terX, terZ] = y;
            soilLayer.terrainData.SetHeights(terX, terZ, height);
        }
    }
}