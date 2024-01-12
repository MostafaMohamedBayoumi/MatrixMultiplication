using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problem
{
    // *****************************************
    // DON'T CHANGE CLASS OR FUNCTION NAME
    // YOU CAN ADD FUNCTIONS IF YOU NEED TO
    // *****************************************
    public static class MatrixMultiplication
    {
        
        
        //Your Code is Here:
        //==================
        /// <summary>
        /// Multiply 2 square matrices in an efficient way [Strassen's Method]
        /// </summary>
        /// <param name="M1">First square matrix</param>
        /// <param name="M2">Second square matrix</param>
        /// <param name="N">Dimension (power of 2)</param>
        /// <returns>Resulting square matrix</returns>
       
        public static int[,] MatrixMultiply(int[,] M1, int[,] M2, int N)
        {
            if(N==0 || IsPowerOf2(N) == false || !CanMultiply(M1,M2) )
            {
                if(IsPowerOf2(N) == false)
                    throw new ArgumentException("N must be a positive integer power of 2.");
                else if(!CanMultiply(M1, M2))
                    throw new ArgumentException("cann't multiply.");
                else
                    throw new ArgumentException("N mustn't be equal 0.");

            }
            else if (N <= 64)
            {
                return NaiveMultiply(M1, M2, N); 
            }
            else if (N==1)
            {
                int[,] out_M = new int[N, N];
                out_M[0, 0] = M1[0, 0] * M2[0, 0];
                return out_M;

            }
            
            else
            {
                
                int[,] A11, A12, A21, A22, B11, B12, B21, B22;
                SplitMatrix(M1, out A11, out A12, out A21, out A22);
                SplitMatrix(M2, out B11, out B12, out B21, out B22);

                var tasks = new Task<int[,]>[7];
                tasks[0] = Task.Factory.StartNew(() => MatrixMultiply(Add(A11, A22, N / 2), Add(B11, B22, N / 2), N / 2));
                tasks[1] = Task.Factory.StartNew(() => MatrixMultiply(Add(A21, A22, N / 2), B11, N / 2));
                tasks[2] = Task.Factory.StartNew(() => MatrixMultiply(A11, Subtract(B12, B22, N / 2), N / 2));
                tasks[3] = Task.Factory.StartNew(() => MatrixMultiply(A22, Subtract(B21, B11, N / 2), N / 2));
                tasks[4] = Task.Factory.StartNew(() => MatrixMultiply(Add(A11, A12, N / 2), B22, N / 2));
                tasks[5] = Task.Factory.StartNew(() => MatrixMultiply(Subtract(A21, A11, N / 2), Add(B11, B12, N / 2), N / 2));
                tasks[6] = Task.Factory.StartNew(() => MatrixMultiply(Subtract(A12, A22, N / 2), Add(B21, B22, N / 2), N / 2));

                Task.WaitAll(tasks);

                int[,] P1 = tasks[0].Result;
                int[,] P2 = tasks[1].Result;
                int[,] P3 = tasks[2].Result;
                int[,] P4 = tasks[3].Result;
                int[,] P5 = tasks[4].Result;
                int[,] P6 = tasks[5].Result;
                int[,] P7 = tasks[6].Result;



                int[,] out_M11 = Add(Subtract(Add(P1, P4, N / 2), P5, N / 2), P7, N / 2);
                int[,] out_M12 = Add(P3, P5, N / 2);
                int[,] out_M21 = Add(P2, P4, N / 2);
                int[,] out_M22 = Add(Subtract(Add(P1, P3, N / 2), P2, N / 2), P6, N / 2);

                
                 

                return combine_matrix(out_M11, out_M12, out_M21, out_M22, N);
            }
        }

        private static bool IsPowerOf2(int N)
        {
            if (N <= 0)
            {
                return false;
            }
            return (N & (N - 1)) == 0;
        }
        public static bool CanMultiply(int[,] matrix1, int[,] matrix2)
        {
            if (matrix1.GetLength(1) != matrix2.GetLength(0))
            {
                return false;
            }

            return true;
        }
        private static int[,] NaiveMultiply(int[,] M1, int[,] M2, int N)
        {
            int[,] out_multiply_Matrix = new int[N, N];

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < N; k++)
                    {
                        sum += M1[i, k] * M2[k, j];
                    }
                    out_multiply_Matrix[i, j] = sum;
                }
            }

            return out_multiply_Matrix;
        }
        private static void SplitMatrix(int[,] M, out int[,] M11, out int[,] M12, out int[,] M21, out int[,] M22)
        {
            int N = M.GetLength(0) / 2;
            M11 = new int[N, N];
            M12 = new int[N, N];
            M21 = new int[N, N];
            M22 = new int[N, N];

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    M11[i, j] = M[i, j];
                    M12[i, j] = M[i, j + N];
                    M21[i, j] = M[i + N, j];
                    M22[i, j] = M[i + N, j + N];
                }
            }
        }
        private static int[,] Add(int[,] M1, int[,] M2, int N)
        {
            int[,] out_sum_of_matrix = new int[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    out_sum_of_matrix[i, j] = M1[i, j] + M2[i, j];
                }
            }
            return out_sum_of_matrix;
            
        }
        private static int[,] Subtract(int[,] M1, int[,] M2, int N)
        {
            int[,] out_sptract_Matrix = new int[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    out_sptract_Matrix[i, j] = M1[i, j] - M2[i, j];
                }
            }
            return out_sptract_Matrix;
        }
        private static int[,] combine_matrix(int[,] M11, int[,] M12, int[,] M21, int[,] M22,int N)
        {
            int[,] out_M = new int[N, N];
            for (int i = 0; i < N / 2; i++)
            {
                for (int j = 0; j < N / 2; j++)
                {
                    out_M[i, j] = M11[i, j];
                    out_M[i, j + N / 2] = M12[i, j];
                    out_M[i + N / 2, j] = M21[i, j];
                    out_M[i + N / 2, j + N / 2] = M22[i, j];
                }
            }

            return out_M;
        }



    }


}
