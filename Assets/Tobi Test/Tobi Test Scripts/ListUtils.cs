using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ListUtils {

    //if too slow consider caching
    public static float getHighestFloat(List<object> list) {

        float returnValue = 0;
        if (list.Count <= 0) return returnValue;

        if (list[0] is string) return getAmountOfObjects(list);
        else returnValue = (float)list[0];

        foreach (object f in list) {
            try {
                if ((float)f > returnValue) returnValue = (float)f;
            } catch (System.InvalidCastException e) {
                Debug.Log(e + " . " + returnValue.GetType());
            }
            
        }
        return returnValue;
    }

    public static float getLowestFloat(List<object> list) {
        float returnValue = 0;
        if (list.Count <= 0) return returnValue;

        if (list[0] is string) return 0;
        else returnValue = (float)list[0];

        foreach (object f in list) if ((float)f < returnValue) returnValue = (float)f;

        return returnValue;
    }

    public static float getMaxAbsolutAmount(List<object> list) {

        if (list.Count <= 0) return 0;

        if (list[0] is string) return getAmountOfObjects(list) + 1;

        return list.Select(x => Math.Abs((float)x)).ToList().Max();
    }

    private static float getAmountOfObjects(List<object> list) {
            return list.Distinct().Count() - 1;
    }

}