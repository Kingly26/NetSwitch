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

## How to get the app (no programming needed)

You do not need to install anything: the app is built with a tool that is already part of Windows 10 and 11.

1. At the top of this page click the green **Code** button, then **Download ZIP**.
2. Open your Downloads folder, right-click the ZIP file and choose **Extract All...**, then **Extract**.
3. Open the extracted folder and double-click **`build.bat`** (it may show simply as `build`).
   - If a blue "Windows protected your PC" window appears, click **More info**, then **Run anyway**. It appears because the file was downloaded from the internet.
4. A black window opens and after a moment says **Done**. Press any key to close it.
5. In the same folder there is now **`NetSwitch.exe`**: double-click it to start the app.
   - Windows asks for administrator permission: answer **Yes**. The app needs it to turn network adapters on and off.
6. The icon appears in the tray, next to the clock. If you do not see it, click the small **^** arrow; you can drag the icon onto the taskbar to keep it always visible.

To start it automatically, click the icon and tick **Start with Windows**. You can move the folder wherever you like first: if you move it afterwards, untick and tick the option again.

To update, download the ZIP again and repeat the steps. Close the app first (click the icon, then **Exit**), or the build cannot replace the file.

### From the command line

If you prefer, this is the command that `build.bat` runs:

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -nologo -target:winexe -out:NetSwitch.exe -win32manifest:app.manifest -r:System.Windows.Forms.dll -r:System.Drawing.dll -r:System.Management.dll NetSwitch.cs Gothic.cs
```
