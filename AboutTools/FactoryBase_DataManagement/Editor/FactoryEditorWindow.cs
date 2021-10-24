using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEditor;

public class FactoryEditorWindow : EditorWindow {

	Vector2	vFactoryListAreaScroll 	= Vector2.zero;
	float	fFactoryListAreaWidth	= 200.0f;

	Vector2	vFactoryContentAreaScroll 	= Vector2.zero;

    
	string	strDefaultPath = "Assets/Loads/Scripts/Factories";

	private int iCreateIndex = 0;
	string[] strMenuItems;

	dfFactoryBase				CurrentFactoryInstance = null;

    string excelmportpath;

	public class FactoryInstanceInfo
	{
		public dfFactoryBase	FactoryInstance = null;
		public Type				FactoryTypeInfo = null;
	}

	// 팩토리 타입 정보 
	List<FactoryInstanceInfo> FactoyTypes = new List<FactoryInstanceInfo> ();

	// 생성된 팩토리 정보
	List<dfFactoryBase> FactoryAssetList = new List<dfFactoryBase> ();

	[MenuItem("Window/Factory Editor")]
	static void Init()
	{
		FactoryEditorWindow window = (FactoryEditorWindow)EditorWindow.GetWindow (typeof(FactoryEditorWindow));
		window.title = "Factory Editor";

        initCore = new GameObject();
        initCore.AddComponent<Assets.Scripts.Cores.Core>();
    }

	void OnEnable()
	{
        Check Factory Editor setting(using EditorPref)
		if (EditorPrefs.HasKey(strDefaultPathKey) == false)
        {
            strFactoriesFolder = EditorUtility.OpenFolderPanel("Select default factories Folder[for SL]", "", "");
            EditorPrefs.SetString(strDefaultPathKey, strFactoriesFolder);
        }
        else
        {
            strFactoriesFolder = EditorPrefs.GetString(strDefaultPathKey);
        }

        InitFactories ();
	}


    void OnDestroy()
    {
        Console.Log( "Editor Terminated!!" );

        AssetDatabase.SaveAssets();
    }



    void InitFactories ()
	{	
		List<string> factoryTypeNames = new List<string> ();
		System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
		foreach (System.Reflection.Assembly A in AS)
		{
			System.Type[] types = A.GetTypes();
			foreach (System.Type T in types)
			{
                if (T.IsSubclassOf(typeof(dfFactoryBase)) == false ) continue;

                string factorymenuname = T.Name;

                if ( T.IsDefined(typeof(FactoryMenu), false) == true )
                {
                    object[] attrs = T.GetCustomAttributes(typeof(FactoryMenu), false);

                    FactoryMenu attrMenu = (FactoryMenu)attrs[0];
                    factorymenuname = attrMenu.menuItem;
                }


                if (factoryTypeNames.Contains(factorymenuname) == true ) continue;

                factoryTypeNames.Add(factorymenuname);
				RegisterFactoryType ( T, ref FactoyTypes );
			}
		}

        strMenuItems = factoryTypeNames.ToArray();

		LoadAllAssetAtPath ( strDefaultPath );
	}

	void LoadAllAssetAtPath(string path)
	{
		string[] strAssetFiles = Directory.GetFiles (path,"*" /*"*.asset"*/);

        List<int> packageids = new List<int>();

        bool bRebuildNetPackageID = false;
		
		for (int i = 0; i < strAssetFiles.Length; i++) 
		{
			UnityEngine.Object loadedasset = AssetDatabase.LoadAssetAtPath(strAssetFiles[i],typeof(dfFactoryBase));
			
			if( loadedasset == null ) continue;

            if (FactoryAssetList.Contains((dfFactoryBase)loadedasset) == true) continue;

            ((dfFactoryBase)loadedasset).OnEnable();

			FactoryAssetList.Add ( (dfFactoryBase)loadedasset );

            int packageid = ((dfFactoryBase)loadedasset).netPackageID;

            if ( packageid != -1 && packageids.Contains(packageid) == false )
            {
                packageids.Add(packageid);
            }
            else
            if ( packageid != -1 && packageids.Contains(packageid) == true )
            {
                bRebuildNetPackageID = true;
            }
		} 

        if ( bRebuildNetPackageID == true )
        {
            packageids.Clear();

            for (int i = 0; i < FactoryAssetList.Count; i++)
            {
                FactoryAssetList[i].netPackageID = -1;
            }
        }

        ArrangeNetPackageID(ref FactoryAssetList, ref packageids);
	}

    void ArrangeNetPackageID(ref List<dfFactoryBase> faclist, ref List<int> packageidlist)
    {
        for (int i = 0; i < FactoryAssetList.Count; i++)
        {
            if (FactoryAssetList[i].netPackageID == -1)
            {
                // find suitable package id and assign
                System.Type T = FactoryAssetList[i].GetType();

                object[] attrs = T.GetCustomAttributes(typeof(FactoryMenu), false);
                FactoryMenu attrMenu = (FactoryMenu)attrs[0];

                int emptyid = attrMenu.netpackageidbase;

                while (packageidlist.Contains(emptyid) == true)
                {
                    emptyid++;
                }

                FactoryAssetList[i].netPackageID = emptyid;
                FactoryAssetList[i].EditorSetDirty();

                packageidlist.Add(emptyid);
            }
        }
    }

	void RegisterFactoryType(System.Type factoryType, ref List<FactoryInstanceInfo> faclist )
	{
		if (factoryType == null)
			return;
		
		if (factoryType.IsSubclassOf (typeof(dfFactoryBase)) == false)
			return;
		
		FactoryInstanceInfo newfactoryinfo = new FactoryInstanceInfo ();
		newfactoryinfo.FactoryInstance = null;
		newfactoryinfo.FactoryTypeInfo = factoryType;
		faclist.Add (newfactoryinfo);

	}

	void OnGUI()
	{
		GUILayout.Label ("Factory Editor V.0.01", EditorStyles.boldLabel);

		GUILayout.Label ("Factory Type Select",EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		iCreateIndex = EditorGUILayout.Popup(iCreateIndex,strMenuItems, GUILayout.Width(fFactoryListAreaWidth));

		if ( GUILayout.Button ("Create", GUILayout.Width(100)) )
		{

			string factoryname = strMenuItems[iCreateIndex];
			string filepath = EditorUtility.SaveFilePanelInProject("Create " + factoryname + " file" ,
				                                                       "",
			                                                      		"Asset" ,
				                                                       "Plz enter file name to create factory",
			                                                       		strDefaultPath );
				
			if ( filepath.Length != 0 )
			{
				// create factory instance.
				dfFactoryBase newCreatedFactory = (dfFactoryBase)ScriptableObject.CreateInstance(FactoyTypes[iCreateIndex].FactoryTypeInfo) ;
				if ( newCreatedFactory != null )
				{
					dfFactoryBase dffactory = newCreatedFactory;
					FactoryAssetList.Add ( dffactory );
					AssetDatabase.CreateAsset(newCreatedFactory,filepath);
					CurrentFactoryInstance = newCreatedFactory;
					EditorUtility.SetDirty(this);

                    BuildNetPackage();
				}
			}
		}

        for (int i = 0; i < FactoryAssetList.Count; ++i)
        {
            if ( FactoryAssetList[i] == null)
            {
                FactoryAssetList.RemoveAt(i);
                i--;
            }
        }
        

		if ( GUILayout.Button ("Save", GUILayout.Width(100)) )
		{
			for ( int i = 0 ; i < FactoryAssetList.Count ; ++i ) 
			{
                FactoryAssetList[i].ArrangeItemList();
				EditorUtility.SetDirty ( FactoryAssetList[i] );
			}
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

		if ( GUILayout.Button ("RemoveAsset", GUILayout.Width(100)) )
		{
			if ( FactoryAssetList.Contains (CurrentFactoryInstance) )
			{
				if ( AssetDatabase.Contains ( CurrentFactoryInstance ) )
				{
					AssetDatabase.DeleteAsset ( AssetDatabase.GetAssetPath( CurrentFactoryInstance ) );
					FactoryAssetList.Remove ( CurrentFactoryInstance );
					Editor.DestroyImmediate ( CurrentFactoryInstance,true );
					EditorUtility.SetDirty(this);
				}
			}
		}

        /*if (GUILayout.Button("Generate Netpackage", GUILayout.Width(140.0f)) )
        {
            BuildNetPackage();
        }*/

        if (GUILayout.Button("Build Netpackage", GUILayout.Width(140.0f)) )
        {
            BuildNetPackage();
        }

		GUILayout.EndHorizontal ();

		//Write here.
		GUILayout.BeginHorizontal (GUIStyle.none);

		OnGUIFactoryListView ();

		OnGUIFactoryContentsView ();

		GUILayout.EndHorizontal ();
	}

    void BuildNetPackage()
    {
        List<int> packageids = new List<int>();

        for (int i = 0; i < FactoryAssetList.Count; i++)
        {
            if (FactoryAssetList[i].netPackageID != -1)
            {
                packageids.Add(FactoryAssetList[i].netPackageID);
            }
        }

        ArrangeNetPackageID(ref FactoryAssetList, ref packageids);
    }

	void OnGUIFactoryListView()
	{
		vFactoryListAreaScroll = 
			GUILayout.BeginScrollView (vFactoryListAreaScroll,"box",GUILayout.Width (fFactoryListAreaWidth));

		for ( int i = 0 ; i < FactoryAssetList.Count ; ++i ) 
		{
			GUILayout.BeginHorizontal(GUIStyle.none);

			string listFocusKey = "FactoyType " + i.ToString();
			GUI.SetNextControlName (listFocusKey);
			EditorGUILayout.Foldout(false,FactoryAssetList[i].name);
			//EditorGUILayout.SelectableLabel(FactoryAssetList[i].name, EditorStyles.boldLabel);
			if ( GUI.GetNameOfFocusedControl() == listFocusKey )
			{
				CurrentFactoryInstance = FactoryAssetList[i];
				//Debug.Log(listFocusKey);
			}
			GUILayout.EndHorizontal ();

		}

		GUILayout.EndScrollView ();
	}

	void OnGUIFactoryContentsView()
	{
        GUILayout.BeginVertical(GUI.skin.box);

		if( CurrentFactoryInstance != null )
		{
			GUILayout.BeginHorizontal();
            GUILayout.Label(CurrentFactoryInstance.GetType().Name + " : " + CurrentFactoryInstance.name, GUILayout.Width(250.0f));
            bool value = EditorGUILayout.Toggle("include netpackage", CurrentFactoryInstance.bIncludeNetPackageBuild);
            if (value != CurrentFactoryInstance.bIncludeNetPackageBuild)
            {
                CurrentFactoryInstance.bIncludeNetPackageBuild = value;
                CurrentFactoryInstance.EditorSetDirty();
            }
            GUILayout.EndHorizontal();

			if( CurrentFactoryInstance.ImportButton() )
			{
				bool bSuccess = ImportFromExcel( CurrentFactoryInstance, CurrentFactoryInstance.importpath );
				CurrentFactoryInstance.OnImportExcelResult( bSuccess );
			}

			CurrentFactoryInstance.FactoryMenu();
        }

		vFactoryContentAreaScroll =
			GUILayout.BeginScrollView (vFactoryContentAreaScroll,"box");

		if ( CurrentFactoryInstance != null )
		{
			CurrentFactoryInstance.OnGUI ();
		}

		GUILayout.EndScrollView ();

        GUILayout.EndVertical();
	}

	bool ImportFromExcel( dfFactoryBase targetFactory, string filepath )
	{
#if UNITY_EDITOR_WIN
		if( targetFactory == null )
			return false;

		FactoryExcelLoader loader = new FactoryExcelLoader( filepath );

		if( loader.isValid )
		{
			if( loader.ImportToFactory( targetFactory ) == false )
			{
				Debug.LogError( "Invalid excel sheet (mismatch class type)" );
				return false;
			}
			else
			{
				Debug.Log( "....Import succeed " + targetFactory.ToString() + " " + filepath );
				return true;
			}
		}
		else
		{
			Debug.Log( "invalide sheet" );
			return false;
		}
#else
		return false;
#endif
	}
}
