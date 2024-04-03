using UnityEngine;

public class MedicPack : Projectile
{
    #region Variables
    // Shared member variables (const or static)
    private const int m_HealValue = 100;
    #endregion

    #region UnityFunctions
    private void OnTriggerEnter(Collider other)
    {
        // If we collide with someting destroy self and heal other if possible
        if (other.TryGetComponent<Health>(out Health otherHealth))
        {
            otherHealth.Heal(m_HealValue);
        }

        Destroy(gameObject);
    }
    #endregion
}
