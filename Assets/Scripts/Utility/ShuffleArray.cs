using System.Collections.Generic;

namespace MageBattle.Utility
{
    public static class ShuffleArray
    {
        public static T[] Shuffle<T>(T[] array)
        {
            System.Random random = new System.Random();

            for (int i = 0; i < array.Length - 1; i++)
            {
                int rndIndex = random.Next(i, array.Length);
                T temp = array[rndIndex];
                array[rndIndex] = array[i];
                array[i] = temp;
            }
            return array;
        }

        public static IList<T> Shuffle<T>(IList<T> array)
        {
            System.Random random = new System.Random();

            for (int i = 0; i < array.Count - 1; i++)
            {
                int rndIndex = random.Next(i, array.Count);
                T temp = array[rndIndex];
                array[rndIndex] = array[i];
                array[i] = temp;
            }
            return array;
        }
    }
}