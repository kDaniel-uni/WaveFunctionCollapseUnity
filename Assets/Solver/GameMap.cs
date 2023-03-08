using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Solver
{
    public class GameMap
    {
        public Tile[,] Grid;
        public int Width;
        public int Height;
        public Dictionary<int, List<Position>> EntropyMap = new();
        public State InitState;
        public int InitStateCount;

        public GameMap(State initState, int width, int height, GameObject[,] gameObjects)
        {
            InitState = initState;
            Width = width;
            Height = height;
            Grid = new Tile[Width,Height];
            InitStateCount = initState.SuperposedTiles.Count;

            for (int i = 1; i < InitStateCount+1; i++)
            {
                EntropyMap.Add(i, new());
            }
            
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Position pos = new Position(i, j);
                    EntropyMap[InitStateCount].Add(pos);
                }
            }
            InitGrid(gameObjects);
        }
    
        private void InitGrid(GameObject[,] gameObjects)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Grid[i,j] = new Tile(new Position(i,j), InitState.Copy(), gameObjects[i,j].GetComponent<SpriteRenderer>());
                }
            }
        }
    
    
        public Position CollapseTileWithLowestEntropy()
        {
            for (int i = 2; i < InitStateCount + 1; i++)
            {
                int count = EntropyMap[i].Count;
                if (count == 0) continue;
                
                int index = new Random().Next(0, count);
                Position pos = EntropyMap[i][index];
                EntropyMap[i].RemoveAt(index);
                Grid[pos.X,pos.Y].State.Collapse();
                EntropyMap[1].Add(pos);
                return pos;
            }

            return new Position(0, 0);
        }
    
    
        // Get the neighbors that are inside the map
        // Usage of Dictionary to allow sparse storage of tiles and access to iterator
        public Dictionary<DirectionType, Tile> GetNeighborTiles(Position pos)
        {
            Dictionary<DirectionType, Tile> neighbors = new();

            if (pos.Y > 0)
            {
                neighbors.Add(DirectionType.Down, Grid[pos.X, pos.Y - 1]);
            }
            
            if (pos.Y < Height - 1)
            {
                neighbors.Add(DirectionType.Up, Grid[pos.X, pos.Y + 1]);
            }
            
            if (pos.X > 0)
            {
                neighbors.Add(DirectionType.Left, Grid[pos.X - 1, pos.Y]);
            }
            
            if (pos.X < Width - 1)
            {
                neighbors.Add(DirectionType.Right, Grid[pos.X + 1, pos.Y]);
            }
            
            return neighbors;
        }

        public void PrintGrid()
        {
            String boundLine = "-";
                
            for (int i = 0; i < Width; i++)
            {
                boundLine += "-";
            }

            boundLine += "-";
            
            Console.WriteLine(boundLine);
            
            for (int j = 0; j < Height; j++)
            {
                String line = "|";
                
                for (int i = 0; i < Width; i++)
                {
                    if (Grid[i,j].State.Collapsed == null)
                    {
                        line += "?";
                        continue;
                    }
                    
                    line += Grid[i,j].State.Collapsed.name;
                }

                line += "|";
                
                Console.WriteLine(line);
            }
            
            Console.WriteLine(boundLine);
        }
    }
}