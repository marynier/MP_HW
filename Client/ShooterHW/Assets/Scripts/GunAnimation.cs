using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    private const string shoot = "Shoot";
    private Gun _gun;
    [SerializeField] private Animator _animator;
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
