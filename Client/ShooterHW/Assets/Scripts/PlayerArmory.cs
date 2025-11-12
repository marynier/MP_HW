using System.Collections.Generic;
using UnityEngine;

public class PlayerArmory : MonoBehaviour
{
    [SerializeField] private PlayerGun[] _guns;    
    [SerializeField] private GunAnimation _gunAnimation;
    private PlayerGun _currentGun;
    public int gunsCount => _guns.Length;    
        
    public bool TryChangeGun(int index, out PlayerGun gun)
    {
        gun = null;
        int count = _guns.Length;
        if (index < 0 || index >= count) return false;

        SwitchGun(index);

        gun = _currentGun;
        return true;
    }

    private void SwitchGun(int index)
    {
        if (_currentGun != null) _currentGun.gameObject.SetActive(false);
        PlayerGun chosenGun = _guns[index];
        chosenGun.gameObject.SetActive(true);
        _currentGun = chosenGun;        
        _gunAnimation.SetGun(_currentGun);
    }

    
}
