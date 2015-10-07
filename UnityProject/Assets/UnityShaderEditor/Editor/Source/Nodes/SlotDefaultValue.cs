using System;
using UnityEngine;

namespace UnityEditor.Graphs.Material
{
/*	public static class SlotExtensions
	{
		public static bool Editable (this Slot slot)
		{
			var node = slot.node as BaseMaterialNode;
			if (node == null)
				return false;
			var properties = node[slot];
			if (properties != null)
				return properties.editable;
			return false;
		}

		public static bool Removable (this Slot slot)
		{
			var node = slot.node as BaseMaterialNode;
			if (node == null)
				return false;
			var properties = node[slot];
			if (properties != null)
				return properties.removable;
			return false;
		}

		public static bool SupportsDefault (this Slot slot)
		{
			var node = slot.node as BaseMaterialNode;
			if (node == null)
				return true;
			var properties = node[slot];
			if (properties != null)
				return properties.supportsDefault;
			return true;
		}

		public static ShaderProperty GetDefaultValue (this Slot slot)
		{
			var node = slot.node as BaseMaterialNode;
			if (node == null)
				return null;
			var properties = node[slot];
			if (properties != null && properties.supportsDefault)
				return properties.defaultValue;
			return null;
		}

		public static void SetDefaultValue (this Slot slot, ShaderProperty value)
		{
			var node = slot.node as BaseMaterialNode;
			if (node == null)
				return;
			var properties = node[slot];
			if (properties != null && properties.supportsDefault)
				slot.SetDefaultValueForSlot (value);
		}

		public static SlotProperties GetSlotProperties (this Slot slot)
		{
			var node = slot.node as BaseMaterialNode;
			if (node == null)
				return null;
			int index = node.m_SlotPropertiesIndexes.FindIndex (x => x == slot.name);
			if (index > -1)
				return node.m_SlotProperties[index];
			return null;
		}

		public static void SetDefaultValueForSlot (this Slot slot, ShaderProperty value)
		{
			var node = slot.node as BaseMaterialNode;
			if (node == null)
				return;
			var properties = node[slot];
			if (properties != null && properties.supportsDefault) {
				Object.DestroyImmediate (properties.defaultValue, true);
				properties.defaultValue = value;
			} else {
				properties = new SlotProperties() {defaultValue = value};
				slot.SetPropertiesForSlot (properties);
			}
		}

		public static void SetPropertiesForSlot (this Slot slot, SlotProperties property)
		{
			var node = slot.node as BaseMaterialNode;
			if (node == null)
				return;
			int index = node.m_SlotPropertiesIndexes.FindIndex (x => x == slot.name);
			var oldValue = index > -1 ? node.m_SlotProperties[index] : null;
			if (oldValue != null && oldValue.defaultValue != null)
				Object.DestroyImmediate (oldValue.defaultValue, true);

			if (property == null && index > -1)
			{
				if (oldValue != null)
					oldValue.defaultValue = null;
				node.m_SlotPropertiesIndexes.RemoveAt (index);
				node.m_SlotProperties.RemoveAt (index);
			}
			else
			{
				if (index > -1) {
					node.m_SlotProperties[index] = property;
					if (node is IPropertyNode)
						((IPropertyNode)node).UpdateProperty ();
				}
				else
				{
					node.m_SlotPropertiesIndexes.Add (slot.name);
					node.m_SlotProperties.Add (property);
					if (node is IPropertyNode)
						((IPropertyNode)node).UpdateProperty ();
				}
			}
		}

		public static void ClearDefaultValue (this Slot slot)
		{
			slot.SetDefaultValueForSlot (null);
		}
	
	}*/

	[Serializable]
	public enum DefaultSlotType
	{
		Vector,
		Texture
	}

	[Serializable]
	public class SlotDefaultValue : IGenerateProperties
	{
		[SerializeField]
		private Vector4 m_DefaultVector;
		public Vector4 defaultValue
		{
			get { return m_DefaultVector; }
		}

		[SerializeField]
		private bool m_Editable;
		public bool editable {
			get { return m_Editable; }
		}

		[SerializeField]
		private string m_SlotName;
		public string slotName
		{
			get { return m_SlotName; }
		}

		[SerializeField]
		private BaseMaterialNode m_Node;
		public string nodeName
		{
			get { return m_Node.GetOutputVariableNameForNode(); }
		}

		public SlotDefaultValue (Vector4 value, BaseMaterialNode theNode, string theSlotName, bool isEditable)
		{
			m_Editable = isEditable;
			m_SlotName = theSlotName;
			m_Node = theNode;

			m_DefaultVector = value;
		}

		public string inputName
		{
			get { return nodeName + "_" + slotName; }
		}
	
		public void GeneratePropertyBlock(PropertyGenerator visitor, GenerationMode generationMode)
		{
			if (!generationMode.IsPreview ())
				return;

			visitor.AddShaderProperty(new VectorPropertyChunk (inputName, inputName, m_DefaultVector, false));
		}

		public void GeneratePropertyUsages(ShaderGenerator visitor, GenerationMode generationMode)
		{
			if (!generationMode.IsPreview ())
				return;

			visitor.AddShaderChunk("float4 " + inputName + ";", true); 
		}
		
		public string GetDefaultValue (GenerationMode generationMode)
		{
			if (!generationMode.IsPreview ())
				return "half4 (" + m_DefaultVector.x + "," + m_DefaultVector.y + "," + m_DefaultVector.z + "," + m_DefaultVector.w + ")";
			else
				return inputName;
		}

		public bool OnGUI ()
		{
			EditorGUI.BeginChangeCheck();
			m_DefaultVector = EditorGUILayout.Vector4Field ("Value", m_DefaultVector);
			return EditorGUI.EndChangeCheck ();

		}
	}
}