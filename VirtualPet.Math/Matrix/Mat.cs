using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet.Math.Matrix
{
    public class Mat
    {
        protected float[,] components;

        public Mat()
        {

        }
        public Mat(int rank)
        {
            components = new float[rank, rank];
            Zero();
        }
        public Mat(float[,] com)
        {
            components = (float[,])com.Clone();
        }
        public float this[int row, int col]
        {
            get { return components[row,col]; }
            set { components[row,col] = value; }
        }

        private void Zero()
        {
            int totalLength = components.GetLength(0) * components.GetLength(1);
            for (int i = 0; i < totalLength; i++)
            {
                int row = i / components.GetLength(1);
                int column = i % components.GetLength(1);

                components[row, column] = 0;
            }
        }

        private void LoadIdentity(int rank)
        {
            components = new float[rank, rank];
            for(int i=0;i<rank;++i)
            {
                components[i, i] = 1;
            }
        }
        private void MakeAbs()
        {
            int rows = components.GetLength(0);
            int cols = components.GetLength(1);
            int totalLength = rows * cols;
            for (int i = 0; i < totalLength; i++)
            {
                int row = i / cols;
                int col = i % cols;
                this[row, col] = MathF.Abs(this[row,col]);
            }
        }
        protected Mat Abs()
        {
            Mat result = new Mat(components);
            result.MakeAbs();
            return result;

        }

        protected Mat Transpose()
        {
            int w = components.GetLength(0);
            int h = components.GetLength(1);

            float[,] result = new float[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = components[i, j];
                }
            }

            return new Mat(result);
        }
        protected Mat Invert()
        {
            const double tiny = 0.00001;

            // Build the augmented matrix.
            int num_rows = components.GetUpperBound(0) + 1;
            float[,] augmented = new float[num_rows, 2 * num_rows];
            for (int row = 0; row < num_rows; row++)
            {
                for (int col = 0; col < num_rows; col++)
                    augmented[row, col] = components[row, col];
                augmented[row, row + num_rows] = 1;
            }

            // num_cols is the number of the augmented matrix.
            int num_cols = 2 * num_rows;

            // Solve.
            for (int row = 0; row < num_rows; row++)
            {
                // Zero out all entries in column r after this row.
                // See if this row has a non-zero entry in column r.
                if (MathF.Abs(augmented[row, row]) < tiny)
                {
                    // Too close to zero. Try to swap with a later row.
                    for (int r2 = row + 1; r2 < num_rows; r2++)
                    {
                        if (MathF.Abs(augmented[r2, row]) > tiny)
                        {
                            // This row will work. Swap them.
                            for (int c = 0; c < num_cols; c++)
                            {
                                float tmp = augmented[row, c];
                                augmented[row, c] = augmented[r2, c];
                                augmented[r2, c] = tmp;
                            }
                            break;
                        }
                    }
                }

                // If this row has a non-zero entry in column r, use it.
                if (MathF.Abs(augmented[row, row]) > tiny)
                {
                    // Divide the row by augmented[row, row] to make this entry 1.
                    for (int col = 0; col < num_cols; col++)
                        if (col != row)
                            augmented[row, col] /= augmented[row, row];
                    augmented[row, row] = 1;

                    // Subtract this row from the other rows.
                    for (int row2 = 0; row2 < num_rows; row2++)
                    {
                        if (row2 != row)
                        {
                            float factor = augmented[row2, row] / augmented[row, row];
                            for (int col = 0; col < num_cols; col++)
                                augmented[row2, col] -= factor * augmented[row, col];
                        }
                    }
                }
            }

            // See if we have a solution.
            if (augmented[num_rows - 1, num_rows - 1] == 0) return null;

            // Extract the inverse array.
            float[,] inverse = new float[num_rows, num_rows];
            for (int i = 0; i < num_rows * num_rows; i++)
            {
                int row = i / num_rows;
                int col = i % num_rows;
                inverse[row, col] = augmented[row, col + num_rows];
            }
            return new Mat(inverse);
        }

        protected Mat Mul(float scalar)
        {
            Mat result = new Mat(components);
            int rows = components.GetLength(0);
            int cols = components.GetLength(1);
            int totalLength = rows * cols;
            for (int i = 0; i < totalLength; i++)
            {
                int row = i / cols;
                int col = i % cols;
                result[row, col] *= scalar;
            }
            return result;
        }

        protected Mat Mul(Mat m)
        {
            var matrix1Rows = components.GetLength(0);
            var matrix1Cols = components.GetLength(1);
            var matrix2Rows = m.components.GetLength(0);
            var matrix2Cols = m.components.GetLength(1);

            // checking if product is defined  
            if (matrix1Cols != matrix2Rows)
            {
                return null;
            }

            // creating the final product matrix  
            float[,] product = new float[matrix1Rows, matrix2Cols];

            // looping through matrix 1 rows  
            for (int matrix1_row = 0; matrix1_row < matrix1Rows; matrix1_row++)
            {
                // for each matrix 1 row, loop through matrix 2 columns  
                for (int matrix2_col = 0; matrix2_col < matrix2Cols; matrix2_col++)
                {
                    // loop through matrix 1 columns to calculate the dot product  
                    for (int matrix1_col = 0; matrix1_col < matrix1Cols; matrix1_col++)
                    {
                        product[matrix1_row, matrix2_col] +=
                          components[matrix1_row, matrix1_col] *
                          m.components[matrix1_col, matrix2_col];
                    }
                }
            }
            return new Mat(product);
        }

        protected Mat MulLeft(Mat m)
        {
           return m.Mul(this);
        }

        protected Vectors.Vec Mul(Vectors.Vec v)
        {
            var matrixRows = components.GetLength(0);
            var matrixCols = components.GetLength(1);
            


            if (matrixCols != v.Count)
            {
                return null;
            }

            float[] product = new float[matrixRows];

            for (int matrix1_row = 0; matrix1_row < matrixRows; matrix1_row++)
            {

                    for (int matrix1_col = 0; matrix1_col < matrixCols; matrix1_col++)
                    {
                        product[matrix1_row] +=
                          components[matrix1_row, matrix1_col] *
                          v[matrix1_col];
                    }
            }
            return new Vectors.Vec(product);
        }

        protected static Mat Identity(int rank)
        {
            Mat result = new Mat();
            result.LoadIdentity(rank);
            return result;
        }

        public override string ToString()
        {
            string result ="";
            int rows = components.GetLength(0);
            int cols = components.GetLength(1);
            int totalLength = rows * cols;
            for (int i = 0; i < totalLength; i++)
            {
                int row = i / cols;
                int column = i % cols;
                if(column < cols -1)
                {
                    result += $"{components[row, column]}, ";
                }
                else
                {
                    result += $"{components[row, column]}\n";
                }
            }
            return result;
        }

    }

    
}
