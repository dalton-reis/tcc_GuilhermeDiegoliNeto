using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    public class DepositTransform : TerrainTransform
    {
        public override bool IsActive()
        {
            return false;
        }

        public override void ApplyTransform()
        {
            int topX = SoilMap.GetLength(0);
            int topY = SoilMap.GetLength(1);

            float[,] baseHeights = SoilMap.Clone() as float[,];
            float[,] heightDiff = new float[topX, topY];

            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
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
                        if (avg < SoilMap[x, y])
                        {
                            SoilMap[x, y] = avg;
                            heightDiff[x, y] = baseHeights[x, y] - avg;
                        }
                    }
                }
            }

            // Distribuir a massa de terra removida para os terrenos mais baixos
            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
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

                            if (absX != x && absY != y && SoilMap[absX, absY] <= SoilMap[x, y])
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

                                if (absX != x && absY != y && SoilMap[absX, absY] <= SoilMap[x, y])
                                    SoilMap[absX, absY] += depositPerPlot;
                            }
                        }
                    }
                    else
                    {
                        SoilMap[x, y] += heightDiff[x, y];
                    }
                }
            }
        }
    }
}
