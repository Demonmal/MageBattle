using UnityEngine;

namespace MageBattle.Utility
{
    public abstract class SingletonObject<T> : MonoBehaviour where T : SingletonObject<T>
    {
        protected static T _instance;

        public static T instance
        {
            get
            {
                if (_instance != null) return _instance;
                if (_instance == null) _instance = FindObjectOfType<T>();
                if (_instance == null) CreateInternal();

                return _instance;
            }
        }

        private static void CreateInternal()
        {
            _instance = new GameObject(typeof(T).Name).AddComponent<T>();
        }
    }
}