using UnityEngine;
public enum Armory
{
    gun_0 = 1,
    blaster_1 = 2,
    blaster_2 = 3,
    blaster_3 = 4

}
public class PlayerArmory : MonoBehaviour
{
    [SerializeField] private ArmorySettings _armorySettings;
    private GameObject _currentGun;
    [SerializeField] private Transform _parentTransform;
    public Armory CurrentArmory { get; private set; }
    private void Start()
    {
        SwitchGun((Armory)1);
    }
    void Update()
    {
        for (int i = 1; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                SwitchGun((Armory)i);

                SendGunChange((Armory)i);
            }
        }
    }
    public void SwitchGun(Armory armory)
    {
        if (_currentGun != null) Destroy(_currentGun);

        int index = (int)armory;
        var prefab = _armorySettings.Guns[index];
        var chosenGun = Instantiate(prefab, _parentTransform.position, _parentTransform.rotation, _parentTransform);
        _currentGun = chosenGun;

        CurrentArmory = armory;
    }
    void SendGunChange(Armory armory)
    {
        Debug.Log((int)armory);
        // Например, отправить на сервер строковое имя:
        //MultiplayerManager.Instance.SendMessage("gunChange", armory.ToString());
        // Или индекс:
        //MultiplayerManager.Instance.SendMessage("gunChange", (int)armory);
    }
}
