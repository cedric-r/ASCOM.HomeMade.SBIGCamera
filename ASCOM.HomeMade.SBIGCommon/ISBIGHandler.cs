using SbigSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public interface ISBIGHandler
    {
        bool Connect(string ipAddress);
        void Disconnect();
        void DisconnectAll();
        void UnivDrvCommand(SBIG.PAR_COMMAND command);
        void UnivDrvCommand<TParams>(SBIG.PAR_COMMAND command, TParams Params) where TParams : SBIG.IParams;
        void UnivDrvCommand<TResults>(SBIG.PAR_COMMAND command, out TResults pResults) where TResults : SBIG.IResults;
        void UnivDrvCommand(SBIG.PAR_COMMAND command, SBIG.ReadoutLineParams Params, out UInt16[] data);
        void UnivDrvCommand<TParams, TResults>(SBIG.PAR_COMMAND command, TParams Params, out TResults pResults) where TParams : SBIG.IParams where TResults : SBIG.IResults;
        void AbortExposure(SBIG.StartExposureParams2 sep2);
        void EndReadout(SBIG.CCD_REQUEST ccd);
        bool ExposureInProgress();
        void ReadoutData(SBIG.StartExposureParams2 sep2, ref UInt16[,] data);
    }
}
