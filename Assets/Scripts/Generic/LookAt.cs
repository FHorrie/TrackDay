using UnityEngine;

#region RaceGame

public class LookAt : MonoBehaviour
{
    #region Editor Fields
    
    [SerializeField]
    private Transform _target;
    
    [SerializeField]
    private float _speed = 3f;
    
    #endregion
    
    #region Life Cycle

    private void Start()
    {
        UpdateRotation(false);
    }
    
    #endregion
    
    #region Methods
    
    // Update is called once per frame
    private void Update()
    {
        UpdateRotation(true);
    }

    private void UpdateRotation(bool useLerp)
    {
        Vector3 direction = (transform.position - _target.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = useLerp ? Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * _speed) : lookRotation;
    }

    #endregion
}

#endregion