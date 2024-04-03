using UnityEngine;

// Base class used for bullet and different equipment
public class Projectile : MonoBehaviour
{
    #region Variables
    // Shared member variables (const or static)
    protected const float m_MaxLifetime = 4.0f;

    // Member variables open to the editor ([SerializeField])
    [SerializeField] protected float m_Speed = 15.0f;

    // Class & struct member variables
    protected Vector3 m_Direction = Vector3.forward;

    // Primitive member variables
    protected float m_LifeTime = 0.0f;
    #endregion

    #region Properties
    public virtual Vector3 Direction
    {
        get { return m_Direction; }
        set { m_Direction = value; }
    }
    #endregion

    #region UnityFunctions
    protected virtual void FixedUpdate()
    {
        // If lives a certain time delete the object (safety measure)
        m_LifeTime += Time.deltaTime;
        if (m_LifeTime > m_MaxLifetime) Destroy(gameObject);

        // Update the position
        transform.position += m_Speed * Time.deltaTime * m_Direction;
    }
    #endregion
}
