using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CarMotion : MonoBehaviour
{
    #region Editor Fields
    
    [SerializeField]
    private float _maxTyreAngleDeg = 50f;
    [SerializeField]
    private float _maxSpeed = 20f;

    [SerializeField]
    private AnimationCurve _torqueCurve;

    [SerializeField]
    private AnimationCurve _steeringCurve;

    [SerializeField]
    private AnimationCurve _brakeCurve;
    
    [SerializeField]
    private CarWheel[] _steeringWheels;

    [SerializeField]
    private float _wheelTurnSpeed = 5;

    [SerializeField]
    private CarWheel[] _powerWheels;

    [SerializeField]
    private CarWheel[] _brakeWheels;

    [SerializeField]
    private float[] _forwardGearRatios;

    [SerializeField]
    private float _reverseGearRatio = 0;

    [SerializeField]
    private Transform _COM;
    
    #endregion
    
    #region Fields
    
    private float _gearMaxSpeed = 0f;

    private float _carCurrentVelocityRatio = 0f;
    private float _carMaxVelocityRatio = 0f;

    private Rigidbody _carRb = null;

    private float _accelerateInput = 0f;
    private float _reverseInput = 0f;
    private float _steeringInput = 0f;
    
    
    private int _currentGear = 0;

    #endregion
    
    #region Properties  
    
    public float MaxSpeed => _maxSpeed;

    public bool IsAccelerating => _accelerateInput > 0.01f;

    public float GearMaxSpeed => _gearMaxSpeed;

    public float CarCurrentVelocityRatio => _carCurrentVelocityRatio;

    public int SteeringWheelCount => _steeringWheels.Length;

    public int BrakingWheelCount => _steeringWheels.Length;

    public int PowerWheelCount => _steeringWheels.Length;
    
    #endregion

    #region Life Cycle
    
    private void Awake()
    {
        _carRb = GetComponent<Rigidbody>();
        if (_carRb == null)
        {
            Debug.LogError("Rigdbody was not found");
            return;
        }

        _carRb.centerOfMass = _COM.localPosition;
    }

    #endregion

    #region Methods
    
    private void Update()
    {
        TurnWheels();
    }

    private void FixedUpdate()
    {
        CalculateCarVelocity();
        Accelerate();
        Brake();
    }

    private void CalculateCarVelocity()
    {
        float currentForwardVelocity = Vector3.Dot(_carRb.linearVelocity, transform.forward);
        _carCurrentVelocityRatio = Mathf.Clamp(currentForwardVelocity / _gearMaxSpeed, -1f, 1f);
        _carMaxVelocityRatio = Mathf.Clamp(currentForwardVelocity / _maxSpeed, -1f, 1f);

        Debug.Log($"[CARMOTION] Current forward velocity: {currentForwardVelocity:0.00}");
    }

    private void TurnWheels()
    {
        float singleStep = Time.deltaTime * _maxSpeed;

        for(int i = 0; i < _steeringWheels.Length; ++i)
        {
            CarWheel wheel = _steeringWheels[i];

            wheel.transform.localRotation = 
                Quaternion.Slerp(wheel.transform.localRotation, 
                Quaternion.Euler(0, _maxTyreAngleDeg * _steeringCurve.Evaluate(_carMaxVelocityRatio) * _steeringInput, 0), 
                singleStep);
        }
    }

    private void Accelerate()
    {
        // Split available torque over power wheels
        //float torquePerWheel = 1f;
        //float torqueSplit = 0f;
        //foreach (CarWheel wheel in _powerWheels)
        //{
        //    if (wheel.IsGrounded)
        //        torqueSplit++;
        //}

        //if(torqueSplit > 0)
        //    torquePerWheel /= torqueSplit;

        // Add torque to wheels
        foreach (CarWheel wheel in _powerWheels)
        {
            if(_accelerateInput > 0f && _carCurrentVelocityRatio < 1f)
                wheel.ApplyTorque(_torqueCurve.Evaluate(_carCurrentVelocityRatio) * _accelerateInput * Mathf.Sign(_currentGear) * Time.fixedDeltaTime);
        }
    }

    private void Brake()
    {
        if (_carCurrentVelocityRatio > 0f)
        {
            for (int i = 0; i < _brakeWheels.Length; ++i)
            {
                _brakeWheels[i].BrakeFactor = _brakeCurve.Evaluate(_reverseInput) * Mathf.Clamp01(CarWheel.BaseBrakeFactor + _carCurrentVelocityRatio);
            }
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

    #region Input Handling

    private void OnDrive(InputValue triggerValue)
    {
        _accelerateInput = triggerValue.Get<float>();
    }

    private void OnBrake(InputValue triggerValue)
    {
        _reverseInput = triggerValue.Get<float>();
    }

    private void OnSteering(InputValue stickValue) 
    {
        _steeringInput = stickValue.Get<float>();
    }

    private void OnShiftUp(InputValue buttonValue)
    {
        if (_currentGear < _forwardGearRatios.Length)
        {
            _currentGear++;
            SetGearRatio();
        }

    }

    private void OnShiftDown(InputValue buttonValue)
    {
        if(_currentGear > -1)
        {
            _currentGear--;
            SetGearRatio();
        }
    }

    private void OnResetCar()
    {
        transform.up = Vector3.up;
        transform.position += Vector3.up * 5;

        Debug.Log("[INPUT] car should flip now");
    }

    #endregion

    #endregion
}
