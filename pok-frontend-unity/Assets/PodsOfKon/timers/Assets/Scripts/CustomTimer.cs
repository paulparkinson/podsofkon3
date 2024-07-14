using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.Networking;

public class CustomTimer : MonoBehaviour
{
    [Tooltip("Duration of timer in seconds")]
    public float duration;

    public enum CountUpOrDown {countUp, countDown}

    [Tooltip("Should the timer text start at 0 & count up to the set Duration or start at the Duration & count down to 0? You can completely disable the text below")]
    public CountUpOrDown countUpOrDown;

    public enum FillUpOrDown {fillUp, fillDown}

    [Tooltip("The sprite(s) can either start empty and fill up over time, or start full and fill down over time. You can disable all movement below")]
    public FillUpOrDown fillUpOrDown;

    [Tooltip("Rotate where the image starts/stops filling")]
    [Range(0, 360)]
    public float rotation;
    [Tooltip("Flips the direction the Timer moves")]
    public bool flipFillDirection;

    [Tooltip("Keep timer steadily running no matter the Time Scale. Useful for timers that run while game is paused or in slow-motion")]
    public bool unscaledTime;

    [System.Serializable]
    public class TopSpriteSettings
    {
        [Tooltip("Enables/disables the Top_Sprite object")]
        public bool enabled;
        [Tooltip("Should the top circle fill/unfill over time?")]
        public bool movement;
        [Tooltip("The 'Top_Sprite' object in scene childed to this object")]
        public Image imageObject;
        [Tooltip("Scale factor of the Top Sprite")]
        public float scale;
        [Tooltip("The color of the Top Sprite")]
        public Color color;
    }
    public TopSpriteSettings m_topSpriteSettings;

    [System.Serializable]
    public class MiddleSpriteSettings
    {      
        [Tooltip("Enables/disables the Middle_Sprite object")]
        public bool enabled;
        [Tooltip("Should the middle circle fill/unfill over time?")]   
        public bool movement;
        [Tooltip("The 'Middle_Sprite' object in scene childed to this object")]
        public Image imageObject;
        [Tooltip("Scale factor of the Middle Sprite")]
        public float scale;
        [Tooltip("The color of the Middle Sprite")]
        public Color color;
    }
    public MiddleSpriteSettings m_middleSpriteSettings;

    [System.Serializable]
    public class BottomSpriteSettings
    {
        [Tooltip("Enables/disables the Bottom_Sprite object")]
        public bool enabled;
        [Tooltip("Should the Bottom circle fill/unfill over time?")]
        public bool movement;
        [Tooltip("The 'Bottom_Sprite' object in scene childed to this object")]
        public Image imageObject;
        [Tooltip("Scale factor of the Bottom Sprite")]
        public float scale;
        [Tooltip("The color of the Bottom Sprite")]
        public Color color;
    }
    public BottomSpriteSettings m_bottomSpriteSettings;

    [System.Serializable]
    public class TimerTextSettings
    {
        [Tooltip("Show or hide the text numbers")]
        public bool textEnabled;
        [Tooltip("Show or hide milliseconds decimals")]
        public bool milliseconds;
        [Tooltip("The 'Timer_Text' object in scene childed to this object")]
        public Text textObject;
        [Tooltip("The size of the font")]
        public int fontSize;
        [Tooltip("The color of the text")]
        public Color color;
    }
    public TimerTextSettings m_timerTextSettings;

    public UnityEvent timerEnd;
    bool timerPaused = true;
    float curTime;

    void OnEnable()
    {
        timerPaused = false;
    }

    private void Awake()
    {
        timerPaused = false;
    }

    void Start()
    {
     //   isOver = false;
        curTime = 0;
        timerPaused = false;
    }

   // private bool isOver = false;
    public GameObject gameObjectToDeactivate;
    public GameObject gameObjectToActivate;
    public GameObject gameGridQuad;
    public GameObject gameGridLeftWall;
    public GameObject gameGridRightWall;
    public Material newMaterial; 
    public Material newMaterialFinal20; 

	void Update () 
    {
   //     if(isOver)return;
   if (Input.GetKeyDown(KeyCode.R))
   {
       Debug.Log("timer curTime="+curTime + " timerPaused:" + timerPaused);
   //    isOver = false;
   //    curTime = 0;
       timerPaused = false;
       
   }
        if (GetTimerValue() == .7f)
        {
            replaceRendererMaterial(gameGridQuad.GetComponent<MeshRenderer>(), newMaterialFinal20);
            replaceRendererMaterial(gameGridLeftWall.GetComponent<MeshRenderer>(), newMaterialFinal20);
            replaceRendererMaterial(gameGridRightWall.GetComponent<MeshRenderer>(), newMaterialFinal20);
        }   if (GetTimerValue() == .5f)
        {
            replaceRendererMaterial(gameGridQuad.GetComponent<MeshRenderer>(), newMaterial);
            replaceRendererMaterial(gameGridLeftWall.GetComponent<MeshRenderer>(), newMaterial);
            replaceRendererMaterial(gameGridRightWall.GetComponent<MeshRenderer>(), newMaterial);
        }  if (GetTimerValue() == 1 || Input.GetKeyDown(KeyCode.T))
        { 
        //    isOver = true;
            curTime = 0;
            timerPaused = true;
            Countdown();
            m_timerTextSettings.textObject.text = "0";
            m_timerTextSettings.textObject.gameObject.SetActive(false);
            //todo ideally put a firebutton continue prompt here and also use blocking to make sure all points are in before doing movescores...
            // if (isBonusRound)
            // { //todo make sure this cant happen more than once
            //     Debug.Log("Bonus Round complete, movescores from currentgame to scores");
            //     StartCoroutine(GetRequest("http://143.47.96.92/podsofkon/movescores?player1Name=testplayer1name&player2Name=testplayer2name"));
            // }   
            if (gameObjectToActivate!=null) gameObjectToActivate.SetActive(true);
        //    if (false && sceneHolder!=null) sceneHolder.DestroyScene(); //there two mechanisms here, call Destroy or just setActive false
            if (gameObjectToDeactivate!=null) gameObjectToDeactivate.SetActive(false);
            MyCharacter.screenNumber++;
        } else Countdown();
    }

    private void replaceRendererMaterial(MeshRenderer renderer, Material replacementMaterial)
    {
        if (renderer != null)
        {
            Material[] materials = renderer.materials;

            if (materials.Length > 0)
            {
                materials[0] = replacementMaterial; // Change the first material
                renderer.materials = materials; // Apply the changed material array back to the renderer
            }
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
#if UNITY_EDITOR
            //   PlayerSettings.insecureHttpOption = local ? InsecureHttpOption.AlwaysAllowed : InsecureHttpOption.NotAllowed;
            PlayerSettings.insecureHttpOption =  InsecureHttpOption.AlwaysAllowed ;
#endif

            yield return webRequest.SendWebRequest();

            //   string[] pages = uri.Split('/');
            //    int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(uri + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

        
    public void UpdateEditorStuff()
    {
        if (m_middleSpriteSettings.enabled)
        {
            m_middleSpriteSettings.imageObject.gameObject.SetActive(true);
        }
        else
        {
            m_middleSpriteSettings.imageObject.gameObject.SetActive(false);
        }
        m_middleSpriteSettings.imageObject.color = m_middleSpriteSettings.color;

        m_middleSpriteSettings.imageObject.rectTransform.eulerAngles = new Vector3(0, 0, rotation);

        if (m_topSpriteSettings.enabled)
        {
            m_topSpriteSettings.imageObject.gameObject.SetActive(true);
        }
        else
        {
            m_topSpriteSettings.imageObject.gameObject.SetActive(false);
        }
        m_topSpriteSettings.imageObject.color = m_topSpriteSettings.color;

        m_topSpriteSettings.imageObject.rectTransform.eulerAngles = new Vector3(0, 0, rotation);

        if (m_bottomSpriteSettings.enabled)
        {
            m_bottomSpriteSettings.imageObject.gameObject.SetActive(true);
        }
        else
        {
            m_bottomSpriteSettings.imageObject.gameObject.SetActive(false);
        }
        m_bottomSpriteSettings.imageObject.color = m_bottomSpriteSettings.color;

        m_bottomSpriteSettings.imageObject.rectTransform.eulerAngles = new Vector3(0, 0, rotation);

        if (flipFillDirection)
        {
            m_topSpriteSettings.imageObject.rectTransform.localScale = new Vector3(-m_topSpriteSettings.scale, m_topSpriteSettings.scale, m_topSpriteSettings.scale);
            m_middleSpriteSettings.imageObject.rectTransform.localScale = new Vector3(-m_middleSpriteSettings.scale, m_middleSpriteSettings.scale, m_middleSpriteSettings.scale);
            m_bottomSpriteSettings.imageObject.rectTransform.localScale = new Vector3(-m_bottomSpriteSettings.scale, m_bottomSpriteSettings.scale, m_bottomSpriteSettings.scale);
        }
        else
        {
            m_topSpriteSettings.imageObject.rectTransform.localScale = new Vector3(m_topSpriteSettings.scale, m_topSpriteSettings.scale, m_topSpriteSettings.scale);
            m_middleSpriteSettings.imageObject.rectTransform.localScale = new Vector3(m_middleSpriteSettings.scale, m_middleSpriteSettings.scale, m_middleSpriteSettings.scale);
            m_bottomSpriteSettings.imageObject.rectTransform.localScale = new Vector3(m_bottomSpriteSettings.scale, m_bottomSpriteSettings.scale, m_bottomSpriteSettings.scale);
        }

        if (m_timerTextSettings.textEnabled)
        {
            m_timerTextSettings.textObject.gameObject.SetActive(true);
        }
        else
        {
            m_timerTextSettings.textObject.gameObject.SetActive(false);
        }
        if (!Application.isPlaying)
        {
            if (m_timerTextSettings.milliseconds)
            {
                m_timerTextSettings.textObject.text = duration.ToString("F2");
            }
            else
            {
                m_timerTextSettings.textObject.text = duration.ToString("F0");
            }

            if (m_topSpriteSettings.movement)
            {
                m_topSpriteSettings.imageObject.fillAmount = 0.92f;
            }
            else
            {
                m_topSpriteSettings.imageObject.fillAmount = 1f;
            }

            if (m_middleSpriteSettings.movement)
            {
                m_middleSpriteSettings.imageObject.fillAmount = 0.92f;
            }
            else
            {
                m_middleSpriteSettings.imageObject.fillAmount = 1f;
            }

            if (m_bottomSpriteSettings.movement)
            {
                m_bottomSpriteSettings.imageObject.fillAmount = 0.92f;
            }
            else
            {
                m_bottomSpriteSettings.imageObject.fillAmount = 1f;
            }
        }
        m_timerTextSettings.textObject.fontSize = m_timerTextSettings.fontSize;
        m_timerTextSettings.textObject.color = m_timerTextSettings.color;
    }

    void Countdown()
    {
        if (!timerPaused)
        {
            if (unscaledTime == false)
            {
                curTime += Time.deltaTime;
            }
            else if(unscaledTime == true)
            {
                curTime += Time.unscaledDeltaTime;
            }

            if (curTime >= duration)
            {
                curTime = duration;
                timerPaused = true;
                timerEnd.Invoke();
            }
        }

        switch (fillUpOrDown)
        {
            case FillUpOrDown.fillUp:

                if (m_middleSpriteSettings.movement)
                {
                    m_middleSpriteSettings.imageObject.fillAmount = curTime / duration;

                }
                if (m_topSpriteSettings.movement)
                {
                    m_topSpriteSettings.imageObject.fillAmount = curTime / duration;
                } 

                if (m_bottomSpriteSettings.movement)
                {
                    m_bottomSpriteSettings.imageObject.fillAmount = curTime / duration;
                } 
                break;

            case FillUpOrDown.fillDown:

                if (m_middleSpriteSettings.movement)
                {
                    m_middleSpriteSettings.imageObject.fillAmount = ((duration - curTime) / duration);
                }

                if (m_topSpriteSettings.movement)
                {
                    m_topSpriteSettings.imageObject.fillAmount = ((duration - curTime) / duration);
                }

                if (m_bottomSpriteSettings.movement)
                {
                    m_bottomSpriteSettings.imageObject.fillAmount = ((duration - curTime) / duration);
                }
                break;
        }

        switch (countUpOrDown)
        {
            case CountUpOrDown.countUp:

                if (m_timerTextSettings.milliseconds)
                {
                    m_timerTextSettings.textObject.text = curTime.ToString("F2");
                }
                else
                {
                    m_timerTextSettings.textObject.text = curTime.ToString("F0");
                }
                break;

            case CountUpOrDown.countDown:

                if (m_timerTextSettings.milliseconds)
                {
                    m_timerTextSettings.textObject.text = (duration - curTime).ToString("F2");
                }
                else
                {
                    m_timerTextSettings.textObject.text = (duration - curTime).ToString("F0");
                }
                break;
        }
    }

    public void SomeButton()
    {
        if (curTime >= duration)
        {
            curTime = 0;
        }
        timerPaused = false;
    }

    public void StartTimer()
    {
        if (curTime >= duration)
        {
            curTime = 0;
        }
        timerPaused = false;
    }

    public void PauseTimer()
    {
        timerPaused = true;
    }

    public void ResetTimer()
    {
        timerPaused = true;
        curTime = 0;
    }

    public void RestartTimer()
    {
        curTime = 0;
        timerPaused = false;
    }        

    public void DisableTimer()
    {
        this.gameObject.SetActive(false);
    }

    public void DestroyTimer()
    {
        Destroy(this.gameObject);
    }

    public void ReverseFillDirection()
    {
        switch (fillUpOrDown)
        {
            case FillUpOrDown.fillUp:

                fillUpOrDown = FillUpOrDown.fillDown;
                break;

            case FillUpOrDown.fillDown:

                fillUpOrDown = FillUpOrDown.fillUp;
                break;
        }
    }

    public void ReverseCountDirection()
    {
        switch (countUpOrDown)
        {
            case CountUpOrDown.countUp:

                countUpOrDown = CountUpOrDown.countDown;
                break;

            case CountUpOrDown.countDown:

                countUpOrDown = CountUpOrDown.countUp;
                break;
        }
    }

    public void LoopAnimation()
    {
        ResetTimer();
        ReverseFillDirection();
        if (flipFillDirection == false)
        {
            flipFillDirection = true;
        }
        else
        {
            flipFillDirection = false;
        }
        StartTimer();

    }

    /// <summary>
    /// This function returns a value between 0 and 1 depending on progress of the timer. 
    /// The value is 0 before the timer starts, 0.5 when its halfway done, 1 when it completes, and everything in between. 
    /// </summary>
    /// <returns>Timer progress as a float between 0 & 1.</returns>
    public float GetTimerValue()
    {
        return (float)System.Math.Round(curTime / duration, 2);
    }
}