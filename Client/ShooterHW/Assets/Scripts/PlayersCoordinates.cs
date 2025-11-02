using UnityEngine;
using System.Collections.Generic;

public class PlayersCoordinates : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnCoordinates = new List<Transform>();
    public List<Transform> EmptySpawnCoordinates { get; private set; } = new List<Transform>();
    private void Start()
    {
        foreach (Transform t in _spawnCoordinates)
        {
            EmptySpawnCoordinates.Add(t);
        }
    }
    public void GetSpawnCoordinates(out float x, out float z)
    {
        int index = Random.Range(0, EmptySpawnCoordinates.Count);
        x = (float)EmptySpawnCoordinates[index].position.x;
        z = (float)EmptySpawnCoordinates[index].position.z;
        EmptySpawnCoordinates.RemoveAt(index);
    }
}
