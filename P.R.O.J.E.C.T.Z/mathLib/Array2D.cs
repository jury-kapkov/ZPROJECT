using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.R.O.J.E.C.T.Z.mathLib
{
    class Array2D
    {
        private double[,] data;
        private int length;
        private int nRows, nCols;
        public Array2D(int size)
        {
            length = size;
            nRows = nCols = size;
            data = new double[size, size];
        }
        public Array2D(Array2D array)
        {
            nRows = array.nRows;
            nCols = array.nCols;
            length = array.length;
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    this.data[i, j] = array.data[i, j];
                }
            }
        }
        public Array2D(int height, int width)
        {
            nRows = height;
            nCols = width;
            data = new double[nRows, nCols];
        }
        public Array2D(double[,] array)
        {
            nRows = array.GetLength(0);
            nCols = array.GetLength(1);
            data = new double[nRows, nCols];
            length = array.Length;
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    this.data[i, j] = array[i, j];
                }
            }
        }
        public Array2D(double[] vector)
        {
            nRows = vector.Length;
            nCols = 1;
            data = new double[vector.Length, 1];
            length = vector.Length;
            for (int row = 0; row < vector.Length; ++row)
            {
                this.data[row, 0] = vector[row];
            }
        }
        public void SetValue(int row, int column, double value)
        {
            this.data[row, column] = value;
        }
        public int GetRowDemention()
        {
            return nRows;
        }
        public int GetColDemention()
        {
            return nCols;
        }
        public Array2D Multiply(Array2D matrix)
        {
        //    int nRows = this.GetRowDemention();
        //    int nCols = matrix.GetColDemention();
        //    int nSum = this.GetColDemention();
        //    double[,] outData = new double[nRows, nCols];
        //    double[] mCol = new double[nSum];
        //    double[,] mData = matrix.data;

        //    for (int col = 0; col < nCols; ++col)
        //    {
        //        int row;
        //        for (row = 0; row < nSum; ++row)
        //        {
        //            mCol[row] = mData[row,col];
        //        }

        //        for (row = 0; row < nRows; ++row)
        //        {
        //            //! Возможана ошибка 
        //            //Формируем строку матрицы
        //            double[] dataRow = new double[this.nCols];
        //            for (int i = 0; i < this.nCols; i++)
        //            {
        //                dataRow[i] = this.data[row, i];
        //            }
                    
        //            double sum = 0.0D;

        //            for (int i = 0; i < nSum; ++i)
        //            {
        //                sum += dataRow[i] * mCol[i];
        //            }

        //            outData[row, col] = sum;
        //        }
        //    }
        //    return new Array2D(outData);

            if (nCols == matrix.nRows)
            {
                Array2D result = new Array2D(nRows, matrix.nCols);
                for (int i = 0; i < result.nCols; ++i)
                {
                    for (int j = 0; j < result.nRows; ++j)
                    {
                        for (int k = 0; k < matrix.nRows; ++k)
                        {
                            result.data[j, i] += this.data[j, k] * matrix.data[k, i];
                        }
                    }
                }
                return result;
            }
            else
            {
                return this;
            }
        }
        public double GetValue(int row, int col)
        {
                return data[row, col];
        }
    }
}
