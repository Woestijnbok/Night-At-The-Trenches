using UnityEngine;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour
{
    #region Variables
    // Class & struct member variables
    private HUD m_HUD = null;
    private UIDocument m_AttachedDocument = null;
    private VisualElement m_Root = null;
    private GroupBox m_EndBox = null;
    private Label m_EndLabel = null;
    private Button m_RestartButton = null;
    #endregion

    #region UnityFunctions
    // Start is called before the first frame update
    void Awake()
    {
        // Cash our variables
        if (TryGetComponent<UIDocument>(out m_AttachedDocument))
        {
            m_Root = m_AttachedDocument.rootVisualElement;

            if (m_Root != null)
            {
                m_EndBox = m_Root.Q<GroupBox>("EndBox");

                if(m_EndBox != null)
                {
                    m_EndLabel = m_EndBox.Q<Label>("WaveLabel");
                    if (m_EndLabel == null) Debug.LogWarning("End screen ui couldn't find 'WaveLabel'(Label) from 'EndBox'(GroupBox)");

                    m_RestartButton = m_EndBox.Q<Button>("RestartButton");
                    if(m_RestartButton != null) m_RestartButton.RegisterCallback<ClickEvent>(RestartButtonClicked);
                    else Debug.LogWarning("End screen ui couldn't find 'RestartButton'(Button) from 'EndBox'(GroupBox)");
                }
                else Debug.LogWarning("End screen ui couldn't find 'EndBox'(GroupBox) from 'root'(VisualElement)");
            }
            else Debug.LogWarning("End screen ui couldn't extract VisualElement from UIDocument");
        }
        else Debug.LogWarning("End screen ui couldn't extract ui document component");

        m_HUD = gameObject.GetComponentInParent<HUD>();
        if (m_HUD == null) Debug.LogWarning("EndScreen ui couldn't find HUD component in parent");
    }
    #endregion

    #region NewFunctions
    public void SetInfo(int waveNumber)
    {
        // Adapts the title to the wave the player failed at
        m_EndLabel.text = "All allies died at wave " + waveNumber.ToString();
    }

    public void RestartButtonClicked(ClickEvent clickEvent)
    {
        // Restarts the objects that need restarting and makes the hud change back to the in game ui
        m_HUD.CurrentPlayer.Restart();
        m_HUD.CurrentField.Restart();
        m_HUD.ToggleUI(HUD.UIOptions.InGame);
    }

    public void Toggle(in bool show)
    {
        m_Root.visible = show;
    }
    #endregion
}
