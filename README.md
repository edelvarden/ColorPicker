<h1 align="center">
<img src="ColorPicker/Resources/icon.ico" width="24" /> ColorPicker
</h1>

## Windows System-Wide Color Picker

ColorPicker is a simple and efficient system-wide eye-dropper tool. It allows you to pick colors from any currently running application.

**To open Color Picker**, you can use one of the following methods:

1. Use the shortcut `LWin` + `C` (default).
2. Left-click on the tray icon.
3. Select the "Pick Color" option from the tray context menu.

**To pick a color**, _left-click_ to copy the selected color to the clipboard in the chosen format.

**To zoom in or out**, use the _mouse wheel_.

[**Download the latest release here**](https://github.com/edelvarden/color-picker/releases/latest)

## Currently Supported Color Formats

The following color formats are supported along with their example string representations:

- **HEX**: _#292929_
- **RGB**: _rgb(31, 31, 31)_
- **HSL**: _hsl(0, 0%, 12%)_
- **HSV**: _hsv(0, 0, 12)_
- **HWB**: _hwb(0, 0, 12)_ (newly supported)
- **VEC4**: _vec4(0.122, 0.122, 0.122, 1)_
- **RGB565**: _#4C8A_
- **Decimal BE (Big-endian)**: _2114460_
- **Decimal LE (Little-endian)**: _1213756_
- **LAB**: _lab(18, -6, -18)_ (newly supported)
- **XYZ**: _xyz(29, 12, 9)_ (newly supported)

## Differences

This is a fork of the [martinchrzan/ColorPicker](https://github.com/martinchrzan/ColorPicker) with several improvements and simplifications in the app design. The main differences are:

- **Functionality**: Retains only the color picking functionality.
- **Lightweight**: Removed unnecessary dependencies, reducing the program size from ~9MB to less than 1MB.
- **Portability**: No installers needed; can run as a standalone executable file.
- **Updates**: Applied fixes and improvements from the [**PowerToys**](https://github.com/microsoft/PowerToys) variant.
