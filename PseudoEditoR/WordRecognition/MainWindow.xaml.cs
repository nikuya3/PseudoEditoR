// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR.UserInteraction
{
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using PseudoEditoR.WordRecognition;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <remarks>
    /// The main <see cref="Window"/>, in which the user controls the application.
    /// </remarks>
    public partial class MainWindow
    {
        /// <summary>
        /// The delay for the execution of the <see cref="TimerCallback"/> of <see cref="MainWindow.codeAlteredTimer"/>.
        /// </summary>
        private const int TimerDelay = 500;

        /// <summary>
        /// A <see cref="TaskScheduler"/> with GUI-context.
        /// </summary>
        private readonly TaskScheduler scheduler;

        /// <summary>
        /// A <see cref="Timer"/> executing performance intensive operations linked with code changes only when they are needed.
        /// </summary>
        private readonly Timer codeAlteredTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            this.codeAlteredTimer = new Timer(state => MainClass.StartPerformanceIntensiveCodeAlteredTasksAsync(this.scheduler), null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Triggers if the user clicked on a <see cref="GridViewColumnHeader"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="RoutedEventArgs"/>.</param>
        public void GridViewColumnHeader_Clicked(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Triggers if the <see cref="MainWindow"/> is done loading.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="object"/> which triggered the event.
        /// </param>
        /// <param name="e">
        /// The specific <see cref="RoutedEventArgs"/>.
        /// </param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => MainClass.StartAsync(this, this.scheduler), CancellationToken.None, TaskCreationOptions.LongRunning, this.scheduler);
        }

        /// <summary>
        /// Triggers if the <see cref="MainWindow"/> is to be closed by the user.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="object"/> which triggered the event.
        /// </param>
        /// <param name="e">
        /// The specific <see cref="CancelEventArgs"/>.
        /// </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.codeAlteredTimer.Dispose();
            MainClass.ExitApplication();
        }

        /// <summary>
        /// Triggers if the text in the <see cref="CodeTextBox"/> changed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="TextChangedEventArgs"/>.</param>
        private async void CodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await MainClass.CodeAlteredAsync(CodeTextBox, e, this.scheduler);
            this.codeAlteredTimer.Change(TimerDelay, Timeout.Infinite);
        }

        /// <summary>
        /// Triggers if a <see cref="Key"/> is pressed while the <see cref="CodeTextBox"/> is focused.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="KeyEventArgs"/>.</param>
        private void CodeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Task.Factory.StartNew(() => Gui.RichTextBoxUserInteraction(CodeTextBox, CodeListBox, e), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }

        /// <summary>
        /// Triggers if a <see cref="Key"/> is pressed while the <see cref="CodeListBox"/> is focused.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="KeyEventArgs"/>.</param>
        private void CodeListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Task.Factory.StartNew(() => Gui.PseudoSenseUserInteraction(CodeTextBox, CodeListBox, RecognitionEngine.PreviousWord, e), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }

        /// <summary>
        /// Triggers if the <see cref="CodeListBox"/> got focused.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="RoutedEventArgs"/>.</param>
        private void CodeListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Gui.PositionListBoxAsync(CodeTextBox, CodeListBox), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }

        /// <summary>
        /// Triggers if a <see cref="MouseButton"/> is clicked while the <see cref="CodeListBox"/> is focused.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="object"/> which triggered the event.
        /// </param>
        /// <param name="e">
        /// The specific <see cref="MouseButtonEventArgs"/>.
        /// </param>
        private void CodeListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Task.Factory.StartNew(() => Gui.PseudoSenseUserInteraction(CodeTextBox, CodeListBox, RecognitionEngine.PreviousWord), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
            }
        }

        /// <summary>
        /// Triggers if a <see cref="MouseButton"/> is clicked  while the <see cref="ErrorListView"/> is focused.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="MouseButtonEventArgs"/>.</param>
        private void ErrorListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Task.Factory.StartNew(() => Gui.MistakeSelectorUserInteraction(this.CodeTextBox, this.ErrorListView), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Triggers if a <see cref="Key"/> is pressed while the <see cref="ErrorListView"/> is focused.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="KeyEventArgs"/>.</param>
        private void ErrorListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Task.Factory.StartNew(() => Gui.MistakeSelectorUserInteraction(this.CodeTextBox, this.ErrorListView), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Triggers if the <see cref="ComboBox.SelectedItemProperty"/> of the <see cref="ProgrammingLanguageComboBox"/> changed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="SelectionChangedEventArgs"/>.</param>
        private void ProgrammingLanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainClass.ChangeProgrammingLanguage((string)ProgrammingLanguageComboBox.SelectedItem, this.scheduler);
        }

        /// <summary>
        /// Triggers if the <see cref="ComboBox.SelectedItemProperty"/> of the <see cref="LocalizationLanguageComboBox"/> changed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="SelectionChangedEventArgs"/>.</param>
        private void LocalizationLanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainClass.ChangeLocalizationLanguage(this.LocalizationLanguageComboBox.SelectedItem.ToString(), this.scheduler);
        }

        /// <summary>
        /// Triggers if the user clicked <see cref="IoLoadButton"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="RoutedEventArgs"/>.</param>
        private void IoLoadButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Io.LoadFileInRichTextBoxAsync(CodeTextBox), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }

        /// <summary>
        /// Triggers if the user clicked on <see cref="IoSaveButton"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> which triggered the event.</param>
        /// <param name="e">The specific <see cref="RoutedEventArgs"/>.</param>
        private void IoSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Io.SaveRichTextBoxAsync(CodeTextBox), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }

        /// <summary>
        /// Triggers if the <see cref="IoPrintButton"/> was clicked.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="object"/> which triggered the event.
        /// </param>
        /// <param name="e">
        /// The specific <see cref="RoutedEventArgs"/>.
        /// </param>
        private void IoPrintButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Io.PrintCodeAsync(CodeTextBox), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }
    }
}
