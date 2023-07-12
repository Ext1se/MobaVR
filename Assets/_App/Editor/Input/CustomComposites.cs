using MobaVR.Input;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

#if UNITY_EDITOR
[InitializeOnLoad] 
#endif
public class CustomComposites
{
    static CustomComposites()
    {
        InputSystem.RegisterBindingComposite<UniqueButton>();
        InputSystem.RegisterBindingComposite<CorrectAndWrongButtons>();
        InputSystem.RegisterBindingComposite<TwoButtons>();
        
        InputSystem.RegisterBindingComposite<TwoAnalogButtons>();
        InputSystem.RegisterBindingComposite<ThreeAnalogButtons>();
        InputSystem.RegisterBindingComposite<FourAnalogButtons>();
        
        InputSystem.RegisterBindingComposite<FourButtons>();
        InputSystem.RegisterBindingComposite<ThreeButtons>();
        
        InputSystem.RegisterInteraction<SyncPressInteraction>();
        InputSystem.RegisterInteraction<SyncAnalogInteraction>();
        
        InputSystem.RegisterInteraction<DelayInteraction>();
        InputSystem.RegisterInteraction<HoldInteraction1>();
    }
}