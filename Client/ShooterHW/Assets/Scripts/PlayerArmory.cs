using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//public enum Armory
//{
//    empty = 0,
//    gun_0 = 1,
//    blaster_1 = 2,
//    blaster_2 = 3,
//    blaster_3 = 4
//}
public class PlayerArmory : MonoBehaviour
{
    //[SerializeField] private ArmorySettings _armorySettings;
    //[SerializeField] private Transform _parentTransform;
    [SerializeField] private PlayerGun[] _guns;
    [SerializeField] private Controller _controller;
    [SerializeField] private GunAnimation _gunAnimation;
    private PlayerGun _currentGun;
    //public Armory CurrentArmory { get; private set; }
    private void Awake()
    {
        SwitchGun(1);
        SendGunChange(1);
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
        //if (_currentGun != null) Destroy(_currentGun.gameObject);
        //int index = (int)armory;
        //var prefab = _armorySettings.Guns[index];

        //var chosenGun = Instantiate(prefab, _parentTransform.position, _parentTransform.rotation, _parentTransform);
        //_currentGun = chosenGun.GetComponent<Gun>();
        //CurrentArmory = armory;

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
            { "g", index }

        };

        MultiplayerManager.Instance.SendMessage("gunChange", data);
    }
}
