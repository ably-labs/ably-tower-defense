using UnityEngine;

public class StateManagerBehavior : MonoBehaviour
{

    public static StateManagerBehavior Instance;
    public string GameID = "default";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(this.gameObject);
        }
    }
}