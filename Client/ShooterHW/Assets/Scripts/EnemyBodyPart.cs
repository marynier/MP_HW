using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    [field: SerializeField] public int DamageMultiplier { get; private set; } = 1;
    [field: SerializeField] public bool CriticalPart { get; private set; } = false;
    [SerializeField] private EnemyCharacter _character;
    private void Start()
    {
        //_character = GetComponentInParent<EnemyCharacter>();
    }
    public void ApplyDamage(int value, out int totalDamage)
    {
        totalDamage = value * DamageMultiplier;
        _character.ApplyDamage(totalDamage);
    }
}
