using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        List<User> users = infos.users.OrderByDescending(x => x.score).ToList();
        Objects = new List<GameObject>();
        for (int i = 0; i < users.Count; i++)
        {
            UserElement user = Instantiate(Prefab, parent).GetComponent<UserElement>();
            user.SetInfo(users[i].name, users[i].score);
            Objects.Add(user.gameObject);
        }
    }
}
