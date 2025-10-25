using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private Transform _head;
    public Vector3 targetPosition {  get; private set; } = Vector3.zero;
    private float _velocityMagnitude = 0;
    private void Start()
    {
        targetPosition = transform.position;
    }
    private void Update()
    {
        if (_velocityMagnitude > 0.1f)
        {
            float maxDistance = _velocityMagnitude * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxDistance);
        }
        else
        {
            transform.position = targetPosition;
        }
    }
    public void SetSpeed(float value) => speed = value;
    public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval)
    {
        targetPosition = position + (velocity * averageInterval);
        _velocityMagnitude = velocity.magnitude;

        this.velocity = velocity;
    }

    public void SetRotateX(float value)
    {
        _head.localEulerAngles = new Vector3(value, 0, 0);
    }
    public void SetRotateY(float value)
    {
        transform.localEulerAngles = new Vector3(0, value, 0);
    }

}
