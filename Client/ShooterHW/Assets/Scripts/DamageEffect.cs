using TMPro;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private void Start()
    {
        Invoke(nameof(Die), 0.3f);
    }
    public void Setup(int value, Color color)
    {
        _text.text = value.ToString();
        _text.color = color;
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}
