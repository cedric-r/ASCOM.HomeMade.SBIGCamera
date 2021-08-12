# ASCOM.HomeMade.SBIGCamera

This is an ASCOM driver for SBIG USB cameras. My main target camera is STF-8300, so it might not work for anything else.

As SBIG was bought by Diffraction Limited a few years ago, DL have decided not to invest time to develop ASCOM drivers for old cameras, and SBIG owners have been left out in the cold. We're limited to the software that natively understands SBIG cameras, which exclude most recent software (e.g. Voyager, NINA).

So I decided to build my own. 

This codebase is my first attempt over a weekend. Consider it alpha software and don't even expect it to work. But if you try it, please tell me and tell me if anything doesn't work. To do so, please activate trace logging in the ASCOM driver options so I can see what happens. The log file lives in your AppData\Local\ASCOM.HomeMade.SBIGCamera\ folder.

For information, I've tested only with an STF-8300M with FW8 using SGP, NINA, and Voyager. I found that everything seems to be working:
- Camera access.
- Cooling management.
- FW access.

If you just want to install the drivers and test them, run the installer: https://github.com/cedric-r/ASCOM.HomeMade.SBIGCamera/blob/main/HomeMade%20SBIG%20Camera%20Setup.exe

Note that the software hasn't been through any certification (ASCOM or otherwise).

Note also that the 64 bits driver uses an unofficial version of the SBIG library released by DL for Prism but isn't part of their official distribution (it officially doesn't exist).

This work uses SbigSharp code written by eliotg (https://github.com/eliotg/SbigSharp).

Task list:<br>
<strike>- Provide access to guiding camera of the STT.</strike><br>
<strike>- Support network cameras, not just USB.</strike>
