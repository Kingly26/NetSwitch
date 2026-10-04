# NetSwitch

[English](README.md) | Italiano

Piccola app per la tray di Windows che alterna con un clic tra Wi-Fi e LAN (Ethernet).

- **Clic** sull'icona (sinistro o destro) per aprire il menu: Alterna Wi-Fi <-> LAN, Solo Wi-Fi, Solo LAN, Entrambe, Stile icona, Avvia con Windows, Esci
- **Alterna con un clic sull'icona** (disattivata di default): se la abiliti dal menu, il clic sinistro alterna subito Wi-Fi <-> LAN; il menu resta disponibile con il clic destro
- L'icona mostra sempre lo stato attuale (Wi-Fi, LAN, entrambe, nessuna), nello stile che scegli
- La lingua dell'interfaccia segue Windows (italiano o inglese)

## A chi serve

Consiglio NetSwitch a chi vuole cambiare connessione più in fretta quando la rete si interrompe. Invece di aprire le impostazioni di Windows per disattivare una scheda e attivare l'altra, basta un clic dalla tray.

Casi tipici:

- **Il Wi-Fi cade o diventa instabile** (chiamate, giochi, download): passi a **Solo LAN** e sei subito sul cavo.
- **La linea via cavo va giù** (guasto del router, problema del provider): passi a **Solo Wi-Fi** e ti colleghi a un'altra rete, per esempio l'hotspot del telefono.
- **Vuoi una riserva sempre pronta**: usa **Entrambe**. Windows preferisce il cavo e ripiega da solo sul Wi-Fi se il cavo smette di funzionare.

Da sapere:

- Cambiare serve solo se le due connessioni non hanno lo stesso guasto. Se Wi-Fi e cavo passano dallo stesso router e il router o la linea sono fuori uso, non funzionerà nessuna delle due: il Wi-Fi deve essere su una rete diversa.
- Quando il cavo è collegato, Windows non riconnette il Wi-Fi da solo. Lo fa NetSwitch: dopo aver riacceso il Wi-Fi prova le reti salvate, e può volerci qualche secondo.
- Il cambio modifica il percorso di rete, quindi chiamate, giochi e VPN possono interrompersi un attimo e riconnettersi.

## Stili dell'icona

Sedici stili, selezionabili dal menu, mostrati qui su barra scura e chiara. Quelli monocromatici seguono il tema di Windows. Gli stili gotici sono disegnati dall'app stessa (un pennino da calligrafia fatto scorrere lungo ogni carattere), con le lettere o con i simboli; negli stili a simboli, Entrambe mostra gli archi del Wi-Fi sopra il simbolo della rete cablata.

![Stili icona](icon-styles.png)

Richiede privilegi di amministratore (per abilitare/disabilitare le schede di rete). Attiva la nuova scheda prima di spegnere quella vecchia, così non resti mai senza connessione. Le schede virtuali (VPN, VMware, Hyper-V, Bluetooth) vengono ignorate.

## Come ottenere l'app (senza saper programmare)

Non devi installare nulla: l'app si crea con uno strumento già presente in Windows 10 e 11.

1. In cima a questa pagina clicca il pulsante verde **Code**, poi **Download ZIP**.
2. Apri la cartella Download, fai clic destro sul file ZIP e scegli **Estrai tutto...**, poi **Estrai**.
3. Apri la cartella estratta e fai doppio clic su **`build.bat`** (può comparire solo come `build`).
   - Se appare una finestra blu "PC protetto da Windows", clicca **Ulteriori informazioni** e poi **Esegui comunque**. Compare perché il file è stato scaricato da internet.
4. Si apre una finestra nera che dopo un attimo scrive **Fatto**. Premi un tasto per chiuderla.
5. Nella stessa cartella ora c'è **`NetSwitch.exe`**: fai doppio clic per avviare l'app.
   - Windows chiede il permesso di amministratore: rispondi **Sì**. Serve all'app per accendere e spegnere le schede di rete.
6. L'icona compare nella tray, accanto all'orologio. Se non la vedi, clicca la freccetta **^**; puoi trascinare l'icona sulla barra per averla sempre in vista.

Per avviarla in automatico, clicca l'icona e spunta **Avvia con Windows**. Prima puoi spostare la cartella dove preferisci: se la sposti dopo, togli e rimetti la spunta.

Per aggiornare, scarica di nuovo lo ZIP e ripeti i passaggi. Prima chiudi l'app (clic sull'icona, poi **Esci**), altrimenti il file non può essere sostituito.

### Da riga di comando

Se preferisci, questo è il comando che `build.bat` esegue:

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -nologo -target:winexe -out:NetSwitch.exe -win32manifest:app.manifest -r:System.Windows.Forms.dll -r:System.Drawing.dll -r:System.Management.dll NetSwitch.cs Gothic.cs
```

## Licenza

Rilasciata con [licenza MIT](LICENSE). I contributi sono benvenuti: vedi [CONTRIBUTING.md](CONTRIBUTING.md) (in inglese).
