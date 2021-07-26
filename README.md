# ASCOM.HomeMade.SBIGCamera

This is an ASCOM driver for SBIG cameras. My main target camera is STF-8300, so it might not work for anything else.

As SBIG has veeb bought by Diffraction Limited and that they have decided not to invest time to develop ASCOM drivers for old cameras, SBIG owners have been left out in the cold. We're limited to the software that natively understands SBIG cameras, which exclude most recent software (e.g. Voyager, NINA).

So I decided to build my own. 

This codebase is my first attempt over a weekend. Consider it alpha software and don't even expect it to work. But if you try it, please tell me and tell me if anything doesn't work. To do so, please activate trace logging in the ASCOM driver options so I can see what happens.

For information, I've tested only with an STF-8300M with FW8 using SGP. I found that everything seems to be working:
- Camera access.
- Cooling management.
- FW access.

Note that the software hasn't been through any certification (ASCOM or otherwise).

This work uses SbigSharp code written by eliotg (https://github.com/eliotg/SbigSharp).
