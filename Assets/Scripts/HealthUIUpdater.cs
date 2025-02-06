using UnityEngine;

public class HealthUIUpdater : MonoBehaviour
{
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            UpdateHeartSprites();
        }
    }

    public int MaxHealth = 5;

    private SpriteRenderer[] _heartSprites;
    private int _currentHealth;

    private void Start()
    {
        _heartSprites = GetComponentsInChildren<SpriteRenderer>();

        if (_heartSprites == null || _heartSprites.Length == 0)
        {
            Debug.LogError("SpriteRenderers not found in children of HealthUIUpdater!");
            return;
        }

        _currentHealth = MaxHealth;
        UpdateHeartSprites();
    }

    private void UpdateHeartSprites()
    {
        // Повторная инициализация на случай вызова метода из LevelController
        if (_heartSprites == null || _heartSprites.Length == 0)
        {
            _heartSprites = GetComponentsInChildren<SpriteRenderer>();
            if (_heartSprites == null || _heartSprites.Length == 0)
            {
                Debug.LogError("SpriteRenderers not found in children of HealthUIUpdater!");
                return;
            }
        }

        for (int i = 0; i < _heartSprites.Length; i++)
        {
            if (i < _currentHealth)
            {
                _heartSprites[i].enabled = true;
            }
            else
            {
                _heartSprites[i].enabled = false;
            }
        }
    }
}
