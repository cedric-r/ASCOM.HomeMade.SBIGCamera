# ASCOM.HomeMade.SBIGCamera

Homebrew ASCOM driver for SBIG USB cameras. Primary target is the **STF-8300M** — other models may work but aren't tested.

## Background

When Diffraction Limited acquired SBIG, they chose not to develop ASCOM drivers for the legacy camera line. This left SBIG owners locked into software with native SBIG support (e.g. CCDSoft, MaxIm DL), unable to use modern observatory packages like NINA, Voyager, or SGP.

This driver bridges that gap. It started as a weekend project and grew into a full driver suite covering imaging, guiding, filter wheels, and network-attached cameras.

## Architecture

![Architecture Diagram](ASCOM.HomeMade.SBIGCamera-architecture.html)

The solution is structured as a layered stack. The full diagram is interactive HTML — open [`ASCOM.HomeMade.SBIGCamera-architecture.html`](ASCOM.HomeMade.SBIGCamera-architecture.html) in a browser.

### Layer Map

| Layer | Project | Role |
| :--- | :--- | :--- |
| **Applications** | — | NINA · SGP · Voyager · CCDCiel (any ASCOM-compatible client) |
| **ASCOM Drivers** | `SBIGImagingCamera` | Main imager driver (COM, signed) |
| | `SBIGGuidingCamera` | STT/STX internal guiding sensor (COM, signed) |
| | `SBIGExternalCamera` | STX remote head (COM, signed) |
| | `SBIGFW` | Filter wheel — CFW-8 (COM, signed) |
| **Core** | `SBIGCamera` | Camera abstraction layer |
| **Client** | `SBIGClient` | Client library, image acquisition thread, Bayer decoding |
| **Hub** | `SBIGHub` | COM local server (WinForms EXE); manages camera connection lifecycle |
| **Common** | `SBIGCommon` | Request/response protocol, ISBIGHandler, debug utilities |
| **Bridge** | `SbigSharp` | .NET Standard 2.0 P/Invoke bindings to the native SBIG library |
| **Native** | `HomeMade.SBIGUDrv.dll` | Native C++ USB driver (x86/x64) |
| **Hardware** | — | SBIG cameras (USB / Ethernet) |

### Supporting Projects

- **`CSharpFits`** — FITS file format library
- **`SBIGServiceConsole`** — diagnostic service console
- **`SBIGWindowsService`** — Windows service wrapper
- **`SBIGCameraTests`** — unit tests and ASCOM conformance tests

### External Dependencies

- **ASCOM Platform 6.5.2** — ASCOM.Astrometry, DeviceInterfaces, DriverAccess, Utilities, etc.
- **Newtonsoft.Json 13.0.1** — JSON serialization (SBIGClient)
- **System.Memory / System.Buffers / System.ValueTuple** — modern .NET APIs

## Target Cameras

Tested with:

- **STF-8300M** + CFW-8 — imaging + filter wheel (USB and Ethernet)
- **STT-8300M** — imaging + internal guiding sensor

The following three ASCOM camera drivers are installed:

1. **ASCOM.HomeMade.SBIGImagingCamera** — main imaging camera
2. **ASCOM.HomeMade.SBIGGuidingCamera** — internal guiding sensor (STT/STX)
3. **ASCOM.HomeMade.SBIGExternalCamera** — remote head (STX)

## Installation

Download and run the installer:

[HomeMade SBIG Camera Setup.exe](https://github.com/cedric-r/ASCOM.HomeMade.SBIGCamera/blob/main/HomeMade%20SBIG%20Camera%20Setup.exe)

> The 64-bit driver bundles an unofficial version of the SBIG library released by Diffraction Limited for Prism. It is not part of their official distribution — it officially doesn't exist.

## Usage

1. Install the ASCOM Platform 6.5 or later.
2. Run the setup above.
3. In your astronomy software, select the ASCOM camera driver matching your hardware:
   - `ASCOM.HomeMade.SBIGImagingCamera` for the main sensor
   - `ASCOM.HomeMade.SBIGGuidingCamera` for the guiding sensor
   - `ASCOM.HomeMade.SBIGExternalCamera` for the STX remote head
   - `ASCOM.HomeMade.SBIGFW` for the filter wheel
4. Enable **trace logging** in the driver options for diagnostics.

### Log Files

Logs are written to `%LOCALAPPDATA%\ASCOM.HomeMade.*\` — useful if something doesn't work.

## Verified Working

- Camera access (USB and Ethernet)
- Cooling management
- Filter wheel access (USB and Ethernet)

Tested with SGP, NINA, CCDCiel, and Voyager.

## Caveats

This is alpha software. It was built for my specific hardware (STF-8300M) and may not work with other SBIG models without modification. If you try it on different hardware and hit issues, please enable trace logging and report what you find.

## Acknowledgements

- **eliotg** — [SbigSharp](https://github.com/eliotg/SbigSharp), the .NET bindings this project builds on
- **Diffraction Limited** — for releasing the unofficial 64-bit SBIG library used by the driver

## Project Status

All original goals have been completed:

- ~~Provide access to guiding camera of the STT~~ ✓
- ~~Support network cameras, not just USB~~ ✓
- ~~Support for STX remote head~~ ✓
