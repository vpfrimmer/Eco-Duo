using UnityEngine;
using SwissArmyKnife;

public class SceneController : Singleton<SceneController>
{
    public delegate void SceneAction();
    public static event SceneAction OnGameStart;
    public static event SceneAction OnGameEnd;

	private SceneState _state = SceneState.intro;
	
	public int timeLimit = 60;			// Le temps pour finir le niveau (en secondes)
	public int totalObjects = 0;		// Le nombre d'objets total dans la scène, calculé au Start
	private int _foundObjects = 0;		// Le nombre d'objets trouvés (aka "cliqués")
	public int foundObjects
	{
		get 
		{
			return _foundObjects;
		}
		
		set 
		{
			_foundObjects = value;
			
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
            	Debug.Log("Game Start !");
            	
                if (OnGameStart != null)
                {
                    OnGameStart();
                }
            }
            else if (value == SceneState.ended)
            {
	            Debug.Log("Game End !");
            	
                if (OnGameEnd != null)
                {
                    OnGameEnd();
                }
            }
        }
    }
	
	void Start() {
		totalObjects = Object.FindObjectsOfType<ClickTarget>().Length;
	}

    public enum SceneState
    {
        intro,
        game,
        ended
    }
}