using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [field: SerializeField] public Transform[] Points { get; private set; }

    public int length { get { return Points.Length; } }



    public void GetPoint(int index, out Vector3 position, out Vector3 rotation)
    {
        if (index >= Points.Length)
        {
            position = Vector3.zero;
            rotation = Vector3.zero;
            return;
        }
        position = Points[index].position;
        rotation = Points[index].eulerAngles;
    }
}
