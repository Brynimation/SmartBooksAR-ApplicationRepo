using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Drawing;

public class CollectedNumber : MonoBehaviour
{
    [SerializeField] TMP_Text numberText;
    Image image;
    void Start()
    {
        image = GetComponent<Image>();
    }
    public void SetNumberText(string text)
    {
        numberText.text = text;
    }
    public float Width 
    {
        //Multiplied by 100 because (I think) the width of the orignal prefab is 100
        get => image.rectTransform.rect.width * 100f;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
