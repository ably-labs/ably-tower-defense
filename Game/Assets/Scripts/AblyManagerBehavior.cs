using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class AblyManagerBehavior : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Connect(string id);

    [DllImport("__Internal")]
    public static extern void Publish(string name, string data);

    public bool started = false;
    public DateTimeOffset? startTimeAbly;
    public int ticksSinceStart = 0;

    public GameObject startButton;

    // Start is called before the first frame update
    void Awake()
    {

    }

    void Start()
    {
        Connect(StateManagerBehavior.Instance.GameID);
    }

    public void AblyPublish(string name, string data)
    {
        Publish(name, data);
    }

    public void StartGame(string timestamp)
    {
        DateTimeOffset time = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(timestamp));
        if (!started) startTimeAbly = time;
        started = true;
    }

    public void SendStartGame()
    {
        if (started)
        {
            return;
        }

        AblyPublish("start", "");
    }

    void FixedUpdate()
    {
        if (started)
        {
            startButton.SetActive(false);
            ticksSinceStart++;
        }
    }
}
