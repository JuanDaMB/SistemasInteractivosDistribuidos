using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class DatabaseManager : MonoBehaviour
{
    public string url;
    public UIManager uiManager;
    public void GetDataButton()
    {
        StartCoroutine(GetData());
    }

    IEnumerator GetData()
    {
        string url = this.url + "/db";
        UnityWebRequest www = UnityWebRequest.Get(url);
        
        yield return www.SendWebRequest();

        switch (www.result)
        {
            case UnityWebRequest.Result.InProgress:
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log(www.downloadHandler.text);
                Root data = JsonUtility.FromJson<Root>(www.downloadHandler.text);
                uiManager.GetData(data);
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.Log(www.error);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
}
[Serializable]
public class Root
{
    public List<User> users;
}

[Serializable]
public class User
{
    public string name;
    public int score;
}