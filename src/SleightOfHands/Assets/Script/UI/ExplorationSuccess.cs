public class ExplorationSuccess : UIWindow
{
    public void RestartLevel()
    {
        LevelManager.Instance.Restart();
        Close();
    }

    public void QuitGame()
    {
        GameManager.Singleton.QuitGame();
    }
}
