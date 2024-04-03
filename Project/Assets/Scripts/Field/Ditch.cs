using UnityEngine;

// Ditch class handles the situation when a character crosses the field
public class Ditch : MonoBehaviour
{
    #region Variables
    // Class member variables
    private Field m_Field = null;
    #endregion

    #region UnityFunctions
    void Awake()
    {
        m_Field = GetComponentInParent<Field>();
        if (m_Field == null) Debug.LogWarning("Ditch couldn't find Field component in parent");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Deletes a character if he passed the field, also changes some corresponding information in the field class
        if (other.TryGetComponent<Character>(out Character _))
        {
            if (other.TryGetComponent<Enemy>(out Enemy _)) m_Field.EnemyCrossed();
            else m_Field.RescuedAlly();

            Destroy(other.gameObject);
        }
    }
    #endregion
}
