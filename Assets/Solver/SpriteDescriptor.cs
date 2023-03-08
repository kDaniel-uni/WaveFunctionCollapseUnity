using UnityEngine;

namespace Solver
{
    [CreateAssetMenu(fileName = "NewSpriteDescriptor", menuName = "Tile/SpriteDescriptor", order = 4)]
    public class SpriteDescriptor : ScriptableObject
    {
        public Sprite Sprite;
        public SpriteBorderType Up;
        public SpriteBorderType Down;
        public SpriteBorderType Left;
        public SpriteBorderType Right;
        public SymmetryType SymmetryType;
        public int Weight = 1;

        public SpriteBorderType OpposingBorder(DirectionType directionType)
        {
            switch (directionType)
            {
                case DirectionType.Up:
                    return Down.opposite;
                case DirectionType.Down:
                    return Up.opposite;
                case DirectionType.Right:
                    return Left.opposite;
                case DirectionType.Left:
                    return Right.opposite;
                default:
                    return new();
            }
        }
        
        public SpriteBorderType Border(DirectionType directionType)
        {
            switch (directionType)
            {
                case DirectionType.Up:
                    return Up;
                case DirectionType.Down:
                    return Down;
                case DirectionType.Right:
                    return Right;
                case DirectionType.Left:
                    return Left;
                default:
                    return new();
            }
        }
    }
}