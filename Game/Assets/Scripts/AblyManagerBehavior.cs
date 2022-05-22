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

    public void StartGame()
    {
        started = true;
    }

    public void SendStartGame()
    {
        gameChannel.Publish("start", "");
    }

    // Start is called before the first frame update
    void Awake()
    {
        gameChannel = realtime.Channels.Get(StateManagerBehavior.Instance.GameID);
        gameChannel.Subscribe("start", (msg) =>
        {
            StartGame();
        });
    }
}
