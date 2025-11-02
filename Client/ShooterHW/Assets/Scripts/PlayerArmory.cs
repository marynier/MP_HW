using System.Collections.Generic;
using UnityEngine;

public class PlayerArmory : MonoBehaviour
{    
    [SerializeField] private PlayerGun[] _guns;
    [SerializeField] private Controller _controller;
    [SerializeField] private GunAnimation _gunAnimation;
    private PlayerGun _currentGun;
    
    private void Awake()
    {
        SwitchGun(1);        
    }

    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                SwitchGun(i);
                SendGunChange(i);
            }
        }
    }

    public void SwitchGun(int index)
    {       
        if (_currentGun != null) _currentGun.gameObject.SetActive(false);
        PlayerGun chosenGun = _guns[index];
        chosenGun.gameObject.SetActive(true);
        _currentGun = chosenGun;
        _controller.SetGun(chosenGun);
        _gunAnimation.SetGun(_currentGun);
    }

    void SendGunChange(int index)
    {
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "gun", index }
        };
        MultiplayerManager.Instance.SendMessage("gunChange", data);
    }
}
