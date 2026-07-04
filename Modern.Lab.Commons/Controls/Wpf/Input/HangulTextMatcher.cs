using System;

namespace Modern.Lab.Controls.Wpf.Input
{
    /// <summary>
    /// 자동완성(검색창 동작)을 위한 한국어 인식 텍스트 매칭.
    ///
    /// 패턴 문자는 다음과 같이 해석된다:
    /// - 자음 자모(ㄱ~ㅎ): 초성이 그 자모인 모든 음절과 매칭 —
    ///   초성 검색을 가능하게 한다 ("ㄱㅁㅅ" → "김민수").
    /// - 완성형 음절: 정확히 일치; 다만 패턴의 마지막 문자는 종성이 없으면
    ///   초성+중성만으로도 매칭될 수 있어, IME 조합 중간 상태에서도 매칭이
    ///   유지된다 ("기" → "김민수").
    /// - 그 외: 서수(ordinal), 대소문자 무시 비교.
    /// 매칭은 후보 문자열에 대해 contains 의미론을 사용한다.
    /// </summary>
    internal static class HangulTextMatcher
    {
        private const char SyllableFirst = '가';   // U+AC00
        private const char SyllableLast = '힣';    // U+D7A3
        private const int MedialCount = 21;
        private const int FinalCount = 28;

        // 음절 순서대로 나열한 초성 목록(호환 자모).
        private const string InitialJamoTable = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";

        /// <summary>패턴이 후보 문자열 어디에든 나타나면 true를 반환한다.</summary>
        internal static bool Contains(string candidate, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return true;
            }

            if (string.IsNullOrEmpty(candidate) || candidate.Length < pattern.Length)
            {
                return false;
            }

            for (int start = 0; start <= candidate.Length - pattern.Length; start++)
            {
                if (MatchesAt(candidate, start, pattern))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MatchesAt(string candidate, int start, string pattern)
        {
            for (int index = 0; index < pattern.Length; index++)
            {
                bool isLastPatternChar = index == pattern.Length - 1;

                if (!MatchChar(candidate[start + index], pattern[index], isLastPatternChar))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool MatchChar(char candidateChar, char patternChar, bool isLastPatternChar)
        {
            if (candidateChar == patternChar)
            {
                return true;
            }

            int initialIndex = InitialJamoTable.IndexOf(patternChar);

            if (initialIndex >= 0)
            {
                // 자음 자모 패턴: 음절의 초성과 비교한다.
                return IsSyllable(candidateChar) &&
                       (candidateChar - SyllableFirst) / (MedialCount * FinalCount) == initialIndex;
            }

            if (IsSyllable(patternChar))
            {
                // 패턴 끝의 종성 없는 음절은 IME 조합 중간 상태이므로
                // 초성+중성만으로 매칭한다.
                if (isLastPatternChar && IsSyllable(candidateChar))
                {
                    int patternOffset = patternChar - SyllableFirst;
                    int candidateOffset = candidateChar - SyllableFirst;
                    bool patternHasNoFinal = patternOffset % FinalCount == 0;

                    return patternHasNoFinal &&
                           patternOffset / FinalCount == candidateOffset / FinalCount;
                }

                return false;
            }

            return char.ToUpperInvariant(candidateChar) == char.ToUpperInvariant(patternChar);
        }

        private static bool IsSyllable(char value)
        {
            return value >= SyllableFirst && value <= SyllableLast;
        }
    }
}
