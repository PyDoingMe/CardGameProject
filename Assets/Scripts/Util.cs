using System;
using UnityEngine;
using System.Runtime.InteropServices;//마샬링을 위한 어셈블리


namespace Utility
{
    enum ECardState { CanNothing, CanMouseOver, CanMouseDrag }
    static public class Constants
    {
        public const int MULLIGAN_COUNT = 4;
        public const int HEADER_SIZE = 4;//헤더 사이즈는 36(ID:ushort + Size:ushort + 이름 : 32)
        public const int MAX_SEND_MSG_BYTE = 100;//전송 메시지의 최대 바이트 수 : 한글33글자, 영어숫자10글자

        public static Vector3 MousePos
        {
            get 
            {
                Vector3 vector3 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));
                vector3.z = -3;
                return vector3;
            }
        }


        /// <summary>
        /// 헤더 구조체 마샬링
        /// </summary>
        [StructLayout(LayoutKind.Sequential/*들어오는순서대로(Queue)*/, Pack = 1/*데이터를 읽을 단위*/)]
        public struct stHeader
        {
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 MsgID; // 메시지 ID
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 PacketSize; // 메시지 크기
        }
        /// <summary>
        /// 헤더 구조체 마샬링 함수(Byte->구조체)
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static stHeader HeaderfromByte(byte[] arr)
        {
            //구조체 초기화
            stHeader str = default(stHeader);
            int size = Marshal.SizeOf(str);//구조체 Size

            //Size만큼 메모리 할당(메모리 자리 빌리기)
            IntPtr ptr = Marshal.AllocHGlobal(size);

            //데이터를 복사하여 메모리에 넣기(데이터 쑤셔넣기)
            Marshal.Copy(arr, 0, ptr, size);

            //구조체에 넣기(쑤셔넣은 데이터 정리 해서 구조체에 넣기)
            str = (stHeader)Marshal.PtrToStructure(ptr, str.GetType());
            //할당한 메모리 해제
            Marshal.FreeHGlobal(ptr);

            //구조체 리턴
            return str;
        }
        /// <summary>
        /// 헤더 구조체 마샬링 함수(구조체->Byte)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetHeaderToByte(stHeader str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);

            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// 내 정보 변경 메시지 구조체 마샬링
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct stTwoIntMsg
        {
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 MsgID; // 메시지 ID
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 PacketSize; // 메시지 크기

            [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
            public UInt16 int1;
            [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
            public UInt16 int2;
        }
        /// <summary>
        /// 내 정보 변경 메시지 구조체 마샬링 함수(Byte->구조체)
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static stTwoIntMsg TwoIntfromByte(byte[] arr)
        {
            //구조체 초기화
            stTwoIntMsg str = default(stTwoIntMsg);
            int size = Marshal.SizeOf(str);//구조체 Size

            //Size만큼 메모리 할당(메모리 자리 빌리기)
            IntPtr ptr = Marshal.AllocHGlobal(size);

            //데이터를 복사하여 메모리에 넣기(데이터 쑤셔넣기)
            Marshal.Copy(arr, 0, ptr, size);

            //구조체에 넣기(쑤셔넣은 데이터 정리 해서 구조체에 넣기)
            str = (stTwoIntMsg)Marshal.PtrToStructure(ptr, str.GetType());
            //할당한 메모리 해제
            Marshal.FreeHGlobal(ptr);

            //구조체 리턴
            return str;
        }
        /// <summary>
        /// 내 정보 변경 메시지 구조체 마샬링 함수(구조체->Byte)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetTwoIntMsgToByte(stTwoIntMsg str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);

            Marshal.FreeHGlobal(ptr);
            return arr;
        }


        /// <summary>
        /// 전송 메시지 구조체 마샬링
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct stSendMsg
        {
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 MsgID; // 메시지 ID
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 PacketSize; // 메시지 크기

            [MarshalAs(UnmanagedType.ByValTStr/*string*/, SizeConst = (int)(Constants.MAX_SEND_MSG_BYTE)/*전송 메시지의 최대 바이트*/)]
            public string strSendMsg; // 전송 메시지

        }
        /// <summary>
        /// 내 정보 변경 메시지 구조체 마샬링 함수(Byte->구조체)
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static stSendMsg SendMsgfromByte(byte[] arr)
        {
            //구조체 초기화
            stSendMsg str = default(stSendMsg);
            int size = Marshal.SizeOf(str);//구조체 Size

            //Size만큼 메모리 할당(메모리 자리 빌리기)
            IntPtr ptr = Marshal.AllocHGlobal(size);

            //데이터를 복사하여 메모리에 넣기(데이터 쑤셔넣기)
            Marshal.Copy(arr, 0, ptr, size);

            //구조체에 넣기(쑤셔넣은 데이터 정리 해서 구조체에 넣기)
            str = (stSendMsg)Marshal.PtrToStructure(ptr, str.GetType());
            //할당한 메모리 해제
            Marshal.FreeHGlobal(ptr);

            //구조체 리턴
            return str;
        }
        /// <summary>
        /// 내 정보 변경 메시지 구조체 마샬링 함수(구조체->Byte)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetSendMsgToByte(stSendMsg str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);

            Marshal.FreeHGlobal(ptr);
            return arr;
        }
    }

    [Serializable]
    public class PosRotSca
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 sca;

        public PosRotSca(Vector3 Pos, Quaternion Rot, Vector3 Sca) { pos = Pos; rot = Rot; sca = Sca; }
    }


}
public class Singleton<T> where T : new()
{
    protected static T instance;

    protected static T GetInstance()
    {
        if (instance == null)
        {
            instance = new T();
        }
        return instance;
    }
}