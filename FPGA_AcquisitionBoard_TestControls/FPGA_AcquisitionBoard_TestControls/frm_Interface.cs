using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using Excel = Microsoft.Office.Interop.Excel;

namespace FPGA_AcquisitionBoard_TestControls
{
    public partial class frm_Interface : Form
    {
        private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);
        Thread ThreadStartAnalysis;
        SaveFileDialog savefileDialogData = new SaveFileDialog();

        int iTotalAutoTestPositionCycle = 0;
        int iTotalAutoTestCurrentPositionCycle = 0;
        string strAutoTestStatus = String.Empty;
        uint uiEdgePerPeriod = 1;

        #region frm_Interface functions
        public frm_Interface()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            InitializeComponent();
            InitTabFPGA();
        }

        //Release the COM port when the form is clsing
        private void frm_Closing(object sender, FormClosingEventArgs e)
        {
            this.AcquisitionControl1.CloseControl();
        }

        //Initialize the Interface
        private void InitTabFPGA()
        {
            nUpDown_Tests_Analysis_TargetTeeth.Value = AcquisitionControl1.uiNbTargetTeeth;
            //---------------------------------------TabAccuracy------------------------//

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

            //fill Accuracy combobox data type
            cbox_Accuracy_MeasurType.DisplayMember = "Name";
            cbox_Accuracy_MeasurType.ValueMember = "Value";
            cbox_Accuracy_MeasurType.DataSource = dtDataType;
            cbox_Accuracy_MeasurType.SelectedIndex = 0; //AcquisitionControl1.cbox_MeasurTypeCH1.SelectedIndex;


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
            cbox_Accuracy_Ticks.DataSource = dtClock;
            cbox_Accuracy_Ticks.DisplayMember = "Name";
            cbox_Accuracy_Ticks.ValueMember = "Value";
            cbox_Accuracy_Ticks.SelectedIndex = 10;//AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex;


            //---------------------------------------Intitializes Measurements controls

            //---------------------------------------TabJitter------------------------//

            //---------------------------------------Intitializes Measurements controls

            //--------Initialize Combobox data type
            string[] strDataName2 = { "angle (quadrature)", "Pin2: analog", "Pin2: pulsewidth", "Pin2: period","gear timer","angle (single phase-timed)",
                                     "Pin3: analog", "Pin3: pulsewidth", "Pin3: period", "Icc: analog (2-wire)", "Vcc pulsewidth" ,"Vcc period"};

            uint[] uiDataVal2 = { 0, 1, 2, 3, 4, 5, 17, 18, 19, 33, 34, 35 };

            DataTable dtDataType2 = new DataTable();
            DataColumn dcRegValue2 = new DataColumn("Value", typeof(string));
            DataColumn dcRegName2 = new DataColumn("Name", typeof(string));
            dtDataType2.Columns.Add(dcRegValue2);
            dtDataType2.Columns.Add(dcRegName2);
            for (int i = 0; i < strDataName2.Length; i++)
            {
                DataRow drRegNameRow2 = dtDataType2.NewRow();

                drRegNameRow2["Name"] = strDataName2[i];
                drRegNameRow2["Value"] = uiDataVal2[i];

                dtDataType2.Rows.Add(drRegNameRow2);
            }

            //fill Accuracy combobox data type
            cbox_Tests_Jitter_MeasureType.DisplayMember = "Name";
            cbox_Tests_Jitter_MeasureType.ValueMember = "Value";
            cbox_Tests_Jitter_MeasureType.DataSource = dtDataType2;
            cbox_Tests_Jitter_MeasureType.SelectedIndex = 0; //AcquisitionControl1.cbox_MeasurTypeCH1.SelectedIndex;


            //--------Initialize Combobox clock
            string[] strClock2 = { "Clock divider", "Pin2: Rise", "Pin2: Fall", "Pin2: R/F","Encoder Index","Pin3: Rise", "Pin3: Fall", "Pin3: R/F",
                                    "Icc: Rise", "Icc: Fall", "Icc: R/F" ,"Single-Edge decoder","Encoder B: R/F","Encoder A: R/F","Quadrature: R/F"};

            uint[] uiClockVal2 = { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15 };

            DataTable dtClock2 = new DataTable();
            DataColumn dcClockValue2 = new DataColumn("Value", typeof(string));
            DataColumn dcClockName2 = new DataColumn("Name", typeof(string));
            dtClock2.Columns.Add(dcClockValue2);
            dtClock2.Columns.Add(dcClockName2);
            for (int i = 0; i < strClock2.Length; i++)
            {
                DataRow drClockNameRow2 = dtClock2.NewRow();

                drClockNameRow2["Name"] = strClock2[i];
                drClockNameRow2["Value"] = uiClockVal2[i];

                dtClock2.Rows.Add(drClockNameRow2);
            }
            //fill combobox clock (ticks) ch1
            cbox_Tests_Jitter_Ticks.DataSource = dtClock2;
            cbox_Tests_Jitter_Ticks.DisplayMember = "Name";
            cbox_Tests_Jitter_Ticks.ValueMember = "Value";
            cbox_Tests_Jitter_Ticks.SelectedIndex = 10;//AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex;


            //--------Initialize Combobox trigger :
            string[] strJitterStartType = { "Random", "Angle"};
            cbox_Tests_Jitter_StartType.DataSource = strJitterStartType;
            cbox_Tests_Jitter_StartType.SelectedIndex = 1;


            //---------------------------------------Intitializes Measurements controls

            //---------------------------------------TabScope------------------------//

            //---------------------------------------Intitializes Measurements controls
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

        #endregion frm_Interface functions


        #region TabAcquisition functions



        #endregion TabAcquisition functions


        #region TabTests functions

        private void TabPageTests_Enter(object sender, EventArgs e)
        {
            decimal dThresholdValue = 0;

            if (CheckConnection() == true)
            {
                //Accuracy Checked
                if (chkbox_Tests_Type_Accuracy.Checked == true) {
                    //nUpDown Sweeps
                    nUpDown_Accuracy_NbSweeps.Value = AcquisitionControl1.nUpDo_SweepsCH1.Value;
                    //ComboBox MeasureTye
                    cbox_Accuracy_MeasurType.SelectedIndex = AcquisitionControl1.cbox_MeasurTypeCH1.SelectedIndex;

                    //Save threshold
                    if ((AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 8) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 9) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 10))
                    {
                        dThresholdValue = AcquisitionControl1.nUpDo_SetIcc_Threshold.Value;
                    }
                    //Pin 3 selected
                    else if ((AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 5) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 6) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 7))
                    {
                        dThresholdValue = AcquisitionControl1.nUpDo_Pin3_Comp.Value;
                    }
                    //Pin 2 selected
                    else if ((AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 1) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 2) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 3))
                    {
                        dThresholdValue = AcquisitionControl1.nUpDo_Pin2_Comp.Value;
                    }
                    else
                    {
                        dThresholdValue = 1;
                    }

                    //ComboBox Ticks
                    cbox_Accuracy_Ticks.SelectedIndex = AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex;

                    //nUpDown Threshold
                    nUpDown_Accuracy_Threshold.Value = dThresholdValue;

                    //CheckBox POWER ON Type
                    if (AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex == 0)
                    {
                        chkbox_Accuracy_RandomPON_Click(null, null);
                    }
                    else if (AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex == 1)
                    {
                        chkbox_Accuracy_PhasedPON_Click(null, null);
                    }

                    //nUpDown Start Angle
                    nUpDown_Accuracy_StartupAngle.Value = AcquisitionControl1.nUpDo_MeasurAngleCH1.Value;
                }

                //Jitter Checked
                else if (chkbox_Tests_Type_Jitter.Checked == true)
                {
                    //nUpDown Sweeps
                    nUpDown_Tests_Jitter_NbSweepsTotal.Value = AcquisitionControl1.nUpDo_SweepsCH1.Value;
                    //ComboBox MeasureTye
                    cbox_Tests_Jitter_MeasureType.SelectedIndex = AcquisitionControl1.cbox_MeasurTypeCH1.SelectedIndex;

                    //Save threshold
                    if ((AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 8) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 9) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 10))
                    {
                        dThresholdValue = AcquisitionControl1.nUpDo_SetIcc_Threshold.Value;
                    }
                    //Pin 3 selected
                    else if ((AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 5) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 6) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 7))
                    {
                        dThresholdValue = AcquisitionControl1.nUpDo_Pin3_Comp.Value;
                    }
                    //Pin 2 selected
                    else if ((AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 1) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 2) || (AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex == 3))
                    {
                        dThresholdValue = AcquisitionControl1.nUpDo_Pin2_Comp.Value;
                    }
                    else
                    {
                        dThresholdValue = 1;
                    }

                    //ComboBox Ticks
                    cbox_Tests_Jitter_Ticks.SelectedIndex = AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex;

                    //nUpDown Threshold
                    nUpDown_Tests_Jitter_Threshold.Value = dThresholdValue;

                    //Combo Box Start Type
                    if (AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex == 0)
                    {
                        cbox_Tests_Jitter_StartType.SelectedIndex = 0;
                    }
                    else if (AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex == 1)
                    {
                        cbox_Tests_Jitter_StartType.SelectedIndex = 1;
                    }

                    //nUpDown Start Angle
                    nUpDown_Tests_Jitter_StartAngle.Value = AcquisitionControl1.nUpDo_MeasurAngleCH1.Value;
                }

                //Analog Checked
                else if (chkbox_Test_Type_Analog.Checked == true)
                {

                }
            }
        }

        public bool CheckConnection()
        {
            bool bConnected = false;

            if (AcquisitionControl1.bStateConnection == true)
            {
                gBox_Tests_Analysis.Enabled = true;
                TabTestType.Enabled = true;

                if (chkbox_Accuracy_PhasedPON.Checked == true)
                {
                    gBox_Accuracy_Phased_PON.Enabled = true;
                }
                else
                {
                    gBox_Accuracy_Phased_PON.Enabled = false;
                }
                bConnected = true;
            }
            else
            {
                gBox_Tests_Analysis.Enabled = false;
                TabTestType.Enabled = false;

                if (chkbox_Accuracy_PhasedPON.Checked == true)
                {
                    gBox_Accuracy_Phased_PON.Enabled = false;
                }
                else
                {
                    gBox_Accuracy_Phased_PON.Enabled = false;
                }
                bConnected = false;
            }

            return bConnected;
        }

        private void chkbox_Tests_Type_Accuracy_Click(object sender, EventArgs e)
        {
            chkbox_Tests_Type_Accuracy.Checked = true;
            chkbox_Tests_Type_Jitter.Checked = false;
            chkbox_Test_Type_Analog.Checked = false;

            gBox_Accuracy_FPGA_Param.Enabled = true;
            gBox_Accuracy_PowerONStart.Enabled = true;

            if (chkbox_Accuracy_PhasedPON.Checked == true)
            {
                gBox_Accuracy_Phased_PON.Enabled = true;
            }
            else
            {
                gBox_Accuracy_Phased_PON.Enabled = false;
            }

            gBox_Tests_Jitter_FPGAParam.Enabled = false;

            TabPageTests_Enter(null,null);

        }

        private void chkbox_Tests_Type_Jitter_Click(object sender, EventArgs e)
        {
            chkbox_Tests_Type_Accuracy.Checked = false;
            chkbox_Tests_Type_Jitter.Checked = true;
            chkbox_Test_Type_Analog.Checked = false;

            gBox_Tests_Jitter_FPGAParam.Enabled = true;

            gBox_Accuracy_FPGA_Param.Enabled = false;
            gBox_Accuracy_PowerONStart.Enabled = false;
            gBox_Accuracy_Phased_PON.Enabled = false;

            TabPageTests_Enter(null, null);
        }

        private void chkbox_Test_Type_Analog_Click(object sender, EventArgs e)
        {
            chkbox_Tests_Type_Accuracy.Checked = false;
            chkbox_Tests_Type_Jitter.Checked = false;
            chkbox_Test_Type_Analog.Checked = true;

            gBox_Accuracy_FPGA_Param.Enabled = false;
            gBox_Accuracy_PowerONStart.Enabled = false;
            gBox_Accuracy_Phased_PON.Enabled = false;

            gBox_Tests_Jitter_FPGAParam.Enabled = false;

            TabPageTests_Enter(null, null);
        }

        //Click on Start Analysis Button
        private void but_Tests_StartAnalysis_Click(object sender, EventArgs e)
        {
            string strTestType = string.Empty;
            uint uiNbCycle = 1;
            double[] dTabAirGap;
            double[] dTabMisalignment;
            int[] iTabSpeed;
            string strTestComment = String.Empty;

            if (CheckConnection() == true)
            {
                if (chkbox_Tests_Type_Accuracy.Checked == true) {

                    strTestType = chkbox_Tests_Type_Accuracy.Text;
                    uiNbCycle = (uint)nUpDown_Accuracy_NbPowerON.Value;
                }
                else if (chkbox_Tests_Type_Jitter.Checked == true)
                {
                    strTestType = chkbox_Tests_Type_Jitter.Text;
                }
                else if (chkbox_Test_Type_Analog.Checked == true)
                {
                    strTestType = chkbox_Test_Type_Analog.Text;
                }

                strTestComment = tbox_TabTest_TestComment.Text;

                dTabAirGap = new double[] { 1.5 , 2.3 };
                dTabMisalignment = new double[] { -1, 0.5 };
                iTabSpeed = new int[] { 100, 500 };

                if (CheckConnection() == true)
                {
                    StartAutoTest(strTestType,strTestComment, uiNbCycle, dTabAirGap,iTabSpeed,dTabMisalignment);
                    TimerAccuracyStatus.Start();
                }

            }
        }

        //START THE THREAD OF AUTO TEST
        public void StartAutoTest(string strTestType, string strComment, uint uiNbTotalTestCycle, double[] dTabAirGap, int[] iTabMotSpeed, double[] dTabMisalignment)
        {
            if (AcquisitionControl1.bThreadControlRunning == false)
            {
                //Create new thread
                AcquisitionControl1.bThreadControlRunning = true;
                AcquisitionControl1.bControlTestStop = false;
                AcquisitionControl1.Enabled = false;
                TabTestType.Enabled = false;
                gBox_Tests_MotorAndAxes.Enabled = false;
                chkbox_Tests_Type_Accuracy.Enabled = false;
                chkbox_Tests_Type_Jitter.Enabled = false;
                chkbox_Test_Type_Analog.Enabled = false;
                nUpDown_Tests_Analysis_TargetTeeth.Enabled = false;

                ThreadStartAnalysis = new Thread(() => ThreadStartAutoTest(strTestType, strComment, uiNbTotalTestCycle, dTabAirGap, iTabMotSpeed, dTabMisalignment));
                ThreadStartAnalysis.IsBackground = true;
                ThreadStartAnalysis.SetApartmentState(ApartmentState.STA);
                ThreadStartAnalysis.Start();

            }
        }

        //Thread that is doing the AUTO TEST
        public void ThreadStartAutoTest(string strTestType, string strComment, uint NbTotalCycle,double[] dTabAirGap, int[] iTabMotSpeed, double[] dTabMisalignment)
        {

            strAutoTestStatus = "Initializing";
            string strTimeTestStarted = String.Empty;
            string strTimeTestCompleted = String.Empty;
            bool bError = false;
            bool bFirstSave = true;
            uint uiNbEdgePerPeriod = uiEdgePerPeriod;

            bool bPowerONTypeCH1 = false;
            int iAngleStartupCH1 = 0;
            double dAngleStepSizeCH1 = 0;

            int iTotalNbAirGap = dTabAirGap.GetLength(0);
            int iTotalNbMisalignment = dTabAirGap.GetLength(0);
            int iTotalNbSpeed = dTabAirGap.GetLength(0);

            if (iTotalNbAirGap == 0) { iTotalNbAirGap = 1; }
            if (iTotalNbMisalignment == 0) { iTotalNbMisalignment = 1; }
            if (iTotalNbSpeed == 0) { iTotalNbSpeed = 1; }
            iTotalAutoTestPositionCycle = iTotalNbAirGap * iTotalNbMisalignment * iTotalNbSpeed;
            iTotalAutoTestCurrentPositionCycle = 0;

            this.Invoke((MethodInvoker)delegate
            {
                if(AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex == 0){ bPowerONTypeCH1 = false; }
                else if (AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex == 1) { bPowerONTypeCH1 = true; }
                iAngleStartupCH1 = (int)AcquisitionControl1.nUpDo_MeasurAngleCH1.Value;
                dAngleStepSizeCH1 = (double)nUpDown_Accuracy_StepSizeAngle.Value;
                
                AcquisitionControl1.PowerON((double)AcquisitionControl1.nUpDo_SetVcc.Value);
                
            });

            double[,] dTabAutoTestResult;
            uint uiChannel = 1;
            uint uiExcelLineStop = 1;

            //------------------------------CREATE EXCEL FILE-------------------------------//
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            xlApp = new Excel.Application();
            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            savefileDialogData.CreatePrompt = true;
            savefileDialogData.OverwritePrompt = true;
            savefileDialogData.InitialDirectory = AcquisitionControl1.strFileDirectory;

            //Get direcory using the savefile dialogbox
            if (savefileDialogData.ShowDialog() == DialogResult.OK)
            {

                //If directory found, create Excel file and start measurements
                AcquisitionControl1.strFileDirectory = savefileDialogData.FileName;

                //Delete the current file if it already exists :
                if (System.IO.File.Exists(AcquisitionControl1.strFileDirectory))
                {
                    System.IO.File.Delete(AcquisitionControl1.strFileDirectory);
                }
                //delete the 2 sheets automatically created in the excel file
                xlWorkBook.Sheets[2].Delete();
                xlWorkBook.Sheets[2].Delete();


                xlApp.ActiveWorkbook.ConflictResolution = Excel.XlSaveConflictResolution.xlLocalSessionChanges;
                //Save the excel workbook AS:
                xlWorkBook.SaveAs(AcquisitionControl1.strFileDirectory, //filename
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

                Thread.Sleep(100);

                //------------------------------END CREATING EXCEL FILE-------------------------------//

                try
                {
                    //------------------------------AUTO TEST-------------------------------//

                    for (int iNbCurrentAirGapCycle = 0; iNbCurrentAirGapCycle < iTotalNbAirGap; iNbCurrentAirGapCycle++)
                    {
                        if (AcquisitionControl1.bControlTestStop == false)
                        {
                            //SET AIR GAP
                            double dCurrentAirGap = MoveAirGap(dTabAirGap[iNbCurrentAirGapCycle]);

                            for (int iNbCurrentMisalignmentCycle = 0; iNbCurrentMisalignmentCycle < iTotalNbMisalignment; iNbCurrentMisalignmentCycle++)
                            {
                                if (AcquisitionControl1.bControlTestStop == false)
                                {
                                    //SET MISALIGNMENT
                                    double dCurrentMisalignment = MoveMisalignment(dTabMisalignment[iNbCurrentMisalignmentCycle]);

                                    if (strTestType == "Jitter") { 
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            AcquisitionControl1.RePower((double)AcquisitionControl1.nUpDo_SetVcc.Value);
                                        });
                                    }
                                    for (int iNbCurrentSpeedCycle = 0; iNbCurrentSpeedCycle < iTotalNbSpeed; iNbCurrentSpeedCycle++)
                                    {
                                        if (AcquisitionControl1.bControlTestStop == false)
                                        {
                                            //SET MOTOR SPEED
                                            int iCurrentSpeed = SetSpeed(iTabMotSpeed[iNbCurrentSpeedCycle]);

                                            if (AcquisitionControl1.bControlTestStop == false)
                                            {
                                                if (strTestType == "Accuracy")
                                                {
                                                    //ACCURACY TEST
                                                    strAutoTestStatus = "Accuracy Running";
                                                    strTimeTestStarted = DateTime.Now.ToString();
                                                    AcquisitionControl1.StartupAccuracy(NbTotalCycle, bPowerONTypeCH1, iAngleStartupCH1, dAngleStepSizeCH1, uiChannel);
                                                    strTimeTestCompleted = DateTime.Now.ToString();
                                                    strAutoTestStatus = "Accuracy Completed";
                                                }

                                                else if (strTestType == "Jitter")
                                                {
                                                    //JITTER TEST
                                                    strAutoTestStatus = "Jitter Running";
                                                    strTimeTestStarted = DateTime.Now.ToString();
                                                    AcquisitionControl1.Jitter(bPowerONTypeCH1, iAngleStartupCH1, uiChannel);
                                                    strTimeTestCompleted = DateTime.Now.ToString();
                                                    strAutoTestStatus = "Jitter Completed";
                                                }
                                            }
                                            Thread.Sleep(50);
                                            if (AcquisitionControl1.bControlTestStop == false)
                                            {
                                                //COLLECT DATA
                                                strAutoTestStatus = "Start Data Collection";
                                                dTabAutoTestResult = AcquisitionControl1.DataCollection(NbTotalCycle, uiChannel);
                                                strAutoTestStatus = "Data Collection Completed";

                                                if (AcquisitionControl1.bControlTestStop == false)
                                                {
                                                    //SAVE DATA
                                                    strAutoTestStatus = "Start Data Saving";
                                                    uiExcelLineStop = AcquisitionControl1.SaveDataToExcelFile(xlWorkSheet, (int)uiExcelLineStop, 1, strComment, dTabAutoTestResult, strTestType, uiChannel, bFirstSave, strTimeTestStarted, strTimeTestCompleted, iCurrentSpeed.ToString(), dCurrentAirGap.ToString(), dCurrentMisalignment.ToString(), uiNbEdgePerPeriod);
                                                    strAutoTestStatus = "Data Saving Completed";
                                                }
                                            }
                                            bFirstSave = false;
                                            iTotalAutoTestCurrentPositionCycle++;
                                            Thread.Sleep(200);
                                            xlWorkBook.Save();
                                        }                                        
                                    }
                                }
                            }
                        }
                    }
                    strAutoTestStatus = "Test Completed";
                    this.Invoke((MethodInvoker)delegate
                    {
                        AcquisitionControl1.PowerOFF();
                    });
                }
                //------------------------------END AUTO TEST-------------------------------//

                catch (Exception exError)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        but_Tests_StopAnalysis_Click(null, null);
                    });
                    bError = true;
                    strAutoTestStatus = "Error";
                    MessageBox.Show(exError.Message);

                }
                finally
                {
                    //------------------------------CLOSE EXCEL FILE-------------------------------//
                    //Close the workbook and release all objects 
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();
                    releaseObject(xlWorkSheet);
                    releaseObject(xlWorkBook);
                    releaseObject(xlApp);
                    //------------------------------END CLOSING EXCEL FILE-------------------------------//

                    MessageBox.Show("Test finished", "");

                    this.Invoke((MethodInvoker)delegate
                    {
                        AcquisitionControl1.fct_Acqui_RefreshDUT();
                    });
                    Thread.Sleep(50);

                    this.Invoke((MethodInvoker)delegate
                    {
                        AcquisitionControl1.fct_Acqui_RefreshDUT();
                        Thread.Sleep(50);
                        AcquisitionControl1.bThreadControlRunning = false;
                        AcquisitionControl1.Enabled = true;
                        TabTestType.Enabled = true;
                        gBox_Tests_MotorAndAxes.Enabled = true;
                        chkbox_Tests_Type_Accuracy.Enabled = true;
                        chkbox_Tests_Type_Jitter.Enabled = true;
                        chkbox_Test_Type_Analog.Enabled = true;
                        nUpDown_Tests_Analysis_TargetTeeth.Enabled = true;
                        TimerAccuracyStatus.Stop();
                    });
                }
            }
        }

        //Click on Stop Analysis Button
        private void but_Tests_StopAnalysis_Click(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                if (AcquisitionControl1.bThreadControlRunning == true)
                {
                    AcquisitionControl1.StopTest();
                    AcquisitionControl1.bThreadControlRunning = false;
                }
                AcquisitionControl1.Enabled = true;
            }
        }

        //SET TARGET AIR GAP
        public double MoveAirGap(double dAirGap)
        {
            Thread.Sleep(100);
            double dRetAirGap = dAirGap;
            return dRetAirGap;
        }

        //SET TARGET MISALIGNMENT
        public double MoveMisalignment(double dMisalignment)
        {
            Thread.Sleep(100);
            double dRetMisalignment = dMisalignment;

            return dRetMisalignment;
        }

        ////SET TARGET SPEED
        public int SetSpeed(int iSetSpeed)
        {
            Thread.Sleep(100);
            int iRetSetSpeed = iSetSpeed;

            return iRetSetSpeed;
        }

        private void nUpDown_Tests_Analysis_TargetTeeth_ValueChanged(object sender, EventArgs e)
        {
            if (nUpDown_Tests_Analysis_TargetTeeth.Value != 0) {
                if (chkbox_Tests_Type_Jitter.Checked == true)
                {
                    nUpDown_Tests_Jitter_NbSweepsTotal.Value = Math.Round(nUpDown_Tests_Jitter_NbSweepsPerEdge.Value * nUpDown_Tests_Analysis_TargetTeeth.Value * uiEdgePerPeriod, 0);
                }

                AcquisitionControl1.uiNbTargetTeeth = (uint)nUpDown_Tests_Analysis_TargetTeeth.Value;
            }
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

        #endregion TabTests functions


        #region TabAccuracy functions

        private void nUpDown_Accuracy_NbSweeps_ValueChanged(object sender, EventArgs e)
        {
         if (CheckConnection() == true)
            {
                AcquisitionControl1.nUpDo_SweepsCH1.Value = (uint)nUpDown_Accuracy_NbSweeps.Value;
            }

        }

        private void cbox_Accuracy_MeasurType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                AcquisitionControl1.cbox_MeasurTypeCH1.SelectedIndex = cbox_Accuracy_MeasurType.SelectedIndex;
            }

        }

        private void cbox_Accuracy_Ticks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex = cbox_Accuracy_Ticks.SelectedIndex;

                //Icc selected
                if ((cbox_Accuracy_Ticks.SelectedIndex == 8) || (cbox_Accuracy_Ticks.SelectedIndex == 9) || (cbox_Accuracy_Ticks.SelectedIndex == 10))
                {
                    //Vcc Internal check
                    if ((AcquisitionControl1.chkbox_VccInternal.Checked == false) || ((AcquisitionControl1.chkbox_VccExternal.Checked == true)))
                    {
                        AcquisitionControl1.chkbox_Acqui_VccInternal_Click(sender, e);
                    }
                    lbl_Accuracy_Threshold.Text = "Icc Threshold [mA]";
                    nUpDown_Accuracy_Threshold.Value = 1;
                    nUpDown_Accuracy_Threshold.Maximum = 50;
                    AcquisitionControl1.nUpDo_SetIcc_Threshold.Value = nUpDown_Accuracy_Threshold.Value;
                    AcquisitionControl1.chkbox_Pin3_Enable.Checked = false;
                    AcquisitionControl1.chkbox_Pin3_Enable.Checked = false;
                }

                //Pin3 selected
                else if ((cbox_Accuracy_Ticks.SelectedIndex == 5) || (cbox_Accuracy_Ticks.SelectedIndex == 6) || (cbox_Accuracy_Ticks.SelectedIndex == 7))
                {
                    // Pin3 Enable check
                    if (AcquisitionControl1.chkbox_Pin3_Enable.Checked == false)
                    {
                        AcquisitionControl1.chkbox_Pin3_Enable.Checked = true;
                        AcquisitionControl1.chkbox_Pin2_Enable.Checked = false;
                    }
                    lbl_Accuracy_Threshold.Text = "Pin3 Threshold [V]";
                    nUpDown_Accuracy_Threshold.Value = 2;
                    nUpDown_Accuracy_Threshold.Maximum = 4;
                    AcquisitionControl1.nUpDo_Pin3_Comp.Value = nUpDown_Accuracy_Threshold.Value;

                }
                // Pin2 selected
                else if ((cbox_Accuracy_Ticks.SelectedIndex == 1) || (cbox_Accuracy_Ticks.SelectedIndex == 2) || (cbox_Accuracy_Ticks.SelectedIndex == 3))
                {
                    //Pin 2 Enable check
                    if (AcquisitionControl1.chkbox_Pin2_Enable.Checked == false)
                    {
                        AcquisitionControl1.chkbox_Pin2_Enable.Checked = true;
                        AcquisitionControl1.chkbox_Pin3_Enable.Checked = false;
                    }
                    lbl_Accuracy_Threshold.Text = "Pin2 Threshold [V]";
                    nUpDown_Accuracy_Threshold.Value = 2;
                    nUpDown_Accuracy_Threshold.Maximum = 4;
                    AcquisitionControl1.nUpDo_Pin2_Comp.Value = nUpDown_Accuracy_Threshold.Value;

                }
            }
        }

        private void nUpDown_Accuracy_Threshold_ValueChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                //Icc selected
                if ((cbox_Accuracy_Ticks.SelectedIndex == 8) || (cbox_Accuracy_Ticks.SelectedIndex == 9) || (cbox_Accuracy_Ticks.SelectedIndex == 10))
                {
                    AcquisitionControl1.nUpDo_SetIcc_Threshold.Value = nUpDown_Accuracy_Threshold.Value;
                }
                //Pin 3 selected
                else if ((cbox_Accuracy_Ticks.SelectedIndex == 5) || (cbox_Accuracy_Ticks.SelectedIndex == 6) || (cbox_Accuracy_Ticks.SelectedIndex == 7))
                {
                    AcquisitionControl1.nUpDo_Pin3_Comp.Value = nUpDown_Accuracy_Threshold.Value;
                }
                //Pin 2 selected
                else if ((cbox_Accuracy_Ticks.SelectedIndex == 1) || (cbox_Accuracy_Ticks.SelectedIndex == 2) || (cbox_Accuracy_Ticks.SelectedIndex == 3))
                {
                    AcquisitionControl1.nUpDo_Pin2_Comp.Value = nUpDown_Accuracy_Threshold.Value;
                }
            }
        }

        private void chkbox_Accuracy_RandomPON_Click(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                chkbox_Accuracy_RandomPON.Checked = true;
                chkbox_Accuracy_PhasedPON.Checked = false;
                gBox_Accuracy_Phased_PON.Enabled = false;
                //Trigger Always
                AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex = 0;
            }
        }

        private void chkbox_Accuracy_PhasedPON_Click(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                chkbox_Accuracy_RandomPON.Checked = false;
                chkbox_Accuracy_PhasedPON.Checked = true;
                gBox_Accuracy_Phased_PON.Enabled = true;
                //Trigger Angle
                AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex = 1;
            }
        }

        private void nUpDown_Accuracy_StartupAngle_ValueChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                AcquisitionControl1.nUpDo_MeasurAngleCH1.Value = (uint)nUpDown_Accuracy_StartupAngle.Value;
            }
        }

        #endregion TabAccuracy functions

        //Timer that is updating the measure status and enable or disable the control
        private void TimerAccuracyStatus_Tick(object sender, EventArgs e)
        {
           this.Invoke((MethodInvoker)delegate{
                try
                { 
                   tbox_Tests_AnalysisStatus.Text = strAutoTestStatus + "    Position : " + iTotalAutoTestCurrentPositionCycle.ToString() + "/" + iTotalAutoTestPositionCycle.ToString();
                   if (chkbox_Tests_Type_Accuracy.Checked)
                   {
                       tbox_Tests_AnalysisStatus.Text += "  Cycle : "+AcquisitionControl1.uiControlCurrentCycle.ToString();
                   }             
                }
                catch (Exception exError)
                {
                    TimerAccuracyStatus.Stop();
                    MessageBox.Show("Error", exError.Message);
                }
            });
        }

        #region TabJitter functions


        private void nUpDown_Tests_Jitter_NbSweepsTotal_ValueChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                nUpDown_Tests_Jitter_NbSweepsTotal.Value = (uint)nUpDown_Tests_Jitter_NbSweepsTotal.Value;
                AcquisitionControl1.nUpDo_SweepsCH1.Value = (uint)nUpDown_Tests_Jitter_NbSweepsTotal.Value;
                nUpDown_Tests_Jitter_NbSweepsPerEdge.Value = (uint)nUpDown_Tests_Jitter_NbSweepsTotal.Value / (nUpDown_Tests_Analysis_TargetTeeth.Value* uiEdgePerPeriod);
            }
        }

        private void nUpDown_Tests_Jitter_NbSweepsPerEdge_ValueChanged(object sender, EventArgs e)
        {
            nUpDown_Tests_Jitter_NbSweepsTotal.Value = Math.Round(nUpDown_Tests_Jitter_NbSweepsPerEdge.Value * nUpDown_Tests_Analysis_TargetTeeth.Value * uiEdgePerPeriod,0);
            tbox_Tests_Jitter_AnalysisTime.Text = Convert.ToString(Math.Round(nUpDown_Tests_Jitter_NbSweepsPerEdge.Value,3));
        }

        private void cbox_Tests_Jitter_MeasureType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                AcquisitionControl1.cbox_MeasurTypeCH1.SelectedIndex = cbox_Tests_Jitter_MeasureType.SelectedIndex;
            }
        }

        private void cbox_Tests_Jitter_Ticks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                AcquisitionControl1.cbox_MeasurTicksCH1.SelectedIndex = cbox_Tests_Jitter_Ticks.SelectedIndex;

                //Icc selected
                if ((cbox_Tests_Jitter_Ticks.SelectedIndex == 8) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 9) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 10))
                {
                    //Vcc Internal check
                    if ((AcquisitionControl1.chkbox_VccInternal.Checked == false) || ((AcquisitionControl1.chkbox_VccExternal.Checked == true)))
                    {
                        AcquisitionControl1.chkbox_Acqui_VccInternal_Click(sender, e);
                    }
                    lbl_Tests_Jitter_Threshold.Text = "Icc Threshold [mA]";
                    nUpDown_Tests_Jitter_Threshold.Value = 1;
                    nUpDown_Tests_Jitter_Threshold.Maximum = 50;
                    AcquisitionControl1.nUpDo_SetIcc_Threshold.Value = nUpDown_Tests_Jitter_Threshold.Value;
                    AcquisitionControl1.chkbox_Pin3_Enable.Checked = false;
                    AcquisitionControl1.chkbox_Pin3_Enable.Checked = false;
                }

                //Pin3 selected
                else if ((cbox_Tests_Jitter_Ticks.SelectedIndex == 5) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 6) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 7))
                {
                    // Pin3 Enable check
                    if (AcquisitionControl1.chkbox_Pin3_Enable.Checked == false)
                    {
                        AcquisitionControl1.chkbox_Pin3_Enable.Checked = true;
                        AcquisitionControl1.chkbox_Pin2_Enable.Checked = false;
                    }
                    lbl_Tests_Jitter_Threshold.Text = "Pin3 Threshold [V]";
                    nUpDown_Tests_Jitter_Threshold.Value = 2;
                    nUpDown_Tests_Jitter_Threshold.Maximum = 4;
                    AcquisitionControl1.nUpDo_Pin3_Comp.Value = nUpDown_Tests_Jitter_Threshold.Value;

                }
                // Pin2 selected
                else if ((cbox_Tests_Jitter_Ticks.SelectedIndex == 1) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 2) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 3))
                {
                    //Pin 2 Enable check
                    if (AcquisitionControl1.chkbox_Pin2_Enable.Checked == false)
                    {
                        AcquisitionControl1.chkbox_Pin2_Enable.Checked = true;
                        AcquisitionControl1.chkbox_Pin3_Enable.Checked = false;
                    }
                    lbl_Tests_Jitter_Threshold.Text = "Pin2 Threshold [V]";
                    nUpDown_Tests_Jitter_Threshold.Value = 2;
                    nUpDown_Tests_Jitter_Threshold.Maximum = 4;
                    AcquisitionControl1.nUpDo_Pin2_Comp.Value = nUpDown_Tests_Jitter_Threshold.Value;

                }

                if (cbox_Tests_Jitter_Ticks.Text.Contains("R/F"))
                {
                    uiEdgePerPeriod = (uint)nUpDo_Test_Jitter_EdgePerPeriod.Value * 2;
                }
                else
                {
                    uiEdgePerPeriod = (uint)nUpDo_Test_Jitter_EdgePerPeriod.Value;
                }

                nUpDown_Tests_Jitter_NbSweepsPerEdge.Value = (uint)nUpDown_Tests_Jitter_NbSweepsTotal.Value / (nUpDown_Tests_Analysis_TargetTeeth.Value * uiEdgePerPeriod);
            }
        }

        private void nUpDown_Tests_Jitter_Threshold_ValueChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                //Icc selected
                if ((cbox_Tests_Jitter_Ticks.SelectedIndex == 8) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 9) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 10))
                {
                    AcquisitionControl1.nUpDo_SetIcc_Threshold.Value = nUpDown_Tests_Jitter_Threshold.Value;
                }
                //Pin 3 selected
                else if ((cbox_Tests_Jitter_Ticks.SelectedIndex == 5) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 6) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 7))
                {
                    AcquisitionControl1.nUpDo_Pin3_Comp.Value = nUpDown_Tests_Jitter_Threshold.Value;
                }
                //Pin 2 selected
                else if ((cbox_Tests_Jitter_Ticks.SelectedIndex == 1) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 2) || (cbox_Tests_Jitter_Ticks.SelectedIndex == 3))
                {
                    AcquisitionControl1.nUpDo_Pin2_Comp.Value = nUpDown_Tests_Jitter_Threshold.Value;
                }
            }
        }

        private void cbox_Tests_Jitter_StartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                if (cbox_Tests_Jitter_StartType.SelectedIndex == 0)
                {//Trigger Always
                    AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex = 0;
                    nUpDown_Tests_Jitter_StartAngle.Enabled = false;
                }

                else if (cbox_Tests_Jitter_StartType.SelectedIndex == 1)
                {//Trigger Angle
                    AcquisitionControl1.cbox_MeasurTriggerCH1.SelectedIndex = 1;
                    nUpDown_Tests_Jitter_StartAngle.Enabled = true;
                }
            }
        }

        private void nUpDown_Tests_Jitter_StartAngle_ValueChanged(object sender, EventArgs e)
        {
            if (CheckConnection() == true)
            {
                AcquisitionControl1.nUpDo_MeasurAngleCH1.Value = (uint)nUpDown_Tests_Jitter_StartAngle.Value;
            }
        }

        private void nUpDo_Test_Jitter_EdgePerPeriod_ValueChanged(object sender, EventArgs e)
        {
            if (cbox_Tests_Jitter_Ticks.Text.Contains("R/F"))
            {
                uiEdgePerPeriod = (uint)nUpDo_Test_Jitter_EdgePerPeriod.Value * 2;
            }
            else
            {
                uiEdgePerPeriod = (uint)nUpDo_Test_Jitter_EdgePerPeriod.Value;
            }

            nUpDown_Tests_Jitter_NbSweepsPerEdge.Value = (uint)nUpDown_Tests_Jitter_NbSweepsTotal.Value / (nUpDown_Tests_Analysis_TargetTeeth.Value * uiEdgePerPeriod);
        }

        #endregion TabJitter functions

        #region TabAnalog functions



        #endregion TabAnalog functions

    }
}
