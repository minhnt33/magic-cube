using UnityEngine;
using System.Collections;

public class EffectActiveCube : BaseBulletEffect
{

    [SerializeField]
    private CubeSkill _skillType;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void TriggerEffect(GameObject targetGO)
    {
        // Cube10, Enemy9, Player8
        if (targetGO.layer == 10)
        {
            CubeInformation info = targetGO.GetComponent<CubeInformation>();
            if (info.CurrentSkill != _skillType || info.GetCubeType != CubeType.MAGIC)
                return;

            targetGO.GetComponent<CubeExplosion>().BulletTriggerExplode(GridMapManager.Instance.GetGridAt(info.CurrentIndex));
        }
    }
}
