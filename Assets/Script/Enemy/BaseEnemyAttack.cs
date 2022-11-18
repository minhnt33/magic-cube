using UnityEngine;
using System.Collections;
using SWS;

public abstract class BaseEnemyAttack : MonoBehaviour, EnemyBehavior
{
    [SerializeField]
    protected Transform[] _attackPoints;
    [SerializeField]
    private SOUND_KEY _attackSoundKey;

    private Animator _anim;
    private splineMove _splineMove;
    protected Transform _trans;
    protected Transform _target;

    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
        _splineMove = GetComponent<splineMove>();
        _trans = transform;
    }

    public void execute()
    {
        SetTransformTarget();
        _splineMove.Pause();
        _splineMove.FaceToPoint(_target, 0.5f, 2.5f);
        _anim.SetBool("Attack", true);
        AudioManager.Instance.PlaySound(_attackSoundKey);
    }

    public abstract void Fire();
    public abstract void SetTransformTarget();

    public void ResetAttack()
    {
        _anim.SetBool("Attack", false);
        _splineMove.Resume();
    }
}
