using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Final_Survivors.UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject mainMenuButtons; 
        [SerializeField] private GameObject optionsMenu;
        [SerializeField] private GameObject creditsMenu;

        [Header("Player Input")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private InputAction showHideMenuAction;

        private TextMeshProUGUI mainTitle;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Make the cursor visible

            mainTitle = GameObject.Find("Text/Logo").GetComponent<TextMeshProUGUI>();
            mainTitle.outlineWidth = 0.1f;
            mainTitle.outlineColor = Color.black;

            playerInput = GetComponent<PlayerInput>();
            showHideMenuAction = playerInput.actions["Show or Hide Menu"];
        }

        private void Update()
        {
            if (optionsMenu.activeSelf == true && showHideMenuAction.triggered)
            {
                Back(); // return to the main menu
            }

            if (creditsMenu.activeSelf== true && showHideMenuAction.triggered)
            {
                Back(); // return to the main menu
            }
        }

        public void Back()
        {
            mainMenuButtons.SetActive(true);
            creditsMenu.SetActive(false);
            optionsMenu.SetActive(false);
        }

        public void Credits()
        {
            mainMenuButtons.SetActive(false);
            creditsMenu.SetActive(true);
        }

        public void Options()
        {
            mainMenuButtons.SetActive(false);
            optionsMenu.SetActive(true);
        }

        public void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}