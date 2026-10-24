using System;
using System.Collections.Generic;

namespace TechnicalTest.Algorithms
{
    /// <summary>
    /// Technical Test: Logic and Algorithm Implementation
    /// Author: Fachriza (FR)
    /// Language: C# (.NET 8)
    ///
    /// This file contains solutions for:
    /// 1. Problem 1: Reverse a String (StringReverser)
    /// 2. Problem 2: Palindrome Check (PalindromeChecker)
    /// 3. Problem 3: Prime Number Generator (PrimeNumberGenerator)
    /// </summary>

    #region Problem 1: Reverse a String

    /// <summary>
    /// Provides methods to reverse strings.
    /// </summary>
    public class StringReverser
    {
        /// <summary>
        /// Reverses the given input string using an in-place two-pointer technique on a character array.
        /// </summary>
        /// <param name="inputStr">The input string to be reversed.</param>
        /// <returns>The reversed string, or null if the input string was null.</returns>
        /// <remarks>
        /// Thought Process &amp; Design Decisions:
        /// 1. Null &amp; Edge Cases:
        ///    - If input is null, we return null to respect input nullability semantics.
        ///    - If input length is 0 or 1, we immediately return the input itself (O(1) fast path)
        ///      without allocating any new memory.
        /// 2. Algorithmic Approach:
        ///    - In C#, strings are immutable. Any reversal requires allocating new memory for the result.
        ///    - To achieve the optimal balance between performance and memory efficiency, we convert
        ///      the string to a char array (ToCharArray()) and apply an in-place two-pointer swap:
        ///      left pointer moving forward, right pointer moving backward until they meet.
        ///    - Finally, a new string is constructed directly from the mutated char array.
        /// 3. Complexity:
        ///    - Time Complexity: O(n) where n is the length of the string. Each character is visited once (n / 2 swaps).
        ///    - Space Complexity: O(n) to store the character array and return the new reversed string.
        ///      Auxiliary Space: O(1) beyond the required output buffer.
        /// 4. Trade-offs Considered:
        ///    - String concatenation in loop (`result += inputStr[i]`):
        ///      Causes O(n^2) runtime due to allocating a new string object on every iteration. Avoided.
        ///    - LINQ (`new string(inputStr.Reverse().ToArray())`):
        ///      Concise, but incurs unnecessary abstraction overhead from IEnumerable iterator state machines.
        ///    - Unicode Surrogate Pairs:
        ///      For standard text, char array reversal is standard. If supporting complex astral plane emojis
        ///      or combining diacritics where a single visible character spans multiple 16-bit code units,
        ///      System.Globalization.StringInfo.GetTextElementEnumerator can be used, though at a higher
        ///      performance cost. For standard algorithmic tests, the two-pointer char[] swap is idiomatic and optimal.
        /// </remarks>
        public static string? ReverseString(string? inputStr)
        {
            // Edge Case 1: Null input
            if (inputStr == null)
            {
                return null;
            }

            // Edge Case 2: Empty or single-character string (no reversal needed)
            if (inputStr.Length <= 1)
            {
                return inputStr;
            }

            char[] charArray = inputStr.ToCharArray();
            int left = 0;
            int right = charArray.Length - 1;

            while (left < right)
            {
                // In-place swap of characters
                char temp = charArray[left];
                charArray[left] = charArray[right];
                charArray[right] = temp;

                left++;
                right--;
            }

            return new string(charArray);
        }
    }

    #endregion

    #region Problem 2: Palindrome Check

    /// <summary>
    /// Provides methods to verify whether a given string is a palindrome.
    /// </summary>
    public class PalindromeChecker
    {
        /// <summary>
        /// Determines if a given string is a palindrome.
        /// By default, this method performs a standard real-world alphanumeric palindrome check:
        /// it ignores casing and non-alphanumeric characters (spaces, punctuation).
        /// For example, "Racecar" and "A man, a plan, a canal: Panama" evaluate to true.
        /// </summary>
        /// <param name="inputStr">The input string to evaluate.</param>
        /// <returns>True if the string is a palindrome; otherwise false.</returns>
        /// <remarks>
        /// Thought Process &amp; Design Decisions:
        /// 1. Null &amp; Edge Cases:
        ///    - A null input is not a palindrome; returns false.
        ///    - An empty string or single-character string is vacuously a palindrome; returns true.
        /// 2. Algorithmic Approach:
        ///    - Two pointers: left pointer starts at 0, right pointer starts at length - 1.
        ///    - Non-alphanumeric characters (spaces, punctuation) are bypassed on-the-fly without
        ///      allocating a cleaned copy of the string (saving memory).
        ///    - Characters are compared case-insensitively using char.ToLowerInvariant.
        ///    - If a mismatch is found, we return false immediately (short-circuit evaluation).
        /// 3. Complexity:
        ///    - Time Complexity: O(n) where n is the length of the string. At most each character is scanned once.
        ///    - Space Complexity: O(1) auxiliary space! Unlike naive solutions that reverse the string
        ///      and compare (`Reverse(s) == s`), this approach allocates 0 extra heap memory.
        /// 4. Trade-offs Considered:
        ///    - Exact vs Normalized:
        ///      In technical tests, both exact match and phrase palindromes ("Madam, I'm Adam") are common.
        ///      We provide `IsPalindrome` (alphanumeric, case-insensitive) as the primary method,
        ///      and also expose `IsExactPalindrome` (strict character-by-character) for cases where
        ///      exact matching is required.
        /// </remarks>
        public static bool IsPalindrome(string? inputStr)
        {
            // Edge Case 1: Null input cannot be a valid palindrome
            if (inputStr == null)
            {
                return false;
            }

            // Edge Case 2: Empty or single character is always a palindrome
            if (inputStr.Length <= 1)
            {
                return true;
            }

            int left = 0;
            int right = inputStr.Length - 1;

            while (left < right)
            {
                // Advance left pointer past non-alphanumeric characters
                while (left < right && !char.IsLetterOrDigit(inputStr[left]))
                {
                    left++;
                }

                // Decrement right pointer past non-alphanumeric characters
                while (left < right && !char.IsLetterOrDigit(inputStr[right]))
                {
                    right--;
                }

                // Compare characters case-insensitively
                if (char.ToLowerInvariant(inputStr[left]) != char.ToLowerInvariant(inputStr[right]))
                {
                    return false;
                }

                left++;
                right--;
            }

            return true;
        }

        /// <summary>
        /// Strict/Exact palindrome check: compares raw characters from both ends without normalization.
        /// e.g. "racecar" -> true, "Racecar" -> false.
        /// </summary>
        /// <param name="inputStr">The input string to evaluate.</param>
        /// <returns>True if strictly identical forwards and backwards; otherwise false.</returns>
        public static bool IsExactPalindrome(string? inputStr)
        {
            if (inputStr == null) return false;
            if (inputStr.Length <= 1) return true;

            int left = 0;
            int right = inputStr.Length - 1;

            while (left < right)
            {
                if (inputStr[left] != inputStr[right])
                {
                    return false;
                }
                left++;
                right--;
            }

            return true;
        }
    }

    #endregion

    #region Problem 3: Prime Number Generator

    /// <summary>
    /// Provides methods to generate prime numbers up to a specified limit.
    /// </summary>
    public class PrimeNumberGenerator
    {
        /// <summary>
        /// Generates a list of all prime numbers up to and including the given limit.
        /// Implements the Sieve of Eratosthenes algorithm.
        /// </summary>
        /// <param name="limit">The upper bound (inclusive) up to which primes are generated.</param>
        /// <returns>A list of prime numbers in ascending order.</returns>
        /// <remarks>
        /// Thought Process &amp; Design Decisions:
        /// 1. Edge Cases:
        ///    - If limit &lt; 2, there are no prime numbers (smallest prime is 2). Returns empty list.
        ///    - If limit == 2, returns [2].
        /// 2. Algorithmic Choice: Sieve of Eratosthenes vs Trial Division
        ///    - Trial Division: Checking each number up to limit for primality takes O(N * sqrt(N)) time.
        ///      For N = 100,000, that is ~31,600,000 operations.
        ///    - Sieve of Eratosthenes: Generates primes in O(N * log(log(N))) time.
        ///      For N = 100,000, that is ~1,160,000 operations (nearly 30x faster!).
        ///    - Therefore, the Sieve of Eratosthenes is the optimal and industry-standard choice.
        /// 3. Implementation Optimizations:
        ///    - Memory: In C#, a bool array defaults to `false`. We use `isComposite` where `false` means prime.
        ///      This avoids having to loop through and initialize the entire array to `true`.
        ///    - Multiples starting point: Outer loop only needs to run up to sqrt(limit) because any composite
        ///      number &lt;= limit must have a prime factor &lt;= sqrt(limit).
        ///    - Inner loop starts at `p * p` instead of `2 * p`, because smaller multiples (2p, 3p, etc.)
        ///      have already been marked by earlier prime factors.
        ///    - 64-bit cast: `(long)p * p` avoids integer overflow if limit is near int.MaxValue.
        ///    - Capacity Pre-allocation: We can estimate prime count using the Prime Number Theorem:
        ///      pi(N) ≈ N / ln(N) to reserve capacity in the list and avoid repeated array resizing.
        /// 4. Complexity:
        ///    - Time Complexity: O(N * log(log(N))) where N is the limit.
        ///    - Space Complexity: O(N) boolean array to mark composite numbers.
        /// </remarks>
        public static List<int> GeneratePrimes(int limit)
        {
            // Edge Case: No primes exist strictly below 2
            if (limit < 2)
            {
                return new List<int>();
            }

            // Estimate capacity using Prime Number Theorem: limit / ln(limit)
            // Add safety margin for small limits
            int estimatedCapacity = limit < 10
                ? 4
                : (int)((double)limit / Math.Log(limit) * 1.15);

            List<int> primesList = new List<int>(Math.Max(estimatedCapacity, 1));

            // isComposite[i] == false means i is prime (takes advantage of C# default false initialization)
            bool[] isComposite = new bool[limit + 1];

            int sqrtLimit = (int)Math.Sqrt(limit);

            for (int p = 2; p <= sqrtLimit; p++)
            {
                if (!isComposite[p])
                {
                    // Mark multiples of p starting from p * p
                    // Cast to long to avoid 32-bit integer overflow
                    for (long multiple = (long)p * p; multiple <= limit; multiple += p)
                    {
                        isComposite[(int)multiple] = true;
                    }
                }
            }

            // Collect all remaining unmarked numbers >= 2
            for (int i = 2; i <= limit; i++)
            {
                if (!isComposite[i])
                {
                    primesList.Add(i);
                }
            }

            return primesList;
        }
    }

    #endregion
}
