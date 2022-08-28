using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class DatabaseManager : MonoBehaviour
{
    public string urlAC1;
    public string urlAuth;
    public UIManager uiManager, UiManagerAC1;
    public TextMeshProUGUI alert;
    public GameObject LoginScreen, GameScreen, Buttons, Actividad2;

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("token")))
        {
            ExitSession();
            return;
        }

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("username")))
        {
            ExitSession();
            return;
        }

        StartCoroutine(ValidToken(null, true));
    }

    public void GetDataButton()
    {
        StartCoroutine(ValidToken(GetData()));
    }

    public void ExitSession()
    {
        PlayerPrefs.SetString("token", "");
        PlayerPrefs.SetString("username", "");
        LoginScreen.SetActive(true);
        GameScreen.SetActive(false);
    }

    IEnumerator GetData()
    {
        string url = urlAuth + "api/usuarios";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", PlayerPrefs.GetString("token"));
        
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
    
    public void GetDataButtonAC1()
    {
        StartCoroutine(GetDataAC1());
    }

    IEnumerator GetDataAC1()
    {
        string url = urlAC1 + "/db";
        UnityWebRequest www = UnityWebRequest.Get(url);
        
        yield return www.SendWebRequest();

        switch (www.result)
        {
            case UnityWebRequest.Result.InProgress:
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log(www.downloadHandler.text);
                Root data = JsonUtility.FromJson<Root>(www.downloadHandler.text);
                UiManagerAC1.GetData(data);
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

    public Login user;
    public Usuario currentUser;

    public void SetUsername(string value)
    {
        user.username = value;
    }
    public void SetPassword(string value)
    {
        user.password = value;
    }
    public void SetScore(string value)
    {
        currentUser.score = int.Parse(value);
    }
    public void ChangeToken(string value)
    {
        PlayerPrefs.SetString("token",value);
    }

    public IEnumerator ValidToken(IEnumerator callback, bool startLogin = false)
    {
        string path = urlAuth+"api/usuarios/"+currentUser.username;
        Debug.Log(path);
        UnityWebRequest www = UnityWebRequest.Get(path);
        www.SetRequestHeader("x-token", PlayerPrefs.GetString("token"));
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        switch (www.result)
        {
            case UnityWebRequest.Result.InProgress:
                break;
            case UnityWebRequest.Result.Success:
                if (startLogin)
                {
                    Buttons.SetActive(false);
                    Actividad2.SetActive(true);
                    LoginScreen.SetActive(false);
                    GameScreen.SetActive(true);
                }
                else
                {
                    StartCoroutine(callback);
                    LoginScreen.SetActive(false);
                    GameScreen.SetActive(true);
                }
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                if (startLogin)
                {
                    Buttons.SetActive(true);
                    Actividad2.SetActive(false);
                    LoginScreen.SetActive(true);
                    GameScreen.SetActive(false);
                }
                else
                {
                    alert.text = "token no valido"; 
                    Debug.Log(www.error);
                    LoginScreen.SetActive(true);
                    GameScreen.SetActive(false);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void StartLogin()
    {
        StartCoroutine(PostLogin(JsonUtility.ToJson(user)));
    }

    public void StartSignUp()
    {
        StartCoroutine(PostSignup(JsonUtility.ToJson(user)));
    }
    public void StartChangeScore()
    {
        StartCoroutine(ValidToken(ChangeScore(JsonUtility.ToJson(currentUser))));
    }
    IEnumerator PostLogin(string data)
    {
        string path = urlAuth+"api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(path, data);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
        switch (www.result)
        {
            case UnityWebRequest.Result.InProgress:
                break;
            case UnityWebRequest.Result.Success:
                
                alert.text = "";
                PlayerPrefs.SetString("token", resData.token);
                PlayerPrefs.SetString("username", resData.usuario.username);
                currentUser.username = resData.usuario.username;
                LoginScreen.SetActive(false);
                GameScreen.SetActive(true);
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                alert.text = resData.msg;
                Debug.Log(www.error);
                LoginScreen.SetActive(true);
                GameScreen.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    IEnumerator PostSignup(string data)
    {
        string path = urlAuth+"api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(path, data);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
        switch (www.result)
        {
            case UnityWebRequest.Result.InProgress:
                break;
            case UnityWebRequest.Result.Success:
                alert.text = "";
                StartCoroutine(PostLogin(data));
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                alert.text = resData.msg;
                Debug.Log(www.error);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    IEnumerator ChangeScore(string data)
    {
        string path = urlAuth+"api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(path, data);
        www.method = "PATCH";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", PlayerPrefs.GetString("token"));
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        switch (www.result)
        {
            case UnityWebRequest.Result.InProgress:
                break;
            case UnityWebRequest.Result.Success:
                AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
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
    public List<Usuario> usuarios;
}

[Serializable]
public class Login
{
    public string username;
    public string password;
}

[Serializable]
public class AuthData
{
    public string username;
    public string password;
    public Usuario usuario;
    public string token;
    public string msg;
}

[Serializable]
public class Usuario
{
    public string _id;
    public string username;
    public string password;
    public bool estado;
    public int score;
}