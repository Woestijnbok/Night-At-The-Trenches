using UnityEngine;

public class Ally : Character
{
    #region UnityFunctions
    protected override void Awake()
    {
        base.Awake();
        m_Animator = GetComponentInChildren<Animator>();
        if (m_Animator == null) Debug.LogWarning("Ally Couldn't find animator component in any child");
    }
    #endregion
}
