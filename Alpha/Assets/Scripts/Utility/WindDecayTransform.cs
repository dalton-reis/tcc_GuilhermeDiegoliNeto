using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Utility.TerrainAlgorithm
{
    public class WindDecayDigTransform : TerrainTransform
    {
        public WindDecaySimConfigs Configs { get; set; }

        public WindDecayDigTransform()
        {
            Configs = new WindDecaySimConfigs()
            {
                Active = false,
                Range = 1,
                Factor = 1.0f,
                WindDirection = Directions.North,
            };
        }

        public override bool IsActive()
        {
            return Configs.Active;
        }

        public override void ApplyTransform(ref float[,] heights)
        {
            int topX = heights.GetLength(0);
            int topY = heights.GetLength(1);

            float[,] baseHeights = heights.Clone() as float[,];

            int startX = GetStartingX();
            int startY = GetStartingY();
            int endX = GetEndingX();
            int endY = GetEndingY();

            int incStartY = GetStartingYIncrement();
            int incEndY = GetEndingYIncrement();

            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    // Fazer a média da altura com base nas alturas vizinhas no hemisfério Sul
                    // Apenas considerar alterações para baixo

                    float sumHeights = 0.0f;
                    int countHeights = 0;

                    int localStartY = startY;
                    int localEndY = endY;

                    for (int relX = startX; relX <= endX; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            break;

                        for (int relY = localStartY; relY <= localEndY; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                break;

                            sumHeights += baseHeights[absX, absY];
                            countHeights++;
                        }

                        localStartY += incStartY;
                        localEndY += incEndY;
                    }

                    if (countHeights > 0)
                    {
                        float avg = sumHeights / countHeights;
                        if (avg < heights[x, y])
                        {
                            float diff = avg - heights[x, y];
                            heights[x, y] += diff * Configs.Factor;
                        }
                    }
                }
            }
        }

        // Funções que definem a área da matriz a ser considerada na transformação

        // X[ ]
        //   Y

        //NW    N    NE
        //  [ ][+][ ]
        //W [-][0][+] E
        //  [ ][-][ ]
        //SW    S    SE

        // As funções retornam os valores de X e Y iniciais e finais para a operação, assim como a variação necessária para áreas triangulares
        private int GetStartingY()
        {
            switch (Configs.WindDirection)
            {
                case Directions.North: return -Configs.Range;
                case Directions.Northeast: return -Configs.Range;
                case Directions.East: return -Configs.Range;
                case Directions.Southeast: return -Configs.Range;
                case Directions.South: return -Configs.Range;
                case Directions.Southwest: return Configs.Range;
                case Directions.West: return 0;
                case Directions.Northwest: return -Configs.Range;
            }

            return 0;
        }

        private int GetEndingY()
        {
            switch (Configs.WindDirection)
            {
                case Directions.North: return Configs.Range;
                case Directions.Northeast: return Configs.Range;
                case Directions.East: return 0;
                case Directions.Southeast: return -Configs.Range;
                case Directions.South: return Configs.Range;
                case Directions.Southwest: return Configs.Range;
                case Directions.West: return Configs.Range;
                case Directions.Northwest: return Configs.Range;
            }

            return 0;
        }

        private int GetStartingYIncrement()
        {
            switch (Configs.WindDirection)
            {
                case Directions.Southwest: return -1;
                case Directions.Northwest: return 1;
            }

            return 0;
        }

        private int GetEndingYIncrement()
        {
            switch (Configs.WindDirection)
            {
                case Directions.Northeast: return -1;
                case Directions.Southeast: return 1;
            }

            return 0;
        }

        private int GetStartingX()
        {
            switch (Configs.WindDirection)
            {
                case Directions.North: return -Configs.Range;
                case Directions.Northeast: return -Configs.Range;
                case Directions.East: return -Configs.Range;
                case Directions.Southeast: return -Configs.Range;
                case Directions.South: return 0;
                case Directions.Southwest: return -Configs.Range;
                case Directions.West: return -Configs.Range;
                case Directions.Northwest: return -Configs.Range;
            }

            return 0;
        }

        private int GetEndingX()
        {
            switch (Configs.WindDirection)
            {
                case Directions.North: return 0;
                case Directions.Northeast: return Configs.Range;
                case Directions.East: return Configs.Range;
                case Directions.Southeast: return Configs.Range;
                case Directions.South: return Configs.Range;
                case Directions.Southwest: return Configs.Range;
                case Directions.West: return Configs.Range;
                case Directions.Northwest: return Configs.Range;
            }

            return 0;
        }
    }
}
