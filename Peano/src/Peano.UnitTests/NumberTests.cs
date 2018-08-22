using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
{
    public class NumberTests
    {
        private static int ToInt(Peano.Number number)
        {
            var result = 0;

            while (number is Peano.Number.Successor successor)
            {
                result++;
                number = successor.Item;
            }

            return result;
        }

        private static Peano.Number ToNumber(int i)
        {
            var result = Peano.Number.Zero;

            while (i > 0)
            {
                result = Peano.Number.NewSuccessor(result);
                i--;
            }

            return result;
        }

        public static IEnumerable<object[]> NumberData =>
            new []
            {
                new object[] { 0,  Peano.Number.Zero },
                new object[] { 1,  Peano.one },
                new object[] { 2,  Peano.two },
                new object[] { 3,  Peano.three },
                new object[] { 4,  Peano.four },
                new object[] { 5,  Peano.five },
                new object[] { 6,  Peano.six },
                new object[] { 7,  Peano.seven },
                new object[] { 8,  Peano.eight },
                new object[] { 9,  Peano.nine },
                new object[] { 10, Peano.ten }
            };

        [Theory, MemberData(nameof(NumberData))]
        public void NumberToInt(int expectedInt, Peano.Number number)
        {
            var actualInt = ToInt(number);
            Assert.Equal(expectedInt, actualInt);
        }

        [Theory, MemberData(nameof(NumberData))]
        public void IntToNumber(int integer, Peano.Number expectedNumber)
        {
            var actualNumber = ToNumber(integer);
            Assert.Equal(expectedNumber, actualNumber);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void Increment(int value)
        {
            var expected = value + 1;
            var number = ToNumber(value);
            var result = Peano.increment(number);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Decrement_Zero()
        {
            Action action = () => Peano.decrement(Peano.Number.Zero);
            Assert.Throws<InvalidOperationException>(action);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(11)]
        public void Decrement(int value)
        {
            var expected = value - 1;
            var number = ToNumber(value);
            var result = Peano.decrement(number);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(5, 0)]
        [InlineData(0, 3)]
        [InlineData(16, 27)]
        public void Addition(int left, int right)
        {
            var expected = left + right;
            var leftNumber = ToNumber(left);
            var rightNumber = ToNumber(right);
            var result = Peano.add(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Subtraction_CantSubtractLargeFromSmall()
        {
            var left = ToNumber(5);
            var right = ToNumber(7);
            Action action = () => Peano.subtract(left, right);
            Assert.Throws<InvalidOperationException>(action);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(5, 3)]
        [InlineData(24, 13)]
        public void Subtraction(int left, int right)
        {
            var expected = left - right;
            var leftNumber = ToNumber(left);
            var rightNumber = ToNumber(right);
            var result = Peano.subtract(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [InlineData(0, 0)]
        [InlineData(5, 0)]
        [InlineData(0, 3)]
        [InlineData(1, 7)]
        [InlineData(8, 9)]
        public void Multiplication(int left, int right)
        {
            var expected = left * right;
            var leftNumber = ToNumber(left);
            var rightNumber = ToNumber(right);
            var result = Peano.multiply(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(6, 6)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(4, 5)]
        [InlineData(7, 19)]
        [InlineData(3, 8)]
        public void LessThan(int left, int right)
        {
            var expected = left < right;
            var leftNumber = ToNumber(left);
            var rightNumber = ToNumber(right);
            var actual = Peano.lessThan(leftNumber, rightNumber);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Division_CantDivideByZero()
        {
            Action action = () => Peano.divide(Peano.Number.Zero, Peano.Number.Zero);
            Assert.Throws<InvalidOperationException>(action);
        }
        
        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 3)]
        [InlineData(15, 3)]
        [InlineData(100, 9)]
        public void Division(int left, int right)
        {
            var expected = left / right;
            var leftNumber = ToNumber(left);
            var rightNumber = ToNumber(right);
            var result = Peano.divide(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Modulo_CantDivideByZero()
        {
            Action action = () => Peano.modulo(Peano.Number.Zero, Peano.Number.Zero);
            Assert.Throws<InvalidOperationException>(action);
        }
        
        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 3)]
        [InlineData(15, 3)]
        [InlineData(100, 9)]
        public void Modulo(int left, int right)
        {
            var expected = left % right;
            var leftNumber = ToNumber(left);
            var rightNumber = ToNumber(right);
            var result = Peano.modulo(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PrintDigit_FailsForMoreThanNine()
        {
            var number = ToNumber(15);
            Action action = () => Peano.printDigit(number);
            Assert.Throws<InvalidOperationException>(action);
        }

        public static IEnumerable<object[]> PrintDigitValues =>
            from value in Enumerable.Range(0, 10)
            select new object[] { value };

        [Theory, MemberData(nameof(PrintDigitValues))]
        public void PrintDigit(int i)
        {
            var number = ToNumber(i);
            var result = Peano.printDigit(number);
            Assert.Equal(i.ToString(), result);
        }

        public static IEnumerable<object[]> PrintValues =>
            from index in Enumerable.Range(0, 11)
            let value = index * 7
            select new object[] { value };

        [Theory, MemberData(nameof(PrintValues))]
        public void Print(int i)
        {
            var number = ToNumber(i);
            var result = Peano.print(number);
            Assert.Equal(i.ToString(), result);
        }

        public static IEnumerable<object[]> ParseValues =>
            from index in Enumerable.Range(0, 11)
            let value = index * 7
            let str = value.ToString()
            select new object[] { value };

        [Theory, MemberData(nameof(ParseValues))]
        public void Parse(string str)
        {
            var result = Peano.parse(str);
            var actual = ToInt(result);
            Assert.Equal(int.Parse(str), actual);
        }

        [Fact]
        public void CombineItAll()
        {
            const int expected = 12 * 34 - 56 + 78 / 90;

            var result = Peano.add(
                Peano.subtract(Peano.multiply(Peano.parse("12"), Peano.parse("34")), Peano.parse("56")),
                Peano.divide(Peano.parse("78"), Peano.parse("90")));

            var actual = Peano.print(result);

            Assert.Equal(expected.ToString(), actual);
        }
    }
}
