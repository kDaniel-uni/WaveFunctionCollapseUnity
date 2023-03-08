using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solver
{
    [CreateAssetMenu(fileName = "NewTileType", menuName = "Tile/Type", order = 1)]
    public class TileType : ScriptableObject
    {
        public Sprite Value;
        public AdjacencyList Up;
        public AdjacencyList Down;
        public AdjacencyList Right;
        public AdjacencyList Left;
        
        public TileType(Sprite value)
        {
            Value = value;
        }
        
        public List<TileType> OpposingTileTypes(DirectionType directionType)
        {
            switch (directionType)
            {
                case DirectionType.Up:
                    return Down.Value;
                case DirectionType.Down:
                    return Up.Value;
                case DirectionType.Right:
                    return Left.Value;
                case DirectionType.Left:
                    return Right.Value;
                default:
                    return new();
            }
        }
    }

}