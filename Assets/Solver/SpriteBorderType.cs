using UnityEngine;

namespace Solver
{
    [CreateAssetMenu(fileName = "NewSpriteBorder", menuName = "Tile/SpriteBorder", order = 3)]
    public class SpriteBorderType : ScriptableObject
    {
        public SpriteBorderType opposite;
    }
}