using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform _camera;
    void Start()
    {
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        transform.LookAt(_camera);
    }
}