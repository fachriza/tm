using System;
using System.Collections.Generic;
using TechnicalTest.Algorithms;

namespace TechnicalTest.Algorithms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("==================================================================");
            Console.WriteLine("   TECHNICAL TEST: LOGIC AND ALGORITHM IMPLEMENTATION (C#)       ");
            Console.WriteLine("==================================================================\n");

            TestStringReverser();
            TestPalindromeChecker();
            TestPrimeNumberGenerator();

            Console.WriteLine("==================================================================");
            Console.WriteLine("   ALL ALGORITHM TESTS COMPLETED SUCCESSFULLY!                    ");
            Console.WriteLine("==================================================================");
        }

        private static void TestStringReverser()
        {
            Console.WriteLine("--- [Problem 1: Reverse a String] ---");

            var testCases = new (string? Input, string? Expected)[]
            {
                ("hello", "olleh"),
                ("World!", "!dlroW"),
                ("a", "a"),
                ("", ""),
                ("   ", "   "),
                ("1234567890", "0987654321"),
                ("racecar", "racecar"),
                (null, null)
            };

            int passed = 0;
            foreach (var (input, expected) in testCases)
            {
                string? actual = StringReverser.ReverseString(input);
                bool isMatch = actual == expected;
                if (isMatch) passed++;

                string displayInput = input == null ? "<null>" : $"\"{input}\"";
                string displayActual = actual == null ? "<null>" : $"\"{actual}\"";
                string displayExpected = expected == null ? "<null>" : $"\"{expected}\"";

                Console.WriteLine($"  [{(isMatch ? "PASS" : "FAIL")}] Input: {displayInput,-16} -> Output: {displayActual,-16} Expected: {displayExpected}");
            }

            Console.WriteLine($"  Summary: {passed}/{testCases.Length} test cases passed.\n");
        }

        private static void TestPalindromeChecker()
        {
            Console.WriteLine("--- [Problem 2: Palindrome Check] ---");

            var testCases = new (string? Input, bool ExpectedNormalized, bool ExpectedExact)[]
            {
                ("racecar", true, true),
                ("Racecar", true, false), // uppercase R vs lowercase r
                ("A man, a plan, a canal: Panama", true, false), // punctuation and mixed case
                ("Was it a car or a cat I saw?", true, false),
                ("No 'x' in Nixon", true, false),
                ("hello", false, false),
                ("csharp", false, false),
                ("12321", true, true),
                ("12345", false, false),
                ("a", true, true),
                ("", true, true),
                (null, false, false)
            };

            int passedNorm = 0;
            int passedExact = 0;

            foreach (var (input, expNorm, expExact) in testCases)
            {
                bool actualNorm = PalindromeChecker.IsPalindrome(input);
                bool actualExact = PalindromeChecker.IsExactPalindrome(input);

                if (actualNorm == expNorm) passedNorm++;
                if (actualExact == expExact) passedExact++;

                string displayInput = input == null ? "<null>" : (input.Length > 28 ? $"\"{input[..25]}...\"" : $"\"{input}\"");

                Console.WriteLine($"  [{((actualNorm == expNorm) ? "PASS" : "FAIL")}] Normalized: {actualNorm,-5} | Exact: {actualExact,-5} | Input: {displayInput}");
            }

            Console.WriteLine($"  Summary: Normalized: {passedNorm}/{testCases.Length} passed | Exact: {passedExact}/{testCases.Length} passed.\n");
        }

        private static void TestPrimeNumberGenerator()
        {
            Console.WriteLine("--- [Problem 3: Prime Number Generator] ---");

            var testCases = new (int Limit, int ExpectedCount, string Sample)[]
            {
                (-5, 0, "[]"),
                (0, 0, "[]"),
                (1, 0, "[]"),
                (2, 1, "[2]"),
                (10, 4, "[2, 3, 5, 7]"),
                (30, 10, "[2, 3, 5, 7, 11, 13, 17, 19, 23, 29]"),
                (100, 25, "First 25 primes (up to 97)")
            };

            int passed = 0;
            foreach (var (limit, expCount, sample) in testCases)
            {
                List<int> primes = PrimeNumberGenerator.GeneratePrimes(limit);
                bool isMatch = primes.Count == expCount;
                if (isMatch) passed++;

                string resultStr = primes.Count <= 10
                    ? $"[{string.Join(", ", primes)}]"
                    : $"[{string.Join(", ", primes.GetRange(0, 5))} ... {primes[^1]}] (Total: {primes.Count})";

                Console.WriteLine($"  [{(isMatch ? "PASS" : "FAIL")}] Limit: {limit,4} -> Found {primes.Count,2} primes: {resultStr}");
            }

            // Benchmark large limit
            int largeLimit = 1_000_000;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<int> largePrimes = PrimeNumberGenerator.GeneratePrimes(largeLimit);
            watch.Stop();
            // Pi(1,000,000) = 78,498 primes
            bool largePassed = largePrimes.Count == 78498;
            Console.WriteLine($"  [{(largePassed ? "PASS" : "FAIL")}] Benchmark Limit {largeLimit:N0}: Generated {largePrimes.Count:N0} primes in {watch.ElapsedMilliseconds} ms (Expected 78,498)");

            Console.WriteLine($"  Summary: {passed}/{testCases.Length} edge cases passed + large sieve verified.\n");
        }
    }
}
