using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    private int score = 0;

    private Text scoreboardText;

    void Start()
    {
        scoreboardText = GameObject.Find("ScoreboardText").GetComponent<Text>();

        if (PhotonNetwork.IsMasterClient)
        {
            // Create a custom room property to store the score
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties.Add("Score", score);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        }
    }

    public void IncrementScore(int amount)
    {
        // Update the score
        score += amount;

        if (PhotonNetwork.IsMasterClient)
        {
            // Update the custom room property
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties.Add("Score", score);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        }
    }

   /* private bool AllPlayersSubmittedScores()
    {
        // Check if all players have submitted their scores
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("Score"))
            {
                return false;
            }
        }

        return true;
    }*/

    private void DisplayScoreboard()
    {
        // Build the scoreboard text
        string scoreboard = "Scoreboard\n";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            scoreboard += player.NickName + ": " + player.CustomProperties["Score"] + "\n";
        }

        // Display the scoreboard
        scoreboardText.text = scoreboard;
    }
}
