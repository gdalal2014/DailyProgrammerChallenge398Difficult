using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace DailyProgrammerChallenge398Difficult
{

    /// <summary>
    /// This is a minimization problem and uses the Hungarian Algorithm to find the minimum sum of a matrix where a column and row do not repeat
    /// 
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please Provide an argument");
            }

            var testMatrix = new long[,]{{ 123456789, 752880530, 826085747, 576968456, 721429729},
                                        {173957326, 1031077599,  407299684,  67656429,    96549194  },
                                        {1048156299, 663035648,   604085049,  1017819398,  325233271 },
                                        {942914780, 664359365,   770319362,  52838563,   720059384 },
                                        {472459921, 662187582,   163882767,  987977812,   394465693 },
                                       };
            var smallestSumValueTest = SmallestSum(testMatrix, testMatrix.GetLength(0), testMatrix.GetLength(1));
            var sumOfDigitsTest = smallestSumValueTest.ToString().Sum(c => c - '0');

            for (int i = 0; i < args.Length; i++)
            {
                var timer = new Stopwatch();
                timer.Start();
                var stringPath = string.IsNullOrWhiteSpace(args[i]) ? null : args[i];

                if (stringPath == null) { Console.WriteLine("Please Provide a path to the argument"); }

                var fileValue = System.IO.File.ReadAllText(stringPath);

                (var matrixArray, var length, var width) = CreateArray(fileValue);

                var smallestSumValue = SmallestSum(matrixArray, length, width);
                var sumOfDigits = smallestSumValue.ToString().Sum(c => c - '0');

                timer.Stop();
                Console.WriteLine($"The Smallest Sum is: {smallestSumValue} and the sum of the digits are {sumOfDigits}");
                Console.WriteLine($"It took {timer.Elapsed} to process");
            }
        }

        public static (long[,], int, int) CreateArray(string value)
        {
            var regex = new Regex("[\\n\\r\\s]+");
            var listOfValues = regex.Split(value).ToList();
            var length = (int)Math.Floor(Math.Sqrt(listOfValues.Count));
            var width = (int)Math.Floor(Math.Sqrt(listOfValues.Count));
            long[,] matrix2020 = new long[length, width];
            var elementCount = 0;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    var item = long.Parse(listOfValues[elementCount].Trim());
                    matrix2020[i, j] = item;
                    elementCount++;

                }
            }

            return (matrix2020, length, width);
        }

        public static long SmallestSum(long[,] matrix, int length, int width)
        {
            var listOfMinimumsInEachRow = new List<long>();
            var listOfMinimumsInEachColumn = new List<long>();
            long[,] updatedMatrix = new long[length, width];

            for (int n = 0; n < length; n++)
            {
                var minValue = matrix[n, 0];

                for (int m = 0; m < width; m++)
                {
                    var value = matrix[n, m];
                    if (value < minValue) minValue = value;
                }
                listOfMinimumsInEachRow.Add(minValue);
            }

            for (int n = 0; n < length; n++)
            {
                var minimumValue = listOfMinimumsInEachRow[n];

                for (int m = 0; m < width; m++)
                {
                    var value = matrix[n, m];
                    var subtractedValue = value - minimumValue;
                    updatedMatrix[n, m] = subtractedValue;
                }

            }

            for (int m = 0; m < width; m++)
            {
                var minValue = updatedMatrix[0, m];

                for (int n = 0; n < length; n++)
                {
                    var value = updatedMatrix[n, m];
                    if (value < minValue) minValue = value;
                }
                listOfMinimumsInEachColumn.Add(minValue);
            }

            for (int m = 0; m < width; m++)
            {
                var minimumValue = listOfMinimumsInEachColumn[m];

                for (int n = 0; n < length; n++)
                {
                    var value = updatedMatrix[n, m];
                    var subtractedValue = value - minimumValue;
                    updatedMatrix[n, m] = subtractedValue;
                }
            }

            long smallestSum = computeSmallestSum(matrix, updatedMatrix, length, width);

            return smallestSum;
        }

        public static long computeSmallestSum(long[,] matrix, long[,] updatedMatrix, int length, int width)
        {
            long smallestSum = 0;
            var rowHasZero = new Dictionary<int, List<int>>();
            var columnHasZero = new Dictionary<int, List<int>>();
            var zeroCoordinates = new List<(int, int, long)>();
            var nonZeroCoordinates = new List<(int, int, long)>();
            var rowAccessed = new Dictionary<int, bool>();
            var columnAccessed = new Dictionary<int, bool>();
            long?[,]? nullableMatrix = new long?[length, width];
            for (int i = 0; i < width; i++)
            {
                rowHasZero.Add(i, new List<int>());
                columnHasZero.Add(i, new List<int>());
                rowAccessed.Add(i, false);
                columnAccessed.Add(i, false);
            }


            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    nullableMatrix[i, j] = updatedMatrix[i, j];
                }
            }


            (updatedMatrix, nullableMatrix, var coveredColumnsAndRows, var rowsCovered, var columnsCovered) = coverAllColumnsAndRows(updatedMatrix, nullableMatrix);


            if (coveredColumnsAndRows > 0 && coveredColumnsAndRows < width)
            {
                updatedMatrix = UpdateSoAllRowsAndColumnsAreCovered(matrix, updatedMatrix, nullableMatrix, rowsCovered, columnsCovered, width - coveredColumnsAndRows);
            }


            for (int n = 0; n < length; n++)
            {
                for (int m = 0; m < width; m++)
                {
                    var value = updatedMatrix[n, m];

                    if (value == 0)
                    {
                        var arrayAtRow = rowHasZero[n];
                        arrayAtRow.Add(m);
                        rowHasZero[n] = arrayAtRow;
                        var arrayAtColumn = columnHasZero[n];
                        arrayAtColumn.Add(m);
                        columnHasZero[m] = arrayAtColumn;
                        var originalValue = matrix[n, m];
                        zeroCoordinates.Add((n, m, originalValue));
                    }
                    else
                    {
                        var originalValue = matrix[n, m];
                        nonZeroCoordinates.Add((n, m, originalValue));
                    }
                }
            }


            var countValues = 0;
            for (int i = 0; i < zeroCoordinates.Count; i++)
            {
                (var row, var column, var value) = zeroCoordinates[i];

                var ListOfRows = zeroCoordinates.Where(coord => coord.Item1 == row).ToList();
                var ListOfColumns = zeroCoordinates.Where(coord => coord.Item1 != row && coord.Item2 == column).ToList();

                var listOfValues = rowHasZero[row];

                if (ListOfRows.Count > 1 && ListOfColumns.Count == 1)
                {
                    var firstItem = ListOfColumns.FirstOrDefault();
                    var listOfMatchingColumns = zeroCoordinates.Where(item => item.Item1 == firstItem.Item1).ToList();

                    if (listOfMatchingColumns.Count == 1)
                    {
                        continue;
                    }


                }

                if (listOfValues.Count > 1)
                {
                    long? minValue = null;
                    int? minColumn = null;
                    for (int j = 0; j < listOfValues.Count; j++)
                    {
                        var columnToCheck = listOfValues[j];
                        if (!columnAccessed[columnToCheck])
                        {
                            var valueAtThatColumn = matrix[row, columnToCheck];
                            if (minValue is null || minValue < valueAtThatColumn)
                            {
                                minValue = valueAtThatColumn;
                                minColumn = columnToCheck;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (minValue is not null && minColumn is not null)
                    {
                        rowAccessed[(row)] = true;
                        columnAccessed[((int)minColumn)] = true;
                        smallestSum += (long)minValue;
                        countValues++;
                    }
                }

                if (!rowAccessed[(row)] && !columnAccessed[column])
                {
                    rowAccessed[(row)] = true;
                    columnAccessed[(column)] = true;
                    smallestSum += value;
                    countValues++;
                }
            }


            return smallestSum;
        }

        private static long[,] UpdateSoAllRowsAndColumnsAreCovered(long[,] originalMatrix, long[,] updatedMatrix, long?[,]? nullableMatrix, Dictionary<int, bool> rowsCovered, Dictionary<int, bool> columnsCovered, int columnsToCover)
        {
            var returnMatrix = new long[updatedMatrix.GetLength(0), updatedMatrix.GetLength(1)];
            var notOneZeroPerColumnAndRow = true;
            var coveredColumnsAndRows = 0;
            while (columnsToCover > 0 || notOneZeroPerColumnAndRow)
            {
                if (updatedMatrix == null) { throw new ArgumentNullException(nameof(updatedMatrix)); }
                if (nullableMatrix == null) { throw new ArgumentNullException(nameof(nullableMatrix)); }

                var listOfUncoveredValuesCoordinates = new List<(int, int)>();
                var listOfCoveredValuesCoordinates = new List<(int, int)>();
                for (var row = 0; row < nullableMatrix.GetLength(0); row++)
                {
                    for (var column = 0; column < updatedMatrix.GetLength(1); column++)
                    {
                        var value = nullableMatrix[row, column];

                        if (value is not null)
                        {
                            listOfUncoveredValuesCoordinates.Add((row, column));
                        }
                        else
                        {
                            listOfCoveredValuesCoordinates.Add((row, column));
                        }

                    }
                }



                var minValueInMatrix = nullableMatrix.FindMinimumValueInNullableMatrix();

                if (minValueInMatrix is not null)
                {
                    var listOfRowsNotCovered = listOfUncoveredValuesCoordinates.Select(item => item.Item1).Distinct().ToList();

                    for (int i = 0; i < listOfRowsNotCovered.Count; i++)
                    {
                        var row = listOfRowsNotCovered[i];
                        for (int column = 0; column < updatedMatrix.GetLength(1); column++)
                        {
                            updatedMatrix[row, column] = updatedMatrix[row, column] - (long)minValueInMatrix;
                        }
                    }

                    var listOfColumnsCovered = listOfCoveredValuesCoordinates.Select(item => item.Item2).Distinct().ToList();
                    for (int i = 0; i < listOfColumnsCovered.Count; i++)
                    {
                        var column = listOfColumnsCovered[i];
                        for (int row = 0; row < updatedMatrix.GetLength(0); row++)
                        {
                            updatedMatrix[row, column] = updatedMatrix[row, column] + (long)minValueInMatrix;
                        }

                    }

                    for (int i = 0; i < updatedMatrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < updatedMatrix.GetLength(1); j++)
                        {
                            nullableMatrix[i, j] = updatedMatrix[i, j];
                        }
                    }


                    (updatedMatrix, nullableMatrix, coveredColumnsAndRows, rowsCovered, columnsCovered) = coverAllColumnsAndRows(updatedMatrix, nullableMatrix);
                }
                else
                {
                    for (int i = 0; i < updatedMatrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < updatedMatrix.GetLength(1); j++)
                        {
                            nullableMatrix[i, j] = updatedMatrix[i, j];
                        }
                    }


                    (updatedMatrix, nullableMatrix, coveredColumnsAndRows, rowsCovered, columnsCovered) = coverAllColumnsAndRows(updatedMatrix, nullableMatrix);
                }
                
                columnsToCover = updatedMatrix.GetLength(0) - coveredColumnsAndRows;

                if (columnsToCover == 1 && updatedMatrix.GetLength(0)>20)
                {

                    var countCovered = 0;
                    for (int i = 0; i < nullableMatrix.GetLength(0); i++)
                    {
                        var rowList = nullableMatrix.GetRowNullableLong(i).ToList();
                        
                        if (rowList.Where(x=> x is null).Count() == nullableMatrix.GetLength(0))
                        {
                            countCovered++;

                        }
                    }
                    for (int i = 0; i < nullableMatrix.GetLength(1); i++)
                    {
                        var columnList = nullableMatrix.GetColumnNullableLong(i).ToList();

                        if (columnList.Where(x => x is null).Count() == nullableMatrix.GetLength(1))
                        {
                            countCovered++;

                        }
                    }
                }

                if (columnsToCover == 0 )
                {  
                    notOneZeroPerColumnAndRow = checkToSeeIfOneZeroPerColumnRow(updatedMatrix, originalMatrix);  
                }
            }

            for (var row = 0; row < updatedMatrix.GetLength(0); row++)
            {
                for (var column = 0; column < updatedMatrix.GetLength(1); column++)
                {
                    returnMatrix[row, column] = updatedMatrix[row, column];

                }
            }


            return returnMatrix;
        }


        private static (long[,], long?[,]?, int, Dictionary<int, bool>, Dictionary<int, bool>) coverAllColumnsAndRows(long[,] updatedMatrix, long?[,]? nullableMatrix)
        {
            if (nullableMatrix is null) throw new NullReferenceException(nameof(nullableMatrix));   

            var coveredColumnsAndRows = 0;
            var length = updatedMatrix.GetLength(1);
            var width = updatedMatrix.GetLength(1);

            var rowCovered = new Dictionary<int, bool>();
            var columnCovered = new Dictionary<int, bool>();



            for (int i = 0; i < width; i++)
            {

                rowCovered.Add(i, false);
                columnCovered.Add(i, false);
            }

           

            for (int m = 0; m < width; m++)
            {
                var columnList = nullableMatrix.GetColumnNullableLong(m).ToList();
                var rowList = nullableMatrix.GetRowNullableLong(m).ToList();
                var columnListCount = columnList.Where(x => x == 0).Count();
                var rowListCount = rowList.Where(x => x == 0).Count();

                if (columnListCount > rowListCount)
                {


                    for (int n = 0; n < width; n++)
                    {
                        nullableMatrix[n, m] = null;
                    }
                    columnCovered[m] = true;
                    coveredColumnsAndRows++;
                }
            }



            for (int n = 0; n < length; n++)
            {
                var rowList = nullableMatrix.GetRowNullableLong(n).ToList();
                if (rowList.Where(x => x == 0).Count() >= 1)
                {


                    for (int m = 0; m < width; m++)
                    {
                        nullableMatrix[n, m] = null;
                    }
                    rowCovered[n] = true;
                    coveredColumnsAndRows++;


                }
            }



            var minimumValueInNullMatrix = nullableMatrix.FindMinimumValueInNullableMatrix();

            if (minimumValueInNullMatrix is null || minimumValueInNullMatrix is 0)
            {

                for (int n = 0; n < length; n++)
                {
                    var rowList = nullableMatrix.GetRowNullableLong(n).ToList();
                    if (rowList.Where(x => x == 0).Count() >= 1)
                    {


                        for (int m = 0; m < width; m++)
                        {
                            nullableMatrix[n, m] = null;
                        }
                        rowCovered[n] = true;
                        coveredColumnsAndRows++;
                    }
                }

            }
            return (updatedMatrix, nullableMatrix, coveredColumnsAndRows, rowCovered, columnCovered);
        }

        
        private static bool checkToSeeIfOneZeroPerColumnRow(long[,] updatedMatrix, long[,] originalMatrix)
        {
            var notOneZeroPerColumnRow = false;
            var coordinates = new List<(int, int)>();

            for (var n = 0;n < updatedMatrix.GetLength(0); n++)
            {
                for (var m=0; m < updatedMatrix.GetLength(1); m++)
                {
                    var value = updatedMatrix[n, m];

                    if (value == 0)
                    {
                        coordinates.Add((n, m));
                    }
                    
                }
            }

            for (var i = 0; i < coordinates.Count; i++)
            {
                if (notOneZeroPerColumnRow) break;

                (var row, var column) = coordinates[i];

                var listOfAllMatchingRows = coordinates.Where(item => item.Item1 == row).ToList();

                var listOfAllMatchingColumns = coordinates.Where(item => item.Item2 == column).ToList();

                if (listOfAllMatchingRows.Count == 1 && listOfAllMatchingColumns.Count != 1)
                {
                    for (int j = 0; j < listOfAllMatchingRows.Count; j++)
                    {
                        if (notOneZeroPerColumnRow) break; 

                        (var row2, var column2) = listOfAllMatchingRows[j];
                        var listOfRowsWithOtherMatchingColumns = coordinates.Where(item => item.Item1 != row && item.Item2 == column).ToList();
                        foreach (var item in listOfRowsWithOtherMatchingColumns)
                        {
                            var itemToCheck = item.Item1;
                            var countOfCoordinates = coordinates.Where(coord => coord.Item1 == itemToCheck).ToList();
                            if (countOfCoordinates.Count == 1)
                            { 
                                notOneZeroPerColumnRow = true;
                                break;
                            }

                        }
                    }
                    
                }

            }



            if (!notOneZeroPerColumnRow)
            {
                var rowHasZero = new Dictionary<int, List<int>>();
                var columnHasZero = new Dictionary<int, List<int>>();
                var zeroCoordinates = new List<(int, int, long)>();
                var nonZeroCoordinates = new List<(int, int, long)>();
                var rowAccessed = new Dictionary<int, bool>();
                var columnAccessed = new Dictionary<int, bool>();
                for (int i = 0; i < originalMatrix.GetLength(0); i++)
                {
                    rowHasZero.Add(i, new List<int>());
                    columnHasZero.Add(i, new List<int>());
                    rowAccessed.Add(i, false);
                    columnAccessed.Add(i, false);
                }

                for (int n = 0; n < originalMatrix.GetLength(0); n++)
                {
                    for (int m = 0; m < originalMatrix.GetLength(1); m++)
                    {
                        var value = updatedMatrix[n, m];

                        if (value == 0)
                        {
                            var arrayAtRow = rowHasZero[n];
                            arrayAtRow.Add(m);
                            rowHasZero[n] = arrayAtRow;
                            var arrayAtColumn = columnHasZero[n];
                            arrayAtColumn.Add(m);
                            columnHasZero[m] = arrayAtColumn;
                            var originalValue = originalMatrix[n, m];
                            zeroCoordinates.Add((n, m, originalValue));
                        }
                        else
                        {
                            var originalValue = originalMatrix[n, m];
                            nonZeroCoordinates.Add((n, m, originalValue));
                        }
                    }
                }


                for (int i = 0; i < zeroCoordinates.Count; i++)
                {
                    (var row, var column, var value) = zeroCoordinates[i];

                    var ListOfRows = zeroCoordinates.Where(coord => coord.Item1 == row).ToList();
                    var ListOfColumns = zeroCoordinates.Where(coord => coord.Item1 != row && coord.Item2 == column).ToList();

                    var listOfValues = rowHasZero[row];

                    if (ListOfRows.Count > 1 && ListOfColumns.Count == 1)
                    {
                        var firstItem = ListOfColumns.FirstOrDefault();
                        var listOfMatchingColumns = zeroCoordinates.Where(item => item.Item1 == firstItem.Item1).ToList();

                        if (listOfMatchingColumns.Count == 1)
                        {
                            continue;
                        }


                    }

                    if (listOfValues.Count > 1)
                    {
                        long? minValue = null;
                        int? minColumn = null;
                        for (int j = 0; j < listOfValues.Count; j++)
                        {
                            var columnToCheck = listOfValues[j];
                            if (!columnAccessed[columnToCheck])
                            {
                                var valueAtThatColumn = originalMatrix[row, columnToCheck];
                                if (minValue is null || minValue < valueAtThatColumn)
                                {
                                    minValue = valueAtThatColumn;
                                    minColumn = columnToCheck;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (minValue is not null && minColumn is not null)
                        {
                            rowAccessed[(row)] = true;
                            columnAccessed[((int)minColumn)] = true;
                        }
                    }

                    if (!rowAccessed[(row)] && !columnAccessed[column])
                    {
                        rowAccessed[(row)] = true;
                        columnAccessed[(column)] = true;
                    }
                }

                var rowAccessedIsFalse = rowAccessed.Where(key => key.Value == false).ToList();
                var columnAccessedIsFalse = columnAccessed.Where(key => key.Value == false).ToList();

                if (rowAccessedIsFalse.Count> 0 || columnAccessedIsFalse.Count > 0)
                {
                    notOneZeroPerColumnRow = true;
                }


            }

            if (!notOneZeroPerColumnRow) 
            { 
               var listOfRows = coordinates.Select(coord => coord.Item1).Distinct().ToList();
               var listOfColumns = coordinates.Select(coord => coord.Item2).Distinct().ToList();

                listOfRows.Sort();
                listOfColumns.Sort();

                if (listOfRows.Count != updatedMatrix.GetLength(0) || listOfColumns.Count != updatedMatrix.GetLength(1))
                {
                    notOneZeroPerColumnRow = true;
                }
            
            }

            return notOneZeroPerColumnRow;
        }

    }
}