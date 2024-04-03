using UnityEngine;

public class HUD : MonoBehaviour
{
    #region NewDataTypes
    public enum UIOptions
    {
        InGame,
        EndWave,
        EndScreen
    }
    #endregion

    #region Variables
    // Class & struct member variables
    private Field m_Field = null;
    private Player m_Player = null;
    private InGame m_InGame = null;
    private EndWave m_EndWave = null;
    private EndScreen m_EndScreen = null;
    #endregion

    #region Properties
    public Field CurrentField
    {
        get { return m_Field; }
    }

    public Player CurrentPlayer
    {
        get { return m_Player; }
    }

    public EndWave EndWaveUI
    {
        get { return m_EndWave; }
    }

    public InGame InGameUI
    { 
        get { return m_InGame; } 
    }

    public EndScreen EndScreenUI
    {
        get { return m_EndScreen; }
    }
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        m_InGame = gameObject.GetComponentInChildren<InGame>();
        if (m_InGame == null ) Debug.LogWarning("Child with InGame component not found");

        m_EndWave = gameObject.GetComponentInChildren<EndWave>();
        if (m_EndWave == null) Debug.LogWarning("Child with EndWave component not found");

        m_EndScreen = gameObject.GetComponentInChildren<EndScreen>();
        if (m_EndScreen == null) Debug.LogWarning("Child with EndScreen component not found");

        m_Field = FindFirstObjectByType<Field>();
        if (m_Field == null) Debug.LogWarning("Object with Field component not found in scene");

        m_Player = FindFirstObjectByType<Player>();
        if (m_Player == null) Debug.LogWarning("Object with player component not found in scene");
    }

    private void Start()
    {
        // Hide all ui's expect the in game one
        ToggleUI(UIOptions.InGame);
    }
    #endregion

    #region NewFunctions
    public void ToggleUI(UIOptions uiOption)
    {
        // Hide all ui's expect the given option
        switch (uiOption)
        {
            case UIOptions.InGame:
                m_EndWave.Toggle(false);
                m_InGame.Toggle(true);
                m_EndScreen.Toggle(false);
                break;
            case UIOptions.EndWave:
                m_EndWave.Toggle(true);
                m_InGame.Toggle(false);
                m_EndScreen.Toggle(false);
                break;
            case UIOptions.EndScreen:
                m_EndWave.Toggle(false);
                m_InGame.Toggle(false);
                m_EndScreen.Toggle(true);
                break;
        }
    }
    #endregion
}
