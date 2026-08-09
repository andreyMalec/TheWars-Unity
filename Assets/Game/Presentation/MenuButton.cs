using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ButtonData {
    public Sprite image;
    public Sprite badge;
    public string text;
}

public class MenuButton : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private Image badge;
    [SerializeField] private TMP_Text text;

    public void SetData(ButtonData buttonData) {
        image.sprite = buttonData.image;
        badge.sprite = buttonData.badge;
        text.text = buttonData.text;
    }
}