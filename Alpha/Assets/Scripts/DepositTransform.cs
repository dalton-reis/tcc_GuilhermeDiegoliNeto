using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainAlgorithm
{
    public class DepositTransform : TerrainTransform
    {
        public override void ApplyTransform(ref float[,] heights)
        {
            int topX = heights.GetLength(0);
            int topY = heights.GetLength(1);

            float[,] baseHeights = heights.Clone() as float[,];
            float[,] heightDiff = new float[topX, topY];

            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    // Fazer a média da altura com base nas alturas vizinhas
                    // Apenas considerar variação para baixo

                    float sumHeights = 0.0f;
                    int countHeights = 0;

                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            break;

                        for (int relY = -1; relY <= 1; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                break;

                            sumHeights += baseHeights[absX, absY];
                            countHeights++;
                        }
                    }

                    if (countHeights > 0)
                    {
                        float avg = sumHeights / countHeights;
                        if (avg < heights[x, y])
                        {
                            heights[x, y] = avg;
                            heightDiff[x, y] = baseHeights[x, y] - avg;
                        }
                    }
                }
            }

            // Distribuir a massa de terra removida para os terrenos mais baixos
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    int countLowLands = 0;

                    // Quantidade de nodos mais baixos
                    for (int relX = -1; relX <= 1; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            break;

                        for (int relY = -1; relY <= 1; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                break;

                            if (absX != x && absY != y && heights[absX, absY] <= heights[x, y])
                                countLowLands++;
                        }
                    }

                    // Distribuição
                    if (countLowLands > 0)
                    {
                        float depositPerPlot = heightDiff[x, y] / countLowLands;

                        for (int relX = -1; relX <= 1; relX++)
                        {
                            int absX = x + relX;
                            if (absX < 0 || absX >= topX)
                                break;

                            for (int relY = -1; relY <= 1; relY++)
                            {
                                int absY = y + relY;
                                if (absY < 0 || absY >= topY)
                                    break;

                                if (absX != x && absY != y && heights[absX, absY] <= heights[x, y])
                                    heights[absX, absY] += depositPerPlot;
                            }
                        }
                    }
                    else
                    {
                        heights[x, y] += heightDiff[x, y];
                    }
                }
            }
        }
    }
}
