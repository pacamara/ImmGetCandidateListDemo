using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// For Chinese input to work need:
// *Use RichTextBox not TextBox
// *Ensure font is an Asian one
// *Ensure Chinese IME is globally enabled (since Win 8 not at app's discretion)
// *Set RichTextBox.ImeMode=On
namespace ImmGetCandidateListDemo
{
    public partial class ImmForm : Form
    {
        public ImmForm()
        {
            InitializeComponent();
            this.ActiveControl = immTextBox;
            immTextBox.ImeMode = System.Windows.Forms.ImeMode.On;
            immTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            swapWndProc(immTextBox.Handle);
        }

        const string PINYIN = "nihao";
        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        IntPtr wndProcBase;

        // Is member to ensure lifetime same as form
        WndProcDelegate wndProcCustom;

        private int swapWndProc(IntPtr hWndTarget)
        {
            wndProcCustom = new WndProcDelegate(ImmEnabledTextEditWndProc);
            wndProcBase = GetWindowLong(hWndTarget, GWL_WNDPROC);
            int res = SetWindowLong(hWndTarget, GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(wndProcCustom));
            return res;
        }

        public IntPtr ImmEnabledTextEditWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (WM_IME_SETCONTEXT==msg)
                {
                    // Needed to enable ImmGetCandidateList on Vista onwards
                    // See https://nyaruru.hatenablog.com/entry/20070307/p1
                    //
                    int lParamValue = lParam.ToInt32();
                    lParamValue &= ~ISC_SHOWUICANDIDATEWINDOW;
                    lParam = (IntPtr)lParamValue;
                    System.Diagnostics.Debug.WriteLine("Cleared ISC_SHOWUICANDIDATEWINDOW");
                }

                if (WM_IME_NOTIFY==msg)
                {
                    System.Diagnostics.Debug.WriteLine("WM_IME_NOTIFY: wParam="+wParam);
                    if (wParam.ToInt32()==IMN_CHANGECANDIDATE)
                    {
                        IntPtr context = ImmGetContext(immTextBox.Handle);
                        getCandidateList(context);
                        ImmReleaseContext(immTextBox.Handle, context);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + e);
            }

            return CallWindowProc(wndProcBase, hWnd, msg, wParam, lParam);
        }

        // TODO: Get ImmSetCompositionString working, stop using SendKeys hack
        private void immButton_Click(object sender, EventArgs e)
        {
            setCompositionStringViaSendKeysHack();

            //IntPtr context = ImmGetContext(textBox1.Handle);
            //setCompositionStringProperly(context);
            //ImmReleaseContext(textBox1.Handle, context);
        }

        private void setCompositionStringProperly(IntPtr context)
        {
            immTextBox.Focus();
            bool successSOS = ImmSetOpenStatus(context, true);

            int IME_CMODE_NATIVE = 1;
            int IME_CMODE_FULLSHAPE = 8;
            int cmode = IME_CMODE_NATIVE | IME_CMODE_FULLSHAPE;
            bool successSCS = ImmSetConversionStatus(context, cmode, 0);

            const int GCS_COMPREADSTR = 0x1;
            const int GCS_COMPSTR = 0x8;
            const int SCS_SETSTR = (GCS_COMPREADSTR | GCS_COMPSTR);
            StringBuilder sb = new StringBuilder();
            sb.Append(PINYIN);
            int successSCS2 = ImmSetCompositionString(context, SCS_SETSTR, sb, sb.Length, null, 0);

            bool successNICS = ImmNotifyIME(context, NI_COMPOSITIONSTR, CPS_CONVERT, 0);
            bool successNIOC = ImmNotifyIME(context, NI_OPENCANDIDATE, 0, 0);
        }

        private void setCompositionStringViaSendKeysHack()
        {
            immTextBox.Text = "";
            immTextBox.Focus();
            SendKeys.SendWait(PINYIN);
        }

        private void getCandidateList(IntPtr context)
        {
            System.Diagnostics.Debug.WriteLine("getCandidateList");

            // Always returns zero on Windows 10
            IntPtr candidateListCount = new IntPtr();
            int result = ImmGetCandidateListCountW(context, candidateListCount);
            System.Diagnostics.Debug.WriteLine("candidate list count=" + candidateListCount + " result="+result);
           
            int bytesNeeded = ImmGetCandidateListW(context, 0, IntPtr.Zero, 0);
            System.Diagnostics.Debug.WriteLine("bytesNeeded=" + bytesNeeded);

            if (0==bytesNeeded)
            {
                immCandidatesBox.Text = "";
                return;
            }

            IntPtr candidateListBytes = Marshal.AllocHGlobal(bytesNeeded);
            int bytesCopied = ImmGetCandidateListW(context, 0, candidateListBytes, bytesNeeded);
            System.Diagnostics.Debug.WriteLine("bytesCopied=" + bytesCopied);

            CANDIDATELIST list = new CANDIDATELIST();

            Marshal.PtrToStructure(candidateListBytes, list);
            byte[] buf = new byte[bytesNeeded];
            Marshal.Copy(candidateListBytes, buf, 0, bytesNeeded);
            Marshal.FreeHGlobal(candidateListBytes);

            System.Diagnostics.Debug.WriteLine("list.dwSize=" + list.dwSize);
            System.Diagnostics.Debug.WriteLine("list.dwStyle=" + list.dwStyle);
            System.Diagnostics.Debug.WriteLine("list.dwCount=" + list.dwCount);
            System.Diagnostics.Debug.WriteLine("list.dwSelection=" + list.dwSelection);
            System.Diagnostics.Debug.WriteLine("list.dwPageStart=" + list.dwPageStart);
            System.Diagnostics.Debug.WriteLine("list.dwPageSize=" + list.dwPageSize);
            System.Diagnostics.Debug.WriteLine("list.dwOffset (first offset)=" + list.dwOffset);

            const int members = 6;
            string output = "";
            for (int i = 0; i < list.dwCount; i++)
            {
                uint ithOffset = BitConverter.ToUInt32(buf, (members+i)*(sizeof(uint)));
                int strLen;
                if (i==list.dwCount-1)
                {
                    strLen = (int)(list.dwSize - ithOffset - 2);
                }
                else
                {
                    uint jthOffset = BitConverter.ToUInt32(buf, (members+i+1)*(sizeof(uint)));
                    strLen = (int)(jthOffset - ithOffset - 2);
                }
                string ithStr = Encoding.Unicode.GetString(buf, (int)ithOffset, strLen);
                string sLine = "Offset " + i + ": " + ithStr;
                System.Diagnostics.Debug.WriteLine(sLine);
                output += sLine + "\n";
            }
            immCandidatesBox.Text = output;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Win api constants. Some unused ones included for completeness
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        const int GWL_WNDPROC = -4;

        const uint WM_CREATE = 0x0001;
        const uint WM_CLOSE = 0x0010;
        const uint WM_IME_COMPOSITION = 271; // 0x10f
        const uint WM_PARENTNOTIFY = 0x0210;
        const uint WM_IME_SETCONTEXT = 0x0281;
        const uint WM_IME_NOTIFY = 0x0282;

        const uint IMN_CHANGECANDIDATE = 0x0003;
        const uint IMN_SETCANDIDATEPOS = 0x0009;
        const int ISC_SHOWUICANDIDATEWINDOW = 0x0001;

        const int NI_OPENCANDIDATE = 0x0010;
        const int NI_CLOSECANDIDATE = 0x0011;
        const int NI_SELECTCANDIDATESTR = 0x0012;
        const int NI_CHANGECANDIDATELIST = 0x0013;
        const int NI_FINALIZECONVERSIONRESULT = 0x0014;
        const int NI_COMPOSITIONSTR = 0x0015;

        const int CPS_COMPLETE = 0x0001;
        const int CPS_CONVERT = 0x0002;
        const int CPS_REVERT = 0x0003;
        const int CPS_CANCEL = 0x0004;

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // Relevant win apis
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr newWndProc);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("Imm32.dll")]
        public static extern bool ImmSetOpenStatus(IntPtr hIMC, bool fOpen);

        [DllImport("Imm32.dll")]
        private static extern int ImmSetCompositionString(
            IntPtr hImc, int dwIndex,
            System.Text.StringBuilder lpComp, int dwCompLen,
            System.Text.StringBuilder lpRead, int dwReadLen);

        [DllImport("Imm32.dll")]
        private static extern int ImmGetCompositionString(
            IntPtr hImc, int dwIndex,
            System.Text.StringBuilder lpComp, int dwCompLen);

        [DllImport("Imm32.dll")]
        public static extern bool ImmSetConversionStatus(IntPtr himc, int lpdw, int lpdw2);

        [DllImport("imm32.dll")]
        public static extern int ImmGetCandidateListW(
           IntPtr hIMC, int deIndex, IntPtr lpCandidateList,
           int dwBufLen);

        [DllImport("imm32.dll")]
        public static extern int ImmGetCandidateListCountW(IntPtr hIMC, IntPtr lpdwListCount);

        [DllImport("imm32.dll")]
        public static extern bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue);

        [StructLayout(LayoutKind.Sequential)]
        public class CANDIDATELIST
        {
            public uint dwSize;
            public uint dwStyle;
            public uint dwCount;
            public uint dwSelection;
            public uint dwPageStart;
            public uint dwPageSize;
            public uint dwOffset;
        }
    }
}
