using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (transform.parent != null && transform.root != null) // 부모 오브젝트가 있거나 최상위의 뭐가 존재한다면
        {
            DontDestroyOnLoad(this.transform.root.gameObject); // 최상위오브젝트를 파괴X
        }
        else
        {
            DontDestroyOnLoad(this.gameObject); // 본인이 최상위라면 본인을 파괴X
        }
    }
}