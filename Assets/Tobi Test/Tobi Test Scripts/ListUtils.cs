using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListUtils {

    public static float getHighestFloat(List<object> list) {
        float returnValue = 0;
        if (list[0] is string) {
            return list.Count;
        }
        if (list.Count > 0) returnValue = (float) list[0];

        foreach (object f in list) {
            if ((float)f > returnValue) returnValue = (float)f;
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
