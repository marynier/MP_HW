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

        //float newX = Mathf.MoveTowardsAngle(currentX, _targetXrotation, rotationStep);
        //float newY = Mathf.MoveTowardsAngle(currentY, _targetYrotation, rotationStep);

        float newX = Mathf.LerpAngle(currentX, _targetXrotation, rotationStep);
        float newY = Mathf.LerpAngle(currentY, _targetYrotation, rotationStep);

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
        Vector3 desiredPosition = position + (velocity * averageInterval);

        //проверка пола (Raycast вниз от desiredPosition)
        RaycastHit hit;
        float rayDistance = 1.0f; //расстояние для проверки пола
        Vector3 rayOrigin = new Vector3(desiredPosition.x, desiredPosition.y + rayDistance, desiredPosition.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance * 2))
        {
            //если луч попал в пол, корректируем позицию по Y
            desiredPosition.y = Mathf.Max(desiredPosition.y, hit.point.y);
        }

        //проверка стен (Raycast от текущей позиции к desiredPosition)
        Vector3 direction = desiredPosition - transform.position;
        float distance = direction.magnitude;
        direction.Normalize();

        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            //если луч попал в стену, ставим targetPosition близко к стене
            desiredPosition = hit.point - direction * 0.1f; //отступ немного назад от стены
        }

        targetPosition = desiredPosition;

        //targetPosition = position + (velocity * averageInterval); //было в уроке
        _velocityMagnitude = velocity.magnitude;

        this.velocity = velocity;
    }
    public void SetCrouch(float value)
    {
       Vector3 newScale = new Vector3(1, value, 1);
       transform.localScale = newScale;
    }
    public void SetRotateX(float value)
    {
        //_head.localEulerAngles = new Vector3(value, 0, 0); //было в уроке
        _targetXrotation = value;

    }
    public void SetRotateY(float value)
    {
        //transform.localEulerAngles = new Vector3(0, value, 0); //было в уроке
        _targetYrotation = value;
    }

}
