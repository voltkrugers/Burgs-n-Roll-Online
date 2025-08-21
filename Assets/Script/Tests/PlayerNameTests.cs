using NUnit.Framework;
using UnityEngine;

public class PlayerNameTests
{
    [Test]
    public void Test_SetAndGetPlayerName()
    {
        // Arrange
        var go = new GameObject();
        var multiplayer = go.AddComponent<KitchenGameMultiplayer>();

        // Act
        multiplayer.SetPlayerName("RockChef");
        string name = multiplayer.GetPlayerName();

        // Assert
        Assert.AreEqual("RockChef", name);
    }
}
