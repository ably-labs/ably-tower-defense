/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System;

public class PlaceMonsterData
{
    public int monsterID;
    public DateTimeOffset? timestamp;
}

public class PlaceMonster : MonoBehaviour
{

    public GameObject monsterPrefab;
    private GameObject monster;
    private GameManagerBehavior gameManager;
    private AblyManagerBehavior ablyManager;
    private Queue actions = new Queue();

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();
        ablyManager = GameObject.Find("AblyManager").GetComponent<AblyManagerBehavior>();
        ablyManager.gameChannel.Subscribe("spot:" + name, message =>
        {
            // Need to uniquely identify actions to avoid unintentional upgrade when trying to place, etc.
            PlaceMonsterData pmd = new PlaceMonsterData();
            pmd.monsterID = int.Parse((string)message.Data);
            pmd.timestamp = message.Timestamp;
            actions.Enqueue(pmd);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (actions.Count == 0) return;
        PlaceMonsterData pmd = (PlaceMonsterData) actions.Peek();
        DateTimeOffset? startTime = ablyManager.startTimeAbly;
        DateTimeOffset? msgTime = pmd.timestamp;
        TimeSpan? diffTime = msgTime - startTime;
        int ticksSince = ablyManager.ticksSinceStart;
        float timeFromTicks = ticksSince * (1000 * Time.fixedDeltaTime);

        if (!diffTime.HasValue)
        {
            PlaceOrUpgradeMonster(pmd.monsterID, msgTime);
            return;
        }

        if (timeFromTicks < diffTime.Value.TotalMilliseconds)
        {
            Time.timeScale = 20;
            return;
        }
        else
        {
            Time.timeScale = 1;
            actions.Dequeue();
            PlaceOrUpgradeMonster(pmd.monsterID, msgTime);
        }
    }

    private bool CanPlaceMonster()
    {
        int cost = monsterPrefab.GetComponent<MonsterData>().levels[0].cost;
        return monster == null && gameManager.Gold >= cost;
    }


    void OnMouseUp()
    {
        if (CanPlaceMonster())
        {
            ablyManager.gameChannel.Publish("spot:" + name, "0");
        }
        else if (CanUpgradeMonster())
        {
            MonsterData monsterData = monster.GetComponent<MonsterData>();
            ablyManager.gameChannel.Publish("spot:" + name, monsterData.levels.IndexOf(monsterData.getNextLevel()).ToString());
        }
    }

    private void PlaceOrUpgradeMonster(int monsterLevel, DateTimeOffset? timestamp)
    {
        if (CanPlaceMonster() && monsterLevel == 0)
        {
            monster = (GameObject)Instantiate(monsterPrefab, transform.position, Quaternion.identity);
            monster.GetComponent<ShootEnemies>().timestamp = timestamp;
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.PlayOneShot(audioSource.clip);

            gameManager.Gold -= monster.GetComponent<MonsterData>().CurrentLevel.cost;
        }
        else if (CanUpgradeMonster())
        {
            MonsterData monsterData = monster.GetComponent<MonsterData>();
            if (monsterData.getNextLevel() == monsterData.levels[monsterLevel])
            {
                monster.GetComponent<MonsterData>().increaseLevel();
                monster.GetComponent<ShootEnemies>().timestamp = timestamp;
                AudioSource audioSource = gameObject.GetComponent<AudioSource>();
                audioSource.PlayOneShot(audioSource.clip);

                gameManager.Gold -= monster.GetComponent<MonsterData>().CurrentLevel.cost;
            }
        }
    }

    private bool CanUpgradeMonster()
    {
        if (monster != null)
        {
            MonsterData monsterData = monster.GetComponent<MonsterData>();
            MonsterLevel nextLevel = monsterData.getNextLevel();
            if (nextLevel != null)
            {
                return gameManager.Gold >= nextLevel.cost;
            }
        }
        return false;
    }

}
