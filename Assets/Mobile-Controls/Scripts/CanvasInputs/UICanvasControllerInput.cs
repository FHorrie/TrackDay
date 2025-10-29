using UnityEngine;

public class UICanvasControllerInput : MonoBehaviour
{
    [SerializeField]
    private CarInputManager _input;

    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        _input.AccelerateInput = virtualMoveDirection.y;
        _input.SteeringInput = virtualMoveDirection.x;
    }

    public void VirtualLookInput(Vector2 virtualLookDirection)
    {
        
    }

    public void VirtualJumpInput(bool virtualJumpState)
    {
        
    }

    public void VirtualSprintInput(bool virtualSprintState)
    {
        _input.ResetInput = virtualSprintState;
    }

    public void VirtualSwitchInput(bool virtualSwitchState)
    {
        
    }
}