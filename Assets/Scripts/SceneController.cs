using UnityEngine;
using SwissArmyKnife;

public class SceneController : Singleton<SceneController>
{
    public delegate void SceneAction();
    public static event SceneAction OnGameStart;
    public static event SceneAction OnGameEnd;

    private SceneState _state = SceneState.intro;

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
                if (OnGameStart != null)
                {
                    OnGameStart();
                }
            }
            else if (value == SceneState.ended)
            {
                if (OnGameEnd != null)
                {
                    OnGameEnd();
                }
            }
        }
    }

    public enum SceneState
    {
        intro,
        game,
        ended
    }
}