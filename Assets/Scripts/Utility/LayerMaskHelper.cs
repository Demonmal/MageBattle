using UnityEngine;

namespace MageBattle.Utility
{
    [CreateAssetMenu(fileName = "LayerMaskHelper", menuName = "LayerMaskHelper")]
    public class LayerMaskHelper : ScriptableObject
    {
        private static LayerMaskHelper _instance;

        public static LayerMaskHelper instance => _instance;

        [SerializeField] private LayerMask _groundMask;

        public LayerMask groundMask => _groundMask;

        public static void Load()
        {
            if (_instance == null)
                _instance = Resources.Load<LayerMaskHelper>("Data/Level/LayerMaskHelper");
        }

        public static void Unload()
        {
            if (_instance != null)
            {
                Resources.UnloadAsset(_instance);
                _instance = null;
            }
        }
    }
}