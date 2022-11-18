using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarController : MonoBehaviour
{

    public Image _filledImage;
    public Text _text;
    public float _smoothTransitionTime;
    private GameObject _player;

    void OnEnable()
    {
        ActorHealth.OnHealthChange += OnHealthChange;
        ActorMaterialEffect.OnStartEffectEvent += OnStartEffectEvent;
        ActorMaterialEffect.OnEndEffectEvent += OnEndEffectEvent;
    }

    void OnDisable()
    {
        ActorHealth.OnHealthChange -= OnHealthChange;
        ActorMaterialEffect.OnStartEffectEvent -= OnStartEffectEvent;
        ActorMaterialEffect.OnEndEffectEvent -= OnEndEffectEvent;
    }

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnHealthChange(GameObject go, float newValue)
    {
        if (go != _player)
            return;

        StartCoroutine(SmoothCoroutine(_filledImage, _filledImage.fillAmount, newValue, _smoothTransitionTime));
    }

    private void OnStartEffectEvent(GameObject go, ActorState state)
    {
        if (_player != go)
            return;

        if (state == ActorState.SLOW)
            _filledImage.color = Color.blue;
        else if (state == ActorState.POISION)
            _filledImage.color = Color.green;
    }

    private void OnEndEffectEvent(GameObject go, ActorState state)
    {
        if (go != _player)
            return;

        _filledImage.color = Color.red;
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
            _text.text = Mathf.Round(health * 100) + "/" + 100;

            if (progress == 1)
                yield break;

            yield return null;
        }
    }
}
