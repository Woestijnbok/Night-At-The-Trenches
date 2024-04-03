using UnityEngine;

public class Health : MonoBehaviour
{
    #region Variables
    // Shared member variables (const or static)
    static private Field m_Field = null;

    // Primitive member variables
    private Character m_Character = null;

    // Class & struct member variables
    private const int m_MaxHealth = 100;
    private const int m_InjuredTreshold = 60;
    private int m_Health = 0;
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        // Set the start health and cash variables

        m_Health = m_MaxHealth;

        m_Character = GetComponent<Character>();
        if (m_Character == null) Debug.LogWarning("Health couldn't find Character component");

        if(m_Field == null) 
        {
            m_Field = FindFirstObjectByType<Field>();
            if (m_Field == null) Debug.LogWarning("No object in the scene has Field component");
        }

    }
    #endregion

    #region NewFunctions
    public void Damage(int damage)
    {
        m_Health -= damage;

        // If we went below or equal to 0 update field information and destroy the character we are connected to
        if (m_Health <= 0) 
        {
            if (TryGetComponent<Enemy>(out Enemy _)) m_Field.KilledEnemy();
            else m_Field.KilledAlly();

            m_Character.Die();
        }
        // We didn't die yet so check if we hit the injured treshold
        else if(m_Health <= m_InjuredTreshold) m_Character.IsInjured = true;
    }

    public void Heal(int amount)
    {
        // Heal the character if it is injured
        if(m_Character.IsInjured)
        {
            m_Character.PlayHealSound();
            m_Health = Mathf.Min((m_Health + amount), m_MaxHealth);
            if (m_Health > m_InjuredTreshold) m_Character.IsInjured = false;
        }
    }
    #endregion
}
