using UnityEngine;
using System.Collections;

public static class MathUtils {
    /// <summary>
    /// Return array of random int numbers which are different each other from min(inclusive) to max(inclusive)
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="amount">Number of result number</param>
    /// <returns></returns>
    public static int[] RandomDistinctNumber(int min, int max, int amount) {

        if (amount > max - min + 1 || amount <= 0)
            return null;

        int[] domainArray = new int[max - min + 1];
        int[] resultArray = new int[amount];

        for (int i = 0; i < domainArray.Length - 1; i++) {
            domainArray[i] = min + i;
        }

        int tmpLength = domainArray.Length;
        for (int k = 0; k < amount; k++) {
            int domainIndex = Random.Range(0, tmpLength);
            resultArray[k] = domainArray[domainIndex];

            int tmp = domainArray[domainIndex];
            domainArray[domainIndex] = domainArray[tmpLength - 1];
            domainArray[tmpLength - 1] = tmp;

            tmpLength--;
        }

        return resultArray;
    }

    public static int[] RandomSubIntArray(int[] arr, int subLength){
        return RandomDistinctNumber(0, arr.Length - 1, subLength);
    }

    /// <summary>
    /// Random a subset of Component[] that elements are different each other.
    /// </summary>
    /// <param name="components"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static Transform[] RandomSubComponentArray(Transform[] components, int amount){
        if (amount <= 0 || components == null || amount > components.Length)
            return null;

        int[] randomIndexs = RandomDistinctNumber(0, components.Length - 1, amount);

        Transform[] resultArray = new Transform[amount];
        for (int i = 0; i < amount; i++) {
            resultArray[i] = components[randomIndexs[i]];
        }

        return resultArray;
    }

    /// <summary>
    /// Random positive or negative sign
    /// </summary>
    /// <param name="chancePositive">Chance of random positive sign. 0 equals 0%, 1 equals 100%.</param>
    /// <returns>1 if positive, -1 if negative</returns>
    public static int RandomSign(float chancePositive)
    {
        return Random.value <= chancePositive ? 1 : -1;
    }

    public static bool RandomBoolean(float trueChance)
    {
        return Random.value <= trueChance ? true : false;
    }

    public static bool RandomBoolean()
    {
        return Random.value <= 0.5f ? true : false;
    }

    public static int[] GetChanceArray(float[] chanceArray)
    {
        int[] indexArray = new int[10];

        int tmpIndex = 0;

        for (int i = 0; i < chanceArray.Length; i++)
        {
            for (int k = 0; k < chanceArray[i] * 10f; k++)
            {
                indexArray[tmpIndex] = i;
                tmpIndex++;
            }
        }

        return indexArray;
    }

    public static int RandomArrayIndexByChance(float[] chanceArray)
    {
        int[] indexArray = GetChanceArray(chanceArray);

        return indexArray[Random.Range(0, indexArray.Length)];
    }

    public static Object GetRandomArrayElement(Object[] array)
    {
        return array[Random.Range(0, array.Length - 1)];    
    }
}
