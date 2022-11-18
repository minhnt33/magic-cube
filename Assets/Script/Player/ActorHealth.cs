using UnityEngine;
using System.Collections;

public class ActorHealth : MonoBehaviour
{
    [SerializeField]
    private SOUND_KEY _deathSound;

    [SerializeField]
    private SOUND_KEY _hurtSound;

    public delegate void OnActorDeath(GameObject go);
    public static event OnActorDeath OnHealthZero;

    public delegate void OnActorHealthChange(GameObject go, float newHealthPercent);
    public static event OnActorHealthChange OnHealthChange;

    [SerializeField]
    private float _maxHealth = 100f;
    [SerializeField]
    private float _currentHealth = 0f;

    public float currentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            if (value < 0f)
                _currentHealth = 0f;

            if (_currentHealth != value)
            {
                _currentHealth = value;
                if (OnHealthChange != null)
                    OnHealthChange(gameObject, CurrentHealthPercentage);

                if (_currentHealth <= 0f)
                {
                    AudioManager.Instance.PlaySound(_deathSound);
                    if (OnHealthZero != null)
                        OnHealthZero(gameObject);
                }
            }
        }
    }

    public float CurrentHealthPercentage { get { return _currentHealth / _maxHealth; } }

    private Coroutine _decreaseHealthCor;

    void Awake()
    {
        currentHealth = _maxHealth;
    }

    public void ChangeCurrentHealthTo(float newCurHealth)
    {
        currentHealth = newCurHealth;
    }

    public void ChangeMaxHealthTo(float newMaxHealth)
    {
        _maxHealth = newMaxHealth;
    }

    public void ChangeCurrentHealthByPercent(float percent)
    {
        currentHealth *= percent / 100;
    }

    public void ChangeMaxHealthByPercent(float percent)
    {
        _maxHealth *= percent / 100;
    }

    public void DecreaseHealth(float amount)
    {
        if (currentHealth - amount < 0f)
        {
            currentHealth = 0f;
            return;
        }

        currentHealth -= amount;
    }

    public void IncreaseHealth(float amount)
    {
        if (currentHealth + amount > _maxHealth)
        {
            currentHealth = _maxHealth;
            return;
        }

        currentHealth += amount;
    }

    public void DecreaseHealthInSeconds(float totalDam, float sec)
    {
        if (_decreaseHealthCor != null)
            return;
        // Start burn health
        _decreaseHealthCor = StartCoroutine(DecreaseHealthCoroutine(totalDam, sec));
    }

    // Coroutine for BurnHealthInSeconds
    IEnumerator DecreaseHealthCoroutine(float totalDam, float sec)
    {
        float timer = sec;
        float progress = 0f;
        float startingHealth = currentHealth;
        float targetHealth = currentHealth - totalDam;

        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(sec, 0f, timer);

            currentHealth = Mathf.Lerp(startingHealth, targetHealth, progress);

            if (currentHealth <= 0f)
                yield break;

            if (progress == 1f)
            {
                _decreaseHealthCor = null;
                yield break;
            }

            yield return null;
        }
    }

    public float GetMaxHealth()
    {
        return _maxHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        // 13 is particle layer, 14 is enenmy bullet layer
        if (other.gameObject.layer == 13 || other.gameObject.layer == 14)
            AudioManager.Instance.PlaySound(_hurtSound);
    }
}
