﻿using System;

namespace LogicalOperations
{
    /// <summary>
    ///     Base class for values
    /// </summary>
    /// <remarks>see derived types</remarks>
    public abstract class Value
    {
        public ValueType Type { get; set; }

        public abstract override string ToString();

        public abstract string ToString(IFormatProvider format);

        public abstract double ToDouble();

        public abstract double ToDouble(IFormatProvider format);

        public abstract void SetValue(string value);

        public abstract void SetValue(string value, IFormatProvider format);

        public abstract void SetValue(double value);

        public abstract void SetValue(double value, IFormatProvider format);
    }
}