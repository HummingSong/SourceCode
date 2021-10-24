#define FACTORYVERSIONUP

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

[System.AttributeUsage(System.AttributeTargets.Class,Inherited=false)]
public sealed class FactoryMenu : System.Attribute
{
    public string   menuItem;
    public int      netpackageidbase;

    public FactoryMenu(string itemName, int packageidbase)
    {
        menuItem = itemName;
        netpackageidbase = packageidbase;
    }
}

[System.Serializable] 
public enum EFactoryViewMode
{
    ItemFoldOut,
    ExcelType,    
};

public interface IScriptData
{

}


[System.Serializable]
public class dfFactoryBase : ScriptableObject, IScriptData
{

    public int              netPackageID = -1;

    public EFactoryViewMode eViewMode = EFactoryViewMode.ItemFoldOut;

    public  string          strFactoryName;

    public string FactoryName
    {
        get
        {
            return strFactoryName;
        }
    }


    [SerializeField]
    public string               strTypeFactoryAssembly;
    
    [SerializeField]
    public string               strTypeFactory;

    [SerializeField]
    public bool                 bIncludeNetPackageBuild;

    [SerializeField]
    protected List<dfFactoryItemBase> listItems = new List<dfFactoryItemBase>();

	protected Dictionary<int, dfFactoryItemBase> mapItems = new Dictionary<int, dfFactoryItemBase>();

	private dfFactoryItemBase	currentSelectedItem = null;

    GUIStyle focusStyle = null;
    GUIStyle normalStyle = null;

    public HideFlags itemHideFlags = HideFlags.None;

    [System.NonSerialized]
    string excelImportpath = string.Empty;
    static string lastSuccessDirectoryName = string.Empty;

	public string importpath { get { return excelImportpath; } }

	// [ypqp35 2016/04/19] 팩토리 줄맞춤 (두 변수를 조율하여 변경해야 합니다)
	[HideInInspector]
	public static float LABEL_WIDTH = 150f;     // 앞부분 간격
	[HideInInspector]
	public static float LAYOUT_WIDTH = 400f;    // 뒷부분 간격
	[HideInInspector]
	public static float BUTTON_WIDTH = 150f;    // 버튼 간격


	protected dfFactoryBase( System.Type typeForFactory )
	{
        strTypeFactory = typeForFactory.ToString();
        strTypeFactoryAssembly = typeForFactory.Assembly.ToString();
	}

#if UNITY_EDITOR

    dfFactoryBase mergeTarget = null;
        
    public void OnEnable()
    {
        BuildItemDictionary();

        for( int i = 0; i < listItems.Count; i++)
        {
            if (listItems[i] == null) continue;
            if ( listItems[i].OwnerFactory != this)
            {
                listItems[i].OwnerFactory = this;
                EditorUtility.SetDirty(listItems[i]);
            }
        }
        
/*        if ( typeFactoryItem != null )
        {
            MemberInfo[] membinfos = typeFactoryItem.GetMembers();

            for( int i = 0; i < membinfos.Length; i++)
            {

            }
        }*/
    }
#endif

    public virtual void BuildItemDictionary()
    {
        for (int i = 0; i < listItems.Count; i++)
        {
            if (listItems[i] == null)
            {
                listItems.RemoveAt(i);
                i--;
            }
        }


        mapItems.Clear();


        foreach (dfFactoryItemBase item in listItems)
        {
            if (item == null) continue;

            if (mapItems.ContainsKey(item.iFactoryIndex) == true)
            {
                Console.LogError("Item index duplicated !!!" + item.name);
            }
            else
            {
				//Security add
				item.SecureInit();
				//Security add

                mapItems.Add(item.iFactoryIndex, item);
            }
        }
    }

    
	
	public virtual System.Type GetBaseTypeFactory()
	{
        return null;
	}

    public virtual System.Type GetInstaceTypeFactory()
    {
        return null;
    }

    public virtual dfFactoryItemBase GetItem(int index)
	{
        dfFactoryItemBase result;

        if ( mapItems.TryGetValue(index,out result) == true )
        {
            return result;
        }

        return null;
	}

    public dfFactoryItemBase GetItemInEditor(int index)
    {
        foreach( dfFactoryItemBase item in listItems)
        {
            if ( item.iFactoryIndex == index)
            {
                return item;
            }
        }

        return null;
    }

    public dfFactoryItemBase GetItemAt(int number)
    {
        if ( listItems.Count > number )
        {
            return listItems[number];
        }
        return null;
    }

    public int GetItemCount()
    {
        return listItems.Count;
    }


    public virtual dfFactoryItemBase MakeClone(int index)
	{
        dfFactoryItemBase srcItem = GetItem(index);

        if (srcItem == null) return null;
        dfFactoryItemBase newItem = srcItem.CreateClone();
		return newItem;
	}


    public dfFactoryItemBase CreateNewItem( System.Type itemclasstype )
    {
        if ( itemclasstype == null || GetBaseTypeFactory() == null) return null;
        if ( itemclasstype != GetBaseTypeFactory() && itemclasstype.IsSubclassOf(GetBaseTypeFactory()) == false) return null;

        dfFactoryItemBase newInstanceItem = (dfFactoryItemBase)ScriptableObject.CreateInstance(itemclasstype);

        return newInstanceItem;               
    }

    public string[] GetItemStrings()
    {
        string[] result = new string[listItems.Count];

        for (int i = 0; i < listItems.Count; i++  )
        {
            result[i] = listItems[i].strRepresentName;
        }

        return result;            
    }

    public IEnumerator<dfFactoryItemBase> GetEnumerator()
    {
        return listItems.GetEnumerator();
    }

    public int itemcount
    {
        get
        {
            return listItems.Count;
        }
    }


#if UNITY_EDITOR

    // item list 들을 정리해준다.
    public void ArrangeItemList()
    {
        listItems.Sort((dfFactoryItemBase a, dfFactoryItemBase b) => a.iFactoryIndex.CompareTo(b.iFactoryIndex));

        mapItems.Clear();

        foreach (dfFactoryItemBase item in listItems)
        {
            if (mapItems.ContainsKey(item.iFactoryIndex) == true)
            {
                Console.LogError("Item index duplicated !!!" + item.name);
            }
            else
            {
                mapItems.Add(item.iFactoryIndex, item);
            }
        }

        EditorSetDirty();
    }

	public void RemoveItem()
	{
		if ( currentSelectedItem != null )
		{
			listItems.Remove ( currentSelectedItem );
			Editor.DestroyImmediate(currentSelectedItem,true);
			currentSelectedItem = null;
            ArrangeItemList();
			EditorSetDirty ();
		}
	}

    public void RemoveItem(dfFactoryItemBase item)
    {
        listItems.Remove(item);
        Editor.DestroyImmediate(item, true);
        ArrangeItemList();
        EditorSetDirty();
    }

	public void RemoveAllItems()
	{
		for (int i = 0; i < listItems.Count; i++) 
		{
			Editor.DestroyImmediate(listItems[i],true);
			listItems[i] = null;
		}
		listItems.Clear ();
        mapItems.Clear();
		EditorSetDirty ();
	}

	public void EditorSetDirty()
	{
		if (AssetDatabase.Contains (this) == true) 
		{
			EditorUtility.SetDirty(this);
		}
	}


    public virtual void OnGUI()
    {
        if ( focusStyle == null )
        {
            focusStyle = new GUIStyle(GUI.skin.box);
        }
        focusStyle.contentOffset = new Vector2(3.0f, 0.0f);
        focusStyle.margin = new RectOffset(2, 2, 0, 0);

        if ( normalStyle == null)
        {
            normalStyle = new GUIStyle();
        }

        normalStyle.contentOffset = new Vector2(3.0f, 0.0f);
        normalStyle.margin = new RectOffset(2, 2, 0, 0);
        normalStyle.normal = new GUIStyleState();
        normalStyle.normal.textColor = Color.white;


        //FactoryMenu();
#if FACTORYVERSIONUP
        FileVersionUp();
#endif

        DrawItems();

    }

    public void FileVersionUp()
    {
        for( int i = 0; i < listItems.Count; ++i )
        {
            if( listItems[i] == null )
                continue;

            listItems[i].TempVersionUp();
        }
    }

    public virtual void DrawItems()
    {
        for (int i = 0; i < listItems.Count; ++i)
        {
            if (listItems[i] == null) continue;

            listItems[i].TempVersionUp();

            string listFocusKey = "FactoryItem " + i.ToString();

            switch (eViewMode)
            {
                case EFactoryViewMode.ItemFoldOut:
                    ItemDrawFoldOut(listItems[i], i, listFocusKey);
                    break;

                case EFactoryViewMode.ExcelType:
                    ItemDrawExcelType(listItems[i], i, listFocusKey);
                    break;
            }
        }
    }


    public virtual void FactoryMenu()
    {
        GUILayout.Label("Net Package ID " + netPackageID);

        GUILayout.BeginHorizontal(GUIStyle.none);
        ItemAddButton();

        DuplicateButton();

        if (GUILayout.Button("Remove Selected Item", GUILayout.Width(BUTTON_WIDTH)) == true)
        {
            RemoveItem();
        }

        if (GUILayout.Button("Remove All Items", GUILayout.Width( BUTTON_WIDTH ) ) == true)
        {
            RemoveAllItems();
        }

        if (GUILayout.Button("Arrange", GUILayout.Width( BUTTON_WIDTH ) ) == true)
        {
            ArrangeItemList();
        }

        if (GUILayout.Button("Fold All", GUILayout.Width( BUTTON_WIDTH ) ) == true)
        {
            FoldAll();
        }

        eViewMode = (EFactoryViewMode)EditorGUILayout.EnumPopup(eViewMode, GUILayout.Width( BUTTON_WIDTH ), GUILayout.Height(16.0f));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
		mergeTarget = (dfFactoryBase)EditorGUILayout.ObjectField( "merge with", mergeTarget, GetType(), false, GUILayout.Width( LAYOUT_WIDTH ) );
		if( mergeTarget == this )
		{
			mergeTarget = null;
        }

        if ( GUILayout.Button("Merge", GUILayout.Width( BUTTON_WIDTH ) ) == true && mergeTarget != null)
        {
            Merge(mergeTarget);
        }
        GUILayout.EndHorizontal();
    }

    public bool ImportButton()
    {
        bool result = false;

        GUILayout.BeginHorizontal(GUI.skin.box);
        GUILayout.TextField(excelImportpath, GUILayout.Width( LAYOUT_WIDTH ) );
        if ( GUILayout.Button("Import from", GUILayout.Width( BUTTON_WIDTH ) ) == true )
        {
			const string EXT = "xlsx";
			string directoryName = string.Empty;

			if( lastSuccessDirectoryName != string.Empty )
			{
				directoryName = lastSuccessDirectoryName;
			}
			else
			{
				if( Path.HasExtension( excelImportpath ) )
				{
					try { directoryName = Path.GetDirectoryName( excelImportpath ); } catch {}
				}
			}

			if( directoryName == string.Empty )
			{
				directoryName = Directory.GetCurrentDirectory();
			}

			excelImportpath = EditorUtility.OpenFilePanel( "Select Excel File", directoryName, EXT );

			result = true;
        }
        GUILayout.EndHorizontal();

        return result;
    }

	public void OnImportExcelResult( bool bSuccess )
	{
		if( bSuccess )
		{
			if( Path.HasExtension( excelImportpath ) )
			{
				try { lastSuccessDirectoryName = Path.GetDirectoryName( excelImportpath ); } catch {}
			}
		}
		else
		{
			excelImportpath = string.Empty;
		}
	}

	public virtual void ItemAddButton()
    {
        if (GUILayout.Button("Add new Item", GUILayout.Width( BUTTON_WIDTH ) ) == true)
        {
            dfFactoryItemBase item = CreateNewItem(GetInstaceTypeFactory());
            ItemAdd(item, true);
        }
    }

    public dfFactoryItemBase AddNewItem(int index)
    {
        dfFactoryItemBase item = CreateNewItem(GetInstaceTypeFactory());
        if ( item != null )
        {
            item.iFactoryIndex = index;
        }
        ItemAdd(item, false);
        return item;
    }

    public void ItemAdd(dfFactoryItemBase item, bool autoAssignIndex = true)
    {
        if (autoAssignIndex == true)
        {
            int baseindex = 0;

            if ( currentSelectedItem != null)
            {
                baseindex = currentSelectedItem.iFactoryIndex;
            }

            while (GetItemInEditor(baseindex) != null)
            {
                baseindex++;
            }

            item.iFactoryIndex = baseindex;
        }
        
        item.OwnerFactory = this;
        if (itemHideFlags != HideFlags.None)
            item.hideFlags = itemHideFlags;
        listItems.Add(item);

        AssetDatabase.AddObjectToAsset(item, this);
        item.OnAddAsset(this);

        ArrangeItemList();
        EditorSetDirty();
    }

    public void DuplicateButton()
    {
        if (GUILayout.Button("Dupliacte Item", GUILayout.Width( BUTTON_WIDTH ) ) == true && currentSelectedItem != null )
        {
            dfFactoryItemBase item = currentSelectedItem.CreateClone();
            ItemAdd(item, true);
        }
    }

    public void ItemDrawFoldOut(dfFactoryItemBase item, int index, string focuskey )
    {
        GUI.SetNextControlName(focuskey);
        item.bFold = EditorGUILayout.Foldout(item.bFold, "[Index :"+ item.iFactoryIndex + "] "+ " [name : "+ item.strRepresentName + "] [type : " + item.GetType().Name +"]");

        EditorGUI.indentLevel++;

        if (GUI.GetNameOfFocusedControl() == focuskey)
        {
            currentSelectedItem = item;
        }

        if (item.bFold == true)
        {
            item.OnEditorGUI();
        }

        EditorGUI.indentLevel--;
    }

    public void ItemDrawExcelType(dfFactoryItemBase item, int index, string focuskey)
    {
        GUI.SetNextControlName(focuskey);
        GUILayout.BeginHorizontal(currentSelectedItem == item ? focusStyle : normalStyle);

        GUILayout.Label(item.iFactoryIndex.ToString(), GUILayout.Width(20.0f));
        item.OnEditorGUI();
        GUILayout.EndHorizontal();

        if (GUI.GetNameOfFocusedControl() == focuskey)
        {
            currentSelectedItem = item;
        }
    }

    public void Merge(dfFactoryBase otherFactory)
    {
        if (otherFactory == null) return;
        if (otherFactory.GetType() != GetType()) return;

        IEnumerator<dfFactoryItemBase> iter = otherFactory.GetEnumerator();

        while( iter.MoveNext() )
        {
            dfFactoryItemBase item = iter.Current;
            if (item == null) continue;

            if (GetItemInEditor(item.iFactoryIndex) != null)
            {
                // 다른 타입일 수가 있기 때문에 복사본을 만들어서 추가해준다.
                dfFactoryItemBase cloneItem = item.CreateClone();

                // overwrite if item exist
                dfFactoryItemBase itemexist = GetItemInEditor(item.iFactoryIndex);
                RemoveItem(itemexist);
                ItemAdd(cloneItem, false);
            }
            else
            {
                // make new item
                dfFactoryItemBase newItem = item.CreateClone();
                ItemAdd(newItem, false);
            }

            EditorSetDirty();
        }
    }

    void FoldAll()
    {
        IEnumerator<dfFactoryItemBase> iter = GetEnumerator();

        while(iter.MoveNext())
        {
            dfFactoryItemBase fac = iter.Current;
            fac.bFold = false;
            EditorUtility.SetDirty(fac);
        }

    }

    public void LinkQuestInfo(int questIndex)
    {
        dfFactoryItemBase info = GetItemInEditor( questIndex );

        if (info != null)
        {
            GUILayout.Space(10.0f);
            EditorGUILayout.BeginVertical(GUI.skin.box);

            info.bFold = EditorGUILayout.Foldout(info.bFold, "Quest link...");
            if (info.bFold)
            {
                info.OnEditorGUI();
            }
            EditorGUILayout.EndVertical();
        }
    }

#endif


}

