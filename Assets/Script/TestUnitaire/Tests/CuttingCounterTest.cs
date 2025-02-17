// using NUnit.Framework;
// using UnityEngine;
//
// public class CuttingCounterTests
// {
//     private GameObject cuttingCounterObject;
//     private CuttingCounter cuttingCounter;
//     private KitchenObjSO dummyKitchenObjSo;
//
//     [SetUp]
//     public void Setup()
//     {
//         cuttingCounterObject = new GameObject();
//         cuttingCounter = cuttingCounterObject.AddComponent<CuttingCounter>();
//
//         
//         dummyKitchenObjSo = ScriptableObject.CreateInstance<KitchenObjSO>();
//         CuttingRecipeSO cuttingRecipe = ScriptableObject.CreateInstance<CuttingRecipeSO>();
//         cuttingRecipe.input = dummyKitchenObjSo;
//         cuttingRecipe.cuttingProgressMax = 3; // Assume max cut is 3 for this test
//
//         cuttingCounter.cuttingRecipeSoArray = new CuttingRecipeSO[] { cuttingRecipe };
//     }
//
//     [Test]
//     public void TestObjectDestroyedAfterMaxCuts()
//     {
//         var kitchenObj = new GameObject().AddComponent<KitchenObj>();
//         kitchenObj.SetKitchenObjSo(dummyKitchenObjSo);
//         kitchenObj.SetKitchenObjParent(cuttingCounter);
//
//         // Simulate cutting actions
//         for (int i = 0; i < 3; i++)
//         {
//             cuttingCounter.SecondInteract(null);  // Passing null for simplicity in this test context
//         }
//
//         // Verify the object is destroyed
//         Assert.IsNull(cuttingCounter.GetKitchenObj());
//     }
// }