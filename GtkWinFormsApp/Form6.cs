using System;
using System.Windows.Forms;

namespace GtkWinFormsApp
{
    /// <summary>
    /// A minimal browser over <see cref="WebBrowser"/>: an address bar, the history and reload commands,
    /// and the status line. Deliberately written the way a ported WinForms application would be - the
    /// navigation events drive the button states, and nothing here knows which engine is behind the
    /// control.
    /// </summary>
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();

            webBrowser1.Navigating += webBrowser1_Navigating;
            webBrowser1.Navigated += webBrowser1_Navigated;
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            webBrowser1.StatusTextChanged += webBrowser1_StatusTextChanged;
            webBrowser1.DocumentTitleChanged += webBrowser1_DocumentTitleChanged;
            webBrowser1.CanGoBackChanged += webBrowser1_CanGoChanged;
            webBrowser1.CanGoForwardChanged += webBrowser1_CanGoChanged;

            UpdateHistoryButtons();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            // The bare string overload on purpose: a host name with no scheme has to navigate, as it does
            // in real WinForms.
            webBrowser1.Navigate(addressBox.Text);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh(WebBrowserRefreshOption.Completely);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            webBrowser1.Stop();
        }

        private void UpdateHistoryButtons()
        {
            backButton.Enabled = webBrowser1.CanGoBack;
            forwardButton.Enabled = webBrowser1.CanGoForward;
        }

        private void webBrowser1_CanGoChanged(object? sender, EventArgs e)
        {
            UpdateHistoryButtons();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Document is readable here and nowhere earlier: the snapshot lands before this event.
            var document = webBrowser1.Document;
            var links = document?.Links.Count ?? 0;

            statusLabel.Text = document == null
                ? "Loaded " + e.Url + " (no document)"
                : "Loaded " + e.Url + " - " + links + " link(s)";

            UpdateHistoryButtons();
        }

        private void webBrowser1_DocumentTitleChanged(object? sender, EventArgs e)
        {
            Text = string.IsNullOrEmpty(webBrowser1.DocumentTitle)
                ? "WebBrowser"
                : "WebBrowser - " + webBrowser1.DocumentTitle;
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            addressBox.Text = e.Url.ToString();
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            statusLabel.Text = "Navigating to " + e.Url + "…";
        }

        private void webBrowser1_StatusTextChanged(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(webBrowser1.StatusText))
            {
                statusLabel.Text = webBrowser1.StatusText;
            }
        }
    }
}
