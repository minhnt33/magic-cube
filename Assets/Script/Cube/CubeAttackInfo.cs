using UnityEngine;
using System.Collections;

[System.Serializable]
public class CubeAttackInfo : System.Object
{
    [SerializeField]
    private CubeSkill _skillType;
    public CubeSkill SkillType { get { return _skillType; } }

    [SerializeField]
    private GameObject _explodingEffect;
    public GameObject ExplodingEffect { get { return _explodingEffect; } }

    [SerializeField]
    private GameObject _bulletEffect;
    public GameObject BulletEffect { get { return _bulletEffect; } }

    [SerializeField]
    private GameObject _collideEffect;
    public GameObject CollideEffect { get { return _collideEffect; } }

    [SerializeField]
    private float _baseDelaySec = 1;
    public float BaseDelaySeconds { get { return _baseDelaySec; } }

    [SerializeField]
    private SOUND_KEY _explosionSoundKey;
    public SOUND_KEY ExplosionSoundKey { get { return _explosionSoundKey; } }
}
