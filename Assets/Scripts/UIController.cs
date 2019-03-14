using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // The UIController handles all the UI elements and actions that they take. It will use references to otherControllers to
    // enact actions during runtime of the scene. Each button will refer to the references in this object as opposed to calling the actions
    // to those objects in the scene.

    // Other Controller Objects
    GameData gd;

    // UI prefabs
    [SerializeField]
    GameObject buttonPrefab = null; // Just a basic button for creating buttons by script

    // Timer Related Objects
    [SerializeField]
    Text timer = null;
    float startTime;
    bool finished;

    [SerializeField]
    Text genText = null;

    //Panels used, each panel gets its buttons added in dynamically using the dictionaries 
    GameObject mainPanel;

    // constants used to shape and scale the UI panels and buttons
    const int SIZE_OF_BUTTON_MARGIN = 5;
    const int WIDTH_OF_BUTTON = 160;
    const int SPACING_OF_BUTTON = 30;

    // DataLog
    bool DebugIsOn;
    GameObject dataLogPanel;
    Text DataLog = null;
    
    // Settings Menu
    GameObject settingsPanel;

    public void Awake()
    {

        gd = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();

        DebugIsOn = true;
        finished = false;
        startTime = Time.time;
    }
    public void Start()
    {
        // Find all relevant Game Objects and their components
        FindAllRelevantUIObjects();

        // Load up the main Panel buttons and apply functionality, resize mainPanel to fit
        ConfigureMainPanel();

        // TODO make a secondary panel like the main one

        // TODO add the click to the settings menu to make it disappear
        // TODO also make the settings menu work like the Main Panel too cause its good



        // disable all hidden elements
        dataLogPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
    private void FindAllRelevantUIObjects()
    {
        mainPanel = GameObject.FindGameObjectWithTag("MainButtonPanel");
        DataLog = GameObject.FindGameObjectWithTag("DataLog").GetComponent<Text>();
        dataLogPanel = GameObject.FindGameObjectWithTag("DataLogPanel");
        settingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");
    }

    private void ConfigureMainPanel()
    {
        Dictionary<string, UnityAction> mainPanelButtonActions = new Dictionary<string, UnityAction>
        {
            { "Back To Editing",                () => BackToEditing()            },
            { "Quit",                           () => QuitApplication()          }            
        };

        // Create a button and parent it to Main Panel for each buttonAction
        foreach (var item in mainPanelButtonActions)
        {
            GameObject button = Instantiate(buttonPrefab, mainPanel.transform);
            button.transform.GetChild(0).GetComponent<Text>().text = item.Key;
            button.GetComponent<Button>().onClick.AddListener(item.Value);
        }
        // Get main panel buttons
        Transform mainPanelTransform = mainPanel.GetComponent<Transform>();
        int children = mainPanelTransform.childCount;

        mainPanelTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(WIDTH_OF_BUTTON + 2 * SIZE_OF_BUTTON_MARGIN, (children * SPACING_OF_BUTTON) + 2 * SIZE_OF_BUTTON_MARGIN);
        mainPanelTransform.GetComponent<RectTransform>().position = new Vector3((WIDTH_OF_BUTTON + 2 * SIZE_OF_BUTTON_MARGIN) / 2, SIZE_OF_BUTTON_MARGIN + (children * SPACING_OF_BUTTON) / 2, 0);

        for (int i = 0; i < children; ++i)
        {
            mainPanelTransform.GetChild(i).GetComponent<RectTransform>().anchorMin = new Vector2(.5f, 1f);
            mainPanelTransform.GetChild(i).GetComponent<RectTransform>().anchorMax = new Vector2(.5f, 1f);

            Transform buttonTransform = mainPanelTransform.GetChild(i);
            buttonTransform.position = new Vector3((WIDTH_OF_BUTTON + SIZE_OF_BUTTON_MARGIN * 2) / 2, SIZE_OF_BUTTON_MARGIN * 4 + (i * 30), 0);
        }
    }
    public void BackToEditing()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
    public void FixedUpdate()
    {
        if (!finished)
        {
            float guiTime = Time.time - startTime;
            timer.text = "Time: " + guiTime;
        }
    }

    public void LogDataToScreen(string data)
    {
        if(DebugIsOn)
            DataLog.text = string.Concat(DataLog.text, System.Environment.NewLine, data);
    }
    public void ToggleShowSettingsMenu()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
