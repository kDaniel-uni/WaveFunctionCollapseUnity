using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Solver
{
    public class Tile
    {
            public Position Position { get; set; }
            public State State { get; set; }
            public SpriteRenderer SpriteRenderer { get; }
            public Tile(Position position, State state, SpriteRenderer spriteRenderer)
            {
                Position = position;
                State = state;
                SpriteRenderer = spriteRenderer;
            }
    }
    public class State
    {
        public List<SpriteDescriptor> SuperposedTiles { get; private set; }
        public SpriteDescriptor? Collapsed { get; private set; } = null;
        public State(List<SpriteDescriptor> superposedTiles)
        {
            SuperposedTiles = new (superposedTiles);
        }

        public State Copy() {
            return new State(SuperposedTiles);
        }

        /// collapse l'état en une seule tuile
        /// collapse de manière "forte" : collapse meme si plusieurs états sont supperposés
        /// @return la tuile résultante après collapse
        public SpriteDescriptor Collapse()
        {
            // here we are assuming the superposed tiles were all possible to be according to the adjacent tiles.
            // wrong tiles should have been removed previously by and Update() call
            List<int> weights = new List<int>();
            foreach (var spriteDescriptor in SuperposedTiles)
            {
                weights.Add(spriteDescriptor.Weight);
            }

            Collapsed = SuperposedTiles[GetRandomWeightedIndex(weights)];
            SuperposedTiles = new (){Collapsed};

            return Collapsed;
        }
            
        /// update les états supperposés en fonction des tuiles adjacentes
        /// collapse de manière "faible" : collapse seulement si il reste un seul état possible
        /// @return true si l'update à collapse
        /*public bool Update(State up, State down, State right, State left)
        {
            SuperposedTiles.RemoveAll(tile =>
                !up.SuperposedTiles.Exists(t => t.Down.Value.Contains(tile)) // il existe dans la liste des superposées en haut une tuile t qui contient la tuile tile en down
                  || !down.SuperposedTiles.Exists(t => t.Up.Value.Contains(tile))
                  || !right.SuperposedTiles.Exists(t => t.Left.Value.Contains(tile))
                  || !left.SuperposedTiles.Exists(t => t.Right.Value.Contains(tile)));

            if (SuperposedTiles.Count <= 1)
            {
                Collapse();
                return true;
            }

            return false;
        }*/

        // Update the current tile using only one direction
        public void UpdateDir(DirectionType dir, State state)
        {
            if (Collapsed != null)
            {
                return;
            }
            
            HashSet<SpriteBorderType> acceptedBorders = new HashSet<SpriteBorderType>();

            foreach (var stateSuperposedTile in state.SuperposedTiles)
            {
                acceptedBorders.Add(stateSuperposedTile.OpposingBorder(dir));
            }

            SuperposedTiles.RemoveAll(tile =>  
                !acceptedBorders.Contains(tile.Border(dir)));

            if (SuperposedTiles.Count == 1)
            {
                Collapsed = SuperposedTiles[0];
            }
        }

        public bool IsCollapsed() {
            return Collapsed != null;
        }
        
        public int GetRandomWeightedIndex(List<int> weights)
        {
            // Get the total sum of all the weights.
            int weightSum = 0;
            for (int i = 0; i < weights.Count; ++i)
            {
                weightSum += weights[i];
            }
 
            // Step through all the possibilities, one by one, checking to see if each one is selected.
            int index = 0;
            int lastIndex = weights.Count - 1;
            while (index < lastIndex)
            {
                // Do a probability check with a likelihood of weights[index] / weightSum.
                if (new Random().Next(0, weightSum) < weights[index])
                {
                    return index;
                }
 
                // Remove the last item from the sum of total untested weights and try again.
                weightSum -= weights[index++];
            }
 
            // No other item was selected, so return very last index.
            return index;
        }

    }

}