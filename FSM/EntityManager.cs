using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager
{
    private static EntityManager instance;

    private EntityManager()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }

    public static EntityManager Instance
    {
        get
        {
            if (instance == null)
            {
                new EntityManager();
            }

            return instance;
        }
    }

    private Dictionary<int, BaseEntity> entityDic = new Dictionary<int, BaseEntity>();

    public void RegisterEntity(BaseEntity entity)
    {
        entityDic.Add(entity.GetInstanceID(), entity);
    }

    public void RemoveEntity(BaseEntity entity)
    {
        entityDic.Remove(entity.GetInstanceID());
    }

    public BaseEntity GetEntity(int id)
    {
        return entityDic[id];
    }
}
