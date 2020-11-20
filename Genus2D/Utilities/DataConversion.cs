using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.Utilities
{
    public class DataConversion
    {

        public static T[] FlattenArray2D<T>(T[,] data)
        {
            int rows0 = data.GetLength(0);
            int rows1 = data.GetLength(1);
            T[] flattened = new T[rows0 * rows1];
            for (int i = 0; i < rows0; i++)
            {
                for (int j = 0; j < rows1; j++)
                {
                    flattened[i + j * rows0] = data[i, j];
                }
            }

            return flattened;
        }

        public static T[,] ExpandArray2D<T>(T[] data, int rows0)
        {
            int length = data.GetLength(0);
            int rows1 = length / rows0;
            T[,] expanded = new T[rows0, rows1];
            for (int i = 0; i < rows0; i++)
            {
                for (int j = 0; j < rows1; j++)
                {
                    expanded[i, j] = data[i + j * rows0];
                }
            }

            return expanded;
        }

        public static T[] FlattenArray3D<T>(T[,,] data)
        {
            int rows0 = data.GetLength(0);
            int rows1 = data.GetLength(1);
            int rows2 = data.GetLength(2);
            T[] flattened = new T[rows0 * rows1 * rows2];
            for (int i = 0; i < rows0; i++)
            {
                for (int j = 0; j < rows1; j++)
                {
                    for (int k = 0; k < rows2; k++)
                    {
                        flattened[i + rows0 * (j + rows1 * k)] = data[i, j, k];
                    }
                }
            }

            return flattened;
        }

        public static T[,,] ExpandArray3D<T>(T[] data, int rows0, int rows2)
        {
            int length = data.GetLength(0);
            int rows1 = length / (rows0 * rows2);
            T[,,] expanded = new T[rows0, rows1, rows2];
            for (int i = 0; i < rows0; i++)
            {
                for (int j = 0; j < rows1; j++)
                {
                    for (int k = 0; k < rows2; k++)
                    {
                        expanded[i, j, k] = data[i + rows0 * (j + rows1 * k)];
                    }
                }
            }

            return expanded;
        }

    }
}
