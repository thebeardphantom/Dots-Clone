using UnityEngine;

namespace DotsClone {
    /// <summary>
    /// A reusable, consistent Singleton behavior base class
    /// </summary>
    /// <typeparam name="T">A self reference to the inheriting type</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
        private static bool appQuitting;
        private static object @lock = new object();
        private static T _get;

        public static T get {
            get {
                return GetSingleton();
            }
        }

        public static T DummyCreate() {
            return get;
        }

        private static T GetSingleton() {
            if(appQuitting) {
                return null;
            }

            lock (@lock) {
                if(_get == null) {
                    var allInstances = FindObjectsOfType<T>();
                    if(allInstances.Length > 1) {
                        Debug.LogError(typeof(T).Name + " duplicates found.");
                        _get = allInstances[0];
                    }
                    else if(allInstances.Length == 0) {
                        _get = new GameObject("[Singleton] " + typeof(T).Name).AddComponent<T>();
                    }
                    else {
                        _get = allInstances[0];
                    }
                }
                return _get;
            }
        }

        protected virtual void Awake() {
            if(_get != null) {
                Destroy(this);
            }
            DontDestroyOnLoad(this);
        }

        protected virtual void OnDestroy() {
            appQuitting = true;
        }
    }
}