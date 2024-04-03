using System.Collections.Generic;
using UnityEngine;

// Field is the main comunicator between different objects, holds different data
public class Field : MonoBehaviour
{
    #region NewDataTypes
    private enum AudioOption
    {
        WaveCompleted,
        WaveFailed
    }
    #endregion

    #region NewEvents
    public delegate void AlliesChanged(int amount);
    public event AlliesChanged OnAlliesChanged;
    #endregion

    #region Variables
    // Member variables open to the editor ([SerializeField])
    [SerializeField] private int m_NumberOfAlliesAtStart = 10;

    // Class & struct member variables
    private AudioSource m_AllyDeathSound = null;
    private AudioSource m_EnemyDeathSound = null;
    private AudioSource m_RescuedSound = null;
    private AudioSource m_WaveCompletedSound = null;
    private AudioSource m_FailedSound = null;
    private HUD m_HUD = null;
    private readonly List<Spawner> m_Spawners = new List<Spawner>();

    // Primitive member variables
    private int m_NrOfKilledEnemies = 0;
    private int m_NrOfAllies = 0;
    private int m_NrOfKilledAllies = 0;
    private int m_NrOfRescuedAllies = 0;
    private bool m_WaveInProgress = true;
    private int m_WaveNumber = 1;
    #endregion

    #region Properties
    public bool WaveInProgress
    {  
        get { return m_WaveInProgress; }
        set { m_WaveInProgress = value; }
    }

    public virtual int NrOfAllies
    {
        get { return m_NrOfAllies; }
    }
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        m_NrOfAllies = m_NumberOfAlliesAtStart;

        // Find all the spawners add them to cash list and adapt them to the first wave
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            if (gameObject.TryGetComponent<Spawner>(out Spawner spawner))
            {
                m_Spawners.Add(spawner);
                spawner.AdaptToWave(m_WaveNumber);
            }
        }

        // Cash all the audio sources
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length == 5)
        {
            m_AllyDeathSound = audioSources[0];
            m_EnemyDeathSound = audioSources[1];
            m_RescuedSound = audioSources[2];
            m_WaveCompletedSound = audioSources[3];
            m_FailedSound = audioSources[4];
        }
        else Debug.LogWarning("Field coulnd't find the 5 needed AudioSources");
    }

    private void Start()
    {
        // Cash the hud and set the start information
        m_HUD = FindFirstObjectByType<HUD>();
        if (m_HUD != null)
        {
            m_HUD.InGameUI.SetWaveLabel(m_WaveNumber);
            m_HUD.InGameUI.UpdateAllyCounter(m_NumberOfAlliesAtStart);
        }
        else Debug.LogWarning("Couldn't find object with HUD component in the scene");
    }

    private void Update()
    {
        // Check if the game is over, communicate it to the hud ifso
        if (IsGameOver())
        {
            if (m_WaveInProgress)
            {
                PlaySound(m_FailedSound);
                m_HUD.CurrentPlayer.DeleteProjectiles();
                m_WaveInProgress = false;
                m_HUD.EndScreenUI.SetInfo(m_WaveNumber);
                m_HUD.ToggleUI(HUD.UIOptions.EndScreen);

                foreach (Spawner spawner in m_Spawners)
                {
                    if (spawner != null) spawner.EmptyAndStop();
                    else Debug.LogWarning("Field contains a corrupt spawner");
                }
            }
        }
        // Check if the current wave ended, communicate it to the hud ifso
        else if (DidWaveEnd())
        {
            if (m_WaveInProgress)
            {
                PlaySound(m_WaveCompletedSound);
                m_HUD.CurrentPlayer.DeleteProjectiles();
                m_HUD.ToggleUI(HUD.UIOptions.EndWave);
                m_HUD.EndWaveUI.SetWaveBox(m_NrOfKilledEnemies, m_NrOfRescuedAllies, m_NrOfKilledAllies, m_NrOfAllies, m_WaveNumber);
                m_HUD.EndWaveUI.SetResupplyBox(m_HUD.CurrentPlayer.NrOfBullets, m_HUD.CurrentPlayer.NrOfMedicPacks, m_HUD.CurrentPlayer.NrOfSmokeGrenades, m_HUD.CurrentPlayer.NrOfGasGrenades);
                m_WaveInProgress = false;
            }
        }
    }
    #endregion

    #region NewFunctions
    public void KilledEnemy()
    {
        ++m_NrOfKilledEnemies;
        OnAlliesChanged?.Invoke(m_NrOfAllies);
        m_EnemyDeathSound.Play();
    }

    public void KilledAlly()
    {
        ++m_NrOfKilledAllies;
        OnAlliesChanged?.Invoke(m_NrOfAllies);
        m_AllyDeathSound.Play();
    }

    public void EnemyCrossed()
    {
        ++m_NrOfKilledAllies;
        --m_NrOfAllies;
        OnAlliesChanged?.Invoke(m_NrOfAllies);
        m_AllyDeathSound.Play();
    }

    public void RescuedAlly()
    {
        ++m_NrOfRescuedAllies;
        ++m_NrOfAllies;
        OnAlliesChanged?.Invoke(m_NrOfAllies);
        m_RescuedSound.Play();
    }

    private bool DidWaveEnd()
    {
        // Return true if all spawners are empty (and no enemy is on field)
        foreach(Spawner spawner in m_Spawners)
        {
            if(spawner != null) 
            {
                if (!spawner.IsEmpty()) return false;
            }
            else Debug.LogWarning("Field contains a corrupt spawner");
        }

        return true;
    }

    public void NewWave()
    {
        // Reset some wave information
        m_NrOfKilledEnemies = 0;
        m_NrOfKilledAllies = 0;
        m_NrOfRescuedAllies = 0;
        ++m_WaveNumber;

        // Reset every spawner and make them adapt to the new wave
        foreach (Spawner spawner in m_Spawners)
        {
            if (spawner != null) 
            {
                spawner.Reset();
                spawner.AdaptToWave(m_WaveNumber);
            }
            else Debug.LogWarning("Field contains a corrupt spawner");
        }

        // Communicate the new wave with the hud
        m_HUD.InGameUI.SetWaveLabel(m_WaveNumber);
        m_HUD.ToggleUI(HUD.UIOptions.InGame);

        m_WaveInProgress = true;
    }

    private bool IsGameOver()
    {
        // Returns true if there are no allies left in the DITCH
        if (m_NrOfAllies <= 0) return true;
        else return false;
    }

    public void Restart()
    {
        // Reset field information
        m_NrOfKilledEnemies = 0;
        m_NrOfKilledAllies = 0;
        m_NrOfRescuedAllies = 0;
        m_NrOfAllies = m_NumberOfAlliesAtStart;
        m_WaveNumber = 1;

        // Reset all the spawners
        foreach (Spawner spawner in m_Spawners)
        {
            if (spawner != null) spawner.Restart();
            else Debug.LogWarning("Field contains a corrupt spawner");
        }

        // Communicate the new reseted information with the hud
        m_HUD.InGameUI.UpdateAllyCounter(NrOfAllies);
        m_HUD.InGameUI.SetWaveLabel(m_WaveNumber);

        m_WaveInProgress = true;
    }

    private void PlaySound(AudioSource audioSource)
    {
        // Functions to stop character audio and plays the given audio source
        m_AllyDeathSound.Stop();
        m_EnemyDeathSound.Stop();
        m_RescuedSound.Stop();
        audioSource.Play();
    }
    #endregion
}
