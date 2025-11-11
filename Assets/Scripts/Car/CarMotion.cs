using System;
using Cinemachine;
using RaceGame.Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RaceGame
{
    public class CarMotion : MonoBehaviour
    {
        #region Editor Fields

        [Header("Car Logic")]
        [SerializeField]
        private CarInputManager _playerInput;

        [SerializeField]
        private AnimationCurve _torqueCurve;

        [SerializeField]
        private AnimationCurve _brakeCurve;

        [SerializeField]
        private AnimationCurve _frictionCurve;

        [SerializeField]
        private CarWheel[] _steeringWheels;

        [SerializeField]
        private CarWheel[] _powerWheels;

        [SerializeField]
        private CarWheel[] _brakeWheels;

        [SerializeField]
        private Transform _COM;

        [SerializeField]
        private CinemachineFreeLook _freeLook;

        [SerializeField]
        private float _maxTyreAngleDeg = 50f;
        [SerializeField]
        private float _maxSpeed = 20f;

        [SerializeField]
        private float _wheelTurnSpeed = 5;

        [SerializeField]
        private float _baseFov = 60f;

        [SerializeField]
        private float _increaseFov = 30f;

        [SerializeField]
        private float[] _forwardGearRatios;

        [SerializeField]
        private float _reverseGearRatio = 0;

        [Header("Car Visuals")]
        [SerializeField]
        private MeshRenderer _vehicleRenderer;

        [Space]
        [SerializeField]
        private Color _brakeLightsActiveColor;
        [SerializeField]
        private Color _brakeLightsInactiveColor;

        [Space]
        [SerializeField]
        private Color _reverseLightsActiveColor;
        [SerializeField]
        private Color _reverseLightsInactiveColor;

        #endregion

        #region Fields

        private bool _updateCamera;
        private bool _isBraking;
        private bool _isReversing;

        private int _currentGear = 0;

        private float _gearMaxSpeed = 0f;
        private float _carForwardVelocity = 0f;
        private float _carCurrentVelocityRatio = 0f;
        private float _carMaxVelocityRatio = 0f;

        private Rigidbody _carRb = null;
        private MaterialPropertyBlock _brakeLightsPropertyBlock;
        private MaterialPropertyBlock _reverseLightsPropertyBlock;

        #endregion

        #region Properties  

        public float MaxSpeed => _maxSpeed;

        public bool IsAccelerating => _playerInput.AccelerateInput > 0.01f;

        public float GearMaxSpeed => _gearMaxSpeed;

        public float CarCurrentVelocityRatio => _carCurrentVelocityRatio;

        public int SteeringWheelCount => _steeringWheels.Length;

        public int BrakingWheelCount => _steeringWheels.Length;

        public int PowerWheelCount => _steeringWheels.Length;

        public float CurrentFriction => _frictionCurve.Evaluate(_carMaxVelocityRatio);

        #endregion

        #region Life Cycle

        private void Awake()
        {
            _carRb = GetComponent<Rigidbody>();
            if (_carRb == null)
            {
                Debug.LogError("Rigdbody was not found");
            }
            else
            {
                _carRb.centerOfMass = _COM.localPosition;
            }

            InitializePropertyBlocks();

            if (_freeLook != null)
            {
                _updateCamera = true;
            }

            _playerInput.ShiftUpPressed += ShiftUp;
            _playerInput.ShiftUpPressed += ShiftDown;
            _playerInput.ResetPressed += ResetCar;
        }

        private void InitializePropertyBlocks()
        {
            _brakeLightsPropertyBlock = new MaterialPropertyBlock();
            _brakeLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _brakeLightsInactiveColor);
            _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 2);
            _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 4);

            _reverseLightsPropertyBlock = new MaterialPropertyBlock();
            _reverseLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _reverseLightsInactiveColor);
            _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 5);
            _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 6);
        }

        private void OnDestroy()
        {
            _playerInput.ShiftUpPressed -= ShiftUp;
            _playerInput.ShiftUpPressed -= ShiftDown;
            _playerInput.ResetPressed -= ResetCar;
        }

        #endregion

        #region Methods

        private void Update()
        {
            TurnWheels();
            UpdateCamera();
            RearLightsVisualUpdate();
        }

        private void FixedUpdate()
        {
            CalculateCarVelocity();
            Accelerate();
        }

        private void TurnWheels()
        {
            float singleStep = Time.deltaTime * _wheelTurnSpeed;

            for (int i = 0; i < _steeringWheels.Length; ++i)
            {
                CarWheel wheel = _steeringWheels[i];

                wheel.transform.localRotation =
                    Quaternion.Slerp(wheel.transform.localRotation,
                    Quaternion.Euler(0, _maxTyreAngleDeg * _playerInput.SteeringInput, 0),
                    singleStep);
            }
        }

        private void UpdateCamera()
        {
            if(_updateCamera == false) return;

            // Increase camera FOV with speed (game feel fast )
            _freeLook.m_Lens.FieldOfView = _baseFov + _increaseFov * _carMaxVelocityRatio;
        }

        private void CalculateCarVelocity()
        {
            _carForwardVelocity = Vector3.Dot(_carRb.linearVelocity, transform.forward);
            _carCurrentVelocityRatio = Mathf.Clamp(_carForwardVelocity / _gearMaxSpeed, -1f, 1f);
            _carMaxVelocityRatio = Mathf.Clamp(_carForwardVelocity / _maxSpeed, -1f, 1f);

            //Debug.Log($"[CARMOTION] Current forward velocity: {_carForwardVelocity:0.00}");
        }

        private void Accelerate()
        {
            // Split available torque over power wheels
            float torquePerWheel = 1f;
            float torqueSplit = 0f;
            foreach (CarWheel wheel in _powerWheels)
            {
                if (wheel.IsGrounded)
                    torqueSplit++;
            }

            if(torqueSplit > 0)
                torquePerWheel /= torqueSplit;

            // Add torque to wheels
            foreach (CarWheel wheel in _powerWheels)
            {
                wheel.ApplyTorque(torquePerWheel * _torqueCurve.Evaluate(_carCurrentVelocityRatio) * (_playerInput.AccelerateInput - _playerInput.ReverseInput) * Mathf.Sign(_currentGear) * Time.fixedDeltaTime);
            }
        }

        private void RearLightsVisualUpdate()
        {
            const float triggerThreshold = 0.05f;

            if(_isBraking == false && _playerInput.ReverseInput >= triggerThreshold && _carForwardVelocity > 0f)
            {
                _brakeLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _brakeLightsActiveColor);
                _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 2);
                _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 4);

                _isBraking = true;
            }
            else if(_isBraking && (_playerInput.ReverseInput < triggerThreshold || _carForwardVelocity <= 0f))
            {
                _brakeLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _brakeLightsInactiveColor);
                _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 2);
                _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 4);
                
                _isBraking = false;
            }

            if(_isReversing == false && _playerInput.ReverseInput >= triggerThreshold && _carForwardVelocity < 0f)
            {
                _reverseLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _reverseLightsActiveColor);
                _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 5);
                _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 6);

                _isReversing = true;
            }
            else if(_isReversing && (_playerInput.ReverseInput < triggerThreshold || _carForwardVelocity >= 0f))
            {
                _reverseLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _reverseLightsInactiveColor);
                _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 5);
                _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 6);

                _isReversing = false;
            }
        }

        private void SetGearRatio()
        {
            //Forward Gear
            if (_currentGear > 0)
                _gearMaxSpeed = _maxSpeed * _forwardGearRatios[_currentGear - 1];
            //Neutral
            else if (_currentGear == 0)
                _gearMaxSpeed = 0;
            //Reverse Gear
            else
                _gearMaxSpeed = -_maxSpeed * _reverseGearRatio;
        }

        #region Input Related

        private void ShiftUp()
        {
            if (_currentGear < _forwardGearRatios.Length)
            {
                _currentGear++;
                SetGearRatio();
            }

        }

        private void ShiftDown()
        {
            if (_currentGear > -1)
            {
                _currentGear--;
                SetGearRatio();
            }
        }

        private void ResetCar()
        {
            transform.up = Vector3.up;
            transform.position += Vector3.up * 5;

            Debug.Log("[INPUT] car should flip now");
        }

        #endregion

        #endregion
    }
}