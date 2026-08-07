using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CreamInstaller.Forms;
using CreamInstaller.Utility;

namespace CreamInstaller.Components;

internal class CustomForm : Form
{
    internal CustomForm()
    {
        Icon = Properties.Resources.Icon;
        KeyPreview = true;
        KeyPress += OnKeyPress;
        ResizeRedraw = true;
        HelpButton = true;
        HelpButtonClicked += OnHelpButtonClicked;
    }

    internal CustomForm(IWin32Window owner) : this()
    {
        if (owner is not Form form)
            return;
        Owner = form;
        InheritLocation(form);
        SizeChanged += (_, _) => InheritLocation(form);
        form.Activated += OnActivation;
        FormClosing += (_, _) => form.Activated -= OnActivation;
        TopLevel = true;
    }

    protected override CreateParams CreateParams // Double buffering for all controls
    {
        get
        {
            CreateParams handleParam = base.CreateParams;
            handleParam.ExStyle |= 0x02; // WS_EX_COMPOSITED       
            return handleParam;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ThemeManager.Apply(this); // apply current theme (initial or toggled)
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        ThemeManager.Apply(this); // ensure late-added controls also themed
    }

    private void OnHelpButtonClicked(object sender, EventArgs args)
    {
        using DialogForm helpDialog = new(this);
        helpDialog.HelpButton = false;

        const string acidicoala = "https://github.com/acidicoala";
        string repository = $"https://github.com/{Program.RepositoryOwner}/{Program.RepositoryName}";
        string discussions = Program.CommunityDiscussions;
        string forum = Program.CommunityForum;
        string abuse = Program.AbuseEmail;
        string donate = Program.DonateUrl;
        _ = helpDialog.Show(SystemIcons.Information,
            $"CreamInstaller v{Program.Version} — Help & Disclaimer\n"
          + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
          + "⚠️  DISCLAIMER\n"
          + "This software is an open-source project developed for the community\n"
          + "and is NOT affiliated with any organization or institution.\n"
          + "It is shared purely for EDUCATIONAL PURPOSES and software development\n"
          + "testing. This software is NOT intended for production use.\n"
          + "We strongly recommend purchasing and using professionally licensed\n"
          + "software for your needs.\n\n"
          + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
          + "🛡️  ANTIVIRUS / FALSE POSITIVE WARNING\n"
          + "All software that modifies or interacts with DLL files may be flagged\n"
          + "by antivirus programs. The ENTIRE PROJECT IS OPEN SOURCE — no\n"
          + "encrypted or obfuscated code is included. If flagged, it is a false\n"
          + "positive. This software is FOR EXPERIENCED USERS ONLY.\n\n"
          + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
          + "⚖️  LEGAL RESPONSIBILITY\n"
          + "By using this software, you agree that ALL RESPONSIBILITY LIES WITH\n"
          + "YOU, THE USER. The platform and its contributors provide this software\n"
          + "\"as is\", without any warranty of any kind, express or implied.\n"
          + "USE AT YOUR OWN RISK.\n\n"
          + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
          + "📖  ABOUT\n"
          + "Automatically finds all installed Steam, Epic and Ubisoft games with\n"
          + "their DLC-related DLL locations, parses SteamCMD / Steam Store /\n"
          + "Epic Games Store for DLCs, then provides a graphical interface for\n"
          + "maintenance of DLC unlockers.\n\n"
          + $"Utilizes [CreamAPI](https://cs.rin.ru/forum/viewtopic.php?f=29&t=70576), [Koaloader]({acidicoala}/Koaloader), [SmokeAPI]({acidicoala}/SmokeAPI),\n"
          + $"[ScreamAPI]({acidicoala}/ScreamAPI), [Uplay R1 Unlocker]({acidicoala}/UplayR1Unlocker)\n"
          + $"and [Uplay R2 Unlocker]({acidicoala}/UplayR2Unlocker) by [acidicoala]({acidicoala}).\n\n"
          + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
          + "🙌  COMMUNITY SUPPORT  (NO OFFICIAL SUPPORT IS PROVIDED)\n"
          + $"• GitHub Discussions → [{discussions}]({discussions})\n"
          + $"• ubden Forum        → [{forum}]({forum})\n\n"
          + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
          + $"🚨  Report abuse / misuse: [{abuse}](mailto:{abuse})\n"
          + $"💙  Support the community: [{donate}]({donate})\n"
          + $"📁  Source code: [{repository}]({repository})\n");
    }

    private void OnActivation(object sender, EventArgs args) => Activate();

    internal void BringToFrontWithoutActivation()
    {
        bool topMost = TopMost;
        NativeImports.SetWindowPos(Handle, NativeImports.HWND_TOPMOST, 0, 0, 0, 0,
            NativeImports.SWP_NOACTIVATE | NativeImports.SWP_SHOWWINDOW | NativeImports.SWP_NOMOVE |
            NativeImports.SWP_NOSIZE);
        if (!topMost)
            NativeImports.SetWindowPos(Handle, NativeImports.HWND_NOTOPMOST, 0, 0, 0, 0,
                NativeImports.SWP_NOACTIVATE | NativeImports.SWP_SHOWWINDOW | NativeImports.SWP_NOMOVE |
                NativeImports.SWP_NOSIZE);
    }

    internal void InheritLocation(Form fromForm)
    {
        if (fromForm is null)
            return;
        int X = fromForm.Location.X + fromForm.Size.Width / 2 - Size.Width / 2;
        int Y = fromForm.Location.Y + fromForm.Size.Height / 2 - Size.Height / 2;
        Location = new(X, Y);
    }

    private void OnKeyPress(object s, KeyPressEventArgs e)
    {
        if (e.KeyChar != 'S')
            return; // Shift + S
        UpdateBounds();
        Rectangle bounds = Bounds;
        using Bitmap bitmap = new(Size.Width - 14, Size.Height - 7);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        using EncoderParameters encoding = new(1);
        using EncoderParameter encoderParam = new(Encoder.Quality, 100L);
        encoding.Param[0] = encoderParam;
        graphics.CopyFromScreen(new(bounds.Left + 7, bounds.Top), Point.Empty, new(Size.Width - 14, Size.Height - 7));
        Clipboard.SetImage(bitmap);
        e.Handled = true;
    }
}