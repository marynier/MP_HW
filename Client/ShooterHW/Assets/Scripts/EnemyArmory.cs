using UnityEngine;

public class EnemyArmory : MonoBehaviour
{
    [SerializeField] private EnemyGun[] _guns;
    [SerializeField] private EnemyController _controller;
    [SerializeField] private GunAnimation _gunAnimation;
    private EnemyGun _currentGun;

    private void Awake()
    {
        SwitchGun(1);
    }

    public void SwitchGun(int armoryIndex)
    {
        if (_currentGun != null) _currentGun.gameObject.SetActive(false);        
        EnemyGun chosenGun = _guns[armoryIndex];
        chosenGun.gameObject.SetActive(true);
        _currentGun = chosenGun;       
        _controller.SetGun(chosenGun);
        _gunAnimation.SetGun(_currentGun);
    }
}
