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
                Active = false,
                MaxInclination = 4.0f / 256,
                DistributionFactor = 0.5f,
            };
        }

        public override bool IsActive()
        {
            return Configs.Active;
        }

        public override void ApplyTransform(float[,] rockHeights, float[,] soilHeights)
        {
            TransformVonNeumann(soilHeights, rockHeights);
        }

        public override void ApplyTransform(float[,] heights)
        {
            TransformVonNeumann(heights);
        }

        [System.Obsolete("Usar transformações Von Neumann")]
        public void TransformMoore(float[,] heights)
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
                                float movedMaterial = baseHeights[absX, absY] - (float)(baseHeights[absX, absY] + (Configs.DistributionFactor * (maxInclination - Configs.MaxInclination) * (inclination / sumInclinations)));
                                heights[absX, absY] -= movedMaterial;
                                sumMovedMaterial += movedMaterial;
                            }
                        }
                    }
                    heights[x, y] += sumMovedMaterial;
                }
            }
        }

        public void TransformVonNeumann(float[,] heights)
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
                            float movedMaterial = baseHeights[absX, y] - (float)(baseHeights[absX, y] + (Configs.DistributionFactor * (maxInclination - Configs.MaxInclination) * (inclination / sumInclinations)));
                            heights[absX, y] -= movedMaterial;
                            sumMovedMaterial += movedMaterial;
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
                            float movedMaterial = baseHeights[x, absY] - (float)(baseHeights[x, absY] + (Configs.DistributionFactor * (maxInclination - Configs.MaxInclination) * (inclination / sumInclinations)));
                            heights[x, absY] -= movedMaterial;
                            sumMovedMaterial += movedMaterial;
                        }
                    }

                    heights[x, y] += sumMovedMaterial;
                }
            }
        }

        public void TransformVonNeumann(float[,] soilHeights, float[,] rockHeights)
        {
            // Loop geral do mapa
            for (int x = 0; x < soilHeights.GetLength(0); x++)
            {
                for (int y = 0; y < soilHeights.GetLength(1); y++)
                {
                    float localSoilMass = soilHeights[x,y] - rockHeights[x,y];

                    // Obter a maior inclinação, a soma das inclinações superiores à configuração e a quantidade esperada de material movido
                    float maxInclination = 0.0f;
                    float sumInclinations = 0.0f;
                    float sumMovedMaterial = 0.0f;

                    VonNeumannTransform(x, y, soilHeights,
                        (ref float localHeight, ref float nearbyHeight) =>
                        {
                            float inclination = localHeight - nearbyHeight;
                            if (inclination > maxInclination)
                            {
                                maxInclination = inclination;
                            }
                            if (inclination > Configs.MaxInclination)
                            {
                                sumInclinations += inclination;
                                sumMovedMaterial += Configs.DistributionFactor * (inclination - Configs.MaxInclination);
                            }
                        }
                    );

                    // Se não houver nada para alterar prosseguir imediatamente
                    if (sumInclinations == 0.0)
                        continue;

                    // Limitar a quantidade de material movimentada se não houver solo o bastante
                    float movementLimitingFactor = 1.0f;
                    if (sumMovedMaterial > localSoilMass)
                    {
                        movementLimitingFactor = localSoilMass / sumMovedMaterial;
                    }

                    // Mover material para os vizinhos com inclinações superiores à configuração
                    sumMovedMaterial = 0.0f;

                    // Constante para os próximos cálculos
                    float inclinationDifference = (maxInclination - Configs.MaxInclination);

                    VonNeumannTransform(x, y, soilHeights,
                        (ref float localHeight, ref float nearbyHeight) =>
                        {
                            float inclination = localHeight - nearbyHeight;
                            if (inclination > Configs.MaxInclination)
                            {
                                float movedMaterial = Configs.DistributionFactor * inclinationDifference * (inclination / sumInclinations);
                                movedMaterial *= movementLimitingFactor;
                                nearbyHeight += movedMaterial;
                                sumMovedMaterial += movedMaterial;
                            }
                        }
                    );

                    soilHeights[x, y] -= sumMovedMaterial;
                }
            }
        }
    }
}

