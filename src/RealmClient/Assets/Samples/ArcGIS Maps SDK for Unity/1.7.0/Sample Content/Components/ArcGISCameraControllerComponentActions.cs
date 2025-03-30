// GENERATED AUTOMATICALLY FROM 'Assets/ArcGISMapsSDK/Samples/Components/ArcGISCameraControllerComponentActions.inputactions'

#if ENABLE_INPUT_SYSTEM && USE_INPUT_SYSTEM
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Esri.ArcGISMapsSDK.Samples.Components
{
    public partial class @ArcGISCameraControllerComponentActions: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @ArcGISCameraControllerComponentActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""ArcGISCameraControllerComponentActions"",
    ""maps"": [
        {
            ""name"": ""Move"",
            ""id"": ""2c9bcb99-1de2-4daf-9c89-769175e99ba2"",
            ""actions"": [
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""3761cf8f-23f1-44ca-820e-28b47b3be36f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Forward"",
                    ""type"": ""Button"",
                    ""id"": ""0d592f84-62a7-45eb-9648-8fc7d5dc1d10"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""f2a0ae8a-49b8-40ef-8d59-493e82dab5e0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Up"",
                    ""id"": ""2a57c5e2-e595-4368-957f-b394bc4d26ed"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""456354b5-74f2-4346-a858-b7f7d7239dc6"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""b71bc2a8-b29a-4c10-b0af-61d850da1b6c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""UpAlt"",
                    ""id"": ""36cf9440-7dd9-4660-a769-8a7da129031a"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""9725a33c-9c3b-42e5-8a39-91fca5fc139a"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""beebb3ee-c53e-4920-8b76-6ef1992c1813"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Forward"",
                    ""id"": ""b89e36fd-5c85-4f75-8c31-e9a4dc139a9a"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""9a773c59-f1e2-48cc-a8fb-621375631d19"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""b6829142-784a-4480-99c2-d132fec6405c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""GamepadForward"",
                    ""id"": ""f5cbf31e-60cd-4d74-87ad-9717131a4f7e"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""84fd39a3-94d1-4aad-8c27-182222fb91b8"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f35ca595-fdfe-42e6-b118-a533b491fa23"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""4f7cc5e1-057c-4d37-8ff5-3405bb1e2ed6"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""cd8e3d18-9665-4ca9-86e2-b41ab73eb74a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""d45b1afd-b610-46a7-943a-8585ad34e765"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""GamepadRight"",
                    ""id"": ""b9bdc1e2-41e3-46c3-b086-6d64114ed1ca"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""56013f5b-a7c8-46ed-917c-79b053a77189"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""3ec015cb-f44f-4572-a91e-f2788585fe64"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Look"",
            ""id"": ""03e73249-03ef-4e7b-8a57-e16f5d3efbbb"",
            ""actions"": [
                {
                    ""name"": ""Position"",
                    ""type"": ""PassThrough"",
                    ""id"": ""64e2d4b3-d325-4d4f-a853-2a5c0fc29e68"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9d0ce706-29e0-44a3-b122-2338080db5d0"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""41fa0b14-9303-4605-8575-51aced366e2f"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Move
            m_Move = asset.FindActionMap("Move", throwIfNotFound: true);
            m_Move_Up = m_Move.FindAction("Up", throwIfNotFound: true);
            m_Move_Forward = m_Move.FindAction("Forward", throwIfNotFound: true);
            m_Move_Right = m_Move.FindAction("Right", throwIfNotFound: true);
            // Look
            m_Look = asset.FindActionMap("Look", throwIfNotFound: true);
            m_Look_Position = m_Look.FindAction("Position", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Move
        private readonly InputActionMap m_Move;
        private List<IMoveActions> m_MoveActionsCallbackInterfaces = new List<IMoveActions>();
        private readonly InputAction m_Move_Up;
        private readonly InputAction m_Move_Forward;
        private readonly InputAction m_Move_Right;
        public struct MoveActions
        {
            private @ArcGISCameraControllerComponentActions m_Wrapper;
            public MoveActions(@ArcGISCameraControllerComponentActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Up => m_Wrapper.m_Move_Up;
            public InputAction @Forward => m_Wrapper.m_Move_Forward;
            public InputAction @Right => m_Wrapper.m_Move_Right;
            public InputActionMap Get() { return m_Wrapper.m_Move; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MoveActions set) { return set.Get(); }
            public void AddCallbacks(IMoveActions instance)
            {
                if (instance == null || m_Wrapper.m_MoveActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_MoveActionsCallbackInterfaces.Add(instance);
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @Forward.started += instance.OnForward;
                @Forward.performed += instance.OnForward;
                @Forward.canceled += instance.OnForward;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
            }

            private void UnregisterCallbacks(IMoveActions instance)
            {
                @Up.started -= instance.OnUp;
                @Up.performed -= instance.OnUp;
                @Up.canceled -= instance.OnUp;
                @Forward.started -= instance.OnForward;
                @Forward.performed -= instance.OnForward;
                @Forward.canceled -= instance.OnForward;
                @Right.started -= instance.OnRight;
                @Right.performed -= instance.OnRight;
                @Right.canceled -= instance.OnRight;
            }

            public void RemoveCallbacks(IMoveActions instance)
            {
                if (m_Wrapper.m_MoveActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IMoveActions instance)
            {
                foreach (var item in m_Wrapper.m_MoveActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_MoveActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public MoveActions @Move => new MoveActions(this);

        // Look
        private readonly InputActionMap m_Look;
        private List<ILookActions> m_LookActionsCallbackInterfaces = new List<ILookActions>();
        private readonly InputAction m_Look_Position;
        public struct LookActions
        {
            private @ArcGISCameraControllerComponentActions m_Wrapper;
            public LookActions(@ArcGISCameraControllerComponentActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Position => m_Wrapper.m_Look_Position;
            public InputActionMap Get() { return m_Wrapper.m_Look; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(LookActions set) { return set.Get(); }
            public void AddCallbacks(ILookActions instance)
            {
                if (instance == null || m_Wrapper.m_LookActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_LookActionsCallbackInterfaces.Add(instance);
                @Position.started += instance.OnPosition;
                @Position.performed += instance.OnPosition;
                @Position.canceled += instance.OnPosition;
            }

            private void UnregisterCallbacks(ILookActions instance)
            {
                @Position.started -= instance.OnPosition;
                @Position.performed -= instance.OnPosition;
                @Position.canceled -= instance.OnPosition;
            }

            public void RemoveCallbacks(ILookActions instance)
            {
                if (m_Wrapper.m_LookActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(ILookActions instance)
            {
                foreach (var item in m_Wrapper.m_LookActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_LookActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public LookActions @Look => new LookActions(this);
        public interface IMoveActions
        {
            void OnUp(InputAction.CallbackContext context);
            void OnForward(InputAction.CallbackContext context);
            void OnRight(InputAction.CallbackContext context);
        }
        public interface ILookActions
        {
            void OnPosition(InputAction.CallbackContext context);
        }
    }
}
#endif
