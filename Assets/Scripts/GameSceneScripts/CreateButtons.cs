using System.Collections;
using UnityEngine;

public class CreateButtons : MonoBehaviour
{
    [SerializeField]
    private Transform cardField;
    [SerializeField]
    private GameObject btn;
   void Awake()
   {
        for(int i = 0;i < 16; i++)
        {
            GameObject button = Instantiate(btn);
            button.name = "" + i;
            button.transform.SetParent(cardField, false);
        }
   }
}
