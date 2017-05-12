using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.TerrainData;

namespace StatsScreen
{
    class StatCollector
    {
        // Dados de entrada
        public float[,] WaterMap { private get; set; }
        public float[,] SoilMap { private get; set; }
        public float[,] RockMap { private get; set; }
        public int[,] SurfaceMap { private get; set; }
        public float[,] HumidityMap { private get; set; }

        // Dados de saída
        public string[] Output { get; private set; }

        public StatCollector()
        {
            Output = new string[(int)Stats.Count];
        }

        public void CollectStats()
        {
            float soilMass = 0;
            float waterMass = 0;
            float humidityMass = 0;
            float highSoil = 0;
            float highWater = 0;
            float highHumidity = 0;
            float highHeight = 0;
            float highIncline = 0;
            float lowSoil = 0;
            float lowWater = 0;
            float lowHumidity = 0;
            float lowHeight = 0;
            float lowIncline = 0;
            float avgSoil = 0;
            float avgWater = 0;
            float avgHumidity = 0;
            float avgHeight = 0;
            float avgIncline = 0;

            float totalHeight = 0;

            float totalIncline = 0;
            int inclineCount = 0;

            int[] surfaceCounter = new int[(int)SurfaceType.Count];

            int topX = SoilMap.GetLength(0);
            int topY = SoilMap.GetLength(1);
            int area = topX * topY;

            for (int x = 0; x < topX; x++)
            {
                for (int y = 0; y < topY; y++)
                {
                    float localSoil = SoilMap[x, y] - RockMap[x, y];
                    float localWater = WaterMap[x, y] - SoilMap[x, y];
                    float localHumidity = HumidityMap[x, y];
                    float localHeight = SoilMap[x, y];

                    if (localSoil < 0) localSoil = 0;
                    if (localWater < 0) localWater = 0;

                    List<float> localInclines = GetLocalInclines(x, y);

                    soilMass        += localSoil;
                    waterMass       += localWater;
                    humidityMass    += localHumidity;
                    totalHeight     += localHeight;

                    if (localSoil       > highSoil      ) highSoil      = localSoil;
                    if (localWater      > highWater     ) highWater     = localWater;
                    if (localHumidity   > highHumidity  ) highHumidity  = localHumidity;
                    if (localHeight     > highHeight    ) highHeight    = localHeight;

                    if (localSoil       < lowSoil       || lowSoil      == 0) lowSoil       = localSoil;
                    if (localWater      < lowWater      || lowWater     == 0) lowWater      = localWater;
                    if (localHumidity   < lowHumidity   || lowHumidity  == 0) lowHumidity   = localHumidity;
                    if (localHeight     < lowHeight     || lowHeight    == 0) lowHeight     = localHeight;

                    foreach (float incline in localInclines)
                    {
                        totalIncline += incline;
                        inclineCount++;

                        if (incline > highIncline) highIncline = incline;
                        if (incline < lowIncline || lowIncline == 0) lowIncline = incline;
                    }

                    surfaceCounter[SurfaceMap[x, y]]++;
                }
            }

            avgSoil     = soilMass      / area;
            avgWater    = waterMass     / area;
            avgHumidity = humidityMass  / area;
            avgHeight   = totalHeight   / area;
            avgIncline  = totalIncline  / inclineCount;

            Output[(int)Stats.SoilMass      ] = soilMass    .ToString();
            Output[(int)Stats.WaterMass     ] = waterMass   .ToString();
            Output[(int)Stats.HumidityMass  ] = humidityMass.ToString();
            Output[(int)Stats.HighSoil      ] = highSoil    .ToString();
            Output[(int)Stats.HighWater     ] = highWater   .ToString();
            Output[(int)Stats.HighHumidity  ] = highHumidity.ToString();
            Output[(int)Stats.HighHeight    ] = highHeight  .ToString();
            Output[(int)Stats.HighIncline   ] = highIncline .ToString();
            Output[(int)Stats.LowSoil       ] = lowSoil     .ToString();
            Output[(int)Stats.LowWater      ] = lowWater    .ToString();
            Output[(int)Stats.LowHumidity   ] = lowHumidity .ToString();
            Output[(int)Stats.LowHeight     ] = lowHeight   .ToString();
            Output[(int)Stats.LowIncline    ] = lowIncline  .ToString();
            Output[(int)Stats.AvgSoil       ] = avgSoil     .ToString();
            Output[(int)Stats.AvgWater      ] = avgWater    .ToString();
            Output[(int)Stats.AvgHumidity   ] = avgHumidity .ToString();
            Output[(int)Stats.AvgHeight     ] = avgHeight   .ToString();
            Output[(int)Stats.AvgIncline    ] = avgIncline  .ToString();
            Output[(int)Stats.MostSurface   ] = GetMostSurface(surfaceCounter);
        }

        // As inclinações são avaliadas de cima para baixo
        private List<float> GetLocalInclines(int x, int y)
        {
            List<float> inclines = new List<float>();

            if (x != 0)
            {
                float incline = SoilMap[x, y] - SoilMap[x - 1, y];
                if (incline > 0)
                    inclines.Add(incline);
            }
            if (y != 0)
            {
                float incline = SoilMap[x, y] - SoilMap[x, y - 1];
                if (incline > 0)
                    inclines.Add(incline);
            }
            if (x != SoilMap.GetLength(0) - 1)
            {
                float incline = SoilMap[x, y] - SoilMap[x + 1, y];
                if (incline > 0)
                    inclines.Add(incline);
            }
            if (y != SoilMap.GetLength(1) - 1)
            {
                float incline = SoilMap[x, y] - SoilMap[x, y + 1];
                if (incline > 0)
                    inclines.Add(incline);
            }

            return inclines;
        }

        private string GetMostSurface(int[] surfaceCounter)
        {
            int mostSurfaceIndex = 0;
            int mostSurfaceValue = 0;

            for (int i = 0; i < surfaceCounter.Length; i++)
            {
                if (surfaceCounter[i] > mostSurfaceValue)
                {
                    mostSurfaceIndex = i;
                    mostSurfaceValue = surfaceCounter[i];
                }
            }

            SurfaceType surface = (SurfaceType)mostSurfaceIndex;
            return surface.ToString();
        }
    }
}
