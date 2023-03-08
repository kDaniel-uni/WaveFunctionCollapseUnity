using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solver
{
    public class Game
    {
        public GameMap GameMap;
        public Game(int width, int height, List<SpriteDescriptor> tileTypes, GameObject[,] gameObjects)
        {
            State baseState = new(tileTypes);
            GameMap = new GameMap(baseState, width, height, gameObjects);
        }

        public void Step()
        {
            Dictionary<Position, bool> updated = new();
            Queue<Tile> queue = new();
                
            Position currentPosition = GameMap.CollapseTileWithLowestEntropy();
            GameMap.Grid[currentPosition.X, currentPosition.Y].SpriteRenderer.sprite =
                GameMap.Grid[currentPosition.X, currentPosition.Y].State.Collapsed!.Sprite;
            updated.Add(currentPosition, true);

            foreach (var neighbor in GameMap.GetNeighborTiles(currentPosition))
            {
                queue.Enqueue(neighbor.Value);
            }

            while (queue.Count > 0) {
                Tile tile = queue.Dequeue();
                Dictionary<DirectionType, Tile> neighbors = GameMap.GetNeighborTiles(tile.Position);

                var stateCount = tile.State.SuperposedTiles.Count;
                
                foreach (var neighbor in neighbors)
                {
                    tile.State.UpdateDir(neighbor.Key, neighbor.Value.State);
                }
                
                var updatedStateCount = tile.State.SuperposedTiles.Count;

                GameMap.EntropyMap[stateCount].Remove(tile.Position);
                GameMap.EntropyMap[updatedStateCount].Add(tile.Position);
                
                if (tile.State.IsCollapsed())
                {
                    tile.SpriteRenderer.sprite = tile.State.Collapsed!.Sprite;
                }

                updated.Add(tile.Position, true);

                if (stateCount == updatedStateCount) {continue;}
                
                foreach (var neighbor in neighbors)
                {
                    if (!updated.ContainsKey(neighbor.Value.Position) && !queue.Contains(neighbor.Value))
                    {
                        queue.Enqueue(neighbor.Value);
                    }
                }
            }
                
            /*Console.Out.WriteLine($"UnCollapsed Tiles : {GameMap.UncollapsedPositions.Count}");
            GameMap.PrintGrid();*/
        }
    }
}