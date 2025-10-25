using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private Transform _head;
    public Vector3 targetPosition { get; private set; } = Vector3.zero;
    private float _velocityMagnitude = 0;

    //плавный поворот
    [SerializeField] private float _rotationSpeed = 360f;
    private float _targetYrotation;
    private float _targetXrotation;

    private void Start()
    {
        targetPosition = transform.position;

        _targetYrotation = transform.localEulerAngles.y;
        _targetXrotation = _head.localEulerAngles.x;
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

        //расчет поворота
        float rotationStep = _rotationSpeed * Time.deltaTime;
        float currentX = _head.localEulerAngles.x;
        float currentY = transform.localEulerAngles.y;

        float newX = Mathf.MoveTowardsAngle(currentX, _targetXrotation, rotationStep);
        float newY = Mathf.MoveTowardsAngle(currentY, _targetYrotation, rotationStep);

        //применение поворота
        var head = _head.localEulerAngles;
        head.x = newX;
        _head.localEulerAngles = head;

        var body = transform.localEulerAngles;
        body.y = newY;
        transform.localEulerAngles = body;
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
        _targetXrotation = value;

    }
    public void SetRotateY(float value)
    {
        //transform.localEulerAngles = new Vector3(0, value, 0);
        _targetYrotation = value;
    }

}
