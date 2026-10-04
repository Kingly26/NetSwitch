using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

[assembly: AssemblyTitle("NetSwitch")]

class Adapter { public ManagementObject Obj; public string Name; public bool Enabled; public bool IsWifi; }

static class Lang
{
    static readonly bool It = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "it";
    public static string T(string it, string en) { return It ? it : en; }
}

static class Settings
{
    const string Key = @"Software\NetSwitch";

    static int Get(string name, int def)
    {
        try { using (var k = Registry.CurrentUser.OpenSubKey(Key)) return k == null ? def : Convert.ToInt32(k.GetValue(name, def)); }
        catch { return def; }
    }
    static void Set(string name, int value)
    {
        try { using (var k = Registry.CurrentUser.CreateSubKey(Key)) k.SetValue(name, value, RegistryValueKind.DWord); }
        catch { }
    }

    public static bool ClickToggle { get { return Get("ClickToggle", 0) != 0; } set { Set("ClickToggle", value ? 1 : 0); } }
    public static int IconStyle
    {
        get { int s = Get("IconStyle", 0); return s < 0 || s >= Icons.Count ? 0 : s; }
        set { Set("IconStyle", value); }
    }
}

static class Icons
{
    // state: 0 = no network, 1 = Wi-Fi, 2 = LAN, 3 = both
    // 0-6 are the classic styles; 7-15 are gothic: three groups (letters, plug symbols, tree symbols)
    // of three variants each (plain, thorns, red thorns)
    public const int Count = 16, FirstGothic = 7;
    static readonly string[][] GothicText = { new[] { "x", "W", "L", "+" }, new[] { "x", "w", "e", "b" }, new[] { "x", "w", "t", "c" } };
    static readonly string[] Letters = { "x", "W", "L", "+" };
    static readonly string[] Glyphs = { "", "", "", "" };
    static readonly Color[] Solid = { Color.FromArgb(192, 57, 43), Color.FromArgb(41, 128, 185), Color.FromArgb(39, 174, 96), Color.FromArgb(142, 68, 173) };
    // brighter variants, readable without a background on a dark taskbar
    static readonly Color[] Bright = { Color.FromArgb(255, 100, 90), Color.FromArgb(86, 180, 255), Color.FromArgb(80, 220, 130), Color.FromArgb(200, 140, 255) };

    public static string Name(int style)
    {
        switch (style)
        {
            case 0: return Lang.T("Cerchio pieno", "Filled circle");
            case 1: return Lang.T("Quadrato arrotondato", "Rounded square");
            case 2: return Lang.T("Anello (senza sfondo)", "Ring (no background)");
            case 3: return Lang.T("Lettera colorata (senza sfondo)", "Colored letter (no background)");
            case 4: return Lang.T("Lettera monocromatica (senza sfondo)", "Monochrome letter (no background)");
            case 5: return Lang.T("Simbolo colorato (senza sfondo)", "Colored symbol (no background)");
            case 6: return Lang.T("Simbolo monocromatico (senza sfondo)", "Monochrome symbol (no background)");
        }
        int k = style - FirstGothic;
        string variant = k % 3 == 0 ? Lang.T("Gotico", "Gothic") : k % 3 == 1 ? Lang.T("Gotico con spine", "Gothic with thorns") : Lang.T("Gotico rosso", "Gothic red");
        string group = k / 3 == 0 ? Lang.T("lettere", "letters") : k / 3 == 1 ? Lang.T("simboli (presa)", "symbols (plug)") : Lang.T("simboli (albero)", "symbols (tree)");
        return variant + ": " + group;
    }

    public static bool LightTaskbar()
    {
        try
        {
            using (var k = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                return k != null && Convert.ToInt32(k.GetValue("SystemUsesLightTheme", 0)) != 0;
        }
        catch { return false; }
    }

    static string IconFont()
    {
        foreach (var name in new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" })
            using (var f = new Font(name, 10)) if (f.Name == name) return name;
        return null;
    }

    static void Text(Graphics g, string s, string font, float px, FontStyle fs, Color c, float dy)
    {
        using (var f = new Font(font, px, fs, GraphicsUnit.Pixel))
        using (var b = new SolidBrush(c))
        using (var sf = new StringFormat(StringFormat.GenericTypographic))
        {
            sf.Alignment = StringAlignment.Center; sf.LineAlignment = StringAlignment.Center;
            g.DrawString(s, f, b, new RectangleF(0, dy, 32, 32), sf);
        }
    }

    public static Bitmap Render(int style, int state, bool light)
    {
        var bmp = new Bitmap(32, 32);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            Color solid = Solid[state], accent = light ? Solid[state] : Bright[state];
            Color mono = light ? Color.FromArgb(30, 30, 30) : Color.White;
            string letter = Letters[state], iconFont = IconFont();
            // without the icon font, the symbol styles fall back to the letter styles
            if (iconFont == null && (style == 5 || style == 6)) style -= 2;
            if (style >= FirstGothic)
            {
                int k = style - FirstGothic;
                Color red = light ? Color.FromArgb(190, 25, 35) : Color.FromArgb(225, 40, 45);
                Gothic.Draw(g, GothicText[k / 3][state], new RectangleF(0, 0, 32, 32), k % 3 == 2 ? red : mono, k % 3 != 0);
                return bmp;
            }
            switch (style)
            {
                case 0:
                    using (var b = new SolidBrush(solid)) g.FillEllipse(b, 1, 1, 30, 30);
                    Text(g, letter, "Segoe UI", 18, FontStyle.Bold, Color.White, 0);
                    break;
                case 1:
                    using (var b = new SolidBrush(solid))
                    using (var p = new GraphicsPath())
                    {
                        int r = 14;
                        p.AddArc(1, 1, r, r, 180, 90); p.AddArc(31 - r, 1, r, r, 270, 90);
                        p.AddArc(31 - r, 31 - r, r, r, 0, 90); p.AddArc(1, 31 - r, r, r, 90, 90);
                        p.CloseFigure();
                        g.FillPath(b, p);
                    }
                    Text(g, letter, "Segoe UI", 19, FontStyle.Bold, Color.White, 0);
                    break;
                case 2:
                    using (var p = new Pen(accent, 3f)) g.DrawEllipse(p, 2.5f, 2.5f, 27, 27);
                    Text(g, letter, "Segoe UI", 16, FontStyle.Bold, accent, 0);
                    break;
                case 3: Text(g, letter, "Segoe UI", 28, FontStyle.Bold, accent, 0); break;
                case 4: Text(g, letter, "Segoe UI", 28, FontStyle.Bold, mono, 0); break;
                case 5: Text(g, Glyphs[state], iconFont, 28, FontStyle.Regular, accent, 1); break;
                default: Text(g, Glyphs[state], iconFont, 28, FontStyle.Regular, mono, 1); break;
            }
        }
        return bmp;
    }

    // contact sheet of every style and state, on a dark and a light taskbar
    public static void SavePreview(string path)
    {
        const int cell = 56, labelW = 300, pad = 12;
        int w = labelW + cell * 8 + pad * 3, h = pad * 2 + 28 + cell * Count;
        using (var bmp = new Bitmap(w, h))
        using (var g = Graphics.FromImage(bmp))
        using (var f = new Font("Segoe UI", 15, FontStyle.Regular, GraphicsUnit.Pixel))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.Clear(Color.FromArgb(250, 250, 250));
            int xDark = labelW + pad, xLight = xDark + cell * 4 + pad, y0 = pad + 28;
            using (var d = new SolidBrush(Color.FromArgb(32, 32, 32))) g.FillRectangle(d, xDark, y0, cell * 4, cell * Count);
            using (var l = new SolidBrush(Color.FromArgb(238, 238, 238))) g.FillRectangle(l, xLight, y0, cell * 4, cell * Count);
            string[] cols = { "Wi-Fi", "LAN", "W+L", "---" };
            int[] states = { 1, 2, 3, 0 };
            for (int c = 0; c < 4; c++)
            {
                g.DrawString(cols[c], f, Brushes.Black, xDark + c * cell + 6, pad);
                g.DrawString(cols[c], f, Brushes.Black, xLight + c * cell + 6, pad);
            }
            for (int s = 0; s < Count; s++)
            {
                int y = y0 + s * cell;
                g.DrawString((s + 1) + ". " + Name(s), f, Brushes.Black, pad, y + 18);
                for (int c = 0; c < 4; c++)
                {
                    using (var a = Render(s, states[c], false)) g.DrawImage(a, xDark + c * cell + 12, y + 12, 32, 32);
                    using (var b = Render(s, states[c], true)) g.DrawImage(b, xLight + c * cell + 12, y + 12, 32, 32);
                }
            }
            bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}

// Minimal Native Wifi wrapper: Windows does not auto-connect Wi-Fi while Ethernet is up,
// so after enabling the adapter we have to ask for the connection ourselves.
static class Wlan
{
    [DllImport("wlanapi.dll")] static extern int WlanOpenHandle(uint version, IntPtr reserved, out uint negotiated, out IntPtr handle);
    [DllImport("wlanapi.dll")] static extern int WlanCloseHandle(IntPtr handle, IntPtr reserved);
    [DllImport("wlanapi.dll")] static extern int WlanEnumInterfaces(IntPtr handle, IntPtr reserved, out IntPtr list);
    [DllImport("wlanapi.dll")] static extern int WlanGetProfileList(IntPtr handle, ref Guid iface, IntPtr reserved, out IntPtr list);
    [DllImport("wlanapi.dll")] static extern int WlanConnect(IntPtr handle, ref Guid iface, ref ConnParams p, IntPtr reserved);
    [DllImport("wlanapi.dll")] static extern void WlanFreeMemory(IntPtr p);

    [StructLayout(LayoutKind.Sequential)]
    struct ConnParams
    {
        public int Mode;                                              // 0 = connect using a saved profile
        [MarshalAs(UnmanagedType.LPWStr)] public string Profile;
        public IntPtr Ssid, BssidList;
        public int BssType;                                           // 3 = any
        public int Flags;
    }

    public class Iface { public Guid Id; public int State; public List<string> Profiles = new List<string>(); }

    // State: 1 = connected, 4 = disconnected, 5/6/7 = associating/discovering/authenticating
    public static List<Iface> Interfaces()
    {
        var result = new List<Iface>();
        IntPtr h; uint ver;
        if (WlanOpenHandle(2, IntPtr.Zero, out ver, out h) != 0) return result;
        try
        {
            IntPtr list;
            if (WlanEnumInterfaces(h, IntPtr.Zero, out list) != 0) return result;
            int n = Marshal.ReadInt32(list);
            for (int i = 0; i < n; i++)
            {
                // WLAN_INTERFACE_INFO: GUID (16) + description WCHAR[256] (512) + state (4)
                IntPtr p = IntPtr.Add(list, 8 + i * 532);
                var f = new Iface { Id = (Guid)Marshal.PtrToStructure(p, typeof(Guid)), State = Marshal.ReadInt32(p, 528) };
                IntPtr profiles;
                if (WlanGetProfileList(h, ref f.Id, IntPtr.Zero, out profiles) == 0)
                {
                    int pn = Marshal.ReadInt32(profiles);
                    // WLAN_PROFILE_INFO: name WCHAR[256] (512) + flags (4), in the user's preference order
                    for (int k = 0; k < pn; k++) f.Profiles.Add(Marshal.PtrToStringUni(IntPtr.Add(profiles, 8 + k * 516)));
                    WlanFreeMemory(profiles);
                }
                result.Add(f);
            }
            WlanFreeMemory(list);
        }
        finally { WlanCloseHandle(h, IntPtr.Zero); }
        return result;
    }

    public static bool Connect(Guid iface, string profile)
    {
        IntPtr h; uint ver;
        if (WlanOpenHandle(2, IntPtr.Zero, out ver, out h) != 0) return false;
        try
        {
            var p = new ConnParams { Mode = 0, Profile = profile, BssType = 3 };
            return WlanConnect(h, ref iface, ref p, IntPtr.Zero) == 0;
        }
        finally { WlanCloseHandle(h, IntPtr.Zero); }
    }
}

class App : ApplicationContext
{
    [DllImport("user32.dll")] static extern bool DestroyIcon(IntPtr h);

    NotifyIcon tray = new NotifyIcon();
    Timer timer = new Timer();
    Timer wifiTimer = new Timer();
    int wifiTicks, wifiProfile, wifiWait;
    IntPtr iconHandle = IntPtr.Zero;
    string iconKey = "";
    int state = 0;
    const string TaskName = "NetSwitchAutostart";
    static readonly Regex WifiRx = new Regex("wi-?fi|wireless|wlan|802[.]11", RegexOptions.IgnoreCase);
    static readonly Regex SkipRx = new Regex("bluetooth|virtual|vmware|hyper-v|vethernet|tap-|vpn|loopback|wan miniport|pseudo|npcap|wintun", RegexOptions.IgnoreCase);

    public App()
    {
        tray.Visible = true;
        tray.ContextMenuStrip = new ContextMenuStrip();
        tray.ContextMenuStrip.Opening += (s, e) => { BuildMenu(tray.ContextMenuStrip); e.Cancel = false; };
        tray.MouseClick += (s, e) =>
        {
            if (e.Button != MouseButtons.Left) return;
            if (Settings.ClickToggle) Toggle();
            else ShowMenu();
        };
        wifiTimer.Interval = 2500;
        wifiTimer.Tick += (s, e) => WifiTick();
        timer.Interval = 3000;
        timer.Tick += (s, e) => Refresh();
        timer.Start();
        Refresh();
    }

    // opens the tray menu the same way a right-click does
    void ShowMenu()
    {
        var mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
        if (mi != null) mi.Invoke(tray, null);
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
        if (wifi) EnsureWifiConnected(); else wifiTimer.Stop();
    }

    // Watches the Wi-Fi adapter for about half a minute after it is enabled. If Windows does not
    // connect it on its own (it will not while Ethernet is up), tries the saved networks in order.
    void EnsureWifiConnected()
    {
        wifiTicks = 0; wifiProfile = 0; wifiWait = 1;
        wifiTimer.Stop();
        wifiTimer.Start();
    }

    void WifiTick()
    {
        try
        {
            if (++wifiTicks > 14) { wifiTimer.Stop(); return; }
            var ifs = Wlan.Interfaces();
            if (ifs.Count == 0) return;                                  // adapter still starting
            if (ifs.Exists(f => f.State == 1)) { wifiTimer.Stop(); return; }
            if (ifs.Exists(f => f.State >= 5 && f.State <= 7)) return;   // a connection is in progress
            if (wifiWait > 0) { wifiWait--; return; }
            var wifi = ifs[0];
            if (wifiProfile >= wifi.Profiles.Count) { wifiTimer.Stop(); return; }
            Wlan.Connect(wifi.Id, wifi.Profiles[wifiProfile++]);
            wifiWait = 2;                                                // give this network time before the next one
        }
        catch { wifiTimer.Stop(); }
    }

    void Toggle()
    {
        Run(() =>
        {
            var all = GetAdapters();
            bool wifiOn = all.Exists(a => a.IsWifi && a.Enabled);
            SetAll(!wifiOn, wifiOn);
        });
    }

    void Refresh()
    {
        try
        {
            var all = GetAdapters();
            bool w = all.Exists(a => a.IsWifi && a.Enabled), l = all.Exists(a => !a.IsWifi && a.Enabled);
            state = w && l ? 3 : w ? 1 : l ? 2 : 0;
            string txt = w && l ? "Wi-Fi + LAN" : w ? "Wi-Fi" : l ? "LAN" : Lang.T("Nessuna rete", "No network");
            tray.Text = "NetSwitch: " + txt + (Settings.ClickToggle ? Lang.T(" (clic per cambiare)", " (click to switch)") : "");
            UpdateIcon();
        }
        catch { }
    }

    void UpdateIcon()
    {
        int style = Settings.IconStyle;
        bool light = Icons.LightTaskbar();
        string key = style + "/" + state + "/" + light;
        if (key == iconKey) return;
        IntPtr h;
        using (var bmp = Icons.Render(style, state, light)) h = bmp.GetHicon();
        tray.Icon = Icon.FromHandle(h);
        if (iconHandle != IntPtr.Zero) DestroyIcon(iconHandle);
        iconHandle = h;
        iconKey = key;
    }

    void BuildMenu(ContextMenuStrip m)
    {
        m.Items.Clear();
        m.Items.Add(Lang.T("Alterna Wi-Fi <-> LAN", "Switch Wi-Fi <-> LAN"), null, (s, e) => Toggle());
        m.Items.Add(Lang.T("Solo Wi-Fi", "Wi-Fi only"), null, (s, e) => Run(() => SetAll(true, false)));
        m.Items.Add(Lang.T("Solo LAN", "LAN only"), null, (s, e) => Run(() => SetAll(false, true)));
        m.Items.Add(Lang.T("Entrambe", "Both"), null, (s, e) => Run(() => SetAll(true, true)));
        m.Items.Add(new ToolStripSeparator());

        var click = new ToolStripMenuItem(Lang.T("Alterna con un clic sull'icona", "Switch by clicking the icon"));
        click.Checked = Settings.ClickToggle;
        click.Click += (s, e) => { Settings.ClickToggle = !Settings.ClickToggle; Refresh(); };
        m.Items.Add(click);

        var styles = new ToolStripMenuItem(Lang.T("Stile icona", "Icon style"));
        int current = Settings.IconStyle;
        for (int i = 0; i < Icons.Count; i++)
        {
            int style = i;
            if (style >= Icons.FirstGothic && (style - Icons.FirstGothic) % 3 == 0) styles.DropDownItems.Add(new ToolStripSeparator());
            var it = new ToolStripMenuItem(Icons.Name(style), Icons.Render(style, state, true));
            it.Checked = style == current;
            it.Click += (s, e) => { Settings.IconStyle = style; UpdateIcon(); };
            styles.DropDownItems.Add(it);
        }
        m.Items.Add(styles);

        var auto = new ToolStripMenuItem(Lang.T("Avvia con Windows", "Start with Windows"));
        auto.Checked = AutostartOn();
        auto.Click += (s, e) => SetAutostart(!AutostartOn());
        m.Items.Add(auto);
        m.Items.Add(new ToolStripSeparator());
        m.Items.Add(Lang.T("Esci", "Exit"), null, (s, e) => { tray.Visible = false; Application.Exit(); });
    }

    void Run(Action a) { try { a(); } catch (Exception ex) { tray.ShowBalloonTip(3000, "NetSwitch", Lang.T("Errore: ", "Error: ") + ex.Message, ToolTipIcon.Error); } }

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
    static void Main(string[] args)
    {
        if (args.Length == 2 && args[0] == "--preview") { Icons.SavePreview(args[1]); return; }
        bool created;
        using (var mtx = new System.Threading.Mutex(true, "NetSwitchSingleton", out created))
        {
            if (!created) return;
            Application.EnableVisualStyles();
            Application.Run(new App());
        }
    }
}
