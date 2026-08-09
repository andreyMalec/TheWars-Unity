using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct UIData {
    public ButtonData spawnUnit;
    public ButtonData buySlot;
    public ButtonData buyTurret;
    public ButtonData destroyTurret;
    public ButtonData empty;
}

public class PlayerUiView : MonoBehaviour {
    [SerializeField] private UIData data;
    [SerializeField] private MenuButton upgradeBaseButton;
    [SerializeField] private MenuButton specialWeaponButton;
    [SerializeField] private MenuButton[] actionButtons;
    [SerializeField] private MenuButton backButton;
    [SerializeField] private TMP_Text moneyText;

    private PlayerInputController _inputController;
    private ConfigDatabase _db;
    private BaseState _state;

    private void Awake() {
        _inputController = GetComponent<PlayerInputController>();
        HandleMenuStateChanged(PlayerInputController.MenuState.Main);
        backButton.SetData(data.empty);
    }

    private void OnEnable() {
        _inputController.OnMenuStateChanged += HandleMenuStateChanged;
    }

    private void OnDisable() {
        _inputController.OnMenuStateChanged -= HandleMenuStateChanged;
    }

    public void Bind(ConfigDatabase db) {
        _db = db;
    }

    public void Present(in BaseState state) {
        _state = state;
        moneyText.text = state.Resources.ToString();
    }

    private void HandleMenuStateChanged(PlayerInputController.MenuState menu) {
        backButton.gameObject.SetActive(menu != PlayerInputController.MenuState.Main);

        switch (menu) {
            case PlayerInputController.MenuState.Main:
                for (int i = 0; i < actionButtons.Length; i++) {
                    actionButtons[i].gameObject.SetActive(true);
                }

                actionButtons[0].SetData(data.spawnUnit);
                actionButtons[1].SetData(data.buySlot);
                actionButtons[2].SetData(data.buyTurret);
                actionButtons[3].SetData(data.destroyTurret);
                break;
            case PlayerInputController.MenuState.SpawnUnit:
                for (int i = 0; i < actionButtons.Length; i++) {
                    actionButtons[i].gameObject.SetActive(true);
                    var unitConfig = _db.GetConfig<UnitConfig>(_state.Epoch, (EntityType)i);
                    if (unitConfig != null) {
                        var sprite = unitConfig.prefab.GetComponentInChildren<SpriteRenderer>().sprite;
                        var button = new ButtonData() {
                            image = sprite,
                            badge = data.empty.badge,
                            text = unitConfig.cost.ToString(),
                        };
                        actionButtons[i].SetData(button);
                    } else {
                        actionButtons[i].gameObject.SetActive(false);
                    }
                }

                break;
            case PlayerInputController.MenuState.BuyTurret:
                for (int i = 0; i < actionButtons.Length; i++) {
                    actionButtons[i].gameObject.SetActive(true);
                    var turretConfig = _db.GetConfig<TurretConfig>(_state.Epoch, (EntityType)i);

                    if (turretConfig != null) {
                        var sprite = turretConfig.prefab.GetComponentInChildren<SpriteRenderer>().sprite;
                        var button = new ButtonData() {
                            image = sprite,
                            badge = data.empty.badge,
                            text = turretConfig.cost.ToString(),
                        };

                        actionButtons[i].SetData(button);
                    } else {
                        actionButtons[i].gameObject.SetActive(false);
                    }
                }

                break;
            case PlayerInputController.MenuState.BuildTurret:
                for (int i = 0; i < actionButtons.Length; i++) {
                    var slot = _state.Slots[i];
                    if (!slot.IsActive) {
                        actionButtons[i].gameObject.SetActive(false);
                        continue;
                    }
                    actionButtons[i].gameObject.SetActive(true);
                    if (!slot.HasTurret) {
                        actionButtons[i].SetData(data.empty);
                        continue;
                    }

                    var turretConfig = _db.GetConfig<TurretConfig>(slot.TurretConfigId);

                    if (turretConfig != null) {
                        var sprite = turretConfig.prefab.GetComponentInChildren<SpriteRenderer>().sprite;
                        var button = new ButtonData {
                            image = sprite,
                            badge = data.empty.badge,
                            text = turretConfig.cost.ToString(),
                        };

                        actionButtons[i].SetData(button);
                    } else {
                        actionButtons[i].gameObject.SetActive(false);
                    }
                }

                break;
            case PlayerInputController.MenuState.DestroyTurret:
                for (int i = 0; i < actionButtons.Length; i++) {
                    var slot = _state.Slots[i];
                    if (!slot.IsActive) {
                        actionButtons[i].gameObject.SetActive(false);
                        continue;
                    }
                    actionButtons[i].gameObject.SetActive(true);
                    if (!slot.HasTurret) {
                        actionButtons[i].SetData(data.empty);
                        continue;
                    }

                    var turretConfig = _db.GetConfig<TurretConfig>(slot.TurretConfigId);

                    if (turretConfig != null) {
                        var sprite = turretConfig.prefab.GetComponentInChildren<SpriteRenderer>().sprite;
                        var button = new ButtonData {
                            image = sprite,
                            badge = data.empty.badge,
                            text = turretConfig.cost.ToString(),
                        };

                        actionButtons[i].SetData(button);
                    } else {
                        actionButtons[i].gameObject.SetActive(false);
                    }
                }

                break;
        }
    }
}