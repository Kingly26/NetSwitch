using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Management;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

[assembly: AssemblyTitle("NetSwitch")]

class Adapter { public ManagementObject Obj; public string Name; public bool Enabled; public bool IsWifi; }

class App : ApplicationContext
{
    NotifyIcon tray = new NotifyIcon();
    Timer timer = new Timer();
    const string TaskName = "NetSwitchAutostart";
    static readonly Regex WifiRx = new Regex("wi-?fi|wireless|wlan|802[.]11", RegexOptions.IgnoreCase);
    static readonly Regex SkipRx = new Regex("bluetooth|virtual|vmware|hyper-v|vethernet|tap-|vpn|loopback|wan miniport|pseudo|npcap|wintun", RegexOptions.IgnoreCase);

    public App()
    {
        tray.Visible = true;
        tray.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) Toggle(); };
        tray.ContextMenuStrip = BuildMenu();
        timer.Interval = 3000;
        timer.Tick += (s, e) => Refresh();
        timer.Start();
        Refresh();
    }

    List<Adapter> GetAdapters()
    {
        var list = new List<Adapter>();
        using (var q = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter=TRUE AND NetConnectionID IS NOT NULL"))
            foreach (ManagementObject o in q.Get())
            {
                string desc = (o["Name"] ?? "").ToString();
                string id = (o["NetConnectionID"] ?? "").ToString();
                if (SkipRx.IsMatch(desc) || SkipRx.IsMatch(id)) continue;
                list.Add(new Adapter {
                    Obj = o, Name = id,
                    Enabled = o["NetEnabled"] != null && (bool)o["NetEnabled"] || IsEnabledStatus(o),
                    IsWifi = WifiRx.IsMatch(desc) || WifiRx.IsMatch(id)
                });
            }
        return list;
    }

    // NetEnabled is null when the adapter is disabled; ConfigManagerErrorCode 22 = disabled
    static bool IsEnabledStatus(ManagementObject o)
    {
        object c = o["ConfigManagerErrorCode"];
        return c != null && Convert.ToInt32(c) == 0 && o["NetEnabled"] != null && (bool)o["NetEnabled"];
    }

    void SetAll(bool wifi, bool lan)
    {
        var all = GetAdapters();
        // enable first, then disable, so there is never a gap without a connection
        foreach (var a in all) { bool want = a.IsWifi ? wifi : lan; if (want && !a.Enabled) a.Obj.InvokeMethod("Enable", null); }
        foreach (var a in all) { bool want = a.IsWifi ? wifi : lan; if (!want && a.Enabled) a.Obj.InvokeMethod("Disable", null); }
        System.Threading.Thread.Sleep(1200);
        Refresh();
    }

    void Toggle()
    {
        try
        {
            var all = GetAdapters();
            bool wifiOn = all.Exists(a => a.IsWifi && a.Enabled);
            SetAll(!wifiOn, wifiOn);
        }
        catch (Exception ex) { tray.ShowBalloonTip(3000, "NetSwitch", "Errore: " + ex.Message, ToolTipIcon.Error); }
    }

    void Refresh()
    {
        try
        {
            var all = GetAdapters();
            bool w = all.Exists(a => a.IsWifi && a.Enabled), l = all.Exists(a => !a.IsWifi && a.Enabled);
            string txt = w && l ? "Wi-Fi + LAN" : w ? "Wi-Fi" : l ? "LAN" : "Nessuna rete";
            string letter = w && l ? "+" : w ? "W" : l ? "L" : "x";
            Color col = w && l ? Color.FromArgb(142, 68, 173) : w ? Color.FromArgb(41, 128, 185) : l ? Color.FromArgb(39, 174, 96) : Color.FromArgb(192, 57, 43);
            var old = tray.Icon;
            tray.Icon = MakeIcon(letter, col);
            if (old != null) old.Dispose();
            tray.Text = "NetSwitch: " + txt + " (clic per cambiare)";
        }
        catch { }
    }

    static Icon MakeIcon(string letter, Color c)
    {
        using (var bmp = new Bitmap(32, 32))
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            using (var b = new SolidBrush(c)) g.FillEllipse(b, 1, 1, 30, 30);
            using (var f = new Font("Segoe UI", 17, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                var sz = g.MeasureString(letter, f);
                g.DrawString(letter, f, Brushes.White, (32 - sz.Width) / 2, (32 - sz.Height) / 2 + 1);
            }
            IntPtr h = bmp.GetHicon();
            return Icon.FromHandle(h);
        }
    }

    ContextMenuStrip BuildMenu()
    {
        var m = new ContextMenuStrip();
        m.Items.Add("Alterna Wi-Fi <-> LAN", null, (s, e) => Toggle());
        m.Items.Add("Solo Wi-Fi", null, (s, e) => Run(() => SetAll(true, false)));
        m.Items.Add("Solo LAN", null, (s, e) => Run(() => SetAll(false, true)));
        m.Items.Add("Entrambe", null, (s, e) => Run(() => SetAll(true, true)));
        m.Items.Add(new ToolStripSeparator());
        var auto = new ToolStripMenuItem("Avvia con Windows");
        auto.Checked = AutostartOn();
        auto.Click += (s, e) => { SetAutostart(!auto.Checked); auto.Checked = AutostartOn(); };
        m.Items.Add(auto);
        m.Items.Add("Esci", null, (s, e) => { tray.Visible = false; Application.Exit(); });
        return m;
    }

    void Run(Action a) { try { a(); } catch (Exception ex) { tray.ShowBalloonTip(3000, "NetSwitch", "Errore: " + ex.Message, ToolTipIcon.Error); } }

    static int Schtasks(string args)
    {
        var p = Process.Start(new ProcessStartInfo("schtasks", args) { CreateNoWindow = true, UseShellExecute = false });
        p.WaitForExit(); return p.ExitCode;
    }
    static bool AutostartOn() { return Schtasks("/Query /TN " + TaskName) == 0; }
    static void SetAutostart(bool on)
    {
        if (on) Schtasks("/Create /F /TN " + TaskName + " /SC ONLOGON /RL HIGHEST /TR \"\\\"" + Application.ExecutablePath + "\\\"\"");
        else Schtasks("/Delete /F /TN " + TaskName);
    }
}

static class Program
{
    [STAThread]
    static void Main()
    {
        bool created;
        using (var mtx = new System.Threading.Mutex(true, "NetSwitchSingleton", out created))
        {
            if (!created) return;
            Application.EnableVisualStyles();
            Application.Run(new App());
        }
    }
}
