using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Math
{
    interface ICalculable<T>{
        void Add(T num);
        void Subtract(T num);
        void Multiply(T num);
        void Divide(T num);
    }

    public class RationalNumberException : Exception
    {
        public RationalNumberException()
            : base()
        {
        }

        public RationalNumberException(string message)
            : base(message)
        {
        }

        public RationalNumberException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RationalNumberException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    [SerializableAttribute]
    public struct RationalNumber : 
        IEquatable<RationalNumber>, IEquatable<UInt64>, IEquatable<Int64>, IEquatable<BigInteger>,
        ICalculable<RationalNumber>, 
        IComparable<RationalNumber>, IComparable<UInt64>, IComparable<Int64>, IComparable<BigInteger>, IComparable
    {
        private BigInteger _numerator;
        private BigInteger _denominator;
        private bool _auto_reduction;

        //====================================================================
        //コンストラクタ
        //====================================================================
        public RationalNumber(bool autoReduction = true)
        {
            _numerator = 0;
            _denominator = 1;
            _auto_reduction = autoReduction;
        }

        public RationalNumber(BigInteger num, bool autoReduction = true)
        {
            _numerator = num;
            _denominator = 1;
            _auto_reduction = autoReduction;
        }

        public RationalNumber(BigInteger numerator, BigInteger denominator, bool autoReduction = true)
        {
            if (denominator.Sign == 0)
            {
                throw new RationalNumberException("denominator is 0");
            }
            else if (denominator.Sign < 0)
            {
                //符号の正負は分子だけで判断．分母は常に正の値
                _numerator = -numerator;
                _denominator = -denominator;
            }
            else
            {
                _numerator = numerator;
                _denominator = denominator;
            }

            _auto_reduction = autoReduction;
            if (_auto_reduction)
            {
                Reduce();
            }
        }

        //====================================================================
        //メソッド
        //====================================================================
        public void Reduce()
        {
            BigInteger gcd;
            while ((gcd = BigInteger.GreatestCommonDivisor(_numerator, _denominator)) > 1)
            {
                _numerator /= gcd;
                _denominator /= gcd;
            }
        }
       
        //====================================================================
        //静的メソッド
        //====================================================================
        public static RationalNumber Add(RationalNumber left, RationalNumber right)
        {
            var result = new RationalNumber(
                left._numerator * right._denominator + right._numerator * left._denominator,
                left._denominator * right._denominator, left._auto_reduction
            );

            if (result.AutoRediction)
            {
                result.Reduce();
            }

            return result;
        }

        public static RationalNumber Subtract(RationalNumber left, RationalNumber right)
        {
            var result = new RationalNumber(
                left._numerator * right._denominator - right._numerator * left._denominator,
                left._denominator * right._denominator, left._auto_reduction
            );

            if (result.AutoRediction)
            {
                result.Reduce();
            }

            return result;
        }

        public static RationalNumber Multiply(RationalNumber left, RationalNumber right)
        {
            return new RationalNumber(left._numerator * right._numerator,
                left._denominator * right._denominator, left._auto_reduction);
        }

        public static RationalNumber Divide(RationalNumber left, RationalNumber right)
        {
            return Multiply(left, Inverse(right));
        }

        public static RationalNumber Inverse(RationalNumber num)
        {
            if (num._numerator.IsZero)
            {
                throw new DivideByZeroException();
            }
            return new RationalNumber(num._denominator, num._numerator, num._auto_reduction);
        }

        public static RationalNumber Abs(RationalNumber num)
        {
            return new RationalNumber(BigInteger.Abs(num._numerator), num._denominator);
        }

        public static RationalNumber Positive(RationalNumber num)
        {
            return Abs(num);
        }

        public static RationalNumber Negative(RationalNumber num)
        {
            return new RationalNumber(-BigInteger.Abs(num._numerator), num._denominator);
        }

        public static int Compare(RationalNumber left, RationalNumber right)
        {
            var result = left - right;
            return result.Sign;
        }

        //====================================================================
        //オーバーライドメソッド
        //====================================================================
        public override bool Equals(object obj)
        {
            if (obj is RationalNumber)
                // 型が適切ならオーバーロードした等価演算子を使って比較
                return this == (RationalNumber)obj;
            else
                // 異なる型の場合はfalse
                return false;
        }

        public override string ToString()
        {
            if (_numerator.IsZero)
            {
                return _numerator.ToString();
            }
            else if (_denominator.IsOne)
            {
                return _numerator.ToString();
            }
            else
            {
                return _numerator.ToString() + "/" + _denominator.ToString();
            }
        }

        public override int GetHashCode()
        {
            return _denominator.GetHashCode() ^ _numerator.GetHashCode();
        }

        //====================================================================
        //IEquatableインターフェースの実装
        //====================================================================     
        public bool Equals(RationalNumber x)
        {
            if (this._numerator == 0 && x._numerator == 0)
            {
                return true;
            }
            else
            {
                // 約分したあとの分母と分子が同じなら等価
                var left_reduction = this.Reduction;
                var right_reduction = x.Reduction;

                return left_reduction._numerator == right_reduction._numerator &&
                    left_reduction._denominator == right_reduction._denominator;
            }
        }

        public bool Equals(BigInteger other)
        {
            return this.Equals((RationalNumber)other);
        }

        public bool Equals(ulong other)
        {
            return this.Equals((RationalNumber)other);
        }

        public bool Equals(long other)
        {
            return this.Equals((RationalNumber)other);
        }

        //====================================================================
        //IComparableインターフェースの実装
        //====================================================================   
        public int CompareTo(RationalNumber other)
        {
            return (this - other).Sign;
        }

        public int CompareTo(BigInteger other)
        {
            return (this - (RationalNumber)other).Sign;
        }

        public int CompareTo(ulong other)
        {
            return (this - (RationalNumber)other).Sign;
        }

        public int CompareTo(long other)
        {
            return (this - (RationalNumber)other).Sign;
        }

        public int CompareTo(object obj)
        {
            if (this.GetType().IsInstanceOfType(obj))
            {
                return (this - (RationalNumber)obj).Sign;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        //====================================================================
        //ICulcurableインターフェースの実装
        //====================================================================   
        public void Add(RationalNumber num)
        {
            throw new NotImplementedException();
        }

        public void Subtract(RationalNumber num)
        {
            throw new NotImplementedException();
        }

        public void Multiply(RationalNumber num)
        {
            throw new NotImplementedException();
        }

        public void Divide(RationalNumber num)
        {
            throw new NotImplementedException();
        }

        //====================================================================
        //演算子のオーバーロード
        //====================================================================
        public static RationalNumber operator +(RationalNumber num)
        {
            return Positive(num);
        }

        public static RationalNumber operator -(RationalNumber num)
        {
            return Negative(num);
        }

        public static RationalNumber operator +(RationalNumber left, RationalNumber right)
        {
            return Add(left, right);
        }

        public static RationalNumber operator -(RationalNumber left, RationalNumber right)
        {
            return Subtract(left, right);
        }

        public static RationalNumber operator *(RationalNumber left, RationalNumber right)
        {
            return Multiply(left, right);
        }

        public static RationalNumber operator /(RationalNumber left, RationalNumber right)
        {
            return Divide(left, right);
        }

        public static bool operator ==(RationalNumber left, RationalNumber right)
        {
            return left.Equals(right);
        }

        public static bool operator ==(RationalNumber left, BigInteger right)
        {
            return left.Equals(right);
        }

        public static bool operator ==(BigInteger left, RationalNumber right)
        {
            return right.Equals(left);
        }

        public static bool operator ==(RationalNumber left, UInt64 right)
        {
            return left.Equals(right);
        }

        public static bool operator ==(UInt64 left, RationalNumber right)
        {
            return right.Equals(left);
        }

        public static bool operator ==(RationalNumber left, Int64 right)
        {
            return left.Equals(right);
        }

        public static bool operator ==(Int64 left, RationalNumber right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(RationalNumber left, RationalNumber right)
        {
            return !(left == right);
        }

        public static bool operator !=(RationalNumber left, BigInteger right)
        {
            return !(left == right);
        }

        public static bool operator !=(BigInteger left, RationalNumber right)
        {
            return !(left == right);
        }

        public static bool operator !=(RationalNumber left, UInt64 right)
        {
            return !(left == right);
        }

        public static bool operator !=(UInt64 left, RationalNumber right)
        {
            return !(left == right);
        }

        public static bool operator !=(RationalNumber left, Int64 right)
        {
            return !(left == right);
        }

        public static bool operator !=(Int64 left, RationalNumber right)
        {
            return !(left == right);
        }

        public static bool operator >(RationalNumber left, RationalNumber right)
        {
            return left.CompareTo(right) > 0 ? true : false;
        }

        public static bool operator >(RationalNumber left, BigInteger right)
        {
            return left.CompareTo(right) > 0 ? true : false;
        }

        public static bool operator >(BigInteger left, RationalNumber right)
        {
            return ((RationalNumber)left).CompareTo(right) > 0 ? true : false;
        }

        public static bool operator >(RationalNumber left, UInt64 right)
        {
            return left.CompareTo(right) > 0 ? true : false;
        }

        public static bool operator >(UInt64 left, RationalNumber right)
        {
            return ((RationalNumber)left).CompareTo(right) > 0 ? true : false;
        }

        public static bool operator >(RationalNumber left, Int64 right)
        {
            return left.CompareTo(right) > 0 ? true : false;
        }

        public static bool operator >(Int64 left, RationalNumber right)
        {
            return ((RationalNumber)left).CompareTo(right) > 0 ? true : false;
        }

        public static bool operator <(RationalNumber left, RationalNumber right)
        {
            return left.CompareTo(right) < 0 ? true : false;
        }

        public static bool operator <(RationalNumber left, BigInteger right)
        {
            return left.CompareTo(right) < 0 ? true : false;
        }

        public static bool operator <(BigInteger left, RationalNumber right)
        {
            return ((RationalNumber)left).CompareTo(right) < 0 ? true : false;
        }

        public static bool operator <(RationalNumber left, UInt64 right)
        {
            return left.CompareTo(right) < 0 ? true : false;
        }

        public static bool operator <(UInt64 left, RationalNumber right)
        {
            return ((RationalNumber)left).CompareTo(right) < 0 ? true : false;
        }

        public static bool operator <(RationalNumber left, Int64 right)
        {
            return left.CompareTo(right) < 0 ? true : false;
        }

        public static bool operator <(Int64 left, RationalNumber right)
        {
            return ((RationalNumber)left).CompareTo(right) < 0 ? true : false;
        }

        //====================================================================
        //キャスト演算子のオーバーロード
        //====================================================================
        public static explicit operator RationalNumber(BigInteger num)
        {
            return new RationalNumber(num);
        }

        public static explicit operator RationalNumber(int num)
        {
            return new RationalNumber(num);
        }

        public static explicit operator RationalNumber(Int64 num)
        {
            return new RationalNumber(num);
        }

        public static explicit operator RationalNumber(uint num)
        {
            return new RationalNumber(num);
        }

        public static explicit operator RationalNumber(UInt64 num)
        {
            return new RationalNumber(num);
        }

        //====================================================================
        //プロパティ
        //====================================================================
        public bool AutoRediction
        {
            get
            {
                return _auto_reduction;
            }

            set
            {
                _auto_reduction = value;
            }
        }

        public RationalNumber Reduction
        {
            get
            {
                RationalNumber result = new RationalNumber(_numerator, _denominator, _auto_reduction);
                BigInteger gcd;
                //while ((gcd = GCD(result._numerator, result._denominator)) > 1)
                while ((gcd = BigInteger.GreatestCommonDivisor(result._numerator, result._denominator)) > 1)
                {
                    result._numerator /= gcd;
                    result._denominator /= gcd;
                }

                return result;
            }
        }

        public bool IsOne
        {
            get
            {
                var result = new RationalNumber(this._numerator, this._denominator, true);
                if (result._numerator.IsOne && result._denominator.IsOne)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsZero
        {
            get
            {
                return this._numerator.IsZero;
            }
        }

        public bool IsBigInteger
        {
            get
            {
                var result = new RationalNumber(this._numerator, this._denominator, true);
                if (result._denominator.IsOne)
                {
                    return true;
                }
                return false;
            }
        }

        public int Sign
        {
            get
            {
                return this._numerator.Sign;
            }
        }

        //====================================================================
        //(静的)プロパティ
        //====================================================================
        public static RationalNumber Zero
        {
            get
            {
                return new RationalNumber(0, 1);
            }
        }

        public static RationalNumber One
        {
            get
            {
                return new RationalNumber(1, 1);
            }
        }

        public static RationalNumber MinusOne
        {
            get
            {
                return new RationalNumber(-1, 1);
            }
        }
    }
}
