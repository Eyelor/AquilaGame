using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

public class NumberParticleEmitterTest
{
    private GameObject emitterGameObject;
    private NumberParticleEmitter emitter;
    private Particle particle;

    [SetUp]
    public void SetUp()
    {
        TestEnvironment.IsPlayModeTest = true;
        Debug.Log("Pocz�tek testu NumberParticleEmitterTests: " + TestEnvironment.IsPlayModeTest);

        // Tworzenie i skonfigurowanie obiektu i cz�steczek testowych
        emitterGameObject = new GameObject("TestEmitter");
        emitter = emitterGameObject.AddComponent<NumberParticleEmitter>();
        particle = new Particle
        {
            particlePrefab = new GameObject("TestParticlePrefab"), // Testowy prefab
            particleLifetime = 2f,
            particleSpeed = 1f,
            color = "red"
        };
        particle.particlePrefab.AddComponent<TextMeshPro>();
        emitter.particles = new Particle[] { particle };
    }

    [TearDown]
    public void TearDown()
    {
        // Usuwamy wszystkie obiekty po ka�dym te�cie
        Object.DestroyImmediate(particle.particlePrefab);
        Object.DestroyImmediate(emitterGameObject);
        TestEnvironment.IsPlayModeTest = false;
        Debug.Log("Koniec testu NumberParticleEmitterTests: " + TestEnvironment.IsPlayModeTest);
    }

    [UnityTest]
    public IEnumerator EmitNumberParticle_CreatesParticle_WithCorrectText()
    {
        emitter.EmitNumberParticle("42", "red");
        yield return null;

        // Sprawdzenie, czy cz�steczka zosta�a stworzona
        GameObject particleInstance = GameObject.Find("TestParticlePrefab(Clone)");
        Assert.IsNotNull(particleInstance, "Cz�steczka nie zosta�a stworzona.");

        // Sprawdzenie, czy TextMeshPro ma poprawny tekst
        TextMeshPro tmp = particleInstance.GetComponent<TextMeshPro>();
        Assert.IsNotNull(tmp, "TextMeshPro nie znaleziono na cz�steczce.");
        Assert.AreEqual("42", tmp.text, "Tekst w TextMeshPro jest niepoprawny.");

        // Poczekanie czasu �ycia cz�steczki, aby upewni� si�, �e zosta�a usuni�ta
        yield return new WaitForSeconds(2f);
        Assert.IsNull(GameObject.Find("TestParticlePrefab(Clone)"), "Cz�steczka nie zosta�a zniszczona.");
    }

    [UnityTest]
    public IEnumerator EmitNumberParticle_LogsWarning_WhenColorNotFound()
    {
        // Emitowanie cz�steczki z kolorem, kt�rego nie ma
        emitter.EmitNumberParticle("42", "blue");

        // Sprawdzenie, czy wy�wietlono ostrze�enie
        LogAssert.Expect(LogType.Warning, "Brak cz�steczki dla koloru: blue");

        yield return null;
    }
}
