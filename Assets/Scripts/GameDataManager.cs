using System.Collections;
using UnityEngine;
public enum GameMode{Levels,Infinite};
public enum Character {Character1,Character2,Character3};
public enum Level{Level1,Level2,Level3,Level4,Level5,Level6,Level7,Level8,Level9,Level10};
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    [SerializeField] private GameMode gameMode;
    [SerializeField] private Character character;
    [SerializeField] private Level level;
    public bool canSetLevel = true;
    void Start()
    {
        if(Instance==null){
            Instance = this;
            DontDestroyOnLoad(Instance);
        }else{
            Destroy(gameObject);
        }
    }
    public void SetGameMode(GameMode mode){
        gameMode = mode;
    }
    public void SetCharacter(Character characterSelected){
        character = characterSelected;
    }
    public void SetLevel(Level levelSelected){
        if (!canSetLevel)
            return;
        level = levelSelected;
        StartCoroutine(DisableSetLevelForTime());
    }
    public GameMode GetGameMode(){
        return gameMode;
    }
    public Character GetCharacter(){
        return character;
    }
    public Level GetLevel(){
        return level;
    }
    IEnumerator DisableSetLevelForTime(){
        canSetLevel = false;
        yield return new WaitForSeconds(6.0f);
        canSetLevel = true;
    }
}
