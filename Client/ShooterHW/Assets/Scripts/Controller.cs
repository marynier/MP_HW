using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private float _mouseSensitivity = 2f;
    private MultiplayerManager _multiplayerManager;
    private PlayerGun _gun;
    private bool _wasCrouched = false;
    private bool _hold = false;
    private bool _hideCursor;

    private void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
        _hideCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            _hideCursor = !_hideCursor;
            Cursor.lockState = _hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }

        if (_hold) return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        float mouseX = 0;
        float mouseY = 0;
        bool isShoot = false;
        if (_hideCursor)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            isShoot = Input.GetMouseButton(0);
        }

        bool space = Input.GetKeyDown(KeyCode.Space);

        bool crouch = Input.GetKey(KeyCode.LeftControl);

        _player.SetInput(h, v, mouseX * _mouseSensitivity);
        _player.RotateX(-mouseY * _mouseSensitivity);

        if (crouch && !_wasCrouched)
        {
            _player.Crouch();
            SendCrouch();
        }
        if (!crouch && _wasCrouched)
        {
            _player.Stand();
            SendCrouch();
        }
        _wasCrouched = crouch;

        if (space) _player.Jump();

        if (isShoot && _gun.TryShoot(out ShootInfo shootInfo)) SendShoot(ref shootInfo);

        SendMove();
    }

    private void SendShoot(ref ShootInfo shootInfo)
    {
        shootInfo.key = _multiplayerManager.GetSessionID();
        string json = JsonUtility.ToJson(shootInfo);
        _multiplayerManager.SendInfo("shoot", json);
    }

    private void SendMove()
    {
        _player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY);
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "pX", position.x },
            { "pY", position.y },
            { "pZ", position.z },
            { "vX", velocity.x },
            { "vY", velocity.y },
            { "vZ", velocity.z },
            { "rX", rotateX },
            { "rY", rotateY }
        };
        MultiplayerManager.Instance.SendInfo("move", data);
    }

    private void SendCrouch()
    {
        _player.GetCrouchInfo(out bool crouch);
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"crouch", crouch }
        };

        MultiplayerManager.Instance.SendInfo("crouch", data);
    }

    public void Restart(int spawnIndex)
    {
        _multiplayerManager._spawnPoints.GetPoint(spawnIndex, out Vector3 position, out Vector3 rotation);
        StartCoroutine(Hold());

        _player.transform.position = position;
        rotation.x = 0;
        rotation.z = 0;
        _player.transform.eulerAngles = rotation;
        _player.SetInput(0, 0, 0);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "pX", position.x },
            { "pY", position.y },
            { "pZ", position.z },
            { "vX", 0 },
            { "vY", 0 },
            { "vZ", 0 },
            { "rX", 0 },
            { "rY", rotation.y }
        };

        _multiplayerManager.SendInfo("move", data);
    }

    private IEnumerator Hold()
    {
        _hold = true;
        yield return new WaitForSecondsRealtime(_restartDelay);
        _hold = false;
    }

    public void SetGun(PlayerGun gun)
    {
        _gun = gun;
    }
}

[System.Serializable]
public struct ShootInfo
{
    public string key;
    public float pX;
    public float pY;
    public float pZ;
    public float dX;
    public float dY;
    public float dZ;
}