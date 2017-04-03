using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    /// <summary>
    /// Algoritmo para simular efeitos da erosão hidráulica.
    /// Mais detalhadamente, este algoritmo simula o acúmulo de água da chuva, recolhendo e depositando material conforme a corrente.
    /// Referência: http://web.mit.edu/cesium/Public/terrain.pdf (p. 8-11)
    /// </summary>
    public class HydroErosionTransform : TerrainTransform
    {
        public HydroErosionSimConfigs Configs { get; set; }

        private float[,] waterHeights = null;
        private int rainCounter = 0;

        public HydroErosionTransform()
        {
            Configs = new HydroErosionSimConfigs()
            {
                Active = false,
                RainIntensity = 0.0001f,
                RainInterval = 20,
                EvaporationFactor = 0.01f,
                TerrainSolubility = 0.01f
            };
        }

        public override bool IsActive()
        {
            return Configs.Active;
        }

        public override void Reset()
        {
            waterHeights = null;
        }

        public override void ApplyTransform(ref float[,] heights)
        {
            // Criar matriz de água se não existir
            if (waterHeights == null)
            {
                waterHeights = new float[heights.GetLength(0), heights.GetLength(1)];
            }

            // Distribuir água da chuva
            if (DistributeWater())
            {
                GroundToSediment(ref heights);
            }

            TransformVonNeumann(ref heights);

            EvaporateWater(ref heights);
        }

        public float[,] GetWaterMatrix(float[,] heights)
        {
            float[,] matrix = heights.Clone() as float[,];

            if (waterHeights != null)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    for (int y = 0; y < matrix.GetLength(1); y++)
                    {
                        matrix[x, y] += waterHeights[x, y];
                    }
                }
            }

            return matrix;
        }

        private void TransformVonNeumann(ref float[,] heights)
        {
            // Transformação usando vizinhança Von Neumann

            int topX = heights.GetLength(0);
            int topY = heights.GetLength(1);

            float[,] baseHeights = heights.Clone() as float[,];

            // Loop geral do mapa
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    // Altura do terreno + altura da água
                    float centerHeight = baseHeights[x, y] + waterHeights[x, y];

                    // Média das alturas
                    float avgHeight = 0;
                    int countHeights = 0;
                    
                    // Soma das diferenças de altura positivas
                    float totalDiff = 0;

                    // Loop horizontal
                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            continue;

                        float localHeight = (baseHeights[absX, y] + waterHeights[absX, y]);
                        float localDiff = centerHeight - localHeight;

                        if (localDiff < 0) continue;

                        totalDiff += localDiff;
                        avgHeight += localHeight;
                        countHeights++;
                    }
                    // Loop vertical
                    for (int relY = -1; relY <= 1; relY++)
                    {
                        int absY = y + relY;
                        if (absY < 0 || absY >= topY)
                            continue;

                        float localHeight = (baseHeights[x, absY] + waterHeights[x, absY]);
                        float localDiff = centerHeight - localHeight;

                        if (localDiff < 0) continue;

                        totalDiff += localDiff;
                        avgHeight += localHeight;
                        countHeights++;
                    }

                    // Descontar o centro que foi considerado duas vezes
                    avgHeight -= centerHeight;
                    countHeights--;
                    avgHeight /= countHeights;

                    float deltaHeight = centerHeight - avgHeight;

                    float totalDeltaWater = 0;

                    // Loop horizontal
                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX || absX == x)
                            continue;

                        float localHeight = (baseHeights[absX, y] + waterHeights[absX, y]);

                        if (localHeight >= centerHeight)
                            continue;

                        float localDiff = centerHeight - localHeight;
                        float deltaWater = Math.Min(waterHeights[x, y], deltaHeight) * (localDiff / totalDiff);

                        waterHeights[absX, y] += deltaWater;
                        totalDeltaWater += deltaWater;
                    }
                    // Loop vertical
                    for (int relY = -1; relY <= 1; relY++)
                    {
                        int absY = y + relY;
                        if (absY < 0 || absY >= topY || absY == y)
                            continue;

                        float localHeight = (baseHeights[x, absY] + waterHeights[x, absY]);

                        if (localHeight >= centerHeight)
                            continue;

                        float localDiff = centerHeight - localHeight;
                        float deltaWater = Math.Min(waterHeights[x, y], deltaHeight) * (localDiff / totalDiff);

                        waterHeights[x, absY] += deltaWater;
                        totalDeltaWater += deltaWater;
                    }

                    waterHeights[x, y] -= totalDeltaWater;
                }
            }
        }

        private bool DistributeWater()
        {
            rainCounter++;
            if (rainCounter == Configs.RainInterval)
            {
                rainCounter = 0;

                // Loop geral do mapa
                for (int x = 0; x < waterHeights.GetLength(0); x++)
                {
                    for (int y = 0; y < waterHeights.GetLength(1); y++)
                    {
                        waterHeights[x, y] += Configs.RainIntensity;
                    }
                }

                return true;
            }

            return false;
        }

        private void GroundToSediment(ref float[,] heights)
        {
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    heights[x, y] -= Configs.TerrainSolubility * waterHeights[x, y];
                }
            }
        }

        private void EvaporateWater(ref float[,] heights)
        {
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    float diff = waterHeights[x, y] - (waterHeights[x, y] * (1 - Configs.EvaporationFactor));
                    waterHeights[x, y] -= diff;
                    heights[x, y] -= Configs.TerrainSolubility * diff;
                }
            }
        }
    }
}
