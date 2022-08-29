using UnityEngine;

/// <summary>
/// Base class for any script that wants to act as a Singleton
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T I
    {
        get
        {
            // Create instance if none exists
            if(_instance == null)
            {
                // Create a persistent gameobject to hold script
                GameObject obj = new GameObject();
                obj.name = $"{typeof(T)} [Auto-Created]";
                DontDestroyOnLoad(obj);

                // Add script component
                _instance = obj.AddComponent<T>();
                Debug.LogWarning($"Created new '{typeof(T)}' Instance");
            }
            return _instance;
        }
    }

    /// <summary>
    /// Static method for subclass to call on Awake to assign Instance immediately
    /// </summary>
    internal static void CreateInstance(T subclass, GameObject gameObject, bool doNotDestroy = false)
    {
        if (_instance == null)
        {
            _instance = subclass;
            if(doNotDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Static method for any class to check if this instance has been created
    /// </summary>
    public static bool HasInstance()
    {
        return _instance != null;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
