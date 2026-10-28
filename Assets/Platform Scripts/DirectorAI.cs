using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;


public class DirectorAI : MonoBehaviour
{
    [SerializeField] PlatformController platform; //reference script on shrinking platform 
    List<PlayerInputScript> players = new List<PlayerInputScript>();//store players in match 

    [SerializeField] float farDistance = 12f;//distance players are considered too far 


    //shrinking speeds
    [SerializeField] float slowSpeed = 0.02f;
    [SerializeField] float normalSpeed = 0.04f;
    [SerializeField] float fastSpeed = 0.08f;

    [SerializeField] float hitWindow = 4f;//recent 
    [SerializeField] int hitsToSlowDown = 3;//number to slow down the srinking

    Queue<float> recentHits = new Queue<float>();//store time of hit
    bool matchEnded;//has match ended

    //for NetworkMatchState
    public int LivingPlayers => CountLivingPlayers();
    public bool MatchEnded => matchEnded;
    public string WinnerName { get; private set; } = "No winner";

    void Update()
    {
        //networked match only the server controls the match.
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        //return if match done
        if (matchEnded)
        {
            return;
        }

        int aliveCount = CountLivingPlayers();//check living players in match 

        //end if there were at lesdt 2 players at start and only one is left 
        if (players.Count >= 2 && aliveCount <= 1)
        {
            EndMatch();
            return;
        }

        //get rid of uncessary old tracked collisions
        while (recentHits.Count > 0 && Time.time - recentHits.Peek() > hitWindow)
        {
            recentHits.Dequeue();
        }


        //changing shrink speed
        // stop shrinking once the platform is minimum size
        if (platform.GetCurrentSize() <= platform.GetMinimumSize())
        {
            platform.SetShrinkSpeed(0f);
            return;
        }

        //if enough hits slow shrink rate
        if (recentHits.Count >= hitsToSlowDown)
        {
            platform.SetShrinkSpeed(slowSpeed);
        }

        //check player distance and speed shrinking if theyre too far apart 
        else if (GetAveragePlayerDistance() > farDistance)
        {
            platform.SetShrinkSpeed(fastSpeed);
        }

        //otherwise normal shrinking speed 
        else
        {
            platform.SetShrinkSpeed(normalSpeed);
        }
    }

    //to register bumpercar hit 
    public void RegisterHit()
    {
        if (matchEnded)
        {
            return;
        }

        recentHits.Enqueue(Time.time);//register time of hit 
    }

    //calculate player distance avg
    float GetAveragePlayerDistance()
    {
        float totalDistance = 0f;//store sum of all player distances
        int pairCount = 0;//counts pairs

        //loop through to make pairs 1st persom
        for (int i = 0; i < players.Count; i++)
        {
            //skip non players
            if (players[i] == null || players[i].IsEliminated())
            {
                continue;
            }

            //loop through to make pairs 2nd person
            for (int j = i + 1; j < players.Count; j++)
            {
                //skip non players
                if (players[j] == null || players[j].IsEliminated())
                {
                    continue;
                }

                totalDistance += Vector3.Distance(players[i].transform.position,players[j].transform.position);//calculate distance between the pair, add to total 

                pairCount++;//record pair count 
            }
        }

        //if no valid pairs give 0 
        if (pairCount == 0)
        {
            return 0f;
        }

        return totalDistance / pairCount;//divide total dist between pairs over num of pairs for avg 
    }

    //active player count
    int CountLivingPlayers()
    {
        int count = 0;//start count at 0 

        //go through each player ref in array 
        foreach (PlayerInputScript player in players)
        {
            //count if there's a player and is not eliminateed 
            if (player != null && !player.IsEliminated())
            {
                count++;
            }
        }

        return count;
    }

    //end the match 
    void EndMatch()
    {
        matchEnded = true;//set to avoid game continuing  
        platform.StopShrinking();//stop shrinking 

        //only 1 is left
        if (LivingPlayers == 1)
        {
            foreach(PlayerInputScript player in players)
            {
                if (player != null &&  !player.IsEliminated())
                {
                    //winner is player # 
                    WinnerName = "Player " + player.GetPlayerIndex();
                    break;
                }
            }
        }
        Debug.Log("GAME ENDED. Winner: " + WinnerName);//print which player # won
    }


    public void AddPlayer(PlayerInputScript player)
    {
        if (player != null && !players.Contains(player))
        {
            players.Add(player);
        }
    }
    public void RemovePlayer(PlayerInputScript player)
    {
        if (player != null)
        {
            players.Remove(player);
        }
    }
}
