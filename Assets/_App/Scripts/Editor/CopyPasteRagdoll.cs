using ExitGames.Client.Photon.StructWrapping;
using MobaVR;
using UnityEditor;
using UnityEngine;

public class CopyPasteRagdoll
{
    [MenuItem("MobaVR/Character Components/Copy and Paste Ragdolls")]
    public static void CopyAndPaste()
    {
        Transform[] selections = Selection.transforms;
        Transform mainSelection = Selection.activeTransform;
        Rigidbody[] mainRigidbodies = mainSelection.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < selections.Length; i++)
        {
            Transform selection = selections[i];
            if (selection == mainSelection)
            {
                continue;
            }

            Rigidbody[] copyRigidbodies = selection.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody copyRigidbody in copyRigidbodies)
            {
                foreach (Rigidbody mainRigidbody in mainRigidbodies)
                {
                    if (mainRigidbody.name.Equals(copyRigidbody.name))
                    {
                        if (!copyRigidbody.TryGetComponent(out HitCollider hitCollider))
                        {
                            copyRigidbody.tag = mainRigidbody.tag;
                            copyRigidbody.gameObject.AddComponent<HitCollider>();
                        }

                        if (copyRigidbody.TryGetComponent(out Collider copyCollider)
                            && mainRigidbody.TryGetComponent(out Collider mainCollider))
                        {
                            EditorUtility.CopySerialized(mainCollider, copyCollider);
                        }

                        if (copyRigidbody.TryGetComponent(out CharacterJoint copyCharacterJoint)
                            && mainRigidbody.TryGetComponent(out CharacterJoint mainCharacterJoint))
                        {
                            Rigidbody connectedBody = copyCharacterJoint.connectedBody;
                            EditorUtility.CopySerialized(mainCharacterJoint, copyCharacterJoint);
                            if (connectedBody != null)
                            {
                                copyCharacterJoint.connectedBody = connectedBody;
                            }
                        }

                        EditorUtility.CopySerialized(mainRigidbody, copyRigidbody);

                        break;
                    }
                }
            }
        }
    }

    [MenuItem("MobaVR/Character Components/Copy and Paste all components")]
    public static void CopyAndPasteComponents()
    {
        Transform[] selections = Selection.transforms;
        Transform mainSelection = Selection.activeTransform;
        //Transform mainSelection = Selection.transforms[0]; //Не работает, так как 0 всегда разный
        Transform[] mainGameObjects = mainSelection.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < selections.Length; i++)
        {
            Transform selection = selections[i];

            if (selection == mainSelection)
            {
                continue;
            }

            Transform[] childrenSelection = selection.GetComponentsInChildren<Transform>(true);
            foreach (Transform copyGameObject in childrenSelection)
            {
                Transform mainGameObject = null;
                foreach (Transform findMainGameObject in mainGameObjects)
                {
                    if (findMainGameObject.name.Equals(copyGameObject.name))
                    {
                        mainGameObject = findMainGameObject;
                        break;
                    }
                }

                if (mainGameObject != null)
                {
                    if (mainGameObject.TryGetComponent(out HitCollider mainHitCollider))
                    {
                        copyGameObject.tag = mainGameObject.tag;
                        if (!copyGameObject.TryGetComponent(out HitCollider selectedHitCollider))
                        {
                            selectedHitCollider = copyGameObject.gameObject.AddComponent<HitCollider>();
                        }

                        selectedHitCollider.enabled = mainHitCollider.enabled;
                    }

                    if (mainGameObject.TryGetComponent(out Rigidbody mainRigidbody))
                    {
                        if (!copyGameObject.TryGetComponent(out Rigidbody selectedRigidbody))
                        {
                            selectedRigidbody = copyGameObject.gameObject.AddComponent<Rigidbody>();
                        }

                        EditorUtility.CopySerialized(mainRigidbody, selectedRigidbody);
                    }

                    if (mainGameObject.TryGetComponent(out Collider mainCollider))
                    {
                        if (!copyGameObject.TryGetComponent(out Collider selectedCollider))
                        {
                            selectedCollider = mainCollider switch
                            {
                                BoxCollider => copyGameObject.gameObject.AddComponent<BoxCollider>(),
                                SphereCollider => copyGameObject.gameObject.AddComponent<SphereCollider>(),
                                CapsuleCollider => copyGameObject.gameObject.AddComponent<CapsuleCollider>(),
                                _ => selectedCollider
                            };
                        }

                        EditorUtility.CopySerialized(mainCollider, selectedCollider);
                        selectedCollider.enabled = mainCollider.enabled;
                    }

                    if (mainGameObject.TryGetComponent(out CharacterJoint mainCharacterJoint))
                    {
                        if (!copyGameObject.TryGetComponent(out CharacterJoint selectedCharacterJoint))
                        {
                            selectedCharacterJoint = copyGameObject.gameObject.AddComponent<CharacterJoint>();
                        }

                        EditorUtility.CopySerialized(mainCharacterJoint, selectedCharacterJoint);
                        if (mainCharacterJoint.connectedBody != null)
                        {
                            Rigidbody mainConnectedBody = mainCharacterJoint.connectedBody;
                            //Transform copyConnectedTransform = copyGameObject.Find(mainConnectedBody.transform.name);
                            Transform copyConnectedTransform = null;
                            
                            foreach (Transform findCopyGameObject in childrenSelection)
                            {
                                if (findCopyGameObject.name.Equals(mainConnectedBody.name))
                                {
                                    copyConnectedTransform = findCopyGameObject;
                                    break;
                                }
                            }
                            
                            if (copyConnectedTransform != null &&
                                copyConnectedTransform.TryGetComponent(out Rigidbody copyConnectedBody))
                            {
                                selectedCharacterJoint.connectedBody = copyConnectedBody;
                            }
                        }
                    }
                }
            }
        }
    }


    /*
    [MenuItem("MobaVR/RagDoll/Copy and Paste", true)]
    static bool ValidateCopyPose()
    {
        return Selection.transforms is { Length: > 1 };
    }
    */
}