using UnityEngine;
using System.Collections;
using SWS;

public interface ISpell {

    void Init(Animator anim, navMove navMove);
    void Cast();
    void Implementation();
}
