using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserElement : MonoBehaviour
{
    public TextMeshProUGUI User, Score;

    public void SetInfo(string User, int Score)
    {
        this.User.text = User;
        this.Score.text = "Score: " + Score;
    }
}
