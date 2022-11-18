using UnityEngine;
using System.Collections.Generic;

public class CubeFaceManager : MonoBehaviour
{
    private static CubeFaceManager _instance;

    public static CubeFaceManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private FaceToSkill[] _skillToFaceArray = new FaceToSkill[6];

    private Dictionary<CubeFace, CubeSkill> _faceToSkillDict;

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _faceToSkillDict = new Dictionary<CubeFace, CubeSkill>();

        for (int i = 0; i < _skillToFaceArray.Length; i++)
        {
            _faceToSkillDict.Add(_skillToFaceArray[i].Face, _skillToFaceArray[i].Skill);
        }
    }

    public CubeSkill UpdateCurrentShowingSkill(Transform cubeTrans)
    {
        return _faceToSkillDict[UpdateCurrentShowingFace(cubeTrans)];
    }

    public CubeFace UpdateCurrentShowingFace(Transform cubeTrans)
    {
        CubeFace face = CubeFace.TOP_FACE;

        if (IsFaceUp(cubeTrans.up))
            face = CubeFace.TOP_FACE;
        else if (IsFaceUp(-cubeTrans.up))
            face = CubeFace.BOTTOM_FACE;
        else if (IsFaceUp(cubeTrans.right))
            face = CubeFace.RIGHT_FACE;
        else if (IsFaceUp(-cubeTrans.right))
            face = CubeFace.LEFT_FACE;
        else if (IsFaceUp(cubeTrans.forward))
            face = CubeFace.FORWARD_FACE;
        else
            face = CubeFace.BACKWARD_FACE;

        return face;
    }

    public CubeSkill GetSkillByFace(CubeFace face)
    {
        return _faceToSkillDict[face];
    }

    private bool IsFaceUp(Vector3 faceSide)
    {
        return Vector3.Dot(faceSide, Vector3.up) > 0.6f;
    }

    [System.Serializable]
    public class FaceToSkill : System.Object
    {
        [SerializeField]
        private CubeFace face;
        public CubeFace Face { get { return face; } }

        [SerializeField]
        private CubeSkill skill;
        public CubeSkill Skill { get { return skill; } }
    }
}
