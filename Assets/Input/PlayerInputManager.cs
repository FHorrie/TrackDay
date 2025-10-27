using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
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

    public float AccelerateInput =>_accelerateInput;
    public float ReverseInput =>_reverseInput;
    public float SteeringInput => _steeringInput;
    public bool ShiftUpInput => _shiftUpInput;
    public bool ShiftDownInput => _shiftDownInput;
    public bool ResetInput => _resetInput;

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
