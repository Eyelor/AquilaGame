using System;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Separate class to handle platform-specific cursor movement
#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;

public static class WindowsCursor
{
    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
}
#endif

public class GameInputSystem : PersistentSingleton<GameInputSystem>
{
    public event EventHandler OnFullMapAction;
    public event EventHandler OnEquipmentAction;
    public event EventHandler OnInteractionAction;
    public event EventHandler OnSprintAction;
    public event EventHandler OnJumpAction;
    public event EventHandler OnAttackAction;
    public event EventHandler OnTakeWeapponAction;

    private PlayerInputActions _playerInputActions;

    private bool isSprintKeyPressed;

    private Vector2 currentCursorPosition;
    private float mouseSensitivity;
#if UNITY_EDITOR
    private bool isGameViewFocused = false; // Tracks if Game view is focused
#endif
    private bool isCursorLocked = false; // Tracks if the cursor should be locked
    private bool forceCursorLockedAndInvisible = false;

    public enum Binding
    {
        openMap,
        //placeTag,
        //swipeLeft,
        //swipeUp,
        //swipeRight,
        //swipeDown,
        //aim,
        attack,
        //reload,
        weapon1,
        //weapon2,
        //walk,
        run,
        interact,
        stepForward,
        stepBack,
        stepLeft,
        stepRight,
        jump,
        openInventory,
        //changeCardForward,
        //changeCardBack,
        //characterEquipment,
        //shipHold,
        //characterStatistics,
        //sortItems,
    }

    protected override void Awake()
    {
        base.Awake();

        _playerInputActions = new PlayerInputActions();

        LoadBindingsFromJson();

        isSprintKeyPressed = false;

        _playerInputActions.Player.Enable();

        _playerInputActions.Player.FullMap.performed += FullMap_performed;
        _playerInputActions.Player.Equipment.performed += Equipment_performed;
        _playerInputActions.Player.Interaction.performed += Interaction_performed;
        _playerInputActions.Player.Sprint.performed += Sprint_performed;
        _playerInputActions.Player.Sprint.canceled += Sprint_canceled;
        _playerInputActions.Player.Jump.performed += Jump_performed;
        _playerInputActions.Player.Attack.performed += Attack_performed;
        _playerInputActions.Player.TakeWeappon.performed += TakeWeappon_performed;
    }

    private void Start()
    {
        ConfigureCursor();
    }

    private void Update()
    {
#if UNITY_EDITOR
        // Handle Escape key to unlock cursor
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            UpdateCursorLockState(false);
        }

        // Check if the Game view is focused in the Editor
        CheckGameViewFocus();

        // Apply changes only if the Game view is focused and cursor is locked
        if (isGameViewFocused && isCursorLocked)
        {
            HandleCursorMovement();
        }
#else
        // Apply changes only if the cursor is locked
        if (isCursorLocked)
        {
            HandleCursorMovement();
        }
#endif
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    public int GetOnJumpActionSubscribersCount()
    {
        return OnJumpAction != null ? OnJumpAction.GetInvocationList().Length : 0;
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttackAction?.Invoke(this, EventArgs.Empty);
    }

    public int GetOnAttackActionSubscribersCount()
    {
        return OnAttackAction != null ? OnAttackAction.GetInvocationList().Length : 0;
    }

    private void TakeWeappon_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnTakeWeapponAction?.Invoke(this, EventArgs.Empty);
    }

    public int GetOnTakeWeapponActionSubscribersCount()
    {
        return OnTakeWeapponAction != null ? OnTakeWeapponAction.GetInvocationList().Length : 0;
    }

    private void Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isSprintKeyPressed = true;
        OnSprintAction?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isSprintKeyPressed = false;
    }

    public bool IsSprintKeyPressed()
    {
        return isSprintKeyPressed;
    }

    public int GetOnSprintActionSubscribersCount()
    {
        return OnSprintAction != null ? OnSprintAction.GetInvocationList().Length : 0;
    }

    private void FullMap_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnFullMapAction?.Invoke(this, EventArgs.Empty);
    }

    public int GetOnFullMapActionSubscribersCount()
    {
        return OnFullMapAction != null ? OnFullMapAction.GetInvocationList().Length : 0;
    }

    private void Equipment_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnEquipmentAction?.Invoke(this, EventArgs.Empty);
    }

    public int GetOnEquipmentActionSubscribersCount()
    {
        return OnEquipmentAction != null ? OnEquipmentAction.GetInvocationList().Length : 0;
    }

    private void Interaction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractionAction?.Invoke(this, EventArgs.Empty);
    }

    public int GetOnInteractionActionSubscribersCount()
    {
        return OnInteractionAction != null ? OnInteractionAction.GetInvocationList().Length : 0;
    }

    public Vector2 GetMovementVectorNormalized()
    {
        return _playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.openMap:
                return _playerInputActions.Player.FullMap.bindings[0].ToDisplayString();
            case Binding.attack:
                return _playerInputActions.Player.Attack.bindings[0].ToDisplayString();
            case Binding.weapon1:
                return _playerInputActions.Player.TakeWeappon.bindings[0].ToDisplayString();
            case Binding.run:
                return _playerInputActions.Player.Sprint.bindings[0].ToDisplayString();
            case Binding.interact:
                return _playerInputActions.Player.Interaction.bindings[0].ToDisplayString();
            case Binding.stepForward:
                return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.stepBack:
                return _playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.stepLeft:
                return _playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.stepRight:
                return _playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.jump:
                return _playerInputActions.Player.Jump.bindings[0].ToDisplayString();
            case Binding.openInventory:
                return _playerInputActions.Player.Equipment.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        _playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch(binding)
        {
            default:
            case Binding.openMap:
                inputAction = _playerInputActions.Player.FullMap;
                bindingIndex = 0;
                break;
            case Binding.attack:
                inputAction = _playerInputActions.Player.Attack;
                bindingIndex = 0;
                break;
            case Binding.weapon1:
                inputAction = _playerInputActions.Player.TakeWeappon;
                bindingIndex = 0;
                break;
            case Binding.run:
                inputAction = _playerInputActions.Player.Sprint;
                bindingIndex = 0;
                break;
            case Binding.interact:
                inputAction = _playerInputActions.Player.Interaction;
                bindingIndex = 0;
                break;
            case Binding.stepForward:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.stepBack:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.stepLeft:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.stepRight:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.jump:
                inputAction = _playerInputActions.Player.Jump;
                bindingIndex = 0;
                break;
            case Binding.openInventory:
                inputAction = _playerInputActions.Player.Equipment;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                _playerInputActions.Player.Enable();
                onActionRebound();
            })
            .Start();
    }

    public void ResetBindings()
    {
        _playerInputActions.Player.Disable();
        _playerInputActions.RemoveAllBindingOverrides();
        _playerInputActions.Player.Enable();
    }

    public void SaveBindingsToJson()
    {
        SettingsData settingsData = SaveManager.Instance.LoadSettingsData();

        if (settingsData != null)
        {
            settingsData.keys.INPUT_SYSTEM_BINDING_OVERRIDES = _playerInputActions.SaveBindingOverridesAsJson();
            SaveManager.Instance.SaveSettingsData(settingsData);
        }
    }

    public void LoadBindingsFromJson()
    {
        SettingsData settingsData = SaveManager.Instance.LoadSettingsData();

        if (settingsData != null)
        {
            if (!string.IsNullOrEmpty(settingsData.keys.INPUT_SYSTEM_BINDING_OVERRIDES))
            {
                _playerInputActions.LoadBindingOverridesFromJson(settingsData.keys.INPUT_SYSTEM_BINDING_OVERRIDES);
            }
        }
    }

    private void ConfigureCursor()
    {
        var settingsData = SaveManager.Instance.LoadSettingsData();

        if (settingsData != null)
        {
            mouseSensitivity = settingsData.controls.mouseSensitivity;
        }
        else
        {
            mouseSensitivity = 1.0f;
        }

        // Initialize cursor position to the current system cursor position
        currentCursorPosition = Mouse.current.position.ReadValue();

#if UNITY_EDITOR
        // Set initial cursor state
        UpdateCursorLockState(false); // Start with unlocked state for the editor
#else
        UpdateCursorLockState(true); // Start with locked state for the build
#endif
    }

    public void UpdateMouseSensitivity()
    {
        var settingsData = SaveManager.Instance.LoadSettingsData();

        if (settingsData != null)
        {
            mouseSensitivity = settingsData.controls.mouseSensitivity;
        }
        else
        {
            mouseSensitivity = 1.0f;
        }
    }

#if UNITY_EDITOR
    private void CheckGameViewFocus()
    {
        // Get the currently focused window in the Editor
        var focusedWindow = EditorWindow.focusedWindow;

        bool wasFocused = isGameViewFocused;
        // Check if the focused window is the Game view
        isGameViewFocused = focusedWindow != null && focusedWindow.titleContent.text == "Game";

        // Automatically lock the cursor when the Game view gains focus
        if (isGameViewFocused && !wasFocused)
        {
            UpdateCursorLockState(true);
        }
        else if (!isGameViewFocused && wasFocused)
        {
            UpdateCursorLockState(false);
        }
    }
#endif

    private void SetSystemCursorPosition(Vector2 position)
    {
        // Use platform-specific API for setting cursor position
#if UNITY_STANDALONE_WIN
        WindowsCursor.SetCursorPos((int)position.x, (int)position.y);
#endif
    }

    private void HandleCursorMovement()
    {
        if (forceCursorLockedAndInvisible) return; // Skip movement handling when forced

        // Get mouse delta movement
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Apply sensitivity scaling and invert Y-axis
        mouseDelta.x *= mouseSensitivity;
        mouseDelta.y *= -mouseSensitivity; // Invert Y-axis for proper upward movement

        // Update cursor position based on the game's resolution and screen scaling
        currentCursorPosition += mouseDelta;

#if UNITY_EDITOR
#else
        currentCursorPosition.x = Mathf.Clamp(currentCursorPosition.x, 0, Screen.width - 1);
        currentCursorPosition.y = Mathf.Clamp(currentCursorPosition.y, 0, Screen.height - 1);
#endif

        // Ustaw now¹ pozycjê kursora (tylko w Play Mode)
        if (Application.isPlaying)
        {
            SetSystemCursorPosition(currentCursorPosition);
        }
    }

    public void SetForceCursorLockedAndInvisible(bool forceLock)
    {
        forceCursorLockedAndInvisible = forceLock;
        UpdateCursorLockState(isCursorLocked); // Reapply current state
    }

    private void UpdateCursorLockState(bool lockState)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return; // Dzia³a tylko w Play Mode
#endif

        isCursorLocked = lockState;

        if (forceCursorLockedAndInvisible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return; // Skip default behavior
        }

        if (lockState)
        {
            Cursor.lockState = CursorLockMode.Confined; // Restrict to game window
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Free the cursor
            Cursor.visible = true;

#if UNITY_EDITOR
            // Focus the main Unity Editor window by opening a menu
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
#endif
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (forceCursorLockedAndInvisible)
        {
            UpdateCursorLockState(true);
            return; // Maintain forced lock state
        }

#if UNITY_EDITOR
        // Automatically lock the cursor when the game regains focus
        if (hasFocus)
        {
            UpdateCursorLockState(true);
        }
#else
        UpdateCursorLockState(hasFocus);
#endif
    }

    protected override void OnDestroy()
    {
        _playerInputActions.Player.FullMap.performed -= FullMap_performed;
        _playerInputActions.Player.Equipment.performed -= Equipment_performed;
        _playerInputActions.Player.Interaction.performed -= Interaction_performed;
        _playerInputActions.Player.Sprint.performed -= Sprint_performed;
        _playerInputActions.Player.Sprint.canceled -= Sprint_canceled;
        _playerInputActions.Player.Jump.performed -= Jump_performed;
        _playerInputActions.Player.Attack.performed -= Attack_performed;
        _playerInputActions.Player.TakeWeappon.performed -= TakeWeappon_performed;

        _playerInputActions.Dispose();

        base.OnDestroy();
    }
}
