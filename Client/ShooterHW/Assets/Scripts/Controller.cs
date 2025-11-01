using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private PlayerCharacter _player;
    private PlayerGun _gun;
    [SerializeField] private float _mouseSensitivity = 2f;
    private MultiplayerManager _multiplayerManager;
    private bool _wasCrouched = false;
    private bool _hold = false;
    private void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
    }
    void Update()
    {
        if (_hold) return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        bool isShoot = Input.GetMouseButton(0);

        bool space = Input.GetKeyDown(KeyCode.Space);

        bool crouch = Input.GetKey(KeyCode.LeftControl);

        //bool stand = Input.GetKeyUp(KeyCode.LeftControl);

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

        //if (stand) _player.Stand();
        if (space) _player.Jump();


        if (isShoot && _gun.TryShoot(out ShootInfo shootInfo)) SendShoot(ref shootInfo);        

            SendMove();
    }
    private void SendShoot(ref ShootInfo shootInfo)
    {
        shootInfo.key = _multiplayerManager.GetSessionID();
        string json = JsonUtility.ToJson(shootInfo);
        _multiplayerManager.SendMessage("shoot", json);
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
        MultiplayerManager.Instance.SendMessage("move", data);
    }
    private void SendCrouch()
    {
        _player.GetCrouchInfo(out bool crouch);
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"crouch", crouch }
        };

        MultiplayerManager.Instance.SendMessage("crouch", data);
    }
    public void Restart(string jsonRestartInfo)
    {
        RestartInfo info = JsonUtility.FromJson<RestartInfo>(jsonRestartInfo);
        StartCoroutine(Hold());

        _player.transform.position = new Vector3(info.x, 0, info.z);
        _player.SetInput(0, 0, 0);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "pX", info.x },
            { "pY", 0 },
            { "pZ", info.z },
            { "vX", 0 },
            { "vY", 0 },
            { "vZ", 0 },
            { "rX", 0 },
            { "rY", 0 }
        };

        _multiplayerManager.SendMessage("move", data);
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
[Serializable]
public struct RestartInfo
{
    public float x;
    public float z;
}
