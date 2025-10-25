using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private Transform _head;
    public Vector3 targetPosition { get; private set; } = Vector3.zero;
    private float _velocityMagnitude = 0;

    //плавный поворот
    [SerializeField] private float _yawSpeed = 360f;     // body Y
    [SerializeField] private float _pitchSpeed = 360f;   // head X
    private float _targetYaw;   // Y (body)
    private float _targetPitch; // X (head)

    private void Start()
    {
        targetPosition = transform.position;

        _targetYaw = transform.localEulerAngles.y;
        _targetPitch = _head.localEulerAngles.x;
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

        // rotation smoothing
        float yawStep = _yawSpeed * Time.deltaTime;
        float pitchStep = _pitchSpeed * Time.deltaTime;

        float currentYaw = transform.localEulerAngles.y;
        float currentPitch = _head.localEulerAngles.x;

        float newYaw = Mathf.MoveTowardsAngle(currentYaw, _targetYaw, yawStep);
        float newPitch = Mathf.MoveTowardsAngle(currentPitch, _targetPitch, pitchStep);

        // apply without touching roll or лишние оси
        var body = transform.localEulerAngles;
        body.y = newYaw;
        transform.localEulerAngles = body;

        var head = _head.localEulerAngles;
        head.x = newPitch;
        _head.localEulerAngles = head;

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
        //_head.localEulerAngles = new Vector3(value, 0, 0);
        _targetPitch = value;

    }
    public void SetRotateY(float value)
    {
        //transform.localEulerAngles = new Vector3(0, value, 0);
        _targetYaw = value;
    }

}
