using UnityEngine;
using UnityEngine.UI;
using SwissArmyKnife;

public class SceneController : Singleton<SceneController>
{
    public delegate void SceneAction();
    public static event SceneAction OnGameStart;
    public static event SceneAction OnGameEnd;


	private SceneState _state = SceneState.intro;

    public Animator coinAnimator;
    public Text coinCounter;

    public int timeLimit = 60;			// Le temps pour finir le niveau (en secondes)
	public int totalObjects = 0;		// Le nombre d'objets total dans la scène, calculé au Start
	private int _foundObjects = 0;		// Le nombre d'objets trouvés (aka "cliqués")

    public AudioClip clipOnStart;
    public AudioClip clipOnGame;

	public int foundObjects
	{
		get 
		{
			return _foundObjects;
		}
		
		set 
		{
			_foundObjects = value;

            PlayCoinAnim();
            UpdateCounter();
            	
			// Quand tous les objets sont trouvés, la partie est finie
			if(_foundObjects == totalObjects)
			{
				state = SceneState.ended;
			}
		}
	}

    static public SceneState state
    {
        get
        {
            return (Instance._state);
        }
        set
        {
            Instance._state = value;
            if (value == SceneState.game)
            {
                Instance.Game();
            }
            else if (value == SceneState.ended)
            {
                Instance.End();
            }
        }
    }
	
	void Start()
    {
		totalObjects = Object.FindObjectsOfType<ClickTarget>().Length;
        UpdateCounter();
        AudioManager.Instance.Play(clipOnStart);
	}

    void Game()
    {
        Debug.Log("Game Start !");

        if (OnGameStart != null)
        {
            OnGameStart();
        }
        AudioManager.Instance.Play(clipOnGame);
    }

    void End()
    {
        Debug.Log("Game End !");

        if (OnGameEnd != null)
        {
            OnGameEnd();
        }
        PopupWin.Instance.Enable(Instance.totalObjects, Instance._foundObjects);
    }

    public enum SceneState
    {
        intro,
        game,
        ended
    }

    void PlayCoinAnim()
    {
        if (coinAnimator)
        {
            coinAnimator.SetTrigger("Start");
        }
    }

    void UpdateCounter()
    {
        if (coinCounter)
        {
            coinCounter.text = foundObjects + "/" + totalObjects;
        }
    }
}