using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ButtonData {
    public Sprite image;
    public Sprite badge;
    public string text;
    public bool spawnInProgress;
    public int queueCount;
    public float queueProgress;
}

public class MenuButton : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private Image badge;
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform queueProgressRoot;
    [SerializeField] private RectTransform queueProgress;
    [SerializeField] private RectTransform queueRoot;

    private float _progressHeight;

    private void Awake() {
        _progressHeight = queueProgressRoot.rect.height;
    }

    public void SetData(ButtonData buttonData) {
        image.sprite = buttonData.image;
        badge.sprite = buttonData.badge;
        text.text = buttonData.text;
        queueProgressRoot.gameObject.SetActive(buttonData.spawnInProgress);
        queueProgress.SetLocalTop((1 - buttonData.queueProgress) * _progressHeight, false);
        for (var i = 0; i < queueRoot.childCount; i++) {
            queueRoot.GetChild(i).gameObject.SetActive(i < buttonData.queueCount);
        }
    }
}