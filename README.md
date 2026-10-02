# NetSwitch

English | [Italiano](README.it.md)

A tiny Windows tray app that switches between Wi-Fi and LAN (Ethernet) with one click.

- **Left-click** the tray icon: switch Wi-Fi <-> LAN
- **Right-click**: Wi-Fi only, LAN only, Both, Start with Windows, Exit
- Icon: blue `W` = Wi-Fi, green `L` = LAN, purple `+` = both, red `x` = none
- The UI language follows Windows (Italian or English)

Requires administrator rights (needed to enable/disable network adapters). It enables the new adapter before disabling the old one, so you are never left without a connection. Virtual adapters (VPN, VMware, Hyper-V, Bluetooth) are ignored.

## Build

Uses the C# compiler that ships with Windows, nothing to install:

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -nologo -target:winexe -out:NetSwitch.exe -win32manifest:app.manifest -r:System.Windows.Forms.dll -r:System.Drawing.dll -r:System.Management.dll NetSwitch.cs
```
