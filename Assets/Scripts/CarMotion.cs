using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CarMotion : MonoBehaviour
{
    [SerializeField]
    private float _maxTyreAngleDeg = 50f;
    [SerializeField]
    private float _maxSpeed = 20f;

    private float _gearMaxSpeed = 0f;

    private float _carCurrentVelocityRatio = 0f;
    private float _carMaxVelocityRatio = 0f;

    private Rigidbody _carRb = null;

    float _accelerateInput = 0f;
    float _reverseInput = 0f;
    float _steeringInput = 0f;

    [SerializeField]
    private AnimationCurve _torqueCurve;

    [SerializeField]
    private AnimationCurve _steeringCurve;

    [SerializeField]
    private AnimationCurve _brakeCurve;


    private CarWheel[] _allWheels;
    
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

    private int _currentGear = 0;

    [SerializeField]
    private Transform _COM;

    public float MaxSpeed
    {
        get { return _maxSpeed; }
    }

    public bool IsAccelerating
    {
        get { return _accelerateInput > 0.05f; }
    }

    public float GearMaxSpeed
    {
        get { return _gearMaxSpeed; }
    }

    public float CarCurrentVelocityRatio
    {
        get { return _carCurrentVelocityRatio; }
    }

    public int TotalWheelCount
    {
        get { return _allWheels.Length; }
    }

    public int SteeringWheelCount
    {
        get { return _steeringWheels.Length; }
    }

    public int BrakingWheelCount
    {
        get { return _steeringWheels.Length; }
    }

    public int PowerWheelCount
    {
        get { return _steeringWheels.Length; }
    }


    private void Start()
    {
        _carRb = GetComponent<Rigidbody>();
        if (_carRb == null)
        {
            Debug.LogError("Rigdbody was not found");
            return;
        }

        _carRb.centerOfMass = _COM.localPosition;

        _allWheels = GetComponentsInChildren<CarWheel>();
    }

    private void Update()
    {
        TurnWheels();
        CalculateCarVelocity();
    }

    private void FixedUpdate()
    {
        Accelerate();
        Brake();
    }

    private void CalculateCarVelocity()
    {
        float currentForwardVelocity = Vector3.Dot(_carRb.velocity, transform.forward);
        _carCurrentVelocityRatio = Mathf.Clamp(currentForwardVelocity / _gearMaxSpeed, -1f, 1f);
        _carMaxVelocityRatio = Mathf.Clamp(currentForwardVelocity / _maxSpeed, -1f, 1f);

        Debug.Log("currentForwardVelocity: " + currentForwardVelocity);
    }

    private void TurnWheels()
    {
        float singleStep = Time.deltaTime * _maxSpeed;

        foreach(CarWheel wheel in _steeringWheels)
        {
            int input = 0;

            if (wheel.IsLeftWheel)
                input = 180;

            wheel.transform.localRotation = 
                Quaternion.Slerp(wheel.transform.localRotation, 
                Quaternion.Euler(0, _maxTyreAngleDeg * _steeringCurve.Evaluate(_carMaxVelocityRatio) * _steeringInput + input, 0), 
                singleStep);
         }
    }

    private void Accelerate()
    {
        //Split available torque over power wheels
        float torquePerWheel = 1f;
        float torqueSplit = 0f;
        foreach (CarWheel wheel in _powerWheels)
        {
            if (wheel.IsGrounded)
                torqueSplit++;
        }

        if(torqueSplit > 0)
            torquePerWheel /= torqueSplit;

        //Add torque to wheels
        foreach (CarWheel wheel in _powerWheels)
        {
            if(_accelerateInput > 0.01f && _carCurrentVelocityRatio < 0.99f)
                wheel.ApplyTorque(_torqueCurve.Evaluate(_carCurrentVelocityRatio) * _accelerateInput * torquePerWheel * Mathf.Sign(_currentGear) * Time.fixedDeltaTime * 100);
        }
    }

    private void Brake()
    {
        if (_carCurrentVelocityRatio > 0f)
            foreach (CarWheel wheel in _brakeWheels)
            {
                wheel.BrakeFactor = _brakeCurve.Evaluate(_reverseInput) * Mathf.Clamp01(0.2f + _carCurrentVelocityRatio);
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

    #region inputHandling

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

    #endregion
}
