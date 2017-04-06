using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    /// <summary>
    /// Classe base para algoritmos de transformação do terreno.
    /// </summary>
    public abstract class TerrainTransform
    {
        /// <summary>
        /// Verificar estado da simulação conforme suas configurações.
        /// </summary>
        /// <returns>O estado da simulação. (ativa/inativa)</returns>
        public abstract bool IsActive();

        /// <summary>
        /// Resetar informações persistentes de uma simulação.
        /// </summary>
        public virtual void Reset()
        {
        }

        /// <summary>
        /// Aplicar uma iteração do algoritmo de transformação.
        /// </summary>
        /// <param name="heights">A matriz de alturas sobre a qual será aplicada a transformação.</param>
        public abstract void ApplyTransform(float[,] heights);

        public abstract void ApplyTransform(float[,] rockHeights, float[,] dirtHeights);

        /// <summary>
        /// Função para ser utilizada em loops de transformação.
        /// </summary>
        /// <param name="localHeight">Referência para a altura do ponto central.</param>
        /// <param name="nearbyHeight">Referência para a altura do ponto vizinho sendo evaluado.</param>
        protected delegate void LocalTransform(ref float localHeight, ref float nearbyHeight);

        /// <summary>
        /// Efetuar uma transformação local utilizando vizinhança Von Neumann (4-conexa).
        /// Mais performática, mas possível custo de qualidade.
        /// </summary>
        /// <param name="x">Coordenada X do ponto central.</param>
        /// <param name="y">Coordenada Y do ponto central.</param>
        /// <param name="heights">Mapa de alturas a ser transformado.</param>
        /// <param name="transform">Função de transformação.</param>
        protected void VonNeumannTransform(int x, int y, float[,] heights, LocalTransform transform)
        {
            if (x != 0)
            {
                transform(ref heights[x, y], ref heights[x - 1, y]);
            }
            if (y != 0)
            {
                transform(ref heights[x, y], ref heights[x, y - 1]);
            }
            if (x != heights.GetLength(0) - 1)
            {
                transform(ref heights[x, y], ref heights[x + 1, y]);
            }
            if (y != heights.GetLength(1) - 1)
            {
                transform(ref heights[x, y], ref heights[x, y + 1]);
            }
        }
    }
}
