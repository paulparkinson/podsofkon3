using System.Collections;
using TMPro;
using UnityEngine;

using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine.Networking;


public class ScoreKeeper : MonoBehaviour
{
    private  int player1Score;
    private  int player2Score;
    private  string player1Name = "steelix";
    private  string player2Name = "umbreon";
    private  bool isChangesToPersist;
    private static ScoreKeeper singleton;
    [SerializeField] private TMP_Text architectureScreenPlayerNames ;
    [SerializeField] private TMP_Text player1ScoreText;
    [SerializeField] private TMP_Text player2ScoreText;
    [SerializeField] private TMP_Text player1ScoreTextBonusPage;
    [SerializeField] private TMP_Text player2ScoreTextBonusPage;
    [SerializeField] private TMP_Text player1ScoreTextTYPage;
    [SerializeField] private TMP_Text player2ScoreTextTYPage;

    private void OnEnable()
    {
        singleton = this;
    }

    public void UpdateScoreForSagasCompleted(int player1Active, int player2Active)
    {
        player1Score += player1Active;
        player2Score += player2Active;
        player1ScoreText.text = player1Name + ": \n" + player1Score;
        player1ScoreTextBonusPage.text = player1Name + ": \n" + player1Score;
        player1ScoreTextTYPage.text = player1Name + ": \n" + player1Score;
        player2ScoreText.text = player2Name + ": \n" + player2Score;
        player2ScoreTextBonusPage.text = player2Name + ": \n" + player2Score;
        player2ScoreTextTYPage.text = player2Name + ": \n" + player2Score;
    }
    public static ScoreKeeper getInstance()
    {
        return singleton;
    }

    public void SetPlayerNames()
    {
        StartCoroutine(GetRequest(EnvProperties.bankendAddress + "/podsofkon/getPlayerName?playerName=player1", 
            true, false));
        StartCoroutine(GetRequest(EnvProperties.bankendAddress + "/podsofkon/getPlayerName?playerName=player2", 
            false, true));
        architectureScreenPlayerNames.text = player1Name + " vs " + player2Name;
    }
    
    public void Reset() //called as part of thanks scene
    {
      //  Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Reset");
        StartCoroutine(GetRequest(
            EnvProperties.bankendAddress + "/podsofkon/movescores?" +
            "player1Name="+player1Name +"&player2Name="+player2Name+
            "&player1Score="+player1Score +"&player2Score="+player2Score, false, false));

        player1ScoreText.text = player1Name + ": \n" + player1Score;
        player1ScoreTextBonusPage.text = player1Name + ": \n" + player1Score;
        player1ScoreTextTYPage.text = player1Name + ": \n" + player1Score;
        player2ScoreText.text = player2Name + ": \n" + player2Score;
        player2ScoreTextBonusPage.text = player2Name + ": \n" + player2Score;
        player2ScoreTextTYPage.text = player2Name + ": \n" + player2Score;
    }
    
    public void ResetScore() //called as part of opening/architecture scene
    {
        player1Score = 0;
        player2Score = 0;
        player1ScoreText.text = player1Name + ": \n" + player1Score;
        player1ScoreTextBonusPage.text = player1Name + ": \n" + player1Score;
        player1ScoreTextTYPage.text = player1Name + ": \n" + player1Score;
        player2ScoreText.text = player2Name + ": \n" + player2Score;
        player2ScoreTextBonusPage.text = player2Name + ": \n" + player2Score;
        player2ScoreTextTYPage.text = player2Name + ": \n" + player2Score;
    }

    public void Provision(string playerName, string objectName)
    {
        StartCoroutine(GetRequest(
            EnvProperties.bankendAddress + "/podsofkon/createDeployment?appName="+playerName +"&serviceName="+objectName,
            false, false));
        UpdateScore(playerName, 100, false);
    }
    public void Derezz(string playerName, string objectName)
    {
        StartCoroutine(GetRequest(
            EnvProperties.bankendAddress + "/podsofkon/deleteDeployment?appName="+playerName +"&serviceName="+objectName+"-deployment", 
            false, false));
        UpdateScore(playerName, 100, true);
    }

    public void CorrectAnsewr(string playerName)
    {
        UpdateScore(playerName, 200, false);
    }

    public void UpdateScore(string playerName, int points, bool isDerezz)
    {
        if (playerName == "player1")
        {
            player1Score += points;
            player1ScoreText.text = player1Name + ": \n" + player1Score;
            player1ScoreTextBonusPage.text = player1Name + ": \n" + player1Score;
            player1ScoreTextTYPage.text = player1Name + ": \n" + player1Score;
        }
        else
        {
            player2Score += points;
            player2ScoreText.text = player2Name + ": \n" + player2Score;
            player2ScoreTextBonusPage.text = player2Name + ": \n" + player2Score;
            player2ScoreTextTYPage.text = player2Name + ": \n" + player2Score;
        }
        isChangesToPersist = true;
    }

    public void Update()
    {  
        if (isChangesToPersist) 
        { 
            isChangesToPersist = false;
            StartCoroutine(GetRequest(
                EnvProperties.bankendAddress + "/podsofkon/updateScores?player1Score="+player1Score +"&player2Score="+player2Score, 
                false, false));
        }
    }
    
    public IEnumerator GetRequest(string uri, bool isSetPlayer1Name, bool isSetPlayer2Name)
    {
        if (EnvProperties.isOffline) yield return null;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
#if UNITY_EDITOR
            //   PlayerSettings.insecureHttpOption = local ? InsecureHttpOption.AlwaysAllowed : InsecureHttpOption.NotAllowed;
            PlayerSettings.insecureHttpOption =  InsecureHttpOption.AlwaysAllowed ;
#endif

            yield return webRequest.SendWebRequest();
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
            //        Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    if (isSetPlayer1Name) player1Name = webRequest.downloadHandler.text;
                    else if (isSetPlayer2Name) player2Name = webRequest.downloadHandler.text;
                    break;
            }
        }
    }
    
    
    
    
    



    //  https://answers.unity.com/questions/1482409/how-to-accept-self-signed-certificate.html
    // Based on https://www.owasp.org/index.php/Certificate_and_Public_Key_Pinning#.Net
    class AcceptAllCertificatesSignedWithASpecificKeyPublicKey : CertificateHandler
    {

        // Encoded RSAPublicKey
        private static string PUB_KEY = "mypublickey";

        protected override bool ValidateCertificate(byte[] certificateData)
        {
            X509Certificate2 certificate = new X509Certificate2(certificateData);
            string pk = certificate.GetPublicKeyString();
            if (pk.ToLower().Equals(PUB_KEY.ToLower()))
                return true;
            return true; //should be false
        }
    }

}
