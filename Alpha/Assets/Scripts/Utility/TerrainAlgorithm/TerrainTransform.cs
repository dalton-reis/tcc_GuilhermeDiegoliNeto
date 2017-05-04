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
        /// Indicador de necessidade de atualizar as texturas do terreno.
        /// </summary>
        public bool UpdateTextures { get; set; }
        /// <summary>
        /// Indicador de necessidade de atualizar o sombreamento do terreno.
        /// </summary>
        public bool UpdateShades { get; set; }
        /// <summary>
        /// Indicador de necessidade de atualizar o modelo do terreno.
        /// </summary>
        public bool UpdateMeshes { get; set; }

        public float[,] SoilMap { get; set; }
        public float[,] RockMap { get; set; }
        public int[,] SurfaceMap { get; set; }
        public float[,] HumidityMap { get; set; }

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
        /// Reseta todos os indicadores de atualização para false.
        /// </summary>
        public void ResetUpdateStates()
        {
            UpdateTextures = false;
            UpdateShades = false;
            UpdateMeshes = false;
        }

        /// <summary>
        /// Aplicar uma iteração do algoritmo de transformação.
        /// </summary>
        public abstract void ApplyTransform();

        /// <summary>
        /// Função para ser utilizada em loops de transformação.
        /// </summary>
        /// <param name="localHeight">Referência para a altura do ponto central.</param>
        /// <param name="nearbyHeight">Referência para a altura do ponto vizinho sendo evaluado.</param>
        protected delegate void LocalTransform(ref float localHeight, ref float nearbyHeight);

        /// <summary>
        /// Função para ser utilizada em loops de transformação.
        /// Versão estendida com as coordenadas do ponto vizinho sendo evaluado.
        /// </summary>
        /// <param name="localHeight">Referência para a altura do ponto central.</param>
        /// <param name="nearbyHeight">Referência para a altura do ponto vizinho sendo evaluado.</param>
        /// <param name="nearbyX">Coordenada X do ponto vizinho sendo evaluado.</param>
        /// <param name="nearbyY">Coordenada Y do ponto vizinho sendo evaluado.</param>
        protected delegate void LocalTransformEx(ref float localHeight, ref float nearbyHeight, int nearbyX, int nearbyY);

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

        /// <summary>
        /// Efetuar uma transformação local utilizando vizinhança Von Neumann (4-conexa).
        /// Mais performática, mas possível custo de qualidade.
        /// </summary>
        /// <param name="x">Coordenada X do ponto central.</param>
        /// <param name="y">Coordenada Y do ponto central.</param>
        /// <param name="heights">Mapa de alturas a ser transformado.</param>
        /// <param name="transform">Função de transformação estendida.</param>
        protected void VonNeumannTransform(int x, int y, float[,] heights, LocalTransformEx transform)
        {
            if (x != 0)
            {
                transform(ref heights[x, y], ref heights[x - 1, y], x - 1, y);
            }
            if (y != 0)
            {
                transform(ref heights[x, y], ref heights[x, y - 1], x, y - 1);
            }
            if (x != heights.GetLength(0) - 1)
            {
                transform(ref heights[x, y], ref heights[x + 1, y], x + 1, y);
            }
            if (y != heights.GetLength(1) - 1)
            {
                transform(ref heights[x, y], ref heights[x, y + 1], x, y + 1);
            }
        }
    }
}
