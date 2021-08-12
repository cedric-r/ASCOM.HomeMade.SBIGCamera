/**
 * ASCOM.HomeMade.SBIGCamera - SBIG camera driver
 * Copyright (C) 2021 Cedric Raguenaud [cedric@raguenaud.earth]
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
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
