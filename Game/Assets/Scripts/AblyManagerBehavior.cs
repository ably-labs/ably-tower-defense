using IO.Ably;
using IO.Ably.Realtime;
using UnityEngine;

public class AblyManagerBehavior : MonoBehaviour
{
    private AblyRealtime realtime = new AblyRealtime(
        new ClientOptions { Key = "INSERT_YOUR_ABLY_API_KEY_HERE" }
        );
    public IRealtimeChannel monsterPlacementChannel;

    // Start is called before the first frame update
    void Awake()
    {
        monsterPlacementChannel = realtime.Channels.Get(StateManagerBehavior.Instance.GameID + ":monsterplacement");
    }
}
