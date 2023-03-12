using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DailyProgrammerChallenge398Difficult
{
    public static class ArrayExtension
    {

        public static IEnumerable<T> GetRow<T>(this T[,] array, int rowNumber)  where T : IComparable<T>
                                                                    
        { 
            var holdingStructure = Enumerable.Empty<T>();
            for (int m = 0; m < array.GetLength(1); m++)
            {
                holdingStructure = holdingStructure.Concat(new[] {array[rowNumber, m] });
            }
           
            return holdingStructure;
        }

        public static IEnumerable<T> GetColumn<T>(this T[,] array, int columnNumber) where T : IComparable<T>
        {
            var holdingStructure = Enumerable.Empty<T>();
            for (int n = 0; n < array.GetLength(0); n++)
            {
                holdingStructure = holdingStructure.Concat(new[] { array[n, columnNumber] });
            }

            return holdingStructure;
        }

        public static IEnumerable<long?> GetRowNullableLong(this long?[,]? array, int rowNumber) 

        {
            if (array == null) return Enumerable.Empty<long?>();

            var holdingStructure = Enumerable.Empty<long?>();
            for (int m = 0; m < array.GetLength(1); m++)
            {
                holdingStructure = holdingStructure.Concat(new[] { array[rowNumber, m] });
            }

            return holdingStructure;
        }

        public static IEnumerable<long?> GetColumnNullableLong(this long?[,]? array, int columnNumber) 

        {
            if (array == null) return Enumerable.Empty<long?>();

            var holdingStructure = Enumerable.Empty<long?>();
            for (int n = 0; n < array.GetLength(0); n++)
            {
                holdingStructure = holdingStructure.Concat(new[] { array[n, columnNumber] });
            }

            return holdingStructure;
        }
        
        public static T FindMinimumValueInMatrix<T>(this T[,] array) where T: IComparable<T>
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var minimumValue = array[0, 0];

            for (var i = 0; i < array.GetLength(0); i++)
            {
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    var value = array[i, j];
                    if (minimumValue is null || minimumValue.CompareTo(value) > 0)
                    {
                        minimumValue = value;
                    }
                }
            }

            return minimumValue;   
        }

        public static long? FindMinimumValueInNullableMatrix(this long?[,]? array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            var minimumValue = array[0, 0];

            for (var i = 0; i < array.GetLength(0); i++)
            {
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    var value = array[i, j];
                    if (minimumValue is null ||(value != null && minimumValue > value))
                    {
                        minimumValue = value;
                    }
                }
            }

            return minimumValue;
        }
    }
}
