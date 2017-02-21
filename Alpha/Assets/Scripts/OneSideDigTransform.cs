using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace TerrainAlgorithm
{
    public class OneSideDigTransform : TerrainTransform
    {
        public int range = 1;
        public float factor = 1.0f;

        public override void ApplyTransform(ref float[,] heights)
        {
            int topX = heights.GetLength(0);
            int topY = heights.GetLength(1);

            float[,] baseHeights = heights.Clone() as float[,];

            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    // Fazer a média da altura com base nas alturas vizinhas no hemisfério Sul
                    // Apenas considerar alterações para baixo

                    float sumHeights = 0.0f;
                    int countHeights = 0;

                    for (int relX = 0; relX <= range; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            break;

                        for (int relY = -range; relY <= range; relY++)
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
                            float diff = avg - heights[x, y];
                            heights[x, y] += diff * factor;
                        }
                    }
                }
            }
        }
    }
}
