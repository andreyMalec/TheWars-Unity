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
    private MenuState Menu {
        get => _menu;
        set {
            _menu = value;
            OnMenuStateChanged?.Invoke(_menu);
        }
    }

    private PlayerInputListener _listener;
    private int _turretIndex;

    public enum MenuState {
        Main,
        SpawnUnit,
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
        if (Menu == MenuState.BuildTurret) {
            Menu = MenuState.BuyTurret;
        } else {
            Menu = MenuState.Main;
        }
    }

    private void HandleAction(int buttonIndex) {
        switch (Menu) {
            case MenuState.Main:
                switch (buttonIndex) {
                    case 0: Menu = MenuState.SpawnUnit; break;
                    case 1: HandleInput(new PlayerInput.BuySlot()); break;
                    case 2: Menu = MenuState.BuyTurret; break;
                    case 3: Menu = MenuState.DestroyTurret; break;
                }

                break;
            case MenuState.SpawnUnit:
                HandleInput(new PlayerInput.SpawnUnit(buttonIndex));
                break;
            case MenuState.BuyTurret:
                _turretIndex = buttonIndex;
                Menu = MenuState.BuildTurret;
                break;
            case MenuState.BuildTurret:
                HandleInput(new PlayerInput.BuildTurret(_turretIndex, buttonIndex));
                Menu = MenuState.Main;
                break;
            case MenuState.DestroyTurret:
                HandleInput(new PlayerInput.DestroyTurret(buttonIndex));
                Menu = MenuState.Main;
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