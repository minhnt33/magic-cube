using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthBar : MonoBehaviour
{

    public Image _filledImage;
    public float _smoothTransitionTime;
    private GameObject _bossGO;

    void OnEnable()
    {
        ActorHealth.OnHealthChange += OnHealthChange;
        ActorMaterialEffect.OnStartEffectEvent += OnStartEffectEvent;
        ActorMaterialEffect.OnEndEffectEvent += OnEndEffectEvent;
        GameModeManager.OnSetGameModeEvent += OnSetGameMode;
    }

    void OnDisable()
    {
        ActorHealth.OnHealthChange -= OnHealthChange;
        ActorMaterialEffect.OnStartEffectEvent -= OnStartEffectEvent;
        ActorMaterialEffect.OnEndEffectEvent -= OnEndEffectEvent;
        GameModeManager.OnSetGameModeEvent -= OnSetGameMode;
    }

    void Start()
    {
        _bossGO = GameObject.FindGameObjectWithTag("Boss");
    }

    private void OnHealthChange(GameObject go, float newValue)
    {
        if (go != _bossGO)
            return;

        StartCoroutine(SmoothCoroutine(_filledImage, _filledImage.fillAmount, newValue, _smoothTransitionTime));
    }

    private void OnStartEffectEvent(GameObject go, ActorState state)
    {
        if (go != _bossGO)
            return;

        if (state == ActorState.SLOW)
            _filledImage.color = Color.blue;
        else if (state == ActorState.POISION)
            _filledImage.color = Color.green;
    }

    private void OnEndEffectEvent(GameObject go, ActorState state)
    {
        if (go != _bossGO)
            return;

        _filledImage.color = Color.red;
    }

    private void OnSetGameMode(GameType gameType)
    {
        if (gameType != GameType.BOSS_FIGHT)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private IEnumerator SmoothCoroutine(Image filled, float from, float to, float secs)
    {
        float timer = secs;
        float progress = 0f;
        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(secs, 0f, timer);
            float health = Mathf.Lerp(from, to, progress);
            filled.fillAmount = health;

            if (progress == 1)
                yield break;

            yield return null;
        }
    }
}
