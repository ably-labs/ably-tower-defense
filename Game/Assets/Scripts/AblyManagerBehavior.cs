using System;
using IO.Ably;
using IO.Ably.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class AblyManagerBehavior : MonoBehaviour
{
    private AblyRealtime realtime = new AblyRealtime(
        new ClientOptions { Key = "INSERT_YOUR_ABLY_API_KEY_HERE" }
        );
    public IRealtimeChannel gameChannel;

    public bool started = false;
    public DateTimeOffset? startTimeAbly;
    public int ticksSinceStart = 0;

    // Start is called before the first frame update
    void Awake()
    {
        gameChannel = realtime.Channels.Get(StateManagerBehavior.Instance.GameID);
        gameChannel.Subscribe("start", (msg) =>
        {
            StartGame(msg.Timestamp);
        });
    }

    public void StartGame(DateTimeOffset? timestamp)
    {
        if (!started) startTimeAbly = timestamp;
        started = true;
    }

    public void SendStartGame()
    {
        gameChannel.Publish("start", "");
    }

    void FixedUpdate()
    {
        if (started)
        {
            ticksSinceStart++;
        }
    }
}
