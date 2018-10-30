using System;

public struct BitOperationUtility
{
    public static int ReadBit(int number, int bit)
    {
        if (bit < 0 || bit > 31)
            throw new ArgumentException(string.Format("[BitOperationUtility] Invalid bit to read ({0})", bit));

        return number & (1 << bit);
    }

    public static void WriteBit(ref int number, int bit, int value)
    {
        if (value != 0 && value != 1)
            throw new ArgumentException(string.Format("[BitOperationUtility] Invalid value to write ({0})", value));

        if (ReadBit(number, bit) != value)
        {
            if (value == 0)
                number -= 1 << bit;
            else
                number += 1 << bit;
        }
    }

    public static void WriteBit(ref int number, int bit, bool flag)
    {
        WriteBit(ref number, bit, flag ? 1 : 0);
    }

    public static int WriteBit(int number, int bit, int value)
    {
        WriteBit(ref number, bit, value);
        return number;
    }

    public static int WriteBit(int number, int bit, bool flag)
    {
        return WriteBit(number, bit, flag ? 1 : 0);
    }

    public static void WriteBits(ref int A, int B, int right, int left)
    {
        if (right > left)
            throw new ArgumentException(string.Format("[BitOperationUtility] Invalid interval to write ({0} ~ {1})", right, left));

        int mask = GetOnesFromLeft(31 - left) + GetOnesFromRight(right);

        A = (A & mask) | (B & ~mask);
    }

    public static int WriteBits(int A, int B, int right, int left)
    {
        WriteBits(ref A, B, right, left);
        return A;
    }

    private static int GetOnesFromRight(int n)
    {
        if (n == 0)
            return 0;

        return ~GetOnesFromLeft(32 - n);
    }

    private static int GetOnesFromLeft(int n)
    {
        if (n == 0)
            return 0;

        return int.MinValue >> n - 1;
    }
}
