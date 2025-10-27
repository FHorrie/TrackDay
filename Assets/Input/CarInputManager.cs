using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarInputManager : MonoBehaviour
{
    #region Fields

    private float _accelerateInput;
    private float _reverseInput;
    private float _steeringInput;
    private bool _shiftUpInput;
    private bool _shiftDownInput;
    private bool _resetInput;

    #endregion

    #region Properties

    public float AccelerateInput
    {
        get => _accelerateInput;
        set => _accelerateInput = value;
    }
    public float ReverseInput
    {
        get => _reverseInput;
        set => _reverseInput = value;
    }
    public float SteeringInput
    {
        get => _steeringInput;
        set => _steeringInput = value;
    }
    public bool ShiftUpInput
    {
        get => _shiftUpInput;
        set => _shiftUpInput = value;
    }
    public bool ShiftDownInput
    {
        get => _shiftDownInput;
        set => _shiftDownInput = value;
    }
    public bool ResetInput
    {
        get => _resetInput;
        set => _resetInput = value;
    }

    #endregion

    #region Events

    public event Action ShiftDownPressed;
    public event Action ShiftUpPressed;
    public event Action ResetPressed;

    #endregion

    #region Methods

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
        bool value = buttonValue.Get<float>() > 0.5f;
        if (_shiftUpInput == false && value == true)
        {
            ResetPressed?.Invoke();
        }
        _shiftUpInput = value;
    }
    private void OnShiftDown(InputValue buttonValue)
    {
        bool value = buttonValue.Get<float>() > 0.5f;
        if (_shiftDownInput == false && value == true)
        {
            ResetPressed?.Invoke();
        }
        _shiftDownInput = value;
    }
    private void OnResetCar(InputValue buttonValue)
    {
        bool value = buttonValue.Get<float>() > 0.5f;
        if(_resetInput == false && value == true)
        {
            ResetPressed?.Invoke();
        }
        _resetInput = value;
    }

    #endregion
}
