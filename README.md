# NetSwitch

English | [Italiano](README.it.md)

A tiny Windows tray app that switches between Wi-Fi and LAN (Ethernet) with one click.

- **Click** the tray icon (left or right) to open the menu: Switch Wi-Fi <-> LAN, Wi-Fi only, LAN only, Both, Icon style, Start with Windows, Exit
- **Switch by clicking the icon** (off by default): when enabled in the menu, a left-click switches Wi-Fi <-> LAN immediately; the menu stays available with a right-click
- The icon always shows the current state (Wi-Fi, LAN, both, none), in the style you pick
- The UI language follows Windows (Italian or English)

## Who it is for

I recommend NetSwitch to anyone who wants a faster way to change connection when the network drops. Instead of opening Windows settings and disabling one adapter and enabling the other, it takes one click from the tray.

Typical cases:

- **The Wi-Fi drops or gets unstable** (calls, games, downloads): switch to **LAN only** and you are on the cable right away.
- **The wired line goes down** (router fault, provider outage): switch to **Wi-Fi only** and connect to another network, for example your phone's hotspot.
- **You want a backup always ready**: use **Both**. Windows prefers the cable and falls back to Wi-Fi by itself if the cable stops working.

Good to know:

- Switching only helps when the two connections do not share the same fault. If Wi-Fi and cable both go through the same router and the router or the line is down, neither will work; the Wi-Fi has to be on a different network.
- While Ethernet is connected, Windows does not reconnect Wi-Fi on its own. NetSwitch does it for you: after turning Wi-Fi back on it tries your saved networks, which can take a few seconds.
- A switch changes your network path, so calls, games and VPNs may drop for a moment and reconnect.

## Icon styles

Sixteen styles, selectable from the menu, shown here on a dark and a light taskbar. The monochrome ones follow the Windows theme. The gothic styles are drawn by the app itself (a calligraphy nib swept along each glyph), with letters or with symbols; in the symbol styles, Both shows the Wi-Fi arches over the wired symbol.

![Icon styles](icon-styles.png)

Requires administrator rights (needed to enable/disable network adapters). It enables the new adapter before disabling the old one, so you are never left without a connection. Virtual adapters (VPN, VMware, Hyper-V, Bluetooth) are ignored.

## Build

Uses the C# compiler that ships with Windows, nothing to install:

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -nologo -target:winexe -out:NetSwitch.exe -win32manifest:app.manifest -r:System.Windows.Forms.dll -r:System.Drawing.dll -r:System.Management.dll NetSwitch.cs Gothic.cs
```
