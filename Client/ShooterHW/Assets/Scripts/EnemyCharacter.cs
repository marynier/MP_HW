using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCharacter : Character
{
    private string _sessionID;

    [SerializeField] private LayerMask _moveRaycastMask;
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
            transform.position = Vector3.Lerp(transform.position, targetPosition, 20 * Time.deltaTime);
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

        Vector3 origin = transform.position;       
        Vector3 direction = desiredPosition - origin;
        float distance = direction.magnitude;
        float maxDistance = float.MaxValue;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, _moveRaycastMask);
        foreach (var hit in hits)
        {
            var currentDistance = Vector3.Distance(transform.position, hit.point);
            if (maxDistance < currentDistance) continue;
            maxDistance = currentDistance;

            desiredPosition = hit.point;
        }

        targetPosition = desiredPosition;        
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
        MultiplayerManager.Instance.SendInfo("damage", data);
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
