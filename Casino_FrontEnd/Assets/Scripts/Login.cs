using Assets.TestApp.TestApp.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    [SerializeField]
    public InputField inputField;
    
	// Use this for initialization
	void Start () {

        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnEnable()
    {
        if (!String.IsNullOrEmpty(PlayerPrefs.GetString("userid")))
        {
            inputField.text = PlayerPrefs.GetString("userid");
        }
        Debug.Log("huhu load");
    }

    public void LoginOnClick()
    {
        string userid = inputField.text;
        string url = "http://107.21.13.205/api/values/" + userid;
        CasinoWarPlayer player = new CasinoWarPlayer();

        if ((player = JsonUtility.FromJson<CasinoWarPlayer>(Get(url))) != null)
        {
            // you know that the parsing attempt
            // was successful

            //PlayerSingleton.SetPlayer(userid, credit);

            Debug.Log(Get(url) + "\n" + JsonUtility.ToJson(player));
            PlayerPrefs.SetString("userid", userid);
            PlayerPrefs.SetString("username", player.getUserName());

            SceneManager.LoadScene(sceneName: "TestScene");
            PlayerSingleton.SetPlayer(player);
            Debug.Log("login as " + player.getUserName() + "credit: " + player.getNewCredit());
        }
        else
        {
            Debug.Log("invalid userid");
        }

    }

    public string Get(string uri)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}
