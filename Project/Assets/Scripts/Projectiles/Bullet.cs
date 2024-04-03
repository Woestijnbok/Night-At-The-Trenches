using UnityEngine;

public class Bullet : Projectile
{
    #region Variables
    // Shared member variables (const or static)
    private const int m_Damage = 100;
    #endregion

    #region UnityFunctions
    private void OnTriggerEnter(Collider other)
    {
        // Damage it if it has a health component aka is a character
        if (other.TryGetComponent<Health>(out Health otherHealth))
        {
            otherHealth.Damage(m_Damage);
        }

        Destroy(gameObject);
    }
    #endregion
}
