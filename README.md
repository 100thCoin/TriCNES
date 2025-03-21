# TriCNES

TriCNES, or "Coin's Contrabulous Cartswapulator", is a NES emulator designed for swapping NES cartridges between CPU cycles in order to try emulating a theoretical arbitrary code exploit for the Nintendo Entertainment System.

This NES emulator was built from the ground up starting from a blank .net winforms project

# Limitations

This emulator does not produce audio.  
This emulator only accepts inputs in the form of a TAS file.  
This emulator can only run NTSC cartridges properly.  
This emulator only supports the following mapper chips:
* 0: NROM
* 1: MMC1
* 2: UxROM
* 3: CNROM
* 4: MMC3
* 7: AOROM
* 69: Sunsoft FME-7

# Supported TAS file types

Due to varying emulator accuracy, this emulator is not guaranteed to sync all TAS files. Despite this, it supports loading inputs from the following formats:
* .bk2 (Bizhawk)
* .tasproj (Bizhawk's TAStudio)
* .fm2 (FCEUX)
* .fm3 (FCEUX's TAS Editor)
* .fmv (Famtasia)
* .r08 (Replay Device)

In addition to those TAS file types, my emulator can also load my very own intercycle-cart-swapping TAS format, .3ct.

# The .3ct TAS file format

TAS files that were made with the intention of swapping cartridges between cycles are stored in the following format.

The first line of the file is an integer, indicating how many cartridges are being used for this TAS. This integer will be called 'n'.

The following 'n' lines are local file paths to the ROMs you wish to use. This will set up an array of cartridges.

The remaining lines until the end of the file are in the following format: "x y" where an integer 'x' is seperated from an integer 'y' with a space.

When the TAS is being played back, before CPU cycle 'x', swap to the cartridge at index 'y' into the cartridge array.

Here's an example:
<pre>
5
Super Mario Bros. [!].nes
Dash Galaxy in the Alien Asylum (U) [!].nes
Kung Fu (JU) [!].nes
Pipe Dream (U) [!].nes
Super Mario Bros. 3 (U) (V1.1) [!].nes
6 0
7 1
8 2
9 3
10 4
</pre>

In this example, there are 5 cartridges. Before cycle 6, the emulator will swap to `Super Mario Bros. [!].nes`, as that is index 0 into the cartridge array. Before cycle 7, the emulator will swap to index 1, `Dash Galaxy in the Alien Asylum (U) [!].nes`, and so on.

# Screenshots
![Screenshot](https://github.com/user-attachments/assets/56a25c5d-5c2f-493f-85bd-90bb192b1322)

![Screenshot2](https://github.com/user-attachments/assets/5e6771fe-0696-4e27-9fdc-b16fd1b407ef)

![Screenshot3](https://github.com/user-attachments/assets/1689f379-7eb8-445e-9632-81c3a3de2301)
