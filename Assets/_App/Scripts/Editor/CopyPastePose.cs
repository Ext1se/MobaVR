using System.Collections;
using System.Collections.Generic;
using System.Text;
using MobaVR;
using UnityEngine;
using UnityEditor;

public class CopyPastePose : MonoBehaviour
{
    [System.Serializable]
    public class TransformInfo
    {
        public string name;
        public Vector3 rotation;
        public Vector3 position;
        public Vector3 scale;
        public TransformInfo[] children;

        public void BuildString(StringBuilder sb)
        {
            sb.Append(string.Format("{0}|{1};{2};{3};{4};{5};{6};{7};{8};{9}",
                                    name,
                                    rotation.x, rotation.y, rotation.z,
                                    position.x, position.y, position.z,
                                    scale.x, scale.y, scale.z));
            if (children != null && children.Length > 0)
            {
                sb.Append("\n(\n");
                for (int i = 0; i < children.Length; i++)
                {
                    children[i].BuildString(sb);
                }
                sb.Append(")");
            }

            sb.Append("\n");
        }

        public static TransformInfo FromString(string[] lines, ref int idx)
        {
            string s = lines[idx];
            int delim = s.IndexOf('|');
            var result = new TransformInfo();
            result.name = s.Substring(0, delim);
            string[] fields = s.Substring(delim + 1).Split(new char[] { ';' });
            float.TryParse(fields[0], out result.rotation.x);
            float.TryParse(fields[1], out result.rotation.y);
            float.TryParse(fields[2], out result.rotation.z);
            float.TryParse(fields[3], out result.position.x);
            float.TryParse(fields[4], out result.position.y);
            float.TryParse(fields[5], out result.position.z);
            float.TryParse(fields[6], out result.scale.x);
            float.TryParse(fields[7], out result.scale.y);
            float.TryParse(fields[8], out result.scale.z);
            idx++;
            if (idx >= lines.Length || lines[idx] != "(")
            {
                return result;
            }

            var chilluns = new List<TransformInfo>();
            idx++;
            while (idx < lines.Length && lines[idx] != ")")
            {
                chilluns.Add(FromString(lines, ref idx));
            }

            idx++;
            result.children = chilluns.ToArray();
            return result;
        }
    }
    
    [MenuItem("MobaVR/Animation/Copy Pose")]
    static void CopyPose()
    {
        Transform obj = Selection.activeTransform;
        TransformInfo info = GetInfo(obj);
        StringBuilder sb = new StringBuilder();
        info.BuildString(sb);
        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log("Copied pose of " + obj.name);
    }

    [MenuItem("MobaVR/Animation/Copy Pose", true)]
    static bool ValidateCopyPose()
    {
        return Selection.activeTransform != null;
    }

    [MenuItem("MobaVR/Animation/Set Male A Pose")]
    static void SetMaleAPose()
    {
        Transform obj = Selection.activeTransform;
        var data = PoseUtils.MaleAPose;
        PastePose(obj, data);
    }
    
    [MenuItem("MobaVR/Animation/Set Female A Pose")]
    static void SetFemaleAPose()
    {
        Transform obj = Selection.activeTransform;
        var data = PoseUtils.FemaleAPose;
        PastePose(obj, data);
    }

    [MenuItem("MobaVR/Animation/Paste Pose")]
    static void PastePose()
    {
        Transform obj = Selection.activeTransform;
        var data = GUIUtility.systemCopyBuffer;
        PastePose(obj, data);
    }
    
    static void PastePose(Transform obj, string data)
    {
        int idx = 0;
        var info = TransformInfo.FromString(data.Split(new char[] { '\n' }), ref idx);
        ApplyInfo(obj, info);
        Debug.Log("Pasted pose onto " + obj.name);
    }

    [MenuItem("MobaVR/Animation/Paste Pose", true)]
    static bool ValidatePastePose()
    {
        return Selection.activeTransform != null && !string.IsNullOrEmpty(GUIUtility.systemCopyBuffer);
    }

    public static TransformInfo GetInfo(Transform t)
    {
        TransformInfo result = new TransformInfo();
        result.name = t.name;
        result.rotation = t.localRotation.eulerAngles;
        result.position = t.localPosition;
        result.scale = t.localScale;
        if (t.childCount > 0)
        {
            result.children = new TransformInfo[t.childCount];
            for (int i = 0; i < t.childCount; i++)
            {
                result.children[i] = GetInfo(t.GetChild(i));
            }
        }

        return result;
    }

    static void ApplyInfo(Transform t, TransformInfo info)
    {
        t.localRotation = Quaternion.Euler(info.rotation);
        t.localPosition = info.position;
        t.localScale = info.scale;
        if (info.children == null)
        {
            return;
        }
        foreach (TransformInfo childInfo in info.children)
        {
            Transform tchild = t.Find(childInfo.name);
            if (tchild != null)
            {
                ApplyInfo(tchild, childInfo);
            }
            else
            {
                Debug.Log("Couldn't find child " + childInfo.name + " of " + t.name, t.gameObject);
            }
        }
    }
}