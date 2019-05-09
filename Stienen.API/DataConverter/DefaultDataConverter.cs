using System;
using System.Collections.Generic;
using System.Text;
using Stienen.Backend.DataAccess.Models;

namespace Stienen.API.DataConverter {
    public class DefaultDataConverter : IDataConverter {
        private Dictionary<int, Dictionary<String, Dictionary<int, ReferenceEx>>> references;

        public DefaultDataConverter()
        {
            this.references = new Dictionary<int, Dictionary<string, Dictionary<int, ReferenceEx>>>();
        }

        private void FillReferences(int hardware, int version)
        {
//            foreach (var reference in refs)
//            {
//                if (!references[hardware].ContainsKey(reference.Name))
//                {
//                    references[hardware].Add(reference.Name, new Dictionary<int, ReferenceEx>());
//                }

//                if (!references[hardware][reference.Name].ContainsKey(version))
//                {
//                    references[hardware][reference.Name][version] = reference;
//                }
//            }
        }
        
        // recursively call some format function on the byte array, passing in the index of the array that has the data based on the reference offset
        // fill some sort of data structure first, only update database after we are done with the message, not on every subsequent variable

        private string FormatData(ReferenceEx reference, byte[] value)
        {
            String result = "";
            VarType type = (VarType)reference.Type;
            int mul = reference.Mul;
            int div = reference.Div;
            double step = reference.Step;
            double tmp;

            if (reference.Text != 0)
            {
                short val = (byte)ReadShortSigned(value);
                result = val.ToString();
            }
            else
            {
                switch (type)
                {
                    case VarType.vtBool:
                        result = ReadBool(value).ToString();
                        break;
                    case VarType.vtString:
                        result = ReadString(value);
                        break;
                    case VarType.vtDateTime:
                        result = ReadDateTime(value, reference);
                        break;
                    case VarType.vtTimeSpan:
                        result = ReadTimeSpan(value, reference);
                        break;
                    case VarType.vtFloat:
                        // UNKNOWN
                        break;
                    case VarType.vtDouble:
                        // UNKNOWN
                        break;
                    case VarType.vtUI8:
                        tmp = ((double)ReadByteUnsigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;
                    case VarType.vtI8:
                        tmp = ((double)ReadByteSigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;
                    case VarType.vtUI16:
                        tmp = ((double)ReadShortUnsigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;
                    case VarType.vtI16:
                        tmp = ((double)ReadShortSigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;
                    case VarType.vtUI32:
                        tmp = ((double)ReadIntUnsigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;
                    case VarType.vtI32:
                        tmp = ((double)ReadIntSigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;
                    case VarType.vtUI64:
                        tmp = ((double)ReadLongUnsigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;
                    case VarType.vtI64:
                        tmp = ((double)ReadLongSigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;
                    case VarType.vtInput:
                        tmp = (ReadShortUnsigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;
                    case VarType.vtOutput:
                        tmp = (ReadShortUnsigned(value) * mul) / div;
                        if ((step > 0) && (step <= 1))
                        {
                            return tmp.ToString("N" + -Math.Round(Math.Log10(step)));
                        }
                        break;

                    case VarType.vtTimespanRelative:
                        break;

                    case VarType.vtTimespanUInt32:
                        break;

                    case VarType.vtStringUnicode:
                        result = ReadStringUnicode(value);
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private bool ReadBool(byte[] value)
        {
            UInt16 tmp = ReadShortUnsigned(value);

            if (tmp > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private String ReadString(byte[] value)
        {
            // Remove 0x00 from the end
            int len = 0;
            while ((len < value.Length) && (value[len] != 0x00))
            {
                len++;
            }

            String result = Encoding.UTF8.GetString(value, 0, len);
            return result.TrimEnd();
        }

        private String ReadStringUnicode(byte[] value)
        {
            // Convert bytes to little endian
            byte[] valueConverted = new byte[value.Length];
            bool stop = false;

            int i = 0;
            while ((i < value.Length - 1) && (!stop))
            {
                if ((value[i] == 0x00) && (value[i + 1] == 0x00))
                {
                    stop = true;
                }
                else
                {
                    valueConverted[i] = value[i + 1];
                    valueConverted[i + 1] = value[i];
                    i += 2;
                }
            }

            String rawName = Encoding.Unicode.GetString(valueConverted, 0, i);
            return rawName.TrimEnd(' ');
        }

        private String ReadDateTime(byte[] value, ReferenceEx reference)
        {
            Double tmp = ((Double)ReadShortUnsigned(value) * reference.Mul) / reference.Div;
            DateTime result = new DateTime(2001, 01, 01).AddDays((Int16)tmp);
            return result.ToShortDateString();
        }

        private String ReadTimeSpan(byte[] value, ReferenceEx reference)
        {
            Double tmp = ((Double)ReadShortUnsigned(value) * reference.Mul) / reference.Div;

            UInt16 fff = (UInt16)(tmp % 1000);
            UInt16 ss = (UInt16)(Math.Floor(tmp /= 1000.0) % 60);
            UInt16 mm = (UInt16)(Math.Floor(tmp /= 60) % 60);
            UInt16 hh = (UInt16)(Math.Floor(tmp / 60));
            String result = "";

            if (hh < 10)
            {
                result += "0";
            }
            result += hh.ToString();
            if (reference.Step < 3600)
            {
                result += ':' + mm.ToString("D2");
                if (reference.Step < 60)
                {
                    result += ':' + ss.ToString("D2");
                    if (reference.Step < 1)
                    {
                        if (reference.Step.Equals(0.1))
                        {
                            result += ':' + fff.ToString("D1");
                        }
                        else if (reference.Step.Equals(0.01))
                        {
                            result += ':' + fff.ToString("D2");
                        }
                        else
                        {
                            result += ':' + fff.ToString("D3");
                        }
                    }
                }
            }

            return result;
        }

        private byte ReadByteUnsigned(byte[] value)
        {
            return (byte)(ReadNumber(value, 1) & (UInt64)byte.MaxValue);
        }

        private char ReadByteSigned(byte[] value)
        {
            return Convert.ToChar(ReadByteUnsigned(value));
        }

        private UInt16 ReadShortUnsigned(byte[] value)
        {
            return (UInt16)(ReadNumber(value, 2) & (UInt64)UInt16.MaxValue);
        }

        private Int16 ReadShortSigned(byte[] value)
        {
            return (Int16)ReadShortUnsigned(value);
        }

        private UInt32 ReadIntUnsigned(byte[] value)
        {
            return (UInt32)(ReadNumber(value, 4) & (UInt64)UInt32.MaxValue);
        }

        private Int32 ReadIntSigned(byte[] value)
        {
            return (Int32)ReadIntUnsigned(value);
        }

        private UInt64 ReadLongUnsigned(byte[] value)
        {
            return (ReadNumber(value, 8) & UInt64.MaxValue);
        }

        private Int64 ReadLongSigned(byte[] value)
        {
            return (Int64)ReadLongUnsigned(value);
        }

        private UInt64 ReadNumber(byte[] value, int count)
        {
            UInt64 result = 0;
            int index = 0;

            while ((--count >= 0) && (index < value.Length))
            {
                result <<= 8;
                result |= value[index];
                index++;
            }

            return result;
        }
    }
}