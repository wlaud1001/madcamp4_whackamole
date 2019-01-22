using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
public class SummaryManager : MonoBehaviour
{
    bool requestFinished;
    bool requestErrorOccurred;


    public TextMeshProUGUI hit;
    public TextMeshProUGUI score;
    public InputField input;
    public Button add;
    public TextMeshProUGUI[] rank_text;



    struct ranking
    {
        public string id;
        public int score;

    }

    private ranking[] ranklist = new ranking[5];


    // Use this for initialization
    void Start()
    {
        //assign the text to the coordinating player Pref
        hit.text = "Total Hits: " + PlayerPrefs.GetInt(ScoreTypes.PlayerHits.ToString());
        score.text = "Total Score: " + PlayerPrefs.GetInt(ScoreTypes.PlayerScore.ToString());

        input.placeholder.GetComponent<Text>().text = "ID";

        recordShow();

   

    }


    public void RestartPressed()
    {

        //load back to Game scene when tapped
        SceneManager.LoadSceneAsync(GameScenes.unetpractice.ToString());
        Debug.Log(input.text);
    }


    public void Add()
    {

        Debug.Log(input.text);
        Debug.Log(PlayerPrefs.GetInt(ScoreTypes.PlayerScore.ToString()));

        recordUpdate(input.text, PlayerPrefs.GetInt(ScoreTypes.PlayerScore.ToString()));

        input.text = "";
 

    }

    public void recordUpdate(string name, int score)
    {
        string json = "{\"name\":\"" + name + "\"," +
             "\"score\":\"" + score + "\"}";
        StartCoroutine(PostRequest("http://143.248.140.106:3280/recordUpdate", json));

    }


    public void recordShow()
    {
        string json = "{\"name\":\"" + "test" + "\"," +
            "\"score\":\"" + "test" + "\"}";
        StartCoroutine(PostRequest("http://143.248.140.106:3280/recordShow", json));

    }

    IEnumerator PostRequest(string url, string bodyJsonString)
    {
        requestFinished = false;
        requestErrorOccurred = false;

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();
        requestFinished = true;

        if (request.isNetworkError)
        {
            Debug.Log("Something went wrong, and returned error: " + request.error);
            requestErrorOccurred = true;
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            string[] name_arr = new string[5];
            int[] score_arr = new int[5];
            var result = request.downloadHandler.text;


            JArray arr = JArray.Parse(result);

            for (int i = 0; i < arr.Count; i++)
            {
                string ind = (i + 1).ToString();

                ranklist[i].id = (string)arr[i].SelectToken("name");
                ranklist[i].score = (int)arr[i].SelectToken("score");
                Debug.Log("id" + i + "/" + name_arr[i]);
                rank_text[i].text = ind + ". " + ranklist[i].id + "    " + ranklist[i].score;
            }


        }
    }
}


