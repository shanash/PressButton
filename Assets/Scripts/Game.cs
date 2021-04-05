using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    private enum State
    {
        Ready = 0,
        Play,
        Result,
    }

    private static readonly float kShowReadyTextSeconds = 1.0f;
    private static readonly float kGameTime = 5.0f;

    #region UI Members
    [SerializeField] private GameObject m_panelReady = null;
    [SerializeField] private GameObject m_panelResult = null;
    [SerializeField] private Text m_time = null;
    [SerializeField] private Text m_score = null;
    #endregion

    #region Variable Members
    private State m_state = State.Ready;
    private float m_showTextSeconds = 0.0f;
    private float m_remainSeconds = 0.0f;
    private int m_numClicks = 0;
    #endregion

    #region Monobehaviour Default Callback
    private void Start()
    {
        SetState(State.Ready);
    }

    private void Update()
    {
        if (m_state == State.Ready)
        {
            m_showTextSeconds += Time.deltaTime;

            if (m_showTextSeconds > kShowReadyTextSeconds) // 준비 끝
            {
                SetState(State.Play);
            }
        }
        else if (m_state == State.Play)
        {
            if (m_remainSeconds > 0.0f) // 게임 중
            {
                m_remainSeconds -= Time.deltaTime;
            }
            else // 타임 오버
            {
                m_remainSeconds = 0.0f;
                SetState(State.Result);
            }

            m_time.text = m_remainSeconds.ToString();
            m_score.text = m_numClicks.ToString();
        }
        else if (m_state == State.Result)
        {

        }
    }
    #endregion

    #region Private Method
    private void SetState(State state)
    {
        m_state = state;
        if (m_state == State.Ready)
        {
            m_remainSeconds = kGameTime;
            m_numClicks = 0;
            m_showTextSeconds = 0;
            m_panelResult.SetActive(false);
            m_panelReady.SetActive(true);
        }
        else if (m_state == State.Play)
        {
            m_panelReady.SetActive(false);
        }
        else if (m_state == State.Result)
        {
            m_panelResult.SetActive(true);
        }
    }
    #endregion

    #region Public Method
    public void OnClickButton()
    {
        if (m_state == State.Play)
            m_numClicks++;
        else if (m_state == State.Result)
            SetState(State.Ready);
    }
    #endregion
}
