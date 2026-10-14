# Accessibility

NetSwitch is a small tray app: an icon next to the clock and a menu. This page says what it does to be usable by everyone, and where it falls short.

It has not been tested with a screen reader or other assistive technology yet. The points below describe how the app is built and what Windows is expected to do with it. If something does not work for you, please [open an issue](../../issues): accessibility problems are treated as bugs.

## What should work

- **Keyboard.** Press `Win` + `B` to move focus to the tray, use the arrow keys to reach the NetSwitch icon, then press the Menu key or `Shift` + `F10` to open the menu. The menu is a standard Windows menu: arrow keys to move, `Enter` to choose, `Esc` to close.
- **Screen readers.** The icon has a text label that always states the current connection, for example "NetSwitch: Wi-Fi" or "NetSwitch: Wi-Fi + LAN". Menu entries are standard menu items with text labels, and ticked options are exposed as checked.
- **Not colour alone.** The connection is always shown by a letter or a symbol (`W`, `L`, `+`, `x`, or the Wi-Fi and network symbols). Colour is only an extra cue, so the icon stays readable with colour blindness.
- **Light and dark taskbars.** The monochrome styles switch between white and black to follow the Windows theme.
- **Language.** The interface follows the Windows language, in English or Italian.
- **No time pressure.** Nothing in the app needs a quick reaction, and it never plays sounds or animations.

## Choosing a readable icon

The icon is only 16 to 32 pixels, so the style matters. From the **Icon style** menu:

- the easiest to read are **Filled circle**, **Rounded square** and **Monochrome letter**;
- the **Gothic** styles are decorative and harder to read at this size, especially the ones with thorns.

## Known limits

- The icon cannot be made larger than Windows draws it. Raising the display scale in Windows settings enlarges the whole tray, icon included.
- There is no global keyboard shortcut to switch connection; you have to go through the tray.
- Pressing `Enter` on the icon does nothing: use the Menu key or `Shift` + `F10`.
- Errors are shown as Windows notifications, which disappear after a few seconds.
- Windows asks for administrator permission at every start, with its standard prompt.
