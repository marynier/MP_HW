using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    private string _sessionID;

    [SerializeField] private Health _health;
    [SerializeField] private Transform _head;
    public Vector3 targetPosition { get; private set; } = Vector3.zero;
    private float _velocityMagnitude = 0;

    private bool _isCrouched = false;

    //плавный поворот
    [SerializeField] private float _rotationSpeed = 360f;
    private float _targetYrotation;
    private float _targetXrotation;

    public void Init(string sessionID)
    {
        _sessionID = sessionID;
    }

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

    public void SetMaxHP(int value)
    {
        maxHealth = value;
        _health.SetMax(value);
        _health.SetCurrent(value);
    }

    public void RestoreHP(int newValue)
    {
        _health.SetCurrent(newValue);
    }

    public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval)
    {
        Vector3 desiredPosition = position + (velocity * averageInterval);

        //проверка пола
        RaycastHit hit;
        float rayDistance = 1.0f;
        Vector3 rayOrigin = new Vector3(desiredPosition.x, desiredPosition.y + rayDistance, desiredPosition.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance * 2))
        {            
            desiredPosition.y = Mathf.Max(desiredPosition.y, hit.point.y);
        }

        //проверка стен
        Vector3 direction = desiredPosition - transform.position;
        float distance = direction.magnitude;
        direction.Normalize();

        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {            
            desiredPosition = hit.point - direction * 0.1f;
        }

        targetPosition = desiredPosition;

        //targetPosition = position + (velocity * averageInterval); //было в уроке
        _velocityMagnitude = velocity.magnitude;

        this.velocity = velocity;
    }
    public void SetCrouch(bool value)
    {
        _isCrouched = value;
        Vector3 newScale;

        if (_isCrouched) newScale = new Vector3(1, 0.75f, 1);

        else newScale = Vector3.one;

        transform.localScale = newScale;
    }
    public void ApplyDamage(int damage)
    {
        _health.ApplyDamage(damage);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "id", _sessionID },
            {"value", damage }
        };
        MultiplayerManager.Instance.SendMessage("damage", data);
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
