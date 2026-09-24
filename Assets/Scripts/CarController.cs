using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public bool IsInCar => _isInCar;
    public bool IsInTransition => _isInTransition;
    public float CurrentMotorTorque => _currentMotorTorque;
    public float CurrentSpeed => _currentForwardSpeed;
    public float CurrentSteerRange => _currentSteerRange;

    public UnityEvent OnEnterCar => _onEnterCar;
    public UnityEvent OnExitCar => _onExitCar;

    [Header("References")]
    [SerializeField] List<WheelData> _wheels;
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private Transform _exitPoint;
    [SerializeField] private Transform _orbitPoint;
    [SerializeField] private Transform _orbitPivot;

    [Header("Camera Settings")]
    [SerializeField] private float _cameraTransitionSpeed = 5f;
    [SerializeField] private float _cameraTurnSensitivity = 5f;
    [SerializeField] private float _minCameraPitch = -25f;
    [SerializeField] private float _maxCameraPitch = 25f;
    [SerializeField] private float _fovChangeIntensity = 1.5f;
    [SerializeField] private float _fovChangeSpeed = 5f;

    [Header("Car Settings")]
    [SerializeField] private float _motorTorque = 2000f;
    [SerializeField] private float _brakeTorque = 2000f;
    [SerializeField] private float _maxSpeed = 20f;
    [SerializeField] private float _steerRange = 30f;
    [SerializeField] private float _steerRangeAtMaxSpeed = 10f;
    [SerializeField] private float _wheelModelLerpSpeed = 5f;

    [Header("Events")]
    [SerializeField] private UnityEvent _onEnterCar;
    [SerializeField] private UnityEvent _onExitCar;

    private PlayerInput _input;
    private Rigidbody _rb;

    private Vector2 _moveInput = Vector2.zero;
    private Vector2 _lookInput = Vector2.zero;

    private float _currentMotorTorque = 0f;
    private float _currentSteerRange = 0f;
    private float _currentForwardSpeed = 0f;
    private float _currentSpeedFactor = 0f;
    private Transform _lastCameraParent = null;
    private Vector3 _lastCameraLocalPos;

    private float _yaw = 0f;
    private float _pitch = 0f;
    private float _defaultFov = 0f;

    private bool _isInCar = false;
    private bool _isInTransition = false;

    private void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        _input.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!_isInCar || _isInTransition) return;
        UpdateSpeed();
        HandleDriving();
        HandleSteering();
    }

    private void Update()
    {
        if (!_isInCar || _isInTransition) return;
        SyncWheelModels();
    }

    private void LateUpdate()
    {
        if (!_isInCar || _isInTransition) return;
        HandleCamera();
    }

    private void SyncWheelModels()
    {
        foreach (var wheel in _wheels)
        {
            if (wheel.Model == null) continue;

            wheel.Collider.GetWorldPose(out Vector3 pos, out Quaternion rot);

            Vector3 targetPos = pos + wheel.PositionOffset;
            Quaternion targetRot = Quaternion.Euler(rot.eulerAngles + wheel.RotationOffset);

            wheel.Model.position = Vector3.Lerp(wheel.Model.position, targetPos, _wheelModelLerpSpeed * Time.deltaTime);
            wheel.Model.rotation = Quaternion.Slerp(wheel.Model.rotation, targetRot, _wheelModelLerpSpeed * Time.deltaTime);
            wheel.Model.localScale += wheel.ScaleOffset;
        }
    }

    private void HandleCamera()
    {
        float mouseX = _lookInput.x * Time.deltaTime * _cameraTurnSensitivity;
        float mouseY = _lookInput.y * Time.deltaTime * _cameraTurnSensitivity;

        _yaw += mouseX;
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, _minCameraPitch, _maxCameraPitch);

        // Increase fov the faster you drive forwards
        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _defaultFov + _fovChangeIntensity * Mathf.Max(0, _currentForwardSpeed), _fovChangeSpeed * Time.deltaTime);

        // Orbit point is child of orbit pivot, so rotate pivot and point will rotate as well
        _orbitPivot.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);

        // Set camera position to orbit point and rotate facing pivot
        _mainCamera.transform.position = _orbitPoint.position; 
        _mainCamera.transform.LookAt(_orbitPivot);
    }

    private void UpdateSpeed()
    {
        // Calculate current speed along the car's forward axis
        _currentForwardSpeed = Vector3.Dot(transform.forward, _rb.linearVelocity);
        _currentSpeedFactor = Mathf.InverseLerp(0, _maxSpeed, Mathf.Abs(_currentForwardSpeed)); // Normalized speed factor
    }

    private void HandleDriving()
    {
        // Reduce motor torque at high speeds
        _currentMotorTorque = Mathf.Lerp(_motorTorque, 0, _currentSpeedFactor);

        bool changingDirection =
            Mathf.Abs(_currentForwardSpeed) > 0.1f &&
            Mathf.Sign(_moveInput.y) != Mathf.Sign(_currentForwardSpeed);

        foreach (var wheel in _wheels.Where(x => x.Motorized))
        {
            if (changingDirection)
            {
                wheel.Collider.motorTorque = 0f;
                wheel.Collider.brakeTorque = Mathf.Abs(_moveInput.y) * _brakeTorque;
            }
            else
            {
                wheel.Collider.motorTorque = _moveInput.y * _currentMotorTorque;
                wheel.Collider.brakeTorque = 0f;
            }
        }
    }

    private void HandleSteering()
    {
        // Reduce steering at high speeds
        _currentSteerRange = Mathf.Lerp(_steerRange, _steerRangeAtMaxSpeed, _currentSpeedFactor);

        foreach (var wheel in _wheels.Where(x => x.Steerable))
        {
            wheel.Collider.steerAngle = _moveInput.x * _currentSteerRange;
        }
    }

    public void OnLook(InputValue value) => _lookInput = value.Get<Vector2>();

    public void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();

    public void EnterCar() => StartCoroutine(EnterCarRoutine());
    private IEnumerator EnterCarRoutine()
    {
        if (IsInCar) yield break;

        _isInTransition = true;
        _isInCar = true;
        _onEnterCar?.Invoke();

        _lastCameraParent = _mainCamera.transform.parent;
        _lastCameraLocalPos = _mainCamera.transform.localPosition;

        _mainCamera.transform.SetParent(transform);
        _defaultFov = _mainCamera.fieldOfView;

        Cursor.lockState = CursorLockMode.Locked;

        while (Vector3.Distance(_mainCamera.transform.position, _orbitPoint.position) > 0.01f)
        {
            _mainCamera.transform.LookAt(_orbitPivot);
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, _orbitPoint.position, Time.deltaTime * _cameraTransitionSpeed);
            yield return null;
        }

        _rb.constraints = RigidbodyConstraints.None;
        _isInTransition = false;
        _input.enabled = true;
    }

    public void ExitCar() => StartCoroutine(ExitCarRoutine());
    private IEnumerator ExitCarRoutine()
    {
        if (!IsInCar) yield break;

        _input.enabled = false;
        _isInCar = false;
        _onExitCar?.Invoke();

        _rb.constraints = RigidbodyConstraints.FreezeAll;
        Cursor.lockState = CursorLockMode.None;

        _mainCamera.transform.position = _exitPoint.TransformPoint(_lastCameraLocalPos);
        _mainCamera.transform.rotation = _exitPoint.rotation;
        _mainCamera.fieldOfView = _defaultFov;

        ResetCameraParent();

        _orbitPivot.localRotation = Quaternion.identity;
        _yaw = 0;
        _pitch = 0;
    }

    private void ResetCameraParent()
    {
        _lastCameraParent.SetPositionAndRotation(_exitPoint.position, _exitPoint.rotation);

        _mainCamera.transform.SetParent(_lastCameraParent);
        _mainCamera.transform.SetLocalPositionAndRotation(_lastCameraLocalPos, Quaternion.identity);
    }
}