using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace FPGA_AcquisitionBoard
{
    class AcquisitionBoard
    {
        private SerialPort serialportBoard = new SerialPort();


        public const uint RELAY_1_2K_MASK = 0x0101;//switch Pin 2-3 relay 1.32K pull up ON (relay 1 + 9)
        public const uint RELAY_4_7K_MASK = 0x0202;//switch Pin 2-3 relay 4.7K pull up ON (relay 2 + 10)
        public const uint RELAY_4_7nF_MASK = 0x0404;//switch Pin 2-3 relay 4.7nF Cap ON (relay 3 + 11)
        public const uint RELAY_1_0nF_MASK = 0x0808;//switch Pin 2-3 relay 1nF Cap ON (relay 4 + 12)
        public const uint RELAY_DIG_MASK = 0x1010;//switch Pin 2-3 relay digital out OFF (relay 5 + 13)
        public const uint RELAY_ANA_MASK = 0x2020;//switch Pin 2-3 relay analog out OFF (relay 6 + 14)
        public const uint RELAY_VCC_MASK = 0x0040;//switch Vcc relay to internal (relay 7)
        public const uint RELAY_Pin2_MASK = 0xC0CF;//Set all relay on pin 2 to OFF
        public const uint RELAY_Pin3_MASK = 0xFFC0;// NOT CLEAR......

        public const uint RELAY_Address = 0xC004020;//Address of relay registers

        public const uint SW_2WIRE_BIT = 0x00001;

        public const uint START_BIT_0 = 0x01;
        public const uint START_BIT_1 = 0x02;

        public const uint STATUS_STARTED = 0x01;
        public const uint STATUS_RUNNING = 0x02;
        public const uint STATUS_DONE = 0x04;
        public const uint STATUS_FIFO_ERR = 0x08;

        public const uint ARM_SEL_ALWAYS = 0x00;
        public const uint ARM_SEL_ANGLE = 0x01;
        public const uint ARM_SEL_PON = 0x02;

        public const uint DEBUG_BIT_ANGLE = 0x01;
        public const uint DEBUG_BIT_ANALOG = 0x02;
        public const uint DEBUG_BIT_PW = 0x04;
        public const uint DEBUG_BIT_SPEED = 0x08;
        public const uint DEBUG_BIT_ENCODER = 0x10;
        public const uint DEBUG_BIT_ENC_DIR = 0x20;
        public const uint DEBUG_BIT_SENS = 0x40;
        public const uint DEBUG_BIT_2WIRE = 0x80;
        public const uint DEBUG_BIT_SENS_EM = 0x100;

        public const uint TRIG_ENA_CLK = 0x00;
        public const uint TRIG_ENA_RISE0 = 0x01;
        public const uint TRIG_ENA_FALL0 = 0x02;
        public const uint TRIG_ENA_BOTH0 = 0x03;
        public const uint TRIG_ENA_NONE = 0x04;
        public const uint TRIG_ENA_RISE1 = 0x05;
        public const uint TRIG_ENA_FALL1 = 0x06;
        public const uint TRIG_ENA_BOTH1 = 0x07;
        public const uint TRIG_ENA_RISE2 = 0x09;
        public const uint TRIG_ENA_FALL2 = 0x0a;
        public const uint TRIG_ENA_BOTH2 = 0x0b;
        public const uint TRIG_ENA_DECS = 0x0c;
        public const uint TRIG_ENA_B = 0x0d;
        public const uint TRIG_ENA_A = 0x0e;
        public const uint TRIG_ENA_DEC = 0x0f;

        public const uint TRIG_SEL_ALWAYS = ARM_SEL_ALWAYS;
        public const uint TRIG_SEL_ANGLE = ARM_SEL_ANGLE;
        public const uint TRIG_SEL_PON = ARM_SEL_PON;


        public const uint DATA_SEL_ANGLE = 0x00;
        public const uint DATA_SEL_SENS0_ANA = 0x01;
        public const uint DATA_SEL_SENS0_PW = 0x02;
        public const uint DATA_SEL_SENS0_PER = 0x03;
        public const uint DATA_SEL_TIMER = 0x04;
        public const uint DATA_SEL_ANGLE_T = 0x05;
        public const uint DATA_SEL_ICC = 0x06;
        public const uint DATA_SEL_SENS1_ANA = 0x11;
        public const uint DATA_SEL_SENS1_PW = 0x12;
        public const uint DATA_SEL_SENS1_PER = 0x13;
        public const uint DATA_SEL_SENS2_ANA = 0x21;
        public const uint DATA_SEL_SENS2_PW = 0x22;
        public const uint DATA_SEL_SENS2_PER = 0x23;

        public const uint SW_Ena_Cal_bit = 0x02;
        public const uint SW_Ena_Kill_bit = 0x01;

        public const uint digital_in_2WIRE_bit = 0x01;
        public const uint digital_in_PON_bit = 0x02;
        public const uint digital_in_DIG2_bit = 0x04;
        public const uint digital_in_DIG3_bit = 0x08;

        public const uint ADIndex_VPU2 = 0x00;
        public const uint ADIndex_VPin2 = 0x01;
        public const uint ADIndex_VP5 = 0x02;
        public const uint ADIndex_VM5 = 0x03;
        public const uint ADIndex_VPU3 = 0x04;
        public const uint ADIndex_VPin3 = 0x05;
        public const uint ADIndex_VCC = 0x06;
        public const uint ADIndex_ICC = 0x07;


        public const uint sensor_mem_2_span = 512;
        public const uint sensor_mem_1_span = 512;
        public const uint sdram_span = 134217728; // 128MB

        public const uint sensor_mem_2 = 0x10420000;
        public const uint sensor_mem_1 = 0x10400000;
        public const uint sram = 0x10000000;
        public const uint mem_DAC = 0x0C004000;
        public const uint sysid = 0x0C000260;
        public const uint pio_digital_in = 0x0C002150;
        public const uint pio_angle = 0x0C002140;
        public const uint pio_speed = 0x0C002130;
        public const uint pio_debug = 0x0C002120;
        public const uint pio_encoder_pprm1 = 0x0C002110;
        public const uint pio_relay = 0x0C002100;
        public const uint pio_AD_8 = 0x0C002070;
        public const uint pio_AD_7 = 0x0C002060;//ADC 7 - VCC pin max 32V
        public const uint pio_AD_6 = 0x0C002050;
        public const uint pio_AD_5 = 0x0C002040;
        public const uint pio_AD_4 = 0x0C002030;
        public const uint pio_AD_3 = 0x0C002020;
        public const uint pio_AD_2 = 0x0C002010;
        public const uint pio_AD_1 = 0x0C002000;
        public const uint pio_ICC_Limit = 0x0C001520;
        public const uint pio_SW_Reset = 0x0C001510;
        public const uint pio_CAL_VCC_Limit = 0x0C001500;
        public const uint pio_ssim_last_addr = 0x0C001490;
        public const uint pio_VDAC_Mux = 0x0C001480;
        public const uint pio_VDAC_Scale = 0x0C001420;
        public const uint pio_TP_Mux = 0x0C001470;
        public const uint pio_SW_Ena = 0x0C001460;
        public const uint spi_relay = 0x0C001440;
        public const uint spi_DAC = 0x0C001400;

        public const uint pio_start = 0x0C0001F0;
        public const uint pio_pw0 = 0x0C000240;
        public const uint pio_pw1 = 0x0C000440;
        public const uint pio_analog0 = 0x0C000190;
        public const uint pio_analog1 = 0x0C000390;

        //Addresses of channel 1 registers
        public const uint pio_timer0 = 0x0C000250;
        public const uint pio_status0 = 0x0C000220;
        public const uint pio_sens_pol0 = 0x0C000210;
        public const uint pio_clk_div0 = 0x0C000200;
        public const uint pio_start_addr0 = 0x0C0001E0;
        public const uint pio_trig_ang0 = 0x0C0001C0;
        public const uint pio_max0 = 0x0C0001B0;
        public const uint pio_count0 = 0x0C0001A0;
        public const uint pio_clk_sel0 = 0x0C000180;
        public const uint pio_ena0 = 0x0C000170;
        public const uint pio_data_sel0 = 0x0C000160;

        //Addresses of channel 2 registers
        public const uint pio_timer1 = 0x0C000450;
        public const uint pio_status1 = 0x0C000420;
        public const uint pio_sens_pol1 = 0x0C000410;
        public const uint pio_clk_div1 = 0x0C000400;
        public const uint pio_start_addr1 = 0x0C0003E0;
        public const uint pio_trig_ang1 = 0x0C0003C0;
        public const uint pio_max1 = 0x0C0003B0;
        public const uint pio_count1 = 0x0C0003A0;
        public const uint pio_clk_sel1 = 0x0C000380;
        public const uint pio_ena1 = 0x0C000370;
        public const uint pio_data_sel1 = 0x0C000360;

        //Addresses of channel 3 registers
        public const uint pio_timer2 = 0x0C000650;
        public const uint pio_status2 =0x0C000620;
        public const uint pio_sens_pol2 =0x0C000610;
        public const uint pio_clk_div2 =0x0C000600;
        public const uint pio_start_addr2 = 0x0C0005E0;
        public const uint pio_trig_ang2 = 0x0C0005C0;
        public const uint pio_max2 =0x0C0005B0;
        public const uint pio_count2 =0x0C0005A0;
        public const uint pio_clk_sel2 =0x0C000580;
        public const uint pio_ena2 =0x0C000570;
        public const uint pio_data_sel2 = 0x0C000560;

        //Addresses of channel 4 registers
        public const uint pio_timer3 =0x0C000850;
        public const uint pio_status3 =0x0C000820;
        public const uint pio_sens_pol3= 0x0C000810;
        public const uint pio_clk_div3 =0x0C000800 ;
        public const uint pio_start_addr3 = 0x0C0007E0;
        public const uint pio_trig_ang3 =0x0C0007C0;
        public const uint pio_max3 =0x0C0007B0;
        public const uint pio_count3 =0x0C0007A0;
        public const uint pio_clk_sel3 =0x0C000780;
        public const uint pio_ena3 =0x0C000770;
        public const uint pio_data_sel3 = 0x0C000760;

        //

        public const uint pio_ana_delay0 = 0x0C0001D0;
        public const uint pio_ana_delay1 = 0x0C0003D0;
        public const uint pio_ana_delay2 = 0x0C0005D0;
        public const uint pio_ana_delay3 = 0x0C0007D0;

        public const uint pio_dac0 = 0x0C002200;
        public const uint pio_dac1 = 0x0C002210;
        public const uint pio_dac2 = 0x0C002220;
        public const uint pio_dac3 = 0x0C002230;
        public const uint pio_dac4 = 0x0C002240;
        public const uint pio_dac5 = 0x0C002250;
        public const uint pio_dac6 = 0x0C002260;
        public const uint pio_dac7 = 0x0C002270;
        public const uint pio_fpga_version = 0x0C001530;
        public const uint pio_SW_Version = 0x0C001540;
        public const uint sd_wp_n = 0x0C000130;
        public const uint sd_dat = 0x0C000120;
        public const uint sd_cmd = 0x0C000110;
        public const uint sd_clk = 0x0C000100;
        public const uint ir = 0x0C0000F0;
        public const uint lcd = 0x0C0000E0;
        public const uint eep_i2c_sda = 0x0C0000D0;
        public const uint eep_i2c_scl = 0x0C0000C0;
        public const uint i2c_sda = 0x0C0000B0;
        public const uint i2c_scl = 0x0C0000A0;
        public const uint ledr = 0x0C000090;
        public const uint ledg = 0x0C000080;
        public const uint sw = 0x0C000070;
        public const uint key = 0x0C000060;
        public const uint seg7 = 0x0C000040;
        public const uint rs232 = 0x0C000020;
        public const uint timer = 0x0C000000;
        public const uint onchip_memory2 = 0x0A000000;
        public const uint cfi_flash = 0x09000000;
        private const uint uart_USB = 0x08201060;
        private const uint jtag_uart = 0x08201050;
        private const uint pll = 0x08201040;
        public const uint sma_out = 0x08201030;
        public const uint sma_in = 0x08201020;
        public const uint audio = 0x08201000;
        public const uint sdram = 0x00000000;


        // DAC Memory=
        public const uint DAC_Val0 = 0x0C004000;//DAC0 -> Pin 2 Edge detect
        public const uint DAC_Val1 = 0x0C004004;//DAC1 -> Pin 2 pull-up voltage
        public const uint DAC_Val2 = 0x0C004008;//DAC2 -> Pin 3 Edge detect
        public const uint DAC_Val3 = 0x0C00400C;//DAC3 -> Pin 3 pull-up voltage
        public const uint DAC_Val4 = 0x0C004010;//DAC4 -> 2 Wire edge detect
        public const uint DAC_Val5 = 0x0C004014;//DAC5 -> Vcc
        public const uint DAC_Val6 = 0x0C004018;//DAC6 -> Power-On Threshold
        public const uint DAC_Val7 = 0x0C00401C;//DAC7 -> Test
        public const uint Relay_Val = 0x0C004020;//DAC5 -> Relay value
        public const uint SW_Version = 0x0C004024;//DAC5 -> Software Version (firmware)

        //Save the start address of all four channels, updated when pio_start_addr_n is modified
        private static uint Ch1StartAddress = 0x00000000;
        private static uint Ch2StartAddress = 0x02000000;
        private static uint Ch3StartAddress = 0x04000000;
        private static uint Ch4StartAddress = 0x06000000;

        //initialize serial connection, make sure FPGA answers
        public string InitSerial(string strCOM,int iBaudrate)
        {
            string strVersion= "";

            if (serialportBoard.IsOpen)
                serialportBoard.Close();

            serialportBoard.PortName = strCOM;
            serialportBoard.BaudRate = iBaudrate;
            serialportBoard.Handshake = Handshake.RequestToSend;
            serialportBoard.DataBits = 8;
            serialportBoard.ReadTimeout = 1000;
            serialportBoard.DtrEnable = true;

            serialportBoard.Open();
            Thread.Sleep(50);
            serialportBoard.DiscardOutBuffer();
            serialportBoard.DiscardInBuffer();
            Thread.Sleep(50);
            //Read firmware version
            string toWrite = "MRL 0x" + (SW_Version.ToString("X8")) + " 1";
            string strRead = "";
            serialportBoard.WriteLine(toWrite);
            Thread.Sleep(50);
            bool b = false;
            while (b == false) // flush anything that came out at boot-up (after sending first command. answer MUST contain "mrl")
            {
                strRead = serialportBoard.ReadLine();
                if (strRead.Substring(0, 3) == "mrl")
                    b = true;
            }

            strVersion = "SW: " + strRead;
            string str = serialportBoard.ReadExisting();

            //Read Hardware version
            serialportBoard.DiscardOutBuffer();
            serialportBoard.WriteLine("MRL 0x" + pio_fpga_version.ToString("X8") + " 1");
            Thread.Sleep(50);
            strVersion = strVersion + "HW : " + serialportBoard.ReadLine();

            return strVersion;
        }

        public void InitBoard()
        {
            Set_DUT_Vcc(0);
            set_DUT2Pullup_Voltage(0);
            set_DUT3Pullup_Voltage(0);
            set_AllRelaysOFF();
        }

        //Manually close serial port
        public void closeSerial()
        {
            if (serialportBoard.IsOpen)
                serialportBoard.Close();
               
        }

        //Set Vcc voltage 
        public void Set_DUT_Vcc(double dValVcc)//Set Vcc, DUT
        {
            if (dValVcc >= 0 && dValVcc <= 32)
            {
                double dDAC = dValVcc / 7.98;
                double dStep = 6.2501E-05;// (4.096/(2^16-1))
                UInt32 iDacVal = Convert.ToUInt32(Math.Round(dDAC / dStep, 0));
                WriteMemory(DAC_Val5, iDacVal);//Set DAC
            }
            else
            {
                throw new System.ArgumentException("Parameter must be between 0 and 32V", "Vcc value");
            }
            //Readback
        }

        //Read Vcc voltage (-39.9 to 39.9 Volts)  18bits
        public double Read_DUT_Vcc()
        {
            double dVal = 0;

            UInt32 iVal = ReadMemory(pio_AD_7);
            dVal = iVal * 609E-6; // 609E-6 uV/LSB  = > (20/(2^18-1))*7.98
            return dVal;
        }

        //Read digital input :
        public uint Read_DUT_DigitalInput(uint uiCompInput)//0=>Two wire, 1=>power on, 2=>pin 2, 3=>pin 3
        {
            uint uiValComp = 0;

            UInt32 iVal = ReadMemory(pio_digital_in);
            if (uiCompInput == 0) { uiValComp = (uiCompInput & 1); }//returns bit 0
            if (uiCompInput == 1) { uiValComp = (uiCompInput >> 1) & 1; }//returns bit 1
            if (uiCompInput == 2) { uiValComp = (uiCompInput >> 2) & 1; }//returns bit 2
            if (uiCompInput == 3) { uiValComp = (uiCompInput >> 3) & 1; }//returns bit 3
            else { uiValComp = iVal; }//returns all

            return uiValComp;
        }



        //Read Vcc Current (-62.6 to 65.5 mA)  18bits
        public double Read_DUT_IccmA()
        {
            double dVal = 0;

            UInt32 iVal = ReadMemory(pio_AD_8);
            dVal = iVal * 956E-9; // 956E-9 nA/LSB = > (20/(2^18-1))/79.9
            return dVal*1000;
        }


        //Read Pin 2 voltage (-5 to 5 Volts) 18bits
        public double Read_Pin2()
        {
            double dVal = 0;

            UInt32 iVal = ReadMemory(pio_AD_2);
            if(iVal>Math.Pow(2,31)-1)
            {
                iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                dVal = -iVal * 76.3E-6; // 76.3E-6 uV/LSB  = > 20/(2^18-1)
            }
            else
            {
                dVal = iVal * 76.3E-6; // 76.3E-6 uV/LSB  = > 20/(2^18-1)   
            } 

            return dVal;
        }

        //Read Pin 3 voltage (-5 to 5 Volts) 18bits
        public double Read_Pin3()
        {
            double dVal = 0;

            UInt32 iVal = ReadMemory(pio_AD_6);
            if (iVal > Math.Pow(2, 31) - 1)
            {
                iVal = (UInt32)Math.Pow(2, 32) - 1 - iVal;
                dVal = -iVal * 76.3E-6; // 76.3E-6 uV/LSB  = > 20/(2^18-1)
            }
            else
            {
                dVal = iVal * 76.3E-6; // 76.3E-6 uV/LSB  = > 20/(2^18-1)   
            }

            return dVal;
        }

        //Read Pin 2 Pull-up voltage (-39.9 to 39.9 Volts) 18bits
        public double Read_Pin2_PullUp()
        {
            double dVal = 0;

            UInt32 iVal = ReadMemory(pio_AD_1);
            dVal = iVal * 609E-6; // 609E-6 uV/LSB  = > (20/(2^18-1))*7.98
            return dVal;
        }

        //Read Pin 3 Pull-up voltage (-39.9 to 39.9 Volts) 18bits
        public double Read_Pin3_PullUp()
        {
            double dVal = 0;

            UInt32 iVal = ReadMemory(pio_AD_5);
            dVal = iVal * 609E-6; // 609E-6 uV/LSB  = > (20/(2^18-1))*7.98
            return dVal;
        }

        //Set threshold for the 2-wire edge detector
        public void set_2Wire_Current(double dcurrent)
        {

            if (dcurrent >= 0 && dcurrent <= 0.05)
            {
                double dDAC = dcurrent; //voltage on DAC = 10 times desired current (due to 10 Ohms sense)
                //dDAC = (dDAC - 0.005 / 0.999); // first compute hysteresis to subtract out V = x+((5-x)*.001) ??????
                double dStep = 6.2501E-05/79.8;// (4.096/(2^16-1))/79.8
                UInt32 iDacVal = Convert.ToUInt32(Math.Round(dDAC /dStep, 0));
                WriteMemory(DAC_Val4, iDacVal);//Set DAC
                
             
            }
            else
            {
                throw new System.ArgumentException("Parameter must be between 0 and 50mA", "2 wire edge detector threshold");
            }

        }

        //Get current threshold for 2 wire.  
        public double Get_PONIccThresh_Voltage()
        {
            double dValThresh = 0;
            uint iVal = ReadMemory(DAC_Val4);//Iccthreshold
            dValThresh = (iVal * (6.2501E-05/79.8))*1000;
            return Math.Round(dValThresh,3);
        }

        //Set voltage threshold for DUT pin 2 edge detectors.  
        public void set_DUT2Thresh_Voltage(double voltage)
        {
            
            if (voltage >= 0 && voltage <= 16)
            {
                double scale_factor = (65536.0 / 4.096) / 4.0;
                double offset = 0;
                double value;
                // Calculate actual comparator threshold voltage desired
                value = voltage * 1.0;
                // if V = desired comparator voltage, solve for x, the threshold voltage
                // that needs to be set to nullify hysteresis:  V = x+((5-x)*.01477)
                // x+(5*.01477)-(x*.01477) = V = x+(.07385) - .01477x  || .98523x + .07385 = V
                // (V-.07385) = .98523x  || (V-.07385)/.98523 = x
                double x = (value - .07385) / 0.98523;
                voltage = x;
                value = voltage * scale_factor + offset;
                uint uiI = (uint)value;

                // Set DAC 5 to voltage
                WriteMemory(DAC_Val0, uiI);
            }
            else
            {
                throw new System.ArgumentException("Parameter must be between 0 and 16V", "Pin 2 Edge detector threshold");
            }
        }

        //Get voltage threshold for DUT pin 2 edge detectors.  
        public double Get_DUT2Thresh_Voltage()
        {
            double dValThresh = 0;
            ReadMemory(DAC_Val0);//pin2threshold
            return dValThresh;
        }

        //Get voltage threshold for DUT pin 3 edge detectors.  
        public double Get_DUT3Thresh_Voltage()
        {
            double dValThresh = 0;
            ReadMemory(DAC_Val2);//pin3 threshold


            return dValThresh;
        }

        //Set voltage threshold for DUT pin 3 edge detectors. 
        public void set_DUT3Thresh_Voltage(double voltage)
        {
            if (voltage >= 0 && voltage <= 16)
            {
                double scale_factor = (65536.0 / 4.096) / 4.0;
                double offset = 0;
                double value;
                value = voltage * 1.0;
                double x = (value - .07385) / 0.98523;
                voltage = x;
                value = voltage * scale_factor + offset;
                uint uiI = (uint)value;

                // Set DAC 5 to voltage
                WriteMemory(DAC_Val2, uiI);
            }
            else
            {
                throw new System.ArgumentException("Parameter must be between 0 and 16V", "Pin 3 Edge detector threshold");
            }
        }

        //Set Pull-up voltage for DUT pin 2
        public void set_DUT2Pullup_Voltage(double voltage)
        {
            if (voltage >= 0 && voltage <= 30)
            {
                double scale_factor = (65536.0 / 4.096) / 7.98;
                double offset = 0;
                double value = voltage * scale_factor + offset;
                uint uiI = (uint)value;

                // Set DAC 1 to voltage
                WriteMemory(DAC_Val1, uiI);
            }
            else
            {
                throw new System.ArgumentException("Parameter must be between 0 and 30V", "Pin 2 Pull-up voltage");
            }
        }

        //Set Pull-up voltage for DUT pin 3
        public void set_DUT3Pullup_Voltage(double voltage)
        {
            if (voltage >= 0 && voltage <= 30)
            {
                double scale_factor = (65536.0 / 4.096) / 7.98;
                double offset = 0;
                double value = voltage * scale_factor + offset;
                uint uiI = (uint)value;

                // Set DAC 3 to voltage
                WriteMemory(DAC_Val3, uiI);
            }
            else
            {
                throw new System.ArgumentException("Parameter must be between 0 and 30V", "Pin 3 Pull-up voltage");
            }
        }

        //Set power-on voltage threshold for VCC
        public void set_PONThresh_Voltage(double voltage)
        {
            if (voltage >= 0 && voltage <= 32)
            {
                double scale_factor = (65536.0 / 4.096) / 7.98;
                double offset = 0;
                double value = voltage * scale_factor + offset;
                uint uiI = (uint)value;

                // Set DAC to voltage
                WriteMemory(DAC_Val6, uiI);
            }
            else
            {
                throw new System.ArgumentException("Parameter must be between 0 and 32V", "power-on voltage threshold");
            }
        }

        //Get current threshold for Vcc.  
        public double Get_PONVccThresh_Voltage()
        {
            double dValThresh = 0;
            uint iVal = ReadMemory(DAC_Val6);//Vccthreshold
            dValThresh = iVal*498.75E-6;
            return dValThresh;
        }

        //Set all relay to OFF state (no matter if they are nc or no)
        public void set_AllRelaysOFF()
        {
            uint uiI = 0x0; //all relay off + vcc set to external
            WriteMemory(RELAY_Address, uiI);
            Thread.Sleep(100);//takes time to relay to switch off/on
        }

        //Read all relay values
        public UInt32 Read_Relays()
        {
            UInt32 uiVal =  ReadMemory(RELAY_Address);
            return uiVal;
        }

       //Write relay register
        public void Write_Relays(uint uiVal)
        {
            WriteMemory(RELAY_Address, uiVal);
            Thread.Sleep(100);//takes time to relay to switch off/on
        }

        //Write relay register
        public UInt32 Get_ppr()
        {
            UInt32 uiVal = ReadMemory(pio_encoder_pprm1);
            return uiVal;
        }

        //Write relay register
        public void Set_ppr(uint uiVal)
        {
            if (uiVal <= 65536 && uiVal >= 2500)
            {
                uiVal = uiVal - 1;
                WriteMemory(pio_encoder_pprm1, uiVal);
            }
        }

      /*
        public void set_2wire_test_mode(ref System.IO.Ports.SerialPort SP)
        {
            uint uiI = 0;
            // Read Relay Settings
            read(ref SP, SSmem.Relay_Val, ref uiI);
            // enable VCC Relay
            write(ref SP, SSmem.Relay_Val, uiI | SSmem.RELAY_VCC_MASK);
            // Set VCC to 5V
            set_VCC_Voltage(10.0);
            // Set Current Sense threshold to 4mA
            set_2Wire_Current(0.004);
            // Read Debug Settings
            read(ref SP, SSmem.pio_debug, ref uiI);
            // Turn on CAL resistor toggle
            write(ref SP, SSmem.pio_debug, uiI | SSmem.DEBUG_BIT_2WIRE | SSmem.DEBUG_BIT_SENS_EM);
        }
        */

     /*   public void set_3wire_test_mode(ref System.IO.Ports.SerialPort SP)
        {
            uint uiI = 0;
            // Read Relay Settings
            read(ref SP, SSmem.Relay_Val, ref uiI);
            // Enable input pin connections
            write(ref SP, SSmem.Relay_Val, uiI | SSmem.RELAY_DIG_MASK | SSmem.RELAY_ANA_MASK);
            // Set threshold voltage
            set_DUT2Thresh_Voltage(1.5);
            set_DUT3Thresh_Voltage(1.5);
            // Read Debug Settings
            read(ref SP, SSmem.pio_debug, ref uiI);
            // Turn on Sensor emulation
            write(ref SP, SSmem.pio_debug,     uiI | SSmem.DEBUG_BIT_SENS_EM);
        }
*/
      
        //Write command : 
        private void WriteMemory(uint uiAddr, uint data)
        {
            string strToWrite;
            strToWrite = "MWL 0x";//use always 32bits for now
            strToWrite = strToWrite + uiAddr.ToString("X8");
            strToWrite = strToWrite + " 0x" + data.ToString("X8");
            serialportBoard.DiscardInBuffer();
            serialportBoard.DiscardOutBuffer();
            serialportBoard.WriteLine(strToWrite);
            Thread.Sleep(20);
        }

        //Read given memory address and return its 32bits decimal value
        public uint ReadMemory(UInt32 uiAddr)
        {
            string strToWrite = "MRL 0x" + uiAddr.ToString("X8") + " 1";//use always 32bits for now
            uint uiRead;
            string strRead = ""; 

            serialportBoard.DiscardInBuffer();
            serialportBoard.DiscardOutBuffer();

            serialportBoard.WriteLine(strToWrite);
            Thread.Sleep(20);
            strRead = serialportBoard.ReadLine();

            if (strRead.Contains("mrl") && strRead.Contains("0x"))
            {
                string str1 = strRead.Remove(0, strRead.IndexOf("x") + 1);
                strRead = str1.Remove(8);
            }
            Thread.Sleep(10);
            uiRead = Convert.ToUInt32(strRead, 16);

            return uiRead;
        }

        //Write dataselection register, used to set which data item is going to be captured for the current data capture cycle.
        public uint DataSelection(uint iID, uint iVal, bool bWrite)
        {
            uint uiRead = 0;
            uint Address=0; 
            if(iID==0)
               Address= pio_data_sel0;
            else if(iID==1)
               Address= pio_data_sel1;
            else if(iID==2)
                Address= pio_data_sel2;
            else if(iID==3)
               Address= pio_data_sel3;

            if (Address != 0)
            {
                if (bWrite && (iVal >= 0 && iVal <= 255))
                {
                    WriteMemory(Address, iVal);
                }
                uiRead = ReadMemory(Address);
            }
            /*
            Data to capture
					0: angle (quadrature)
					1: sensor0 analog
					2: sensor0 pulsewidth
					3: sensor0 period
					4: gear timer
					5: angle (single phase-timed)
					6 to 16: RESERVED
					17: sensor1 analog
					18: sensor1 pulsewidth
					19: sensor1 period
					20 to 32: RESERVED
					33: sensor2 analog (2-wire)
					34: sensor2 pulsewidth
					35: sensor2 period
					36 to 255: RESERVED
                */
            return uiRead;
        }

        //Write TriggerSelection "pio_ena" register, set which trigger is going to be used to start the capture cycle.
        public uint TriggerSelection(uint iID, uint iVal, bool bWrite)
        {
            uint uiRead = 0;

            uint Address = 0;
            if (iID == 0)
                Address = pio_ena0;
            else if (iID == 1)
                Address = pio_ena1;
            else if(iID==2)
                Address= pio_ena2;
            else if(iID==3)
                 Address= pio_ena3;

            if (Address != 0)
            {
                if (bWrite && (iVal >= 0 && iVal <= 255))
                {
                    WriteMemory(Address, iVal);
                }
                uiRead= ReadMemory(Address);
            }
            /*		0: ALWAYS TRIGGER
					1: angle
					2: POR
            */

            return uiRead;
        }

        //Write clockSelection "pio_clk_sel" register, set which clock is going to be used to capture data during the current capture cycle
        public uint ClockSelection(uint iID, uint iVal, bool bWrite)
        {
            uint uiRead = 0;
            uint Address = 0;
            if (iID == 0)
                Address = pio_clk_sel0;
            else if (iID == 1)
                Address = pio_clk_sel1;
            else if(iID==2)
                  Address= pio_clk_sel2;
            else if(iID==3)
                 Address= pio_clk_sel3;

            if (Address != 0)
            {
                if (bWrite && (iVal >= 0 && iVal <= 255))
                {
                    WriteMemory(Address, iVal);
                }
                uiRead = ReadMemory(Address);
            }
                    /*
                 	0: clock divider
					1: sensor0 rise
					2: sensor0 fall
					3: sensor0 rise or fall
					4: Encoder I input, single edge
					5: sensor1 rise
					6: sensor1 fall
					7: sensor1 rise or fall
					8: NOT USED
					9: sensor2 rise
					10: sensor2 fall
					11: sensor2 rise or fall
					12: Single-edge Decoder
					13: Encoder B input, both edges
					14: Encoder A input, both edges
					15: Quadrature Decoder, all edges*/

            return uiRead;
        }

        //Read/Write Trigger angle "pio_trig_ang" register, set the angle at which the trigger angle pulse will be generated
        public uint TriggerAngleSelection(uint iID, uint iVal, bool bWrite)
        {
            uint uiRead=0;
            uint Address = 0;

            if (iID == 0)
                Address = pio_trig_ang0;
            else if (iID == 1)
                Address = pio_trig_ang1;
            else if(iID==2)
                  Address= pio_trig_ang2;
            else if(iID==3)
                 Address= pio_trig_ang3;
           
            if (Address != 0) 
            {
                if (bWrite && (iVal >= 0 && iVal <= 0xFFFFFFFF))
                {
                    WriteMemory(Address, iVal);
                }
                uiRead = ReadMemory(Address);
            }
            /*		
                    bit 0 to 15 => Trig angle lower
                    bit 16 to 31 => Trig angle upper
             
                    Angle that generates trigger pulse
					LSB is encoder dependent.
					Normally 360/(PPR*64) (quadrature + interpolation = 4*16 =64)

            */

            return uiRead;
        }

        //Write/read Sensor polarity "pio_sens_pol" register, set polarity 
        public uint Polarity(uint iID, uint iVal, bool bWrite)
        {
            uint uiRead = 0;
            uint Address = 0;
            if (iID == 0)
                Address = pio_sens_pol0;
            else if (iID == 1)
                Address = pio_sens_pol1;
            else if(iID==2)
                Address= pio_sens_pol2;
            else if(iID==3)
                 Address= pio_sens_pol3;

            if (Address != 0)
            {
                if (bWrite && (iVal >= 0 && iVal <= 2))
                {
                    WriteMemory(Address, iVal);
                }
                uiRead = ReadMemory(Address);
            }
            /*
            Polarity of digital signal
					0 => Normal
					1 => Inverted
            */

            return uiRead;
        }

        //Write/read DataCaptureCycle "pio_start" register  used to start or stop the data capture cycle
        public uint StartDataCaptureCycle(uint iValBinary, bool bWrite)
        {
            uint uiRead = 0;
           
            if (bWrite && (iValBinary >= 0 && iValBinary <= 15))
            {
                WriteMemory(pio_start, iValBinary);
            }
            uiRead = ReadMemory(pio_start);
            /*
               Bit
                0	START0	R/W		Start or stop capture cycle on channel 0
					                0 => Stop
					                1 => Start
                1	START1	R/W		Start or stop capture cycle on channel 1
                2	START2	R/W		Start or stop capture cycle on channel 2
                3	START3	R/W		Start or stop capture cycle on channel 3

            */

            return uiRead;
        }

        //Get Data Capture Cycle Status "pio_status" register  used to onserve the status of the data capture cycle
        public uint DataCaptureCycleStatus(uint iID)
        {
            uint uiRead = 0;
            uint Address = 0;
            if (iID == 0)
                Address = pio_status0;
            else if (iID == 1)
                Address = pio_status1;
            else if(iID==2)
                 Address= pio_status2;
            else if(iID==3)
                 Address= pio_status3;

            if (Address != 0)
            {
                uiRead = ReadMemory(Address);
            }
            /*
                    0	started	R		Equivalent to the corresponding Start bit in the Start Data Capture Cycle Register
					                    0 => not started
					                    1 => started
                    1	running	R		FIFO enable Status
					                    0 => Trigger has not occurred
					                    1 => Trigger has occurred, FIFO Enabled
                    2	complete	R		Data Acquisition Complete
					                    0 => Not started or not complete
					                    1 => Max data count reached
                    3	FIFO Error	R		Internal FIFO Overflow
					                    0 => Overflow has not occurred
					                    1 => Overflow has occurred
            */

            return uiRead;
        }

        //Write/read Encoder Speed "pio_speed", contains the current encoder speed. Only writeable in debug mode.  
        public uint EncoderSpeedCount(uint iVal, bool bWrite)
        {
            uint uiRead = 0;

            if (bWrite && (iVal >= 0 && iVal <= 0xFFFFFFFF))
            {
                WriteMemory(pio_speed, iVal);
            }
            uiRead = ReadMemory(pio_speed);
            /*
                31:0	SPEED	R/W		Current Encoder Speed LSB is counts per 1/16 second
            */

            return uiRead;
        }

        //Read/Write Pulse width "pio_pw" register
        public uint Pulsewidth(uint iID, uint iVal, bool bWrite)
        {
            uint uiRead = 0;
            uint Address = 0;
            if (iID == 0)
                Address = pio_pw0;
            else if (iID == 1)
                Address = pio_pw1;

            if (Address != 0)
            {
                if (bWrite && (iVal >= 0 && iVal <= 0xFFFFFFFF))
                {
                    WriteMemory(Address, iVal);
                }
                uiRead = ReadMemory(Address);
            }
            /*
                  31:0	PULSEWIDTH	R	Pulsewidth in System Clocks LSB is 1/100M
                  31 to 16 => upper 
                  15 to 0 => Lower

            */

            return uiRead;
        }

        //Read/Write DataStartAddress, set the base address in SDRAM for each channels
        public uint DataStartAddress(uint iID, uint iVal, bool bWrite)
        {
            uint uiRead = 0;
            uint Address = 0;
            if (iID == 0)
            {
                Address = pio_start_addr0;
                iVal += Ch1StartAddress;
                if (iVal>= Ch2StartAddress)
                {
                    throw new Exception("Warning an overflow as occured on the channel 1 memory, the number of items available do not correspond to the number of items wanted.");
                }
            }
            else if (iID == 1)
            {
                Address = pio_start_addr1;
                iVal += Ch2StartAddress;
                if (iVal >= Ch3StartAddress)
                {
                    throw new Exception("Warning an overflow as occured on the channel 1 memory, the number of items available do not correspond to the number of items wanted.");
                }
            }
            else if (iID == 2)
            {
                Address = pio_start_addr2;
                iVal += Ch3StartAddress;
                if (iVal >= Ch4StartAddress)
                {
                    throw new Exception("Warning an overflow as occured on the channel 1 memory, the number of items available do not correspond to the number of items wanted.");
                }
            }
            else if (iID == 3)
            {
                Address = pio_start_addr3;
                iVal += Ch4StartAddress;
                if (iVal > 0x07FFFFFC)
                {
                    throw new Exception("Warning an overflow as occured on the channel 1 memory, the number of items available do not correspond to the number of items wanted.");
                }
            }
            if (Address != 0)
            {
               
                if (bWrite && (iVal >= 0 && iVal <= 0x07FFFFFC))
                {
                    WriteMemory(Address, iVal);
                }

                uiRead = ReadMemory(Address);
            }
            /*
                  START_ADDR	R/W		DATA Start Address in SDRAM
					Range: 0x00000000 to 0x07FFFFFC
                    if using 4 channels, start addresses : 0x00000000, 0x02000000, 0x04000000, 0x0600000 => 33554432 items per channel

            */

            return uiRead;
        }

        //Set start address for 4 channels (default)
        public void DataStartAddressDefault()
        {
            WriteMemory(pio_start_addr0, Ch1StartAddress);
            Thread.Sleep(10);
            WriteMemory(pio_start_addr1, Ch2StartAddress);
            Thread.Sleep(10);
            WriteMemory(pio_start_addr2, Ch3StartAddress);
            Thread.Sleep(10);
            WriteMemory(pio_start_addr3, Ch4StartAddress);
            Thread.Sleep(10);
            /*
                  START_ADDR	R/W		DATA Start Address in SDRAM
					Range: 0x00000000 to 0x07FFFFFC                    
                    if using 4 channels, start addresses : 0x00000000, 0x02000000, 0x04000000, 0x0600000 => 33554432 items per channel
            */
        }

        //Read/Write "pio_max" DataMaximumCount is used to set maximum number of data items to be stored in SDRAM.  
        public uint DataMaximumCount(uint iID, uint iVal, bool bWrite)
        {
            uint uiRead = 0;
            uint Address = 0;
            if (iID == 0)
                Address = pio_max0;
            else if (iID == 1)
                Address = pio_max1;
            else if(iID==2)
                Address = pio_max2;
            else if(iID==3)
                Address = pio_max3;

            if (Address != 0)
            {
                if (bWrite && (iVal >= 0 && iVal <= 0x02000000))
                {
                    WriteMemory(Address, iVal);
                }
                uiRead = ReadMemory(Address);
            }
            /*
                    Maximum number of data words to capture
					Range: 0x00000000 to 0x02000000

            */

            return uiRead;
        }

        //Get the number of data items currently stored in SDRAM
        public uint GetNumberOfSweeps(uint iID)
        {
            uint uiRead = 0;
            uint Address = 0;
            if (iID == 0)
                Address = pio_count0;
            else if (iID == 1)
                Address = pio_count1;
            else if(iID==2)
                 Address= pio_count2;
            else if(iID==3)
                 Address= pio_count3;

            if (Address != 0)
            {
                uiRead = ReadMemory(Address);
            }
            /*
    `               Number of data words captured
					Range: 0x00000000 to 0x02000000
            */

            return uiRead;
        }

        //Read/Write Clock divider "pio_clk_div" used for acquisition
        public uint ClockDivider(uint iID, uint iVal, bool bWrite)
        {
            uint uiRead = 0;
            uint Address = 0;
            if (iID == 0)
                Address = pio_clk_div0;
            else if (iID == 1)
                Address = pio_clk_div1;
             else if(iID==2)
                Address = pio_clk_div2;
              else if(iID==3)
                Address = pio_clk_div3;

            //Pio_clk_div is the number of 10ns periods (+1) between samples: 0=10ns, 1=20ns, 2=30ns.
            
            if (Address != 0)
            {
                if (bWrite && (iVal >= 0 && iVal <= 65535))
                {
                    WriteMemory(Address, iVal);
                }
                uiRead = ReadMemory(Address);
            }

            return uiRead;
        }

        //Read given memory address and return its 32bits decimal value
        public string ReadFIFO(UInt32 uiAddr)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();

            string strToWrite = "FRL " + uiAddr.ToString() + " 2";//use always 32bits for now
            //uint uiRead;
            string strRead = ""; ;

            serialportBoard.DiscardInBuffer();
            serialportBoard.DiscardOutBuffer();

            serialportBoard.WriteLine(strToWrite);
            Thread.Sleep(20);
            strRead = serialportBoard.ReadLine();
            
            stopwatch.Stop();
            long dR = stopwatch.ElapsedMilliseconds;

            if (strRead.Contains("frl") && strRead.Contains("ERROR")==false)
            {
                strRead = strRead.Remove(0,strRead.IndexOf("0x"));
                strRead = strRead.Remove(strRead.IndexOf('\r'));

                strRead = strRead.Split(' ')[0];

                strRead = Convert.ToInt32(strRead, 16).ToString();

                if (strRead.Contains('\0')) { strRead = strRead.Remove(strRead.IndexOf('\0') - 1); }
            }

            return strRead;
        }

        //Read given memory address and return its 32bits decimal value
        public string ReadMemory(uint ChannelToRead,UInt32 uiStart, UInt32 uiNumberToRead)
        {
            uiStart = uiStart * 4; // Start address needs to be multiply per 4 as we read 4 bytes anytime 4x8=32 bit words
            string strRead = ""; 
            UInt32 uiStartAddr = 0;
            bool bOk = false;
            if (ChannelToRead == 0)
            {
                uiStartAddr = Ch1StartAddress + uiStart;
                bOk = true;
            }
            else if (ChannelToRead == 1)
            {
                uiStartAddr = Ch2StartAddress + uiStart;
                bOk = true;
            }
            else if (ChannelToRead == 2)
            {
                uiStartAddr = Ch3StartAddress + uiStart;
                bOk = true;
            }
            else if (ChannelToRead == 3)
            {
                uiStartAddr = Ch4StartAddress + uiStart;
                bOk = true;
            }

            if (bOk)//do not perfrom the action if channel specified is not correct
            {
                string strToWrite = "MRL 0x" + uiStartAddr.ToString("X8") + " "+uiNumberToRead.ToString();//use always 32bits for now
                //uint uiRead;

                serialportBoard.DiscardInBuffer();
                serialportBoard.DiscardOutBuffer();

                serialportBoard.WriteLine(strToWrite);
                Thread.Sleep(20);
                strRead = serialportBoard.ReadLine();

                //check reading
                if (strRead.ToLower().Contains("mrl"))
                {
                    strRead = strRead.Remove(0, strRead.IndexOf("0x"));
                    strRead = strRead.Remove(strRead.IndexOf('\r'));
                    if (strRead.Contains('\0'))
                        strRead = strRead.Remove(strRead.IndexOf('\0') - 1);

                    string[] strReadArray = strRead.Split(' ');//put all values into array
                    int iSize = strReadArray.Length;
                    if (iSize != uiNumberToRead)
                    {
                        throw new Exception("Warning an overflow as occured on the serial port, the number of items read do not correspond to the number of items wanted.");
                    }
                }
                else
                {
                    throw new Exception("Error, the board did not answer into the correct format: \n\n" + strRead);
                }
            }
            return strRead;
        }

    }
}
