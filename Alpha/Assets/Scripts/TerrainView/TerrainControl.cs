using System.Collections;
using Utility.TerrainAlgorithm;
using Utility;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using Utility.TerrainData;
using Utility.HeatAlgorithm;

namespace TerrainView
{
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
        public Terrain heatLayer;

        // Mapas de altura
        public float[,] RockMap { get; set; }
        public float[,] SoilMap { get; set; }
        public float[,] WaterMap { get; set; }

        // Mapas auxiliares
        public int[,] SurfaceMap { get; set; }
        public float[,] HumidityMap { get; set; }

        // Dados da simulação
        public TransformSet transformSet { get; private set; }

        private int LastSimulationTime { get; set; }
        public int SimulationInterval { get; set; }

        public EditConfigs EditConfigs { get; set; }

        // Mapa de calor
        public HeatCalculator HeatCalculator { get; set; }

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

            transformSet = new TransformSet();
            HeatCalculator = new HeatCalculator() { Type = HeatTypes.None };

            int res = 513;
            RockMap = new float[res, res];
            SoilMap = new float[res, res];
            WaterMap = new float[res, res];
            SurfaceMap = new int[res, res];
            HumidityMap = new float[res, res];

            LoadMaps();

            SimulationInterval = 5000;
            EditConfigs = new EditConfigs();

            UpdateTransformMaps();
            UpdateHeatCalculatorMaps();
            UpdateView(true);
        }

        public void LoadSoilHeightmap(string filename, float minDepth)
        {
            byte[] image = File.ReadAllBytes(filename);

            // Load heightmap.
            Texture2D heightmap = new Texture2D(2, 2);
            heightmap.LoadImage(image);

            float[,] OldRockMap = RockMap;

            // Acquire an array of colour values.
            Color[] values = heightmap.GetPixels();
            SoilMap = new float[heightmap.height, heightmap.width];
            RockMap = new float[heightmap.height, heightmap.width];
            WaterMap = new float[heightmap.height, heightmap.width];

            // Run through array and read height values.
            int index = 0;
            for (int z = 0; z < heightmap.height; z++)
            {
                for (int x = 0; x < heightmap.width; x++)
                {
                    SoilMap[z, x] = values[index].r;

                    if (z < OldRockMap.GetLength(0) && x < OldRockMap.GetLength(1))
                    {
                        RockMap[z, x] = OldRockMap[z, x];
                        if (RockMap[z, x] > SoilMap[z, x] - minDepth)
                            RockMap[z, x] = SoilMap[z, x] - minDepth;
                    }
                    else
                    {
                        RockMap[z, x] = 0;
                    }

                    WaterMap[z, x] = SoilMap[z, x];
                    index++;
                }
            }

            LoadMaps();
        }

        public void LoadRockHeightmap(string filename, float minDepth)
        {
            byte[] image = File.ReadAllBytes(filename);

            // Load heightmap.
            Texture2D heightmap = new Texture2D(2, 2);
            heightmap.LoadImage(image);

            float[,] OldSoilMap = SoilMap;

            // Acquire an array of colour values.
            Color[] values = heightmap.GetPixels();
            SoilMap = new float[heightmap.height, heightmap.width];
            RockMap = new float[heightmap.height, heightmap.width];
            WaterMap = new float[heightmap.height, heightmap.width];

            // Run through array and read height values.
            int index = 0;
            for (int z = 0; z < heightmap.height; z++)
            {
                for (int x = 0; x < heightmap.width; x++)
                {
                    RockMap[z, x] = values[index].r;

                    if (z < OldSoilMap.GetLength(0) && x < OldSoilMap.GetLength(1))
                    {
                        SoilMap[z, x] = OldSoilMap[z, x];
                        if (SoilMap[z, x] < RockMap[z, x] + minDepth)
                            SoilMap[z, x] = RockMap[z, x] + minDepth;
                    }
                    else
                    {
                        SoilMap[z, x] = RockMap[z, x] + minDepth;
                    }

                    WaterMap[z, x] = SoilMap[z, x];
                    index++;
                }
            }

            LoadMaps();
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
            SoilMap = new float[heightmap.height, heightmap.width];
            RockMap = new float[heightmap.height, heightmap.width];
            WaterMap = new float[heightmap.height, heightmap.width];

            // Run through array and read height values.
            int index = 0;
            for (int z = 0; z < heightmap.height; z++)
            {
                for (int x = 0; x < heightmap.width; x++)
                {
                    SoilMap[z, x] = values[index].r;
                    RockMap[z, x] = SoilMap[z, x] - minDepth;
                    WaterMap[z, x] = SoilMap[z, x];
                    index++;
                }
            }

            LoadMaps();
        }

        public void LoadMaps()
        {
            rockLayer.terrainData.heightmapResolution = RockMap.GetLength(0);
            rockLayer.terrainData.SetHeights(0, 0, RockMap);

            soilLayer.terrainData.heightmapResolution = SoilMap.GetLength(0);
            soilLayer.terrainData.alphamapResolution = SoilMap.GetLength(0);
            soilLayer.terrainData.SetHeights(0, 0, SoilMap);

            waterLayer.terrainData.heightmapResolution = SoilMap.GetLength(0);
            waterLayer.terrainData.alphamapResolution = SoilMap.GetLength(0);
            waterLayer.terrainData.SetHeights(0, 0, WaterMap);

            heatLayer.terrainData.heightmapResolution = SoilMap.GetLength(0);
            heatLayer.terrainData.alphamapResolution = SoilMap.GetLength(0);
            heatLayer.terrainData.SetHeights(0, 0, SoilMap);

            ResetAllTransforms();
            UpdateView(true);
        }

        // ----------------

        void Update()
        {
            if (GameControl.Instance.BackgroundMode)
                return;

            int currentTime = Environment.TickCount;
            if (currentTime - LastSimulationTime >= SimulationInterval)
            {
                RunAllTransforms();
                RunHeatCalculation();
                LastSimulationTime = currentTime;
            }

            UpdateView();
        }

        void FixedUpdate()
        {
            MouseSetSoilType();

            soilLayer.ApplyDelayedHeightmapModification();
            waterLayer.ApplyDelayedHeightmapModification();
            heatLayer.ApplyDelayedHeightmapModification();
        }

        private void RunAllTransforms()
        {
            foreach (TerrainTransform transform in transformSet.transformSet)
            {
                if (transform.IsActive())
                {
                    transform.ApplyTransform();
                    UpdateMeshes = transform.UpdateMeshes;
                    UpdateShades = transform.UpdateShades;
                    UpdateTextures = transform.UpdateTextures;
                    transform.ResetUpdateStates();
                }
            }

            if (UpdateMeshes)
            {
                if (GameControl.Instance.HeatMode == HeatTypes.None)
                {
                    soilLayer.terrainData.SetHeightsDelayLOD(0, 0, SoilMap);
                    waterLayer.terrainData.SetHeightsDelayLOD(0, 0, WaterMap);
                }
                else
                {
                    heatLayer.terrainData.SetHeightsDelayLOD(0, 0, SoilMap);
                }
            }
        }

        private void RunHeatCalculation()
        {
            if (HeatCalculator.Type != HeatTypes.None)
            {
                HeatCalculator.CalculateHeat();
            }
        }

        private void UpdateTransformMaps()
        {
            transformSet.SetSoilMap(SoilMap);
            transformSet.SetRockMap(RockMap);
            transformSet.SetWaterMap(WaterMap);
            transformSet.SetSurfaceMap(SurfaceMap);
            transformSet.SetHumidityMap(HumidityMap);
        }

        private void UpdateHeatCalculatorMaps()
        {
            HeatCalculator.SoilMap = SoilMap;
            HeatCalculator.RockMap = RockMap;
            HeatCalculator.WaterMap = WaterMap;
            HeatCalculator.SurfaceMap = SurfaceMap;
            HeatCalculator.HumidityMap = HumidityMap;
        }

        private void ResetAllTransforms()
        {
            foreach (TerrainTransform transform in transformSet.transformSet)
            {
                transform.Reset();
            }

            UpdateTransformMaps();
            UpdateHeatCalculatorMaps();
        }

        void UpdateView(bool forceUpdate = false)
        {
            if (forceUpdate)
                SetAllUpdates(true);

            if (HeatCalculator.Type == HeatTypes.None)
            {
                UpdateSoilTexture();
                UpdateWaterShade();
            }
            else
            {
                UpdateHeatMap();
            }

            SetAllUpdates(false);
        }

        public void SetHeatMode(HeatTypes mode)
        {
            bool active = mode != HeatTypes.None;

            HeatCalculator.Type = mode;

            waterLayer.gameObject.SetActive(!active);
            soilLayer.gameObject.SetActive(!active);
            rockLayer.gameObject.SetActive(!active);

            heatLayer.gameObject.SetActive(active);

            RunHeatCalculation();
            UpdateView(true);
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

                                if (z == SurfaceMap[x, y])
                                {
                                    value = 1.0f;
                                }
                                else if (z == shadeIndex)
                                {
                                    value = 1.0f - HumidityMap[x, y];
                                }

                                alphaMap[x, y, z] = value;
                            }
                        }
                        else
                        {
                            alphaMap[x, y, shadeIndex] = 1.0f - HumidityMap[x, y];
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
                        float waterMass = WaterMap[x, y] - SoilMap[x, y];
                        alphaMap[x, y, 1] = 1.0f - (waterMass * 10);
                    }
                }

                waterLayer.terrainData.SetAlphamaps(0, 0, alphaMap);
            }
        }

        private void UpdateHeatMap()
        {
            if (UpdateShades || UpdateMeshes || UpdateTextures)
            {
                float[,] map = HeatCalculator.HeatMap;
                float[, ,] alphaMap = heatLayer.terrainData.GetAlphamaps(0, 0, heatLayer.terrainData.alphamapWidth, heatLayer.terrainData.alphamapHeight);

                for (int x = 0; x < map.GetLength(0); x++)
                {
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        alphaMap[x, y, 0] = 1.0f - map[x, y];
                        alphaMap[x, y, 1] = map[x, y];
                    }
                }

                heatLayer.terrainData.SetAlphamaps(0, 0, alphaMap);
            }
        }

        // ----------------

        private void MouseSetSoilType()
        {
            if (GameControl.Instance.BackgroundMode)
                return;

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
            int maxX = SoilMap.GetLength(0);
            int maxZ = SoilMap.GetLength(1);

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

                    SurfaceMap[curZ, curX] = type;
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

            int terX = (int)((point.x / soilLayer.terrainData.size.x) * SoilMap.GetLength(0));
            int terZ = (int)((point.z / soilLayer.terrainData.size.z) * SoilMap.GetLength(1));
            float y = SoilMap[terX, terZ];
            y += 0.001f;
            float[,] height = new float[1, 1];
            height[0, 0] = y;
            SoilMap[terX, terZ] = y;
            soilLayer.terrainData.SetHeights(terX, terZ, height);
        }

        private void LowerTerrain(Vector3 point)
        {
            // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

            int terX = (int)((point.x / soilLayer.terrainData.size.x) * SoilMap.GetLength(0));
            int terZ = (int)((point.z / soilLayer.terrainData.size.z) * SoilMap.GetLength(1));
            float y = SoilMap[terX, terZ];
            y -= 0.001f;
            float[,] height = new float[1, 1];
            height[0, 0] = y;
            SoilMap[terX, terZ] = y;
            soilLayer.terrainData.SetHeights(terX, terZ, height);
        }
    }
}