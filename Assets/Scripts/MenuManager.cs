using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class MenuManager : MonoBehaviour
{
    public GameObject[] displayCanvas;
    private int activeCanvasIndex = 0;
    private int numberLevels;
    private GameDataManager gameDataManager;
    private LevelTransition levelTransitionInstance;
    public bool isPaused = false;
    private bool isNextLevelEnabled = true;
    public AudioSource audioSource;
    void Start(){
        gameDataManager = GameDataManager.Instance;
        levelTransitionInstance = LevelTransition.Instance;
        SetActiveCanvas();
        numberLevels = 2;
        audioSource = GetComponent<AudioSource>();
    }
    void SetActiveCanvas(){
        for (int i = 0; i < displayCanvas.Length; i++)
            displayCanvas[i].SetActive(i == activeCanvasIndex);
    }

    // Buttons For Title Screen/Changing Canvas
    public void ActivateNextCanvas(){
        audioSource.Play();
        activeCanvasIndex++;
        SetActiveCanvas();
    }
    public void QuitGame(){
        audioSource.Play();
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }

    // Buttons For Character Selection
    public void CharacterSelection(int charID){
        audioSource.Play();
        gameDataManager.SetCharacter((Character)(charID-1));
        ActivateNextCanvas();
    }
    
    // Buttons For Game Mode Selection
    public void LevelModeSelection(string levelModeSelected){
        audioSource.Play();
        if (levelModeSelected == "Levels"){
            gameDataManager.SetGameMode(GameMode.Levels);
            ActivateNextCanvas();
        }else{
            gameDataManager.SetGameMode(GameMode.Infinite);
            int SceneNumber = Random.Range(0,numberLevels)+1;

            levelTransitionInstance.levelToLoad = SceneNumber;
            levelTransitionInstance.FadeOut();
        }
    }

    // Buttons for LevelSelection
    public void LevelSelection(int levelNumber){
        audioSource.Play();
        levelNumber--;
        gameDataManager.SetLevel((Level)levelNumber);
        int SceneNumber = levelNumber%numberLevels+1;

        levelTransitionInstance.levelToLoad = SceneNumber;
        levelTransitionInstance.FadeOut();
    }

    public void Retry(){
        audioSource.Play();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        levelTransitionInstance.levelToLoad = currentSceneIndex;
        levelTransitionInstance.FadeOut();
    }
    public void GoToMenu(){
        audioSource.Play();
        Time.timeScale = 1;

        levelTransitionInstance.levelToLoad = 0;
        levelTransitionInstance.FadeOut();

    }
    public void NextLevel()
    {
        if (isNextLevelEnabled)
        {
            audioSource.Play();
            GameDataManager gameDataManager = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();
            Level level = gameDataManager.GetLevel();
            int levelInt = (int)level;

            levelInt = Mathf.Clamp(levelInt + 1, 0, 9);
            level = (Level)levelInt;

            gameDataManager.SetLevel(level);
            int sceneToLoad = levelInt % numberLevels + 1;

            levelTransitionInstance.levelToLoad = sceneToLoad;
            levelTransitionInstance.FadeOut();
            StartCoroutine(DisableNextLevelForSeconds());
        }
    }
    private IEnumerator DisableNextLevelForSeconds()
    {
        isNextLevelEnabled = false;
        yield return new WaitForSeconds(5.0f);
        isNextLevelEnabled = true;
    }
    public void PauseGame(){
        if (!isPaused)
        {
            transform.Find("GameWinPanel").gameObject.SetActive(false);
            transform.Find("GameOverPanel").gameObject.SetActive(false);
            transform.Find("GamePausePanel").gameObject.SetActive(true);
            Time.timeScale = 0;
        }else{
            Time.timeScale = 1;
            transform.Find("GameWinPanel").gameObject.SetActive(false);
            transform.Find("GameOverPanel").gameObject.SetActive(false);
            transform.Find("GamePausePanel").gameObject.SetActive(false);
        }
        isPaused = !isPaused;
    }
}
