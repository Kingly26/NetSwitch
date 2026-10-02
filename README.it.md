# NetSwitch

[English](README.md) | Italiano

Piccola app per la tray di Windows che alterna con un clic tra Wi-Fi e LAN (Ethernet).

- **Clic sinistro** sull'icona: alterna Wi-Fi <-> LAN
- **Clic destro**: Solo Wi-Fi, Solo LAN, Entrambe, Avvia con Windows, Esci
- Icona: blu `W` = Wi-Fi, verde `L` = LAN, viola `+` = entrambe, rossa `x` = nessuna
- La lingua dell'interfaccia segue Windows (italiano o inglese)

Richiede privilegi di amministratore (per abilitare/disabilitare le schede di rete). Attiva la nuova scheda prima di spegnere quella vecchia, così non resti mai senza connessione. Le schede virtuali (VPN, VMware, Hyper-V, Bluetooth) vengono ignorate.

## Compilazione

Usa il compilatore C# incluso in Windows, nessuna installazione necessaria:

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -nologo -target:winexe -out:NetSwitch.exe -win32manifest:app.manifest -r:System.Windows.Forms.dll -r:System.Drawing.dll -r:System.Management.dll NetSwitch.cs
```
