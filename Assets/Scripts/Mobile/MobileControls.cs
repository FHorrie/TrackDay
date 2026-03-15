using System;
using UnityEngine;

public class MobileControls : MonoBehaviour
{
    #region Life Cycle

    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
