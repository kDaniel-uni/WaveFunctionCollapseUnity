using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solver
{
    [CreateAssetMenu(fileName = "NewAdjacencyList", menuName = "Tile/AdjacencyList", order = 2)]
    public class AdjacencyList : ScriptableObject
    {
        public List<TileType> Value;
    }
}