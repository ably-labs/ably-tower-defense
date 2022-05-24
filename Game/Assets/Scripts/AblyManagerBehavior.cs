using System;
using System.Collections.Generic;
using IO.Ably;
using IO.Ably.Realtime;
using UnityEngine;

public class AblyManagerBehavior : MonoBehaviour
{
    private AblyRealtime realtime = new AblyRealtime(
        new ClientOptions { Key = "INSERT_YOUR_ABLY_API_KEY_HERE" }
      );
    public IRealtimeChannel gameChannel;

    public bool started = false;
    public DateTimeOffset? startTimeAbly;
    public int ticksSinceStart = 0;

    public GameObject startButton;

    // Start is called before the first frame update
    void Awake()
    {
        gameChannel = realtime.Channels.Get("game:" + StateManagerBehavior.Instance.GameID);
        CheckForOldMessages();
    }

    async void CheckForOldMessages()
    {
        PaginatedRequestParams prp = new PaginatedRequestParams();
        prp.Limit = 100;
        PaginatedResult<Message> resultPage = await gameChannel.HistoryAsync(prp);
        List<Message> msgs = resultPage.Items;
        // We need to only use rewind to the most recent start message to avoid starting from a prior game
        for (int i=0; i < msgs.Count; i++)
        {
            if(msgs[i].Name == "start")
            {
                ChannelParams channelParams = new ChannelParams();
                channelParams.Add("rewind", "" + i);
                ChannelOptions channelOptions = new ChannelOptions();
                channelOptions.Params = channelParams;
                gameChannel.SetOptions(channelOptions);
                break;
            }
        }
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
            startButton.SetActive(false);
            ticksSinceStart++;
        }
    }
}
