using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    void Awake()
    {
        var objs = GameObject.FindGameObjectsWithTag("AmbientSound");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
