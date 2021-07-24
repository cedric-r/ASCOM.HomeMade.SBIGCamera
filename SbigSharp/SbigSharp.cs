using System;
using System.Net;
using System.Runtime.InteropServices;

namespace SbigSharp
{
    /// <summary>
    /// This supports the following devices:
    /// * ST-5C/237/237A (PixCel255/237)
    /// * ST-7E/8E/9E/10E
    /// * ST-1K, ST-2K, ST-4K
    /// * STL Large Format Cameras
    /// * ST-402 Family of Cameras
    /// * ST-8300 Cameras
    /// * STF-8300, 8050 Cameras
    /// * STT Cameras
    /// * STX/STXL Cameras
    /// * ST-i Cameras
    /// * AO-7, AOL, AO-8
    /// * CFW-8, CFW-9, CFW-10, CFW-L
    /// * FW5-8300, FW8-8300
    /// * ST Focuser
    /// * Differential Guider Accessory (Preliminary)
    /// 
    /// Enumerated Constants
    ///     Note that the various constants are declared here as enums
    ///     for ease of declaration but in the structures that use the
    ///     enums unsigned shorts are used to force the various
    ///     16 and 32 bit compilers to use 16 bits.
    /// 
    /// </summary>
    public static class SBIG
    {
        #region SBIG C language header file "sbigudrv.h"

        #region SBIG enum

        // Supported Camera Commands
        // ===========================
        // These are the commands supported by the driver.
        // They are prefixed by CC_ to designate them as camera commands 
        //  and avoid conflicts with other enums.
        //
        // Some of the commands are marked as SBIG use only 
        //  and have been included to enhance testability of the driver for SBIG.

        /// <summary>
        /// Command ID enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum PAR_COMMAND : UInt16
        {
            /// <summary>
            /// Null Command
            /// </summary>
            CC_NULL,

            #region 1 - 10
            /// <summary>
            /// Start exposure command
            /// </summary>
            CC_START_EXPOSURE = 1,
            /// <summary>
            /// End exposure command
            /// </summary>
            CC_END_EXPOSURE,
            /// <summary>
            /// Readout line command
            /// </summary>
            CC_READOUT_LINE,
            /// <summary>
            /// Dump lines command
            /// </summary>
            CC_DUMP_LINES,
            /// <summary>
            /// Set Temperature regulation command
            /// </summary>
            CC_SET_TEMPERATURE_REGULATION,
            /// <summary>
            /// Query temperature status command
            /// </summary>
            CC_QUERY_TEMPERATURE_STATUS,
            /// <summary>
            /// Activate Relay command
            /// </summary>
            CC_ACTIVATE_RELAY,
            /// <summary>
            /// Pulse out command
            /// </summary>
            CC_PULSE_OUT,
            /// <summary>
            /// Establish link command
            /// </summary>
            CC_ESTABLISH_LINK,
            /// <summary>
            /// Get driver info command
            /// </summary>
            CC_GET_DRIVER_INFO,
            #endregion // 1 - 10 

            #region 11 - 20
            /// <summary>
            /// Get CCD info command
            /// </summary>
            CC_GET_CCD_INFO,
            /// <summary>
            /// Query command status command
            /// </summary>
            CC_QUERY_COMMAND_STATUS,
            /// <summary>
            /// Miscellaneous control command
            /// </summary>
            CC_MISCELLANEOUS_CONTROL,
            /// <summary>
            /// Read subtract line command
            /// </summary>
            CC_READ_SUBTRACT_LINE,
            /// <summary>
            /// Update clock command
            /// </summary>
            CC_UPDATE_CLOCK,
            /// <summary>
            /// Read offset command
            /// </summary>
            CC_READ_OFFSET,
            /// <summary>
            /// Open driver command
            /// </summary>
            CC_OPEN_DRIVER,
            /// <summary>
            /// Close driver command
            /// </summary>
            CC_CLOSE_DRIVER,
            /// <summary>
            /// TX Serial bytes command
            /// </summary>
            CC_TX_SERIAL_BYTES,
            /// <summary>
            /// Get serial status command
            /// </summary>
            CC_GET_SERIAL_STATUS,
            #endregion // 11 - 20 

            #region 21 - 30
            /// <summary>
            /// AO tip/tilt command
            /// </summary>
            CC_AO_TIP_TILT,

            /// <summary>
            /// AO set focus command
            /// </summary>
            CC_AO_SET_FOCUS,
            /// <summary>
            /// AO delay command
            /// </summary>
            CC_AO_DELAY,
            /// <summary>
            /// Get turbo status command
            /// </summary>
            CC_GET_TURBO_STATUS,
            /// <summary>
            /// End readout command
            /// </summary>
            CC_END_READOUT,
            /// <summary>
            /// Get US timer command
            /// </summary>
            CC_GET_US_TIMER,
            /// <summary>
            /// Open device command
            /// </summary>
            CC_OPEN_DEVICE,
            /// <summary>
            /// Close device command
            /// </summary>
            CC_CLOSE_DEVICE,
            /// <summary>
            /// Set IRQL command
            /// </summary>
            CC_SET_IRQL,
            /// <summary>
            /// Get IRQL command
            /// </summary>
            CC_GET_IRQL,
            #endregion // 21 - 30

            #region // 31 - 40
            /// <summary>
            /// Get line command
            /// </summary>
            CC_GET_LINE,
            /// <summary>
            /// Get link status command
            /// </summary>
            CC_GET_LINK_STATUS,
            /// <summary>
            /// Get driver handle command
            /// </summary>
            CC_GET_DRIVER_HANDLE,
            /// <summary>
            /// Set driver handle command
            /// </summary>
            CC_SET_DRIVER_HANDLE,
            /// <summary>
            /// Start readout command
            /// </summary>
            CC_START_READOUT,
            /// <summary>
            /// Get error string command
            /// </summary>
            CC_GET_ERROR_STRING,
            /// <summary>
            /// Set driver control command
            /// </summary>
            CC_SET_DRIVER_CONTROL,
            /// <summary>
            /// Get driver control command
            /// </summary>
            CC_GET_DRIVER_CONTROL,
            /// <summary>
            /// USB A/D control command
            /// </summary>
            CC_USB_AD_CONTROL,
            /// <summary>
            /// Query USB command
            /// </summary>
            CC_QUERY_USB,
            #endregion // 31 - 40

            #region 41 - 50
            /// <summary>
            /// Get Pentium cycle count command
            /// </summary>
            CC_GET_PENTIUM_CYCLE_COUNT,
            /// <summary>
            /// Read/Write USB I2C command
            /// </summary>
            CC_RW_USB_I2C,
            /// <summary>
            /// Control Filter Wheel command
            /// </summary>
            CC_CFW,
            /// <summary>
            /// Bit I/O command
            /// </summary>
            CC_BIT_IO,
            /// <summary>
            /// User EEPROM command
            /// </summary>
            CC_USER_EEPROM,
            /// <summary>
            /// AO Center command
            /// </summary>
            CC_AO_CENTER,
            /// <summary>
            /// BTDI setup command
            /// </summary>
            CC_BTDI_SETUP,
            /// <summary>
            /// Motor focus command
            /// </summary>
            CC_MOTOR_FOCUS,
            /// <summary>
            /// Query Ethernet command
            /// </summary>
            CC_QUERY_ETHERNET,
            /// <summary>
            /// Start Exposure command v2
            /// </summary>
            CC_START_EXPOSURE2,
            #endregion // 41 - 50

            #region 51 - 60
            /// <summary>
            /// Set Temperature regulation command
            /// </summary>
            CC_SET_TEMPERATURE_REGULATION2,
            /// <summary>
            /// Read offset command v2
            /// </summary>
            CC_READ_OFFSET2,
            /// <summary>
            /// Differential Guider command
            /// </summary>
            CC_DIFF_GUIDER,
            /// <summary>
            /// Column EEPROM command
            /// </summary>
            CC_COLUMN_EEPROM,
            /// <summary>
            /// Customer Options command
            /// </summary>
            CC_CUSTOMER_OPTIONS,
            /// <summary>
            /// Debug log command
            /// </summary>
            CC_DEBUG_LOG,
            /// <summary>
            /// Query USB command v2
            /// </summary>
            CC_QUERY_USB2,
            /// <summary>
            /// Query Ethernet command v2
            /// </summary>
            CC_QUERY_ETHERNET2,
            /// <summary>
            /// Get AO model command
            /// </summary>
            CC_GET_AO_MODEL,
            /// <summary>
            /// Query up to 24 USB cameras
            /// </summary>
            CC_QUERY_USB3,
            /// <summary>
            /// Expanded Query Command Status to include extra information
            /// </summary>
            CC_QUERY_COMMAND_STATUS2,
            #endregion // 51 - 60

            // SBIG Use Only Commands
            #region 90 - 99
            /// <summary>
            /// Send block command
            /// </summary>
            CC_SEND_BLOCK = 90,
            /// <summary>
            /// Send byte command
            /// </summary>
            CC_SEND_BYTE,
            /// <summary>
            /// Get byte command
            /// </summary>
            CC_GET_BYTE,
            /// <summary>
            /// Send A/D command
            /// </summary>
            CC_SEND_AD,
            /// <summary>
            /// Get A/D command
            /// </summary>
            CC_GET_AD,
            /// <summary>
            /// Clock A/D command
            /// </summary>
            CC_CLOCK_AD,
            /// <summary>
            /// System test command
            /// </summary>
            CC_SYSTEM_TEST,
            /// <summary>
            /// Get driver options command
            /// </summary>
            CC_GET_DRIVER_OPTIONS,
            /// <summary>
            /// Set driver options command
            /// </summary>
            CC_SET_DRIVER_OPTIONS,
            /// <summary>
            /// Firmware command
            /// </summary>
            CC_FIRMWARE,
            #endregion // 90 - 99

            #region 100 - 109
            /// <summary>
            /// Bulk I/O command
            /// </summary>
            CC_BULK_IO,
            /// <summary>
            /// Ripple correction command
            /// </summary>
            CC_RIPPLE_CORRECTION,
            /// <summary>
            /// EZUSB Reset command
            /// </summary>
            CC_EZUSB_RESET,
            /// <summary>
            /// Breakpoint command
            /// </summary>
            CC_BREAKPOINT,
            /// <summary>
            /// Query exposure ticks command
            /// </summary>
            CC_QUERY_EXPOSURE_TICKS,
            /// <summary>
            /// Set active CCD area command
            /// </summary>
            CC_SET_ACTIVE_CCD_AREA,
            /// <summary>
            /// Returns TRUE if a readout is in progress on any driver handle
            /// </summary>
            CC_READOUT_IN_PROGRESS,
            /// <summary>
            /// Updates the RBI Preflash parameters
            /// </summary>
            CC_GET_RBI_PARAMETERS,
            /// <summary>
            /// Obtains the RBI Preflash parameters from the camera
            /// </summary>
            CC_SET_RBI_PARAMETERS,
            /// <summary>
            /// Checks to see if a camera's firmware supports a command.
            /// </summary>
            CC_QUERY_FEATURE_SUPPORTED,
            /// <summary>
            /// Last command ID
            /// </summary>
            CC_LAST_COMMAND
            #endregion // 100 - 109

        };

        // Return Error Codes
        // ====================
        // These are the error codes returned by the driver function.
        // They are prefixed with CE_ to designate them as camera errors.

        #region BASE_STRUCTURES

        /// <summary>
        /// Base value for all error IDs.
        /// </summary>
        public const UInt16 CE_ERROR_BASE = 1;

        /// <summary>
        /// Error ID enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum PAR_ERROR : UInt16
        {
            #region 0 - 10
            /// <summary>
            /// No error ID
            /// </summary>
            CE_NO_ERROR,
            /// <summary>
            /// Camera not found error
            /// </summary>
            CE_CAMERA_NOT_FOUND = CE_ERROR_BASE,
            /// <summary>
            /// Exposure in progress error
            /// </summary>
            CE_EXPOSURE_IN_PROGRESS,
            /// <summary>
            /// No exposure in progress error
            /// </summary>
            CE_NO_EXPOSURE_IN_PROGRESS,
            /// <summary>
            /// Unknown command error
            /// </summary>
            CE_UNKNOWN_COMMAND,
            /// <summary>
            /// Bad camera command error
            /// </summary>
            CE_BAD_CAMERA_COMMAND,
            /// <summary>
            /// Bad parameter command
            /// </summary>
            CE_BAD_PARAMETER,
            /// <summary>
            /// Transfer (Tx) timeout error
            /// </summary>
            CE_TX_TIMEOUT,
            /// <summary>
            /// Receive (Rx) timeout error
            /// </summary>
            CE_RX_TIMEOUT,
            /// <summary>
            /// Received Negative Acknowledgement
            /// </summary>
            CE_NAK_RECEIVED,
            /// <summary>
            /// Received Cancel
            /// </summary>
            CE_CAN_RECEIVED,
            #endregion // 0 - 10

            #region 11 - 20
            /// <summary>
            /// Unknown response error
            /// </summary>
            CE_UNKNOWN_RESPONSE,
            /// <summary>
            /// Bad length error
            /// </summary>
            CE_BAD_LENGTH,
            /// <summary>
            /// A/D timeout error
            /// </summary>
            CE_AD_TIMEOUT,
            /// <summary>
            /// Keyboard error
            /// </summary>
            CE_KBD_ESC,
            /// <summary>
            /// Checksum error
            /// </summary>
            CE_CHECKSUM_ERROR,
            /// <summary>
            /// EEPROM error
            /// </summary>
            CE_EEPROM_ERROR,
            /// <summary>
            /// Shutter error
            /// </summary>
            CE_SHUTTER_ERROR,
            /// <summary>
            /// Unknown camera error
            /// </summary>
            CE_UNKNOWN_CAMERA,
            /// <summary>
            /// Driver not found error
            /// </summary>
            CE_DRIVER_NOT_FOUND,
            /// <summary>
            /// Driver not open error
            /// </summary>
            CE_DRIVER_NOT_OPEN,
            #endregion // 11 - 20

            #region 21 - 30
            /// <summary>
            /// Driver not closed error
            /// </summary>
            CE_DRIVER_NOT_CLOSED,
            /// <summary>
            /// Share error
            /// </summary>
            CE_SHARE_ERROR,
            /// <summary>
            /// TCE not found error
            /// </summary>
            CE_TCE_NOT_FOUND,
            /// <summary>
            /// AO error
            /// </summary>
            CE_AO_ERROR,
            /// <summary>
            /// ECP error
            /// </summary>
            CE_ECP_ERROR,
            /// <summary>
            /// Memory error
            /// </summary>
            CE_MEMORY_ERROR,
            /// <summary>
            /// Device not found error
            /// </summary>
            CE_DEVICE_NOT_FOUND,
            /// <summary>
            /// Device not open error
            /// </summary>
            CE_DEVICE_NOT_OPEN,
            /// <summary>
            /// Device not closed error
            /// </summary>
            CE_DEVICE_NOT_CLOSED,
            /// <summary>
            /// Device not implemented error
            /// </summary>
            CE_DEVICE_NOT_IMPLEMENTED,
            #endregion // 21 - 30

            #region 31 - 40
            /// <summary>
            /// Device disabled error
            /// </summary>
            CE_DEVICE_DISABLED,
            /// <summary>
            /// OS error
            /// </summary>
            CE_OS_ERROR,
            /// <summary>
            /// Socket error
            /// </summary>
            CE_SOCK_ERROR,
            /// <summary>
            /// Server not found error
            /// </summary>
            CE_SERVER_NOT_FOUND,
            /// <summary>
            /// Filter wheel error
            /// </summary>
            CE_CFW_ERROR,
            /// <summary>
            /// Motor Focus error
            /// </summary>
            CE_MF_ERROR,
            /// <summary>
            /// Firmware error
            /// </summary>
            CE_FIRMWARE_ERROR,
            /// <summary>
            /// Differential guider error
            /// </summary>
            CE_DIFF_GUIDER_ERROR,
            /// <summary>
            /// Ripple corrections error
            /// </summary>
            CE_RIPPLE_CORRECTION_ERROR,
            /// <summary>
            /// EZUSB Reset error
            /// </summary>
            CE_EZUSB_RESET,
            #endregion // 31 - 40

            #region 41 - 50
            /// <summary>
            /// Firmware needs update to support feature.
            /// </summary>
            CE_INCOMPATIBLE_FIRMWARE,
            /// <summary>
            /// An invalid R/W handle was supplied for I/O
            /// </summary>
            CE_INVALID_HANDLE,
            /// <summary>
            /// Development purposes: Next Error
            /// </summary>
            CE_NEXT_ERROR
            #endregion // 41 - 50
        };

        // Camera Command State Codes
        // ============================
        // These are the return status codes for the Query Command Status command.
        // They are prefixed with CS_ to designate them as camera status.

        /// <summary>
        /// Camera states enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum PAR_COMMAND_STATUS : UInt16
        {
            /// <summary>
            /// Camera state: Idle.
            /// </summary>
            CS_IDLE,
            /// <summary>
            /// Camera state: Exposure in progress
            /// </summary>
            CS_IN_PROGRESS,
            /// <summary>
            /// Camera state: Integrating
            /// </summary>
            CS_INTEGRATING,
            /// <summary>
            /// Camera state: Integration complete
            /// </summary>
            CS_INTEGRATION_COMPLETE
        };

        /// <summary>
        /// Feature Firmware Requirement
        /// </summary>
        public enum FeatureFirmwareRequirement : UInt16
        {
            /// <summary>
            /// FFR_CTRL_OFFSET_CORRECTION
            /// </summary>
            FFR_CTRL_OFFSET_CORRECTION,
            /// <summary>
            /// FFR_CTRL_EXT_SHUTTER_ONLY
            /// </summary>
            FFR_CTRL_EXT_SHUTTER_ONLY,
            /// <summary>
            /// FFR_ASYNC_TRIGGER_IN
            /// </summary>
            FFR_ASYNC_TRIGGER_IN,
            /// <summary>
            /// FFR_LAST
            /// </summary>
            FFR_LAST
        };

        /// <summary>
        /// Pulse in is currently active state modifier flag.
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public const UInt16 CS_PULSE_IN_ACTIVE = 0x8000;

        /// <summary>
        /// Waiting for trigger state modifier flag
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public const UInt16 CS_WAITING_FOR_TRIGGER = 0x8000;

        /// <summary>
        /// 
        /// </summary>
        public const UInt16 RBI_PREFLASH_LENGTH_MASK = 0x0FFF;
        /// <summary>
        /// 
        /// </summary>
        public const UInt16 RBI_PREFLASH_FLUSH_MASK = 0xF000;
        /// <summary>
        /// 
        /// </summary>
        public const Byte RBI_PREFLASH_FLUSH_BIT = 0x0C;

        // Misc. Enumerated Constants
        // QUERY_TEMP_STATUS_REQUEST - Used with the Query Temperature Status command.
        // ABG_STATE7 - Passed to Start Exposure Command
        // MY_LOGICAL - General purpose type
        // DRIVER_REQUEST - Used with Get Driver Info command
        // CCD_REQUEST - Used with Imaging commands to specify CCD
        // CCD_INFO_REQUEST - Used with Get CCD Info Command
        // PORT - Used with Establish Link Command
        // CAMERA_TYPE - Returned by Establish Link and Get CCD Info commands
        // SHUTTER_COMMAND, SHUTTER_STATE7 - Used with Start Exposure and Miscellaneous Control Commands
        // TEMPERATURE_REGULATION - Used with Enable Temperature Regulation
        // LED_STATE - Used with the Miscellaneous Control Command
        // FILTER_COMMAND, FILTER_STATE - Used with the Miscellaneous Control Command
        // AD_SIZE, FILTER_TYPE - Used with the GetCCDInfo3 Command
        // AO_FOCUS_COMMAND - Used with the AO Set Focus Command
        // SBIG_DEVICE_TYPE - Used with Open Device Command
        // DRIVER_CONTROL_PARAM - Used with Get/SetDriverControl Command
        // USB_AD_CONTROL_COMMAND - Used with UsbADControl Command
        // CFW_MODEL_SELECT, CFW_STATUS, CFW_ERROR - Used with CFW command
        // CFW_POSITION, CFW_GET_INFO_SELECT - Used with CFW Command
        // BIT_IO_OPERATION, BIT_IO_NMAE - Used with BitIO command
        // MF_MODEL_SELECT, MF_STATUS, MF_ERROR, MF_GET_INFO_SELECT - Used with Motor Focus Command
        // DIFF_GUIDER_COMMAND, DIFF_GUIDER_STATE, DIFF_GUIDER_ERROR - Used with the Diff Guider Command

        /// <summary>
        /// Query Temperature Status enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum QUERY_TEMP_STATUS_REQUEST : UInt16
        {
            /// <summary>
            /// Temperature status Standard
            /// </summary>
            TEMP_STATUS_STANDARD,
            /// <summary>
            /// Temperature status Advanced
            /// </summary>
            TEMP_STATUS_ADVANCED,
            /// <summary>
            /// Temperature status Advanced 2
            /// </summary>
            TEMP_STATUS_ADVANCED2
        };

        /// <summary>
        /// ABG state enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum ABG_STATE7 : UInt16
        {
            /// <summary>
            /// ABG Low 7
            /// </summary>
            ABG_LOW7,
            /// <summary>
            /// ABG Clock Low 7
            /// </summary>
            ABG_CLK_LOW7,
            /// <summary>
            /// ABG Clock Medium 7
            /// </summary>
            ABG_CLK_MED7,
            /// <summary>
            /// ABG Clock High 7
            /// </summary>
            ABG_CLK_HI7
        };

        /// <summary>
        /// Boolean type definition 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct MY_LOGICAL
        {
            /// <summary>
            /// MY_LOGICAL false definition.
            /// </summary>
            public const UInt16 FALSE = 0;
            /// <summary>
            /// MY_LOGICAL true definition.
            /// </summary>
            public const UInt16 TRUE = 1;

            /// <summary>
            /// bool value.
            /// </summary>
            public UInt16 value;

            /// <summary>
            /// MY_LOGICAL constructor.
            /// </summary>
            /// <param name="value"></param>
            public MY_LOGICAL(UInt16 value)
            {
                this.value = value;
            }

            /// <summary>
            /// MY_LOGICAL constructor.
            /// </summary>
            /// <param name="value">bool value</param>
            public MY_LOGICAL(bool value)
            {
                this.value = (UInt16)(value ? 1 : 0);
            }

            /// <summary>
            /// implicit UInt16 to MY_LOGICAL conversion operator
            /// </summary>
            /// <param name="us">UInt16 type value.</param>
            public static implicit operator MY_LOGICAL(UInt16 us)
            {
                return new MY_LOGICAL(us);
            }

            /// <summary>
            /// implicit bool to MY_LOGICAL conversion operator
            /// </summary>
            /// <param name="b">bool type value.</param>
            public static implicit operator MY_LOGICAL(bool b)
            {
                return new MY_LOGICAL(b);
            }

            /// <summary>
            /// implicit MY_LOGICAL to UInt16 conversion operator
            /// </summary>
            /// <param name="logical">MY_LOGICAL type value.</param>
            public static implicit operator bool(MY_LOGICAL logical)
            {
                return (logical.value != FALSE);
            }
        };

        /// <summary>
        /// Driver request enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum DRIVER_REQUEST : UInt16
        {
            /// <summary>
            /// Driver standard
            /// </summary>
            DRIVER_STD,
            /// <summary>
            /// Driver extended
            /// </summary>
            DRIVER_EXTENDED,
            /// <summary>
            /// Driver USB loader
            /// </summary>
            DRIVER_USB_LOADER
        };

        /// <summary>
        /// CCD Request enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum CCD_REQUEST : UInt16
        {
            /// <summary>
            /// Request Imaging CCD
            /// </summary>
            CCD_IMAGING,
            /// <summary>
            /// Request Internal Tracking CCD
            /// </summary>
            CCD_TRACKING,
            /// <summary>
            /// Request External Tracking CCD
            /// </summary>
            CCD_EXT_TRACKING
        };

        /// <summary>
        /// Readout Modes enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum READOUT_BINNING_MODE : UInt16
        {
            /// <summary>
            /// 1x1 binning readout mode
            /// </summary>
            RM_1X1,
            /// <summary>
            /// 2x2 binning readout mode
            /// </summary>
            RM_2X2,
            /// <summary>
            /// 3x3 binning readout mode
            /// </summary>
            RM_3X3,
            /// <summary>
            /// Nx1 binning readout mode
            /// </summary>
            RM_NX1,
            /// <summary>
            /// Nx2 binning readout mode
            /// </summary>
            RM_NX2,
            /// <summary>
            /// Nx3 binning readout mode
            /// </summary>
            RM_NX3,
            /// <summary>
            /// 1x1 Off-chip binning readout mode
            /// </summary>
            RM_1X1_VOFFCHIP,
            /// <summary>
            /// 2x2 Off-chip binning readout mode
            /// </summary>
            RM_2X2_VOFFCHIP,
            /// <summary>
            /// 3x3 Off-chip binning readout mode
            /// </summary>
            RM_3X3_VOFFCHIP,
            /// <summary>
            /// 9x9 binning readout mode
            /// </summary>
            RM_9X9,
            /// <summary>
            /// NxN binning readout mode
            /// </summary>
            RM_NXN
        };

        /// <summary>
        /// CCD Information request enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum CCD_INFO_REQUEST : UInt16
        {
            /// <summary>
            /// Imaging CCD Info
            /// </summary>
            CCD_INFO_IMAGING,
            /// <summary>
            /// Tracking CCD Info
            /// </summary>
            CCD_INFO_TRACKING,
            /// <summary>
            /// Extended CCD Info
            /// </summary>
            CCD_INFO_EXTENDED,
            /// <summary>
            /// Extended CCD Info 5C
            /// </summary>
            CCD_INFO_EXTENDED_5C,
            /// <summary>
            /// Extended Imaging CCD Info 2
            /// </summary>
            CCD_INFO_EXTENDED2_IMAGING,
            /// <summary>
            /// Extended Tracking CCD Info 2
            /// </summary>
            CCD_INFO_EXTENDED2_TRACKING,
            /// <summary>
            /// Extended Imaging CCD Info 3
            /// </summary>
            CCD_INFO_EXTENDED3
        };

        /// <summary>
        /// Anti-blooming gate capability enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum IMAGING_ABG : UInt16
        {
            /// <summary>
            /// Anti-blooming gate not Present
            /// </summary>
            ABG_NOT_PRESENT,
            /// <summary>
            /// Anti-blooming gate present
            /// </summary>
            ABG_PRESENT
        };

        /// <summary>
        /// Port bit-rate enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum PORT_RATE : UInt16
        {
            /// <summary>
            /// Bit-rate auto
            /// </summary>
            BR_AUTO,
            /// <summary>
            /// Bit-rate 9600
            /// </summary>
            BR_9600,
            /// <summary>
            /// Bit-rate 19K
            /// </summary>
            BR_19K,
            /// <summary>
            /// Bit-rate 38K
            /// </summary>
            BR_38K,
            /// <summary>
            /// Bit-rate 57K
            /// </summary>
            BR_57K,
            /// <summary>
            /// Bit-rate 115K
            /// </summary>
            BR_115K
        };

        /// <summary>
        /// Camera type enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum CAMERA_TYPE : UInt16
        {
            /// <summary>
            /// ST-7 Camera
            /// </summary>
            ST7_CAMERA = 4,
            /// <summary>
            /// ST-8 Camera
            /// </summary>
            ST8_CAMERA,
            /// <summary>
            /// ST-5C Camera
            /// </summary>
            ST5C_CAMERA,
            /// <summary>
            /// TCE-Controller
            /// </summary>
            TCE_CONTROLLER,
            /// <summary>
            /// ST-237 Camera
            /// </summary>
            ST237_CAMERA,
            /// <summary>
            /// ST-K Camera
            /// </summary>
            STK_CAMERA,
            /// <summary>
            /// ST-9 Camera
            /// </summary>
            ST9_CAMERA,
            /// <summary>
            /// ST-V Camera
            /// </summary>
            STV_CAMERA,
            /// <summary>
            /// ST-10 Camera
            /// </summary>
            ST10_CAMERA,
            /// <summary>
            /// ST-1000 Camera
            /// </summary>
            ST1K_CAMERA,
            /// <summary>
            /// ST-2000 Camera
            /// </summary>
            ST2K_CAMERA,
            /// <summary>
            /// STL Camera
            /// </summary>
            STL_CAMERA,
            /// <summary>
            /// ST-402 Camera
            /// </summary>
            ST402_CAMERA,
            /// <summary>
            /// STX Camera
            /// </summary>
            STX_CAMERA,
            /// <summary>
            /// ST-4000 Camera
            /// </summary>
            ST4K_CAMERA,
            /// <summary>
            /// STT Camera
            /// </summary>
            STT_CAMERA,
            /// <summary>
            /// ST-i Camera
            /// </summary>
            STI_CAMERA,
            /// <summary>
            /// STF Camera,
            /// NOTE: STF8, and STF cameras both report this kind,
            /// but have *DIFFERENT CAMERA MODEL ID VARIABLES* (stf8CameraID and stfCameraID)
            /// </summary>
            STF_CAMERA,
            /// <summary>
            /// Next Camera
            /// </summary>
            NEXT_CAMERA,
            /// <summary>
            /// No Camera
            /// </summary>
            NO_CAMERA = 0xFFFF
        };

        /// <summary>
        /// Shutter Control enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum SHUTTER_COMMAND : UInt16
        {
            /// <summary>
            /// Shutter Control: Leave shutter in current state.
            /// </summary>
            SC_LEAVE_SHUTTER,
            /// <summary>
            /// Shutter Control: Open shutter.
            /// </summary>
            SC_OPEN_SHUTTER,
            /// <summary>
            /// Shutter Control: Close shutter.
            /// </summary>
            SC_CLOSE_SHUTTER,
            /// <summary>
            /// Shutter Control: Initialize shutter.
            /// </summary>
            SC_INITIALIZE_SHUTTER,
            /// <summary>
            /// Shutter Control: Open external shutter.
            /// </summary>
            SC_OPEN_EXT_SHUTTER,
            /// <summary>
            /// Shutter Control: Close external shutter.
            /// </summary>
            SC_CLOSE_EXT_SHUTTER
        };

        /// <summary>
        /// Shutter State enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum SHUTTER_STATE7 : UInt16
        {
            /// <summary>
            /// Shuter State: Open
            /// </summary>
            SS_OPEN,
            /// <summary>
            /// Shuter State: Closed
            /// </summary>
            SS_CLOSED,
            /// <summary>
            /// Shutter State: Opening
            /// </summary>
            SS_OPENING,
            /// <summary>
            /// Shutter State: Closing
            /// </summary>
            SS_CLOSING
        };

        /// <summary>
        /// Temperature regulation enum 
        /// <para>ingroup BASE_STRUCTURES</para>
        /// </summary>
        public enum TEMPERATURE_REGULATION : UInt16
        {
            /// <summary>
            /// Temperature regulation off
            /// </summary>
            REGULATION_OFF,
            /// <summary>
            /// Temperature regulation on
            /// </summary>
            REGULATION_ON,
            /// <summary>
            /// Temperature regulation override
            /// </summary>
            REGULATION_OVERRIDE,
            /// <summary>
            /// Temperature regulation freeze
            /// </summary>
            REGULATION_FREEZE,
            /// <summary>
            /// Temperature regulation unfreeze
            /// </summary>
            REGULATION_UNFREEZE,
            /// <summary>
            /// Temperature regulation enable autofreeze
            /// </summary>
            REGULATION_ENABLE_AUTOFREEZE,
            /// <summary>
            /// Temperature regulation disable autofreeze
            /// </summary>
            REGULATION_DISABLE_AUTOFREEZE
        };

        #endregion // BASE_STRUCTURES

        /// <summary>
        /// Mask for Temperature Regulation frozen state
        /// </summary>
        public const UInt16 REGULATION_FROZEN_MASK = 0x8000;

        /// <summary>
        /// LED State enum 
        /// </summary>
        public enum LED_STATE : UInt16
        {
            /// <summary>
            /// LED off
            /// </summary>
            LED_OFF,
            /// <summary>
            /// LED on
            /// </summary>
            LED_ON,
            /// <summary>
            /// LED Blink low
            /// </summary>
            LED_BLINK_LOW,
            /// <summary>
            /// LED Blink high
            /// </summary>
            LED_BLINK_HIGH
        };

        /// <summary>
        /// Filter command enum
        /// </summary>
        public enum FILTER_COMMAND : UInt16
        {
            /// <summary>
            /// Filter leave
            /// </summary>
            FILTER_LEAVE,
            /// <summary>
            /// Filter slot 1
            /// </summary>
            FILTER_SET_1,
            /// <summary>
            /// Filter slot 2
            /// </summary>
            FILTER_SET_2,
            /// <summary>
            /// Filter slot 3
            /// </summary>
            FILTER_SET_3,
            /// <summary>
            /// Filter slot 4
            /// </summary>
            FILTER_SET_4,
            /// <summary>
            /// Filter slot 5
            /// </summary>
            FILTER_SET_5,
            /// <summary>
            /// Stop filter
            /// </summary>
            FILTER_STOP,
            /// <summary>
            /// Initialize filter
            /// </summary>
            FILTER_INIT
        };

        /// <summary>
        /// Filter State enum 
        /// </summary>
        public enum FILTER_STATE : UInt16
        {
            /// <summary>
            /// Filter wheel moving
            /// </summary>
            FS_MOVING,
            /// <summary>
            /// Filter wheel at slot 1
            /// </summary>
            FS_AT_1,
            /// <summary>
            /// Filter wheel at slot 2
            /// </summary>
            FS_AT_2,
            /// <summary>
            /// Filter wheel at slot 3
            /// </summary>
            FS_AT_3,
            /// <summary>
            /// Filter wheel at slot 4
            /// </summary>
            FS_AT_4,
            /// <summary>
            /// Filter wheel at slot 5
            /// </summary>
            FS_AT_5,
            /// <summary>
            /// Filter wheel at slot Unknown
            /// </summary>
            FS_UNKNOWN
        };

        /// <summary>
        /// A/D Size enum 
        /// </summary>
        public enum AD_SIZE : UInt16
        {
            /// <summary>
            /// Unknown size
            /// </summary>
            AD_UNKNOWN,
            /// <summary>
            /// 12-bits
            /// </summary>
            AD_12_BITS,
            /// <summary>
            /// 16-bits
            /// </summary>
            AD_16_BITS
        };

        /// <summary>
        /// Filter Wheel Type enum
        /// </summary>
        public enum FILTER_TYPE : UInt16
        {
            /// <summary>
            /// Unkwown Filter Wheel
            /// </summary>
            FW_UNKNOWN,
            /// <summary>
            /// External Filter Wheel
            /// </summary>
            FW_EXTERNAL,
            /// <summary>
            /// Vane Filter Wheel
            /// </summary>
            FW_VANE,
            /// <summary>
            /// Standard Filter Wheel
            /// </summary>
            FW_FILTER_WHEEL
        };

        /// <summary>
        /// AO Focus enum 
        /// </summary>
        public enum AO_FOCUS_COMMAND : UInt16
        {
            /// <summary>
            /// AO Focus hard center
            /// </summary>
            AOF_HARD_CENTER,
            /// <summary>
            /// AO Focus soft center
            /// </summary>
            AOF_SOFT_CENTER,
            /// <summary>
            /// AO Focus step in
            /// </summary>
            AOF_STEP_IN,
            /// <summary>
            /// AO Focus step out
            /// </summary>
            AOF_STEP_OUT
        };

        // Ethernet stuff
        /// <summary>
        /// Service port for Ethernet access.
        /// </summary>
        public const Int16 SRV_SERVICE_PORT = 5000;

        /// <summary>
        /// Broadcast port for SBIG Cameras
        /// </summary>
        public const Int16 BROADCAST_PORT = 5001;

        /// <summary>
        /// SBIG Device types enum 
        /// </summary>
        public enum SBIG_DEVICE_TYPE : UInt16
        {
            /// <summary>
            /// Device type: None
            /// </summary>
            DEV_NONE,
            /// <summary>
            /// LPT port slot 1
            /// </summary>
            DEV_LPT1,
            /// <summary>
            /// LPT port slot 2
            /// </summary>
            DEV_LPT2,
            /// <summary>
            /// LPT port slot 3
            /// </summary>
            DEV_LPT3,
            /// <summary>
            /// USB autodetect
            /// </summary>
            DEV_USB = 0x7F00,
            /// <summary>
            /// Ethernet
            /// </summary>
            DEV_ETH,
            /// <summary>
            /// USB slot 1 CC_QUERY_USB
            /// </summary>
            DEV_USB1,
            /// <summary>
            /// USB slot 2
            /// </summary>
            DEV_USB2,
            /// <summary>
            /// USB slot 3
            /// </summary>
            DEV_USB3,
            /// <summary>
            /// USB slot 4
            /// </summary>
            DEV_USB4,
            /// <summary>
            /// USB slot 5 <seealso cref="PAR_COMMAND.CC_QUERY_USB2"/>
            /// </summary>
            DEV_USB5,
            /// <summary>
            /// USB slot 6
            /// </summary>
            DEV_USB6,
            /// <summary>
            /// USB slot 7
            /// </summary>
            DEV_USB7,
            /// <summary>
            /// USB slot 8
            /// </summary>
            DEV_USB8,
            /// <summary>
            /// USB slot 9 <seealso cref="PAR_COMMAND.CC_QUERY_USB3"/>
            /// </summary>
            DEV_USB9,
            /// <summary>
            /// USB slot 10
            /// </summary>
            DEV_USB10,
            /// <summary>
            /// USB slot 11
            /// </summary>
            DEV_USB11,
            /// <summary>
            /// USB slot 12
            /// </summary>
            DEV_USB12,
            /// <summary>
            /// USB slot 13
            /// </summary>
            DEV_USB13,
            /// <summary>
            /// USB slot 14
            /// </summary>
            DEV_USB14,
            /// <summary>
            /// USB slot 15
            /// </summary>
            DEV_USB15,
            /// <summary>
            /// USB slot 16
            /// </summary>
            DEV_USB16,
            /// <summary>
            /// USB slot 17
            /// </summary>
            DEV_USB17,
            /// <summary>
            /// USB slot 18
            /// </summary>
            DEV_USB18,
            /// <summary>
            /// USB slot 19
            /// </summary>
            DEV_USB19,
            /// <summary>
            /// USB slot 20
            /// </summary>
            DEV_USB20,
            /// <summary>
            /// USB slot 21
            /// </summary>
            DEV_USB21,
            /// <summary>
            /// USB slot 22
            /// </summary>
            DEV_USB22,
            /// <summary>
            /// USB slot 23
            /// </summary>
            DEV_USB23,
            /// <summary>
            /// USB slot 24
            /// </summary>
            DEV_USB24,
        };

        /// <summary>
        /// Driver control parameters enum
        /// </summary>
        public enum DRIVER_CONTROL_PARAM : UInt16
        {
            /// <summary>
            /// Enable FIFO
            /// </summary>
            DCP_USB_FIFO_ENABLE,
            /// <summary>
            /// Enable Journaling
            /// </summary>
            DCP_CALL_JOURNAL_ENABLE,
            /// <summary>
            /// IV to H Ratio
            /// </summary>
            DCP_IVTOH_RATIO,
            /// <summary>
            /// USB FIFO size
            /// </summary>
            DCP_USB_FIFO_SIZE,
            /// <summary>
            /// USB Driver
            /// </summary>
            DCP_USB_DRIVER,
            /// <summary>
            /// KAI Relative Gain
            /// </summary>
            DCP_KAI_RELGAIN,
            /// <summary>
            /// USB Pixel D\L enable
            /// </summary>
            DCP_USB_PIXEL_DL_ENABLE,
            /// <summary>
            /// High throughput
            /// </summary>
            DCP_HIGH_THROUGHPUT,
            /// <summary>
            /// VDD Optimized
            /// </summary>
            DCP_VDD_OPTIMIZED,
            /// <summary>
            /// Auto A/D Gain
            /// </summary>
            DCP_AUTO_AD_GAIN,
            /// <summary>
            /// No H-Clocks for Integration
            /// </summary>
            DCP_NO_HCLKS_FOR_INTEGRATION,
            /// <summary>
            /// TDI Mode Enable
            /// </summary>
            DCP_TDI_MODE_ENABLE,
            /// <summary>
            /// Vertical Flush control enable
            /// </summary>
            DCP_VERT_FLUSH_CONTROL_ENABLE,
            /// <summary>
            /// Ethernet pipeline enable
            /// </summary>
            DCP_ETHERNET_PIPELINE_ENABLE,
            /// <summary>
            /// Fast link
            /// </summary>
            DCP_FAST_LINK,
            /// <summary>
            /// Overscan Rows/Columns
            /// </summary>
            DCP_OVERSCAN_ROWSCOLS,
            /// <summary>
            /// Enable Pixel Pipeline
            /// </summary>
            DCP_PIXEL_PIPELINE_ENABLE,
            /// <summary>
            /// Enable column repair
            /// </summary>
            DCP_COLUMN_REPAIR_ENABLE,
            /// <summary>
            /// Enable warm pixel repair
            /// </summary>
            DCP_WARM_PIXEL_REPAIR_ENABLE,
            /// <summary>
            /// warm pixel repair count
            /// </summary>
            DCP_WARM_PIXEL_REPAIR_COUNT,
            /// <summary>
            /// TDI Drift rate in [XXX]
            /// </summary>
            DCP_TDI_MODE_DRIFT_RATE,
            /// <summary>
            /// Override A/D Converter's Gain
            /// </summary>
            DCP_OVERRIDE_AD_GAIN,
            /// <summary>
            /// Override auto offset adjustments in certain cameras.
            /// </summary>
            DCP_ENABLE_AUTO_OFFSET,
            /// <summary>
            /// Last Device control parameter
            /// </summary>
            DCP_LAST
        };

        /// <summary>
        /// USB A/D Control commands 
        /// </summary>
        public enum USB_AD_CONTROL_COMMAND : UInt16
        {
            /// <summary>
            /// Imaging gain
            /// </summary>
            USB_AD_IMAGING_GAIN,
            /// <summary>
            /// Imaging offset
            /// </summary>
            USB_AD_IMAGING_OFFSET,
            /// <summary>
            /// Internal tracking gain
            /// </summary>

            USB_AD_TRACKING_GAIN,
            /// <summary>
            /// Internal tracking offset
            /// </summary>
            USB_AD_TRACKING_OFFSET,
            /// <summary>
            /// External tracking gain
            /// </summary>

            USB_AD_EXTTRACKING_GAIN,
            /// <summary>
            /// External tracking offset
            /// </summary>
            USB_AD_EXTTRACKING_OFFSET,
            /// <summary>
            /// Imaging gain channel 2
            /// </summary>

            USB_AD_IMAGING2_GAIN,
            /// <summary>
            /// Imaging offset channel 2
            /// </summary>
            USB_AD_IMAGING2_OFFSET,
            /// <summary>
            /// Imaging gain right channel
            /// </summary>

            USB_AD_IMAGING_GAIN_RIGHT,
            /// <summary>
            /// Imaging offset right channel
            /// </summary>
            USB_AD_IMAGING_OFFSET_RIGHT,
        };

        /// <summary>
        /// USB Driver enum 
        /// </summary>
        public enum ENUM_USB_DRIVER : UInt16
        {
            /// <summary>
            /// SBIG E
            /// </summary>
            USBD_SBIGE,
            /// <summary>
            /// SBIG I
            /// </summary>
            USBD_SBIGI,
            /// <summary>
            /// SBIG_M
            /// </summary>
            USBD_SBIGM,
            /// <summary>
            /// Next
            /// </summary>
            USBD_NEXT
        };

        /// <summary>
        /// Filter Weel Model Selection enum 
        /// </summary>
        public enum CFW_MODEL_SELECT : UInt16
        {
            /// <summary>
            /// Unknown Model
            /// </summary>
            CFWSEL_UNKNOWN,
            /// <summary>
            /// CFW2
            /// </summary>
            CFWSEL_CFW2,
            /// <summary>
            /// CFW5
            /// </summary>
            CFWSEL_CFW5,
            /// <summary>
            /// CFW8
            /// </summary>
            CFWSEL_CFW8,
            /// <summary>
            /// CFWL
            /// </summary>
            CFWSEL_CFWL,
            /// <summary>
            /// CFW-402
            /// </summary>
            CFWSEL_CFW402,
            /// <summary>
            /// Auto
            /// </summary>
            CFWSEL_AUTO,
            /// <summary>
            /// CFW-6A
            /// </summary>
            CFWSEL_CFW6A,
            /// <summary>
            /// CFW10
            /// </summary>
            CFWSEL_CFW10,
            /// <summary>
            /// CFW10-Serial
            /// </summary>
            CFWSEL_CFW10_SERIAL,
            /// <summary>
            /// CFW9
            /// </summary>
            CFWSEL_CFW9,
            /// <summary>
            /// CFWL8
            /// </summary>
            CFWSEL_CFWL8,
            /// <summary>
            /// CFWL8-G
            /// </summary>
            CFWSEL_CFWL8G,
            /// <summary>
            /// CFW1603
            /// </summary>
            CFWSEL_CFW1603,
            /// <summary>
            /// FW5-STX
            /// </summary>
            CFWSEL_FW5_STX,
            /// <summary>
            /// FW5-8300
            /// </summary>
            CFWSEL_FW5_8300,
            /// <summary>
            /// FW8-8300
            /// </summary>
            CFWSEL_FW8_8300,
            /// <summary>
            /// FW7-STX
            /// </summary>
            CFWSEL_FW7_STX,
            /// <summary>
            /// FW8-STT
            /// </summary>
            CFWSEL_FW8_STT,
            /// <summary>
            /// FW5-STF Detent
            /// </summary>
            CFWSEL_FW5_STF_DETENT
        };

        /// <summary>
        /// Filter Wheel Command enum 
        /// </summary>
        public enum CFW_COMMAND : UInt16
        {
            /// <summary>
            /// Query
            /// </summary>
            CFWC_QUERY,
            /// <summary>
            /// Go-to slot
            /// </summary>
            CFWC_GOTO,
            /// <summary>
            /// Initialize
            /// </summary>
            CFWC_INIT,
            /// <summary>
            /// Get Info
            /// </summary>
            CFWC_GET_INFO,
            /// <summary>
            /// Open device
            /// </summary>
            CFWC_OPEN_DEVICE,
            /// <summary>
            /// Close device
            /// </summary>
            CFWC_CLOSE_DEVICE
        };

        /// <summary>
        /// Filter Wheel Status enum 
        /// </summary>
        public enum CFW_STATUS : UInt16
        {
            /// <summary>
            /// Unknown state
            /// </summary>
            CFWS_UNKNOWN,
            /// <summary>
            /// Idle state
            /// </summary>
            CFWS_IDLE,
            /// <summary>
            /// Busy state
            /// </summary>
            CFWS_BUSY
        };

        /// <summary>
        /// Filter Wheel errors enum 
        /// </summary>
        public enum CFW_ERROR : UInt16
        {
            /// <summary>
            /// No error
            /// </summary>
            CFWE_NONE,
            /// <summary>
            /// Busy error
            /// </summary>
            CFWE_BUSY,
            /// <summary>
            /// Bad command error
            /// </summary>
            CFWE_BAD_COMMAND,
            /// <summary>
            /// Calibration error
            /// </summary>
            CFWE_CAL_ERROR,
            /// <summary>
            /// Motor timeout error
            /// </summary>
            CFWE_MOTOR_TIMEOUT,
            /// <summary>
            /// Bad model error
            /// </summary>
            CFWE_BAD_MODEL,
            /// <summary>
            /// Device not closed error
            /// </summary>
            CFWE_DEVICE_NOT_CLOSED,
            /// <summary>
            /// Device not open error
            /// </summary>
            CFWE_DEVICE_NOT_OPEN,
            /// <summary>
            /// I2C communication error
            /// </summary>
            CFWE_I2C_ERROR
        };

        /// <summary>
        /// Filter Wheel position enum 
        /// </summary>
        public enum CFW_POSITION : UInt16
        {
            /// <summary>
            /// Unknown
            /// </summary>
            CFWP_UNKNOWN,
            /// <summary>
            /// Slot 1
            /// </summary>
            CFWP_1,
            /// <summary>
            /// Slot 2
            /// </summary>
            CFWP_2,
            /// <summary>
            /// Slot 3
            /// </summary>
            CFWP_3,
            /// <summary>
            /// Slot 4
            /// </summary>
            CFWP_4,
            /// <summary>
            /// Slot 5
            /// </summary>
            CFWP_5,
            /// <summary>
            /// Slot 6
            /// </summary>
            CFWP_6,
            /// <summary>
            /// Slot 7
            /// </summary>
            CFWP_7,
            /// <summary>
            /// Slot 8
            /// </summary>
            CFWP_8,
            /// <summary>
            /// Slot 9
            /// </summary>
            CFWP_9,
            /// <summary>
            /// Slot 10
            /// </summary>
            CFWP_10
        };

        /// <summary>
        /// Filter Wheel COM port enum 
        /// </summary>
        public enum CFW_COM_PORT : UInt16
        {
            /// <summary>
            /// COM1
            /// </summary>
            CFWPORT_COM1 = 1,
            /// <summary>
            /// COM2
            /// </summary>
            CFWPORT_COM2,
            /// <summary>
            /// COM3
            /// </summary>
            CFWPORT_COM3,
            /// <summary>
            /// COM4
            /// </summary>
            CFWPORT_COM4
        };

        /// <summary>
        /// Filter Wheel Get Info select enum 
        /// </summary>
        public enum CFW_GETINFO_SELECT : UInt16
        {
            /// <summary>
            /// Firmware version
            /// </summary>
            CFWG_FIRMWARE_VERSION,
            /// <summary>
            /// Calibration data
            /// </summary>
            CFWG_CAL_DATA,
            /// <summary>
            /// Data registers
            /// </summary>
            CFWG_DATA_REGISTERS
        };

        /// <summary>
        /// Bit I/O Operation enum 
        /// </summary>
        public enum BITIO_OPERATION : UInt16
        {
            /// <summary>
            /// Write
            /// </summary>
            BITIO_WRITE,
            /// <summary>
            /// Read
            /// </summary>
            BITIO_READ
        };

        /// <summary>
        /// Bit I/O Name enum 
        /// </summary>
        public enum BITIO_NAME : UInt16
        {
            /// <summary>
            /// In: PS Low
            /// </summary>
            BITI_PS_LOW,
            /// <summary>
            /// Out: I/O 1
            /// </summary>
            BITO_IO1,
            /// <summary>
            /// Out: I/O 2
            /// </summary>
            BITO_IO2,
            /// <summary>
            /// In: I/O 3
            /// </summary>
            BITI_IO3,
            /// <summary>
            /// FPGA WE
            /// </summary>
            BITO_FPGA_WE
        };

        /// <summary>
        /// Biorad TDI Error enum 
        /// </summary>
        public enum BTDI_ERROR : UInt16
        {
            /// <summary>
            /// BTDI Schedule error
            /// </summary>
            BTDI_SCHEDULE_ERROR = 1,
            /// <summary>
            /// BTDI Overrun error
            /// </summary>
            BTDI_OVERRUN_ERROR = 2
        };

        /// <summary>
        /// Motor Focus Model Selection enum 
        /// </summary>
        public enum MF_MODEL_SELECT : UInt16
        {
            /// <summary>
            /// Unknown
            /// </summary>
            MFSEL_UNKNOWN,
            /// <summary>
            /// Automatic
            /// </summary>
            MFSEL_AUTO,
            /// <summary>
            /// STF
            /// </summary>
            MFSEL_STF
        };

        /// <summary>
        /// Motor Focus Command enum 
        /// </summary>
        public enum MF_COMMAND : UInt16
        {
            /// <summary>
            /// Query
            /// </summary>
            MFC_QUERY,
            /// <summary>
            /// Go-to
            /// </summary>
            MFC_GOTO,
            /// <summary>
            /// Initialize
            /// </summary>
            MFC_INIT,
            /// <summary>
            /// Get Info
            /// </summary>
            MFC_GET_INFO,
            /// <summary>
            /// Abort
            /// </summary>
            MFC_ABORT
        };

        /// <summary>
        /// Motor Focus Status 
        /// </summary>
        public enum MF_STATUS : UInt16
        {
            /// <summary>
            /// Unknown
            /// </summary>
            MFS_UNKNOWN,
            /// <summary>
            /// Idle
            /// </summary>
            MFS_IDLE,
            /// <summary>
            /// Busy
            /// </summary>
            MFS_BUSY
        };

        /// <summary>
        /// Motor Focus Error state enum 
        /// </summary>
        public enum MF_ERROR : UInt16
        {
            /// <summary>
            /// None
            /// </summary>
            MFE_NONE,
            /// <summary>
            /// Busy
            /// </summary>
            MFE_BUSY,
            /// <summary>
            /// Bad command
            /// </summary>
            MFE_BAD_COMMAND,
            /// <summary>
            /// Calibration error
            /// </summary>
            MFE_CAL_ERROR,
            /// <summary>
            /// Motor timeout
            /// </summary>
            MFE_MOTOR_TIMEOUT,
            /// <summary>
            /// Bad model
            /// </summary>
            MFE_BAD_MODEL,
            /// <summary>
            /// I2C error
            /// </summary>
            MFE_I2C_ERROR,
            /// <summary>
            /// Not found
            /// </summary>
            MFE_NOT_FOUND
        };

        /// <summary>
        /// Motor Focus Get Info Select enum 
        /// </summary>
        public enum MF_GETINFO_SELECT : UInt16
        {
            /// <summary>
            /// Firmware Version
            /// </summary>
            MFG_FIRMWARE_VERSION,
            /// <summary>
            /// Data Registers
            /// </summary>
            MFG_DATA_REGISTERS
        };

        /// <summary>
        /// Differential guider commands enum 
        /// </summary>
        public enum DIFF_GUIDER_COMMAND : UInt16
        {
            /// <summary>
            /// Detect Differential guider hardware
            /// </summary>
            DGC_DETECT,
            /// <summary>
            /// Get brightness
            /// </summary>
            DGC_GET_BRIGHTNESS,
            /// <summary>
            /// Set brightness
            /// </summary>
            DGC_SET_BRIGHTNESS
        };

        /// <summary>
        /// Differential guider error enum 
        /// </summary>
        public enum DIFF_GUIDER_ERROR : UInt16
        {
            /// <summary>
            /// No error
            /// </summary>
            DGE_NO_ERROR,
            /// <summary>
            /// Differential guider not found
            /// </summary>
            DGE_NOT_FOUND,
            /// <summary>
            /// Bad command
            /// </summary>
            DGE_BAD_COMMAND,
            /// <summary>
            /// Bad parameter
            /// </summary>
            DGE_BAD_PARAMETER
        };

        /// <summary>
        /// Differential Guider status enum 
        /// </summary>
        public enum DIFF_GUIDER_STATUS : UInt16
        {
            /// <summary>
            /// Unknown
            /// </summary>
            DGS_UNKNOWN,
            /// <summary>
            /// Idle
            /// </summary>
            DGS_IDLE,
            /// <summary>
            /// Busy
            /// </summary>
            DGS_BUSY
        };

        /// <summary>
        /// Fan state enum 
        /// </summary>
        public enum FAN_STATE : UInt16
        {
            /// <summary>
            /// Fan Off
            /// </summary>
            FS_OFF,
            /// <summary>
            /// Fan On
            /// </summary>
            FS_ON,
            /// <summary>
            /// Fan Auto
            /// </summary>
            FS_AUTOCONTROL
        };

        /// <summary>
        /// Bulk IO command enum 
        /// </summary>
        public enum BULK_IO_COMMAND : UInt16
        {
            /// <summary>
            /// Read
            /// </summary>
            BIO_READ,
            /// <summary>
            /// Write
            /// </summary>
            BIO_WRITE,
            /// <summary>
            /// Flush
            /// </summary>
            BIO_FLUSH
        };

        /// <summary>
        /// Pixel channel mode enum 
        /// </summary>
        public enum PIXEL_CHANNEL_MODE : UInt16
        {
            /// <summary>
            /// Pixel Channel A
            /// </summary>
            PIXEL_CHANNEL_MODE_A,
            /// <summary>
            /// Pixel Channel B
            /// </summary>
            PIXEL_CHANNEL_MODE_B,
            /// <summary>
            /// Pixel Channel AB
            /// </summary>
            PIXEL_CHANNEL_MODE_AB
        };

        /// <summary>
        /// Active Pixel Channel enum 
        /// </summary>
        public enum ACTIVE_PIXEL_CHANNEL : UInt16
        {
            /// <summary>
            /// Pixel Channel A
            /// </summary>
            PIXEL_CHANNEL_A,
            /// <summary>
            /// Pixel Channel B
            /// </summary>
            PIXEL_CHANNEL_B
        };

        /// <summary>
        /// <seealso cref="PAR_COMMAND.CC_QUERY_COMMAND_STATUS2"/>
        /// </summary>
        public enum EXTRA_EXPOSURE_STATUS : UInt16
        {
            /// <summary>
            /// CCD is currently idle.
            /// </summary>
            XES_IDLE,
            /// <summary>
            /// CCD is in the pre-exposure phase.
            /// </summary>
            XES_PRE_EXP,
            /// <summary>
            /// CCD is currently exposing/integrating an image.
            /// </summary>
            XES_INTEGRATING,
            /// <summary>
            /// CCD is in the post-exposure phase.
            /// </summary>
            XES_POST_EXP
        };
        #endregion // SBIG enum

        #region General Purpose Flags

        /// <summary>
        /// set in <see cref="EndExposureParams.ccd"/> to skip synchronization delay - 
        /// Use this to increase the rep rate when taking darks to later be subtracted 
        /// from <see cref="SHUTTER_COMMAND.SC_LEAVE_SHUTTER"/> exposures such as when tracking and imaging.
        /// </summary>
        public const UInt16 END_SKIP_DELAY = 0x8000;

        /// <summary>
        /// Set in <see cref="StartExposureParams.ccd"/> to skip lowering Imaging CCD Vdd during integration.
        /// - Use this to increase the rep rate when you don't care about glow 
        /// in the upper-left corner of the imaging CCD.
        /// </summary>
        public const UInt16 START_SKIP_VDD = 0x8000;

        /// <summary>
        /// Set in <see cref="StartExposureParams.ccd"/> and <see cref="EndExposureParams.ccd"/> to force shutter motor 
        /// to stay on all the time which reduces delays in Start and End Exposure timing 
        /// and yields higher image throughput.
        /// Don't do this too often or camera head will heat up.
        /// </summary>
        public const UInt16 START_MOTOR_ALWAYS_ON = 0x4000;

        /// <summary>
        /// Set in <see cref="EndExposureParams.ccd"/> to abort the exposure completely instead 
        /// of just ending the integration phase for cameras with internal frame buffers like the STX.
        /// </summary>
        public const UInt16 ABORT_DONT_END = 0x2000;

        #region EXPOSURE_FLAGS
        //TODO: Add supported cameras.
        /// <summary>
        /// Set in <see cref="StartExposureParams2.exposureTime"/> enable TDI readout mode
        /// <para>ingroup EXPOSURE_FLAGS</para>
        /// </summary>
        public const UInt32 EXP_TDI_ENABLE = 0x01000000;

        /// <summary>
        /// Set in <see cref="StartExposureParams2.exposureTime"/> ripple correction for STF-8050/4070
        /// <para>ingroup EXPOSURE_FLAGS</para>
        /// </summary>
        public const UInt32 EXP_RIPPLE_CORRECTION = 0x02000000;

        /// <summary>
        /// Set in <see cref="StartExposureParams2.exposureTime"/> to activate the dual channel 
        /// CCD readout mode of the STF-8050.
        /// <para>ingroup EXPOSURE_FLAGS</para>
        /// </summary>
        public const UInt32 EXP_DUAL_CHANNEL_MODE = 0x04000000;

        /// <summary>
        /// Set in <see cref="StartExposureParams2.exposureTime"/> to activate the fast readout mode 
        /// of the STF-8300, etc.
        /// <para>ingroup EXPOSURE_FLAGS</para>
        /// </summary>
        public const UInt32 EXP_FAST_READOUT = 0x08000000;

        /// <summary>
        /// Set in <see cref="StartExposureParams2.exposureTime"/> to interpret exposure time as milliseconds.
        /// <para>ingroup EXPOSURE_FLAGS</para>
        /// </summary>
        public const UInt32 EXP_MS_EXPOSURE = 0x10000000;

        /// <summary>
        /// Set in <see cref="StartExposureParams2.exposureTime"/> to do light clear of the CCD.
        /// <para>ingroup EXPOSURE_FLAGS</para>
        /// </summary>
        public const UInt32 EXP_LIGHT_CLEAR = 0x20000000;

        /// <summary>
        /// Set in <see cref="StartExposureParams2.exposureTime"/> to send trigger out Y-.
        /// <para>ingroup EXPOSURE_FLAGS</para>
        /// </summary>
        public const UInt32 EXP_SEND_TRIGGER_OUT = 0x40000000;

        /// <summary>
        /// Set in <see cref="StartExposureParams2.exposureTime"/> to wait for trigger in pulse.
        /// <para>ingroup EXPOSURE_FLAGS</para>
        /// </summary>
        public const UInt32 EXP_WAIT_FOR_TRIGGER_IN = 0x80000000;

        /// <summary>
        /// Set in <see cref="StartExposureParams2.exposureTime"/> to mask with exposure time to remove flags.
        /// <para>ingroup EXPOSURE_FLAGS</para>
        /// </summary>
        public const UInt32 EXP_TIME_MASK = 0x00FFFFFF;
        #endregion // EXPOSURE_FLAGS

        // Bit Field Definitions for the in the GetCCDInfoResults4 struct.
        #region CAPABILITIES_BITS
        /// <summary>
        /// mask for CCD type
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_TYPE_MASK = 0x0001;

        /// <summary>
        /// b0=0 is full frame CCD
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_TYPE_FULL_FRAME = 0x0000;

        /// <summary>
        /// b0=1 is frame transfer CCD
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_TYPE_FRAME_TRANSFER = 0x0001;

        /// <summary>
        /// mask for electronic shutter type
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_ESHUTTER_MASK = 0x0002;

        /// <summary>
        /// b1=0 indicates no electronic shutter
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_ESHUTTER_NO = 0x0000;

        /// <summary>
        /// b1=1 indicates electronic shutter
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_ESHUTTER_YES = 0x0002;

        /// <summary>
        /// mask for external tracker support
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_EXT_TRACKER_MASK = 0x0004;

        /// <summary>
        /// b2=0 indicates no external tracker support
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_EXT_TRACKER_NO = 0x0000;

        /// <summary>
        /// b2=1 indicates external tracker support
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_EXT_TRACKER_YES = 0x0004;

        /// <summary>
        /// mask for BTDI support
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_BTDI_MASK = 0x0008;

        /// <summary>
        /// b3=0 indicates no BTDI support
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_BTDI_NO = 0x0000;

        /// <summary>
        /// b3=1 indicates BTDI support
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_CCD_BTDI_YES = 0x0008;

        /// <summary>
        /// mask for AO-8 detected 
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_AO8_MASK = 0x0010;

        /// <summary>
        /// b4=0 indicates no AO-8 detected
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_AO8_NO = 0x0000;

        /// <summary>
        /// b4=1 indicates AO-8 detected
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_AO8_YES = 0x0010;

        /// <summary>
        /// mask for camera with frame buffer
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_FRAME_BUFFER_MASK = 0x0020;

        /// <summary>
        /// b5=0 indicates camera without Frame Buffer
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_FRAME_BUFFER_NO = 0x0000;

        /// <summary>
        /// b5=1 indicates camera with Frame Buffer
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_FRAME_BUFFER_YES = 0x0020;

        /// <summary>
        /// mask for camera that requires StartExposure2
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_REQUIRES_STARTEXP2_MASK = 0x0040;

        /// <summary>
        /// b6=0 indicates camera works with StartExposure
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_REQUIRES_STARTEXP2_NO = 0x0000;

        /// <summary>
        /// b6=1 indicates camera Requires StartExposure2
        /// <para>ingroup CAPABILITIES_BITS</para>
        /// </summary>
        public const UInt16 CB_REQUIRES_STARTEXP2_YES = 0x0040;
        #endregion // CAPABILITIES_BITS

        #region MINIMUM_DEFINES
        /// <summary>
        /// Minimum exposure for ST-7 cameras in 1/100ths second
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_ST7_EXPOSURE = 12;

        /// <summary>
        /// Minimum exposure for ST-402 cameras in 1/100ths second
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_ST402_EXPOSURE = 4;

        /// <summary>
        /// Minimum exposure fpr STF-3200 cameras in 1/100ths second
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_ST3200_EXPOSURE = 9;


        /// <summary>
        /// Minimum exposure for STF-8300 cameras in 1/100ths second
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_STF8300_EXPOSURE = 9;

        /// <summary>
        /// Minimum exposure for STF-8050 cameras in 1/1000ths second since has E Shutter
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_STF8050_EXPOSURE = 1;

        /// <summary>
        /// Minimum exposure for STF-4070 cameras in 1/1000ths second since has E Shutter
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_STF4070_EXPOSURE = 1;


        /// <summary>
        /// Minimum exposure for STF-0402 cameras in 1/100ths second.
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_STF0402_EXPOSURE = 4;

        /// <summary>
        /// Minimum exposure for STX cameras in 1/100ths second
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_STX_EXPOSURE = 18;

        /// <summary>
        /// Minimum exposure for STT cameras in 1/100ths second
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_STT_EXPOSURE = 12;

        /// <summary>
        /// Minimum exposure in 1/1000ths second since ST-i has E Shutter
        /// <para>ingroup MINIMUM_DEFINES</para>
        /// </summary>
        public const Int16 MIN_STU_EXPOSURE = 1;
        #endregion // MINIMUM_DEFINES

        #endregion // General Purpose Flags

        #region Command Parameter and Results Structs
        // Make sure you set your compiler for byte structure alignment as that is how the driver was built.

        /// <summary>
        /// SBIG Parameters structure interface.
        /// </summary>
        public interface IParams
        { }

        /// <summary>
        /// SBIG Results structure interface.
        /// </summary>
        public interface IResults
        { }

        /// <summary>
        /// Parameters used to start SBIG camera exposures.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct StartExposureParams : IParams
        {
            /// <summary>
            /// Requested CCD. see also: <seealso cref="CCD_REQUEST"/> enum.
            /// </summary>
            public CCD_REQUEST ccd;

            /// <summary>
            /// Exposure time in hundredths of a second in least significant 24 bits. 
            /// Most significant bits are bit-flags described in exposureTime #define block.
            /// </summary>
            public UInt32 exposureTime;
            /// <summary>
            /// see also: <seealso cref="ABG_STATE7"/> enum.
            /// </summary>
            public ABG_STATE7 abgState;

            /// <summary>
            /// see also: <seealso cref="SHUTTER_COMMAND"/> enum.
            /// </summary>
            public SHUTTER_COMMAND openShutter;

        };

        /// <summary>
        /// Expanded parameters structure used to start SBIG camera exposures.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct StartExposureParams2 : IParams
        {
            /// <summary>
            /// Requested CCD. see also: <seealso cref="CCD_REQUEST"/> enum.
            /// </summary>
            public CCD_REQUEST ccd;

            /// <summary>
            /// Exposure time in hundredths of a second in least significant 24 bits. Most significant bits are bit-flags described in exposureTime #define block.
            /// </summary>
            public UInt32 exposureTime;

            /// <summary>
            /// Deprecated. See also: <seealso cref="ABG_STATE7"/>.
            /// </summary>
            public ABG_STATE7 abgState;

            /// <summary>
            /// see also: <seealso cref="SHUTTER_COMMAND"/> enum.
            /// </summary>
            public SHUTTER_COMMAND openShutter;

            /// <summary>
            /// readout mode. See also: <seealso cref="READOUT_BINNING_MODE"/> enum.
            /// </summary>
            public READOUT_BINNING_MODE readoutMode;

            /// <summary>
            /// top-most row to read out. (0 based)
            /// </summary>
            public UInt16 top;
            /// <summary>
            /// left-most column to read out. (0 based)
            /// </summary>
            public UInt16 left;
            /// <summary>
            /// image height in binned pixels.
            /// </summary>
            public UInt16 height;
            /// <summary>
            /// image width in binned pixels.
            /// </summary>
            public UInt16 width;
        };

        /// <summary>
        /// Parameters used to end SBIG camera exposures.
        /// <para>Set ABORT_DONT_END flag in ccd to abort exposures in supported cameras.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct EndExposureParams : IParams
        {
            /// <summary>
            /// Requested CCD. see also: <seealso cref="CCD_REQUEST"/> enum.
            /// </summary>
            public CCD_REQUEST ccd;

        };

        /// <summary>
        /// Parameters used to readout lines of SBIG cameras during readout.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ReadoutLineParams : IParams
        {
            /// <summary>
            /// Requested CCD. see also: <seealso cref="CCD_REQUEST"/> enum.
            /// </summary>
            public CCD_REQUEST ccd;

            /// <summary>
            /// readout mode. See also: <seealso cref="READOUT_BINNING_MODE"/> enum.
            /// </summary>
            public READOUT_BINNING_MODE readoutMode;

            /// <summary>
            /// left-most pixel to read out.
            /// </summary>
            public UInt16 pixelStart;
            /// <summary>
            /// number of pixels to digitize.
            /// </summary>
            public UInt16 pixelLength;
        };

        /// <summary>
        /// Parameters used to dump/flush CCD lines during readout.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct DumpLinesParams : IParams
        {
            /// <summary>
            /// Requested CCD. see also: <seealso cref="CCD_REQUEST"/> enum.
            /// </summary>
            public CCD_REQUEST ccd;

            /// <summary>
            /// readout mode. See also: <seealso cref="READOUT_BINNING_MODE"/> enum.
            /// </summary>
            public READOUT_BINNING_MODE readoutMode;

            /// <summary>
            /// number of lines to dump.
            /// </summary>
            public UInt16 lineLength;
        };

        /// <summary>
        /// Parameters used to end SBIG camera readout.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct EndReadoutParams : IParams
        {
            /// <summary>
            /// Requested CCD. see also: <seealso cref="CCD_REQUEST"/> enum.
            /// </summary>
            public CCD_REQUEST ccd;

        };

        /// <summary>
        /// (Optional) Parameters used to start SBIG camera readout.
        /// <para>Automatically dumps unused exposure lines.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct StartReadoutParams : IParams
        {
            /// <summary>
            /// Requested CCD. see also: <seealso cref="CCD_REQUEST"/> enum.
            /// </summary>
            public CCD_REQUEST ccd;

            /// <summary>
            /// readout mode. See also: <seealso cref="READOUT_BINNING_MODE"/> enum.
            /// </summary>
            public READOUT_BINNING_MODE readoutMode;

            /// <summary>
            /// top-most row to read out. (0 based)
            /// </summary>
            public UInt16 top;
            /// <summary>
            /// left-most column to read out. (0 based)
            /// </summary>
            public UInt16 left;
            /// <summary>
            /// image height in binned pixels.
            /// </summary>
            public UInt16 height;
            /// <summary>
            /// image width in binned pixels.
            /// </summary>
            public UInt16 width;
        };


        /// <summary>
        /// The Set Temperature Regulation command is used to enable or disable the CCD's temperature regulation. Uses special units for the CCD temperature. 
        /// The <seealso cref="SetTemperatureRegulationParams2"/> described in the next section is easier to use with temperatures stated in Degrees Celsius.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SetTemperatureRegulationParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="TEMPERATURE_REGULATION"/> enum.
            /// </summary>
            public TEMPERATURE_REGULATION regulation;

            /// <summary>
            /// CCD temperature setpoint in A/D units if regulation on or TE drive level (0-255 = 0-100%) if regulation override.
            /// </summary>
            public UInt16 ccdSetpoint;
        };

        /// <summary>
        /// The Set Temperature Regulation 2 command is used to enable or disable the CCD's temperature
        /// <para>regulation using temperatures in Degrees C instead of the funny A/D units described above.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SetTemperatureRegulationParams2 : IParams
        {
            /// <summary>
            /// see also: <seealso cref="TEMPERATURE_REGULATION"/> enum.
            /// </summary>
            public TEMPERATURE_REGULATION regulation;

            /// <summary>
            /// CCD temperature setpoint in degrees Celsius.
            /// </summary>
            public double ccdSetpoint;
        };

        /// <summary>
        /// The Query Temperature Status command is used to monitor the CCD's temperature regulation. 
        /// The original version of this command took no Parameters (a NULL pointer) but the command has been expanded to allow a more user friendly result. 
        /// If you pass a NULL pointer in the Parameters variable you'll get the classic result. 
        /// If you pass a pointer to a QueryTemperatureStatusParams struct you'll have access to the expanded results.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryTemperatureStatusParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="QUERY_TEMP_STATUS_REQUEST"/> enum.
            /// </summary>
            public QUERY_TEMP_STATUS_REQUEST request;
        };

        /// <summary>
        /// The results struct of a Temperature Status Query, with request set to TEMP_STATUS_STANDARD.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryTemperatureStatusResults : IResults
        {
            /// <summary>
            /// temperature regulation is enabled when this is TRUE.
            /// </summary>
            public MY_LOGICAL enabled;
            /// <summary>
            /// CCD temperature or thermistor setpoint in A/D units.
            /// </summary>
            public UInt16 ccdSetpoint;
            /// <summary>
            /// this is the power being applied to the TE cooler to maintain temperature regulation and is in the range 0 thru 255.
            /// </summary>
            public UInt16 power;
            /// <summary>
            /// this is the CCD thermistor reading in A/D units.
            /// </summary>
            public UInt16 ccdThermistor;
            /// <summary>
            /// this is the ambient thermistor reading in A/D units.
            /// </summary>
            public UInt16 ambientThermistor;
        };

        /// <summary>
        /// The results struct of a Temperature Status Query, with request set to TEMP_STATUS_ADVANCED.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryTemperatureStatusResults2 : IResults
        {
            /// <summary>
            /// temperature regulation is enabled when this is TRUE. &amp;REGULATION_FROZEN_MASK is TRUE when TE is frozen.
            /// </summary>
            public MY_LOGICAL coolingEnabled;
            /// <summary>
            /// fan state and is one of the following: FS_OFF (off), FS_ON (manual control) or FS_AUTOCONTROL (auto speed control).
            /// </summary>
            public MY_LOGICAL fanEnabled;
            /// <summary>
            /// CCD Setpoint temperature in °C.
            /// </summary>
            public double ccdSetpoint;
            /// <summary>
            /// imaging CCD temperature in degrees °C.
            /// </summary>
            public double imagingCCDTemperature;
            /// <summary>
            /// tracking CCD temperature in degrees °C.
            /// </summary>
            public double trackingCCDTemperature;
            /// <summary>
            /// external tracking CCD temperature in °C.
            /// </summary>
            public double externalTrackingCCDTemperature;
            /// <summary>
            /// ambient camera temperature in °C.
            /// </summary>
            public double ambientTemperature;
            /// <summary>
            /// percent power applied to the imaging CCD TE cooler.
            /// </summary>
            public double imagingCCDPower;
            /// <summary>
            /// percent power applied to the tracking CCD TE cooler.
            /// </summary>
            public double trackingCCDPower;
            /// <summary>
            /// percent power applied to the external tracking TE cooler.
            /// </summary>
            public double externalTrackingCCDPower;
            /// <summary>
            /// imaging CCD heatsink temperature in °C.
            /// </summary>
            public double heatsinkTemperature;
            /// <summary>
            /// percent power applied to the fan.
            /// </summary>
            public double fanPower;
            /// <summary>
            /// fan speed in RPM.
            /// </summary>
            public double fanSpeed;
            /// <summary>
            /// tracking CCD Setpoint temperature in °C.
            /// </summary>
            public double trackingCCDSetpoint;
        };

        /// <summary>
        /// The Activate Relay command is used to activate one or more of the telescope control outputs or to cancel an activation in progress.
        /// <para>The status for this command (from <seealso cref="PAR_COMMAND.CC_QUERY_COMMAND_STATUS"/>) consists of four bit fields:</para>
        /// <para>b3 = +X Relay, 0=Off, 1= Active</para>
        /// <para>b2 = -X Relay, 0=Off, 1= Active</para>
        /// <para>b1 = +Y Relay, 0=Off, 1= Active</para>
        /// <para>b0 = -Y Relay, 0=Off, 1= Active</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ActivateRelayParams : IParams
        {
            /// <summary>
            /// x plus activation duration in hundredths of a second
            /// </summary>
            public UInt16 tXPlus;
            /// <summary>
            /// x minus activation duration in hundredths of a second
            /// </summary>
            public UInt16 tXMinus;
            /// <summary>
            /// y plus activation duration in hundredths of a second
            /// </summary>
            public UInt16 tYPlus;
            /// <summary>
            /// y minus activation duration in hundredths of a second
            /// </summary>
            public UInt16 tYMinus;
        };

        /// <summary>
        /// The Pulse Out command is used with the ST-7/8/etc to position the CFW-6A/CFW-8 and with the PixCel255 and PixCel237 to position the internal vane/filter wheel.
        /// <para>The status for this command is: </para>
        /// <para>b0 - Normal status, 0 = inactive, 1 = pulse out in progress</para>
        /// <para>b1-b3 - PixCel255/237 Filter state, 0=moving, 1-5=at position 1-5, 6=unknown</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct PulseOutParams : IParams
        {
            /// <summary>
            /// number of pulses to generate (0 thru 255).
            /// </summary>
            public UInt16 numberPulses;
            /// <summary>
            /// width of pulses in units of microseconds with a minimum of 9 microseconds.
            /// </summary>
            public UInt16 pulseWidth;
            /// <summary>
            /// period of pulses in units of microseconds with a minimum of 29 plus the pulseWidth microseconds.
            /// </summary>
            public UInt16 pulsePeriod;
        };

        /// <summary>
        /// The TX Serial Bytes command is for internal use by SBIG.
        /// It's a very low level version of commands like AO Tip Tilt that are used to send data out the ST-7/8/etc's telescope port to accessories like the AO-7. 
        /// There're no reason why you should need to use this command. 
        /// Just use the dedicated commands like AO Tip Tilt.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct TXSerialBytesParams : IParams
        {
            /// <summary>
            /// Length of data buffer to send
            /// </summary>
            public UInt16 dataLength;
            /// <summary>
            /// Buffer of data to send.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public Byte[] data;
        };

        /// <summary>
        /// Transfer Serial Bytes command results.
        /// <para>Results of a TXSerialBytes command.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct TXSerialBytesResults : IResults
        {
            /// <summary>
            /// Bytes sent out.
            /// </summary>
            public UInt16 bytesSent;
        };

        /// <summary>
        /// The Get Serial Status command is for internal use by SBIG. 
        /// It's a very low level version of commands like AO Tip Tilt that are used to send data out the ST-7/8/etc's telescope port to accessories like the AO-7. 
        /// There're no reason why you should need to use this command. 
        /// Just use the dedicated commands like AO Tip Tilt.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetSerialStatusResults : IResults
        {
            /// <summary>
            /// Clear to COM.
            /// </summary>
            public MY_LOGICAL clearToCOM;
        };

        /// <summary>
        /// The Establish Link command is used by the application to establish a communications link with the camera.
        /// It should be used before any other commands are issued to the camera (excluding the Get Driver Info command).
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct EstablishLinkParams : IParams
        {
            /// <summary>
            /// Maintained for historical purposes. Keep set to 0.
            /// </summary>
            public UInt16 sbigUseOnly;
        };

        /// <summary>
        /// Results from an EstablishLink command.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct EstablishLinkResults : IResults
        {
            /// <summary>
            /// Returns connected camera's type ID. See also: <seealso cref="CAMERA_TYPE"/> enum. 
            /// </summary>
            public CAMERA_TYPE cameraType;

        };

        /// <summary>
        /// The Get Driver Info command is used to determine the version and capabilities of the DLL/Driver. 
        /// For future expandability this command allows you to request several types of information. 
        /// Initially the standard request and extended requests will be supported but as the driver evolves additional requests will be added.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetDriverInfoParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="DRIVER_REQUEST"/> enum.
            /// </summary>
            public DRIVER_REQUEST request;

        };

        /// <summary>
        /// Standard, Extended and USB Loader Results Struct.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetDriverInfoResults0 : IResults
        {
            /// <summary>
            /// driver version in BCD with the format XX.XX
            /// </summary>
            public UInt16 version;
            /// <summary>
            /// driver name, null terminated string
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string name;
            /// <summary>
            /// maximum request response available from this driver
            /// </summary>
            public UInt16 maxRequest;
        };

        /// <summary>
        /// The Get CCD Info command is used by the application to determine the model of camera being controlled and its capabilities. 
        /// For future expandability this command allows you to request several types of information. 
        /// Currently 6 standard requests are supported but as the driver evolves additional requests will be added.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetCCDInfoParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="CCD_INFO_REQUEST"/>.
            /// </summary>
            public CCD_INFO_REQUEST request;
        };

        /// <summary>
        /// Internal structure for storing readout modes.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct READOUT_INFO
        {
            /// <summary>
            /// readout mode ID (see also: <seealso cref="READOUT_BINNING_MODE"/>)
            /// </summary>
            public READOUT_BINNING_MODE mode;

            /// <summary>
            /// width of image in pixels
            /// </summary>
            public UInt16 width;
            /// <summary>
            /// height of image in pixels
            /// </summary>
            public UInt16 height;
            /// <summary>
            /// a four digit BCD number specifying the amplifier gain in e-/ADU in XX.XX format
            /// </summary>
            public UInt16 gain;
            /// <summary>
            /// an eight digit BCD number specifying the pixel width in microns in the XXXXXX.XX format
            /// </summary>
            public UInt32 pixel_width;
            /// <summary>
            /// an eight digit BCD number specifying the pixel height in microns in the XXXXXX.XX format
            /// </summary>
            public UInt32 pixel_height;
        };

        /// <summary>
        /// Get CCD Info command results 0 and 1 request.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetCCDInfoResults0 : IResults
        {
            /// <summary>
            /// version of the firmware in the resident microcontroller in BCD format (XX.XX, 0x1234 = 12.34).
            /// </summary>
            public UInt16 firmwareVersion;
            /// <summary>
            /// Camera type ID. see also: <seealso cref="CAMERA_TYPE"/> enum.
            /// </summary>
            public CAMERA_TYPE cameraType;

            /// <summary>
            /// null terminated string containing the name of the camera.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string name;
            /// <summary>
            /// number of readout modes supported.
            /// </summary>
            public UInt16 readoutModes;
            /// <summary>
            /// number of readout modes supported.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 20)]
            public READOUT_INFO[] readoutInfo;
        };

        /// <summary>
        /// Get CCD Info command results second request.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetCCDInfoResults2 : IResults
        {
            /// <summary>
            /// number of bad columns in imaging CCD
            /// </summary>
            public UInt16 badColumns;
            /// <summary>
            /// bad columns
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] columns;
            /// <summary>
            /// type of Imaging CCD, 0= No ABG Protection, 1 = ABG Present.
            /// see also: <seealso cref="IMAGING_ABG"/> enum.
            /// </summary>
            public IMAGING_ABG imagingABG;

            /// <summary>
            /// null terminated serial number string
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public string serialNumber;
        };

        /// <summary>
        /// Get CCD Info command results third request. (For the PixCel255/237)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetCCDInfoResults3 : IResults
        {
            /// <summary>
            /// 0 = Unknown, 1 = 12 bits, 2 = 16 bits. see also: <seealso cref="AD_SIZE"/> enum.
            /// </summary>
            public AD_SIZE adSize;

            /// <summary>
            /// 0 = Unknown, 1 = External, 2 = 2 Position, 3 = 5 Position. see also: <seealso cref="FILTER_TYPE"/> enum. 
            /// </summary>
            public FILTER_TYPE filterType;

        };

        /// <summary>
        /// Get CCD Info command results fourth and fifth request. (For all cameras)
        /// <para>Capabilities bits:</para>
        /// <para>b0: 0 = CCD is Full Frame Device, 1 = CCD is Frame Transfer Device,</para>
        /// <para>b1: 0 = No Electronic Shutter, 1 = Interline Imaging CCD with Electronic Shutter and millisecond exposure capability</para>
        /// <para>b2: 0 = No hardware support for external Remote Guide Head, 1 = Detected hardware support for external Remote Guide Head.</para>
        /// <para>b3: 1 = Supports the special Biorad TDI acquisition mode.</para>
        /// <para>b4: 1 = AO8 detected.</para>
        /// <para>b5: 1 = Camera contains an internal frame buffer.</para>
        /// <para>b6: 1 = Camera requires the StartExposure2 command instead of the older depricated StartExposure command.</para>
        /// <para>Other: See the CB_XXX_XXX definitions in the sbigurdv.h header file.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetCCDInfoResults4 : IResults
        {
            /// <summary>
            /// Camera capabilities. See the CB_XXX_XXX definitions in the sbigurdv.h header file.
            /// </summary>
            public UInt16 capabilitiesBits;
            /// <summary>
            /// Number of unbinned rows to dump to transfer image area to storage area.
            /// </summary>
            public UInt16 dumpExtra;
        };

        /// <summary>
        /// Get CCD Info command results sixth request. (For all cameras)
        /// <para>Camera bits:</para>
        /// <para>b0: 0 = STX camera, 1 = STXL camera</para>
        /// <para>b1: 0 = Mechanical shutter, 1 = No mechanical shutter (only an electronic shutter)</para>
        /// <para>b2 ?b31: reserved for future expansion</para>
        /// <para>CCD Bits:</para>
        /// <para>b0: 0 = Imaging Mono CCD, 1 = Imaging Color CCD</para>
        /// <para>b1: 0 = Bayer color matrix, 1 = Truesense color matrix</para>
        /// <para>b2 ?b31: reserved for future expansion</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetCCDInfoResults6 : IResults
        {
            /// <summary>
            /// Set of bits for additional camera capabilities
            /// </summary>
            public UInt32 cameraBits;
            /// <summary>
            /// Set of bits for additional CCD capabilities
            /// </summary>
            public UInt32 ccdBits;
            /// <summary>
            /// Set of bits for additional capabilities
            /// </summary>
            public UInt32 extraBits;
        };

        /// <summary>
        /// The Query Command Status command is used to monitor the progress of a previously requested command.
        /// Typically this will be used to monitor the progress of an exposure, relay closure or CFW-6A move command.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryCommandStatusParams : IParams
        {
            /// <summary>
            /// command of which the status is desired. 
            /// See also <seealso cref="PAR_COMMAND"/> enum.
            /// </summary>
            public PAR_COMMAND command;
        };

        /// <summary>
        /// Results for the Query Command Status command.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryCommandStatusResults : IResults
        {
            /// <summary>
            /// command status. See also <seealso cref="PAR_ERROR"/> enum.
            /// </summary>
            public PAR_ERROR status;
        };

        /// <summary>
        /// Results for the Query Command Status command.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryCommandStatusResults2 : IResults
        {
            /// <summary>
            /// command status. 
            /// </summary>
            public UInt16 status;
            /// <summary>
            /// expanded information on command status.
            /// </summary>
            public UInt16 info;
        };

        /// <summary>
        /// The Miscellaneous Control command is used to control the Fan, LED, and shutter.
        /// The camera powers up with the Fan on, the LED on solid, and the shutter closed.
        /// The driver flashes the LED at the low rate while the Imaging CCD is integrating, flashes the LED at the high rate while the Tracking CCD is integrating and sets it on solid during the readout.
        /// <para>The status returned for this command from Query Command Status has the following structure:</para>
        /// <para>b7-b0 - Shutter edge - This is the position the edge of the shutter was detected at for the last shutter move. Normal values are 7 thru 9. Any other value including 255 indicates a shutter failure and the shutter should be reinitialized.</para>
        /// <para>b8 - the Fan is enabled when this bit is 1</para>
        /// <para>b10b9 - Shutter state, 0=open, 1=closed, 2=opening, 3=closing</para>
        /// <para>b12b11 - LED state, 0=off, 1=on, 2=blink low, 3=blink high</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct MiscellaneousControlParams : IParams
        {
            /// <summary>
            /// set TRUE to turn on the Fan.
            /// </summary>
            public MY_LOGICAL fanEnable;
            /// <summary>
            /// see also: <seealso cref="SHUTTER_COMMAND"/> enum.
            /// </summary>
            public SHUTTER_COMMAND shutterCommand;

            /// <summary>
            /// see also: <seealso cref="LED_STATE"/> enum.
            /// </summary>
            public LED_STATE ledState;

        };

        /// <summary>
        /// The Read Offset command is used to measure the CCD's offset.
        /// In the SBIG cameras the offset is adjusted at the factory and this command is for testing or informational purposes only.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ReadOffsetParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="CCD_REQUEST"/> enum.
            /// </summary>
            public CCD_REQUEST ccd;

        };

        /// <summary>
        /// Results structure for the Read Offset command.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ReadOffsetResults : IResults
        {
            /// <summary>
            /// the CCD's offset.
            /// </summary>
            public UInt16 offset;
        };

        /// <summary>The Read Offset 2 command is used to measure the CCD's offset and the noise in the readout register.
        /// In the SBIG cameras the offset is adjusted at the factory and this command is for testing or informational purposes only.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ReadOffsetResults2 : IResults
        {
            /// <summary>
            /// the CCD's offset.
            /// </summary>
            public UInt16 offset;
            /// <summary>
            /// noise in the ccd readout register in ADUs rms.
            /// </summary>
            public double rms;
        };

        /// <summary>
        /// The AO Tip Tilt Command is used to position an AO-7 attached to the telescope port of an ST-7/8/etc.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AOTipTiltParams : IParams
        {
            /// <summary>
            /// this is the desired position of the mirror in the X axis.
            /// </summary>
            public UInt16 xDeflection;
            /// <summary>
            /// this is the desired position of the mirror in the Y axis
            /// </summary>
            public UInt16 yDeflection;
        };

        /// <summary>
        /// This command is reserved for future use with motorized focus units.
        /// Prototypes of the AO-7 had motorized focus but the feature was removed in the production units. 
        /// This command is a holdover from that.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AOSetFocusParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="AO_FOCUS_COMMAND"/> enum.
            /// </summary>
            public AO_FOCUS_COMMAND focusCommand;

        };

        /// <summary>
        /// The AO Delay Command is used to generate millisecond type delays for exposing the Tracking CCD.
        /// <para>This sleep command is blocking.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AODelayParams : IParams
        {
            /// <summary>
            /// this is the desired delay in microseconds.
            /// </summary>
            public UInt32 delay;
        };

        /// <summary>
        /// The current driver does not use this command.
        /// It was added in a previous version and never removed.
        /// It could be reassigned in the future.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetTurboStatusResults : IResults
        {
            /// <summary>
            /// TRUE if turbo is detected.
            /// </summary>
            public MY_LOGICAL turboDetected;
        };

        /// <summary>
        /// The Open Device command is used to load and initialize the low-level driver. 
        /// You will typically call this second (after Open Driver).
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct OpenDeviceParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="SBIG_DEVICE_TYPE"/> enum. specifies LPT, Ethernet, etc.
            /// </summary>
            public SBIG_DEVICE_TYPE deviceType;

            /// <summary>
            /// for deviceType::DEV_LPTN: Windows 9x Only, Win NT uses deviceSelect.
            /// </summary>
            public UInt16 lptBaseAddress;
            /// <summary>
            /// for deviceType::DEV_ETH: Ethernet address.
            /// </summary>
            public UInt32 ipAddress;

            /// <summary>
            /// Create an <seealso cref="OpenDeviceParams"/> using an IP string.
            /// </summary>
            /// <param name="IP">IP address</param>
            public OpenDeviceParams(string IP) : this()
            {
                try
                {
                    // try to parse as an IP address
                    byte[] b = IPAddress.Parse(IP).GetAddressBytes();
                    ipAddress =
                        (((uint)b[0]) << 24) |
                        (((uint)b[1]) << 16) |
                        (((uint)b[2]) << 08) |
                        (((uint)b[3]) << 00);
                    deviceType = SBIG_DEVICE_TYPE.DEV_ETH;
                }
                catch (FormatException)
                {
                    throw new ArgumentException("Must pass either an IP address.");
                }
            }
        };

        /// <summary>
        /// This command allows you to control the IRQ priority of the driver under Windows NT/2000/XP. 
        /// The default settings should work fine for all users and these commands should not need to be used.
        /// We use three settings in our CCDOPS software: High = 27, Medium = 15, Low = 2. 
        /// Under fast machines Low will work fine. On slower machines the mouse may get sluggish unless you select the Medium or High priority.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SetIRQLParams : IParams
        {
            /// <summary>
            /// IRQ Level.
            /// </summary>
            public UInt16 level;
        };

        /// <summary>
        /// Results of Get IRQ Level command.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetIRQLResults : IResults
        {
            /// <summary>
            /// IRQ Level.
            /// </summary>
            public UInt16 level;
        };

        /// <summary>
        /// This command returns the status of the communications link established with the camera.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetLinkStatusResults : IResults
        {
            /// <summary>
            /// TRUE when a link has been established
            /// </summary>
            public MY_LOGICAL linkEstablished;
            /// <summary>
            /// base address of the LPT port.
            /// </summary>
            public UInt16 baseAddress;
            /// <summary>
            /// see also: <seealso cref="CAMERA_TYPE"/> enum.
            /// </summary>
            public CAMERA_TYPE cameraType;

            /// <summary>
            /// total number of communications with camera.
            /// </summary>
            public UInt32 comTotal;
            /// <summary>
            /// total number of failed communications with camera.
            /// </summary>
            public UInt32 comFailed;
        };

        /// <summary>
        /// This command is of extremely limited (and unknown) use.
        /// When you have established a link to a parallel port based camera under Windows NT/2000/XP this command returns a counter with 1 microsecond resolution.
        /// Under all other circumstances the counter is zero.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetUSTimerResults : IResults
        {
            /// <summary>
            /// counter value in microseconds.
            /// </summary>
            public UInt32 count;
        };

        /// <summary>
        /// \internal
        /// <para>Intended for SBIG internal use only. Unimplemented.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct SendBlockParams : IParams
        {
            /// <summary>
            /// Destination port.
            /// </summary>
            public UInt16 port;
            /// <summary>
            /// Length of data buffer.
            /// </summary>
            public UInt16 length;
            //TODO: unsigned char* 檢查移植是否正確
            /// <summary>
            /// Buffer of data to send.
            /// </summary>
            public UIntPtr source;
        };

        /// <summary>
        /// \internal
        /// <para>Intended for SBIG internal use only. Unimplemented.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct SendByteParams : IParams
        {
            /// <summary>
            /// Destination port.
            /// </summary>
            public UInt16 port;
            /// <summary>
            /// Buffer of data to send.
            /// </summary>
            public UInt16 data;
        };

        /// <summary>
        /// \internal
        /// <para>Intended for SBIG internal use only. Clock the AD the number of times passed.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ClockADParams : IParams
        {
            /// <summary>
            /// CCD to clock. see also: <seealso cref="CCD_REQUEST"/> enum. (Unused)
            /// </summary>
            public CCD_REQUEST ccd;

            /// <summary>
            /// Readout mode. see also: <seealso cref="READOUT_BINNING_MODE"/> enum. (Unused)
            /// </summary>
            public READOUT_BINNING_MODE readoutMode;

            /// <summary>
            /// Starting pixel. (Unused)
            /// </summary>
            public UInt16 pixelStart;
            /// <summary>
            /// Count of cycles to pass.
            /// </summary>
            public UInt16 pixelLength;
        };

        /// <summary>
        /// \internal
        /// <para>Intended for SBIG internal use only. Pass the SystemTest command to the micro.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SystemTestParams : IParams
        {
            /// <summary>
            /// Flag TRUE to test the clocks.
            /// </summary>
            public UInt16 testClocks;
            /// <summary>
            /// Flag TRUE to test the motors.
            /// </summary>
            public UInt16 testMotor;
            /// <summary>
            /// Flag TRUE to test 5800 (???).
            /// </summary>
            public UInt16 test5800;
            /// <summary>
            /// Flag true to align STL (???).
            /// </summary>
            public UInt16 stlAlign;
            /// <summary>
            /// Flag true for motor always on (???).
            /// </summary>
            public UInt16 motorAlwaysOn;
        };

        /// <summary>
        /// \internal
        /// <para>Intended for SBIG internal use only. Unused.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct SendSTVBlockParams : IParams
        {
            /// <summary>
            /// Outgoing buffer length.
            /// </summary>
            public UInt16 outLength;
            //TODO: unsigned char* 檢查移植是否正確
            /// <summary>
            /// Outgoing buffer.
            /// </summary>
            public UIntPtr outPtr;
            /// <summary>
            /// Incoming buffer length.
            /// </summary>
            public UInt16 inLength;
            //TODO: unsigned char* 檢查移植是否正確
            /// <summary>
            /// Incoming buffer.
            /// </summary>
            public UIntPtr inPtr;
        };

        /// <summary>
        /// This command returns a null terminated C string in English (not Unicode) corresponding to the passed
        /// <para>error number. It's handy for reporting driver level errors to the user.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetErrorStringParams : IParams
        {
            /// <summary>
            /// Error code. see also: <seealso cref="PAR_ERROR"/> enum.
            /// </summary>
            public PAR_ERROR errorNo;

        };

        /// <summary>
        /// This command returns a null terminated C string in English (not Unicode) corresponding to the passed
        /// <para>error number. It's handy for reporting driver level errors to the user.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetErrorStringResults : IResults
        {
            /// <summary>
            /// Error string in english (not unicode).
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string errorString;
        };


        /// <summary>
        /// The Get/Set Driver Handle commands are for use by applications that wish to talk to multiple cameras on various ports at the same time.
        /// If your software only wants to talk to one camera at a time you can ignore these commands.
        /// </summary>
        /// <remarks>
        /// The Get Driver Handle command takes a NULL Parameters pointer and a pointer to a
        /// GetDriverHandleResults struct for Results. The Set Driver Handle command takes a pointer to a
        /// SetDriverHandleParams struct for Parameters and a NULL pointer for Results. To establish links to
        /// multiple cameras do the following sequence:
        /// * Call Open Driver for Camera 1
        /// * Call Open Device for Camera 1
        /// * Call Establish Link for Camera 1
        /// * Call Get Driver Handle and save the result as Handle1
        /// * Call Set Driver Handle with INVALID_HANDLE_VALUE in the handle parameter
        /// * Call Open Driver for Camera 2
        /// * Call Open Device for Camera 2
        /// * Call Establish Link for Camera 2
        /// * Call Get Driver Handle and save the result as Handle2
        /// Then, when you want to talk to Camera 1, call Set Driver Handle with Handle1 and when you want to
        /// talk to Camera 2, call Set Driver Handle with Handle2. To shut down you must call Set Driver Handle,
        /// Close Device and Close Driver in that sequence for each camera.
        /// Each time you call Set Driver Handle with INVALID_HANDLE_VALUE you are allowing access to an
        /// additional camera up to a maximum of four cameras. These cameras can be on different LPT ports,
        /// multiple USB4 cameras or at different Ethernet addresses. There is a restriction though due to memory
        /// considerations. You can only have a single readout in process at a time for all cameras and CCDs within
        /// a camera. Readout begins with the Start Readout or Readout Line commands and ends with the End
        /// Readout command. If you try to do multiple interleaved readouts the data from the multiple cameras
        /// will be commingled. To avoid this, simply readout one camera/CCD at a time in an atomic process.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SetDriverHandleParams : IParams
        {
            /// <summary>
            /// Handle to driver.
            /// </summary>
            public Int16 handle;
        };

        /// <summary>The Get/Set Driver Handle commands are for use by applications that wish to talk to multiple cameras on various ports at the same time.
        /// If your software only wants to talk to one camera at a time you can ignore these commands.
        /// </summary>
        /// <remarks>
        /// The Get Driver Handle command takes a NULL Parameters pointer and a pointer to a
        /// GetDriverHandleResults struct for Results. The Set Driver Handle command takes a pointer to a
        /// SetDriverHandleParams struct for Parameters and a NULL pointer for Results. To establish links to
        /// multiple cameras do the following sequence:
        /// * Call Open Driver for Camera 1
        /// * Call Open Device for Camera 1
        /// * Call Establish Link for Camera 1
        /// * Call Get Driver Handle and save the result as Handle1
        /// * Call Set Driver Handle with INVALID_HANDLE_VALUE in the handle parameter
        /// * Call Open Driver for Camera 2
        /// * Call Open Device for Camera 2
        /// * Call Establish Link for Camera 2
        /// * Call Get Driver Handle and save the result as Handle2
        /// Then, when you want to talk to Camera 1, call Set Driver Handle with Handle1 and when you want to
        /// talk to Camera 2, call Set Driver Handle with Handle2. To shut down you must call Set Driver Handle,
        /// Close Device and Close Driver in that sequence for each camera.
        /// Each time you call Set Driver Handle with INVALID_HANDLE_VALUE you are allowing access to an
        /// additional camera up to a maximum of four cameras. These cameras can be on different LPT ports,
        /// multiple USB4 cameras or at different Ethernet addresses. There is a restriction though due to memory
        /// considerations. You can only have a single readout in process at a time for all cameras and CCDs within
        /// a camera. Readout begins with the Start Readout or Readout Line commands and ends with the End
        /// Readout command. If you try to do multiple interleaved readouts the data from the multiple cameras
        /// will be commingled. To avoid this, simply readout one camera/CCD at a time in an atomic process.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetDriverHandleResults : IResults
        {
            /// <summary>
            /// Handle to driver.
            /// </summary>
            public Int16 handle;
        };

        /// <summary>
        /// This command is used to modify the behavior of the driver by changing the settings of one of the driver control parameters. 
        /// Driver options can be enabled or disabled with this command.
        /// There is one set of parameters for the whole DLL vs. one per handle.
        /// </summary>
        /// <remarks>
        /// * The DCP_USB_FIFO_ENABLE parameter defaults to TRUE and can be set FALSE to disable
        ///   the FIFO and associated pipelining in the USB cameras. You would do this for example in
        ///   applications using Time Delay Integration (TDI) where you don't want data in the CCD digitized
        ///   until the actual call to ReadoutLine is made.
        /// * The DCP_CALL_JOURNAL_ENABLE parameter defaults to FALSE and can be set to TRUE
        ///   to have the driver broadcast Driver API calls. These broadcasts are handy as a debug tool for
        ///   monitoring the sequence of API calls made to the driver. The broadcasts can be received and
        ///   displayed with the Windows based SBIGUDRVJournalRx.exe application.
        ///   Only use this for testing purposes and do not enabled this feature in your released version of you
        ///   application as the journaling mechanism can introduce minor artifacts in the readout.
        /// * The DCP_IVTOH_RATIO parameter sets the number of Vertical Rows that are dumped (fast)
        ///   before the Horizontal Register is dumped (not as fast) in the DumpRows command for Parallel
        ///   Port based cameras. This is a very specialized parameter and you should think hard about
        ///   changing it if you do. The default of 5 for the IHTOV_RATIO has been determined to offer a
        ///   good compromise between the time it takes to clear the CCD or Dump Rows and the ability to
        ///   effectively clear the CCD after imaging a bright object. Finally should you find it necessary to
        ///   change it read the current setting and restore it when you're done.
        /// * The DCP_USB_FIFO_SIZE parameter sets the size of the FIFO used to receive data from USB
        ///   cameras. The default and maximum value of 16384 yields the highest download speeds.
        ///   Lowering the value will cause the camera to digitize and download pixels in smaller chunks.
        ///   Again this is a specialized parameter that 99.9% of programs out there will have no need for
        ///   changing.
        /// * The DCP_USB_PIXEL_DL_ENABLE parameter allows disabling the actual downloading of
        ///   pixel data from the camera for testing purposes. This parameter defaults to TRUE.
        /// * The DCP_HIGH_THROUGHPUT parameter allows configuring the driver for the highest
        ///   possible imaging throughput at the expense of image noise and or artifacts. This parameter
        ///   defaults to FALSE and you should only enable this for short periods of time. You might use this
        ///   in Focus mode for example to get higher image throughput but you should never use it when you
        ///   are taking keeper images. It does things that avoid timed delays in the camera like leaving the
        ///   shutter motor on all the time, etc. At this time this feature is supported in the driver but not all
        ///   cameras show a benefit from its use.
        /// * The DCP_VDD_OPTIMIZED parameter defaults to TRUE which lowers the CCD's Vdd (which
        ///   reduces amplifier glow) only for images 3 seconds and longer. This was done to increase the
        ///   image throughput for short exposures as raising and lowering Vdd takes 100s of milliseconds.
        ///   The lowering and subsequent raising of Vdd delays the image readout slightly which causes short
        ///   exposures to have a different bias structure than long exposures. Setting this parameter to
        ///   FALSE stops the short exposure optimization from occurring.
        /// * The DCP_AUTO_AD_GAIN parameter defaults to TRUE whereby the driver is responsible for
        ///   setting the A/D gain in USB cameras. Setting this to FALSE allows overriding the driver
        ///   imposed A/D gains.
        /// * The DCP_NO_HCLKS_FOR_INTEGRATION parameter defaults to FALSE and setting it to
        ///   TRUE disables the horizontal clocks during exposure integration and is intended for SBIG
        ///   testing only.
        /// * The DCP_TDI_MODE_ENABLE parameter defaults to FALSE and setting it to TRUE enables
        ///   the special Biorad TDI mode.
        /// * The DCP_VERT_FLUSH_CONTROL_ENABLE parameter defaults to TRUE and setting it to
        ///   FALSE it disables the background flushing of the vertical clocks of KAI CCDs during exposure
        ///   integration and is intended for SBIG testing only.
        /// * The DCP_ETHERNET_PIPELINE_ENABLE parameter defaults to FALSE and setting it to
        ///   TRUE can increase the throughput of Ethernet based cameras like the STX &amp; STT but doing so
        ///   is not recommended for robust operation.
        /// * The DCP_FAST_LINK parameter defaults to FALSE and setting it to TRUE speeds up the
        ///   Establish Link command by not dumping the pixel FIFOs in the camera, It is used internally to
        ///   speed up the Query USB and Query Ethernet commands.
        /// * The DCP_COLUMN_REPAIR_ENABLE defaults to FALSE and setting it to TRUE causes the
        ///   Universal Driver Library to repair up to 7 columns in the Imaging CCD automatically. This is
        ///   done in conjunction with column data stored in nonvolatile memory in the cameras. Under
        ///   Windows the setting of this parameter persists in the Registry through the setting of the
        ///   HKEY_CURRENT_USER\Software\SBIG\SBIGUDRV\Filter\ColumnRepairEnable setting.
        /// * The DCP_WARM_PIXEL_REPAIR_ENABLE defaults to Zero and setting it to 1 through 8
        ///   causes the Universal Driver Library to repair warm pixels in the Imaging CCD automatically. A
        ///   setting of 8 replaces approximately 5% of pixels and a setting of 1 replaces approximately 1 in a
        ///   million. A decrease of 1 in the setting replaces approximately 1/10th the number of pixels of the
        ///   higher setting (7 ~ 0.5%, 6 ~ 0.05%, etc). Under Windows the setting of this parameter persists
        ///   in the Registry through the setting of the
        ///   HKEY_CURRENT_USER\Software\SBIG\SBIGUDRV\Filter\WarmPixelRepairEnable setting.
        /// * The DCP_WARM_PIXEL_REPAIR_COUNT parameter returns the total number of pixels
        ///   replaced in the last image by the Warm Pixel Repair routine described above. You can use this
        ///   parameter to tweak the DCP_WARM_PIXEL_REPAIR_ENABLE parameter to filter as many
        ///   warm pixels as your application requires.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SetDriverControlParams : IParams
        {
            /// <summary>
            /// the parameter to modify. see also: <seealso cref="DRIVER_CONTROL_PARAM"/> enum.
            /// </summary>
            public DRIVER_CONTROL_PARAM controlParameter;

            /// <summary>
            /// the value of the control parameter.
            /// </summary>
            public UInt32 controlValue;
        };

        /// <summary>
        /// Requests the value of a driver control parameter.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetDriverControlParams : IParams
        {
            /// <summary>
            /// the driver parameter to be retrieved. see also: <seealso cref="DRIVER_CONTROL_PARAM"/> enum.
            /// </summary>
            public DRIVER_CONTROL_PARAM controlParameter;
        };

        /// <summary>
        /// Returns the value of a driver control parameter.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetDriverControlResults : IResults
        {
            /// <summary>
            /// The value of the requested driver parameter. see also: <seealso cref="DRIVER_CONTROL_PARAM"/> enum.
            /// </summary>
            public DRIVER_CONTROL_PARAM controlValue;
        };

        /// <summary>
        /// This command is used to modify the USB cameras A/D gain and offset registers.
        /// This command is intended for OEM use only.
        /// The typical application does not need to use this command as the USB cameras initialize the A/D to factory set defaults when the camera powers up.
        /// </summary>
        /// <remarks>
        /// * For the USB_AD_IMAGING_GAIN and AD_USB_TRACKING_GAIN commands the allowed
        ///   setting for the data parameter is 0 through 63. The actual Gain of the A/D (in Volts/Volt) ranges
        ///   from 1.0 to 6.0 and is determined by the following formula:
        ///   Gain = 6.0 / ( 1.0 + 5.0 * ( (63 - data) / 63 )
        ///   Note that the default A/D Gain set by the camera at power up is 1.2 for the Imaging CCD and 2.0
        ///   for the Tracking CCD. Furthermore, the gain item reported by the Get CCD Info command will
        ///   always report the default factory-set gain and will not change based upon changes made to the
        ///   A/D gain by this command.
        /// * For the USB_AD_IMAGING_OFFSET and USB_AD_TRACKING_OFFSET commands the
        ///   allowed setting for the data parameter is -255 through 255. Positive offsets increase the video
        ///   black level in ADUs. The cameras are programmed at the factory to typically have a 900 to 1000
        ///   ADU black level offset.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct USBADControlParams : IParams
        {
            /// <summary>
            /// Imaging/Tracking Gain or offset. see also: <seealso cref="USB_AD_CONTROL_COMMAND"/> enum.
            /// </summary>
            public USB_AD_CONTROL_COMMAND command;

            /// <summary>
            /// Command specific.
            /// </summary>
            public Int16 data;
        };

        /// <summary>
        /// Results for a single USB query.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QUERY_USB_INFO
        {
            /// <summary>
            /// TRUE if a camera was found.
            /// </summary>
            public MY_LOGICAL cameraFound;
            /// <summary>
            /// Camera type found. see also: <seealso cref="CAMERA_TYPE"/> enum.
            /// </summary>
            public CAMERA_TYPE cameraType;
            /// <summary>
            /// null terminated string. Name of found camera.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string name;
            /// <summary>
            /// null terminated string. Serial number of found camera.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public string serialNumber;
        };

        /// <summary>
        /// Returns a list of up to 4 cameras found by the driver via USB.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryUSBResults : IResults
        {
            /// <summary>
            /// Number of cameras found. (Max 4)
            /// </summary>
            public UInt16 camerasFound;
            /// <summary>
            /// Information returned by cameras.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 4)]
            public QUERY_USB_INFO[] usbInfo;
        };

        /// <summary>
        /// Returns a list of up to 8 cameras found by the driver via USB.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryUSBResults2 : IResults
        {
            /// <summary>
            /// Number of cameras found. (Max 8)
            /// </summary>
            public UInt16 camerasFound;
            /// <summary>
            /// Information returned by cameras.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 8)]
            public QUERY_USB_INFO[] usbInfo;
        };

        /// <summary>
        /// Returns a list of up to 24 cameras found by the driver via USB.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryUSBResults3 : IResults
        {
            /// <summary>
            /// Number of cameras found. (Max 24)
            /// </summary>
            public UInt16 camerasFound;
            /// <summary>
            /// Information returned by cameras.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 24)]
            public QUERY_USB_INFO[] usbInfo;
        };

        /// <summary>
        /// Returned information for a single device over Ethernet.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QUERY_ETHERNET_INFO
        {
            /// <summary>
            /// TRUE if a camera was found.
            /// </summary>
            public MY_LOGICAL cameraFound;
            /// <summary>
            /// IP address of camera found.
            /// </summary>
            public UInt32 ipAddress;
            /// <summary>
            /// Camera type found. see also: <seealso cref="CAMERA_TYPE"/> enum.
            /// </summary>
            public CAMERA_TYPE cameraType;

            /// <summary>
            /// null terminated string. Name of found camera.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string name;
            /// <summary>
            /// null terminated string. Serial number of found camera. 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public string serialNumber;
        };

        /// <summary>
        /// Returns a list of up to eight cameras found by the driver via Ethernet.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryEthernetResults : IResults
        {
            /// <summary>
            /// Number of cameras found.
            /// </summary>
            public UInt16 camerasFound;
            /// <summary>
            /// Information of found devices.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 4)]
            public QUERY_ETHERNET_INFO[] ethernetInfo;
        };

        /// <summary>
        /// Returns a list of up to eight cameras found by the driver via Ethernet.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryEthernetResults2 : IResults
        {
            /// <summary>
            /// Number of cameras found.
            /// </summary>
            public UInt16 camerasFound;
            /// <summary>
            /// Information of found devices.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 8)]
            public QUERY_ETHERNET_INFO[] ethernetInfo;
        };

        /// <summary>
        /// This command is used to read a Pentium processor's internal cycle counter.
        /// Pentium processors have a 32 or 64 bit register that increments every clock cycle. 
        /// For example on a 1 GHz Pentium the counter advances 1 billion counts per second. 
        /// This command can be used to retrieve that counter.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetPentiumCycleCountParams : IParams
        {
            /// <summary>
            /// number of bits to shift the results to the right (dividing by 2)
            /// </summary>
            public UInt16 rightShift;
        };

        /// <summary>
        /// This command is used to read a Pentium processor's internal cycle counter. 
        /// Pentium processors have a 32 or 64 bit register that increments every clock cycle.
        /// For example on a 1 GHz Pentium the counter advances 1 billion counts per second. 
        /// This command can be used to retrieve that counter.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetPentiumCycleCountResults : IResults
        {
            /// <summary>
            /// lower 32 bits of the Pentium cycle counter
            /// </summary>
            public UInt32 countLow;
            /// <summary>
            /// upper 32 bits of the Pentium cycle counter
            /// </summary>
            public UInt32 countHigh;
        };

        /// <summary>
        /// This command is used read or write data to the USB cameras I2C expansion port.
        /// It writes the supplied data to the I2C port, or reads data from the supplied address.
        /// This command is typically called by SBIG code in the Universal Driver.
        /// If you think you have some reason to call this function you should check with SBIG first.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct RWUSBI2CParams : IParams
        {
            /// <summary>
            /// Address to read from or write to
            /// </summary>
            public Byte address;
            /// <summary>
            /// Data to write to the external I2C device, ignored for read
            /// </summary>
            public Byte data;
            /// <summary>
            /// TRUE when write is desired , FALSE when read is desired
            /// </summary>
            public MY_LOGICAL write;
            /// <summary>
            /// Device Address of the I2C peripheral
            /// </summary>
            public Byte deviceAddress;
        };

        /// <summary>
        /// This command is used read or write data to the USB cameras I2C expansion port. 
        /// It returns the result of the read request.
        /// This command is typically called by SBIG code in the Universal Driver.
        /// If you think you have some reason to call this function you should check with SBIG first.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct RWUSBI2CResults : IResults
        {
            /// <summary>
            /// Data read from the external I2C device
            /// </summary>
            public Byte data;
        };

        /// <summary>
        /// The CFW Command is a high-level API for controlling the SBIG color filter wheels.
        /// It supports 
        /// the CFW-2 (two position shutter wheel in the ST-5C/237),
        /// the CFW-5 (internal color filter wheel for the ST-5C/237),
        /// the CFW-8, the internal filter wheel(CFW-L) in the ST-L Large Format Camera, 
        /// the internal filter wheel(CFW-402) in the ST-402 camera, 
        /// the old 6-position CFW-6A, the 10-position
        /// CFW-10 in both I2C and RS-232 interface modes,
        /// the I2C based CFW-9 and 8-position CFW for the STL(CFW-L8),
        /// the five(FW5-STX) and seven(FW7-STX) position CFWs for the STX,
        /// the five(FW5-8300) and eight(FW8-8300) position CFWs for the ST-8300 and the eight(FW8-STT) position CFW for the STT cameras.
        /// </summary>
        /// <remarks>
        /// * CFW Command CFWC_QUERY
        ///   Use this command to monitor the progress of the Goto sub-command. This command takes no
        ///   additional parameters in the CFParams. You would typically do this several times a second after
        ///   the issuing the Goto command until it reports CFWS_IDLE in the cfwStatus entry of the
        ///   CFWResults. Additionally filter wheels that can report their current position (all filter wheels
        ///   except the CFW-6A or CFW-8) have that position reported in cfwPosition entry of the
        ///   CFWResults.
        /// * CFW Command CFWC_GOTO
        ///   Use this command to start moving the color filter wheel towards a given position. Set the desired
        ///   position in the cfwParam1 entry with entries defined by the CFW_POSITION enum.
        ///   CFW Command CFWC_INIT
        /// * Use this command to initialize/self-calibrate the color filter wheel. All SBIG color filter wheels
        ///   self calibrate on power-up and should not require further initialization. We offer this option for
        ///   users that experience difficulties with their color filter wheels or when changing between the
        ///   CFW-2 and CFW-5 in the ST-5C/237. This command takes no additional parameters in the
        ///   CFWParams struct.
        /// * CFW Command CFWC_GET_INFO
        ///   This command supports several sub-commands as determined by the cfwParam1 entry (see the
        ///   CFW_GETINFO_SELECT enum). Command CFWG_FIRMWARE_VERSION returns the
        /// * CFWC_OPEN_DEVICE and CFWC_CLOSE_DEVICE:
        ///   These commands are used to Open and Close any OS based communications port associated
        ///   with the CFW and should proceed the first command sent and follow the last command sent to
        ///   the CFW. While strictly only required for the RS-232 version of the CFW-10 calling these
        ///   commands is a good idea for future compatibility. For the RS-232 based CFW-10 set the 
        ///   cfwParam1 entry to one of the settings CFW_COM_PORT enum to indicate which PC COM port is 
        ///   used to control the CFW-10. Again, only the RS232 controlled CFW-10 requires these calls.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct CFWParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="CFW_MODEL_SELECT"/> enum.
            /// </summary>
            public CFW_MODEL_SELECT cfwModel;

            /// <summary>
            /// see also: <seealso cref="CFW_COMMAND"/> enum.
            /// </summary>
            public CFW_COMMAND cfwCommand;

            /// <summary>
            /// command specific
            /// </summary>
            public UInt32 cfwParam1;
            /// <summary>
            /// command specific
            /// </summary>
            public UInt32 cfwParam2;
            /// <summary>
            /// command specific
            /// </summary>
            public UInt16 outLength;
            //TODO: unsigned char* 檢查移植是否正確
            /// <summary>
            /// command specific
            /// </summary>
            public UIntPtr outPtr;
            /// <summary>
            /// command specific
            /// </summary>
            public UInt16 inLength;
            //TODO: unsigned char* 檢查移植是否正確
            /// <summary>
            /// command specific
            /// </summary>
            public UIntPtr inPtr;
        };

        /// <summary>
        /// The CFW Command is a high-level API for controlling the SBIG color filter wheels. 
        /// It supports the
        /// CFW-2 (two position shutter wheel in the ST-5C/237), 
        /// the CFW-5 (internal color filter wheel for the ST-5C/237),
        /// the CFW-8, the internal filter wheel (CFW-L) in the ST-L Large Format Camera,
        /// the internal filter wheel (CFW-402) in the ST-402 camera, 
        /// the old 6-position CFW-6A,
        /// the 10-position
        /// CFW-10 in both I2C and RS-232 interface modes,
        /// the I2C based CFW-9 and 8-position CFW for the
        /// STL (CFW-L8), the five (FW5-STX) and seven (FW7-STX) position CFWs for the STX,
        /// the five (FW5-8300) and eight (FW8-8300) position CFWs for the ST-8300 and 
        /// the eight (FW8-STT) position CFW for the STT cameras.
        /// </summary>
        /// <remarks>
        /// * CFW Command CFWC_QUERY
        ///   Use this command to monitor the progress of the Goto sub-command. This command takes no
        ///   additional parameters in the CFParams. You would typically do this several times a second after
        ///   the issuing the Goto command until it reports CFWS_IDLE in the cfwStatus entry of the
        ///   CFWResults. Additionally filter wheels that can report their current position (all filter wheels
        ///   except the CFW-6A or CFW-8) have that position reported in cfwPosition entry of the
        ///   CFWResults.
        /// * CFW Command CFWC_GOTO
        ///   Use this command to start moving the color filter wheel towards a given position. Set the desired
        ///   position in the cfwParam1 entry with entries defined by the CFW_POSITION enum.
        ///   CFW Command CFWC_INIT
        /// * Use this command to initialize/self-calibrate the color filter wheel. All SBIG color filter wheels
        ///   self calibrate on power-up and should not require further initialization. We offer this option for
        ///   users that experience difficulties with their color filter wheels or when changing between the
        ///   CFW-2 and CFW-5 in the ST-5C/237. This command takes no additional parameters in the
        ///   CFWParams struct.
        /// * CFW Command CFWC_GET_INFO
        ///   This command supports several sub-commands as determined by the cfwParam1 entry (see the
        ///   CFW_GETINFO_SELECT enum). Command CFWG_FIRMWARE_VERSION returns the
        /// * CFWC_OPEN_DEVICE and CFWC_CLOSE_DEVICE:
        ///   These commands are used to Open and Close any OS based communications port associated
        ///   with the CFW and should proceed the first command sent and follow the last command sent to
        ///   the CFW. While strictly only required for the RS-232 version of the CFW-10 calling these
        ///   commands is a good idea for future compatibility. For the RS-232 based CFW-10 set the 
        ///   cfwParam1 entry to one of the settings CFW_COM_PORT enum to indicate which PC COM port is 
        ///   used to control the CFW-10. Again, only the RS232 controlled CFW-10 requires these calls.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct CFWResults : IResults
        {
            /// <summary>
            /// see also: <seealso cref="CFW_MODEL_SELECT"/> enum.
            /// </summary>
            public CFW_MODEL_SELECT cfwModel;

            /// <summary>
            /// see also: <seealso cref="CFW_POSITION"/> enum.
            /// </summary>
            public CFW_POSITION cfwPosition;

            /// <summary>
            /// see also: <seealso cref="CFW_STATUS"/> enum.
            /// </summary>
            public CFW_STATUS cfwStatus;

            /// <summary>
            /// see also: <seealso cref="CFW_ERROR"/> enum.
            /// </summary>
            public CFW_ERROR cfwError;

            /// <summary>
            /// command specific
            /// </summary>
            public UInt32 cfwResult1;
            /// <summary>
            /// command specific
            /// </summary>
            public UInt32 cfwResult2;
        };

        /// <summary>
        /// This command is used read or write control bits in the USB cameras.
        /// On the ST-L camera you can use this command to monitor whether the input power supply has dropped to the point where you ought to warn the user. 
        /// Do this by issuing a Read operation on bit 0 and if that bit is set the power has dropped below 10 Volts.
        /// <para>bitName values:</para>
        /// <para>* 0=Read Power Supply Low Voltage, </para>
        /// <para>* 1=Write Genl. Purp. Bit 1,</para>
        /// <para>* 2=Write Genl. Purp. Bit 2, </para>
        /// <para>* 3=Read Genl. Purp. Bit 3</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct BitIOParams : IParams
        {
            /// <summary>
            /// 0=Write, 1=Read. see also: <seealso cref="BITIO_OPERATION"/> enum.
            /// </summary>
            public BITIO_OPERATION bitOperation;

            /// <summary>
            /// see also: <seealso cref="BITIO_NAME"/> enum.
            /// </summary>
            public BITIO_NAME bitName;

            /// <summary>
            /// 1=Set Bit, 0=Clear Bit
            /// </summary>
            public MY_LOGICAL setBit;
        };

        /// <summary>
        /// This command is used read or write control bits in the USB cameras.
        /// On the ST-L camera you can use this command to monitor whether the input power supply has dropped to the point where you ought to warn the user.
        /// Do this by issuing a Read operation on bit 0 and if that bit is set the power has dropped below 10 Volts.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct BitIOResults : IResults
        {
            /// <summary>
            /// 1=Bit is set, 0=Bit is clear
            /// </summary>
            public MY_LOGICAL bitIsSet;
        };

        /// <summary>
        /// Read or write a block of data the user space in the EEPROM.
        /// </summary>

        /// <summary>
        /// Read or write a block of data the user space in the EEPROM.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct UserEEPROMResults : IResults
        {
            /// <summary>
            /// TRUE to write data to user EEPROM space, FALSE to read.
            /// </summary>
            public MY_LOGICAL writeData;
            /// <summary>
            /// Buffer of data to be written.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] data;
        };

        /// <summary>
        /// Internal SBIG use only.
        /// This command is used read or write the STF-8300's Column Repair data stored in the camera for use with that camera's Auto Filter Capability.
        /// 
        /// <para>* The left most column is column 1 (not zero). Specifying a column zero doesn't filter any columns.</para>
        /// <para>* This command is somewhat unique in that the Parameters and the Results are the same struct.</para>
        /// <para>* To enable column filtering you must use this command and also the Set Driver Control command to set the DCP_COLUMN_REPAIR parameter to 1.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ColumnEEPROMResults : IResults
        {
            /// <summary>
            /// TRUE to write data to specified EEPROM column, FALSE to read.
            /// </summary>
            public MY_LOGICAL writeData;
            /// <summary>
            /// Specify up to 7 columns to repair.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public UInt16[] columns;
            /// <summary>
            /// not used at this time.
            /// </summary>
            public UInt16 flags;
        };

        /// <summary>
        /// Send the Biorad setup to the camera, returning any error.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct BTDISetupParams : IParams
        {
            /// <summary>
            /// Row period.
            /// </summary>
            public Byte rowPeriod;
        };

        /// <summary>
        /// Results of the Biorad setup, returning any error.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct BTDISetupResults : IResults
        {
            /// <summary>
            /// Results of the command. see also: <seealso cref="BTDI_ERROR"/> enum.
            /// </summary>
            public BTDI_ERROR btdiErrors;

        };

        /// <summary>
        /// The Motor Focus Command is a high-level API for controlling SBIG Motor Focus accessories. 
        /// It supports the new ST Motor Focus unit and will be expanded as required to support new models in the future.
        /// </summary>
        /// <remarks>
        /// * Motor Focus Command MFC_QUERY
        ///   Use this command to monitor the progress of the Goto sub-command. This command takes no
        ///   additional parameters in the MFParams. You would typically do this several times a second after
        ///   the issuing the Goto command until it reports MFS_IDLE in the mfStatus entry of the
        ///   MFResults. Motor Focus accessories report their current position in the mfPosition entry of the
        ///   MFResults struct where the position is a signed long with 0 designating the center of motion or
        ///   the home position. Also the Temperature in hundredths of a degree-C is reported in the
        ///   mfResult1 entry.
        /// * Motor Focus Command MFC_GOTO
        ///   Use this command to start moving the Motor Focus accessory towards a given position. Set the
        ///   desired position in the mfParam1 entry. Again, the position is a signed long with 0 representing
        ///   the center or home position.
        /// * Motor Focus Command MFC_INIT
        ///   Use this command to initialize/self-calibrate the Motor Focus accessory. This causes the Motor
        ///   Focus accessory to find the center or Home position. You can not count on SBIG Motor Focus
        ///   accessories to self calibrate upon power-up and should issue this command upon first
        ///   establishing a link to the Camera. Additionally you should retain the last position of the Motor
        ///   Focus accessory in a parameter file and after initializing the Motor Focus accessory, you should
        ///   return it to its last position. Finally, note that this command takes no additional parameters in the
        ///   MFParams struct.
        /// * Motor Focus Command MFC_GET_INFO
        ///   This command supports several sub-commands as determined by the mfParam1 entry (see the
        ///   MF_GETINFO_SELECT enum). Command MFG_FIRMWARE_VERSION returns the version
        ///   of the Motor Focus firmware in the mfResults1 entry of the MFResults and the Maximum
        ///   Extension (plus or minus) that the Motor Focus supports is in the mfResults2 entry. The
        ///   MFG_DATA_REGISTERS command is internal SBIG use only and all other commands are
        ///   undefined.
        /// * Motor Focus Command MFC_ABORT
        ///   Use this command to abort a move in progress from a previous Goto command. Note that this
        ///   will not abort an Init.
        /// Notes: 
        /// * The Motor Focus Command takes pointers to MFParams as parameters and MFResults as
        ///   results.
        /// * Set the mfModel entry in the MFParams to the type of Motor Focus accessory you want to
        ///   control. The same value is returned in the mfModel entry of the MFResults. If you select the
        ///   MFSEL_AUTO option the driver will use the most appropriate model and return the model it
        ///   found in the mfModel entry of the MFResults.
        /// * The Motor Focus Command is a single API call that supports multiple sub-commands through
        ///   the mfCommand entry in the MFParams. Each of the sub-commands requires certain settings of
        ///   the MFParams entries and returns varying results in the MFResults. Each of these
        ///   sub-commands is discussed in detail above.
        /// * As with all API calls the Motor Focus Command returns an error code. If the error code is
        ///   CE_MF_ERROR, then in addition the mfError entry in the MFResults further enumerates the
        ///   error.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct MFParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="MF_MODEL_SELECT"/> enum.
            /// </summary>
            public MF_MODEL_SELECT mfModel;

            /// <summary>
            /// see also: <seealso cref="MF_COMMAND"/> enum.
            /// </summary>
            public MF_COMMAND mfCommand;

            /// <summary>
            /// command specific.
            /// </summary>
            public Int32 mfParam1;
            /// <summary>
            /// command specific.
            /// </summary>
            public Int32 mfParam2;
            /// <summary>
            /// command specific.
            /// </summary>
            public UInt16 outLength;
            //TODO: unsigned char* 檢查移植是否正確
            /// <summary>
            /// command specific.
            /// </summary>
            public UIntPtr outPtr;
            /// <summary>
            /// command specific.
            /// </summary>
            public UInt16 inLength;
            //TODO: unsigned char* 檢查移植是否正確
            /// <summary>
            /// command specific.
            /// </summary>
            public UIntPtr inPtr;
        };

        /// <summary>
        /// The Motor Focus Command is a high-level API for controlling SBIG Motor Focus accessories. 
        /// It supports the new ST Motor Focus unit and will be expanded as required to support new models in the future.
        /// </summary>
        /// <remarks>
        /// * Motor Focus Command MFC_QUERY
        ///   Use this command to monitor the progress of the Goto sub-command. This command takes no
        ///   additional parameters in the MFParams. You would typically do this several times a second after
        ///   the issuing the Goto command until it reports MFS_IDLE in the mfStatus entry of the
        ///   MFResults. Motor Focus accessories report their current position in the mfPosition entry of the
        ///   MFResults struct where the position is a signed long with 0 designating the center of motion or
        ///   the home position. Also the Temperature in hundredths of a degree-C is reported in the
        ///   mfResult1 entry.
        /// * Motor Focus Command MFC_GOTO
        ///   Use this command to start moving the Motor Focus accessory towards a given position. Set the
        ///   desired position in the mfParam1 entry. Again, the position is a signed long with 0 representing
        ///   the center or home position.
        /// * Motor Focus Command MFC_INIT
        ///   Use this command to initialize/self-calibrate the Motor Focus accessory. This causes the Motor
        ///   Focus accessory to find the center or Home position. You can not count on SBIG Motor Focus
        ///   accessories to self calibrate upon power-up and should issue this command upon first
        ///   establishing a link to the Camera. Additionally you should retain the last position of the Motor
        ///   Focus accessory in a parameter file and after initializing the Motor Focus accessory, you should
        ///   return it to its last position. Finally, note that this command takes no additional parameters in the
        ///   MFParams struct.
        /// * Motor Focus Command MFC_GET_INFO
        ///   This command supports several sub-commands as determined by the mfParam1 entry (see the
        ///   MF_GETINFO_SELECT enum). Command MFG_FIRMWARE_VERSION returns the version
        ///   of the Motor Focus firmware in the mfResults1 entry of the MFResults and the Maximum
        ///   Extension (plus or minus) that the Motor Focus supports is in the mfResults2 entry. The
        ///   MFG_DATA_REGISTERS command is internal SBIG use only and all other commands are
        ///   undefined.
        /// * Motor Focus Command MFC_ABORT
        ///   Use this command to abort a move in progress from a previous Goto command. Note that this
        ///   will not abort an Init.
        /// Notes: 
        /// * The Motor Focus Command takes pointers to MFParams as parameters and MFResults as
        ///   results.
        /// * Set the mfModel entry in the MFParams to the type of Motor Focus accessory you want to
        ///   control. The same value is returned in the mfModel entry of the MFResults. If you select the
        ///   MFSEL_AUTO option the driver will use the most appropriate model and return the model it
        ///   found in the mfModel entry of the MFResults.
        /// * The Motor Focus Command is a single API call that supports multiple sub-commands through
        ///   the mfCommand entry in the MFParams. Each of the sub-commands requires certain settings of
        ///   the MFParams entries and returns varying results in the MFResults. Each of these
        ///   sub-commands is discussed in detail above.
        /// * As with all API calls the Motor Focus Command returns an error code. If the error code is
        ///   CE_MF_ERROR, then in addition the mfError entry in the MFResults further enumerates the
        ///   error.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct MFResults : IResults
        {
            /// <summary>
            /// see also: <seealso cref="MF_MODEL_SELECT"/> enum. 
            /// </summary>
            public MF_MODEL_SELECT mfModel;
            /// <summary>
            /// position of the Motor Focus, 0=Center, signed.
            /// </summary>
            public Int32 mfPosition;
            /// <summary>
            /// see also: <seealso cref="MF_STATUS"/> enum.
            /// </summary>
            public MF_STATUS mfStatus;
            /// <summary>
            /// see also: <seealso cref="MF_ERROR"/>  enum.
            /// </summary>
            public MF_ERROR mfError;
            /// <summary>
            /// command specific.
            /// </summary>
            public Int32 mfResult1;
            /// <summary>
            /// command specific.
            /// </summary>
            public Int32 mfResult2;
        };


        /// <summary>
        /// Differential Guider Command Guide:
        /// <para>* DGC_DETECT detects whether a Differential Guide unit is connected to the camera.</para>
        /// <para>  Command takes no arguments.</para>
        /// <para>* DGC_GET_BRIGHTNESS obtains the brightness setting of the red and IR LEDs in the differential guide unit.</para>
        /// <para>  inPtr should be a pointer to a DGLEDState struct.</para>
        /// <para>* DGC_SET_BRIGHTNESS sets the brightness registers of the red and IR LEDs in the differential guide unit.</para>
        /// <para>  outPtr should be a pointer to a DGLEDState struct with the desired values register values set.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct DiffGuiderParams : IParams
        {
            /// <summary>
            /// Command for Differential Guider. see also: <seealso cref="DIFF_GUIDER_COMMAND"/> enum. 
            /// </summary>
            public DIFF_GUIDER_COMMAND diffGuiderCommand;
            /// <summary>
            /// Unused.
            /// </summary>
            public UInt16 spareShort;
            /// <summary>
            /// Unused.
            /// </summary>
            public UInt32 diffGuiderParam1;
            /// <summary>
            /// Unused.
            /// </summary>
            public UInt32 diffGuiderParam2;
            /// <summary>
            /// Size of output buffer. Command specific.
            /// </summary>
            public UInt16 outLength;
            //TODO: unsigned char* 檢查移植是否正確
            /// <summary>
            /// output buffer. Command specific.
            /// </summary>
            public UIntPtr outPtr;
            /// <summary>
            /// Size of input buffer. Command specific.
            /// </summary>
            public UInt16 inLength;
            //TODO: unsigned char* 檢查移植是否正確
            /// <summary>
            /// input buffer. Command specific.
            /// </summary>
            public UIntPtr inPtr;
        };

        /// <summary>
        /// Returned results of a Differential Guider Command.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct DiffGuiderResults : IResults
        {
            /// <summary>
            /// see also: <seealso cref="DIFF_GUIDER_ERROR"/> enum.
            /// </summary>
            public DIFF_GUIDER_ERROR diffGuiderError;
            /// <summary>
            /// see also: <seealso cref="DIFF_GUIDER_STATUS"/> enum.
            /// </summary>
            public DIFF_GUIDER_STATUS diffGuiderStatus;
            /// <summary>
            /// Unused.
            /// </summary>
            public UInt32 diffGuiderResult1;
            /// <summary>
            /// Unused.
            /// </summary>
            public UInt32 diffGuiderResult2;
        };

        /// <summary>
        /// State of the Differential Guider LEDs.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct DGLEDState
        {
            /// <summary>
            /// TRUE if Red LED is on, FALSE otherwise.
            /// </summary>
            public UInt16 bRedEnable;
            /// <summary>
            /// TRUE if IR LED is on, FALSE otherwise.
            /// </summary>
            public UInt16 bIREnable;
            /// <summary>
            /// brightness setting of Red LED from 0x00 to 0xFF.
            /// </summary>
            public UInt16 nRedBrightness;
            /// <summary>
            /// brightness setting of IR LED from 0x00 to 0xFF.
            /// </summary>
            public UInt16 nIRBrightness;
        };

        /// <summary>
        /// \internal
        /// <para>Internal SBIG use only. Implement the Bulk IO command which is used for Bulk Reads/Writes to the camera for diagnostic purposes.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct BulkIOParams : IParams
        {
            /// <summary>
            /// see also: <seealso cref="BULK_IO_COMMAND"/> enum.
            /// </summary>
            public BULK_IO_COMMAND command;
            /// <summary>
            /// TRUE if reading/writing data to/from the Pixel pipe, FALSE to read/write from the com pipe.
            /// </summary>
            public MY_LOGICAL isPixelData;
            /// <summary>
            /// Length of data buffer.
            /// </summary>
            public UInt32 dataLength;
            //TODO: char* 檢查移植是否正確
            /// <summary>
            /// data buffer.
            /// </summary>
            public IntPtr dataPtr;
        };

        /// <summary>
        /// \internal
        /// <para>Internal SBIG use only. Results of a Bulk I/O command.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct BulkIOResults : IResults
        {
            /// <summary>
            /// Bytes sent/received.
            /// </summary>
            public UInt32 dataLength;
        };

        /// <summary>
        /// This command is used read or write the STX/STXL/STT's customer options.
        /// </summary>

        /// <summary>
        /// This command is used read or write the STX/STXL/STT's customer options.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct CustomerOptionsResults : IResults
        {
            /// <summary>
            /// TRUE/FALSE = set/get options respectively.
            /// </summary>
            public MY_LOGICAL bSetCustomerOptions;
            /// <summary>
            /// TRUE to include Overscan region in images.
            /// </summary>
            public MY_LOGICAL bOverscanRegions;
            /// <summary>
            /// TRUE to turn on window heater.
            /// </summary>
            public MY_LOGICAL bWindowHeater;
            /// <summary>
            /// TRUE to preflash CCD.
            /// </summary>
            public MY_LOGICAL bPreflashCcd;
            /// <summary>
            /// TRUE to turn VDD off.
            /// </summary>
            public MY_LOGICAL bVddNormallyOff;
        };

        /// <summary>
        /// Results of a CC_GET_AO_MODEL command.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetI2CAoModelResults : IResults
        {
            /// <summary>
            /// AO model.
            /// </summary>
            public UInt16 i2cAoModel;
        };

        /// <summary>
        /// Flags for enabling debug messages of CC_***_*** commands.
        /// </summary>
        public enum DEBUG_LOG_CC_FLAGS : UInt16
        {
            /// <summary>
            /// Log MC_SYSTEM, CC_BREAKPOINT, CC_OPEN_*, CC_CLOSE_*, etc.
            /// </summary>
            DLF_CC_BASE = 0x0001,
            /// <summary>
            /// Log readout commands.
            /// </summary>
            DLF_CC_READOUT = 0x0002,
            /// <summary>
            /// Log status commands.
            /// </summary>
            DLF_CC_STATUS = 0x0004,
            /// <summary>
            /// Log temperature commands.
            /// </summary>
            DLF_CC_TEMPERATURE = 0x0008,
            /// <summary>
            /// Log filter wheel commands.
            /// </summary>
            DLF_CC_CFW = 0x0010,
            /// <summary>
            /// Log AO commands
            /// </summary>
            DLF_CC_AO = 0x0020,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_CC_40 = 0x0040,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_CC_80 = 0x0080
        };

        /// <summary>
        /// Flags for enabling debug messages of MC_***_*** commands.
        /// </summary>
        public enum DEBUG_LOG_MC_FLAGS : UInt16
        {
            /// <summary>
            /// Log MC_START_*, MC_END_*, MC_OPEN_*, MC_CLOSE_*, etc...
            /// </summary>
            DLF_MC_BASE = 0x0001,
            /// <summary>
            /// Log readout commands at microcommand level.
            /// </summary>
            DLF_MC_READOUT = 0x0002,
            /// <summary>
            /// Log status commands at microcommand level.
            /// </summary>
            DLF_MC_STATUS = 0x0004,
            /// <summary>
            /// Log temperature commands at microcommand level.
            /// </summary>
            DLF_MC_TEMPERATURE = 0x0008,
            /// <summary>
            /// Log EEPROM microcommands.
            /// </summary>
            DLF_MC_EEPROM = 0x0010,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_MC_20 = 0x0020,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_MC_40 = 0x0040,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_MC_80 = 0x0080
        };

        /// <summary>
        /// Flags for enabling debug messages of communication methods.
        /// </summary>
        public enum DEBUG_LOG_FCE_FLAGS : UInt16
        {
            /// <summary>
            /// Log Ethernet communication functions.
            /// </summary>
            DLF_FCE_ETH = 0x0001,
            /// <summary>
            /// Log USB communication functions.
            /// </summary>
            DLF_FCE_USB = 0x0002,
            /// <summary>
            /// Log FIFO communication functions.
            /// </summary>
            DLF_FCE_FIFO = 0x0004,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_FCE_0008 = 0x0008,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_FCE_0010 = 0x0010,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_FCE_0020 = 0x0020,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_FCE_0040 = 0x0040,
            /// <summary>
            /// Log camera communication responses.
            /// </summary>
            DLF_FCE_CAMERA = 0x0080
        };

        /// <summary>
        /// Flags for enabling debug messages of I/O operations.
        /// </summary>
        public enum DEBUG_LOG_IO_FLAGS : UInt16
        {
            /// <summary>
            /// Log reading from com pipe.
            /// </summary>
            DLF_IO_RD_COM_PIPE = 0x0001,
            /// <summary>
            /// Log writing to com pipe.
            /// </summary>
            DLF_IO_WR_COM_PIPE = 0x0002,
            /// <summary>
            /// Log reading from pixel pipe.
            /// </summary>
            DLF_IO_RD_PIXEL_PIPE = 0x0004,
            /// <summary>
            /// Log reading from alternate pixel pipe.
            /// </summary>
            DLF_IO_RD_ALT_PIPE = 0x0008,
            /// <summary>
            /// Log writing to alternate pixel pipe.
            /// </summary>
            DLF_IO_WR_ALT_PIPE = 0x0010,
            /// <summary>
            /// Log reading from Async I/O.
            /// </summary>
            DLF_IO_RD = 0x0020,
            /// <summary>
            /// Log writing to Async I/O.
            /// </summary>
            DLF_IO_WR = 0x0040,
            /// <summary>
            /// Unused.
            /// </summary>
            DLF_IO_0080 = 0x0080
        };

        /// <summary>
        /// Change debug logging, and path to log file.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct DebugLogParams : IParams
        {
            /// <summary>
            /// Command flags.
            /// </summary>
            public UInt16 ccFlags;
            /// <summary>
            /// Microcommand flags.
            /// </summary>
            public UInt16 mcFlags;
            /// <summary>
            /// Communication flags.
            /// </summary>
            public UInt16 fceFlags;
            /// <summary>
            /// I/O flags.
            /// </summary>
            public UInt16 ioFlags;
            /// <summary>
            /// Path to SBIGUDRV log file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string logFilePathName;
        };

        /// <summary>
        /// Get Readout In Progress Results
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetReadoutInProgressResults : IResults
        {
            /// <summary>
            /// Readout In Progress. TRUE if RIP, FALSE otherwise.
            /// </summary>
            public MY_LOGICAL RIP;
        };

        /// <summary>
        /// Set RBI Preflash Params
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SetRBIPreflashParams : IParams
        {
            /// <summary>
            /// darkFrameLength
            /// </summary>
            public UInt16 darkFrameLength;
            /// <summary>
            /// flushCount
            /// </summary>
            public UInt16 flushCount;
        };

        /// <summary>
        /// Get RBI Preflash Results
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GetRBIPreflashResults : IResults
        {
            /// <summary>
            /// darkFrameLength
            /// </summary>
            public UInt16 darkFrameLength;
            /// <summary>
            /// flushCount
            /// </summary>
            public UInt16 flushCount;
        };

        /// <summary>
        /// The <seealso cref="PAR_COMMAND.CC_QUERY_FEATURE_SUPPORTED"/> command queries the driver to see if the camera's firmware supports a given feature. 
        /// See <see cref="FeatureFirmwareRequirement"/> enum for information on supported features.
        /// The QueryFeatureSupportedResults structure's result variable will be TRUE if the feature is supported by the connected camera, and false otherwise.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryFeatureSupportedParams : IParams
        {
            /// <summary>
            /// Feature to query for firmware support. 
            /// </summary>
            public FeatureFirmwareRequirement ffr;
        };

        /// <summary>
        /// See also <seealso cref="QueryFeatureSupportedParams"/> parameter.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryFeatureSupportedResults : IResults
        {
            /// <summary>
            /// TRUE if feature is supported, FALSE otherwise.
            /// </summary>
            public MY_LOGICAL result;
        };

        /// <summary>
        /// Internal SBIG use only. Queries Start/End exposure performance tracking.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct QueryExposureTicksResults : IResults
        {
            /// <summary>
            /// Union LARGE_INTEGER
            /// </summary>
            [StructLayout(LayoutKind.Explicit, Pack = 8)]
            public struct LARGE_INTEGER
            {
                /// <summary>
                /// Double Word
                /// </summary>
                [FieldOffset(0)]
                public UInt32 LowPart;
                /// <summary>
                /// Double Word
                /// </summary>
                [FieldOffset(4)]
                public Int32 HighPart;

                /// <summary>
                /// Quad Word
                /// </summary>
                [FieldOffset(0)]
                public Int64 QuadPart;
            };

            /// <summary>
            /// Start exposure tick initial value.
            /// </summary>
            public LARGE_INTEGER startExposureTicks0;
            /// <summary>
            /// Start exposure tick final value.
            /// </summary>
            public LARGE_INTEGER startExposureTicks1;
            /// <summary>
            /// End exposure tick initial value.
            /// </summary>
            public LARGE_INTEGER endExposureTicks0;
            /// <summary>
            /// End exposure tick final value.
            /// </summary>
            public LARGE_INTEGER endExposureTicks1;
        };

        /// <summary>
        /// gets thrown whenever an SBIG operation doesn't return success (CE_NO_ERROR)
        /// </summary>
        public class FailedOperationException : Exception
        {
            /// <summary>
            /// Error code.
            /// </summary>
            public PAR_ERROR ErrorCode { get; private set; }

            /// <summary>
            /// FailedOperationException constructor.
            /// </summary>
            /// <param name="errorCode"></param>
            public FailedOperationException(PAR_ERROR errorCode)
            {
                ErrorCode = errorCode;
            }

            /// <summary>
            /// Error message.
            /// </summary>
            public override string Message
            {
                get
                {
                    return ErrorCode.ToString();
                }
            }
        }
        #endregion // Command Parameter and Results Structs

        #region SBIGUnivDrvCommand
        /// <summary>
        /// Command function: Supports Parallel, USB and Ethernet based cameras
        /// <para>
        ///     The master API hook for the SBIG Universal Driver dll. 
        ///     The calling program needs to allocate the memory for the parameters 
        ///     and results structs and these routines read them and fill them in respectively.
        /// </para>
        /// </summary>
        /// <param name="command">See also <seealso cref="PAR_COMMAND"/> enum.</param>
        /// <param name="Params">Params pointer to a command-specific structure containing the relevant command parameters.</param>
        /// <param name="pResults">pResults pointer to a comand-specific results structure containing the results of the command.</param>
        /// <returns>See also <seealso cref="PAR_ERROR"/> enum.</returns>
        [DllImport("SBIGUDrv.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern PAR_ERROR SBIGUnivDrvCommand(
            PAR_COMMAND command, IntPtr Params, IntPtr pResults);

        /// <summary>
        /// Command function: Supports Parallel, USB and Ethernet based cameras.
        /// An exception is thrown when an error occurs.
        /// </summary>
        /// <param name="command">The command to be executed. See also <seealso cref="PAR_COMMAND"/> enum.</param>
        /// <param name="Params">Command parameters.</param>
        /// <param name="pResults">Output from the operation.</param>
        /// <exception cref="FailedOperationException">
        ///     An error has occurred. See also <seealso cref="PAR_ERROR"/> enum.
        /// </exception>
        private static object _UnivDrvCommandLock = new object();
        private static void _UnivDrvCommand(
            PAR_COMMAND command, IntPtr Params, IntPtr pResults)
        {
            lock (_UnivDrvCommandLock)
            {
                PAR_ERROR errorCode = SBIGUnivDrvCommand(command, Params, pResults);
                if (PAR_ERROR.CE_NO_ERROR != errorCode)
                    throw new FailedOperationException(errorCode);
            }
        }

        /// <summary>
        /// Call SBIG Universal Driver, pass command.
        /// </summary>
        /// <param name="command">The command to be executed. See also <seealso cref="PAR_COMMAND"/> enum.</param>
        /// <exception cref="FailedOperationException">
        ///     An error has occurred. See also <seealso cref="PAR_ERROR"/> enum.
        /// </exception>
        public static void UnivDrvCommand(PAR_COMMAND command)
        {
            // make the call
            try
            {
                _UnivDrvCommand(command, IntPtr.Zero, IntPtr.Zero);
            }
            catch (FailedOperationException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Call SBIG Universal Driver, pass command and parameters.
        /// </summary>
        /// <typeparam name="TParams">SBIG parameters struct.</typeparam>
        /// <param name="command">The command to be executed. See also <seealso cref="PAR_COMMAND"/> enum.</param>
        /// <param name="Params">Command parameters.</param>
        /// <exception cref="FailedOperationException">
        ///     An error has occurred. See also <seealso cref="PAR_ERROR"/> enum.
        /// </exception>
        public static void UnivDrvCommand<TParams>(
            PAR_COMMAND command, TParams Params)
            where TParams : IParams
        {
            // marshall the input structure, if it exists
            var ParamGch = GCHandle.Alloc(Params, GCHandleType.Pinned);
            var ParamPtr = ParamGch.AddrOfPinnedObject();

            try
            {
                // make the call
                _UnivDrvCommand(command, ParamPtr, IntPtr.Zero);
            }
            catch (FailedOperationException e)
            {
                throw e;
            }

            // clean up
            if (IntPtr.Zero != ParamPtr)
                ParamGch.Free();
        }

        /// <summary>
        /// Call SBIG Universal Driver, pass command and output the results.
        /// </summary>
        /// <typeparam name="TResults">SBIG results struct.</typeparam>
        /// <param name="command">The command to be executed. See also <seealso cref="PAR_COMMAND"/> enum.</param>
        /// <param name="pResults">Output from the operation.</param>
        /// <exception cref="FailedOperationException">
        ///     An error has occurred. See also <seealso cref="PAR_ERROR"/> enum.
        /// </exception>
        public static void UnivDrvCommand<TResults>(
            PAR_COMMAND command, out TResults pResults)
            where TResults : IResults
        {
            pResults = default(TResults);

            // translate the struct into bytes, which are pinned
            IntPtr ResultsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pResults));
            // pass true to free up any incoming memory, since we're going to overwrite it
            Marshal.StructureToPtr(pResults, ResultsPtr, true);

            try
            {
                // make the call
                _UnivDrvCommand(command, IntPtr.Zero, ResultsPtr);
            }
            catch (FailedOperationException e)
            {
                throw e;
            }

            // Marshall back
            //Marshal.PtrToStructure(ResultsPtr, Results);
            pResults = (TResults)Marshal.PtrToStructure(ResultsPtr, typeof(TResults));

            // clean up
            Marshal.FreeHGlobal(ResultsPtr);
        }

        /// <summary>
        /// Call SBIG Universal Driver, pass command and parameters, and output the results.
        /// <para>Specialization for Readout line data.</para>
        /// </summary>
        /// <param name="command">Only <seealso cref="PAR_COMMAND.CC_READOUT_LINE"/> can be passed.</param>
        /// <param name="Params">Readout line params. See also <seealso cref="ReadoutLineParams"/> enum.</param>
        /// <param name="data">Output UInt16 type of readout line data.</param>
        /// <exception cref="ArgumentException">
        ///     When the command is not equal to "PAR_COMMAND.CC_READOUT_LINE".
        /// </exception>
        /// <exception cref="FailedOperationException">
        ///     An error has occurred. See also <seealso cref="PAR_ERROR"/> enum.
        /// </exception>
        public static void UnivDrvCommand(
            PAR_COMMAND command, ReadoutLineParams Params, out UInt16[] data)
        {
            if (PAR_COMMAND.CC_READOUT_LINE != command)
                throw new ArgumentException(
                    "The command is not equal to \"PAR_COMMAND.CC_READOUT_LINE!\"");

            data = new UInt16[Params.pixelLength];

            // allocate the image buffer
            var datagch = GCHandle.Alloc(data, GCHandleType.Pinned);
            var dataPtr = datagch.AddrOfPinnedObject();

            // put the data into it
            GCHandle rlpgch = GCHandle.Alloc(Params, GCHandleType.Pinned);

            try
            {
                // make the call
                _UnivDrvCommand(command, rlpgch.AddrOfPinnedObject(), dataPtr);
            }
            catch (FailedOperationException e)
            {
                throw e;
            }

            // clean up memory
            rlpgch.Free();
            datagch.Free();
        }

        /// <summary>
        /// Call SBIG Universal Driver, pass command and parameters, and output the results.
        /// </summary>
        /// <typeparam name="TParams">SBIG parameters struct.</typeparam>
        /// <typeparam name="TResults">SBIG results struct.</typeparam>
        /// <param name="command">The command to be executed. See also <seealso cref="PAR_COMMAND"/> enum.</param>
        /// <param name="Params">Command parameters.</param>
        /// <param name="pResults">Output from the operation.</param>
        /// <exception cref="FailedOperationException">
        ///     An error has occurred. See also <seealso cref="PAR_ERROR"/> enum.
        /// </exception>
        public static void UnivDrvCommand<TParams, TResults>(
            PAR_COMMAND command, TParams Params, out TResults pResults)
            where TParams : IParams
            where TResults : IResults
        {
            pResults = default(TResults);
            // marshall the input structure, if it exists
            var ParamGch = GCHandle.Alloc(Params, GCHandleType.Pinned);
            var ParamPtr = ParamGch.AddrOfPinnedObject();
            // translate the struct into bytes, which are pinned
            IntPtr ResultsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pResults));
            // pass true to free up any incoming memory, since we're going to overwrite it
            Marshal.StructureToPtr(pResults, ResultsPtr, true);

            try
            {
                // make the call
                _UnivDrvCommand(command, ParamPtr, ResultsPtr);
            }
            catch (FailedOperationException e)
            {
                throw e;
            }

            // Marshall back
            pResults = (TResults)Marshal.PtrToStructure(ResultsPtr, typeof(TResults));

            // clean up
            Marshal.FreeHGlobal(ResultsPtr);
            if (IntPtr.Zero != ParamPtr)
                ParamGch.Free();
        }

        #endregion

        //TODO: 檢查移植是否正確 extern "C" Int16 __stdcall SBIGLogDebugMsg(char* pStr, UInt16 length);
        /// <summary>
        /// Command function: Supports Parallel, USB and Ethernet based cameras
        /// <para>
        ///     \internal
        ///     A function used to expose writing to the log file to calling programs. Useful for debugging purposes.
        /// </para>
        /// </summary>
        /// <param name="pStr">pointer to an array of characters, null-terminated, which should be written to the log file.</param>
        /// <param name="length">unsigned int of buffer's length in bytes.</param>
        /// <returns></returns>
        [DllImport("SBIGUDrv.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern Int16 SBIGLogDebugMsg(UIntPtr pStr, UInt16 length);

        #endregion // SBIG C language header file "sbigudrv.h"

        #region Extension

        /// <summary>
        /// Establish link.
        /// </summary>
        /// <returns>The type of establish link camera.</returns>
        public static CAMERA_TYPE EstablishLink()
        {
            UnivDrvCommand(
                PAR_COMMAND.CC_ESTABLISH_LINK,
                new EstablishLinkParams(),
                out EstablishLinkResults elr);
            return elr.cameraType;
        }

        /// <summary>
        /// Use <seealso cref="StartExposureParams"/> to start exposure.
        /// </summary>
        /// <param name="sep"><seealso cref="StartExposureParams"/></param>
        public static void Exposure(StartExposureParams sep)
        {
            UnivDrvCommand(PAR_COMMAND.CC_START_EXPOSURE, sep);
        }

        /// <summary>
        /// Use <seealso cref="StartExposureParams2"/> to start exposure.
        /// </summary>
        /// <param name="sep2"><seealso cref="StartExposureParams2"/></param>
        public static void Exposure(StartExposureParams2 sep2)
        {
            UnivDrvCommand(PAR_COMMAND.CC_START_EXPOSURE2, sep2);
        }

        /// <summary>
        /// Is the exposure in progress?
        /// </summary>
        /// <returns>Yes or No</returns>
        public static bool ExposureInProgress()
        {
            var qcsp = new QueryCommandStatusParams()
            {
                command = PAR_COMMAND.CC_START_EXPOSURE
            };

            var qcsr = new QueryCommandStatusResults()
            {
                status = PAR_ERROR.CE_NO_ERROR
            };

            UnivDrvCommand(PAR_COMMAND.CC_QUERY_COMMAND_STATUS, qcsp, out qcsr);

            return (PAR_ERROR.CE_EXPOSURE_IN_PROGRESS == qcsr.status) &&
                (PAR_ERROR.CE_NO_EXPOSURE_IN_PROGRESS != qcsr.status);
        }

        /// <summary>
        /// Waits for any exposure in progress to complete.
        /// </summary>
        public static void WaitExposure()
        {
            // wait for the exposure to be done
            while (ExposureInProgress()) ;
        }

        /// <summary>
        /// Abort exposure.
        /// </summary>
        /// <param name="sep2"><seealso cref="StartExposureParams2"/></param>
        public static void AbortExposure(StartExposureParams2 sep2)
        {
            UnivDrvCommand(
                PAR_COMMAND.CC_END_EXPOSURE,
                new EndExposureParams()
                {
                    ccd = sep2.ccd
                });
        }

        /// <summary>
        /// Abort exposure.
        /// </summary>
        /// <param name="sep2"><seealso cref="StartExposureParams2"/></param>
        public static void AbortExposure(StartExposureParams sep)
        {
            UnivDrvCommand(
                PAR_COMMAND.CC_END_EXPOSURE,
                new EndExposureParams()
                {
                    ccd = sep.ccd
                });
        }

        /// <summary>
        /// Read data into the buffer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sep2"></param>
        /// <param name="buffer"></param>
        private static void _ReadoutData<T>(
            StartExposureParams2 sep2, ref T buffer)
            where T : class
        {
            // prepare the CCD for readout
            AbortExposure(sep2);
            // then telling it where and how we're going to read
            UnivDrvCommand(
                PAR_COMMAND.CC_START_READOUT,
                new StartReadoutParams
                {
                    ccd = sep2.ccd,
                    readoutMode = sep2.readoutMode,
                    left = sep2.left,
                    top = sep2.top,
                    width = sep2.width,
                    height = sep2.height
                });

            // put the data into it
            var rlp = new ReadoutLineParams
            {
                ccd = sep2.ccd,
                readoutMode = sep2.readoutMode,
                pixelStart = sep2.left,
                pixelLength = sep2.width
            };

            var rlpGCH = GCHandle.Alloc(rlp, GCHandleType.Pinned);
            GCHandle dataGCH = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr dataPtr = dataGCH.AddrOfPinnedObject();

            // get the image from the camera, line by line
            for (int y = 0; y < sep2.height; y++)
            {
                _UnivDrvCommand(
                    PAR_COMMAND.CC_READOUT_LINE,
                    rlpGCH.AddrOfPinnedObject(),
                    dataPtr + (y * sep2.width * sizeof(UInt16)));
            }
            // cleanup our memory goo
            rlpGCH.Free();
            dataGCH.Free();
        }

        /// <summary>
        /// Read data into the buffer.
        /// </summary>
        /// <param name="sep2">See also <seealso cref="StartExposureParams2"/> struct.</param>
        /// <param name="data">1D UInt16 Array of data uses buffer references.</param>
        public static void ReadoutData(
            StartExposureParams2 sep2, ref UInt16[] data)
        {
            _ReadoutData(sep2, ref data);
        }

        /// <summary>
        /// Read data into the buffer.
        /// </summary>
        /// <param name="sep2">See also <seealso cref="StartExposureParams2"/> struct.</param>
        /// <param name="data">2D UInt16 Array of data uses buffer references.</param>
        public static void ReadoutData(
            StartExposureParams2 sep2, ref UInt16[,] data)
        {
            _ReadoutData(sep2, ref data);
        }

        /// <summary>
        /// Waits for any exposure in progress to complete, ends it, 
        /// and reads it out into a 2D UInt16 array.
        /// </summary>
        /// <param name="sep2">See also <seealso cref="StartExposureParams2"/> struct.</param>
        /// <returns>2D UInt16 array of Data.</returns>
        public static Int32[,] WaitEndAndReadoutExposure(StartExposureParams2 sep2)
        {
            WaitExposure();

            var data = new Int32[sep2.height, sep2.width];
            _ReadoutData(sep2, ref data);

            return data as Int32[,];
        }

        #endregion // Extension
    } // class
} // namespace
