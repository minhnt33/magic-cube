using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance = null;
    public static PlayerController Instance
    {
        get
        {
            return _instance;
        }
    }

    private PlayerAnimator _anim;
    private PlayerMovement _playerMovement;
    private PlayerCubeControl _playerCubeControl;
    private ActorHealth _playerHealth;
    private Rigidbody _playerBody;
    private CapsuleCollider _collider;

    public bool OnGround { get { return _anim.IsOnGround; } }

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _anim = GetComponent<PlayerAnimator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCubeControl = GetComponent<PlayerCubeControl>();
        _playerBody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _playerHealth = GetComponent<ActorHealth>();
    }

    void OnEnable()
    {
        ActorHealth.OnHealthZero += OnHealthZero;
        PlayerMovement.HasCubeForwardEvent += HasCubeForwardEvent;
        PlayerCubeControl.PlayerOnNoCubeEvent += PlayerOnNoCubeEvent;
        HealthSpell.OnHealthBuffEvent += OnHealthBuff;
    }

    void OnDisable()
    {
        ActorHealth.OnHealthZero -= OnHealthZero;
        PlayerMovement.HasCubeForwardEvent -= HasCubeForwardEvent;
        PlayerCubeControl.PlayerOnNoCubeEvent -= PlayerOnNoCubeEvent;
        HealthSpell.OnHealthBuffEvent -= OnHealthBuff;
    }

    private void OnHealthBuff(float amount)
    {
        _playerHealth.IncreaseHealth(amount);
    }

    private void OnHealthZero(GameObject go)
    {
        if (go != gameObject)
            return;

        _collider.enabled = false;
        _playerMovement.enabled = false;
        _playerCubeControl.enabled = false;
        _anim.enabled = false;
    }

    private void PlayerFallingToGround(bool falling)
    {
        _playerBody.useGravity = falling;
        _playerBody.isKinematic = !falling;
        _collider.isTrigger = !falling;

        _playerMovement.enabled = !falling;
        _playerCubeControl.enabled = !falling;
    }

    private void HasCubeForwardEvent(Transform cubeTrans)
    {
        _playerMovement.MoveOnCube(cubeTrans);
    }

    public void OnEnableMovementEvent()
    {
        PlayerFallingToGround(false);
    }

    private void PlayerOnNoCubeEvent()
    {
        PlayerFallingToGround(true);
        _anim.FallToGroundAnimation();
    }
}
