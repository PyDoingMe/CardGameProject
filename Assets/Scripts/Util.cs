using System;
using UnityEngine;
using System.Runtime.InteropServices;//�������� ���� �����


namespace Utility
{
    enum ECardState { CanNothing, CanMouseOver, CanMouseDrag }
    static public class Constants
    {
        public const int MULLIGAN_COUNT = 4;
        public const int HEADER_SIZE = 4;//��� ������� 36(ID:ushort + Size:ushort + �̸� : 32)
        public const int MAX_SEND_MSG_BYTE = 100;//���� �޽����� �ִ� ����Ʈ �� : �ѱ�33����, �������10����

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
        /// ��� ����ü ������
        /// </summary>
        [StructLayout(LayoutKind.Sequential/*�����¼������(Queue)*/, Pack = 1/*�����͸� ���� ����*/)]
        public struct stHeader
        {
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 MsgID; // �޽��� ID
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 PacketSize; // �޽��� ũ��
        }
        /// <summary>
        /// ��� ����ü ������ �Լ�(Byte->����ü)
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static stHeader HeaderfromByte(byte[] arr)
        {
            //����ü �ʱ�ȭ
            stHeader str = default(stHeader);
            int size = Marshal.SizeOf(str);//����ü Size

            //Size��ŭ �޸� �Ҵ�(�޸� �ڸ� ������)
            IntPtr ptr = Marshal.AllocHGlobal(size);

            //�����͸� �����Ͽ� �޸𸮿� �ֱ�(������ ���ųֱ�)
            Marshal.Copy(arr, 0, ptr, size);

            //����ü�� �ֱ�(���ų��� ������ ���� �ؼ� ����ü�� �ֱ�)
            str = (stHeader)Marshal.PtrToStructure(ptr, str.GetType());
            //�Ҵ��� �޸� ����
            Marshal.FreeHGlobal(ptr);

            //����ü ����
            return str;
        }
        /// <summary>
        /// ��� ����ü ������ �Լ�(����ü->Byte)
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
        /// �� ���� ���� �޽��� ����ü ������
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct stTwoIntMsg
        {
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 MsgID; // �޽��� ID
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 PacketSize; // �޽��� ũ��

            [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
            public UInt16 int1;
            [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
            public UInt16 int2;
        }
        /// <summary>
        /// �� ���� ���� �޽��� ����ü ������ �Լ�(Byte->����ü)
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static stTwoIntMsg TwoIntfromByte(byte[] arr)
        {
            //����ü �ʱ�ȭ
            stTwoIntMsg str = default(stTwoIntMsg);
            int size = Marshal.SizeOf(str);//����ü Size

            //Size��ŭ �޸� �Ҵ�(�޸� �ڸ� ������)
            IntPtr ptr = Marshal.AllocHGlobal(size);

            //�����͸� �����Ͽ� �޸𸮿� �ֱ�(������ ���ųֱ�)
            Marshal.Copy(arr, 0, ptr, size);

            //����ü�� �ֱ�(���ų��� ������ ���� �ؼ� ����ü�� �ֱ�)
            str = (stTwoIntMsg)Marshal.PtrToStructure(ptr, str.GetType());
            //�Ҵ��� �޸� ����
            Marshal.FreeHGlobal(ptr);

            //����ü ����
            return str;
        }
        /// <summary>
        /// �� ���� ���� �޽��� ����ü ������ �Լ�(����ü->Byte)
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
        /// ���� �޽��� ����ü ������
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct stSendMsg
        {
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 MsgID; // �޽��� ID
            [MarshalAs(UnmanagedType.U2/*ushort*/, SizeConst = 2/*ushort size*/)]
            public UInt16 PacketSize; // �޽��� ũ��

            [MarshalAs(UnmanagedType.ByValTStr/*string*/, SizeConst = (int)(Constants.MAX_SEND_MSG_BYTE)/*���� �޽����� �ִ� ����Ʈ*/)]
            public string strSendMsg; // ���� �޽���

        }
        /// <summary>
        /// �� ���� ���� �޽��� ����ü ������ �Լ�(Byte->����ü)
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static stSendMsg SendMsgfromByte(byte[] arr)
        {
            //����ü �ʱ�ȭ
            stSendMsg str = default(stSendMsg);
            int size = Marshal.SizeOf(str);//����ü Size

            //Size��ŭ �޸� �Ҵ�(�޸� �ڸ� ������)
            IntPtr ptr = Marshal.AllocHGlobal(size);

            //�����͸� �����Ͽ� �޸𸮿� �ֱ�(������ ���ųֱ�)
            Marshal.Copy(arr, 0, ptr, size);

            //����ü�� �ֱ�(���ų��� ������ ���� �ؼ� ����ü�� �ֱ�)
            str = (stSendMsg)Marshal.PtrToStructure(ptr, str.GetType());
            //�Ҵ��� �޸� ����
            Marshal.FreeHGlobal(ptr);

            //����ü ����
            return str;
        }
        /// <summary>
        /// �� ���� ���� �޽��� ����ü ������ �Լ�(����ü->Byte)
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