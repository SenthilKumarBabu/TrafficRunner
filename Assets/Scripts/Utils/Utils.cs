using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{

    #region Sort

    public static List<int> SortAscending(List<int> originalList) {
        return originalList.OrderBy(i => i).ToList();
    }

    public static List<int> SortDescending(List<int> originalList) {
        return originalList.OrderByDescending(i => i).ToList();
    }

    #endregion

    #region List

    public static List<int> GetRandomNonRepeatList(List<int> originalList,int randomCount) {
        if (randomCount > originalList.Count) {
            Debug.Log("Input List Size is small");
            return null;
        }

        List<int> list = new List<int>();
        for (int i = 0; i < randomCount; i++) {
            int randomIndex = Random.Range(0, originalList.Count - 1);
            list.Add(originalList[randomIndex]);
            originalList.RemoveAt(randomIndex);
        }

        return list;
    }

   

    public static void GetOddEvenList(List<int> originalList,ref List<int> oddList,ref List<int> evenList) {
        oddList.Clear();
        evenList.Clear();
        foreach (int numbers in originalList) {
            if (numbers % 2 == 0) {
                evenList.Add(numbers);
            } else {
                oddList.Add(numbers);
            }
        }
    }

    #endregion

    #region Random
    
    public static int GetRandomIndex<T>(List<T> list)
    {
        return Random.Range(0, list.Count);
    }
    
    public static int GetRandomIndex<T>(T[] array)
    {
        return Random.Range(0, array.Length);
    }

    #endregion

}
