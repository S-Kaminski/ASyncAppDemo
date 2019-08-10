using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Asyncin4PPD3M0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SyncBtn_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();
            RunDownload();
            watch.Stop();
            var ElapsedMs = watch.ElapsedMilliseconds;
            LogTxtBox.Text += $"Total execution time: {ElapsedMs}";

        }

        private async void AsyncBtn_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;

            var watch = Stopwatch.StartNew();
            var results = await RunDownloadAsync(progress);
            PrintResults(results);
                //await RunDownloadAsync(progress);
            watch.Stop();

            var ElapsedMs = watch.ElapsedMilliseconds;
            LogTxtBox.Text += $"Total execution time: {ElapsedMs}";
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            ProgressBar.Value = e.Percentage;
            PrintResults(e.WebsitesDownloaded);
        }

        private async void ParallelAsyncBtn_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();
            await RunDownloadParallelAsync();
            watch.Stop();
            var ElapsedMs = watch.ElapsedMilliseconds;
            LogTxtBox.Text += $"Total execution time: {ElapsedMs}";
        }

        private List<string> PrepData()
        {
            List<string> output = new List<string>();

            LogTxtBox.Text += ""; //clears out the log text box

            output.Add("https://www.reddit.com/");
            output.Add("https://google.com/");
            output.Add("https://osu.ppy.sh/");
            output.Add("https://youtube.com/");
            output.Add("https://www.amazon.com");
            output.Add("https://www.facebook.com");
            output.Add("https://www.twitter.com");
            output.Add("https://www.codeproject.com");
            output.Add("https://www.stackoverflow.com");
            output.Add("https://en.wikipedia.org/wiki/.NET_Framework");
            //output.Add("http://plan.ii.us.edu.pl/");

            return output;
        }

        //Sync Download Methods

        private void RunDownload()
        {
            List<string> websites = PrepData();
            LogTxtBox.Text = "";
            foreach (var site in websites)
            {
                DemoClass results = DownloadWebsite(site);
                ReportWebsiteInfo(results);
            }
        }

        private DemoClass DownloadWebsite(string websiteURL)
        {
            DemoClass output = new DemoClass();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);

            return output;
        }

        //Async Download Methods
        private async Task<List<DemoClass>> RunDownloadAsync(IProgress<ProgressReportModel> progress)
        {
            List<string> websites = PrepData();
            List<DemoClass> output = new List<DemoClass>();
            ProgressReportModel report = new ProgressReportModel();

            LogTxtBox.Text = "";
            foreach (var site in websites)
            {
                DemoClass result = await DownloadWebsiteAsync(site);
                output.Add(result);


                report.WebsitesDownloaded = output;
                report.Percentage = (output.Count * 100) / websites.Count;
                progress.Report(report);

               // ReportWebsiteInfo(results);
            }

            return output;
        }

        private async Task<DemoClass> DownloadWebsiteAsync(string websiteURL)
        {
            DemoClass output = new DemoClass();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);

            return output;
        }


        //Parallel Download Methods
        private async Task RunDownloadParallelAsync()
        {
            LogTxtBox.Text = "";
            List<string> websites = PrepData();
            List<Task<DemoClass>> tasks = new List<Task<DemoClass>>();

            foreach (var site in websites) tasks.Add(DownloadWebsiteAsync(site));

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                ReportWebsiteInfo(result);
            }

        }

        //Shared Methods
        private void ReportWebsiteInfo(DemoClass data)
        {
            LogTxtBox.Text += $"{data.WebsiteUrl} downloaded: {data.WebsiteData.Length} characters long. {Environment.NewLine}";
        }

        private void PrintResults(List<DemoClass> results)
        {
            LogTxtBox.Text = "";
            foreach (var result in results)
            {
                LogTxtBox.Text += $"{result.WebsiteUrl} downloaded: {result.WebsiteData.Length} characters long. {Environment.NewLine}";
            }
        }

        
    }
}
