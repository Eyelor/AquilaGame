using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ShipControllerTests
{
    private GameObject ship;
    private ShipController shipController;

    [SetUp]
    public void Setup()
    {
        TestEnvironment.IsPlayModeTest = true;
        Debug.Log("Pocz�tek testu ShipControllerTests: " + TestEnvironment.IsPlayModeTest);

        ship = new GameObject("Ship");
        ship.AddComponent<Rigidbody>().useGravity = false;  // Dodanie Rigidbody
        shipController = ship.AddComponent<ShipController>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(ship);
        TestEnvironment.IsPlayModeTest = false;
        Debug.Log("Koniec testu ShipControllerTests: " + TestEnvironment.IsPlayModeTest);
    }

    [UnityTest]
    public IEnumerator ShipMovement_AcceleratesWhenMovingForward()
    {
        yield return new WaitForSeconds(1f);

        // Symulowanie ruchu do przodu
        shipController.moveForward = true;

        // Symulacja przyspieszania
        yield return new WaitForSeconds(5f);

        Assert.Greater(shipController._currentMoveSpeed, 0f, "Statek powinien przyspiesza�.");
        Assert.LessOrEqual(shipController._currentMoveSpeed, shipController.maxForwardSpeed, "Pr�dko�� nie powinna przekroczy� maksymalnej.");
    }
    
    [UnityTest]
    public IEnumerator ShipMovement_StopsWhenNoInput()
    {
        yield return new WaitForSeconds(1f);

        // Symulowanie ruchu do przodu i czekanie na przyspieszenie
        shipController.moveForward = true;
        yield return new WaitForSeconds(4f);

        // Symulowanie braku ruchu statku i czekanie na zwolnienie
        shipController.moveForward = false;
        yield return new WaitForSeconds(4f); 

        Assert.AreEqual(shipController._currentMoveSpeed, 0f, "Statek powinien zatrzyma� si�.");
    }

    [UnityTest]
    public IEnumerator ShipMovement_RotatesWhenInputGiven()
    {
        yield return new WaitForSeconds(1f);

        // Symolowanie obrotu w prawo
        shipController.rotationInput = 1f; 

        Quaternion initialRotation = ship.transform.rotation;

        yield return new WaitForSeconds(4f);  // Czekamy na obr�t

        Assert.AreNotEqual(initialRotation, ship.transform.rotation, "Statek powinien obr�ci� si�.");
        Assert.LessOrEqual(shipController._currentRotationSpeed, shipController.maxRotationSpeed, "Pr�dko�� obrotu nie powinna przekroczy� maksymalnej.");
    }

    [UnityTest]
    public IEnumerator ShipMovement_StopsRotatingWhenNoRotationInput()
    {
        yield return new WaitForSeconds(1f);

        // Symolowanie obrotu w prawo
        shipController.rotationInput = 1f; 

        yield return new WaitForSeconds(4f);  // Czekamy na obr�t

        // Symolowanie zatrzymania obrotu
        shipController.rotationInput = 0f; 
        yield return new WaitForSeconds(4f); 

        Assert.AreEqual(shipController._currentRotationSpeed, 0f, "Statek powinien przesta� si� obraca�.");
    }
}
