using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    private const string shoot = "Shoot";
    [SerializeField] private Animator _animator;
    private Gun _gun;

    //private void Start()
    //{        
    //    _gun.shoot += Shoot;
    //}

    private void Shoot()
    {
        _animator.SetTrigger(shoot);
    }

    private void OnDestroy()
    {
        if (_gun) _gun.shoot -= Shoot;
    }

    public void SetGun(Gun gun)
    {
        if (_gun) _gun.shoot -= Shoot;
        _gun = gun;
        _gun.shoot += Shoot;
    }
}
