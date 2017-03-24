using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPGA_AcquisitionBoard
{
    class GlobalVariables
    {
        // Global variables related to Acquisition board
        public static UInt32 RelaysVal
        {
            get
            {
                return uiRelaysReg;
            }
            set
            {
                uiRelaysReg = value;
            }
        }
        public static UInt32 EncocderPPR//Set Current number or pulse per revolution
        {
            get
            {
                return uiEncocderPPR;
            }
            set
            {
                uiEncocderPPR = value;
            }
        }
        public static string FileDirectory//Set Current file directory
        {
            get
            {
                return strFileDirectory;
            }
            set
            {
                strFileDirectory = value;
            }
        }
        public static bool AcquisitionBoardConnected
        {
            get
            {
                return bAcquisitionBoardConnected;
            }
            set
            {
                bAcquisitionBoardConnected = value;
            }
        }
        public static int AcquisitionBoard_BaudRate
        {
            get
            {
                return iAcquisitionBoard_BaudRate;
            }
            set
            {
                iAcquisitionBoard_BaudRate = value;
            }
        }
        public static int AcquisitionBoard_Port
        {
            get
            {
                return iAcquisitionBoard_Port;
            }
            set
            {
                iAcquisitionBoard_Port = value;
            }
        }

        public static bool debugMode
        {
            get
            {
                return bdebugMode;
            }
            set
            {
                bdebugMode = value;
            }
        }

        //represents the current status of each measurement channel
        public static uint[] AcquisitionStatus
        {
            get
            {
                return uiAcquisitionStatus;
            }
            set
            {
                uiAcquisitionStatus = value;
            }
        }


     
        private static bool bdebugMode = false;
        //variables related to Motor controls 


        //variables related to Acquisition board
        private static bool bAcquisitionBoardConnected=false;
        private static UInt32 uiRelaysReg;
        private static UInt32 uiEncocderPPR = (uint)WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.EncoderPPR;
        private static string strFileDirectory = "";
        private static int iAcquisitionBoard_Port = WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.AcquisitionPort;
        private static int iAcquisitionBoard_BaudRate = WindowsFormsControlLibrary_FPGA.Properties.Settings.Default.AcquisitionComSpeed;

        private static uint[] uiAcquisitionStatus = new uint[4];//represents the current status of each measurement channel


    }
}
