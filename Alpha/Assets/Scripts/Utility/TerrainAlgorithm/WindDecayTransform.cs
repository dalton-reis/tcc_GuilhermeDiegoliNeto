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
                UseMoore = false,
                WindDirection = Directions.North,
            };
        }

        public override bool IsActive()
        {
            return Configs.Active;
        }

        public override void ApplyTransform(float[,] rockHeights, float[,] dirtHeights)
        {

        }

        public override void ApplyTransform(float[,] heights)
        {
            if (Configs.UseMoore)
            {
                ApplyMoore(heights);
            }
            else
            {
                ApplyVonNeumann(heights);
            }
        }

        public void ApplyVonNeumann(float[,] heights)
        {
            Directions direction = Configs.WindDirection;
            int topX = heights.GetLength(0);
            int topY = heights.GetLength(1);

            float[,] baseHeights = heights.Clone() as float[,];

            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    float sumHeights = 0.0f;
                    int countHeights = 0;

                    // Direções horizontais
                    if (direction == Directions.East || direction == Directions.West)
                    {
                        int from = direction == Directions.East ? -Configs.Range : 0;
                        int to = direction == Directions.East ? 0 : Configs.Range;

                        // Todos os vizinhos verticais
                        for (int relX = -Configs.Range; relX <= Configs.Range; relX++)
                        {
                            int absX = x + relX;
                            if (absX < 0 || absX >= topX)
                                continue;

                            sumHeights += baseHeights[absX, y];
                            countHeights++;
                        }

                        // Metade dos vizinhos horizontais
                        for (int relY = from; relY <= to; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                continue;

                            sumHeights += baseHeights[x, absY];
                            countHeights++;
                        }

                        // Subtrair o valor da célula central que foi somado duas vezes
                        sumHeights -= baseHeights[x, y];
                        countHeights--;
                    }
                    // Direções verticais
                    if (direction == Directions.North || direction == Directions.South)
                    {
                        int from = direction == Directions.North ? -Configs.Range : 0;
                        int to = direction == Directions.North ? 0 : Configs.Range;

                        // Metade dos vizinhos verticais
                        for (int relX = from; relX <= to; relX++)
                        {
                            int absX = x + relX;
                            if (absX < 0 || absX >= topX)
                                continue;

                            sumHeights += baseHeights[absX, y];
                            countHeights++;
                        }

                        // Todos os vizinhos horizontais
                        for (int relY = -Configs.Range; relY <= Configs.Range; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                continue;

                            sumHeights += baseHeights[x, absY];
                            countHeights++;
                        }

                        // Subtrair o valor da célula central que foi somado duas vezes
                        sumHeights -= baseHeights[x, y];
                        countHeights--;
                    }
                    // Direções diagonais
                    else
                    {
                        int fromX = 0;
                        int fromY = 0;
                        int toX = 0;
                        int toY = 0;

                        // Selecionar valores para representar a diagonal selecionada
                        switch (direction)
                        {
                            case Directions.Northeast:
                                fromX = -Configs.Range;
                                fromY = -Configs.Range;
                                break;
                            case Directions.Southeast:
                                toX = Configs.Range;
                                fromY = -Configs.Range;
                                break;
                            case Directions.Southwest:
                                toX = Configs.Range;
                                toY = Configs.Range;
                                break;
                            case Directions.Northwest:
                                fromX = -Configs.Range;
                                toY = Configs.Range;
                                break;
                        }

                        // Vizinhos verticais
                        for (int relX = fromX; relX <= toX; relX++)
                        {
                            int absX = x + relX;
                            if (absX < 0 || absX >= topX)
                                continue;

                            sumHeights += baseHeights[absX, y];
                            countHeights++;
                        }

                        // Vizinhos horizontais
                        for (int relY = fromY; relY <= toY; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                continue;

                            sumHeights += baseHeights[x, absY];
                            countHeights++;
                        }

                        // Subtrair o valor da célula central que foi somado duas vezes
                        sumHeights -= baseHeights[x, y];
                        countHeights--;
                    }

                    // Aplicar média
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

        public void ApplyMoore(float[,] heights)
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
                            continue;

                        for (int relY = localStartY; relY <= localEndY; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                continue;

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
