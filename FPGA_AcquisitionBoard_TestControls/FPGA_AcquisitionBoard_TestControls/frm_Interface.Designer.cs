namespace FPGA_AcquisitionBoard_TestControls
{
    partial class frm_Interface
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TimerAccuracyStatus = new System.Windows.Forms.Timer(this.components);
            this.TabPageTests = new System.Windows.Forms.TabPage();
            this.lbl_TabTest_TestComment = new System.Windows.Forms.Label();
            this.tbox_TabTest_TestComment = new System.Windows.Forms.TextBox();
            this.gBox_Tests_MotorAndAxes = new System.Windows.Forms.GroupBox();
            this.TabTestType = new System.Windows.Forms.TabControl();
            this.TabPageAccuracy = new System.Windows.Forms.TabPage();
            this.gBox_Accuracy_Phased_PON = new System.Windows.Forms.GroupBox();
            this.nUpDown_Accuracy_StepSizeAngle = new System.Windows.Forms.NumericUpDown();
            this.lbl_Accuracy_StepSizeAngle = new System.Windows.Forms.Label();
            this.nUpDown_Accuracy_StartupAngle = new System.Windows.Forms.NumericUpDown();
            this.lbl_Accuracy_StartupAngle = new System.Windows.Forms.Label();
            this.gBox_Accuracy_PowerONStart = new System.Windows.Forms.GroupBox();
            this.chkbox_Accuracy_RandomPON = new System.Windows.Forms.CheckBox();
            this.chkbox_Accuracy_PhasedPON = new System.Windows.Forms.CheckBox();
            this.gBox_Accuracy_FPGA_Param = new System.Windows.Forms.GroupBox();
            this.nUpDown_Accuracy_Threshold = new System.Windows.Forms.NumericUpDown();
            this.lbl_Accuracy_Threshold = new System.Windows.Forms.Label();
            this.cbox_Accuracy_MeasurType = new System.Windows.Forms.ComboBox();
            this.lbl_Accuracy_MeasurType = new System.Windows.Forms.Label();
            this.nUpDown_Accuracy_NbPowerON = new System.Windows.Forms.NumericUpDown();
            this.nUpDown_Accuracy_NbSweeps = new System.Windows.Forms.NumericUpDown();
            this.cbox_Accuracy_Ticks = new System.Windows.Forms.ComboBox();
            this.lbl_Accuracy_Ticks = new System.Windows.Forms.Label();
            this.lbl_Accuracy_NbSweeps = new System.Windows.Forms.Label();
            this.lbl_Accuracy_NbPowerON = new System.Windows.Forms.Label();
            this.TabPageJitter = new System.Windows.Forms.TabPage();
            this.gBox_Tests_Jitter_FPGAParam = new System.Windows.Forms.GroupBox();
            this.nUpDo_Test_Jitter_EdgePerPeriod = new System.Windows.Forms.NumericUpDown();
            this.lbl_Test_Jitter_EdgePerPeriod = new System.Windows.Forms.Label();
            this.tbox_Tests_Jitter_AnalysisTime = new System.Windows.Forms.TextBox();
            this.lbl_Tests_Jitter_Or = new System.Windows.Forms.Label();
            this.nUpDown_Tests_Jitter_NbSweepsPerEdge = new System.Windows.Forms.NumericUpDown();
            this.lbl_Tests_Jitter_NbSweepsPerEdge = new System.Windows.Forms.Label();
            this.nUpDown_Tests_Jitter_StartAngle = new System.Windows.Forms.NumericUpDown();
            this.lbl_Tests_Jitter_StartAngle = new System.Windows.Forms.Label();
            this.cbox_Tests_Jitter_StartType = new System.Windows.Forms.ComboBox();
            this.lbl_Tests_Jitter_StartType = new System.Windows.Forms.Label();
            this.nUpDown_Tests_Jitter_Threshold = new System.Windows.Forms.NumericUpDown();
            this.lbl_Tests_Jitter_Threshold = new System.Windows.Forms.Label();
            this.cbox_Tests_Jitter_MeasureType = new System.Windows.Forms.ComboBox();
            this.lbl_Tests_Jitter_MeasureType = new System.Windows.Forms.Label();
            this.nUpDown_Tests_Jitter_NbSweepsTotal = new System.Windows.Forms.NumericUpDown();
            this.cbox_Tests_Jitter_Ticks = new System.Windows.Forms.ComboBox();
            this.lbl_Tests_Jitter_Ticks = new System.Windows.Forms.Label();
            this.lbl_Tests_Jitter_NbSweepsTotal = new System.Windows.Forms.Label();
            this.lbl_Tests_Jitter_AnalysisTime = new System.Windows.Forms.Label();
            this.TabPageAnalog = new System.Windows.Forms.TabPage();
            this.gBox_Tests_Analysis = new System.Windows.Forms.GroupBox();
            this.lbl_Tests_Analysis_TargetTeeth = new System.Windows.Forms.Label();
            this.lbl_Tests_Type = new System.Windows.Forms.Label();
            this.nUpDown_Tests_Analysis_TargetTeeth = new System.Windows.Forms.NumericUpDown();
            this.chkbox_Tests_Type_Accuracy = new System.Windows.Forms.CheckBox();
            this.chkbox_Tests_Type_Jitter = new System.Windows.Forms.CheckBox();
            this.chkbox_Test_Type_Analog = new System.Windows.Forms.CheckBox();
            this.but_Tests_StopAnalysis = new System.Windows.Forms.Button();
            this.but_Tests_StartAnalysis = new System.Windows.Forms.Button();
            this.tbox_Tests_AnalysisStatus = new System.Windows.Forms.TextBox();
            this.TabPageAcquisition = new System.Windows.Forms.TabPage();
            this.AcquisitionControl1 = new FPGA_AcquisitionBoard.AcquisitionControl();
            this.TabFPGA = new System.Windows.Forms.TabControl();
            this.TabPageTests.SuspendLayout();
            this.TabTestType.SuspendLayout();
            this.TabPageAccuracy.SuspendLayout();
            this.gBox_Accuracy_Phased_PON.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_StepSizeAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_StartupAngle)).BeginInit();
            this.gBox_Accuracy_PowerONStart.SuspendLayout();
            this.gBox_Accuracy_FPGA_Param.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_Threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_NbPowerON)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_NbSweeps)).BeginInit();
            this.TabPageJitter.SuspendLayout();
            this.gBox_Tests_Jitter_FPGAParam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDo_Test_Jitter_EdgePerPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Jitter_NbSweepsPerEdge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Jitter_StartAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Jitter_Threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Jitter_NbSweepsTotal)).BeginInit();
            this.gBox_Tests_Analysis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Analysis_TargetTeeth)).BeginInit();
            this.TabPageAcquisition.SuspendLayout();
            this.TabFPGA.SuspendLayout();
            this.SuspendLayout();
            // 
            // TimerAccuracyStatus
            // 
            this.TimerAccuracyStatus.Enabled = true;
            this.TimerAccuracyStatus.Interval = 200;
            this.TimerAccuracyStatus.Tick += new System.EventHandler(this.TimerAccuracyStatus_Tick);
            // 
            // TabPageTests
            // 
            this.TabPageTests.BackColor = System.Drawing.Color.Gainsboro;
            this.TabPageTests.Controls.Add(this.lbl_TabTest_TestComment);
            this.TabPageTests.Controls.Add(this.tbox_TabTest_TestComment);
            this.TabPageTests.Controls.Add(this.gBox_Tests_MotorAndAxes);
            this.TabPageTests.Controls.Add(this.TabTestType);
            this.TabPageTests.Controls.Add(this.gBox_Tests_Analysis);
            this.TabPageTests.Location = new System.Drawing.Point(4, 22);
            this.TabPageTests.Name = "TabPageTests";
            this.TabPageTests.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTests.Size = new System.Drawing.Size(1032, 660);
            this.TabPageTests.TabIndex = 1;
            this.TabPageTests.Text = "Tests";
            this.TabPageTests.Enter += new System.EventHandler(this.TabPageTests_Enter);
            // 
            // lbl_TabTest_TestComment
            // 
            this.lbl_TabTest_TestComment.AutoSize = true;
            this.lbl_TabTest_TestComment.Location = new System.Drawing.Point(17, 413);
            this.lbl_TabTest_TestComment.Name = "lbl_TabTest_TestComment";
            this.lbl_TabTest_TestComment.Size = new System.Drawing.Size(81, 13);
            this.lbl_TabTest_TestComment.TabIndex = 26;
            this.lbl_TabTest_TestComment.Text = "Test Comment :";
            // 
            // tbox_TabTest_TestComment
            // 
            this.tbox_TabTest_TestComment.Location = new System.Drawing.Point(20, 429);
            this.tbox_TabTest_TestComment.Multiline = true;
            this.tbox_TabTest_TestComment.Name = "tbox_TabTest_TestComment";
            this.tbox_TabTest_TestComment.Size = new System.Drawing.Size(500, 50);
            this.tbox_TabTest_TestComment.TabIndex = 25;
            // 
            // gBox_Tests_MotorAndAxes
            // 
            this.gBox_Tests_MotorAndAxes.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.gBox_Tests_MotorAndAxes.Location = new System.Drawing.Point(20, 34);
            this.gBox_Tests_MotorAndAxes.Name = "gBox_Tests_MotorAndAxes";
            this.gBox_Tests_MotorAndAxes.Size = new System.Drawing.Size(500, 367);
            this.gBox_Tests_MotorAndAxes.TabIndex = 24;
            this.gBox_Tests_MotorAndAxes.TabStop = false;
            this.gBox_Tests_MotorAndAxes.Text = "Motor and Axes";
            // 
            // TabTestType
            // 
            this.TabTestType.Controls.Add(this.TabPageAccuracy);
            this.TabTestType.Controls.Add(this.TabPageJitter);
            this.TabTestType.Controls.Add(this.TabPageAnalog);
            this.TabTestType.Enabled = false;
            this.TabTestType.Location = new System.Drawing.Point(539, 34);
            this.TabTestType.Name = "TabTestType";
            this.TabTestType.SelectedIndex = 0;
            this.TabTestType.Size = new System.Drawing.Size(460, 596);
            this.TabTestType.TabIndex = 19;
            // 
            // TabPageAccuracy
            // 
            this.TabPageAccuracy.BackColor = System.Drawing.Color.LightSalmon;
            this.TabPageAccuracy.Controls.Add(this.gBox_Accuracy_Phased_PON);
            this.TabPageAccuracy.Controls.Add(this.gBox_Accuracy_PowerONStart);
            this.TabPageAccuracy.Controls.Add(this.gBox_Accuracy_FPGA_Param);
            this.TabPageAccuracy.Location = new System.Drawing.Point(4, 22);
            this.TabPageAccuracy.Name = "TabPageAccuracy";
            this.TabPageAccuracy.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageAccuracy.Size = new System.Drawing.Size(452, 570);
            this.TabPageAccuracy.TabIndex = 0;
            this.TabPageAccuracy.Text = "Accuracy";
            this.TabPageAccuracy.Enter += new System.EventHandler(this.TabPageTests_Enter);
            // 
            // gBox_Accuracy_Phased_PON
            // 
            this.gBox_Accuracy_Phased_PON.BackColor = System.Drawing.Color.Transparent;
            this.gBox_Accuracy_Phased_PON.Controls.Add(this.nUpDown_Accuracy_StepSizeAngle);
            this.gBox_Accuracy_Phased_PON.Controls.Add(this.lbl_Accuracy_StepSizeAngle);
            this.gBox_Accuracy_Phased_PON.Controls.Add(this.nUpDown_Accuracy_StartupAngle);
            this.gBox_Accuracy_Phased_PON.Controls.Add(this.lbl_Accuracy_StartupAngle);
            this.gBox_Accuracy_Phased_PON.Enabled = false;
            this.gBox_Accuracy_Phased_PON.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.gBox_Accuracy_Phased_PON.Location = new System.Drawing.Point(156, 353);
            this.gBox_Accuracy_Phased_PON.Name = "gBox_Accuracy_Phased_PON";
            this.gBox_Accuracy_Phased_PON.Size = new System.Drawing.Size(290, 154);
            this.gBox_Accuracy_Phased_PON.TabIndex = 12;
            this.gBox_Accuracy_Phased_PON.TabStop = false;
            this.gBox_Accuracy_Phased_PON.Text = "Phased Power ON";
            // 
            // nUpDown_Accuracy_StepSizeAngle
            // 
            this.nUpDown_Accuracy_StepSizeAngle.DecimalPlaces = 1;
            this.nUpDown_Accuracy_StepSizeAngle.Location = new System.Drawing.Point(19, 90);
            this.nUpDown_Accuracy_StepSizeAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nUpDown_Accuracy_StepSizeAngle.Name = "nUpDown_Accuracy_StepSizeAngle";
            this.nUpDown_Accuracy_StepSizeAngle.Size = new System.Drawing.Size(122, 20);
            this.nUpDown_Accuracy_StepSizeAngle.TabIndex = 11;
            // 
            // lbl_Accuracy_StepSizeAngle
            // 
            this.lbl_Accuracy_StepSizeAngle.AutoSize = true;
            this.lbl_Accuracy_StepSizeAngle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Accuracy_StepSizeAngle.Location = new System.Drawing.Point(147, 92);
            this.lbl_Accuracy_StepSizeAngle.Name = "lbl_Accuracy_StepSizeAngle";
            this.lbl_Accuracy_StepSizeAngle.Size = new System.Drawing.Size(129, 13);
            this.lbl_Accuracy_StepSizeAngle.TabIndex = 10;
            this.lbl_Accuracy_StepSizeAngle.Text = "Step Size Angle (degrees)";
            // 
            // nUpDown_Accuracy_StartupAngle
            // 
            this.nUpDown_Accuracy_StartupAngle.DecimalPlaces = 1;
            this.nUpDown_Accuracy_StartupAngle.Location = new System.Drawing.Point(19, 52);
            this.nUpDown_Accuracy_StartupAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nUpDown_Accuracy_StartupAngle.Name = "nUpDown_Accuracy_StartupAngle";
            this.nUpDown_Accuracy_StartupAngle.Size = new System.Drawing.Size(122, 20);
            this.nUpDown_Accuracy_StartupAngle.TabIndex = 4;
            this.nUpDown_Accuracy_StartupAngle.ValueChanged += new System.EventHandler(this.nUpDown_Accuracy_StartupAngle_ValueChanged);
            // 
            // lbl_Accuracy_StartupAngle
            // 
            this.lbl_Accuracy_StartupAngle.AutoSize = true;
            this.lbl_Accuracy_StartupAngle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Accuracy_StartupAngle.Location = new System.Drawing.Point(147, 54);
            this.lbl_Accuracy_StartupAngle.Name = "lbl_Accuracy_StartupAngle";
            this.lbl_Accuracy_StartupAngle.Size = new System.Drawing.Size(118, 13);
            this.lbl_Accuracy_StartupAngle.TabIndex = 9;
            this.lbl_Accuracy_StartupAngle.Text = "Startup Angle (degrees)";
            // 
            // gBox_Accuracy_PowerONStart
            // 
            this.gBox_Accuracy_PowerONStart.BackColor = System.Drawing.Color.Transparent;
            this.gBox_Accuracy_PowerONStart.Controls.Add(this.chkbox_Accuracy_RandomPON);
            this.gBox_Accuracy_PowerONStart.Controls.Add(this.chkbox_Accuracy_PhasedPON);
            this.gBox_Accuracy_PowerONStart.Enabled = false;
            this.gBox_Accuracy_PowerONStart.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.gBox_Accuracy_PowerONStart.Location = new System.Drawing.Point(6, 388);
            this.gBox_Accuracy_PowerONStart.Name = "gBox_Accuracy_PowerONStart";
            this.gBox_Accuracy_PowerONStart.Size = new System.Drawing.Size(134, 87);
            this.gBox_Accuracy_PowerONStart.TabIndex = 17;
            this.gBox_Accuracy_PowerONStart.TabStop = false;
            this.gBox_Accuracy_PowerONStart.Text = "Power ON Start";
            // 
            // chkbox_Accuracy_RandomPON
            // 
            this.chkbox_Accuracy_RandomPON.AutoSize = true;
            this.chkbox_Accuracy_RandomPON.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkbox_Accuracy_RandomPON.Location = new System.Drawing.Point(10, 33);
            this.chkbox_Accuracy_RandomPON.Name = "chkbox_Accuracy_RandomPON";
            this.chkbox_Accuracy_RandomPON.Size = new System.Drawing.Size(118, 17);
            this.chkbox_Accuracy_RandomPON.TabIndex = 14;
            this.chkbox_Accuracy_RandomPON.Text = "Random Power ON";
            this.chkbox_Accuracy_RandomPON.UseVisualStyleBackColor = true;
            this.chkbox_Accuracy_RandomPON.Click += new System.EventHandler(this.chkbox_Accuracy_RandomPON_Click);
            // 
            // chkbox_Accuracy_PhasedPON
            // 
            this.chkbox_Accuracy_PhasedPON.AutoSize = true;
            this.chkbox_Accuracy_PhasedPON.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkbox_Accuracy_PhasedPON.Location = new System.Drawing.Point(10, 56);
            this.chkbox_Accuracy_PhasedPON.Name = "chkbox_Accuracy_PhasedPON";
            this.chkbox_Accuracy_PhasedPON.Size = new System.Drawing.Size(114, 17);
            this.chkbox_Accuracy_PhasedPON.TabIndex = 15;
            this.chkbox_Accuracy_PhasedPON.Text = "Phased Power ON";
            this.chkbox_Accuracy_PhasedPON.UseVisualStyleBackColor = true;
            this.chkbox_Accuracy_PhasedPON.Click += new System.EventHandler(this.chkbox_Accuracy_PhasedPON_Click);
            // 
            // gBox_Accuracy_FPGA_Param
            // 
            this.gBox_Accuracy_FPGA_Param.BackColor = System.Drawing.Color.Transparent;
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.nUpDown_Accuracy_Threshold);
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.lbl_Accuracy_Threshold);
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.cbox_Accuracy_MeasurType);
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.lbl_Accuracy_MeasurType);
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.nUpDown_Accuracy_NbPowerON);
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.nUpDown_Accuracy_NbSweeps);
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.cbox_Accuracy_Ticks);
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.lbl_Accuracy_Ticks);
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.lbl_Accuracy_NbSweeps);
            this.gBox_Accuracy_FPGA_Param.Controls.Add(this.lbl_Accuracy_NbPowerON);
            this.gBox_Accuracy_FPGA_Param.Enabled = false;
            this.gBox_Accuracy_FPGA_Param.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.gBox_Accuracy_FPGA_Param.Location = new System.Drawing.Point(65, 16);
            this.gBox_Accuracy_FPGA_Param.Name = "gBox_Accuracy_FPGA_Param";
            this.gBox_Accuracy_FPGA_Param.Size = new System.Drawing.Size(323, 309);
            this.gBox_Accuracy_FPGA_Param.TabIndex = 10;
            this.gBox_Accuracy_FPGA_Param.TabStop = false;
            this.gBox_Accuracy_FPGA_Param.Text = "FPGA Parameters";
            // 
            // nUpDown_Accuracy_Threshold
            // 
            this.nUpDown_Accuracy_Threshold.DecimalPlaces = 1;
            this.nUpDown_Accuracy_Threshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUpDown_Accuracy_Threshold.Location = new System.Drawing.Point(25, 265);
            this.nUpDown_Accuracy_Threshold.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUpDown_Accuracy_Threshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUpDown_Accuracy_Threshold.Name = "nUpDown_Accuracy_Threshold";
            this.nUpDown_Accuracy_Threshold.Size = new System.Drawing.Size(122, 20);
            this.nUpDown_Accuracy_Threshold.TabIndex = 12;
            this.nUpDown_Accuracy_Threshold.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUpDown_Accuracy_Threshold.ValueChanged += new System.EventHandler(this.nUpDown_Accuracy_Threshold_ValueChanged);
            // 
            // lbl_Accuracy_Threshold
            // 
            this.lbl_Accuracy_Threshold.AutoSize = true;
            this.lbl_Accuracy_Threshold.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Accuracy_Threshold.Location = new System.Drawing.Point(151, 267);
            this.lbl_Accuracy_Threshold.Name = "lbl_Accuracy_Threshold";
            this.lbl_Accuracy_Threshold.Size = new System.Drawing.Size(96, 13);
            this.lbl_Accuracy_Threshold.TabIndex = 13;
            this.lbl_Accuracy_Threshold.Text = "Icc Threshold [mA]";
            // 
            // cbox_Accuracy_MeasurType
            // 
            this.cbox_Accuracy_MeasurType.FormattingEnabled = true;
            this.cbox_Accuracy_MeasurType.Location = new System.Drawing.Point(26, 155);
            this.cbox_Accuracy_MeasurType.Name = "cbox_Accuracy_MeasurType";
            this.cbox_Accuracy_MeasurType.Size = new System.Drawing.Size(121, 21);
            this.cbox_Accuracy_MeasurType.TabIndex = 10;
            this.cbox_Accuracy_MeasurType.SelectedIndexChanged += new System.EventHandler(this.cbox_Accuracy_MeasurType_SelectedIndexChanged);
            // 
            // lbl_Accuracy_MeasurType
            // 
            this.lbl_Accuracy_MeasurType.AutoSize = true;
            this.lbl_Accuracy_MeasurType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Accuracy_MeasurType.Location = new System.Drawing.Point(153, 158);
            this.lbl_Accuracy_MeasurType.Name = "lbl_Accuracy_MeasurType";
            this.lbl_Accuracy_MeasurType.Size = new System.Drawing.Size(75, 13);
            this.lbl_Accuracy_MeasurType.TabIndex = 11;
            this.lbl_Accuracy_MeasurType.Text = "Measure Type";
            // 
            // nUpDown_Accuracy_NbPowerON
            // 
            this.nUpDown_Accuracy_NbPowerON.Location = new System.Drawing.Point(26, 41);
            this.nUpDown_Accuracy_NbPowerON.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUpDown_Accuracy_NbPowerON.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUpDown_Accuracy_NbPowerON.Name = "nUpDown_Accuracy_NbPowerON";
            this.nUpDown_Accuracy_NbPowerON.Size = new System.Drawing.Size(121, 20);
            this.nUpDown_Accuracy_NbPowerON.TabIndex = 0;
            this.nUpDown_Accuracy_NbPowerON.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nUpDown_Accuracy_NbSweeps
            // 
            this.nUpDown_Accuracy_NbSweeps.Location = new System.Drawing.Point(26, 97);
            this.nUpDown_Accuracy_NbSweeps.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUpDown_Accuracy_NbSweeps.Name = "nUpDown_Accuracy_NbSweeps";
            this.nUpDown_Accuracy_NbSweeps.Size = new System.Drawing.Size(121, 20);
            this.nUpDown_Accuracy_NbSweeps.TabIndex = 1;
            this.nUpDown_Accuracy_NbSweeps.ValueChanged += new System.EventHandler(this.nUpDown_Accuracy_NbSweeps_ValueChanged);
            // 
            // cbox_Accuracy_Ticks
            // 
            this.cbox_Accuracy_Ticks.FormattingEnabled = true;
            this.cbox_Accuracy_Ticks.Location = new System.Drawing.Point(25, 211);
            this.cbox_Accuracy_Ticks.Name = "cbox_Accuracy_Ticks";
            this.cbox_Accuracy_Ticks.Size = new System.Drawing.Size(121, 21);
            this.cbox_Accuracy_Ticks.TabIndex = 2;
            this.cbox_Accuracy_Ticks.SelectedIndexChanged += new System.EventHandler(this.cbox_Accuracy_Ticks_SelectedIndexChanged);
            // 
            // lbl_Accuracy_Ticks
            // 
            this.lbl_Accuracy_Ticks.AutoSize = true;
            this.lbl_Accuracy_Ticks.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Accuracy_Ticks.Location = new System.Drawing.Point(152, 214);
            this.lbl_Accuracy_Ticks.Name = "lbl_Accuracy_Ticks";
            this.lbl_Accuracy_Ticks.Size = new System.Drawing.Size(69, 13);
            this.lbl_Accuracy_Ticks.TabIndex = 7;
            this.lbl_Accuracy_Ticks.Text = "Ticks (Clock)";
            // 
            // lbl_Accuracy_NbSweeps
            // 
            this.lbl_Accuracy_NbSweeps.AutoSize = true;
            this.lbl_Accuracy_NbSweeps.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Accuracy_NbSweeps.Location = new System.Drawing.Point(152, 99);
            this.lbl_Accuracy_NbSweeps.Name = "lbl_Accuracy_NbSweeps";
            this.lbl_Accuracy_NbSweeps.Size = new System.Drawing.Size(136, 13);
            this.lbl_Accuracy_NbSweeps.TabIndex = 6;
            this.lbl_Accuracy_NbSweeps.Text = "Number of Edges per Cycle";
            // 
            // lbl_Accuracy_NbPowerON
            // 
            this.lbl_Accuracy_NbPowerON.AutoSize = true;
            this.lbl_Accuracy_NbPowerON.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Accuracy_NbPowerON.Location = new System.Drawing.Point(151, 43);
            this.lbl_Accuracy_NbPowerON.Name = "lbl_Accuracy_NbPowerON";
            this.lbl_Accuracy_NbPowerON.Size = new System.Drawing.Size(108, 13);
            this.lbl_Accuracy_NbPowerON.TabIndex = 5;
            this.lbl_Accuracy_NbPowerON.Text = "Number of Power ON";
            // 
            // TabPageJitter
            // 
            this.TabPageJitter.BackColor = System.Drawing.Color.LemonChiffon;
            this.TabPageJitter.Controls.Add(this.gBox_Tests_Jitter_FPGAParam);
            this.TabPageJitter.Location = new System.Drawing.Point(4, 22);
            this.TabPageJitter.Name = "TabPageJitter";
            this.TabPageJitter.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageJitter.Size = new System.Drawing.Size(452, 570);
            this.TabPageJitter.TabIndex = 1;
            this.TabPageJitter.Text = "Jitter";
            this.TabPageJitter.Enter += new System.EventHandler(this.TabPageTests_Enter);
            // 
            // gBox_Tests_Jitter_FPGAParam
            // 
            this.gBox_Tests_Jitter_FPGAParam.BackColor = System.Drawing.Color.Transparent;
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.nUpDo_Test_Jitter_EdgePerPeriod);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Test_Jitter_EdgePerPeriod);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.tbox_Tests_Jitter_AnalysisTime);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Tests_Jitter_Or);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.nUpDown_Tests_Jitter_NbSweepsPerEdge);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Tests_Jitter_NbSweepsPerEdge);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.nUpDown_Tests_Jitter_StartAngle);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Tests_Jitter_StartAngle);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.cbox_Tests_Jitter_StartType);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Tests_Jitter_StartType);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.nUpDown_Tests_Jitter_Threshold);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Tests_Jitter_Threshold);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.cbox_Tests_Jitter_MeasureType);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Tests_Jitter_MeasureType);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.nUpDown_Tests_Jitter_NbSweepsTotal);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.cbox_Tests_Jitter_Ticks);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Tests_Jitter_Ticks);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Tests_Jitter_NbSweepsTotal);
            this.gBox_Tests_Jitter_FPGAParam.Controls.Add(this.lbl_Tests_Jitter_AnalysisTime);
            this.gBox_Tests_Jitter_FPGAParam.Enabled = false;
            this.gBox_Tests_Jitter_FPGAParam.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.gBox_Tests_Jitter_FPGAParam.Location = new System.Drawing.Point(34, 36);
            this.gBox_Tests_Jitter_FPGAParam.Name = "gBox_Tests_Jitter_FPGAParam";
            this.gBox_Tests_Jitter_FPGAParam.Size = new System.Drawing.Size(380, 491);
            this.gBox_Tests_Jitter_FPGAParam.TabIndex = 11;
            this.gBox_Tests_Jitter_FPGAParam.TabStop = false;
            this.gBox_Tests_Jitter_FPGAParam.Text = "FPGA Parameters";
            // 
            // nUpDo_Test_Jitter_EdgePerPeriod
            // 
            this.nUpDo_Test_Jitter_EdgePerPeriod.Location = new System.Drawing.Point(31, 129);
            this.nUpDo_Test_Jitter_EdgePerPeriod.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUpDo_Test_Jitter_EdgePerPeriod.Name = "nUpDo_Test_Jitter_EdgePerPeriod";
            this.nUpDo_Test_Jitter_EdgePerPeriod.Size = new System.Drawing.Size(121, 20);
            this.nUpDo_Test_Jitter_EdgePerPeriod.TabIndex = 23;
            this.nUpDo_Test_Jitter_EdgePerPeriod.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUpDo_Test_Jitter_EdgePerPeriod.ValueChanged += new System.EventHandler(this.nUpDo_Test_Jitter_EdgePerPeriod_ValueChanged);
            // 
            // lbl_Test_Jitter_EdgePerPeriod
            // 
            this.lbl_Test_Jitter_EdgePerPeriod.AutoSize = true;
            this.lbl_Test_Jitter_EdgePerPeriod.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Test_Jitter_EdgePerPeriod.Location = new System.Drawing.Point(157, 131);
            this.lbl_Test_Jitter_EdgePerPeriod.Name = "lbl_Test_Jitter_EdgePerPeriod";
            this.lbl_Test_Jitter_EdgePerPeriod.Size = new System.Drawing.Size(141, 13);
            this.lbl_Test_Jitter_EdgePerPeriod.TabIndex = 24;
            this.lbl_Test_Jitter_EdgePerPeriod.Text = "Edge(s) per Magnetic Period";
            // 
            // tbox_Tests_Jitter_AnalysisTime
            // 
            this.tbox_Tests_Jitter_AnalysisTime.Location = new System.Drawing.Point(112, 178);
            this.tbox_Tests_Jitter_AnalysisTime.Name = "tbox_Tests_Jitter_AnalysisTime";
            this.tbox_Tests_Jitter_AnalysisTime.Size = new System.Drawing.Size(54, 20);
            this.tbox_Tests_Jitter_AnalysisTime.TabIndex = 21;
            // 
            // lbl_Tests_Jitter_Or
            // 
            this.lbl_Tests_Jitter_Or.AutoSize = true;
            this.lbl_Tests_Jitter_Or.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Jitter_Or.Location = new System.Drawing.Point(199, 58);
            this.lbl_Tests_Jitter_Or.Name = "lbl_Tests_Jitter_Or";
            this.lbl_Tests_Jitter_Or.Size = new System.Drawing.Size(16, 13);
            this.lbl_Tests_Jitter_Or.TabIndex = 20;
            this.lbl_Tests_Jitter_Or.Text = "or";
            // 
            // nUpDown_Tests_Jitter_NbSweepsPerEdge
            // 
            this.nUpDown_Tests_Jitter_NbSweepsPerEdge.DecimalPlaces = 3;
            this.nUpDown_Tests_Jitter_NbSweepsPerEdge.Location = new System.Drawing.Point(31, 81);
            this.nUpDown_Tests_Jitter_NbSweepsPerEdge.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUpDown_Tests_Jitter_NbSweepsPerEdge.Name = "nUpDown_Tests_Jitter_NbSweepsPerEdge";
            this.nUpDown_Tests_Jitter_NbSweepsPerEdge.Size = new System.Drawing.Size(121, 20);
            this.nUpDown_Tests_Jitter_NbSweepsPerEdge.TabIndex = 18;
            this.nUpDown_Tests_Jitter_NbSweepsPerEdge.ValueChanged += new System.EventHandler(this.nUpDown_Tests_Jitter_NbSweepsPerEdge_ValueChanged);
            // 
            // lbl_Tests_Jitter_NbSweepsPerEdge
            // 
            this.lbl_Tests_Jitter_NbSweepsPerEdge.AutoSize = true;
            this.lbl_Tests_Jitter_NbSweepsPerEdge.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Jitter_NbSweepsPerEdge.Location = new System.Drawing.Point(157, 83);
            this.lbl_Tests_Jitter_NbSweepsPerEdge.Name = "lbl_Tests_Jitter_NbSweepsPerEdge";
            this.lbl_Tests_Jitter_NbSweepsPerEdge.Size = new System.Drawing.Size(149, 13);
            this.lbl_Tests_Jitter_NbSweepsPerEdge.TabIndex = 19;
            this.lbl_Tests_Jitter_NbSweepsPerEdge.Text = "Number of Sweeps (per Edge)";
            // 
            // nUpDown_Tests_Jitter_StartAngle
            // 
            this.nUpDown_Tests_Jitter_StartAngle.DecimalPlaces = 1;
            this.nUpDown_Tests_Jitter_StartAngle.Location = new System.Drawing.Point(29, 441);
            this.nUpDown_Tests_Jitter_StartAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nUpDown_Tests_Jitter_StartAngle.Name = "nUpDown_Tests_Jitter_StartAngle";
            this.nUpDown_Tests_Jitter_StartAngle.Size = new System.Drawing.Size(122, 20);
            this.nUpDown_Tests_Jitter_StartAngle.TabIndex = 16;
            this.nUpDown_Tests_Jitter_StartAngle.ValueChanged += new System.EventHandler(this.nUpDown_Tests_Jitter_StartAngle_ValueChanged);
            // 
            // lbl_Tests_Jitter_StartAngle
            // 
            this.lbl_Tests_Jitter_StartAngle.AutoSize = true;
            this.lbl_Tests_Jitter_StartAngle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Jitter_StartAngle.Location = new System.Drawing.Point(155, 443);
            this.lbl_Tests_Jitter_StartAngle.Name = "lbl_Tests_Jitter_StartAngle";
            this.lbl_Tests_Jitter_StartAngle.Size = new System.Drawing.Size(101, 13);
            this.lbl_Tests_Jitter_StartAngle.TabIndex = 17;
            this.lbl_Tests_Jitter_StartAngle.Text = "Start Angle (degree)";
            // 
            // cbox_Tests_Jitter_StartType
            // 
            this.cbox_Tests_Jitter_StartType.FormattingEnabled = true;
            this.cbox_Tests_Jitter_StartType.Location = new System.Drawing.Point(29, 392);
            this.cbox_Tests_Jitter_StartType.Name = "cbox_Tests_Jitter_StartType";
            this.cbox_Tests_Jitter_StartType.Size = new System.Drawing.Size(121, 21);
            this.cbox_Tests_Jitter_StartType.TabIndex = 14;
            this.cbox_Tests_Jitter_StartType.SelectedIndexChanged += new System.EventHandler(this.cbox_Tests_Jitter_StartType_SelectedIndexChanged);
            // 
            // lbl_Tests_Jitter_StartType
            // 
            this.lbl_Tests_Jitter_StartType.AutoSize = true;
            this.lbl_Tests_Jitter_StartType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Jitter_StartType.Location = new System.Drawing.Point(156, 395);
            this.lbl_Tests_Jitter_StartType.Name = "lbl_Tests_Jitter_StartType";
            this.lbl_Tests_Jitter_StartType.Size = new System.Drawing.Size(56, 13);
            this.lbl_Tests_Jitter_StartType.TabIndex = 15;
            this.lbl_Tests_Jitter_StartType.Text = "Start Type";
            // 
            // nUpDown_Tests_Jitter_Threshold
            // 
            this.nUpDown_Tests_Jitter_Threshold.DecimalPlaces = 1;
            this.nUpDown_Tests_Jitter_Threshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUpDown_Tests_Jitter_Threshold.Location = new System.Drawing.Point(28, 342);
            this.nUpDown_Tests_Jitter_Threshold.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUpDown_Tests_Jitter_Threshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUpDown_Tests_Jitter_Threshold.Name = "nUpDown_Tests_Jitter_Threshold";
            this.nUpDown_Tests_Jitter_Threshold.Size = new System.Drawing.Size(122, 20);
            this.nUpDown_Tests_Jitter_Threshold.TabIndex = 12;
            this.nUpDown_Tests_Jitter_Threshold.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUpDown_Tests_Jitter_Threshold.ValueChanged += new System.EventHandler(this.nUpDown_Tests_Jitter_Threshold_ValueChanged);
            // 
            // lbl_Tests_Jitter_Threshold
            // 
            this.lbl_Tests_Jitter_Threshold.AutoSize = true;
            this.lbl_Tests_Jitter_Threshold.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Jitter_Threshold.Location = new System.Drawing.Point(154, 344);
            this.lbl_Tests_Jitter_Threshold.Name = "lbl_Tests_Jitter_Threshold";
            this.lbl_Tests_Jitter_Threshold.Size = new System.Drawing.Size(96, 13);
            this.lbl_Tests_Jitter_Threshold.TabIndex = 13;
            this.lbl_Tests_Jitter_Threshold.Text = "Icc Threshold [mA]";
            // 
            // cbox_Tests_Jitter_MeasureType
            // 
            this.cbox_Tests_Jitter_MeasureType.FormattingEnabled = true;
            this.cbox_Tests_Jitter_MeasureType.Location = new System.Drawing.Point(29, 232);
            this.cbox_Tests_Jitter_MeasureType.Name = "cbox_Tests_Jitter_MeasureType";
            this.cbox_Tests_Jitter_MeasureType.Size = new System.Drawing.Size(121, 21);
            this.cbox_Tests_Jitter_MeasureType.TabIndex = 10;
            this.cbox_Tests_Jitter_MeasureType.SelectedIndexChanged += new System.EventHandler(this.cbox_Tests_Jitter_MeasureType_SelectedIndexChanged);
            // 
            // lbl_Tests_Jitter_MeasureType
            // 
            this.lbl_Tests_Jitter_MeasureType.AutoSize = true;
            this.lbl_Tests_Jitter_MeasureType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Jitter_MeasureType.Location = new System.Drawing.Point(156, 235);
            this.lbl_Tests_Jitter_MeasureType.Name = "lbl_Tests_Jitter_MeasureType";
            this.lbl_Tests_Jitter_MeasureType.Size = new System.Drawing.Size(75, 13);
            this.lbl_Tests_Jitter_MeasureType.TabIndex = 11;
            this.lbl_Tests_Jitter_MeasureType.Text = "Measure Type";
            // 
            // nUpDown_Tests_Jitter_NbSweepsTotal
            // 
            this.nUpDown_Tests_Jitter_NbSweepsTotal.DecimalPlaces = 3;
            this.nUpDown_Tests_Jitter_NbSweepsTotal.Location = new System.Drawing.Point(31, 29);
            this.nUpDown_Tests_Jitter_NbSweepsTotal.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUpDown_Tests_Jitter_NbSweepsTotal.Name = "nUpDown_Tests_Jitter_NbSweepsTotal";
            this.nUpDown_Tests_Jitter_NbSweepsTotal.Size = new System.Drawing.Size(121, 20);
            this.nUpDown_Tests_Jitter_NbSweepsTotal.TabIndex = 1;
            this.nUpDown_Tests_Jitter_NbSweepsTotal.ValueChanged += new System.EventHandler(this.nUpDown_Tests_Jitter_NbSweepsTotal_ValueChanged);
            // 
            // cbox_Tests_Jitter_Ticks
            // 
            this.cbox_Tests_Jitter_Ticks.FormattingEnabled = true;
            this.cbox_Tests_Jitter_Ticks.Location = new System.Drawing.Point(28, 288);
            this.cbox_Tests_Jitter_Ticks.Name = "cbox_Tests_Jitter_Ticks";
            this.cbox_Tests_Jitter_Ticks.Size = new System.Drawing.Size(121, 21);
            this.cbox_Tests_Jitter_Ticks.TabIndex = 2;
            this.cbox_Tests_Jitter_Ticks.SelectedIndexChanged += new System.EventHandler(this.cbox_Tests_Jitter_Ticks_SelectedIndexChanged);
            // 
            // lbl_Tests_Jitter_Ticks
            // 
            this.lbl_Tests_Jitter_Ticks.AutoSize = true;
            this.lbl_Tests_Jitter_Ticks.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Jitter_Ticks.Location = new System.Drawing.Point(155, 291);
            this.lbl_Tests_Jitter_Ticks.Name = "lbl_Tests_Jitter_Ticks";
            this.lbl_Tests_Jitter_Ticks.Size = new System.Drawing.Size(69, 13);
            this.lbl_Tests_Jitter_Ticks.TabIndex = 7;
            this.lbl_Tests_Jitter_Ticks.Text = "Ticks (Clock)";
            // 
            // lbl_Tests_Jitter_NbSweepsTotal
            // 
            this.lbl_Tests_Jitter_NbSweepsTotal.AutoSize = true;
            this.lbl_Tests_Jitter_NbSweepsTotal.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Jitter_NbSweepsTotal.Location = new System.Drawing.Point(157, 31);
            this.lbl_Tests_Jitter_NbSweepsTotal.Name = "lbl_Tests_Jitter_NbSweepsTotal";
            this.lbl_Tests_Jitter_NbSweepsTotal.Size = new System.Drawing.Size(130, 13);
            this.lbl_Tests_Jitter_NbSweepsTotal.TabIndex = 6;
            this.lbl_Tests_Jitter_NbSweepsTotal.Text = "Number of Sweeps (Total)";
            // 
            // lbl_Tests_Jitter_AnalysisTime
            // 
            this.lbl_Tests_Jitter_AnalysisTime.AutoSize = true;
            this.lbl_Tests_Jitter_AnalysisTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Jitter_AnalysisTime.Location = new System.Drawing.Point(25, 181);
            this.lbl_Tests_Jitter_AnalysisTime.Name = "lbl_Tests_Jitter_AnalysisTime";
            this.lbl_Tests_Jitter_AnalysisTime.Size = new System.Drawing.Size(206, 13);
            this.lbl_Tests_Jitter_AnalysisTime.TabIndex = 22;
            this.lbl_Tests_Jitter_AnalysisTime.Text = "Analysis during :                      revolution(s)";
            // 
            // TabPageAnalog
            // 
            this.TabPageAnalog.BackColor = System.Drawing.Color.Honeydew;
            this.TabPageAnalog.Location = new System.Drawing.Point(4, 22);
            this.TabPageAnalog.Name = "TabPageAnalog";
            this.TabPageAnalog.Size = new System.Drawing.Size(452, 570);
            this.TabPageAnalog.TabIndex = 2;
            this.TabPageAnalog.Text = "Analog";
            this.TabPageAnalog.Enter += new System.EventHandler(this.TabPageTests_Enter);
            // 
            // gBox_Tests_Analysis
            // 
            this.gBox_Tests_Analysis.Controls.Add(this.lbl_Tests_Analysis_TargetTeeth);
            this.gBox_Tests_Analysis.Controls.Add(this.lbl_Tests_Type);
            this.gBox_Tests_Analysis.Controls.Add(this.nUpDown_Tests_Analysis_TargetTeeth);
            this.gBox_Tests_Analysis.Controls.Add(this.chkbox_Tests_Type_Accuracy);
            this.gBox_Tests_Analysis.Controls.Add(this.chkbox_Tests_Type_Jitter);
            this.gBox_Tests_Analysis.Controls.Add(this.chkbox_Test_Type_Analog);
            this.gBox_Tests_Analysis.Controls.Add(this.but_Tests_StopAnalysis);
            this.gBox_Tests_Analysis.Controls.Add(this.but_Tests_StartAnalysis);
            this.gBox_Tests_Analysis.Controls.Add(this.tbox_Tests_AnalysisStatus);
            this.gBox_Tests_Analysis.Enabled = false;
            this.gBox_Tests_Analysis.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.gBox_Tests_Analysis.Location = new System.Drawing.Point(20, 485);
            this.gBox_Tests_Analysis.Name = "gBox_Tests_Analysis";
            this.gBox_Tests_Analysis.Size = new System.Drawing.Size(500, 145);
            this.gBox_Tests_Analysis.TabIndex = 18;
            this.gBox_Tests_Analysis.TabStop = false;
            this.gBox_Tests_Analysis.Text = "Analysis";
            // 
            // lbl_Tests_Analysis_TargetTeeth
            // 
            this.lbl_Tests_Analysis_TargetTeeth.AutoSize = true;
            this.lbl_Tests_Analysis_TargetTeeth.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Analysis_TargetTeeth.Location = new System.Drawing.Point(234, 84);
            this.lbl_Tests_Analysis_TargetTeeth.Name = "lbl_Tests_Analysis_TargetTeeth";
            this.lbl_Tests_Analysis_TargetTeeth.Size = new System.Drawing.Size(116, 13);
            this.lbl_Tests_Analysis_TargetTeeth.TabIndex = 25;
            this.lbl_Tests_Analysis_TargetTeeth.Text = "Target\'s Teeth Number";
            // 
            // lbl_Tests_Type
            // 
            this.lbl_Tests_Type.AutoSize = true;
            this.lbl_Tests_Type.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Tests_Type.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_Tests_Type.Location = new System.Drawing.Point(410, 16);
            this.lbl_Tests_Type.Name = "lbl_Tests_Type";
            this.lbl_Tests_Type.Size = new System.Drawing.Size(70, 13);
            this.lbl_Tests_Type.TabIndex = 23;
            this.lbl_Tests_Type.Text = "Tests Type";
            // 
            // nUpDown_Tests_Analysis_TargetTeeth
            // 
            this.nUpDown_Tests_Analysis_TargetTeeth.Location = new System.Drawing.Point(179, 82);
            this.nUpDown_Tests_Analysis_TargetTeeth.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nUpDown_Tests_Analysis_TargetTeeth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUpDown_Tests_Analysis_TargetTeeth.Name = "nUpDown_Tests_Analysis_TargetTeeth";
            this.nUpDown_Tests_Analysis_TargetTeeth.Size = new System.Drawing.Size(49, 20);
            this.nUpDown_Tests_Analysis_TargetTeeth.TabIndex = 24;
            this.nUpDown_Tests_Analysis_TargetTeeth.Value = new decimal(new int[] {
            18,
            0,
            0,
            0});
            this.nUpDown_Tests_Analysis_TargetTeeth.ValueChanged += new System.EventHandler(this.nUpDown_Tests_Analysis_TargetTeeth_ValueChanged);
            // 
            // chkbox_Tests_Type_Accuracy
            // 
            this.chkbox_Tests_Type_Accuracy.AutoSize = true;
            this.chkbox_Tests_Type_Accuracy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkbox_Tests_Type_Accuracy.Location = new System.Drawing.Point(413, 47);
            this.chkbox_Tests_Type_Accuracy.Name = "chkbox_Tests_Type_Accuracy";
            this.chkbox_Tests_Type_Accuracy.Size = new System.Drawing.Size(71, 17);
            this.chkbox_Tests_Type_Accuracy.TabIndex = 20;
            this.chkbox_Tests_Type_Accuracy.Text = "Accuracy";
            this.chkbox_Tests_Type_Accuracy.UseVisualStyleBackColor = true;
            this.chkbox_Tests_Type_Accuracy.Click += new System.EventHandler(this.chkbox_Tests_Type_Accuracy_Click);
            // 
            // chkbox_Tests_Type_Jitter
            // 
            this.chkbox_Tests_Type_Jitter.AutoSize = true;
            this.chkbox_Tests_Type_Jitter.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkbox_Tests_Type_Jitter.Location = new System.Drawing.Point(413, 81);
            this.chkbox_Tests_Type_Jitter.Name = "chkbox_Tests_Type_Jitter";
            this.chkbox_Tests_Type_Jitter.Size = new System.Drawing.Size(48, 17);
            this.chkbox_Tests_Type_Jitter.TabIndex = 21;
            this.chkbox_Tests_Type_Jitter.Text = "Jitter";
            this.chkbox_Tests_Type_Jitter.UseVisualStyleBackColor = true;
            this.chkbox_Tests_Type_Jitter.Click += new System.EventHandler(this.chkbox_Tests_Type_Jitter_Click);
            // 
            // chkbox_Test_Type_Analog
            // 
            this.chkbox_Test_Type_Analog.AutoSize = true;
            this.chkbox_Test_Type_Analog.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkbox_Test_Type_Analog.Location = new System.Drawing.Point(413, 116);
            this.chkbox_Test_Type_Analog.Name = "chkbox_Test_Type_Analog";
            this.chkbox_Test_Type_Analog.Size = new System.Drawing.Size(59, 17);
            this.chkbox_Test_Type_Analog.TabIndex = 22;
            this.chkbox_Test_Type_Analog.Text = "Analog";
            this.chkbox_Test_Type_Analog.UseVisualStyleBackColor = true;
            this.chkbox_Test_Type_Analog.Click += new System.EventHandler(this.chkbox_Test_Type_Analog_Click);
            // 
            // but_Tests_StopAnalysis
            // 
            this.but_Tests_StopAnalysis.ForeColor = System.Drawing.SystemColors.ControlText;
            this.but_Tests_StopAnalysis.Location = new System.Drawing.Point(25, 82);
            this.but_Tests_StopAnalysis.Name = "but_Tests_StopAnalysis";
            this.but_Tests_StopAnalysis.Size = new System.Drawing.Size(90, 40);
            this.but_Tests_StopAnalysis.TabIndex = 12;
            this.but_Tests_StopAnalysis.Text = "Stop Analysis";
            this.but_Tests_StopAnalysis.UseVisualStyleBackColor = true;
            this.but_Tests_StopAnalysis.Click += new System.EventHandler(this.but_Tests_StopAnalysis_Click);
            // 
            // but_Tests_StartAnalysis
            // 
            this.but_Tests_StartAnalysis.ForeColor = System.Drawing.SystemColors.ControlText;
            this.but_Tests_StartAnalysis.Location = new System.Drawing.Point(25, 36);
            this.but_Tests_StartAnalysis.Name = "but_Tests_StartAnalysis";
            this.but_Tests_StartAnalysis.Size = new System.Drawing.Size(90, 40);
            this.but_Tests_StartAnalysis.TabIndex = 10;
            this.but_Tests_StartAnalysis.Text = "Start Analysis";
            this.but_Tests_StartAnalysis.UseVisualStyleBackColor = true;
            this.but_Tests_StartAnalysis.Click += new System.EventHandler(this.but_Tests_StartAnalysis_Click);
            // 
            // tbox_Tests_AnalysisStatus
            // 
            this.tbox_Tests_AnalysisStatus.Location = new System.Drawing.Point(129, 47);
            this.tbox_Tests_AnalysisStatus.Name = "tbox_Tests_AnalysisStatus";
            this.tbox_Tests_AnalysisStatus.Size = new System.Drawing.Size(268, 20);
            this.tbox_Tests_AnalysisStatus.TabIndex = 11;
            // 
            // TabPageAcquisition
            // 
            this.TabPageAcquisition.BackColor = System.Drawing.Color.Gainsboro;
            this.TabPageAcquisition.Controls.Add(this.AcquisitionControl1);
            this.TabPageAcquisition.Location = new System.Drawing.Point(4, 22);
            this.TabPageAcquisition.Name = "TabPageAcquisition";
            this.TabPageAcquisition.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageAcquisition.Size = new System.Drawing.Size(1032, 660);
            this.TabPageAcquisition.TabIndex = 0;
            this.TabPageAcquisition.Text = "Acquisition";
            // 
            // AcquisitionControl1
            // 
            this.AcquisitionControl1.bControlTestStop = false;
            this.AcquisitionControl1.bStateConnection = false;
            this.AcquisitionControl1.bStatePowerON = false;
            this.AcquisitionControl1.bThreadControlRunning = false;
            this.AcquisitionControl1.Location = new System.Drawing.Point(6, 6);
            this.AcquisitionControl1.Name = "AcquisitionControl1";
            this.AcquisitionControl1.Size = new System.Drawing.Size(1042, 634);
            this.AcquisitionControl1.strControlAutoTestStatus = "";
            this.AcquisitionControl1.strFileDirectory = "";
            this.AcquisitionControl1.TabIndex = 0;
            this.AcquisitionControl1.uiNbTargetTeeth = ((uint)(18u));
            // 
            // TabFPGA
            // 
            this.TabFPGA.Controls.Add(this.TabPageAcquisition);
            this.TabFPGA.Controls.Add(this.TabPageTests);
            this.TabFPGA.Location = new System.Drawing.Point(12, 12);
            this.TabFPGA.Name = "TabFPGA";
            this.TabFPGA.SelectedIndex = 0;
            this.TabFPGA.Size = new System.Drawing.Size(1040, 686);
            this.TabFPGA.TabIndex = 1;
            // 
            // frm_Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 710);
            this.Controls.Add(this.TabFPGA);
            this.Name = "frm_Interface";
            this.Text = "FPGA Acquisition Board";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Closing);
            this.TabPageTests.ResumeLayout(false);
            this.TabPageTests.PerformLayout();
            this.TabTestType.ResumeLayout(false);
            this.TabPageAccuracy.ResumeLayout(false);
            this.gBox_Accuracy_Phased_PON.ResumeLayout(false);
            this.gBox_Accuracy_Phased_PON.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_StepSizeAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_StartupAngle)).EndInit();
            this.gBox_Accuracy_PowerONStart.ResumeLayout(false);
            this.gBox_Accuracy_PowerONStart.PerformLayout();
            this.gBox_Accuracy_FPGA_Param.ResumeLayout(false);
            this.gBox_Accuracy_FPGA_Param.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_Threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_NbPowerON)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Accuracy_NbSweeps)).EndInit();
            this.TabPageJitter.ResumeLayout(false);
            this.gBox_Tests_Jitter_FPGAParam.ResumeLayout(false);
            this.gBox_Tests_Jitter_FPGAParam.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDo_Test_Jitter_EdgePerPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Jitter_NbSweepsPerEdge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Jitter_StartAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Jitter_Threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Jitter_NbSweepsTotal)).EndInit();
            this.gBox_Tests_Analysis.ResumeLayout(false);
            this.gBox_Tests_Analysis.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUpDown_Tests_Analysis_TargetTeeth)).EndInit();
            this.TabPageAcquisition.ResumeLayout(false);
            this.TabFPGA.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer TimerAccuracyStatus;
        private System.Windows.Forms.TabPage TabPageTests;
        private System.Windows.Forms.GroupBox gBox_Tests_Analysis;
        private System.Windows.Forms.Button but_Tests_StopAnalysis;
        private System.Windows.Forms.Button but_Tests_StartAnalysis;
        private System.Windows.Forms.TextBox tbox_Tests_AnalysisStatus;
        private System.Windows.Forms.GroupBox gBox_Accuracy_PowerONStart;
        private System.Windows.Forms.CheckBox chkbox_Accuracy_RandomPON;
        private System.Windows.Forms.CheckBox chkbox_Accuracy_PhasedPON;
        private System.Windows.Forms.GroupBox gBox_Accuracy_Phased_PON;
        private System.Windows.Forms.NumericUpDown nUpDown_Accuracy_StepSizeAngle;
        private System.Windows.Forms.Label lbl_Accuracy_StepSizeAngle;
        private System.Windows.Forms.NumericUpDown nUpDown_Accuracy_StartupAngle;
        private System.Windows.Forms.Label lbl_Accuracy_StartupAngle;
        private System.Windows.Forms.GroupBox gBox_Accuracy_FPGA_Param;
        private System.Windows.Forms.NumericUpDown nUpDown_Accuracy_Threshold;
        private System.Windows.Forms.Label lbl_Accuracy_Threshold;
        private System.Windows.Forms.ComboBox cbox_Accuracy_MeasurType;
        private System.Windows.Forms.Label lbl_Accuracy_MeasurType;
        private System.Windows.Forms.NumericUpDown nUpDown_Accuracy_NbPowerON;
        private System.Windows.Forms.NumericUpDown nUpDown_Accuracy_NbSweeps;
        private System.Windows.Forms.ComboBox cbox_Accuracy_Ticks;
        private System.Windows.Forms.Label lbl_Accuracy_Ticks;
        private System.Windows.Forms.Label lbl_Accuracy_NbSweeps;
        private System.Windows.Forms.Label lbl_Accuracy_NbPowerON;
        private System.Windows.Forms.TabPage TabPageAcquisition;
        private FPGA_AcquisitionBoard.AcquisitionControl AcquisitionControl1;
        private System.Windows.Forms.TabControl TabFPGA;
        private System.Windows.Forms.TabControl TabTestType;
        private System.Windows.Forms.TabPage TabPageAccuracy;
        private System.Windows.Forms.TabPage TabPageJitter;
        private System.Windows.Forms.TabPage TabPageAnalog;
        private System.Windows.Forms.CheckBox chkbox_Tests_Type_Accuracy;
        private System.Windows.Forms.CheckBox chkbox_Tests_Type_Jitter;
        private System.Windows.Forms.CheckBox chkbox_Test_Type_Analog;
        private System.Windows.Forms.Label lbl_Tests_Type;
        private System.Windows.Forms.GroupBox gBox_Tests_MotorAndAxes;
        private System.Windows.Forms.GroupBox gBox_Tests_Jitter_FPGAParam;
        private System.Windows.Forms.NumericUpDown nUpDown_Tests_Jitter_Threshold;
        private System.Windows.Forms.Label lbl_Tests_Jitter_Threshold;
        private System.Windows.Forms.ComboBox cbox_Tests_Jitter_MeasureType;
        private System.Windows.Forms.Label lbl_Tests_Jitter_MeasureType;
        private System.Windows.Forms.NumericUpDown nUpDown_Tests_Jitter_NbSweepsTotal;
        private System.Windows.Forms.ComboBox cbox_Tests_Jitter_Ticks;
        private System.Windows.Forms.Label lbl_Tests_Jitter_Ticks;
        private System.Windows.Forms.Label lbl_Tests_Jitter_NbSweepsTotal;
        private System.Windows.Forms.NumericUpDown nUpDown_Tests_Jitter_NbSweepsPerEdge;
        private System.Windows.Forms.Label lbl_Tests_Jitter_NbSweepsPerEdge;
        private System.Windows.Forms.NumericUpDown nUpDown_Tests_Jitter_StartAngle;
        private System.Windows.Forms.Label lbl_Tests_Jitter_StartAngle;
        private System.Windows.Forms.ComboBox cbox_Tests_Jitter_StartType;
        private System.Windows.Forms.Label lbl_Tests_Jitter_StartType;
        private System.Windows.Forms.Label lbl_Tests_Analysis_TargetTeeth;
        private System.Windows.Forms.NumericUpDown nUpDown_Tests_Analysis_TargetTeeth;
        private System.Windows.Forms.TextBox tbox_Tests_Jitter_AnalysisTime;
        private System.Windows.Forms.Label lbl_Tests_Jitter_Or;
        private System.Windows.Forms.Label lbl_Tests_Jitter_AnalysisTime;
        private System.Windows.Forms.NumericUpDown nUpDo_Test_Jitter_EdgePerPeriod;
        private System.Windows.Forms.Label lbl_Test_Jitter_EdgePerPeriod;
        private System.Windows.Forms.Label lbl_TabTest_TestComment;
        private System.Windows.Forms.TextBox tbox_TabTest_TestComment;
    }
}

