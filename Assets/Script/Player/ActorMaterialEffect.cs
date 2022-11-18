using UnityEngine;
using System.Collections;

public class ActorMaterialEffect : MonoBehaviour
{
    [SerializeField]
    private StateMaterials[] _stateMats;

    private Renderer[] _renderers;
    private Material[] _defaultMats;

    private Coroutine _effectCor;
    private Texture _textureMat;

    public delegate void OnStartEffect(GameObject go, ActorState state);
    public static event OnStartEffect OnStartEffectEvent;

    public delegate void OnEndEffect(GameObject go, ActorState state);
    public static event OnEndEffect OnEndEffectEvent;

    void OnEnable()
    {
        EffectSlow.OnSlowEffectEvent += OnSlowEffectEvent;
        EffectPoison.OnPoisionEffectEvent += OnPoisionEffectEvent;
    }

    void OnDisable()
    {
        EffectSlow.OnSlowEffectEvent -= OnSlowEffectEvent;
        EffectPoison.OnPoisionEffectEvent -= OnPoisionEffectEvent;
    }

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _defaultMats = new Material[_renderers.Length];
        _textureMat = _renderers[0].material.mainTexture;
        for (int i = 0; i < _renderers.Length; i++)
            _defaultMats[i] = _renderers[i].material;
    }

    private void SetActorMaterials(Material mat)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            mat.mainTexture = _textureMat;
            _renderers[i].material = mat;
        }
    }

    private void SetActorMaterials(ActorState state)
    {
        for (int i = 0; i < _stateMats.Length; i++)
        {
            if (_stateMats[i].State == state)
            {
                SetActorMaterials(_stateMats[i].EffectMat);
                break;
            }
        }
    }

    private void ResetMaterials()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = _defaultMats[i];
        }
    }

    private void OnSlowEffectEvent(GameObject go, float duration)
    {
        if (go != gameObject)
            return;
        ChangeActorEffectState(ActorState.SLOW, duration);
    }

    private void OnPoisionEffectEvent(GameObject go, float duration)
    {
        if (go != gameObject)
            return;
        ChangeActorEffectState(ActorState.POISION, duration);
    }

    private void ChangeActorEffectState(ActorState state, float duration)
    {
        if (_effectCor != null)
            StopCoroutine(_effectCor);

        if (OnStartEffectEvent != null)
            OnStartEffectEvent(gameObject, state);

        SetActorMaterials(state);

        _effectCor = StartCoroutine(DelayCor(state, duration));
    }

    IEnumerator DelayCor(ActorState state, float duration)
    {
        yield return new WaitForSeconds(duration);
        ResetMaterials();

        if (OnEndEffectEvent != null)
            OnEndEffectEvent(gameObject, state);
    }

    [System.Serializable]
    public class StateMaterials : System.Object
    {
        public ActorState State;
        public Material EffectMat;
    }
}
