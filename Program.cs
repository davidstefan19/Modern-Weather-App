using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace weatherapp
{
    public class WeatherAppForm : Form
    {
        private TextBox cityInput;
        private Label resultLabel;
        private Button searchBtn;
        private ListBox historyBox;
        private Panel cardPanel;
        private string filePath = "history.txt";

        private Dictionary<string, string> cityMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "nisa", "nice" },
            { "londra", "london" },
            { "roma", "rome" }
        };

        public WeatherAppForm()
        {
            this.Text = "Weather Monitor Pro";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.KeyPreview = true;

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };

            cardPanel = new Panel();
            cardPanel.Width = 450;
            cardPanel.Height = 600;
            cardPanel.BackColor = Color.Transparent;
            this.Controls.Add(cardPanel);

            cardPanel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
                {
                    FillRoundedRectangle(e.Graphics, brush, cardPanel.ClientRectangle, 30);
                }
                using (Pen pen = new Pen(Color.FromArgb(220, 255, 255, 255), 2))
                {
                    DrawRoundedRectangle(e.Graphics, pen, cardPanel.ClientRectangle, 30);
                }
            };

            this.Load += (s, e) => { CenterCard(); };
            this.Resize += (s, e) => { CenterCard(); };

            Label titleLabel = new Label();
            titleLabel.Text = "WEATHER";
            titleLabel.Font = new Font("Segoe UI Light", 26, FontStyle.Regular);
            titleLabel.ForeColor = Color.FromArgb(30, 41, 59);
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Bounds = new Rectangle(25, 40, 400, 50);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            cardPanel.Controls.Add(titleLabel);

            Panel inputWrapper = new Panel();
            inputWrapper.Bounds = new Rectangle(50, 120, 350, 50);
            inputWrapper.BackColor = Color.FromArgb(248, 250, 252);
            inputWrapper.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(200, 203, 213, 225), 1))
                {
                    DrawRoundedRectangle(e.Graphics, pen, inputWrapper.ClientRectangle, 15);
                }
            };
            cardPanel.Controls.Add(inputWrapper);

            cityInput = new TextBox();
            cityInput.Bounds = new Rectangle(15, 12, 320, 30);
            cityInput.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            cityInput.BorderStyle = BorderStyle.None;
            cityInput.BackColor = Color.FromArgb(248, 250, 252);
            cityInput.TextAlign = HorizontalAlignment.Center;
            inputWrapper.Controls.Add(cityInput);

            searchBtn = new Button();
            searchBtn.Text = "SEARCH";
            searchBtn.Bounds = new Rectangle(50, 185, 350, 50);
            searchBtn.BackColor = Color.FromArgb(15, 23, 42);
            searchBtn.ForeColor = Color.White;
            searchBtn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            searchBtn.FlatStyle = FlatStyle.Flat;
            searchBtn.FlatAppearance.BorderSize = 0;
            searchBtn.Cursor = Cursors.Hand;
            searchBtn.Click += new EventHandler(OnSearchClick);
            cardPanel.Controls.Add(searchBtn);

            resultLabel = new Label();
            resultLabel.Text = "--";
            resultLabel.Font = new Font("Segoe UI Semibold", 22, FontStyle.Bold);
            resultLabel.ForeColor = Color.FromArgb(15, 23, 42);
            resultLabel.BackColor = Color.Transparent;
            resultLabel.Bounds = new Rectangle(25, 260, 400, 80);
            resultLabel.TextAlign = ContentAlignment.MiddleCenter;
            cardPanel.Controls.Add(resultLabel);

            Label historyLabel = new Label();
            historyLabel.Text = "HISTORY";
            historyLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            historyLabel.ForeColor = Color.FromArgb(100, 116, 139);
            historyLabel.BackColor = Color.Transparent;
            historyLabel.Bounds = new Rectangle(50, 370, 350, 20);
            cardPanel.Controls.Add(historyLabel);

            historyBox = new ListBox();
            historyBox.Bounds = new Rectangle(50, 395, 350, 160);
            historyBox.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            historyBox.ForeColor = Color.FromArgb(51, 65, 85);
            historyBox.BorderStyle = BorderStyle.None;
            historyBox.BackColor = Color.FromArgb(240, 244, 248);
            historyBox.ItemHeight = 30;
            historyBox.DoubleClick += new EventHandler(OnHistoryDoubleClick);
            cardPanel.Controls.Add(historyBox);

            LoadHistoryFromFile();
        }

        private void CenterCard()
        {
            cardPanel.Left = (this.ClientSize.Width - cardPanel.Width) / 2;
            cardPanel.Top = (this.ClientSize.Height - cardPanel.Height) / 2;
        }

        private void FillRoundedRectangle(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath path = GetRoundedPath(rect, radius))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawRoundedRectangle(Graphics g, Pen pen, Rectangle rect, int radius)
        {
            using (GraphicsPath path = GetRoundedPath(rect, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private async void OnSearchClick(object sender, EventArgs e)
        {
            string city = cityInput.Text.Trim();
            if (string.IsNullOrEmpty(city)) return;
            await FetchWeatherAndBackgroundAsync(city);
        }

        private async void OnHistoryDoubleClick(object sender, EventArgs e)
        {
            if (historyBox.SelectedItem != null)
            {
                string selectedCity = historyBox.SelectedItem.ToString();
                cityInput.Text = selectedCity;
                await FetchWeatherAndBackgroundAsync(selectedCity);
            }
        }

        private async Task FetchWeatherAndBackgroundAsync(string city)
        {
            resultLabel.Text = "...";
            string searchKey = city.ToLower().Trim();
            string englishQuery = cityMap.ContainsKey(searchKey) ? cityMap[searchKey] : searchKey;

            try
            {
                using (HttpClient clientWeather = new HttpClient())
                {
                    string weather = await clientWeather.GetStringAsync($"https://wttr.in/{Uri.EscapeDataString(searchKey)}?format=%C+%t");
                    resultLabel.Text = weather;
                }

                using (HttpClient clientImage = new HttpClient())
                {
                    clientImage.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    string imageUrl = $"https://images.unsplash.com/featured/1920x1080/?{Uri.EscapeDataString(englishQuery)},city,landscape";

                    byte[] imageBytes = await clientImage.GetByteArrayAsync(imageUrl);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        this.BackgroundImage = Image.FromStream(ms);
                        this.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                }

                string formatted = char.ToUpper(city[0]) + city.Substring(1).ToLower();
                if (!historyBox.Items.Contains(formatted))
                {
                    historyBox.Items.Insert(0, formatted);
                    SaveHistoryToFile();
                }
            }
            catch
            {
                if (resultLabel.Text == "...")
                {
                    resultLabel.Text = "NOT FOUND";
                }
            }
        }

        private void SaveHistoryToFile()
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (var item in historyBox.Items)
                {
                    lines.Add(item.ToString());
                }
                File.WriteAllLines(filePath, lines);
            }
            catch { }
        }

        private void LoadHistoryFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    foreach (var line in File.ReadAllLines(filePath))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            historyBox.Items.Add(line);
                    }
                }
            }
            catch { }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WeatherAppForm());
        }
    }
}