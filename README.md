<h1 align="center">
<img src="ColorPicker/Resources/icon.ico" width="24" /> ColorPicker
</h1>

## Windows System-Wide Color Picker

ColorPicker is a simple and efficient system-wide eye-dropper tool that allows you to pick colors from any currently running application.

**To open ColorPicker**, you can use **one** of the following methods:

- Use the shortcut <kbd>LWin</kbd> + <kbd>C</kbd> (default).
- Left-click on the tray icon.
- Select the "Pick Color" option from the tray context menu.

**To pick a color**, _left-click_ to copy the selected color to the clipboard in the chosen format.

**To zoom in or out**, use the _mouse wheel_.

[**Download the latest release here**](https://github.com/edelvarden/ColorPicker/releases/latest)

## Currently Supported Color Formats

The following color formats are supported, along with their example string representations:

- **HEX**: _#FFA500_
- **RGB**: _rgb(255, 165, 0)_
- **HSL**: _hsl(39, 100%, 50%)_
- **HSV**: _hsv(39, 100, 100)_
- **HWB**: _hwb(39, 0, 0.2)_ (newly supported)
- **LAB**: _lab(74, 20, 94)_ (newly supported)
- **XYZ**: _xyz(64, 45, 7)_ (newly supported)
- **VEC4**: _vec4(1.0, 0.647, 0.0, 1)_
- **RGB565**: _#F80_ (approximation)
- **Decimal BE (Big-endian)**: _16753920_
- **Decimal LE (Little-endian)**: _8054160_

## Differences

This is a fork of the [martinchrzan/ColorPicker](https://github.com/martinchrzan/ColorPicker) with several improvements and simplifications in the app design. The main differences are:

- **Functionality**: Retains only the color-picking functionality.
- **Lightweight**: Removed unnecessary dependencies, reducing the program size from ~9MB to less than 1MB.
- **Portability**: No installers needed; runs as a standalone executable file.
- **Design**: Adopts [**PowerToys**](https://github.com/microsoft/PowerToys) design with Light/Dark mode colors.
- **Updates**: Includes fixes and improvements from the [**PowerToys**](https://github.com/microsoft/PowerToys) variant.
