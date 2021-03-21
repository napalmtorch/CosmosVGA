# CosmosVGA
Custom Cosmos VGA Driver

# Features
  - Supports 4 modes
      - 80x25x4 Text
      - 80x50x4 Text
      - 90x60x4 Text
      - 320x200x8 Pixel
      - 320x200x8 Pixel(Double Buffered)
  - VGA BIOS font rendering(text and pixel mode)
  - Custom Image format(4-byte header for width and height, remainder is 8bit pixels)
      - Loading from disk
      - Resize by factor
      - Tool to convert 32-bit JPEG/PNG/BMP to custom PIC format
  - Custom palettes(defined in VGADriverII.cs)

# Usage
To use this driver, simply add all the .cs files that start with "VGA" to your project.
Once the files are in your project, you can use Cosmos.HAL to access the driver and
Cosmos.System.Graphics to access its features. The first thing you need to do is initialize
the driver and set the mode using Cosmos.HAL.VGADriverII.Initialize(VGAMode mode);
Once you've set the mode, you can use the functions in Cosmos.System.Graphics.VGAGraphics
to do various drawing commands, or just use the VGADriverII class for direct access to the
VGA device. When using 320x200 double buffered mode, remember to use VGAGraphics.Display()
after drawing to swap buffers.

# Example
![vgatut](https://user-images.githubusercontent.com/55903118/111919703-b8186780-8a61-11eb-968c-2b3bc14a401f.png)
