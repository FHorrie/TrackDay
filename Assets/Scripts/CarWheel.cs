using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    #region Editor Fields
    
    [SerializeField]
    private float _springStrength = 30000f;

    //use formula (2 * sqrt(K * mass) * Zeta) = dampingDensity 
    //Zeta between 0.2 and 1
    //Chosen values give (min: 2190), (max: 10954)
    
    [SerializeField]
    private float _dampingDensity = 4800f;
    
    [SerializeField]
    private float _tyreWeight = 14f;
    
    [SerializeField]
    private float _maxSpringOffset = 0.3f;
    
    [SerializeField]
    private float _restDist = 0.5f;
    
    [SerializeField]
    private float _wheelRadius = 0.25f;
    
    [SerializeField]
    private AnimationCurve _tyreGripCurve;
    
    [SerializeField]
    private float _peakTorque = 6800f;

    [SerializeField]
    private Transform _wheelVisualTransform = null;
    
    [SerializeField]
    private bool _shouldLog = false;
    
    #endregion
    
    #region Fields

    private Vector3 _groundedPoint;
    private Vector3 _contactPoint;

    private Rigidbody _carRb = null;

    private CarMotion _carMotion = null;

    private bool _isGrounded = false;
    private float _brakeFactor = 0;

    public const float BaseBrakeFactor = 0.1f;
    
    #endregion

    #region Properties

    public bool IsGrounded => _isGrounded;

    public float BrakeFactor
    {
        get => _brakeFactor;
        set => _brakeFactor = value;
    }
    
    #endregion

    #region Life Cycle
    
    private void Awake()
    {
        _brakeFactor = BaseBrakeFactor;
    }

    private void Start()
    {
        _carRb = GetComponentInParent<Rigidbody>();
        if (_carRb == null)
            Debug.LogError("Rigdbody was not found");

        _carMotion = GetComponentInParent<CarMotion>();
        if (_carMotion == null)
            Debug.LogError("Motion script was not found");
    }
    
    #endregion

    #region Methods
    
    private void Update()
    {
        RollTyreVisual();
    }

    private void FixedUpdate()
    {
        UpdateTyre();
    }

    private void UpdateTyre()
    {
        if (_carRb == null)
            return;

        float maxLength = _restDist + _maxSpringOffset;

        bool rayDidHit = Physics.SphereCast(transform.position, _wheelRadius, -transform.up, out RaycastHit hitInfo, maxLength - _wheelRadius);

        if (rayDidHit)
        {
            _isGrounded = true;

            UpdateTyreGrip();
            UpdateFriction();
            UpdateSuspension(hitInfo);
        }
        else
        {
            _isGrounded = false;

            _wheelVisualTransform.position = transform.position - transform.up * (-_wheelRadius + _restDist + _maxSpringOffset);
        }
    }

    private void UpdateSuspension(RaycastHit hitInfo)
    {
        //World space direction of spring force
        Vector3 springForceDir = transform.up;

        //World space velocity of the suspension
        Vector3 tyreWorldVelocity = _carRb.GetPointVelocity(transform.position);

        //Calc spring offset
        float springOffset = _restDist - hitInfo.distance;

        //Visual ground point
        _groundedPoint = transform.position - springForceDir * (_restDist - springOffset);

        //Contact point
        _contactPoint = _groundedPoint - springForceDir * _wheelRadius;

        //Calc velocity along spring dir
        //Note: springDir is unit vector
        float velocity = Vector3.Dot(springForceDir, tyreWorldVelocity);

        //Calculate force using simple spring formula:
        float force = (_springStrength * springOffset) - (velocity * _dampingDensity);

        //F = (Offset * SpringStrength) - (Vel * Damping)
        _carRb.AddForceAtPosition(force * springForceDir, transform.position);

        //Match wheels to ray transform
        if (_wheelVisualTransform)
        {
            _wheelVisualTransform.position = _groundedPoint;
        }
    }

    private void UpdateTyreGrip()
    {
        //World space velocity of the suspension
        Vector3 tyreWorldVelocity = _carRb.GetPointVelocity(transform.position);

        //World space normal of the tyre (right vector)
        Vector3 tyreNormal = transform.right;

        //Tire velocity in steering dir
        //Note: springDir is unit vector
        float steeringVelocity = Vector3.Dot(tyreNormal, tyreWorldVelocity);

        Debug.DrawRay(transform.position, tyreNormal * steeringVelocity, Color.red, 1f, true);

        float sideVelocityRatio = Mathf.Clamp01(steeringVelocity / _carMotion.GearMaxSpeed);
        
        if (_shouldLog)
        {
            Debug.Log("steeringVelocity: " + steeringVelocity);
            Debug.Log("sideVelocityRatio: " + sideVelocityRatio);
        }

        //the change in velocity that we're looking for is -steeringVel * gripFactor
        float desiredVelocityChange = -steeringVelocity * _tyreGripCurve.Evaluate(sideVelocityRatio);

        //change turn velocity into acceleration (vel / time)
        float desiredAccel = desiredVelocityChange / Time.fixedDeltaTime;

        //F = Mass * Acceleration
        _carRb.AddForceAtPosition(tyreNormal * _tyreWeight * desiredAccel, _groundedPoint);
    }

    private void UpdateFriction()
    {
        //World space velocity of the suspension
        Vector3 tyreWorldVelocity = _carRb.GetPointVelocity(transform.position);

        //World space direction of the spring force
        Vector3 forwardDir = transform.forward;

        //Tire velocity in steering dir
        //Note: springDir is unit vector
        float frictionVelocity = Vector3.Dot(forwardDir, tyreWorldVelocity);

        //the change in velocity that we're looking for is -steeringVel * gripFactor
        float desiredVelocityChange = -frictionVelocity * _brakeFactor;

        //change turn velocity into acceleration (vel / time)
        float desiredAccel = desiredVelocityChange / Time.fixedDeltaTime;

        //F = Mass * Acceleration
        _carRb.AddForceAtPosition(forwardDir * _tyreWeight * desiredAccel, _groundedPoint);
    }

    private void RollTyreVisual()
    {
        _wheelVisualTransform.Rotate(Vector3.Dot(_carRb.linearVelocity, transform.forward) / (_wheelRadius * 2 * Mathf.PI) * 360 * Time.deltaTime, 0, 0);

        //if(_shouldLog)
        //    Debug.Log("carVelocity: " + Vector3.Dot(_carRb.velocity, transform.forward));
    }

    public void ApplyTorque(float torqueAmount)
    {
        if (_isGrounded)
        { 
            _carRb.AddForceAtPosition(transform.forward * torqueAmount * _peakTorque, _groundedPoint);
        }
    }
    
    #endregion
}