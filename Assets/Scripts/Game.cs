using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BackEnd; 

public class Game : MonoBehaviour
{
    #region Defines
    private enum State
    {
        None = 0,
        Title,
        Ready,
        Play,
        Result,
    }
    #endregion

    #region Constants
    private static readonly float kShowReadyTextSeconds = 1.0f;
    private static readonly float kGameTime = 5.0f;
    #endregion

    #region UI Members
    [SerializeField] private GameObject m_panelTitle = null;
    [SerializeField] private GameObject m_panelReady = null;

    [SerializeField] private Text m_time = null;
    [SerializeField] private Text m_score = null;

    [SerializeField] private GameObject m_panelResult = null;
    [SerializeField] private GameObject m_inputResult = null;
    [SerializeField] private GameObject m_rank = null;
    [SerializeField] private Text m_baseRankText = null;
    private List<Text> m_texts = new List<Text>(); // 사본
    [SerializeField] private InputField m_inputNick = null;
    #endregion

    #region Variable Members
    private State m_state = State.None;
    private float m_showTextSeconds = 0.0f;
    private float m_remainSeconds = 0.0f;
    private int m_numClicks = 0;

    private Dictionary<string, int> m_records = new Dictionary<string, int>();
    #endregion

    #region Monobehaviour Callback
    private void Start()
    {
        InitBackend();
        SetState(State.Title);
    }

    private void Update()
    {
        Backend.AsyncPoll();
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
    private void InitBackend()
    {
        Backend.InitializeAsync(true, callback => {
            if (callback.IsSuccess())
            {
                // 초기화 성공 시 로직
                Debug.LogWarning("suc");
            }
            else
            {
                Debug.LogWarning("fail");
                // 초기화 실패 시 로직
            }
        });
    }

    private void SetState(State state)
    {
        m_state = state;
        if (m_state == State.Title)
        {
            m_panelTitle.SetActive(true);
        }
        else if (m_state == State.Ready)
        {
            m_remainSeconds = kGameTime;
            m_numClicks = 0;
            m_time.text = m_remainSeconds.ToString();
            m_score.text = m_numClicks.ToString();

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
            m_inputResult.SetActive(true);
            m_rank.SetActive(false);
        }
    }
    #endregion

    #region Public Method
    public void OnClickButton(Button btn)
    {
        if (m_state == State.Title)
        {
            if (btn.name.Equals("ButtonStart"))
            {
                m_panelTitle.SetActive(false);
                SetState(State.Ready);
            }
        }
        else if (m_state == State.Play)
        {
            if (btn.name.Equals("ButtonGame"))
                m_numClicks++;
        }
        else if (m_state == State.Result)
        {
            if (btn.name.Equals("ButtonNick"))
            {
                m_records.Add(m_inputNick.text, m_numClicks);
                m_inputResult.SetActive(false);
                m_rank.SetActive(true);

                int index = 0;
                foreach(KeyValuePair<string, int> pair in m_records)
                {
                    if (m_texts.Count == index)
                    {
                        Text copy = Instantiate(m_baseRankText, m_baseRankText.transform.parent);
                        copy.gameObject.SetActive(true);
                        m_texts.Add(copy);
                    }
                    m_texts[index].text = pair.Key + " : " + pair.Value;

                    index++;
                }
            }
            if (btn.name.Equals("ButtonResult"))
            {
                SetState(State.Ready);
            }
        }
    }
    #endregion
}
