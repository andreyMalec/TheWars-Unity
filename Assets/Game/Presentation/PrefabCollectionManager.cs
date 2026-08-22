using System;
using UnityEngine;

public class PrefabCollectionManager : MonoBehaviour {
    public static PrefabCollection Instance;
    [SerializeField] private PrefabCollection prefabCollection;

    private void Awake() {
        Instance = prefabCollection;
        DontDestroyOnLoad(this);
    }
}