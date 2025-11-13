using RaceGame;
using UnityEngine;
using UnityEngine.InputSystem;

public class DynamicLights : MonoBehaviour
{
    #region Editor Fields

    [Header("Car Visuals")]
    [SerializeField]
    private MeshRenderer _vehicleRenderer;

    [SerializeField]
    private CarMotion _carMotion;
    
    [SerializeField]
    private CarInputManager _input;

    [Header("Car Visuals")]
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

    private MaterialPropertyBlock _brakeLightsPropertyBlock;
    private MaterialPropertyBlock _reverseLightsPropertyBlock;

    private bool _isBraking;
    private bool _isReversing;

    #endregion

    #region Life Cycle

    private void Awake()
    {
        InitializePropertyBlocks();
    }

    #endregion

    #region Methods

    private void Update()
    {
        RearLightsVisualUpdate();
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

    private void RearLightsVisualUpdate()
    {
        const float triggerThreshold = 0.05f;

        if (_isBraking == false && _input.ReverseInput >= triggerThreshold && _carMotion.CarForwardVelocity > 0f)
        {
            _brakeLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _brakeLightsActiveColor);
            _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 2);
            _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 4);

            _isBraking = true;
        }
        else if (_isBraking && (_input.ReverseInput < triggerThreshold || _carMotion.CarForwardVelocity <= 0f))
        {
            _brakeLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _brakeLightsInactiveColor);
            _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 2);
            _vehicleRenderer.SetPropertyBlock(_brakeLightsPropertyBlock, 4);

            _isBraking = false;
        }

        if (_isReversing == false && _input.ReverseInput >= triggerThreshold && _carMotion.CarForwardVelocity < 0f)
        {
            _reverseLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _reverseLightsActiveColor);
            _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 5);
            _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 6);

            _isReversing = true;
        }
        else if (_isReversing && (_input.ReverseInput < triggerThreshold || _carMotion.CarForwardVelocity >= 0f))
        {
            _reverseLightsPropertyBlock.SetColor(Constants.ColorPropertyId, _reverseLightsInactiveColor);
            _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 5);
            _vehicleRenderer.SetPropertyBlock(_reverseLightsPropertyBlock, 6);

            _isReversing = false;
        }
    }

    #endregion
}
