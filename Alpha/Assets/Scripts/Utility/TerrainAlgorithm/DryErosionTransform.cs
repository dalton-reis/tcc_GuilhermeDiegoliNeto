using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    /// <summary>
    /// Algoritmo para simular efeitos da erosão térmica.
    /// Mais detalhadamente, este algoritmo simula material caindo de encostas inclinadas e sendo depositado na sua base.
    /// Referência: http://web.mit.edu/cesium/Public/terrain.pdf (p. 5-6)
    /// </summary>
    public class DryErosionTransform : TerrainTransform
    {
        public DryErosionSimConfigs Configs { get; set; }

        public DryErosionTransform()
        {
            Configs = new DryErosionSimConfigs()
            {
                Active = true,
                MaxInclination = 4.0 / 256,
                DistributionFactor = 0.5,
            };
        }

        public override bool IsActive()
        {
            return Configs.Active;
        }

        public override void ApplyTransform(ref float[,] heights)
        {
            TransformVonNeumann(ref heights);
        }

        public void TransformMoore(ref float[,] heights)
        {
            // Transformação usando vizinhança Moore

            int topX = heights.GetLength(0);
            int topY = heights.GetLength(1);

            float[,] baseHeights = heights.Clone() as float[,];

            // Loop geral do mapa
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    // Obter a maior inclinação e a soma das inclinações superiores à configuração
                    double maxInclination = 0.0;
                    double sumInclinations = 0.0;

                    // Loop interno dos vizinhos 
                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            continue;

                        for (int relY = -1; relY <= 1; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                continue;

                            double inclination = baseHeights[x, y] - baseHeights[absX, absY];
                            if (inclination > maxInclination)
                            {
                                maxInclination = inclination;
                            }
                            if (inclination > Configs.MaxInclination)
                            {
                                sumInclinations += inclination;
                            }
                        }
                    }

                    // Se não houver nada para alterar prosseguir imediatamente
                    if (sumInclinations == 0.0)
                        continue;

                    // Mover material para os vizinhos com inclinações superiores à configuração
                    float sumMovedMaterial = 0.0f;

                    // Loop interno dos vizinhos 
                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            continue;

                        for (int relY = -1; relY <= 1; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                continue;

                            double inclination = baseHeights[x, y] - baseHeights[absX, absY];
                            if (inclination > Configs.MaxInclination)
                            {
                                heights[absX, absY] = (float)(baseHeights[absX, absY] + (Configs.DistributionFactor * (maxInclination - Configs.MaxInclination) * (inclination / sumInclinations)));
                                sumMovedMaterial += baseHeights[absX, absY] - heights[absX, absY];
                            }
                        }
                    }
                    heights[x, y] += sumMovedMaterial;
                }
            }
        }

        public void TransformVonNeumann(ref float[,] heights)
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
                    // Obter a maior inclinação e a soma das inclinações superiores à configuração
                    double maxInclination = 0.0;
                    double sumInclinations = 0.0;

                    // Loop horizontal
                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            continue;

                        double inclination = baseHeights[x, y] - baseHeights[absX, y];
                        if (inclination > maxInclination)
                        {
                            maxInclination = inclination;
                        }
                        if (inclination > Configs.MaxInclination)
                        {
                            sumInclinations += inclination;
                        }
                    }

                    // Loop vertical
                    for (int relY = -1; relY <= 1; relY++)
                    {
                        int absY = y + relY;
                        if (absY < 0 || absY >= topY)
                            continue;

                        double inclination = baseHeights[x, y] - baseHeights[x, absY];
                        if (inclination > maxInclination)
                        {
                            maxInclination = inclination;
                        }
                        if (inclination > Configs.MaxInclination)
                        {
                            sumInclinations += inclination;
                        }
                    }

                    // Se não houver nada para alterar prosseguir imediatamente
                    if (sumInclinations == 0.0)
                        continue;

                    // Mover material para os vizinhos com inclinações superiores à configuração
                    float sumMovedMaterial = 0.0f;

                    // Loop horizontal
                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            continue;

                        double inclination = baseHeights[x, y] - baseHeights[absX, y];
                        if (inclination > Configs.MaxInclination)
                        {
                            heights[absX, y] = (float)(baseHeights[absX, y] + (Configs.DistributionFactor * (maxInclination - Configs.MaxInclination) * (inclination / sumInclinations)));
                            sumMovedMaterial += baseHeights[absX, y] - heights[absX, y];
                        }
                    }

                    // Loop vertical
                    for (int relY = -1; relY <= 1; relY++)
                    {
                        int absY = y + relY;
                        if (absY < 0 || absY >= topY)
                            continue;

                        double inclination = baseHeights[x, y] - baseHeights[x, absY];
                        if (inclination > Configs.MaxInclination)
                        {
                            heights[x, absY] = (float)(baseHeights[x, absY] + (Configs.DistributionFactor * (maxInclination - Configs.MaxInclination) * (inclination / sumInclinations)));
                            sumMovedMaterial += baseHeights[x, absY] - heights[x, absY];
                        }
                    }

                    heights[x, y] += sumMovedMaterial;
                }
            }
        }
    }
}

