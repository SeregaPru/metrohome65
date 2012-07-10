using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MAPIdotnet
{
    internal static partial class cemapi
    {
        #region IMAPITable
        public interface IMAPITable : IMAPIUnknown
        {
            uint GetRowCount();
            void SetColumns(PropTags[] tags);
            SRow[] QueryRows(int lRowCount);
            void SeekRow(uint position);
            void SortRows(PropTags tag, TableSortOrder order);
        }

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        private struct _PV
        {
            [FieldOffset(0)]
            public Int16 i;
            [FieldOffset(0)]
            public int l;
            [FieldOffset(0)]
            public uint ul;
            [FieldOffset(0)]
            public float flt;
            [FieldOffset(0)]
            public double dbl;
            [FieldOffset(0)]
            public UInt16 b;
            [FieldOffset(0)]
            public double at;
            [FieldOffset(0)]
            public IntPtr lpszA;
            [FieldOffset(0)]
            public IntPtr lpszW;
            [FieldOffset(0)]
            public IntPtr lpguid;
            /*[FieldOffset(0)]
            public IntPtr bin;*/
            [FieldOffset(0)]
            public UInt64 li;
            [FieldOffset(0)]
            public SRowSet bin;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct pSPropValue
        {
            public uint ulPropTag;
            public uint dwAlignPad;
            public _PV Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SRowSet
        {
            public uint cRows;
            public IntPtr aRow; // pSRow

            public byte[] AsBytes
            {
                get
                {
                    byte[] b = new byte[this.cRows];
                    for (int i = 0; i < this.cRows; i++)
                        b[i] = Marshal.ReadByte(aRow, i);
                    return b;
                }
            }
        }

        private class MAPITable : MAPIUnknown, IMAPITable
        {
            [DllImport("cemapi.dll")]
            private static extern void FreeProws(IntPtr prows);

            [DllImport("MAPIlib.dll", EntryPoint = "IMAPITableGetRowCount")]
            private static extern HRESULT pIMAPITableGetRowCount(IntPtr table, out uint lpulCount);

            [DllImport("MAPIlib.dll", EntryPoint = "IMAPITableSetColumns")]
            private static extern HRESULT pIMAPITableSetColumns(IntPtr table, [MarshalAs(UnmanagedType.LPArray)] uint[] propTags);

            /// <summary>
            /// If this call fails "rows" must be passed to MAPIFreeBuffer().
            /// If successful FreeRows() must be called when done
            /// </summary>
            /// <param name="table"></param>
            /// <param name="lRowCount"></param>
            /// <param name="rows">[Out] Rows</param>
            /// <returns></returns>
            [DllImport("MAPIlib.dll", EntryPoint = "IMAPITableQueryRows")]
            private static extern HRESULT pIMAPITableQueryRows(IntPtr table, int lRowCount, out IntPtr rows);

            [DllImport("MAPIlib.dll", EntryPoint = "IMAPITableSeekRow")]
            private static extern HRESULT pIMAPITableSeekRow(IntPtr table, int lRowCount);

            [DllImport("MAPIlib.dll", EntryPoint = "IMAPITableSortTable")]
            private static extern HRESULT pIMAPITableSortTable(IntPtr table, IntPtr lpSortCriteria);

            [StructLayout(LayoutKind.Sequential)]
            private struct pSRow
            {
                public uint ulAdrEntryPad;
                public uint cValues;
                public IntPtr lpProps; // pSPropValue
            }
            private const int SizeOfSRow = 12;

            public MAPITable(IntPtr tablePtr) { this.ptr = tablePtr; }

            ~MAPITable() { }

            public uint GetRowCount()
            {
                uint count = 0;
                HRESULT hr = pIMAPITableGetRowCount(this.ptr, out count);
                if (hr != HRESULT.S_OK)
                    throw new Exception("pIMAPITableGetRowCount failed: " + hr.ToString());
                return count;
            }

            public void SetColumns(PropTags[] tags)
            {
                uint[] t = new uint[tags.Length + 1];
                t[0] = (uint)tags.Length;
                for (int i = 0; i < tags.Length; i++)
                    t[i + 1] = (uint)tags[i];
                HRESULT hr = pIMAPITableSetColumns(this.ptr, t);
                //if (hr != HRESULT.S_OK)
                //    throw new Exception("pIMAPITableSetColumns failed: " + hr.ToString());
            }

            public SRow[] QueryRows(int lRowCount)
            {
                if (lRowCount > MaxQueryRowCount)
                    throw new Exception("Max row count is " + MaxQueryRowCount.ToString());
                IntPtr pRowSet = IntPtr.Zero;
                HRESULT hr = pIMAPITableQueryRows(this.ptr, lRowCount, out pRowSet);
                if (hr != HRESULT.S_OK)
                {
                    cemapi.MAPIFreeBuffer(pRowSet);
                    throw new Exception("QueryRows failed: " + hr.ToString());
                }

                uint cRows = (uint)Marshal.ReadInt32(pRowSet);
                SRow[] sRows = new SRow[cRows];

                if (cRows < 1)
                {
                    FreeProws(pRowSet);
                    return sRows;
                }

                int pIntSize = IntPtr.Size, intSize = Marshal.SizeOf(typeof(Int32));
                IntPtr rows = (IntPtr)(((uint)pRowSet) + intSize);
                for (int i = 0; i < cRows; i++)
                {
                    uint pRowOffset = (uint)(rows) + (uint)(i * SizeOfSRow);
                    uint cValues = (uint)Marshal.ReadInt32((IntPtr)(pRowOffset + intSize));
                    uint pProps = (uint)Marshal.ReadInt32((IntPtr)(pRowOffset + intSize * 2));

                    IPropValue[] lpProps = new IPropValue[cValues];
                    for (int j = 0; j < cValues; j++) // each column
                    {
                        pSPropValue lpProp = (pSPropValue)Marshal.PtrToStructure((IntPtr)(pProps + j * cemapi.SizeOfSPropValue), typeof(pSPropValue));
                        lpProps[j] = new SPropValue(lpProp);
                    }
                    sRows[i].propVals = lpProps;
                }
                FreeProws(pRowSet);
                return sRows;
            }

            public void SeekRow(uint position)
            {
                HRESULT hr = pIMAPITableSeekRow(this.ptr, (int)position);
                if (hr != HRESULT.S_OK)
                    throw new Exception("pIMAPITableSeekRow failed: " + hr.ToString());
            }

            public void SortRows(PropTags tag, TableSortOrder order)
            {
                int sizeS = Marshal.SizeOf(typeof(SSortOrder));
                IntPtr sortArray = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(uint)) * 3 + sizeS);
                Marshal.WriteInt32(sortArray, 1);
                Marshal.WriteInt64(sortArray, Marshal.SizeOf(typeof(uint)), 0);
                SSortOrder s;
                s.ulOrder = order;
                s.ulPropTag = tag;
                Marshal.StructureToPtr(s, (IntPtr)(((uint)sortArray) + Marshal.SizeOf(typeof(uint)) * 3), false);
                HRESULT hr = pIMAPITableSortTable(this.ptr, sortArray);
                if (hr != HRESULT.S_OK)
                    throw new Exception("pIMAPITableSetColumns failed: " + hr.ToString());
                Marshal.FreeHGlobal(sortArray);
            }
        }

        #region SPropValue
        public interface IPropValue
        {
            Type Type { get; }
            PropTags Tag { get; }
            string AsString { get; }
            int AsInt32 { get; }
            uint AsEntryID { get; }
            byte[] AsBinary { get; }
            UInt64 AsUInt64 { get; }
        }

        private class SPropValue : IPropValue
        {
            private PropTags tag;
            private Type t;
            private uint ul = 0;
            //private pSBinary binary;
            private byte[] binary;
            //private uint p_ul;
            //private float p_flt;
            //private double p_dbl;
            //private IntPtr ptr = IntPtr.Zero;
            private string str = null;
            private UInt64 p_li;

            public SPropValue(pSPropValue prop)
            {
                this.tag = (PropTags)prop.ulPropTag;
                switch ((PT)((uint)this.tag & 0xFFFF))
                {
                    case PT.PT_TSTRING:
                        this.t = typeof(string);
                        this.str = Marshal.PtrToStringUni(prop.Value.lpszW);
                        break;
                    case PT.PT_LONG:
                    case PT.PT_I2:
                    case PT.PT_BOOLEAN:
                        this.t = typeof(int);
                        this.ul = prop.Value.ul;
                        break;
                    case PT.PT_BINARY:
                        this.t = typeof(SBinary);
                        this.binary = prop.Value.bin.AsBytes;
                        break;
                    case PT.PT_SYSTIME:
                    case PT.PT_I8:
                        this.t = typeof(UInt64);
                        this.p_li = prop.Value.li;
                        break;
                    default:
                        this.t = null;
                        break;
                }
            }

            public Type Type { get { return this.t; } }
            public PropTags Tag { get { return this.tag; } }

            public string AsString
            {
                get
                {
                    if (this.t == typeof(string))
                        return this.str;
                    else
                        throw new Exception("Invalid type request");
                }
            }

            public int AsInt32
            {
                get
                {
                    if (this.t == typeof(int))
                        return (int)this.ul;
                    else
                        throw new Exception("Invalid type request");
                }
            }

            public uint AsEntryID
            {
                get
                {
                    if (this.t == typeof(int) && this.tag == PropTags.PR_ENTRYID)
                        return this.ul;
                    else
                        throw new Exception("Invalid type request");
                }
            }

            public byte[] AsBinary
            {
                get
                {
                    if (this.t == typeof(SBinary))
                        return this.binary;
                    else
                        throw new Exception("Invalid type request");
                }
            }

            public UInt64 AsUInt64
            {
                get
                {
                    if (this.t == typeof(UInt64))
                        return this.p_li;
                    else
                        throw new Exception("Invalid type request");
                }
            }
        }
        #endregion

        #region SBinary
        private static SBinary SBinaryCreate(byte[] data)
        {
            SBinary b;
            b.cb = (uint)data.Length;
            b.lpb = Marshal.AllocHGlobal((int)b.cb);
            for (int i = 0; i < b.cb; i++)
                Marshal.WriteByte(b.lpb, i, data[i]);
            return b;
        }

        private static void SBinaryRelease(ref SBinary b)
        {
            if (b.lpb != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(b.lpb);
                b.lpb = IntPtr.Zero;
                b.cb = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SBinary
        {
            public uint cb;
            public IntPtr lpb;
        }
        #endregion

        public struct SRow
        {
            /// <summary>
            /// Row columns
            /// </summary>
            public IPropValue[] propVals;

            public static string[] PropsToStringArray(SRow[] rows, int propIndex)
            {
                int len = rows.Length;
                string[] strings = new string[len];
                for (int i = 0; i < len; i++)
                    strings[i] = rows[i].propVals[propIndex].AsString;
                return strings;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SSortOrder
        {
            public PropTags ulPropTag;
            public TableSortOrder ulOrder;
        }

        #endregion
    }
}
