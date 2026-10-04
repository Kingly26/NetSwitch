# NetSwitch

[English](README.md) | Italiano

Piccola app per la tray di Windows che alterna con un clic tra Wi-Fi e LAN (Ethernet).

- **Clic** sull'icona (sinistro o destro) per aprire il menu: Alterna Wi-Fi <-> LAN, Solo Wi-Fi, Solo LAN, Entrambe, Stile icona, Avvia con Windows, Esci
- **Alterna con un clic sull'icona** (disattivata di default): se la abiliti dal menu, il clic sinistro alterna subito Wi-Fi <-> LAN; il menu resta disponibile con il clic destro
- Icona: blu `W` = Wi-Fi, verde `L` = LAN, viola `+` = entrambe, rossa `x` = nessuna
- La lingua dell'interfaccia segue Windows (italiano o inglese)

## Stili dell'icona

Sedici stili, selezionabili dal menu, mostrati qui su barra scura e chiara. Quelli monocromatici seguono il tema di Windows. Gli stili gotici sono disegnati dall'app stessa (un pennino da calligrafia fatto scorrere lungo ogni carattere), con le lettere o con i simboli; negli stili a simboli, Entrambe mostra gli archi del Wi-Fi sopra il simbolo della rete cablata.

![Stili icona](icon-styles.png)

Richiede privilegi di amministratore (per abilitare/disabilitare le schede di rete). Attiva la nuova scheda prima di spegnere quella vecchia, così non resti mai senza connessione. Le schede virtuali (VPN, VMware, Hyper-V, Bluetooth) vengono ignorate.

## Compilazione

Usa il compilatore C# incluso in Windows, nessuna installazione necessaria:

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -nologo -target:winexe -out:NetSwitch.exe -win32manifest:app.manifest -r:System.Windows.Forms.dll -r:System.Drawing.dll -r:System.Management.dll NetSwitch.cs Gothic.cs
```
