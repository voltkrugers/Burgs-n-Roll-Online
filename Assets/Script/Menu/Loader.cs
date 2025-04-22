using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{

    public enum Scene
    {
        MainMenu,
        Game,
        LoadingScene,
        LobbyScene,
        CharacterSelectedScene
    }
    private static Scene TargetScene;

    public static void Load(Scene targetScene)
    {
        Loader.TargetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallBack()
    {
        SceneManager.LoadScene(TargetScene.ToString());
    }
}
