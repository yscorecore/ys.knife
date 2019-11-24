using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// KMP算法。
    /// </summary>
    public static class KMP
    {
        /// <summary>
        /// Next函数。 
        /// </summary>
        /// <param name="pattern">模式串</param>
        /// <returns>回溯函数</returns>
        public static int[] Next<T>(T[] pattern) where T : IEquatable<T>
        {
            int[] nextFunction = new int[pattern.Length];
            nextFunction[0] = -1;
            if (pattern.Length < 2)
            {
                return nextFunction;
            }

            nextFunction[1] = 0;
            int computingIndex = 2;
            int tempIndex = 0;
            while (computingIndex < pattern.Length)
            {
                if (pattern[computingIndex - 1].Equals(pattern[tempIndex]))
                {
                    nextFunction[computingIndex++] = ++tempIndex;
                }
                else
                {
                    tempIndex = nextFunction[tempIndex];
                    if (tempIndex == -1)
                    {
                        nextFunction[computingIndex++] = ++tempIndex;
                    }
                }
            }
            return nextFunction;
        }

        /// <summary>
        /// KMP计算
        /// </summary>
        /// <param name="source">主串</param>       
        /// <param name="pattern">模式串</param>
        /// <returns>匹配的第一个元素的索引。-1表示没有匹配</returns>
        public static int KMPSearch<T>(this T[] source, T[] pattern) where T : IEquatable<T>
        {
            int[] next = Next(pattern);
            return KMPSearch(source, 0, source.Length, pattern, next);
        }

        /// <summary>
        /// KMP计算
        /// </summary>
        /// <param name="source">主串</param>
        /// <param name="sourceOffset">主串起始偏移</param>
        /// <param name="sourceCount">被查找的主串的元素个数</param>
        /// <param name="pattern">模式串</param>
        /// <returns>匹配的第一个元素的索引。-1表示没有匹配</returns>
        public static int KMPSearch<T>(this T[] source, int sourceOffset, int sourceCount, T[] pattern) where T : IEquatable<T>
        {
            int[] next = Next(pattern);
            return KMPSearch(source, sourceOffset, sourceCount, pattern, next);
        }

        /// <summary>
        /// KMP计算
        /// </summary>
        /// <param name="source">主串</param>       
        /// <param name="pattern">模式串</param>
        /// <param name="next">回溯函数</param>
        /// <returns>匹配的第一个元素的索引。-1表示没有匹配</returns>
        public static int KMPSearch<T>(T[] source, T[] pattern, int[] next) where T : IEquatable<T>
        {
            return KMPSearch(source, 0, source.Length, pattern, next);
        }

        /// <summary>
        /// KMP计算
        /// </summary>
        /// <param name="source">主串</param>
        /// <param name="sourceOffset">主串起始偏移</param>
        /// <param name="sourceCount">被查找的主串的元素个数</param>
        /// <param name="pattern">模式串</param>
        /// <param name="next">回溯函数</param>
        /// <returns>匹配的第一个元素的索引。-1表示没有匹配</returns>
        public static int KMPSearch<T>(T[] source, int sourceOffset, int sourceCount, T[] pattern, int[] next) where T : IEquatable<T>
        {
            int sourceIndex = sourceOffset;
            int patternIndex = 0;
            while (patternIndex < pattern.Length && sourceIndex < sourceOffset + sourceCount)
            {
                if (source[sourceIndex].Equals(pattern[patternIndex]))
                {
                    sourceIndex++;
                    patternIndex++;
                }
                else
                {
                    patternIndex = next[patternIndex];
                    if (patternIndex == -1)
                    {
                        sourceIndex++;
                        patternIndex++;
                    }
                }
            }
            return patternIndex < pattern.Length ? -1 : sourceIndex - patternIndex;
        }
    }
}
