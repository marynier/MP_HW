using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private DamageEffect _damageEffectPrefab;
    private int _damage;

    public void Init(Vector3 velocity, int damage = 0)
    {
        _damage = damage;
        _rigidbody.linearVelocity = velocity;
        StartCoroutine(DelayDestroy());
    }

    private IEnumerator DelayDestroy()
    {
        yield return new WaitForSecondsRealtime(_lifeTime);
        Destroy();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out EnemyBodyPart bodyPart))
        {
            int totalDamage = _damage * bodyPart.DamageMultiplier;
            var enemy = collision.collider.GetComponentInParent<EnemyCharacter>();
            enemy.ApplyDamage(totalDamage);
            
            Color color;
            if (bodyPart.CriticalPart) color = Color.red;
            else color = Color.yellow;
            DamageEffect newEffect = Instantiate(_damageEffectPrefab, transform.position, Quaternion.identity);
            newEffect.Setup(totalDamage, color);
        }
        //if (collision.collider.TryGetComponent(out EnemyCharacter enemy))
        //{
        //    enemy.ApplyDamage(_damage);
        //}
        Destroy();
    }
}
