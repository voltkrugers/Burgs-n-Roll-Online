using NUnit.Framework;

public class KitchenGameLobbyTests
{
    [Test]
    public void Test_IsLobbyHost_WhenNullLobby_ReturnsFalse()
    {
        var go = new UnityEngine.GameObject();
        var lobby = go.AddComponent<KitchenGameLobby>();

        // On n’a pas encore rejoint de lobby
        var method = typeof(KitchenGameLobby)
            .GetMethod("IsLobbyHost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        bool result = (bool)method.Invoke(lobby, null);

        Assert.IsFalse(result); // Sans lobby, on ne peut pas être host
    }
}