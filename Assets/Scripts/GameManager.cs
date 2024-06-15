using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameDataManager gameDataManager;
    GameMode gameMode;
    Character character;
    Level level;
    public bool gameOver = false;
    public GameObject[] characterPrefabs;
    public GameObject gameEndCanvas;
    public GameObject WaveCanvas;
    public GameObject waveCanvasInstance;
    public Vector3 playerStartingPosition = new(0,0,-85f);

    void Start()
    {
        gameDataManager = GameDataManager.Instance;
        gameMode = gameDataManager.GetGameMode();
        level = gameDataManager.GetLevel();
        character = gameDataManager.GetCharacter();

        if (gameMode == GameMode.Infinite)
        {
            waveCanvasInstance = Instantiate(WaveCanvas);
            waveCanvasInstance.name = WaveCanvas.name;
        }
        InstantiateCharacter();
        SetMenuBar();
    }
    void InstantiateCharacter(){
        if (characterPrefabs.Length > (int)character && !GameObject.Find("Player")){  // Remove the find statement Later
            if (Physics.Raycast(playerStartingPosition+Vector3.up*10, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain"))) {
                playerStartingPosition = hit.point;
            }
            GameObject player = Instantiate(characterPrefabs[(int)character], playerStartingPosition, characterPrefabs[(int)character].transform.rotation);
            player.name = "Player";
        }
    }
    public void SetWaveCanvas(int waveNo){
        waveCanvasInstance.SetActive(true);
        TextMeshProUGUI textMeshProUGUI = waveCanvasInstance.transform.GetComponentInChildren<TextMeshProUGUI>();
        textMeshProUGUI.text = "Wave : " + waveNo;
    }
    void SetMenuBar(){
        gameEndCanvas = Instantiate(gameEndCanvas);
        gameEndCanvas.name = "GameEndCanvas";
        MenuManager menuManager = gameEndCanvas.GetComponent<MenuManager>();

        Button menuButton1 = gameEndCanvas.transform.Find("GameOverPanel/Menu").GetComponent<Button>();
        Button retryButton = gameEndCanvas.transform.Find("GameOverPanel/Retry").GetComponent<Button>();
        Button menuButton2 = gameEndCanvas.transform.Find("GameWinPanel/Menu").GetComponent<Button>();
        Button nextLevelButton = gameEndCanvas.transform.Find("GameWinPanel/NextLevel").GetComponent<Button>();
        Button menuButton3 = gameEndCanvas.transform.Find("GamePausePanel/Menu").GetComponent<Button>();

        menuButton1.onClick.AddListener(menuManager.GoToMenu);
        retryButton.onClick.AddListener(menuManager.Retry);
        menuButton2.onClick.AddListener(menuManager.GoToMenu);
        nextLevelButton.onClick.AddListener(menuManager.NextLevel);
        menuButton3.onClick.AddListener(menuManager.GoToMenu);
        
        gameEndCanvas.transform.Find("GameWinPanel").gameObject.SetActive(false);
        gameEndCanvas.transform.Find("GameOverPanel").gameObject.SetActive(false);
        gameEndCanvas.transform.Find("GamePausePanel").gameObject.SetActive(false);
    }
    public void EnemiesDead(){
        gameOver = true;
        gameEndCanvas.transform.Find("GameWinPanel").gameObject.SetActive(true);
        gameEndCanvas.transform.Find("GameOverPanel").gameObject.SetActive(false);
        gameEndCanvas.transform.Find("GamePausePanel").gameObject.SetActive(false);
    }
    public void PlayerDead(){
        gameOver = true;
        gameEndCanvas.transform.Find("GameWinPanel").gameObject.SetActive(false);
        gameEndCanvas.transform.Find("GameOverPanel").gameObject.SetActive(true);
        gameEndCanvas.transform.Find("GamePausePanel").gameObject.SetActive(false);
    }
    
}
