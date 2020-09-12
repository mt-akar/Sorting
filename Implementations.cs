using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SortAnalysis
{
    public static class ArrayExtender
    {
        #region Private Members

        /* Optimal (best known) sequence of increments for shell sort algorithm (A102549).
         * See this paper at http://sun.aei.polsl.pl/~mciura/publikacje/shellsort.pdf */
        static readonly int[] gaps = { 1750, 701, 301, 132, 57, 23, 10, 4, 1, };

        #endregion

        #region Public Helpers

        /// <summary>
        /// Given an array, creates a deep clone of it.
        /// </summary>
        /// <param name="array">Array to be cloned</param>
        /// <returns></returns>
        public static T[] ArrayDeepClone<T>(this T[] array)
        {
            // If null, return null
            if (array == null) return null;

            // Define new array
            T[] ret = new T[array.Length];

            // Clone the array
            for (int i = 0; i < array.Length; i++)
                ret[i] = array[i];

            return ret;
        }

        /// <summary>
        /// Checks is an <see cref="IComparable"/> array is sorted (non-decreasing). Returns false if null.
        /// </summary>
        /// <typeparam name="T"> IComperable type </typeparam>
        /// <param name="array"> Array to be checked </param>
        /// <returns></returns>
        public static bool IsSorted<T>(T[] array)
           where T : IComparable
        {
            if (array == null) return false;

            for (int i = 1; i < array.Length; i++)
                if (array[i - 1].CompareTo(array[i]) > 0)
                    return false;
            return true;
        }

        /// <summary>
        /// Generates and returns an <see cref="ushort"/> array with given length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static ushort[] GenerateRandomUshortArray(long length)
        {
            if (length < 0) return new ushort[0];

            ushort[] randomArray = new ushort[length];
            Random rand = new Random();

            for (int i = 0; i < length; i++)
                randomArray[i] = (ushort)rand.Next(0, 65536 - 1);

            return randomArray;
        }

        /// <summary>
        /// Generates and returns an <see cref="int"/> array with given length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int[] GenerateRandomIntArray(long length)
        {
            int[] randomArray = new int[length];
            Random rand = new Random();

            for (long i = 0; i < length; i++)
                randomArray[i] = rand.Next(0, Int16.MaxValue - 1);

            return randomArray;
        }

        /// <summary>
        /// Converts contents of an array into a string. (Not perfect)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ArrayToString<T>(this T[] array)
        {
            if (array == null) return "";

            string s = "";

            foreach (T u in array)
                s = s + u.ToString() + ", ";

            return "[" + s + "]";
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Prings a log message to the console with the current thread id.
        /// </summary>
        /// <param name="s">Mesage to be displayed</param>
        static void Log(string s)
        {
            Console.WriteLine($"{s} [{Thread.CurrentThread.ManagedThreadId}]");
        }

        /// <summary>
        /// Swaps two given object
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="a">An object to swap</param>
        /// <param name="b">An object to swap</param>
        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        #endregion

        #region Insertion Sort

        /// <summary>
        /// Insertion sorts an array of <see cref="IComparable"/> elements.
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array">Array to be sorted</param>
        public static void InsertionSort<T>(this T[] array) where T : IComparable
        {
            // If null, return
            if (array == null) return;

            // Every iteration makes sorted list 1 element longer
            for (int i = 1; i < array.Length; i++)
                // Every iteration takes the next unsorted element 1 step closer to its sorted place
                for (int j = i; j > 0 && array[j - 1].CompareTo(array[j]) > 0; j--)
                {
                    T temp = array[j - 1];
                    array[j - 1] = array[j];
                    array[j] = temp;
                }
        }

        /// <summary>
        /// Version of Insertion sort that sorts given interval (bounds included)
        /// Throws error if given array is null
        /// </summary>
        /// <typeparam name="T"> Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array"> Array to be sorted </param>
        /// <param name="initial"> Initial bound of the interval </param>
        /// <param name="final"> Final bound of the interval </param>
        static void IntervalInsertionSort<T>(this T[] array, int initial, int final)
            where T : IComparable
        {
            // No null check since the method is private

            // Insertion sort algorithm in interval [initial, final]
            for (int i = initial + 1; i <= final; i++)
                for (int j = i; j > initial && array[j - 1].CompareTo(array[j]) > 0; j--)
                {
                    T temp = array[j - 1];
                    array[j - 1] = array[j];
                    array[j] = temp;
                }
        }

        #endregion

        #region Bubble Sort

        /// <summary>
        /// Bubble sorts an array of <see cref="IComparable"/> elements.
        /// This is the most otimized implementation of the bubble sort.
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array">Array to be sorted</param>
        public static void BubbleSort<T>(this T[] array)
            where T : IComparable
        {
            // If null, return
            if (array == null) return;

            // Save the location of the last swap so that the sorted part will not be recompared.
            // First bubble loop
            for (int sortedAfter = array.Length, lastComparison = 0; sortedAfter != 0; sortedAfter = lastComparison, lastComparison = 0)
                // Second bubble loop
                for (int j = 1; j < sortedAfter; j++)
                    // If number at smaller index is greater, swap
                    if (array[j - 1].CompareTo(array[j]) > 0)
                    {
                        T temp = array[j - 1];
                        array[j - 1] = array[j];
                        array[j] = temp;

                        // Store last swap index
                        lastComparison = j;
                    }
        }

        #endregion

        #region Merge

        #region Standart Merge Sort

        /// <summary>
        /// Merge sorts an array of <see cref="IComparable"/> elements.
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array">Array to be sorted</param>
        public static void MergeSort<T>(this T[] array)
            where T : IComparable
        {
            // If null or empty, return
            if (array == null || array.Length == 0) return;

            // Call the functional signiture of the merge sort with initial and final elemenets to sort.
            array.MergeSort(0, array.Length - 1);
        }

        /// <summary>
        /// Merge sorts an array of <see cref="IComparable"/> elements.
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array">Array to be sorted.</param>
        /// <param name="initial">Initial index of the elements to be sorted.</param>
        /// <param name="final">Final index of the elements to be sorted.</param>
        static void MergeSort<T>(this T[] array, int initial, int final)
            where T : IComparable
        {
            // If there is only one element to sort, return.
            if (initial == final) return;

            // Recursively sort the first and secod half. If number elements is odd, first half has the excess element.
            array.MergeSort(initial, (initial + final) / 2);
            array.MergeSort((initial + final) / 2 + 1, final);

            // Merge sorted halfs.
            array.Merge(initial, final);
        }

        /// <summary>
        /// Merges sorted first and second halves of an array. If number elements is odd, it is assumed that first half has the excess element.
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array">Array to be merged.</param>
        /// <param name="initial">Initial index of the elements to be merged.</param>
        /// <param name="final">Final index of the elements to be merged.</param>
        static void Merge<T>(this T[] array, int initial, int final)
            where T : IComparable
        {
            // Create a temp array to store merged array
            T[] temp = new T[final - initial + 1];

            int iter1 = initial; // First half iterator
            int iter2 = (initial + final) / 2 + 1; // Second half iterator
            int tempIter = 0; // Merged array iterator

            // While both sides have unsorted elements
            while (iter1 <= (initial + final) / 2 && iter2 <= final)
                if (array[iter1].CompareTo(array[iter2]) < 0)
                    temp[tempIter++] = array[iter1++];
                else
                    temp[tempIter++] = array[iter2++];

            // If all remaining unsorted elements are in first half
            if (iter1 <= (initial + final) / 2)
                while (iter1 <= (initial + final) / 2)
                    temp[tempIter++] = array[iter1++];
            // If all remaining unsorted elements are in second half
            else
                while (iter2 <= final)
                    temp[tempIter++] = array[iter2++];

            // Pass sorted list back to original array
            for (int i = 0; i <= final - initial; i++)
                array[i + initial] = temp[i];
        }

        #endregion

        #region Merge Sort Multithreading

        /// <summary>
        /// Merge sorts an array of <see cref="IComparable"/> elements.
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array">Array to be sorted</param>
        public static void MergeSortMT<T>(this T[] array)
            where T : IComparable
        {
            // If null or empty, return
            if (array == null || array.Length == 0) return;

            // Call the functional signiture of the merge sort with initial and final elemenets to sort.
            array.MergeSortMT(0, array.Length - 1, 1);
        }

        /// <summary>
        /// Merge sorts an array of <see cref="IComparable"/> elements.
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array">Array to be sorted.</param>
        /// <param name="initial">Initial index of the elements to be sorted.</param>
        /// <param name="final">Final index of the elements to be sorted.</param>
        static void MergeSortMT<T>(this T[] array, int initial, int final, int mtCount)
            where T : IComparable
        {
            // If there is only one element to sort, return.
            if (initial == final) return;

            if (mtCount > 0)
            {
                Task t = Task.Run(() =>
                {
                    array.MergeSortMT(initial, (initial + final) / 2, mtCount - 1);
                });
                array.MergeSortMT((initial + final) / 2 + 1, final, mtCount - 1);

                t.Wait();
            }
            else
            {
                array.MergeSort(initial, (initial + final) / 2);
                array.MergeSort((initial + final) / 2 + 1, final);
            }

            #region Commented out the logs
            /*
                if (mtCount > 0)
                {
                    Task t = Task.Run(() =>
                    {
                        Log($"from {initial} to {(initial + final) / 2} is sorted by");
                        array.MergeSortMT(initial, (initial + final) / 2, mtCount - 1);
                    });

                    Log($"from {(initial + final) / 2 + 1} to {final} is sorted by");
                    array.MergeSortMT((initial + final) / 2 + 1, final, mtCount - 1);

                    t.Wait();
                }
                else
                {
                    Log($"from {initial} to {(initial + final) / 2} is sorted by");
                    array.MergeSortMT(initial, (initial + final) / 2, mtCount - 1);


                    Log($"from {(initial + final) / 2 + 1} to {final} is sorted by");
                    array.MergeSortMT((initial + final) / 2 + 1, final, mtCount - 1);
                }

                Log($"from {initial} to {final} is MERGED by");
                */
            #endregion

            array.Merge(initial, final);
        }

        #endregion

        #region Merge-Insertion Sort

        public static void MergeInsertionSort<T>(this T[] array)
            where T : IComparable
        {
            // If null or empty, return
            if (array == null || array.Length == 0) return;

            // Call the functional signiture of the merge sort with initial and final elemenets to sort.
            array.MergeInsertionSort(0, array.Length - 1);
        }

        static void MergeInsertionSort<T>(this T[] array, int initial, int final)
            where T : IComparable
        {
            // If there is only one element to sort, return.
            if (initial == final) return;

            if (final - initial > 43)
            {
                // Recursively sort the first and secod half. If number elements is odd, first half has the excess element.
                array.MergeInsertionSort(initial, (initial + final) / 2);
                array.MergeInsertionSort((initial + final) / 2 + 1, final);

                // Merge sorted halfs.
                array.Merge(initial, final);
            }
            else
            {
                array.IntervalInsertionSort(initial, final);
            }
        }

        public static void MergeShellSort<T>(this T[] array)
            where T : IComparable
        {
            // If null or empty, return
            if (array == null || array.Length == 0) return;

            // Call the functional signiture of the merge sort with initial and final elemenets to sort.
            array.MergeShellSort(0, array.Length - 1);
        }

        static void MergeShellSort<T>(this T[] array, int initial, int final)
            where T : IComparable
        {
            // If there is only one element to sort, return.
            if (initial == final) return;

            if (final - initial > 70)
            {
                // Recursively sort the first and secod half. If number elements is odd, first half has the excess element.
                array.MergeShellSort(initial, (initial + final) / 2);
                array.MergeShellSort((initial + final) / 2 + 1, final);

                // Merge sorted halfs.
                array.Merge(initial, final);
            }
            else
            {
                array.IntervalShellSort(initial, final);
            }
        }

        #endregion

        #region Merge-Insertion Multithreading

        public static void MergeInsertionSortMT<T>(this T[] array)
            where T : IComparable
        {
            // If null or empty, return
            if (array == null || array.Length == 0) return;

            // Call the functional signiture of the merge sort with initial and final elemenets to sort.
            array.MergeInsertionSortMT(0, array.Length - 1, 1);
        }

        static void MergeInsertionSortMT<T>(this T[] array, int initial, int final, int mtCount)
            where T : IComparable
        {
            if (final - initial > 43)
            {
                if (mtCount > 0)
                {
                    // If there is only one element to sort, return.
                    if (initial == final) return;

                    Task t = Task.Run(() =>
                    {
                        array.MergeInsertionSortMT(initial, (initial + final) / 2, mtCount - 1);
                    });
                    array.MergeInsertionSortMT((initial + final) / 2 + 1, final, mtCount - 1);

                    t.Wait();
                }
                else
                {
                    array.MergeInsertionSort(initial, (initial + final) / 2);
                    array.MergeInsertionSort((initial + final) / 2 + 1, final);
                }
            }
            else
            {
                array.IntervalInsertionSort(initial, final);
            }

            // Merge sorted halfs.
            array.Merge(initial, final);
        }

        public static void MergeShellSortMT<T>(this T[] array)
            where T : IComparable
        {
            // If null or empty, return
            if (array == null || array.Length == 0) return;

            // Call the functional signiture of the merge sort with initial and final elemenets to sort.
            array.MergeShellSortMT(0, array.Length - 1, 4);
        }

        static void MergeShellSortMT<T>(this T[] array, int initial, int final, int mtCount)
            where T : IComparable
        {
            if (final - initial > 43)
            {
                if (mtCount > 0)
                {
                    // If there is only one element to sort, return.
                    if (initial == final) return;

                    Task t = Task.Run(() =>
                    {
                        array.MergeShellSortMT(initial, (initial + final) / 2, mtCount - 1);
                    });
                    array.MergeShellSortMT((initial + final) / 2 + 1, final, mtCount - 1);

                    t.Wait();
                }
                else
                {
                    array.MergeShellSort(initial, (initial + final) / 2);
                    array.MergeShellSort((initial + final) / 2 + 1, final);
                }
            }
            else
            {
                array.IntervalShellSort(initial, final);
            }

            // Merge sorted halfs.
            array.Merge(initial, final);
        }

        #endregion Merge-Insertion Multithreading

        #endregion Merge

        #region QuickSort

        public static void QuickSort<T>(this T[] array)
            where T : IComparable
        {
            if (array == null) return;

            array.QuickSort(0, array.Length - 1);
        }

        static void QuickSort<T>(this T[] array, int initial, int final)
            where T : IComparable
        {
            int pivotIndex = Median3(0, (final - initial) / 2, final, array);



        }

        #region quicksort homewok in C++
        /*
            void quickSort(int* arr, int size, int &compCount, int &moveCount){
                if (size <= 1)
                    return;

                int piv = arr[size - 1], front = 0, back = size - 2;
                moveCount++;

                while (front < back)
                {

                    compCount++;
                    while (arr[front] < piv)
                    {
                        compCount++;
                        front++;
                    }

                    compCount++;
                    while (front < back && arr[back] >= piv)
                    {
                        compCount++;
                        back--;
                    }

                    if (front < back)
                    {
                        arr[front] = arr[back] + arr[front];
                        arr[back] = arr[front] - arr[back];
                        arr[front] = arr[front] - arr[back];
                        moveCount += 3;
                    }
                }
                int temp = arr[front];
                arr[front] = arr[size - 1];
                arr[size - 1] = temp;
                moveCount += 3;

                quickSort(arr, front, compCount, moveCount);
                quickSort((arr + front + 1), size - front - 1, compCount, moveCount);
                */
        #endregion

        /// <summary>
        /// Given an array of <see cref="IComparable"/> object and 3 indexes,
        /// finds the median of the objects at those indexes.
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="a">First index</param>
        /// <param name="b">Second index</param>
        /// <param name="c">Third index</param>
        /// <param name="arr">Array</param>
        /// <returns></returns>
        static int Median3<T>(int a, int b, int c, T[] arr)
            where T : IComparable
        {
            // Simple, optimized median finding algorithm for 3 elements
            if (arr[a].CompareTo(arr[b]) >= 0) // 8 possiblity out of 13
                if (arr[b].CompareTo(arr[c]) >= 0) // 4 possiblity
                    return b;
                else if (arr[c].CompareTo(arr[a]) >= 0) // 3 possiblity
                    return a;
                else // 1 possiblity
                    return c;
            else if (arr[a].CompareTo(arr[c]) >= 0) // 2 possiblity
                return a;
            else if (arr[c].CompareTo(arr[b]) >= 0) // 2 possiblity
                return b;
            else // 1 possiblity
                return c;
        }

        #endregion

        #region Shell Sort

        /// <summary>
        /// Shell sorts an array of <see cref="IComparable"/> elements.
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array">Array to be sorted</param>
        public static void ShellSort<T>(this T[] array)
            where T : IComparable
        {
            // If null, return
            if (array == null) return;

            // Fing which gap is going to be the first
            // Fist gap is the greatest gap that is smaller or equal to array.Length / 2
            int gapIndex = 0;
            for (; gapIndex < gaps.Length - 1 && gaps[gapIndex] > array.Length / 2; gapIndex++) { }

            // Use decreasing gaps
            for (; gapIndex < gaps.Length; gapIndex++)
            {
                int gap = gaps[gapIndex];
                // Iterate array starting from gap'th number to the end
                for (int i = gap; i < array.Length; i++)
                    // Swap numbers with "gap" gap going from right to left
                    for (int j = i; j >= gap; j -= gap)
                        if (array[j - gap].CompareTo(array[j]) <= 0) break;
                        else
                        {
                            T temp = array[j - gap];
                            array[j - gap] = array[j];
                            array[j] = temp;
                        }

                ;
            }
        }

        /// <summary>
        /// Version of Shell sort that sorts given interval (bounds included)
        /// </summary>
        /// <typeparam name="T">Array element of type <see cref="IComparable"/></typeparam>
        /// <param name="array">Array to be sorted.</param>
        /// <param name="initial">Initial bound of the interval</param>
        /// <param name="final">Final bound of the interval</param>
        public static void IntervalShellSort<T>(this T[] array, int initial, int final)
            where T : IComparable
        {
            // If null, return
            if (array == null) return;

            // Fing which gap is going to be the first
            int gapIndex = 0;
            for (; gapIndex < 7; gapIndex++)
                // Fist gap is the greatest gap that is smaller or equal to array.Length / 2
                if (gaps[gapIndex] <= (final - initial + 1) / 2)
                    break;

            // Use decreasing gaps

            for (int gap; gapIndex <= 8; gapIndex++)
            {
                gap = gaps[gapIndex];
                // Iterate array starting from gap'th number to the end
                for (int i = initial + gap; i <= final; i++)
                    // Swap numbers with 'gap' gap going from right to left
                    for (int j = i; j >= initial + gap; j -= gap)
                        if (array[j - gap].CompareTo(array[j]) > 0)
                        {
                            T temp = array[j - gap];
                            array[j - gap] = array[j];
                            array[j] = temp;
                        }
                        else break;
            }
        }

        #endregion

        #region Counting Sort

        /// <summary>
        /// Counting sorts an array of <see cref="ushort"/> (of size at most Int.MaxValue).
        /// Initial array stays unsorted. Sorted array is returned.
        /// </summary>
        /// <param name="array"> Array to be sorted. </param>
        public static ushort[] CountingSort(this ushort[] array)
        {
            // If array is null, simply return null.
            if (array == null) return null;

            // Create a "counting array" of length ushort.MaxValue
            int[] count = new int[ushort.MaxValue + 1];

            // Count number of appereances of each element is the array.
            foreach (ushort i in array)
                count[i]++;

            // Calculate the final position of each set of same elements in the array.
            for (int i = 1; i < ushort.MaxValue + 1; i++)
                count[i] += count[i - 1];

            // Initialize the output array.
            ushort[] outp = new ushort[array.Length];

            // Construct the output array.
            foreach (ushort i in array)
                outp[--count[i]] = i;

            // Return the output array.
            return outp;
        }

        /// <summary>
        /// This method is created to analyse the different parts of this algorithm and how long does they take.
        /// </summary>
        /// <param name="array"> Array to be sorted </param>
        /// <param name="w"> Array of stopwatches given by ref. Length has to be 5. </param>
        public static ushort[] CountingSort(ushort[] array, ref Stopwatch[] w)
        {
            if (array == null) return null;
            if (w.Length != 5) return null;

            w[0].Start();
            int[] count = new int[65536];
            w[0].Stop();

            w[1].Start();
            foreach (ushort i in array)
                count[i]++;
            w[1].Stop();

            w[2].Start();
            for (int i = 1; i < 65536; i++)
                count[i] += count[i - 1];
            w[2].Stop();

            w[3].Start();
            ushort[] outp = new ushort[array.Length];
            w[3].Stop();

            w[4].Start();
            foreach (ushort i in array)
                outp[count[i]-- - 1] = i;
            w[4].Stop();

            return outp;
        }

        #endregion
    }
}
