using TMPro;
using UnityEngine;

namespace RaceGame
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LapDisplay : MonoBehaviour
    {
        #region Fields

        TMP_Text _text;

        #endregion

        #region Editor Fields

        [SerializeField] private string _lapFormatString = "Current lap: {0}";

        #endregion
        
        #region Life Cycle

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            if(GameManager.Instance.CheckPointTracker == null) return;
            
            GameManager.Instance.CheckPointTracker.OnNewLapStarted += UpdateText;
            _text.text = string.Format(_lapFormatString, GameManager.Instance.CheckPointTracker.LapCounter);
        }

        private void OnDestroy()
        {
            if(GameManager.Instance.CheckPointTracker == null) return;
            
            GameManager.Instance.CheckPointTracker.OnNewLapStarted -= UpdateText;
        }
        
        #endregion
        
        #region Methods

        private void UpdateText(int lapIndex)
        {
            _text.text = string.Format(_lapFormatString, lapIndex);
        }
        
        #endregion
    }
}