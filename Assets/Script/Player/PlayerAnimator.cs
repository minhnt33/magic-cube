using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _anim;
    public bool IsOnGround { set { _anim.SetBool("Ground", value); } get { return _anim.GetBool("Ground"); } }

    void OnEnable()
    {
        PlayerMovement.OnPlayerMoveSuccessEvent += OnPlayerMoveSuccessEvent;
        PlayerMovement.OnPlayerEndMoveEvent += OnPlayerEndMoveEvent;
        PlayerCubeControl.PlayerOnCubeEvent += PlayerOnCubeEvent;
        PlayerCubeControl.PlayerOnNoCubeEvent += PlayerOnNoCubeEvent;
        PlayerMovement.HasCubeForwardEvent += HasCubeForwardEvent;
        PlayerMovement.OnPlayerEndJumpCubeEvent += OnPlayerEndJumpCubeEvent;
        ActorHealth.OnHealthZero += OnHealthZeroEvent;
    }

    void OnDisable()
    {
        PlayerMovement.OnPlayerMoveSuccessEvent -= OnPlayerMoveSuccessEvent;
        PlayerMovement.OnPlayerEndMoveEvent -= OnPlayerEndMoveEvent;
        PlayerCubeControl.PlayerOnCubeEvent -= PlayerOnCubeEvent;
        PlayerCubeControl.PlayerOnNoCubeEvent -= PlayerOnNoCubeEvent;
        PlayerMovement.HasCubeForwardEvent -= HasCubeForwardEvent;
        PlayerMovement.OnPlayerEndJumpCubeEvent -= OnPlayerEndJumpCubeEvent;
        ActorHealth.OnHealthZero -= OnHealthZeroEvent;
    }

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnPlayerMoveSuccessEvent(Direction dir)
    {
        _anim.SetBool("Run", true);
    }

    private void OnPlayerEndMoveEvent()
    {
        _anim.SetBool("Run", false);
    }

    private void PlayerOnCubeEvent(GameObject cube, bool isDangerous)
    {
        if (isDangerous)
            _anim.SetBool("Scare", true);
        else
            _anim.SetBool("Scare", false);
    }

    private void PlayerOnNoCubeEvent()
    {
        FallToGroundAnimation();
    }

    private void HasCubeForwardEvent(Transform cubeTrans)
    {
        _anim.SetBool("Jump", true);
    }

    private void OnPlayerEndJumpCubeEvent()
    {
        IsOnGround = false;
        _anim.SetBool("Jump", false);
    }

    private void OnHealthZeroEvent(GameObject go)
    {
        if (go != gameObject)
            return;

        _anim.SetTrigger("Die");
    }

    public void FallToGroundAnimation()
    {
        IsOnGround = true;
        _anim.SetBool("Scare", false);
    }
}
