using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( GameFactory ) )]
public class GameFactoryEditor : Editor
{
    private List<dfFactoryBase> _dfFactories = new List<dfFactoryBase>();
    private GameFactory _factory;

    void OnEnable()
    {
        _factory = target as GameFactory;
        _dfFactories.Clear();
        for( int i = 0; i < _factory.dfFactoriesName.Count; ++i )
        {
            dfFactoryBase asset = AssetDatabase.LoadAssetAtPath( "Assets/Loads/Scripts/Factories/" + _factory.dfFactoriesName[i] + ".asset", typeof( dfFactoryBase ) ) as dfFactoryBase;
            _dfFactories.Add( asset );
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label( "Factory" );
        if( GUILayout.Button( "+", GUILayout.Width( 50 ) ) )
        {
            _factory.dfFactoriesName.Add( string.Empty );
            _dfFactories.Add( null );
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        for( int i = _dfFactories.Count - 1; i >= 0; --i )
        {
            GUILayout.BeginHorizontal();
            _dfFactories[i] = EditorGUILayout.ObjectField( _dfFactories[i], typeof( dfFactoryBase ), false ) as dfFactoryBase;
            if( GUILayout.Button( "-", GUILayout.Width( 50 ) ) )
            {
                _dfFactories.RemoveAt( i );
                _factory.dfFactoriesName.RemoveAt( i );
                continue;
            }
            GUILayout.EndHorizontal();

            if( null == _dfFactories[i] )
            {
                _factory.dfFactoriesName[i] = string.Empty;
            }
            else if( false == _dfFactories[i].name.Equals( _factory.dfFactoriesName[i] ) )
            {
                _factory.dfFactoriesName[i] = _dfFactories[i].name;
            }
        }
        GUILayout.EndVertical();
    }
}
