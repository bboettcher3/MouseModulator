using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEditorInternal;

public class MapperDevice : MonoBehaviour {

    [Serializable]
    public struct MapperSignal {
        public enum SignalDirection {
            INCOMING = 0x01,
            OUTGOING = 0x02
        }

        public SignalDirection Direction;
        public string Name;
        public float Min;
        public float Max;
        public float Value;
    }

    public string DeviceName = "UnityMapper";
    public List<MapperSignal> Signals = new List<MapperSignal>();

    private IntPtr dev = IntPtr.Zero;
    private List<IntPtr> sigs = new List<IntPtr>();

    /* Returns 0 if signal not found */
    public float getSignalValue(string signalName) {
        for (int i = 0; i < Signals.Count; ++i) {
            if (String.Equals(Signals[i].Name, signalName)) {
                IntPtr sigVal = mpr.mpr_sig_get_value(sigs[i]);
                if (sigVal != IntPtr.Zero) {
                    unsafe {
                        return *(float*)sigVal;
                    }
                }
            }
        }
        return 0;
    }

    /* Does nothing if signal not found */
    public void setSignalValue(String signalName, float newVal) {
        for (int i = 0; i < Signals.Count; ++i) {
            if (String.Equals(Signals[i].Name, signalName) && Signals[i].Direction == MapperSignal.SignalDirection.OUTGOING) {
                mpr.mpr_sig_set_value(sigs[i], newVal);
                break;
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        dev = mpr.mpr_dev_new(DeviceName.Equals("") ? "UnityMapper" : DeviceName);
        for (int i = 0; i < Signals.Count; ++i) {
            sigs.Add(mpr.mpr_sig_new(dev, (mpr.Direction)Signals[i].Direction, Signals[i].Name, 1, mpr.Type.FLOAT, Signals[i].Min, Signals[i].Max));
        }
    }
    
    // Update is called once per frame
    void Update()
    {   
        mpr.mpr_dev_poll(dev);   
    }
    
    // Normalized update function, called once every physics update (instead of per frame)
    private void FixedUpdate()
    {

        
          
    }
}

[CustomEditor(typeof(MapperDevice))]
public class MapperDeviceEditor : Editor {
    public SerializedProperty deviceName;
    private ReorderableList signalList;
    
    private void OnEnable() {
        deviceName = serializedObject.FindProperty("DeviceName");

        signalList = new ReorderableList(serializedObject, 
                serializedObject.FindProperty("Signals"), 
                true, true, true, true);

        signalList.onAddCallback = (ReorderableList l) => {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Direction").enumValueIndex = 0;
            element.FindPropertyRelative("Name").stringValue = "signal_" + l.index;
            element.FindPropertyRelative("Min").floatValue = 0;
            element.FindPropertyRelative("Max").floatValue = 1;
        };

        signalList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = signalList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            var itemHeight = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 80, EditorGUIUtility.singleLineHeight),
                "Signal Name");
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + itemHeight, 80, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Name"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.x + 82, rect.y, 90, EditorGUIUtility.singleLineHeight),
                "Direction");
            EditorGUI.PropertyField(
                new Rect(rect.x + 82, rect.y + itemHeight, 90, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Direction"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.x + 174, rect.y, 40, EditorGUIUtility.singleLineHeight),
                "Min");
            EditorGUI.PropertyField(
                new Rect(rect.x + 174, rect.y + itemHeight, 40, EditorGUIUtility.singleLineHeight), 
                    element.FindPropertyRelative("Min"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.x + 216, rect.y, 40, EditorGUIUtility.singleLineHeight),
                "Max");
            EditorGUI.PropertyField(
                new Rect(rect.x + 216, rect.y + itemHeight, 40, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Max"), GUIContent.none);
        };

        signalList.elementHeightCallback = (index) => {
            return EditorGUIUtility.singleLineHeight * 2 + 10;
        };

        signalList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Signals");
        };
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(deviceName);
        signalList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}

public class mpr
    {
        public enum Direction {
            INCOMING = 0x01,
            OUTGOING = 0x02,
            ANY = 0x03,
            BOTH = 0x04,
        }
        public enum Type {
            DOUBLE = 0x64,
            FLOAT = 0x66,
            INT32 = 0x69,
            INT64 = 0x68,
        }

        // Define libmapper function for new device
        [DllImport ("mapper")]
        public static extern IntPtr mpr_dev_new(String name_prefix, int graph);
        [DllImport ("mapper")]
        public static extern int mpr_dev_poll(IntPtr dev, int block_ms);
        [DllImport ("mapper")]
        private static extern IntPtr mpr_sig_new(IntPtr parent_dev, Direction dir, String name, int length,
                        Type type, int unit, ref float min, ref float max, int num_inst, int h, int events);
        [DllImport ("mapper")]                
        public static extern IntPtr mpr_sig_get_value(IntPtr signal, int instance, int time);
        [DllImport ("mapper")]
        public static extern void mpr_sig_set_value(IntPtr signal, int id, int len, Type type, ref float val);
        [DllImport ("mapper")]
        private static extern int mpr_sig_reserve_inst(IntPtr sig, int num_reservations, int[] ids, IntPtr[] values);
        [DllImport ("mapper")]
        public static extern void mpr_sig_release_inst(IntPtr sig, int id);
        [DllImport ("mapper")]
        public static extern int mpr_sig_get_inst_is_active(IntPtr sig, int id);
        
        // Function overloads to allow calling the function without unnecessary parameters 
        public static IntPtr mpr_dev_new(String name_prefix) {
            return mpr_dev_new(name_prefix, 0);
        }
        public static int mpr_dev_poll(IntPtr dev) {
            return mpr_dev_poll(dev, 0);
        }
        public static int mpr_sig_reserve_inst(IntPtr sig, int num_reservations) {
            return mpr_sig_reserve_inst(sig, num_reservations, null, null);
        }
        public static IntPtr mpr_sig_new(IntPtr parent_dev, Direction dir, String name, int length, Type type, float min, float max) {
            return mpr_sig_new(parent_dev, dir, name, length, type, 0, ref min, ref max, 0, 0, 0);
        }
        public static IntPtr mpr_sig_get_value(IntPtr signal) {
            return mpr_sig_get_value(signal, 0, 0);
        }
        public static IntPtr mpr_sig_get_value(IntPtr signal, int instance) {
            return mpr_sig_get_value(signal, instance, 0);
        }
        public static void mpr_sig_set_value(IntPtr signal, float val) {
            mpr_sig_set_value(signal, 0, 1, Type.FLOAT, ref val);
        }
    }