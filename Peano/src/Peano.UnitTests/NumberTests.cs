using System;
using Xunit;

namespace Peano.UnitTests
{
    public class NumberTests
    {
        private static int ToInt(Number.Number number)
        {
            var result = 0;

            while (number is Number.Number.Successor successor)
            {
                result++;
                number = successor.Item;
            }

            return result;
        }

        private static Number.Number ToNumber(int i)
        {
            var result = Number.Number.Zero;

            while (i > 0)
            {
                result = Number.Number.NewSuccessor(result);
                i--;
            }

            return result;
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(10, 11)]
        public void Increment(int value, int expected)
        {
            var number = ToNumber(value);
            var result = Number.increment(number);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Decrement_Zero()
        {
            Action action = () => Number.decrement(Number.Number.Zero);
            Assert.Throws<InvalidOperationException>(action);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(11, 10)]
        public void Decrement(int value, int expected)
        {
            var number = ToNumber(value);
            var result = Number.decrement(number);
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
            var result = Number.add(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Subtraction_CantSubtractLargeFromSmall()
        {
            var left = ToNumber(5);
            var right = ToNumber(7);
            Action action = () => Number.subtract(left, right);
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
            var result = Number.subtract(leftNumber, rightNumber);
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
            var result = Number.multiply(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Division_CantDivideByZero()
        {
            Action action = () => Number.divide(Number.Number.Zero, Number.Number.Zero);
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
            var result = Number.divide(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Modulo_CantDivideByZero()
        {
            Action action = () => Number.modulo(Number.Number.Zero, Number.Number.Zero);
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
            var result = Number.modulo(leftNumber, rightNumber);
            var actual = ToInt(result);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Ten()
        {
            var result = Number.ten;
            var actual = ToInt(result);
            Assert.Equal(10, actual);
        }

        [Fact]
        public void PrintDigit_FailsForMoreThanNine()
        {
            var number = ToNumber(15);
            Action action = () => Number.printDigit(number);
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void PrintDigit()
        {
            for (var i = 0; i < 10; i++)
            {
                var number = ToNumber(i);
                var result = Number.printDigit(number);
                Assert.Equal(i.ToString(), result);
            }
        }

        [Fact]
        public void Print()
        {
            for (var i = 0; i <= 70; i += 7)
            {
                var number = ToNumber(i);
                var result = Number.print(number);
                Assert.Equal(i.ToString(), result);
            }
        }

        [Fact]
        public void Parse()
        {
            for (var i = 0; i <= 70; i += 7)
            {
                var str = i.ToString();
                var result = Number.parse(str);
                var actual = ToInt(result);
                Assert.Equal(i, actual);
            }
        }

        [Fact]
        public void CombineItAll()
        {
            const int expected = 12 * 34 - 56 + 78 / 90;

            var result = Number.add(
                Number.subtract(Number.multiply(Number.parse("12"), Number.parse("34")), Number.parse("56")),
                Number.divide(Number.parse("78"), Number.parse("90")));

            var actual = Number.print(result);

            Assert.Equal(expected.ToString(), actual);
        }
    }
}
