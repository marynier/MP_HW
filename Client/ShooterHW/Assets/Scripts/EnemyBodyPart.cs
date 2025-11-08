using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    [field: SerializeField] public int DamageMultiplier { get; private set; } = 1;
    [field: SerializeField] public bool CriticalPart { get; private set; } = false;
}
