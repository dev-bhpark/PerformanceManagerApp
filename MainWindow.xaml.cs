using System.Windows;
using System.Windows.Threading;

namespace PerformanceManagerApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MyDataManager dataManager = new MyDataManager();
    private DispatcherTimer timer;

    public MainWindow()
    {
        InitializeComponent();

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Time_Tick;

        timer.Start();
    }

    private void Time_Tick(object? sender, EventArgs e)
    {
        var snapshot = dataManager.GetCurrentSystemSnapShot();

        CpuTextBlock.Text = $"CPU Usage: {snapshot.Cpu:F2}%";
        RamTextBlock.Text = $"RAM Usage: {snapshot.UsedRamPercent:F2}%";
        // RamProgressBar.Value = snapshot.UsedRamPercent;
        var formattedList = snapshot.Procs
            .Select(p => $"{p.Name,-50} | {p.Ram:F2} MB")
            .ToList();

        ProcessListBox.ItemsSource = formattedList;
    }

    // void button_Click(object sender, RoutedEventArgs e)
    // {
    //     MessageBox.Show("Hello, windows Presentation Foundation!");
    // }
}