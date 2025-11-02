using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    private static Dictionary<string, LinkedList<object>> _pools = new Dictionary<string, LinkedList<object>>();

    #region GetGenerics

        public static T GetPool<T>(T prefab, Vector3 pos, Quaternion quaternion) where T : MonoBehaviour
        {
            string name = prefab.gameObject.name;
            if (!_pools.ContainsKey(name))
                _pools[name] = new LinkedList<object>();

            T result;

            if(_pools[name].Count > 0)
            {
                result = GetPoolFirst<T>(name, pos, quaternion);
                return result;
            }
            
            result = Object.Instantiate(prefab, pos, quaternion);
            result.gameObject.name = name;

            return result;
        }

        public static T GetPool<T>(T prefab, Vector3 pos) where T : MonoBehaviour
        {
            string name = prefab.gameObject.name;
            if (!_pools.ContainsKey(name))
                _pools[name] = new LinkedList<object>();

            T result;
            var quaternion = prefab.transform.rotation;

            if(_pools[name].Count > 0)
            {
                result = GetPoolFirst<T>(name, pos, quaternion);
                return result;
            }
            
            result = Object.Instantiate(prefab, pos, quaternion);
            result.gameObject.name = name;

            return result;
        }
        
        private static T GetPoolFirst<T>(string name, Vector3 pos, Quaternion quaternion) where T : MonoBehaviour
        {
            T result = _pools[name].First.Value is T 
                ? _pools[name].First.Value as T 
                : (_pools[name].First.Value as GameObject).GetComponent<T>();
                
            result.gameObject.SetActive(true);
            result.transform.rotation = quaternion;
            result.transform.position = pos;
            _pools[name].RemoveFirst();

            return result;
        }

    #endregion
    
    #region GetGameObject

        public static GameObject GetPool(GameObject prefab, Vector3 pos, Quaternion quaternion)
        {
            string name = prefab.name;
        
            if (!_pools.ContainsKey(name))
                _pools[name] = new LinkedList<object>();

            GameObject result;

            if(_pools[name].Count > 0)
            {
                result = GetPoolFirst(name, pos, quaternion);
                return result;
            }
            
            result = Object.Instantiate(prefab, pos, quaternion);
            result.name = name;
            
            return result;
        }
        
        public static GameObject GetPool(GameObject prefab, Vector3 pos)
        {
            string name = prefab.name;
        
            if (!_pools.ContainsKey(name))
                _pools[name] = new LinkedList<object>();

            GameObject result;
            var quaternion = prefab.transform.rotation;

            if(_pools[name].Count > 0)
            {
                result = GetPoolFirst(name, pos, quaternion);
                return result;
            }
            
            result = Object.Instantiate(prefab, pos, quaternion);
            result.name = name;
            
            return result;
        }
        
        private static GameObject GetPoolFirst(string name, Vector3 pos, Quaternion quaternion)
        {
            GameObject result = _pools[name].First.Value is GameObject 
                ? _pools[name].First.Value as GameObject 
                : (_pools[name].First.Value as MonoBehaviour).gameObject;
                
            result.SetActive(true);
            result.transform.rotation = quaternion;
            result.transform.position = pos;
            _pools[name].RemoveFirst();

            return result;
        }

    #endregion

    public static void PoolPreLaunch<T>(T prefab, int count, Transform container) where T : MonoBehaviour
    {
        string name = prefab.gameObject.name;
        if (!_pools.ContainsKey(name))
            _pools[name] = new LinkedList<object>();

        T result;

        for (int i = 0; i < count; i++)
        {
            result = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            result.gameObject.name = name;
        
            _pools[name].AddFirst(result);
            result.gameObject.SetActive(false);
            result.transform.SetParent(container);
        }
    }
    
    public static void PoolPreLaunch(GameObject prefab, int count, Transform container)
    {
        string name = prefab.gameObject.name;
        if (!_pools.ContainsKey(name))
            _pools[name] = new LinkedList<object>();

        GameObject result;

        for (int i = 0; i < count; i++)
        {
            result = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            result.name = name;
        
            _pools[name].AddFirst(result);
            result.SetActive(false);
            result.transform.SetParent(container);
        }
    }
    
    public static void SetPool(GameObject target)
    {
        if (!_pools.ContainsKey(target.name))
        {
            Object.Destroy(target);
            return;
        }
        
        _pools[target.name].AddFirst(target);
        target.SetActive(false);
    }
    
    public static void SetPool(Transform target)
    {
        if (!_pools.ContainsKey(target.name))
        {
            Object.Destroy(target);
            return;
        }
        
        _pools[target.name].AddFirst(target.gameObject);
        target.gameObject.SetActive(false);
    }
    
    public static void SetPool<T>(T target) where T: MonoBehaviour
    {
        if (!_pools.ContainsKey(target.gameObject.name))
        {
            Object.Destroy(target.gameObject);
            return;
        }
        
        _pools[target.name].AddFirst(target);
        target.gameObject.SetActive(false);
    }	
    
    public static void ClearPool()
    {
        _pools.Clear();
    }
}