using UnityEngine;
using System.Collections;
using SWS;

public class BossSpell : MonoBehaviour, EnemyBehavior {

    public GameObject[] _spellPrefab;

    private Animator _anim;
    private navMove _navMove;
    private ISpell _curSpell;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _navMove = GetComponent<navMove>();
    }

    public void execute()
    {
        GameObject go = Instantiate(_spellPrefab[Random.Range(0, _spellPrefab.Length)]) as GameObject;

        _curSpell = go.GetComponent<ISpell>();
        _curSpell.Init(_anim, _navMove);
        _curSpell.Cast();
    }

    public void ActiveSpell()
    {
        _curSpell.Implementation();
    }

    public void ResetSpell()
    {
        _navMove.Resume();
        _anim.SetBool("Spell", false);
    }
}
