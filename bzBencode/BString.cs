﻿/*
Copyright (c) 2021, Darren Horrocks
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this
  list of conditions and the following disclaimer in the documentation and/or
  other materials provided with the distribution.

* Neither the name of Darren Horrocks nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/

using System;
using System.Globalization;
using System.IO;

namespace bzBencode
{
    /// <summary>
    /// Represents a string object. It cannot contain a null value.
    /// </summary>
    public class BString : IEquatable<string>, IEquatable<BString>, IComparable<string>, IComparable<BString>, IBencodingType
    {
        public BString()
        {

        }

        public BString(string value)
        {
            Value = value;
        }

        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set
            {
                if (value != null)
                {
                    _value = value;
                }
            }
        }
        public byte[] ByteValue { get; set; }

        /// <summary>
        /// Decode the next token as a string.
        /// Assumes the next token is a string.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="bytesConsumed"></param>
        /// <returns>Decoded string</returns>
        public static BString Decode(BinaryReader inputStream, ref int bytesConsumed)
        {
            // Read up to ':'
            var numberLength = "";
            char ch;

            while ((ch = inputStream.ReadChar()) != ':')
            {
                numberLength += ch;
                bytesConsumed++;
            }

            bytesConsumed++;

            // Read chars out
            //char[] stringData = new char[int.Parse(numberLength)];
            //inputStream.Read(stringData, 0, stringData.Length);
            var stringData = inputStream.ReadBytes(int.Parse(numberLength));
            bytesConsumed += int.Parse(numberLength);
            // Return
            return new BString { Value = BencodingUtils.ExtendedASCIIEncoding.GetString(stringData), ByteValue = stringData };
        }

        public void Encode(BinaryWriter writer)
        {
            var ascii = ByteValue ?? BencodingUtils.ExtendedASCIIEncoding.GetBytes(Value);

            // Write length
            writer.Write(BencodingUtils.ExtendedASCIIEncoding.GetBytes(ascii.Length.ToString(CultureInfo.InvariantCulture)));

            // Write seperator
            writer.Write(':');

            // Write ASCII representation
            writer.Write(ascii);
        }

        public int CompareTo(string other)
        {
            return StringComparer.InvariantCulture.Compare(Value, other);
        }
        public int CompareTo(BString other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return CompareTo(other.Value);
        }

        public override bool Equals(object obj)
        {
            var other = obj as BString;

            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }
        public bool Equals(BString other)
        {
            if (other == null)
            {
                return false;
            }

            if (Equals(other, this))
            {
                return true;
            }

            return Equals(other.Value, Value);
        }
        public bool Equals(string other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(Value, other);
        }
        public override int GetHashCode()
        {
            // Value should never be null
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator BString(string x)
        {
            return new BString(x);
        }

        public static implicit operator string(BString x)
        {
            return x.Value;
        }
    }
}
