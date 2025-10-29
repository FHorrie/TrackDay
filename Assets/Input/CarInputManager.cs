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
        set
        {
            if (_shiftUpInput == false && value == true)
            {
                ShiftUpPressed?.Invoke();
            }
            _shiftUpInput = value;
        }
    }
    public bool ShiftDownInput
    {
        get => _shiftDownInput;
        set
        {
            if (_shiftDownInput == false && value == true)
            {
                ShiftDownPressed?.Invoke();
            }
            _shiftDownInput = value;
        }
    }
    public bool ResetInput
    {
        get => _resetInput;
        set 
        {
            if (_resetInput == false && value == true)
            {
                ResetPressed?.Invoke();
            }
            _resetInput = value;
        } 
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
        AccelerateInput = triggerValue.Get<float>();
    }
    private void OnBrake(InputValue triggerValue)
    {
        ReverseInput = triggerValue.Get<float>();
    }
    private void OnSteering(InputValue stickValue)
    {
        SteeringInput = stickValue.Get<float>();
    }
    private void OnShiftUp(InputValue buttonValue)
    {
        ShiftUpInput = buttonValue.Get<float>() > 0.5f;
    }
    private void OnShiftDown(InputValue buttonValue)
    {
        ShiftDownInput = buttonValue.Get<float>() > 0.5f;
    }
    private void OnResetCar(InputValue buttonValue)
    {
        ResetInput = buttonValue.Get<float>() > 0.5f;
    }

    #endregion
}
