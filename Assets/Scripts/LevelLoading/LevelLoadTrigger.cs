using UnityEngine;
using UnityEngine.SceneManagement;

#region RaceGame

public class LevelLoadTrigger : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private string _mapName = string.Empty;

    #endregion

    #region Methods

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer != LayerMask.NameToLayer(Constants.PlayerLayerName)) return;
        SceneManager.LoadSceneAsync(_mapName, LoadSceneMode.Single);
    }

    #endregion
}

#endregion