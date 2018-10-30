public class IngameMenu : UserInterface
{
    public void QuitGame()
    {
        GameManager.Singleton.Quit();
    }
}
