using UnityEngine;

public class MobileControls : MonoBehaviour
{
    #region Properties
    
    private static MobileControls _instance;
    
    #endregion
    
    #region Life Cycle

    private void Awake()
    {
        if (Application.isMobilePlatform && _instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
