using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ListUtils {

    //if too slow consider caching
    public static float getHighestFloat(List<object> list) {
        
        if (list[0] is string) {
            return list.Distinct().Count() - 1;
        }

        float returnValue = 0;
        if (list.Count > 0) returnValue = (float) list[0];

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
        if (list[0] is string) {
            return 0;
        }
        if (list.Count > 0) returnValue = (float)list[0];

        foreach (object f in list) {
            if ((float)f < returnValue) returnValue = (float)f;
        }
        return returnValue;
    }

}
