using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace HighlightPlus {

    public static class InputProxy {

#if ENABLE_INPUT_SYSTEM
        public static Vector3 mousePosition {
            get {
                return Mouse.current.position.ReadValue();
            }
        }

        public static bool GetMouseButtonDown(int buttonIndex) {
            switch (buttonIndex) {
                case 1: return Mouse.current.rightButton.wasPressedThisFrame;
                case 2: return Mouse.current.middleButton.wasPressedThisFrame;
                default: return Mouse.current.leftButton.wasPressedThisFrame;
            }
        }

        public static int touchCount { get { return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count; } }

        public static int GetFingerIdFromTouch(int touchIndex) {
            UnityEngine.InputSystem.EnhancedTouch.Touch touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[touchIndex];
            return touch.finger.index;
        }

        public static bool GetKeyDown(string name) {
            return ((KeyControl)Keyboard.current[name]).wasPressedThisFrame;
        }

#else
        public static Vector3 mousePosition {
            get {
                return Input.mousePosition;
            }
        }

        public static bool GetMouseButtonDown(int buttonIndex) {
            return Input.GetMouseButtonDown(buttonIndex);
        }

        public static int touchCount {
            get { return Input.touchCount; }
        }

        public static int GetFingerIdFromTouch(int touchIndex) {
            return Input.GetTouch(touchIndex).fingerId;
        }

        public static bool GetKeyDown(string name) {
            return Input.GetKeyDown(name);
        }

#endif

    }
}
