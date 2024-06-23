using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDEBUG : MonoBehaviour
{

        [Header("Debug UI gameobject")]
        public GameObject debugUI;
        public PlayerController playerController;

        [Header("Player State DEBUG UI ")] 
        public TextMeshProUGUI currentPLayerStateStatusText;
        public string currentPlayerStateString;

        [Header(" Left Hand State DEBUG UI ")]
        public TextMeshProUGUI currentLeftHandStateStatusText;
        public string currentLeftHandStateString;

        [Header(" Right Hand State DEBUG UI ")]

        public TextMeshProUGUI currentRightHandStateStatusText;
        public string currentRightHandStateString;


        public void UpdatePlayerStateUI(string currentPlayerState)
        {      
            currentPlayerStateString = currentPlayerState;
            currentPLayerStateStatusText.text = currentPlayerState;
        }

        public void UpdateLeftHandStateUI(string newLeftHandState)
        {
            currentLeftHandStateString = newLeftHandState;
            currentLeftHandStateStatusText.text = newLeftHandState;
        }

        public void UpdateRightHandStateUI(string newRightHandState)
        {
            currentRightHandStateString = newRightHandState;
            currentRightHandStateStatusText.text = newRightHandState;
        }

        public void setUiDebugActive(bool value)
        {
            
           bool uiDebugActive = value;
            if (uiDebugActive)
            {
                Debug.Log("UI Debug is enabled at start");
                playerController.playerStateUI.SetActive(true);
            }
            else
            {
                Debug.Log("UI Debug is disabled at start");
                playerController.playerStateUI.SetActive(false);
            }
        
        }

        
        
}
