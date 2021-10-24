using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public abstract class dfFactoryItemBase : ScriptableObject
{

    public NetPackageUIDType netPackageID
    {
        get { return new NetPackageUIDType(OwnerFactory.netPackageID, iFactoryIndex); }
    }

    [SerializeField]
	public string   strDescription; // description

    [SerializeField]
    public string   strRepresentName; // name for display

	
    [SerializeField]
	public int	    iFactoryIndex;			// factory item 의 index (해당 factory 내에서의 index)
		
	[SerializeField]
	public bool bFold = false;

    [SerializeField]
    public dfFactoryBase OwnerFactory = null;


    // for editor only
#if UNITY_EDITOR
    [System.NonSerialized]
    SerializedObject serializedInfo = null;
#endif

	// [ypqp35 2016/04/19] 팩토리 아이템 줄맞춤 (두 변수를 조율하여 변경해야 합니다)
	[HideInInspector]
	public static float LABEL_WIDTH = 200f;		// 앞부분(변수) 간격
	[HideInInspector]
	public static float LAYOUT_WIDTH = 450f;	// 뒷부분(값) 간격


	public bool CheckValidCopyType( dfFactoryItemBase other )
    {
        if (other == null) return false;
        return GetType() == other.GetType();
    }


    public virtual dfFactoryItemBase CreateClone()
    {
        dfFactoryItemBase result = (dfFactoryItemBase)ScriptableObject.Instantiate(this);
        result.name = result.strRepresentName;

        return result;
    }

    public virtual void DestroyItem()
    {
        GameObject.Destroy(this);
    }


    public virtual void OnLoad()
    {
#if UNITY_EDITOR
		Console.Assert( "이 함수는 호출되면 안 됩니다." );
#endif
	}


    public virtual void TempVersionUp()
    {

    }


	//Security add
	public virtual void SecureInit()
	{
	}
	//Security add

#if UNITY_EDITOR

    public SerializedObject ToSerializedObject()
    {
        return serializedInfo;
    }

    public void BeginGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUIUtility.labelWidth = 150.0f;
    }

    public void EndGUI()
    {
        if (EditorGUI.EndChangeCheck() == true)
        {
            ToSerializedObject().ApplyModifiedProperties();
            EditorUtility.SetDirty(this);
            OnValueChanged();
        }

    }

    public void OnEditorGUI()
    {
        serializedInfo = new SerializedObject( this );

        BeginGUI();
        GUIContents();
        EndGUI();
    }

    public SerializedObject MakeSerialzedObject()
    {
        serializedInfo = new SerializedObject(this);
        return serializedInfo;
    }

    public virtual void GUIContents()
    {
		EditorGUIUtility.labelWidth = LABEL_WIDTH;

		PropertyField( "iFactoryIndex", "index ", GUILayout.Width( LAYOUT_WIDTH ) );
		PropertyField( "strRepresentName", "Represent Name ", GUILayout.Width( LAYOUT_WIDTH ) );
		PropertyField( "strDescription", "Description ", GUILayout.Width( LAYOUT_WIDTH ) );
	}

	public virtual void OnValueChanged()
    {
        name = strRepresentName;
    }

    public bool PropertyField(string propertyname, params GUILayoutOption[] options)
    {
        if ( serializedInfo == null)
        {
            serializedInfo = new SerializedObject(this);
        }

        SerializedProperty serializedprop = serializedInfo.FindProperty(propertyname);

        if ( serializedprop != null )
        {
			return EditorGUILayout.PropertyField( serializedprop, true, options );
		}

		return false;       
    }

	// 팩토리 데이터가 배열 형식일때 'Element 0'을 임의의 string으로 바꾸고 싶을때 사용합니다.
	public void PropertyFieldArray( string propertyname, params GUILayoutOption[] options )
	{
		if( serializedInfo == null )
			serializedInfo = new SerializedObject( this );

		SerializedProperty list = serializedInfo.FindProperty( propertyname );
		if( list != null )
		{
			EditorGUILayout.PropertyField( list );

			EditorGUI.indentLevel += 1;
			for( int i = 0; i < list.arraySize; i++ )
			{
				EditorGUILayout.PropertyField( list.GetArrayElementAtIndex(i), new GUIContent( GetCustomElementName(i) ), options );
			}
			EditorGUI.indentLevel -= 1;
		}
	}

	// override하여 화면에 표시되는 'Element 0' 을 바꿀 수 있습니다.
	public virtual string GetCustomElementName( int n )
	{
		return "Element " + n.ToString();
	}

	public SerializedProperty property(string propertyname)
    {
        if (serializedInfo == null)
        {
            serializedInfo = new SerializedObject(this);
        }

        return serializedInfo.FindProperty(propertyname);
    }


    public bool SetPropertyValue(string propertyname, object value)
    {
        if (serializedInfo == null)
        {
            serializedInfo = new SerializedObject(this);
        }

        SerializedProperty serializedprop = serializedInfo.FindProperty(propertyname);

        if (serializedprop != null)
        {
            switch(serializedprop.propertyType)
            {
                case SerializedPropertyType.Integer :
                    serializedprop.intValue = (int)value;
                    break;

                case SerializedPropertyType.Float:
                    serializedprop.floatValue = (float)value;
                    break;

                case SerializedPropertyType.Boolean:
                    serializedprop.boolValue = (bool)value;
                    break;

                case SerializedPropertyType.String :
                    serializedprop.stringValue = (string)value;
                    break;

                case SerializedPropertyType.Vector3 :
                    serializedprop.vector3Value = (Vector3)value;
                    break;

                case SerializedPropertyType.Quaternion :
                    serializedprop.quaternionValue = (Quaternion)value;
                    break;

                case SerializedPropertyType.ObjectReference :
                    serializedprop.objectReferenceValue = (UnityEngine.Object)value;
                    break;

                /*default :

                    if ( serializedprop.isArray == true )
                    {
                        IList listif = (IList)value;

                        serializedprop.arraySize = listif.Count;
                        

                    }
                    break;*/

            }

            return true;
        }

        return false;
    }

    public bool PropertyField(string propertyname, string label, params GUILayoutOption[] options)
    {
        if (serializedInfo == null)
        {
            serializedInfo = new SerializedObject(this);
        }

        SerializedProperty serializedprop = serializedInfo.FindProperty(propertyname);

        if (serializedprop != null)
        {
            return EditorGUILayout.PropertyField(serializedprop, new GUIContent(label), true, options);
        }

        return false;   
    }

    public virtual void OnAddAsset(UnityEngine.Object assetobject)
    {

    }

#endif




}
