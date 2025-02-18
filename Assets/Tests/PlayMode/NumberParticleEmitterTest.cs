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
        Debug.Log("Pocz¹tek testu NumberParticleEmitterTests: " + TestEnvironment.IsPlayModeTest);

        // Tworzenie i skonfigurowanie obiektu i cz¹steczek testowych
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
        // Usuwamy wszystkie obiekty po ka¿dym teœcie
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

        // Sprawdzenie, czy cz¹steczka zosta³a stworzona
        GameObject particleInstance = GameObject.Find("TestParticlePrefab(Clone)");
        Assert.IsNotNull(particleInstance, "Cz¹steczka nie zosta³a stworzona.");

        // Sprawdzenie, czy TextMeshPro ma poprawny tekst
        TextMeshPro tmp = particleInstance.GetComponent<TextMeshPro>();
        Assert.IsNotNull(tmp, "TextMeshPro nie znaleziono na cz¹steczce.");
        Assert.AreEqual("42", tmp.text, "Tekst w TextMeshPro jest niepoprawny.");

        // Poczekanie czasu ¿ycia cz¹steczki, aby upewniæ siê, ¿e zosta³a usuniêta
        yield return new WaitForSeconds(2f);
        Assert.IsNull(GameObject.Find("TestParticlePrefab(Clone)"), "Cz¹steczka nie zosta³a zniszczona.");
    }

    [UnityTest]
    public IEnumerator EmitNumberParticle_LogsWarning_WhenColorNotFound()
    {
        // Emitowanie cz¹steczki z kolorem, którego nie ma
        emitter.EmitNumberParticle("42", "blue");

        // Sprawdzenie, czy wyœwietlono ostrze¿enie
        LogAssert.Expect(LogType.Warning, "Brak cz¹steczki dla koloru: blue");

        yield return null;
    }
}
