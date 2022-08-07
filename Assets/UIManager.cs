using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject Prefab;
    public Transform parent;
    public List<GameObject> Objects;
    public void GetData(Root infos)
    {
        foreach (GameObject o in Objects)
        {
            Destroy(o);
        }

        Objects = new List<GameObject>();
        for (int i = 0; i < infos.users.Count; i++)
        {
            UserElement user = Instantiate(Prefab, parent).GetComponent<UserElement>();
            user.SetInfo(infos.users[i].name, infos.users[i].score);
            Objects.Add(user.gameObject);
        }
    }
}
