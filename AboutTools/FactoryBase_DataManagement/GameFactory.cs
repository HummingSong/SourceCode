using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Cores;
using Assets.Scripts.Cores.Resources;
using Assets.Scripts.Utility;

public class GameFactory : Presenter
{
    [HideInInspector]
    public List<string>  dfFactoriesName;
    

	// 팩토리를 찾기 위한 Dictionary .  key로 사용하는 값은 해당 자료형 type 의 Hash 값이다.
	private	Dictionary<int,dfFactoryBase>	mapFactories = new Dictionary<int,dfFactoryBase> ();
    private Dictionary<int, dfFactoryBase> mapNetPackageFactory = new Dictionary<int, dfFactoryBase>();
    private dfItemInfoFactory iteminfofactory = null;
    private dfItemUpgradeTableFactory itemupgradefactory = null;
    private bool _registered = false;


	void Start ()
    {
		// 각각의 팩토리를 팩토리 맵에 저장한다. 
		// 순수 하게 Data만 가지고 있으며 해당 factory 에 있는 것만 리턴 하면 되기 때문에 update 등이 필요 하지 않다.
		enabled = false;
	}


    // 팩토리를 팩토리 맵에 저장한다.
    // 각각의 팩토리가 저장하고 있는 자료형 type 의 해시값을 key 로 사용 한다.
    public IEnumerator RegisterFactories()
    {
        //Core.Presenter.Get<UI_BeforeLoadBundle>().SetBackGroundImage_LoadWaitUI("SFM_Img_8");

        if (_registered.Equals(true))
        {
            yield break;
        }

        iteminfofactory = null;

        mapFactories.Clear();
        mapNetPackageFactory.Clear();

        var rm = Core.Presenter.Get<ResourceManager>();

        if (null == rm)
        {
#if UNITY_EDITOR
            Console.Assert("GameFactory::RegisterFactories() - ResourceManager is null.");
#endif
            yield break;
        }

        for (int i = 0; i < dfFactoriesName.Count; i++)
        {
            if (!Util.IsGameEngineDev())
            {
                if(!Util.IsRelease())
                {
                    //Release 가 아닐경우 팩토리 이름까지 상세하게 표시.
                    Core.Presenter.Get<UI_BeforeLoadBundle>().Start_LoadWaitUI("시스템 데이터를 로드하는 중 입니다 - " + dfFactoriesName[i] + " (" + (i + 1).ToString() + "/" + dfFactoriesName.Count.ToString() + ")");
                }
                else
                {
                    Core.Presenter.Get<UI_BeforeLoadBundle>().Start_LoadWaitUI("시스템 데이터를 로드하는 중 입니다. " + " (" + (i + 1).ToString() + "/" + dfFactoriesName.Count.ToString() + ")");
                }

                yield return new WaitForSeconds(0.001f);
            }

            var result = rm.Get("Factories", dfFactoriesName[i], ResourceManager.ResourceCategory.Scripts, typeof(dfFactoryBase));

            dfFactoryBase factory = result as dfFactoryBase;

            if (factory == null)
            {
                Console.LogError("팩토리 변환 실패 : " + dfFactoriesName[i]);
                continue;
            }

            if (factory.GetBaseTypeFactory() == null)
                continue;

            mapFactories.Add(factory.GetBaseTypeFactory().GetHashCode(), factory);

            factory.BuildItemDictionary();

            if (factory.GetType() == typeof(dfItemInfoFactory) && iteminfofactory == null)
            {
                iteminfofactory = (dfItemInfoFactory)factory;
            }
            else if (factory.GetType() == typeof(dfItemUpgradeTableFactory) && itemupgradefactory == null)
            {
                itemupgradefactory = (dfItemUpgradeTableFactory)factory;
            }

            if (factory.bIncludeNetPackageBuild == true)
            {
                if (mapNetPackageFactory.ContainsKey(factory.netPackageID) == true)
                {
                    Console.LogError("Check!!!! rearragne factories net package id");
                }
                else
                {
                    mapNetPackageFactory.Add(factory.netPackageID, factory);
                }
            }
        }

        if (!Util.IsGameEngineDev())
        {
            //Core.Presenter.Get<UI_BeforeLoadBundle>().Start_LoadWaitUI("로비 데이터를 로드하는 중 입니다.");
            yield return new WaitForSeconds(0.001f);
        }

        _registered = true;
    }



    // factory 에서 해당 타입을 가진 아이템을 가져온다.
    // 주의점 : 여기서 가져온 데이터를 변조 하면 팩토리 내의 값이 바뀌게 되어서 
    // 이후에 가져올때는 변경된 값이 적용된 데이터를 가져온다.
    // factory 내의 아이템 값이 변조되어야 할 때는 GetFactoryCopiedItem 을 가져온다.
    public T GetFactoryItem<T>(int index)
    {
        System.Type t = typeof(T);
        int iTypeHash = t.GetHashCode ();

        dfFactoryBase factory = null;

        if (mapFactories.TryGetValue (iTypeHash, out factory) == true) 
        {
            object findObject = factory.GetItem(index);

            if ( findObject != null )
            {
                return (T)findObject;
            }
            else
            {
                return default(T);
            }
        }
        else 
        {
            return default(T);
        }
    }

    public void GetFactoryItems<T>( ref List<T> outList )
    {
        System.Type t = typeof( T );
        int iTypeHash = t.GetHashCode();

        dfFactoryBase factory = null;

        if( mapFactories.TryGetValue( iTypeHash, out factory ) == true )
        {
            for( int i = 0 ; i < factory.GetItemCount(); ++i )
            {
                object findObject = factory.GetItemAt(i);
                outList.Add((T)findObject);
            }
        }
    }

	public dfFactoryBase GetFactoryItems<T>() where T : dfFactoryItemBase
	{
		System.Type t = typeof( T );
		int iTypeHash = t.GetHashCode();

		dfFactoryBase factory = null;
		if( mapFactories.TryGetValue( iTypeHash, out factory ) )
			return factory;

		return null;
	}

	public T GetFactoryItemInEditor<T>( int index )
    {
        foreach( var factory in mapFactories )
        {
            if( factory.Value.Equals( null ) )
            {
                continue;
            }

            if( factory.Value.GetBaseTypeFactory().Equals( typeof( T ) ) )
            {
                object obj = factory.Value.GetItem( index );

                return (T)obj;
            }
        }

        return default( T );
    }

	// GetFactoryItem 과 유사한 기능이나 차이점은 해당 아이템의 복사본을 생성한후 가져온다.
	// 
	public T GetFactoryCopiedItem<T> (int index )
	{
		System.Type t = typeof(T);
		int iTypeHash = t.GetHashCode ();

        dfFactoryBase factory = null;
		
		if (mapFactories.TryGetValue (iTypeHash, out factory) == true) 
		{
			object madeObject = factory.MakeClone(index);
			
			if ( madeObject != null )
			{
				return (T)madeObject;
			}
			else
			{
				return default(T);
			}
		}
		else 
		{
			return default(T);
		}
	}


    public int GetFactoryItemCount<T>()
    {
        System.Type t = typeof(T);
        int iTypeHash = t.GetHashCode();

        dfFactoryBase factory = null;

        if (mapFactories.TryGetValue(iTypeHash, out factory) == true)
        {
            return factory.itemcount;
        }
        else
        {
            return 0;
        }
    }

    public IEnumerator<dfFactoryItemBase> GetEnumerator<T>()
    {
        System.Type t = typeof(T);
        int iTypeHash = t.GetHashCode();

        dfFactoryBase factory = null;

        if (mapFactories.TryGetValue(iTypeHash, out factory) == true)
        {
            return factory.GetEnumerator();
        }
        else
        {
            return null;
        }
    }

    public dfFactoryItemBase GetNetPackageItem(NetPackageUIDType packageUID)
    {
        if (packageUID == null) return null;

        if (mapNetPackageFactory.ContainsKey(packageUID.factoryID) == false) return null;

        return mapNetPackageFactory[packageUID.factoryID].GetItem(packageUID.itemID);
    }


	public string[] GetFactoryItemStrings<T> ()
	{
		System.Type t = typeof(T);
		int iTypeHash = t.GetHashCode ();

        dfFactoryBase factory = null;
		
		if (mapFactories.TryGetValue (iTypeHash, out factory) == true) 
		{
			return factory.GetItemStrings();
		}

		return null;
	}

    public dfItemInfo GetItemInfo(string itemCode)
    {
		if( string.IsNullOrEmpty( itemCode ) )
			return null;

		if( iteminfofactory == null )
			return null;

		return iteminfofactory.GetItemInfo(itemCode);
    }

    public dfItemUpgradeTableFactory GetUpgradeTableFactory()
    {
        return itemupgradefactory;
    }
}
