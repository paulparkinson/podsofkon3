using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Networking;

public class BonusRoundQuestions : MonoBehaviour
{
    public bool isBonusRound;
    public string playerName;
    private static bool isQuestionsLoaded;
    private static BonusRound bonusRound;
    public GameObject successSound;
    public GameObject failureSound;
    public GameObject[] answerObjects;
    private bool[] isAnswerCorrect = new bool[4];
    private bool[] isAnswerSelected = new bool[4];
    private int randomNumber;
    private List<int> availableIndices;
    public Material buttonUnSelectedMaterial;
    public Material buttonSelectedMaterial;

    void Start()
    {
        if(!EnvProperties.isOffline) StartCoroutine(GetRequest(EnvProperties.bankendAddress + "/podsofkon/questions", true));
    }

    IEnumerator GetRequest(string uri, bool isProcessQuestion)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
#if UNITY_EDITOR
            //   PlayerSettings.insecureHttpOption = local ? InsecureHttpOption.AlwaysAllowed : InsecureHttpOption.NotAllowed;
            PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
#endif
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                if (isProcessQuestion)
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                    Debug.Log($"jsonResponse: {jsonResponse}");
                    bonusRound = JsonUtility.FromJson<BonusRound>(jsonResponse);
                    availableIndices = new List<int>(bonusRound.questions.Count);
                    for (int i = 0; i < bonusRound.questions.Count; i++)
                    {
                        availableIndices.Add(i);
                    }
                    ProcessQuestion(true);
                }
            }
            else
            {
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
    }

    public int GetUniqueRandomIndex()
    {
        if (availableIndices.Count == 0)
        {
            Debug.LogError("No more unique indices available.");
            // went through all the questions and so will keep asking the same/first question -
            // we have so many questions this generally doesn't occur but we could potentially re-ask incorrectly answered questions or some such
           // return 0; 
            return Random.Range(0, bonusRound.questions.Count -  1); 
        }
        int randomIndex = Random.Range(0, availableIndices.Count);
        int uniqueIndex = availableIndices[randomIndex];
        availableIndices.RemoveAt(randomIndex);
        return uniqueIndex;
    }

    void ProcessQuestion(bool isStartupCall) //called during setup and during ProcessSubmit
    {
        if (isBonusRound)
        {
            Question question = bonusRound.questions[GetUniqueRandomIndex()];
            TextMeshPro mText = GetComponent<TextMeshPro>();
            mText.text = question.text;
            Debug.Log($"ProcessQuestion question.text: {question.text}");
            int answerArrayIndex = 0;
            foreach (var answer in question.answers)
            {
                answerObjects[answerArrayIndex].GetComponentInChildren<TextMeshPro>().text = answer.text;
                isAnswerCorrect[answerArrayIndex] = answer.isCorrect.Equals("true") || answer.isCorrect.Equals("True");
                Debug.Log(
                    $"ProcessQuestion answer : {answer.text}, " +
                    $"isCorrect text from json: {answer.isCorrect}, should match" +
                    $" isAnswerCorrect[answerArrayIndex]: {isAnswerCorrect[answerArrayIndex]}");
                answerObjects[answerArrayIndex].SetActive(true);
                answerArrayIndex++;
            }
            if (isStartupCall && answerArrayIndex < 4) //hack to get rid of "false" answer options in first question bug.
            {
                answerObjects[answerArrayIndex].SetActive(false);
                if(answerArrayIndex == 2) answerObjects[answerArrayIndex + 1].SetActive(false);
            }
         //  currentQuestionIndex++;
        }
    }


    private bool isArraysMatch()
    {
        for (int i = 0; i < isAnswerCorrect.Length; i++)
        {
            Debug.Log("isAnswerCorrect[" + i + "]: " + isAnswerCorrect[i]);
            Debug.Log("isAnswerSelected[" + i + "]: " + isAnswerSelected[i]);
            if (isAnswerCorrect[i] != isAnswerSelected[i])
            {
                return false;
            }
        }
        return true;
    }

    public void ProcessAnswerSelection(int answerIndexNumber, GameObject buttonGameObject)
    {
        if (isAnswerSelected[answerIndexNumber])
        {
            Debug.Log("ProcessSubmit correct answer isAnswerSelected[answerIndexNumber]:" +
                      isAnswerSelected[answerIndexNumber]);
            isAnswerSelected[answerIndexNumber] = false;
            replaceRendererMaterial(buttonGameObject.GetComponent<MeshRenderer>(), buttonUnSelectedMaterial);
        }
        else
        {
            isAnswerSelected[answerIndexNumber] = true;
            replaceRendererMaterial(buttonGameObject.GetComponent<MeshRenderer>(), buttonSelectedMaterial);
        }
    }

    public void ProcessSubmit()
    {
        if (isArraysMatch())
        {
            successSound.GetComponent<AudioSource>().Play();
            Debug.Log("ProcessSubmit correct answer");
            ScoreKeeper.getInstance().CorrectAnsewr(playerName);
        }
        else
        {
            Debug.Log("ProcessSubmit incorrect answer");
            failureSound.GetComponent<AudioSource>().Play();
        }
        SetInactiveAnswerObjects();
        ProcessQuestion(false); //load the next
    }

    public void SetInactiveAnswerObjects()
    {
        foreach (GameObject answerObject in answerObjects)
        {
            replaceRendererMaterial(answerObject.GetComponent<MeshRenderer>(), buttonUnSelectedMaterial);
            answerObject.SetActive(false);
        }

        isAnswerCorrect = new bool[5];
        isAnswerSelected = new bool[5];
    }


    private void replaceRendererMaterial(MeshRenderer renderer, Material replacementMaterial)
    {
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            if (materials.Length > 0)
            {
                materials[0] = replacementMaterial; 
                renderer.materials = materials; 
            }
        }
    }

    [System.Serializable]
    public class BonusRound
    {
        public List<Question> questions;
    }

    [System.Serializable]
    public class Question
    {
        public string text;
        public List<Answer> answers;
    }

    [System.Serializable]
    public class Answer
    {
        public string text;
        public string isCorrect;
    }
}