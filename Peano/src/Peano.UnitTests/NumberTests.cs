using System;
using System.Collections.Generic;
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

        public static IEnumerable<object[]> NumberData()
        {
            return new []
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
        }

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
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(10, 11)]
        public void Increment(int value, int expected)
        {
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
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(11, 10)]
        public void Decrement(int value, int expected)
        {
            var number = ToNumber(value);
            var result = Peano.decrement(number);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(5, 0, 5)]
        [InlineData(0, 3, 3)]
        [InlineData(16, 27, 43)]
        public void Addition(int left, int right, int expected)
        {
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
        [InlineData(1, 0, 1)]
        [InlineData(5, 3, 2)]
        [InlineData(24, 13, 11)]
        public void Subtraction(int left, int right, int expected)
        {
            var leftNumber = ToNumber(left);
            var rightNumber = ToNumber(right);
            var result = Peano.subtract(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(5, 0, 0)]
        [InlineData(0, 3, 0)]
        [InlineData(1, 7, 7)]
        [InlineData(8, 9, 72)]
        public void Multiplication(int left, int right, int expected)
        {
            var leftNumber = ToNumber(left);
            var rightNumber = ToNumber(right);
            var result = Peano.multiply(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Division_CantDivideByZero()
        {
            Action action = () => Peano.divide(Peano.Number.Zero, Peano.Number.Zero);
            Assert.Throws<InvalidOperationException>(action);
        }
        
        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(5, 3, 1)]
        [InlineData(15, 3, 5)]
        [InlineData(100, 9, 11)]
        public void Division(int left, int right, int expected)
        {
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
        [InlineData(1, 1, 0)]
        [InlineData(5, 3, 2)]
        [InlineData(15, 3, 0)]
        [InlineData(100, 9, 1)]
        public void Modulo(int left, int right, int expected)
        {
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

        [Fact]
        public void PrintDigit()
        {
            for (var i = 0; i < 10; i++)
            {
                var number = ToNumber(i);
                var result = Peano.printDigit(number);
                Assert.Equal(i.ToString(), result);
            }
        }

        [Fact]
        public void Print()
        {
            for (var i = 0; i <= 70; i += 7)
            {
                var number = ToNumber(i);
                var result = Peano.print(number);
                Assert.Equal(i.ToString(), result);
            }
        }

        [Fact]
        public void Parse()
        {
            for (var i = 0; i <= 70; i += 7)
            {
                var str = i.ToString();
                var result = Peano.parse(str);
                var actual = ToInt(result);
                Assert.Equal(i, actual);
            }
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
