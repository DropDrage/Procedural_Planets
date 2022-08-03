using UnityEngine;

namespace Utils
{
    public static class SpawnUtils
    {
        /// <summary>
        ///    Load prefab from Resources
        /// </summary>
        /// <param name="prefabName">Only name of prefab</param>
        /// <returns> Return not spawned Object, that can be spawned later</returns>
        public static Object LoadPrefab(string prefabName) => Resources.Load($"Prefabs/{prefabName}");

        /// <summary>
        ///     Spawn prefab
        /// </summary>
        /// <param name="prefabName">Only name of prefab</param>
        /// <returns>GameObject of spawned prefab</returns>
        public static GameObject SpawnPrefab(string prefabName) => SpawnPrefab(LoadPrefab(prefabName));

        public static GameObject SpawnPrefab(string prefabName, Vector3 pos) =>
            SpawnPrefab(LoadPrefab(prefabName), pos);

        /// <summary>
        ///     Instantiate Object as GameObject
        /// </summary>
        /// <param name="prefab">Object to instantiate as GameObject</param>
        /// <returns></returns>
        public static GameObject SpawnPrefab(Object prefab) => Object.Instantiate(prefab) as GameObject;

        public static GameObject SpawnPrefab(Object prefab, Vector3 position) =>
            Object.Instantiate(prefab, position, Quaternion.identity) as GameObject;
    }
}
