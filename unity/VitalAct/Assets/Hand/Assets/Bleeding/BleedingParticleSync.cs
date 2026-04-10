using UnityEngine;

public class BleedingParticleSync : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private BleedingSystem bleedingSystem;
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private AudioSource bleedLoopAudio;

    [Header("Particle Response")]
    [SerializeField] private float minEmissionRate = 0f;
    [SerializeField] private float maxEmissionRate = 45f;
    [SerializeField] private float minStartSpeed = 0.2f;
    [SerializeField] private float maxStartSpeed = 1.4f;

    [Header("Audio Response")]
    [SerializeField] private float minVolume = 0f;
    [SerializeField] private float maxVolume = 0.85f;

    private void Update()
    {
        if (bleedingSystem == null)
        {
            return;
        }

        float bleed = bleedingSystem.BleedingLevel;

        if (bloodParticles != null)
        {
            var emission = bloodParticles.emission;
            emission.rateOverTime = Mathf.Lerp(minEmissionRate, maxEmissionRate, bleed);

            var main = bloodParticles.main;
            main.startSpeed = Mathf.Lerp(minStartSpeed, maxStartSpeed, bleed);

            bool shouldPlay = bleed > 0.02f && bleedingSystem.CurrentState != BleedingGameState.Prepare;
            if (shouldPlay && !bloodParticles.isPlaying)
            {
                bloodParticles.Play();
            }
            else if (!shouldPlay && bloodParticles.isPlaying)
            {
                bloodParticles.Stop();
            }
        }

        if (bleedLoopAudio != null)
        {
            bleedLoopAudio.volume = Mathf.Lerp(minVolume, maxVolume, bleed);
        }
    }
}
