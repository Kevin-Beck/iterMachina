using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using B83.Win32;

public class EditorUIController : MonoBehaviour
{
    // Other Controller Objects
    GameData gd;
    BotEditorCameraTest bect;

    [SerializeField] GameObject mainPanel = null;
    [SerializeField] GameObject InstructionCanvas = null;
    [SerializeField] InputField dnaInputText;
    bool instructions = false;

    string[] FirstNames = { "adorable", "beautiful","clean","drab","elegant","fancy","glamorous","handsome",
    "long","magnificent","old","plain","quaint","sparkling","ugliest","unsightly","wildeyed","angry","bewildered",
    "clumsy","defeated","embarrassed","fierce", "grumpy","helpless","itchy","jealous","lazy","mysterious","nervous",
    "obnoxious","panicky","repulsive","scary","thoughtless","uptight","worried","agreeable","brave","calm",
    "delightful","eager","faithful","gentle","happy","jolly","kind","lively","nice","obedient","proud",
    "relieved","silly","thankful","victorious","witty","zealous","ancient","brief","early","fast","late","long",
    "modern","quick","rapid","short","slow","swift","young","bitter","delicious","fresh","greasy","juicy","hot",
    "icy","loose","melted","nutritious","prickly","rainy","rotten","salty","sticky","strong","sweet","tart",
    "tasteless","uneven","weak","wet","wooden","yummy", "malnourished", "brutish", "ogrish", "fierce", "violent"};

    string[] LastNames = {"Aardvark", "Abdomen", "Academics", "Accordian", "Action", "Algorithm", "Album",
    "Alarm", "Ambassador", "Ammunition", "Analyst", "Anatomy", "Antique", "Ape", "Attorney", "Avocado",
    "Barber", "Balloonist", "Battalion", "BeanStalk", "Belly", "Begger", "Blizzard", "Bloodflow", "Bricklayer",
    "Broom", "Bronco", "Cannibal", "Candidate", "Carbon", "Caravan", "Circle", "Conspiracy", "Connection",
    "Detour", "Diamond", "Detention", "Doctrine", "Divide", "Fallacy", "Fantasy", "Familiar", "Force",
    "Folklore", "Formula", "Giraffe", "Genie", "Gift", "Gondola", "Hyphenation", "Ignorance", "Impala",
    "JellyFish", "Justice", "Knife", "King", "Liver", "Livestock", "Mandolin", "Manhunt", "Monopoly", "Molecule",
    "Orange", "Paperwork", "Pickaxe", "Pocket", "Portfolio", "Protein", "Raisin", "Rain", "Railroad",
    "Salsa", "Sabre", "Salmon", "Keyboard", "Shoreline", "Shipyard", "Toaster", "Tractor", "Tripod", "Trowel",
    "Violin", "Vessel", "Venom", "Virtue", "Widget", "Wisdom", "Willpower", "Zucchini", "Suit", "Triceratops"};

    private void Awake()
    {
        gd = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
        bect = GameObject.FindGameObjectWithTag("EditorController").GetComponent<BotEditorCameraTest>();
    }
    void Start()
    {
        ConfigureMainPanel();
    }
    // important to keep the instance alive while the hook is active.
    UnityDragAndDropHook hook;
    void OnEnable()
    {
        // must be created on the main thread to get the right thread id.
        hook = new UnityDragAndDropHook();
        hook.InstallHook();
        hook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {
        hook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.
        Debug.LogError("Dropped " + aFiles.Count + " files at: " + aPos + "\n" +
            aFiles.Aggregate((a, b) => a + "\n" + b));

        OpenDNAFile(aFiles.Aggregate((a, b) => a + "\n" + b));
    }
    public void ToggleInstructionPanel()
    {
        InstructionCanvas.SetActive(!instructions);
        instructions = !instructions;
    }
    public string GetCuteTxtFilename()
    {
        string name = "";
        name += FirstNames[Random.Range(0, FirstNames.Length)];
        name += LastNames[Random.Range(0, LastNames.Length)];
        return name;
    }
    public void OpenDNAFile(string filepath)
    {
        ClearBot();
        gd.editorDNA = new DNA(File.ReadAllText(filepath));
        bect.ConstructBotFromEditorDNA();
    }
    public void WriteEditorDNAtoFile()
    {
        string path = Application.dataPath + "\\DNA\\Editor";
        string fileName = GetCuteTxtFilename();
        string data = gd.editorDNA.toDataString();

        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            while(File.Exists(path + "\\" + fileName))
                fileName = GetCuteTxtFilename() + ".txt";

            File.WriteAllText(path + "\\" + fileName, data);
        }
        catch (System.Exception ex)
        {
            string ErrorMessages = "File Write Error\n" + ex.Message;
            Debug.LogError(ErrorMessages);
        }
        Debug.LogError("Done");
    }
    public void ConsoleDNAString()
    {
        Debug.Log(gd.editorDNA.toDataString());
    }
    public void ClearBot()
    {
        bect.DeleteAllNodesAndJoints();
    }
    public void ContinueToSimWrapper()
    {
        SaveBot();
        SceneManager.LoadScene(1);
    }
    public void CreateBotFromInputField()
    {
        ClearBot();
        gd.editorDNA = new DNA(dnaInputText.text);
        bect.ConstructBotFromEditorDNA();
    }
    public void ExitProgram()
    {
        Application.Quit();
    }
    public void SaveBot()
    {
        bect.SaveBot();
    }
    private void SaveToFileWrapper()
    {
        SaveBot();
        WriteEditorDNAtoFile();
    }
    // Update is called once per frame  
    private void ConfigureMainPanel()
    {
        Dictionary<string, UnityAction> mainPanelButtonActions = new Dictionary<string, UnityAction>
        {            
            { "Save Bot",           () => SaveToFileWrapper()           },
      //      { "BuildFromText",      () => CreateBotFromInputField()     },
      //      { "Print",              () => ConsoleDNAString()            },
            { "Reset",              () => ClearBot()                    },
            { "Instructions",       () => ToggleInstructionPanel()      },
      //      { "Save Bot",           () => SaveBot()                     },
            { "Next",               () => ContinueToSimWrapper()        },
            { "Exit",               () => ExitProgram()                 }
        };

        // Create a button and parent it to Main Panel for each buttonAction
        foreach (var item in mainPanelButtonActions)
        {
            GameObject button = Instantiate(gd.buttonPrefab, mainPanel.transform);
            button.transform.GetChild(0).GetComponent<Text>().text = item.Key;
            button.GetComponent<Button>().onClick.AddListener(item.Value);
        }

        // Get main panel buttons
        Transform mainPanelTransform = mainPanel.GetComponent<Transform>();
        int children = mainPanelTransform.childCount;

        mainPanelTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(gd.widthOfButton + 2 * gd.sizeOfButtonMargin,
            2 * gd.sizeOfButtonMargin + children * (gd.heightOfButton+gd.verticalSpacing)-gd.verticalSpacing);
        mainPanelTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2((gd.widthOfButton + 2 * gd.sizeOfButtonMargin),
            (2 * gd.sizeOfButtonMargin + children * (gd.heightOfButton + gd.verticalSpacing)));
        

        for (int i = 0; i < children; ++i)
        {
            RectTransform rt = mainPanelTransform.GetChild(i).GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(.5f, 1f);
            rt.anchorMax = new Vector2(.5f, 1f);            
            rt.anchoredPosition = new Vector2(0,-1*i*(gd.heightOfButton+gd.verticalSpacing) - gd.sizeOfButtonMargin-.5f*gd.heightOfButton);

        }
    }
}
