using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntity {

    GameObject thisEntity;

    Vector3 myPosition;

    private int? thisEntrance;

    Dictionary<MyDictionary.Platform, GameObject> map = new Dictionary<MyDictionary.Platform, GameObject>();
    MyDictionary.Platform myShape;

    protected LevelEntity()
    {

    }

    public LevelEntity(Vector3 pos)
    {
        myPosition = pos;
        map.Add(MyDictionary.Platform.High, Resources.Load("HighPlatform", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.Platform.Low, Resources.Load("LowPlatform", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.Platform.Stairs, Resources.Load("Stairs", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.Platform.Ramp, Resources.Load("Ramp", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.Platform.Tunnel, Resources.Load("Tunnel", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.Platform.Start, Resources.Load("StartingPlatform", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.Platform.End, Resources.Load("EndingPlatform", typeof(GameObject)) as GameObject);
    }

    public void unload()
    {
        UnityEngine.Object.Destroy(thisEntity);
        thisEntity = null;
        myShape = MyDictionary.Platform.Nothing;
    }

    public int load(string code)
    {
        myShape = (MyDictionary.Platform)code[0];

        if (myShape.Equals(MyDictionary.Platform.Nothing)) //empty area
        {
            return 0;
        }
        map.TryGetValue(myShape, out thisEntity);

        GameObject objectToLoad;
        map.TryGetValue(myShape, out objectToLoad);

        Vector3 rotation = objectToLoad.transform.rotation.eulerAngles;
        rotation.x += Convert.ToInt32(code.Substring(1, 3));
        int yAngle = Convert.ToInt32(code.Substring(4, 3));
        if(myShape.Equals(MyDictionary.Platform.Stairs) || myShape.Equals(MyDictionary.Platform.Ramp))
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

        if (myShape.Equals(MyDictionary.Platform.Start))
        {
            return 1;
        }
        if (myShape.Equals(MyDictionary.Platform.End))
        {
            return 2;
        }

        return 0;
    }

    public bool canBeWalkedBy(char shape)
    {
        switch (myShape)
        {
            case MyDictionary.Platform.High:
            case MyDictionary.Platform.Low:
            case MyDictionary.Platform.Start:
            case MyDictionary.Platform.End:
                return true;

            //Height displacers
            case MyDictionary.Platform.Ramp:
                return shape == MyDictionary.SHAPE_SPHERE;
            case MyDictionary.Platform.Stairs:
                return shape == MyDictionary.SHAPE_CROSS || shape == MyDictionary.SHAPE_CYLINDER_CROSS;

            //Special crossings
            case MyDictionary.Platform.Tunnel:
                return shape == MyDictionary.SHAPE_SPHERE;

            //Everything else
            default:
                return false;
        }
    }

    public float getPlacementHeight()
    {
        switch (myShape)
        {
            case MyDictionary.Platform.High:
            case MyDictionary.Platform.Ramp:
            case MyDictionary.Platform.Stairs:
                return 0.55f;

            case MyDictionary.Platform.Low:
            case MyDictionary.Platform.Tunnel:
            case MyDictionary.Platform.Start:
            case MyDictionary.Platform.End:
                return 0.05f;

            //Everything else
            default: return 0;
        }
    }

    public Boolean canEnterFrom(int direction)
    {
        return thisEntrance == null || !thisEntrance.HasValue || direction == thisEntrance.Value;
    }

    public Boolean canExitTo(int direction)
    {
        return canEnterFrom(4 - direction);
    }

    public MyDictionary.Platform getShape()
    {
        return this.myShape;
    }
}
