using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ButtonData {
    public Sprite image;
    public string text;
}

public class MenuButton : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;

    public void SetData(ButtonData buttonData) {
        image.sprite = buttonData.image;
        text.text = buttonData.text;
    }
}