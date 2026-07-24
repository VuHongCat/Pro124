using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        MainMenu,
        Map,
        Battle,
        Reward,
        Shop,
        Pause
    }

    public GameState CurrentState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState state)
    {
        CurrentState = state;
    }
}
