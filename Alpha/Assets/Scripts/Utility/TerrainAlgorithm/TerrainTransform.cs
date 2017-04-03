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
        public abstract void ApplyTransform(ref float[,] heights);
    }
}
