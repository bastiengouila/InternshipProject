using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using Excel = Microsoft.Office.Interop.Excel;

namespace FPGA_AcquisitionBoard
{
    public partial class AcquisitionControl : UserControl
    {
        private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);
        Thread ThreadTransferData;
        Thread ThreadAutoTest;
        // Thread ThreadTest;
        private bool bPowerON = false;
        private bool bThreadTranferRunning = false;
        private bool bThreadControl = false;
        private bool bTestStop = false;
        private uint uiNbCurrentCycle = 0;
        private uint uiCurrentCycleStatus = 0;
        private string strAutoTestStatus = string.Empty;
        private uint uiTargetNbTeeth = 18;
        private string strError =String.Empty;
        private int iNbError = 0;

        //
        AcquisitionBoard Board = new AcquisitionBoard();

        SaveFileDialog savefileDialogData = new SaveFileDialog();

        private bool bPanelInitilized = false;
        struct AcquiChannel
        {
            public uint uiMeasuretype;
            public string strMeasuretype;
            public uint uiPolarity;
            public uint uiSweepsMax;
            public uint uiSweepsAcquired;
            public uint uiClock;
            public uint uiTrigger;
            public uint uiTriggerAngle;
            public uint uiTimeTicks;
            public string strClockType;
            public uint uiStatus;
            public double dQuantum;

        };
        private AcquiChannel CH1;
        private AcquiChannel CH2;
        private AcquiChannel CH3;
        private AcquiChannel CH4;

        public AcquisitionControl()
        {

            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            GlobalVariables.FileDirectory = WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.DataDirectory;

            InitializeComponent();

            fct_InitAcquisitionPanel();

            bPanelInitilized = true;
            this.statusStrip1.BringToFront();
        }

        //Occurs when the main formular is closing = release the COM port
        public void CloseControl()
        {
            try
            {
                fct_Acqui_SaveAcquisitionParam();
                Board.closeSerial();
                this.Cursor = null;
                lbl_Acquisition_ConnectionStatus.Text = "Not connected";
                lbl_Acquisition_ConnectionStatus.ForeColor = Color.Red;
                GlobalVariables.AcquisitionBoardConnected = false;
                panel_Acquisition.Enabled = false;
                but_Acqui_PreviousSettings.Enabled = false;
                but_Acquisition_Connect.Text = "Connect";
            }
            catch (Exception exErrorLog)
            {
                MessageBox.Show(exErrorLog.Message, "Error Close Control");
            }
        }

        #region Acquistion board functions

        //this function initializes the acquisition panel with default values
        private void fct_InitAcquisitionPanel()
        {
            int iIndex = 0;

            SerialPort port = new SerialPort();
            string[] strSerialPort = SerialPort.GetPortNames();
            cbox_Acquisition_PortName.DataSource = strSerialPort;
            foreach (string strCom in strSerialPort)
            {
                if (strCom == "COM" + GlobalVariables.AcquisitionBoard_Port.ToString())
                {
                    cbox_Acquisition_PortName.SelectedIndex = iIndex;
                    break;
                }
                iIndex++;
            }


            cbox_Acquisition_PortName.Refresh();

            //---------------------------------------Intitializes Measurements controls

            //--------Initialize Combobox data type
            string[] strDataName = { "angle (quadrature)", "Pin2: analog", "Pin2: pulsewidth", "Pin2: period","gear timer","angle (single phase-timed)",
                                     "Pin3: analog", "Pin3: pulsewidth", "Pin3: period", "Icc: analog (2-wire)", "Vcc pulsewidth" ,"Vcc period"};

            uint[] uiDataVal = { 0, 1, 2, 3, 4, 5, 17, 18, 19, 33, 34, 35 };

            DataTable dtDataType = new DataTable();
            DataColumn dcRegValue = new DataColumn("Value", typeof(string));
            DataColumn dcRegName = new DataColumn("Name", typeof(string));
            dtDataType.Columns.Add(dcRegValue);
            dtDataType.Columns.Add(dcRegName);
            for (int i = 0; i < strDataName.Length; i++)
            {
                DataRow drRegNameRow = dtDataType.NewRow();

                drRegNameRow["Name"] = strDataName[i];
                drRegNameRow["Value"] = uiDataVal[i];

                dtDataType.Rows.Add(drRegNameRow);
            }

            //fill combobox data type ch1
            cbox_Acqui_MeasurTypeCH1.DisplayMember = "Name";
            cbox_Acqui_MeasurTypeCH1.ValueMember = "Value";
            cbox_Acqui_MeasurTypeCH1.DataSource = dtDataType;

            //fill combobox data type ch2
            DataTable dtDataTypeCH2 = new DataTable();
            dtDataTypeCH2 = dtDataType.Copy();
            cbox_Acqui_MeasurTypeCH2.DisplayMember = "Name";
            cbox_Acqui_MeasurTypeCH2.ValueMember = "Value";
            cbox_Acqui_MeasurTypeCH2.DataSource = dtDataTypeCH2;


            //fill combobox data type ch3
            DataTable dtDataTypeCH3 = new DataTable();
            dtDataTypeCH3 = dtDataType.Copy();
            cbox_Acqui_MeasurTypeCH3.DisplayMember = "Name";
            cbox_Acqui_MeasurTypeCH3.ValueMember = "Value";
            cbox_Acqui_MeasurTypeCH3.DataSource = dtDataTypeCH3;


            //fill combobox data type ch4
            DataTable dtDataTypeCH4 = new DataTable();
            dtDataTypeCH4 = dtDataType.Copy();
            cbox_Acqui_MeasurTypeCH4.DisplayMember = "Name";
            cbox_Acqui_MeasurTypeCH4.ValueMember = "Value";
            cbox_Acqui_MeasurTypeCH4.DataSource = dtDataTypeCH4;

            cbox_Acqui_MeasurTypeCH1.SelectedIndex = 0;
            cbox_Acqui_MeasurTypeCH2.SelectedIndex = 5;
            cbox_Acqui_MeasurTypeCH3.SelectedIndex = 1;
            cbox_Acqui_MeasurTypeCH4.SelectedIndex = 0;

            //--------Initialize Combobox clock
            string[] strClock = { "Clock divider", "Pin2: Rise", "Pin2: Fall", "Pin2: R/F","Encoder Index","Pin3: Rise", "Pin3: Fall", "Pin3: R/F",
                                    "Icc: Rise", "Icc: Fall", "Icc: R/F" ,"Single-Edge decoder","Encoder B: R/F","Encoder A: R/F","Quadrature: R/F"};

            uint[] uiClockVal = { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15 };

            DataTable dtClock = new DataTable();
            DataColumn dcClockValue = new DataColumn("Value", typeof(string));
            DataColumn dcClockName = new DataColumn("Name", typeof(string));
            dtClock.Columns.Add(dcClockValue);
            dtClock.Columns.Add(dcClockName);
            for (int i = 0; i < strClock.Length; i++)
            {
                DataRow drClockNameRow = dtClock.NewRow();

                drClockNameRow["Name"] = strClock[i];
                drClockNameRow["Value"] = uiClockVal[i];

                dtClock.Rows.Add(drClockNameRow);
            }
            //fill combobox clock (ticks) ch1
            cbox_Acqui_MeasurTicksCH1.DataSource = dtClock;
            cbox_Acqui_MeasurTicksCH1.DisplayMember = "Name";
            cbox_Acqui_MeasurTicksCH1.ValueMember = "Value";

            //fill combobox clock (ticks) ch2
            DataTable dtClockCH2 = new DataTable();
            dtClockCH2 = dtClock.Copy();
            cbox_Acqui_MeasurTicksCH2.DataSource = dtClockCH2;
            cbox_Acqui_MeasurTicksCH2.DisplayMember = "Name";
            cbox_Acqui_MeasurTicksCH2.ValueMember = "Value";

            //fill combobox clock (ticks) ch3
            DataTable dtClockCH3 = new DataTable();
            dtClockCH3 = dtClock.Copy();
            cbox_Acqui_MeasurTicksCH3.DataSource = dtClockCH3;
            cbox_Acqui_MeasurTicksCH3.DisplayMember = "Name";
            cbox_Acqui_MeasurTicksCH3.ValueMember = "Value";

            //fill combobox clock (ticks) ch4
            DataTable dtClockCH4 = new DataTable();
            dtClockCH4 = dtClock.Copy();
            cbox_Acqui_MeasurTicksCH4.DataSource = dtClockCH4;
            cbox_Acqui_MeasurTicksCH4.DisplayMember = "Name";
            cbox_Acqui_MeasurTicksCH4.ValueMember = "Value";

            cbox_Acqui_MeasurTicksCH1.SelectedIndex = 2;
            cbox_Acqui_MeasurTicksCH2.SelectedIndex = 2;
            cbox_Acqui_MeasurTicksCH3.SelectedIndex = 14;
            cbox_Acqui_MeasurTicksCH4.SelectedIndex = 0;

            //--------Initialize Combobox trigger :
            string[] strTriggerCH1 = { "Always", "Angle", "POR" };
            string[] strTriggerCH2 = { "Always", "Angle", "POR" };
            string[] strTriggerCH3 = { "Always", "Angle", "POR" };
            string[] strTriggerCH4 = { "Always", "Angle", "POR" };
            cbox_Acqui_MeasurTriggerCH1.DataSource = strTriggerCH1;
            cbox_Acqui_MeasurTriggerCH2.DataSource = strTriggerCH2;
            cbox_Acqui_MeasurTriggerCH3.DataSource = strTriggerCH3;
            cbox_Acqui_MeasurTriggerCH4.DataSource = strTriggerCH4;
            cbox_Acqui_MeasurTriggerCH1.SelectedIndex = 1;
            cbox_Acqui_MeasurTriggerCH2.SelectedIndex = 1;
            cbox_Acqui_MeasurTriggerCH3.SelectedIndex = 0;
            cbox_Acqui_MeasurTriggerCH4.SelectedIndex = 0;


            //---------------------------------------Intitializes Measurements controls

            chkbox_Acqui_Pin2_Enable.Checked = false;
            chkbox_Acqui_Pin3_Enable.Checked = false; //by default pin 2 and 3 are disabled

            fct_Acqui_UpdatePinControls();

            chkbox_Acqui_MeasureChannel_CheckedChanged(null, null);//update measurement channel selection

            nUpDown_Encoderppr.Value = GlobalVariables.EncocderPPR;

        }

        //This function saves the configuration of the acquisition box into the local memory
        public void fct_Acqui_SaveAcquisitionParam()
        {
            int iCoutPin2 = 0;
            int iCoutPin3 = 0;
            int iPullUpResPin2 = 0;
            int iPullUpResPin3 = 0;
            //Vcc external or internal
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.VccExternal = chkbox_Acqui_VccExternal.Checked;
            //retrieves pin 2/pin 3 type (analog or digital)
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2Analog = !chkbox_Acqui_Pin2_Digital.Checked;
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3Analog = !chkbox_Acqui_Pin3_Digital.Checked;
            //retrieves pin 2/pin3 pullup resistor value
            if (chkbox_Acqui_Pin2_PullUp1K.Checked) { iPullUpResPin2 = iPullUpResPin2 + 1; }
            if (chkbox_Acqui_Pin2_PullUp4K.Checked) { iPullUpResPin2 = iPullUpResPin2 + 4; }
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2PullupRes = iPullUpResPin2;
            if (chkbox_Acqui_Pin3_PullUp1K.Checked) { iPullUpResPin3 = iPullUpResPin3 + 1; }
            if (chkbox_Acqui_Pin3_PullUp4K.Checked) { iPullUpResPin3 = iPullUpResPin3 + 4; }
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3PullupRes = iPullUpResPin3;
            //retrieves pin 2/pin3 Cout value
            if (chkbox_Acqui_Pin2_Cout1nF.Checked) { iCoutPin2 = iCoutPin2 + 1; }
            if (chkbox_Acqui_Pin2_Cout4nF.Checked) { iCoutPin2 = iCoutPin2 + 4; }
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2Cout = iCoutPin2;
            if (chkbox_Acqui_Pin3_Cout1nF.Checked) { iCoutPin3 = iCoutPin3 + 1; }
            if (chkbox_Acqui_Pin3_Cout4nF.Checked) { iCoutPin3 = iCoutPin3 + 4; }
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3Cout = iCoutPin3;
            //Pin 2/3 enabled
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2Enabled = chkbox_Acqui_Pin2_Enable.Checked;
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3Enabled = chkbox_Acqui_Pin3_Enable.Checked;
            //Encoder ppr
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.EncoderPPR = (int)GlobalVariables.EncocderPPR;
            //
            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.DataDirectory = GlobalVariables.FileDirectory;

            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Save();
        }

        //Updates the acquisition settings displayed.
        public void fct_Acqui_UpdatePinControls()
        {
            //----update in function of Pin2 type :
            if (chkbox_Acqui_Pin2_Digital.Checked)//enables controls related to digital mode
            {
                chkbox_Acqui_Pin2_PullUp1K.Enabled = true;
                chkbox_Acqui_Pin2_PullUp4K.Enabled = true;
                nUpDo_Acqui_Pin2_PullUpVolt.Enabled = true;
                txt_Acqui_Pin2_PullUpVoltRead.Enabled = true;
                lbl_Acqui_Pin2_Pullup.Enabled = true;
                lbl_Acqui_Pin2_PullUpvolt.Enabled = true;
                lbl_Acqui_Pin2_PullupRes.Enabled = true;
                lbl_Acqui_Pin2volt.Visible = false;
            }
            else//disables controls related to digital mode
            {
                chkbox_Acqui_Pin2_PullUp1K.Enabled = false;
                chkbox_Acqui_Pin2_PullUp4K.Enabled = false;
                nUpDo_Acqui_Pin2_PullUpVolt.Enabled = false;
                txt_Acqui_Pin2_PullUpVoltRead.Enabled = false;
                lbl_Acqui_Pin2_Pullup.Enabled = false;
                lbl_Acqui_Pin2_PullUpvolt.Enabled = false;
                lbl_Acqui_Pin2_PullupRes.Enabled = false;
                chkbox_Acqui_Pin2_PullUp1K.Checked = false;
                chkbox_Acqui_Pin2_PullUp4K.Checked = false;
                lbl_Acqui_Pin2volt.Visible = true;
                txt_Acqui_Pin2_OutRead.ForeColor = Color.Black;
            }
            //----update in function of Pin3 type :
            if (chkbox_Acqui_Pin3_Digital.Checked)//disables controls related to digital mode
            {
                chkbox_Acqui_Pin3_PullUp1K.Enabled = true;
                chkbox_Acqui_Pin3_PullUp4K.Enabled = true;
                nUpDo_Acqui_Pin3_PullUpVolt.Enabled = true;
                txt_Acqui_Pin3_PullUpVoltRead.Enabled = true;
                lbl_Acqui_Pin3_Pullup.Enabled = true;
                lbl_Acqui_Pin3_PullUpvolt.Enabled = true;
                lbl_Acqui_Pin3_PullupRes.Enabled = true;
                lbl_Acqui_Pin3volt.Visible = false;
            }
            else //enables controls related to digital mode
            {
                chkbox_Acqui_Pin3_PullUp1K.Enabled = false;
                chkbox_Acqui_Pin3_PullUp4K.Enabled = false;
                nUpDo_Acqui_Pin3_PullUpVolt.Enabled = false;
                txt_Acqui_Pin3_PullUpVoltRead.Enabled = false;
                lbl_Acqui_Pin3_Pullup.Enabled = false;
                lbl_Acqui_Pin3_PullUpvolt.Enabled = false;
                lbl_Acqui_Pin3_PullupRes.Enabled = false;
                chkbox_Acqui_Pin3_PullUp1K.Checked = false;
                chkbox_Acqui_Pin3_PullUp4K.Checked = false;
                lbl_Acqui_Pin3volt.Visible = true;
                txt_Acqui_Pin3_OutRead.ForeColor = Color.Black;
            }

            if (chkbox_Acqui_VccInternal.Checked)//disables Vcc controls-> Vcc is from external source
            {
                nUpDo_Acqui_SetVcc.Enabled = true;
                lbl_Acqui_Vcc_Value.Enabled = true;
                but_Acqui_Vcc_ONOFF.Enabled = true;
                nUpDo_Acqui_SetVcc_Threshold.Enabled = true;
                lbl_Acqui_Vcc_CompThreshold.Enabled = true;
                nUpDo_Acqui_SetIcc_Threshold.Enabled = true;
                lbl_Acqui_Icc_CompThreshold.Enabled = true;
                txt_Acqui_Vcc_ThresholdVoltRead.Enabled = true;
                txt_Acqui_Icc_Threshold_mARead.Enabled = true;
            }
            else//enables Vcc controls-> Vcc is internal (on-board)
            {
                nUpDo_Acqui_SetVcc.Enabled = false;
                lbl_Acqui_Vcc_Value.Enabled = false;
                but_Acqui_Vcc_ONOFF.Enabled = false;
                nUpDo_Acqui_SetVcc_Threshold.Enabled = false;
                lbl_Acqui_Vcc_CompThreshold.Enabled = false;
                nUpDo_Acqui_SetIcc_Threshold.Enabled = false;
                lbl_Acqui_Icc_CompThreshold.Enabled = false;
                txt_Acqui_Vcc_ThresholdVoltRead.Enabled = false;
                txt_Acqui_Icc_Threshold_mARead.Enabled = false;
            }

            gbox_Acqui_Pin3.Enabled = chkbox_Acqui_Pin3_Enable.Checked;
            gbox_Acqui_Pin2.Enabled = chkbox_Acqui_Pin2_Enable.Checked;

        }

        //Configure the acquisition board relays with the values displayed.
        private void fct_Acqui_SetRelays()
        {

            bool[] bNewconfig = new bool[16];

            bNewconfig[0] = chkbox_Acqui_Pin2_PullUp1K.Checked;
            bNewconfig[1] = chkbox_Acqui_Pin2_PullUp4K.Checked;
            bNewconfig[2] = chkbox_Acqui_Pin2_Cout4nF.Checked;
            bNewconfig[3] = chkbox_Acqui_Pin2_Cout1nF.Checked;
            bNewconfig[4] = chkbox_Acqui_Pin2_Digital.Checked;
            bNewconfig[5] = !chkbox_Acqui_Pin2_Digital.Checked;
            bNewconfig[6] = chkbox_Acqui_VccInternal.Checked;//Vcc relay 0-> external, 1 internal
            bNewconfig[7] = false;//NC
            bNewconfig[8] = chkbox_Acqui_Pin3_PullUp1K.Checked;
            bNewconfig[9] = chkbox_Acqui_Pin3_PullUp4K.Checked;
            bNewconfig[10] = chkbox_Acqui_Pin3_Cout4nF.Checked;
            bNewconfig[11] = chkbox_Acqui_Pin3_Cout1nF.Checked;
            bNewconfig[12] = chkbox_Acqui_Pin3_Digital.Checked;
            bNewconfig[13] = !chkbox_Acqui_Pin3_Digital.Checked;
            bNewconfig[14] = false;//Vcc to pin 2 relay
            bNewconfig[15] = false;//NC

            if (chkbox_Acqui_Pin2_Enable.Checked == false)//Pin 2 disabled, set all relays on pin 2 to 0
            {
                bNewconfig[0] = false;
                bNewconfig[1] = false;
                bNewconfig[2] = false;
                bNewconfig[3] = false;
                bNewconfig[4] = false;
                bNewconfig[5] = false;
            }

            if (chkbox_Acqui_Pin3_Enable.Checked == false)//Pin 3 disabled, set all relays on pin 3 to 0
            {
                bNewconfig[8] = false;
                bNewconfig[9] = false;
                bNewconfig[10] = false;
                bNewconfig[11] = false;
                bNewconfig[12] = false;
                bNewconfig[13] = false;
            }

            //convert bin array to bin string
            string strBin = "";
            for (int i = 0; i < 16; i++)
            {
                if (bNewconfig[i] == true)
                    strBin = "1" + strBin;
                else
                    strBin = "0" + strBin;

            }
            uint uiRelayValOnScreen = Convert.ToUInt32(strBin, 2);//convert to int
            Board.Write_Relays(uiRelayValOnScreen);//Set relays
        }

        //function that reads DUT values on board and refresh displayed values.
        public void fct_Acqui_RefreshDUT()
        {
            double dVcc = Math.Round(Board.Read_DUT_Vcc(), 3);
            double dIcc = Math.Round(Board.Read_DUT_IccmA(), 3);
            txt_Acqui_VccRead.Text = dVcc.ToString();
            txt_Acqui_IccRead.Text = dIcc.ToString();

            if (chkbox_Acqui_Pin2_Enable.Checked)
            {
                if (chkbox_Acqui_Pin2_Digital.Checked)//digital mode
                {
                    Board.set_DUT2Thresh_Voltage((double)nUpDo_Acqui_Pin2_Comp.Value);

                    double dOutPin2_PullUp = Math.Round(Board.Read_Pin2_PullUp(), 3);
                    txt_Acqui_Pin2_PullUpVoltRead.Text = dOutPin2_PullUp.ToString();
                    if (Board.Read_DUT_DigitalInput(2) == 1) { txt_Acqui_Pin2_OutRead.Text = "High"; txt_Acqui_Pin2_OutRead.ForeColor = Color.Green; }
                    else { txt_Acqui_Pin2_OutRead.Text = "Low"; txt_Acqui_Pin2_OutRead.ForeColor = Color.Blue; }

                }
                else//analog mode
                {
                    double dOutPin2 = Math.Round(Board.Read_Pin2(), 3);
                    txt_Acqui_Pin2_OutRead.Text = dOutPin2.ToString();
                }

            }

            if (chkbox_Acqui_Pin3_Enable.Checked)
            {
                if (chkbox_Acqui_Pin3_Digital.Checked)//digital mode
                {
                    Board.set_DUT3Thresh_Voltage((double)nUpDo_Acqui_Pin3_Comp.Value);

                    double dOutPin3_PullUp = Math.Round(Board.Read_Pin3_PullUp(), 3);
                    txt_Acqui_Pin3_PullUpVoltRead.Text = dOutPin3_PullUp.ToString();
                    if (Board.Read_DUT_DigitalInput(3) == 1) { txt_Acqui_Pin3_OutRead.Text = "High"; txt_Acqui_Pin3_OutRead.ForeColor = Color.Green; }
                    else { txt_Acqui_Pin3_OutRead.Text = "Low"; txt_Acqui_Pin3_OutRead.ForeColor = Color.Blue; }
                }
                else//analog mode
                {
                    double dOutPin3 = Math.Round(Board.Read_Pin3(), 3);
                    txt_Acqui_Pin3_OutRead.Text = dOutPin3.ToString();
                }

            }
        }

        //function that reads status of each channel and displays it
        public void fct_Acqui_RefreshStatus()
        {
            string[] strStatusString = new string[4];
            for (uint i = 0; i < 4; i++)
            {

                GlobalVariables.AcquisitionStatus[i] = Board.DataCaptureCycleStatus(i);
                if ((GlobalVariables.AcquisitionStatus[i] & 2) == 1)
                    strStatusString[i] = "Ongoing";
                if ((GlobalVariables.AcquisitionStatus[i] & 1) == 1)
                    strStatusString[i] = "Waiting trigger";
                if ((GlobalVariables.AcquisitionStatus[i] & 3) == 1)
                    strStatusString[i] = "Completed";
                if ((GlobalVariables.AcquisitionStatus[i] >> 3) == 1)
                    strStatusString[i] = "Overflow";
                if (GlobalVariables.AcquisitionStatus[i] == 0)
                    strStatusString[i] = "Idle";
            }
            CH1.uiStatus = GlobalVariables.AcquisitionStatus[0];
            CH2.uiStatus = GlobalVariables.AcquisitionStatus[1];
            CH3.uiStatus = GlobalVariables.AcquisitionStatus[2];
            CH4.uiStatus = GlobalVariables.AcquisitionStatus[3];

            txt_Acqui_MeasurStatusCH1.Text = strStatusString[0];
            txt_Acqui_MeasurStatusCH2.Text = strStatusString[1];
            txt_Acqui_MeasurStatusCH3.Text = strStatusString[2];
            txt_Acqui_MeasurStatusCH4.Text = strStatusString[3];

            txt_Acqui_MeasurStatusCH1.Refresh();
            txt_Acqui_MeasurStatusCH2.Refresh();
            txt_Acqui_MeasurStatusCH3.Refresh();
            txt_Acqui_MeasurStatusCH4.Refresh();
        }


        //When looking at the combobox port list, auto updates it.
        private void cbox_ComPort_DropDown(object sender, EventArgs e)
        {
            cbox_Acquisition_PortName.DataSource = SerialPort.GetPortNames();

        }

        //click on button connect
        public void but_Acquisition_Connect_Click(object sender, EventArgs e)
        {
            if (GlobalVariables.AcquisitionBoardConnected == false)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    string strPort = "";
                    if (cbox_Acquisition_PortName.SelectedValue != null)
                    {
                        strPort = cbox_Acquisition_PortName.SelectedValue.ToString();
                    }
                    if (strPort.Contains("COM"))
                    {
                        string strVer = Board.InitSerial(strPort, GlobalVariables.AcquisitionBoard_BaudRate);

                        if (strVer != "")
                        {
                            lbl_Acquisition_BoardVersion.Text = strVer;
                            lbl_Acquisition_ConnectionStatus.Text = "Connected";
                            lbl_Acquisition_ConnectionStatus.ForeColor = Color.Green;
                            Board.InitBoard();//set all off for safety

                            Board.Set_ppr(GlobalVariables.EncocderPPR);
                            GlobalVariables.AcquisitionBoardConnected = true;
                            panel_Acquisition.Enabled = true;
                            but_Acqui_PreviousSettings.Enabled = true;
                            int iportNumber = Convert.ToInt32(strPort.Remove(0, 3));

                            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.AcquisitionPort = iportNumber;//save the last working port number into memory
                            WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Save();
                            GlobalVariables.AcquisitionBoard_Port = iportNumber;
                            this.Cursor = null;

                            fct_Acqui_SetRelays();
                            but_Acquisition_Connect.Text = "Disconnect";

                        }
                        else
                        {
                            throw new Exception("The board did not answer");
                        }
                    }
                    else
                    {
                        throw new Exception("no COM port selected");
                    }

                }
                catch (Exception ExError)
                {
                    this.Cursor = null;
                    lbl_Acquisition_ConnectionStatus.Text = "Not connected";
                    lbl_Acquisition_ConnectionStatus.ForeColor = Color.Red;
                    MessageBox.Show(ExError.Message,"Error Connection");
                    GlobalVariables.AcquisitionBoardConnected = false;
                    panel_Acquisition.Enabled = false;
                    but_Acqui_PreviousSettings.Enabled = false;
                    but_Acquisition_Connect.Text = "Connect";
                }
            }

            else
            {
                try
                {
                    Board.closeSerial();
                    this.Cursor = null;
                    lbl_Acquisition_ConnectionStatus.Text = "Not connected";
                    lbl_Acquisition_ConnectionStatus.ForeColor = Color.Red;
                    GlobalVariables.AcquisitionBoardConnected = false;
                    panel_Acquisition.Enabled = false;
                    but_Acqui_PreviousSettings.Enabled = false;
                    but_Acquisition_Connect.Text = "Connect";
                }
                catch (Exception ExError)
                {
                    MessageBox.Show("Disconnection Failed");
                }
            }
        }

        //checkbox pin2 digital check state changed
        private void chkbox_Acqui_Pin2_Digital_CheckedChanged(object sender, EventArgs e)
        {
            if (bPanelInitilized)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    txt_Acqui_Pin2_OutRead.Clear();
                    fct_Acqui_UpdatePinControls();//unckeck,check related controls
                    fct_Acqui_SetRelays();//Configure board according to controls checked
                    fct_Acqui_RefreshDUT();//refresh DUT displayed values

                    this.Cursor = null;
                }
                catch (Exception exError)
                {
                    this.Cursor = null;
                    MessageBox.Show(exError.Message, "Error Pin 2 Digit Check");
                }
            }
        }

        //checkbox pin3 digital check state changed
        private void chkbox_Acqui_Pin3_Digital_CheckedChanged(object sender, EventArgs e)
        {
            if (bPanelInitilized)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    txt_Acqui_Pin3_OutRead.Clear();
                    fct_Acqui_UpdatePinControls();//unckeck,check related controls
                    fct_Acqui_SetRelays();//Configure board according to controls checked
                    fct_Acqui_RefreshDUT();//refresh DUT displayed values

                    this.Cursor = null;
                }
                catch (Exception exError)
                {
                    this.Cursor = null;
                    MessageBox.Show(exError.Message, "Error Pin 3 Digit Check");
                }
            }
        }

        //click on checkbox Vcc internal 
        public void chkbox_Acqui_VccInternal_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                chkbox_Acqui_VccInternal.Checked = true;
                chkbox_Acqui_VccExternal.Checked = false;
                fct_Acqui_UpdatePinControls();//unckeck,check related controls
                fct_Acqui_SetRelays();//Configure board according to controls checked
                fct_Acqui_RefreshDUT();//refresh DUT displayed values
                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Vcc Internal");
            }
        }

        //click on checkbox Vcc external 
        public void chkbox_Acqui_VccExternal_Click(object sender, EventArgs e)
        {

            try
            {
                this.Cursor = Cursors.WaitCursor;
                chkbox_Acqui_VccInternal.Checked = false;
                chkbox_Acqui_VccExternal.Checked = true;
                fct_Acqui_UpdatePinControls();//unckeck,check related controls
                fct_Acqui_SetRelays();//Configure board according to controls checked
                fct_Acqui_RefreshDUT();//refresh DUT displayed values
                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Vcc External");
            }
        }

        //Read DUT values 
        public void but_Acqui_Refresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                fct_Acqui_RefreshDUT();
                fct_Acqui_RefreshStatus();

                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Refresh");
            }
        }

        //click on powr ON/OFF VCC
        public void but_Acqui_Vcc_ONOFF_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                if (but_Acqui_Vcc_ONOFF.Text == "Power OFF")
                {
                    Board.Set_DUT_Vcc(0);
                    but_Acqui_Vcc_ONOFF.Text = "Power ON";
                    bPowerON = false;
                }
                else
                {
                    Board.Set_DUT_Vcc((double)nUpDo_Acqui_SetVcc.Value);
                    but_Acqui_Vcc_ONOFF.Text = "Power OFF";
                    bPowerON = true;
                }
                double dVcc = Math.Round(Board.Read_DUT_Vcc(), 3);
                double dIcc = Math.Round(Board.Read_DUT_IccmA(), 3);
                txt_Acqui_VccRead.Text = dVcc.ToString();
                txt_Acqui_IccRead.Text = dIcc.ToString();

                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Power ON-OFF");
            }
        }

        //click on use previous settings button :
        private void but_Acqui_PreviousSettings_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                //---------------------------uses program settigns to retreive previous session settings :------------------------------------
                //-----pin 2/pin 3 type (analog or digital)
                chkbox_Acqui_Pin2_Digital.Checked = !WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2Analog;
                chkbox_Acqui_Pin3_Digital.Checked = !WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3Analog;
                //-----Pin2/Pin3 pullup resistor:
                if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2PullupRes == 0) { chkbox_Acqui_Pin2_PullUp1K.Checked = false; chkbox_Acqui_Pin2_PullUp4K.Checked = false; }//no pullup connected on pin 2
                else if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2PullupRes == 1) { chkbox_Acqui_Pin2_PullUp1K.Checked = true; chkbox_Acqui_Pin2_PullUp4K.Checked = false; }//pullup res pin2 = 1.21 KOhms 
                else if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2PullupRes == 4) { chkbox_Acqui_Pin2_PullUp1K.Checked = false; chkbox_Acqui_Pin2_PullUp4K.Checked = true; }//pullup res pin2 = 4.7 KOhms 
                else { chkbox_Acqui_Pin2_PullUp1K.Checked = true; chkbox_Acqui_Pin2_PullUp4K.Checked = true; }//pullup res pin2 = 960 Ohms (1.21//4.7)
                if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3PullupRes == 0) { chkbox_Acqui_Pin3_PullUp1K.Checked = false; chkbox_Acqui_Pin3_PullUp4K.Checked = false; }//no pullup connected on pin 3
                else if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3PullupRes == 1) { chkbox_Acqui_Pin3_PullUp1K.Checked = true; chkbox_Acqui_Pin3_PullUp4K.Checked = false; }//pullup res pin3 = 1.21 KOhms 
                else if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3PullupRes == 4) { chkbox_Acqui_Pin3_PullUp1K.Checked = false; chkbox_Acqui_Pin3_PullUp4K.Checked = true; }//pullup res pin3 = 4.7 KOhms 
                else { chkbox_Acqui_Pin3_PullUp1K.Checked = true; chkbox_Acqui_Pin3_PullUp4K.Checked = true; }//pullup res pin3 = 960 Ohms (1.21//4.7)
                //-----Cout capacitor Pin2/Pin3 :
                if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2Cout == 0) { chkbox_Acqui_Pin2_Cout1nF.Checked = false; chkbox_Acqui_Pin2_Cout4nF.Checked = false; }//Cout on pin2 = 0 nF
                else if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2Cout == 1) { chkbox_Acqui_Pin2_Cout1nF.Checked = true; chkbox_Acqui_Pin2_Cout4nF.Checked = false; }//Cout on pin2 = 1 nF
                else if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2Cout == 4) { chkbox_Acqui_Pin2_Cout1nF.Checked = false; chkbox_Acqui_Pin2_Cout4nF.Checked = true; }//Cout on pin2 = 4.7 nF
                else { chkbox_Acqui_Pin2_Cout1nF.Checked = true; chkbox_Acqui_Pin2_Cout4nF.Checked = true; }//Cout on pin2 = 5.7 nF (1+4.7)
                if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3Cout == 0) { chkbox_Acqui_Pin3_Cout1nF.Checked = false; chkbox_Acqui_Pin3_Cout4nF.Checked = false; }//Cout on pin2 = 0 nF
                else if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3Cout == 1) { chkbox_Acqui_Pin3_Cout1nF.Checked = true; chkbox_Acqui_Pin3_Cout4nF.Checked = false; }//Cout on pin2 = 1 nF
                else if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3Cout == 4) { chkbox_Acqui_Pin3_Cout1nF.Checked = false; chkbox_Acqui_Pin3_Cout4nF.Checked = true; }//Cout on pin2 = 4.7 nF
                else { chkbox_Acqui_Pin3_Cout1nF.Checked = true; chkbox_Acqui_Pin3_Cout4nF.Checked = true; }//Cout on pin2 = 5.7 nF (1+4.7)
                //---Vcc source internal or external
                if (WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.VccExternal == false) { chkbox_Acqui_VccInternal.Checked = true; chkbox_Acqui_VccExternal.Checked = false; }
                else { chkbox_Acqui_VccInternal.Checked = false; chkbox_Acqui_VccExternal.Checked = true; }
                //--Pin2 pin 3 enabled:
                chkbox_Acqui_Pin2_Enable.Checked = WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin2Enabled;
                chkbox_Acqui_Pin3_Enable.Checked = WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.Pin3Enabled;

                fct_Acqui_UpdatePinControls();
                fct_Acqui_SetRelays();
                fct_Acqui_RefreshDUT();
                fct_Acqui_RefreshStatus();
                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Previous settings");
            }


        }

        //click on resistor or capa in pin 2/3
        private void chkbox_Acqui_Pin_ResistorOrCapa_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                fct_Acqui_UpdatePinControls();
                fct_Acqui_SetRelays();
                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Resistor or Capa");
            }
        }


        //Check state changed Measure panel, CH1
        private void chkbox_Acqui_MeasureChannel_CheckedChanged(object sender, EventArgs e)
        {
            //CH1 controls :
            cbox_Acqui_MeasurTypeCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked;
            cbox_Acqui_MeasurTicksCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked;
            cbox_Acqui_MeasurTriggerCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked;
            chkbox_Acqui_MeasurPolCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked;
            if (cbox_Acqui_MeasurTriggerCH1.SelectedIndex == 1) { nUpDo_Acqui_MeasurAngleCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked; }//enable angle trigger only if trigger set on angle
            if (cbox_Acqui_MeasurTicksCH1.SelectedValue.ToString() == "0") { nUpDo_Acqui_MeasurTimeTickCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked; txt_Acqui_MeasurFrequencySweepsCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked; txt_Acqui_MeasurFrequencySweepsCH1.Text = System.Convert.ToString(1 / (nUpDo_Acqui_MeasurTimeTickCH1.Value) * 1000); }//enables tick time only if clock divider is selected
            nUpDo_Acqui_SweepsCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked;
            txt_Acqui_MeasurRemainingSweepsCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked;
            txt_Acqui_MeasurStatusCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked;

            //CH2 controls :
            cbox_Acqui_MeasurTypeCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked;
            cbox_Acqui_MeasurTicksCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked;
            cbox_Acqui_MeasurTriggerCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked;
            chkbox_Acqui_MeasurPolCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked;
            if (cbox_Acqui_MeasurTriggerCH2.SelectedIndex == 1) { nUpDo_Acqui_MeasurAngleCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked; }//enable angle trigger only if trigger set on angle
            if (cbox_Acqui_MeasurTicksCH2.SelectedValue.ToString() == "0") { nUpDo_Acqui_MeasurTimeTickCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked; txt_Acqui_MeasurFrequencySweepsCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked; txt_Acqui_MeasurFrequencySweepsCH2.Text = System.Convert.ToString(1 / (nUpDo_Acqui_MeasurTimeTickCH2.Value) * 1000); }//enables tick time only if clock divider is selected
            nUpDo_Acqui_SweepsCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked;
            txt_Acqui_MeasurRemainingSweepsCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked;
            txt_Acqui_MeasurStatusCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked;

            //CH3 controls :
            cbox_Acqui_MeasurTypeCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked;
            cbox_Acqui_MeasurTicksCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked;
            cbox_Acqui_MeasurTriggerCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked;
            chkbox_Acqui_MeasurPolCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked;
            if (cbox_Acqui_MeasurTriggerCH3.SelectedIndex == 1) { nUpDo_Acqui_MeasurAngleCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked; }//enable angle trigger only if trigger set on angle
            if (cbox_Acqui_MeasurTicksCH3.SelectedValue.ToString() == "0") { nUpDo_Acqui_MeasurTimeTickCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked; txt_Acqui_MeasurFrequencySweepsCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked; txt_Acqui_MeasurFrequencySweepsCH3.Text = System.Convert.ToString(1 / (nUpDo_Acqui_MeasurTimeTickCH3.Value) * 1000); }//enables tick time only if clock divider is selected
            nUpDo_Acqui_SweepsCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked;
            txt_Acqui_MeasurRemainingSweepsCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked;
            txt_Acqui_MeasurStatusCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked;

            //CH4 controls :
            cbox_Acqui_MeasurTypeCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked;
            cbox_Acqui_MeasurTicksCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked;
            cbox_Acqui_MeasurTriggerCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked;
            chkbox_Acqui_MeasurPolCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked;
            if (cbox_Acqui_MeasurTriggerCH4.SelectedIndex == 1) { nUpDo_Acqui_MeasurAngleCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked; }//enable angle trigger only if trigger set on angle
            if (cbox_Acqui_MeasurTicksCH4.SelectedValue.ToString() == "0") { nUpDo_Acqui_MeasurTimeTickCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked; txt_Acqui_MeasurFrequencySweepsCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked; txt_Acqui_MeasurFrequencySweepsCH4.Text = System.Convert.ToString(1 / (nUpDo_Acqui_MeasurTimeTickCH4.Value) * 1000); }//enables tick time only if clock divider is selected
            nUpDo_Acqui_SweepsCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked;
            txt_Acqui_MeasurRemainingSweepsCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked;
            txt_Acqui_MeasurStatusCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked;

            if (!chkbox_Acqui_MeasureCH1.Checked && !chkbox_Acqui_MeasureCH2.Checked && !chkbox_Acqui_MeasureCH3.Checked && !chkbox_Acqui_MeasureCH4.Checked)//if no channel enabled, dimm the button start test
            { but_Acqui_Measur_Start.Enabled = false; }
            else { but_Acqui_Measur_Start.Enabled = true; }

        }

        //Value in numeric updown pullup pin 2 changed :
        private void nUpDo_Acqui_Pin2_PullUpVolt_ValueChanged(object sender, EventArgs e)
        {

            try
            {
                this.Cursor = Cursors.WaitCursor;
                if (chkbox_Acqui_Pin2_Enable.Checked && chkbox_Acqui_Pin2_Digital.Checked) { Board.set_DUT2Pullup_Voltage((double)nUpDo_Acqui_Pin2_PullUpVolt.Value); }//set pullup voltage

                double dOutPin2_PullUp = Math.Round(Board.Read_Pin2_PullUp(), 3);
                txt_Acqui_Pin2_PullUpVoltRead.Text = dOutPin2_PullUp.ToString();

                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Pin 2 PullUp");
            }

        }

        //Value in numeric updown pullup pin 3 changed :
        private void nUpDo_Acqui_Pin3_PullUpVolt_ValueChanged(object sender, EventArgs e)
        {

            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (chkbox_Acqui_Pin3_Enable.Checked && chkbox_Acqui_Pin3_Digital.Checked) { Board.set_DUT3Pullup_Voltage((double)nUpDo_Acqui_Pin3_PullUpVolt.Value); }//set pullup voltage

                double dOutPin3_PullUp = Math.Round(Board.Read_Pin3_PullUp(), 3);
                txt_Acqui_Pin3_PullUpVoltRead.Text = dOutPin3_PullUp.ToString();

                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Pin 3 PullUp");
            }

        }

        //Check state of pin 2 enable checkbox changed:
        private void chkbox_Acqui_Pin2_Enabled_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                this.Cursor = Cursors.WaitCursor;
                gbox_Acqui_Pin2.Enabled = chkbox_Acqui_Pin2_Enable.Checked;

                if (chkbox_Acqui_Pin2_Enable.Checked == false) { Board.set_DUT2Pullup_Voltage(0); }//set pullup to 0 V
                else { Board.set_DUT2Pullup_Voltage((double)nUpDo_Acqui_Pin2_PullUpVolt.Value); } //Reset pullup to its previous value   
                fct_Acqui_SetRelays();

                if (chkbox_Acqui_Pin2_Enable.Checked && chkbox_Acqui_Pin2_Digital.Checked) { Board.set_DUT2Pullup_Voltage((double)nUpDo_Acqui_Pin2_PullUpVolt.Value); }//set pullup voltage

                double dOutPin2 = Math.Round(Board.Read_Pin2(), 3);
                double dOutPin2_PullUp = Math.Round(Board.Read_Pin2_PullUp(), 3);
                txt_Acqui_Pin2_PullUpVoltRead.Text = dOutPin2_PullUp.ToString();
                txt_Acqui_Pin2_OutRead.Text = dOutPin2.ToString();

                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Pin 2 Enable");
            }



        }

        //Check state of pin 3 enable checkbox changed:
        private void chkbox_Acqui_Pin3_Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                gbox_Acqui_Pin3.Enabled = chkbox_Acqui_Pin3_Enable.Checked;

                if (chkbox_Acqui_Pin3_Enable.Checked == false) { Board.set_DUT3Pullup_Voltage(0); }//set pullup to 0 V
                else { Board.set_DUT3Pullup_Voltage((double)nUpDo_Acqui_Pin3_PullUpVolt.Value); } //Reset pullup to its previous value   
                fct_Acqui_SetRelays();

                if (chkbox_Acqui_Pin3_Enable.Checked && chkbox_Acqui_Pin3_Digital.Checked) { Board.set_DUT3Pullup_Voltage((double)nUpDo_Acqui_Pin3_PullUpVolt.Value); }//set pullup voltage

                double dOutPin3 = Math.Round(Board.Read_Pin3(), 3);
                double dOutPin3_PullUp = Math.Round(Board.Read_Pin3_PullUp(), 3);
                txt_Acqui_Pin3_PullUpVoltRead.Text = dOutPin3_PullUp.ToString();
                txt_Acqui_Pin3_OutRead.Text = dOutPin3.ToString();

                this.Cursor = null;
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Pin 3 Enable");
            }
        }

        //Selected index in combox trigger CH1,CH2, CH3 or CH4 changed
        private void cbox_Acqui_MeasurTrigger_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bPanelInitilized)//update only after panel is initialized
            {
                if (cbox_Acqui_MeasurTriggerCH1.SelectedIndex == 1) { nUpDo_Acqui_MeasurAngleCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked; }//enables angle selection only if trigger set on angle
                else { nUpDo_Acqui_MeasurAngleCH1.Enabled = false; }
                if (cbox_Acqui_MeasurTriggerCH2.SelectedIndex == 1) { nUpDo_Acqui_MeasurAngleCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked; }//enables angle selection only if trigger set on angle
                else { nUpDo_Acqui_MeasurAngleCH2.Enabled = false; }
                if (cbox_Acqui_MeasurTriggerCH3.SelectedIndex == 1) { nUpDo_Acqui_MeasurAngleCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked; }//enables angle selection only if trigger set on angle
                else { nUpDo_Acqui_MeasurAngleCH3.Enabled = false; }
                if (cbox_Acqui_MeasurTriggerCH4.SelectedIndex == 1) { nUpDo_Acqui_MeasurAngleCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked; }//enables angle selection only if trigger set on angle
                else { nUpDo_Acqui_MeasurAngleCH4.Enabled = false; }
            }
        }

        //Selected index in combox measure ticks CH1,CH2, CH3 or CH4 changed
        private void cbox_Acqui_MeasurTicks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bPanelInitilized)//update only after panel is initialized
            {
                switch (((ComboBox)sender).Name)
                {
                    case "cbox_Acqui_MeasurTicksCH1":

                        if (cbox_Acqui_MeasurTicksCH1.SelectedValue.ToString() == "0") { nUpDo_Acqui_MeasurTimeTickCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked; txt_Acqui_MeasurFrequencySweepsCH1.Enabled = chkbox_Acqui_MeasureCH1.Checked; txt_Acqui_MeasurFrequencySweepsCH1.Text = System.Convert.ToString(1 / (nUpDo_Acqui_MeasurTimeTickCH1.Value) * 1000); }//enables tick time only if clock divider is selected
                        else { nUpDo_Acqui_MeasurTimeTickCH1.Enabled = false; txt_Acqui_MeasurFrequencySweepsCH1.Enabled = false; }
                        break;

                    case "cbox_Acqui_MeasurTicksCH2":

                        if (cbox_Acqui_MeasurTicksCH2.SelectedValue.ToString() == "0") { nUpDo_Acqui_MeasurTimeTickCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked; txt_Acqui_MeasurFrequencySweepsCH2.Enabled = chkbox_Acqui_MeasureCH2.Checked; txt_Acqui_MeasurFrequencySweepsCH2.Text = System.Convert.ToString(1 / (nUpDo_Acqui_MeasurTimeTickCH2.Value) * 1000); }//enables tick time only if clock divider is selected
                        else { nUpDo_Acqui_MeasurTimeTickCH2.Enabled = false; txt_Acqui_MeasurFrequencySweepsCH2.Enabled = false; }
                        break;

                    case "cbox_Acqui_MeasurTicksCH3":

                        if (cbox_Acqui_MeasurTicksCH3.SelectedValue.ToString() == "0") { nUpDo_Acqui_MeasurTimeTickCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked; txt_Acqui_MeasurFrequencySweepsCH3.Enabled = chkbox_Acqui_MeasureCH3.Checked; txt_Acqui_MeasurFrequencySweepsCH3.Text = System.Convert.ToString(1 / (nUpDo_Acqui_MeasurTimeTickCH3.Value) * 1000); }//enables tick time only if clock divider is selected
                        else { nUpDo_Acqui_MeasurTimeTickCH3.Enabled = false; txt_Acqui_MeasurFrequencySweepsCH3.Enabled = false; }
                        break;

                    case "cbox_Acqui_MeasurTicksCH4":

                        if (cbox_Acqui_MeasurTicksCH4.SelectedValue.ToString() == "0") { nUpDo_Acqui_MeasurTimeTickCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked; txt_Acqui_MeasurFrequencySweepsCH4.Enabled = chkbox_Acqui_MeasureCH4.Checked; txt_Acqui_MeasurFrequencySweepsCH4.Text = System.Convert.ToString(1 / (nUpDo_Acqui_MeasurTimeTickCH4.Value) * 1000); }//enables tick time only if clock divider is selected
                        else { nUpDo_Acqui_MeasurTimeTickCH4.Enabled = false; txt_Acqui_MeasurFrequencySweepsCH4.Enabled = false; }
                        break;
                }

            }
        }

        //Click on start measurment button
        public void but_Acqui_Measur_Start_Click(object sender, EventArgs e)
        {
            //Set encoder PPR
            uint uiBitChEnabled = 0;
            Board.Set_ppr(GlobalVariables.EncocderPPR);

            double dLSB = 360.0 / (double)(GlobalVariables.EncocderPPR * 64);

            //Stop data colection channel
            Board.StartDataCaptureCycle(0, true);//stop any runnning measurements 
            //Reset the number of sweeps acquired
            CH1.uiSweepsAcquired = 0;
            CH2.uiSweepsAcquired = 0;
            CH3.uiSweepsAcquired = 0;
            CH4.uiSweepsAcquired = 0;

            //Set memory start adddress 
            Board.DataStartAddressDefault();//Set start address for all channels
            Thread.Sleep(100);

            //Set parameters for CH1
            if (chkbox_Acqui_MeasureCH1.Checked)
            {
                //Set measure type CH1 :
                CH1.uiMeasuretype = Convert.ToUInt32(cbox_Acqui_MeasurTypeCH1.SelectedValue);
                Board.DataSelection(0, CH1.uiMeasuretype, true);
                //Set polarity CH1
                CH1.uiPolarity = 0;
                if (chkbox_Acqui_MeasurPolCH1.Checked) { CH1.uiPolarity = 1; }
                Board.Polarity(0, CH1.uiPolarity, true);
                //Set number of sweeps CH1 :
                CH1.uiSweepsMax = Convert.ToUInt32(nUpDo_Acqui_SweepsCH1.Value);
                Board.DataMaximumCount(0, CH1.uiSweepsMax, true);
                //Set ticks (clock) CH1
                CH1.uiClock = Convert.ToUInt32(cbox_Acqui_MeasurTicksCH1.SelectedValue);
                CH1.strClockType = cbox_Acqui_MeasurTicksCH1.Text;
                Board.ClockSelection(0, CH1.uiClock, true);
                //Set Trigger CH1
                CH1.uiTrigger = Convert.ToUInt32(cbox_Acqui_MeasurTriggerCH1.SelectedIndex);
                Board.TriggerSelection(0, CH1.uiTrigger, true);
                //Set Angle Trigger CH1 (no effect if trigger is not on angle) 
                double dWantedAngleCH1 = (float)nUpDo_Acqui_MeasurAngleCH1.Value;
                CH1.uiTriggerAngle = 0;
                CH1.uiTriggerAngle = Convert.ToUInt32(Math.Round(dWantedAngleCH1 / dLSB, 0));
                Board.TriggerAngleSelection(0, CH1.uiTriggerAngle, true);//Angle that generates trigger pulse LSB is encoder dependent.	Normally 360/(PPR*64)
                //Set timeticks CH1:
                CH1.uiTimeTicks = 0;
                double dTimeticksCH1 = (float)nUpDo_Acqui_MeasurTimeTickCH1.Value;//get time between ticks in us (no effect if clock(ticks) is not on clock divider)
                if (dTimeticksCH1 > 0.01) { CH1.uiTimeTicks = Convert.ToUInt32((float)(dTimeticksCH1 * 100) - 1); }
                Board.ClockDivider(0, CH1.uiTimeTicks, true);
                uiBitChEnabled = uiBitChEnabled + 1;
            }

            //Set parameters for CH2
            if (chkbox_Acqui_MeasureCH2.Checked)
            {
                //Set measure type CH2 :
                CH2.uiMeasuretype = Convert.ToUInt32(cbox_Acqui_MeasurTypeCH2.SelectedValue);
                Board.DataSelection(1, CH2.uiMeasuretype, true);
                //Set polarity CH2
                CH2.uiPolarity = 0;
                if (chkbox_Acqui_MeasurPolCH2.Checked) { CH2.uiPolarity = 1; }
                Board.Polarity(1, CH2.uiPolarity, true);
                //Set number of sweeps CH2 :
                CH2.uiSweepsMax = Convert.ToUInt32(nUpDo_Acqui_SweepsCH2.Value);
                Board.DataMaximumCount(1, CH2.uiSweepsMax, true);
                //Set ticks (clock) CH2
                CH2.uiClock = Convert.ToUInt32(cbox_Acqui_MeasurTicksCH2.SelectedValue);
                CH2.strClockType = cbox_Acqui_MeasurTicksCH2.Text;
                Board.ClockSelection(1, CH2.uiClock, true);
                //Set Trigger CH2
                CH2.uiTrigger = Convert.ToUInt32(cbox_Acqui_MeasurTriggerCH2.SelectedIndex);
                Board.TriggerSelection(1, CH2.uiTrigger, true);
                //Set Angle Trigger CH2 (no effect if trigger is not on angle) 
                double dWantedAngleCH2 = (float)nUpDo_Acqui_MeasurAngleCH2.Value;
                CH2.uiTriggerAngle = 0;
                CH2.uiTriggerAngle = Convert.ToUInt32(Math.Round(dWantedAngleCH2 / dLSB, 0));
                Board.TriggerAngleSelection(1, CH2.uiTriggerAngle, true);//Angle that generates trigger pulse LSB is encoder dependent.	Normally 360/(PPR*64)
                //Set timeticks CH2:
                CH2.uiTimeTicks = 0;
                double dTimeticksCH2 = (float)nUpDo_Acqui_MeasurTimeTickCH2.Value;//get time between ticks in us (no effect if clock(ticks) is not on clock divider)
                if (dTimeticksCH2 > 0.01) { CH2.uiTimeTicks = Convert.ToUInt32((float)(dTimeticksCH2 * 100) - 1); }
                Board.ClockDivider(1, CH2.uiTimeTicks, true);
                uiBitChEnabled = uiBitChEnabled + 2;
            }

            //Set parameters for CH3
            if (chkbox_Acqui_MeasureCH3.Checked)
            {
                //Set measure type CH3 :
                CH3.uiMeasuretype = Convert.ToUInt32(cbox_Acqui_MeasurTypeCH3.SelectedValue);
                Board.DataSelection(2, CH3.uiMeasuretype, true);
                //Set polarity CH3
                CH3.uiPolarity = 0;
                if (chkbox_Acqui_MeasurPolCH3.Checked) { CH3.uiPolarity = 1; }
                Board.Polarity(2, CH3.uiPolarity, true);
                //Set number of sweeps CH3 :
                CH3.uiSweepsMax = Convert.ToUInt32(nUpDo_Acqui_SweepsCH3.Value);
                Board.DataMaximumCount(2, CH3.uiSweepsMax, true);
                //Set ticks (clock) CH3
                CH3.uiClock = Convert.ToUInt32(cbox_Acqui_MeasurTicksCH3.SelectedValue);
                CH3.strClockType = cbox_Acqui_MeasurTicksCH3.Text;
                Board.ClockSelection(2, CH3.uiClock, true);
                //Set Trigger CH3
                CH3.uiTrigger = Convert.ToUInt32(cbox_Acqui_MeasurTriggerCH3.SelectedIndex);
                Board.TriggerSelection(2, CH3.uiTrigger, true);
                //Set Angle Trigger CH3 (no effect if trigger is not on angle) 
                double dWantedAngleCH3 = (float)nUpDo_Acqui_MeasurAngleCH3.Value;
                CH3.uiTriggerAngle = 0;
                CH3.uiTriggerAngle = Convert.ToUInt32(Math.Round(dWantedAngleCH3 / dLSB, 0));
                Board.TriggerAngleSelection(2, CH3.uiTriggerAngle, true);//Angle that generates trigger pulse LSB is encoder dependent.	Normally 360/(PPR*64)
                //Set timeticks CH3:
                CH3.uiTimeTicks = 0;
                double dTimeticksCH3 = (float)nUpDo_Acqui_MeasurTimeTickCH3.Value;//get time between ticks in us (no effect if clock(ticks) is not on clock divider)
                if (dTimeticksCH3 > 0.01) { CH3.uiTimeTicks = Convert.ToUInt32((float)(dTimeticksCH3 * 100) - 1); }
                Board.ClockDivider(2, CH3.uiTimeTicks, true);
                uiBitChEnabled = uiBitChEnabled + 4;
            }

            //Set parameters for CH4
            if (chkbox_Acqui_MeasureCH4.Checked)
            {
                //Set measure type CH4 :
                CH4.uiMeasuretype = Convert.ToUInt32(cbox_Acqui_MeasurTypeCH4.SelectedValue);
                Board.DataSelection(3, CH4.uiMeasuretype, true);
                //Set polarity CH4
                CH4.uiPolarity = 0;
                if (chkbox_Acqui_MeasurPolCH4.Checked) { CH4.uiPolarity = 1; }
                Board.Polarity(3, CH4.uiPolarity, true);
                //Set number of sweeps CH4 :
                CH4.uiSweepsMax = Convert.ToUInt32(nUpDo_Acqui_SweepsCH4.Value);
                Board.DataMaximumCount(3, CH4.uiSweepsMax, true);
                //Set ticks (clock) CH4
                CH4.uiClock = Convert.ToUInt32(cbox_Acqui_MeasurTicksCH4.SelectedValue);
                CH4.strClockType = cbox_Acqui_MeasurTicksCH4.Text;
                Board.ClockSelection(3, CH4.uiClock, true);
                //Set Trigger CH4
                CH4.uiTrigger = Convert.ToUInt32(cbox_Acqui_MeasurTriggerCH4.SelectedIndex);
                Board.TriggerSelection(3, CH4.uiTrigger, true);
                //Set Angle Trigger CH4 (no effect if trigger is not on angle) 
                double dWantedAngleCH4 = (float)nUpDo_Acqui_MeasurAngleCH4.Value;
                CH4.uiTriggerAngle = 0;
                CH4.uiTriggerAngle = Convert.ToUInt32(Math.Round(dWantedAngleCH4 / dLSB, 0));
                Board.TriggerAngleSelection(3, CH4.uiTriggerAngle, true);//Angle that generates trigger pulse LSB is encoder dependent.	Normally 360/(PPR*64)
                //Set timeticks CH3:
                CH4.uiTimeTicks = 0;
                double dTimeticksCH4 = (float)nUpDo_Acqui_MeasurTimeTickCH4.Value;//get time between ticks in us (no effect if clock(ticks) is not on clock divider)
                if (dTimeticksCH4 > 0.01) { CH4.uiTimeTicks = Convert.ToUInt32((float)(dTimeticksCH4 * 100) - 1); }
                Board.ClockDivider(3, CH4.uiTimeTicks, true);
                uiBitChEnabled = uiBitChEnabled + 8;
            }

            //Start data colection channel for enabled channels :
            Board.StartDataCaptureCycle(uiBitChEnabled, true);


            //Start Thread that monitor current measurements status
            timer_RefreshDataAcquisition.Interval = 1000;
            timer_RefreshDataAcquisition.Enabled = true;
            timer_RefreshDataAcquisition.Start();

        }

        //allow use of deleguate in Thread
        public static void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe), new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.SetProperty, null, control, new object[] { propertyValue });
            }
        }

        //This function updates the channel parameters defined in channel structure (only enabled channels, set disabled to 0) (according to parameters on measure panel)
        public void fct_UpdateChannelsQuantumAndItemNumber()
        {
            CH1.uiSweepsAcquired = 0;
            CH2.uiSweepsAcquired = 0;
            CH3.uiSweepsAcquired = 0;
            CH4.uiSweepsAcquired = 0;
            CH1.dQuantum = 1;
            CH2.dQuantum = 1;
            CH3.dQuantum = 1;
            CH4.dQuantum = 1;
            CH1.uiMeasuretype = 99;
            CH2.uiMeasuretype = 99;
            CH3.uiMeasuretype = 99;
            CH4.uiMeasuretype = 99;
            CH1.strMeasuretype = "";
            CH2.strMeasuretype = "";
            CH3.strMeasuretype = "";
            CH4.strMeasuretype = "";

            //CH1 :
            if (chkbox_Acqui_MeasureCH1.Checked)
            {

                //CH1.uiMeasuretype = Board.DataSelection(0, 0, false);//get previous data type and update channel structure //this was a good idea but it seems the board doesn't keep in memory the previous settings  after repower
                CH1.uiMeasuretype = Convert.ToUInt32(cbox_Acqui_MeasurTypeCH1.SelectedValue.ToString());//better to use user selected values

                //Quantum :
                if (CH1.uiMeasuretype == 0 || CH1.uiMeasuretype == 5)//angle
                {
                    CH1.dQuantum = 360.0 / ((double)GlobalVariables.EncocderPPR * 64);

                    if (CH1.uiMeasuretype == 0) { CH1.strMeasuretype = "Angle [°] (Quadrature)"; }
                    else { CH1.strMeasuretype = "Angle [°] (Single phase)"; }
                }
                else if (CH1.uiMeasuretype == 1 || CH1.uiMeasuretype == 17)//analog pin 2/3
                {
                    CH1.dQuantum = 76.3E-3; // 76.3E-6 uV/LSB  = > 20/(2^18-1)
                    if (CH1.uiMeasuretype == 1) { CH1.strMeasuretype = "Pin 2 [mV]"; }
                    else { CH1.strMeasuretype = "Pin 3 [mV]"; }
                }
                /*else if (CH1.uiMeasuretype == 36)//analog Vcc
                {
                    CH1.dQuantum = 609E-3; // 609E-6 uV/LSB  = > (20/(2^18-1))*7.98
                    CH1.strMeasuretype = "Vcc [mV]";
                }*/
                else if (CH1.uiMeasuretype == 33)//analog Icc
                {
                    CH1.dQuantum = 956E-6; // 956E-9 nA/LSB = > (20/(2^18-1))/79.9
                    CH1.strMeasuretype = "Icc [mA]";
                }
                else if (CH1.uiMeasuretype == 2 || CH1.uiMeasuretype == 3 || CH1.uiMeasuretype == 18 || CH1.uiMeasuretype == 19 || CH1.uiMeasuretype == 34 || CH1.uiMeasuretype == 35)//measuring time
                {
                    CH1.dQuantum = 1.0 / (100E6); // 1/100M LSB => 100E3 us/LSB
                    if (CH1.uiMeasuretype == 2) { CH1.strMeasuretype = "Pin 2 Pulse width [s]"; }
                    else if (CH1.uiMeasuretype == 3) { CH1.strMeasuretype = "Pin 2 Period [s]"; }
                    else if (CH1.uiMeasuretype == 18) { CH1.strMeasuretype = "Pin 3 Pulse width [s]"; }
                    else if (CH1.uiMeasuretype == 19) { CH1.strMeasuretype = "Pin 3 Period [s]"; }
                    else if (CH1.uiMeasuretype == 34) { CH1.strMeasuretype = "Vcc Pulse width [s]"; }
                    else if (CH1.uiMeasuretype == 35) { CH1.strMeasuretype = "Vcc Period [s]"; }
                }
                else //measuring gear timer, dont know what this is....
                {
                    CH1.dQuantum = 1.0;
                    CH1.strMeasuretype = "Gear timer [Dec]";
                }
                //Get number of sweeps measured :
                //CH1.uiSweepsAcquired = Board.GetNumberOfSweeps(0);//same as upper, number of sweeps acquired is reset to 0 after board switched Off-on
                CH1.uiSweepsAcquired = (uint)nUpDo_Acqui_SweepsCH1.Value;
            }

            //CH2 :
            if (chkbox_Acqui_MeasureCH2.Checked)
            {
                CH2.uiMeasuretype = Board.DataSelection(1, 0, false);//get previous data type and update channel structure
                //Quantum :
                if (CH2.uiMeasuretype == 0 || CH2.uiMeasuretype == 5)//angle
                {
                    CH2.dQuantum = 360.0 / ((double)GlobalVariables.EncocderPPR * 64);

                    if (CH2.uiMeasuretype == 0) { CH2.strMeasuretype = "Angle [°] (Quadrature)"; }
                    else { CH2.strMeasuretype = "Angle [°] (Single phase)"; }
                }
                else if (CH2.uiMeasuretype == 1 || CH2.uiMeasuretype == 17)//analog pin 2/3
                {
                    CH2.dQuantum = 76.3E-3; // 76.3E-6 uV/LSB  = > 20/(2^18-1)
                    if (CH2.uiMeasuretype == 1) { CH2.strMeasuretype = "Pin 2 [mV]"; }
                    else { CH2.strMeasuretype = "Pin 3 [mV]"; }
                }
                /*else if (CH2.uiMeasuretype == 36)//analog Vcc
                {
                    CH2.dQuantum = 609E-3; // 609E-6 uV/LSB  = > (20/(2^18-1))*7.98
                    CH2.strMeasuretype = "Vcc [mV]";
                }*/
                else if (CH2.uiMeasuretype == 33)//analog Icc
                {
                    CH2.dQuantum = 956E-6; // 956E-9 nA/LSB = > (20/(2^18-1))/79.9
                    CH2.strMeasuretype = "Icc [mA]";
                }
                else if (CH2.uiMeasuretype == 2 || CH2.uiMeasuretype == 3 || CH2.uiMeasuretype == 18 || CH2.uiMeasuretype == 19 || CH2.uiMeasuretype == 34 || CH2.uiMeasuretype == 35)//measuring time
                {
                    CH2.dQuantum = 1.0 / (100E6); // 1/100M LSB => 100E3 us/LSB
                    if (CH2.uiMeasuretype == 2) { CH2.strMeasuretype = "Pin 2 Pulse width [s]"; }
                    else if (CH2.uiMeasuretype == 3) { CH2.strMeasuretype = "Pin 2 Period [s]"; }
                    else if (CH2.uiMeasuretype == 18) { CH2.strMeasuretype = "Pin 3 Pulse width [s]"; }
                    else if (CH2.uiMeasuretype == 19) { CH2.strMeasuretype = "Pin 3 Period [s]"; }
                    else if (CH2.uiMeasuretype == 34) { CH2.strMeasuretype = "Vcc Pulse width [s]"; }
                    else if (CH2.uiMeasuretype == 35) { CH2.strMeasuretype = "Vcc Period [s]"; }
                }
                else //measuring gear timer, dont know what this is....
                {
                    CH2.dQuantum = 1.0;
                    CH2.strMeasuretype = "Gear timer [Dec]";
                }
                //Get number of sweeps measured :
                //CH2.uiSweepsAcquired = Board.GetNumberOfSweeps(1);
                CH2.uiSweepsAcquired = (uint)nUpDo_Acqui_SweepsCH2.Value;
            }

            //CH3
            if (chkbox_Acqui_MeasureCH3.Checked)
            {
                CH3.uiMeasuretype = Board.DataSelection(2, 0, false);//get previous data type and update channel structure
                //Quantum :
                if (CH3.uiMeasuretype == 0 || CH3.uiMeasuretype == 5)//angle
                {
                    CH3.dQuantum = 360.0 / ((double)GlobalVariables.EncocderPPR * 64);

                    if (CH3.uiMeasuretype == 0) { CH3.strMeasuretype = "Angle [°] (Quadrature)"; }
                    else { CH3.strMeasuretype = "Angle [°] (Single phase)"; }
                }
                else if (CH3.uiMeasuretype == 1 || CH3.uiMeasuretype == 17)//analog pin 2/3
                {
                    CH3.dQuantum = 76.3E-3; // 76.3E-6 uV/LSB  = > 20/(2^18-1)
                    if (CH3.uiMeasuretype == 1) { CH3.strMeasuretype = "Pin 2 [mV]"; }
                    else { CH3.strMeasuretype = "Pin 3 [mV]"; }
                }
                /*else if (CH3.uiMeasuretype == 36)//analog Vcc
                {
                    CH3.dQuantum = 609E-3; // 609E-6 uV/LSB  = > (20/(2^18-1))*7.98
                    CH3.strMeasuretype = "Vcc [mV]";
                }*/
                else if (CH3.uiMeasuretype == 33)//analog Icc
                {
                    CH3.dQuantum = 956E-6; // 956E-9 nA/LSB = > (20/(2^18-1))/79.9
                    CH3.strMeasuretype = "Icc [mA]";
                }
                else if (CH3.uiMeasuretype == 2 || CH3.uiMeasuretype == 3 || CH3.uiMeasuretype == 18 || CH3.uiMeasuretype == 19 || CH3.uiMeasuretype == 34 || CH3.uiMeasuretype == 35)//measuring time
                {
                    CH3.dQuantum = 1.0 / (100E6); // 1/100M LSB => 100E3 us/LSB
                    if (CH3.uiMeasuretype == 2) { CH3.strMeasuretype = "Pin 2 Pulse width [s]"; }
                    else if (CH3.uiMeasuretype == 3) { CH3.strMeasuretype = "Pin 2 Period [s]"; }
                    else if (CH3.uiMeasuretype == 18) { CH3.strMeasuretype = "Pin 3 Pulse width [s]"; }
                    else if (CH3.uiMeasuretype == 19) { CH3.strMeasuretype = "Pin 3 Period [s]"; }
                    else if (CH3.uiMeasuretype == 34) { CH3.strMeasuretype = "Vcc Pulse width [s]"; }
                    else if (CH3.uiMeasuretype == 35) { CH3.strMeasuretype = "Vcc Period [s]"; }
                }
                else //measuring gear timer, dont know what this is....
                {
                    CH3.dQuantum = 1.0;
                    CH3.strMeasuretype = "Gear timer [Dec]";
                }
                //Get number of sweeps measured :
                //CH3.uiSweepsAcquired = Board.GetNumberOfSweeps(2);
                CH3.uiSweepsAcquired = (uint)nUpDo_Acqui_SweepsCH3.Value;
            }

            //CH4
            if (chkbox_Acqui_MeasureCH4.Checked)
            {
                CH4.uiMeasuretype = Board.DataSelection(3, 0, false);//get previous data type and update channel structure
                //Quantum :
                if (CH4.uiMeasuretype == 0 || CH4.uiMeasuretype == 5)//angle
                {
                    CH4.dQuantum = 360.0 / ((double)GlobalVariables.EncocderPPR * 64);

                    if (CH4.uiMeasuretype == 0) { CH4.strMeasuretype = "Angle [°] (Quadrature)"; }
                    else { CH4.strMeasuretype = "Angle [°] (Single phase)"; }
                }
                else if (CH4.uiMeasuretype == 1 || CH4.uiMeasuretype == 17)//analog pin 2/3
                {
                    CH4.dQuantum = 76.3E-3; // 76.3E-6 uV/LSB  = > 20/(2^18-1)
                    if (CH4.uiMeasuretype == 1) { CH4.strMeasuretype = "Pin 2 [mV]"; }
                    else { CH4.strMeasuretype = "Pin 3 [mV]"; }
                }
                /*else if (CH4.uiMeasuretype == 36)//analog Vcc
                {
                    CH4.dQuantum = 609E-3; // 609E-6 uV/LSB  = > (20/(2^18-1))*7.98
                    CH4.strMeasuretype = "Vcc [mV]";
                }*/
                else if (CH4.uiMeasuretype == 33)//analog Icc
                {
                    CH4.dQuantum = 956E-6; // 956E-9 nA/LSB = > (20/(2^18-1))/79.9
                    CH4.strMeasuretype = "Icc [mA]";
                }
                else if (CH4.uiMeasuretype == 2 || CH4.uiMeasuretype == 3 || CH4.uiMeasuretype == 18 || CH4.uiMeasuretype == 19 || CH4.uiMeasuretype == 34 || CH4.uiMeasuretype == 35)//measuring time
                {
                    CH4.dQuantum = 1.0 / (100E6); // 1/100M LSB => 100E3 us/LSB
                    if (CH4.uiMeasuretype == 2) { CH4.strMeasuretype = "Pin 2 Pulse width [s]"; }
                    else if (CH4.uiMeasuretype == 3) { CH4.strMeasuretype = "Pin 2 Period [s]"; }
                    else if (CH4.uiMeasuretype == 18) { CH4.strMeasuretype = "Pin 3 Pulse width [s]"; }
                    else if (CH4.uiMeasuretype == 19) { CH4.strMeasuretype = "Pin 3 Period [s]"; }
                    else if (CH4.uiMeasuretype == 34) { CH4.strMeasuretype = "Vcc Pulse width [s]"; }
                    else if (CH4.uiMeasuretype == 35) { CH4.strMeasuretype = "Vcc Period [s]"; }
                }
                else //measuring gear timer, dont know what this is....
                {
                    CH4.dQuantum = 1.0;
                    CH4.strMeasuretype = "Gear timer [Dec]";
                }
                //Get number of sweeps measured :
                // CH4.uiSweepsAcquired = Board.GetNumberOfSweeps(3);
                CH4.uiSweepsAcquired = (uint)nUpDo_Acqui_SweepsCH4.Value;
            }





        }

        //Save the results in a CSV Excel file
        public void SaveAcquisitionData()
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            xlApp = new Excel.Application();
            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            bThreadTranferRunning = true;
            try
            {
                savefileDialogData.CreatePrompt = true;
                savefileDialogData.OverwritePrompt = true;
                savefileDialogData.InitialDirectory = GlobalVariables.FileDirectory;

                int iCurrentExcelLine = 0;
                int iNumberOfChannels = 0;

                //Get direcory using the savefile dialogbox
                if (savefileDialogData.ShowDialog() == DialogResult.OK)
                {

                    bool bsaveInsameFile = false;

                    if (chkbox_Acqui_MeasureCH1.Checked) { iNumberOfChannels++; }
                    if (chkbox_Acqui_MeasureCH2.Checked) { iNumberOfChannels++; }
                    if (chkbox_Acqui_MeasureCH3.Checked) { iNumberOfChannels++; }
                    if (chkbox_Acqui_MeasureCH4.Checked) { iNumberOfChannels++; }

                    //Define number of items to save
                    uint uiItemsToSaveCH1 = 0;
                    uint uiItemsToSaveCH2 = 0;
                    uint uiItemsToSaveCH3 = 0;
                    uint uiItemsToSaveCH4 = 0;

                    uint uiItemsMaxCH1 = 0;
                    uint uiItemsMaxCH2 = 0;
                    uint uiItemsMaxCH3 = 0;
                    uint uiItemsMaxCH4 = 0;

                    if (chkbox_Acqui_MeasureCH1.Checked) { uiItemsToSaveCH1 = CH1.uiSweepsAcquired; uiItemsMaxCH1 = uiItemsToSaveCH1; }
                    if (chkbox_Acqui_MeasureCH2.Checked) { uiItemsToSaveCH2 = CH2.uiSweepsAcquired; uiItemsMaxCH2 = uiItemsToSaveCH2; }
                    if (chkbox_Acqui_MeasureCH3.Checked) { uiItemsToSaveCH3 = CH3.uiSweepsAcquired; uiItemsMaxCH3 = uiItemsToSaveCH3; }
                    if (chkbox_Acqui_MeasureCH4.Checked) { uiItemsToSaveCH4 = CH4.uiSweepsAcquired; uiItemsMaxCH4 = uiItemsToSaveCH4; }

                    //bSaveOK = true;
                    if (bsaveInsameFile == false)
                    {
                        iCurrentExcelLine = 0;
                        //If directory found, create Excel file and start measurements
                        GlobalVariables.FileDirectory = savefileDialogData.FileName;

                        //Delete the current file if it already exists :
                        if (System.IO.File.Exists(GlobalVariables.FileDirectory))
                        {
                            System.IO.File.Delete(GlobalVariables.FileDirectory);
                        }
                        //delete the 2 sheets automatically created in the excel file
                        xlWorkBook.Sheets[2].Delete();
                        xlWorkBook.Sheets[2].Delete();

                        //Configure data template :
                        if (uiItemsToSaveCH1 > 0)
                        {
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 1] = "Data CH1 #: ";
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 2] = CH1.strMeasuretype;
                        }
                        if (uiItemsToSaveCH2 > 0)
                        {
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 3] = "Data CH2 #: ";
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 4] = CH2.strMeasuretype;
                        }
                        if (uiItemsToSaveCH3 > 0)
                        {
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 5] = "Data CH3 #: ";
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 6] = CH3.strMeasuretype;
                        }
                        if (uiItemsToSaveCH4 > 0)
                        {
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 7] = "Data CH4 #: ";
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 8] = CH4.strMeasuretype;
                        }

                        xlApp.ActiveWorkbook.ConflictResolution = Excel.XlSaveConflictResolution.xlLocalSessionChanges;
                        //Save the excel workbook AS:
                        xlWorkBook.SaveAs(GlobalVariables.FileDirectory, //filename
                            Excel.XlFileFormat.xlCSV, //fileformat
                            misValue, //Password
                            misValue, //Write Res Password
                            misValue, //ReadOnlyRecommended
                            misValue, //CreateBackup
                            Excel.XlSaveAsAccessMode.xlExclusive,//AccessMode
                            Excel.XlSaveConflictResolution.xlLocalSessionChanges, //Conflict Resolution
                            misValue, //Add To Mru
                            misValue,//TextCodePage
                            misValue, //TextVisuallayout
                            misValue);//Local

                    }
                    /* else //open an existing workbook :
                     {
                         xlWorkBook = xlApp.Workbooks.Open(GlobalVariables.FileDirectory, 0, false, 5, "", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                         xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                         bSaveOK = true;
                         xlWorkSheet.Cells[iCurrentExcelLine + 1, 1] = "ID: ";
                         xlWorkSheet.Cells[iCurrentExcelLine + 1, 2] = "Angle [DEG]: ";
                     }*/

                    //Define Quantum corresponding to the data type acquired :




                    uint uiTotalItems = uiItemsToSaveCH1 + uiItemsToSaveCH2 + uiItemsToSaveCH3 + uiItemsToSaveCH4;
                    uint iReadingSize = 200;//read 200 items per reading cycle
                    UInt32 iVal = 0;

                    this.Invoke((MethodInvoker)delegate
                    {
                        tool_lblProgBar.Visible = true;
                        tool_ProgBar.Visible = true;
                        tool_ProgBar.Value = 0;
                        tool_ProgBar.Maximum = (int)uiTotalItems + 1;
                    });


                    //Get CH1
                    if (uiItemsToSaveCH1 > 0)
                    {
                        for (int iR = 0; iR < uiItemsToSaveCH1; iR++)
                        {
                            iVal = 0;
                            uint iStartItem = (uint)iR * iReadingSize;
                            string[] strRead = Board.ReadMemory(0, iStartItem, iReadingSize).Split(' ');//Read CH1 memory


                            int iMax = strRead.Length;
                            double[] dValueTable = new double[iMax];

                            if (uiItemsToSaveCH1 < iMax)
                            {

                                iMax = (int)uiItemsToSaveCH1;

                            }
                            for (uint i = 0; i < iMax; i++)
                            {
                                iVal = Convert.ToUInt32(strRead[i], 16);//convert hex number read to int

                                if (iVal > Math.Pow(2, 31) - 1)
                                {
                                    iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                                    dValueTable[i] = -(double)iVal * CH1.dQuantum;//convert to corresponding data type
                                }
                                else
                                {
                                    dValueTable[i] = (double)iVal * CH1.dQuantum;//convert to corresponding data type 
                                }

                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                xlWorkSheet.Cells[iCurrentExcelLine + 2, 1] = ((uint)iR * iReadingSize) + i;
                                xlWorkSheet.Cells[iCurrentExcelLine + 2, 2] = dValueTable[i].ToString();
                                if ((iCurrentExcelLine + 2) == (uiItemsMaxCH1 + 1))
                                {
                                    iCurrentExcelLine = 0;
                                }
                                else
                                {
                                    iCurrentExcelLine++;
                                }


                            }
                            if (uiItemsToSaveCH1 > iReadingSize) { uiItemsToSaveCH1 = uiItemsToSaveCH1 - iReadingSize; }
                            else { iR = (int)uiItemsToSaveCH1 - 1; }

                            this.Invoke((MethodInvoker)delegate
                            {
                                if ((tool_ProgBar.Value + (int)iReadingSize) < tool_ProgBar.Maximum)
                                    tool_ProgBar.Value += (int)iReadingSize;
                                else
                                    tool_ProgBar.Value = tool_ProgBar.Maximum;

                                Refresh();
                            });
                        }
                        xlWorkBook.Save();
                    }
                    //Get CH2
                    if (uiItemsToSaveCH2 > 0)
                    {
                        for (int iR = 0; iR < uiItemsToSaveCH2; iR++)
                        {
                            iVal = 0;
                            uint iStartItem = (uint)iR * iReadingSize;
                            string[] strRead = Board.ReadMemory(1, iStartItem, iReadingSize).Split(' ');//Read CH2 memory

                            int iMax = strRead.Length;
                            double[] dValueTable = new double[iMax];

                            for (uint i = 0; i < iMax; i++)
                            {
                                iVal = Convert.ToUInt32(strRead[i], 16);//convert hex number read to int
                                if (iVal > Math.Pow(2, 31) - 1)
                                {
                                    iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                                    dValueTable[i] = -(double)iVal * CH2.dQuantum;//convert to corresponding data type
                                }
                                else
                                {
                                    dValueTable[i] = (double)iVal * CH2.dQuantum;//convert to corresponding data type 
                                }
                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                //xlWorkSheet.Cells[iCurrentExcelLine + 2, 3] = ((uint)iR * iReadingSize) + i;
                                xlWorkSheet.Cells[iCurrentExcelLine + 2, 4] = dValueTable[i].ToString();
                                if ((iCurrentExcelLine + 2) == (uiItemsMaxCH2 + 1))
                                {
                                    iCurrentExcelLine = 0;
                                }
                                else
                                {
                                    iCurrentExcelLine++;
                                }
                            }
                            if (uiItemsToSaveCH2 > iReadingSize) { uiItemsToSaveCH2 = uiItemsToSaveCH2 - iReadingSize; }
                            else { iR = (int)uiItemsToSaveCH2 - 1; }

                            this.Invoke((MethodInvoker)delegate
                            {
                                if ((tool_ProgBar.Value + (int)iReadingSize) < tool_ProgBar.Maximum)
                                    tool_ProgBar.Value += (int)iReadingSize;
                                else
                                    tool_ProgBar.Value = tool_ProgBar.Maximum;

                                Refresh();
                            });
                        }
                        xlWorkBook.Save();
                    }
                    //Get CH3
                    if (uiItemsToSaveCH3 > 0)
                    {

                        for (int iR = 0; iR < uiItemsToSaveCH3; iR++)
                        {
                            iVal = 0;
                            uint iStartItem = (uint)iR * iReadingSize;
                            string[] strRead = Board.ReadMemory(2, iStartItem, iReadingSize).Split(' ');//Read CH3 memory

                            int iMax = strRead.Length;
                            double[] dValueTable = new double[iMax];

                            for (uint i = 0; i < iMax; i++)
                            {
                                iVal = Convert.ToUInt32(strRead[i], 16);//convert hex number read to int
                                if (iVal > Math.Pow(2, 31) - 1)
                                {
                                    iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                                    dValueTable[i] = -(double)iVal * CH3.dQuantum;//convert to corresponding data type
                                }
                                else
                                {
                                    dValueTable[i] = (double)iVal * CH3.dQuantum;//convert to corresponding data type 
                                }
                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                //xlWorkSheet.Cells[iCurrentExcelLine + 2, 5] = ((uint)iR * iReadingSize) + i;
                                xlWorkSheet.Cells[iCurrentExcelLine + 2, 6] = dValueTable[i].ToString();
                                if ((iCurrentExcelLine + 2) == (uiItemsMaxCH3 + 1))
                                {
                                    iCurrentExcelLine = 0;
                                }
                                else
                                {
                                    iCurrentExcelLine++;
                                }
                            }
                            if (uiItemsToSaveCH3 > iReadingSize) { uiItemsToSaveCH3 = uiItemsToSaveCH3 - iReadingSize; }
                            else { iR = (int)uiItemsToSaveCH3 - 1; }

                            this.Invoke((MethodInvoker)delegate
                            {
                                if ((tool_ProgBar.Value + (int)iReadingSize) < tool_ProgBar.Maximum)
                                    tool_ProgBar.Value += (int)iReadingSize;
                                else
                                    tool_ProgBar.Value = tool_ProgBar.Maximum;

                                Refresh();
                            });
                        }
                        xlWorkBook.Save();
                    }
                    //Get CH4
                    if (uiItemsToSaveCH4 > 0)
                    {
                        for (int iR = 0; iR < uiItemsToSaveCH4; iR++)
                        {
                            iVal = 0;
                            uint iStartItem = (uint)iR * iReadingSize;
                            string[] strRead = Board.ReadMemory(3, iStartItem, iReadingSize).Split(' ');//Read CH3 memory

                            int iMax = strRead.Length;
                            double[] dValueTable = new double[iMax];

                            for (uint i = 0; i < iMax; i++)
                            {
                                iVal = Convert.ToUInt32(strRead[i], 16);//convert hex number read to int
                                if (iVal > Math.Pow(2, 31) - 1)
                                {
                                    iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                                    dValueTable[i] = -(double)iVal * CH4.dQuantum;//convert to corresponding data type
                                }
                                else
                                {
                                    dValueTable[i] = (double)iVal * CH4.dQuantum;//convert to corresponding data type 
                                }
                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                //xlWorkSheet.Cells[iCurrentExcelLine + 2, 7] = ((uint)iR * iReadingSize) + i;
                                xlWorkSheet.Cells[iCurrentExcelLine + 2, 8] = dValueTable[i].ToString();

                                if ((iCurrentExcelLine + 2) == (uiItemsMaxCH4 + 1))
                                {
                                    iCurrentExcelLine = 0;
                                }
                                else
                                {
                                    iCurrentExcelLine++;
                                }
                            }
                            if (uiItemsToSaveCH4 > iReadingSize) { uiItemsToSaveCH4 = uiItemsToSaveCH4 - iReadingSize; }
                            else { iR = (int)uiItemsToSaveCH4 - 1; }

                            this.Invoke((MethodInvoker)delegate
                            {
                                if ((tool_ProgBar.Value + (int)iReadingSize) < tool_ProgBar.Maximum)
                                    tool_ProgBar.Value += (int)iReadingSize;
                                else
                                    tool_ProgBar.Value = tool_ProgBar.Maximum;

                                Refresh();
                            });
                        }
                        xlWorkBook.Save();
                    }
                }
            }
            catch (Exception exErrorlog)
            {
                MessageBox.Show("An error occured in the auto test :\n" + exErrorlog.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Invoke((MethodInvoker)delegate
                {
                    tool_ProgBar.Visible = false;
                    tool_lblProgBar.Visible = false;
                });

                //Close the workbook and release all objects 
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                bThreadTranferRunning = false;
            }
            MessageBox.Show("File saved", "");
        }


        //Release objects (for excel files) to avoid stack overflow
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception exErrorlog)
            {
                obj = null;
                MessageBox.Show("Exception occured while releasing object:\n" + exErrorlog.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                GC.Collect();
            }
        }

        //click on button Save to transfert the data from board to PC
        public void but_Acqui_SaveData_Click(object sender, EventArgs e)
        {
            if (bThreadTranferRunning == false)
            {
                fct_UpdateChannelsQuantumAndItemNumber();//Very important => updates data acquisition parameters (to know what will be saved)
                ThreadTransferData = new Thread(new ThreadStart(SaveAcquisitionData));
                ThreadTransferData.IsBackground = true;
                ThreadTransferData.SetApartmentState(ApartmentState.STA);
                ThreadTransferData.Start();
                bThreadTranferRunning = true;
            }
        }

        //Occurs when timer reaches number of ticks (every 1s) to update the measurement status
        private void timer_RefreshDataAcquisition_Tick(object sender, EventArgs e)
        {
            try
            {
                timer_RefreshDataAcquisition.Stop();

                fct_Acqui_RefreshDUT();
                fct_Acqui_RefreshStatus();


                bool bStop = true;
                foreach (uint uivalStatus in GlobalVariables.AcquisitionStatus)
                {
                    if ((uivalStatus & 2) == 1 || (uivalStatus & 1) == 1)
                    {
                        bStop = false;
                    }
                }

                timer_RefreshDataAcquisition.Start();
                timer_RefreshDataAcquisition.Enabled = bStop;


            }
            catch (Exception exError)
            {
                timer_RefreshDataAcquisition.Stop();
                timer_RefreshDataAcquisition.Enabled = false;
                MessageBox.Show(exError.Message, "Error Timer refresh");

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txt_Acqui_Pin2_OutRead.ForeColor = Color.Blue;
        }

        //When value on Comparator threshold numeric updown Pin 2 changed 
        private void nUpDo_Acqui_MeasurCompThreshPin2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkbox_Acqui_Pin2_Enable.Checked)
                {
                    this.Cursor = Cursors.WaitCursor;
                    Board.set_DUT2Thresh_Voltage((double)nUpDo_Acqui_Pin2_Comp.Value);
                    this.Cursor = null;
                }
            }
            catch (Exception exError)
            {
                this.Cursor = null;

                MessageBox.Show(exError.Message, "Error Pin 2 Threshold");
            }
        }

        //When value on Comparator threshold numeric updown Pin 3 changed 
        private void nUpDo_Acqui_MeasurCompThreshPin3_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkbox_Acqui_Pin3_Enable.Checked)
                {
                    this.Cursor = Cursors.WaitCursor;
                    Board.set_DUT3Thresh_Voltage((double)nUpDo_Acqui_Pin3_Comp.Value);
                    this.Cursor = null;
                }
            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Pin 3 Threshold");
            }
        }

        //When value on Comparator threshold numeric updown Vcc changed 
        private void nUpDo_Acqui_MeasurCompThreshVcc_ValueChanged(object sender, EventArgs e)
        {
            try
            {

                this.Cursor = Cursors.WaitCursor;
                Board.set_PONThresh_Voltage((double)nUpDo_Acqui_SetVcc_Threshold.Value);
                this.Cursor = null;
                txt_Acqui_Vcc_ThresholdVoltRead.Text = Convert.ToString(Math.Round(Board.Get_PONVccThresh_Voltage(), 3));

            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Vcc Threshold");
            }
        }

        //When value on Comparator threshold numeric updown Icc changed 
        private void nUpDo_Acqui_MeasurCompThreshIcc_ValueChanged(object sender, EventArgs e)
        {
            try
            {

                this.Cursor = Cursors.WaitCursor;
                double d2wireCurrentThreshold = 0;
                d2wireCurrentThreshold = ((double)nUpDo_Acqui_SetIcc_Threshold.Value) / 1000;
                Board.set_2Wire_Current(d2wireCurrentThreshold);
                this.Cursor = null;
                txt_Acqui_Icc_Threshold_mARead.Text = Convert.ToString(Board.Get_PONIccThresh_Voltage());

            }
            catch (Exception exError)
            {
                this.Cursor = null;
                MessageBox.Show(exError.Message, "Error Icc Threshold");
            }
        }

        //When value on timer ticks changed
        private void nUpDo_Acqui_MeasurTimeTicks_Value_Changed(object sender, EventArgs e)
        {
            switch (((NumericUpDown)sender).Name)
            {
                //time ticks changed on CH1
                case "nUpDo_Acqui_MeasurTimeTickCH1":
                    try
                    {
                        txt_Acqui_MeasurFrequencySweepsCH1.Text = System.Convert.ToString(Math.Round(1 / (nUpDo_Acqui_MeasurTimeTickCH1.Value) * 1000, 3));
                    }
                    catch (Exception exError)
                    {
                        MessageBox.Show("Wrong time ticks specified", "Error TimeTicks CH1");
                    }
                    break;

                //time ticks changed on CH2
                case "nUpDo_Acqui_MeasurTimeTickCH2":
                    try
                    {
                        txt_Acqui_MeasurFrequencySweepsCH2.Text = System.Convert.ToString(Math.Round(1 / (nUpDo_Acqui_MeasurTimeTickCH2.Value) * 1000, 3));
                    }
                    catch (Exception exError)
                    {
                        MessageBox.Show("Wrong time ticks specified", "Error TimeTicks CH2");
                    }
                    break;

                //time ticks changed on CH3
                case "nUpDo_Acqui_MeasurTimeTickCH3":
                    try
                    {
                        txt_Acqui_MeasurFrequencySweepsCH3.Text = System.Convert.ToString(Math.Round(1 / (nUpDo_Acqui_MeasurTimeTickCH3.Value) * 1000, 3));
                    }
                    catch (Exception exError)
                    {
                        MessageBox.Show("Wrong time ticks specified", "Error TimeTicks CH3");
                    }
                    break;

                //time ticks changed on CH4
                case "nUpDo_Acqui_MeasurTimeTickCH4":
                    try
                    {
                        txt_Acqui_MeasurFrequencySweepsCH4.Text = System.Convert.ToString(Math.Round(1 / (nUpDo_Acqui_MeasurTimeTickCH4.Value) * 1000, 3));
                    }
                    catch (Exception exError)
                    {
                        MessageBox.Show("Wrong time ticks specified", "Error TimeTicks CH4");
                    }
                    break;
            }
        }

        //When measure type on one channel changed
        private void cbox_Acqui_MeasurType_SelectedIndexChanged(object sender, EventArgs e)
        {

            string strCH1type = String.Empty;
            string strCH2type = String.Empty;
            string strCH3type = String.Empty;
            string strCH4type = String.Empty;

            if (cbox_Acqui_MeasurTypeCH1.Items.Count > 0) { strCH1type = cbox_Acqui_MeasurTypeCH1.Text; }
            if (cbox_Acqui_MeasurTypeCH2.Items.Count > 0) { strCH2type = cbox_Acqui_MeasurTypeCH2.Text; }
            if (cbox_Acqui_MeasurTypeCH3.Items.Count > 0) { strCH3type = cbox_Acqui_MeasurTypeCH3.Text; }
            if (cbox_Acqui_MeasurTypeCH4.Items.Count > 0) { strCH4type = cbox_Acqui_MeasurTypeCH4.Text; }

            //CH1
            if (strCH1type != "")
            {

                //analog mode
                if (strCH1type.ToLower().Contains("analog"))
                {
                    if (nUpDo_Acqui_MeasurTimeTickCH1.Value < 10)
                    {
                        nUpDo_Acqui_MeasurTimeTickCH1.Value = (decimal)10;
                    }
                    nUpDo_Acqui_MeasurTimeTickCH1.Minimum = (decimal)10; // 10us }
                }

                else//digital mode
                {
                    nUpDo_Acqui_MeasurTimeTickCH1.Minimum = (decimal)0.01; // 10ns
                }

            }
            //CH2
            if (strCH2type != "")
            {
                //analog mode
                if (strCH2type.ToLower().Contains("analog"))
                {
                    if (nUpDo_Acqui_MeasurTimeTickCH2.Value < 10)
                    {
                        nUpDo_Acqui_MeasurTimeTickCH2.Value = (decimal)10;
                    }
                    nUpDo_Acqui_MeasurTimeTickCH2.Minimum = (decimal)10; // 10us }
                }

                else//digital mode
                {
                    nUpDo_Acqui_MeasurTimeTickCH2.Minimum = (decimal)0.01; // 10ns
                }
            }
            //CH3
            if (strCH3type != "")
            {
                //analog mode
                if (strCH3type.ToLower().Contains("analog"))
                {
                    if (nUpDo_Acqui_MeasurTimeTickCH3.Value < 10)
                    {
                        nUpDo_Acqui_MeasurTimeTickCH3.Value = (decimal)10;
                    }
                    nUpDo_Acqui_MeasurTimeTickCH3.Minimum = (decimal)10; // 10us }
                }

                else//digital mode
                {
                    nUpDo_Acqui_MeasurTimeTickCH3.Minimum = (decimal)0.01; // 10ns
                }
            }

            //CH4
            if (strCH4type != "")
            {
                //analog mode
                if (strCH4type.ToLower().Contains("analog"))
                {
                    if (nUpDo_Acqui_MeasurTimeTickCH4.Value < 10)
                    {
                        nUpDo_Acqui_MeasurTimeTickCH4.Value = (decimal)10;
                    }
                    nUpDo_Acqui_MeasurTimeTickCH4.Minimum = (decimal)10; // 10us }
                }

                else//digital mode
                {
                    nUpDo_Acqui_MeasurTimeTickCH4.Minimum = (decimal)0.01; // 10ns
                }
            }

        }

        //Get encoder ppr
        public void but_GetParamEncoder_Click(object sender, EventArgs e)
        {
            try
            {
                UInt32 uiPPR = Convert.ToUInt32(Board.Get_ppr());
                GlobalVariables.EncocderPPR = uiPPR + 1;

                UInt32 uiSpeed = Convert.ToUInt32(Board.EncoderSpeedCount(1, false));

                // double dSpeed = 16 * 60 * 100E6 /((double)uiSpeed * 64 * (double)(GlobalVariables.EncocderPPR)); //convert number of pulses cout to speed in rpm : (counts*16)/(4xPPR) *60 for minutes
                double dSpeed = (double)uiSpeed * 16.0 / (double)(4 * GlobalVariables.EncocderPPR) * 60; //convert number of pulses cout to speed in rpm : (counts*16)/(4xPPR) *60 for minutes

                nUpDo_Encoderppr.Value = uiPPR + 1;
                txt_MotorSpeed.Text = (Math.Round(dSpeed,2)).ToString();
            }
            catch (Exception exError)
            {
                MessageBox.Show("Problems with the encoder. Unable to read the encoder ppr", "Error Encoder");
            }
        }

        //Set encoder ppr
        public void But_Set_Encoderppr_Click(object sender, EventArgs e)
        {
            try
            {
                Board.Set_ppr((UInt32)nUpDo_Encoderppr.Value);
                GlobalVariables.EncocderPPR = (UInt32)nUpDo_Encoderppr.Value;
            }
            catch (Exception exError)
            {
                MessageBox.Show("Problems with the encoder. Unable to set the encoder ppr", "Error Encoder");
            }
        }

        //Save the data of multiple acquisition cycle
        public void SaveAcquisitionDataMultipleCycle(uint uiTotalCycleNumber)
        {
            fct_UpdateChannelsQuantumAndItemNumber();
            uint uiNumberCycle = uiTotalCycleNumber;

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            xlApp = new Excel.Application();
            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            try
            {
                savefileDialogData.CreatePrompt = true;
                savefileDialogData.OverwritePrompt = true;
                savefileDialogData.InitialDirectory = GlobalVariables.FileDirectory;

                int iCurrentExcelLine = 0;
                int iCurrentExcelColumn = 0;
                int iNumberOfChannels = 0;


                //Get direcory using the savefile dialogbox
                if (savefileDialogData.ShowDialog() == DialogResult.OK)
                {

                    bool bsaveInsameFile = false;

                    if (chkbox_Acqui_MeasureCH1.Checked) { iNumberOfChannels++; }
                    if (chkbox_Acqui_MeasureCH2.Checked) { iNumberOfChannels++; }
                    if (chkbox_Acqui_MeasureCH3.Checked) { iNumberOfChannels++; }
                    if (chkbox_Acqui_MeasureCH4.Checked) { iNumberOfChannels++; }

                    //Define number of items to save
                    uint uiItemsToSaveCH1 = 0;
                    uint uiItemsToSaveCH2 = 0;
                    uint uiItemsToSaveCH3 = 0;
                    uint uiItemsToSaveCH4 = 0;

                    uint uiItemsMaxCH1 = 0;
                    uint uiItemsMaxCH2 = 0;
                    uint uiItemsMaxCH3 = 0;
                    uint uiItemsMaxCH4 = 0;

                    if (chkbox_Acqui_MeasureCH1.Checked) { uiItemsToSaveCH1 = (CH1.uiSweepsAcquired) * uiNumberCycle; uiItemsMaxCH1 = CH1.uiSweepsAcquired; }
                    if (chkbox_Acqui_MeasureCH2.Checked) { uiItemsToSaveCH2 = (CH2.uiSweepsAcquired) * uiNumberCycle; uiItemsMaxCH2 = CH2.uiSweepsAcquired; }
                    if (chkbox_Acqui_MeasureCH3.Checked) { uiItemsToSaveCH3 = (CH3.uiSweepsAcquired) * uiNumberCycle; uiItemsMaxCH3 = CH3.uiSweepsAcquired; }
                    if (chkbox_Acqui_MeasureCH4.Checked) { uiItemsToSaveCH4 = (CH4.uiSweepsAcquired) * uiNumberCycle; uiItemsMaxCH4 = CH4.uiSweepsAcquired; }

                    //bSaveOK = true;
                    if (bsaveInsameFile == false)
                    {
                        iCurrentExcelLine = 0;
                        iCurrentExcelColumn = 0;

                        //If directory found, create Excel file and start measurements
                        GlobalVariables.FileDirectory = savefileDialogData.FileName;

                        //Delete the current file if it already exists :
                        if (System.IO.File.Exists(GlobalVariables.FileDirectory))
                        {
                            System.IO.File.Delete(GlobalVariables.FileDirectory);
                        }
                        //delete the 2 sheets automatically created in the excel file
                        xlWorkBook.Sheets[2].Delete();
                        xlWorkBook.Sheets[2].Delete();

                        //Configure data template :
                        if (uiItemsToSaveCH1 > 0)
                        {
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 1] = "Data CH1 #: Cycle Number 0 ";
                            xlWorkSheet.Cells[iCurrentExcelLine + 1, 2] = CH1.strMeasuretype;
                        }
                        if (uiItemsToSaveCH2 > 0)
                        {
                            xlWorkSheet.Cells[iCurrentExcelLine + 4 + uiItemsMaxCH1, 1] = "Data CH2 #: Cycle Number 0";
                            xlWorkSheet.Cells[iCurrentExcelLine + 4 + uiItemsMaxCH1, 2] = CH2.strMeasuretype;
                        }
                        if (uiItemsToSaveCH3 > 0)
                        {
                            xlWorkSheet.Cells[iCurrentExcelLine + 8 + uiItemsMaxCH1 + uiItemsMaxCH2, 1] = "Data CH3 #: Cycle Number 0";
                            xlWorkSheet.Cells[iCurrentExcelLine + 8 + uiItemsMaxCH1 + uiItemsMaxCH2, 2] = CH3.strMeasuretype;
                        }
                        if (uiItemsToSaveCH4 > 0)
                        {
                            xlWorkSheet.Cells[iCurrentExcelLine + 12 + uiItemsMaxCH1 + uiItemsMaxCH2 + uiItemsMaxCH3, 1] = "Data CH4 #: Cycle Number 0";
                            xlWorkSheet.Cells[iCurrentExcelLine + 12 + uiItemsMaxCH1 + uiItemsMaxCH2 + uiItemsMaxCH3, 2] = CH4.strMeasuretype;
                        }

                        xlApp.ActiveWorkbook.ConflictResolution = Excel.XlSaveConflictResolution.xlLocalSessionChanges;
                        //Save the excel workbook AS:
                        xlWorkBook.SaveAs(GlobalVariables.FileDirectory, //filename
                            Excel.XlFileFormat.xlCSV, //fileformat
                            misValue, //Password
                            misValue, //Write Res Password
                            misValue, //ReadOnlyRecommended
                            misValue, //CreateBackup
                            Excel.XlSaveAsAccessMode.xlExclusive,//AccessMode
                            Excel.XlSaveConflictResolution.xlLocalSessionChanges, //Conflict Resolution
                            misValue, //Add To Mru
                            misValue,//TextCodePage
                            misValue, //TextVisuallayout
                            misValue);//Local

                    }
                    /* else //open an existing workbook :
                     {
                         xlWorkBook = xlApp.Workbooks.Open(GlobalVariables.FileDirectory, 0, false, 5, "", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                         xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                         bSaveOK = true;
                         xlWorkSheet.Cells[iCurrentExcelLine + 1, 1] = "ID: ";
                         xlWorkSheet.Cells[iCurrentExcelLine + 1, 2] = "Angle [DEG]: ";
                     }*/

                    //Define Quantum corresponding to the data type acquired :




                    uint uiTotalItems = uiItemsToSaveCH1 + uiItemsToSaveCH2 + uiItemsToSaveCH3 + uiItemsToSaveCH4;
                    uint iReadingSize = 200;//read 200 items per reading cycle
                    UInt32 iVal = 0;

                    this.Invoke((MethodInvoker)delegate
                    {
                        tool_lblProgBar.Visible = true;
                        tool_ProgBar.Visible = true;
                        tool_ProgBar.Value = 0;
                        tool_ProgBar.Maximum = (int)uiTotalItems + 1;
                    });


                    //Get CH1
                    if (uiItemsToSaveCH1 > 0)
                    {
                        uint uiCurrentCycle = 0;
                        for (int iR = 0; iR < uiItemsToSaveCH1; iR++)
                        {
                            iVal = 0;
                            uint iStartItem = (uint)iR * iReadingSize;
                            string[] strRead = Board.ReadMemory(0, iStartItem, iReadingSize).Split(' ');//Read CH1 memory


                            int iMax = strRead.Length;
                            double[] dValueTable = new double[iMax];

                            if (uiItemsToSaveCH1 < iMax)
                            {

                                iMax = (int)uiItemsToSaveCH1;

                            }
                            for (uint i = 0; i < iMax; i++)
                            {
                                iVal = Convert.ToUInt32(strRead[i], 16);//convert hex number read to int

                                if (iVal > Math.Pow(2, 31) - 1)
                                {
                                    iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                                    dValueTable[i] = -(double)iVal * CH1.dQuantum;//convert to corresponding data type
                                }
                                else
                                {
                                    dValueTable[i] = (double)iVal * CH1.dQuantum;//convert to corresponding data type 
                                }

                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                xlWorkSheet.Cells[iCurrentExcelLine + 2, 1 + iCurrentExcelColumn] = ((uint)iR * iReadingSize) + i;
                                xlWorkSheet.Cells[iCurrentExcelLine + 2, 2 + iCurrentExcelColumn] = dValueTable[i].ToString();

                                if (((iCurrentExcelLine + 2) == (uiItemsMaxCH1 + 1)) && (uiNumberCycle == (uiCurrentCycle + 1)))
                                {
                                    iCurrentExcelLine = (int)uiItemsMaxCH1;
                                    iCurrentExcelColumn = 0;
                                    uiCurrentCycle = 0;
                                }
                                else if ((iCurrentExcelLine + 2) == (uiItemsMaxCH1 + 1))
                                {
                                    iCurrentExcelLine = 0;
                                    iCurrentExcelColumn += 2;
                                    uiCurrentCycle++;
                                    xlWorkSheet.Cells[iCurrentExcelLine + 1, 1 + iCurrentExcelColumn] = "Cycle Number " + Convert.ToString(uiCurrentCycle);
                                }
                                else
                                {
                                    iCurrentExcelLine++;
                                }


                            }
                            if (uiItemsToSaveCH1 > iReadingSize) { uiItemsToSaveCH1 = uiItemsToSaveCH1 - iReadingSize; }
                            else { iR = (int)uiItemsToSaveCH1 - 1; }

                            this.Invoke((MethodInvoker)delegate
                            {
                                if ((tool_ProgBar.Value + (int)iReadingSize) < tool_ProgBar.Maximum)
                                    tool_ProgBar.Value += (int)iReadingSize;
                                else
                                    tool_ProgBar.Value = tool_ProgBar.Maximum;

                                Refresh();
                            });
                        }
                        xlWorkBook.Save();
                    }
                    //Get CH2
                    if (uiItemsToSaveCH2 > 0)
                    {
                        uint uiCurrentCycle = 0;
                        for (int iR = 0; iR < uiItemsToSaveCH2; iR++)
                        {
                            iVal = 0;
                            uint iStartItem = (uint)iR * iReadingSize;
                            string[] strRead = Board.ReadMemory(1, iStartItem, iReadingSize).Split(' ');//Read CH2 memory

                            int iMax = strRead.Length;
                            double[] dValueTable = new double[iMax];

                            if (uiItemsToSaveCH2 < iMax)
                            {

                                iMax = (int)uiItemsToSaveCH2;

                            }
                            for (uint i = 0; i < iMax; i++)
                            {
                                iVal = Convert.ToUInt32(strRead[i], 16);//convert hex number read to int
                                if (iVal > Math.Pow(2, 31) - 1)
                                {
                                    iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                                    dValueTable[i] = -(double)iVal * CH2.dQuantum;//convert to corresponding data type
                                }
                                else
                                {
                                    dValueTable[i] = (double)iVal * CH2.dQuantum;//convert to corresponding data type 
                                }
                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                //xlWorkSheet.Cells[iCurrentExcelLine + 2, 3] = ((uint)iR * iReadingSize) + i;
                                xlWorkSheet.Cells[iCurrentExcelLine + 5, 2 + iCurrentExcelColumn] = dValueTable[i].ToString();
                                if (((iCurrentExcelLine + 5) == (uiItemsMaxCH1 + uiItemsMaxCH2 + 4)) && (uiNumberCycle == (uiCurrentCycle + 1)))
                                {
                                    iCurrentExcelLine = (int)(uiItemsMaxCH2 + uiItemsMaxCH1 + 4);
                                    iCurrentExcelColumn = 0;
                                    uiCurrentCycle = 0;
                                }
                                else if ((iCurrentExcelLine + 5) == (uiItemsMaxCH1 + uiItemsMaxCH2 + 4))
                                {
                                    iCurrentExcelLine = (int)uiItemsMaxCH1;
                                    iCurrentExcelColumn += 2;
                                    uiCurrentCycle++;
                                    xlWorkSheet.Cells[iCurrentExcelLine + 4, 1 + iCurrentExcelColumn] = "Cycle Number " + Convert.ToString(uiCurrentCycle);
                                }

                                else
                                {
                                    iCurrentExcelLine++;
                                }
                            }
                            if (uiItemsToSaveCH2 > iReadingSize) { uiItemsToSaveCH2 = uiItemsToSaveCH2 - iReadingSize; }
                            else { iR = (int)uiItemsToSaveCH2 - 1; }

                            this.Invoke((MethodInvoker)delegate
                            {
                                if ((tool_ProgBar.Value + (int)iReadingSize) < tool_ProgBar.Maximum)
                                    tool_ProgBar.Value += (int)iReadingSize;
                                else
                                    tool_ProgBar.Value = tool_ProgBar.Maximum;

                                Refresh();
                            });
                        }
                        xlWorkBook.Save();
                    }
                    //Get CH3
                    if (uiItemsToSaveCH3 > 0)
                    {
                        uint uiCurrentCycle = 0;
                        for (int iR = 0; iR < uiItemsToSaveCH3; iR++)
                        {
                            iVal = 0;
                            uint iStartItem = (uint)iR * iReadingSize;
                            string[] strRead = Board.ReadMemory(2, iStartItem, iReadingSize).Split(' ');//Read CH3 memory

                            int iMax = strRead.Length;
                            double[] dValueTable = new double[iMax];

                            if (uiItemsToSaveCH3 < iMax)
                            {

                                iMax = (int)uiItemsToSaveCH3;

                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                iVal = Convert.ToUInt32(strRead[i], 16);//convert hex number read to int
                                if (iVal > Math.Pow(2, 31) - 1)
                                {
                                    iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                                    dValueTable[i] = -(double)iVal * CH3.dQuantum;//convert to corresponding data type
                                }
                                else
                                {
                                    dValueTable[i] = (double)iVal * CH3.dQuantum;//convert to corresponding data type 
                                }
                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                //xlWorkSheet.Cells[iCurrentExcelLine + 2, 5] = ((uint)iR * iReadingSize) + i;
                                xlWorkSheet.Cells[iCurrentExcelLine + 5, 2 + iCurrentExcelColumn] = dValueTable[i].ToString();
                                if (((iCurrentExcelLine + 5) == (uiItemsMaxCH1 + uiItemsMaxCH2 + uiItemsMaxCH3 + 8)) && (uiNumberCycle == (uiCurrentCycle + 1)))
                                {
                                    iCurrentExcelLine = (int)(uiItemsMaxCH3 + uiItemsMaxCH2 + uiItemsMaxCH1 + 8);
                                    iCurrentExcelColumn = 0;
                                    uiCurrentCycle = 0;
                                }
                                else if ((iCurrentExcelLine + 5) == (uiItemsMaxCH1 + uiItemsMaxCH2 + uiItemsMaxCH3 + 8))
                                {
                                    iCurrentExcelLine = (int)(uiItemsMaxCH2 + uiItemsMaxCH1 + 4);
                                    iCurrentExcelColumn += 2;
                                    uiCurrentCycle++;
                                    xlWorkSheet.Cells[iCurrentExcelLine + 4, 1 + iCurrentExcelColumn] = "Cycle Number " + Convert.ToString(uiCurrentCycle);
                                }
                                else
                                {
                                    iCurrentExcelLine++;
                                }
                            }
                            if (uiItemsToSaveCH3 > iReadingSize) { uiItemsToSaveCH3 = uiItemsToSaveCH3 - iReadingSize; }
                            else { iR = (int)uiItemsToSaveCH3 - 1; }

                            this.Invoke((MethodInvoker)delegate
                            {
                                if ((tool_ProgBar.Value + (int)iReadingSize) < tool_ProgBar.Maximum)
                                    tool_ProgBar.Value += (int)iReadingSize;
                                else
                                    tool_ProgBar.Value = tool_ProgBar.Maximum;

                                Refresh();
                            });
                        }
                        xlWorkBook.Save();
                    }
                    //Get CH4
                    if (uiItemsToSaveCH4 > 0)
                    {
                        uint uiCurrentCycle = 0;
                        for (int iR = 0; iR < uiItemsToSaveCH4; iR++)
                        {
                            iVal = 0;
                            uint iStartItem = (uint)iR * iReadingSize;
                            string[] strRead = Board.ReadMemory(3, iStartItem, iReadingSize).Split(' ');//Read CH3 memory

                            int iMax = strRead.Length;
                            double[] dValueTable = new double[iMax];

                            if (uiItemsToSaveCH4 < iMax)
                            {

                                iMax = (int)uiItemsToSaveCH4;

                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                iVal = Convert.ToUInt32(strRead[i], 16);//convert hex number read to int
                                if (iVal > Math.Pow(2, 31) - 1)
                                {
                                    iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                                    dValueTable[i] = -(double)iVal * CH4.dQuantum;//convert to corresponding data type
                                }
                                else
                                {
                                    dValueTable[i] = (double)iVal * CH4.dQuantum;//convert to corresponding data type 
                                }
                            }

                            for (uint i = 0; i < iMax; i++)
                            {
                                //xlWorkSheet.Cells[iCurrentExcelLine + 2, 7] = ((uint)iR * iReadingSize) + i;
                                xlWorkSheet.Cells[iCurrentExcelLine + 5, 2 + iCurrentExcelColumn] = dValueTable[i].ToString();
                                if (((iCurrentExcelLine + 5) == (uiItemsMaxCH1 + uiItemsMaxCH2 + uiItemsMaxCH3 + uiItemsMaxCH4 + 12)) && (uiNumberCycle == (uiCurrentCycle + 1)))
                                {
                                    iCurrentExcelLine = (int)(uiItemsMaxCH4 + uiItemsMaxCH3 + uiItemsMaxCH2 + uiItemsMaxCH1 + 12);
                                    iCurrentExcelColumn = 0;
                                    uiCurrentCycle = 0;
                                }
                                else if ((iCurrentExcelLine + 5) == (uiItemsMaxCH1 + uiItemsMaxCH2 + uiItemsMaxCH3 + uiItemsMaxCH4 + 12))
                                {
                                    iCurrentExcelLine = (int)(uiItemsMaxCH3 + uiItemsMaxCH2 + uiItemsMaxCH1 + 8);
                                    iCurrentExcelColumn += 2;
                                    uiCurrentCycle++;
                                    xlWorkSheet.Cells[iCurrentExcelLine + 4, 1 + iCurrentExcelColumn] = "Cycle Number " + Convert.ToString(uiCurrentCycle);
                                }
                                else
                                {
                                    iCurrentExcelLine++;
                                }
                            }
                            if (uiItemsToSaveCH4 > iReadingSize) { uiItemsToSaveCH4 = uiItemsToSaveCH4 - iReadingSize; }
                            else { iR = (int)uiItemsToSaveCH4 - 1; }

                            this.Invoke((MethodInvoker)delegate
                            {
                                if ((tool_ProgBar.Value + (int)iReadingSize) < tool_ProgBar.Maximum)
                                    tool_ProgBar.Value += (int)iReadingSize;
                                else
                                    tool_ProgBar.Value = tool_ProgBar.Maximum;

                                Refresh();
                            });
                        }
                        xlWorkBook.Save();
                    }
                }
            }
            catch (Exception exErrorlog)
            {
                MessageBox.Show("An error occured in the auto test :\n" + exErrorlog.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Invoke((MethodInvoker)delegate
                {
                    tool_ProgBar.Visible = false;
                    tool_lblProgBar.Visible = false;
                });

                //Close the workbook and release all objects 
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

            }
            MessageBox.Show("File saved", "");
        }

        //Multiple Acquisition. Use this function 
        public void Multiple_Acqui_Measur_Start(uint uiCurrentCycleNbr)
        {
            uint uiCycle = uiCurrentCycleNbr;
            uint uiItemsCH1 = 0;
            uint uiItemsCH2 = 0;
            uint uiItemsCH3 = 0;
            uint uiItemsCH4 = 0;

            uint uiStartAddrHexaCH1 = 0;
            uint uiStartAddrHexaCH2 = 0;
            uint uiStartAddrHexaCH3 = 0;
            uint uiStartAddrHexaCH4 = 0;

            uint uiBitChEnabled = 0;

            double dLSB = 360.0 / (double)(GlobalVariables.EncocderPPR * 64);

            //Stop data colection channel
            Board.StartDataCaptureCycle(0, true);//stop any runnning measurements 
            //Reset the number of sweeps acquired
            CH1.uiSweepsAcquired = 0;
            CH2.uiSweepsAcquired = 0;
            CH3.uiSweepsAcquired = 0;
            CH4.uiSweepsAcquired = 0;

            //Set memory default start adddress 
            Board.DataStartAddressDefault();//Set start address for all channels
            Thread.Sleep(100);

            try
            {
                if (chkbox_Acqui_MeasureCH1.Checked)
                {//Set memory start adddress CH1
                    uiItemsCH1 = (uint)nUpDo_Acqui_SweepsCH1.Value;
                    uiStartAddrHexaCH1 = uiCycle * (uiItemsCH1 * 4);
                    Board.DataStartAddress(0, uiStartAddrHexaCH1, true);//Set start address for all channels
                    Thread.Sleep(100);
                }
                if (chkbox_Acqui_MeasureCH2.Checked)
                { //Set memory start adddress CH2
                    uiItemsCH2 = (uint)nUpDo_Acqui_SweepsCH2.Value;
                    uiStartAddrHexaCH2 = uiCycle * (uiItemsCH2 * 4);
                    Board.DataStartAddress(1, uiStartAddrHexaCH2, true);//Set start address for all channels
                    Thread.Sleep(100);
                }
                if (chkbox_Acqui_MeasureCH3.Checked)
                { //Set memory start adddress Ch3
                    uiItemsCH3 = (uint)nUpDo_Acqui_SweepsCH3.Value;
                    uiStartAddrHexaCH3 = uiCycle * (uiItemsCH3 * 4);
                    Board.DataStartAddress(2, uiStartAddrHexaCH3, true);//Set start address for all channels
                    Thread.Sleep(100);
                }
                if (chkbox_Acqui_MeasureCH4.Checked)
                {   //Set memory start adddress CH4
                    uiItemsCH4 = (uint)nUpDo_Acqui_SweepsCH4.Value;
                    uiStartAddrHexaCH4 = uiCycle * (uiItemsCH4 * 4);
                    Board.DataStartAddress(3, uiStartAddrHexaCH4, true);//Set start address for all channels
                    Thread.Sleep(100);
                }
            }
            catch (Exception exError)
            {
                MessageBox.Show(exError.Message, "Error Multiple Acquisition");
            }

            //Set parameters for CH1
            if (chkbox_Acqui_MeasureCH1.Checked)
            {
                //Set measure type CH1 :
                CH1.uiMeasuretype = Convert.ToUInt32(cbox_Acqui_MeasurTypeCH1.SelectedValue);
                Board.DataSelection(0, CH1.uiMeasuretype, true);
                //Set polarity CH1
                CH1.uiPolarity = 0;
                if (chkbox_Acqui_MeasurPolCH1.Checked) { CH1.uiPolarity = 1; }
                Board.Polarity(0, CH1.uiPolarity, true);
                //Set number of sweeps CH1 :
                CH1.uiSweepsMax = Convert.ToUInt32(nUpDo_Acqui_SweepsCH1.Value);
                Board.DataMaximumCount(0, CH1.uiSweepsMax, true);
                //Set ticks (clock) CH1
                CH1.uiClock = Convert.ToUInt32(cbox_Acqui_MeasurTicksCH1.SelectedValue);
                CH1.strClockType = cbox_Acqui_MeasurTicksCH1.Text;
                Board.ClockSelection(0, CH1.uiClock, true);
                //Set Trigger CH1
                CH1.uiTrigger = Convert.ToUInt32(cbox_Acqui_MeasurTriggerCH1.SelectedIndex);
                Board.TriggerSelection(0, CH1.uiTrigger, true);
                //Set Angle Trigger CH1 (no effect if trigger is not on angle) 
                double dWantedAngleCH1 = (float)nUpDo_Acqui_MeasurAngleCH1.Value;
                CH1.uiTriggerAngle = 0;
                CH1.uiTriggerAngle = Convert.ToUInt32(Math.Round(dWantedAngleCH1 / dLSB, 0));
                Board.TriggerAngleSelection(0, CH1.uiTriggerAngle, true);//Angle that generates trigger pulse LSB is encoder dependent.	Normally 360/(PPR*64)
                //Set timeticks CH1:
                CH1.uiTimeTicks = 0;
                double dTimeticksCH1 = (float)nUpDo_Acqui_MeasurTimeTickCH1.Value;//get time between ticks in us (no effect if clock(ticks) is not on clock divider)
                if (dTimeticksCH1 > 0.01) { CH1.uiTimeTicks = Convert.ToUInt32((float)(dTimeticksCH1 * 100) - 1); }
                Board.ClockDivider(0, CH1.uiTimeTicks, true);
                uiBitChEnabled = uiBitChEnabled + 1;
            }

            //Set parameters for CH2
            if (chkbox_Acqui_MeasureCH2.Checked)
            {
                //Set measure type CH2 :
                CH2.uiMeasuretype = Convert.ToUInt32(cbox_Acqui_MeasurTypeCH2.SelectedValue);
                Board.DataSelection(1, CH2.uiMeasuretype, true);
                //Set polarity CH2
                CH2.uiPolarity = 0;
                if (chkbox_Acqui_MeasurPolCH2.Checked) { CH2.uiPolarity = 1; }
                Board.Polarity(1, CH2.uiPolarity, true);
                //Set number of sweeps CH2 :
                CH2.uiSweepsMax = Convert.ToUInt32(nUpDo_Acqui_SweepsCH2.Value);
                Board.DataMaximumCount(1, CH2.uiSweepsMax, true);
                //Set ticks (clock) CH2
                CH2.uiClock = Convert.ToUInt32(cbox_Acqui_MeasurTicksCH2.SelectedValue);
                CH2.strClockType = cbox_Acqui_MeasurTicksCH2.Text;
                Board.ClockSelection(1, CH2.uiClock, true);
                //Set Trigger CH2
                CH2.uiTrigger = Convert.ToUInt32(cbox_Acqui_MeasurTriggerCH2.SelectedIndex);
                Board.TriggerSelection(1, CH2.uiTrigger, true);
                //Set Angle Trigger CH2 (no effect if trigger is not on angle) 
                double dWantedAngleCH2 = (float)nUpDo_Acqui_MeasurAngleCH2.Value;
                CH2.uiTriggerAngle = 0;
                CH2.uiTriggerAngle = Convert.ToUInt32(Math.Round(dWantedAngleCH2 / dLSB, 0));
                Board.TriggerAngleSelection(1, CH2.uiTriggerAngle, true);//Angle that generates trigger pulse LSB is encoder dependent.	Normally 360/(PPR*64)
                //Set timeticks CH2:
                CH2.uiTimeTicks = 0;
                double dTimeticksCH2 = (float)nUpDo_Acqui_MeasurTimeTickCH2.Value;//get time between ticks in us (no effect if clock(ticks) is not on clock divider)
                if (dTimeticksCH2 > 0.01) { CH2.uiTimeTicks = Convert.ToUInt32((float)(dTimeticksCH2 * 100) - 1); }
                Board.ClockDivider(1, CH2.uiTimeTicks, true);
                uiBitChEnabled = uiBitChEnabled + 2;
            }

            //Set parameters for CH3
            if (chkbox_Acqui_MeasureCH3.Checked)
            {
                //Set measure type CH3 :
                CH3.uiMeasuretype = Convert.ToUInt32(cbox_Acqui_MeasurTypeCH3.SelectedValue);
                Board.DataSelection(2, CH3.uiMeasuretype, true);
                //Set polarity CH3
                CH3.uiPolarity = 0;
                if (chkbox_Acqui_MeasurPolCH3.Checked) { CH3.uiPolarity = 1; }
                Board.Polarity(2, CH3.uiPolarity, true);
                //Set number of sweeps CH3 :
                CH3.uiSweepsMax = Convert.ToUInt32(nUpDo_Acqui_SweepsCH3.Value);
                Board.DataMaximumCount(2, CH3.uiSweepsMax, true);
                //Set ticks (clock) CH3
                CH3.uiClock = Convert.ToUInt32(cbox_Acqui_MeasurTicksCH3.SelectedValue);
                CH3.strClockType = cbox_Acqui_MeasurTicksCH3.Text;
                Board.ClockSelection(2, CH3.uiClock, true);
                //Set Trigger CH3
                CH3.uiTrigger = Convert.ToUInt32(cbox_Acqui_MeasurTriggerCH3.SelectedIndex);
                Board.TriggerSelection(2, CH3.uiTrigger, true);
                //Set Angle Trigger CH3 (no effect if trigger is not on angle) 
                double dWantedAngleCH3 = (float)nUpDo_Acqui_MeasurAngleCH3.Value;
                CH3.uiTriggerAngle = 0;
                CH3.uiTriggerAngle = Convert.ToUInt32(Math.Round(dWantedAngleCH3 / dLSB, 0));
                Board.TriggerAngleSelection(2, CH3.uiTriggerAngle, true);//Angle that generates trigger pulse LSB is encoder dependent.	Normally 360/(PPR*64)
                //Set timeticks CH3:
                CH3.uiTimeTicks = 0;
                double dTimeticksCH3 = (float)nUpDo_Acqui_MeasurTimeTickCH3.Value;//get time between ticks in us (no effect if clock(ticks) is not on clock divider)
                if (dTimeticksCH3 > 0.01) { CH3.uiTimeTicks = Convert.ToUInt32((float)(dTimeticksCH3 * 100) - 1); }
                Board.ClockDivider(2, CH3.uiTimeTicks, true);
                uiBitChEnabled = uiBitChEnabled + 4;
            }

            //Set parameters for CH4
            if (chkbox_Acqui_MeasureCH4.Checked)
            {
                //Set measure type CH4 :
                CH4.uiMeasuretype = Convert.ToUInt32(cbox_Acqui_MeasurTypeCH4.SelectedValue);
                Board.DataSelection(3, CH4.uiMeasuretype, true);
                //Set polarity CH4
                CH4.uiPolarity = 0;
                if (chkbox_Acqui_MeasurPolCH4.Checked) { CH4.uiPolarity = 1; }
                Board.Polarity(3, CH4.uiPolarity, true);
                //Set number of sweeps CH4 :
                CH4.uiSweepsMax = Convert.ToUInt32(nUpDo_Acqui_SweepsCH4.Value);
                Board.DataMaximumCount(3, CH4.uiSweepsMax, true);
                //Set ticks (clock) CH4
                CH4.uiClock = Convert.ToUInt32(cbox_Acqui_MeasurTicksCH4.SelectedValue);
                CH4.strClockType = cbox_Acqui_MeasurTicksCH4.Text;
                Board.ClockSelection(3, CH4.uiClock, true);
                //Set Trigger CH4
                CH4.uiTrigger = Convert.ToUInt32(cbox_Acqui_MeasurTriggerCH4.SelectedIndex);
                Board.TriggerSelection(3, CH4.uiTrigger, true);
                //Set Angle Trigger CH4 (no effect if trigger is not on angle) 
                double dWantedAngleCH4 = (float)nUpDo_Acqui_MeasurAngleCH4.Value;
                CH4.uiTriggerAngle = 0;
                CH4.uiTriggerAngle = Convert.ToUInt32(Math.Round(dWantedAngleCH4 / dLSB, 0));
                Board.TriggerAngleSelection(3, CH4.uiTriggerAngle, true);//Angle that generates trigger pulse LSB is encoder dependent.	Normally 360/(PPR*64)
                //Set timeticks CH3:
                CH4.uiTimeTicks = 0;
                double dTimeticksCH4 = (float)nUpDo_Acqui_MeasurTimeTickCH4.Value;//get time between ticks in us (no effect if clock(ticks) is not on clock divider)
                if (dTimeticksCH4 > 0.01) { CH4.uiTimeTicks = Convert.ToUInt32((float)(dTimeticksCH4 * 100) - 1); }
                Board.ClockDivider(3, CH4.uiTimeTicks, true);
                uiBitChEnabled = uiBitChEnabled + 8;
            }

            //Start data colection channel for enabled channels :
            Board.StartDataCaptureCycle(uiBitChEnabled, true);
        }

        public void StartupAccuracy(uint iNbPowerON, bool bPowerONType, int iAngleStartup, double dAngleStepSize, uint uiChannel)
        {
            //----------Channel Identification------------//
            if ((uiChannel > 0) && (uiChannel < 5))
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        //Ch1
                        if (uiChannel == 1)
                        {
                            if (chkbox_Acqui_MeasureCH1.Checked == false)
                            {
                                chkbox_Acqui_MeasureCH1.Checked = true;
                            }
                        }
                        //Ch2
                        if (uiChannel == 2)
                        {
                            if (chkbox_Acqui_MeasureCH2.Checked == false)
                            {
                                chkbox_Acqui_MeasureCH2.Checked = true;
                            }
                        }
                        //Ch3
                        if (uiChannel == 3)
                        {
                            if (chkbox_Acqui_MeasureCH3.Checked == false)
                            {
                                chkbox_Acqui_MeasureCH3.Checked = true;
                            }
                        }
                        //Ch4
                        if (uiChannel == 4)
                        {
                            if (chkbox_Acqui_MeasureCH4.Checked == false)
                            {
                                chkbox_Acqui_MeasureCH4.Checked = true;
                            }
                        }

                        if (chkbox_Acqui_VccInternal.Checked == false)
                        {
                            chkbox_Acqui_VccInternal_Click(null, null);
                        }
                        if (bPowerON == true)
                        {
                            Board.Set_DUT_Vcc(0);
                        }
                    });

                    uint uiNumberPONOFF = iNbPowerON;
                    bool bPowerOnType = bPowerONType;
                    int iStartupAngle = iAngleStartup;
                    double dStepSizeAngle = dAngleStepSize;

                    uint uiStatus = 0;
                    int iDelay = 0;
                    Random rand = new Random();

                    this.Invoke((MethodInvoker)delegate
                    {
                        fct_UpdateChannelsQuantumAndItemNumber(); //Update CH1 quantum : Very important !
                        uiNbCurrentCycle = 0;
                    });
                    Thread.Sleep(100);

                    for (uint uiNumberOfCycle = 0; uiNumberOfCycle < iNbPowerON; uiNumberOfCycle++)
                    {
                        //User Stopped the test
                        if (bTestStop == true) { break; }

                        //--------TEST SET UP AND START------//
                        this.Invoke((MethodInvoker)delegate
                        {
                            Board.Set_DUT_Vcc(0);
                        });
                        Thread.Sleep(30);
                        this.Invoke((MethodInvoker)delegate
                        {
                            //Set parameters
                            Multiple_Acqui_Measur_Start(uiNumberOfCycle);
                        });

                        //Let the FPGA initialize
                        Thread.Sleep(100);
                        this.Invoke((MethodInvoker)delegate
                        {
                            uiCurrentCycleStatus = Board.DataCaptureCycleStatus(uiChannel - 1);
                            uiStatus = uiCurrentCycleStatus;
                        });
                        Thread.Sleep(50);
                        //--------TEST SET UP AND START------//


                        //------POWER ON------//
                        //PowerON Random
                        if (bPowerONType == false)
                        {
                            iDelay = rand.Next(0, 1000);
                        }
                        Thread.Sleep(iDelay);

                        this.Invoke((MethodInvoker)delegate
                        {
                            Board.Set_DUT_Vcc((double)nUpDo_Acqui_SetVcc.Value);
                        });
                        //------POWER ON------//

                        //-------Wait the end of the measure-----//
                        while (uiStatus < 5)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                //Get the Measure Status >= 5 means Completed or OverFlow
                                uiCurrentCycleStatus = Board.DataCaptureCycleStatus(uiChannel -1);
                                uiStatus = uiCurrentCycleStatus;
                            });
                            //User Stopped the test
                            if (bTestStop == true) { break; }
                            Thread.Sleep(100);
                        }
                        //-------Wait the end of the measure-----//


                        //--------POWER OFF---------//
                        this.Invoke((MethodInvoker)delegate
                        {
                            Board.Set_DUT_Vcc(0);
                            uiNbCurrentCycle++;
                        });
                        //--------POWER OFF---------//

                    }

                }
                catch (Exception ExError)
                {
                    iNbError++;
                    strError += "   Accuracy" + ExError.Message;
                    if (iNbError == 3)
                    {
                        iNbError = 0;
                        throw new Exception(strError);
                    }
                }

                finally
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        //Update the status off the channels
                        fct_Acqui_RefreshStatus();
                    });
                    Thread.Sleep(50);
                    this.Invoke((MethodInvoker)delegate
                    {
                        //--------POWER OFF---------//
                        but_Acqui_Vcc_ONOFF.Text = "Power ON";
                        Board.Set_DUT_Vcc(0);
                        //--------POWER OFF---------//
                    });

                    this.Invoke((MethodInvoker)delegate
                    {
                        fct_Acqui_RefreshDUT();
                        
                    });
                    Thread.Sleep(50);
                    if (iNbError == 3)
                    {
                        strError = String.Empty;
                    }
                }
            }
            else
            {
                MessageBox.Show("Wrong channel specidfied : it must be beetween 1,2,3 or 4 \nThe acquisition couldn't be done", "Wrong channel");
            }
        }

        public void Jitter(bool bPowerONType, int iAngleStartup, uint uiChannel)
        {
            //----------Channel Identification------------//
            if ((uiChannel > 0) && (uiChannel < 5))
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                    //Ch1
                    if (uiChannel == 1)
                        {
                            if (chkbox_Acqui_MeasureCH1.Checked == false)
                            {
                                chkbox_Acqui_MeasureCH1.Checked = true;
                            }
                        }
                    //Ch2
                    if (uiChannel == 2)
                        {
                            if (chkbox_Acqui_MeasureCH2.Checked == false)
                            {
                                chkbox_Acqui_MeasureCH2.Checked = true;
                            }
                        }
                    //Ch3
                    if (uiChannel == 3)
                        {
                            if (chkbox_Acqui_MeasureCH3.Checked == false)
                            {
                                chkbox_Acqui_MeasureCH3.Checked = true;
                            }
                        }
                    //Ch4
                    if (uiChannel == 4)
                        {
                            if (chkbox_Acqui_MeasureCH4.Checked == false)
                            {
                                chkbox_Acqui_MeasureCH4.Checked = true;
                            }
                        }

                        if (chkbox_Acqui_MeasureCH1.Checked == false)
                        {
                            chkbox_Acqui_MeasureCH1.Checked = true;
                        }
                        if (chkbox_Acqui_VccInternal.Checked == false)
                        {
                            chkbox_Acqui_VccInternal_Click(null, null);
                        }

                        fct_Acqui_RefreshDUT();

                    });
                    Thread.Sleep(100);

                    bool bPowerOnType = bPowerONType;
                    int iStartupAngle = iAngleStartup;

                    uint uiStatus = 0;
                    uint uiNumberCycle = 0;

                    this.Invoke((MethodInvoker)delegate
                    {
                        fct_UpdateChannelsQuantumAndItemNumber(); //Update CH1 quantum : Very important !

                    });
                    Thread.Sleep(100);

                    //--------TEST SET UP AND START------//
                    this.Invoke((MethodInvoker)delegate
                    {
                        //Start 
                        Multiple_Acqui_Measur_Start(uiNumberCycle);
                    });

                    //Let the FPGA initialize
                    Thread.Sleep(100);
                    this.Invoke((MethodInvoker)delegate
                    {
                        uiCurrentCycleStatus = Board.DataCaptureCycleStatus(uiChannel - 1);
                        uiStatus = uiCurrentCycleStatus;
                    });
                    Thread.Sleep(50);
                    //--------TEST SET UP AND START------//



                    //-------Wait the end of the measure-----//
                    while (uiStatus < 5)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            //Get the Measure Status >= 5 means Completed or OverFlow
                            uiCurrentCycleStatus = Board.DataCaptureCycleStatus(uiChannel - 1);
                            uiStatus = uiCurrentCycleStatus;
                        });
                        //User Stopped the test
                        if (bTestStop == true) { break; }
                        Thread.Sleep(100);
                    }
                    //-------Wait the end of the measure-----//

                }
                catch (Exception ExError)
                {
                    iNbError++;
                    strError += "   Jitter" + ExError.Message;
                    if (iNbError == 3)
                    {
                        iNbError = 0;
                        throw new Exception(strError);
                    }
                }

                finally
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        //Update the status off the channels
                        fct_Acqui_RefreshStatus();
                    });
                    Thread.Sleep(50);

                    this.Invoke((MethodInvoker)delegate
                    {
                        fct_Acqui_RefreshDUT();
                    });
                    Thread.Sleep(50);
                    if (iNbError == 3)
                    {
                        strError = String.Empty;
                    }
                }
            }
            else
            {
                MessageBox.Show("Wrong channel specidfied : it must be beetween 1,2,3 or 4 \nThe acquisition couldn't be done", "Wrong channel");
            }
        }

        public double [,] DataCollection(uint uiTotalNumberOfCycle, uint uiChannel)
        {
            double[,] dTabAllDataChannel;
            double[,]dTabDataChannelFinal;

            this.Invoke((MethodInvoker)delegate
            {
                fct_UpdateChannelsQuantumAndItemNumber(); //Update channels quantum : Very important !
            });
            Thread.Sleep(100);
            uint uiNumberCycle = uiTotalNumberOfCycle;

            //Define number of items to save
            uint uiItemsToSaveChannel = 0;

            uint uiItemsMaxChannel = 0;

            //----------Channel Identification------------//
            if ((uiChannel > 0) && (uiChannel < 5))
            {
                //Ch1
                if (uiChannel == 1)
                {
                    uiItemsToSaveChannel = (CH1.uiSweepsAcquired) * uiNumberCycle;
                    uiItemsMaxChannel = CH1.uiSweepsAcquired;
                }
                //Ch2
                if (uiChannel == 2)
                {
                    uiItemsToSaveChannel = (CH2.uiSweepsAcquired) * uiNumberCycle;
                    uiItemsMaxChannel = CH2.uiSweepsAcquired;
                }
                //Ch3
                if (uiChannel == 3)
                {
                    uiItemsToSaveChannel = (CH3.uiSweepsAcquired) * uiNumberCycle;
                    uiItemsMaxChannel = CH3.uiSweepsAcquired;
                }
                //Ch4
                if (uiChannel == 4)
                {
                    uiItemsToSaveChannel = (CH4.uiSweepsAcquired) * uiNumberCycle;
                    uiItemsMaxChannel = CH4.uiSweepsAcquired;
                }
            }
            //Channel doesn't exist
            else
            {
                throw new Exception("Warning an error as occured during channel selection, The channel number specified doesn't exist");
            }

            //----------Channel Identification------------//


            dTabAllDataChannel = new double[uiItemsToSaveChannel, 1];
            dTabDataChannelFinal = new double[uiItemsMaxChannel, uiNumberCycle];
            string[] strRead;
            uint iReadingSize = 200;//read 200 items per reading cycle
            UInt32 iVal = 0;


            try
            {
                //----------Data Acquisition------------//

                if (uiItemsToSaveChannel > 0)
                {
                    uint uiItemsAlreadyRead = 0;

                    for (int iR = 0; iR < uiItemsToSaveChannel; iR++)
                    {
                        iVal = 0;
                        uint iStartItem = (uint)iR * iReadingSize;


                        strRead = Board.ReadMemory((uiChannel - 1), iStartItem, iReadingSize).Split(' ');//Read CH1 memory


                        int iMax = strRead.Length;
                        double[] dValueTable = new double[iMax];

                        if (uiItemsToSaveChannel < iMax)
                        {

                            iMax = (int)uiItemsToSaveChannel;

                        }
                        for (uint i = 0; i < iMax; i++)
                        {
                            iVal = Convert.ToUInt32(strRead[i], 16);//convert hex number read to int

                            //Negative Numbers
                            if (iVal > Math.Pow(2, 31) - 1)
                            {
                                iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                                //Ch1
                                if (uiChannel == 1)
                                {
                                    dValueTable[i] = -(double)iVal * CH1.dQuantum;//convert to corresponding data type
                                }
                                //Ch2
                                if (uiChannel == 2)
                                {
                                    dValueTable[i] = -(double)iVal * CH2.dQuantum;//convert to corresponding data type
                                }
                                //Ch3
                                if (uiChannel == 3)
                                {
                                    dValueTable[i] = -(double)iVal * CH3.dQuantum;//convert to corresponding data type
                                }
                                //Ch4
                                if (uiChannel == 4)
                                {
                                    dValueTable[i] = -(double)iVal * CH4.dQuantum;//convert to corresponding data type
                                }
                            }

                            //Positive Numbers
                            else
                            {
                                //Ch1
                                if (uiChannel == 1)
                                {
                                    dValueTable[i] = (double)iVal * CH1.dQuantum;//convert to corresponding data type 
                                }
                                //Ch2
                                if (uiChannel == 2)
                                {
                                    dValueTable[i] = (double)iVal * CH2.dQuantum;//convert to corresponding data type 
                                }
                                //Ch3
                                if (uiChannel == 3)
                                {
                                    dValueTable[i] = (double)iVal * CH3.dQuantum;//convert to corresponding data type 
                                }
                                //Ch4
                                if (uiChannel == 4)
                                {
                                    dValueTable[i] = (double)iVal * CH4.dQuantum;//convert to corresponding data type 
                                }
                            }

                            dTabAllDataChannel[i + uiItemsAlreadyRead, 0] = dValueTable[i];
                        }

                        if (uiItemsToSaveChannel > iReadingSize) { uiItemsToSaveChannel = uiItemsToSaveChannel - iReadingSize; uiItemsAlreadyRead += iReadingSize; }
                        else { iR = (int)uiItemsToSaveChannel - 1; }
                    }

                    for (int i = 0; i < uiNumberCycle; i++)
                    {
                        for (int j = 0; j < uiItemsMaxChannel; j++)
                        {

                            dTabDataChannelFinal[j, i] = dTabAllDataChannel[(j + i * uiItemsMaxChannel), 0];
                        }
                    }
                    //----------Data Acquisition------------//
                }
            }
            catch(Exception ExError)
            {
                iNbError++;
                strError += "   Data Collection" + ExError.Message;
                if (iNbError == 3)
                {
                    iNbError = 0;
                    throw new Exception(strError);
                }
            }
            finally
            {
                if (iNbError == 3)
                {
                    strError = String.Empty;
                }
            }
            return dTabDataChannelFinal;
        }

        public uint SaveDataToExcelFile(Excel.Worksheet xlWorkSheet, int iStartLine, int iStartColumn, string strComment, double [,] dTabAllData, string strTestType, uint uiChannel, bool bFirstsave, string strTestStartDateTime, string strTestEndDateTime, string strMotorSpeed, string strAirGap, string strMisalignment, uint uiEdgePerPeriod)
        {
            uint uiExcelLineStopped = 1;

            int iRow = dTabAllData.GetLength(0);
            int iColumn = dTabAllData.GetLength(1);
            double[,] dTabDataToWrite = new double[iRow,iColumn];
            dTabDataToWrite = (double[,])dTabAllData.Clone();

            int iCurrentExcelLine = 0;
            int iCurrentExcelColumn = 0;
            string strTestMeasureType = String.Empty;
            string strTestSignal = String.Empty;
            string strSpeed = String.Empty;
            string strError = String.Empty;
            uint uiNbTotalSweepsJitter = 0;
            uint uiTargetTeethJitter = 0;

            this.Invoke((MethodInvoker)delegate
            {
                but_GetParamEncoder_Click(null, null);
                strSpeed = txt_MotorSpeed.Text;

            });

                uiTargetTeethJitter = uiTargetNbTeeth* uiEdgePerPeriod;

                if (uiChannel == 1)
                {
                    strTestMeasureType = CH1.strMeasuretype;
                    strTestSignal = CH1.strClockType;
                    uiNbTotalSweepsJitter = CH1.uiSweepsAcquired;
                }
                else if (uiChannel == 2)
                {
                    strTestMeasureType = CH2.strMeasuretype;
                    strTestSignal = CH2.strClockType;
                    uiNbTotalSweepsJitter = CH2.uiSweepsAcquired;
                }
                else if (uiChannel == 3)
                {
                    strTestMeasureType = CH3.strMeasuretype;
                    strTestSignal = CH3.strClockType;
                    uiNbTotalSweepsJitter = CH3.uiSweepsAcquired;
                }
                else if (uiChannel == 4)
                {
                    strTestMeasureType = CH4.strMeasuretype;
                    strTestSignal = CH4.strClockType;
                    uiNbTotalSweepsJitter = CH4.uiSweepsAcquired;
                }
                
                else
                {
                    strError = "Wrong Channel specified";
                }

            try
            {
                //----------------------------------------------------------First Save Time-----------------------------------------------------//
                if (bFirstsave == true)
                {

                    iCurrentExcelLine = 1;
                    iCurrentExcelColumn = 1;

                        xlWorkSheet.Cells[iCurrentExcelLine,iCurrentExcelColumn] = "Test started at : " + strTestStartDateTime; iCurrentExcelLine++;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Test type: " + strTestType; iCurrentExcelLine++;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Channel : " + uiChannel.ToString() ; iCurrentExcelLine++;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Measure type : " + strTestMeasureType; iCurrentExcelLine++;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Signal acquired : " + strTestSignal; iCurrentExcelLine++;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Comment : " + strComment; iCurrentExcelLine++;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Misalignment : " + strMisalignment; iCurrentExcelLine++;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Air Gap : " + strAirGap; iCurrentExcelLine++;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Motor Speed : " + strMotorSpeed + "    Speed FPGA : " + strSpeed; iCurrentExcelLine++;

                        //------------------------------------------Test Accuracy-------------------------------//

                        if (strTestType == "Accuracy")
                        {
                            uiExcelLineStopped = (uint)iCurrentExcelLine;
                            xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Cycle v \\ Edge >"; iCurrentExcelLine++;

                            for (uint i = 0; i < iColumn; i++) {

                                for (uint j = 0; j < iRow; j++)
                                {
                                xlWorkSheet.Cells[uiExcelLineStopped, iCurrentExcelColumn + 1] = j.ToString();
                                xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn + 1] = dTabDataToWrite[j,i].ToString();

                                    if (((iCurrentExcelColumn + 1) == (iRow + 1)) && (iColumn == (i + 1)))
                                    {
                                        iCurrentExcelLine = (int)uiExcelLineStopped + iColumn;
                                        iCurrentExcelColumn = 1;
                                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = Convert.ToString(i + 1); iCurrentExcelLine++;
                                    }
                                    else if ((iCurrentExcelColumn + 1) == (iRow + 1))
                                    {
                                        iCurrentExcelLine = (int)(uiExcelLineStopped + 1 + i);
                                        iCurrentExcelColumn = 1;
                                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = Convert.ToString(i + 1); iCurrentExcelLine++;
                                    }
                                    else
                                    {
                                        iCurrentExcelColumn++;
                                    }
                                    
                                }
                            }

                            uiExcelLineStopped += (uint)iColumn;
                        }
                        //------------------------------------------Test Accuracy-------------------------------//


                        //------------------------------------------Test Jitter-------------------------------//
                        else if (strTestType == "Jitter")
                        {
                            
                            double[,] dTabDataToWriteJitter;

                            //Find the number of revolution
                            uint uiNbRevolution = uiNbTotalSweepsJitter / uiTargetTeethJitter;
                            uint uiNbsweepsRemaining = uiNbTotalSweepsJitter % uiTargetTeethJitter;

                            if (uiNbsweepsRemaining > 0)
                            {
                                uiNbRevolution++;
                            }

                            dTabDataToWriteJitter = new double[uiTargetTeethJitter, uiNbRevolution];


                            if (uiNbsweepsRemaining > 0)
                            {
                                for (int i = 0; i < (uiNbRevolution -1) ; i++)
                                {
                                    for (int j = 0; j < uiTargetTeethJitter; j++)
                                    {
                                        dTabDataToWriteJitter[j, i] = dTabDataToWrite[(j + i * uiTargetTeethJitter), 0];
                                    }
                                }
                                for (int j = 0; j < uiNbsweepsRemaining; j++)
                                {
                                    dTabDataToWriteJitter[j, (uiNbRevolution - 1)] = dTabDataToWrite[(j + (uiNbRevolution - 1) * uiTargetTeethJitter), 0];
                                }
                                
                            }

                            else if(uiNbsweepsRemaining == 0)
                            {

                                for (int i = 0; i < uiNbRevolution; i++)
                                {
                                    for (int j = 0; j < uiTargetTeethJitter; j++)
                                    {
                                        dTabDataToWriteJitter[j, i] = dTabDataToWrite[(j + i * uiTargetTeethJitter), 0];
                                    }
                                }
                            }


                        uiExcelLineStopped = (uint)iCurrentExcelLine;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Revolution v \\ Edge >"; iCurrentExcelLine++;

                        for (uint i = 0; i < uiNbRevolution; i++)
                        {

                            for (uint j = 0; j < uiTargetTeethJitter; j++)
                            {
                                xlWorkSheet.Cells[uiExcelLineStopped, iCurrentExcelColumn + 1] = j.ToString();
                                xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn + 1] = dTabDataToWriteJitter[j, i].ToString();

                                if (((iCurrentExcelColumn + 1) == (uiTargetTeethJitter + 1)) && (uiNbRevolution == (i + 1)))
                                {
                                    iCurrentExcelLine = (int)(uiExcelLineStopped + uiNbRevolution);
                                    iCurrentExcelColumn = 1;
                                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = Convert.ToString(i + 1); iCurrentExcelLine++;
                                }
                                else if ((iCurrentExcelColumn + 1) == (uiTargetTeethJitter + 1))
                                {
                                    iCurrentExcelLine = (int)(uiExcelLineStopped + 1 + i);
                                    iCurrentExcelColumn = 1;
                                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = Convert.ToString(i + 1); iCurrentExcelLine++;
                                }
                                else
                                {
                                    iCurrentExcelColumn++;
                                }

                            }
                        }

                        uiExcelLineStopped += (uint)uiNbRevolution;
                    }
                        //------------------------------------------Test Jitter-------------------------------//


                        //------------------------------------------Wrong Test Type-------------------------------//
                        else
                        {
                            strError = "Wrong Test Type specified";
                            xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = strError;
                        }
                        //------------------------------------------Wrong Test Type-------------------------------//

                        xlWorkSheet.Cells[uiExcelLineStopped + 1, iCurrentExcelColumn] = "Test ended at : " + strTestEndDateTime;
                        xlWorkSheet.Cells[uiExcelLineStopped + 2, iCurrentExcelColumn] = "Error : " + strError;
                        uiExcelLineStopped += 4;
                    
                }

                //----------------------------------------------------------First Save Time-----------------------------------------------------//

                //----------------------------------------------------------Open Existing Save File-----------------------------------------------------//

                else //open an existing workbook :
                {

                    iCurrentExcelLine = iStartLine;
                    iCurrentExcelColumn = iStartColumn;

                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Test started at : " + strTestStartDateTime; iCurrentExcelLine++;
                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Test type: " + strTestType; iCurrentExcelLine++;
                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Channel : " + uiChannel.ToString(); iCurrentExcelLine++;
                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Measure type : " + strTestMeasureType; iCurrentExcelLine++;
                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Signal acquired : " + strTestSignal; iCurrentExcelLine++;
                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Comment : " + strComment; iCurrentExcelLine++;
                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Misalignment : " + strMisalignment; iCurrentExcelLine++;
                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Air Gap : " + strAirGap; iCurrentExcelLine++;
                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Motor Speed : " + strMotorSpeed + "    Speed FPGA : " + strSpeed; iCurrentExcelLine++;

                    //------------------------------------------Test Accuracy-------------------------------//

                    if (strTestType == "Accuracy")
                    {
                        uiExcelLineStopped = (uint)iCurrentExcelLine;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Cycle v \\ Edge >"; iCurrentExcelLine++;

                        for (uint i = 0; i < iColumn; i++)
                        {

                            for (uint j = 0; j < iRow; j++)
                            {
                                xlWorkSheet.Cells[uiExcelLineStopped, iCurrentExcelColumn + 1] = j.ToString();
                                xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn + 1] = dTabDataToWrite[j, i].ToString();

                                if (((iCurrentExcelColumn + 1) == (iRow + 1)) && (iColumn == (i + 1)))
                                {
                                    iCurrentExcelLine = (int)uiExcelLineStopped + iColumn;
                                    iCurrentExcelColumn = iStartColumn;
                                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = Convert.ToString(i + 1); iCurrentExcelLine++;
                                }
                                else if ((iCurrentExcelColumn + 1) == (iRow + 1))
                                {
                                    iCurrentExcelLine = (int)(uiExcelLineStopped + 1 + i);
                                    iCurrentExcelColumn = iStartColumn;
                                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = Convert.ToString(i + 1); iCurrentExcelLine++;
                                }
                                else
                                {
                                    iCurrentExcelColumn++;
                                }

                            }
                        }

                        uiExcelLineStopped += (uint)iColumn;
                    }
                    //------------------------------------------Test Accuracy-------------------------------//


                    //------------------------------------------Test Jitter-------------------------------//
                    else if (strTestType == "Jitter")
                    {

                        double[,] dTabDataToWriteJitter;

                        //Find the number of revolution
                        uint uiNbRevolution = uiNbTotalSweepsJitter / uiTargetTeethJitter;
                        uint uiNbsweepsRemaining = uiNbTotalSweepsJitter % uiTargetTeethJitter;

                        if (uiNbsweepsRemaining > 0)
                        {
                            uiNbRevolution++;
                        }

                        dTabDataToWriteJitter = new double[uiTargetTeethJitter, uiNbRevolution];


                        if (uiNbsweepsRemaining > 0)
                        {
                            for (int i = 0; i < (uiNbRevolution - 1); i++)
                            {
                                for (int j = 0; j < uiTargetTeethJitter; j++)
                                {
                                    dTabDataToWriteJitter[j, i] = dTabDataToWrite[(j + i * uiTargetTeethJitter), 0];
                                }
                            }
                            for (int j = 0; j < uiNbsweepsRemaining; j++)
                            {
                                dTabDataToWriteJitter[j, (uiNbRevolution - 1)] = dTabDataToWrite[(j + (uiNbRevolution - 1) * uiTargetTeethJitter), 0];
                            }

                        }

                        else if (uiNbsweepsRemaining == 0)
                        {

                            for (int i = 0; i < uiNbRevolution; i++)
                            {
                                for (int j = 0; j < uiTargetTeethJitter; j++)
                                {
                                    dTabDataToWriteJitter[j, i] = dTabDataToWrite[(j + i * uiTargetTeethJitter), 0];
                                }
                            }
                        }

                        uiExcelLineStopped = (uint)iCurrentExcelLine;
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = "Revolution v \\ Edge >"; iCurrentExcelLine++;

                        for (uint i = 0; i < uiNbRevolution; i++)
                        {

                            for (uint j = 0; j < uiTargetTeethJitter; j++)
                            {
                                xlWorkSheet.Cells[uiExcelLineStopped, iCurrentExcelColumn + 1] = j.ToString();
                                xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn + 1] = dTabDataToWriteJitter[j, i].ToString();

                                if (((iCurrentExcelColumn + 1) == (uiTargetTeethJitter + 1)) && (uiNbRevolution == (i + 1)))
                                {
                                    iCurrentExcelLine = (int)(uiExcelLineStopped + uiNbRevolution);
                                    iCurrentExcelColumn = iStartColumn;
                                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = Convert.ToString(i + 1); iCurrentExcelLine++;
                                }
                                else if ((iCurrentExcelColumn + 1) == (uiTargetTeethJitter + 1))
                                {
                                    iCurrentExcelLine = (int)(uiExcelLineStopped + 1 + i);
                                    iCurrentExcelColumn = iStartColumn;
                                    xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = Convert.ToString(i + 1); iCurrentExcelLine++;
                                }
                                else
                                {
                                    iCurrentExcelColumn++;
                                }

                            }
                        }

                        uiExcelLineStopped += (uint)uiNbRevolution;
                    }
                    //------------------------------------------Test Jitter-------------------------------//


                    //------------------------------------------Wrong Test Type-------------------------------//
                    else
                    {
                        strError = "Wrong Test Type specified";
                        xlWorkSheet.Cells[iCurrentExcelLine, iCurrentExcelColumn] = strError;
                    }
                    //------------------------------------------Wrong Test Type-------------------------------//

                    xlWorkSheet.Cells[uiExcelLineStopped + 1, iCurrentExcelColumn] = "Test ended at : " + strTestEndDateTime;
                    xlWorkSheet.Cells[uiExcelLineStopped + 2, iCurrentExcelColumn] = "Error : " + strError;
                    uiExcelLineStopped += 4;

                }
                //----------------------------------------------------------Open Existing Save File-----------------------------------------------------//

            }

            catch (Exception exErrorlog)
            {
                iNbError++;
                strError += "   Save Data " + exErrorlog.Message;
                if (iNbError == 3)
                {
                    iNbError = 0;
                    throw new Exception(strError);
                }
            }
            finally
            {
                this.Invoke((MethodInvoker)delegate
                {
                    tool_ProgBar.Visible = false;
                    tool_lblProgBar.Visible = false;
                });
                if (iNbError == 3)
                {
                    strError = String.Empty;
                }
            }

            return uiExcelLineStopped;
        }

        public void DimAcquisitionControl(bool bDim)
        {
            if(bDim == true)
            {
                gbox_Acqui_Pin2.Enabled = false;
                gbox_Acqui_Pin3.Enabled = false;
                gbox_Acqui_Vcc.Enabled = false;
                gBox_Encoder.Enabled = false;
                tableLayoutPanel_Acqui.Enabled = false;
                but_Acqui_Measur_Start.Enabled = false;
                but_Acqui_SaveData.Enabled = false;
                but_Acqui_Measur_Stop.Enabled = true;
                but_Acqui_Read.Enabled = true;
            }
            else
            {
                gbox_Acqui_Pin2.Enabled = true;
                gbox_Acqui_Pin3.Enabled = true;
                gbox_Acqui_Vcc.Enabled = true;
                gBox_Encoder.Enabled = true;
                tableLayoutPanel_Acqui.Enabled = true;
                but_Acqui_Measur_Start.Enabled = true;
                but_Acqui_SaveData.Enabled = true;
                but_Acqui_Measur_Stop.Enabled = true;
                but_Acqui_Read.Enabled = true;
            }
        }

        public void StopTest() {

            bTestStop = true;
            Thread.Sleep(300);

            //Stop data colection channel
            Board.StartDataCaptureCycle(0, true);//stop any runnning measurements 
                                                 //Reset Vcc OFF
            if (bPowerON == true)
            {
                Board.Set_DUT_Vcc(0);
            }

            DimAcquisitionControl(false);
            but_Acqui_Refresh_Click(null, null);
            MessageBox.Show("Test stopped", "User Command");
        }

        public void but_Acqui_Measur_Stop_Click(object sender, EventArgs e)
        {
            StopTest();
        }

        public void PowerON(double dValueVcc)
        {
            Board.Set_DUT_Vcc(dValueVcc);
            bPowerON = true;
            Thread.Sleep(50);
        }
        public void PowerOFF()
        {
            Board.Set_DUT_Vcc(0);
            bPowerON = false;
            Thread.Sleep(50);
        }
        public void RePower(double dValueVcc)
        {
            Board.Set_DUT_Vcc(0);
            bPowerON = false;
            Thread.Sleep(50);
            Board.Set_DUT_Vcc(dValueVcc);
            bPowerON = true;
            Thread.Sleep(1000);
        }
        #endregion Acquistion board functions

        #region GetterSetter

        public NumericUpDown nUpDown_Encoderppr
        {
            get
            {
                return nUpDo_Encoderppr;
            }
            set
            {
                nUpDo_Encoderppr = value;
            }
        }

        public NumericUpDown nUpDown_MeasurTimeTickCH1
        {
            get
            {
                return nUpDo_Acqui_MeasurTimeTickCH1;
            }
            set
            {
                nUpDo_Acqui_MeasurTimeTickCH1 = value;
            }
        }

        public NumericUpDown nUpDown_MeasurTimeTickCH2
        {
            get
            {
                return nUpDo_Acqui_MeasurTimeTickCH2;
            }
            set
            {
                nUpDo_Acqui_MeasurTimeTickCH2 = value;
            }
        }

        public NumericUpDown nUpDown_MeasurTimeTickCH3
        {
            get
            {
                return nUpDo_Acqui_MeasurTimeTickCH3;
            }
            set
            {
                nUpDo_Acqui_MeasurTimeTickCH3 = value;
            }
        }

        public NumericUpDown nUpDown_MeasurTimeTickCH4
        {
            get
            {
                return nUpDo_Acqui_MeasurTimeTickCH4;
            }
            set
            {
                nUpDo_Acqui_MeasurTimeTickCH4 = value;
            }
        }

        public NumericUpDown nUpDo_MeasurAngleCH1
        {
            get
            {
                return nUpDo_Acqui_MeasurAngleCH1;
            }
            set
            {
                nUpDo_Acqui_MeasurAngleCH1 = value;
            }
        }

        public NumericUpDown nUpDo_MeasurAngleCH2
        {
            get
            {
                return nUpDo_Acqui_MeasurAngleCH2;
            }
            set
            {
                nUpDo_Acqui_MeasurAngleCH2 = value;
            }
        }

        public NumericUpDown nUpDo_MeasurAngleCH3
        {
            get
            {
                return nUpDo_Acqui_MeasurAngleCH3;
            }
            set
            {
                nUpDo_Acqui_MeasurAngleCH3 = value;
            }
        }

        public NumericUpDown nUpDo_MeasurAngleCH4
        {
            get
            {
                return nUpDo_Acqui_MeasurAngleCH4;
            }
            set
            {
                nUpDo_Acqui_MeasurAngleCH4 = value;
            }
        }

        public ComboBox cbox_MeasurTriggerCH1
        {
            get
            {
                return cbox_Acqui_MeasurTriggerCH1;
            }
            set
            {
                cbox_Acqui_MeasurTriggerCH1 = value;
            }
        }

        public ComboBox cbox_MeasurTriggerCH2
        {
            get
            {
                return cbox_Acqui_MeasurTriggerCH2;
            }
            set
            {
                cbox_Acqui_MeasurTriggerCH2 = value;
            }
        }

        public ComboBox cbox_MeasurTriggerCH3
        {
            get
            {
                return cbox_Acqui_MeasurTriggerCH3;
            }
            set
            {
                cbox_Acqui_MeasurTriggerCH3 = value;
            }
        }

        public ComboBox cbox_MeasurTriggerCH4
        {
            get
            {
                return cbox_Acqui_MeasurTriggerCH4;
            }
            set
            {
                cbox_Acqui_MeasurTriggerCH4 = value;
            }
        }

        public ComboBox cbox_MeasurTicksCH1
        {
            get
            {
                return cbox_Acqui_MeasurTicksCH1;
            }
            set
            {
                cbox_Acqui_MeasurTicksCH1 = value;
            }
        }

        public ComboBox cbox_MeasurTicksCH2
        {
            get
            {
                return cbox_Acqui_MeasurTicksCH2;
            }
            set
            {
                cbox_Acqui_MeasurTicksCH2 = value;
            }
        }

        public ComboBox cbox_MeasurTicksCH3
        {
            get
            {
                return cbox_Acqui_MeasurTicksCH3;
            }
            set
            {
                cbox_Acqui_MeasurTicksCH3 = value;
            }
        }

        public ComboBox cbox_MeasurTicksCH4
        {
            get
            {
                return cbox_Acqui_MeasurTicksCH4;
            }
            set
            {
                cbox_Acqui_MeasurTicksCH4 = value;
            }
        }

        public NumericUpDown nUpDo_SweepsCH1
        {
            get
            {
                return nUpDo_Acqui_SweepsCH1;
            }
            set
            {
                nUpDo_Acqui_SweepsCH1 = value;
            }
        }

        public NumericUpDown nUpDo_SweepsCH2
        {
            get
            {
                return nUpDo_Acqui_SweepsCH2;
            }
            set
            {
                nUpDo_Acqui_SweepsCH2 = value;
            }
        }

        public NumericUpDown nUpDo_SweepsCH3
        {
            get
            {
                return nUpDo_Acqui_SweepsCH3;
            }
            set
            {
                nUpDo_Acqui_SweepsCH3 = value;
            }
        }

        public NumericUpDown nUpDo_SweepsCH4
        {
            get
            {
                return nUpDo_Acqui_SweepsCH4;
            }
            set
            {
                nUpDo_Acqui_SweepsCH4 = value;
            }
        }

        public CheckBox chkbox_MeasureCH1
        {
            get
            {
                return chkbox_Acqui_MeasureCH1;
            }
            set
            {
                chkbox_Acqui_MeasureCH1 = value;
            }
        }

        public CheckBox chkbox_MeasureCH2
        {
            get
            {
                return chkbox_Acqui_MeasureCH2;
            }
            set
            {
                chkbox_Acqui_MeasureCH2 = value;
            }
        }

        public CheckBox chkbox_MeasureCH3
        {
            get
            {
                return chkbox_Acqui_MeasureCH3;
            }
            set
            {
                chkbox_Acqui_MeasureCH3 = value;
            }
        }

        public CheckBox chkbox_MeasureCH4
        {
            get
            {
                return chkbox_Acqui_MeasureCH4;
            }
            set
            {
                chkbox_Acqui_MeasureCH4 = value;
            }
        }

        public ComboBox cbox_MeasurTypeCH1
        {
            get
            {
                return cbox_Acqui_MeasurTypeCH1;
            }
            set
            {
                cbox_Acqui_MeasurTypeCH1 = value;
            }
        }

        public ComboBox cbox_MeasurTypeCH2
        {
            get
            {
                return cbox_Acqui_MeasurTypeCH2;
            }
            set
            {
                cbox_Acqui_MeasurTypeCH2 = value;
            }
        }

        public ComboBox cbox_MeasurTypeCH3
        {
            get
            {
                return cbox_Acqui_MeasurTypeCH3;
            }
            set
            {
                cbox_Acqui_MeasurTypeCH3 = value;
            }
        }

        public ComboBox cbox_MeasurTypeCH4
        {
            get
            {
                return cbox_Acqui_MeasurTypeCH4;
            }
            set
            {
                cbox_Acqui_MeasurTypeCH4 = value;
            }
        }

        public CheckBox chkbox_MeasurPolCH1
        {
            get
            {
                return chkbox_Acqui_MeasurPolCH1;
            }
            set
            {
                chkbox_Acqui_MeasurPolCH1 = value;
            }
        }

        public CheckBox chkbox_MeasurPolCH2
        {
            get
            {
                return chkbox_Acqui_MeasurPolCH2;
            }
            set
            {
                chkbox_Acqui_MeasurPolCH2 = value;
            }
        }

        public CheckBox chkbox_MeasurPolCH3
        {
            get
            {
                return chkbox_Acqui_MeasurPolCH3;
            }
            set
            {
                chkbox_Acqui_MeasurPolCH3 = value;
            }
        }

        public CheckBox chkbox_MeasurPolCH4
        {
            get
            {
                return chkbox_Acqui_MeasurPolCH4;
            }
            set
            {
                chkbox_Acqui_MeasurPolCH4 = value;
            }
        }

        public TextBox txt_MeasurStatusCH1
        {
            get
            {
                return txt_Acqui_MeasurStatusCH1;
            }
            set
            {
                txt_Acqui_MeasurStatusCH1 = value;
            }
        }

        public TextBox txt_MeasurStatusCH2
        {
            get
            {
                return txt_Acqui_MeasurStatusCH2;
            }
            set
            {
                txt_Acqui_MeasurStatusCH2 = value;
            }
        }

        public TextBox txt_MeasurStatusCH3
        {
            get
            {
                return txt_Acqui_MeasurStatusCH3;
            }
            set
            {
                txt_Acqui_MeasurStatusCH3 = value;
            }
        }

        public TextBox txt_MeasurStatusCH4
        {
            get
            {
                return txt_Acqui_MeasurStatusCH4;
            }
            set
            {
                txt_Acqui_MeasurStatusCH4 = value;
            }
        }

        public NumericUpDown nUpDo_SetIcc_Threshold
        {
            get
            {
                return nUpDo_Acqui_SetIcc_Threshold;
            }
            set
            {
                nUpDo_Acqui_SetIcc_Threshold = value;
            }
        }

        public NumericUpDown nUpDo_SetVcc_Threshold
        {
            get
            {
                return nUpDo_Acqui_SetVcc_Threshold;
            }
            set
            {
                nUpDo_Acqui_SetVcc_Threshold = value;
            }
        }

        public NumericUpDown nUpDo_SetVcc
        {
            get
            {
                return nUpDo_Acqui_SetVcc;
            }
            set
            {
                nUpDo_Acqui_SetVcc = value;
            }
        }

        public CheckBox chkbox_VccInternal
        {
            get
            {
                return chkbox_Acqui_VccInternal;
            }
            set
            {
                chkbox_Acqui_VccInternal = value;
            }
        }

        public CheckBox chkbox_VccExternal
        {
            get
            {
                return chkbox_Acqui_VccExternal;
            }
            set
            {
                chkbox_Acqui_VccExternal = value;
            }
        }

        public CheckBox chkbox_Pin3_Cout4nF
        {
            get
            {
                return chkbox_Acqui_Pin3_Cout4nF;
            }
            set
            {
                chkbox_Acqui_Pin3_Cout4nF = value;
            }
        }

        public CheckBox chkbox_Pin3_Cout1nF
        {
            get
            {
                return chkbox_Acqui_Pin3_Cout1nF;
            }
            set
            {
                chkbox_Acqui_Pin3_Cout1nF = value;
            }
        }

        public CheckBox chkbox_Pin3_PullUp4K
        {
            get
            {
                return chkbox_Acqui_Pin3_PullUp4K;
            }
            set
            {
                chkbox_Acqui_Pin3_PullUp4K = value;
            }
        }

        public CheckBox chkbox_Pin3_PullUp1K
        {
            get
            {
                return chkbox_Acqui_Pin3_PullUp1K;
            }
            set
            {
                chkbox_Acqui_Pin3_PullUp1K = value;
            }
        }

        public NumericUpDown nUpDo_Pin3_Comp
        {
            get
            {
                return nUpDo_Acqui_Pin3_Comp;
            }
            set
            {
                nUpDo_Acqui_Pin3_Comp = value;
            }
        }

        public CheckBox chkbox_Pin3_Digital
        {
            get
            {
                return chkbox_Acqui_Pin3_Digital;
            }
            set
            {
                chkbox_Acqui_Pin3_Digital = value;
            }
        }

        public NumericUpDown nUpDo_Pin3_PullUpVolt
        {
            get
            {
                return nUpDo_Acqui_Pin3_PullUpVolt;
            }
            set
            {
                nUpDo_Acqui_Pin3_PullUpVolt = value;
            }
        }

        public CheckBox chkbox_Pin3_Enable
        {
            get
            {
                return chkbox_Acqui_Pin3_Enable;
            }
            set
            {
                chkbox_Acqui_Pin3_Enable = value;
            }
        }

        public CheckBox chkbox_Pin2_Cout4nF
        {
            get
            {
                return chkbox_Acqui_Pin2_Cout4nF;
            }
            set
            {
                chkbox_Acqui_Pin2_Cout4nF = value;
            }
        }

        public CheckBox chkbox_Pin2_Cout1nF
        {
            get
            {
                return chkbox_Acqui_Pin2_Cout1nF;
            }
            set
            {
                chkbox_Acqui_Pin2_Cout1nF = value;
            }
        }

        public CheckBox chkbox_Pin2_PullUp4K
        {
            get
            {
                return chkbox_Acqui_Pin2_PullUp4K;
            }
            set
            {
                chkbox_Acqui_Pin2_PullUp4K = value;
            }
        }

        public CheckBox chkbox_Pin2_PullUp1K
        {
            get
            {
                return chkbox_Acqui_Pin2_PullUp1K;
            }
            set
            {
                chkbox_Acqui_Pin2_PullUp1K = value;
            }
        }

        public NumericUpDown nUpDo_Pin2_Comp
        {
            get
            {
                return nUpDo_Acqui_Pin2_Comp;
            }
            set
            {
                nUpDo_Acqui_Pin2_Comp = value;
            }
        }

        public CheckBox chkbox_Pin2_Digital
        {
            get
            {
                return chkbox_Acqui_Pin2_Digital;
            }
            set
            {
                chkbox_Acqui_Pin2_Digital = value;
            }
        }

        public NumericUpDown nUpDo_Pin2_PullUpVolt
        {
            get
            {
                return nUpDo_Acqui_Pin2_PullUpVolt;
            }
            set
            {
                nUpDo_Acqui_Pin2_PullUpVolt = value;
            }
        }

        public CheckBox chkbox_Pin2_Enable
        {
            get
            {
                return chkbox_Acqui_Pin2_Enable;
            }
            set
            {
                chkbox_Acqui_Pin2_Enable = value;
            }
        }

        public Button but_Vcc_ONOFF
        {
            get
            {
                return but_Acqui_Vcc_ONOFF;
            }
            set
            {
                but_Acqui_Vcc_ONOFF = value;
            }
        }

        public bool bStatePowerON
        {
            get
            {
                return bPowerON;
            }
            set
            {
                bPowerON = value;
            }
        }

        public bool bStateConnection
        {
            get
            {
                return GlobalVariables.AcquisitionBoardConnected;
            }
            set
            {
                GlobalVariables.AcquisitionBoardConnected = value;
            }
        }

        public uint EncoderPPR
        {
            get
            {
                return GlobalVariables.EncocderPPR;
            }
            set
            {
                GlobalVariables.EncocderPPR = value;
            }
        }

        public bool bThreadControlRunning
        {
            get
            {
                return bThreadControl;
            }
            set
            {
                bThreadControl = value;
            }
        }

        public uint uiControlCurrentCycle
        {
            get
            {
                return uiNbCurrentCycle;
            }
        }

        public uint uiControlCurrentCycleStatus
        {
            get
            {
                return uiCurrentCycleStatus;
            }
        }

        public uint uiNbTargetTeeth
        {
            get
            {
                return uiTargetNbTeeth;
            }
            set
            {
                uiTargetNbTeeth = value;
            }
        }

        public string strFileDirectory
        {
            get
            {
                return GlobalVariables.FileDirectory;
            }
            set
            {
                GlobalVariables.FileDirectory = value;
            }
        }

        public string strControlAutoTestStatus
        {
            get
            {
                return strAutoTestStatus;
            }
            set
            {
                strAutoTestStatus = value;
            }
        }

        public bool bControlTestStop
        {
            get
            {
                return bTestStop;
            }
            set
            {
                bTestStop = value;
            }
        }

        #endregion GetterSetter

    }
}

