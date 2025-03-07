using UnityEngine;

namespace RBPT.Core.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private bool _isSignleton = false;

        public static bool Exists => _instance != null;

        public static T Instance
        {
            get
            {
                if(!Exists)
                {
                    _instance = CreateSingleton();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if(Exists && !_isSignleton)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this as T;
            _isSignleton = true;

            DontDestroyOnLoad(this.gameObject);
        }

        private static T CreateSingleton()
        {
            return new GameObject(typeof(T).Name).AddComponent<T>();
        }
    }
}
