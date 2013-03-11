using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MAPIdotnet
{
    internal static partial class cemapi
    {
        #region IMAPIProp
        public interface IMAPIProp : IMAPIUnknown
        {
            IPropValue[] GetProps(PropTags[] tags);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tags"></param>
            /// <param name="values">You are responsible for ensuring types match!</param>
            /// <returns></returns>
            bool SetProps(PropTags[] tags, object[] values);
        }

        private abstract class MAPIProp : MAPIUnknown, IMAPIProp
        {
            protected enum OpenPropertyFlags : uint
            {
                Default = 0,
                MAPI_MODIFY = 0x00000001,
            };

            [DllImport("MAPIlib.dll", EntryPoint = "IMAPIPropGetProps")]
            private static extern HRESULT pIMAPIPropGetProps(IntPtr prop, [MarshalAs(UnmanagedType.LPArray)] uint[] propTags, out uint count, ref IntPtr propArray);

            [DllImport("MAPIlib.dll", EntryPoint = "IMAPIPropSetProps")]
            private static extern HRESULT pIMAPIPropSetProps(IntPtr prop, uint cValues, IntPtr lpPropArray);

            /*[DllImport("MAPIlib.dll", EntryPoint = "IMAPIPropOpenProperty")]
            private static extern HRESULT pIMAPIPropOpenProperty(IntPtr prop, uint propTag, OpenPropertyFlags flags, */

            public MAPIProp(IntPtr propPtr) { this.ptr = propPtr; }

            //~MAPIProp() { }

            public IPropValue[] GetProps(PropTags[] tags)
            {
                uint[] t = new uint[tags.Length + 1];
                t[0] = (uint)tags.Length;
                for (int i = 0; i < tags.Length; i++)
                    t[i + 1] = (uint)tags[i];

                IntPtr propVals = IntPtr.Zero;
                uint count = 0;
                HRESULT hr = pIMAPIPropGetProps(this.ptr, t, out count, ref propVals);
                if (hr != HRESULT.S_OK)
                    throw new Exception("GetProps failed: " + hr.ToString());

                IPropValue[] props = new IPropValue[count];
                uint pProps = (uint)propVals;
                for (int i = 0; i < count; i++)
                {
                    pSPropValue lpProp = (pSPropValue)Marshal.PtrToStructure((IntPtr)(pProps + i * cemapi.SizeOfSPropValue), typeof(pSPropValue));
                    props[i] = new SPropValue(lpProp);
                }
                cemapi.MAPIFreeBuffer(propVals);
                return props;
            }

            public bool SetProps(PropTags[] tags, object[] values)
            {
                int num = tags.Length;
                if (num != values.Length)
                    throw new Exception("Num tags must be same as num of values!!");

                IntPtr array = Marshal.AllocHGlobal(cemapi.SizeOfSPropValue * num);

                for (int i = 0; i < num; i++)
                {
                    PropTags tag = tags[i];
                    object value = values[i];

                    pSPropValue val = new pSPropValue();
                    val.ulPropTag = (uint)tag;
                    switch ((PT)((uint)tag & 0xFFFF))
                    {
                        case PT.PT_BINARY:
                        case PT.PT_TSTRING:
                            throw new Exception("Can't set property " + tag.ToString() + '!');
                        case PT.PT_BOOLEAN:
                            val.Value.li = (bool)value ? 1UL : 0UL;
                            break;
                        case PT.PT_SYSTIME:
                        case PT.PT_I8:
                            val.Value.li = (ulong)value;
                            break;
                        case PT.PT_I2:
                            val.Value.li = (ulong)((short)value);
                            break;
                        case PT.PT_LONG:
                            val.Value.li = (ulong)((uint)value);
                            break;
                        default:
                            throw new Exception("Can't set property " + tag.ToString() + '!');
                    }
                    Marshal.StructureToPtr(val, (IntPtr)((uint)array + cemapi.SizeOfSPropValue * i), false);
                }

                HRESULT hr = pIMAPIPropSetProps(this.ptr, (uint)num, array);
                Marshal.FreeHGlobal(array);

                if (hr == HRESULT.S_OK)
                    return true;
                else if (hr == HRESULT.MAPI_E_COMPUTED)
                    return false;
                else
                    throw new Exception("SetProps failed: " + hr.ToString());
            }
        }

        #endregion
    }
}
