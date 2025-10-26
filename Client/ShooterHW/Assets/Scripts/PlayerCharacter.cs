using UnityEngine;

public class PlayerCharacter : Character
{

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private float _maxHeadAngle = 90;
    [SerializeField] private float _minHeadAngle = -90;
    [SerializeField] private float _jumpForce = 50f;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private float _jumpDelay = 0.2f;
    private float _inputH;
    private float _inputV;
    private float _rotateY;
    private float _currentRotateX;
    private float _jumpTime;
    private bool _isInSquat = false;
    private void Start()
    {
        Transform camera = Camera.main.transform;
        camera.parent = _cameraPoint;
        camera.localPosition = Vector3.zero;
        camera.localRotation = Quaternion.identity;
    }
    public void SetInput(float h, float v, float rotateY)
    {
        _inputH = h;
        _inputV = v;
        _rotateY += rotateY;
    }
    void FixedUpdate()
    {
        Move();
        RotateY();
    }
    private void Move()
    {
        Vector3 velocity = (transform.forward * _inputV + transform.right * _inputH).normalized * speed;
        velocity.y = _rigidbody.linearVelocity.y;
        base.velocity = velocity;
        _rigidbody.linearVelocity = base.velocity;
    }
    private void RotateY()
    {
        _rigidbody.angularVelocity = new Vector3(0, _rotateY, 0);
        _rotateY = 0;
    }
    public void RotateX(float value)
    {
        _currentRotateX = Mathf.Clamp(_currentRotateX + value, _minHeadAngle, _maxHeadAngle);
        _head.localEulerAngles = new Vector3(_currentRotateX, 0, 0);
    }

    public void GetMoveInfo(out Vector3 position, out Vector3 velocity, out float scaleY, out float rotateX, out float rotateY)
    {
        position = transform.position;
        velocity = _rigidbody.linearVelocity;
        scaleY = transform.localScale.y;

        rotateX = _head.localEulerAngles.x;
        rotateY = transform.eulerAngles.y;
    }
    public void Jump()
    {
        if (_checkFly.IsFly) return;
        if (Time.time - _jumpTime < _jumpDelay) return;
        if (_isInSquat) Stand(true);
        _jumpTime = Time.time;
        _rigidbody.AddForce(0, _jumpForce, 0, ForceMode.VelocityChange);

    }
    public void Squat()
    {
        if (_checkFly.IsFly) return;
        if (_isInSquat) return;
        transform.localScale = new Vector3(1, 0.75f, 1);
        _isInSquat = true;
    }
    public void Stand(bool force = false)
    {
        if (!force && _checkFly.IsFly) return;
        transform.localScale = Vector3.one;
        _isInSquat = false;
    }
}
