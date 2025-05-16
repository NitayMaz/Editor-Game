using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorValidatorUnity6
{
    [MenuItem("Tools/Animator/Check For Missing Clips")]
    public static void CheckMissingAnimations()
    {
        var guids = AssetDatabase.FindAssets("t:AnimatorController");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            foreach (var layer in controller.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    if (state.state.motion == null)
                    {
                        Debug.LogWarning($"Missing Motion in state '{state.state.name}' of controller '{controller.name}'", controller);
                    }
                }
            }
        }
    }
}