using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// utilities for usage with list objects
/// </summary>
public static class ListUtils {

    //if too slow consider caching
    /// <summary>
    /// return the highest float in the given list
    /// if it is a string list return the count
    /// </summary>
    /// <param name="list">the list to search in</param>
    /// <returns>highest float representive in the list</returns>
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

    /// <summary>
    /// returns the lowest float value found in the list
    /// if the list contains only strings return 0
    /// </summary>
    /// <param name="list">the list to search in</param>
    /// <returns>return the lowest found float value</returns>
    public static float getLowestFloat(List<object> list) {
        float returnValue = 0;
        if (list.Count <= 0) return returnValue;

        if (list[0] is string) return 0;
        else returnValue = (float)list[0];

        foreach (object f in list) if ((float)f < returnValue) returnValue = (float)f;

        return returnValue;
    }

    /// <summary>
    /// return the maximum absolut amount found in the list
    /// if it is a string list return the list count
    /// </summary>
    /// <param name="list">the list to search in</param>
    /// <returns>maximum absolut value in list</returns>
    public static float getMaxAbsolutAmount(List<object> list) {

        if (list.Count <= 0) return 0;

        if (list[0] is string) return getAmountOfObjects(list) + 1;

        return list.Select(x => Math.Abs((float)x)).ToList().Max();
    }

    /// <summary>
    /// get the distinct amount of objects in the given list
    /// </summary>
    /// <param name="list">the list to search in</param>
    /// <returns>distinct count</returns>
    private static float getAmountOfObjects(List<object> list) {
            return list.Distinct().Count() - 1;
    }

}