using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputController : MonoBehaviour {
    [SerializeField] private Button upgradeBaseButton;
    [SerializeField] private Button specialWeaponButton;
    [SerializeField] private Button[] actionButtons;
    [SerializeField] private Button backButton;

    public event Action<MenuState> OnMenuStateChanged;
    private MenuState _menu = MenuState.Main;

    private PlayerInputListener _listener;
    private int _turretIndex;

    public enum MenuState {
        Main,
        SpawnUnit,
        BuySlot,
        BuyTurret,
        BuildTurret,
        DestroyTurret
    }

    public void Initialize(PlayerInputListener listener) {
        _listener = listener;
    }

    private void OnEnable() {
        upgradeBaseButton.onClick.AddListener(() => HandleInput(new PlayerInput.UpgradeBase()));
        specialWeaponButton.onClick.AddListener(() => HandleInput(new PlayerInput.SpecialWeapon()));
        for (int i = 0; i < actionButtons.Length; i++) {
            int index = i; // Capture the current index
            actionButtons[i].onClick.AddListener(() => HandleAction(index));
        }

        backButton.onClick.AddListener(HandleBack);
    }

    private void OnDisable() {
        upgradeBaseButton.onClick.RemoveAllListeners();
        specialWeaponButton.onClick.RemoveAllListeners();
        foreach (var button in actionButtons) {
            button.onClick.RemoveAllListeners();
        }

        backButton.onClick.RemoveAllListeners();
    }

    private void HandleBack() {
        if (_menu == MenuState.BuildTurret) {
            _menu = MenuState.BuyTurret;
        } else {
            _menu = MenuState.Main;
        }

        OnMenuStateChanged?.Invoke(_menu);
    }

    private void HandleAction(int buttonIndex) {
        switch (_menu) {
            case MenuState.Main:
                switch (buttonIndex) {
                    case 0: _menu = MenuState.SpawnUnit; break;
                    case 1: _menu = MenuState.BuySlot; break;
                    case 2: _menu = MenuState.BuyTurret; break;
                    case 3: _menu = MenuState.DestroyTurret; break;
                }

                OnMenuStateChanged?.Invoke(_menu);
                break;
            case MenuState.SpawnUnit:
                HandleInput(new PlayerInput.SpawnUnit(buttonIndex));
                break;
            case MenuState.BuySlot:
                HandleInput(new PlayerInput.BuySlot(buttonIndex));
                break;
            case MenuState.BuyTurret:
                _turretIndex = buttonIndex;
                _menu = MenuState.BuildTurret;
                OnMenuStateChanged?.Invoke(_menu);
                break;
            case MenuState.BuildTurret:
                HandleInput(new PlayerInput.BuildTurret(_turretIndex, buttonIndex));
                break;
            case MenuState.DestroyTurret:
                HandleInput(new PlayerInput.DestroyTurret(buttonIndex));
                break;
        }
    }

    private void HandleInput(PlayerInput playerInput) {
        _listener?.OnPlayerInput(playerInput);
    }
}

public interface PlayerInputListener {
    void OnPlayerInput(PlayerInput input);
}