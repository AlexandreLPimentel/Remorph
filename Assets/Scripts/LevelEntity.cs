using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntity {

    GameObject thisEntity;

    Vector3 myPosition;

    private int? thisEntrance;

    Dictionary<char, GameObject> map = new Dictionary<char, GameObject>();
    char myShape;

    protected LevelEntity()
    {

    }

    public LevelEntity(Vector3 pos)
    {
        myPosition = pos;
        map.Add(MyDictionary.PLATFORM_HIGH, Resources.Load("HighPlatform", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_LOW, Resources.Load("LowPlatform", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_STAIRS, Resources.Load("Stairs", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_RAMP, Resources.Load("Ramp", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_TUNNEL, Resources.Load("Tunnel", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_CUBE_PORTAL, Resources.Load("CubePortal", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_SPHERE_PORTAL, Resources.Load("SpherePortal", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_CROSS_PORTAL, Resources.Load("CrossPortal", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_PYRAMID_PORTAL, Resources.Load("PyramidPortal", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_START, Resources.Load("StartingPlatform", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.PLATFORM_END, Resources.Load("EndingPlatform", typeof(GameObject)) as GameObject);
    }

    public void unload()
    {
        UnityEngine.Object.Destroy(thisEntity);
        thisEntity = null;
        myShape = MyDictionary.NOTHING;
    }

    public int load(string code)
    {
        myShape = code[0];

        if (code[0].Equals(MyDictionary.NOTHING)) //empty area
        {
            return 0;
        }
        map.TryGetValue(code[0], out thisEntity);

        GameObject objectToLoad;
        map.TryGetValue(code[0], out objectToLoad);

        Vector3 rotation = objectToLoad.transform.rotation.eulerAngles;
        rotation.x += Convert.ToInt32(code.Substring(1, 3));
        int yAngle = Convert.ToInt32(code.Substring(4, 3));
        if(code[0].Equals(MyDictionary.PLATFORM_STAIRS) || code[0].Equals(MyDictionary.PLATFORM_RAMP))
        {
            thisEntrance = yAngle / 90;
        }
        else
        {
            thisEntrance = null;
        }
        rotation.y += yAngle;
        rotation.z += Convert.ToInt32(code.Substring(7, 3));

        Quaternion quaternionRotation = Quaternion.identity;
        quaternionRotation.eulerAngles = rotation;

        Vector3 position = objectToLoad.transform.position;
        position += myPosition;

        thisEntity = GameObject.Instantiate(objectToLoad, position, quaternionRotation);
        thisEntity.transform.localScale = objectToLoad.transform.localScale;

        if (code[0].Equals(MyDictionary.PLATFORM_START))
        {
            return 1;
        }
        if (code[0].Equals(MyDictionary.PLATFORM_END))
        {
            return 2;
        }

        return 0;
    }

    public bool canBeWalkedBy(char shape)
    {
        switch (myShape)
        {
            case MyDictionary.PLATFORM_HIGH: return true;
            case MyDictionary.PLATFORM_LOW: return true;
            case MyDictionary.PLATFORM_START: return true;
            case MyDictionary.PLATFORM_END: return true;

            //Portals
            case MyDictionary.PLATFORM_PYRAMID_PORTAL: return shape.Equals(MyDictionary.SHAPE_PYRAMID);
            case MyDictionary.PLATFORM_CROSS_PORTAL: return shape.Equals(MyDictionary.SHAPE_CROSS) || shape.Equals(MyDictionary.SHAPE_CYLINDER_CROSS);
            case MyDictionary.PLATFORM_CUBE_PORTAL: return shape == MyDictionary.SHAPE_CUBE;
            case MyDictionary.PLATFORM_SPHERE_PORTAL: return shape == MyDictionary.SHAPE_SPHERE;

            //Height displacers
            case MyDictionary.PLATFORM_RAMP: return shape == MyDictionary.SHAPE_SPHERE;
            case MyDictionary.PLATFORM_STAIRS: return shape == MyDictionary.SHAPE_CROSS || shape == MyDictionary.SHAPE_CYLINDER_CROSS;

            //Special crossings
            case MyDictionary.PLATFORM_TUNNEL: return shape == MyDictionary.SHAPE_SPHERE;

            //Everything else
            default: return false;
        }
    }

    public float getPlacementHeight()
    {
        switch (myShape)
        {
            case MyDictionary.PLATFORM_HIGH: return 0.55f;
            case MyDictionary.PLATFORM_LOW: return 0.05f;
            case MyDictionary.PLATFORM_START: return 0.05f;
            case MyDictionary.PLATFORM_END: return 0.05f;

            //Portals
            case MyDictionary.PLATFORM_PYRAMID_PORTAL: return 0.05f;
            case MyDictionary.PLATFORM_CROSS_PORTAL: return 0.05f;
            case MyDictionary.PLATFORM_CUBE_PORTAL: return 0.05f;
            case MyDictionary.PLATFORM_SPHERE_PORTAL: return 0.05f;

            //Height displacers
            case MyDictionary.PLATFORM_RAMP: return 0.55f;
            case MyDictionary.PLATFORM_STAIRS: return 0.55f;

            //Special crossings
            case MyDictionary.PLATFORM_TUNNEL: return 0.05f;

            //Everything else
            default: return 0;
        }
    }

    public Boolean canEnterFrom(int direction)
    {
        return thisEntrance == null || !thisEntrance.HasValue || direction == thisEntrance.Value;
    }

    public char getShape()
    {
        return this.myShape;
    }
}
