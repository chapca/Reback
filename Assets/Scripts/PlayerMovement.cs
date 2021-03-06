using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInputMap _inputs;
    CharacterController _charaCon;
    bool _isMoving;
    public Vector3 WantedDirection;
    [SerializeField] float _speed;
    float _easeInValue;
    [SerializeField] float _easeInSpeed;

    public bool CanMove { get; internal set; } = true;

    Rigidbody _rb;
    [SerializeField] GameObject _playerBody;

    private void Awake()
    {
        _inputs = new PlayerInputMap();
        _inputs.Movement.Move.started += ReadMovementInputs;
        _inputs.Movement.Move.canceled += StopReadingMovementInputs;
    }

    private void Start()
    {
        _charaCon = GetComponent<CharacterController>();
        _isMoving = false;
        WantedDirection = Vector3.zero;
        _easeInValue = 0;
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_isMoving && CanMove)
        {
            EaseInMovement();
            Move();
        }
        if (_rb.IsSleeping())
            _rb.WakeUp();
    }

    private void ReadMovementInputs(InputAction.CallbackContext obj)
    {
        _isMoving = true;
        _easeInValue = 0;
    }

    private void StopReadingMovementInputs(InputAction.CallbackContext obj)
    {
        _isMoving = false;
        WantedDirection = Vector3.zero;
    }

    private void EaseInMovement()
    {
        if (_easeInValue < 1)
        {
            _easeInValue += Time.deltaTime * _easeInSpeed;
            _easeInValue = Mathf.Clamp(_easeInValue, 0, 1);
        }
    }

    private void Move()
    {

        WantedDirection = new Vector3(_inputs.Movement.Move.ReadValue<Vector2>().x, 0 , _inputs.Movement.Move.ReadValue<Vector2>().y);
        _charaCon.Move(WantedDirection * _speed * _easeInValue * Time.deltaTime);

        _playerBody.transform.forward = Vector3.Lerp(_playerBody.transform.forward, WantedDirection, 22 * Time.deltaTime);
    }

    #region disable inputs on Player disable to avoid weird inputs
    private void OnEnable()
    {
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }
    #endregion
}
