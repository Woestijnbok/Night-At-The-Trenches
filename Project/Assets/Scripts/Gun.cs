using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Gun : MonoBehaviour
{
    #region Variables
    // Member variables open to the editor ([SerializeField])
    [SerializeField] private Bullet m_BulletTemplate;
    [SerializeField] private List<Bullet> m_Bullets = new List<Bullet>();

    // Class & struct member variables
    private AudioSource m_AudioSource = null;
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        if (!TryGetComponent<AudioSource>(out m_AudioSource)) Debug.LogWarning("Gun couldn't find audio source component");
    }

    public void Reset()
    {
        // Destroy each non null bullet object in the list, and empty the list

        foreach (Bullet bullet in m_Bullets)
        {
            if (bullet != null)
            {
                Destroy(bullet.gameObject);
            }
        }

        m_Bullets.Clear();
    }
    #endregion

    #region NewFunctinons
    public void Shoot(Vector3 direction)
    {
        // Add a bullet to the list and set its direction also play the audio source
        m_Bullets.Add(Instantiate<Bullet>(m_BulletTemplate, transform.position, transform.rotation));
        m_Bullets.Last().Direction = direction;
        m_AudioSource.Play();
    }
    #endregion
}
