using UnityEngine;
using System.Collections;
using SWS;

public class BrickSpell : MonoBehaviour, ISpell
{
    public GameObject[] _brickPrefab;
    public Transform[] _brickTrans;
    public GameObject _effect;

    private Transform _trans;
    private Animator _anim;
    private navMove _navMove;
    private Transform _targetTrans;

    public void Init(Animator anim, navMove navMove)
    {
        _trans = transform;
        _anim = anim;
        _navMove = navMove;
    }

    public void Cast()
    {
        _targetTrans = _brickTrans[Random.Range(0, _brickTrans.Length)];
        _navMove.Pause(15f);
        _navMove.FaceToPoint(_targetTrans, 0.5f, 4f);
        _anim.SetBool("Spell", true);
    }

    public void Implementation()
    {
        Instantiate(MathUtils.GetRandomArrayElement(_brickPrefab) as GameObject, _targetTrans.position, Quaternion.identity);
        GameObject effect = Instantiate(_effect, _targetTrans.position, Quaternion.identity) as GameObject;
        effect.transform.parent = transform;
    }
}
