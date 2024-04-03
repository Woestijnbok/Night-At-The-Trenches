using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Variables
    // Member variables open to the editor ([SerializeField])
    [SerializeField] private Ally[] m_AllyTemplates = null;
    [SerializeField] private Enemy[] m_EnemyTemplates = null;
    [SerializeField] private float m_StartSpawnTime = 3.0f;
    [SerializeField] private int m_StartSpawnAmount = 1;
    [SerializeField] private int m_StartEnemyChance = 70;
    [SerializeField] private float m_StartHealthySpeed = 3.0f;
    [SerializeField] private float m_StartInjuredSpeed = 0.7f;
    [SerializeField] private float m_StartNormalSpeed = 1.0f;

    // Class & struct member variables
    private List<Character> m_Spawns = new List<Character>();
    private Field m_Field = null;

    // Primitive member variables
    private float m_HealthySpeed = 3.0f;
    private float m_InjuredSpeed = 0.1f;
    private float m_NormalSpeed = 1.0f;
    private float m_SpawnTime = 3.0f;
    private int m_SpawnAmount = 1;
    private int m_EnemyChance = 50;
    private float m_Timer = 0.0f;
    private int m_SpawnCount = 0;
    #endregion

    #region UnityFunctions
    public void Start()
    {
        // Set variables
        m_HealthySpeed = m_StartHealthySpeed;
        m_InjuredSpeed = m_StartInjuredSpeed;
        m_NormalSpeed = m_StartNormalSpeed;

        // Cash variables
        m_Field = FindFirstObjectByType<Field>();
        if (m_Field == null) Debug.LogWarning("Couldn't find object with Field component in scene");
    }

    private void FixedUpdate()
    {
        // Spawn a character if we can haven't spawn enough yet and the timer allows

        if (m_SpawnCount < m_SpawnAmount)
        {
            m_Timer += Time.deltaTime;

            if (m_Timer > m_SpawnTime)
            {
                m_Timer -= m_SpawnTime;
                SpawnCharacter();
            }
        }
    }

    public void Reset()
    {
        // Delete any character left in the list and empty the list, set timer back and spawncount to 0

        Empty();
        m_Spawns.Clear();
        m_Timer = 0.0f;
        m_SpawnCount = 0;
    }
    #endregion

    #region NewFunctions
    public void AdaptToWave(int waveNumber)
    {
        // Some logic o make the waves harder

        m_SpawnAmount = waveNumber;
        m_SpawnTime = 3.1f - (0.1f * waveNumber);
        m_EnemyChance = 49 + waveNumber;
        m_HealthySpeed = m_StartHealthySpeed + (0.2f * waveNumber);
        m_InjuredSpeed = m_StartInjuredSpeed + (0.1f * waveNumber);
        m_NormalSpeed = m_StartNormalSpeed + (0.1f * waveNumber);
    }

    private void SpawnCharacter()
    {
        // spawn random ally or enemy based on enemy chance

        Character character = null;

        if (Random.Range(0.0f, 100.0f) < m_EnemyChance)
        {
            int randomEnemyIndex = Random.Range(0, m_EnemyTemplates.Length);
            character = Instantiate(m_EnemyTemplates[randomEnemyIndex], transform);
            m_Spawns.Add(character);
        }
        else
        {
            int randomAllyIndex = Random.Range(0, m_AllyTemplates.Length);
            character = Instantiate(m_AllyTemplates[randomAllyIndex], transform);
            m_Spawns.Add(character);
        }

        character.HealthySpeed = m_HealthySpeed;
        character.InjuredSpeed = m_InjuredSpeed;
        character.NormalSpeed = m_NormalSpeed;

        ++m_SpawnCount;
    }

    public bool IsEmpty()
    {
        // A Spawner is not empty when he didn't spawned everything and the are enemies left

        bool isEmpty = true;
        bool FoundAlly = false;

        if (m_SpawnCount >= m_SpawnAmount)
        {
            foreach (Character character in m_Spawns)
            {
                if (character != null)
                {
                    if (character.gameObject.TryGetComponent<Enemy>(out Enemy _))
                    {
                        isEmpty = false;
                        break;
                    }
                    else FoundAlly = true;
                }
            }
        }
        else isEmpty = false;

        // There are only allies left and he is done spawning
        if(isEmpty && FoundAlly) RescueAllLeftCharacters();

        return isEmpty;
    }

    private void Empty()
    {
        // Destroy all characters left on the field made by this spawner

        foreach (Character character in m_Spawns)
        {
            if (character != null) Destroy(character.gameObject);
        }
    }

    public void Restart()
    {
        // Clear all spawns and set variables back to start values

        Empty();
        m_Spawns.Clear();
        m_Timer = 0.0f;
        m_SpawnCount = 0;
        m_SpawnTime = m_StartSpawnTime;
        m_EnemyChance = m_StartEnemyChance;
        m_SpawnAmount = m_StartSpawnAmount;
    }

    public void EmptyAndStop()
    {
        Empty();
        m_SpawnCount = m_SpawnAmount;
    }

    private void RescueAllLeftCharacters()
    {
        foreach (Character character in m_Spawns)
        {
            if(character != null)
            {
                Destroy(character.gameObject);
                m_Field.RescuedAlly();
            }
        }
    }
    #endregion
}
