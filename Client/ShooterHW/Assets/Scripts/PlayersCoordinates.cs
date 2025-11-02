using UnityEngine;
using System.Collections.Generic;

public class PlayersCoordinates : MonoBehaviour
{
    public List<Vector3> PlayerCoordinates = new List<Vector3>();
    
    void Update()
    {
        UpdatePlayerCoordinates();
    }
    private void UpdatePlayerCoordinates()
    {
        PlayerCoordinates.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PlayerCoordinates.Add(player.transform.position);
        }
    }
}
